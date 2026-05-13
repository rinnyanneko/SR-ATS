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

using Godot;

public partial class AtsPsDebugWord : RichTextLabel {
    private readonly ConfigFile cfg = new ConfigFile();

    public override void _Ready() {
        cfg.Load("user://config.cfg");
        Visible = cfg.GetValue("System", "Debug", false).AsBool();
    }

    public override void _Process(double delta) {
        cfg.Load("user://config.cfg");
        if (!cfg.GetValue("System", "Debug", false).AsBool()) {
            Visible = false;
            Text = string.Empty;
            return;
        }

        Visible = true;
        AtsPsScene scene = GetParent<AtsPsScene>();
        SimRailConnectTelemetry telemetry = GetNode<SimRailConnectTelemetry>("../TelemetryWebSocket");
        ControlBrake brakeOutput = GetNode<ControlBrake>("../Brake Control");
        Text = "Data source = SimRailConnect WebSocket"
            + "\nTelemetry WS URL = " + telemetry.DebugWsUrl
            + "\nTelemetry WS state = " + telemetry.DebugWsState
            + "\nTelemetry subscribed = " + telemetry.DebugSubscribed
            + "\nTelemetry reconnect in = " + telemetry.DebugReconnectInSeconds.ToString("0.00") + "s"
            + "\nBrake WS URL = " + brakeOutput.DebugWsUrl
            + "\nBrake WS state = " + brakeOutput.DebugWsState
            + "\nBrake queued commands = " + brakeOutput.DebugQueuedCommandCount
            + "\nBrake reconnect in = " + brakeOutput.DebugReconnectInSeconds.ToString("0.00") + "s"
            + "\nUpdateTime = " + scene.UpdateTime
            + "\nVelocity = " + scene.Velocity + " km/h"
            + "\npattern_generated = " + scene.PatternGenerated
            + "\npattern_speed = " + scene.PatternSpeed
            + "\nem_brake = " + scene.EmBrake
            + "\nbrake_disable = " + scene.BrakeDisable
            + "\nSignal telemetry = unavailable in current SimRailConnect WS API";
    }
}
