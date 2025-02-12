// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace MudBlazor;

#nullable enable
/// <summary>
/// Provides extension methods for string manipulation and formatting.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Determines whether the specified string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <c>true</c> if the value parameter is null or <see cref="string.Empty"/>, or if value consists exclusively of white-space characters.
    /// </returns>
    public static bool IsEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Returns a new string in which all leading and trailing white-space characters from the current string object are removed.
    /// If the string is null, an empty string is returned.
    /// </summary>
    /// <param name="value">The string to trim.</param>
    /// <returns>
    /// The string that remains after all white-space characters are removed from the start and end of the current string.
    /// If the string is null, an empty string is returned.
    /// </returns>
    public static string Trimmed(this string? value) => value is null ? string.Empty : value.Trim();

    /// <summary>
    /// Converts the specified decimal value to a percentage string with up to two decimal places.
    /// </summary>
    /// <param name="value">The decimal value to convert.</param>
    /// <returns>
    /// A string representation of the decimal value formatted as a percentage with up to two decimal places.
    /// </returns>
    public static string ToPercentage(this decimal value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
