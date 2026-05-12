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

public partial class ConfigTabBar : TabBar {
    private readonly ConfigFile cfg = new ConfigFile();

    public override void _Ready() {
        cfg.Load("user://config.cfg");
        GetNode<CanvasItem>("../Train Data").Visible = true;
        GetNode<CanvasItem>("../Debug").Visible = false;
        GetNode<CanvasItem>("../Credits").Visible = false;
        ClearTabs();
        AddTab(Tr("TRAIN_DATA"));
        AddTab(Tr("GENERAL_SETTING"));
        AddTab("Debug");
        SetTabDisabled(2, !cfg.GetValue("System", "DevSetting", false).AsBool());
        AddTab("Credits");
    }

    public void OnTabChanged(long tab) {
        GetNode<CanvasItem>("../Train Data").Visible = tab == 0;
        GetNode<CanvasItem>("../General").Visible = tab == 1;
        GetNode<CanvasItem>("../Debug").Visible = tab == 2;
        GetNode<CanvasItem>("../Credits").Visible = tab == 3;
    }
}
