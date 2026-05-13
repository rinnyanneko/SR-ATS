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

public partial class LanguageButton : OptionButton {
	private static readonly (string Code, string Label, string Message)[] Languages = [
		("en", "English", "English(en)!"),
		("jp", "Japanese", "Japanese(jp)!"),
		("cmn", "Traditional Chinese", "Tradition Chinese(cmn)!"),
		("zh", "Simplified Chinese", "Simplified Chinese(zh)!"),
		("ko", "Korean", "Korean(ko)!"),
		("pl", "Polish", "Polish(pl)!")
	];

	private readonly ConfigFile cfg = new ConfigFile();

	public override void _Ready() {
		cfg.Load("user://config.cfg");
		AddLanguageItems();
		Select(FindLanguageIndex(cfg.GetValue("System", "lang", "en").AsString()));
		SetLanguage(Languages[Selected].Code, null);
		cfg.Save("user://config.cfg");
	}

	public void OnItemSelected(long index) {
		if (index < 0 || index >= Languages.Length) {
			return;
		}

		int selectedIndex = (int)index;
		(string code, _, string message) = Languages[selectedIndex];
		SetLanguage(code, message);
		cfg.Save("user://config.cfg");
		GetNodeOrNull<Node>("../Language name")?.CallDeferred("OnLanguageChanged");
		GetNodeOrNull<Node>("../Please enter train data")?.CallDeferred("OnLanguageChanged");
	}

	private void AddLanguageItems() {
		Clear();
		for (int i = 0; i < Languages.Length; i++) {
			AddItem(Languages[i].Label, i);
		}
	}

	private int FindLanguageIndex(string language) {
		for (int i = 0; i < Languages.Length; i++) {
			if (Languages[i].Code == language) {
				return i;
			}
		}

		return 0;
	}

	private void SetLanguage(string language, string? message) {
		cfg.SetValue("System", "lang", language);
		TranslationServer.SetLocale(language);
		if (message != null) {
			GD.Print(message);
		}
	}
}
