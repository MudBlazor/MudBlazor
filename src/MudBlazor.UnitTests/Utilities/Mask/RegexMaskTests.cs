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
    public void RegexMask_AlignAgainstMask_SimpleDigits()
    {
        // Arrange
        var mask = new RegexMask("^[0-9]+$");

        // Act
        mask.Insert("123");

        // Assert
        mask.Text.Should().Be("123");
    }

    [Test]
    public void RegexMask_AlignAgainstMask_FilterInvalidChars()
    {
        // Arrange
        var mask = new RegexMask("^[0-9]+$");

        // Act
        mask.Insert("1a2b3c");

        // Assert
        mask.Text.Should().Be("123");
    }

    [Test]
    public void RegexMask_Delimiters_AutoInsert()
    {
        // Arrange - Use IPv4 which has delimiters built-in
        var mask = RegexMask.IPv4();

        // Act
        mask.Insert("192168001001");

        // Assert
        mask.Text.Should().Contain(".");
    }

    [Test]
    public void RegexMask_GetCleanText()
    {
        // Arrange
        var mask = new RegexMask("^[0-9]+$");
        mask.Insert("12345");

        // Act
        var cleanText = mask.GetCleanText();

        // Assert
        cleanText.Should().Be("12345");
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
    public void RegexMask_IPv4_Basic()
    {
        // Arrange
        var mask = RegexMask.IPv4();

        // Act
        mask.Insert("192.168.1.1");

        // Assert
        mask.Text.Should().Be("192.168.1.1");
    }

    [Test]
    public void RegexMask_IPv4_WithPort()
    {
        // Arrange
        var mask = RegexMask.IPv4(includePort: true);

        // Act
        mask.Insert("192168001001:8080");

        // Assert
        mask.Text.Should().Contain(":");
        mask.Text.Should().Contain("8080");
    }

    [Test]
    public void RegexMask_IPv6_Basic()
    {
        // Arrange
        var mask = RegexMask.IPv6();

        // Act
        mask.Insert("2001:0db8:85a3:0000:0000:8a2e:0370:7334");

        // Assert
        mask.Text.Should().Contain(":");
    }

    [Test]
    public void RegexMask_IPv6_WithPort()
    {
        // Arrange
        var mask = RegexMask.IPv6(includePort: true);

        // Act
        mask.Insert("[::1]:8080");

        // Assert
        mask.Text.Should().Contain("[");
        mask.Text.Should().Contain("]");
    }

    [Test]
    public void RegexMask_Email_Basic()
    {
        // Arrange
        var mask = RegexMask.Email();

        // Act
        mask.Insert("test@example.com");

        // Assert
        mask.Text.Should().Be("test@example.com");
    }

    [Test]
    public void RegexMask_Email_WithSubdomain()
    {
        // Arrange
        var mask = RegexMask.Email();

        // Act
        mask.Insert("user@mail.sub.domain.com");

        // Assert
        mask.Text.Should().Contain("@");
        mask.Text.Should().Contain(".");
    }

    [Test]
    public void RegexMask_HexPattern()
    {
        // Arrange
        var mask = new RegexMask("^[0-9A-Fa-f]+$");

        // Act
        mask.Insert("1A2B3C");

        // Assert
        mask.Text.Should().Be("1A2B3C");
    }

    [Test]
    public void RegexMask_UpdateFrom_WithDelimiters()
    {
        // Arrange
        var mask = new RegexMask("^[0-9]+$");
        var otherMask = new RegexMask("^[0-9.]+$");

        // Act
        mask.UpdateFrom(otherMask);
        mask.Insert("123.456");

        // Assert - After UpdateFrom, behavior should be updated
        mask.Text.Should().NotBeNullOrEmpty();
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
}
