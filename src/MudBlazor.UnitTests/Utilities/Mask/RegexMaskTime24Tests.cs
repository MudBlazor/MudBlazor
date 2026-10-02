// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class RegexMaskTime24Tests
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
    /// The time mask shows hours and minutes, adds seconds on request, and honors a custom mask character.
    /// </summary>
    [Test]
    public void Time24_Mask()
    {
        var mask = RegexMask.Time24();
        mask.Mask.Should().Be("00:00");
        mask.ToString().Should().Be("|");
        mask.Insert("0730");
        mask.Mask.Should().Be("00:00");
        mask = RegexMask.Time24(maskChar: '_');
        mask.Mask.Should().Be("__:__");
        mask = RegexMask.Time24(includeSeconds: true);
        mask.Mask.Should().Be("00:00:00");
    }

    /// <summary>
    /// Every intermediate state while typing a time is accepted and the colon appears on its own.
    /// </summary>
    [Test]
    public void Time24_AcceptsEveryTypedPrefix()
    {
        var mask = RegexMask.Time24();
        TypeOneByOne(mask, "0730").Should().Equal(
            "0",
            "07",
            "07:3",
            "07:30");
    }

    /// <summary>
    /// A colon typed by the user is kept instead of being inserted twice.
    /// </summary>
    [Test]
    public void Time24_AcceptsTypedColon()
    {
        var mask = RegexMask.Time24();
        TypeOneByOne(mask, "07:30").Should().Equal(
            "0",
            "07",
            "07:",
            "07:3",
            "07:30");
        mask.ToString().Should().Be("07:30|");
    }

    /// <summary>
    /// The last valid time of the day is accepted.
    /// </summary>
    [Test]
    public void Time24_AcceptsEndOfDay()
    {
        var mask = RegexMask.Time24();
        mask.Insert("2359");
        mask.ToString().Should().Be("23:59|");
        mask.Clear();
        mask.Insert("0000");
        mask.ToString().Should().Be("00:00|");
    }

    /// <summary>
    /// A single-digit hour is accepted and the colon is still inserted after it.
    /// </summary>
    [Test]
    public void Time24_AcceptsSingleDigitHour()
    {
        var mask = RegexMask.Time24();
        mask.Insert("930");
        mask.ToString().Should().Be("9:30|");
    }

    /// <summary>
    /// An hour above twenty-three is rejected, so the second digit starts the minutes instead.
    /// </summary>
    [Test]
    public void Time24_RejectsHourAboveTwentyThree()
    {
        var mask = RegexMask.Time24();
        mask.Insert("25");
        mask.Text.Should().Be("2:5");
        mask.Clear();
        mask.Insert("2400");
        mask.Text.Should().Be("2:40");
    }

    /// <summary>
    /// A minute above fifty-nine is rejected instead of being reinterpreted.
    /// </summary>
    [Test]
    public void Time24_RejectsMinuteAboveFiftyNine()
    {
        var mask = RegexMask.Time24();
        mask.Insert("07:61");
        mask.Text.Should().Be("07:1");
        mask.Clear();
        mask.Insert("0769");
        mask.Text.Should().Be("07");
    }

    /// <summary>
    /// Text made up only of colons is discarded.
    /// </summary>
    [Test]
    public void Time24_IgnoresColonOnlyInput()
    {
        var mask = RegexMask.Time24();
        mask.Insert(":");
        mask.Text.Should().BeEmpty();
    }

    /// <summary>
    /// Characters other than digits and the colon are rejected.
    /// </summary>
    [Test]
    public void Time24_IgnoresNonDigitCharacters()
    {
        var mask = RegexMask.Time24();
        mask.Insert("0a7b3c0");
        mask.Text.Should().Be("07:30");
        mask.Clear();
        mask.Insert("abc");
        mask.Text.Should().BeEmpty();
        mask.Clear();
        mask.Insert("0730\n");
        mask.Text.Should().Be("07:30");
        mask.Text.IndexOf('\n').Should().Be(-1);
    }

    /// <summary>
    /// Digits typed beyond the minutes are discarded when seconds are not enabled.
    /// </summary>
    [Test]
    public void Time24_IgnoresInputBeyondMinutes()
    {
        var mask = RegexMask.Time24();
        mask.Insert("073045");
        mask.ToString().Should().Be("07:30|");
    }

    /// <summary>
    /// Every intermediate state while typing a time with seconds is accepted.
    /// </summary>
    [Test]
    public void Time24_IncludeSecondsAcceptsEveryTypedPrefix()
    {
        var mask = RegexMask.Time24(includeSeconds: true);
        TypeOneByOne(mask, "073045").Should().Equal(
            "0",
            "07",
            "07:3",
            "07:30",
            "07:30:4",
            "07:30:45");
    }

    /// <summary>
    /// Colons typed by the user between all three fields are kept instead of being inserted twice.
    /// </summary>
    [Test]
    public void Time24_IncludeSecondsAcceptsTypedColons()
    {
        var mask = RegexMask.Time24(includeSeconds: true);
        mask.Insert("07:30:45");
        mask.ToString().Should().Be("07:30:45|");
        mask.Clear();
        mask.Insert("235959");
        mask.ToString().Should().Be("23:59:59|");
    }

    /// <summary>
    /// A second above fifty-nine is rejected and digits beyond the seconds are discarded.
    /// </summary>
    [Test]
    public void Time24_IncludeSecondsRejectsOutOfRangeInput()
    {
        var mask = RegexMask.Time24(includeSeconds: true);
        mask.Insert("07306");
        mask.Text.Should().Be("07:30");
        mask.Clear();
        mask.Insert("07304567");
        mask.ToString().Should().Be("07:30:45|");
    }

    /// <summary>
    /// Backspace removes the character before the caret and keeps the remaining text aligned.
    /// </summary>
    [Test]
    public void Time24_Backspace()
    {
        var mask = RegexMask.Time24();
        mask.SetText("0730");
        mask.Text.Should().Be("07:30");
        mask.CaretPos = 5;
        mask.Backspace();
        mask.ToString().Should().Be("07:3|");
        mask.Clear();
        mask.SetText("0730");
        mask.CaretPos = 3;
        mask.Backspace();
        mask.ToString().Should().Be("0|3:0");
    }

    /// <summary>
    /// Delete removes the character after the caret and keeps the remaining text aligned.
    /// </summary>
    [Test]
    public void Time24_Delete()
    {
        var mask = RegexMask.Time24();
        mask.SetText("0730");
        mask.CaretPos = 0;
        mask.Delete();
        mask.ToString().Should().Be("|7:30");
    }
}
