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

public partial class UpdateChannelOptionButton : OptionButton {
	private readonly ConfigFile cfg = new ConfigFile();
	private LocalizationManager? localizationManager;

	public override void _Ready() {
		cfg.Load("user://config.cfg");
		localizationManager = GetNodeOrNull<LocalizationManager>("/root/LocalizationManager");
		if (localizationManager != null) {
			localizationManager.LocaleChanged += OnLocaleChanged;
		}

		RefreshTranslations();
		Selected = UpdateChannelExtensions.ParseConfigValue(
			cfg.GetValue("General", "UpdateChannel", "Stable").AsString()) == UpdateChannel.Preview
				? 1
				: 0;
	}

	public override void _ExitTree() {
		if (localizationManager != null) {
			localizationManager.LocaleChanged -= OnLocaleChanged;
		}
	}

	public void RefreshTranslations() {
		GetNode<RichTextLabel>("RichTextLabel").Text = Tr("UPDATE_CHANNEL");
		SetItemText(0, Tr("UPDATE_CHANNEL_STABLE"));
		SetItemText(1, Tr("UPDATE_CHANNEL_PREVIEW"));
	}

	private void OnLocaleChanged(string locale) {
		RefreshTranslations();
	}
}
