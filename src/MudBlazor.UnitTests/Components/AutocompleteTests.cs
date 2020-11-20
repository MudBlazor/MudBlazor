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

    }
}
