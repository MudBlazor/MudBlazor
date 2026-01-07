// Copyright (c) MudBlazor 2024
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using MudBlazor.Addons.UnitTests.TestComponents.AddonExample;
using NUnit.Framework;

namespace MudBlazor.Addons.UnitTests.Components
{
    [TestFixture]
    public class AddonExampleComponentTests : BunitTest
    {
        [Test]
        public void AddonExampleComponent_Renders_WithDefaultValues()
        {
            var comp = Context.Render<MudAddonExampleComponent>();
            comp.Find("div").Should().NotBeNull("Component should render a div");
            comp.Find("h3").TextContent.Should().Be("Addon Example");
            comp.Find("p").TextContent.Should().Be("This is an example addon component.");
        }

        [Test]
        public async Task AddonExampleComponent_Renders_WithCustomTitle()
        {
            var comp = Context.Render<MudAddonExampleComponent>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Title, "Custom Title"));
            comp.Find("h3").TextContent.Should().Be("Custom Title");
        }

        [Test]
        public async Task AddonExampleComponent_Renders_WithCustomContent()
        {
            var comp = Context.Render<MudAddonExampleComponent>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Content, "Custom content"));
            comp.Find("p").TextContent.Should().Be("Custom content");
        }

        [Test]
        public void AddonExampleComponent_Test_Viewer_Component()
        {
            var comp = Context.Render<AddonExampleComponentTest>();
            comp.FindComponents<MudAddonExampleComponent>().Should().HaveCount(2, "Test viewer should render 2 addon example components");
        }
    }
}
