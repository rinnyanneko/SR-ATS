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

public partial class ServerOptionButton : OptionButton {
    private readonly ConfigFile cfg = new ConfigFile();

    public override void _Ready() {
        cfg.Load("user://config.cfg");
        GetNode<RichTextLabel>("RichTextLabel").Text = Tr("SERVER");
        GetNode<RichTextLabel>("../TrainNumber/RichTextLabel").Text = Tr("TRAIN_NUMBER");
        GetNode<Button>("../Save").Text = Tr("SAVE");
        Variant savedServer = cfg.GetValue("Train Data", "server");
        if (savedServer.VariantType != Variant.Type.Nil) {
            Text = savedServer.AsString();
        }

        SetLineEditTextIfPresent("../TrainNumber", cfg.GetValue("Train Data", "trainNumber"));
        SetLineEditTextIfPresent("../Braking Ratio", cfg.GetValue("Train Data", "brakingRatio"));
        SetLineEditTextIfPresent("../Vmax", cfg.GetValue("Train Data", "Vmax"));
    }

    public void OnSaveButtonPressed() {
        if (Selected >= 0) {
            RequestServers requestServers = GetNode<RequestServers>("../../RequestServers");
            if (Selected < requestServers.Codes.Count) {
                cfg.SetValue("Train Data", "server", requestServers.Codes[Selected]);
            }

            cfg.SetValue("Train Data", "trainNumber", GetNode<LineEdit>("../TrainNumber").Text);
        }

        string brakingRatio = GetNode<LineEdit>("../Braking Ratio").Text;
        if (!string.IsNullOrWhiteSpace(brakingRatio)
            && float.TryParse(brakingRatio, out float ratio)
            && int.TryParse(GetNode<LineEdit>("../Vmax").Text, out int vmax)) {
            cfg.SetValue("Train Data", "brakingRatio", brakingRatio);
            cfg.SetValue("Train Data", "brakeDistance", ratio == 0 ? 0 : 480 / ratio);
            cfg.SetValue("Train Data", "decelRate", 0.805 * Mathf.Pow(ratio, 2));
            cfg.SetValue("Train Data", "Vmax", vmax);
        }

        cfg.Save("user://config.cfg");
    }

    private void SetLineEditTextIfPresent(string path, Variant value) {
        if (value.VariantType != Variant.Type.Nil) {
            GetNode<LineEdit>(path).Text = value.AsString();
        }
    }
}
