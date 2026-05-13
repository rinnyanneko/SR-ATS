// SR-ATS
// https://github.com/rinnyanneko/SR-ATS
// Copyright © 2025 rinnyanneko. Some Rights Reserved.
// Licensed under the Apache License, Version 2.0
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0

using Godot;

public partial class DebugWord : RichTextLabel {
    private ConfigFile _cfg = new ConfigFile();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        _cfg.Load("user://config.cfg");
        if (_cfg.GetValue("System", "Debug", false).AsBool())
            Visible = true;
        else
            Visible = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        _cfg.Load("user://config.cfg");
        if (_cfg.GetValue("System", "Debug", false).AsBool()) {
            var parent = GetParent();
            if (parent != null) {
                AtsPSimRailConnectTelemetry telemetry = GetNode<AtsPSimRailConnectTelemetry>("../TelemetryWebSocket");
                ControlBrake brakeOutput = GetNode<ControlBrake>("../ControlBrake");
                Text = "Data source = SimRailConnect WebSocket\n" +
                       $"Telemetry WS URL = {telemetry.DebugWsUrl}\n" +
                       $"Telemetry WS state = {telemetry.DebugWsState}\n" +
                       $"Telemetry subscribed = {telemetry.DebugSubscribed}\n" +
                       $"Telemetry reconnect in = {telemetry.DebugReconnectInSeconds:0.00}s\n" +
                       $"Brake WS URL = {brakeOutput.DebugWsUrl}\n" +
                       $"Brake WS state = {brakeOutput.DebugWsState}\n" +
                       $"Brake queued commands = {brakeOutput.DebugQueuedCommandCount}\n" +
                       $"Brake reconnect in = {brakeOutput.DebugReconnectInSeconds:0.00}s\n" +
                       $"UpdateTime = {parent.Get("UpdateTime")}\n" +
                       $"Velocity = {parent.Get("Velocity")} km/h\n" +
                       $"PatternSpeed = {parent.Get("PatternSpeed")}\n" +
                       $"Brake = {parent.Get("Brake")}\n" +
                       $"BrakeOpen = {parent.Get("BrakeOpen")}\n" +
                       $"Fail = {parent.Get("Fail")}\n" +
                       "Signal telemetry = unavailable in current SimRailConnect WS API";
            }
        }
    }
}
