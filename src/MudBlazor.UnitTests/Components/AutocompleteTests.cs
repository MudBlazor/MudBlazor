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
    public class AutocompleteTests
    {
        /// <summary>
        /// Initial value should be shown and popup should not open.
        /// </summary>
        [Test]
        public async Task AutocompleteTest1() {
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            var comp = ctx.RenderComponent<AutocompleteTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;
            var menu = comp.Find("div.mud-popover");

            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
            menu.ClassList.Should().NotContain("mud-popover-open");
            await Task.Delay(100);
            menu = comp.Find("div.mud-popover");
            menu.ClassList.Should().NotContain("mud-popover-open");

            // now let's type a different state to see the popup open
            autocomplete.Text = "Calif";
            await Task.Delay(100);
            menu = comp.Find("div.mud-popover");
            menu.ClassList.Should().Contain("mud-popover-open");
            Console.WriteLine(comp.Markup);
            var items=comp.FindComponents<MudListItem>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");
            // click on California!
            comp.Find("div.mud-list-item").Click();
            // check state
            autocomplete.Value.Should().Be("California");
            autocomplete.Text.Should().Be("California");
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
            await Task.Delay(100);
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
        }

        /// <summary>
        /// Popup should open when 3 characters are typed and close when below.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AutocompleteTest2()
        {
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            var comp = ctx.RenderComponent<AutocompleteTest2>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudAutocomplete<string>>();
            var menu = comp.Find("div.mud-popover");
            var inputControl = comp.Find("div.mud-input-control");

            // check initial state
            menu.ClassList.Should().NotContain("mud-popover-open");

            // click and check if it has toggled the menu
            inputControl.Click();
            await Task.Delay(100);
            menu.ClassList.Should().NotContain("mud-popover-open");

            // type 3 characters and check if it has toggled the menu
            select.Instance.Text = "ala";
            await Task.Delay(100);
            menu.ClassList.Should().Contain("mud-popover-open");

            // type 2 characters and check if it has toggled the menu
            select.Instance.Text = "al";
            await Task.Delay(100);
            menu.ClassList.Should().NotContain("mud-popover-open");
        }
    }
}
