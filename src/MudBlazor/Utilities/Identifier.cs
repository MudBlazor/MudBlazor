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
/// <para>
/// This class uses optimized algorithms to generate identifiers quickly while maintaining uniqueness.
/// Identifiers consist of lowercase letters and digits from the set [a-z0-9].
/// </para>
/// <para>
/// Performance is prioritized over perfect uniform distribution. The implementation uses modulo operations 
/// which introduce a small bias (~0.39%) where the first 4 characters (a-d) appear slightly more frequently 
/// than the remaining 32 characters. This bias is negligible for identifier generation purposes.
/// </para>
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
        var prefixStr = prefix.ToString();
        return string.Create(prefixStr.Length + RandomStringLength, prefixStr, static (span, pfx) =>
        {
            pfx.AsSpan().CopyTo(span);
            
            // Generate random characters using bit shifting for performance
            // Each 64-bit integer provides up to 8 characters (1 byte per character)
            var charsGenerated = 0;
            while (charsGenerated < RandomStringLength)
            {
                var random = Random.Shared.NextInt64();
                var charsInThisBatch = Math.Min(8, RandomStringLength - charsGenerated);

                for (var i = 0; i < charsInThisBatch; i++)
                {
                    span[pfx.Length + charsGenerated + i] = GetCharFromRandomBits(random, i * 8);
                }

                charsGenerated += charsInThisBatch;
            }
        });
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
        return string.Create(RandomStringLength + 1, 0, static (span, _) =>
        {
            // Use two random values for optimal performance with RandomStringLength = 8
            // First random: provides the letter prefix (byte 7) + next 7 characters (bytes 0-6)
            // Second random: provides the final character (byte 0)
            var random1 = Random.Shared.NextInt64();
            var random2 = Random.Shared.NextInt64();

            // First character must be a letter for valid HTML IDs (use high byte of first random)
            span[0] = Chars[(int)((random1 >> 56) % LettersCount)];

            // Next 7 characters from the remaining bytes (bits 0-55) of first random
            for (var i = 0; i < 7; i++)
            {
                span[i + 1] = GetCharFromRandomBits(random1, i * 8);
            }

            // Final character from second random value
            span[RandomStringLength] = GetCharFromRandomBits(random2, 0);
        });
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
