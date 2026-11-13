// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// A character which represents a set of allowed values in a <see cref="MudMask"/>.
/// </summary>
/// <remarks>
/// Masks are built from mask characters, which each represent a regular expression for that character.<br />
/// For example: the mask character <c>0</c> with regular expression <c>\d</c> would allow any digit for that character.
/// </remarks>
public readonly struct MaskChar : IEquatable<MaskChar>
{
    // Only a custom expression from the public constructor reaches the regular expression engine.
    // The built-in characters test the character directly, so a trimmed app that never creates a custom mask character can leave the engine out of its download.
    private readonly Func<char, bool>? _isMatch;

    /// <summary>
    /// Creates a new mask character with its associated regular expression.
    /// </summary>
    /// <param name="c">The character to use in the mask.</param>
    /// <param name="regex">The regular expression for allowed values for this character.</param>
    /// <remarks>
    /// For example: Given a mask character of <c>0</c> and expression of <c>\d</c>, a mask of <c>000-00-0000</c> would allow any digit for the <c>0</c> character.
    /// </remarks>
    public MaskChar(char c, string regex)
    {
        Char = c;
        Regex = regex;
        _isMatch = textChar => System.Text.RegularExpressions.Regex.IsMatch(textChar.ToString(), regex, RegexOptions.None, RegexDefaults.MatchTimeout);
    }

    private MaskChar(char c, string regex, Func<char, bool> isMatch)
    {
        Char = c;
        Regex = regex;
        _isMatch = isMatch;
    }

    /// <summary>
    /// The character to use in the mask.
    /// </summary>
    public char Char { get; }

    /// <summary>
    /// The regular expression defining allowed characters.
    /// </summary>
    public string Regex { get; }

    /// <summary>
    /// Gets a mask character which allows any letter (uppercase or lowercase).
    /// </summary>
    /// <param name="c">The mask character to create.</param>
    /// <returns>A character with a regular expression of <c>\p{L}</c> for any letter.</returns>
    public static MaskChar Letter(char c) => new(c, @"\p{L}", char.IsLetter);

    /// <summary>
    /// Gets a mask character which allows any digit.
    /// </summary>
    /// <param name="c">The mask character to create.</param>
    /// <returns>A character with a regular expression of <c>\d</c> for any digit.</returns>
    public static MaskChar Digit(char c) => new(c, @"\d", char.IsDigit);

    /// <summary>
    /// Gets a mask character which allows any letter (uppercase or lowercase) or any digit.
    /// </summary>
    /// <param name="c">The mask character to create.</param>
    /// <returns>A character with a regular expression of <c>\p{L}|\d</c> for any letter or digit.</returns>
    public static MaskChar LetterOrDigit(char c) => new(c, @"\p{L}|\d", char.IsLetterOrDigit);

    /// <summary>
    /// Gets whether a typed character is allowed for this mask character.
    /// </summary>
    /// <param name="textChar">The typed character.</param>
    /// <returns><c>true</c> when the character matches <see cref="Regex"/>.</returns>
    internal bool IsMatch(char textChar)
    {
        // Only a default instance has no expression, and the engine rejected that with the same exception type.
        ArgumentNullException.ThrowIfNull(Regex);
        return _isMatch!(textChar);
    }

    /// <inheritdoc />
    public bool Equals(MaskChar other) => Char == other.Char && Regex == other.Regex;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is MaskChar other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Char, Regex);

    /// <summary>
    /// Determines whether two <see cref="MaskChar"/> instances are equal.
    /// </summary>
    public static bool operator ==(MaskChar left, MaskChar right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="MaskChar"/> instances are not equal.
    /// </summary>
    public static bool operator !=(MaskChar left, MaskChar right) => !left.Equals(right);
}
