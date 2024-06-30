// Copyright (c) mudblazor 2021
// License MIT

#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.Dummy;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
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
            var items = comp.FindComponents<MudListItem<string>>().ToArray();
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
            // select elements needed for the test
            var select = comp.FindComponent<MudAutocomplete<string>>();

            // check initial state
            comp.Markup.Should().NotContain("mud-popover-open");

            // focus and check if it has toggled the menu
            select.Find("input").Focus();
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
            var autocomplete = comp.FindComponent<MudAutocomplete<AutocompleteTest4.State>>().Instance;
            autocomplete.Text.Should().Be("Assam");
        }

        /// <summary>
        /// We search for a value not in list and coercion will go back to the last valid value,
        /// discarding the current search text.
        /// </summary>
        [Test]
        public async Task AutocompleteCoercionTest()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            autocompletecomp.SetParam(x => x.DebounceInterval, 0);
            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
            // set a value the search won't find
            autocompletecomp.SetParam(a => a.Text, "Austria"); // not part of the U.S.
            // Open must be true to properly simulate a user clicking outside of the component, which is what the next ToggleMenu call below does.
            autocompletecomp.WaitForAssertion(() => autocomplete.Open.Should().BeTrue());
            // now trigger the coercion by closing the menu
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
        }

        /// <summary>
        /// We search for a value not in list and value coercion will force the invalid value to be applied
        /// allowing to validate the user input.
        /// </summary>
        [Test]
        public async Task AutocompleteCoerceValueTest()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
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
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            comp.WaitForAssertion(() => autocomplete.Value.Should().Be("Austria"));
            autocomplete.Text.Should().Be("Austria");
        }

        /// <summary>
        /// Test to cover issue #5993.
        /// </summary>
        [Test]
        public async Task AutocompleteImmediateCoerceValueTest()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            autocompletecomp.SetParam(x => x.DebounceInterval, 0);
            autocompletecomp.SetParam(x => x.CoerceValue, true);
            autocompletecomp.SetParam(x => x.CoerceText, false);
            autocompletecomp.SetParam(x => x.Immediate, true);
            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
            // set a value the search won't find
            autocompletecomp.SetParam(p => p.Text, "Austria"); // not part of the U.S.

            comp.WaitForAssertion(() => autocomplete.Value.Should().Be("Austria"));
            autocomplete.Text.Should().Be("Austria");
        }

        [Test]
        public async Task AutocompleteCoercionOffTest()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            autocompletecomp.SetParam(x => x.DebounceInterval, 0);
            autocompletecomp.SetParam(x => x.CoerceText, false);
            // check initial state
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Alabama");
            // set a value the search won't find
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            autocompletecomp.SetParam(a => a.Text, "Austria");
            // now trigger the coercion by closing the menu
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            autocomplete.Value.Should().Be("Alabama");
            autocomplete.Text.Should().Be("Austria");
        }

        [Test]
        public async Task Autocomplete_Should_TolerateNullFromSearchFunc()
        {
            var comp = Context.RenderComponent<MudAutocomplete<string>>((a) =>
            {
                a.Add(x => x.DebounceInterval, 0);
                a.Add(x => x.SearchFunc, new Func<string, CancellationToken, Task<IEnumerable<string>>>(async (s, token) => null)); // <--- searchfunc returns null instead of sequence
            });
            // enter a text so the search func will return null, and it shouldn't throw an exception
            comp.SetParam(a => a.Text, "Do not throw");
            comp.SetParam(x => x.SearchFunc, new Func<string, CancellationToken, Task<IEnumerable<string>>>((s, token) => null)); // <-- search func returns null instead of task!
            comp.SetParam(a => a.Text, "Don't throw here neither");
        }

        [Test]
        public async Task Autocomplete_ReadOnly_Should_Not_Open()
        {
            var comp = Context.RenderComponent<AutocompleteTest5>();
            comp.FindAll(".mud-input-adornment-icon-button")[0].Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
        }

        [Test]
        public async Task AutocompleteReadOnlyShouldNotHaveClearButton()
        {
            var comp = Context.RenderComponent<MudAutocomplete<string>>(p => p
            .Add(x => x.Text, "some value")
            .Add(x => x.Clearable, true)
            .Add(x => x.ReadOnly, false));

            comp.FindAll(".mud-input-clear-button").Count.Should().Be(1);

            comp.SetParametersAndRender(p => p.Add(x => x.ReadOnly, true)); //no clear button when readonly
            comp.FindAll(".mud-input-clear-button").Count.Should().Be(0);
        }

        /// <summary>
        /// MoreItemsTemplate should render when there are more items than the MaxItems limit
        /// </summary>
        [Test]
        public async Task AutocompleteTest6()
        {
            var comp = Context.RenderComponent<AutocompleteTest6>();

            comp.Find("input").Focus();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            var mudText = comp.FindAll("p.mud-typography");
            mudText[mudText.Count - 1].InnerHtml.Should().Contain("Not all items are shown"); //ensure the text is shown

            comp.FindAll("div.mud-popover .mud-autocomplete-more-items").Count.Should().Be(1);
        }

        /// <summary>
        /// NoItemsTemplate should render when there are no items
        /// </summary>
        [Test]
        public async Task AutocompleteTest7()
        {
            var comp = Context.RenderComponent<AutocompleteTest7>();

            comp.Find("input").Focus();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            var mudText = comp.FindAll("p.mud-typography");
            mudText[mudText.Count - 1].InnerHtml.Should().Contain("No items found, try another search"); //ensure the text is shown

            comp.FindAll("div.mud-popover .mud-autocomplete-no-items").Count.Should().Be(1);
        }

        /// <summary>
        /// After press Enter key down, the selected value should be shown in the input value
        /// </summary>
        [Test]
        public async Task Autocomplete_after_Enter_Should_show_Selected_Value()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
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
            autocomplete.Open.Should().BeFalse();
        }

        /// <summary>
        /// Based on this try https://try.mudblazor.com/snippet/GacPunvDUyjdUJAh
        /// and this issue https://github.com/MudBlazor/MudBlazor/issues/1235
        /// </summary>
        [Test]
        public async Task Autocomplete_Initialize_Value_on_SetParametersAsync()
        {
            var comp = Context.RenderComponent<AutocompleteSetParametersInitialization>();
            // select elements needed for the test
            await Task.Delay(100);
            var autocompletecomp = comp.FindComponent<MudAutocomplete<ExternalList>>();
            autocompletecomp.Find("input").GetAttribute("value").Should().Be("One");
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
            comp.WaitForAssertion(() => comp.FindAll(".mud-input-clear-button").Should().BeEmpty());

            // Button shows after entering text
            comp.Find("input").Input("text");
            comp.WaitForAssertion(() => comp.Find(".mud-input-clear-button").Should().NotBeNull());
            // Text cleared and button removed after clicking clear button
            comp.Find(".mud-input-clear-button").Click();
            comp.WaitForAssertion(() => comp.FindAll(".mud-input-clear-button").Should().BeEmpty());

            // Button shows again after entering text
            comp.Find("input").Input("text");
            comp.WaitForAssertion(() => comp.Find(".mud-input-clear-button").Should().NotBeNull());
            // Button removed after clearing text
            comp.Find("input").Input(string.Empty);
            comp.WaitForAssertion(() => comp.FindAll(".mud-input-clear-button").Should().BeEmpty());
        }

        [Test]
        public async Task Autocomplete_Should_Validate_Data_Attribute_Fail()
        {
            var comp = Context.RenderComponent<AutocompleteValidationDataAttrTest>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            await comp.InvokeAsync(() => autocomplete.DebounceInterval = 0);
            // Set invalid option
            await comp.InvokeAsync(() => autocomplete.SelectOptionAsync("Quux"));
            // check initial state
            autocomplete.Value.Should().Be("Quux");
            autocomplete.Text.Should().Be("Quux");
            // check validity
            await comp.InvokeAsync(autocomplete.Validate);
            autocomplete.ValidationErrors.Should().NotBeEmpty();
            autocomplete.ValidationErrors.Should().HaveCount(1);
            autocomplete.ValidationErrors[0].Should().Be("Should not be longer than 3");
        }

        [Test]
        public async Task Autocomplete_Should_Validate_Data_Attribute_Success()
        {
            var comp = Context.RenderComponent<AutocompleteValidationDataAttrTest>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            await comp.InvokeAsync(() => autocomplete.DebounceInterval = 0);
            // Set valid option
            await comp.InvokeAsync(() => autocomplete.SelectOptionAsync("Qux"));
            // check initial state
            autocomplete.Value.Should().Be("Qux");
            autocomplete.Text.Should().Be("Qux");
            // check validity
            await comp.InvokeAsync(autocomplete.Validate);
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

            await comp.InvokeAsync(autocomplete.Validate);

            autocomplete.ValidationErrors.First().Should().Be("Required");
        }

        /// <summary>
        /// Test for <seealso cref="https://github.com/MudBlazor/MudBlazor/issues/1761"/>
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Close_OnTab()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;

            // Should be closed
            comp.WaitForAssertion(() => autocomplete.Open.Should().BeFalse());

            // Lets type something to cause it to open
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => autocomplete.Open.Should().BeTrue());

            // Lets call blur on the input and confirm that it closed
            autocompletecomp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Tab" });
            comp.WaitForAssertion(() => autocomplete.Open.Should().BeFalse());

            // Tab closes the drop-down and does not select the selected value (California)
            // because SelectValueOnTab is false by default
            autocomplete.Value.Should().Be("Alabama");
        }

        [Test]
        public async Task Autocomplete_Should_SelectValue_On_Tab_With_SelectValueOnTab()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            // select elements needed for the test
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            autocompletecomp.SetParam(x => x.SelectValueOnTab, true);
            var autocomplete = autocompletecomp.Instance;

            // Should be closed
            comp.WaitForAssertion(() => autocomplete.Open.Should().BeFalse());

            // Lets type something to cause it to open
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => autocomplete.Open.Should().BeTrue());

            // Lets call blur on the input and confirm that it closed
            autocompletecomp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Tab" });
            comp.WaitForAssertion(() => autocomplete.Open.Should().BeFalse());

            // Tab closes the drop-down and selects the selected value (California)
            // because SelectValueOnTab is true
            autocomplete.Value.Should().Be("California");
        }

        /// <summary>
        /// <para>
        /// When selecting a value by clicking on it in the list the input will blur. However, this
        /// must not cause the dropdown to close or else the click on the item will not be possible!
        /// </para>
        /// <para>
        /// If this test fails it means the dropdown has closed before we can even click any value in the list.
        /// Such a regression happened and caused PR #1807 to be reverted
        /// </para>
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_NotCloseDropdownOnInputBlur()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
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
            var items = comp.FindComponents<MudListItem<string>>().ToArray();
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
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            await comp.InvokeAsync(() => autocomplete.ClearAsync().Wait());
            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.Value.Should().Be(null);
            autocomplete.Text.Should().Be("");

            // now let's type a different state
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            var items = comp.FindComponents<MudListItem<string>>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");

            // Clearing it and check the close status text and value again
            await comp.InvokeAsync(() => autocomplete.ClearAsync().Wait());
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            autocomplete.Value.Should().Be(null);
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

            // create the component
            var component = Context.RenderComponent<AutocompleteDisabledItemsTest>();

            // get the elements needed for the test
            var autocompleteComponent = component.FindComponent<MudAutocomplete<string>>();

            // Set the clear function on value changed
            autocompleteComponent.SetCallback(x => x.ValueChanged, async x => await autocompleteComponent.Instance.ClearAsync());

            // get the instance
            var autocompleteInstance = autocompleteComponent.Instance;

            // focus to open the popup
            autocompleteComponent.Find(TagNames.Input).Focus();

            // ensure popup is open
            component.WaitForAssertion(() => autocompleteInstance.Open.Should().BeTrue("Input has been focused and should open the popup"));

            // get the matching states
            var matchingStates = component.FindComponents<MudListItem<string>>().ToArray();

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
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            await comp.InvokeAsync(autocomplete.ResetAsync);
            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.Value.Should().Be(null);
            autocomplete.Text.Should().Be("");

            // now let's type a different state
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            var items = comp.FindComponents<MudListItem<string>>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");

            // Reseting it should close popover and set Text and Value to null again
            await comp.InvokeAsync(autocomplete.ResetAsync);
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            autocomplete.Value.Should().Be(null);
            autocomplete.Text.Should().Be("");
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

            // create the component
            var component = Context.RenderComponent<AutocompleteDisabledItemsTest>();

            // get the elements needed for the test
            var autocompleteComponent = component.FindComponent<MudAutocomplete<string>>();

            // get the instance
            var autocompleteInstance = autocompleteComponent.Instance;

            // focus to open the popup
            autocompleteComponent.Find(TagNames.Input).Focus();

            // ensure popup is open
            component.WaitForAssertion(() => autocompleteInstance.Open.Should().BeTrue("Input has been focused and should open the popup"));

            // get the matching states
            var matchingStates = component.FindComponents<MudListItem<string>>().ToArray();

            // try clicking 'American Samoa'
            matchingStates.Single(s => s.Markup.Contains(americanSamoaString)).Find(listItemQuerySelector).Click();
            component.WaitForAssertion(() => autocompleteInstance.Value.Should().BeNullOrEmpty($"{americanSamoaString} should not be clickable."));

            // try clicking 'Alaska'
            matchingStates.Single(s => s.Markup.Contains(alaskaString)).Find(listItemQuerySelector).Click();
            component.WaitForAssertion(() => autocompleteInstance.Value.Should().Be(alaskaString));

            // reset search-string
            autocompleteComponent.Find(TagNames.Input).Input(string.Empty);

            // wait till popup is visible
            component.WaitForAssertion(() => autocompleteInstance.Open.Should().BeTrue());

            // update found elements
            matchingStates = component.FindComponents<MudListItem<string>>().ToArray();

            // ensure alabama is selected
            component.WaitForAssertion(() => matchingStates.Single(s => s.Markup.Contains(alabamaString)).Find(listItemQuerySelector).ClassList.Should().Contain(selectedItemClassName, $"{alabamaString} should be selected/highlighted"));

            // define the event-args for arrow-down
            var arrowDownKeyboardEventArgs = new KeyboardEventArgs { Key = Key.Down.Value, Type = "keyup" };

            // invoke key down twice
            await autocompleteComponent.Find("input").KeyDownAsync(arrowDownKeyboardEventArgs);
            await autocompleteComponent.Find("input").KeyDownAsync(arrowDownKeyboardEventArgs);

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
                comp.Find(".toggle-value-button").Click();
                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Georgia"));
                autocomplete.Value.Should().Be("Georgia");
                autocomplete.Text.Should().Be("Georgia");

                //Change the value of the current bound value component
                //insert "Alabam"
                autocompletecomp.Find("input").Input("Alabam");
                await Task.Delay(100);

                //press Enter key
                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Enter" }));
                //ensure autocomplete is closed and new value is committed/bound
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeFalse());

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Enter" }));
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeTrue());

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Escape" }));
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeFalse());

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp" }));
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeTrue());

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", AltKey = true }));
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeFalse());

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown" }));
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeTrue());

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Escape" }));
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeFalse());

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "NumpadEnter" }));
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeTrue());

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown" }));
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeTrue());

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown" }));

                //The value of the input should be Alabama
                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Alabama"));
                autocomplete.Value.Should().Be("Alabama");
                autocomplete.Text.Should().Be("Alabama");

                //Again Change the bound object
                comp.Find(".toggle-value-button").Click();

                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Florida"));
                autocomplete.Value.Should().Be("Florida");
                autocomplete.Text.Should().Be("Florida");

                //Change the bound object back and check again.
                comp.Find(".toggle-value-button").Click();
                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Alabama"));
                autocomplete.Value.Should().Be("Alabama");
                autocomplete.Text.Should().Be("Alabama");

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp" }));
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeTrue());

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp" }));
                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Alabama"));

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp" }));
                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "Tab" }));
                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Tab" }));
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeFalse());

                autocompletecomp.SetParam("SelectValueOnTab", true);
                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp" }));
                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "Tab" }));
                comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Alabama"));

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Backspace", CtrlKey = true, ShiftKey = true }));
                comp.WaitForAssertion(() => autocompletecomp.Instance.Value.Should().Be(null));

                await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "Tab" }));
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeFalse());
                //Check popover is closed if coerce text is true (it fixed with a PR)
                autocomplete.CoerceText = true;
                await comp.InvokeAsync(() => autocompletecomp.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Enter" }));
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeTrue());
                await comp.InvokeAsync(() => autocomplete.OnEnterKeyAsync());
                autocompletecomp.Find("input").Input("abc");
                await comp.InvokeAsync(async () => await autocomplete.SelectAsync());
                await comp.InvokeAsync(async () => await autocomplete.SelectRangeAsync(0, 1));
                autocompletecomp.Find("input").Input("");
                await comp.InvokeAsync(() => autocomplete.ToggleMenuAsync());
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeTrue());

                await comp.InvokeAsync(() => autocomplete.OnEnterKeyAsync());
                comp.WaitForAssertion(() => autocomplete.Open.Should().BeFalse());
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

            autocomplete.Find($".{AutocompleteSyncTest.AutocompleteClass} input").Focus();

            popoverProvider.WaitForAssertion(() =>
            {
                popover.Instance.Open.Should().BeTrue("Should be open once clicked");

                popoverProvider
                    .FindComponents<MudListItem<string>>().Count
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
            var icon = Parameter(nameof(AutocompleteAdornmentChange.Icon), Icons.Material.Filled.Abc);
            var comp = Context.RenderComponent<AutocompleteAdornmentChange>(icon);
            var instance = comp.Instance;

            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;

            var markupBefore = comp.Find("svg.mud-icon-root").Children.ToMarkup().Trim();

            // change icon and render again
            instance.Icon = Icons.Material.Filled.Remove;

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
            // TODO: use a TaskCompletionSource that allows control over the search task
            // for reliable testing.  Applies to other tests like this one.
            // Currently we increase the load time to 50mms to catch the progress UI

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
            autocompletecomp.SetParam(x => x.AdornmentIcon, Icons.Material.Filled.Info);
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

            autocompletecomp.SetParam(p => p.SearchFunc, new Func<string, CancellationToken, Task<IEnumerable<string>>>((s, cancellationToken) =>
            {
                cancelToken = cancellationToken;
                // Return task that never completes.
                return first.Task;
            }));

            comp.Find("input").Input("Foo");

            await Task.Delay(20);

            // Test

            comp.WaitForAssertion(() => cancelToken?.IsCancellationRequested.Should().BeFalse());

            // Arrange second call

            var second = new TaskCompletionSource<IEnumerable<string>>();

            autocompletecomp.SetParam(p => p.SearchFunc, new Func<string, CancellationToken, Task<IEnumerable<string>>>((s, cancellationToken) =>
            {
                return second.Task;
            }));

            comp.Find("input").Input("Bar");

            await Task.Delay(20);

            // Test

            comp.WaitForAssertion(() => cancelToken?.IsCancellationRequested.Should().BeTrue());

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

        [Test]
        public async Task Autocomplete_Should_HaveValueWithTextChangedEvent()
        {
            // Arrange
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();

            const string testText = "testText";
            string eventText = null;
            autocompletecomp.Instance.TextChanged = new EventCallbackFactory().Create<string>(this, v =>
            {
                eventText = v;
            });

            // Act
            // enter a text so the TextChanged event will fire
            autocompletecomp.SetParam(a => a.Text, testText);

            // Assert
            autocompletecomp.WaitForAssertion(() => eventText.Should().Be(testText));
        }

        [Test]
        [TestCase(0)] //test toStringFunc
        [TestCase(1)] //test toString
        public async Task AutocompleteStrictFalseTest(int index)
        {
            var listItemQuerySelector = "div.mud-list-item";
            var selectedItemClassName = "mud-selected-item";
            var californiaString = "California";
            var virginiaString = "Virginia";

            var comp = Context.RenderComponent<AutocompleteStrictFalseTest>();
            var autocompletecomp = comp.FindComponents<MudAutocomplete<AutocompleteStrictFalseTest.State>>()[index];
            var autocomplete = autocompletecomp.Instance;

            //search for and select California
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover")[index].ClassList.Should().Contain("mud-popover-open"));
            await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Enter" }));
            autocomplete.Text.Should().Be(californiaString);
            autocomplete.Value.StateName.Should().Be(californiaString);

            //California should appear as index 5 and be selected
            await comp.InvokeAsync(autocompletecomp.Instance.OpenMenuAsync); // reopen menu because Enter closes it.
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover")[index].ClassList.Should().Contain("mud-popover-open"));
            var items = comp.FindComponents<MudListItem<AutocompleteStrictFalseTest.State>>().ToArray();
            items.Length.Should().Be(10);
            var item = items.SingleOrDefault(x => x.Markup.Contains(californiaString));
            items.ToList().IndexOf(item).Should().Be(5);
            comp.WaitForAssertion(() => items.Single(s => s.Markup.Contains(californiaString)).Find(listItemQuerySelector).ClassList.Should().Contain(selectedItemClassName));

            await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Escape" })); // Close autocomplete.

            //search for and select Virginia
            autocompletecomp.Find("input").Input("Virginia");
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover")[index].ClassList.Should().Contain("mud-popover-open"));
            await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Enter" }));
            autocomplete.Text.Should().Be(virginiaString);
            autocomplete.Value.StateName.Should().Be(virginiaString);

            //West Virginia is not in the first 10 states, so it should not appear in the list
            await comp.InvokeAsync(autocompletecomp.Instance.OpenMenuAsync); // reopen menu because Enter closes it.
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover")[index].ClassList.Should().Contain("mud-popover-open"));
            var items2 = comp.FindComponents<MudListItem<AutocompleteStrictFalseTest.State>>().ToArray();
            items2.Length.Should().Be(10);
            var item2 = items2.SingleOrDefault(x => x.Markup.Contains(virginiaString));
            items2.ToList().IndexOf(item).Should().Be(-1);
            items2.Count(s => s.Find(listItemQuerySelector).ClassList.Contains(selectedItemClassName)).Should().Be(0);
        }

        [Test]
        public async Task Autocomplete_Should_Not_Throw_When_SearchFunc_Is_Null()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();

            autocompletecomp.SetParam(p => p.SearchFunc, null);

            comp.Find("input").Input("Foo");

            await Task.Delay(20);

            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ToMarkup().Should().NotContain("Foo"));
        }

        [Test]
        public async Task Autocomplete_Should_Raise_KeyDown_KeyUp_Event()
        {
            //Create comp
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var result = new List<string>();
            //create eventCallback
            var customEvent = new EventCallbackFactory().Create<KeyboardEventArgs>("A", () => result.Add("keyevent thrown"));

            //set eventCallback
            //SetCallback also possible
            //autocompletecomp.SetCallback(p => p.OnKeyDown, (KeyboardEventArgs e ) => result.Add("keyevent thrown"));
            autocompletecomp.SetParam(p => p.OnKeyDown, customEvent);
            autocompletecomp.SetParam(p => p.OnKeyUp, customEvent);

            result.Should().BeEmpty();
            //Act
            autocompletecomp.Find("input").KeyDown("a");
            autocompletecomp.Find("input").KeyUp("a");
            //Assert
            result.Count.Should().Be(2);
        }

        /// <summary>
        /// Test case for <seealso cref="https://github.com/MudBlazor/MudBlazor/issues/6412"/>
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Highlight_Selected_Item_After_Disabled()
        {
            var disabledItemSelector = "mud-list-item-disabled";
            var selectedItemSelector = "mud-selected-item";
            var popoverSelector = "div.mud-popover";

            var selectedItemString = "peach";
            var disabledItemString = "carrot";

            var comp = Context.RenderComponent<AutocompleteStrictFalseSelectedHighlight>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;

            // Select the peach list item
            autocompletecomp.Find("input").Input(selectedItemString);
            comp.WaitForAssertion(() => comp.Find(popoverSelector).ClassList.Should().Contain("mud-popover-open"));
            await comp.InvokeAsync(async () => await autocompletecomp.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Enter" }));
            autocomplete.Text.Should().Be(selectedItemString);
            autocomplete.Value.Should().Be(selectedItemString);

            // Opening the list of autocomplete
            await comp.InvokeAsync(autocompletecomp.Instance.OpenMenuAsync);
            comp.WaitForAssertion(() => comp.Find(popoverSelector).ClassList.Should().Contain("mud-popover-open"));
            var listItems = comp.FindComponents<MudListItem<string>>().ToArray();

            // Ensure that the carrot list item is disabled
            var disabledItem = listItems.Single(x => x.Markup.Contains(disabledItemSelector));
            disabledItem.Markup.Should().Contain(disabledItemString);

            // Assert if the peach is highlighted
            var selectedItem = listItems.Single(x => x.Markup.Contains(selectedItemSelector));
            selectedItem.Markup.Should().Contain(selectedItemString);
        }

        /// <summary>
        /// https://github.com/MudBlazor/MudBlazor/issues/6475
        /// </summary>
        [Test]
        public async Task Autocomplete_Reset_Value_ShouldBe_Empty()
        {
            var component = Context.RenderComponent<AutocompleteResetTest>();
            var autocompleteComponent = component.FindComponent<MudAutocomplete<string>>();

            // get the instance
            var autocompleteInstance = autocompleteComponent.Instance;

            // focus to open the popup
            autocompleteComponent.Find(TagNames.Input).Focus();

            // ensure popup is open
            component.WaitForAssertion(() => autocompleteInstance.Open.Should().BeTrue("Input has been focused and should open the popup"));

            // get the matching states
            var matchingStates = component.FindComponents<MudListItem<string>>().ToArray();

            // try clicking 'Test'
            matchingStates.Single(s => s.Markup.Contains("Test")).Find("div.mud-list-item").Click();
            component.WaitForAssertion(() => autocompleteInstance.Text.Should().Be(string.Empty));
        }

        /// <summary>
        /// BeforeItemsTemplate should render when there are items
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_LoadListStartWhenSetAndThereAreItems()
        {
            var comp = Context.RenderComponent<AutocompleteListBeforeAndAfterRendersWithItemsTest>();

            comp.Find("div.mud-input-control input").Focus();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            var mudText = comp.FindAll("p.mud-typography");
            mudText[0].InnerHtml.Should().Contain("StartList_Content"); //ensure the text is shown

            comp.FindAll("div.mud-popover .mud-autocomplete-before-items").Count.Should().Be(1);
        }

        /// <summary>
        /// AfterItemsTemplate should render when there are items
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_LoadListEndWhenSetAndThereAreItems()
        {
            var comp = Context.RenderComponent<AutocompleteListBeforeAndAfterRendersWithItemsTest>();

            comp.Find("div.mud-input-control input").Focus();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            var mudText = comp.FindAll("p.mud-typography");
            mudText[mudText.Count - 1].InnerHtml.Should().Contain("EndList_Content"); //ensure the text is shown

            comp.FindAll("div.mud-popover .mud-autocomplete-after-items").Count.Should().Be(1);
        }

        /// <summary>
        /// BeforeItemsTemplate should not render when there are no items
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Not_LoadListStartWhenSet()
        {
            var comp = Context.RenderComponent<AutocompleteListStartRendersTest>();

            comp.Find("div.mud-input-control input").Focus();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            comp.Find("div.mud-popover").InnerHtml.Should().BeEmpty();
        }

        /// <summary>
        /// AfterItemsTemplate should not render when there are no items
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Not_LoadListEndWhenSet()
        {
            var comp = Context.RenderComponent<AutocompleteListEndRendersTest>();

            comp.Find("div.mud-input-control input").Focus();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            comp.Find("div.mud-popover").InnerHtml.Should().BeEmpty();
        }

        [Test]
        public async Task Autocomplete_Should_ApplyListItemClass()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var listItemClassTest = "list-item-class-test";

            autocompletecomp.SetParam(a => a.ListItemClass, listItemClassTest);
            comp.Find("div.mud-input-control input").Focus();

            comp.WaitForAssertion(() => comp.Find("div.mud-list-item").ClassList.Should().Contain(listItemClassTest));
        }

        [Test]
        public async Task Autocomplete_Should_OpenMenuOnFocus()
        {
            var comp = Context.RenderComponent<AutocompleteTest1>();

            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));

            comp.Find("input.mud-input-root").Focus();

            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
        }

        [Test]
        public async Task Autocomplete_ReturnedItemsCount_Should_Be_Accurate()
        {
            Task<IEnumerable<string>> search(string value, CancellationToken token)
            {
                var values = new string[] { "Lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit" };
                return Task.FromResult(values.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
            }

            var comp = Context.RenderComponent<MudAutocomplete<string>>();
            comp.SetParametersAndRender(p => p
                .Add(x => x.Value, "nothing will ever match this")
                .Add(x => x.SearchFunc, search)
                .Add(x => x.DebounceInterval, 0));

            int? count = null;
            comp.Instance.ReturnedItemsCountChanged = new EventCallbackFactory().Create<int>(this, v => count = v);

            comp.Find("input").Input("Lorem");
            comp.WaitForAssertion(() => count.Should().Be(1));
            ;
            comp.Find("input").Input("ip");
            comp.WaitForAssertion(() => count.Should().Be(2));
            ;
            comp.Find("input").Input("wtf");
            comp.WaitForAssertion(() => count.Should().Be(0));
        }

        /// <summary>
        /// An autocomplete component with a label should auto-generate an id for the input element and use that id on the label's for attribute.
        /// </summary>
        [Test]
        public void AutocompleteWithLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.RenderComponent<MudAutocomplete<string>>(parameters
                => parameters.Add(p => p.Label, "Test Label"));

            comp.Find("input").Id.Should().NotBeNullOrEmpty();
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(comp.Find("input").Id);
        }

        /// <summary>
        /// An autocomplete component with a label and a UserAttributesId should use the UserAttributesId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void AutocompleteWithLabelAndUserAttributesId_Should_UseUserAttributesIdForInputAndAccompanyingLabel()
        {
            var expectedId = "userattribute-id";
            var comp = Context.RenderComponent<MudAutocomplete<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label").Add(p => p.UserAttributes, new Dictionary<string, object>
                    {
                        { "Id", expectedId }
                    }));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// An autocomplete component with a label, a UserAttributesId, and an InputId should use the InputId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void AutocompleteWithLabelAndUserAttributesIdAndInputId_Should_UseInputIdForInputAndAccompanyingLabel()
        {
            var expectedId = "input-id";
            var comp = Context.RenderComponent<MudAutocomplete<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes, new Dictionary<string, object>
                    {
                        { "Id", "userattribute-id" }
                    })
                    .Add(p => p.InputId, expectedId));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// Optional Autocomplete should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalAutocomplete_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.RenderComponent<MudAutocomplete<string>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required Autocomplete should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredAutocomplete_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.RenderComponent<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required Autocomplete attributes should be dynamic.
        /// </summary>
        [Test]
        public void RequiredAndAriaRequiredAutocompleteAttributes_Should_BeDynamic()
        {
            var comp = Context.RenderComponent<MudAutocomplete<string>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Ensure that the ItemDisabledTemplate and ItemSelectedTemplate both can display when ItemTemplate isn't provided (null)
        /// </summary>
        [Test]
        public void AutocompleteItemTemplateDisplayTest()
        {
            var comp = Context.RenderComponent<AutocompleteItemTemplateDisplayTest>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;

            // Search for a to get Alabama, Alaska, American Samoa,...
            autocompletecomp.Find("input").Input("a");

            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            // Any state with 'l' is disabled: ItemDisabledFunc="@((string state) => (state.Contains('l')))"
            var items = comp.FindComponents<MudListItem<string>>().ToArray();
            // Alabama should have the ItemDisabledTemplate applied "Alabama Disabled State"
            items.First().Markup.Should().Contain("Alabama Disabled State");
            // American Samoa should have the ItemSelectedTemplate applied "American Samoa Selected State"
            items[2].Markup.Should().Contain("American Samoa Selected State");
        }

        [Test]
        public void Should_render_conversion_error_message()
        {
            var comp = Context.RenderComponent<MudAutocomplete<int>>(parameters => parameters
                .Add(p => p.ErrorId, "error-id")
                .Add(p => p.Text, "not a number")
                .Add(p => p.Converter, new DummyErrorConverter()));

            comp.Instance.ConversionErrorMessage.Should().NotBeNullOrEmpty();
            comp.Find("#error-id").InnerHtml.Should().Be(comp.Instance.ConversionErrorMessage);
        }

        [TestCase(Adornment.Start)]
        [TestCase(Adornment.End)]
        public void Should_render_aria_label_for_adornment_if_provided(Adornment adornment)
        {
            var ariaLabel = "the aria label";
            var comp = Context.RenderComponent<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.Adornment, adornment)
                .Add(p => p.AdornmentIcon, Icons.Material.Filled.Accessibility)
                .Add(p => p.AdornmentAriaLabel, ariaLabel));

            comp.Find(".mud-input-adornment-icon-button").Attributes.GetNamedItem("aria-label")!.Value.Should().Be(ariaLabel);
        }

