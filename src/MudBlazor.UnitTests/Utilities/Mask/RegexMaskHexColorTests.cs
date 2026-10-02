// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class RegexMaskHexColorTests
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
    /// The color mask shows the longest accepted form and honors a custom mask character.
    /// </summary>
    [Test]
    public void HexColor_Mask()
    {
        var mask = RegexMask.HexColor();
        mask.Mask.Should().Be("#XXXXXXXX");
        mask.ToString().Should().Be("|");
        mask.Insert("1A2B3C");
        mask.Mask.Should().Be("#XXXXXXXX");
        mask = RegexMask.HexColor(maskChar: '_');
        mask.Mask.Should().Be("#________");
    }

    /// <summary>
    /// The leading hash is inserted as soon as the first hexadecimal digit is typed.
    /// </summary>
    [Test]
    public void HexColor_AcceptsEveryTypedPrefix()
    {
        var mask = RegexMask.HexColor();
        TypeOneByOne(mask, "1A2B3C").Should().Equal(
            "#1",
            "#1A",
            "#1A2",
            "#1A2B",
            "#1A2B3",
            "#1A2B3C");
    }

    /// <summary>
    /// A hash typed by the user is kept instead of being inserted twice.
    /// </summary>
    [Test]
    public void HexColor_AcceptsTypedHash()
    {
        var mask = RegexMask.HexColor();
        TypeOneByOne(mask, "#1A2B3C").Should().Equal(
            "",
            "#1",
            "#1A",
            "#1A2",
            "#1A2B",
            "#1A2B3",
            "#1A2B3C");
        mask.ToString().Should().Be("#1A2B3C|");
        mask.Clear();
        mask.Insert("##1A");
        mask.Text.Should().Be("#1A");
    }

    /// <summary>
    /// The three, four, six, and eight digit CSS forms are all accepted.
    /// </summary>
    [Test]
    public void HexColor_AcceptsShorthandAndAlphaForms()
    {
        var mask = RegexMask.HexColor();
        mask.Insert("abc");
        mask.Text.Should().Be("#abc");
        mask.Clear();
        mask.Insert("abcd");
        mask.Text.Should().Be("#abcd");
        mask.Clear();
        mask.Insert("1A2B3C");
        mask.Text.Should().Be("#1A2B3C");
        mask.Clear();
        mask.Insert("1A2B3C4D");
        mask.Text.Should().Be("#1A2B3C4D");
    }

    /// <summary>
    /// Characters outside the hexadecimal range are rejected.
    /// </summary>
    [Test]
    public void HexColor_IgnoresNonHexCharacters()
    {
        var mask = RegexMask.HexColor();
        mask.Insert("1g2z3");
        mask.Text.Should().Be("#123");
        mask.Clear();
        mask.Insert("xyz");
        mask.Text.Should().BeEmpty();
        mask.Clear();
        mask.Insert("1A2B3C\n");
        mask.Text.Should().Be("#1A2B3C");
        mask.Text.IndexOf('\n').Should().Be(-1);
    }

    /// <summary>
    /// Text made up only of the hash is discarded.
    /// </summary>
    [Test]
    public void HexColor_IgnoresHashOnlyInput()
    {
        var mask = RegexMask.HexColor();
        mask.Insert("#");
        mask.Text.Should().BeEmpty();
    }

    /// <summary>
    /// Digits typed beyond the eighth are discarded.
    /// </summary>
    [Test]
    public void HexColor_IgnoresInputBeyondEightDigits()
    {
        var mask = RegexMask.HexColor();
        mask.Insert("1A2B3C4D5E");
        mask.ToString().Should().Be("#1A2B3C4D|");
    }

    /// <summary>
    /// Backspace removes the character before the caret and keeps the remaining text aligned.
    /// </summary>
    [Test]
    public void HexColor_Backspace()
    {
        var mask = RegexMask.HexColor();
        mask.SetText("1A2B3C");
        mask.Text.Should().Be("#1A2B3C");
        mask.CaretPos = 7;
        mask.Backspace();
        mask.ToString().Should().Be("#1A2B3|");
    }

    /// <summary>
    /// Delete removes the character after the caret and keeps the remaining text aligned.
    /// </summary>
    [Test]
    public void HexColor_Delete()
    {
        var mask = RegexMask.HexColor();
        mask.SetText("1A2B3C");
        mask.CaretPos = 0;
        mask.Delete();
        mask.ToString().Should().Be("#|A2B3C");
    }
}
