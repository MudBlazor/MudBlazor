// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
}