#nullable enable
        /// <summary>
        /// Verifies that an autocomplete field with various configurations renders the expected <c>aria-describedby</c> attribute.
        /// </summary>
        // no helpers, validates error id is present when error is present
        [TestCase(false, false)]
        // with helper text, helper element should only be present when there is no error
        [TestCase(false, true)]
        // with user helper id, helper id should always be present
        [TestCase(true, false)]
        // with user helper id and helper text, should always favour user helper id
        [TestCase(true, true)]
        public void Should_pass_various_aria_describedby_tests(
            bool withUserHelperId,
            bool withHelperText)
        {
            var inputId = "input-id";
            var helperId = withUserHelperId ? "user-helper-id" : null;
            var helperText = withHelperText ? "helper text" : null;
            var errorId = "error-id";
            var errorText = "error text";
            var inputSelector = "input";
            var firstExpectedAriaDescribedBy = withUserHelperId
                ? helperId
                : withHelperText
                    ? $"{inputId}-helper-text"
                    : null;

            var comp = Context.RenderComponent<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.InputId, inputId)
                .Add(p => p.HelperId, helperId)
                .Add(p => p.HelperText, helperText)
                .Add(p => p.Error, false)
                .Add(p => p.ErrorId, errorId)
                .Add(p => p.ErrorText, errorText));

            // verify helper text is rendered
            if (withUserHelperId is false && withHelperText)
            {
                var action = () => comp.Find($"#{inputId}-helper-text");
                action.Should().NotThrow();
            }

            if (firstExpectedAriaDescribedBy is null)
            {
                comp.Find(inputSelector).HasAttribute("aria-describedby").Should().BeFalse();
            }
            else
            {
                comp.Find(inputSelector).GetAttribute("aria-describedby").Should().Be(firstExpectedAriaDescribedBy);
            }

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.Error, true));
            var secondExpectedAriaDescribedBy = withUserHelperId ? $"{errorId} {helperId}" : errorId;

            // verify error text is rendered
            var errorAction = () => comp.Find($"#{errorId}");
            errorAction.Should().NotThrow();

            comp.Find(inputSelector).GetAttribute("aria-describedby").Should().Be(secondExpectedAriaDescribedBy);
        }
#nullable disable
    }
}
