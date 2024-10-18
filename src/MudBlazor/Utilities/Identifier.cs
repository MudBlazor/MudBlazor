// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor;

/// <summary>
/// Provides methods to create unique identifiers with optional prefixes.
/// </summary>
internal static class Identifier
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const int CharsLength = 35;
    private const int RandomStringLength = 8;

    /// <summary>
    /// Creates a unique identifier with the specified prefix.
    /// </summary>
    /// <param name="prefix">The prefix to prepend to the unique identifier.</param>
    /// <returns>A unique identifier string with the specified prefix.</returns>
    /// <example><code>prefixdb54bcd0</code></example>
    internal static string Create(ReadOnlySpan<char> prefix)
    {
        Span<char> identifierSpan = stackalloc char[prefix.Length + RandomStringLength];
        prefix.CopyTo(identifierSpan);
        for (var i = 0; i < RandomStringLength; i++)
        {
            var index = Random.Shared.Next(CharsLength);
            identifierSpan[prefix.Length + i] = Chars[index];
        }

        return identifierSpan.ToString();
    }

    /// <summary>
    /// Creates a unique identifier.
    /// </summary>
    /// <returns>A unique identifier string.</returns>
    /// <example><code>adb54bcd0</code></example>
    internal static string Create() => Create(['a']);
}
