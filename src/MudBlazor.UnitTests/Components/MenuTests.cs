#pragma warning disable IDE1006 // leading underscore

using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class MenuTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();


        [Test]
        public void OpenMenu_ClickFirstItem_CheckClosed()
        {
            var comp = ctx.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public void OpenMenu_ClickSecondItem_CheckClosed()
        {
            var comp = ctx.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item")[1].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public void OpenMenu_ClickThirdItem_CheckClosed()
        {
            var comp = ctx.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item")[2].Click();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public void OpenMenu_ClickClassItem_CheckClass()
        {
            var comp = ctx.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root")[0].Click();
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
            comp.FindAll("div.mud-list-item.test-class").Count.Should().Be(1);
        }
    }
}
