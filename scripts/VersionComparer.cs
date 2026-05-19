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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static partial class VersionComparer {
    private sealed record ParsedVersion(int Major, int Minor, int Patch, IReadOnlyList<string> PreRelease);

    public static bool TryCompare(string? left, string? right, out int result) {
        result = 0;
        if (!TryParse(left, out ParsedVersion? leftVersion) || !TryParse(right, out ParsedVersion? rightVersion)) {
            return false;
        }

        result = Compare(leftVersion!, rightVersion!);
        return true;
    }

    public static bool IsNewer(string? candidate, string? current) {
        return TryCompare(candidate, current, out int result) && result > 0;
    }

    private static bool TryParse(string? rawVersion, out ParsedVersion? version) {
        version = null;
        if (string.IsNullOrWhiteSpace(rawVersion)) {
            return false;
        }

        Match match = VersionRegex().Match(rawVersion.Trim());
        if (!match.Success) {
            return false;
        }

        List<string> preRelease = [];
        string semverPreRelease = match.Groups["pre"].Value;
        string betaNumber = match.Groups["beta"].Value;
        if (!string.IsNullOrEmpty(semverPreRelease)) {
            preRelease.AddRange(semverPreRelease.Split('.', StringSplitOptions.RemoveEmptyEntries));
        } else if (!string.IsNullOrEmpty(betaNumber)) {
            preRelease.Add("beta");
            preRelease.Add(betaNumber);
        }

        version = new ParsedVersion(
            int.Parse(match.Groups["major"].Value),
            int.Parse(match.Groups["minor"].Value),
            int.Parse(match.Groups["patch"].Value),
            preRelease);
        return true;
    }

    private static int Compare(ParsedVersion left, ParsedVersion right) {
        int coreResult = left.Major.CompareTo(right.Major);
        if (coreResult != 0) {
            return coreResult;
        }

        coreResult = left.Minor.CompareTo(right.Minor);
        if (coreResult != 0) {
            return coreResult;
        }

        coreResult = left.Patch.CompareTo(right.Patch);
        if (coreResult != 0) {
            return coreResult;
        }

        bool leftStable = left.PreRelease.Count == 0;
        bool rightStable = right.PreRelease.Count == 0;
        if (leftStable && rightStable) {
            return 0;
        }

        if (leftStable) {
            return 1;
        }

        if (rightStable) {
            return -1;
        }

        int count = Math.Max(left.PreRelease.Count, right.PreRelease.Count);
        for (int i = 0; i < count; i++) {
            if (i >= left.PreRelease.Count) {
                return -1;
            }

            if (i >= right.PreRelease.Count) {
                return 1;
            }

            string leftIdentifier = left.PreRelease[i];
            string rightIdentifier = right.PreRelease[i];
            bool leftNumber = int.TryParse(leftIdentifier, out int leftNumeric);
            bool rightNumber = int.TryParse(rightIdentifier, out int rightNumeric);
            if (leftNumber && rightNumber) {
                int numericResult = leftNumeric.CompareTo(rightNumeric);
                if (numericResult != 0) {
                    return numericResult;
                }
            } else if (leftNumber) {
                return -1;
            } else if (rightNumber) {
                return 1;
            } else {
                int textResult = string.Compare(leftIdentifier, rightIdentifier, StringComparison.OrdinalIgnoreCase);
                if (textResult != 0) {
                    return textResult;
                }
            }
        }

        return 0;
    }

    [GeneratedRegex(@"^v?(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<pre>[0-9A-Za-z.-]+)|b(?<beta>0|[1-9]\d*))?$", RegexOptions.CultureInvariant)]
    private static partial Regex VersionRegex();
}
