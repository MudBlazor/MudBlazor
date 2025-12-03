// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities;

#nullable enable
public static class RangeUtility
{
    /// <summary>
    /// Joins two element string values into the canonical range representation <c>[{start};{end}]</c>.
    /// </summary>
    /// <param name="valueStart">The string representation of the start value (maybe <c>null</c> or empty).</param>
    /// <param name="valueEnd">The string representation of the end value (maybe <c>null</c> or empty).</param>
    /// <returns>The joined range string or an empty string when both parts are empty.</returns>
    public static string Join(string? valueStart, string? valueEnd)
    {
        if (string.IsNullOrEmpty(valueStart) && string.IsNullOrEmpty(valueEnd))
        {
            return string.Empty;
        }

        return $"[{valueStart};{valueEnd}]";
    }

    /// <summary>
    /// Splits a canonical range string into its start and end parts.
    /// </summary>
    /// <param name="value">The range string to split (expected in the form <c>[{start};{end}]</c>).</param>
    /// <param name="valueStart">Output parameter that receives the start part on success; otherwise empty string.</param>
    /// <param name="valueEnd">Output parameter that receives the end part on success; otherwise empty string.</param>
    /// <returns><c>true</c> when <paramref name="value"/> is a valid canonical range and parts were extracted; otherwise <c>false</c>.</returns>
    public static bool Split(string? value, out string valueStart, out string valueEnd)
    {
        valueStart = valueEnd = string.Empty;

        if (string.IsNullOrEmpty(value) || value[0] != '[' || value[^1] != ']')
        {
            return false;
        }

        var idx = value.IndexOf(';');
        if (idx < 1)
        {
            return false;
        }

        valueStart = value[1..idx];
        valueEnd = value[(idx + 1)..^1];

        return true;
    }
}
