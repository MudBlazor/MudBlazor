// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Mask;

[TestFixture]
public class RegexMaskMacAddressTests
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
    /// The MAC address mask shows six pairs of digits and honors a custom mask character and separator.
    /// </summary>
    [Test]
    public void MacAddress_Mask()
    {
        var mask = RegexMask.MacAddress();
        mask.Mask.Should().Be("XX:XX:XX:XX:XX:XX");
        mask.ToString().Should().Be("|");
        mask.Insert("1A2B3C4D5E6F");
        mask.Mask.Should().Be("XX:XX:XX:XX:XX:XX");
        mask = RegexMask.MacAddress(maskChar: '_');
        mask.Mask.Should().Be("__:__:__:__:__:__");
        mask = RegexMask.MacAddress(separator: '-');
        mask.Mask.Should().Be("XX-XX-XX-XX-XX-XX");
    }

    /// <summary>
    /// Every intermediate state while typing a full MAC address is accepted.
    /// </summary>
    [Test]
    public void MacAddress_AcceptsEveryTypedPrefix()
    {
        var mask = RegexMask.MacAddress();
        TypeOneByOne(mask, "1A2B3C4D5E6F").Should().Equal(
            "1",
            "1A",
            "1A:2",
            "1A:2B",
            "1A:2B:3",
            "1A:2B:3C",
            "1A:2B:3C:4",
            "1A:2B:3C:4D",
            "1A:2B:3C:4D:5",
            "1A:2B:3C:4D:5E",
            "1A:2B:3C:4D:5E:6",
            "1A:2B:3C:4D:5E:6F");
    }

    /// <summary>
    /// A separator typed by the user is kept instead of being inserted twice.
    /// </summary>
    [Test]
    public void MacAddress_AcceptsTypedSeparator()
    {
        var mask = RegexMask.MacAddress();
        TypeOneByOne(mask, "1A:2B:3C:4D:5E:6F").Should().Equal(
            "1",
            "1A",
            "1A:",
            "1A:2",
            "1A:2B",
            "1A:2B:",
            "1A:2B:3",
            "1A:2B:3C",
            "1A:2B:3C:",
            "1A:2B:3C:4",
            "1A:2B:3C:4D",
            "1A:2B:3C:4D:",
            "1A:2B:3C:4D:5",
            "1A:2B:3C:4D:5E",
            "1A:2B:3C:4D:5E:",
            "1A:2B:3C:4D:5E:6",
            "1A:2B:3C:4D:5E:6F");
        mask.ToString().Should().Be("1A:2B:3C:4D:5E:6F|");
    }

    /// <summary>
    /// Lowercase hexadecimal digits are accepted as typed.
    /// </summary>
    [Test]
    public void MacAddress_AcceptsLowercaseHexDigits()
    {
        var mask = RegexMask.MacAddress();
        mask.Insert("aabbccddeeff");
        mask.ToString().Should().Be("aa:bb:cc:dd:ee:ff|");
    }

    /// <summary>
    /// Characters outside the hexadecimal range are rejected.
    /// </summary>
    [Test]
    public void MacAddress_IgnoresNonHexCharacters()
    {
        var mask = RegexMask.MacAddress();
        mask.Insert("1G2H3Z");
        mask.Text.Should().Be("12:3");
        mask.Clear();
        mask.Insert("xyz");
        mask.Text.Should().BeEmpty();
        mask.Clear();
        mask.Insert("1A2B3C4D5E6F\n");
        mask.Text.Should().Be("1A:2B:3C:4D:5E:6F");
        mask.Text.IndexOf('\n').Should().Be(-1);
    }

    /// <summary>
    /// Text made up only of separators is discarded.
    /// </summary>
    [Test]
    public void MacAddress_IgnoresSeparatorOnlyInput()
    {
        var mask = RegexMask.MacAddress();
        mask.Insert(":");
        mask.Text.Should().BeEmpty();
    }

    /// <summary>
    /// Digits typed beyond the sixth pair are discarded.
    /// </summary>
    [Test]
    public void MacAddress_IgnoresInputBeyondSixPairs()
    {
        var mask = RegexMask.MacAddress();
        mask.Insert("1A2B3C4D5E6F7A");
        mask.ToString().Should().Be("1A:2B:3C:4D:5E:6F|");
    }

    /// <summary>
    /// A custom separator is inserted automatically and the default separator is then rejected.
    /// </summary>
    [Test]
    public void MacAddress_CustomSeparator()
    {
        var mask = RegexMask.MacAddress(separator: '-');
        mask.Insert("1A2B3C4D5E6F");
        mask.ToString().Should().Be("1A-2B-3C-4D-5E-6F|");
        mask.Clear();
        mask.Insert("1A:2B");
        mask.Text.Should().Be("1A-2B");
    }

    /// <summary>
    /// Backspace removes the character before the caret and keeps the remaining text aligned.
    /// </summary>
    [Test]
    public void MacAddress_Backspace()
    {
        var mask = RegexMask.MacAddress();
        mask.SetText("1A2B3C");
        mask.Text.Should().Be("1A:2B:3C");
        mask.CaretPos = 8;
        mask.Backspace();
        mask.ToString().Should().Be("1A:2B:3|");
    }

    /// <summary>
    /// Delete removes the character after the caret and keeps the remaining text aligned.
    /// </summary>
    [Test]
    public void MacAddress_Delete()
    {
        var mask = RegexMask.MacAddress();
        mask.SetText("1A2B3C");
        mask.CaretPos = 0;
        mask.Delete();
        mask.ToString().Should().Be("|A:2B:3C");
    }
}
