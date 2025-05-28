// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
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

            var comp = Context.RenderComponent<MenuTest1>();
            var menu = comp.FindComponent<MudMenu>();

            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("div.mud-menu-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);

            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("div.mud-menu-item")[0].Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));

            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("div.mud-menu-item")[0].Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));

            //Disabled item's click ot touch should not close popover
            comp.FindAll("button.mud-button-root")[0].Click();

            var menuItems = comp.FindComponents<MudMenuItem>();
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            menuItems[2].Instance.Disabled = true;
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.

            comp.FindAll("a.mud-menu-item")[1].Click();
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
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public void OpenMenu_ClickThirdItem_CheckClosed()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item")[1].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public void OpenMenu_ClickClassItem_CheckClass()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("div.mud-menu-item.test-class").Count.Should().Be(1);
        }

        [Test]
        public void OpenMenu_CheckClass()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.Find("div.mud-popover").ClassList.Should().Contain("menu-popover-class");
        }

        [Test]
        public async Task IsOpen_CheckState()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            var menu = comp.FindComponent<MudMenu>().Instance;
            menu.GetState(x => x.Open).Should().BeFalse();

            var args = new MouseEventArgs { OffsetX = 1.0, OffsetY = 1.0 };
            await comp.InvokeAsync(() => menu.OpenMenuAsync(args));
            menu.GetState(x => x.Open).Should().BeTrue();

            await comp.InvokeAsync(() => menu.CloseMenuAsync());
            menu.GetState(x => x.Open).Should().BeFalse();
        }

        [Test]
        public void MouseOver_PointerLeave_ShouldClose()
        {
            var comp = Context.RenderComponent<MenuTestMouseOver>();

            // Briefly hover over the button and wait for it to open.
            comp.Find("div.mud-menu").PointerEnter();
            comp.WaitForAssertion(() => comp.Markup.Should().Contain("mud-popover-open"));

            // Close it again and wait for that to happen.
            comp.Find("div.mud-menu").PointerLeave();
            comp.WaitForAssertion(() => comp.Markup.Should().NotContain("mud-popover-open"));
        }

        [Test]
        public async Task MouseOver_Hover_ShouldOpenMenu()
        {
            var comp = Context.RenderComponent<MenuTestMouseOver>();

            IElement Menu() => comp.Find(".mud-menu");
            comp.Markup.Should().NotContain("mud-popover-open");

            // Pointer over to menu to open popover
            await Menu().TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.WaitForAssertion(() => comp.Markup.Should().Contain("mud-popover-open"));

            // Popover open, captures pointer
            await Menu().TriggerEventAsync("onpointerleave", new PointerEventArgs());
            comp.WaitForAssertion(() => comp.Markup.Should().NotContain("mud-popover-open"));

            // Pointer moves to menu, still need to open
            await Menu().TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.WaitForAssertion(() => comp.Markup.Should().Contain("mud-popover-open"));
        }

        [Test]
        public async Task MouseOver_Click_ShouldKeepMenuOpen()
        {
            var comp = Context.RenderComponent<MenuTestMouseOver>();

            // Enter opens the menu (after a delay).
            comp.Find("div.mud-menu").PointerEnter();
            comp.WaitForAssertion(() => comp.Markup.Should().Contain("mud-popover-open"));

            // Clicking the button should close the menu.
            await comp.InvokeAsync(() => comp.Find("button.mud-button-root").Click());
            // Check that the component is closed
            comp.WaitForAssertion(() => comp.Markup.Should().NotContain("mud-popover-open"));

            // Clicking the button again should open the menu indefinitely.
            await comp.InvokeAsync(() => comp.Find("button.mud-button-root").Click());
            comp.WaitForState(() => comp.FindComponent<MudPopover>().Instance.Open);

            // Leaving the menu should no longer close it.
            comp.Find("div.mud-menu").PointerLeave();
            await Task.Delay(1000);
            comp.FindComponent<MudPopover>().Instance.Open.Should().BeTrue();

            // Hover the list shouldn't change anything.
            await comp.Find("div.mud-list").TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.FindComponent<MudPopover>().Instance.Open.Should().BeTrue();

            // Leave the list shouldn't change anything.
            await comp.Find("div.mud-list").TriggerEventAsync("onpointerleave", new PointerEventArgs());
            comp.FindComponent<MudPopover>().Instance.Open.Should().BeTrue();

            // Clicking the button should now close the menu.
            await comp.InvokeAsync(() => comp.Find("button.mud-button-root").Click());
            // Check that the component is closed
            comp.WaitForAssertion(() => comp.Markup.Should().NotContain("mud-popover-open"));
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
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("div.mud-menu-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[0].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Standart button menu -- right click
            comp.FindAll("button.mud-button-root")[1].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("div.mud-menu-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[1].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Icon button menu -- left click
            comp.FindAll("button.mud-button-root")[2].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("div.mud-menu-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[2].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Icon button menu -- right click
            comp.FindAll("button.mud-button-root")[3].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("div.mud-menu-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[3].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Activator content menu -- left click
            comp.FindAll("button.mud-button-root")[4].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("div.mud-menu-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[4].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Activator content menu -- right click
            comp.FindAll("button.mud-button-root")[5].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("div.mud-menu-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[5].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public void MenuItem_Should_RenderIcons()
        {
            var comp = Context.RenderComponent<MenuItemIconTest>();

            comp.Find(".mud-menu-button-activator").Click();
            comp.WaitForElement("div.mud-popover-open");

            comp.FindAll(".mud-menu-list div.mud-menu-item svg.mud-svg-icon.mud-menu-item-icon.mud-icon-size-medium").Count.Should().Be(3);
        }

        [Test]
        public void MenuItem_Should_RenderIconColors()
        {
            var comp = Context.RenderComponent<MenuItemIconTest>();

            comp.Find(".mud-menu-button-activator").Click();
            comp.WaitForElement("div.mud-popover-open");

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
            var comp = Context.RenderComponent<MenuErrorContenCaughtException>();
            await comp.FindAll("button.mud-button-root")[0].ClickAsync(new MouseEventArgs());
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            await comp.FindAll("div.mud-menu-item")[0].ClickAsync(new MouseEventArgs());
            var mudAlert = comp.FindComponent<MudAlert>();
            var text = mudAlert.Find("div.mud-alert-message");
            text.InnerHtml.Should().Be("Oh my! We caught an error and handled it!");
        }

        [Test]
        public void OpenMenu_CloseMenuOnClick_CheckStillOpen()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(2);
            comp.FindAll("div.mud-menu-item")[1].Click();
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
            comp.FindAll("div.mud-menu-item").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item").Count.Should().Be(3);
            comp.FindAll("a.mud-menu-item")[0].Attributes["href"].TextContent.Should().Be("https://www.test.com/1");
            comp.FindAll("a.mud-menu-item")[1].Attributes["href"].TextContent.Should().Be("https://www.test.com/2");
            comp.FindAll("a.mud-menu-item")[2].Click(); // disabled
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("a.mud-menu-item")[1].Click(); // enabled
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

            popover.OuterHtml.Should().Contain("top:0px;left:0px;");

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
        public void ContextMenu_WithLabel_Should_HaveButton_And_BeVisible()
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
        public void ContextMenu_WithActivatorContent_Should_HaveActivatorContent_And_BeVisible()
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

        [Test]
        public void Open_TwoWayBinding()
        {
            var comp = Context.RenderComponent<MenuTwoWayTest>();
            var menu = comp.FindComponent<MudMenu>();
            IElement SwitchElement() => comp.Find("#switch");

            menu.Instance.GetState(x => x.Open).Should().BeFalse("The menu should be closed initially.");
            comp.Instance.Open.Should().BeFalse();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0, "No popovers should be visible.");

            comp.Find("button.mud-button-root").Click();
            menu.Instance.GetState(x => x.Open).Should().BeTrue("Clicking the button should open the menu.");
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "One popover should be visible after opening.");

            SwitchElement().Change(false);
            menu.Instance.GetState(x => x.Open).Should().BeFalse("Manually setting Open to false should close the menu.");
            comp.Instance.Open.Should().BeFalse();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0, "Popover should disappear after closing.");

            comp.Find("button.mud-button-root").Click();
            menu.Instance.GetState(x => x.Open).Should().BeTrue("Clicking the button again should open the menu.");
            comp.Instance.Open.Should().BeTrue();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Popover should reappear after reopening.");

            SwitchElement().Change(true);
            menu.Instance.GetState(x => x.Open).Should().BeTrue("Setting Open to true again should not change the state.");
            comp.Instance.Open.Should().BeTrue();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Popover count should remain the same.");

            comp.Find("button.mud-button-root").Click();
            menu.Instance.GetState(x => x.Open).Should().BeFalse("Clicking the button should close the menu.");
            comp.Instance.Open.Should().BeFalse();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0, "Popover should no longer be visible.");

            comp.Find("button.mud-button-root").Click();
            menu.Instance.GetState(x => x.Open).Should().BeTrue("Clicking the button again should open the menu.");
            comp.Instance.Open.Should().BeTrue();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Popover should appear again.");
        }

        [Test]
        public void ActivatorClass()
        {
            var comp = Context.RenderComponent<MenuActivatorsTest>();

            comp.FindAll(".mud-menu")[0].FirstElementChild.ClassName.Should().Contain("mud-menu-button-activator");

            comp.FindAll(".mud-menu")[1].FirstElementChild.ClassName.Should().Contain("mud-menu-icon-button-activator");

            comp.FindAll(".mud-menu")[2].FirstElementChild.ClassName.Should().Contain("mud-menu-activator");

            comp.FindAll(".mud-menu")[3].FirstElementChild.Click();
            comp.Find(".mud-popover-open > .mud-menu-list .mud-menu-item.mud-menu-sub-menu-activator").Should().NotBeNull();
        }

        [Test]
        public void ShouldRenderLabelOrChildContent()
        {
            var comp = Context.RenderComponent<MenuItemLabelTest>();

            var childContent = comp.FindAll(".mud-menu-item")[0].InnerHtml;
            var label = comp.FindAll(".mud-menu-item")[1].InnerHtml;
            childContent.Should().BeEquivalentTo(label);

            // ChildContent should override Label.
            comp.FindAll(".mud-menu-item")[2].InnerHtml.Should().Contain("ContentText");
            comp.FindAll(".mud-menu-item")[2].InnerHtml.Should().NotContain("LabelText");
        }

        [Test]
        public void OpenNestedMenu()
        {
            var comp = Context.RenderComponent<MenuWithNestingTest>();

            // Open the first menu.
            comp.Find("button:contains('1')").Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);

            // Click the nested menu item to open the nested menu.
            comp.Find("div.mud-menu-item:contains('1.3')").Click();

            // Ensure both the main menu and the nested menu are open
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2);
        }

        [Test]
        public void ClickingMenuItem_ClosesNestedMenu()
        {
            var comp = Context.RenderComponent<MenuWithNestingTest>();

            // Open the first menu.
            comp.Find("button:contains('1')").Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);

            // Click the nested menu item to open the nested menu.
            comp.Find("div.mud-menu-item:contains('1.3')").Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2);

            // Click a non-nested menu item inside the nested menu.
            comp.Find("div.mud-menu-item:contains('2.2')").Click();

            // Ensure all popovers are closed.
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public async Task IActivatable_Activate_Should_ToggleMenu_Except_For_InputAdornmentIconButton()
        {
            // Arrange
            var comp = Context.RenderComponent<MudMenu>(parameters => parameters
                .Add(p => p.ActivatorContent, builder =>
                {
                    builder.OpenComponent<MudIconButton>(0);
                    builder.AddAttribute(1, "Class", "mud-no-activator");
                    builder.CloseComponent();
                }));
            var menu = comp.Instance;
            var activatable = (Interfaces.IActivatable)menu;

            // Normal case
            activatable.Activate(new object(), new MouseEventArgs());
            comp.WaitForAssertion(() => menu.GetState(x => x.Open).Should().BeTrue("Menu should open when Activate is called with a regular object"));

            // Close Menu
            activatable.Activate(new object(), new MouseEventArgs());
            comp.WaitForAssertion(() => menu.GetState(x => x.Open).Should().BeFalse("Menu should be closed after calling Activate again"));

            // Special case with icon button
            var classButton = comp.FindComponent<MudIconButton>().Instance;
            activatable.Activate(classButton, new MouseEventArgs());
            comp.WaitForAssertion(() => menu.GetState(x => x.Open).Should().BeFalse("Menu should not open when Activate is called with an mud-no-activator selector"));

            // Special case with regular button
            var compButton = Context.RenderComponent<MudButton>(p => p.Add(p => p.Class, "mud-no-activator")).Instance;
            activatable.Activate(compButton, new MouseEventArgs());
            comp.WaitForAssertion(() => menu.GetState(x => x.Open).Should().BeFalse("Menu should not open when Activate is called with an mud-no-activator selector"));

            // Clean up
            await menu.CloseMenuAsync();
        }

        [Test]
        public void Menu_ButtonActivator()
        {
            var provider = Context.RenderComponent<MudPopoverProvider>();
            var comp = Context.RenderComponent<MudMenu>(parameters => parameters
                .Add(p => p.ActivatorContent, builder =>
                {
                    builder.OpenComponent<MudButton>(0);
                    builder.CloseComponent();
                }));

            provider.FindAll("div.mud-popover-open").Count.Should().Be(0);

            // Click the MudButton inside the ActivatorContent
            var button = comp.Find("button.mud-button-root");
            button.Click();
            provider.FindAll("div.mud-popover-open").Count.Should().Be(1);
            button.Click();
            // Render component with MudIconButton inside ActivatorContent
            comp = Context.RenderComponent<MudMenu>(parameters => parameters
                .Add(p => p.ActivatorContent, builder =>
                {
                    builder.OpenComponent<MudIconButton>(0);
                    builder.AddAttribute(1, "Class", "mud-icon-button-activator");
                    builder.CloseComponent();
                }));

            provider.FindAll("div.mud-popover-open").Count.Should().Be(0);

            // Click the MudIconButton inside the ActivatorContent
            comp.Find("button.mud-icon-button-activator").Click();
            provider.FindAll("div.mud-popover-open").Count.Should().Be(1);
        }

        [Test]
        public async Task Menu_PointerEvents_ShowHide_WithDebounce()
        {
            // This method uses CatchAndLog to allow async events to run syncronously so we can test timing
            // Set a predictable hover delay for testing
            var hoverDelay = 300;
            MudGlobal.MenuDefaults.HoverDelay = hoverDelay;

            var comp = Context.RenderComponent<MenuWithNestingTest>();

            // Open the main menu first
            comp.Find("button:contains('1')").Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Main menu should be open");

            // 1. Test SHOW debounce behavior
            // Trigger pointer enter on submenu item
            var menuItem = comp.Find("div.mud-menu:contains('1.3')");

            // Immediately after hover, submenu should not be visible yet (debounce in effect)
            menuItem.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Submenu should not open immediately");

            // After the hover delay, submenu should become visible
            await Task.Delay(hoverDelay + 50);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2, "Submenu should open after hover delay");

            // 2. Test HIDE debounce behavior

            // Trigger pointer leave
            menuItem.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();

            // Immediately after leave, submenu should still be visible (hide debounce in effect)
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2, "Submenu should remain open immediately after pointer leave");

            // Wait less than the delay
            await Task.Delay(hoverDelay / 2);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2, "Submenu should still be open before hide delay completes");

            // After the full delay, submenu should close
            await Task.Delay(hoverDelay + 50);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Submenu should close after full hide delay (2x hover delay)");
        }

        [Test]
        public async Task Menu_PointerEvents_Cancellation()
        {
            // This method uses CatchAndLog to allow async events to run syncronously so we can test timing
            // Set a predictable hover delay for testing
            var hoverDelay = 300;
            MudGlobal.MenuDefaults.HoverDelay = hoverDelay;

            var comp = Context.RenderComponent<MenuWithNestingTest>();

            // Open the main menu first
            comp.Find("button:contains('1')").Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Main menu should be open");

            // 1. Test cancellation of SHOW debounce

            var menus = comp.FindComponents<MudMenu>();
            var menu = menus.FirstOrDefault(x => x.Instance.Label == "1.3").Instance;
            menu.Should().NotBeNull();

            // Start hover, then leave before debounce completes
            menu.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await Task.Delay(50);
            menu._showDebouncer.Cancel();

            // Wait for original show debounce to have completed
            await Task.Delay(hoverDelay + 100);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1,
                "Submenu should not open because show debounce was cancelled by pointer leave");

            // 2. Test cancellation of HIDE debounce

            // First, open the submenu properly, use await since we are testing this
            await menu.PointerEnterAsync(new PointerEventArgs());
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2, "Submenu should be open");

            // Start leave sequence, but re-enter before hide completes
            menu.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await Task.Delay(hoverDelay / 2);
            menu._hideDebouncer.Cancel();

            // Wait for what would have been the full hide delay
            await Task.Delay(hoverDelay + 100);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2,
                "Submenu should still be open because hide debounce was cancelled by re-entering");
        }

        [Test]
        public async Task Menu_PointerEvents_MultipleLevels()
        {
            // This method uses CatchAndLog to allow async events to run syncronously so we can test timing
            // Set a predictable hover delay for testing
            var hoverDelay = 300;
            MudGlobal.MenuDefaults.HoverDelay = hoverDelay;

            var comp = Context.RenderComponent<MenuWithNestingTest>();

            // Open the main menu first
            comp.Find("button:contains('1')").Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Main menu should be open");

            // Open first level submenu
            var menuItem1 = comp.Find("div.mud-menu:contains('1.3')");
            menuItem1.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await Task.Delay(hoverDelay + 100);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2, "First level submenu should be open");

            // Open second level submenu
            var menuItem2 = comp.Find("div.mud-menu:contains('2.1')");
            menuItem2.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await Task.Delay(hoverDelay + 100);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(3, "Second level submenu should be open");

            // Leaving second level should close only that level after delay
            menuItem2.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await Task.Delay((hoverDelay * 2) + 100);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2,
                "Second level should close but first level should remain open");

            // Leaving first level should close it after delay
            menuItem1.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await Task.Delay((hoverDelay * 2) + 100);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1,
                "First level should close but main menu should remain open");
        }

        [Test]
        public async Task Menu_PointerEvents_RapidMovement()
        {
            // This method uses CatchAndLog to allow async events to run syncronously so we can test timing
            // Set a predictable hover delay for testing
            var hoverDelay = 300;
            MudGlobal.MenuDefaults.HoverDelay = hoverDelay;

            var comp = Context.RenderComponent<MenuWithNestingTest>();

            // Open the main menu first
            comp.Find("button:contains('1')").Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Main menu should be open");

            var menuItem = comp.Find("div.mud-menu:contains('1.3')");

            // Simulate rapid mouse movement: enter -> leave -> enter -> leave -> enter
            menuItem.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await Task.Delay(50);
            menuItem.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await Task.Delay(50);
            menuItem.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await Task.Delay(50);
            menuItem.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await Task.Delay(50);
            menuItem.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();

            // Final state should be "entering" so menu should open
            await Task.Delay(hoverDelay + 50);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(2,
                "Menu should open after rapid movement ending with pointer enter");

            // Now rapid movement ending with leaving
            menuItem.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();
            await Task.Delay(50);
            menuItem.PointerEnterAsync(new PointerEventArgs()).CatchAndLog();
            await Task.Delay(50);
            menuItem.PointerLeaveAsync(new PointerEventArgs()).CatchAndLog();

            // Final state should be "leaving" so menu should close
            await Task.Delay((hoverDelay * 2) + 50);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1,
                "Menu should close after rapid movement ending with pointer leave");
        }
    }
}
