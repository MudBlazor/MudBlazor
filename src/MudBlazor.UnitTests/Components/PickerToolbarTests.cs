// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class PickerToolbarTests : BunitTest
{
    [Test]
    public void PickerToolbar_ShouldBeLandscape_WhenStaticAndOrientationLandscape()
    {
        var component = Context.RenderComponent<MudPickerToolbar>(parameters => parameters
            .Add(p => p.PickerVariant, PickerVariant.Static)
            .Add(p => p.Orientation, Orientation.Landscape));

        var pickerToolbar = component.Instance;
        component.FindAll(".mud-picker-toolbar-landscape").Count.Should().Be(1);
    }

    [Test]
    [TestCase(PickerVariant.Inline)]
    [TestCase(PickerVariant.Dialog)]
    public void PickerToolbar_ShouldNotBeLandscape_WhenNonStaticAndOrientationLandscape(PickerVariant pickerVariant)
    {
        var component = Context.RenderComponent<MudPickerToolbar>(parameters => parameters
            .Add(p => p.PickerVariant, pickerVariant)
            .Add(p => p.Orientation, Orientation.Landscape));

        var pickerToolbar = component.Instance;
        component.FindAll(".mud-picker-toolbar-landscape").Count.Should().Be(0);
    }
}
