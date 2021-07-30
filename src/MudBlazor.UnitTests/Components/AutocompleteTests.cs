// Copyright (c) mudblazor 2021
// License MIT

#pragma warning disable CS1998 // async without await
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
    public class AutocompleteTests : BunitTest
    {
        /// <summary>
        /// Initial value should be shown and popup should not open.
        /// </summary>
        [Test]
        public async Task AutocompleteTest1()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;

            //No popover-open, due it's closed
            comp.Markup.Should().NotContain("mud-popover-open");

            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");

            // now let's type a different state to see the popup open
            autocompletecomp.Find("input").Input("Calif");

            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            Console.WriteLine(comp.Markup);
            var items = comp.FindComponents<MudListItem>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");
            // click on California!
            comp.Find("div.mud-list-item").Click();
            // check state
            comp.WaitForAssertion(() => autocomplete.Value.Should().Be("California"));
            autocomplete.Text.Should().Be("California");
        }

        /// <summary>
        /// Popup should open when 3 characters are typed and close when below.
        /// </summary>
        [Test]
        public async Task AutocompleteTest2()
        {
            var comp = Context.RenderComponent<AutocompleteTest2>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudAutocomplete<string>>();

            var inputControl = comp.Find("div.mud-input-control");

            // check initial state
            comp.Markup.Should().NotContain("mud-popover-open");

            // click and check if it has toggled the menu
            inputControl.Click();
            comp.WaitForAssertion(() => comp.Markup.Should().NotContain("mud-popover-open"));

            // type 3 characters and check if it has toggled the menu
            select.Find("input").Input("ala");
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            // type 2 characters and check if it has toggled the menu
            select.Find("input").Input("al");
            comp.WaitForAssertion(() => comp.Markup.Should().NotContain("mud-popover-open"));
        }

        /// <summary>
        /// Autocomplete should show 'Assam' (using ToStringFunc)
        /// </summary>
        [Test]
        public void AutocompleteTest3()
        {
            var comp = Context.RenderComponent<AutocompleteTest3>();
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
            var comp = Context.RenderComponent<AutocompleteTest4>();
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
            var comp = Context.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            autocompletecomp.SetParam(x => x.DebounceInterval, 0);
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

        /// <summary>
        /// We search for a value not in list and value coercion will force the invalid value to be applied
        /// allowing to validate the user input.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AutocompleteCoerceValueTest()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            autocompletecomp.SetParam(x => x.DebounceInterval, 0);
            autocompletecomp.SetParam(x => x.CoerceValue, true); // if CoerceValue==true CoerceText will be ignored
            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
            // set a value the search won't find
            autocompletecomp.SetParam(p => p.Text, "Austria"); // not part of the U.S.

            // now trigger the coercion by toggling the the menu (it won't even open for invalid values, but it will coerce)
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            comp.WaitForAssertion(() => autocomplete.Value.Should().Be("Austria"));
            autocomplete.Text.Should().Be("Austria");
        }

        [Test]
        public async Task AutocompleteCoercionOffTest()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            autocompletecomp.SetParam(x => x.DebounceInterval, 0);
            autocompletecomp.SetParam(x => x.CoerceText, false);
            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
            // set a value the search won't find
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            autocompletecomp.SetParam(a => a.Text, "Austria");
            // now trigger the coercion by closing the menu
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Austria");
        }

        [Test]
        public async Task Autocomplete_Should_TolerateNullFromSearchFunc()
        {
            var comp = Context.RenderComponent<MudAutocomplete<string>>((a) =>
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
            var comp = Context.RenderComponent<AutocompleteTest5>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("div.mud-input-adornment")[0].Click();
            Console.WriteLine(comp.Markup);
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
        }

        /// <summary>
        /// After press Enter key down, the selected value should be shown in the input value
        /// </summary>

        [Test]
        public async Task Autocomplete_after_Enter_Should_show_Selected_Value()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;

            //insert "Calif"
            autocompletecomp.Find("input").Input("Calif");
            await Task.Delay(100);
            var args = new KeyboardEventArgs();
            args.Key = "Enter";

            //press Enter key
            autocompletecomp.Find("input").KeyUp(args);

            //The value of the input should be California
            comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("California"));

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
            var comp = Context.RenderComponent<AutocompleteSetParametersInitialization>();
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
            var comp = Context.RenderComponent<MudAutocomplete<string>>((a) =>
            {
                a.Add(x => x.OnBlur, fn);
            });
            var input = comp.Find("input");

            calls.Should().Be(0);
            input.Blur();
            calls.Should().Be(1);
        }

        [Test]
        public async Task AutoCompleteClearableTest()
        {
            var comp = Context.RenderComponent<AutocompleteTestClearable>();
            // No button when initialized empty
            comp.WaitForAssertion(() => comp.FindAll("button").Should().BeEmpty());

            // Button shows after entering text
            comp.Find("input").Input("text");
            comp.WaitForAssertion(() => comp.Find("button").Should().NotBeNull());
            // Text cleared and button removed after clicking clear button
            comp.Find("button").Click();
            comp.WaitForAssertion(() => comp.FindAll("button").Should().BeEmpty());

            // Button shows again after entering text
            comp.Find("input").Input("text");
            comp.WaitForAssertion(() => comp.Find("button").Should().NotBeNull());
            // Button removed after clearing text
            comp.Find("input").Input(string.Empty);
            comp.WaitForAssertion(() => comp.FindAll("button").Should().BeEmpty());
        }

        [Test]
        public async Task Autocomplete_Should_Validate_Data_Attribute_Fail()
        {
            var comp = Context.RenderComponent<AutocompleteValidationDataAttrTest>();
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
            var comp = Context.RenderComponent<AutocompleteValidationDataAttrTest>();
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

        /// <summary>
        /// Tests the required property.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_SetRequiredTrue()
        {
            var comp = Context.RenderComponent<AutocompleteRequiredTest>();

            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            autocomplete.Required.Should().BeTrue();

            await comp.InvokeAsync(() => autocomplete.Validate());

            autocomplete.ValidationErrors.First().Should().Be("Required");
        }

        /// <summary>
        /// Test for <seealso cref="https://github.com/Garderoben/MudBlazor/issues/1761"/>
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Close_OnTab()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;

            // Should be closed
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

            // Lets type something to cause it to open
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

            // Lets call blur on the input and confirm that it closed
            autocompletecomp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Tab" });
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

            // Tab closes the drop-down and does not select the selected value (California)
            // because SelectValueOnTab is false by default
            autocomplete.Value.Should().Be("Alabama");
        }

        [Test]
        public async Task Autocomplete_Should_SelectValue_On_Tab_With_SelectValueOnTab()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            autocompletecomp.SetParam(x => x.SelectValueOnTab, true);
            var autocomplete = autocompletecomp.Instance;

            // Should be closed
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

            // Lets type something to cause it to open
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

            // Lets call blur on the input and confirm that it closed
            autocompletecomp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Tab" });
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

            // Tab closes the drop-down and selects the selected value (California)
            // because SelectValueOnTab is true
            autocomplete.Value.Should().Be("California");
        }

        /// <summary>
        /// When selecting a value by clicking on it in the list the input will blur. However, this
        /// must not cause the dropdown to close or else the click on the item will not be possible!
        ///
        /// If this test fails it means the dropdown has closed before we can even click any value in the list.
        /// Such a regression happened and caused PR #1807 to be reverted
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_NotCloseDropdownOnInputBlur()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;

            //No popover-open, due it's closed
            comp.Markup.Should().NotContain("mud-popover-open");

            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");

            // now let's type a different state to see the popup open
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            var items = comp.FindComponents<MudListItem>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");

            // now, we blur the input and assert that the popover is still open.
            autocompletecomp.Find("input").Blur();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
        }

        /// <summary>
        /// When calling Clear() the popup should not open and Value and Text should be cleared.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_CloseOnClear()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            autocompletecomp.SetParam(x => x.CoerceValue, true);
            var autocomplete = autocompletecomp.Instance;

            //No popover-open, due it's closed
            comp.Markup.Should().NotContain("mud-popover-open");

            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");

            // Clearing it
            await comp.InvokeAsync(() => autocomplete.Clear().Wait());

            comp.WaitForAssertion(() => comp.Markup.Should().NotContain("mud-popover-open"));
            autocomplete.Value.Should().Be("");
            autocomplete.Text.Should().Be("");

            // now let's type a different state to see the popup open
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            Console.WriteLine(comp.Markup);
            var items = comp.FindComponents<MudListItem>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");

            // Clearing it should close the popup
            await comp.InvokeAsync(() => autocomplete.Clear().Wait());

            comp.WaitForAssertion(() => comp.Markup.Should().NotContain("mud-popover-open"));
            autocomplete.Value.Should().Be("");
            autocomplete.Text.Should().Be("");
        }
    }
}
