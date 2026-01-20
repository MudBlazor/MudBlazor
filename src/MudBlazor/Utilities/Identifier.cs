// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

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
/// Performance is prioritized over perfect uniform distribution. The implementation uses modulo operations, 
/// which introduce a slight modulo bias so that some characters may appear marginally more frequently than others.
/// For the purposes of identifier generation, this bias is negligible.
/// </para>
/// </remarks>
public static class Identifier
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const int LettersCount = 26; // Number of letters (a-z) in Chars
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
    public static string Create(string prefix)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        return string.Create(prefix.Length + RandomStringLength, prefix, static (span, pfx) =>
        {
            pfx.CopyTo(span);

            var random = Random.Shared.NextInt64();
            var written = pfx.Length;
            var bitShift = 0;

            // Loop safely consumes 8 bytes of `random` at a time; the first char is from a separate high byte.
            // `bitShift > 56` triggers a new random value, so no bits are reused even for longer lengths.
            while (written < span.Length)
            {
                if (bitShift > 56)
                {
                    random = Random.Shared.NextInt64();
                    bitShift = 0;
                }

                span[written++] = GetCharFromRandomBits(random, bitShift);
                bitShift += 8;
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
            // First character: must be a letter (a–z)
            var random = Random.Shared.NextInt64();
            span[0] = Chars[(int)(((ulong)random >> 56) % LettersCount)];

            var written = 1;
            var bitShift = 0;

            // Loop safely consumes 8 bytes of `random` at a time; the first char is from a separate high byte.
            // `bitShift > 56` triggers a new random value, so no bits are reused even for longer lengths.
            while (written < span.Length)
            {
                if (bitShift > 56)
                {
                    random = Random.Shared.NextInt64();
                    bitShift = 0;
                }

                span[written++] = GetCharFromRandomBits(random, bitShift);

                bitShift += 8;
            }
        });
    }

    /// <summary>
    /// Extracts a character from random bits using bit-shifting.
    /// </summary>
    /// <param name="random">The random 64-bit integer.</param>
    /// <param name="bitShift">The number of bits to shift right (0, 8, 16, 24, etc.).</param>
    /// <returns>A character from the Chars set.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char GetCharFromRandomBits(long random, int bitShift)
    {
        unchecked
        {
            return Chars[(int)(((ulong)random >> bitShift) % (ulong)Chars.Length)];
        }
    }
}
