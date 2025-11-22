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
        mask.Text.Should().Be("aaa@gmail.com");
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
        mask.Text.Should().Be("te@stdomain.com");
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

    [Test]
    [TestCase("user@example.com", "user@example.com")] //valid
    [TestCase("valid.email@domain.co1", "valid.email@domain.co1")]
    [TestCase("user-123@my-domain.org9", "user-123@my-domain.org9")]
    [TestCase("user_name@domain123.net5", "user_name@domain123.net5")]
    [TestCase("firstname.lastname@sub.domain123.uk", "firstname.lastname@sub.domain123.uk")]
    [TestCase("user.name+alias@sub-domain.example.com", "user.name+alias@sub-domain.example.com")]
    [TestCase("john.doe+9anime-sub@mail-server-2.domain.com", "john.doe+9anime-sub@mail-server-2.domain.com")]
    [TestCase("user@.example.com", "user@example.com")] //invalid
    [TestCase("user@-example.com", "user@example.com")]
    [TestCase("user@example-.com", "user@example-com")]
    [TestCase("user@example..com", "user@example.com")]
    [TestCase("user.@example.com", "user.example.com")]
    [TestCase(".user@mail.domain.com", "user@mail.domain.com")]
    [TestCase("user@sub_domain.example.com", "user@subdomain.example.com")]
    public void Email_Mask_Format(string input, string result)
    {
        var mask = RegexMask.Email();

        mask.Insert(input);

        mask.Text.Should().Be(result);
    }
}
