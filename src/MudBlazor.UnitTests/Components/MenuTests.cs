
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
        public void OpenMenu_ClickFirstItem_CheckClosed()
        {
            var comp = Context.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
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
            var list = comp.FindAll("div.mud-list")[0];

            await list.TriggerEventAsync("onmouseenter", new MouseEventArgs());
            await list.TriggerEventAsync("onmouseleave", new MouseEventArgs());

            comp.WaitForAssertion(() => pop.Instance.Open.Should().BeFalse());
        }
    }
}
