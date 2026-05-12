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

public partial class RequestErrorMessage : AcceptDialog {
    public override void _Ready() {
        if (GetChild(1, true) is Label label) {
            label.HorizontalAlignment = HorizontalAlignment.Center;
        }
    }

    public void OnConfirmed() {
        GetTree().ChangeSceneToFile("res://main.tscn");
    }

    public void TrainNotFound() {
        DialogText = Tr("TRAIN_NOT_FOUND");
        Visible = true;
    }

    public void ServerNotFound() {
        DialogText = Tr("SERVER_NOT_FOUND");
        Visible = true;
    }

    public void ConnectionErr(int code) {
        DialogText = Tr("CONNECTION_ERROR") + "\n" + GetHttpStatusText(code);
        Visible = true;
    }

    public void ConnectionTimeout() {
        DialogText = Tr("CONNECTION_TIMEOUT");
        Visible = true;
    }

    private static string GetHttpStatusText(int code) {
        return code switch {
            400 => "400 Bad Request",
            401 => "401 Unauthorized",
            402 => "402 Payment Required",
            403 => "403 Forbidden",
            404 => "404 Not Found",
            405 => "405 Method Not Allowed",
            406 => "406 Not Acceptable",
            407 => "407 Proxy Authentication Required",
            408 => "408 Request Timeout",
            425 => "425 Too Early",
            451 => "451 Unavailable For Legal Reasons",
            500 => "500 Internal Server Error",
            501 => "501 Not Implemented",
            502 => "502 Bad Gateway",
            503 => "503 Service Unavailable",
            504 => "504 Gateway Timeout",
            505 => "505 HTTP Version Not Supported",
            508 => "508 Loop Detected",
            511 => "511 Network Authentication Required",
            444 => "444 No Response",
            450 => "450 Blocked by Windows Parental Controls",
            _ => code.ToString()
        };
    }
}
