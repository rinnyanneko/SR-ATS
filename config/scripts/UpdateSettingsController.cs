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

public partial class UpdateSettingsController : VBoxContainer {
    private LocalizationManager? localizationManager;

    public override void _Ready() {
        localizationManager = GetNodeOrNull<LocalizationManager>("/root/LocalizationManager");
        if (localizationManager != null) {
            localizationManager.LocaleChanged += OnLocaleChanged;
        }

        Refresh();
    }

    public override void _ExitTree() {
        if (localizationManager != null) {
            localizationManager.LocaleChanged -= OnLocaleChanged;
        }
    }

    public async void OnCheckNowPressed() {
        UpdateManager? updateManager = GetNodeOrNull<UpdateManager>("/root/UpdateManager");
        if (updateManager == null) {
            SetStatus(Tr("UPDATE_NETWORK_ERROR"));
            return;
        }

        SetStatus(Tr("UPDATE_CHECKING"));
        UpdateChannel channel = GetNode<OptionButton>("../UpdateChannel").Selected == 1
            ? UpdateChannel.Preview
            : UpdateChannel.Stable;
        updateManager.SetConfiguredChannel(channel);

        UpdateCheckResult result = await updateManager.CheckForUpdatesAsync(channel);
        if (result.Status == UpdateCheckStatus.UpdateAvailable && result.Manifest != null) {
            SetStatus(Tr("UPDATE_AVAILABLE_TITLE"));
            UpdateUiHelper.ShowUpdateAvailableDialog(this, result.Manifest, SetStatus);
            return;
        }

        SetStatus(UpdateUiHelper.GetStatusText(this, result));
    }

    public void Refresh() {
        UpdateManager? updateManager = GetNodeOrNull<UpdateManager>("/root/UpdateManager");
        string currentVersion = updateManager?.GetCurrentVersion() ?? "UNKNOWN";
        GetNode<Label>("CurrentVersion").Text = Tr("UPDATE_CURRENT_VERSION").Replace("{version}", currentVersion);
        GetNode<Button>("CheckNow").Text = Tr("UPDATE_CHECK_NOW");

        if (string.IsNullOrWhiteSpace(GetNode<Label>("Status").Text)) {
            SetStatus(string.Empty);
        }
    }

    private void SetStatus(string status) {
        GetNode<Label>("Status").Text = status;
    }

    private void OnLocaleChanged(string locale) {
        Refresh();
    }
}
