using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            var comp = Context.RenderComponent<MenuTest1>();
            var menu = comp.FindComponent<MudMenu>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);

            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            var menuItems = comp.FindComponents<MudMenuItem>();
            await comp.InvokeAsync(() => menuItems[0].Instance.OnTouchHandler(new TouchEventArgs()));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));

            comp.FindAll("button.mud-button-root")[0].Click();
            menuItems = comp.FindComponents<MudMenuItem>();
            await comp.InvokeAsync(() => menuItems[1].Instance.OnTouchHandler(new TouchEventArgs()));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));

            //Disabled item's click ot touch should not close popover
            comp.FindAll("button.mud-button-root")[0].Click();
            menuItems = comp.FindComponents<MudMenuItem>();
#pragma warning disable BL0005
            await comp.InvokeAsync(() => menuItems[2].Instance.Disabled = true);
            await comp.InvokeAsync(() => menuItems[2].Instance.OnTouchHandler(new TouchEventArgs()));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => menu.Instance.ToggleMenuTouch(new TouchEventArgs()));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
            await comp.InvokeAsync(() => menu.Instance.ToggleMenuTouch(new TouchEventArgs()));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));
        }

        [Test]
        public void OpenMenu_ClickSecondItem_CheckClosed()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item")[1].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public void OpenMenu_ClickThirdItem_CheckClosed()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item")[2].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public void OpenMenu_ClickClassItem_CheckClass()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item.test-class").Count.Should().Be(1);
        }

        [Test]
        public void OpenMenu_CheckClass()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.Find("div.mud-popover").ClassList.Should().Contain("menu-popover-class");
        }

        [Test]
        public async Task MenuMouseLeave_CheckClosed()
        {
            var comp = Context.RenderComponent<MenuTestMouseOver>();
            var pop = comp.FindComponent<MudPopover>();
            comp.FindAll("button.mud-button-root")[0].Click();

            var list = comp.FindAll("div.mud-list")[0];

            await list.TriggerEventAsync("onmouseenter", new MouseEventArgs());
            await list.TriggerEventAsync("onmouseleave", new MouseEventArgs());

            comp.WaitForAssertion(() => pop.Instance.Open.Should().BeFalse());
        }

        [Test]
        public async Task MenuMouseLeave_MenuMouseEnter_CheckOpen()
        {
            var comp = Context.RenderComponent<MenuTestMouseOver>();
            var pop = comp.FindComponent<MudPopover>();

            // Mouse over to menu to open popover
            var menu = comp.Find(".mud-menu");
            await menu.TriggerEventAsync("onmouseenter", new MouseEventArgs());

            // Popover open, captures mouse
            await menu.TriggerEventAsync("onmouseleave", new MouseEventArgs());
            await comp.FindAll("div.mud-list")[0].TriggerEventAsync("onmouseenter", new MouseEventArgs());

            // Mouse moves to menu, still need to open
            await comp.FindAll("div.mud-list")[0].TriggerEventAsync("onmouseleave", new MouseEventArgs());
            await menu.TriggerEventAsync("onmouseenter", new MouseEventArgs());

            comp.WaitForAssertion(() => pop.Instance.Open.Should().BeTrue());
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
        public void MenuTest_LeftAndRightClick_CheckClosed()
        {
            //Standart button menu -- left click
            var comp = Context.RenderComponent<MenuTestVariants>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[0].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Standart button menu -- right click
            comp.FindAll("button.mud-button-root")[1].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[1].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Icon button menu -- left click
            comp.FindAll("button.mud-button-root")[2].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[2].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Icon button menu -- right click
            comp.FindAll("button.mud-button-root")[3].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[3].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Activator content menu -- left click
            comp.FindAll("button.mud-button-root")[4].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            comp.FindAll("button.mud-button-root")[4].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            //Activator content menu -- right click
            comp.FindAll("button.mud-button-root")[5].Click(new MouseEventArgs() { Button = 2 });
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
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
    }
}
