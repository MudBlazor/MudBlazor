// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class RegexMaskTests
{
    [Test]
    public void RegexMask_Insert()
    {
        var mask = new RegexMask("^[0-9]+$");
        mask.ToString().Should().Be("|");
        mask.Insert("12.");
        mask.ToString().Should().Be("12|");
        mask.Clear();
        mask.Insert("xx12.34xx.5678");
        mask.Text.Should().Be("12345678");
        mask.Clear();
        mask = new RegexMask("^[a-f0-9]+$");
        mask.Insert("this beef is dead for 10 hours now");
        mask.ToString().Should().Be("beefdeadf10|");
        mask.Text.Should().Be("beefdeadf10");
        mask.GetCleanText().Should().Be("beefdeadf10");
        mask.Selection = (0, 1);
        mask.Insert("1");
        mask.ToString().Should().Be("1|eefdeadf10");
    }

    [Test]
    public void RegexMask_Delete()
    {
        var mask = new RegexMask("^[0-9]+$");
        mask.SetText("1234");
        mask.CaretPos = 1;
        mask.Delete();
        mask.ToString().Should().Be("1|34");
    }

    [Test]
    public void RegexMask_Backspace()
    {
        var mask = new RegexMask("^[0-9]+$");
        mask.SetText("1234");
        mask.CaretPos = 1;
        mask.Backspace();
        mask.ToString().Should().Be("|234");
    }

    [Test]
    public void RegexMask_UpdateFrom()
    {
        var mask = new RegexMask("^[0-9]+$");
        mask.SetText("1234");
        mask.CaretPos = 1;
        mask.UpdateFrom(null);
        mask.ToString().Should().Be("1|234");
    }

    [Test]
    public void RegexMask_SelectionDelete()
    {
        // Arrange
        var mask = new RegexMask("^[0-9]+$");
        mask.Insert("12345");
        mask.Selection = (1, 4);

        // Act
        mask.Delete();

        // Assert
        mask.Text.Should().Be("15");
    }

    [Test]
    public void RegexMask_SelectionBackspace()
    {
        // Arrange
        var mask = new RegexMask("^[0-9]+$");
        mask.Insert("12345");
        mask.Selection = (1, 4);

        // Act
        mask.Backspace();

        // Assert
        mask.Text.Should().Be("15");
    }

    [Test]
    public void RegexMask_EmptyInput()
    {
        // Arrange
        var mask = new RegexMask("^[0-9]*$");

        // Act
        mask.Insert("");

        // Assert
        mask.Text.Should().BeEmpty();
    }

    [Test]
    public void RegexMask_NullInput()
    {
        // Arrange
        var mask = new RegexMask("^[0-9]*$");

        // Act
        mask.Insert(null);

        // Assert
        mask.Text.Should().BeNullOrEmpty();
    }

    [Test]
    public void RegexMask_NonProgressiveRegex_BlocksAllInput()
    {
        // Documented contract: the regex must match every input prefix. A fixed-length
        // pattern like ^[0-9]{5}$ matches neither "1" nor "12345" as it is being typed,
        // so AlignAgainstMask appends nothing and the input is silently rejected.
        var mask = new RegexMask("^[0-9]{5}$");

        mask.Insert("12345");

        mask.Text.Should().BeEmpty();
    }

    [Test]
    public void RegexMask_ProgressiveRegex_AcceptsBoundedInput()
    {
        // The progressive form ^[0-9]{0,5}$ matches partial input, so the same digits
        // are accepted up to the upper bound and further digits are dropped.
        var mask = new RegexMask("^[0-9]{0,5}$");

        mask.Insert("1234567");

        mask.Text.Should().Be("12345");
    }

}
