using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class MenuTests : BunitTest
    {
        [Test]
        public async Task OpenMenu_ClickFirstItem_CheckClosed()
        {
            // https://github.com/MudBlazor/MudBlazor/issues/4063

            var comp = Context.RenderComponent<MenuTest1>();
            var menu = comp.FindComponent<MudMenu>();

            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(2);
            comp.FindAll("a.mud-list-item").Count.Should().Be(2);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);

            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(2);
            comp.FindAll("a.mud-list-item").Count.Should().Be(2);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));

            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(2);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));

            //Disabled item's click ot touch should not close popover
            comp.FindAll("button.mud-button-root")[0].Click();

            var menuItems = comp.FindComponents<MudMenuItem>();
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            menuItems[2].Instance.Disabled = true;
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.

            comp.FindAll("a.mud-list-item")[1].Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => menu.Instance.ToggleMenuAsync(new TouchEventArgs()));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
            await comp.InvokeAsync(() => menu.Instance.ToggleMenuAsync(new TouchEventArgs()));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));
        }

        [Test]
        public void OpenMenu_ClickSecondItem_CheckClosed()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(2);
            comp.FindAll("a.mud-list-item").Count.Should().Be(2);
            comp.FindAll("a.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public void OpenMenu_ClickThirdItem_CheckClosed()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(2);
            comp.FindAll("a.mud-list-item").Count.Should().Be(2);
            comp.FindAll("a.mud-list-item")[1].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public void OpenMenu_ClickClassItem_CheckClass()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(2);
            comp.FindAll("a.mud-list-item").Count.Should().Be(2);
            comp.FindAll("div.mud-list-item.test-class").Count.Should().Be(1);
        }

        [Test]
        public void OpenMenu_CheckClass()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.Find("div.mud-popover").ClassList.Should().Contain("menu-popover-class");
        }

        [Test]
        public async Task IsOpen_CheckState()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            var menu = comp.FindComponent<MudMenu>().Instance;
            menu.Open.Should().BeFalse();

            var args = new MouseEventArgs { OffsetX = 1.0, OffsetY = 1.0 };
            await comp.InvokeAsync(() => menu.OpenMenuAsync(args));
            menu.Open.Should().BeTrue();

            await comp.InvokeAsync(() => menu.CloseMenuAsync());
            menu.Open.Should().BeFalse();
        }

        [Test]
        public async Task MouseOver_PointerLeave_ShouldClose()
        {
            var comp = Context.RenderComponent<MenuTestMouseOver>();
            var pop = comp.FindComponent<MudPopover>();

            // Briefly hover over the button which will open the popover while leaving a small delay to allow the user to move the pointer to the menu.
            comp.FindAll("div.mud-menu")[0].PointerEnter();
            comp.FindAll("div.mud-menu")[0].PointerLeave();

            IElement List() => comp.FindAll("div.mud-list")[0];

            await List().TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.WaitForAssertion(() => pop.Instance.Open.Should().BeTrue());

            await List().TriggerEventAsync("onpointerleave", new PointerEventArgs());
            comp.WaitForAssertion(() => pop.Instance.Open.Should().BeFalse());
        }

        [Test]
        public async Task MouseOver_Hover_ShouldOpenMenu()
        {
            var comp = Context.RenderComponent<MenuTestMouseOver>();
            IRenderedComponent<MudPopover> Popover() => comp.FindComponent<MudPopover>();

            IElement Menu() => comp.Find(".mud-menu");

            comp.WaitForAssertion(() => Popover().Instance.Open.Should().BeFalse());

            // Pointer over to menu to open popover
            await Menu().TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.WaitForAssertion(() => Popover().Instance.Open.Should().BeTrue());

            // Popover open, captures pointer
            await Menu().TriggerEventAsync("onpointerleave", new PointerEventArgs());
            comp.WaitForAssertion(() => Popover().Instance.Open.Should().BeFalse());

            // Pointer moves to menu, still need to open
            await Menu().TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.WaitForAssertion(() => Popover().Instance.Open.Should().BeTrue());
        }

        [Test]
        public async Task MouseOver_Click_ShouldKeepOpen()
        {
            var comp = Context.RenderComponent<MenuTestMouseOver>();
            var pop = comp.FindComponent<MudPopover>();

            // Enter opens the menu.
            comp.FindAll("div.mud-menu")[0].PointerEnter();

            // Clicking the button should close the menu.
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.WaitForAssertion(() => pop.Instance.Open.Should().BeFalse());

            // Clicking the button again should open the menu permanently.
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.WaitForAssertion(() => pop.Instance.Open.Should().BeTrue());

            // Leaving the menu should not close it.
            comp.FindAll("div.mud-menu")[0].PointerLeave();
            comp.WaitForAssertion(() => pop.Instance.Open.Should().BeTrue());

            IElement List() => comp.FindAll("div.mud-list")[0];

            // Hover over the list shouldn't change anything.
            await List().TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.WaitForAssertion(() => pop.Instance.Open.Should().BeTrue());

            // Leave the list shouldn't change anything.
            await List().TriggerEventAsync("onpointerleave", new PointerEventArgs());
            comp.WaitForAssertion(() => pop.Instance.Open.Should().BeTrue());

            // Clicking the button should now close the menu.
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.WaitForAssertion(() => pop.Instance.Open.Should().BeFalse());
        }

        [Test]
        public void ActivatorContent_Disabled_CheckDisabled()
        {
            var comp = Context.RenderComponent<MenuTestDisabledCustomActivator>();
            var activator = comp.Find("div.mud-menu-activator");
            activator.ClassList.Should().Contain("mud-disabled");
            activator.GetAttribute("disabled").Should().NotBeNull();
        }

        [Test]
        public void Default_Disabled_CheckDisabled()
        {
            var comp = Context.RenderComponent<MenuTest1>(x =>
                x.Add(p => p.DisableMenu, true)
            );

            var button = comp.Find("button.mud-button-root");
            button.Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public async Task ToggleEventArgs()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            var menu = comp.FindComponent<MudMenu>();

            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);

            await comp.InvokeAsync(() => menu.Instance.ToggleMenuAsync(new MouseEventArgs()));
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            await comp.InvokeAsync(() => menu.Instance.ToggleMenuAsync(new MouseEventArgs()));
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);

            await comp.InvokeAsync(() => menu.Instance.ToggleMenuAsync(new TouchEventArgs()));
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            await comp.InvokeAsync(() => menu.Instance.ToggleMenuAsync(new TouchEventArgs()));
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public async Task ToggleMenuDoesNotWorkIfDisabled()
        {
            var comp = Context.RenderComponent<MenuTest1>(x =>
                x.Add(p => p.DisableMenu, true)
            );

            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);

            var menu = comp.FindComponent<MudMenu>();

            await menu.Instance.ToggleMenuAsync(new MouseEventArgs());
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);

            await menu.Instance.ToggleMenuAsync(new TouchEventArgs());
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public void MenuTest_LeftAndRightClick_CheckClosed()
        {
            //Standart button menu -- left click
            var comp = Context.RenderComponent<MenuTestVariants>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(1);
            comp.FindAll("a.mud-list-item").Count.Should().Be(2);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[0].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Standart button menu -- right click
            comp.FindAll("button.mud-button-root")[1].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(1);
            comp.FindAll("a.mud-list-item").Count.Should().Be(2);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[1].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Icon button menu -- left click
            comp.FindAll("button.mud-button-root")[2].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(1);
            comp.FindAll("a.mud-list-item").Count.Should().Be(2);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[2].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Icon button menu -- right click
            comp.FindAll("button.mud-button-root")[3].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(1);
            comp.FindAll("a.mud-list-item").Count.Should().Be(2);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[3].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Activator content menu -- left click
            comp.FindAll("button.mud-button-root")[4].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(1);
            comp.FindAll("a.mud-list-item").Count.Should().Be(2);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[4].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Activator content menu -- right click
            comp.FindAll("button.mud-button-root")[5].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(1);
            comp.FindAll("a.mud-list-item").Count.Should().Be(2);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[5].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public void MenuItem_Should_SupportIcons()
        {
            var comp = Context.RenderComponent<MenuItemIconTest>();
            comp.FindAll("button.mud-button-root")[0].Click();
            var listItems = comp.FindAll("div.mud-list-item");
            listItems.Count.Should().Be(3);
            listItems[0].QuerySelector("div.mud-list-item-icon").Should().NotBeNull();
            listItems[0].QuerySelector("svg.mud-svg-icon").Should().NotBeNull();
        }

        [Test]
        public void MenuItem_IconAppearance_Test()
        {
            var comp = Context.RenderComponent<MenuItemIconTest>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            var listItems = comp.FindAll("div.mud-list-item");

            // 1st MenuItem
            var svg = listItems[0].QuerySelector("svg");
            svg.ClassList.Should().Contain("mud-icon-size-small");
            svg.ClassList.Should().Contain("mud-primary-text");

            // 2nd MenuItem
            svg = listItems[1].QuerySelector("svg");
            svg.ClassList.Should().Contain("mud-icon-size-medium");
            // Ensure no color classes are present, like "mud-primary-text", "mud-error-text", etc.
            foreach (var className in svg.ClassList)
                Regex.IsMatch(className, "^mud-[a-z]+-text$", RegexOptions.IgnoreCase).Should().BeFalse();

            // 3rd MenuItem
            svg = listItems[2].QuerySelector("svg");
            svg.ClassList.Should().Contain("mud-icon-size-large");
            svg.ClassList.Should().Contain("mud-secondary-text");
        }

        /// <summary>
        /// https://github.com/MudBlazor/MudBlazor/issues/6645
        /// </summary>
        [Test]
        public async Task OnClickErrorContentCaughtException()
        {
            var comp = Context.RenderComponent<MenuErrorContenCaughtException>();
            await comp.FindAll("button.mud-button-root")[0].ClickAsync(new MouseEventArgs());
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(1);
            await comp.FindAll("div.mud-list-item")[0].ClickAsync(new MouseEventArgs());
            var mudAlert = comp.FindComponent<MudAlert>();
            var text = mudAlert.Find("div.mud-alert-message");
            text.InnerHtml.Should().Be("Oh my! We caught an error and handled it!");
        }

        [Test]
        public void OpenMenu_CloseMenuOnClick_CheckStillOpen()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(2);
            comp.FindAll("a.mud-list-item").Count.Should().Be(2);
            comp.FindAll("div.mud-list-item")[1].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
        }

        [Test]
        public async Task IsOpenChanged_InvokedWhenOpened_CheckTrueInvocationCountIsOne()
        {
            var comp = Context.RenderComponent<MenuIsOpenChangedTest>();
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Menu.OpenMenuAsync(EventArgs.Empty));
            comp.Instance.TrueInvocationCount.Should().Be(1);
            comp.Instance.FalseInvocationCount.Should().Be(0);
        }

        [Test]
        public async Task IsOpenChanged_InvokedWhenClosed_CheckTrueInvocationCountIsOneClickFalseInvocationCountIsOne()
        {
            var comp = Context.RenderComponent<MenuIsOpenChangedTest>();
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Menu.OpenMenuAsync(EventArgs.Empty));
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Menu.CloseMenuAsync());
            comp.Instance.TrueInvocationCount.Should().Be(1);
            comp.Instance.FalseInvocationCount.Should().Be(1);
        }

        [Test]
        public void ItemsWithHrefShouldRenderAsAnchor()
        {
            var comp = Context.RenderComponent<MenuHrefTest>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(1);
            comp.FindAll("a.mud-list-item").Count.Should().Be(3);
            comp.FindAll("a.mud-list-item")[0].Attributes["href"].TextContent.Should().Be("https://www.test.com/1");
            comp.FindAll("a.mud-list-item")[1].Attributes["href"].TextContent.Should().Be("https://www.test.com/2");
            comp.FindAll("a.mud-list-item")[2].Click(); // disabled
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("a.mud-list-item")[1].Click(); // enabled
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        [TestCase("x", null, null)]
        [TestCase(null, "Close menu", "Close menu")]
        [TestCase("x", "Close menu", "Close menu")]
        [TestCase(null, null, null, Description = "Ensures aria-label is not present instead of empty string")]
        public void MenuWithLabelAndAriaLabel_Should_HaveExpectedAriaLabel(string label, string ariaLabel, string expectedAriaLabel)
        {
            var comp = Context.RenderComponent<MenuAccessibilityTest>(parameters => parameters
                .Add(p => p.Label, label)
                .Add(p => p.AriaLabel, ariaLabel));

            comp.Find("button").GetAttribute("aria-label").Should().Be(expectedAriaLabel);
        }

        [Test]
        [TestCase("Close menu", "Close menu")]
        [TestCase(null, null, Description = "Ensures aria-label is not present instead of empty string")]
        public void IconMenuWithAriaLabel_Should_HaveExpectedAriaLabel(string ariaLabel, string expectedAriaLabel)
        {
            var comp = Context.RenderComponent<MenuAccessibilityTest>(parameters => parameters
                .Add(p => p.Icon, Icons.Material.Filled.Accessibility)
                .Add(p => p.AriaLabel, ariaLabel));

            comp.Find("button").GetAttribute("aria-label").Should().Be(expectedAriaLabel);
        }
    }
}
