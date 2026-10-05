// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
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
    public void MudTheme_STJ_SourceGen_PreservesPolymorphicPaletteTypeAndColors()
    {
        // Palette is abstract with [JsonDerivedType] discriminators; round-trip must keep
        // the concrete subtype and the custom MudColor value.
        var originalMudTheme = new MudTheme
        {
            PaletteLight = new PaletteLight { Primary = "#112233" },
            PaletteDark = new PaletteDark { Primary = "#abcdef" }
        };

        var mudThemeType = typeof(MudTheme);
        var context = MudThemeSerializerContext.Default;

        var jsonString = System.Text.Json.JsonSerializer.Serialize(originalMudTheme, mudThemeType, context);
        var deserializeMudTheme = (MudTheme)System.Text.Json.JsonSerializer.Deserialize(jsonString, mudThemeType, context)!;

        deserializeMudTheme.PaletteLight.Should().BeOfType<PaletteLight>();
        deserializeMudTheme.PaletteDark.Should().BeOfType<PaletteDark>();
        deserializeMudTheme.PaletteLight.Primary.Should().Be(new MudColor("#112233"));
        deserializeMudTheme.PaletteDark.Primary.Should().Be(new MudColor("#abcdef"));
    }

    [Test]
    public void MudTheme_STJ_SourceGen_PreservesExplicitlySetDarken()
    {
        // PrimaryDarken is normally computed from Primary, but an explicitly set value
        // must survive the round-trip rather than reverting to the computed default.
        var explicitDarken = new MudColor("#010203").ToString(MudColorOutputFormats.RGB);
        var originalMudTheme = new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#594AE2",
                PrimaryDarken = explicitDarken
            }
        };

        var mudThemeType = typeof(MudTheme);
        var context = MudThemeSerializerContext.Default;

        var jsonString = System.Text.Json.JsonSerializer.Serialize(originalMudTheme, mudThemeType, context);
        var deserializeMudTheme = (MudTheme)System.Text.Json.JsonSerializer.Deserialize(jsonString, mudThemeType, context)!;

        deserializeMudTheme.PaletteLight.PrimaryDarken.Should().Be(explicitDarken);
        // Sanity: explicit value differs from what Primary would compute.
        deserializeMudTheme.PaletteLight.PrimaryDarken.Should().NotBe(new MudColor("#594AE2").ColorRgbDarken().ToString(MudColorOutputFormats.RGB));
    }
}
