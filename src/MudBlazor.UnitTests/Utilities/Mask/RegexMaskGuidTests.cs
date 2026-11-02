// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class RegexMaskGuidTests
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
    /// The identifier mask shows the 8-4-4-4-12 structure and honors a custom mask character.
    /// </summary>
    [Test]
    public void Guid_Mask()
    {
        var mask = RegexMask.Guid();
        mask.Mask.Should().Be("XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX");
        mask.ToString().Should().Be("|");
        mask.Insert("2f9d8e7a1b3c4d5e6f708192a3b4c5d6");
        mask.Mask.Should().Be("XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX");
        mask = RegexMask.Guid(maskChar: '_');
        mask.Mask.Should().Be("________-____-____-____-____________");
    }

    /// <summary>
    /// Every intermediate state while typing a full identifier is accepted and the dashes appear on their own.
    /// </summary>
    [Test]
    public void Guid_AcceptsEveryTypedPrefix()
    {
        var mask = RegexMask.Guid();
        TypeOneByOne(mask, "2f9d8e7a1b3c4d5e6f708192a3b4c5d6").Should().Equal(
            "2",
            "2f",
            "2f9",
            "2f9d",
            "2f9d8",
            "2f9d8e",
            "2f9d8e7",
            "2f9d8e7a",
            "2f9d8e7a-1",
            "2f9d8e7a-1b",
            "2f9d8e7a-1b3",
            "2f9d8e7a-1b3c",
            "2f9d8e7a-1b3c-4",
            "2f9d8e7a-1b3c-4d",
            "2f9d8e7a-1b3c-4d5",
            "2f9d8e7a-1b3c-4d5e",
            "2f9d8e7a-1b3c-4d5e-6",
            "2f9d8e7a-1b3c-4d5e-6f",
            "2f9d8e7a-1b3c-4d5e-6f7",
            "2f9d8e7a-1b3c-4d5e-6f70",
            "2f9d8e7a-1b3c-4d5e-6f70-8",
            "2f9d8e7a-1b3c-4d5e-6f70-81",
            "2f9d8e7a-1b3c-4d5e-6f70-819",
            "2f9d8e7a-1b3c-4d5e-6f70-8192",
            "2f9d8e7a-1b3c-4d5e-6f70-8192a",
            "2f9d8e7a-1b3c-4d5e-6f70-8192a3",
            "2f9d8e7a-1b3c-4d5e-6f70-8192a3b",
            "2f9d8e7a-1b3c-4d5e-6f70-8192a3b4",
            "2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c",
            "2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c5",
            "2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c5d",
            "2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c5d6");
    }

    /// <summary>
    /// Dashes typed by the user at the block boundaries are kept instead of being inserted twice.
    /// </summary>
    [Test]
    public void Guid_AcceptsTypedDashes()
    {
        var mask = RegexMask.Guid();
        mask.Insert("2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c5d6");
        mask.ToString().Should().Be("2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c5d6|");
    }

    /// <summary>
    /// Uppercase hexadecimal digits are accepted as typed.
    /// </summary>
    [Test]
    public void Guid_AcceptsUppercaseHexDigits()
    {
        var mask = RegexMask.Guid();
        mask.Insert("2F9D8E7A1B3C4D5E6F708192A3B4C5D6");
        mask.ToString().Should().Be("2F9D8E7A-1B3C-4D5E-6F70-8192A3B4C5D6|");
    }

    /// <summary>
    /// A dash is only accepted once the block before it is complete.
    /// </summary>
    [Test]
    public void Guid_IgnoresDashBeforeBlockIsComplete()
    {
        var mask = RegexMask.Guid();
        mask.Insert("-");
        mask.Text.Should().BeEmpty();
        mask.Clear();
        mask.Insert("1-2");
        mask.Text.Should().Be("12");
        mask.Clear();
        mask.Insert("2f9d8e7a-1b3-c");
        mask.Text.Should().Be("2f9d8e7a-1b3c");
    }

    /// <summary>
    /// Characters outside the hexadecimal range are rejected.
    /// </summary>
    [Test]
    public void Guid_IgnoresNonHexCharacters()
    {
        var mask = RegexMask.Guid();
        mask.Insert("1g2z3");
        mask.Text.Should().Be("123");
        mask.Clear();
        mask.Insert("xyz");
        mask.Text.Should().BeEmpty();
        mask.Clear();
        mask.Insert("2f9d8e7a1b3c4d5e6f708192a3b4c5d6\n");
        mask.Text.Should().Be("2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c5d6");
        mask.Text.IndexOf('\n').Should().Be(-1);
    }

    /// <summary>
    /// Digits typed beyond the thirty-two hexadecimal digits are discarded.
    /// </summary>
    [Test]
    public void Guid_IgnoresInputBeyondCanonicalLength()
    {
        var mask = RegexMask.Guid();
        mask.Insert("2f9d8e7a1b3c4d5e6f708192a3b4c5d6ffff");
        mask.ToString().Should().Be("2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c5d6|");
    }

    /// <summary>
    /// The braced form is not accepted and its braces are discarded.
    /// </summary>
    [Test]
    public void Guid_IgnoresBraces()
    {
        var mask = RegexMask.Guid();
        mask.Insert("{2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c5d6}");
        mask.Text.Should().Be("2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c5d6");
    }

    /// <summary>
    /// Backspace removes the character before the caret and keeps the remaining text aligned.
    /// </summary>
    [Test]
    public void Guid_Backspace()
    {
        var mask = RegexMask.Guid();
        mask.SetText("2f9d8e7a1b3c4d5e6f708192a3b4c5d6");
        mask.Text.Should().Be("2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c5d6");
        mask.CaretPos = 36;
        mask.Backspace();
        mask.ToString().Should().Be("2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c5d|");
        mask.Clear();
        mask.SetText("2f9d8e7a1b3c4d5e6f708192a3b4c5d6");
        mask.CaretPos = 9;
        mask.Backspace();
        mask.ToString().Should().Be("2f9d8e7|1-b3c4-d5e6-f708-192a3b4c5d6");
    }

    /// <summary>
    /// Delete removes the character after the caret and keeps the remaining text aligned.
    /// </summary>
    [Test]
    public void Guid_Delete()
    {
        var mask = RegexMask.Guid();
        mask.SetText("2f9d8e7a1b3c4d5e6f708192a3b4c5d6");
        mask.CaretPos = 0;
        mask.Delete();
        mask.ToString().Should().Be("|f9d8e7a1-b3c4-d5e6-f708-192a3b4c5d6");
    }
}
