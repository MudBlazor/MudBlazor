// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class MaskCharTests
{
    [Test]
    public void MaskChar_Letter_CreatesCorrectRegex()
    {
        // Arrange & Act
        var maskChar = MaskChar.Letter('a');

        // Assert
        maskChar.Char.Should().Be('a');
        maskChar.Regex.Should().Be(@"\p{L}");
    }

    [Test]
    public void MaskChar_Digit_CreatesCorrectRegex()
    {
        // Arrange & Act
        var maskChar = MaskChar.Digit('0');

        // Assert
        maskChar.Char.Should().Be('0');
        maskChar.Regex.Should().Be(@"\d");
    }

    [Test]
    public void MaskChar_LetterOrDigit_CreatesCorrectRegex()
    {
        // Arrange & Act
        var maskChar = MaskChar.LetterOrDigit('*');

        // Assert
        maskChar.Char.Should().Be('*');
        maskChar.Regex.Should().Be(@"\p{L}|\d");
    }

    [Test]
    public void MaskChar_Equality_SameCharAndRegex()
    {
        // Arrange
        var maskChar1 = new MaskChar('x', @"\d");
        var maskChar2 = new MaskChar('x', @"\d");

        // Act & Assert
        maskChar1.Equals(maskChar2).Should().BeTrue();
    }

    [Test]
    public void MaskChar_Inequality_DifferentChar()
    {
        // Arrange
        var maskChar1 = new MaskChar('x', @"\d");
        var maskChar2 = new MaskChar('y', @"\d");

        // Act & Assert
        maskChar1.Equals(maskChar2).Should().BeFalse();
    }

    [Test]
    public void MaskChar_Inequality_DifferentRegex()
    {
        // Arrange
        var maskChar1 = new MaskChar('x', @"\d");
        var maskChar2 = new MaskChar('x', @"\w");

        // Act & Assert
        maskChar1.Equals(maskChar2).Should().BeFalse();
    }

    [Test]
    public void MaskChar_EqualsObject_DifferentType_ReturnsFalse()
    {
        // Arrange
        var maskChar = new MaskChar('x', @"\d");

        // Act & Assert
        maskChar.Equals("x").Should().BeFalse();
    }

    [Test]
    public void MaskChar_OperatorEquals_SameValue_ReturnsTrue()
    {
        // Arrange
        var maskChar1 = new MaskChar('x', @"\d");
        var maskChar2 = new MaskChar('x', @"\d");

        // Act & Assert
        (maskChar1 == maskChar2).Should().BeTrue();
        (maskChar1 != maskChar2).Should().BeFalse();
    }

    /// <summary>
    /// Built-in mask characters accept exactly the characters their regular expression accepts, without using the regex engine.
    /// </summary>
    [Test]
    public void BuiltInMaskChars_MatchExactlyWhatTheirExpressionMatches()
    {
        foreach (var maskChar in new[] { MaskChar.Letter('a'), MaskChar.Digit('0'), MaskChar.LetterOrDigit('*') })
        {
            for (var c = char.MinValue; ; c++)
            {
                maskChar.IsMatch(c).Should().Be(Regex.IsMatch(c.ToString(), maskChar.Regex), $"'{maskChar.Regex}' on U+{(int)c:X4}");
                if (c == char.MaxValue)
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// A mask character created with a custom expression still matches through that expression.
    /// </summary>
    [Test]
    public void CustomMaskChar_MatchesThroughItsExpression()
    {
        var maskChar = new MaskChar('x', "[a-c]");

        maskChar.IsMatch('b').Should().BeTrue();
        maskChar.IsMatch('d').Should().BeFalse();
    }

    /// <summary>
    /// A default mask character has no expression and throws the same exception type the regex engine threw.
    /// </summary>
    [Test]
    public void DefaultMaskChar_IsMatch_ThrowsArgumentNullException()
    {
        var maskChar = default(MaskChar);

        maskChar.Invoking(x => x.IsMatch('a')).Should().Throw<ArgumentNullException>();
    }
}
