// Copyright (c) mudblazor 2021
// License MIT

#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static MudBlazor.UnitTests.TestComponents.AutocompleteSetParametersInitialization;
using static Bunit.ComponentParameterFactory;
using Microsoft.AspNetCore.Components;
using System.Threading;

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
            ////Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            await comp.InvokeAsync(() => autocomplete.FocusAsync());
            //No popover-open, due it's closed
            comp.Markup.Should().NotContain("mud-popover-open");

            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");

            // now let's type a different state to see the popup open
            autocompletecomp.Find("input").Input("Calif");

            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            //Console.WriteLine(comp.Markup);
            var items = comp.FindComponents<MudListItem>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");
            // click on California!
            comp.Find("div.mud-list-item").Click();
            // check popover class
            comp.Find("div.mud-popover").ClassList.Should().Contain("autocomplete-popover-class");
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
            //Console.WriteLine(comp.Markup);
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
            //Console.WriteLine(comp.Markup);
            var autocomplete = comp.FindComponent<MudAutocomplete<AutocompleteTest3.State>>().Instance;
            autocomplete.Text.Should().Be("Assam");
        }

        /// <summary>
        /// Autocomplete id should propagate to label for attribute
        /// </summary>
        [Test]
        public void AutocompleteLabelFor()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var label = comp.FindAll(".mud-input-label");
            label[0].Attributes.GetNamedItem("for")?.Value.Should().Be("autocompleteLabelTest");
        }

        /// <summary>
        /// Autocomplete should show 'Assam' (using state.ToString())
        /// </summary>
        [Test]
        public void AutocompleteTest4()
        {
            var comp = Context.RenderComponent<AutocompleteTest4>();
            //Console.WriteLine(comp.Markup);
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
            //Console.WriteLine(comp.Markup);
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            autocompletecomp.SetParam(x => x.DebounceInterval, 0);
            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
            // set a value the search won't find
            autocompletecomp.SetParam(a => a.Text, "Austria"); // not part of the U.S.
            // IsOpen must be true to properly simulate a user clicking outside of the component, which is what the next ToggleMenu call below does.
            autocomplete.IsOpen.Should().BeTrue();
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
            //Console.WriteLine(comp.Markup);
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
            //Console.WriteLine(comp.Markup);
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
            //Console.WriteLine(comp.Markup);
            comp.FindAll("div.mud-input-adornment")[0].Click();
            //Console.WriteLine(comp.Markup);
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
        }

        /// <summary>
        /// MoreItemsTemplate should render when there are more items than the MaxItems limit
        /// </summary>
        [Test]
        public async Task AutocompleteTest6()
        {
            var comp = Context.RenderComponent<AutocompleteTest6>();

            var inputControl = comp.Find("div.mud-input-control");
            inputControl.Click();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            var mudText = comp.FindAll("p.mud-typography");
            mudText[mudText.Count - 1].InnerHtml.Should().Contain("Not all items are shown"); //ensure the text is shown
        }

        /// <summary>
        /// NoItemsTemplate should render when there are no items
        /// </summary>
        [Test]
        public async Task AutocompleteTest7()
        {
            var comp = Context.RenderComponent<AutocompleteTest7>();

            var inputControl = comp.Find("div.mud-input-control");
            inputControl.Click();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            var mudText = comp.FindAll("p.mud-typography");
            mudText[mudText.Count - 1].InnerHtml.Should().Contain("No items found, try another search"); //ensure the text is shown
        }

        /// <summary>
        /// After press Enter key down, the selected value should be shown in the input value
        /// </summary>

        [Test]
        public async Task Autocomplete_after_Enter_Should_show_Selected_Value()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            //Console.WriteLine(comp.Markup);
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
        /// and this issue https://github.com/MudBlazor/MudBlazor/issues/1235
        /// </summary>
        [Test]
        public async Task Autocomplete_Initialize_Value_on_SetParametersAsync()
        {
            var comp = Context.RenderComponent<AutocompleteSetParametersInitialization>();
            //Console.WriteLine(comp.Markup);
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
        /// Test for <seealso cref="https://github.com/MudBlazor/MudBlazor/issues/1415"/>
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
            //Console.WriteLine(comp.Markup);
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
            autocomplete.ValidationErrors[0].Should().Be("Should not be longer than 3");
        }

        [Test]
        public async Task Autocomplete_Should_Validate_Data_Attribute_Success()
        {
            var comp = Context.RenderComponent<AutocompleteValidationDataAttrTest>();
            //Console.WriteLine(comp.Markup);
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
        /// Test for <seealso cref="https://github.com/MudBlazor/MudBlazor/issues/1761"/>
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Close_OnTab()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            //Console.WriteLine(comp.Markup);
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
            //Console.WriteLine(comp.Markup);
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
            //Console.WriteLine(comp.Markup);
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
        /// When calling Clear(), menu should closed, Value and Text should be cleared.
        /// </summary>
        [Test]
        public async Task Autocomplete_CheckTextValueandOpenState_OnClear()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            autocompletecomp.SetParam(x => x.CoerceValue, true);
            var autocomplete = autocompletecomp.Instance;

            //No popover-open, due it's closed
            comp.Markup.Should().NotContain("mud-popover-open");

            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");

            // ToggleMenu to open menu and Clear to close it and check the text and value
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            await comp.InvokeAsync(() => autocomplete.Clear().Wait());
            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.Value.Should().Be("");
            autocomplete.Text.Should().Be("");

            // now let's type a different state
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            //Console.WriteLine(comp.Markup);
            var items = comp.FindComponents<MudListItem>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");

            // Clearing it and check the close status text and value again
            await comp.InvokeAsync(() => autocomplete.Clear().Wait());
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            autocomplete.Value.Should().Be("");
            autocomplete.Text.Should().Be("");
        }

        /// <summary>
        /// When calling Clear(), menu should closed, Value and Text should be cleared.
        /// </summary>
        [Test]
        public async Task Autocomplete_CheckTextValueCleared_OnClear()
        {
            // define some constant values
            var alaskaString = "Alaska";
            var listItemQuerySelector = "div.mud-list-item";

            var selectedItemIndexPropertyInfo = typeof(MudAutocomplete<string>).GetField("_selectedListItemIndex", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new ArgumentException("Cannot find field named '_selectedListItemIndex' on type 'MudAutocomplete<T>'");
            var onInputKeyUpMemberInfo = typeof(MudAutocomplete<string>).GetMethod("OnInputKeyUp", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new ArgumentException("Cannot find method named 'OnInputKeyUp' on type 'MudAutocomplete<T>'");

            // create the component
            var component = Context.RenderComponent<AutocompleteDisabledItemsTest>();

            // get the elements needed for the test
            var autocompleteComponent = component.FindComponent<MudAutocomplete<string>>();

            // Set the clear function on value changed
            autocompleteComponent.SetCallback(x => x.ValueChanged, async x => await autocompleteComponent.Instance.Clear());

            // get the instance
            var autocompleteInstance = autocompleteComponent.Instance;

            // click to open the popup
            autocompleteComponent.Find(TagNames.Input).Click();

            // ensure popup is open
            component.WaitForAssertion(() => autocompleteInstance.IsOpen.Should().BeTrue("Input has been focused and should open the popup"));

            // get the matching states
            var matchingStates = component.FindComponents<MudListItem>().ToArray();

            // try clicking 'Alaska'
            matchingStates.Single(s => s.Markup.Contains(alaskaString)).Find(listItemQuerySelector).Click();
            component.WaitForAssertion(() => autocompleteInstance.Text.Should().Be(string.Empty));
        }

        /// <summary>
        /// When calling Reset(), menu should closed, Value and Text should be null.
        /// </summary>
        [Test]
        public async Task Autocomplete_CheckTextAndValue_OnReset()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            autocompletecomp.SetParam(x => x.CoerceValue, true);
            var autocomplete = autocompletecomp.Instance;

            //No popover-open, due it's closed
            comp.Markup.Should().NotContain("mud-popover-open");

            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");

            // Reset it
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            await comp.InvokeAsync(() => autocomplete.Reset());
            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.Value.Should().Be(null);
            autocomplete.Text.Should().Be(null);

            // now let's type a different state
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            //Console.WriteLine(comp.Markup);
            var items = comp.FindComponents<MudListItem>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");

            // Reseting it should close popover and set Text and Value to null again
            await comp.InvokeAsync(() => autocomplete.Reset());
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            autocomplete.Value.Should().Be(null);
            autocomplete.Text.Should().Be(null);
        }

        [Test]
        public async Task Autocomplete_Should_Not_Select_Disabled_Item()
        {
            // define some constant values
            var alabamaString = "Alabama";
            var alaskaString = "Alaska";
            var americanSamoaString = "American Samoa";
            var arkansasString = "Arkansas";
            var listItemQuerySelector = "div.mud-list-item";
            var selectedItemClassName = "mud-selected-item";

            var selectedItemIndexPropertyInfo = typeof(MudAutocomplete<string>).GetField("_selectedListItemIndex", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new ArgumentException("Cannot find field named '_selectedListItemIndex' on type 'MudAutocomplete<T>'");
            var onInputKeyUpMemberInfo = typeof(MudAutocomplete<string>).GetMethod("OnInputKeyUp", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new ArgumentException("Cannot find method named 'OnInputKeyUp' on type 'MudAutocomplete<T>'");

            // create the component
            var component = Context.RenderComponent<AutocompleteDisabledItemsTest>();

            // get the elements needed for the test
            var autocompleteComponent = component.FindComponent<MudAutocomplete<string>>();

            // get the instance
            var autocompleteInstance = autocompleteComponent.Instance;

            // click to open the popup
            autocompleteComponent.Find(TagNames.Input).Click();

            // ensure popup is open
            component.WaitForAssertion(() => autocompleteInstance.IsOpen.Should().BeTrue("Input has been focused and should open the popup"));

            // get the matching states
            var matchingStates = component.FindComponents<MudListItem>().ToArray();

            // try clicking 'American Samoa'
            matchingStates.Single(s => s.Markup.Contains(americanSamoaString)).Find(listItemQuerySelector).Click();
            component.WaitForAssertion(() => autocompleteInstance.Value.Should().BeNullOrEmpty($"{americanSamoaString} should not be clickable."));

            // try clicking 'Alaska'
            matchingStates.Single(s => s.Markup.Contains(alaskaString)).Find(listItemQuerySelector).Click();
            component.WaitForAssertion(() => autocompleteInstance.Value.Should().Be(alaskaString));

            // reset search-string
            autocompleteComponent.Find(TagNames.Input).Input(string.Empty);

            // wait till popup is visible
            component.WaitForAssertion(() => autocompleteInstance.IsOpen.Should().BeTrue());

            // update found elements
            matchingStates = component.FindComponents<MudListItem>().ToArray();

            // ensure alabama is selected
            component.WaitForAssertion(() => matchingStates.Single(s => s.Markup.Contains(alabamaString)).Find(listItemQuerySelector).ClassList.Should().Contain(selectedItemClassName, $"{alabamaString} should be selected/highlighted"));

            // define the event-args for arrow-down
            var arrowDownKeyboardEventArgs = new KeyboardEventArgs { Key = Key.Down.Value, Type = "keyup" };

            // invoke directly (but twice)
            onInputKeyUpMemberInfo.Invoke(autocompleteInstance, new[] { arrowDownKeyboardEventArgs });
            onInputKeyUpMemberInfo.Invoke(autocompleteInstance, new[] { arrowDownKeyboardEventArgs });

            // ensure that index '4' is selected
            component.WaitForAssertion(() => selectedItemIndexPropertyInfo.GetValue(autocompleteInstance).Should().Be(4));

            // select the highlighted value
            component.Find(TagNames.Input).KeyUp(Key.Enter);

            // Arkansas should be selected value
            autocompleteInstance.Value.Should().Be(arkansasString);
        }

        /// <summary>
        /// When changing the bound value, ensure the new value is displayed
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Autocomplete_ChangeBoundValue()
        {
            await ImproveChanceOfSuccess(async () =>
            {
                var comp = Context.RenderComponent<AutocompleteChangeBoundObjectTest>();
                var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
                var autocomplete = autocompletecomp.Instance;
                autocompletecomp.SetParametersAndRender(parameters => parameters.Add(p => p.DebounceInterval, 0));
                autocompletecomp.SetParametersAndRender(parameters => parameters.Add(p => p.CoerceText, true));
                // this needs to be false because in the unit test the autocomplete's input does not lose focus state on click of another button.
                // TextUpdateSuppression is used to avoid binding to change the input text while typing.  
                autocompletecomp.SetParametersAndRender(parameters => parameters.Add(p => p.TextUpdateSuppression, false));
                // check initial state
                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Florida"));
                autocomplete.Value.Should().Be("Florida");
                autocomplete.Text.Should().Be("Florida");

                //Get the button to toggle the value
                comp.Find("button").Click();
                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Georgia"));
                autocomplete.Value.Should().Be("Georgia");
                autocomplete.Text.Should().Be("Georgia");

                //Change the value of the current bound value component
                //insert "Alabam"
                autocompletecomp.Find("input").Input("Alabam");
                await Task.Delay(100);

                //press Enter key
                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "Enter" }));
                //ensure autocomplete is closed and new value is committed/bound
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "Enter" }));
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "Escape" }));
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "ArrowUp" }));
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "ArrowUp", AltKey = true }));
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "ArrowDown" }));
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "Escape" }));
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "NumpadEnter" }));
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "ArrowDown" }));
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "ArrowDown" }));

                //The value of the input should be Alabama
                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Alabama"));
                autocomplete.Value.Should().Be("Alabama");
                autocomplete.Text.Should().Be("Alabama");

                //Again Change the bound object
                comp.Find("button").Click();

                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Florida"));
                autocomplete.Value.Should().Be("Florida");
                autocomplete.Text.Should().Be("Florida");

                //Change the bound object back and check again.
                comp.Find("button").Click();
                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Alabama"));
                autocomplete.Value.Should().Be("Alabama");
                autocomplete.Text.Should().Be("Alabama");

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "ArrowUp" }));
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "ArrowUp" }));
                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Alabama"));

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "ArrowUp" }));
                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyDown(new KeyboardEventArgs() { Key = "Tab" }));
                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "Tab" }));
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

                autocompletecomp.SetParam("SelectValueOnTab", true);
                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "ArrowUp" }));
                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyDown(new KeyboardEventArgs() { Key = "Tab" }));
                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Alabama"));

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "Backspace", CtrlKey = true, ShiftKey = true }));
                comp.WaitForAssertion(() => autocompletecomp.Instance.Value.Should().Be(null));

                await comp.InvokeAsync(async () => await autocomplete.OnInputKeyDown(new KeyboardEventArgs() { Key = "Tab" }));
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());
                //Check popover is closed if coerce text is true (it fixed with a PR)
                autocomplete.CoerceText = true;
                await comp.InvokeAsync(() => autocomplete.OnInputKeyUp(new KeyboardEventArgs() { Key = "Enter" }));
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());
                await comp.InvokeAsync(() => autocomplete.OnEnterKey());
                autocompletecomp.Find("input").Input("abc");
                await comp.InvokeAsync(() => autocomplete.SelectAsync());
                await comp.InvokeAsync(() => autocomplete.SelectRangeAsync(0, 1));
                autocompletecomp.Find("input").Input("");
                await comp.InvokeAsync(() => autocomplete.ToggleMenu());
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

                await comp.InvokeAsync(() => autocomplete.OnEnterKey());
                comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());
            });
        }

        [Test]
        public async Task Autocomplete_Should_Support_Sync_Search()
        {
            var root = Context.RenderComponent<AutocompleteSyncTest>();

            var popoverProvider = root.FindComponent<MudPopoverProvider>();
            var autocomplete = root.FindComponent<MudAutocomplete<string>>();
            var popover = autocomplete.FindComponent<MudPopover>();

            popover.Instance.Open.Should().BeFalse("Should start as closed");

            await autocomplete
                .Find($".{AutocompleteSyncTest.AutocompleteClass}")
                .ClickAsync(new MouseEventArgs());

            popoverProvider.WaitForAssertion(() =>
            {
                popover.Instance.Open.Should().BeTrue("Should be open once clicked");

                popoverProvider
                    .FindComponents<MudListItem>().Count
                    .Should().Be(AutocompleteSyncTest.Items.Length, "Should show the expected items");
            });
        }

        /// <summary>
        /// The adornment icon should change live without having to re-open the autocomplete
        /// This test a bugfix where changing the icon property would not cause the icon to visually change until the autocomplete was opened or closed
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_ChangeAdornmentIcon()
        {
            var icon = Parameter(nameof(AutocompleteAdornmentChange.Icon), Icons.Filled.Abc);
            var comp = Context.RenderComponent<AutocompleteAdornmentChange>(icon);
            var instance = comp.Instance;

            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;

            var markupBefore = comp.Find("svg.mud-icon-root").Children.ToMarkup().Trim();

            // change icon and render again
            instance.Icon = Icons.Filled.Remove;

            comp.Render();

            // check the initial icon
            var markupAfter = comp.Find("svg.mud-icon-root").Children.ToMarkup().Trim();
            markupAfter.Should().NotBe(markupBefore);
        }

        [Test]
        public async Task Autocomplete_Should_NotIndicateLoadingByDefault()
        {
            // Arrange
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();

            comp.Markup.Should().NotContain("progress-indicator-circular");
            autocompletecomp.Find("input").Input("Calif");

            // Test
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress"));
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().NotContain("progress-indicator-circular"));
        }

        [Test]
        public async Task Autocomplete_Should_IndicateLoadingWithCircularProgressIndicator()
        {
            // Arrange
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            autocompletecomp.SetParam(x => x.ShowProgressIndicator, true);
            autocompletecomp.SetParam(x => x.Adornment, null);
            autocompletecomp.SetParam(x => x.Adornment, null);

            comp.Markup.Should().NotContain("progress-indicator-circular");
            autocompletecomp.Find("input").Input("Calif");

            // Test show
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").ClassList.Should().Contain("mud-autocomplete--with-progress"));
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().Contain("progress-indicator-circular"));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            // Test hide
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress"));
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().NotContain("progress-indicator-circular"));
        }

        [Test]
        public async Task Autocomplete_Should_IndicateLoadingWithCircularProgressIndicatorAndAdornmentAdjustment()
        {
            // Arrange
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            autocompletecomp.SetParam(x => x.ShowProgressIndicator, true);
            autocompletecomp.SetParam(x => x.AdornmentIcon, Icons.Filled.Info);
            autocompletecomp.SetParam(x => x.Adornment, Adornment.End);

            comp.Markup.Should().NotContain("progress-indicator-circular");
            autocompletecomp.Find("input").Input("Calif");

            // Test show
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").ClassList.Should().Contain("mud-autocomplete--with-progress"));
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().Contain("progress-indicator-circular"));
            comp.WaitForAssertion(() => comp.Find("div.progress-indicator-circular").ClassList.Should().Contain("progress-indicator-circular--with-adornment"));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            // Test hide
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress"));
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().NotContain("progress-indicator-circular"));
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().NotContain("progress-indicator-circular--with-adornment"));
        }

        [Test]
        public async Task Autocomplete_Should_IndicateLoadingWithCustomProgressIndicator()
        {
            // Arrange
            RenderFragment fragment = builder =>
            {
                builder.AddContent(0, "Loading...");
            };

            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();

            autocompletecomp.SetParam(x => x.ShowProgressIndicator, true);
            autocompletecomp.SetParam(p => p.ProgressIndicatorTemplate, fragment);

            comp.Markup.Should().NotContain("Loading...");
            autocompletecomp.Find("input").Input("Calif");

            // Test show
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").ClassList.Should().Contain("mud-autocomplete--with-progress"));
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().Contain("Loading..."));
            
            // Test hide
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress"));
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().NotContain("Loading..."));
        }

        [Test]
        public async Task Autocomplete_Should_IndicateLoadingWithProgressIndicatorInsidePopover()
        {
            // Arrange
            RenderFragment fragment = builder =>
            {
                builder.AddContent(0, "Loading...");
            };

            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();

            autocompletecomp.SetParam(x => x.ShowProgressIndicator, true);
            autocompletecomp.SetParam(p => p.ProgressIndicatorInPopoverTemplate, fragment);

            comp.Markup.Should().NotContain("Loading...");
            autocompletecomp.Find("input").Input("Calif");

            // Test show
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").ClassList.Should().Contain("mud-autocomplete--with-progress"));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ToMarkup().Should().Contain("Loading..."));

            // Test hide
            comp.WaitForAssertion(() => comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress"));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ToMarkup().Should().NotContain("Loading..."));
        }

        [Test]
        public async Task Autocomplete_Should_Cancel_Search()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();

            // Arrange first call

            CancellationToken? cancelToken = null;

            var first = new TaskCompletionSource<IEnumerable<string>>();

            autocompletecomp.SetParam(p => p.SearchFuncWithCancel, new Func<string, CancellationToken, Task<IEnumerable<string>>>((s, cancellationToken) => {
                cancelToken = cancellationToken;
                // Return task that never completes.
                return first.Task;
            }));

            comp.Find("input").Input("Foo");

            await Task.Delay(20);

            // Test

            comp.WaitForAssertion(() => Assert.IsFalse(cancelToken?.IsCancellationRequested));

            // Arrange second call
            
            var second = new TaskCompletionSource<IEnumerable<string>>();

            autocompletecomp.SetParam(p => p.SearchFuncWithCancel, new Func<string, CancellationToken, Task<IEnumerable<string>>>((s, cancellationToken) => 
            {
                return second.Task;
            }));

            comp.Find("input").Input("Bar");

            await Task.Delay(20);

            // Test

            comp.WaitForAssertion(() => Assert.IsTrue(cancelToken?.IsCancellationRequested));

            first.SetCanceled();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ToMarkup().Should().NotContain("Foo"));

            second.SetResult(new List<string> { "Bar" });
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ToMarkup().Should().Contain("Bar"));
        }

        [Test]
        public async Task Autocomplete_FullWidth()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompleteComp = comp.FindComponent<MudAutocomplete<string>>();

            autocompleteComp.Find("div.mud-select").ClassList.Should().Contain("mud-autocomplete");
            autocompleteComp.Find("div.mud-select").ClassList.Should().NotContain("mud-width-full");

            autocompleteComp.SetParam(p => p.FullWidth, true);

            autocompleteComp.Find("div.mud-select").ClassList.Should().Contain("mud-autocomplete");
            autocompleteComp.Find("div.mud-select").ClassList.Should().Contain("mud-width-full");
        }
    }
}
