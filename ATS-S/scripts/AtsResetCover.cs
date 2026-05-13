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

public partial class AtsResetCover : TextureButton {
    public override void _Ready() {
        OnAtsResetCoverOpenPressed();
    }

    public void OnPressed() {
        GD.Print("Open reset cover");
        Visible = false;
        GetNode<CanvasItem>("../ATS reset cover open").Visible = true;
        GetNode<BaseButton>("../ATS Reset").Disabled = false;
        GetNode<CanvasItem>("../ATS Reset").Visible = true;
    }

    public void OnAtsResetCoverOpenPressed() {
        GetNode<CanvasItem>("../ATS reset cover open").Visible = false;
        Visible = true;
        GetNode<BaseButton>("../ATS Reset").Disabled = true;
        GetNode<CanvasItem>("../ATS Reset").Visible = false;
        GD.Print("Close reset cover");
    }
}
