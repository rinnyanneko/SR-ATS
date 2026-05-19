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
using System;
using System.Linq;

public static class UpdateUiHelper {
    public static void ShowUpdateAvailableDialog(Node owner, UpdateManifest manifest, Action<string>? statusChanged = null) {
        UpdateManager? updateManager = owner.GetNodeOrNull<UpdateManager>("/root/UpdateManager");
        if (updateManager == null) {
            statusChanged?.Invoke(owner.Tr("UPDATE_INSTALLER_START_FAILED"));
            return;
        }

        ConfirmationDialog dialog = new() {
            Title = owner.Tr("UPDATE_AVAILABLE_TITLE"),
            DialogText = FormatUpdateMessage(owner, manifest),
            InitialPosition = Window.WindowInitialPosition.CenterMainWindowScreen,
            Unresizable = false
        };

        owner.AddChild(dialog);
        dialog.GetOkButton().Text = owner.Tr("UPDATE_DOWNLOAD_AND_INSTALL");
        dialog.GetCancelButton().Text = owner.Tr("UPDATE_LATER");
        dialog.Canceled += dialog.QueueFree;
        dialog.Confirmed += async () => {
            statusChanged?.Invoke(owner.Tr("UPDATE_DOWNLOADING"));
            if (string.IsNullOrWhiteSpace(manifest.Sha256)) {
                OS.ShellOpen(UpdateManager.ReleasesUrl);
                statusChanged?.Invoke(owner.Tr("UPDATE_HASH_MISSING"));
                dialog.QueueFree();
                return;
            }

            string? installerPath = await updateManager.DownloadInstallerAsync(manifest);
            if (string.IsNullOrWhiteSpace(installerPath)) {
                statusChanged?.Invoke(owner.Tr("UPDATE_DOWNLOAD_FAILED"));
                ShowError(owner, owner.Tr("UPDATE_DOWNLOAD_FAILED"));
                dialog.QueueFree();
                return;
            }

            if (!updateManager.VerifyInstallerSha256(installerPath, manifest.Sha256)) {
                statusChanged?.Invoke(owner.Tr("UPDATE_VERIFY_FAILED"));
                ShowError(owner, owner.Tr("UPDATE_VERIFY_FAILED"));
                dialog.QueueFree();
                return;
            }

            statusChanged?.Invoke(owner.Tr("UPDATE_RESTART_REQUIRED"));
            if (!updateManager.LaunchInstallerAndQuit(installerPath)) {
                statusChanged?.Invoke(owner.Tr("UPDATE_INSTALLER_START_FAILED"));
                ShowError(owner, owner.Tr("UPDATE_INSTALLER_START_FAILED"));
                dialog.QueueFree();
            }
        };
        dialog.PopupCentered();
    }

    public static string GetStatusText(Node owner, UpdateCheckResult result) {
        return result.Status switch {
            UpdateCheckStatus.NoUpdateAvailable => owner.Tr("UPDATE_NOT_AVAILABLE"),
            UpdateCheckStatus.ManifestInvalid => owner.Tr("UPDATE_MANIFEST_INVALID"),
            UpdateCheckStatus.NetworkError => owner.Tr("UPDATE_NETWORK_ERROR"),
            UpdateCheckStatus.CurrentVersionInvalid => owner.Tr("UPDATE_CURRENT_VERSION_INVALID"),
            _ => string.Empty
        };
    }

    private static string FormatUpdateMessage(Node owner, UpdateManifest manifest) {
        string notes = manifest.Notes == null || manifest.Notes.Count == 0
            ? string.Empty
            : "\n\n" + string.Join("\n", manifest.Notes.Select(note => $"* {note}"));

        return owner.Tr("UPDATE_AVAILABLE_MESSAGE")
            .Replace("{version}", manifest.Version ?? string.Empty)
            .Replace("{channel}", manifest.Channel ?? string.Empty) + notes;
    }

    private static void ShowError(Node owner, string message) {
        AcceptDialog dialog = new() {
            Title = owner.Tr("ERROR"),
            DialogText = message,
            InitialPosition = Window.WindowInitialPosition.CenterMainWindowScreen
        };
        owner.AddChild(dialog);
        dialog.Confirmed += dialog.QueueFree;
        dialog.PopupCentered();
    }
}
