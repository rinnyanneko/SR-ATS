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

public partial class AtsAlarmOn : Sprite2D {
    private AtsSScene scene = null!;
    private AudioStreamPlayer alarm = null!;
    private AudioStreamPlayer chime = null!;
    private Timer alarmTimer = null!;

    public override void _Ready() {
        scene = GetParent<AtsSScene>();
        alarm = GetNode<AudioStreamPlayer>("ATS Alarm");
        chime = GetNode<AudioStreamPlayer>("../AtsNormalOff/ATS Chime");
        alarmTimer = GetNode<Timer>("../Timer");
        scene.Alarm = true;
        scene.Chime = true;
        Visible = true;
        alarm.Play();
        chime.Play();
        GD.Print("Test Alarm!");
    }

    public override void _Process(double delta) {
        double velocity = scene.Velocity;
        double distanceToSignalInFront = scene.DistanceToSignalInFront;
        double signalInFrontSpeed = scene.SignalInFrontSpeed;
        string signalInFront = scene.SignalInFront;
        string passedSignal = scene.LastSignalInFront;
        bool hasNewRestrictiveSignal = !string.IsNullOrEmpty(signalInFront)
            && passedSignal != signalInFront
            && velocity > 0
            && distanceToSignalInFront < 600
            && distanceToSignalInFront >= 0
            && signalInFrontSpeed <= 100
            && signalInFrontSpeed >= 0;

        if (hasNewRestrictiveSignal) {
            TriggerAlarm(signalInFront);
            GD.Print("Alarm Triggered!");
        }

        if (Input.IsActionJustPressed("ATS confirm")) {
            OnAtsConfirmPressed();
        }

        if (velocity > 0 && distanceToSignalInFront >= 0 && distanceToSignalInFront < 10 && signalInFrontSpeed == 0) {
            scene.AlarmHard = true;
            TriggerAlarm(signalInFront);
            GD.Print("Hard alarm!");
        }

        Visible = scene.Alarm || scene.AlarmHard;
    }

    public void OnAtsConfirmPressed() {
        if (!scene.AlarmHard) {
            scene.Alarm = false;
            alarm.Stop();
            alarmTimer.Stop();
            GD.Print("Alarm confirmed!");
        }
    }

    private void TriggerAlarm(string signalInFront) {
        scene.Alarm = true;
        scene.Chime = true;
        scene.LastSignalInFront = signalInFront;

        if (!alarm.Playing) {
            alarm.Play();
        }

        if (!chime.Playing) {
            chime.Play();
        }

        if (alarmTimer.IsStopped() && !scene.AlarmHard) {
            alarmTimer.Start();
        }
    }
}
