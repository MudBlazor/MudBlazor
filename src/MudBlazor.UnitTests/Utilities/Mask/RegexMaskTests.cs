// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
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

}
