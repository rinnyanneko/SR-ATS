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

public partial class AtsNormalOff : Sprite2D {
    private AtsSScene scene = null!;

    public override void _Ready() {
        scene = GetParent<AtsSScene>();
        scene.Chime = true;
        GetNode<AudioStreamPlayer>("ATS Chime").Play();
    }

    public override void _Process(double delta) {
        if (scene.Alarm) {
            GetNode<CanvasItem>("../AtsNormalOn").Visible = false;
            scene.Chime = true;
        } else {
            GetNode<CanvasItem>("../AtsNormalOn").Visible = true;
        }
    }

    public void OnChimeOffPressed() {
        if (!scene.Alarm && !scene.AlarmHard) {
            scene.Chime = false;
            GetNode<AudioStreamPlayer>("ATS Chime").Stop();
            GD.Print("Chime off!");
        }
    }
}
