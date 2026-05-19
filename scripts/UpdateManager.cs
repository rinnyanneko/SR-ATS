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
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public partial class UpdateManager : Node {
    public const string ConfigSection = "General";
    public const string UpdateChannelKey = "UpdateChannel";
    public const string StableManifestUrl = "https://mirukuneko.cc/sr-ats/latest.json";
    public const string PreviewManifestUrl = "https://mirukuneko.cc/sr-ats/latest-preview.json";
    public const string ReleasesUrl = "https://github.com/rinnyanneko/SR-ATS/releases";

    private const string ConfigPath = "user://config.cfg";
    private const string VersionPath = "res://version.txt";
    private const string UpdatesPath = "user://updates";
    private const string ProductName = "SR-ATS";

    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    private readonly System.Net.Http.HttpClient httpClient = new() {
        Timeout = TimeSpan.FromSeconds(30)
    };

    public override void _ExitTree() {
        httpClient.Dispose();
    }

    public string GetCurrentVersion() {
        string version = Godot.FileAccess.GetFileAsString(VersionPath).Trim();
        return string.IsNullOrWhiteSpace(version) ? "UNKNOWN" : version;
    }

    public UpdateChannel GetConfiguredChannel() {
        ConfigFile cfg = new ConfigFile();
        cfg.Load(ConfigPath);
        string value = cfg.GetValue(ConfigSection, UpdateChannelKey, UpdateChannel.Stable.ToConfigValue()).AsString();
        return UpdateChannelExtensions.ParseConfigValue(value);
    }

    public void SetConfiguredChannel(UpdateChannel channel) {
        ConfigFile cfg = new ConfigFile();
        cfg.Load(ConfigPath);
        cfg.SetValue(ConfigSection, UpdateChannelKey, channel.ToConfigValue());
        cfg.Save(ConfigPath);
    }

    public async Task<UpdateCheckResult> CheckForUpdatesAsync(UpdateChannel? channel = null, CancellationToken cancellationToken = default) {
        UpdateChannel selectedChannel = channel ?? GetConfiguredChannel();
        string manifestUrl = GetManifestUrl(selectedChannel);
        UpdateManifest? manifest;
        try {
            string json = await httpClient.GetStringAsync(manifestUrl, cancellationToken);
            manifest = JsonSerializer.Deserialize<UpdateManifest>(json, JsonOptions);
        } catch (OperationCanceledException) {
            return UpdateCheckResult.Failure(UpdateCheckStatus.NetworkError);
        } catch (HttpRequestException ex) {
            return UpdateCheckResult.Failure(UpdateCheckStatus.NetworkError, ex.Message);
        } catch (JsonException ex) {
            return UpdateCheckResult.Failure(UpdateCheckStatus.ManifestInvalid, ex.Message);
        }

        if (!IsManifestSafeForChannel(manifest, selectedChannel)) {
            return UpdateCheckResult.Failure(UpdateCheckStatus.ManifestInvalid);
        }

        string currentVersion = GetCurrentVersion();
        if (!VersionComparer.TryCompare(manifest!.Version, currentVersion, out int comparison)) {
            return UpdateCheckResult.Failure(UpdateCheckStatus.CurrentVersionInvalid);
        }

        return comparison > 0
            ? UpdateCheckResult.Available(manifest)
            : UpdateCheckResult.NoUpdate(manifest);
    }

    public async Task<string?> DownloadInstallerAsync(UpdateManifest manifest, CancellationToken cancellationToken = default) {
        if (string.IsNullOrWhiteSpace(manifest.InstallerUrl) ||
            !Uri.TryCreate(manifest.InstallerUrl, UriKind.Absolute, out Uri? installerUri)) {
            return null;
        }

        string updatesDirectory = ProjectSettings.GlobalizePath(UpdatesPath);
        Directory.CreateDirectory(updatesDirectory);

        string fileName = Path.GetFileName(installerUri.LocalPath);
        if (string.IsNullOrWhiteSpace(fileName) || !fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) {
            fileName = $"SR-ATS-Setup-{SanitizeFileName(manifest.Version ?? "update")}.exe";
        }

        string downloadPath = Path.Combine(updatesDirectory, fileName);
        try {
            await using Stream remoteStream = await httpClient.GetStreamAsync(installerUri, cancellationToken);
            await using FileStream localStream = File.Create(downloadPath);
            await remoteStream.CopyToAsync(localStream, cancellationToken);
            return downloadPath;
        } catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or HttpRequestException or TaskCanceledException) {
            GD.PushWarning($"Update installer download failed: {ex.Message}");
            return null;
        }
    }

    public bool VerifyInstallerSha256(string installerPath, string? expectedSha256) {
        if (string.IsNullOrWhiteSpace(expectedSha256)) {
            OS.ShellOpen(ReleasesUrl);
            return false;
        }

        string normalizedExpectedHash = expectedSha256.Trim().Replace(" ", string.Empty).ToLowerInvariant();
        if (normalizedExpectedHash.Length != 64) {
            return false;
        }

        try {
            using FileStream file = File.OpenRead(installerPath);
            byte[] hash = SHA256.HashData(file);
            string actualHash = Convert.ToHexString(hash).ToLowerInvariant();
            return actualHash == normalizedExpectedHash;
        } catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
            GD.PushWarning($"Update installer verification failed: {ex.Message}");
            return false;
        }
    }

    public bool LaunchInstallerAndQuit(string installerPath) {
        if (!File.Exists(installerPath)) {
            return false;
        }

        int processId = OS.CreateProcess(installerPath, []);
        if (processId <= 0) {
            return false;
        }

        GetTree().Quit();
        return true;
    }

    public static string GetManifestUrl(UpdateChannel channel) {
        return channel == UpdateChannel.Preview ? PreviewManifestUrl : StableManifestUrl;
    }

    private static bool IsManifestSafeForChannel(UpdateManifest? manifest, UpdateChannel channel) {
        if (manifest == null ||
            !string.Equals(manifest.Product, ProductName, StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(manifest.Version) ||
            string.IsNullOrWhiteSpace(manifest.InstallerUrl)) {
            return false;
        }

        if (!Uri.TryCreate(manifest.InstallerUrl, UriKind.Absolute, out _)) {
            return false;
        }

        string manifestChannel = manifest.Channel?.Trim().ToLowerInvariant() ?? string.Empty;
        if (channel == UpdateChannel.Stable) {
            return !manifest.Prerelease && manifestChannel == UpdateChannel.Stable.ToManifestValue();
        }

        return manifestChannel is "preview" or "stable";
    }

    private static string SanitizeFileName(string fileName) {
        foreach (char invalidChar in Path.GetInvalidFileNameChars()) {
            fileName = fileName.Replace(invalidChar, '-');
        }

        return fileName;
    }
}
