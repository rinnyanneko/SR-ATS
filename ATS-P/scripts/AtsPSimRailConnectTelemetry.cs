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
    private const int FirstConnectionWarningAttemptCount = 3;
    private const double ConnectionAttemptTimeoutSeconds = 2.5;

    private readonly WebSocketPeer socket = new WebSocketPeer();
    private readonly ConfigFile cfg = new ConfigFile();
    private double reconnectInSeconds;
    private bool subscribed;
    private bool hasConnected;
    private bool connectionLostShown;
    private bool firstConnectionWarningShown;
    private int connectionAttemptCount;
    private string currentUrl = UpdateMirrorOptionButton.DefaultSimRailConnectUrl;

    public override void _Ready() {
        ConnectToSimRailConnect();
    }

    public override void _Process(double delta) {
        socket.Poll();
        WebSocketPeer.State state = socket.GetReadyState();

        if (state == WebSocketPeer.State.Open) {
            hasConnected = true;
            connectionLostShown = false;
            firstConnectionWarningShown = false;
            connectionAttemptCount = 0;
            if (!subscribed) {
                subscribed = true;
                Send(new Godot.Collections.Dictionary {
                    ["type"] = "subscribe",
                    ["id"] = "sr-ats-p-subscribe",
                    ["channels"] = new Godot.Collections.Array { "train", "environment", "signals", "safety", "status" },
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
        } else if (state == WebSocketPeer.State.Connecting) {
            reconnectInSeconds -= delta;
            if (reconnectInSeconds <= 0) {
                ShowFirstConnectionWarningIfNeeded();
                socket.Close();
                reconnectInSeconds = 0;
            }
        } else if (state == WebSocketPeer.State.Closed) {
            if (hasConnected && subscribed && !connectionLostShown) {
                ShowError(TranslateOrFallback(
                    "SIMRAILCONNECT_CONNECTION_LOST",
                    "SimRailConnect connection lost. Reconnecting..."));
                SetFail(true);
                connectionLostShown = true;
            }

            subscribed = false;
            reconnectInSeconds -= delta;
            if (reconnectInSeconds <= 0) {
                ConnectToSimRailConnect();
                ShowFirstConnectionWarningIfNeeded();
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
        connectionAttemptCount++;
        Error error = socket.ConnectToUrl(currentUrl);
        reconnectInSeconds = ConnectionAttemptTimeoutSeconds;
        if (error != Error.Ok) {
            GD.PrintErr("SimRailConnect WebSocket connect failed: " + error);
            ShowFirstConnectionWarningIfNeeded();
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
            string channel = GetString(message, "channel", "telemetry");
            Godot.Collections.Dictionary? data = GetDictionary(message, "data");
            if (channel == "telemetry" || channel == "full") {
                ApplySnapshot(data);
            } else {
                ApplyChannel(channel, data);
            }
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
        ApplyTrain(GetDictionary(snapshot, "train"), parent);
        ApplySignals(GetDictionary(snapshot, "signals"), parent);
        ApplyEnvironment(GetDictionary(snapshot, "environment"), parent);
        parent.VDDelayedTimetableIndex = -1.0;
    }

    private void ApplyChannel(string channel, Godot.Collections.Dictionary? data) {
        if (data == null) {
            return;
        }

        Scene parent = GetNode<Scene>("..");
        switch (channel) {
            case "train":
                ApplyTrain(data, parent);
                break;
            case "environment":
                ApplyEnvironment(data, parent);
                break;
            case "signals":
                ApplySignals(data, parent);
                break;
            case "safety":
                break;
            case "status":
                if (!GetBool(data, "isActive", true)) {
                    ShowError(GetString(data, "status", Tr("SIMRAILCONNECT_WAITING_TELEMETRY")));
                    SetFail(true);
                }
                break;
        }
    }

    private static void ApplyTrain(Godot.Collections.Dictionary? train, Scene parent) {
        if (train == null) {
            return;
        }

        parent.ControlledBySteamID = "SimRailConnect";
        parent.InBorderStationArea = false;
        parent.Latititute = -1.0;
        parent.Longitute = -1.0;
        parent.Velocity = GetInt(train, "velocityInt", (int)Mathf.Round(GetDouble(train, "velocity")));
        parent.Vmax = GetInt(train, new[] { "vmax", "Vmax", "maxSpeed", "maximumSpeed", "maxVelocity" }, parent.Vmax);
        parent.EffectiveDecel = ReadEffectiveDecel(train, parent.EffectiveDecel);
    }

    private static void ApplyEnvironment(Godot.Collections.Dictionary? environment, Scene parent) {
        if (environment == null) {
            return;
        }

        parent.UpdateTime = ReadUpdateTime(environment);
    }

    private static void ApplySignals(Godot.Collections.Dictionary? signals, Scene parent) {
        if (signals == null || !GetBool(signals, "hasSignal")) {
            parent.SignalInFront = "";
            parent.DistanceToSignalInFront = -1.0;
            parent.SignalInFrontSpeed = -1;
            return;
        }

        parent.SignalInFront = GetString(signals, "name", GetString(signals, "objectIdentifier", "SimRailConnectSignal"));
        parent.DistanceToSignalInFront = GetDouble(signals, "distanceMeters", -1.0);
        parent.SignalInFrontSpeed = ReadSignalSpeed(signals);
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

    private void ShowFirstConnectionWarningIfNeeded() {
        if (hasConnected
            || firstConnectionWarningShown
            || connectionAttemptCount < FirstConnectionWarningAttemptCount) {
            return;
        }

        ShowError(TranslateOrFallback(
                "SIMRAILCONNECT_FIRST_CONNECTION_FAILED",
                "Unable to connect to SimRailConnect after {retries} retries. Check that SimRail and the plugin are running. URL: {url}")
            .Replace("{retries}", FirstConnectionWarningAttemptCount.ToString())
            .Replace("{url}", currentUrl));
        firstConnectionWarningShown = true;
    }

    private string TranslateOrFallback(string key, string fallback) {
        string translated = Tr(key);
        return translated == key ? fallback : translated;
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

    private static bool GetBool(Godot.Collections.Dictionary data, string key, bool fallback = false) {
        return data.ContainsKey(key) && data[key].VariantType != Variant.Type.Nil ? data[key].AsBool() : fallback;
    }

    private static int GetInt(Godot.Collections.Dictionary? data, string key, int fallback = 0) {
        return data != null && data.ContainsKey(key) ? data[key].AsInt32() : fallback;
    }

    private static int GetInt(Godot.Collections.Dictionary? data, string[] keys, int fallback = 0) {
        if (data == null) {
            return fallback;
        }

        foreach (string key in keys) {
            if (data.ContainsKey(key) && data[key].VariantType != Variant.Type.Nil) {
                return data[key].AsInt32();
            }
        }

        return fallback;
    }

    private static double GetDouble(Godot.Collections.Dictionary? data, string key, double fallback = 0) {
        return data != null && data.ContainsKey(key) ? data[key].AsDouble() : fallback;
    }

    private static int GetNullableInt(Godot.Collections.Dictionary? data, string key, int fallback = 0) {
        return data != null && data.ContainsKey(key) && data[key].VariantType != Variant.Type.Nil
            ? data[key].AsInt32()
            : fallback;
    }

    private static int ReadSignalSpeed(Godot.Collections.Dictionary signals) {
        int speed = GetNullableInt(signals, "speedLimitKmh", -1);
        if (speed >= 0) {
            return speed;
        }

        return GetString(signals, "color").ToLowerInvariant() switch {
            "red" => 0,
            _ => -1
        };
    }

    private static double GetDouble(Godot.Collections.Dictionary? data, string[] keys, double fallback = 0) {
        if (data == null) {
            return fallback;
        }

        foreach (string key in keys) {
            if (data.ContainsKey(key) && data[key].VariantType != Variant.Type.Nil) {
                return data[key].AsDouble();
            }
        }

        return fallback;
    }

    private static double ReadEffectiveDecel(Godot.Collections.Dictionary? train, double fallback) {
        double telemetryDecel = GetDouble(train, new[] { "effectiveDecel", "effectiveDeceleration", "decelRate", "deceleration" }, -1);
        if (telemetryDecel > 0) {
            return telemetryDecel;
        }

        double brakingRatio = GetDouble(train, new[] { "brakingRatio", "brakeRatio", "brakePercent", "brakePercentage" }, -1);
        if (brakingRatio > 0) {
            double normalizedRatio = brakingRatio > 10 ? brakingRatio / 100.0 : brakingRatio;
            double decelRate = 0.805 * Mathf.Pow(normalizedRatio, 2);
            return decelRate * normalizedRatio;
        }

        return fallback;
    }

    public string DebugWsUrl => currentUrl;
    public string DebugWsState => socket.GetReadyState().ToString();
    public bool DebugSubscribed => subscribed;
    public double DebugReconnectInSeconds => reconnectInSeconds;
}
