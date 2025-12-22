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
    private const int LettersCount = 26; // Number of letters (a-z) in Chars
    private const int RandomStringLength = 8;

    // Helper property to ensure CharsLength always matches the actual Chars string length
    private static int CharsLength => Chars.Length;

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

        // Generate random characters using bit shifting for performance
        // Each 64-bit integer provides up to 8 characters (8 bits per character)
        var charsGenerated = 0;
        while (charsGenerated < RandomStringLength)
        {
            var random = Random.Shared.NextInt64();
            var charsInThisBatch = Math.Min(8, RandomStringLength - charsGenerated);

            for (var i = 0; i < charsInThisBatch; i++)
            {
                identifierSpan[prefix.Length + charsGenerated + i] = GetCharFromRandomBits(random, i * 8);
            }

            charsGenerated += charsInThisBatch;
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

        // First character must be a letter for valid HTML IDs
        var random = Random.Shared.NextInt64();
        identifierSpan[0] = Chars[(int)((random >> 56) % LettersCount)];

        // Generate remaining characters using bit shifting for performance
        // Reuse unused bytes from the initial random value before requesting a new one
        var charsGenerated = 0;
        var nextByteIndex = 0; // Start consuming from the lowest byte (bits 0-7)
        var randomBytesAvailable = 7; // We already used the high byte (bits 56-63) for the first character

        while (charsGenerated < RandomStringLength)
        {
            if (randomBytesAvailable == 0)
            {
                random = Random.Shared.NextInt64();
                randomBytesAvailable = 8;
                nextByteIndex = 0;
            }

            identifierSpan[charsGenerated + 1] = GetCharFromRandomBits(random, nextByteIndex * 8);
            charsGenerated++;
            nextByteIndex++;
            randomBytesAvailable--;
        }

        return identifierSpan.ToString();
    }

    /// <summary>
    /// Extracts a character from random bits using bit-shifting.
    /// </summary>
    /// <param name="random">The random 64-bit integer.</param>
    /// <param name="bitShift">The number of bits to shift right (0, 8, 16, 24, etc.).</param>
    /// <returns>A character from the Chars set.</returns>
    private static char GetCharFromRandomBits(long random, int bitShift)
    {
        return Chars[(int)((random >> bitShift) % CharsLength)];
    }
}
