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
using System.Text;

public partial class UpdateRequest : HttpRequest {
    private readonly ConfigFile cfg = new ConfigFile();
    private bool update = false;
    private bool updateBeta = false;

    public override void _Ready() {
        GetNode<Label>("../../version").Text = FileAccess.GetFileAsString("res://version.txt");
        CheckForUpdate();
    }

    public void OnVersionPressed() {
        GetParent<CanvasItem>().Visible = true;
    }

    public void OnUpdaterConfirmed() {
        if (update) {
            if (cfg.GetValue("General", "UpdateMirror").AsString() == "GitCode") {
                OS.ShellOpen("https://gitcode.com/rinnyanneko/SR-ATS/releases");
            } else {
                OS.ShellOpen("https://github.com/rinnyanneko/SR-ATS/releases/latest");
            }
        }

        if (updateBeta) {
            if (cfg.GetValue("General", "UpdateMirror").AsString() == "GitCode") {
                OS.ShellOpen("https://gitcode.com/rinnyanneko/SR-ATS/releases");
            } else {
                OS.ShellOpen("https://github.com/rinnyanneko/SR-ATS/releases");
            }
        }
    }

    public async void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body) {
        if (responseCode != 200) {
            return;
        }

        GetParent<CanvasItem>().Visible = true;
        Variant parsed = Json.ParseString(Encoding.UTF8.GetString(body));
        if (parsed.VariantType != Variant.Type.Dictionary) {
            return;
        }

        Godot.Collections.Dictionary json = parsed.AsGodotDictionary();
        AcceptDialog dialog = GetParent<AcceptDialog>();
        string currentVersion = GetNode<Label>("../../version").Text;
        if (cfg.GetValue("General", "UpdateChannel", "Stable").AsString() == "Beta") {
            if (json["latest-beta"].AsString() != currentVersion) {
                dialog.DialogText = Tr("NEW_BETA_UPDATE").Replace("{version}", json["latest-beta"].AsString());
                updateBeta = true;
            } else {
                await ShowNoUpdateThenHide(dialog);
            }
        } else if (json["latest-stable"].AsString() != currentVersion) {
            dialog.DialogText = Tr("NEW_STABLE_UPDATE").Replace("{version}", json["latest-stable"].AsString());
            update = true;
        } else {
            await ShowNoUpdateThenHide(dialog);
        }
    }

    private void CheckForUpdate() {
        cfg.Load("user://config.cfg");
        string mirror = cfg.GetValue("General", "UpdateMirror", "GitHub").AsString();
        if (mirror == "GitHub") {
            Request("https://raw.githubusercontent.com/rinnyanneko/SR-ATS/refs/heads/main/news/news.json");
        } else if (mirror == "GitCode") {
            Request("https://raw.gitcode.com/rinnyanneko/SR-ATS/raw/main/news/news.json");
        }
    }

    private async System.Threading.Tasks.Task ShowNoUpdateThenHide(AcceptDialog dialog) {
        dialog.DialogText = Tr("NO_UPDATE");
        await ToSignal(GetTree().CreateTimer(3), Timer.SignalName.Timeout);
        dialog.Visible = false;
    }
}
