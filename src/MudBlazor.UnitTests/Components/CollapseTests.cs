// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using MudBlazor.UnitTests.TestComponents.Collapse;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class CollapseTests : BunitTest
    {
        [Test]
        public async Task Collapse_TwoWayBinding_Test1()
        {
            var comp = Context.Render<CollapseBindingTest>();
            var collapse = comp.FindComponent<MudCollapse>();

            collapse.Markup.Should().Contain("mud-collapse-entered");

            IElement Button() => comp.Find("#outside_btn");

            IRenderedComponent<MudSwitch<bool>> MudSwitch() => comp.FindComponent<MudSwitch<bool>>();
            // Initial state is expanded
            MudSwitch().Find("input").HasAttribute("checked").Should().BeTrue();

            // Collapse via button
            await Button().ClickAsync();
            MudSwitch().Find("input").HasAttribute("checked").Should().BeFalse();

            // Expand via button
            await Button().ClickAsync();
            MudSwitch().Find("input").HasAttribute("checked").Should().BeTrue();

            // Collapse via switch
            await MudSwitch().Find("input").ChangeAsync(false);
            MudSwitch().Find("input").HasAttribute("checked").Should().BeFalse();

            // Expand via switch
            await MudSwitch().Find("input").ChangeAsync(true);
            MudSwitch().Find("input").HasAttribute("checked").Should().BeTrue();
        }

        [Test]
        public async Task Collapse_OnAnimationEnd_ShouldIgnoreChildTransitionEnd()
        {
            var comp = Context.Render<CollapseAnimationEndChildTransitionTest>();
            comp.Find("#animation_end_count").TextContent.Should().Be("0");

            await comp.Find("#inner").TriggerEventAsync("ontransitionend", EventArgs.Empty);

            comp.Find("#animation_end_count").TextContent.Should().Be("0");
        }
    }
}
