// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using ColorCode.Styling;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class IconButtonTests : BunitTest
{
    [Test]
    public void WithFullWidth_ThenStretched_AfterwardsWithoutFullWidth_ThenNotStretched()
    {
        // Arrange

        var comp = Context.RenderComponent<MudIconButton>(parameters => parameters.Add(c => c.FullWidth, true));

        // Assert

        comp.Find("button").ClassList.Should().Contain("mud-width-full");

        // Act

        comp.SetParametersAndRender(parameters => parameters.Add(c => c.FullWidth, false));

        // Assert

        comp.Find("button").ClassList.Should().NotContain("mud-width-full");
    }

    [Test]
    public void WithoutFullWidth_ThenNotStretched_AfterwardsWithFullWidth_ThenStretched()
    {
        // Arrange

        var comp = Context.RenderComponent<MudIconButton>(parameters => parameters.Add(c => c.FullWidth, false));

        // Assert

        comp.Find("button").ClassList.Should().NotContain("mud-width-full");

        // Act

        comp.SetParametersAndRender(parameters => parameters.Add(c => c.FullWidth, true));

        // Assert

        comp.Find("button").ClassList.Should().Contain("mud-width-full");
    }
}
