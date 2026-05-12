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
using System.Text;

public partial class NewsRequest : HttpRequest {
    private readonly ConfigFile cfg = new ConfigFile();
    private Godot.Collections.Dictionary? data;

    public override void _Ready() {
        Request("https://gitlab.com/rinnyanneko/SR-ATS/-/raw/main/news/news.json");
    }

    public void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body) {
        if (responseCode != 200) {
            return;
        }

        cfg.Load("user://config.cfg");
        Variant parsed = Json.ParseString(Encoding.UTF8.GetString(body));
        if (parsed.VariantType != Variant.Type.Dictionary) {
            return;
        }

        foreach (Variant item in parsed.AsGodotDictionary()["data"].AsGodotArray()) {
            Godot.Collections.Dictionary newsItem = item.AsGodotDictionary();
            if (!newsItem["draft"].AsBool()) {
                data = newsItem;
            }
        }

        if (data == null) {
            return;
        }

        ConfirmationDialog newsDialog = GetNode<ConfirmationDialog>("../news");
        newsDialog.Title = data["title"].AsString();
        GetNode<RichTextLabel>("../news/RichTextLabel").Text = data["text"].AsString();
        newsDialog.CancelButtonText = Tr("CLOSE");
        newsDialog.OkButtonText = Tr("DO_NOT_SHOW");
        if (data["type"].AsString() != ""
            && data["number"].AsInt32() > cfg.GetValue("News", "NeverShow", 0).AsInt32()
            && !data["draft"].AsBool()) {
            newsDialog.Visible = true;
        }
    }

    public void OnNewsConfirmed() {
        if (data == null) {
            return;
        }

        cfg.SetValue("News", "NeverShow", data["number"]);
        cfg.Save("user://config.cfg");
        GD.Print("Never show this news");
        GetNode<CanvasItem>("../news").Visible = false;
    }

    public void OnNewsCanceled() {
        GetNode<CanvasItem>("../news").Visible = false;
        GD.Print("News closed");
    }

    public void OnRichTextLabelMetaClicked(Variant meta) {
        OS.ShellOpen(meta.AsString());
    }
}
