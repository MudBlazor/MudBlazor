// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;

namespace MudBlazor.UnitTests.Components;
public class PickerToolbarTests : BunitTest
{
    [Test]
    public void PickerToolbar_ShouldBeLandscape_WhenStaticAndOrientationLandscape()
    {
        var component = Context.Render<MudPickerToolbar>(parameters => parameters
            .Add(p => p.PickerVariant, PickerVariant.Static)
            .Add(p => p.Orientation, Orientation.Landscape));

        var pickerToolbar = component.Instance;
        component.FindAll(".mud-picker-toolbar-landscape").Count.Should().Be(1);
    }

    [Test]
    [Arguments(PickerVariant.Inline)]
    [Arguments(PickerVariant.Dialog)]
    public void PickerToolbar_ShouldNotBeLandscape_WhenNonStaticAndOrientationLandscape(PickerVariant pickerVariant)
    {
        var component = Context.Render<MudPickerToolbar>(parameters => parameters
            .Add(p => p.PickerVariant, pickerVariant)
            .Add(p => p.Orientation, Orientation.Landscape));

        var pickerToolbar = component.Instance;
        component.FindAll(".mud-picker-toolbar-landscape").Count.Should().Be(0);
    }
}