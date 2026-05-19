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

public partial class ConfigTabBar : TabBar {
    private readonly ConfigFile cfg = new ConfigFile();
    private LocalizationManager? localizationManager;

    public override void _Ready() {
        cfg.Load("user://config.cfg");
        WindowSettings.ApplyFromConfig();
        GetNode<CanvasItem>("../General").Visible = true;
        GetNode<CanvasItem>("../Debug").Visible = false;
        GetNode<CanvasItem>("../Credits").Visible = false;
        localizationManager = GetNodeOrNull<LocalizationManager>("/root/LocalizationManager");
        if (localizationManager != null) {
            localizationManager.LocaleChanged += OnLocaleChanged;
        }

        RefreshTranslations();
    }

    public override void _ExitTree() {
        if (localizationManager != null) {
            localizationManager.LocaleChanged -= OnLocaleChanged;
        }
    }

    public void RefreshTranslations() {
        int currentTab = CurrentTab;
        ClearTabs();
        AddTab(Tr("GENERAL_SETTING"));
        AddTab("Debug");
        SetTabDisabled(1, !cfg.GetValue("System", "DevSetting", false).AsBool());
        AddTab(Tr("CREDITS"));
        CurrentTab = Mathf.Clamp(currentTab, 0, TabCount - 1);
    }

    public void OnTabChanged(long tab) {
        GetNode<CanvasItem>("../General").Visible = tab == 0;
        GetNode<CanvasItem>("../Debug").Visible = tab == 1;
        GetNode<CanvasItem>("../Credits").Visible = tab == 2;
    }

    private void OnLocaleChanged(string locale) {
        RefreshTranslations();
    }
}
