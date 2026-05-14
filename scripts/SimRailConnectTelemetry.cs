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
using System;
using System.Text;

public partial class SimRailConnectTelemetry : Node {
    private const int FirstConnectionWarningAttemptCount = 4;
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
                    ["id"] = "sr-ats-subscribe",
                    ["channels"] = new Godot.Collections.Array { "train", "environment", "signals", "safety", "status" },
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
        Node parent = GetParent();

        ApplyTrain(GetDictionary(snapshot, "train"), parent);
        ApplySignals(GetDictionary(snapshot, "signals"), parent);
        ApplyEnvironment(GetDictionary(snapshot, "environment"), parent);
        parent.Set("VDDelayedTimetableIndex", -1.0);
    }

    private void ApplyChannel(string channel, Godot.Collections.Dictionary? data) {
        if (data == null) {
            return;
        }

        Node parent = GetParent();
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
                }
                break;
        }
    }

    private static void ApplyTrain(Godot.Collections.Dictionary? train, Node parent) {
        if (train == null) {
            return;
        }

        parent.Set("ControlledBySteamID", "SimRailConnect");
        parent.Set("InBorderStationArea", false);
        parent.Set("Latititute", -1.0);
        parent.Set("Longitute", -1.0);
        parent.Set("Velocity", GetInt(train, "velocityInt", (int)Mathf.Round(GetDouble(train, "velocity"))));

        if (HasProperty(parent, "DriverBrakeApplied")) {
            parent.Set("DriverBrakeApplied", ReadDriverBrakeApplied(train));
        }
    }

    private static void ApplyEnvironment(Godot.Collections.Dictionary? environment, Node parent) {
        if (environment == null) {
            return;
        }

        parent.Set("UpdateTime", ReadUpdateTime(environment));
    }

    private static void ApplySignals(Godot.Collections.Dictionary? signals, Node parent) {
        if (signals == null || !GetBool(signals, "hasSignal")) {
            parent.Set("SignalInFront", "");
            parent.Set("DistanceToSignalInFront", -1.0);
            parent.Set("SignalInFrontSpeed", -1);
            return;
        }

        string signalName = GetString(signals, "name", GetString(signals, "objectIdentifier", "SimRailConnectSignal"));
        parent.Set("SignalInFront", signalName);
        parent.Set("DistanceToSignalInFront", GetDouble(signals, "distanceMeters", -1.0));
        parent.Set("SignalInFrontSpeed", ReadSignalSpeed(signals));
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

    private static double GetDouble(Godot.Collections.Dictionary? data, string key, double fallback = 0) {
        return data != null && data.ContainsKey(key) ? data[key].AsDouble() : fallback;
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

    private static string GetString(Godot.Collections.Dictionary? data, string[] keys, string fallback = "") {
        if (data == null) {
            return fallback;
        }

        foreach (string key in keys) {
            if (data.ContainsKey(key) && data[key].VariantType != Variant.Type.Nil) {
                return data[key].AsString();
            }
        }

        return fallback;
    }

    private static bool ReadDriverBrakeApplied(Godot.Collections.Dictionary train) {
        double brakingRatio = GetDouble(train, new[] { "brakingRatio", "brakeRatio", "brakePercent", "brakePercentage" }, -1);
        if (brakingRatio > 0) {
            return true;
        }

        double brakePosition = GetDouble(train, new[] { "brakePosition", "brakeNotch", "brakeControllerPosition", "brakeControllerNotch" }, -1);
        if (brakePosition > 0) {
            return true;
        }

        string mainControllerPosition = GetString(
            train,
            new[] { "mainControllerPosition", "mainControllerNotch", "mainController", "masterControllerPosition", "masterControllerNotch", "masterController" });
        if (IsBrakeControllerPosition(mainControllerPosition)) {
            return true;
        }

        return GetBool(train, "brakeApplied")
            || GetBool(train, "isBraking")
            || GetBool(train, "driverBrakeApplied");
    }

    private static bool IsBrakeControllerPosition(string position) {
        if (string.IsNullOrWhiteSpace(position)) {
            return false;
        }

        string normalizedPosition = position.Trim().ToLowerInvariant();
        return normalizedPosition.StartsWith("b", StringComparison.Ordinal)
            || normalizedPosition.Contains("brake")
            || normalizedPosition.Contains("emergency");
    }

    private static bool HasProperty(GodotObject obj, string propertyName) {
        foreach (Godot.Collections.Dictionary property in obj.GetPropertyList()) {
            if (GetString(property, "name") == propertyName) {
                return true;
            }
        }

        return false;
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

    public string DebugWsUrl => currentUrl;
    public string DebugWsState => socket.GetReadyState().ToString();
    public bool DebugSubscribed => subscribed;
    public double DebugReconnectInSeconds => reconnectInSeconds;
}
