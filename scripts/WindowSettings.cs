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

public static class WindowSettings {
    public const string Section = "System";
    public const string AlwaysOnTopKey = "AlwaysOnTop";
    private const int StableMaxFps = 60;

    public static void ApplyFromConfig() {
        ConfigFile cfg = new ConfigFile();
        cfg.Load("user://config.cfg");
        ApplyFrameRate();
        ApplyAlwaysOnTop(cfg.GetValue(Section, AlwaysOnTopKey, false).AsBool());
    }

    public static void ApplyAlwaysOnTop(bool enabled) {
        if (DisplayServer.WindowGetFlag(DisplayServer.WindowFlags.AlwaysOnTop) == enabled) {
            return;
        }

        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.AlwaysOnTop, enabled);
    }

    public static void ApplyFrameRate() {
        Engine.MaxFps = StableMaxFps;
    }
}
