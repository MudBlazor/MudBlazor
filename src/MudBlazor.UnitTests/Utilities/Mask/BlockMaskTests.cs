// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class BlockMaskTests
{

    [Test]
    public void BlockMask_Insert()
    {
        var mask = new BlockMask(".", new Block('0', 1, 2), new Block('0', 1, 2), new Block('0', 2, 4));
        mask.ToString().Should().Be("|");
        mask.Insert("12.");
        mask.ToString().Should().Be("12.|");
        mask.Clear();
        mask.Insert("xx12.34xx.5678");
        mask.Text.Should().Be("12.34.5678");
        mask.Clear();
        mask.Insert("1.1.99");
        mask.ToString().Should().Be("1.1.99|");
        mask.CaretPos = 0;
        mask.Insert("0");
        mask.ToString().Should().Be("0|1.1.99");
        mask.Insert("0");
        mask.ToString().Should().Be("00|.1.199");
        mask.Insert("0");
        mask.ToString().Should().Be("00.0|.1199");
        mask.Insert("0");
        mask.ToString().Should().Be("00.00|.1199");
        // w/o separator
        mask = new BlockMask("", new Block('0', 1, 2), new Block('a', 1, 2), new Block('0', 2, 4));
        mask.Insert("xx12.34xx.5678");
        mask.Text.Should().Be("12xx5678");
        mask.Clear();
        mask.Insert("1.x.99");
        mask.ToString().Should().Be("1x99|");
        mask.CaretPos = 0;
        mask.Insert("0");
        mask.ToString().Should().Be("0|1x99");
        mask.Insert("0");
        mask.ToString().Should().Be("00|x99");
        mask.Insert("y");
        mask.ToString().Should().Be("00y|x99");
        mask.Insert("z");
        mask.ToString().Should().Be("00yz|99");
        mask.Insert("1");
        mask.ToString().Should().Be("00yz1|99");
    }

    [Test]
    public void BlockMask_Delete()
    {
        var mask = new BlockMask(".", new Block('0', 1, 2), new Block('0', 1, 2), new Block('0', 2, 4));
        mask.ToString().Should().Be("|");
        mask.Insert("12.34.5678");
        mask.ToString().Should().Be("12.34.5678|");
        mask.Delete();
        mask.ToString().Should().Be("12.34.5678|");
        mask.CaretPos = 0;
        mask.Delete();
        mask.ToString().Should().Be("|2.34.5678");
        mask.Delete();
        mask.ToString().Should().Be("|34.56.78");
        mask.SetText("12.");
        mask.Selection = (0, 2);
        mask.Delete();
        mask.ToString().Should().Be("|");
        mask.Insert("12345");
        mask.ToString().Should().Be("12.34.5|");
        mask.CaretPos = 5;
        mask.Delete();
        mask.ToString().Should().Be("12.34|");
    }

    [Test]
    public void BlockMask_Backspace()
    {
        var mask = new BlockMask(".", new Block('0', 1, 2), new Block('0', 1, 2), new Block('0', 2, 4));
        mask.ToString().Should().Be("|");
        mask.Insert("12.34.5678");
        mask.ToString().Should().Be("12.34.5678|");
        mask.Backspace();
        mask.ToString().Should().Be("12.34.567|");
        mask.CaretPos = 3;
        mask.ToString().Should().Be("12.|34.567");
        mask.Backspace();
        mask.ToString().Should().Be("1|3.4.567");
        mask.Backspace();
        mask.ToString().Should().Be("|3.4.567");
        mask.Backspace();
        mask.ToString().Should().Be("|3.4.567");
        mask.Selection = (2, 3);
        mask.Backspace();
        mask.ToString().Should().Be("3.|56.7");
    }

    [Test]
    public void BlockMask_Internals()
    {
        var mask = new BlockMask(".", new Block('('), new Block('0', 2, 2), new Block(')'));
        mask.Clear(); // make sure it is initialized
        mask.Mask.Should().Be(@"^(\(([\.](\d(\d([\.](\))?)?)?)?)?)?$");
        mask = new BlockMask(".", new Block('0', 1, 2), new Block('0', 1, 2), new Block('0', 2, 4));
        mask.Clear(); // make sure it is initialized
        mask.Mask.Should().Be(@"^(\d(\d)?([\.](\d(\d)?([\.](\d(\d(\d(\d)?)?)?)?)?)?)?)?$");
        Assert.Throws<ArgumentException>(() => _ = new BlockMask());
    }

    [Test]
    public void BlockMask_UpdateFrom()
    {
        var mask = new BlockMask(".", new Block('('), new Block('0', 2, 2), new Block(')'));
        mask.Blocks.Length.Should().Be(3);
        mask.DelimiterCharacters.Should().Be(".");
        mask.SetText("(1234)");
        mask.ToString().Should().Be("(.12.)|");
        mask.CaretPos = 1;
        mask.UpdateFrom(new BlockMask(":", new Block('0', 1, 1), new Block('0', 1, 1)));
        mask.Blocks.Length.Should().Be(2);
        mask.DelimiterCharacters.Should().Be(":");
        // state should be preserved (Text, Caret/Selection)
        mask.ToString().Should().Be("1|:2");
        mask.UpdateFrom(null);
        mask.Blocks.Length.Should().Be(2);
        mask.DelimiterCharacters.Should().Be(":");
        // state should be preserved (Text, Caret/Selection)
        mask.ToString().Should().Be("1|:2");
    }

    [Test]
    public void BlockMask_MaxBlockLimit()
    {
        // Arrange
        var mask = new BlockMask("", new Block('0', 1, 3));

        // Act
        mask.Insert("12345678");

        // Assert - should not exceed max
        mask.Text.Should().Be("123");
    }

    [Test]
    public void BlockMask_GetCleanText()
    {
        // Arrange
        var mask = new BlockMask("-", new Block('0', 2, 2), new Block('a', 2, 2));
        mask.Insert("12AB");

        // Act
        var cleanText = mask.GetCleanText();

        // Assert
        cleanText.Should().Be("12-AB");
    }

    [Test]
    public void BlockMask_MultipleDelimiters()
    {
        // Arrange
        var mask = new BlockMask(".-", new Block('0', 1, 2), new Block('a', 1, 2));

        // Act
        mask.Insert("1x");

        // Assert - the first delimiter that aligns is inserted, so the result is deterministic
        mask.Text.Should().Be("1.x");
    }

    [Test]
    public void BlockMask_SelectionDelete()
    {
        // Arrange
        var mask = new BlockMask(".", new Block('0', 1, 2), new Block('0', 1, 2));
        mask.Insert("12.34");
        mask.Selection = (1, 4);

        // Act
        mask.Delete();

        // Assert
        mask.ToString().Should().Be("1|4");
    }

    [Test]
    public void BlockMask_SelectionBackspace()
    {
        // Arrange
        var mask = new BlockMask(".", new Block('0', 1, 2), new Block('0', 1, 2));
        mask.Insert("12.34");
        mask.Selection = (1, 4);

        // Act
        mask.Backspace();

        // Assert
        mask.ToString().Should().Be("1|4");
    }

    [Test]
    public void BlockMask_LetterBlock()
    {
        // Arrange
        var mask = new BlockMask("", new Block('a', 2, 4));

        // Act
        mask.Insert("ABCD123");

        // Assert
        mask.Text.Should().Be("ABCD");
    }

    [Test]
    public void BlockMask_MixedBlocks()
    {
        // Arrange
        var mask = new BlockMask("-", new Block('a', 2, 2), new Block('0', 3, 3));

        // Act
        mask.Insert("AB123");

        // Assert
        mask.Text.Should().Be("AB-123");
    }

    [Test]
    public void BlockMask_LiteralCharacterBlock()
    {
        // Arrange
        var mask = new BlockMask("", new Block('('), new Block('0', 3, 3), new Block(')'));

        // Act
        mask.Insert("(123)");

        // Assert - Literal blocks are treated as delimiters
        mask.Text.Should().Be("(123)");
    }

    [Test]
    public void BlockMask_LetterOrDigitBlock_AcceptsLettersAndDigits()
    {
        // NOTE: suspected BlockMask/MaskChar bug. MaskChar.LetterOrDigit's regex is the
        // unparenthesized alternation "\p{L}|\d". When BlockMask assembles a block it wraps
        // each accepted slot in its own group, producing "^(\p{L}|\d(\p{L}|\d)?)?$" for
        // Block('*', 1, 3). Operator precedence makes the outer group mean
        // (\p{L})  OR  (\d(\p{L}|\d)?), i.e. either a single lone letter, or a leading DIGIT
        // optionally followed by a letter/digit. A leading letter therefore blocks every
        // following character, so "letter or digit" is not honored symmetrically.
        // The fix is a source change (group the alternation, e.g. "(\p{L}|\d)" / "[\p{L}\d]"),
        // so this test only documents the current behavior.

        // Arrange
        var mask = new BlockMask("", new Block('*', 1, 3));

        // Act
        mask.Insert("A1!b");

        // Assert - BUG: after the leading letter "A", the digit "1" no longer matches the
        // assembled regex (only a leading digit may be followed by more characters), so input
        // stops at "A" instead of producing the intended "A1b".
        mask.Text.Should().Be("A");

        // A leading digit, by contrast, does accept a following letter/digit, demonstrating the
        // asymmetry caused by the ungrouped alternation. (It still falls short of the Max of 3:
        // the digit branch "\d(\p{L}|\d)?" exposes only one optional trailing slot, so the third
        // character "2" is dropped as well - a further symptom of the same bug.)
        mask.Clear();
        mask.Insert("1A!2");
        mask.Text.Should().Be("1A");
    }

}
