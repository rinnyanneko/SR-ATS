/*
 * Copyright 2026 rinnyanneko
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
// SPDX-License-Identifier: Apache-2.0

#nullable enable

using Godot;
using System.Text;

public partial class SimRailConnectTelemetry : Node {
    private readonly WebSocketPeer socket = new WebSocketPeer();
    private readonly ConfigFile cfg = new ConfigFile();
    private double reconnectInSeconds;
    private bool subscribed;
    private string currentUrl = UpdateMirrorOptionButton.DefaultSimRailConnectUrl;

    public override void _Ready() {
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
                    ["id"] = "sr-ats-subscribe",
                    ["channels"] = new Godot.Collections.Array { "telemetry" },
                    ["rateHz"] = 10
                });
                Send(new Godot.Collections.Dictionary {
                    ["type"] = "getSnapshot",
                    ["id"] = "sr-ats-snapshot"
                });
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
        }
    }

    private void ApplySnapshot(Godot.Collections.Dictionary? snapshot) {
        if (snapshot == null) {
            return;
        }

        if (!GetBool(snapshot, "isActive")) {
            string status = GetString(snapshot, "status", Tr("SIMRAILCONNECT_WAITING_TELEMETRY"));
            ShowError(status);
            return;
        }

        GetNode<CanvasItem>("../ErrorMsg").Visible = false;
        Godot.Collections.Dictionary? train = GetDictionary(snapshot, "train");
        Godot.Collections.Dictionary? environment = GetDictionary(snapshot, "environment");
        Node parent = GetParent();

        parent.Set("ControlledBySteamID", "SimRailConnect");
        parent.Set("InBorderStationArea", false);
        parent.Set("Latititute", -1.0);
        parent.Set("Longitute", -1.0);
        parent.Set("Velocity", GetInt(train, "velocityInt", (int)Mathf.Round(GetDouble(train, "velocity"))));
        parent.Set("SignalInFront", "");
        parent.Set("DistanceToSignalInFront", -1.0);
        parent.Set("SignalInFrontSpeed", -1);
        parent.Set("VDDelayedTimetableIndex", -1.0);
        parent.Set("UpdateTime", ReadUpdateTime(environment));
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
