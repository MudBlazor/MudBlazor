// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class DateMaskTests
{

    [Test]
    public void DateMask1()
    {
        var mask = new DateMask("yyyy-MM-dd");
        // input invalid text
        mask.ToString().Should().Be("|");
        mask.Insert("?asdfqa vyczlausdhf!°§$\"%\"$\"&\"");
        mask.ToString().Should().Be("|");
        // input invalid month and day, simple cases
        mask.Insert("9999");
        mask.ToString().Should().Be("9999-|");
        mask.Insert("9");
        mask.ToString().Should().Be("9999-09-|");
        mask.Insert("9");
        mask.ToString().Should().Be("9999-09-09|");
        mask.Clear();
        mask.Insert("2222 2 4");
        mask.ToString().Should().Be("2222-02-04|");
        mask.Clear();
        // month and day must be > 0
        mask.Insert("2222 00 00");
        mask.ToString().Should().Be("2222-01-01|");
        mask.Clear();
        // month must be < 13
        mask.Insert("0000 13");
        mask.ToString().Should().Be("0000-12-|");
        mask.Clear();
        // special cases that need awareness about the days in a month
        mask.Clear();
        mask.Insert("0000 01 32");
        mask.ToString().Should().Be("0000-01-31|");
        mask.Clear();
        // year 0000 was a leap year
        mask.Clear();
        mask.Insert("0000 02 3");
        mask.ToString().Should().Be("0000-02-03|");
        // try to paste invalid day
        mask.Selection = (8, 10);
        mask.ToString().Should().Be("0000-02-[03]");
        mask.Insert("44");
        mask.ToString().Should().Be("0000-02-04|");
        // ordinary feb
        mask.Clear();
        mask.Insert("0001 02 29");
        mask.ToString().Should().Be("0001-02-28|");
        mask.Selection = (8, 10);
        mask.ToString().Should().Be("0001-02-[28]");
        mask.Insert("29");
        mask.ToString().Should().Be("0001-02-28|");
        // mar
        mask.Clear();
        mask.Insert("0000 03 33");
        mask.ToString().Should().Be("0000-03-31|");
        mask.Clear();
    }

    [Test]
    public void DateMask2()
    {
        var mask = new DateMask("MM/dd/yyyy");
        // input invalid month and day, simple cases
        mask.Insert("999999");
        mask.ToString().Should().Be("09/09/9999|");
        mask.Clear();
        mask.Insert("2 4 2222");
        mask.ToString().Should().Be("02/04/2222|");
        mask.Clear();
        // special cases that need awareness about the days in a month
        mask.Clear();
        mask.Insert("01 32");
        mask.ToString().Should().Be("01/31/|");
        mask.Clear();
        // invalid feb
        mask.Clear();
        mask.Insert("02 3 0000");
        mask.ToString().Should().Be("02/03/0000|");
        // year 0000 was a leap year
        mask.Clear();
        mask.Insert("02 29");
        mask.ToString().Should().Be("02/29/|");
        // year is at the back so it should correct feb 29th to 28th once it is entered
        mask.Clear();
        mask.Insert("02 29 000");
        mask.ToString().Should().Be("02/29/000|");
        mask.Insert("1");
        mask.ToString().Should().Be("02/28/0001|");
        // set invalid days by changing month
        mask.Clear();
        mask.Insert("03 31 2000");
        mask.Selection = (1, 2);
        mask.ToString().Should().Be("0[3]/31/2000");
        mask.Insert("4");
        mask.ToString().Should().Be("04/|30/2000");
    }

    [Test]
    public void DateMask_TwoDigitYear()
    {
        var mask = new DateMask("MM/dd/yy");
        // input invalid month and day, simple cases
        mask.Insert("9999");
        mask.ToString().Should().Be("09/09/99|");
        mask.Clear();
        mask.Insert("2 4 22");
        mask.ToString().Should().Be("02/04/22|");
        mask.Clear();
        // special cases that need awareness about the days in a month
        mask.Clear();
        mask.Insert("01 32");
        mask.ToString().Should().Be("01/31/|");
        mask.Clear();
        // invalid feb
        mask.Clear();
        mask.Insert("02 3 00");
        mask.ToString().Should().Be("02/03/00|");
        // year 0000 was a leap year
        mask.Clear();
        mask.Insert("02 29");
        mask.ToString().Should().Be("02/29/|");
        // year is at the back so it should correct feb 29th to 28th once it is entered
        mask.Clear();
        mask.Insert("02 29 0");
        mask.ToString().Should().Be("02/29/0|");
        mask.Insert("1");
        mask.ToString().Should().Be("02/28/01|");
    }

    [Test]
    public void DateMask_WithoutDay()
    {
        var mask = new DateMask("yy/MM");
        // input invalid month and day, simple cases
        mask.Insert("9999");
        mask.ToString().Should().Be("99/09|");
        mask.Clear();
        mask.Insert("02 4");
        mask.ToString().Should().Be("02/04|");
        mask.Clear();
        // month check
        mask.Clear();
        mask.Insert("01 13");
        mask.ToString().Should().Be("01/12|");
        mask.Clear();
        mask.Insert("00 00");
        mask.ToString().Should().Be("00/01|");
    }

    [Test]
    public void DateMask_CustomChars()
    {
        var mask = new DateMask("jjjj-mm-tt", 'j', 'm', 't');
        mask.Insert("2222 2 4");
        mask.ToString().Should().Be("2222-02-04|");
        mask = new DateMask("tt.mm.jjjj", 'j', 'm', 't');
        mask.Insert("421999");
        mask.ToString().Should().Be("04.02.1999|");
    }

    [Test]
    public void DateMask_Delete()
    {
        var mask = new DateMask("MM/dd/yy");
        mask.Insert("12/31/99");
        // delete creates invalid day 39
        mask.CaretPos = 4;
        mask.ToString().Should().Be("12/3|1/99");
        mask.Delete();
        mask.ToString().Should().Be("12/3|1/9");
        // delete creates invalid month 13
        mask.CaretPos = 1;
        mask.ToString().Should().Be("1|2/31/9");
        mask.Delete();
        mask.ToString().Should().Be("1|2/19/");
    }

    [Test]
    public void DateMask_Backspace()
    {
        var mask = new DateMask("MM/dd/yy");
        mask.Insert("12/31/99");
        // backspace creates invalid day 39
        mask.CaretPos = 5;
        mask.ToString().Should().Be("12/31|/99");
        mask.Backspace();
        mask.ToString().Should().Be("12/3|1/9");
        // backspace creates invalid month 13
        mask.CaretPos = 2;
        mask.ToString().Should().Be("12|/31/9");
        mask.Backspace();
        mask.ToString().Should().Be("1|2/19/");
    }

    [Test]
    public void DateMask_UpdateFrom()
    {
        var mask = new DateMask("MM/dd/yyyy");
        mask.UpdateFrom(new DateMask("jjjj-mm-tt", 'j', 'm', 't'));
        mask.Insert("2222 2 4");
        mask.ToString().Should().Be("2222-02-04|");
    }

    [Test]
    public void DateMask_WithPlaceholder()
    {
        var mask = new DateMask("yyyy-MM-dd") { Placeholder = '_' };
        // input invalid text
        mask.ToString().Should().Be("|");
        mask.Insert("?asdfqa vyczlausdhf!°§$\"%\"$\"&\"");
        mask.ToString().Should().Be("|");
        // input invalid month and day, simple cases
        mask.Insert("9999");
        mask.ToString().Should().Be("9999-|__-__");
        mask.Insert("9");
        mask.ToString().Should().Be("9999-09-|__");
        mask.Insert("9");
        mask.ToString().Should().Be("9999-09-09|");
        mask.Clear();
        mask.Insert("2222 2 4");
        mask.ToString().Should().Be("2222-02-04|");
        mask.Clear();
        // month and day must be > 0
        mask.Insert("2222 00 00");
        mask.ToString().Should().Be("2222-01-01|");
        mask.Clear();
        // month must be < 13
        mask.Insert("0000 13");
        mask.ToString().Should().Be("0000-12-|__");
        mask.Clear();
        // special cases that need awareness about the days in a month
        mask.Clear();
        mask.Insert("0000 01 32");
        mask.ToString().Should().Be("0000-01-31|");
        mask.Clear();
        // year 0000 was a leap year
        mask.Clear();
        mask.Insert("0000 02 3");
        mask.ToString().Should().Be("0000-02-03|");
        // try to paste invalid day
        mask.Selection = (8, 10);
        mask.ToString().Should().Be("0000-02-[03]");
        mask.Insert("44");
        mask.ToString().Should().Be("0000-02-04|");
        // ordinary feb
        mask.Clear();
        mask.Insert("0001 02 29");
        mask.ToString().Should().Be("0001-02-28|");
        mask.Selection = (8, 10);
        mask.ToString().Should().Be("0001-02-[28]");
        mask.Insert("29");
        mask.ToString().Should().Be("0001-02-28|");
        // mar
        mask.Clear();
        mask.Insert("0000 03 33");
        mask.ToString().Should().Be("0000-03-31|");
        mask.Clear();
    }

}
