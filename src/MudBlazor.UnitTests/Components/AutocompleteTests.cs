// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static MudBlazor.UnitTests.TestComponents.AutocompleteSetParametersInitialization;

namespace MudBlazor.UnitTests.Components
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

            //No popover, due it's closed
            comp.Markup.Should().NotContain("mud-popover");

            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
            await Task.Delay(100);


            // now let's type a different state to see the popup open
            autocompletecomp.Find("input").Input("Calif");
            await Task.Delay(100);
            var menu = comp.Find("div.mud-popover");
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

            var inputControl = comp.Find("div.mud-input-control");

            // check initial state
            comp.Markup.Should().NotContain("mud-popover");

            // click and check if it has toggled the menu
            inputControl.Click();
            comp.WaitForAssertion(() => comp.Markup.Should().NotContain("mud-popover"));

            // type 3 characters and check if it has toggled the menu
            select.Find("input").Input("ala");
            await Task.Delay(200);
            var menu = comp.Find("div.mud-popover");
            comp.WaitForAssertion(() => menu.ClassList.Should().Contain("mud-popover-open"));

            // type 2 characters and check if it has toggled the menu
            select.Find("input").Input("al");
            comp.WaitForAssertion(() => menu.ClassList.Should().NotContain("mud-popover-open"));
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
            autocompletecomp.SetParam(a => a.Text, "Austria"); // not part of the U.S.
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
            autocompletecomp.SetParam(x => x.DebounceInterval, 0);
            autocompletecomp.SetParam(x => x.CoerceText, false);
            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
            // set a value the search won't find
            autocompletecomp.SetParam(a => a.Text, "Austria");
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            // now trigger the coercion by closing the menu
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Austria");
        }

        [Test]
        public async Task Autocomplete_Should_TolerateNullFromSearchFunc()
        {
            var comp = ctx.RenderComponent<MudAutocomplete<string>>((a) =>
            {
                a.Add(x => x.DebounceInterval, 0);
                a.Add(x => x.SearchFunc, new Func<string, Task<IEnumerable<string>>>(async s => null)); // <--- searchfunc returns null instead of sequence
            });
            // enter a text so the search func will return null, and it shouldn't throw an exception
            comp.SetParam(a => a.Text, "Do not throw");
            comp.SetParam(x => x.SearchFunc, new Func<string, Task<IEnumerable<string>>>(s => null)); // <-- search func returns null instead of task!
            comp.SetParam(a => a.Text, "Don't throw here neither");
        }

        [Test]
        public async Task Autocomplete_ReadOnly_Should_Not_Open()
        {
            var comp = ctx.RenderComponent<AutocompleteTest5>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("div.mud-input-adornment")[0].Click();
            Console.WriteLine(comp.Markup);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        /// <summary>
        /// After press Enter key down, the selected value should be shown in the input value
        /// </summary>

        [Test]
        public async Task Autocomplete_after_Enter_Should_show_Selected_Value()
        {
            var comp = ctx.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;

            var input = autocompletecomp.Find("input");

            //insert "Calif"
            input.Input("Calif");
            await Task.Delay(100);
            var args = new KeyboardEventArgs();
            args.Key = "Enter";

            //press Enter key
            input.KeyUp(args);
            input = autocompletecomp.Find("input");
            var wrappedElement = ((dynamic)input).WrappedElement;
            var value = ((IHtmlInputElement)wrappedElement).Value;

            //The value of the input should be California
            value.Should().Be("California");

            //and the autocomplete it's closed
            autocomplete.IsOpen.Should().BeFalse();
        }

        /// <summary>
        /// Based on this try https://try.mudblazor.com/snippet/GacPunvDUyjdUJAh
        /// and this issue https://github.com/Garderoben/MudBlazor/issues/1235
        /// </summary>

        [Test]
        public async Task Autocomplete_Initialize_Value_on_SetParametersAsync()
        {
            var comp = ctx.RenderComponent<AutocompleteSetParametersInitialization>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            await Task.Delay(100);
            var autocompletecomp = comp.FindComponent<MudAutocomplete<ExternalList>>();
            var input = autocompletecomp.Find("input");

            var wrappedElement = ((dynamic)input).WrappedElement;
            var value = ((IHtmlInputElement)wrappedElement).Value;

            //The value of the input should be California
            value.Should().Be("One");


        }

        /// <summary>
        /// Test for <seealso cref="https://github.com/Garderoben/MudBlazor/issues/1415"/>
        /// </summary>
        [Test]
        public async Task Autocomplete_OnBlurShouldBeCalled()
        {
            var calls = 0;
            Action<FocusEventArgs> fn = (args) => calls++;
            var comp = ctx.RenderComponent<MudAutocomplete<string>>((a) =>
            {
                a.Add(x => x.OnBlur, fn);
            });
            var input = comp.Find("input");

            calls.Should().Be(0);
            input.Blur();
            calls.Should().Be(1);
        }


        #region DataAttribute validation
        [Test]
        public async Task Autocomplete_Should_Validate_Data_Attribute_Fail()
        {
            var comp = ctx.RenderComponent<AutocompleteValidationDataAttrTest>();
            Console.WriteLine(comp.Markup);
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            await comp.InvokeAsync(() => autocomplete.DebounceInterval = 0);
            // Set invalid option
            await comp.InvokeAsync(() => autocomplete.SelectOption("Quux"));
            // check initial state
            autocomplete.Value.Should().Be("Quux");
            autocomplete.Text.Should().Be("Quux");
            // check validity
            await comp.InvokeAsync(() => autocomplete.Validate());
            autocomplete.ValidationErrors.Should().NotBeEmpty();
            autocomplete.ValidationErrors.Should().HaveCount(1);
            autocomplete.ValidationErrors[0].Should().Equals("Should not be longer than 3");
        }

        [Test]
        public async Task Autocomplete_Should_Validate_Data_Attribute_Success()
        {
            var comp = ctx.RenderComponent<AutocompleteValidationDataAttrTest>();
            Console.WriteLine(comp.Markup);
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            await comp.InvokeAsync(() => autocomplete.DebounceInterval = 0);
            // Set valid option
            await comp.InvokeAsync(() => autocomplete.SelectOption("Qux"));
            // check initial state
            autocomplete.Value.Should().Be("Qux");
            autocomplete.Text.Should().Be("Qux");
            // check validity
            await comp.InvokeAsync(() => autocomplete.Validate());
            autocomplete.ValidationErrors.Should().BeEmpty();
        }
        #endregion
    }
}
