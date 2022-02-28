// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class RegexMaskEmailTests
{
    [Test]
    public void Email_Mask()
    {
        var mask = RegexMask.Email();
        mask.Mask.Should().Be("Ex. user@domain.com");
        mask = RegexMask.Email("test@domain.com");
        mask.Mask.Should().Be("test@domain.com");
        mask.ToString().Should().Be("|");
        mask.Insert("aaa@gmail.com");
        mask.Mask.Should().Be("test@domain.com");
    }

    [Test]
    public void Email_Insert()
    {
        var mask = RegexMask.Email();
        mask.Mask.Should().Be("Ex. user@domain.com");
        mask.ToString().Should().Be("|");
        mask.Insert("test@domain.com");
        mask.ToString().Should().Be("test@domain.com|");
        mask.Clear();
        mask.Insert("te!@#$%^&*() _+=-{}[]\\|;:'\",<.>/?`~st@domain.com");
        mask.Text.Should().Be("te@_.stdomain.com");
        mask.Clear();
        mask = RegexMask.Email();
        mask.Insert("this beef is dead for 10 hours and 3.45 min");
        mask.ToString().Should().Be("thisbeefisdeadfor10hoursand3.45min|");
        mask.Text.Should().Be("thisbeefisdeadfor10hoursand3.45min");
        mask.GetCleanText().Should().Be("thisbeefisdeadfor10hoursand3.45min");
        mask.Selection = (0, 1);
        mask.Insert("@");
        mask.ToString().Should().Be("|hisbeefisdeadfor10hoursand3.45min");
        mask.Clear();
        mask = RegexMask.Email();
        mask.Insert("username@gmail.com");
        mask.ToString().Should().Be("username@gmail.com|");
        mask.Selection = (3, 3);
        mask.Insert("128");
        mask.ToString().Should().Be("use128|rname@gmail.com");
        mask.Clear();
        mask.Insert("username@gmail.com");
        mask.ToString().Should().Be("username@gmail.com|");
        mask.Selection = (2, 1);
        mask.Insert("267");
        mask.ToString().Should().Be("us267|ername@gmail.com");
        mask.Clear();
        mask.Insert("username@gmail.com\n");
        mask.ToString().Should().Be("username@gmail.com|");
        mask.Text.IndexOf('\n').Should().Be(-1);
    }

    [Test]
    public void Email_Delete()
    {
        var mask = RegexMask.Email();
        mask.SetText("test@domain.com");
        mask.CaretPos = 1;
        mask.Delete();
        mask.ToString().Should().Be("t|st@domain.com");
    }

    [Test]
    public void Email_Backspace()
    {
        var mask = RegexMask.Email();
        mask.SetText("test@domain.com");
        mask.CaretPos = 1;
        mask.Backspace();
        mask.ToString().Should().Be("|est@domain.com");
    }

    [Test]
    public void Email_UpdateFrom()
    {
        var mask = RegexMask.Email();
        mask.SetText("test@domain.com");
        mask.CaretPos = 1;
        mask.UpdateFrom(null);
        mask.ToString().Should().Be("t|est@domain.com");
    }
}
