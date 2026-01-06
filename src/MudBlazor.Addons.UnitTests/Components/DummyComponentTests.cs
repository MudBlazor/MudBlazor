// Copyright (c) MudBlazor 2024
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using MudBlazor.Addons.UnitTests.TestComponents.DummyComponent;
using NUnit.Framework;

namespace MudBlazor.Addons.UnitTests.Components
{
    [TestFixture]
    public class DummyComponentTests : BunitTest
    {
        [Test]
        public void DummyComponent_Renders_WithDefaultValues()
        {
            var comp = Context.Render<MudDummyComponent>();
            comp.Find("div").Should().NotBeNull("Component should render a div");
            comp.Find("h3").TextContent.Should().Be("Dummy Component");
            comp.Find("p").TextContent.Should().Be("This is a dummy component for testing.");
        }

        [Test]
        public async Task DummyComponent_Renders_WithCustomTitle()
        {
            var comp = Context.Render<MudDummyComponent>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Title, "Custom Title"));
            comp.Find("h3").TextContent.Should().Be("Custom Title");
        }

        [Test]
        public async Task DummyComponent_Renders_WithCustomContent()
        {
            var comp = Context.Render<MudDummyComponent>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Content, "Custom content"));
            comp.Find("p").TextContent.Should().Be("Custom content");
        }

        [Test]
        public void DummyComponent_Test_Viewer_Component()
        {
            var comp = Context.Render<DummyComponentTest>();
            comp.FindComponents<MudDummyComponent>().Should().HaveCount(2, "Test viewer should render 2 dummy components");
        }
    }
}
