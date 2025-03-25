// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// A character which represents a set of allowed values in a <see cref="MudMask"/>.
/// </summary>
/// <remarks>
/// Masks are built from mask characters, which each represent a regular expression for that character.<br />
/// For example: the mask character <c>0</c> with regular expression <c>\d</c> would allow any digit for that character.
/// </remarks>
public struct MaskChar
{
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
    }

    /// <summary>
    /// The character to use in the mask.
    /// </summary>
    public char Char { get; set; }

    /// <summary>
    /// The regular expression defining allowed characters.
    /// </summary>
    public string Regex { get; set; }

    /// <summary>
    /// Gets a mask character which allows any letter (uppercase or lowercase).
    /// </summary>
    /// <param name="c">The mask character to create.</param>
    /// <returns>A character with a regular expression of <c>\p{L}</c> for any letter.</returns>
    public static MaskChar Letter(char c) => new MaskChar { Char = c, Regex = @"\p{L}" };

    /// <summary>
    /// Gets a mask character which allows any digit.
    /// </summary>
    /// <param name="c">The mask character to create.</param>
    /// <returns>A character with a regular expression of <c>\d</c> for any digit.</returns>
    public static MaskChar Digit(char c) => new MaskChar { Char = c, Regex = @"\d" };

    /// <summary>
    /// Gets a mask character which allows any letter (uppercase or lowercase) or any digit.
    /// </summary>
    /// <param name="c">The mask character to create.</param>
    /// <returns>A character with a regular expression of <c>\p{L}|\d</c> for any letter or digit.</returns>
    public static MaskChar LetterOrDigit(char c) => new MaskChar { Char = c, Regex = @"\p{L}|\d" };
}
