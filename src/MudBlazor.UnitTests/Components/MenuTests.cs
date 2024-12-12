// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.Menu;
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
        [TestCase("x", "Close menu", "Close menu")]
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
                .Add(p => p.Label, "Accessibility")
                .Add(p => p.AriaLabel, ariaLabel));

            comp.Find("button").GetAttribute("aria-label").Should().Be(expectedAriaLabel);
        }

        [Test]
        public async Task MultiNest_MenuPointerLeave_MenuPointerEnter_Closing()
        {
            var comp = Context.RenderComponent<MenuTestNestWithMouseOver>();
            // open all sub menus
            var mudMenus = comp.FindComponents<MudMenu>();
            var menu = mudMenus[0].WaitForElement(".mud-menu");
            comp.WaitForAssertion(() => mudMenus.Count.Should().Be(1));
            await menu.TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.WaitForAssertion(() => mudMenus[0].Instance.Open.Should().BeTrue());
            mudMenus = comp.FindComponents<MudMenu>();
            comp.WaitForAssertion(() => mudMenus.Count.Should().Be(2));
            menu = mudMenus[0].WaitForElement(".mud-menu");
            await menu.TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.WaitForAssertion(() => mudMenus[0].Instance.Open.Should().BeTrue());
            mudMenus = comp.FindComponents<MudMenu>();
            comp.WaitForAssertion(() => mudMenus.Count.Should().Be(3));
            menu = mudMenus[1].WaitForElement(".mud-menu");
            await menu.TriggerEventAsync("onpointerenter", new PointerEventArgs());
            await menu.TriggerEventAsync("onpointermove", new PointerEventArgs());
            comp.WaitForAssertion(() => mudMenus[1].Instance.Open.Should().BeTrue());

            // try to keep mouse enter
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            _ = Task.Run(async () =>
            {
                var menuItem = mudMenus[2].Find(".mud-menu");
                while (!token.IsCancellationRequested)
                {
                    await menuItem.TriggerEventAsync("onpointermove", new PointerEventArgs());
                    await Task.Delay(35, token);
                }
            }, token);
            await Task.Delay(10, CancellationToken.None);
            // click menu item
            // all opened menu should be close
            _ = mudMenus[1].InvokeAsync(() => mudMenus[1].Instance.CloseMenuAsync());
            _ = cancellationTokenSource.CancelAsync();
            await Task.Delay(200, CancellationToken.None);
            mudMenus = comp.FindComponents<MudMenu>();
            comp.WaitForAssertion(() => mudMenus.Count.Should().Be(1));
            comp.WaitForAssertion(() => mudMenus[0].Instance.Open.Should().BeFalse());
        }

        [Test]
        public async Task MultiNest_MenuPointerLeave_OpenNextSubMenuClosePreviousSubMenu()
        {
            var comp = Context.RenderComponent<MenuTestNestWithQuicklyMouseMove>();
            // open all sub menus
            var mudMenus = comp.FindComponents<MudMenu>();
            var menu = mudMenus[0].WaitForElement(".mud-menu");
            comp.WaitForAssertion(() => mudMenus.Count.Should().Be(1));
            await menu.TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.WaitForAssertion(() => mudMenus[0].Instance.Open.Should().BeTrue());
            mudMenus = comp.FindComponents<MudMenu>();
            comp.WaitForAssertion(() => mudMenus.Count.Should().Be(11));
            var menuA = mudMenus[0].WaitForElement(".mud-menu");
            var menuB = mudMenus[1].WaitForElement(".mud-menu");
            await menuA.TriggerEventAsync("onpointerenter", new PointerEventArgs());
            _ = menuA.TriggerEventAsync("onpointerleave", new PointerEventArgs());
            _ = menuB.TriggerEventAsync("onpointerenter", new PointerEventArgs());
        }

        [Test]
        public async Task MultiNest_MenuPointerLeave_MenuPointerEnter_CheckOpenClose()
        {
            var comp = Context.RenderComponent<MenuTestNestWithMouseOver>();
            // open all sub menus
            var mudMenus = comp.FindComponents<MudMenu>();
            var menu = mudMenus[0].WaitForElement(".mud-menu");
            comp.WaitForAssertion(() => mudMenus.Count.Should().Be(1));
            await menu.TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.WaitForAssertion(() => mudMenus[0].Instance.Open.Should().BeTrue());
            mudMenus = comp.FindComponents<MudMenu>();
            comp.WaitForAssertion(() => mudMenus.Count.Should().Be(2));
            menu = mudMenus[0].WaitForElement(".mud-menu");
            await menu.TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.WaitForAssertion(() => mudMenus[0].Instance.Open.Should().BeTrue());
            mudMenus = comp.FindComponents<MudMenu>();
            comp.WaitForAssertion(() => mudMenus.Count.Should().Be(3));
            menu = mudMenus[1].WaitForElement(".mud-menu");
            await menu.TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.WaitForAssertion(() => mudMenus[1].Instance.Open.Should().BeTrue());

            mudMenus = comp.FindComponents<MudMenu>();

            // keep the first menu open
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            _ = Task.Run(async () =>
            {
                var menuItem = mudMenus[2].Find(".mud-menu");
                while (!token.IsCancellationRequested)
                {
                    await menuItem.TriggerEventAsync("onpointermove", new PointerEventArgs());
                    await Task.Delay(10, token);
                }
            }, token);
            // leave last sub menu , and then parent menu open will be false
            // but the first menu should still be open
            menu = mudMenus[1].WaitForElement(".mud-menu");
            await menu.TriggerEventAsync("onpointerleave", new PointerEventArgs());
            await Task.Delay(100, CancellationToken.None);
            mudMenus = comp.FindComponents<MudMenu>();
            comp.WaitForAssertion(() => mudMenus.Count.Should().Be(2));
            comp.WaitForAssertion(() => mudMenus[0].Instance.Open.Should().BeFalse());
            comp.WaitForAssertion(() => mudMenus[1].Instance.Open.Should().BeTrue());
            await cancellationTokenSource.CancelAsync();
        }

        [Test]
        public async Task OpenMenuAsync_Should_Set_FixedPosition()
        {
            // Arrange
            var comp = Context.RenderComponent<MenuPositionAtCursorTest>();
            var menuComponent = comp.FindComponent<MudMenu>();
            var mudMenuContext = menuComponent.Instance;
            mudMenuContext.Should().NotBeNull();

            // Act
            await Context.Renderer.Dispatcher.InvokeAsync(() => mudMenuContext.OpenMenuAsync(new MouseEventArgs()));

            // find popover element
            var popover = comp.Find("div.mud-popover");

            // Assert
            popover.ClassList.Should().Contain("mud-popover-anchor-top-left");
            popover.ClassList.Should().Contain("mud-popover-position-override");

            popover.OuterHtml.Contains("top: 0px; left: 0px;").Should().BeTrue();

            await Context.Renderer.Dispatcher.InvokeAsync(mudMenuContext.CloseMenuAsync);
        }

        [Test]
        public void ContextMenu_Should_NotHaveButton_And_NotBeVisible()
        {
            // Arrange
            var comp = Context.RenderComponent<ContextMenuTest>();
            var menuComponent = comp.FindComponent<MudMenu>();

            // Assert
            comp.FindAll("button.mud-button-root").Count.Should().Be(0);
            menuComponent.Find("div.mud-menu").ClassList.Should().Contain("mud-menu-button-hidden");
        }

        [Test]
        public void ContextMenu_WithLabel_Sould_HaveButton_And_BeVisible()
        {
            // Arrange
            var comp = Context.RenderComponent<ContextMenuTest>(parameters
                => parameters.Add(p => p.Label, "Context Menu"));
            var menuComponent = comp.FindComponent<MudMenu>();

            // Assert
            menuComponent.FindAll("button").Count.Should().Be(1);
            menuComponent.Find("div.mud-menu").ClassList.Should().NotContain("mud-menu-button-hidden");
        }

        [Test]
        public void ContextMenu_WithActivatorContent_Sould_HaveActivatorContent_And_BeVisible()
        {
            // Arrange
            var comp = Context.RenderComponent<ContextMenuTest>(parameters
                => parameters.Add(p => p.ActivatorContent, "<div id=\"custom-activator\">Custom Activator Content</div>"));
            var menuComponent = comp.FindComponent<MudMenu>();

            // Assert
            menuComponent.FindAll("button").Count.Should().Be(0);
            menuComponent.Find("div.mud-menu").ClassList.Should().NotContain("mud-menu-button-hidden");
            menuComponent.Find("div#custom-activator").TextContent.Should().Be("Custom Activator Content");
        }
    }
}
