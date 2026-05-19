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

public enum UpdateCheckStatus {
    UpdateAvailable,
    NoUpdateAvailable,
    ManifestInvalid,
    NetworkError,
    CurrentVersionInvalid
}

public sealed class UpdateCheckResult {
    private UpdateCheckResult(UpdateCheckStatus status, UpdateManifest? manifest, string? message) {
        Status = status;
        Manifest = manifest;
        Message = message;
    }

    public UpdateCheckStatus Status { get; }
    public UpdateManifest? Manifest { get; }
    public string? Message { get; }

    public static UpdateCheckResult Available(UpdateManifest manifest) {
        return new UpdateCheckResult(UpdateCheckStatus.UpdateAvailable, manifest, null);
    }

    public static UpdateCheckResult NoUpdate(UpdateManifest? manifest = null) {
        return new UpdateCheckResult(UpdateCheckStatus.NoUpdateAvailable, manifest, null);
    }

    public static UpdateCheckResult Failure(UpdateCheckStatus status, string? message = null) {
        return new UpdateCheckResult(status, null, message);
    }
}
