// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.UnitTests.Dummy;
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
}
