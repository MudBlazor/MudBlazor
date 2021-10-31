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
        /// and this issue https://github.com/MudBlazor/MudBlazor/issues/1235
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
        /// Test for <seealso cref="https://github.com/MudBlazor/MudBlazor/issues/1761"/>
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
        /// When calling Clear(), menu should closed, Value and Text should be cleared.
        /// </summary>
        [Test]
        public async Task Autocomplete_CheckTextValueandOpenState_OnClear()
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

            // ToggleMenu to open menu and Clear to close it and check the text and value
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            await comp.InvokeAsync(() => autocomplete.Clear().Wait());
            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.Value.Should().Be("");
            autocomplete.Text.Should().Be("");

            // now let's type a different state
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            Console.WriteLine(comp.Markup);
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
        /// When calling Reset(), menu should closed, Value and Text should be null.
        /// </summary>
        [Test]
        public async Task Autocomplete_CheckTextAndValue_OnReset()
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

            // Reset it
            await comp.InvokeAsync(() => autocomplete.ToggleMenu());
            await comp.InvokeAsync(() => autocomplete.Reset());
            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.Value.Should().Be(null);
            autocomplete.Text.Should().Be(null);

            // now let's type a different state
            autocompletecomp.Find("input").Input("Calif");
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            Console.WriteLine(comp.Markup);
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
            var comp = Context.RenderComponent<AutocompleteChangeBoundObjectTest>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;
            autocompletecomp.SetParametersAndRender(parameters => parameters.Add(p=> p.DebounceInterval, 0));
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
            autocompletecomp.Find("input").KeyUp(key: Key.Enter);
            //ensure autocomplete is closed and new value is committed/bound
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

            autocompletecomp.Find("input").KeyUp(key: Key.Enter);
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

            // this greatly increases test reliability because obviously earlier key presses might influence later assertions due to async code still running
            await Task.Delay(100);
            autocompletecomp.Find("input").KeyUp(key: Key.Escape);
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

            autocompletecomp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowUp" });
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

            // this greatly increases test reliability because obviously earlier key presses might influence later assertions due to async code still running
            await Task.Delay(100);
            autocompletecomp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowUp", AltKey = true });
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

            autocompletecomp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowDown" });
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

            // this greatly increases test reliability because obviously earlier key presses might influence later assertions due to async code still running
            await Task.Delay(100);
            autocompletecomp.Find("input").KeyUp(key: Key.Escape);
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

            autocompletecomp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "NumpadEnter" });
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

            autocompletecomp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowDown" });
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

            autocompletecomp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowDown" });

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

            autocompletecomp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowUp" });
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeTrue());

            autocompletecomp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowUp" });
            comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Alabama"));

            autocompletecomp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowUp" });
            autocompletecomp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Tab" });
            autocompletecomp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "Tab" });
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());

            autocompletecomp.SetParam("SelectValueOnTab", true);
            autocompletecomp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowUp" });
            autocompletecomp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Tab" });
            comp.WaitForAssertion(() => autocompletecomp.Find("input").GetAttribute("value").Should().Be("Alabama"));

            autocompletecomp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Tab" });
            comp.WaitForAssertion(() => autocomplete.IsOpen.Should().BeFalse());
        }
    }
}
