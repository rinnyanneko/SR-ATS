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

public partial class LanguageButton : TextureButton {
    private readonly ConfigFile cfg = new ConfigFile();
    private int selectedLanguage = 0;

    public override void _Ready() {
        cfg.Load("user://config.cfg");
        ApplyInitialLanguage(cfg.GetValue("System", "lang", "null").AsString());
        cfg.Save("user://config.cfg");
    }

    public async void OnPressed() {
        switch (selectedLanguage) {
            case 0:
                SetLanguage("en", "English(en)!");
                break;
            case 1:
                SetLanguage("jp", "Japanese(jp)!");
                break;
            case 2:
                SetLanguage("cmn", "Tradition Chinese(cmn)!");
                break;
            case 3:
                SetLanguage("zh", "Simplified Chinese(zh)!");
                break;
            case 4:
                SetLanguage("ko", "Korean(ko)!");
                break;
            case 5:
                SetLanguage("pl", "Polish(pl)!");
                break;
        }

        cfg.Save("user://config.cfg");
        selectedLanguage++;
        if (selectedLanguage > 5) {
            selectedLanguage = 0;
        }

        Disabled = true;
        await ToSignal(GetTree().CreateTimer(0.1), Timer.SignalName.Timeout);
        Disabled = false;
    }

    private void ApplyInitialLanguage(string language) {
        switch (language) {
            case "en":
                SetLanguage("en", "English(en)!");
                selectedLanguage = 0;
                break;
            case "jp":
                SetLanguage("jp", "Japanese(jp)!");
                selectedLanguage = 1;
                break;
            case "cmn":
                SetLanguage("cmn", "Tradition Chinese(cmn)!");
                selectedLanguage = 2;
                break;
            case "zh":
                SetLanguage("zh", "Simplified Chinese(zh)!");
                selectedLanguage = 3;
                break;
            case "ko":
                SetLanguage("ko", "Korean(ko)!");
                selectedLanguage = 4;
                break;
            case "pl":
                SetLanguage("pl", "Polish(pl)!");
                selectedLanguage = 5;
                break;
            default:
                selectedLanguage = 0;
                SetLanguage("en", null);
                break;
        }
    }

    private void SetLanguage(string language, string? message) {
        cfg.SetValue("System", "lang", language);
        TranslationServer.SetLocale(language);
        if (message != null) {
            GD.Print(message);
        }
    }
}
