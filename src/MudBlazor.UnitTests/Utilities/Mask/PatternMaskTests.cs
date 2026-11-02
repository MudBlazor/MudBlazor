// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class PatternMaskTests
{
    [Test]
    public void PatternMask_Insert()
    {
        var mask = new PatternMask("(aa) 00-0");
        mask.ToString().Should().Be("|");
        mask.Insert("?");
        mask.ToString().Should().Be("|");
        mask.Insert("ab123");
        mask.Text.Should().Be("(ab) 12-3");
        mask.ToString().Should().Be("(ab) 12-3|");
        mask.CaretPos = 2;
        mask.ToString().Should().Be("(a|b) 12-3");
        mask.Insert("x");
        mask.ToString().Should().Be("(ax) |12-3");
        mask.Text.Should().Be("(ax) 12-3");
        mask.Insert("9");
        mask.ToString().Should().Be("(ax) 9|1-2");
        mask.Text.Should().Be("(ax) 91-2");
        mask.Insert("99");
        mask.ToString().Should().Be("(ax) 99-9|");
        mask.Text.Should().Be("(ax) 99-9");
        mask.Insert("xyz1234");
        mask.ToString().Should().Be("(ax) 99-9|");
        mask.Text.Should().Be("(ax) 99-9");
        mask.Clear();
        mask.ToString().Should().Be("|");
        mask.Text.Should().Be("");
        mask.Insert("1");
        mask.ToString().Should().Be("|");
        mask.Text.Should().Be("");
        mask.Insert("x");
        mask.ToString().Should().Be("(x|");
        mask.Text.Should().Be("(x");
        mask.Insert("y");
        mask.ToString().Should().Be("(xy) |");
        mask.Text.Should().Be("(xy) ");
        mask.Insert("z");
        mask.ToString().Should().Be("(xy) |");
        mask.Text.Should().Be("(xy) ");
        // paste
        mask.Clear();
        mask.Insert("(XX) 99-9");
        mask.ToString().Should().Be("(XX) 99-9|");
    }

    [Test]
    public void PatternMask_AutoFilling()
    {
        var mask = new PatternMask("---0---");
        mask.ToString().Should().Be("|");
        mask.Insert("1");
        mask.Text.Should().Be("---1---");
        mask.ToString().Should().Be("---1---|");
        mask.CaretPos = 1;
        mask.ToString().Should().Be("-|--1---");
        mask.Insert("x");
        mask.Text.Should().Be("---1---");
        mask.ToString().Should().Be("---|1---");
        mask.Insert("9");
        mask.Text.Should().Be("---9---");
        mask.ToString().Should().Be("---9---|");
    }

    [Test]
    public void PatternMask_Placeholder()
    {
        var mask = new PatternMask("(+00) 000 0000") { Placeholder = '_' };
        mask.ToString().Should().Be("|");
        mask.Text.Should().BeNullOrEmpty();
        mask.Insert("x");
        mask.ToString().Should().Be("|");
        mask.Text.Should().Be("");
        mask.Clear();
        mask.Text.Should().BeNullOrEmpty();
        mask.ToString().Should().Be("|");
        mask.Insert("43");
        mask.Text.Should().Be("(+43) ___ ____");
        mask.ToString().Should().Be("(+43) |___ ____");
        mask.Insert("abc123");
        mask.ToString().Should().Be("(+43) 123 |____");
        mask.Insert("5678901234");
        mask.ToString().Should().Be("(+43) 123 5678|");
        // del key
        mask.Delete();
        mask.ToString().Should().Be("(+43) 123 5678|");
        mask.CaretPos = 0;
        mask.ToString().Should().Be("|(+43) 123 5678");
        mask.Delete();
        mask.ToString().Should().Be("(+|31) 235 678_");
        mask.Delete();
        mask.ToString().Should().Be("(+|12) 356 78__");
        mask.Insert("430");
        mask.ToString().Should().Be("(+43) 0|12 3567");
    }

    [Test]
    public void PatternMask_CleaningPlaceholder()
    {
        var mask = new PatternMask("(+00) 000 0000") { Placeholder = '_' };
        mask.Insert("x");
        mask.ToString().Should().Be("|");
        mask.Text.Should().Be("");
        mask.GetCleanText().Should().Be("");
        mask.Insert("123456789");
        mask.Text.Should().Be("(+12) 345 6789");
        mask.GetCleanText().Should().Be("(+12) 345 6789");
        mask.Clear();
        mask.CleanDelimiters = true;
        mask.GetCleanText().Should().Be("");
        mask.Insert("123456789");
        mask.Text.Should().Be("(+12) 345 6789");
        mask.GetCleanText().Should().Be("123456789");
    }

    [Test]
    public void PatternMask_Delete()
    {
        var mask = new PatternMask("(+00) 000 0000"); // no placeholder
        mask.ToString().Should().Be("|");
        mask.Insert("43");
        mask.Text.Should().Be("(+43) ");
        mask.ToString().Should().Be("(+43) |");
        mask.Insert("abc123");
        mask.ToString().Should().Be("(+43) 123 |");
        mask.Insert("5678901234");
        mask.ToString().Should().Be("(+43) 123 5678|");
        // del key
        mask.Delete();
        mask.ToString().Should().Be("(+43) 123 5678|");
        mask.CaretPos = 0;
        mask.ToString().Should().Be("|(+43) 123 5678");
        mask.Delete();
        mask.ToString().Should().Be("(+|31) 235 678");
        mask.Delete();
        mask.ToString().Should().Be("(+|12) 356 78");
        mask.Insert("430");
        mask.ToString().Should().Be("(+43) 0|12 3567");
        mask.Selection = (2, 77);
        mask.ToString().Should().Be("(+[43) 012 3567]");
        mask.Delete();
        mask.ToString().Should().Be("|");
        mask.Text.Should().Be("");
        mask.GetCleanText().Should().Be("");
    }

    [Test]
    public void PatternMask_Backspace()
    {
        var mask = new PatternMask("(+00) 000 0000"); // no placeholder
        mask.ToString().Should().Be("|");
        mask.Insert("43abc1235678901234");
        mask.ToString().Should().Be("(+43) 123 5678|");
        // Backspace key
        mask.Backspace();
        mask.ToString().Should().Be("(+43) 123 567|");
        mask.CaretPos = 0;
        mask.ToString().Should().Be("|(+43) 123 567");
        mask.Backspace();
        mask.ToString().Should().Be("|(+43) 123 567");
        mask.CaretPos = 6;
        mask.ToString().Should().Be("(+43) |123 567");
        mask.Backspace();
        mask.ToString().Should().Be("(+4|1) 235 67");
        mask.Backspace();
        mask.ToString().Should().Be("(+|12) 356 7");
        mask.Backspace();
        mask.ToString().Should().Be("|(+12) 356 7");
        mask.Insert("4309");
        mask.ToString().Should().Be("(+43) 09|1 2356");
    }

    [Test]
    public void PatternMask_Selection()
    {
        var mask = new PatternMask("(+00) 000 0000"); // no placeholder
        mask.ToString().Should().Be("|");
        mask.Insert("43abc1235678901234");
        mask.ToString().Should().Be("(+43) 123 5678|");
        // set selection
        mask.Selection = (-1, 111);
        mask.ToString().Should().Be("[(+43) 123 5678]");
        mask.CaretPos = 0;
        mask.Selection = (1, 1);
        mask.ToString().Should().Be("(|+43) 123 5678");
        mask.Selection = (3, 11);
        mask.ToString().Should().Be("(+4[3) 123 5]678");
        // input with selection
        mask.Insert("9");
        mask.ToString().Should().Be("(+49) |678 ");
        mask.Selection = (0, 6);
        mask.ToString().Should().Be("[(+49) ]678 ");
        mask.Insert("01");
        mask.ToString().Should().Be("(+01) |678 ");
        // del with selection
        mask.Selection = (0, 6);
        mask.ToString().Should().Be("[(+01) ]678 ");
        mask.Delete();
        mask.ToString().Should().Be("|(+67) 8");
        // backspace with selection
        mask.Selection = (0, 6);
        mask.ToString().Should().Be("[(+67) ]8");
        mask.Backspace();
        mask.ToString().Should().Be("|(+8");
        mask = new PatternMask("00 00") { Placeholder = '_' };
        mask.Insert("1234");
        mask.ToString().Should().Be("12 34|");
        mask.Backspace();
        mask.ToString().Should().Be("12 3|_");
        mask.Selection = (0, 2);
        mask.ToString().Should().Be("[12] 3_");
        mask.Backspace();
        mask.ToString().Should().Be("|3_ __");
    }

    [Test]
    public void PatternMask_ChangeMaskChars()
    {
        var mask = new PatternMask("(bb+) 999-bb")
        {
            MaskChars = [MaskChar.Letter('b'), MaskChar.Digit('9'), MaskChar.LetterOrDigit('+')],
        };
        mask.Insert("xyz");
        mask.ToString().Should().Be("(xyz) |");
        mask.Backspace();
        mask.ToString().Should().Be("(xy|");
        mask.Insert("1234");
        mask.ToString().Should().Be("(xy1) 234-|");
    }

    [Test]
    public void PatternMask_TransformationFunc()
    {
        var mask = new PatternMask("(aaa) 000")
        {
            Transformation = c => c.ToString().ToUpperInvariant()[0],
            CleanDelimiters = true,
        };
        mask.Insert("xyä123");
        mask.ToString().Should().Be("(XYÄ) 123|");
        mask.GetCleanText().Should().Be("XYÄ123");
        mask.SetText("ABß...");
        mask.ToString().Should().Be("(ABß) |");
        mask.GetCleanText().Should().Be("ABß");
    }

    [Test]
    public void PatternMask_UpdateFrom()
    {
        var mask = new PatternMask("(aaa) 000");
        mask.MaskChars.Length.Should().Be(3); // '0', 'a' and '*'
        mask.CleanDelimiters.Should().BeFalse();
        mask.Placeholder.Should().BeNull();
        mask.SetText("abc12");
        mask.Selection = (1, 2);
        mask.ToString().Should().Be("([a]bc) 12");
        mask.UpdateFrom(new PatternMask("999") { Placeholder = '#', MaskChars = [new MaskChar('9', "[0-9]")], CleanDelimiters = true });
        mask.MaskChars.Length.Should().Be(1); // '9'
        mask.MaskChars[0].Char.Should().Be('9');
        mask.MaskChars[0].Regex.Should().Be("[0-9]");
        mask.CleanDelimiters.Should().BeTrue();
        mask.Placeholder.Should().Be('#');
        // state should be preserved (Text, Caret/Selection)
        mask.ToString().Should().Be("1[2]#");
        mask.UpdateFrom(null);
        mask.MaskChars.Length.Should().Be(1); // '9'
        mask.MaskChars[0].Char.Should().Be('9');
        mask.MaskChars[0].Regex.Should().Be("[0-9]");
        mask.CleanDelimiters.Should().BeTrue();
        mask.Placeholder.Should().Be('#');
        // state should be preserved (Text, Caret/Selection)
        mask.ToString().Should().Be("1[2]#");
    }

    [Test]
    public void PatternMask_AlignAgainstMask_SimpleCase()
    {
        // Arrange
        var mask = new PatternMask("00-00");

        // Act
        mask.Insert("1234");

        // Assert
        mask.Text.Should().Be("12-34");
    }

    [Test]
    public void PatternMask_AlignAgainstMask_SkipDelimiters()
    {
        // Arrange
        var mask = new PatternMask("00-00");

        // Act
        mask.Insert("12-34");

        // Assert
        mask.Text.Should().Be("12-34");
    }

    [Test]
    public void PatternMask_AlignAgainstMask_ExtraCharacters()
    {
        // Arrange
        var mask = new PatternMask("000");

        // Act
        mask.Insert("12345");

        // Assert
        mask.Text.Should().Be("123");
    }

    [Test]
    public void PatternMask_IsMatch_ValidDigit()
    {
        // Arrange
        var mask = new PatternMask("000");

        // Act
        mask.Insert("1");

        // Assert
        mask.Text.Should().Be("1");
    }

    [Test]
    public void PatternMask_IsMatch_InvalidCharacter()
    {
        // Arrange
        var mask = new PatternMask("000");

        // Act
        mask.Insert("a");

        // Assert
        mask.Text.Should().BeEmpty();
    }

    [Test]
    public void PatternMask_FillWithPlaceholder_FullMask()
    {
        // Arrange
        var mask = new PatternMask("000-000") { Placeholder = '_' };

        // Act
        mask.Insert("123");

        // Assert
        mask.Text.Should().Be("123-___");
    }

    [Test]
    public void PatternMask_FillWithPlaceholder_EmptyText()
    {
        // Arrange
        var mask = new PatternMask("000") { Placeholder = '_' };

        // Act - no insert

        // Assert
        mask.Text.Should().BeNullOrEmpty();
    }

    [Test]
    public void PatternMask_FillWithPlaceholder_CompleteMask()
    {
        // Arrange
        var mask = new PatternMask("000") { Placeholder = '_' };

        // Act
        mask.Insert("123");

        // Assert
        mask.Text.Should().Be("123");
    }

    [Test]
    public void PatternMask_Placeholder_InMiddleOfMask()
    {
        // Arrange
        var mask = new PatternMask("00-00") { Placeholder = '_' };
        mask.Insert("12");
        mask.CaretPos = 3;

        // Act
        mask.Insert("3");

        // Assert
        mask.Text.Should().Be("12-3_");
    }

    [Test]
    public void PatternMask_CleanDelimiters_True()
    {
        // Arrange
        var mask = new PatternMask("00-00") { CleanDelimiters = true };
        mask.Insert("1234");

        // Act
        var cleanText = mask.GetCleanText();

        // Assert
        cleanText.Should().Be("1234");
    }

    [Test]
    public void PatternMask_CleanDelimiters_False()
    {
        // Arrange
        var mask = new PatternMask("00-00") { CleanDelimiters = false };
        mask.Insert("1234");

        // Act
        var cleanText = mask.GetCleanText();

        // Assert
        cleanText.Should().Be("12-34");
    }

    [Test]
    public void PatternMask_EmptyMask()
    {
        // Arrange
        var mask = new PatternMask("");

        // Act
        mask.Insert("123");

        // Assert
        mask.Text.Should().BeEmpty();
    }

    [Test]
    public void PatternMask_OnlyDelimiters()
    {
        // Arrange
        var mask = new PatternMask("---");

        // Act
        mask.Insert("123");

        // Assert
        mask.Text.Should().BeEmpty();
    }

    [Test]
    public void PatternMask_MultipleConsecutiveDelimiters()
    {
        // Arrange
        var mask = new PatternMask("0---0");

        // Act
        mask.Insert("12");

        // Assert
        mask.Text.Should().Be("1---2");
    }

    [Test]
    public void PatternMask_ChangeMaskChars_ForcesReinitialization()
    {
        // Arrange - default 'a' would accept letters
        var mask = new PatternMask("aa");
        mask.Insert("xy");
        mask.Text.Should().Be("xy");

        // Act - swap mask chars so 'a' now means digit, then re-evaluate
        mask.MaskChars = [MaskChar.Digit('a')];
        mask.SetText("xy");

        // Assert - letters no longer accepted after reinitialization
        mask.Text.Should().BeEmpty();
        mask.SetText("12");
        mask.Text.Should().Be("12");
    }

    [Test]
    public void PatternMask_UpdateFrom_ChangesMaskAndPreservesAlignedText()
    {
        // Arrange
        var mask = new PatternMask("000-000");
        mask.SetText("123456");
        mask.Text.Should().Be("123-456");

        // Act - widen the mask; existing text is re-aligned against the new mask
        mask.UpdateFrom(new PatternMask("00-00-00"));

        // Assert
        mask.Text.Should().Be("12-34-56");
    }

}
