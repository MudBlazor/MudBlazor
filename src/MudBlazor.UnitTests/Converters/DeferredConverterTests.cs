// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Converters;

#nullable enable
[TestFixture]
internal class DeferredConverterTests
{
    [Test]
    public void Convert_Throws_WhenForwardNotSet()
    {
        var conv = new DeferredConverter<int, string>();

        Action act = () => conv.Convert(1);

        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("Conversion not initialized.");
    }

    [Test]
    public void ConvertBack_Throws_WhenBackwardNotSet()
    {
        var conv = new DeferredConverter<int, string>();

        Action act = () => conv.ConvertBack("1");

        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("Reverse conversion not initialized.");
    }

    [Test]
    public void SetForward_AllowsConversion_ButConvertBackStillThrowsUntilBackwardIsSet()
    {
        var conv = new DeferredConverter<int, string>();

        conv.SetForward(i => (i * 2).ToString());

        conv.Convert(21).Should().Be("42");

        Action act = () => conv.ConvertBack("42");
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("Reverse conversion not initialized.");

        conv.SetBackward(s => int.Parse(s) / 2);
        conv.ConvertBack("100").Should().Be(50);
    }

    [Test]
    public void Set_SetsBothDelegates_AndTheyWorkAsExpected()
    {
        var conv = new DeferredConverter<int, string>();

        conv.Set(
            forward: i => (i + 1).ToString(),
            backward: s => int.Parse(s) - 1);

        conv.Convert(9).Should().Be("10");
        conv.ConvertBack("20").Should().Be(19);
    }

    [Test]
    public void SettingDelegatesAgain_ReplacesPreviousBehavior()
    {
        var conv = new DeferredConverter<int, string>();

        conv.Set(i => i.ToString(), int.Parse);
        conv.Convert(7).Should().Be("7");
        conv.ConvertBack("7").Should().Be(7);

        // Replace forward only
        conv.SetForward(i => (i + 2).ToString());
        conv.Convert(7).Should().Be("9");

        // Replace backward only
        conv.SetBackward(s => int.Parse(s) + 3);
        conv.ConvertBack("4").Should().Be(7);
    }
}
