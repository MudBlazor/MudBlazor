// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Globalization;

// ReSharper disable once CheckNamespace
internal static class StringExtensions
{
    public static bool IsEmpty(this string value) => string.IsNullOrWhiteSpace(value);
    public static bool IsNonEmpty(this string value) => !string.IsNullOrWhiteSpace(value);
    public static string Trimmed(this string value) => value == null ? string.Empty : value.Trim();
    public static string ToPercentage(this decimal value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
