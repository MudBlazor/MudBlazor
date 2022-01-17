// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities;

[TestFixture]
public class MaskingTests
{
    #region Base Mask Tests
    
    [Test]
    public void SimpleMask_Internals()
    {
        BaseMask.SplitAt("asdf", 1).Should().Be(("a", "sdf"));
        BaseMask.SplitAt("", 1).Should().Be(("", ""));
        BaseMask.SplitAt("asdf", -1).Should().Be(("", "asdf"));
        BaseMask.SplitAt("asdf", 10).Should().Be(("asdf", ""));
    }
    
    #endregion
    
    #region Simple Mask Tests

    [Test]
    public void SimpleMask_Insert()
    {
        var mask = new SimpleMask("(aa) 00-0");
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
        mask.ToString().Should().Be("(|");
        mask.Text.Should().Be("(");
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
    public void SimpleMask_AutoFilling()
    {
        var mask = new SimpleMask("---0---");
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
    public void SimpleMask_Placeholder()
    {
        var mask = new SimpleMask("(+00) 000 0000") { Placeholder = '_' };
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
    public void SimpleMask_Delete()
    {
        var mask = new SimpleMask("(+00) 000 0000"); // no placeholder
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
    }

    [Test]
    public void SimpleMask_Backspace()
    {
        var mask = new SimpleMask("(+00) 000 0000"); // no placeholder
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
    public void SimpleMask_Selection()
    {
        var mask = new SimpleMask("(+00) 000 0000"); // no placeholder
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
    }

    #endregion

    #region Block Mask Tests
    
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
    }

    [Test]
    public void BlockMask_Internals()
    {
        var mask = new BlockMask(".", new Block('('), new Block('0', 2, 2), new Block(')'));
        mask.Clear(); // make sure it is initialized
        mask.Mask.Should().Be(@"^\(([\.](\d(\d([\.](\))?)?)?)?)?$");
        mask = new BlockMask(".", new Block('0', 1, 2), new Block('0', 1, 2), new Block('0', 2, 4));
        mask.Clear(); // make sure it is initialized
        mask.Mask.Should().Be(@"^\d(\d)?([\.](\d(\d)?([\.](\d(\d(\d(\d)?)?)?)?)?)?)?$");
    }
    
    #endregion
}
