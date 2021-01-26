#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class AutocompleteTests
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

        /// <summary>
        /// Initial value should be shown and popup should not open.
        /// </summary>
        [Test]
        public async Task AutocompleteTest1()
        {
            var comp = ctx.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            var menu = comp.Find("div.mud-popover");

            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
            menu.ClassList.Should().NotContain("mud-popover-open");
            await Task.Delay(100);
            menu = comp.Find("div.mud-popover");
            menu.ClassList.Should().NotContain("mud-popover-open");

            // now let's type a different state to see the popup open
            autocompletecomp.Find("input").Input("Calif");
            await Task.Delay(100);
            menu = comp.Find("div.mud-popover");
            menu.ClassList.Should().Contain("mud-popover-open");
            Console.WriteLine(comp.Markup);
            var items = comp.FindComponents<MudListItem>().ToArray();
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
        [Test]
        public async Task AutocompleteTest2()
        {
            var comp = ctx.RenderComponent<AutocompleteTest2>();
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
            select.Find("input").Input("ala");
            await Task.Delay(100);
            menu.ClassList.Should().Contain("mud-popover-open");

            // type 2 characters and check if it has toggled the menu
            select.Find("input").Input("al");
            await Task.Delay(100);
            menu.ClassList.Should().NotContain("mud-popover-open");
        }

        /// <summary>
        /// Autocomplete should show 'Assam' (using ToStringFunc)
        /// </summary>
        [Test]
        public void AutocompleteTest3()
        {
            var comp = ctx.RenderComponent<AutocompleteTest3>();
            Console.WriteLine(comp.Markup);
            var autocomplete = comp.FindComponent<MudAutocomplete<AutocompleteTest3.State>>().Instance;
            autocomplete.Text.Should().Be("Assam");
        }

        /// <summary>
        /// Autocomplete should show 'Assam' (using state.ToString())
        /// </summary>
        [Test]
        public void AutocompleteTest4()
        {
            var comp = ctx.RenderComponent<AutocompleteTest4>();
            Console.WriteLine(comp.Markup);
            var autocomplete = comp.FindComponent<MudAutocomplete<AutocompleteTest4.State>>().Instance;
            autocomplete.Text.Should().Be("Assam");
        }

        /// <summary>
        /// We search for a value not in list and coercion will go back to the last valid value,
        /// discarding the current search text.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AutocompleteCoercionTest()
        {
            var comp = ctx.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            await comp.InvokeAsync(() => autocomplete.DebounceInterval = 0);
            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
            // set a value the search won't find
            autocompletecomp.Find("input").Input("Austria"); // not part of the U.S. thank god
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            // now trigger the coercion by closing the menu 
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
        }

        [Test]
        public async Task AutocompleteCoercionOffTest()
        {
            var comp = ctx.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            await comp.InvokeAsync(() => autocomplete.DebounceInterval = 0);
            await comp.InvokeAsync(() => autocomplete.CoerceText = false);
            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
            // set a value the search won't find
            autocompletecomp.Find("Input").Input("Austria");
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            // now trigger the coercion by closing the menu 
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Austria");
        }

    }
}
