// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Provides high-performance methods to create unique identifiers with optional prefixes.
/// </summary>
/// <remarks>
/// This class uses optimized algorithms to generate identifiers quickly while maintaining uniqueness.
/// Identifiers consist of lowercase letters and digits from the set [a-z0-9].
/// </remarks>
public static class Identifier
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const int CharsLength = 36;
    private const int RandomStringLength = 8;

    /// <summary>
    /// Creates a unique identifier with the specified prefix.
    /// </summary>
    /// <param name="prefix">The prefix to prepend to the unique identifier.</param>
    /// <returns>A unique identifier string with the specified prefix.</returns>
    /// <example>
    /// <code>
    /// var id = Identifier.Create("button");
    /// // Returns something like: "buttonx7k2n9q4"
    /// </code>
    /// </example>
    public static string Create(ReadOnlySpan<char> prefix)
    {
        Span<char> identifierSpan = stackalloc char[prefix.Length + RandomStringLength];
        prefix.CopyTo(identifierSpan);

        // Generate two random 64-bit integers for maximum performance
        var random1 = Random.Shared.NextInt64();
        var random2 = Random.Shared.NextInt64();

        // Extract characters from the random bits
        for (var i = 0; i < 4; i++)
        {
            identifierSpan[prefix.Length + i] = Chars[(int)((random1 >> (i * 8)) % CharsLength)];
        }
        for (var i = 4; i < 8; i++)
        {
            identifierSpan[prefix.Length + i] = Chars[(int)((random2 >> ((i - 4) * 8)) % CharsLength)];
        }

        return identifierSpan.ToString();
    }

    /// <summary>
    /// Creates a unique identifier with a randomly generated prefix.
    /// </summary>
    /// <returns>A unique identifier string.</returns>
    /// <remarks>
    /// Unlike a fixed prefix, this method generates a random first character for improved uniqueness.
    /// </remarks>
    /// <example>
    /// <code>
    /// var id = Identifier.Create();
    /// // Returns something like: "m7k2n9q4p" (9 characters, first one random)
    /// </code>
    /// </example>
    public static string Create()
    {
        Span<char> identifierSpan = stackalloc char[RandomStringLength + 1];

        // Generate two random 64-bit integers for maximum performance
        var random1 = Random.Shared.NextInt64();
        var random2 = Random.Shared.NextInt64();

        // First character from first random bits (letters only for valid HTML IDs)
        identifierSpan[0] = Chars[(int)((random1 >> 56) % 26)];

        // Remaining characters from random bits
        for (var i = 0; i < 4; i++)
        {
            identifierSpan[i + 1] = Chars[(int)((random1 >> (i * 8)) % CharsLength)];
        }
        for (var i = 4; i < 8; i++)
        {
            identifierSpan[i + 1] = Chars[(int)((random2 >> ((i - 4) * 8)) % CharsLength)];
        }

        return identifierSpan.ToString();
    }
}
