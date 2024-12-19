// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.Chip;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ChipTests : BunitTest
    {
        /// <summary>
        /// Clicks on the chip and tests if the OnClick event works
        /// </summary>
        [Test]
        public void Chip_OnClick_Test()
        {
            var comp = Context.RenderComponent<ChipOnClickTest>();
            // print the generated html

            // chip should have mud-clickable and mud-ripple classes
            var chip = comp.Find("div.mud-chip");
            chip.ClassName.Should().Contain("mud-clickable");
            chip.ClassName.Should().Contain("mud-ripple");

            // click on chip
            chip.Click();

            var expectedEvent = comp.Find("#chip-click-test-expected-value");
            expectedEvent.InnerHtml.Should().Be("OnClick");
        }

        /// <summary>
        /// Clicks on the close button and tests if the OnClose event works
        /// </summary>
        [Test]
        public void Chip_OnClose_Test()
        {
            var comp = Context.RenderComponent<ChipOnClickTest>();
            // print the generated html

            // chip should have mud-clickable and mud-ripple classes
            var chip = comp.Find("div.mud-chip");
            chip.ClassName.Should().Contain("mud-clickable");
            chip.ClassName.Should().Contain("mud-ripple");

            // click on close button
            comp.Find("button.mud-chip-close-button").Click();

            var expectedEvent = comp.Find("#chip-click-test-expected-value");
            expectedEvent.InnerHtml.Should().Be("OnClose");
        }

        [Test]
        public async Task Chip_Link_Test()
        {
            var comp = Context.RenderComponent<ChipLinkTest>();
            var chip = comp.FindComponent<MudChip<string>>();

            await comp.InvokeAsync(() => ((IMudStateHasChanged)chip.Instance).StateHasChanged());
            await comp.InvokeAsync(() => chip.Instance.OnClickAsync(new MouseEventArgs()));

            comp.WaitForAssertion(() => comp.Find("#chip-click-test-expected-value").InnerHtml.Should().Be(""));
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            await comp.InvokeAsync(() => chip.Instance.Target = "_blank");
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
            await comp.InvokeAsync(() => chip.Instance.OnClickAsync(new MouseEventArgs()));

            comp.WaitForAssertion(() => comp.Find("#chip-click-test-expected-value").InnerHtml.Should().Be(""));
        }

        [Test]
        [Combinatorial]
        public void Chip_ShouldRenderAnchorIfLinkSet([Values("", "ASDF", "_blank")] string target, [Values(true, false)] bool disabled)
        {
            var comp = Context.RenderComponent<MudChip<string>>(parameters => parameters
                .Add(p => p.Href, "https://example.com")
                .Add(p => p.Target, target)
                .Add(p => p.Disabled, disabled)
            );

            // The chip should be rendered as an anchor tag if the href is set, regardless of the other parameters.
            comp.Find(".mud-chip").TagName.Should().Be("A");
        }

        [Test]
        public void Chip_Should_Render_Avatar_Test()
        {
            var comp = Context.RenderComponent<ChipAvatarContentTest>();

            comp.Find("div.mud-chip").InnerHtml.Should().Contain("mud-avatar");
        }

        [Test]
        [TestCase(null, "_blank", "noopener")]
        [TestCase("nofollow", "_blank", "nofollow")]
        [TestCase(null, "_self", null)]
        public void RelShouldBeExpectedValue(string rel, string target, string expected)
        {
            var comp = Context.RenderComponent<MudChip<string>>(parameters => parameters
                .Add(p => p.Rel, rel)
                .Add(p => p.Target, target)
            );

            comp.Find(".mud-chip").GetAttribute("rel").Should().Be(expected);
        }
    }
}
