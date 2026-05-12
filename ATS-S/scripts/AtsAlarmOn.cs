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

    public override void _Ready() {
        scene = GetParent<AtsSScene>();
        scene.Alarm = true;
        GetNode<AudioStreamPlayer>("ATS Alarm").Play();
        GD.Print("Test Alarm!");
    }

    public override void _Process(double delta) {
        double velocity = scene.Velocity;
        double distanceToSignalInFront = scene.DistanceToSignalInFront;
        double signalInFrontSpeed = scene.SignalInFrontSpeed;
        string signalInFront = scene.SignalInFront;
        string passedSignal = scene.LastSignalInFront;
        if (velocity > 0
            && distanceToSignalInFront < 600
            && signalInFrontSpeed <= 100
            && passedSignal != signalInFront) {
            Visible = true;
            TopLevel = true;
            GetNode<AudioStreamPlayer>("ATS Alarm").Play();
            GetNode<AudioStreamPlayer>("../AtsNormalOff/ATS Chime").Play();
            scene.LastSignalInFront = signalInFront;
            GD.Print("Alarm Triggered!");
            GetNode<Timer>("../Timer").Start();
        }

        Visible = scene.Alarm;
        if (Input.IsActionJustPressed("ATS confirm")) {
            OnAtsConfirmPressed();
        }

        if (velocity > 0 && distanceToSignalInFront < 10 && signalInFrontSpeed == 0) {
            scene.AlarmHard = true;
            GD.Print("Hard alarm!");
            Visible = true;
            TopLevel = true;
            GetNode<AudioStreamPlayer>("ATS Alarm").Play();
            GetNode<AudioStreamPlayer>("../AtsNormalOff/ATS Chime").Play();
            scene.LastSignalInFront = signalInFront;
        }
    }

    public void OnAtsConfirmPressed() {
        if (!scene.AlarmHard) {
            GetNode<AudioStreamPlayer>("ATS Alarm").Stop();
            scene.Alarm = false;
            GetNode<Timer>("../Timer").Stop();
            GD.Print("Alarm confirmed!");
        }
    }
}
