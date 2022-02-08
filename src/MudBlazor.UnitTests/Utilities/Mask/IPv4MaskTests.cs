// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class IPv4MaskTests
{
    [Test]
    public void IPv4Mask_Mask()
    {
        var mask = new IPv4Mask();
        mask.Mask.Should().Be("000.000.000.000");
        mask.ToString().Should().Be("|");
        mask.Insert("255255255255");
        mask.Mask.Should().Be("000.000.000.000");
    }

    [Test]
    public void IPv4Mask_Insert()
    {
        var mask = new IPv4Mask();
        mask.Mask.Should().Be("000.000.000.000");
        mask.ToString().Should().Be("|");
        mask.Insert("255255");
        mask.ToString().Should().Be("255.255|");
        mask.Clear();
        mask.Insert("xx12.34xx.5678");
        mask.Text.Should().Be("12.34.56.78");
        mask.Clear();
        mask = new IPv4Mask();
        mask.Insert("this beef is dead for 10000 hours and 345 min");
        mask.ToString().Should().Be("100.003.45|");
        mask.Text.Should().Be("100.003.45");
        mask.GetCleanText().Should().Be("100.003.45");
        mask.Selection = (0, 1);
        mask.Insert("2");
        mask.ToString().Should().Be("2|00.003.45");
        mask.Clear();
        mask = new IPv4Mask();
        mask.Insert("255255255255");
        mask.ToString().Should().Be("255.255.255.255|");
        mask.Clear();
        mask.Insert("000000000000");
        mask.ToString().Should().Be("000.000.000.000|");
        mask.Selection = (3, 3);
        mask.Insert("128");
        mask.ToString().Should().Be("000.128|.000.000");
        mask.Clear();
        mask.Insert("0.0.0.0");
        mask.ToString().Should().Be("0.0.0.0|");
        mask.Selection = (2, 1);
        mask.Insert("267");
        mask.ToString().Should().Be("0.26.7|0.00");
    }

    [Test]
    public void IPv4Mask_Delete()
    {
        var mask = new IPv4Mask();
        mask.SetText("265245");
        mask.CaretPos = 1;
        mask.Delete();
        mask.ToString().Should().Be("2|.52.45");
    }

    [Test]
    public void IPv4Mask_Backspace()
    {
        var mask = new IPv4Mask();
        mask.SetText("265245");
        mask.CaretPos = 1;
        mask.Backspace();
        mask.ToString().Should().Be("|6.52.45");
    }

    [Test]
    public void IPv4Mask_UpdateFrom()
    {
        var mask = new IPv4Mask();
        mask.SetText("265245");
        mask.CaretPos = 1;
        mask.UpdateFrom(null);
        mask.ToString().Should().Be("2|6.52.45");
    }
}
