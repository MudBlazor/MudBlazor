// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class RegexMaskUsZipCodeTests
{
    /// <summary>
    /// Types each character of an input separately and returns the mask text after every keystroke.
    /// </summary>
    /// <param name="mask">The mask to type into.</param>
    /// <param name="input">The characters to type, one at a time.</param>
    /// <returns>One entry per typed character, holding the mask text at that point.</returns>
    private static List<string> TypeOneByOne(RegexMask mask, string input)
    {
        var states = new List<string>();
        foreach (var c in input)
        {
            mask.Insert(c.ToString());
            states.Add(mask.Text);
        }

        return states;
    }

    /// <summary>
    /// The ZIP code mask shows the ZIP+4 structure and honors a custom mask character.
    /// </summary>
    [Test]
    public void UsZipCode_Mask()
    {
        var mask = RegexMask.UsZipCode();
        mask.Mask.Should().Be("00000-0000");
        mask.ToString().Should().Be("|");
        mask.Insert("902100123");
        mask.Mask.Should().Be("00000-0000");
        mask = RegexMask.UsZipCode(maskChar: '_');
        mask.Mask.Should().Be("_____-____");
    }

    /// <summary>
    /// Every intermediate state while typing a full ZIP+4 code is accepted.
    /// </summary>
    [Test]
    public void UsZipCode_AcceptsEveryTypedPrefix()
    {
        var mask = RegexMask.UsZipCode();
        TypeOneByOne(mask, "902100123").Should().Equal(
            "9",
            "90",
            "902",
            "9021",
            "90210",
            "90210-0",
            "90210-01",
            "90210-012",
            "90210-0123");
    }

    /// <summary>
    /// A dash typed by the user after five digits is kept instead of being inserted twice.
    /// </summary>
    [Test]
    public void UsZipCode_AcceptsTypedDash()
    {
        var mask = RegexMask.UsZipCode();
        TypeOneByOne(mask, "90210-0123").Should().Equal(
            "9",
            "90",
            "902",
            "9021",
            "90210",
            "90210-",
            "90210-0",
            "90210-01",
            "90210-012",
            "90210-0123");
        mask.ToString().Should().Be("90210-0123|");
    }

    /// <summary>
    /// A dash is only accepted once five digits have been typed.
    /// </summary>
    [Test]
    public void UsZipCode_IgnoresDashBeforeFiveDigits()
    {
        var mask = RegexMask.UsZipCode();
        mask.Insert("-");
        mask.Text.Should().BeEmpty();
        mask.Clear();
        mask.Insert("12-3");
        mask.Text.Should().Be("123");
    }

    /// <summary>
    /// Letters, whitespace, and punctuation other than the dash are rejected.
    /// </summary>
    [Test]
    public void UsZipCode_IgnoresInvalidCharacters()
    {
        var mask = RegexMask.UsZipCode();
        mask.Insert("9a0 2!1@0");
        mask.Text.Should().Be("90210");
        mask.Clear();
        mask.Insert("abc");
        mask.Text.Should().BeEmpty();
        mask.Clear();
        mask.Insert("90210\n");
        mask.Text.Should().Be("90210");
        mask.Text.IndexOf('\n').Should().Be(-1);
    }

    /// <summary>
    /// Digits typed beyond the ZIP+4 length are discarded.
    /// </summary>
    [Test]
    public void UsZipCode_IgnoresInputBeyondZipPlusFour()
    {
        var mask = RegexMask.UsZipCode();
        mask.Insert("9021001234");
        mask.ToString().Should().Be("90210-0123|");
    }

    /// <summary>
    /// Backspace removes the character before the caret and keeps the remaining text aligned.
    /// </summary>
    [Test]
    public void UsZipCode_Backspace()
    {
        var mask = RegexMask.UsZipCode();
        mask.SetText("902100123");
        mask.Text.Should().Be("90210-0123");
        mask.CaretPos = 10;
        mask.Backspace();
        mask.ToString().Should().Be("90210-012|");
        mask.Clear();
        mask.SetText("902100123");
        mask.CaretPos = 7;
        mask.Backspace();
        mask.ToString().Should().Be("90210-|123");
    }

    /// <summary>
    /// Delete removes the character after the caret and keeps the remaining text aligned.
    /// </summary>
    [Test]
    public void UsZipCode_Delete()
    {
        var mask = RegexMask.UsZipCode();
        mask.SetText("902100123");
        mask.CaretPos = 0;
        mask.Delete();
        mask.ToString().Should().Be("|02100-123");
    }
}
