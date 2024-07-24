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
    private const int GuidLength = 32;
    private const int GuidSubLength = 8;

    /// <summary>
    /// Creates a unique identifier with the specified prefix.
    /// </summary>
    /// <param name="prefix">The prefix to prepend to the unique identifier.</param>
    /// <returns>A unique identifier string with the specified prefix.</returns>
    /// <example><code>prefixdb54bcd0</code></example>
    internal static string Create(ReadOnlySpan<char> prefix)
    {
        Span<char> identifierSpan = stackalloc char[prefix.Length + GuidLength];
        prefix.CopyTo(identifierSpan);
        Guid.NewGuid().TryFormat(identifierSpan[prefix.Length..], out _, ['n']);
        return identifierSpan[..(prefix.Length + GuidSubLength)].ToString();
    }

    /// <summary>
    /// Creates a unique identifier.
    /// </summary>
    /// <returns>A unique identifier string.</returns>
    /// <example><code>adb54bcd0</code></example>
    internal static string Create() => Create(['a']);
}
