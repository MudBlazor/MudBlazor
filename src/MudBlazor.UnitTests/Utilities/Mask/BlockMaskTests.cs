// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FluentAssertions;
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
        Assert.Throws<ArgumentException>(() => new BlockMask());
    }

    [Test]
    public void BlockMask_UpdateFrom()
    {
        var mask = new BlockMask(".", new Block('('), new Block('0', 2, 2), new Block(')'));
        mask.Blocks.Length.Should().Be(3);
        mask.Delimiters.Should().Be(".");
        mask.SetText("(1234)");
        mask.ToString().Should().Be("(.12.)|");
        mask.CaretPos = 1;
        mask.UpdateFrom(new BlockMask(":", new Block('0', 1, 1), new Block('0', 1, 1)));
        mask.Blocks.Length.Should().Be(2);
        mask.Delimiters.Should().Be(":");
        // state should be preserved (Text, Caret/Selection)
        mask.ToString().Should().Be("1|:2");
        mask.UpdateFrom(null);
        mask.Blocks.Length.Should().Be(2);
        mask.Delimiters.Should().Be(":");
        // state should be preserved (Text, Caret/Selection)
        mask.ToString().Should().Be("1|:2");
    }
}
