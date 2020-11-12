using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;
using static MudBlazor.UnitTests.SelectWithEnumTest;

namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class SelectTests
    {
        [Test]
        public void SelectTest1() {
            // Click should open the Menu and selecting a value should update the bindable value.
            // setup
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            var comp = ctx.RenderComponent<SelectTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");

            // check initial state
            select.Instance.Value.Should().BeNullOrEmpty();
            menu.ClassList.Should().NotContain("mud-popover-open");
            // click and check if it has toggled the menu
            input.Click();
            menu.ClassList.Should().Contain("mud-popover-open");

            // now click an item and see the value change
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            // menu should be closed now
            menu.ClassList.Should().NotContain("mud-popover-open");
            select.Instance.Value.Should().Be("2");
            // now we cheat and click the list without opening the menu ;)
            items[0].Click();
            select.Instance.Value.Should().Be("1");
        }

        [Test]
        public async Task MultiSelectTest1()
        {
            //Click should not close the menu and selecting multiple values should update the bindable value with a comma separated list.
            // setup
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            var comp = ctx.RenderComponent<MultiSelectTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
                var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");

            // check initial state
            select.Instance.Value.Should().BeNullOrEmpty();
            menu.ClassList.Should().NotContain("mud-popover-open");
            // click and check if it has toggled the menu
            input.Click();
            menu.ClassList.Should().Contain("mud-popover-open");

            // now click an item and see the value change
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            // menu should still be open now!!
            menu.ClassList.Should().Contain("mud-popover-open");
            select.Instance.Text.Should().Be("2");
            items[0].Click();
            select.Instance.Text.Should().Be("2, 1");
            items[2].Click();
            select.Instance.Text.Should().Be("2, 1, 3");
            items[0].Click();
            select.Instance.Text.Should().Be("2, 3");
            select.Instance.SelectedValues.Count.Should().Be(2);
            select.Instance.SelectedValues.Should().Contain("2");
            select.Instance.SelectedValues.Should().Contain("3");
            //Console.WriteLine(comp.Markup);
            const string @unchecked =
                "M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z";
            const string @checked =
                "M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z";
            var icons = comp.FindAll("div.mud-list-item path").ToArray();
            // check that the correct items are checked
            icons[0].Attributes["d"].Value.Should().Be(@unchecked);
            icons[1].Attributes["d"].Value.Should().Be(@checked);
            icons[2].Attributes["d"].Value.Should().Be(@checked);

            // now check how setting the SelectedValues makes items checked or unchecked
            await comp.InvokeAsync(() => {
                select.Instance.SelectedValues = new HashSet<string>() { "1", "2" };
            });

            icons = comp.FindAll("div.mud-list-item path").ToArray();
            icons[0].Attributes["d"].Value.Should().Be(@checked);
            icons[1].Attributes["d"].Value.Should().Be(@checked);
            icons[2].Attributes["d"].Value.Should().Be(@unchecked);

        }

        [Test]
        public async Task SelectWithEnumTest()
        {
            // Initial Text should be enums default value
            // setup
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            var comp = ctx.RenderComponent<SelectWithEnumTest>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<MyEnum>>();
            select.Instance.Value.Should().Be(default(MyEnum));
            select.Instance.Text.Should().Be(default(MyEnum).ToString());
        }
    }
}
