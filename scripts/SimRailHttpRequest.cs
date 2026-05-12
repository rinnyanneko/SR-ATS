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

public partial class SimRailHttpRequest : HttpRequest {
    private readonly ConfigFile cfg = new ConfigFile();

    public override async void _Ready() {
        cfg.Load("user://config.cfg");
        if (cfg.GetValue("Train Data", "server", "null").AsString() == "127") {
            return;
        }

        while (true) {
            if (GetHttpClientStatus() == HttpClient.Status.Disconnected) {
                GD.Print("getting data from server...");
                Error value = Request("https://panel.simrail.eu:8084/trains-open?serverCode="
                    + cfg.GetValue("Train Data", "server", "null").AsString());
                if (value == Error.Timeout) {
                    GD.PrintErr("REQUEST TIMEOUT");
                    GetNode<RequestErrorMessage>("../ErrorMsg").ConnectionTimeout();
                } else if (value == Error.Unavailable) {
                    GD.PrintErr("[HTTP REQUEST ERROR]UNAVAILABLE");
                } else if (value != Error.Ok) {
                    GD.PrintErr("[HTTP REQUEST ERROR]" + value);
                }
            }

            await ToSignal(GetTree().CreateTimer(2.5), Timer.SignalName.Timeout);
        }
    }

    public void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body) {
        if (responseCode != 200) {
            GD.PrintErr("HTTP response code:" + responseCode);
            GetNode<RequestErrorMessage>("../ErrorMsg").ConnectionErr((int)responseCode);
            return;
        }

        Variant parsed = Json.ParseString(Encoding.UTF8.GetString(body));
        if (parsed.VariantType != Variant.Type.Dictionary) {
            return;
        }

        Godot.Collections.Dictionary json = parsed.AsGodotDictionary();
        if (json["count"].AsInt32() == 0) {
            GetNode<RequestErrorMessage>("../ErrorMsg").ServerNotFound();
            return;
        }

        Godot.Collections.Dictionary? data = ReadArray(json["data"].AsGodotArray());
        if (data != null) {
            GetNode<CanvasItem>("../ErrorMsg").Visible = false;
            Node parent = GetParent();
            parent.Set("ControlledBySteamID", GetNullableString(data, "ControlledBySteamID", "null"));
            parent.Set("InBorderStationArea", data["InBorderStationArea"]);
            parent.Set("Latititute", data["Latititute"]);
            parent.Set("Longitute", data["Longitute"]);
            parent.Set("Velocity", data["Velocity"]);
            parent.Set("SignalInFront", data["SignalInFront"]);
            parent.Set("DistanceToSignalInFront", data["DistanceToSignalInFront"]);
            parent.Set("SignalInFrontSpeed", data["SignalInFrontSpeed"]);
            parent.Set("VDDelayedTimetableIndex", data["VDDelayedTimetableIndex"]);
            Godot.Collections.Dictionary time = Time.GetTimeDictFromSystem();
            parent.Set(
                "UpdateTime",
                $"{time["hour"].AsInt32():00}:{time["minute"].AsInt32():00}:{time["second"].AsInt32():00}");
        } else {
            GetNode<RequestErrorMessage>("../ErrorMsg").TrainNotFound();
        }
    }

    private Godot.Collections.Dictionary? ReadArray(Godot.Collections.Array array) {
        foreach (Variant item in array) {
            Godot.Collections.Dictionary data = item.AsGodotDictionary();
            if (data["TrainNoLocal"].AsString() == cfg.GetValue("Train Data", "trainNumber").AsString()) {
                return data["TrainData"].AsGodotDictionary();
            }
        }

        return null;
    }

    private static string GetNullableString(Godot.Collections.Dictionary data, string key, string fallback) {
        if (data.ContainsKey(key) && data[key].VariantType != Variant.Type.Nil) {
            return data[key].AsString();
        }

        return fallback;
    }
}
