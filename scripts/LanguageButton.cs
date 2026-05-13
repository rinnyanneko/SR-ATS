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
using System.Collections.Generic;

public partial class LanguageButton : OptionButton {
	private readonly ConfigFile cfg = new ConfigFile();
	private readonly List<string> languages = [];

	public override void _Ready() {
		cfg.Load("user://config.cfg");
		WindowSettings.ApplyFromConfig();
		AddLanguageItems();
		Select(FindLanguageIndex(cfg.GetValue("System", "lang", "en").AsString()));
		ApplyLanguage(languages[Selected]);
		cfg.Save("user://config.cfg");
	}

	public void OnItemSelected(long index) {
		if (index < 0 || index >= languages.Count) {
			return;
		}

		int selectedIndex = (int)index;
		ApplyLanguage(languages[selectedIndex]);
		cfg.Save("user://config.cfg");
		GetNodeOrNull<Node>("../Language name")?.CallDeferred("OnLanguageChanged");
	}

	private void AddLanguageItems() {
		Clear();
		languages.Clear();
		foreach (string locale in TranslationServer.GetLoadedLocales()) {
			languages.Add(locale);
		}

		for (int i = 0; i < languages.Count; i++) {
			AddItem(GetLanguageName(languages[i]), i);
		}
	}

	private int FindLanguageIndex(string language) {
		for (int i = 0; i < languages.Count; i++) {
			if (languages[i] == language) {
				return i;
			}
		}

		return 0;
	}

	private void ApplyLanguage(string language) {
		cfg.SetValue("System", "lang", language);
		TranslationServer.SetLocale(language);
	}

	private string GetLanguageName(string language) {
		string previousLocale = TranslationServer.GetLocale();
		TranslationServer.SetLocale(language);
		string languageName = Tr("LANG_NAME");
		TranslationServer.SetLocale(previousLocale);
		return languageName == "LANG_NAME" ? language : languageName;
	}
}
