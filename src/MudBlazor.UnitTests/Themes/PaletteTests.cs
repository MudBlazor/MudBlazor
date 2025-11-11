// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Themes;

#nullable enable
[TestFixture]
public class PaletteTests
{
    [Test]
    public void ComputedDerivatives()
    {
        var palette = new Palette
        {
            Primary = "#123456"
        };

        palette.PrimaryDarken.Should().Be(palette.Primary.ColorRgbDarken().ToString(MudColorOutputFormats.RGB));
        palette.PrimaryLighten.Should().Be(palette.Primary.ColorRgbLighten().ToString(MudColorOutputFormats.RGB));
    }

    [Test]
    public void ComputedPrimaryDerivatives_RespectManualOverrides()
    {
        var palette = new Palette
        {
            Primary = "#123456",
            PrimaryDarken = "#000001",
            PrimaryLighten = "#abcdef"
        };

        palette.PrimaryDarken.Should().Be("#000001");
        palette.PrimaryLighten.Should().Be("#abcdef");

        palette.Primary = Colors.Green.Accent2;

        palette.PrimaryDarken.Should().Be("#000001");
        palette.PrimaryLighten.Should().Be("#abcdef");
    }

    [Test]
    public void WithExpression_ProducesIndependentPalette()
    {
        var palette = new Palette
        {
            Primary = Colors.Blue.Default,
            Secondary = Colors.Pink.Accent2
        };

        var clone = palette with
        {
            Primary = Colors.Green.Default
        };

        clone.Should().NotBeSameAs(palette);
        clone.Primary.Should().Be(new MudColor(Colors.Green.Default));
        clone.PrimaryDarken.Should().Be(clone.Primary.ColorRgbDarken().ToString(MudColorOutputFormats.RGB));

        palette.Primary.Should().Be(new MudColor(Colors.Blue.Default));
        palette.PrimaryDarken.Should().Be(palette.Primary.ColorRgbDarken().ToString(MudColorOutputFormats.RGB));
    }
}
