// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
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
            var comp = Context.Render<MudChip<string>>();

            var chip = comp.Find(".mud-chip");

            chip.TagName.Should().Be("DIV");

            chip.GetAttribute("tabindex").Should().Be("-1");
            chip.GetAttribute("href").Should().BeNull();
            chip.GetAttribute("target").Should().BeNull();
            chip.GetAttribute("type").Should().BeNull();
            chip.GetAttribute("rel").Should().BeNull();
        }

        [Test]
        public void Chip_PlainChips_ShouldNotSubscribeToKeyInterceptor()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();

            for (var i = 0; i < 20; i++)
            {
                Context.Render<MudChip<string>>();
            }

            keyInterceptorService.ObserversCount.Should().Be(0);
        }

        [Test]
        public void Chip_InteractiveChips_ShouldSubscribeToKeyInterceptor()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();

            Context.Render<MudChip<string>>(parameters => parameters
                .Add(p => p.OnClick, () => { }));
            Context.Render<MudChip<string>>(parameters => parameters
                .Add(p => p.OnClose, () => { }));

            keyInterceptorService.ObserversCount.Should().Be(2);
        }

        [Test]
        public async Task Chip_KeyInterceptorSubscription_ShouldFollowParameterTransitions()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<MudChip<string>>();

            keyInterceptorService.ObserversCount.Should().Be(0);

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.OnClick, () => { }));
            keyInterceptorService.ObserversCount.Should().Be(1);

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Disabled, true));
            keyInterceptorService.ObserversCount.Should().Be(0);

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Disabled, false));
            keyInterceptorService.ObserversCount.Should().Be(1);
        }

        [Test]
        public async Task Chip_KeyboardInterceptor_ShouldInvokeClickAndClose()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var clicked = 0;
            var closed = 0;
            var clickable = Context.Render<MudChip<string>>(parameters => parameters
                .Add(p => p.OnClick, () => clicked++)
                .Add(p => p.OnClose, () => { }));
            var closable = Context.Render<MudChip<string>>(parameters => parameters
                .Add(p => p.OnClose, () => closed++));
            var clickableId = clickable.Find(".mud-chip-container").GetAttribute("id")!;
            var closableId = closable.Find(".mud-chip-container").GetAttribute("id")!;

            await clickable.InvokeAsync(() => keyInterceptorService.OnKeyDown(clickableId, new KeyboardEventArgs { Key = " " }));
            await closable.InvokeAsync(() => keyInterceptorService.OnKeyDown(closableId, new KeyboardEventArgs { Key = "Delete" }));
            await closable.InvokeAsync(() => keyInterceptorService.OnKeyDown(closableId, new KeyboardEventArgs { Key = "Backspace" }));

            clicked.Should().Be(1);
            closed.Should().Be(2);
        }

        [Test]
        [Combinatorial]
        public void Chip_ShouldRenderAnchorIfLinkSet(
            [Values("", "ASDF", "nofollow", "_blank")] string target,
            [Values(null, "noopener", "nofollow")] string rel)
        {

            var comp = Context.Render<MudChip<string>>(parameters => parameters
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
            var comp = Context.Render<MudChip<string>>(parameters => parameters
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

            var comp = Context.Render<MudChip<string>>(parameters => parameters
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
            var comp = Context.Render<ChipAvatarContentTest>();

            comp.Find("div.mud-chip").InnerHtml.Should().Contain("mud-avatar");
        }

        /// <summary>
        /// Clicks on the chip and tests if the OnClick event works
        /// </summary>
        [Test]
        public async Task Chip_OnClick()
        {
            var comp = Context.Render<ChipOnClickTest>();
            // print the generated html

            // chip should have mud-clickable and mud-ripple classes
            var chip = comp.Find("button.mud-chip");
            chip.ClassName.Should().Contain("mud-clickable");
            chip.ClassName.Should().Contain("mud-ripple");

            // click on chip
            await chip.ClickAsync();

            var expectedEvent = comp.Find("#chip-click-test-expected-value");
            expectedEvent.InnerHtml.Should().Be("OnClick");
        }

        /// <summary>
        /// Clicks on the close button and tests if the OnClose event works
        /// </summary>
        [Test]
        public async Task Chip_OnClose()
        {
            var comp = Context.Render<ChipOnClickTest>();
            // print the generated html

            // chip should have mud-clickable and mud-ripple classes
            var chip = comp.Find("button.mud-chip");
            chip.ClassName.Should().Contain("mud-clickable");
            chip.ClassName.Should().Contain("mud-ripple");

            // click on close button
            await comp.Find("button.mud-chip-close-button").ClickAsync();

            var expectedEvent = comp.Find("#chip-click-test-expected-value");
            expectedEvent.InnerHtml.Should().Be("OnClose");
        }

        /// <summary>
        /// A disabled clickable chip keeps its button role and reports aria-disabled.
        /// </summary>
        [Test]
        public void Chip_Disabled_ShouldExposeAriaDisabled()
        {
            var comp = Context.Render<MudChip<string>>(parameters => parameters
                .Add(p => p.Disabled, true)
                .Add(p => p.OnClick, () => { }));

            var chip = comp.Find(".mud-chip");
            chip.TagName.Should().Be("DIV");
            chip.GetAttribute("role").Should().Be("button");
            chip.GetAttribute("aria-disabled").Should().Be("true");
            chip.HasAttribute("aria-pressed").Should().BeFalse();
        }

        /// <summary>
        /// A chip that is not clickable is a plain element with no button role or state.
        /// </summary>
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void Chip_Plain_ShouldNotExposeToggleState(bool disabled)
        {
            var comp = Context.Render<MudChip<string>>(parameters => parameters.Add(p => p.Disabled, disabled));

            var chip = comp.Find(".mud-chip");
            chip.HasAttribute("role").Should().BeFalse();
            chip.HasAttribute("aria-disabled").Should().BeFalse();
            chip.HasAttribute("aria-pressed").Should().BeFalse();
        }
    }
}
