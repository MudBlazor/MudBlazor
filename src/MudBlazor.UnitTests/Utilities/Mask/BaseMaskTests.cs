// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
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
}
