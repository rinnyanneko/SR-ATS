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

public partial class LocalizationManager : Node {
    public const string Section = "System";
    public const string LocaleKey = "lang";
    public const string DefaultLocale = "en";

    private const string ConfigPath = "user://config.cfg";
    private const string TranslationCsvPath = "res://translation.csv";

    private static readonly Dictionary<string, string> LocaleAliases = new() {
        ["ja"] = "jp",
        ["ja_JP"] = "jp",
        ["zh_TW"] = "cmn",
        ["zh_HK"] = "cmn",
        ["zh_MO"] = "cmn",
        ["zh_CN"] = "zh",
        ["zh_SG"] = "zh"
    };

    [Signal]
    public delegate void LocaleChangedEventHandler(string locale);

    public override void _Ready() {
        LoadCsvTranslations();
        ApplySavedOrOsLocale();
    }

    public void ApplySavedOrOsLocale() {
        ConfigFile cfg = new ConfigFile();
        cfg.Load(ConfigPath);

        string savedLocale = cfg.GetValue(Section, LocaleKey, string.Empty).AsString();
        string locale = string.IsNullOrWhiteSpace(savedLocale)
            ? FindSupportedLocale(TranslationServer.GetLocale())
            : FindSupportedLocale(savedLocale);

        ApplyLocale(locale, !string.IsNullOrWhiteSpace(savedLocale));
    }

    public void ApplyLocale(string locale, bool save = true) {
        string supportedLocale = FindSupportedLocale(locale);
        if (TranslationServer.GetLocale() != supportedLocale) {
            TranslationServer.SetLocale(supportedLocale);
        }

        if (save) {
            ConfigFile cfg = new ConfigFile();
            cfg.Load(ConfigPath);
            cfg.SetValue(Section, LocaleKey, supportedLocale);
            cfg.Save(ConfigPath);
        }

        EmitSignal(SignalName.LocaleChanged, supportedLocale);
    }

    public string[] GetAvailableLocales() {
        return TranslationServer.GetLoadedLocales();
    }

    public string GetCurrentLocale() {
        return FindSupportedLocale(TranslationServer.GetLocale());
    }

    public string GetLocaleDisplayName(string locale) {
        string supportedLocale = FindSupportedLocale(locale);
        string previousLocale = TranslationServer.GetLocale();
        TranslationServer.SetLocale(supportedLocale);
        string languageName = Tr("LANG_NAME");
        TranslationServer.SetLocale(previousLocale);

        if (languageName == "LANG_NAME") {
            languageName = supportedLocale;
        }

        return $"{languageName}({GetDisplayLocaleCode(supportedLocale)})";
    }

    public string TranslateOrFallback(string key, string fallback) {
        string translated = Tr(key);
        return translated == key || string.IsNullOrEmpty(translated) ? fallback : translated;
    }

    private static string FindSupportedLocale(string locale) {
        string normalizedLocale = locale.Replace("-", "_");
        if (LocaleAliases.TryGetValue(normalizedLocale, out string? alias)) {
            normalizedLocale = alias;
        }

        string[] loadedLocales = TranslationServer.GetLoadedLocales();
        foreach (string loadedLocale in loadedLocales) {
            if (loadedLocale == normalizedLocale) {
                return loadedLocale;
            }
        }

        string language = normalizedLocale.Split('_')[0];
        if (LocaleAliases.TryGetValue(language, out alias)) {
            language = alias;
        }

        foreach (string loadedLocale in loadedLocales) {
            if (loadedLocale == language || loadedLocale.Split('_')[0] == language) {
                return loadedLocale;
            }
        }

        return DefaultLocale;
    }

    private static void LoadCsvTranslations() {
        using FileAccess? file = FileAccess.Open(TranslationCsvPath, FileAccess.ModeFlags.Read);
        if (file == null) {
            return;
        }

        string[] header = file.GetCsvLine(",");
        if (header.Length < 2 || header[0] != "key") {
            return;
        }

        Dictionary<string, Translation> translations = [];
        for (int i = 1; i < header.Length; i++) {
            if (string.IsNullOrWhiteSpace(header[i])) {
                continue;
            }

            translations[header[i]] = new Translation {
                Locale = header[i]
            };
        }

        while (!file.EofReached()) {
            string[] row = file.GetCsvLine(",");
            if (row.Length == 0 || string.IsNullOrWhiteSpace(row[0])) {
                continue;
            }

            string key = row[0];
            string englishFallback = row.Length > 1 ? UnescapeTranslation(row[1]) : key;
            for (int i = 1; i < header.Length; i++) {
                if (!translations.TryGetValue(header[i], out Translation? translation)) {
                    continue;
                }

                string message = i < row.Length && !string.IsNullOrEmpty(row[i])
                    ? UnescapeTranslation(row[i])
                    : englishFallback;
                translation.AddMessage(new StringName(key), new StringName(message), new StringName());
            }
        }

        TranslationServer.Clear();
        foreach (Translation translation in translations.Values) {
            TranslationServer.AddTranslation(translation);
        }
    }

    private static string UnescapeTranslation(string translation) {
        return translation.Replace("\\n", "\n");
    }

    private static string GetDisplayLocaleCode(string locale) {
        return locale switch {
            "cmn" => "zh-TW",
            "zh" => "zh-CN",
            "jp" => "ja",
            _ => locale.Replace("_", "-")
        };
    }
}
