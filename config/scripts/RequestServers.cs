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

public partial class RequestServers : HttpRequest {
    public Godot.Collections.Array Codes { get; } = new Godot.Collections.Array();

    private readonly ConfigFile cfg = new ConfigFile();

    public override void _Ready() {
        Error value = Request("https://panel.simrail.eu:8084/servers-open");
        if (value == Error.Timeout) {
            GD.PrintErr("REQUEST TIMEOUT");
            GetNode<RequestErrorMessage>("ErrorMsg").ConnectionTimeout();
        } else if (value == Error.Busy) {
            GD.PrintErr("[HTTP REQUEST ERROR]BUSY");
        } else if (value != Error.Ok) {
            GD.PrintErr("[HTTP REQUEST ERROR]" + value);
        }
    }

    public void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body) {
        cfg.Load("user://config.cfg");
        if (responseCode != 200) {
            GD.PrintErr("HTTP response code:" + responseCode);
            GetNode<RequestErrorMessage>("ErrorMsg").ConnectionErr((int)responseCode);
            return;
        }

        Variant parsed = Json.ParseString(Encoding.UTF8.GetString(body));
        if (parsed.VariantType != Variant.Type.Dictionary) {
            return;
        }

        Godot.Collections.Dictionary json = parsed.AsGodotDictionary();
        OptionButton server = GetNode<OptionButton>("../Train Data/Server");
        int count = json["count"].AsInt32();
        for (int i = 0; i < count; i++) {
            Godot.Collections.Dictionary data = json["data"].AsGodotArray()[i].AsGodotDictionary();
            if (data["IsActive"].AsBool()) {
                int optionId = Codes.Count;
                server.AddItem(data["ServerName"].AsString(), optionId);
                Codes.Add(data["ServerCode"]);
            }
        }

        Variant selected = cfg.GetValue("Train Data", "server");
        if (selected.VariantType == Variant.Type.Nil) {
            return;
        }

        for (int i = 0; i < Codes.Count; i++) {
            if (Codes[i].AsString() == selected.AsString()) {
                server.Selected = i;
            }
        }
    }
}
