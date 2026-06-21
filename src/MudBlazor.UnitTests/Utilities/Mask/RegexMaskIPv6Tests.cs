// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class RegexMaskIPv6Tests
{
    [Test]
    public void IPv6_Mask()
    {
        var mask = RegexMask.IPv6();
        mask.Mask.Should().Be("XXXX:XXXX:XXXX:XXXX:XXXX:XXXX:XXXX:XXXX");
        mask.ToString().Should().Be("|");
        mask.Insert("255255255255");
        mask.Mask.Should().Be("XXXX:XXXX:XXXX:XXXX:XXXX:XXXX:XXXX:XXXX");
        mask = RegexMask.IPv6(maskChar: '_');
        mask.Mask.Should().Be("____:____:____:____:____:____:____:____");
    }

    [Test]
    public void IPv6_Insert()
    {
        var mask = RegexMask.IPv6();
        mask.Mask.Should().Be("XXXX:XXXX:XXXX:XXXX:XXXX:XXXX:XXXX:XXXX");
        mask.ToString().Should().Be("|");
        mask.Insert("::");
        mask.ToString().Should().Be("::|");
        mask.Insert("123");
        mask.ToString().Should().Be("::123|");
        mask.Insert("456");
        mask.ToString().Should().Be("::1234:56|");
        mask.Clear();
        mask.Insert("20010db80000000034f400000000f3dd");
        mask.ToString().Should().Be("2001:0db8:0000:0000:34f4:0000:0000:f3dd|");
        mask.Clear();
        mask.Insert("2001:0db8:0000:0000:34f4:0000:0000:f3dd");
        mask.ToString().Should().Be("2001:0db8:0000:0000:34f4:0000:0000:f3dd|");
        mask.Clear();
        mask.Insert("2001db8::34f400000000f3dd");
        mask.ToString().Should().Be("2001:db8::34f4:0000:0000:f3dd|");
        mask.Clear();
        mask.Insert("2001db8::34f40:0:f3dd");
        mask.ToString().Should().Be("2001:db8::34f4:0:0:f3dd|");
        mask.Clear();
        mask = RegexMask.IPv6();
        mask.Insert("this beef is dead for 2001 hours and 345 min");
        mask.ToString().Should().Be("beef:dead:f200:1ad3:45|");
        mask.Text.Should().Be("beef:dead:f200:1ad3:45");
        mask.GetCleanText().Should().Be("beef:dead:f200:1ad3:45");
        mask.Selection = (0, 1);
        mask.Insert("2");
        mask.ToString().Should().Be("2|eef:dead:f200:1ad3:45");
        mask.Clear();
        mask = RegexMask.IPv6();
        mask.Insert("00000000000000000000000000000000");
        mask.ToString().Should().Be("0000:0000:0000:0000:0000:0000:0000:0000|");
        mask.Selection = (5, 4);
        mask.Insert("34f4");
        mask.ToString().Should().Be("0000:34f4|:0000:0000:0000:0000:0000:0000");
        mask.Clear();
        mask.Insert("0:0:0:0:0:0:0:0");
        mask.ToString().Should().Be("0:0:0:0:0:0:0:0|");
        mask.Selection = (14, 1);
        mask.Insert("0db8");
        mask.ToString().Should().Be("0:0:0:0:0:0:0:0db8|");
        mask = RegexMask.IPv6();
        mask.ToString().Should().Be("|");
        mask.Insert("0:0:0:0:0:0:0:0\n");
        mask.ToString().Should().Be("0:0:0:0:0:0:0:0|");
        mask.Text.IndexOf('\n').Should().Be(-1);
    }

    [Test]
    public void IPv6_Insert_DropsNonHexLetters()
    {
        // only [0-9A-Fa-f] are valid hex; g-z must be filtered out
        var mask = RegexMask.IPv6();
        mask.Insert("gbeefz");
        mask.ToString().Should().Be("beef|");
    }

    [Test]
    public void IPv6_Insert_FifthHexDigitJumpsToNextGroup()
    {
        // each group is capped at 4 hex chars ([0-9A-Fa-f]{0,4}); the 5th jumps the delimiter
        var mask = RegexMask.IPv6();
        mask.Insert("aaaaa");
        mask.ToString().Should().Be("aaaa:a|");
    }

    [Test]
    public void IPv6_Insert_NinthGroupRejected()
    {
        // an IPv6 address has at most 8 groups (Hex(:Hex){0,7}); the 9th colon is dropped
        var mask = RegexMask.IPv6();
        mask.Insert("1:2:3:4:5:6:7:8:9");
        mask.ToString().Should().Be("1:2:3:4:5:6:7:89|");
    }

    [Test]
    public void IPv6_Insert_TripleColonRejected()
    {
        // the "::" zero-group shorthand may appear only once; ":::" is blocked by the IPv6 filter
        var mask = RegexMask.IPv6();
        mask.Insert("::::");
        mask.ToString().Should().Be("::|");
    }

    [Test]
    public void IPv6_Delete()
    {
        var mask = RegexMask.IPv6();
        mask.SetText("2001db8::34f400000000f3dd");
        mask.CaretPos = 1;
        mask.Delete();
        mask.ToString().Should().Be("2|01:db8::34f4:0000:0000:f3dd");
    }

    [Test]
    public void IPv6_Backspace()
    {
        var mask = RegexMask.IPv6();
        mask.SetText("2001db8::34f400000000f3dd");
        mask.CaretPos = 1;
        mask.Backspace();
        mask.ToString().Should().Be("|001:db8::34f4:0000:0000:f3dd");
    }

    [Test]
    public void IPv6_UpdateFrom()
    {
        var mask = RegexMask.IPv6();
        mask.SetText("2001db8::34f400000000f3dd");
        mask.CaretPos = 1;
        mask.UpdateFrom(null);
        mask.ToString().Should().Be("2|001:db8::34f4:0000:0000:f3dd");
    }

    [Test]
    public void IPv6WithPort_Mask()
    {
        var mask = RegexMask.IPv6(true);
        mask.Mask.Should().Be("[XXXX:XXXX:XXXX:XXXX:XXXX:XXXX:XXXX:XXXX]:00000");
        mask.ToString().Should().Be("|");
        mask.Insert("255255255255");
        mask.Mask.Should().Be("[XXXX:XXXX:XXXX:XXXX:XXXX:XXXX:XXXX:XXXX]:00000");
        mask = RegexMask.IPv6(true, '_');
        mask.Mask.Should().Be("[____:____:____:____:____:____:____:____]:00000");
        mask = RegexMask.IPv6(true, '_', '_');
        mask.Mask.Should().Be("[____:____:____:____:____:____:____:____]:_____");
    }

    [Test]
    public void IPv6WithPort_Insert()
    {
        var mask = RegexMask.IPv6(true);
        mask.Mask.Should().Be("[XXXX:XXXX:XXXX:XXXX:XXXX:XXXX:XXXX:XXXX]:00000");
        mask.ToString().Should().Be("|");
        mask.Insert("[::");
        mask.ToString().Should().Be("[::|");
        mask.Insert("123");
        mask.ToString().Should().Be("[::123|");
        mask.Insert("456");
        mask.ToString().Should().Be("[::1234:56|");
        mask.Clear();
        mask.Insert("20010db80000000034f400000000f3dd");
        mask.ToString().Should().Be("[2001:0db8:0000:0000:34f4:0000:0000:f3dd|");
        mask.Clear();
        mask.Insert("2001:0db8:0000:0000:34f4:0000:0000:f3dd");
        mask.ToString().Should().Be("[2001:0db8:0000:0000:34f4:0000:0000:f3dd|");
        mask.Clear();
        mask.Insert("2001db8::34f400000000f3dd");
        mask.ToString().Should().Be("[2001:db8::34f4:0000:0000:f3dd|");
        mask.Clear();
        mask.Insert("2001db8::34f40:0:f3dd");
        mask.ToString().Should().Be("[2001:db8::34f4:0:0:f3dd|");
        mask.Clear();
        mask = RegexMask.IPv6(true);
        mask.Insert("this beef is dead for 2001 hours and 345 min");
        mask.ToString().Should().Be("[beef:dead:f200:1ad3:45|");
        mask.Text.Should().Be("[beef:dead:f200:1ad3:45");
        mask.GetCleanText().Should().Be("[beef:dead:f200:1ad3:45");
        mask.Selection = (1, 2);
        mask.Insert("2");
        mask.ToString().Should().Be("[2|eef:dead:f200:1ad3:45");
        mask.Clear();
        mask = RegexMask.IPv6(true);
        mask.Insert("00000000000000000000000000000000");
        mask.ToString().Should().Be("[0000:0000:0000:0000:0000:0000:0000:0000|");
        mask.Selection = (6, 4);
        mask.Insert("34f4");
        mask.ToString().Should().Be("[0000:34f4|:0000:0000:0000:0000:0000:0000]:");
        mask.Clear();
        mask.Insert("0:0:0:0:0:0:0:0");
        mask.ToString().Should().Be("[0:0:0:0:0:0:0:0|");
        mask.Selection = (15, 1);
        mask.Insert("0db8");
        mask.ToString().Should().Be("[0:0:0:0:0:0:0:0db8|");
        mask = RegexMask.IPv6(true);
        mask.ToString().Should().Be("|");
        mask.Insert("0:0:0:0:0:0:0:0\n");
        mask.ToString().Should().Be("[0:0:0:0:0:0:0:0|");
        mask.Text.IndexOf('\n').Should().Be(-1);

        mask = RegexMask.IPv6(true);
        mask.Insert("00000000000000000000000000000000:1");
        mask.ToString().Should().Be("[0000:0000:0000:0000:0000:0000:0000:0000]:1|");
        mask.Selection = (6, 4);
        mask.Insert("34f4");
        mask.ToString().Should().Be("[0000:34f4|:0000:0000:0000:0000:0000:0000]:1");
        mask.Clear();
        mask.Insert("0:0:0:0:0:0:0:0:1");
        mask.ToString().Should().Be("[0:0:0:0:0:0:0:0]:1|");
        mask.Selection = (15, 1);
        mask.Insert("0db8");
        mask.ToString().Should().Be("[0:0:0:0:0:0:0:0db8|]:1");
        mask = RegexMask.IPv6(true);
        mask.ToString().Should().Be("|");
        mask.Insert("0:0:0:0:0:0:0:0:1\n");
        mask.ToString().Should().Be("[0:0:0:0:0:0:0:0]:1|");
        mask.Text.IndexOf('\n').Should().Be(-1);
    }

    [Test]
    public void IPv6WithPort_Insert_PortAbove65535Rejected()
    {
        // ports are capped at 65535; the digit that pushes the value over the limit is dropped
        var mask = RegexMask.IPv6(true);
        mask.Insert("1:2:3:4:5:6:7:8:65536");
        mask.ToString().Should().Be("[1:2:3:4:5:6:7:8]:6553|");
    }

    [Test]
    public void IPv6WithPort_Delete()
    {
        var mask = RegexMask.IPv6(true);
        mask.SetText("2001db8::34f400000000f3dd34f4");
        mask.CaretPos = 2;
        mask.Delete();
        mask.ToString().Should().Be("[2|01:db8::34f4:0000:0000:f3dd:34f4");

        mask = RegexMask.IPv6(true);
        mask.SetText("2001db8::34f400000000f3dd34f4:24524");
        mask.CaretPos = 2;
        mask.Delete();
        mask.ToString().Should().Be("[2|01:db8::34f4:0000:0000:f3dd:34f4]:24524");
    }

    [Test]
    public void IPv6WithPort_Backspace()
    {
        var mask = RegexMask.IPv6(true);
        mask.SetText("2001db8::34f400000000f3dd34f4");
        mask.CaretPos = 2;
        mask.Backspace();
        mask.ToString().Should().Be("[|001:db8::34f4:0000:0000:f3dd:34f4");

        mask = RegexMask.IPv6(true);
        mask.SetText("2001db8::34f400000000f3dd34f434f4:24524");
        mask.ToString().Should().Be("[2001:db8::34f4:0000:0000:f3dd:34f4]:24524|");
        mask.CaretPos = 2;
        mask.Backspace();
        mask.ToString().Should().Be("[|001:db8::34f4:0000:0000:f3dd:34f4]:24524");
    }

    [Test]
    public void IPv6WithPort_UpdateFrom()
    {
        var mask = RegexMask.IPv6(true);
        mask.SetText("2001db8::34f400000000f3dd34f4");
        mask.CaretPos = 2;
        mask.UpdateFrom(null);
        mask.ToString().Should().Be("[2|001:db8::34f4:0000:0000:f3dd:34f4");

        mask = RegexMask.IPv6(true);
        mask.SetText("2001db8::34f400000000f3dd34f4:24524");
        mask.CaretPos = 2;
        mask.UpdateFrom(null);
        mask.ToString().Should().Be("[2|001:db8::34f4:0000:0000:f3dd:34f4]:24524");
    }
}
