// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. Some Rights Reserved.
// Licensed under the Apache License, Version 2.0
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0

#nullable enable

using Godot;
using System.Text;

public partial class AtsPSimRailConnectTelemetry : Node {
    private readonly WebSocketPeer socket = new WebSocketPeer();
    private readonly ConfigFile cfg = new ConfigFile();
    private double reconnectInSeconds;
    private bool subscribed;
    private string currentUrl = UpdateMirrorOptionButton.DefaultSimRailConnectUrl;

    public override void _Ready() {
        ConnectToSimRailConnect();
    }

    public void _on_main_ats_ready() {
        ConnectToSimRailConnect();
    }

    public override void _Process(double delta) {
        socket.Poll();
        WebSocketPeer.State state = socket.GetReadyState();

        if (state == WebSocketPeer.State.Open) {
            if (!subscribed) {
                subscribed = true;
                Send(new Godot.Collections.Dictionary {
                    ["type"] = "subscribe",
                    ["id"] = "sr-ats-p-subscribe",
                    ["channels"] = new Godot.Collections.Array { "telemetry" },
                    ["rateHz"] = 10
                });
                Send(new Godot.Collections.Dictionary {
                    ["type"] = "getSnapshot",
                    ["id"] = "sr-ats-p-snapshot"
                });
                SetFail(false);
            }

            while (socket.GetAvailablePacketCount() > 0) {
                HandlePacket(Encoding.UTF8.GetString(socket.GetPacket()));
            }
        } else if (state == WebSocketPeer.State.Closed) {
            subscribed = false;
            reconnectInSeconds -= delta;
            if (reconnectInSeconds <= 0) {
                ConnectToSimRailConnect();
            }
        }
    }

    private void ConnectToSimRailConnect() {
        if (socket.GetReadyState() == WebSocketPeer.State.Connecting
            || socket.GetReadyState() == WebSocketPeer.State.Open) {
            return;
        }

        cfg.Load("user://config.cfg");
        currentUrl = cfg.GetValue("SimRailConnect", "url", UpdateMirrorOptionButton.DefaultSimRailConnectUrl).AsString();
        Error error = socket.ConnectToUrl(currentUrl);
        reconnectInSeconds = 2.5;
        if (error != Error.Ok) {
            GD.PrintErr("SimRailConnect WebSocket connect failed: " + error);
            ShowError(Tr("SIMRAILCONNECT_CONNECTION_FAILED").Replace("{error}", error.ToString()));
            SetFail(true);
        }
    }

    private void HandlePacket(string text) {
        Variant parsed = Json.ParseString(text);
        if (parsed.VariantType != Variant.Type.Dictionary) {
            return;
        }

        Godot.Collections.Dictionary message = parsed.AsGodotDictionary();
        string type = GetString(message, "type");
        if (type == "state") {
            ApplySnapshot(GetDictionary(message, "data"));
        } else if (type == "snapshot" && GetBool(message, "ok")) {
            ApplySnapshot(GetDictionary(message, "data"));
        } else if (type == "error") {
            ShowError(Tr("SIMRAILCONNECT_ERROR").Replace("{message}", GetString(message, "message", "")));
            SetFail(true);
        }
    }

    private void ApplySnapshot(Godot.Collections.Dictionary? snapshot) {
        Scene parent = GetNode<Scene>("..");
        if (snapshot == null) {
            return;
        }

        if (!GetBool(snapshot, "isActive")) {
            string status = GetString(snapshot, "status", Tr("SIMRAILCONNECT_WAITING_TELEMETRY"));
            ShowError(status);
            SetFail(true);
            return;
        }

        GetNode<AcceptDialog>("../ErrorMsg").Visible = false;
        SetFail(false);
        Godot.Collections.Dictionary? train = GetDictionary(snapshot, "train");
        Godot.Collections.Dictionary? environment = GetDictionary(snapshot, "environment");

        parent.ControlledBySteamID = "SimRailConnect";
        parent.InBorderStationArea = false;
        parent.Latititute = -1.0;
        parent.Longitute = -1.0;
        parent.Velocity = GetInt(train, "velocityInt", (int)Mathf.Round(GetDouble(train, "velocity")));
        parent.SignalInFront = "";
        parent.DistanceToSignalInFront = -1.0;
        parent.SignalInFrontSpeed = -1;
        parent.VDDelayedTimetableIndex = -1.0;
        parent.UpdateTime = ReadUpdateTime(environment);
    }

    private void SetFail(bool fail) {
        Scene parent = GetNode<Scene>("..");
        ATSIndicators indicators = GetNode<ATSIndicators>("../Indicators");
        if (parent.Fail == fail) {
            return;
        }

        parent.Fail = fail;
        indicators.Fail(fail);
        indicators.PlayBell();
    }

    private void Send(Godot.Collections.Dictionary message) {
        socket.SendText(Json.Stringify(message));
    }

    private void ShowError(string message) {
        AcceptDialog? dialog = GetNodeOrNull<AcceptDialog>("../ErrorMsg");
        if (dialog == null) {
            return;
        }

        dialog.DialogText = message;
        dialog.Visible = true;
    }

    private static string ReadUpdateTime(Godot.Collections.Dictionary? environment) {
        if (environment != null && environment.Count > 0) {
            return $"{GetInt(environment, "hours"):00}:{GetInt(environment, "minutes"):00}:{GetInt(environment, "seconds"):00}";
        }

        Godot.Collections.Dictionary time = Time.GetTimeDictFromSystem();
        return $"{time["hour"].AsInt32():00}:{time["minute"].AsInt32():00}:{time["second"].AsInt32():00}";
    }

    private static Godot.Collections.Dictionary? GetDictionary(Godot.Collections.Dictionary data, string key) {
        return data.ContainsKey(key) && data[key].VariantType == Variant.Type.Dictionary
            ? data[key].AsGodotDictionary()
            : null;
    }

    private static string GetString(Godot.Collections.Dictionary data, string key, string fallback = "") {
        return data.ContainsKey(key) && data[key].VariantType != Variant.Type.Nil
            ? data[key].AsString()
            : fallback;
    }

    private static bool GetBool(Godot.Collections.Dictionary data, string key) {
        return data.ContainsKey(key) && data[key].AsBool();
    }

    private static int GetInt(Godot.Collections.Dictionary? data, string key, int fallback = 0) {
        return data != null && data.ContainsKey(key) ? data[key].AsInt32() : fallback;
    }

    private static double GetDouble(Godot.Collections.Dictionary? data, string key, double fallback = 0) {
        return data != null && data.ContainsKey(key) ? data[key].AsDouble() : fallback;
    }

    public string DebugWsUrl => currentUrl;
    public string DebugWsState => socket.GetReadyState().ToString();
    public bool DebugSubscribed => subscribed;
    public double DebugReconnectInSeconds => reconnectInSeconds;
}
