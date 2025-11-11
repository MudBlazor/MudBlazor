// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.UnitTests.Dummy;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Themes;

#nullable enable
[TestFixture]
public class MudThemeTests
{
    [Test]
    public void MudTheme_STJ_SourceGen_Serialization()
    {
        var originalMudTheme = new MudTheme
        {
            ZIndex = new ZIndex
            {
                Drawer = 5000
            }
        };

        var mudThemeType = typeof(MudTheme);
        var context = MudThemeSerializerContext.Default;

        var jsonString = System.Text.Json.JsonSerializer.Serialize(originalMudTheme, mudThemeType, context);
        var deserializeMudTheme = (MudTheme)System.Text.Json.JsonSerializer.Deserialize(jsonString, mudThemeType, context)!;

        deserializeMudTheme.ZIndex.Drawer.Should().Be(originalMudTheme.ZIndex.Drawer);
        deserializeMudTheme.Should().NotBeSameAs(originalMudTheme, "Objects have same values, but instances are different and has on custom Equals");
    }

    [Test]
    public void MudTheme_AllowsAssigningBasePaletteToLightPalette()
    {
        var basePalette = new Palette
        {
            Primary = Colors.Cyan.Default,
            Secondary = Colors.Orange.Accent2
        };

        var theme = new MudTheme
        {
            PaletteLight = basePalette
        };

        theme.PaletteLight.Should().BeSameAs(basePalette);
        theme.PaletteLight.PrimaryDarken.Should().Be(basePalette.Primary.ColorRgbDarken().ToString(MudColorOutputFormats.RGB));
        theme.PaletteLight.SecondaryLighten.Should().Be(basePalette.Secondary.ColorRgbLighten().ToString(MudColorOutputFormats.RGB));
    }

    [Test]
    public void MudTheme_AllowsAssigningBasePaletteToDarkPalette()
    {
        var basePalette = new Palette
        {
            Primary = Colors.Green.Darken1,
            Info = Colors.Blue.Accent2
        };

        var theme = new MudTheme
        {
            PaletteDark = basePalette
        };

        theme.PaletteDark.Should().BeSameAs(basePalette);
        theme.PaletteDark.PrimaryDarken.Should().Be(basePalette.Primary.ColorRgbDarken().ToString(MudColorOutputFormats.RGB));
        theme.PaletteDark.InfoLighten.Should().Be(basePalette.Info.ColorRgbLighten().ToString(MudColorOutputFormats.RGB));
    }
}
