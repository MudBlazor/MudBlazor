// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class BaseMaskTests
{
    [Test]
    public void BaseMask_SplitAt_PositiveIndex()
    {
        // Arrange & Act
        var (left, right) = BaseMask.SplitAt("asdf", 1);

        // Assert
        left.Should().Be("a");
        right.Should().Be("sdf");
    }

    [Test]
    public void BaseMask_SplitAt_EmptyString()
    {
        // Arrange & Act
        var (left, right) = BaseMask.SplitAt("", 1);

        // Assert
        left.Should().Be("");
        right.Should().Be("");
    }

    [Test]
    public void BaseMask_SplitAt_NegativeIndex()
    {
        // Arrange & Act
        var (left, right) = BaseMask.SplitAt("asdf", -1);

        // Assert
        left.Should().Be("");
        right.Should().Be("asdf");
    }

    [Test]
    public void BaseMask_SplitAt_IndexBeyondLength()
    {
        // Arrange & Act
        var (left, right) = BaseMask.SplitAt("asdf", 10);

        // Assert
        left.Should().Be("asdf");
        right.Should().Be("");
    }

    [Test]
    public void BaseMask_SplitAt_ZeroIndex()
    {
        // Arrange & Act
        var (left, right) = BaseMask.SplitAt("asdf", 0);

        // Assert
        left.Should().Be("");
        right.Should().Be("asdf");
    }

    [Test]
    public void BaseMask_SplitAt_NullString()
    {
        // Arrange & Act
        var (left, right) = BaseMask.SplitAt(null, 2);

        // Assert
        left.Should().Be("");
        right.Should().Be("");
    }

    [Test]
    public void BaseMask_SplitAt_AtEnd()
    {
        // Arrange & Act
        var (left, right) = BaseMask.SplitAt("test", 4);

        // Assert
        left.Should().Be("test");
        right.Should().Be("");
    }

    [Test]
    public void BaseMask_SplitSelection_ValidSelection()
    {
        // Arrange & Act
        var (before, selected, after) = BaseMask.SplitSelection("hello world", (6, 11));

        // Assert
        before.Should().Be("hello ");
        selected.Should().Be("world");
        after.Should().Be("");
    }

    [Test]
    public void BaseMask_SplitSelection_FullText()
    {
        // Arrange & Act
        var (before, selected, after) = BaseMask.SplitSelection("test", (0, 4));

        // Assert
        before.Should().Be("");
        selected.Should().Be("test");
        after.Should().Be("");
    }

    [Test]
    public void BaseMask_SplitSelection_EmptySelection()
    {
        // Arrange & Act
        var (before, selected, after) = BaseMask.SplitSelection("test", (2, 2));

        // Assert
        before.Should().Be("te");
        selected.Should().Be("");
        after.Should().Be("st");
    }

    [Test]
    public void BaseMask_SplitSelection_NullText()
    {
        // Arrange & Act
        var (before, selected, after) = BaseMask.SplitSelection(null, (0, 5));

        // Assert
        before.Should().Be("");
        selected.Should().Be("");
        after.Should().Be("");
    }

    [Test]
    public void BaseMask_SplitSelection_BeyondLength()
    {
        // Arrange & Act
        var (before, selected, after) = BaseMask.SplitSelection("ab", (0, 10));

        // Assert
        before.Should().Be("");
        selected.Should().Be("ab");
        after.Should().Be("");
    }

    [Test]
    public void PatternMask_Clear_ResetsState()
    {
        // Arrange
        var mask = new PatternMask("000-000");
        mask.Insert("123456");

        // Act
        mask.Clear();

        // Assert
        mask.Text.Should().BeNullOrEmpty();
        mask.CaretPos.Should().Be(0);
        mask.Selection.Should().BeNull();
    }

    [Test]
    public void PatternMask_SetText_InsertsText()
    {
        // Arrange
        var mask = new PatternMask("000-000");

        // Act
        mask.SetText("123456");

        // Assert
        mask.Text.Should().Be("123-456");
    }

    [Test]
    public void PatternMask_SetText_NullValue()
    {
        // Arrange
        var mask = new PatternMask("000-000");
        mask.Insert("123");

        // Act
        mask.SetText(null);

        // Assert
        mask.Text.Should().BeNullOrEmpty();
    }

    [Test]
    public void PatternMask_AllowOnlyDelimiters_False()
    {
        // Arrange
        var mask = new PatternMask("---0---") { AllowOnlyDelimiters = false };

        // Act
        mask.Insert("-");

        // Assert
        mask.Text.Should().BeNullOrEmpty();
    }

    [Test]
    public void PatternMask_ToString_EmptyText()
    {
        // Arrange
        var mask = new PatternMask("000");

        // Act & Assert
        mask.ToString().Should().Be("|");
    }

    [Test]
    public void PatternMask_ToString_WithCaret()
    {
        // Arrange
        var mask = new PatternMask("000");
        mask.Insert("123");
        mask.CaretPos = 1;

        // Act & Assert
        mask.ToString().Should().Be("1|23");
    }

    [Test]
    public void PatternMask_ToString_WithSelection()
    {
        // Arrange
        var mask = new PatternMask("000");
        mask.Insert("123");
        mask.Selection = (0, 2);

        // Act & Assert
        mask.ToString().Should().Be("[12]3");
    }

    [Test]
    public void PatternMask_ToString_CaretAtEnd()
    {
        // Arrange
        var mask = new PatternMask("000");
        mask.Insert("123");

        // Act & Assert
        mask.ToString().Should().Be("123|");
    }

    [Test]
    public void PatternMask_ToString_CaretBeyondText()
    {
        // Arrange
        var mask = new PatternMask("000");
        mask.Insert("12");
        mask.CaretPos = 10;

        // Act & Assert
        mask.ToString().Should().Be("12|");
    }

    [Test]
    public void PatternMask_ToString_NegativeCaret()
    {
        // ConsolidateCaret clamps a negative caret to 0, so the marker lands at the start.
        var mask = new PatternMask("000");
        mask.Insert("123");
        mask.CaretPos = -5;

        mask.ToString().Should().Be("|123");
    }

    [Test]
    public void PatternMask_UpdateFrom_NullOther()
    {
        // Arrange
        var mask = new PatternMask("000");
        mask.Insert("123");
        var originalText = mask.Text;

        // Act
        mask.UpdateFrom(null);

        // Assert
        mask.Text.Should().Be(originalText);
    }

    [Test]
    public void Adopt_StartsEmpty_AndIsIndependentOfOriginal()
    {
        // Arrange
        var original = new RegexMask("^[0-9]+$");
        original.SetText("123");

        // Act : a different/absent current forces a defensive copy
        var copy = (RegexMask)BaseMask.Adopt(null, original);
        copy.SetText("9");
        original.Insert("4");

        // Assert : the copy starts empty and the two never share state
        copy.Should().NotBeSameAs(original);
        copy.Text.Should().Be("9");
        original.Text.Should().Be("1234");
    }

    [Test]
    public void Adopt_SameType_UpdatesInPlace_RetainingState()
    {
        // Arrange
        var current = new PatternMask("0000");
        current.SetText("12");
        var incoming = new PatternMask("0000") { Placeholder = '_' };

        // Act
        var owned = (PatternMask)BaseMask.Adopt(current, incoming);

        // Assert : same instance is reused (typed input retained) and the new config is copied in
        owned.Should().BeSameAs(current);
        owned.GetCleanText().Should().Be("12");
        owned.Placeholder.Should().Be('_');
    }

    [Test]
    public void Adopt_PatternMask_PreservesConfiguration()
    {
        // Arrange
        var original = new PatternMask("(000) 000") { Placeholder = '_', CleanDelimiters = true };

        // Act
        var copy = (PatternMask)BaseMask.Adopt(null, original);
        copy.SetText("12");

        // Assert : placeholder fills the remaining slots, proving the config survived the copy
        copy.Placeholder.Should().Be('_');
        copy.CleanDelimiters.Should().BeTrue();
        copy.Text.Should().Be("(12_) ___");
    }

    [Test]
    public void Adopt_RegexMask_PreservesPattern()
    {
        // Arrange
        var original = new RegexMask("^[0-9]+$");

        // Act
        var copy = (RegexMask)BaseMask.Adopt(null, original);

        // Assert : the regex still validates (only digits accepted), so _regexPattern was carried via the constructor
        copy.SetText("12ab34");
        copy.GetCleanText().Should().Be("1234");
    }

    [Test]
    public void Adopt_RegexMask_PreservesAllowOnlyDelimiters()
    {
        // Arrange : RegexMask.IPv6 sets AllowOnlyDelimiters, which UpdateFrom must carry over
        var original = RegexMask.IPv6();

        // Act
        var copy = (RegexMask)BaseMask.Adopt(null, original);

        // Assert
        copy.AllowOnlyDelimiters.Should().BeTrue();
    }

    [Test]
    public void Adopt_DateMask_PreservesFormat()
    {
        // Arrange
        var original = new DateMask("MM/dd/yyyy");

        // Act
        var copy = (DateMask)BaseMask.Adopt(null, original);
        copy.SetText("12312024");

        // Assert : day/month/year alignment still applies, so the date format survived the copy
        copy.Text.Should().Be("12/31/2024");
    }

    [Test]
    public void Adopt_MultiMask_PreservesOptions_AndResetsDetectedOption()
    {
        // Arrange
        var original = new MultiMask("0000 0000 0000 0000",
            new MaskOption("American Express", "0000 000000 00000", @"^(34|37)"));
        original.Insert("3712");
        original.DetectedOption.Should().NotBeNull();

        // Act
        var copy = (MultiMask)BaseMask.Adopt(null, original);

        // Assert : copy starts with no detected option, but option detection still works
        copy.DetectedOption.Should().BeNull();
        copy.Insert("3712");
        copy.DetectedOption.Should().NotBeNull();
        copy.Mask.Should().Be("0000 000000 00000");
    }

    [Test]
    public void Adopt_BlockMask_PreservesPattern()
    {
        // Arrange
        var original = new BlockMask("-", new Block('0', 1, 4), new Block('a', 1, 4));

        // Act
        var copy = (BlockMask)BaseMask.Adopt(null, original);
        copy.SetText("12ab");

        // Assert : the block pattern (digits then letters, '-' delimiter) survived the copy
        copy.Text.Should().Be("12-ab");
    }
}
