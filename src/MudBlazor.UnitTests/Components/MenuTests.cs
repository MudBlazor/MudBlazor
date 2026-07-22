// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
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

            var comp = Context.Render<MenuTest1>();
            var menu = comp.FindComponent<MudMenu>();

            await comp.FindAll("button.mud-button-root")[0].ClickAsync();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            await comp.FindAll("div.mud-menu-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => { comp.FindAll("div.mud-popover-open").Count.Should().Be(0); });

            await comp.FindAll("button.mud-button-root")[0].ClickAsync();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            await comp.FindAll("div.mud-menu-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));

            await comp.FindAll("button.mud-button-root")[0].ClickAsync();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            await comp.FindAll("div.mud-menu-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));

            //Disabled item's click ot touch should not close popover
            await comp.FindAll("button.mud-button-root")[0].ClickAsync();

            var menuItems = comp.FindComponents<MudMenuItem>();
            await menuItems[2].SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));

            await comp.FindAll("a.mud-menu-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => menu.Instance.ToggleMenuAsync(new TouchEventArgs()));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
            await comp.InvokeAsync(() => menu.Instance.ToggleMenuAsync(new TouchEventArgs()));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));
        }

        [Test]
        public async Task OpenMenu_ClickSecondItem_CheckClosed()
        {
            var comp = Context.Render<MenuTest1>();
            await comp.FindAll("button.mud-button-root")[0].ClickAsync();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            await comp.FindAll("a.mud-menu-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => { comp.FindAll("div.mud-popover-open").Count.Should().Be(0); });
        }

        [Test]
        public async Task OpenMenu_ClickThirdItem_CheckClosed()
        {
            var comp = Context.Render<MenuTest1>();
            await comp.FindAll("button.mud-button-root")[0].ClickAsync();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            await comp.FindAll("a.mud-menu-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => { comp.FindAll("div.mud-popover-open").Count.Should().Be(0); });
        }

        [Test]
        public async Task OpenMenu_ClickClassItem_CheckClass()
        {
            var comp = Context.Render<MenuTest1>();
            await comp.FindAll("button.mud-button-root")[0].ClickAsync();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("div.mud-menu-item.test-class").Count.Should().Be(1);
        }

        [Test]
        public async Task OpenMenu_CheckClass()
        {
            var comp = Context.Render<MenuTest1>();
            await comp.FindAll("button.mud-button-root")[0].ClickAsync();
            comp.Find("div.mud-popover").ClassList.Should().Contain("menu-popover-class");
        }

        [Test]
        public async Task OpenMenu_ListReachableFromPopoverForOverflowClamping()
        {
            // Regression guard for https://github.com/MudBlazor/MudBlazor/issues/13141.
            // The automatic viewport-overflow scrollbar (and an explicit MaxHeight) only work if
            // mudPopover.js can reach the menu's .mud-list by descending single-child wrappers from
            // the popover content node, and if that wrapper carries the class the SCSS max-height
            // inheritance chain targets. The v9 keyboard/focus wrapper silently broke both, with no
            // test catching it.
            var comp = Context.Render<MenuTest1>();
            await comp.Find("button.mud-button-root").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            var popover = comp.FindAll("div.mud-popover").Single(p => p.QuerySelector(".mud-menu-list") is not null);

            // Mirror the descent in mudPopover.js: walk down through single-child wrappers to the list.
            var node = popover.FirstElementChild;
            while (node is not null && !node.ClassList.Contains("mud-list") && node.ChildElementCount == 1)
            {
                node = node.FirstElementChild;
            }

            node.Should().NotBeNull("mudPopover.js locates the scrollable menu list by descending single-child wrappers from the popover");
            node!.ClassList.Should().Contain("mud-list");

            // The list's wrapper must keep the class the SCSS max-height inheritance chain targets.
            var wrapper = comp.Find("[data-testid='menu-wrapper']");
            wrapper.ClassList.Should().Contain("mud-menu-list-wrapper");
            wrapper.QuerySelector(".mud-menu-list").Should().NotBeNull();
        }

        [Test]
        public async Task Menu_ModelessOverlay_IgnoresActivatorRootForAutoCloseHitTesting()
        {
            var comp = Context.Render<MenuTest1>();

            await comp.Find("button.mud-button-root").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            var menuRoot = comp.Find("div.mud-menu");
            var overlay = comp.Find("div.mud-overlay");
            overlay.GetAttribute("data-modeless-ignore-element-id").Should().Be(menuRoot.Id);
            menuRoot.Id.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task IsOpen_CheckState()
        {
            var comp = Context.Render<MenuTest1>();
            var menu = comp.FindComponent<MudMenu>().Instance;
            menu.GetState(x => x.Open).Should().BeFalse();

            var args = new MouseEventArgs { OffsetX = 1.0, OffsetY = 1.0 };
            await comp.InvokeAsync(() => menu.OpenMenuAsync(args));
            menu.GetState(x => x.Open).Should().BeTrue();

            await comp.InvokeAsync(() => menu.CloseMenuAsync());
            menu.GetState(x => x.Open).Should().BeFalse();
        }

        [Test]
        public async Task MouseOver_PointerLeave_ShouldClose()
        {
            var comp = Context.Render<MenuTestMouseOver>();

            // Briefly hover over the button and wait for it to open.
            await comp.Find("div.mud-menu").PointerEnterAsync();
            await comp.WaitForAssertionAsync(() => comp.Markup.Should().Contain("mud-popover-open"));

            // Close it again and wait for that to happen.
            await comp.Find("div.mud-menu").PointerLeaveAsync();
            await comp.WaitForAssertionAsync(() => comp.Markup.Should().NotContain("mud-popover-open"));
        }

        [Test]
        public async Task MouseOver_Hover_ShouldOpenMenu()
        {
            var comp = Context.Render<MenuTestMouseOver>();

            IElement Menu() => comp.Find(".mud-menu");
            comp.Markup.Should().NotContain("mud-popover-open");

            // Pointer over to menu to open popover
            await Menu().PointerEnterAsync(new PointerEventArgs());
            await comp.WaitForAssertionAsync(() => comp.Markup.Should().Contain("mud-popover-open"));

            // Popover open, captures pointer
            await Menu().PointerLeaveAsync(new PointerEventArgs());
            await comp.WaitForAssertionAsync(() => comp.Markup.Should().NotContain("mud-popover-open"));

            // Pointer moves to menu, still need to open
            await Menu().PointerEnterAsync(new PointerEventArgs());
            await comp.WaitForAssertionAsync(() => comp.Markup.Should().Contain("mud-popover-open"));
        }

        [Test]
        public async Task MouseOver_Click_ShouldKeepMenuOpen()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            var hoverDelay = MudGlobal.MenuDefaults.HoverDelay;
            var comp = Context.Render<MenuTestMouseOver>();
            var menu = comp.FindComponent<MudMenu>().Instance;

            // Enter opens the menu (after a delay).
            comp.Find("div.mud-menu").PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(hoverDelay + 100)));
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeTrue());

            // Clicking the button should close the menu.
            await comp.Find("button.mud-button-root").ClickAsync();
            // Check that the component is closed
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeFalse());

            // Clicking the button again should open the menu indefinitely.
            await comp.Find("button.mud-button-root").ClickAsync();
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeTrue());

            // Leaving the menu should no longer close it.
            comp.Find("div.mud-menu").PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(hoverDelay + 100)));
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeTrue());

            // Hover the list shouldn't change anything.
            comp.Find("[data-testid='menu-wrapper']").PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeTrue());

            // Leave the list shouldn't change anything.
            comp.Find("[data-testid='menu-wrapper']").PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(hoverDelay + 100)));
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeTrue());

            // Clicking the button should now close the menu.
            await comp.Find("button.mud-button-root").ClickAsync();
            // Check that the component is closed
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeFalse());
        }

        [Test]
        public void ActivatorContent_Disabled_CheckDisabled()
        {
            var comp = Context.Render<MenuTestDisabledCustomActivator>();
            var activator = comp.Find("div.mud-menu-activator");
            activator.ClassList.Should().Contain("mud-disabled");
            activator.GetAttribute("disabled").Should().NotBeNull();
        }

        [Test]
        public async Task Default_Disabled_CheckDisabled()
        {
            var comp = Context.Render<MenuTest1>(x =>
                x.Add(p => p.DisableMenu, true)
            );

            var button = comp.Find("button.mud-button-root");
            await button.ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public async Task ToggleEventArgs()
        {
            var comp = Context.Render<MenuTest1>();
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
            var comp = Context.Render<MenuTest1>(x =>
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
        public async Task Menu_LeftAndRightClick_CheckClosed()
        {
            //Standart button menu -- left click
            var comp = Context.Render<MenuTestVariants>();
            await comp.FindAll("button.mud-button-root")[0].ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            await comp.FindAll("div.mud-menu-item")[0].ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            await comp.FindAll("button.mud-button-root")[0].ClickAsync(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Standart button menu -- right click
            await comp.FindAll("button.mud-button-root")[1].ClickAsync(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            await comp.FindAll("div.mud-menu-item")[0].ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            await comp.FindAll("button.mud-button-root")[1].ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Icon button menu -- left click
            await comp.FindAll("button.mud-button-root")[2].ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            await comp.FindAll("div.mud-menu-item")[0].ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            await comp.FindAll("button.mud-button-root")[2].ClickAsync(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Icon button menu -- right click
            await comp.FindAll("button.mud-button-root")[3].ClickAsync(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            await comp.FindAll("div.mud-menu-item")[0].ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            await comp.FindAll("button.mud-button-root")[3].ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Activator content menu -- left click
            await comp.FindAll("button.mud-button-root")[4].ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            await comp.FindAll("div.mud-menu-item")[0].ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            await comp.FindAll("button.mud-button-root")[4].ClickAsync(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Activator content menu -- right click (must trigger contextmenu on the user's div inside ActivatorContent)
            // Find the div that wraps the button in the right-click ActivatorContent (it has the @oncontextmenu handler)
            var rightClickMenus = comp.FindAll("div.mud-menu");
            var rightClickMenu = rightClickMenus.FirstOrDefault(m => m.QuerySelector("div[style*='inline-block']") != null);
            var userDiv = rightClickMenu?.QuerySelector("div[style*='inline-block']");
            userDiv.Should().NotBeNull("User's div with contextmenu handler should exist");
            await userDiv!.ContextMenuAsync(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            await comp.FindAll("div.mud-menu-item")[0].ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            await comp.FindAll("button.mud-button-root")[5].ClickAsync();
            await comp.WaitForAssertionAsync(() =>
            {
                comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            });
        }

        [Test]
        public async Task MenuItem_Should_RenderIcons()
        {
            var comp = Context.Render<MenuItemIconTest>();

            await comp.Find(".mud-menu-button-activator").ClickAsync();
            await comp.WaitForElementAsync("div.mud-popover-open");

            comp.FindAll(".mud-menu-list div.mud-menu-item svg.mud-svg-icon.mud-menu-item-icon.mud-icon-size-medium").Count.Should().Be(3);
        }

        [Test]
        public async Task MenuItem_Should_RenderIconColors()
        {
            var comp = Context.Render<MenuItemIconTest>();

            await comp.Find(".mud-menu-button-activator").ClickAsync();
            await comp.WaitForElementAsync("div.mud-popover-open");

            comp.FindAll("div.mud-menu-item").Count.Should().Be(3);
            var items = comp.FindAll("div.mud-menu-item");

            items[0].QuerySelector("svg").ClassList.Should().NotContainMatch("mud-*-text");
            items[1].QuerySelector("svg").ClassList.Should().Contain("mud-secondary-text");
            items[2].QuerySelector("svg").ClassList.Should().Contain("mud-tertiary-text");
        }

        /// <summary>
        /// https://github.com/MudBlazor/MudBlazor/issues/6645
        /// </summary>
        [Test]
        public async Task OnClickErrorContentCaughtException()
        {
            var comp = Context.Render<MenuErrorContenCaughtException>();
            await comp.FindAll("button.mud-button-root")[0].ClickAsync(new MouseEventArgs());
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            await comp.FindAll("div.mud-menu-item")[0].ClickAsync(new MouseEventArgs());
            var mudAlert = comp.FindComponent<MudAlert>();
            var text = mudAlert.Find("div.mud-alert-message");
            text.InnerHtml.Should().Be("Oh my! We caught an error and handled it!");
        }

        [Test]
        public async Task OpenMenu_CloseMenuOnClick_CheckStillOpen()
        {
            var comp = Context.Render<MenuTest1>();
            await comp.FindAll("button.mud-button-root")[0].ClickAsync();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            await comp.FindAll("div.mud-menu-item")[1].ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
        }

        [Test]
        public async Task IsOpenChanged_InvokedWhenOpened_CheckTrueInvocationCountIsOne()
        {
            var comp = Context.Render<MenuIsOpenChangedTest>();
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Menu.OpenMenuAsync(EventArgs.Empty));
            comp.Instance.TrueInvocationCount.Should().Be(1);
            comp.Instance.FalseInvocationCount.Should().Be(0);
        }

        [Test]
        public async Task IsOpenChanged_InvokedWhenClosed_CheckTrueInvocationCountIsOneClickFalseInvocationCountIsOne()
        {
            var comp = Context.Render<MenuIsOpenChangedTest>();
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Menu.OpenMenuAsync(EventArgs.Empty));
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Menu.CloseMenuAsync());
            comp.Instance.TrueInvocationCount.Should().Be(1);
            comp.Instance.FalseInvocationCount.Should().Be(1);
        }

        [Test]
        public async Task ItemsWithHrefShouldRenderAsAnchor()
        {
            var comp = Context.Render<MenuHrefTest>();
            await comp.FindAll("button.mud-button-root")[0].ClickAsync();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(3);
            comp.FindAll("a.mud-menu-item")[0].Attributes["href"].TextContent.Should().Be("https://www.test.com/1");
            comp.FindAll("a.mud-menu-item")[1].Attributes["href"].TextContent.Should().Be("https://www.test.com/2");
            await comp.FindAll("a.mud-menu-item")[2].ClickAsync(); // disabled
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            await comp.FindAll("a.mud-menu-item")[1].ClickAsync(); // enabled
            await comp.WaitForAssertionAsync(() =>
            {
                comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            });
        }

        [Test]
        [TestCase("x", null, null)]
        [TestCase("x", "Close menu", "Close menu")]
        public void MenuWithLabelAndAriaLabel_Should_HaveExpectedAriaLabel(string label, string ariaLabel, string expectedAriaLabel)
        {
            var comp = Context.Render<MenuAccessibilityTest>(parameters => parameters
                .Add(p => p.Label, label)
                .Add(p => p.AriaLabel, ariaLabel));

            comp.Find("button").GetAttribute("aria-label").Should().Be(expectedAriaLabel);
        }

        [Test]
        [TestCase("Close menu", "Close menu")]
        [TestCase(null, null, Description = "Ensures aria-label is not present instead of empty string")]
        public void IconMenuWithAriaLabel_Should_HaveExpectedAriaLabel(string ariaLabel, string expectedAriaLabel)
        {
            var comp = Context.Render<MenuAccessibilityTest>(parameters => parameters
                .Add(p => p.Icon, Icons.Material.Filled.Accessibility)
                .Add(p => p.Label, "Accessibility")
                .Add(p => p.AriaLabel, ariaLabel));

            comp.Find("button").GetAttribute("aria-label").Should().Be(expectedAriaLabel);
        }

        [Test]
        public async Task OpenMenuAsync_Should_Set_FixedPosition()
        {
            // Arrange
            var comp = Context.Render<MenuPositionAtCursorTest>();
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

            popover.OuterHtml.Should().Contain("data-pc-x=\"0\" data-pc-y=\"0\"");

            await Context.Renderer.Dispatcher.InvokeAsync(mudMenuContext.CloseMenuAsync);
        }

        [Test]
        public void ContextMenu_Should_NotHaveButton_And_NotBeVisible()
        {
            // Arrange
            var comp = Context.Render<ContextMenuTest>();
            var menuComponent = comp.FindComponent<MudMenu>();

            // Assert
            comp.FindAll("button.mud-button-root").Count.Should().Be(0);
            menuComponent.Find("div.mud-menu").ClassList.Should().Contain("mud-menu-button-hidden");
        }

        [Test]
        public void ContextMenu_WithLabel_Should_HaveButton_And_BeVisible()
        {
            // Arrange
            var comp = Context.Render<ContextMenuTest>(parameters
                => parameters.Add(p => p.Label, "Context Menu"));
            var menuComponent = comp.FindComponent<MudMenu>();

            // Assert
            menuComponent.FindAll("button").Count.Should().Be(1);
            menuComponent.Find("div.mud-menu").ClassList.Should().NotContain("mud-menu-button-hidden");
        }

        [Test]
        public void ContextMenu_WithActivatorContent_Should_HaveActivatorContent_And_BeVisible()
        {
            // Arrange - Use a RenderFragment<MenuContext> that renders the custom content
            var comp = Context.Render<ContextMenuTest>(parameters
                => parameters.Add(p => p.ActivatorContent, context => builder =>
                {
                    builder.OpenElement(0, "div");
                    builder.AddAttribute(1, "id", "custom-activator");
                    builder.AddContent(2, "Custom Activator Content");
                    builder.CloseElement();
                }));
            var menuComponent = comp.FindComponent<MudMenu>();

            // Assert
            menuComponent.FindAll("button").Count.Should().Be(0);
            menuComponent.Find("div.mud-menu").ClassList.Should().NotContain("mud-menu-button-hidden");
            menuComponent.Find("div#custom-activator").TextContent.Should().Be("Custom Activator Content");
        }

        [Test]
        public async Task Open_TwoWayBinding()
        {
            var comp = Context.Render<MenuTwoWayTest>();
            var menu = comp.FindComponent<MudMenu>();
            IElement SwitchElement() => comp.Find("#switch");

            menu.Instance.GetState(x => x.Open).Should().BeFalse("The menu should be closed initially.");
            comp.Instance.Open.Should().BeFalse();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0, "No popovers should be visible.");

            await comp.Find("button.mud-button-root").ClickAsync();
            await comp.WaitForAssertionAsync(() =>
            {
                menu.Instance.GetState(x => x.Open).Should().BeTrue("Clicking the button should open the menu.");
                comp.Instance.Open.Should().BeTrue();
                comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "One popover should be visible after opening.");
            });

            await SwitchElement().ChangeAsync(false);
            await comp.WaitForAssertionAsync(() =>
            {
                menu.Instance.GetState(x => x.Open).Should().BeFalse("Manually setting Open to false should close the menu.");
                comp.Instance.Open.Should().BeFalse();
                comp.FindAll("div.mud-popover-open").Count.Should().Be(0, "Popover should disappear after closing.");
            });

            await comp.Find("button.mud-button-root").ClickAsync();
            await comp.WaitForAssertionAsync(() =>
            {
                menu.Instance.GetState(x => x.Open).Should().BeTrue("Clicking the button again should open the menu.");
                comp.Instance.Open.Should().BeTrue();
                comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Popover should reappear after reopening.");
            });

            await SwitchElement().ChangeAsync(true);
            await comp.WaitForAssertionAsync(() =>
            {
                menu.Instance.GetState(x => x.Open).Should().BeTrue("Setting Open to true again should not change the state.");
                comp.Instance.Open.Should().BeTrue();
                comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Popover count should remain the same.");
            });

            await comp.Find("button.mud-button-root").ClickAsync();
            await comp.WaitForAssertionAsync(() =>
            {
                menu.Instance.GetState(x => x.Open).Should().BeFalse("Clicking the button should close the menu.");
                comp.Instance.Open.Should().BeFalse();
                comp.FindAll("div.mud-popover-open").Count.Should().Be(0, "Popover should no longer be visible.");
            });

            await comp.Find("button.mud-button-root").ClickAsync();
            await comp.WaitForAssertionAsync(() =>
            {
                menu.Instance.GetState(x => x.Open).Should().BeTrue("Clicking the button again should open the menu.");
                comp.Instance.Open.Should().BeTrue();
                comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Popover should appear again.");
            });
        }

        [Test]
        public async Task ActivatorClass()
        {
            var comp = Context.Render<MenuActivatorsTest>();

            comp.FindAll(".mud-menu")[0].FirstElementChild.ClassName.Should().Contain("mud-menu-button-activator");

            comp.FindAll(".mud-menu")[1].FirstElementChild.ClassName.Should().Contain("mud-menu-icon-button-activator");

            comp.FindAll(".mud-menu")[2].FirstElementChild.ClassName.Should().Contain("mud-menu-activator");

            await comp.FindAll(".mud-menu")[3].FirstElementChild.ClickAsync();

            comp.Find(".mud-popover-open .mud-menu-list .mud-menu-item.mud-menu-sub-menu-activator").Should().NotBeNull();
        }

        [Test]
        public void ShouldRenderLabelOrChildContent()
        {
            var comp = Context.Render<MenuItemLabelTest>();

            var childContent = comp.FindAll(".mud-menu-item")[0].InnerHtml;
            var label = comp.FindAll(".mud-menu-item")[1].InnerHtml;
            childContent.Should().BeEquivalentTo(label);

            // ChildContent should override Label.
            comp.FindAll(".mud-menu-item")[2].InnerHtml.Should().Contain("ContentText");
            comp.FindAll(".mud-menu-item")[2].InnerHtml.Should().NotContain("LabelText");
        }

        [Test]
        public async Task OpenNestedMenu()
        {
            var comp = Context.Render<MenuWithNestingTest>();

            // Open the first menu.
            await comp.Find("button:contains('1')").ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);

            // Click the nested menu item to open the nested menu.
            await comp.Find("div.mud-menu-item:contains('1.3')").ClickAsync();

            // Ensure both the main menu and the nested menu are open
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2);
        }

        [Test]
        public async Task ClickingMenuItem_ClosesNestedMenu()
        {
            var comp = Context.Render<MenuWithNestingTest>();

            // Open the first menu.
            await comp.Find("button:contains('1')").ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);

            // Click the nested menu item to open the nested menu.
            await comp.Find("div.mud-menu-item:contains('1.3')").ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2);

            // Click a non-nested menu item inside the nested menu.
            await comp.Find("div.mud-menu-item:contains('2.2')").ClickAsync();

            // Ensure all popovers are closed.
            await comp.WaitForAssertionAsync(() =>
            {
                comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            });
        }

        [Test]
        public async Task MenuContext_Should_ControlMenu()
        {
            // MenuContext (passed to ActivatorContent) forwards to the menu's open/close/toggle methods.
            var comp = Context.Render<MudMenu>(parameters => parameters.Add(p => p.Label, "Test Menu"));
            var menu = comp.Instance;
            var context = new MenuContext(menu);

            await comp.InvokeAsync(() => context.OpenAsync());
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeTrue());

            await comp.InvokeAsync(() => context.CloseAsync());
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeFalse());

            // Explicit args flow through the `args ?? EventArgs.Empty` overloads (Button 0 matches the default LeftClick).
            await comp.InvokeAsync(() => context.ToggleAsync(new MouseEventArgs { Button = 0 }));
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeTrue());

            await comp.InvokeAsync(() => context.ToggleAsync());
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeFalse());

            await comp.InvokeAsync(() => context.OpenAsync(new MouseEventArgs()));
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeTrue());

            await comp.InvokeAsync(() => context.CloseAllAsync());
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeFalse());
        }

        [Test]
        public async Task Menu_ButtonActivator_WithContext()
        {
            // Test that the activator content renders and clicking it opens the menu
            // Use a test component that properly uses the context
            var comp = Context.Render<MenuActivatorsTest>();

            // The MudButton inside the ActivatorContent should be rendered
            var button = comp.Find("button.mud-button-root");
            button.Should().NotBeNull();

            // Click the button - the context.ToggleAsync should toggle the menu
            await button.ClickAsync(new MouseEventArgs());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            await button.ClickAsync(new MouseEventArgs());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
        }

        [Test]
        public async Task Menu_PointerEvents_ShowHide_WithDebounce()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            // This method uses CatchAndLog to allow async events to run syncronously so we can test timing
            var hoverDelay = MudGlobal.MenuDefaults.HoverDelay;

            var comp = Context.Render<MenuWithNestingTest>();

            // Open the main menu first
            await comp.Find("button:contains('1')").ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Main menu should be open");

            // 1. Test SHOW debounce behavior
            // Trigger pointer enter on submenu item
            var menuItem = comp.Find("div.mud-menu:contains('1.3')");

            // Immediately after hover, submenu should not be visible yet (debounce in effect)
            menuItem.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Submenu should not open immediately");

            // After the hover delay, submenu should become visible
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(hoverDelay + 50)));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(2, "Submenu should open after hover delay"));

            // 2. Test HIDE debounce behavior

            // Trigger pointer leave
            menuItem.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();

            // Immediately after leave, submenu should still be visible (hide debounce in effect)
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2, "Submenu should remain open immediately after pointer leave");

            // Wait less than the delay
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(hoverDelay / 2)));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(2, "Submenu should still be open before hide delay completes"));

            // After the full delay, submenu should close
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(hoverDelay + 50)));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Submenu should close after full hide delay (2x hover delay)"));
        }

        [Test]
        public async Task Menu_PointerEvents_MultipleLevels()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            // This method uses CatchAndLog to allow async events to run syncronously so we can test timing
            var hoverDelay = MudGlobal.MenuDefaults.HoverDelay;

            var comp = Context.Render<MenuWithNestingTest>();

            // Open the main menu first
            await comp.Find("button:contains('1')").ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Main menu should be open");

            // Open first level submenu
            comp.Find("div.mud-menu:contains('1.3')").PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(hoverDelay + 100)));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(2, "First level submenu should be open"));

            // Open second level submenu
            comp.Find("div.mud-menu:contains('2.1')").PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(hoverDelay + 100)));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(3, "Second level submenu should be open"));

            // Leaving second level should close only that level after delay
            comp.Find("div.mud-menu:contains('2.1')").PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds((hoverDelay * 2) + 100)));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(2,
                "Second level should close but first level should remain open"));

            // Leaving first level should close it after delay
            comp.Find("div.mud-menu:contains('1.3')").PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds((hoverDelay * 2) + 100)));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1,
                "First level should close but main menu should remain open"));
        }

        [Test]
        public async Task Menu_PointerEvents_RapidMovement()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            // This method uses CatchAndLog to allow async events to run syncronously so we can test timing
            var hoverDelay = MudGlobal.MenuDefaults.HoverDelay;

            var comp = Context.Render<MenuWithNestingTest>();

            // Open the main menu first
            await comp.Find("button:contains('1')").ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Main menu should be open");

            var menuItem = comp.Find("div.mud-menu:contains('1.3')");

            // Simulate rapid mouse movement: enter -> leave -> enter -> leave -> enter
            menuItem.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(50)));
            menuItem.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(50)));
            menuItem.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(50)));
            menuItem.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(50)));
            menuItem.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();

            // Final state should be "entering" so menu should open
            timeProvider.Advance(TimeSpan.FromMilliseconds(hoverDelay + 50));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(2,
                "Menu should open after rapid movement ending with pointer enter"));

            // Now rapid movement ending with leaving
            menuItem.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(50)));
            menuItem.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(50)));
            menuItem.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();

            // Final state should be "leaving" so menu should close
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds((hoverDelay * 2) + 50)));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1,
                "Menu should close after rapid movement ending with pointer leave"));
        }

        [Test]
        public async Task Menu_ArrowDown_FocusSecondItem()
        {
            var comp = Context.Render<MenuKeydownTest>();

            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            var menuWrapper = comp.Find("[data-testid='menu-wrapper']");
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

            var last = comp.Instance.LastInvokedIndex;
            last.Should().Be(1);
        }

        [Test]
        public async Task Menu_ArrowUp_FocusLastItem()
        {
            var comp = Context.Render<MenuKeydownTest>();

            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            var allFocusableItems = comp.FindAll(".mud-menu-item[tabindex='0']");

            if (allFocusableItems.Count > 0)
            {
                var lastItem = allFocusableItems.Last();
                await lastItem.ClickAsync(new MouseEventArgs());
            }

            comp.Instance.LastInvokedIndex.Should().Be(6);
        }

        [Test]
        public async Task Menu_ArrowRight_OpensNestedMenu()
        {
            var comp = Context.Render<MenuKeydownTest>();

            // Open the menu (focus starts at index -1)
            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            // Press ArrowDown x4 to move to index 3 with nested menu
            var menuWrapper = comp.Find("[data-testid='menu-wrapper']");
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });

            // Press ArrowRight opens submenu
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

            // Should now be 2 open menus: the root and the submenu
            comp.FindAll(".mud-popover-open").Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Menu_ArrowRight_NoFurtherAction()
        {
            var comp = Context.Render<MenuKeydownTest>();

            // Open with the mouse, so no item is focused (_focusedIndex == -1).
            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            // With nothing focused, ArrowRight is a no-op and must not close the menu.
            var menuWrapper = comp.Find("[data-testid='menu-wrapper']");
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

            comp.FindAll(".mud-popover-open").Count.Should().Be(1);
        }

        [Test]
        public async Task Menu_ArrowLeft_ClosesMenu()
        {
            var comp = Context.Render<MenuKeydownTest>();

            // Open the menu (focus starts at index 0)
            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            // Press Arrow Left to close menu
            var menuWrapper = comp.Find("[data-testid='menu-wrapper']");
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });

            // Ensure all popovers are closed
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public async Task Menu_ArrowLeft_ClosesNestedMenu()
        {
            var comp = Context.Render<MenuKeydownTest>();

            // Open the root menu
            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            // Move focus to index 3 and open nested submenu
            var menuWrapper = comp.Find("[data-testid='menu-wrapper']");
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

            // Ensure we now have more than one menu open
            var openBefore = comp.FindAll(".mud-popover-open");
            openBefore.Count.Should().BeGreaterThan(1);

            // Simulate ArrowLeft keypress inside the last opened (nested) menu
            var nestedMenu = comp.FindAll("div[data-testid='menu-wrapper']").Last();
            await nestedMenu.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });

            // Assert the submenu was closed (back to just one open popover)
            var openAfter = comp.FindAll(".mud-popover-open");
            openAfter.Count.Should().Be(openBefore.Count - 1);
        }

        [Test]
        public async Task Menu_Enter_ClosesMenuOnLinkItem()
        {
            var comp = Context.Render<MenuKeydownTest>();

            // Open the root menu
            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            // Arrow down to second item (index 1, a link)
            var menuWrapper = comp.Find("[data-testid='menu-wrapper']");
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });

            // Press Enter to enter menu item
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

            // Assert: menu should be closed
            comp.FindAll(".mud-popover-open").Should().BeEmpty();
        }

        [Test]
        public async Task Menu_Tab_ClosesAllMenus()
        {
            var comp = Context.Render<MenuKeydownTest>();

            // Open the root menu
            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            // Arrow into nested menu
            var menuWrapper = comp.Find("[data-testid='menu-wrapper']");
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

            // Press Tab to tab menu item
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "Tab" });

            // Assert: menu should be closed
            comp.FindAll(".mud-popover-open").Should().BeEmpty();
        }

        [Test]
        public async Task Menu_Escape_ClosesSubmenuAndReturnsFocus()
        {
            var comp = Context.Render<MenuKeydownTest>();

            // Open the root menu
            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            // Arrow into nested menu
            var menuWrapper = comp.Find("[data-testid='menu-wrapper']");
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await menuWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

            // Press Escape to close submenu
            var nestedWrapper = comp.FindAll("[data-testid='menu-wrapper']").Last();
            await nestedWrapper.KeyDownAsync(new KeyboardEventArgs { Key = "Escape" });

            // Only parent menu should remain
            comp.FindAll(".mud-popover-open").Count.Should().Be(1);
        }

        [Test]
        public async Task Menu_DisabledItem_IsFocusableButNotInvokable()
        {
            var comp = Context.Render<MenuKeydownTest>();

            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            // Find the disabled item
            var disabledItem = comp.Find(".mud-menu-item.mud-disabled");

            // Verify it has the right accessibility attributes
            disabledItem.GetAttribute("aria-disabled").Should().Be("true");
            disabledItem.GetAttribute("tabindex").Should().Be("-1"); // Focusable by script, not by tab

            // Verify it's in the DOM and visible for screen readers
            disabledItem.Should().NotBeNull();
            disabledItem.TextContent.Should().Contain("5 Disabled");

            // Try to click it - it should NOT invoke the action
            await disabledItem.ClickAsync(new MouseEventArgs());

            // LastInvokedIndex should still be null because disabled items shouldn't invoke
            comp.Instance.LastInvokedIndex.Should().BeNull();
        }

        [Test]
        public async Task Menu_DisabledItem_HasCorrectAccessibilityAttributes()
        {
            var comp = Context.Render<MenuKeydownTest>();

            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            var disabledItem = comp.Find(".mud-menu-item.mud-disabled");

            // Test all the accessibility attributes
            disabledItem.GetAttribute("aria-disabled").Should().Be("true");
            disabledItem.GetAttribute("class").Should().Contain("mud-disabled");

            // The item should have the preventDefault attribute to stop normal click behavior
            disabledItem.GetAttribute("blazor:onclick:preventDefault").Should().NotBeNull();

            // But it should still be present in the DOM for screen readers
            disabledItem.TextContent.Should().Contain("5 Disabled");
        }

        [Test]
        public void TrackKeyboardInteraction_WhenMenuClosed_DoesNothing()
        {
            var comp = Context.Render<MenuKeydownTest>();
            var menu = comp.FindComponent<MudMenu>().Instance;

            // Menu should start closed
            menu.GetState(x => x.Open).Should().BeFalse();

            // Simulate an ArrowDown key press when the menu is closed
            comp.InvokeAsync(() =>
            {
                var method = typeof(MudMenu)
                    .GetMethod("TrackKeyboardInteraction", BindingFlags.NonPublic | BindingFlags.Instance);
                method!.Invoke(menu, new object[] { new KeyboardEventArgs { Key = "ArrowDown" } });
            });

            // Nothing should change
            menu.GetState(x => x.Open).Should().BeFalse();
        }

        [Test]
        public async Task TrackKeyboardInteraction_WhenArrowDown_FocusesFirstItem()
        {
            var comp = Context.Render<MenuKeydownTest>();
            var menu = comp.FindComponent<MudMenu>().Instance;

            // Open the menu through its activator
            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            // Simulate ArrowDown to move focus to the first item
            await comp.InvokeAsync(() =>
            {
                var method = typeof(MudMenu)
                    .GetMethod("TrackKeyboardInteraction", BindingFlags.NonPublic | BindingFlags.Instance);
                method!.Invoke(menu, new object[] { new KeyboardEventArgs { Key = "ArrowDown" } });
            });

            // Verify that the focused index is now 0 (the first item)
            var focusedIndex = (int)typeof(MudMenu)
                .GetField("_focusedIndex", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(menu)!;

            focusedIndex.Should().Be(0);
        }

        [Test]
        public async Task TrackKeyboardInteraction_WhenArrowUp_FocusesLastItem()
        {
            var comp = Context.Render<MenuKeydownTest>();
            var menu = comp.FindComponent<MudMenu>().Instance;

            // Open the menu through its activator
            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            // Simulate ArrowUp to move focus to the last item
            await comp.InvokeAsync(() =>
            {
                var method = typeof(MudMenu)
                    .GetMethod("TrackKeyboardInteraction", BindingFlags.NonPublic | BindingFlags.Instance);
                method!.Invoke(menu, new object[] { new KeyboardEventArgs { Key = "ArrowUp" } });
            });

            // Verify that the focused index is now the last in the menu
            var menuItems = (IReadOnlyList<object>)typeof(MudMenu)
                .GetField("_menuItems", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(menu)!;

            var focusedIndex = (int)typeof(MudMenu)
                .GetField("_focusedIndex", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(menu)!;

            focusedIndex.Should().Be(menuItems.Count - 1);
        }

        [Test]
        public void PopoverSettings_SetsDefaultValues()
        {
            var menu = Context.Render<MudMenu>();

            menu.Instance.PopoverFixed.Should().BeFalse();
            // When not set, should use global default from PopoverOptions
            menu.Instance.Modal.Should().BeNull();
        }

        [Test]
        public void PopoverSettings_OverridesDefaultValues()
        {
            var menu = Context.Render<MudMenu>(p =>
            {
                p.Add(p => p.PopoverFixed, true);
                p.Add(p => p.Modal, true);
            });

            menu.Instance.PopoverFixed.Should().BeTrue();
            menu.Instance.Modal.Should().BeTrue();
        }

        [Test]
        public void PopoverSettings_UsesGlobalDefaultsFromPopoverOptions()
        {
            // The default PopoverOptions should have OverflowBehavior.FlipAlways and ModalOverlay = false
            var menu = Context.Render<MudMenu>();

            // Access the resolved values through the private methods via reflection or by checking the rendered markup
            // Since we can't easily test private methods, we verify that the defaults are used correctly
            // by checking that the component doesn't throw when rendering without explicit values
            menu.Instance.Should().NotBeNull();

            // Verify that the component is using the global defaults
            // Modal should be null (using PopoverOptions defaults)
            menu.Instance.Modal.Should().BeNull();
        }

        [Test]
        public async Task NestedMenu_SubMenuArrow_PointsRightInLtr()
        {
            var comp = Context.Render<MenuWithNestingTest>();
            await comp.Find("button:contains('1')").ClickAsync();
            var icon = comp.Find(".mud-menu-submenu-icon");
            icon.InnerHtml.Should().Contain("M10 17l5-5-5-5v10z"); // ArrowRight path
        }

        [Test]
        public async Task NestedMenu_SubMenuArrow_PointsLeftInRtl()
        {
            var comp = Context.Render<MenuWithNestingTest>(p => p.AddCascadingValue("RightToLeft", true));
            await comp.Find("button:contains('1')").ClickAsync();
            var icon = comp.Find(".mud-menu-submenu-icon");
            icon.InnerHtml.Should().Contain("M14 7l-5 5 5 5V7z"); // ArrowLeft path
        }

        [Test]
        public void MenuContext_Constructor_NullMenu_Throws()
        {
            var act = () => new MenuContext(null!);

            act.Should().Throw<ArgumentNullException>().WithParameterName("menu");
        }

        [Test]
        public async Task Menu_EmptyMenu_KeyboardKeys_AreNoOps()
        {
            var comp = Context.Render<MenuEmptyKeydownTest>();
            var menu = comp.FindComponent<MudMenu>().Instance;

            await comp.Find("button.mud-button-root").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            // With no items, every keyboard handler returns early: the menu stays open and nothing throws.
            var wrapper = comp.Find("[data-testid='menu-wrapper']");
            foreach (var key in new[] { "ArrowDown", "ArrowUp", "ArrowRight", "ArrowLeft", "Enter", " ", "Tab", "Escape" })
            {
                await wrapper.KeyDownAsync(new KeyboardEventArgs { Key = key });
            }

            menu.GetState(x => x.Open).Should().BeTrue();
        }

        [Test]
        public async Task Menu_Escape_OnTopLevelMenu_ClosesMenu()
        {
            // Escape on a top-level (parent-less) menu takes the CloseAllMenusAsync branch; the nested case is covered elsewhere.
            var comp = Context.Render<MenuKeydownTest>();
            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            await comp.Find("[data-testid='menu-wrapper']").KeyDownAsync(new KeyboardEventArgs { Key = "Escape" });

            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
        }

        [Test]
        public async Task Menu_ArrowRight_OnFocusedItem_InvokesOnClick()
        {
            // ArrowRight on a focused leaf item invokes its OnClick directly (the Enter path goes through OnClickHandlerAsync instead).
            var comp = Context.Render<MenuKeydownTest>();
            await comp.Find(".mud-menu-button-activator").ClickAsync(new MouseEventArgs());

            var wrapper = comp.Find("[data-testid='menu-wrapper']");
            await wrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await wrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

            comp.Instance.LastInvokedIndex.Should().Be(1);
        }

        [Test]
        public async Task Menu_Enter_OnFocusedSubMenu_OpensSubmenu()
        {
            // Existing keyboard tests open submenus with ArrowRight; this covers Enter on a focused submenu.
            var comp = Context.Render<MenuSiblingSubmenusTest>();
            await comp.Find("button.mud-button-root").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            var wrapper = comp.Find("[data-testid='menu-wrapper']");
            await wrapper.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await wrapper.KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(2));
        }

        [Test]
        [TestCase("touch")]
        [TestCase("pen")]
        public async Task Menu_PointerEnter_TouchOrPen_DoesNotOpen(string pointerType)
        {
            // Touch and pen pointers are not hoverable, so a MouseOver menu must not open from them.
            var timeProvider = Context.AddFakeTimeProvider();
            var comp = Context.Render<MenuTestMouseOver>();
            var menu = comp.FindComponent<MudMenu>().Instance;

            await comp.Find("div.mud-menu").PointerEnterAsync(new PointerEventArgs { PointerType = pointerType });
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(MudGlobal.MenuDefaults.HoverDelay + 100)));

            menu.GetState(x => x.Open).Should().BeFalse();
        }

        [Test]
        public async Task Menu_ActivatorContent_KeyboardActivation_TogglesAndFocusesFirstItem()
        {
            var comp = Context.Render<MenuActivatorContentTest>();
            var menu = comp.FindComponent<MudMenu>().Instance;
            var activator = comp.Find("div.mud-menu-activator");

            // A non-activation key on the activator is ignored.
            await activator.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            menu.GetState(x => x.Open).Should().BeFalse();

            // Enter opens via HandleActivatorKeydown, and a keyboard-opened menu auto-focuses its first item.
            await activator.KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeTrue());
            await comp.WaitForAssertionAsync(() =>
            {
                var focusedIndex = (int)typeof(MudMenu)
                    .GetField("_focusedIndex", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .GetValue(menu)!;
                focusedIndex.Should().Be(0);
            });

            // Enter again closes; Space also opens.
            await activator.KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeFalse());
            await activator.KeyDownAsync(new KeyboardEventArgs { Key = " " });
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeTrue());
        }

        [Test]
        public async Task Menu_ActivatorContent_SyntheticClickAfterKeyboard_IsSuppressed()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            var comp = Context.Render<MenuActivatorContentTest>();
            var menu = comp.FindComponent<MudMenu>().Instance;
            var activator = comp.Find("div.mud-menu-activator");

            // Keyboard activation opens the menu and stamps the activation time.
            await activator.KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeTrue());

            // The synthetic mouse click the browser fires right after (<50ms) is suppressed: the menu stays open.
            await comp.Find("button.mud-button-root").ClickAsync(new MouseEventArgs { Button = 0 });
            menu.GetState(x => x.Open).Should().BeTrue();

            // Past the 50ms window, a real click is honored and closes it.
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(100)));
            await comp.Find("button.mud-button-root").ClickAsync(new MouseEventArgs { Button = 0 });
            await comp.WaitForAssertionAsync(() => menu.GetState(x => x.Open).Should().BeFalse());
        }

        [Test]
        public async Task Menu_SiblingSubMenu_OpeningOne_ClosesOther()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            var hoverDelay = MudGlobal.MenuDefaults.HoverDelay;
            var comp = Context.Render<MenuSiblingSubmenusTest>();

            await comp.Find("button.mud-button-root").ClickAsync();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);

            comp.Find("div.mud-menu:contains('Alpha')").PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(hoverDelay + 50)));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(2));

            // Opening the second sibling closes the first, so only root + Bravo remain (not three).
            comp.Find("div.mud-menu:contains('Bravo')").PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await comp.InvokeAsync(() => timeProvider.Advance(TimeSpan.FromMilliseconds(hoverDelay + 50)));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(2));
        }

        [Test]
        public async Task Menu_AnchorOrigin_Explicit_OverridesDefault()
        {
            // An explicit AnchorOrigin overrides the default top-level BottomLeft anchor.
            var comp = Context.Render<MenuAnchorOriginTest>(p => p.Add(x => x.AnchorOrigin, Origin.BottomRight));
            await comp.Find("button.mud-button-root").ClickAsync();

            var popover = comp.FindAll("div.mud-popover").Single(p => p.QuerySelector(".mud-menu-list") is not null);
            popover.ClassList.Should().Contain("mud-popover-anchor-bottom-right");
            popover.ClassList.Should().NotContain("mud-popover-anchor-bottom-left");
        }

        [Test]
        [TestCase(0, true)]   // ForceLoad + Href + no Target -> manual NavigateTo
        [TestCase(1, false)]  // Href only, no ForceLoad -> anchor handles navigation
        [TestCase(2, false)]  // ForceLoad + Href + Target -> anchor handles navigation
        public async Task MenuItem_ForceLoad_NavigatesOnlyWithHrefAndNoTarget(int itemIndex, bool shouldNavigate)
        {
            var nav = Context.Services.GetRequiredService<NavigationManager>();
            var comp = Context.Render<MenuForceLoadTest>();
            var before = nav.Uri;

            await comp.Find("button.mud-button-root").ClickAsync();
            await comp.FindAll("a.mud-menu-item")[itemIndex].ClickAsync();

            if (shouldNavigate)
            {
                nav.Uri.Should().EndWith("menu-force-target");
            }
            else
            {
                nav.Uri.Should().Be(before);
            }
        }

        [Test]
        public async Task OpenMenu_WhenFocusInteropThrows_DoesNotCrash()
        {
            // https://github.com/MudBlazor/MudBlazor/issues/12184
            // On MAUI BlazorWebView the wrapper-focus interop in OnAfterRenderAsync can run after
            // the element has been detached (rapid open/close, navigation), throwing
            // "Unable to focus an invalid element". Opening the menu must swallow that rather
            // than surface it as an unhandled JSException. This reproduces the reported stack
            // frame (MudMenu.OnAfterRenderAsync -> focus) as closely as bUnit allows.
            Context.JSInterop
                .SetupVoid("Blazor._internal.domWrapper.focus", _ => true)
                .SetException(new JSException("Unable to focus an invalid element."));

            var comp = Context.Render<MenuTest1>();

            var open = async () => await comp.Find("button.mud-button-root").ClickAsync();

            await open.Should().NotThrowAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));
        }

        [Test]
        public async Task FocusItemAsync_WhenFocusInteropThrows_DoesNotPropagate()
        {
            // https://github.com/MudBlazor/MudBlazor/issues/12184
            // Every keyboard navigation path funnels through FocusItemAsync (including the
            // fire-and-forget calls in TrackKeyboardInteraction). If the target element is
            // detached when focus runs, the JSException must be swallowed, not propagated.
            var comp = Context.Render<MenuTest1>();
            var menu = comp.FindComponent<MudMenu>().Instance;
            await comp.Find("button.mud-button-root").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            // Simulate the element being detached: the focus interop now throws.
            Context.JSInterop
                .SetupVoid("Blazor._internal.domWrapper.focus", _ => true)
                .SetException(new JSException("Unable to focus an invalid element."));

            var focus = async () => await comp.InvokeAsync(() => menu.FocusItemAsync(0));

            await focus.Should().NotThrowAsync();
        }

        [Test]
        public async Task FocusItemAsync_AfterDispose_DoesNotInvokeFocusInterop()
        {
            // https://github.com/MudBlazor/MudBlazor/issues/12184
            // When the menu is torn down mid-flight (navigation), queued focus work must
            // short-circuit on the _disposed guard and not run JS interop against the
            // disposed component / detached DOM.
            var comp = Context.Render<MenuTest1>();
            var menu = comp.FindComponent<MudMenu>().Instance;
            await comp.Find("button.mud-button-root").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            var focusCountBeforeDispose = Context.JSInterop.Invocations["Blazor._internal.domWrapper.focus"].Count;

            menu.Dispose();
            await comp.InvokeAsync(() => menu.FocusItemAsync(0));

            Context.JSInterop.Invocations["Blazor._internal.domWrapper.focus"].Count
                .Should().Be(focusCountBeforeDispose);
        }

        [Test]
        public async Task Dispose_CalledTwiceAfterHover_IsIdempotent()
        {
            // https://github.com/MudBlazor/MudBlazor/issues/12184 (review follow-up)
            // Dispose must be idempotent. A hovered menu holds a live CancellationTokenSource,
            // so a second Dispose() (e.g. user code disposing a @ref before the renderer tears
            // the component down) would otherwise call Cancel() on the already-disposed CTS and
            // throw ObjectDisposedException.
            var comp = Context.Render<MenuTestMouseOver>();
            var menu = comp.FindComponent<MudMenu>().Instance;

            // Hover to open, which creates the hover CancellationTokenSource.
            await comp.Find("div.mud-menu").PointerEnterAsync();
            await comp.WaitForAssertionAsync(() => comp.Markup.Should().Contain("mud-popover-open"));

            menu.Dispose();
            var secondDispose = () => menu.Dispose();

            secondDispose.Should().NotThrow();
        }

        [Test]
        public async Task OnClick_InvokedBeforeMenuCloses()
        {
            // https://github.com/MudBlazor/MudBlazor/issues/6349
            var popoverOpenDuringClick = false;

            // Render the provider separately so it hosts the menu's popover content.
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudMenu>(parameters => parameters
                .Add(p => p.Label, "Menu")
                .Add(p => p.ChildContent, builder =>
                {
                    builder.OpenComponent<MudMenuItem>(0);
                    builder.AddAttribute(1, nameof(MudMenuItem.Label), "Item");
                    builder.AddAttribute(2, nameof(MudMenuItem.OnClick), EventCallback.Factory.Create<MouseEventArgs>(this, async () =>
                    {
                        // Yield so a wrong ordering would dispose the menu mid-await.
                        await Task.Yield();
                        popoverOpenDuringClick = provider.FindAll("div.mud-popover-open").Count > 0;
                    }));
                    builder.CloseComponent();
                }));

            await comp.Find("button.mud-button-root").ClickAsync();
            await provider.WaitForAssertionAsync(() => provider.FindAll("div.mud-menu-item").Count.Should().Be(1));

            await provider.Find("div.mud-menu-item").ClickAsync();

            popoverOpenDuringClick.Should().BeTrue();
            await provider.WaitForAssertionAsync(() => provider.FindAll("div.mud-popover-open").Count.Should().Be(0));
        }
    }
}
