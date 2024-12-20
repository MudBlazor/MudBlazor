// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents.Chip;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ChipTests : BunitTest
    {
        [Test]
        public void Chip_ShouldRenderDivByDefault()
        {
            var comp = Context.RenderComponent<MudChip<string>>();

            var chip = comp.Find(".mud-chip");

            chip.TagName.Should().Be("DIV");

            chip.GetAttribute("tabindex").Should().Be("-1");
            chip.GetAttribute("href").Should().BeNull();
            chip.GetAttribute("target").Should().BeNull();
            chip.GetAttribute("type").Should().BeNull();
            chip.GetAttribute("rel").Should().BeNull();
        }

        [Test]
        [Combinatorial]
        public void Chip_ShouldRenderAnchorIfLinkSet(
            [Values("", "ASDF", "nofollow", "_blank")] string target,
            [Values(null, "noopener", "nofollow")] string rel)
        {

            var comp = Context.RenderComponent<MudChip<string>>(parameters => parameters
                .Add(p => p.Href, "https://example.com")
                .Add(p => p.Target, target)
                .Add(p => p.Rel, rel)
            );

            var chip = comp.Find(".mud-chip");

            chip.TagName.Should().Be("A");

            chip.GetAttribute("href").Should().Be("https://example.com");
            chip.GetAttribute("target").Should().Be(target);

            var expectedRel = rel ?? (target == "_blank" ? "noopener" : null);
            chip.GetAttribute("rel").Should().Be(expectedRel);
        }

        [Test]
        [Combinatorial]
        public void Chip_ShouldRenderButtonAndNotAnchorIfOnClickSet(
            [Values(null, "", "https://example.com")] string href,
            [Values(null, "", "ASDF", "_blank")] string target,
            [Values(null, "", "noopener", "nofollow")] string rel)
        {
            var comp = Context.RenderComponent<MudChip<string>>(parameters => parameters
                .Add(p => p.OnClick, () => { })
                .Add(p => p.Href, href)
                .Add(p => p.Target, target)
                .Add(p => p.Rel, rel)
            );

            var chip = comp.Find(".mud-chip");

            chip.TagName.Should().Be("BUTTON");

            chip.GetAttribute("tabindex").Should().Be("0");
            chip.GetAttribute("type").Should().Be("button");
            chip.GetAttribute("href").Should().BeNull();
            chip.GetAttribute("target").Should().BeNull();
            chip.GetAttribute("rel").Should().BeNull();
        }

        [Test]
        public void Chip_ShouldAllowUserDefinedAttributesToOverrideDefaults()
        {
            var userAttributes = new Dictionary<string, object>
            {
                { "tabindex", 5 },
                { "type", "submit" },
                { "data-test", "testValue" }
            };

            var comp = Context.RenderComponent<MudChip<string>>(parameters => parameters
                .Add(p => p.OnClick, () => { })
                .Add(p => p.UserAttributes, userAttributes)
            );

            var chip = comp.Find(".mud-chip");

            // User attributes should take precedence.
            chip.GetAttribute("tabindex").Should().Be("5");
            chip.GetAttribute("type").Should().Be("submit");
            chip.GetAttribute("data-test").Should().Be("testValue");
        }

        [Test]
        public void Chip_ShouldRenderAvatar()
        {
            var comp = Context.RenderComponent<ChipAvatarContentTest>();

            comp.Find("div.mud-chip").InnerHtml.Should().Contain("mud-avatar");
        }

        /// <summary>
        /// Clicks on the chip and tests if the OnClick event works
        /// </summary>
        [Test]
        public void Chip_OnClick()
        {
            var comp = Context.RenderComponent<ChipOnClickTest>();
            // print the generated html

            // chip should have mud-clickable and mud-ripple classes
            var chip = comp.Find("button.mud-chip");
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
        public void Chip_OnClose()
        {
            var comp = Context.RenderComponent<ChipOnClickTest>();
            // print the generated html

            // chip should have mud-clickable and mud-ripple classes
            var chip = comp.Find("button.mud-chip");
            chip.ClassName.Should().Contain("mud-clickable");
            chip.ClassName.Should().Contain("mud-ripple");

            // click on close button
            comp.Find("button.mud-chip-close-button").Click();

            var expectedEvent = comp.Find("#chip-click-test-expected-value");
            expectedEvent.InnerHtml.Should().Be("OnClose");
        }
    }
}
