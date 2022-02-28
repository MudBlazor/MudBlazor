// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class RegexMaskIPv4Tests
{
    [Test]
    public void IPv4_Mask()
    {
        var mask = RegexMask.IPv4();
        mask.Mask.Should().Be("000.000.000.000");
        mask.ToString().Should().Be("|");
        mask.Insert("255255255255");
        mask.Mask.Should().Be("000.000.000.000");
        mask = RegexMask.IPv4(maskChar: '_');
        mask.Mask.Should().Be("___.___.___.___");
    }

    [Test]
    public void IPv4_Insert()
    {
        var mask = RegexMask.IPv4();
        mask.Mask.Should().Be("000.000.000.000");
        mask.ToString().Should().Be("|");
        mask.Insert("255255");
        mask.ToString().Should().Be("255.255|");
        mask.Clear();
        mask.Insert("xx12.34xx.5678");
        mask.Text.Should().Be("12.34.56.78");
        mask.Clear();
        mask = RegexMask.IPv4();
        mask.Insert("this beef is dead for 10000 hours and 345 min");
        mask.ToString().Should().Be("100.003.45|");
        mask.Text.Should().Be("100.003.45");
        mask.GetCleanText().Should().Be("100.003.45");
        mask.Selection = (0, 1);
        mask.Insert("2");
        mask.ToString().Should().Be("2|00.003.45");
        mask.Clear();
        mask = RegexMask.IPv4();
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
        mask = RegexMask.IPv4();
        mask.ToString().Should().Be("|");
        mask.Insert("255255255255\n");
        mask.ToString().Should().Be("255.255.255.255|");
        mask.Text.IndexOf('\n').Should().Be(-1);
    }

    [Test]
    public void IPv4_Delete()
    {
        var mask = RegexMask.IPv4();
        mask.SetText("265245");
        mask.CaretPos = 1;
        mask.Delete();
        mask.ToString().Should().Be("2|.52.45");
    }

    [Test]
    public void IPv4_Backspace()
    {
        var mask = RegexMask.IPv4();
        mask.SetText("265245");
        mask.CaretPos = 1;
        mask.Backspace();
        mask.ToString().Should().Be("|6.52.45");
    }

    [Test]
    public void IPv4_UpdateFrom()
    {
        var mask = RegexMask.IPv4();
        mask.SetText("265245");
        mask.CaretPos = 1;
        mask.UpdateFrom(null);
        mask.ToString().Should().Be("2|6.52.45");
    }

    [Test]
    public void IPv4WithPort_Mask()
    {
        var mask = RegexMask.IPv4(true);
        mask.Mask.Should().Be("000.000.000.000:00000");
        mask.ToString().Should().Be("|");
        mask.Insert("25525525525525525");
        mask.Mask.Should().Be("000.000.000.000:00000");
        mask = RegexMask.IPv4(true, '_');
        mask.Mask.Should().Be("___.___.___.___:_____");
    }

    [Test]
    public void IPv4WithPort_Insert()
    {
        var mask = RegexMask.IPv4(true);
        mask.Mask.Should().Be("000.000.000.000:00000");
        mask.ToString().Should().Be("|");
        mask.Insert("255255");
        mask.ToString().Should().Be("255.255|");
        mask.Clear();
        mask.Insert("xx12.34xx.5678");
        mask.Text.Should().Be("12.34.56.78");
        mask.Clear();
        mask = RegexMask.IPv4(true);
        mask.Insert("this beef is dead for 10000 hours and 345 min");
        mask.ToString().Should().Be("100.003.45|");
        mask.Text.Should().Be("100.003.45");
        mask.GetCleanText().Should().Be("100.003.45");
        mask.Selection = (0, 1);
        mask.Insert("2");
        mask.ToString().Should().Be("2|00.003.45");
        mask.Clear();
        mask = RegexMask.IPv4(true);
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
        mask = RegexMask.IPv4(true);
        mask.ToString().Should().Be("|");
        mask.Insert("255255255255\n");
        mask.ToString().Should().Be("255.255.255.255|");
        mask.Text.IndexOf('\n').Should().Be(-1);

        mask = RegexMask.IPv4(true);
        mask.Insert("25525525525525525");
        mask.ToString().Should().Be("255.255.255.255:25525|");
        mask.Clear();
        mask.Insert("00000000000000000");
        mask.ToString().Should().Be("000.000.000.000|");
        mask.Clear();
        mask.Insert("00000000000000001");
        mask.ToString().Should().Be("000.000.000.000:1|");
        mask.Selection = (3, 3);
        mask.Insert("128");
        mask.ToString().Should().Be("000.128|.000.000:1");
        mask.Clear();
        mask.Insert("0.0.0.0:0");
        mask.ToString().Should().Be("0.0.0.0:|");
        mask.Insert("1");
        mask.ToString().Should().Be("0.0.0.0:1|");
        mask.Selection = (2, 1);
        mask.Insert("267");
        mask.ToString().Should().Be("0.26.7|0.00:1");
        mask.Selection = (9, 1);
        mask.Insert(":267");
        mask.ToString().Should().Be("0.26.70.0:267|01");
        mask = RegexMask.IPv4(true);
        mask.ToString().Should().Be("|");
        mask.Insert("25525525525512345\n");
        mask.ToString().Should().Be("255.255.255.255:12345|");
        mask.Text.IndexOf('\n').Should().Be(-1);
    }

    [Test]
    public void IPv4WithPort_Delete()
    {
        var mask = RegexMask.IPv4(true);
        mask.SetText("265245");
        mask.CaretPos = 1;
        mask.Delete();
        mask.ToString().Should().Be("2|.52.45");

        mask = RegexMask.IPv4(true);
        mask.SetText("265245245245246");
        mask.CaretPos = 1;
        mask.Delete();
        mask.ToString().Should().Be("2|.52.45.245:24524");
    }

    [Test]
    public void IPv4WithPort_Backspace()
    {
        var mask = RegexMask.IPv4(true);
        mask.SetText("265245");
        mask.CaretPos = 1;
        mask.Backspace();
        mask.ToString().Should().Be("|6.52.45");

        mask = RegexMask.IPv4(true);
        mask.SetText("265245245245246");
        mask.CaretPos = 1;
        mask.Backspace();
        mask.ToString().Should().Be("|6.52.45.245:24524");
    }

    [Test]
    public void IPv4WithPort_UpdateFrom()
    {
        var mask = RegexMask.IPv4(true);
        mask.SetText("265245");
        mask.CaretPos = 1;
        mask.UpdateFrom(null);
        mask.ToString().Should().Be("2|6.52.45");

        mask = RegexMask.IPv4(true);
        mask.SetText("265245245245246");
        mask.CaretPos = 1;
        mask.UpdateFrom(null);
        mask.ToString().Should().Be("2|6.52.45.245:24524");
    }
}
