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

public partial class UpdateMirrorOptionButton : OptionButton {
	private readonly ConfigFile cfg = new ConfigFile();

	public override void _Ready() {
		cfg.Load("user://config.cfg");
		string mirror = cfg.GetValue("General", "UpdateMirror", "GitHub").AsString();
		if (mirror == "GitHub") {
			Selected = 0;
		} else if (mirror == "GitCode") {
			Selected = 1;

		GetNode<BaseButton>("../vJoy").ButtonPressed = cfg.GetValue("General", "vJoy", false).AsBool();
		GetNode<RichTextLabel>("RichTextLabel").Text = Tr("UPDATE_MIRROR");
		GetNode<BaseButton>("../vJoy").Disabled = !cfg.GetValue("System", "DevSetting", false).AsBool();
	}

	public void OnSavePressed() {
		if (GetSelectedId() == 0) {
			cfg.SetValue("General", "UpdateMirror", "GitHub");
		} else if (GetSelectedId() == 1) {
			cfg.SetValue("General", "UpdateMirror", "GitCode");

		if (GetNode<OptionButton>("../UpdateChannel").GetSelectedId() == 0) {
			cfg.SetValue("General", "UpdateChannel", "Stable");
		} else {
			cfg.SetValue("General", "UpdateChannel", "Beta");
		}

		cfg.SetValue("General", "vJoy", GetNode<BaseButton>("../vJoy").ButtonPressed);
		cfg.Save("user://config.cfg");
	}
}
