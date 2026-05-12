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

public partial class AtsSScene : Node2D {
    [Export] public bool Alarm { get; set; } = false;
    [Export] public bool Chime { get; set; } = false;
    [Export] public bool AlarmHard { get; set; } = false;
    [Export] public string LastSignalInFront { get; set; } = "";
    [Export] public double VDDelayedTimetableIndex { get; set; } = -1.0;
    [Export] public int SignalInFrontSpeed { get; set; } = -1;
    [Export] public double DistanceToSignalInFront { get; set; } = -1.0;
    [Export] public string SignalInFront { get; set; } = "";
    [Export] public int Velocity { get; set; } = -1;
    [Export] public double Longitute { get; set; } = -1.0;
    [Export] public double Latititute { get; set; } = -1.0;
    [Export] public bool InBorderStationArea { get; set; } = false;
    [Export] public string ControlledBySteamID { get; set; } = "";
    [Export] public string UpdateTime { get; set; } = "";

    public void OnTimerTimeout() {
        if (Alarm) {
            AlarmHard = true;
            GD.Print("Hard alarm!");
        }
    }

    public override void _Notification(int what) {
        if (what == NotificationApplicationFocusIn) {
            Engine.MaxFps = 0;
        }

        if (what == NotificationApplicationFocusOut) {
            Engine.MaxFps = 20;
        }
    }
}
