// Copyright (c) mudblazor 2021
// License MIT

using System.Reflection;
using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.UnitTests.Dummy;
using MudBlazor.UnitTests.TestComponents.Autocomplete;
using NUnit.Framework;
using static MudBlazor.UnitTests.TestComponents.Autocomplete.AutocompleteSetParametersInitialization;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class AutocompleteTests : BunitTest
    {
        [Test]
        public async Task Autocomplete_Should_Handle_Converter_WithStrict()
        {
            var comp = Context.Render<AutocompleteConverterStrictTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<AutocompleteConverterStrictTest.ConverterElement>>();
            comp.Markup.Should().NotContain("mud-popover-open");

            // https://github.com/bUnit-dev/bUnit/discussions/474
            // https://github.com/bUnit-dev/bUnit/issues/517
            // https://github.com/bUnit-dev/bUnit/issues/634
            Func<Task> ButtonClicker = async () =>
            {
                var button = await autocompleteComponent.WaitForElementAsync(".mud-button-root.mud-no-activator");
                await autocompleteComponent.InvokeAsync(() => button.Click());
            };

            await ButtonClicker(); // open popover
            var pop = await comp.WaitForElementAsync("div.mud-popover"); // doesn't return until popover exists
            await comp.WaitForAssertionAsync(() => pop.ClassList.Should().Contain("mud-popover-open")); // wait for popover to open
            var items = comp.FindComponents<MudListItem<AutocompleteConverterStrictTest.ConverterElement>>();
            items.Count.Should().Be(10, "The popover should contain 10 items."); // default maxitems is 10
            await ButtonClicker(); // close popover
            await comp.WaitForAssertionAsync(() => pop.ClassList.Should().NotContain("mud-popover-open"));

            // set search
            await autocompleteComponent.Find("input").InputAsync("he");
            await comp.WaitForAssertionAsync(() => pop.ClassList.Should().Contain("mud-popover-open"));
            var filteredItems = comp.FindComponents<MudListItem<AutocompleteConverterStrictTest.ConverterElement>>();
            filteredItems.Count.Should().Be(4, "The popover should contain 4 items.");
        }

        /// <summary>
        /// Initial value should be shown and popup should not open.
        /// </summary>
        [Test]
        public async Task AutocompleteTest1()
        {
            var comp = Context.Render<AutocompleteTest1>();
            // select elements needed for the test
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            //No popover-open, due it's closed
            comp.Markup.Should().NotContain("mud-popover-open");

            // check initial state
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");

            // now let's type a different state to see the popup open
            await autocompleteComponent.Find("input").InputAsync("Calif");

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            var items = comp.FindComponents<MudListItem<string>>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");
            // click on California!
            await comp.Find("div.mud-list-item").ClickAsync();
            // check popover class
            comp.Find("div.mud-popover").ClassList.Should().Contain("autocomplete-popover-class");
            // check state
            await comp.WaitForAssertionAsync(() => autocomplete.ReadValue.Should().Be("California"));
            autocomplete.ReadText.Should().Be("California");
        }

        /// <summary>
        /// Popup should open when 3 characters are typed and close when below.
        /// </summary>
        [Test]
        public async Task AutocompleteTest2()
        {
            var comp = Context.Render<AutocompleteTest2>();
            // select elements needed for the test
            var select = comp.FindComponent<MudAutocomplete<string>>();

            // check initial state
            comp.Markup.Should().NotContain("mud-popover-open");

            // focus and check if it has toggled the menu
            await select.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.Markup.Should().NotContain("mud-popover-open"));

            // type 3 characters and check if it has toggled the menu
            await select.Find("input").InputAsync("ala");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            // type 2 characters and check if it has toggled the menu
            await select.Find("input").InputAsync("al");
            await comp.WaitForAssertionAsync(() => comp.Markup.Should().NotContain("mud-popover-open"));
        }

        /// <summary>
        /// Autocomplete should show 'Assam' (using ToStringFunc)
        /// </summary>
        [Test]
        public void AutocompleteTest3()
        {
            var comp = Context.Render<AutocompleteTest3>();
            var autocomplete = comp.FindComponent<MudAutocomplete<AutocompleteTest3.State>>().Instance;
            autocomplete.ReadText.Should().Be("Assam");
        }

        /// <summary>
        /// The autocomplete should stop loading data when it is disposed
        /// </summary>
        [Test]
        public async Task AutocompleteCancelDisposeTest()
        {
            var comp = Context.Render<AutocompleteTest8>();
            var autocompleteContainerComp = comp.FindComponent<AutoCompleteContainer>();
            var autocompleteComp = autocompleteContainerComp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComp.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, "Alabama"));
            await Task.Delay(500);
            comp.Instance.MustBeShown = false;
            await Task.Delay(500);
            comp.Render();
            await Task.Delay(500);
            comp.Instance.HasBeenDisposed.Should().Be(true);
        }

        /// <summary>
        /// Autocomplete id should propagate to label for attribute
        /// </summary>
        [Test]
        public void AutocompleteLabelFor()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var label = comp.FindAll(".mud-input-label");
            label[0].Attributes.GetNamedItem("for")?.Value.Should().Be("autocompleteLabelTest");
        }

        /// <summary>
        /// Autocomplete should show 'Assam' (using state.ToString())
        /// </summary>
        [Test]
        public void AutocompleteTest4()
        {
            var comp = Context.Render<AutocompleteTest4>();
            var autocomplete = comp.FindComponent<MudAutocomplete<AutocompleteTest4.State>>().Instance;
            autocomplete.ReadText.Should().Be("Assam");
        }

        /// <summary>
        /// We search for a value not in list and coercion will go back to the last valid value,
        /// discarding the current search text.
        /// </summary>
        [Test]
        public async Task AutocompleteCoercionTest()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.DebounceInterval, 0));
            // check initial state
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");
            // set a value the search won't find
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, "Austria"));
            // Open must be true to properly simulate a user clicking outside of the component, which is what the next ToggleMenu call below does.
            await autocompleteComponent.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());
            // now trigger the coercion by closing the menu
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");
        }

        /// <summary>
        /// We search for a value not in list and value coercion will force the invalid value to be applied
        /// allowing to validate the user input.
        /// </summary>
        [Test]
        public async Task AutocompleteCoerceValueTest()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.CoerceValue, true));
            // check initial state
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");
            // set a value the search won't find
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, "Austria"));

            // now trigger the coercion by toggling the the menu (it won't even open for invalid values, but it will coerce)
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            await comp.WaitForAssertionAsync(() => autocomplete.ReadValue.Should().Be("Austria"));
            autocomplete.ReadText.Should().Be("Austria");
        }

        /// <summary>
        /// Test to cover issue #5993.
        /// </summary>
        [Test]
        public async Task AutocompleteImmediateCoerceValueTest()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.CoerceValue, true)
                .Add(x => x.CoerceText, false)
                .Add(x => x.Immediate, true));
            // check initial state
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");
            // set a value the search won't find
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, "Austria"));

            await comp.WaitForAssertionAsync(() => autocomplete.ReadValue.Should().Be("Austria"));
            autocomplete.ReadText.Should().Be("Austria");
        }

        [Test]
        public async Task OnTextChanged_WithCoerceValueAndNotCoerceTextAndImmediateNotDebounce_SetValueAndOpenMenuImmediately()
        {
            // Arrange

            var valueChangedCount = 0;
            var comp = Context.Render<AutocompleteStates>(parameters =>
            {
                parameters.Add(p => p.DebounceInterval, 0);
                parameters.Add(p => p.CoerceText, false);
                parameters.Add(p => p.CoerceValue, true);
                parameters.Add(p => p.Immediate, true);
                parameters.Add(p => p.ValueChanged, v => valueChangedCount++);
            });
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;

            // Assert : initial state, menu closed and text/value null

            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.Open.Should().BeFalse();
            autocomplete.ReadValue.Should().BeNull();
            autocomplete.ReadText.Should().BeNull();
            comp.Instance.SearchFuncCallCount.Should().Be(0);
            valueChangedCount.Should().Be(0);

            // Act

            await comp.Find("input").InputAsync("Al");

            // Assert : debounce disable, so menu is opened immediately

            autocomplete.Open.Should().BeTrue();
            comp.Markup.Should().Contain("mud-popover-open");

            // Assert : CoercedValue and immediate enabled, so value is set immediately on text input

            autocompletecomp.Instance.ReadText.Should().Be("Al");
            autocompletecomp.Instance.ReadValue.Should().Be("Al");
            valueChangedCount.Should().Be(1);
        }

        [Test]
        public async Task OnTextChanged_CoerceValueAndNotCoerceTextAndImmediateAndDebounce_SetValueImmediatelyButDelaysMenuOpening()
        {
            // Arrange

            var valueChangedCount = 0;
            var comp = Context.Render<AutocompleteStates>(parameters =>
            {
                parameters.Add(p => p.DebounceInterval, 500);
                parameters.Add(p => p.CoerceText, false);
                parameters.Add(p => p.CoerceValue, true);
                parameters.Add(p => p.Immediate, true);
                parameters.Add(p => p.ValueChanged, v => valueChangedCount++);
            });
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompletecomp.Instance;

            // Assert : initial state, menu closed and text/value null

            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.Open.Should().BeFalse();
            autocomplete.ReadValue.Should().BeNull();
            autocomplete.ReadText.Should().BeNull();
            comp.Instance.SearchFuncCallCount.Should().Be(0);
            valueChangedCount.Should().Be(0);

            // Act

            await comp.Find("input").InputAsync("Al");

            // Assert : debounce enable, so menu is not opened immediately

            autocomplete.Open.Should().BeFalse();
            comp.Markup.Should().NotContain("mud-popover-open");

            // Assert : CoercedValue and immediate enabled, so value is set immediately on text input

            autocompletecomp.Instance.ReadText.Should().Be("Al");
            autocompletecomp.Instance.ReadValue.Should().Be("Al");
            valueChangedCount.Should().Be(1);

            // Act : Wait the debounce timer that open the menu

            await autocompletecomp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());
            await autocompletecomp.WaitForAssertionAsync(() => comp.Markup.Should().Contain("mud-popover-open"));

            // Assert : value and text unchanged

            autocompletecomp.Instance.ReadText.Should().Be("Al");
            autocompletecomp.Instance.ReadValue.Should().Be("Al");
            valueChangedCount.Should().Be(1);
        }

        [Test]
        public async Task CoerceValueAndNotCoerceTextAndNotImmediate_ValueSetOnBlurAsync()
        {
            // Arrange

            var comp = Context.Render<MudAutocomplete<string>>(parameters =>
            {
                parameters.Add(a => a.CoerceValue, true);
                parameters.Add(a => a.CoerceText, false);
                parameters.Add(a => a.Immediate, false);
                parameters.Add(a => a.DebounceInterval, 0);
            });
            var ccc = comp.FindComponent<MudInput<string>>();

            // Assert : Initial

            comp.Instance.ReadText.Should().BeNull();
            comp.Instance.ReadValue.Should().BeNull();

            // Act

            await comp.Find("input").InputAsync("ABC");

            // Assert : Immediate false, so value is not set on text changed

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().BeNull();

            // Act

            await comp.Find("input").BlurAsync();

            // Assert : CoercedValue enabled, so value is set on focus lost

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().Be("ABC");
        }

        [Test]
        public async Task NotCoerceValueAndNotCoerceTextAndNotImmediate_ValueSetOnBlurAsync()
        {
            // Arrange

            var comp = Context.Render<MudAutocomplete<string>>(parameters =>
            {
                parameters.Add(a => a.CoerceValue, false);
                parameters.Add(a => a.CoerceText, false);
                parameters.Add(a => a.Immediate, false);
                parameters.Add(a => a.DebounceInterval, 0);
            });
            var ccc = comp.FindComponent<MudInput<string>>();

            // Assert : Initial

            comp.Instance.ReadText.Should().BeNull();
            comp.Instance.ReadValue.Should().BeNull();

            // Act

            await comp.Find("input").InputAsync("ABC");

            // Assert : Immediate false, so value is not set on text changed

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().BeNull();

            // Act

            await comp.Find("input").BlurAsync();

            // Assert : CoercedValue disabled, so value is not set on focus lost

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().BeNull();
        }

        [Test]
        public async Task CoerceValueAndNotCoerceTextAndNotImmediate_ValueSetOnEnterAsync()
        {
            // Arrange

            var comp = Context.Render<MudAutocomplete<string>>(parameters =>
            {
                parameters.Add(a => a.CoerceValue, true);
                parameters.Add(a => a.CoerceText, false);
                parameters.Add(a => a.Immediate, false);
                parameters.Add(a => a.DebounceInterval, 0);
            });

            // Assert : Initial

            comp.Instance.ReadText.Should().BeNull();
            comp.Instance.ReadValue.Should().BeNull();

            // Act

            await comp.Find("input").InputAsync("ABC");

            // Assert : Immediate false, so value is not set

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().BeNull();

            // Act

            comp.Find("input").KeyUp("Enter");

            // Assert : CoercedValue enabled, so value is set on key enter pressed

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().Be("ABC");
        }

        [Test]
        public async Task NotCoerceValueAndNotCoerceTextAndNotImmediate_ValueNotSetOnEnterAsync()
        {
            // Arrange

            var comp = Context.Render<MudAutocomplete<string>>(parameters =>
            {
                parameters.Add(a => a.CoerceValue, false);
                parameters.Add(a => a.CoerceText, false);
                parameters.Add(a => a.Immediate, false);
                parameters.Add(a => a.DebounceInterval, 0);
            });

            // Assert : Initial

            comp.Instance.ReadText.Should().BeNull();
            comp.Instance.ReadValue.Should().BeNull();

            // Act

            await comp.Find("input").InputAsync("ABC");

            // Assert : Immediate false, so value is not set

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().BeNull();

            // Act

            comp.Find("input").KeyUp("Enter");

            // Assert : CoercedValue disabled, so value is not set on key enter pressed

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().BeNull();
        }

        [Test]
        public async Task AutocompleteCoercionOffTest()
        {
            var comp = Context.Render<AutocompleteTestCoersionAndBlur>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.CoerceText, false));
            // check initial state
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");
            // set a value the search won't find
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, "Austria"));
            // now trigger the coercion by closing the menu
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Austria");
        }

        [Test]
        public async Task AutocompleteTextCoercionOnTabKeyTest()
        {
            var comp = Context.Render<AutocompleteTestCoersionAndBlur>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.CoerceText, true));

            // check initial state
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");

            // set a value the search won't find
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, "Austria"));
            autocomplete.ReadText.Should().Be("Austria");

            // now trigger the coercion by call MudInput.BlurAsync
            autocompleteComponent.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Tab" });
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");
        }

        [Test]
        public async Task AutocompleteTextCoercionAndResetIfEmptyTextTest()
        {
            var comp = Context.Render<AutocompleteTestCoersionAndBlur>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.CoerceText, true)
                .Add(x => x.ResetValueOnEmptyText, true));

            // check initial state
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");

            // set a value the search won't find
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, ""));
            autocomplete.ReadText.Should().Be(null);

            // now trigger the coercion by call MudInput.BlurAsync
            autocompleteComponent.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Tab" });
            autocomplete.ReadValue.Should().Be(null);
            autocomplete.ReadText.Should().Be(expected: null);
        }

        [Test]
        public async Task Autocomplete_Should_TolerateNullFromSearchFunc()
        {
            var comp = Context.Render<MudAutocomplete<string>>((a) =>
            {
                a.Add(x => x.DebounceInterval, 0);
                a.Add(x => x.SearchFunc, (_, _) => Task.FromResult<IEnumerable<string>>(null)); // <--- searchfunc returns null instead of sequence
            });
            // enter a text so the search func will return null, and it shouldn't throw an exception
            var setText1 = async () => await comp.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, "Do not throw"));
            var setSearchFunc = async () => await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SearchFunc, new Func<string, CancellationToken, Task<IEnumerable<string>>>((_, _) => null))); // <-- search func returns null instead of task!
            var setText2 = async () => await comp.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, "Don't throw here neither"));

            await setText1.Should().NotThrowAsync();
            await setSearchFunc.Should().NotThrowAsync();
            await setText2.Should().NotThrowAsync();
        }

        [Test]
        public async Task Autocomplete_ReadOnly_Should_Not_Open()
        {
            var comp = Context.Render<AutocompleteTest5>();
            await comp.FindAll(".mud-input-control")[0].MouseDownAsync(new MouseEventArgs());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
        }

        [Test]
        public async Task AutocompleteReadOnlyShouldNotHaveClearButton()
        {
            var comp = Context.Render<MudAutocomplete<string>>(p => p
                .Add(x => x.Text, "some value")
                .Add(x => x.Clearable, true)
                .Add(x => x.ReadOnly, false));

            comp.FindAll(".mud-input-clear-button").Count.Should().Be(1);

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ReadOnly, true)); //no clear button when readonly
            comp.FindAll(".mud-input-clear-button").Count.Should().Be(0);
        }

        /// <summary>
        /// MoreItemsTemplate should render when there are more items than the MaxItems limit
        /// </summary>
        [Test]
        public async Task AutocompleteTest6()
        {
            var comp = Context.Render<AutocompleteTest6>();

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            var mudText = comp.FindAll("p.mud-typography");
            mudText[^1].InnerHtml.Should().Contain("Not all items are shown"); //ensure the text is shown

            comp.FindAll("div.mud-popover .mud-autocomplete-more-items").Count.Should().Be(1);
        }

        /// <summary>
        /// NoItemsTemplate should render when there are no items
        /// </summary>
        [Test]
        public async Task AutocompleteTest7()
        {
            var comp = Context.Render<AutocompleteTest7>();

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            var mudText = comp.FindAll("p.mud-typography");
            mudText[^1].InnerHtml.Should().Contain("No items found, try another search"); //ensure the text is shown

            comp.FindAll("div.mud-popover .mud-autocomplete-no-items").Count.Should().Be(1);
        }

        /// <summary>
        /// After press Enter key down, the selected value should be shown in the input value
        /// </summary>
        [Test]
        public async Task Autocomplete_after_Enter_Should_show_Selected_Value()
        {
            var comp = Context.Render<AutocompleteTest1>();
            // select elements needed for the test
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;

            //insert "Calif"
            await autocompleteComponent.Find("input").InputAsync("Calif");
            await Task.Delay(100);
            var args = new KeyboardEventArgs { Key = "Enter" };

            //press Enter key
            autocompleteComponent.Find("input").KeyUp(args);

            //The value of the input should be California
            await comp.WaitForAssertionAsync(() => autocompleteComponent.Find("input").GetAttribute("value").Should().Be("California"));

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
            var comp = Context.Render<AutocompleteSetParametersInitialization>();
            // select elements needed for the test
            await Task.Delay(100);
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<ExternalList>>();
            autocompleteComponent.Find("input").GetAttribute("value").Should().Be("One");
        }

        /// <summary>
        /// Test for <seealso cref="https://github.com/MudBlazor/MudBlazor/issues/1415"/>
        /// </summary>
        [Test]
        public async Task Autocomplete_OnBlurShouldBeCalledAsync()
        {
            var calls = 0;
            void Fn(FocusEventArgs args) => calls++;
            var comp = Context.Render<MudAutocomplete<string>>(a =>
            {
                a.Add(x => x.OnBlur, Fn);
            });
            var input = comp.Find("input");

            calls.Should().Be(0);
            await input.BlurAsync();
            calls.Should().Be(1);
        }

        [Test]
        public async Task AutoCompleteClearableTest()
        {
            var comp = Context.Render<AutocompleteTestClearable>();

            // No button when initialized empty
            await comp.WaitForAssertionAsync(() => comp.FindAll(".mud-input-clear-button").Should().BeEmpty());

            // Button shows after entering text
            await comp.Find("input").InputAsync("text");
            await comp.WaitForAssertionAsync(() => comp.Find(".mud-input-clear-button").Should().NotBeNull());
            // Text cleared and button removed after clicking clear button
            await comp.Find(".mud-input-clear-button").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll(".mud-input-clear-button").Should().BeEmpty());

            // Button shows again after entering text
            await comp.Find("input").InputAsync("text");
            await comp.WaitForAssertionAsync(() => comp.Find(".mud-input-clear-button").Should().NotBeNull());
            // Button removed after clearing text
            await comp.Find("input").InputAsync(string.Empty);
            await comp.WaitForAssertionAsync(() => comp.FindAll(".mud-input-clear-button").Should().BeEmpty());
        }

        [Test]
        public async Task Autocomplete_Should_Validate_Data_Attribute_Fail()
        {
            var comp = Context.Render<AutocompleteValidationDataAttrTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.DebounceInterval, 0));
            // Set invalid option
            await comp.InvokeAsync(() => autocomplete.SelectOptionAsync("Quux"));
            // check initial state
            autocomplete.ReadValue.Should().Be("Quux");
            autocomplete.ReadText.Should().Be("Quux");
            // check validity
            await comp.InvokeAsync(autocomplete.ValidateAsync);
            autocomplete.ValidationErrors.Should().NotBeEmpty();
            autocomplete.ValidationErrors.Should().HaveCount(1);
            autocomplete.ValidationErrors[0].Should().Be("Should not be longer than 3");
        }

        [Test]
        public async Task Autocomplete_Should_Validate_Data_Attribute_Success()
        {
            var comp = Context.Render<AutocompleteValidationDataAttrTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.DebounceInterval, 0));
            // Set valid option
            await comp.InvokeAsync(() => autocomplete.SelectOptionAsync("Qux"));
            // check initial state
            autocomplete.ReadValue.Should().Be("Qux");
            autocomplete.ReadText.Should().Be("Qux");
            // check validity
            await comp.InvokeAsync(autocomplete.ValidateAsync);
            autocomplete.ValidationErrors.Should().BeEmpty();
        }

        /// <summary>
        /// Tests the required property.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_SetRequiredTrue()
        {
            var comp = Context.Render<AutocompleteRequiredTest>();

            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            autocomplete.Required.Should().BeTrue();

            await comp.InvokeAsync(autocomplete.ValidateAsync);

            autocomplete.ValidationErrors.First().Should().Be("Required");
        }

        /// <summary>
        /// Test for <seealso cref="https://github.com/MudBlazor/MudBlazor/issues/1761"/>
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Close_OnTab()
        {
            var comp = Context.Render<AutocompleteTest1>();
            // select elements needed for the test
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;

            // Should be closed
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());

            // Let's type something to cause it to open
            await autocompleteComponent.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            // Let's call blur on the input and confirm that it closed
            autocompleteComponent.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Tab" });
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());

            // Tab closes the drop-down and does not select the selected value (California)
            // because SelectValueOnTab is false by default
            autocomplete.ReadValue.Should().Be("Alabama");
        }

        [Test]
        public async Task Autocomplete_Should_SelectValue_On_Tab_With_SelectValueOnTab()
        {
            var comp = Context.Render<AutocompleteTest1>();
            // select elements needed for the test
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SelectValueOnTab, true));
            var autocomplete = autocompleteComponent.Instance;

            // Should be closed
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());

            // Lets type something to cause it to open
            await autocompleteComponent.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            // Lets call blur on the input and confirm that it closed
            autocompleteComponent.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Tab" });
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());

            // Tab closes the drop-down and selects the selected value (California)
            // because SelectValueOnTab is true
            autocomplete.ReadValue.Should().Be("California");
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
            var comp = Context.Render<AutocompleteTest1>();
            // select elements needed for the test
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;

            //No popover-open, due it's closed
            comp.Markup.Should().NotContain("mud-popover-open");

            // check initial state
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");

            // now let's type a different state to see the popup open
            await autocompleteComponent.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            var items = comp.FindComponents<MudListItem<string>>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");

            // now, we blur the input and assert that the popover is still open.
            await autocompleteComponent.Find("input").BlurAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
        }

        /// <summary>
        /// When calling Clear(), menu should closed, Value and Text should be cleared.
        /// </summary>
        [Test]
        public async Task Autocomplete_CheckTextValueandOpenState_OnClear()
        {
            var comp = Context.Render<AutocompleteTest1>();
            // select elements needed for the test
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.CoerceValue, true));
            var autocomplete = autocompleteComponent.Instance;

            //No popover-open, due it's closed
            comp.Markup.Should().NotContain("mud-popover-open");

            // check initial state
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");

            // ToggleMenu to open menu and Clear to close it and check the text and value
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            await comp.InvokeAsync(() => autocomplete.ClearAsync());
            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.ReadValue.Should().Be(null);
            autocomplete.ReadText.Should().Be("");

            // now let's type a different state
            await autocompleteComponent.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            var items = comp.FindComponents<MudListItem<string>>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");

            // Clearing it and check the close status text and value again
            await comp.InvokeAsync(() => autocomplete.ClearAsync());
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            autocomplete.ReadValue.Should().Be(null);
            autocomplete.ReadText.Should().Be("");
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
            var component = Context.Render<AutocompleteDisabledItemsTest>();

            // get the elements needed for the test
            var autocompleteComponent = component.FindComponent<MudAutocomplete<string>>();

            // Set the clear function on value changed
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ValueChanged, () => autocompleteComponent.Instance.ClearAsync()));

            // get the instance
            var autocompleteInstance = autocompleteComponent.Instance;

            // focus to open the popup
            await autocompleteComponent.Find("div.mud-input-control").FocusAsync();

            // ensure popup is open
            await component.WaitForAssertionAsync(() => autocompleteInstance.Open.Should().BeTrue("Input has been focused and should open the popup"));

            // get the matching states
            var matchingStates = component.FindComponents<MudListItem<string>>().ToArray();

            // try clicking 'Alaska'
            await matchingStates.Single(s => s.Markup.Contains(alaskaString)).Find(listItemQuerySelector).ClickAsync();
            await component.WaitForAssertionAsync(() => autocompleteInstance.ReadText.Should().Be(string.Empty));
        }

        /// <summary>
        /// When calling Reset(), menu should closed, Value and Text should be null.
        /// </summary>
        [Test]
        public async Task Autocomplete_CheckTextAndValue_OnReset()
        {
            var comp = Context.Render<AutocompleteTest1>();
            // select elements needed for the test
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.CoerceValue, true));
            var autocomplete = autocompleteComponent.Instance;

            //No popover-open, due it's closed
            comp.Markup.Should().NotContain("mud-popover-open");

            // check initial state
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");

            // Reset it
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            await comp.InvokeAsync(autocomplete.ResetAsync);
            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.ReadValue.Should().Be(null);
            autocomplete.ReadText.Should().Be("");

            // now let's type a different state
            await autocompleteComponent.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            var items = comp.FindComponents<MudListItem<string>>().ToArray();
            items.Length.Should().Be(1);
            items.First().Markup.Should().Contain("California");

            // Resetting it should close popover and set Text and Value to null again
            await comp.InvokeAsync(autocomplete.ResetAsync);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            autocomplete.ReadValue.Should().Be(null);
            autocomplete.ReadText.Should().Be("");
        }

        /// <summary>
        /// Generate parameters for the test of `ResetAsync`.
        /// </summary>
        /// <remarks>
        /// `ResetAsync` has the same behavior, regardless of the component's parameters.
        /// So this method generates all parameter combinations.
        /// </remarks>
        private static IEnumerable<bool[]> ResetAsyncParameters()
        {
            const int NbParameters = 4;
            var max = (int)Math.Pow(2, NbParameters);
            for (var i = 0; i < max; i++)
            {
                var bits = new System.Collections.BitArray([i]);
                yield return bits.Cast<bool>().Take(NbParameters).ToArray();
            }
        }

        /// <summary>
        /// When calling ResetAsync() without debounce,
        /// so menu should be closed, Text empty and Value null.
        /// </summary>
        [TestCaseSource(nameof(ResetAsyncParameters))]
        public async Task ResetAsync_WithoutDebounce_SoTextEmptyAndValueNull(bool resetValueOnEmptyText, bool coerceText, bool coerceValue, bool immediate)
        {
            // Arrange

            var comp = Context.Render<AutocompleteStates>(parameters =>
            {
                parameters.Add(a => a.DebounceInterval, 0);
            });
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;

            // Assert : initial state, menu closed and text/value null

            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.Open.Should().BeFalse();
            autocomplete.ReadValue.Should().BeNull();
            autocomplete.ReadText.Should().BeNull();
            comp.Instance.SearchFuncCallCount.Should().Be(0);

            // Act : Call ResetAsync()

            await comp.InvokeAsync(autocomplete.ResetAsync);

            // Assert : menu closed, text empty and value null

            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.ReadValue.Should().BeNull();
            autocomplete.ReadText.Should().BeEmpty();
            comp.Instance.SearchFuncCallCount.Should().Be(0);
        }

        /// <summary>
        /// When calling ResetAsync() with value and without debounce,
        /// so menu should be closed, Text empty and Value null.
        /// </summary>
        [TestCaseSource(nameof(ResetAsyncParameters))]
        public async Task ResetAsync_WithValueAndWithoutDebounce_SoTextEmptyAndValueNull(bool resetValueOnEmptyText, bool coerceText, bool coerceValue, bool immediate)
        {
            // Arrange

            var comp = Context.Render<AutocompleteStates>(parameters =>
            {
                parameters.Add(a => a.Value, "Idaho");
                parameters.Add(a => a.ResetValueOnEmptyText, resetValueOnEmptyText);
                parameters.Add(a => a.DebounceInterval, 0);
                parameters.Add(a => a.CoerceText, coerceText);
                parameters.Add(a => a.CoerceValue, coerceValue);
                parameters.Add(a => a.Immediate, immediate);
            });
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;

            // Assert : initial state, menu closed and text/value null

            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.Open.Should().BeFalse();
            autocomplete.ReadValue.Should().Be("Idaho");
            autocomplete.ReadText.Should().Be("Idaho");
            comp.Instance.SearchFuncCallCount.Should().Be(0);

            // Act : Call ResetAsync()

            await comp.InvokeAsync(autocomplete.ResetAsync);

            // Assert : menu closed, text empty and value null

            comp.Markup.Should().NotContain("mud-popover-open");
            autocomplete.ReadValue.Should().BeNull();
            autocomplete.ReadText.Should().BeEmpty();
            comp.Instance.SearchFuncCallCount.Should().Be(0);
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
            var component = Context.Render<AutocompleteDisabledItemsTest>();

            // get the elements needed for the test
            var autocompleteComponent = component.FindComponent<MudAutocomplete<string>>();

            // get the instance
            var autocompleteInstance = autocompleteComponent.Instance;

            // focus to open the popup
            await autocompleteComponent.Find("div.mud-input-control").FocusAsync();

            // ensure popup is open
            await component.WaitForAssertionAsync(() => autocompleteInstance.Open.Should().BeTrue("Input has been focused and should open the popup"));

            // get the matching states
            var matchingStates = component.FindComponents<MudListItem<string>>().ToArray();

            // try clicking 'American Samoa'
            await matchingStates.Single(s => s.Markup.Contains(americanSamoaString)).Find(listItemQuerySelector).ClickAsync();
            await component.WaitForAssertionAsync(() => autocompleteInstance.Value.Should().BeNullOrEmpty($"{americanSamoaString} should not be clickable."));

            // try clicking 'Alaska'
            await matchingStates.Single(s => s.Markup.Contains(alaskaString)).Find(listItemQuerySelector).ClickAsync();
            await component.WaitForAssertionAsync(() => autocompleteInstance.Value.Should().Be(alaskaString));

            // reset search-string
            await autocompleteComponent.Find(TagNames.Input).InputAsync(string.Empty);

            // wait till popup is visible
            await component.WaitForAssertionAsync(() => autocompleteInstance.Open.Should().BeTrue());

            // update found elements
            matchingStates = component.FindComponents<MudListItem<string>>().ToArray();

            // ensure alabama is selected
            await component.WaitForAssertionAsync(() => matchingStates.Single(s => s.Markup.Contains(alabamaString)).Find(listItemQuerySelector).ClassList.Should().Contain(selectedItemClassName, $"{alabamaString} should be selected/highlighted"));

            // define the event-args for arrow-down
            var arrowDownKeyboardEventArgs = new KeyboardEventArgs { Key = Key.Down.Value, Type = "keyup" };

            // invoke key down twice
            await autocompleteComponent.Find("input").KeyDownAsync(arrowDownKeyboardEventArgs);
            await autocompleteComponent.Find("input").KeyDownAsync(arrowDownKeyboardEventArgs);

            // ensure that index '4' is selected
            await component.WaitForAssertionAsync(() => selectedItemIndexPropertyInfo.GetValue(autocompleteInstance).Should().Be(4));

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
            var comp = Context.Render<AutocompleteChangeBoundObjectTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.DebounceInterval, 0));
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.CoerceText, true));
            await comp.WaitForAssertionAsync(() => autocompleteComponent.Find("input").GetAttribute("value").Should().Be("Florida"));
            autocomplete.ReadValue.Should().Be("Florida");
            autocomplete.ReadText.Should().Be("Florida");

            await comp.Find(".toggle-value-button").ClickAsync();
            await comp.WaitForAssertionAsync(() => autocompleteComponent.Find("input").GetAttribute("value").Should().Be("Georgia"));
            autocomplete.ReadValue.Should().Be("Georgia");
            autocomplete.ReadText.Should().Be("Georgia");

            await autocompleteComponent.Find("input").InputAsync("Alabam");
            await comp.WaitForAssertionAsync(() => autocompleteComponent.Find("input").GetAttribute("value").Should().Be("Alabam"));

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Enter" }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Enter" }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Escape" }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp" }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", AltKey = true }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown" }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Escape" }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "NumpadEnter" }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown" }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown" }));
            await comp.WaitForAssertionAsync(() => autocompleteComponent.Find("input").GetAttribute("value").Should().Be("Alabama"));
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");

            await comp.Find(".toggle-value-button").ClickAsync();

            await comp.WaitForAssertionAsync(() => autocompleteComponent.Find("input").GetAttribute("value").Should().Be("Florida"));
            autocomplete.ReadValue.Should().Be("Florida");
            autocomplete.ReadText.Should().Be("Florida");

            await comp.Find(".toggle-value-button").ClickAsync();
            await comp.WaitForAssertionAsync(() => autocompleteComponent.Find("input").GetAttribute("value").Should().Be("Alabama"));
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp" }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp" }));
            await comp.WaitForAssertionAsync(() => autocompleteComponent.Find("input").GetAttribute("value").Should().Be("Alabama"));

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp" }));
            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "Tab" }));
            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Tab" }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());

            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SelectValueOnTab, true));
            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp" }));
            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "Tab" }));
            await comp.WaitForAssertionAsync(() => autocompleteComponent.Find("input").GetAttribute("value").Should().Be("Alabama"));

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Backspace", CtrlKey = true, ShiftKey = true }));
            await comp.WaitForAssertionAsync(() => autocompleteComponent.Instance.ReadValue.Should().Be(null));

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "Tab" }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.CoerceText, true));
            await comp.InvokeAsync(() => autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Enter" }));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());
            await comp.InvokeAsync(() => autocomplete.OnEnterKeyAsync());
            await autocompleteComponent.Find("input").InputAsync("abc");
            await comp.InvokeAsync(async () => await autocomplete.SelectAsync());
            await comp.InvokeAsync(async () => await autocomplete.SelectRangeAsync(0, 1));
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await autocompleteComponent.Find("input").InputAsync("");
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await comp.InvokeAsync(() => autocomplete.OnEnterKeyAsync());
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());
        }

        [Test]
        public async Task Autocomplete_Should_Support_Sync_Search()
        {
            var root = Context.Render<AutocompleteSyncTest>();

            var popoverProvider = root.FindComponent<MudPopoverProvider>();
            var autocomplete = root.FindComponent<MudAutocomplete<string>>();
            var popover = autocomplete.FindComponent<MudPopover>();

            popover.Instance.Open.Should().BeFalse("Should start as closed");

            await autocomplete.Find("div.mud-input-control").FocusAsync();

            await popoverProvider.WaitForAssertionAsync(() =>
            {
                popover.Instance.Open.Should().BeTrue("Should be open once clicked");

                popoverProvider
                    .FindComponents<MudListItem<string>>().Count
                    .Should().Be(AutocompleteSyncTest.Items.Length, "Should show the expected items");
            });
        }

        /// <summary>
        /// When a user clicks on the adornment icon, the input should get focused so they can start typing
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_FocusInputOnAdornmentClick()
        {
            var jsRuntimeMock = new Mock<IJSRuntime>();
            jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("Blazor._internal.domWrapper.focus", It.IsAny<object[]>()));
            Context.Services.AddSingleton(jsRuntimeMock.Object);

            var comp = Context.Render<AutocompleteStates>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();

            var adornment = comp.Find(".mud-input-adornment-icon-button");
            await adornment.ClickAsync();

            // verifies FocusAsync was called
            jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("Blazor._internal.domWrapper.focus",
                It.Is<object[]>(args =>
                    args.Length == 2 &&
                    args[0] is ElementReference &&
                    args[1] is bool)),
                Times.AtMost(1));

            var input = comp.Find("input");
            await input.InputAsync("Wyo");

            await input.KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

            autocompleteComponent.Instance.ReadValue.Should().Be("Wyoming");
        }

        /// <summary>
        /// The adornment icon should change live without having to re-open the autocomplete
        /// This test a bugfix where changing the icon property would not cause the icon to visually change until the autocomplete was opened or closed
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_ChangeAdornmentIcon()
        {
            var comp = Context.Render<AutocompleteAdornmentChange>(parameters => parameters.Add(x => x.Icon, Icons.Material.Filled.Abc));
            var instance = comp.Instance;

            var markupBefore = comp.Find("svg.mud-icon-root").Children.ToMarkup().Trim();

            // change icon and render again
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Icon, Icons.Material.Filled.Remove));

            comp.Render();

            // check the initial icon
            var markupAfter = comp.Find("svg.mud-icon-root").Children.ToMarkup().Trim();
            markupAfter.Should().NotBe(markupBefore);
        }

        [Test]
        public async Task Autocomplete_Should_NotIndicateLoadingByDefault()
        {
            // Arrange
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();

            comp.Markup.Should().NotContain("progress-indicator-circular");
            await autocompleteComponent.Find("input").InputAsync("Calif");

            // Test
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().NotContain("progress-indicator-circular"));
        }

        [Test]
        public async Task Autocomplete_Should_IndicateLoadingWithCircularProgressIndicator()
        {
            // TODO: use a TaskCompletionSource that allows control over the search task
            // for reliable testing.  Applies to other tests like this one.
            // Currently, we increase the load time to 50mms to catch the progress UI

            // Arrange
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ShowProgressIndicator, true));

            comp.Markup.Should().NotContain("progress-indicator-circular");
            await autocompleteComponent.Find("input").InputAsync("Calif");

            // Test show
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").ClassList.Should().Contain("mud-autocomplete--with-progress"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().Contain("progress-indicator-circular"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            // Test hide
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().NotContain("progress-indicator-circular"));
        }

        [Test]
        public async Task Autocomplete_Should_IndicateLoadingWithCircularProgressIndicatorAndAdornmentAdjustment()
        {
            // Arrange
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.ShowProgressIndicator, true)
                .Add(x => x.AdornmentIcon, Icons.Material.Filled.Info)
                .Add(x => x.Adornment, Adornment.End));

            comp.Markup.Should().NotContain("progress-indicator-circular");
            await autocompleteComponent.Find("input").InputAsync("Calif");

            // Test show
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").ClassList.Should().Contain("mud-autocomplete--with-progress"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().Contain("progress-indicator-circular"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.progress-indicator-circular").ClassList.Should().Contain("progress-indicator-circular--with-adornment"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            // Test hide
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().NotContain("progress-indicator-circular"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().NotContain("progress-indicator-circular--with-adornment"));
        }

        [Test]
        public async Task Autocomplete_Should_IndicateLoadingWithCustomProgressIndicator()
        {
            // Arrange
            RenderFragment fragment = builder =>
            {
                builder.AddContent(0, "Loading...");
            };

            var comp = Context.Render<AutocompleteTest1>();
            var autocompletecomp = comp.FindComponent<MudAutocomplete<string>>();

            await autocompletecomp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.ShowProgressIndicator, true)
                .Add(p => p.ProgressIndicatorTemplate, fragment));

            comp.Markup.Should().NotContain("Loading...");
            await comp.InvokeAsync(() => autocompletecomp.Find("input").InputAsync("Calif"));

            // Test show
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").ClassList.Should().Contain("mud-autocomplete--with-progress"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().Contain("Loading..."));

            // Test hide
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").Children.ToMarkup().Should().NotContain("Loading..."));
        }

        [Test]
        public async Task Autocomplete_Should_IndicateLoadingWithProgressIndicatorInsidePopover()
        {
            // Arrange
            RenderFragment fragment = builder =>
            {
                builder.AddContent(0, "Loading...");
            };

            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();

            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.ShowProgressIndicator, true)
                .Add(p => p.ProgressIndicatorInPopoverTemplate, fragment));

            comp.Markup.Should().NotContain("Loading...");
           await comp.InvokeAsync(() => autocompleteComponent.Find("input").InputAsync("Calif"));

            // Test show
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").ClassList.Should().Contain("mud-autocomplete--with-progress"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ToMarkup().Should().Contain("Loading..."));

            // Test hide
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ToMarkup().Should().NotContain("Loading..."));
        }

        [Test]
        public async Task Autocomplete_Should_Cancel_Search()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();

            // Arrange first call

            CancellationToken? cancelToken = null;

            var first = new TaskCompletionSource<IEnumerable<string>>();

            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.SearchFunc, new Func<string, CancellationToken, Task<IEnumerable<string>>>((s, cancellationToken) =>
            {
                cancelToken = cancellationToken;
                // Return task that never completes.
                return first.Task;
            })));

            await comp.Find("input").InputAsync("Foo");

            await Task.Delay(20);

            // Test

            await comp.WaitForAssertionAsync(() => cancelToken?.IsCancellationRequested.Should().BeFalse());

            // Arrange second call

            var second = new TaskCompletionSource<IEnumerable<string>>();

            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.SearchFunc, new Func<string, CancellationToken, Task<IEnumerable<string>>>((s, cancellationToken) =>
            {
                return second.Task;
            })));

            await comp.Find("input").InputAsync("Bar");

            await Task.Delay(20);

            // Test

            await comp.WaitForAssertionAsync(() => cancelToken?.IsCancellationRequested.Should().BeTrue());

            first.SetCanceled();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ToMarkup().Should().NotContain("Foo"));

            second.SetResult(["Bar"]);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ToMarkup().Should().Contain("Bar"));
        }

        [Test]
        public async Task Autocomplete_FullWidth()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComp = comp.FindComponent<MudAutocomplete<string>>();

            autocompleteComp.Find("div.mud-select").ClassList.Should().Contain("mud-autocomplete");
            autocompleteComp.Find("div.mud-select").ClassList.Should().NotContain("mud-width-full");

            await autocompleteComp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.FullWidth, true));

            autocompleteComp.Find("div.mud-select").ClassList.Should().Contain("mud-autocomplete");
            autocompleteComp.Find("div.mud-select").ClassList.Should().Contain("mud-width-full");
        }

        [Test]
        public async Task Autocomplete_Should_HaveValueWithTextChangedEvent()
        {
            // Arrange
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            const string TestText = "testText";
            var currentText = string.Empty;

            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.TextChanged, text => currentText = text));

            // Act
            // Simulate user typing text (which should fire TextChanged)
            var input = comp.Find("input");
            await input.InputAsync(TestText);

            // Assert
            await autocompleteComponent.WaitForAssertionAsync(() => currentText.Should().Be(TestText));
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

            var comp = Context.Render<AutocompleteStrictFalseTest>();
            var autocompleteComponent = comp.FindComponents<MudAutocomplete<AutocompleteStrictFalseTest.State>>()[index];
            var autocomplete = autocompleteComponent.Instance;

            //search for and select California
            await autocompleteComponent.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover")[index].ClassList.Should().Contain("mud-popover-open"));
            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Enter" }));
            autocomplete.ReadText.Should().Be(californiaString);
            autocomplete.ReadValue.StateName.Should().Be(californiaString);

            //California should appear as index 5 and be selected
            await comp.InvokeAsync(autocompleteComponent.Instance.OpenMenuAsync); // reopen menu because Enter closes it.
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover")[index].ClassList.Should().Contain("mud-popover-open"));
            var items = comp.FindComponents<MudListItem<AutocompleteStrictFalseTest.State>>().ToArray();
            items.Length.Should().Be(10);
            var item = items.SingleOrDefault(x => x.Markup.Contains(californiaString));
            items.ToList().IndexOf(item).Should().Be(5);
            await comp.WaitForAssertionAsync(() => items.Single(s => s.Markup.Contains(californiaString)).Find(listItemQuerySelector).ClassList.Should().Contain(selectedItemClassName));

            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Escape" })); // Close autocomplete.

            //search for and select Virginia
            await autocompleteComponent.Find("input").InputAsync("Virginia");
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover")[index].ClassList.Should().Contain("mud-popover-open"));
            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Enter" }));
            autocomplete.ReadText.Should().Be(virginiaString);
            autocomplete.ReadValue.StateName.Should().Be(virginiaString);

            await comp.InvokeAsync(autocompleteComponent.Instance.OpenMenuAsync); // reopen menu because Enter closes it.
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover")[index].ClassList.Should().Contain("mud-popover-open"));
            var items2 = comp.FindComponents<MudListItem<AutocompleteStrictFalseTest.State>>().ToArray();
            items2.Length.Should().Be(10);
            // Select Virginia
            var item2 = items2.FirstOrDefault(x => x.Markup.Contains(virginiaString));
            // Virginia and West Virginia should be in the list
            var count = items2.Count(x => x.Markup.Contains(virginiaString));
            count.Should().Be(2);
            items2.ToList().IndexOf(item2).Should().Be(5);
            items2.Count(s => s.Find(listItemQuerySelector).ClassList.Contains(selectedItemClassName)).Should().Be(1);
        }

        [Test]
        public async Task Autocomplete_Should_Not_Throw_When_SearchFunc_Is_Null()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();

            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.SearchFunc, null));

            await comp.Find("input").InputAsync("Foo");

            await Task.Delay(20);

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ToMarkup().Should().NotContain("Foo"));
        }

        [Test]
        public async Task Autocomplete_Should_Raise_KeyDown_KeyUp_Event()
        {
            //Create comp
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var result = new List<string>();
            //create eventCallback
            var customEvent = new EventCallbackFactory().Create<KeyboardEventArgs>("A", () => result.Add("keyevent thrown"));

            //set eventCallback
            //SetCallback also possible
            //autocompletecomp.SetCallback(p => p.OnKeyDown, (KeyboardEventArgs e ) => result.Add("keyevent thrown"));
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.OnKeyDown, customEvent)
                .Add(p => p.OnKeyUp, customEvent));

            result.Should().BeEmpty();
            //Act
            autocompleteComponent.Find("input").KeyDown("a");
            autocompleteComponent.Find("input").KeyUp("a");
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

            var comp = Context.Render<AutocompleteStrictFalseSelectedHighlight>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;

            // Select the peach list item
            await autocompleteComponent.Find("input").InputAsync(selectedItemString);
            await comp.WaitForAssertionAsync(() => comp.Find(popoverSelector).ClassList.Should().Contain("mud-popover-open"));
            await comp.InvokeAsync(async () => await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs() { Key = "Enter" }));
            autocomplete.ReadText.Should().Be(selectedItemString);
            autocomplete.ReadValue.Should().Be(selectedItemString);

            // Opening the list of autocomplete
            await comp.InvokeAsync(autocompleteComponent.Instance.OpenMenuAsync);
            await comp.WaitForAssertionAsync(() => comp.Find(popoverSelector).ClassList.Should().Contain("mud-popover-open"));
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
            var component = Context.Render<AutocompleteResetTest>();
            var autocompleteComponent = component.FindComponent<MudAutocomplete<string>>();

            // get the instance
            var autocompleteInstance = autocompleteComponent.Instance;

            // focus to open the popup
            await autocompleteComponent.Find("div.mud-input-control").FocusAsync();

            // ensure popup is open
            await component.WaitForAssertionAsync(() => autocompleteInstance.Open.Should().BeTrue("Input has been focused and should open the popup"));

            // get the matching states
            var matchingStates = component.FindComponents<MudListItem<string>>().ToArray();

            // try clicking 'Test'
            await matchingStates.Single(s => s.Markup.Contains("Test")).Find("div.mud-list-item").ClickAsync();
            await component.WaitForAssertionAsync(() => autocompleteInstance.ReadText.Should().Be(string.Empty));
        }

        /// <summary>
        /// BeforeItemsTemplate should render when there are items
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_LoadListStartWhenSetAndThereAreItems()
        {
            var comp = Context.Render<AutocompleteListBeforeAndAfterRendersWithItemsTest>();

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

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
            var comp = Context.Render<AutocompleteListBeforeAndAfterRendersWithItemsTest>();

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            var mudText = comp.FindAll("p.mud-typography");
            mudText[^1].InnerHtml.Should().Contain("EndList_Content"); //ensure the text is shown

            comp.FindAll("div.mud-popover .mud-autocomplete-after-items").Count.Should().Be(1);
        }

        /// <summary>
        /// BeforeItemsTemplate should not render when there are no items
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Not_LoadListStartWhenSet()
        {
            var comp = Context.Render<AutocompleteListStartRendersTest>();

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            comp.Find("div.mud-popover").InnerHtml.Should().BeEmpty();
        }

        /// <summary>
        /// AfterItemsTemplate should not render when there are no items
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Not_LoadListEndWhenSet()
        {
            var comp = Context.Render<AutocompleteListEndRendersTest>();

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            comp.Find("div.mud-popover").InnerHtml.Should().BeEmpty();
        }

        [Test]
        public async Task Autocomplete_Should_ApplyListItemClass()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var listItemClassTest = "list-item-class-test";

            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.ListItemClass, listItemClassTest));
            await comp.Find("div.mud-input-control").FocusAsync();

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-list-item").ClassList.Should().Contain(listItemClassTest));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Autocomplete_Should_OpenMenuOnFocus(bool openOnFocus)
        {
            var comp = Context.Render<AutocompleteFocusTest>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.OpenOnFocus, openOnFocus));

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));

            await comp.Find("div.mud-input-control").FocusAsync();

            if (openOnFocus)
            {
                await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            }
            else
            {
                await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            }
        }

        [Test]
        public async Task Autocomplete_Should_OpenMenuOnFocus_AlwaysOnClick()
        {
            var comp = Context.Render<AutocompleteFocusTest>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.OpenOnFocus, false));

            await comp.Find("div.mud-input-control").FocusAsync(); // Browser would focus first.
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));

            await comp.Find("input.mud-input-root").MouseDownAsync(new MouseEventArgs());

            // OpenOnFocus=false isn't respected by clicks. It added after the fact to allow opting in to v6 behavior.
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
        }

        [Test]
        public async Task Autocomplete_ReturnedItemsCount_Should_Be_Accurate()
        {
            Task<IEnumerable<string>> Search(string value, CancellationToken token)
            {
                var values = new string[] { "Lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit" };
                return Task.FromResult(values.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
            }

            var comp = Context.Render<MudAutocomplete<string>>();
            await comp.SetParametersAndRenderAsync(p => p
                .Add(x => x.Value, "nothing will ever match this")
                .Add(x => x.SearchFunc, Search)
                .Add(x => x.DebounceInterval, 0));

            int? count = null;
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ReturnedItemsCountChanged, (Action<int>)(v => count = v)));

            await comp.Find("input").InputAsync("Lorem");
            await comp.WaitForAssertionAsync(() => count.Should().Be(1));

            await comp.Find("input").InputAsync("ip");
            await comp.WaitForAssertionAsync(() => count.Should().Be(2));

            await comp.Find("input").InputAsync("wtf");
            await comp.WaitForAssertionAsync(() => count.Should().Be(0));
        }

        /// <summary>
        /// An autocomplete component with a label should auto-generate an id for the input element and use that id on the label's for attribute.
        /// </summary>
        [Test]
        public void AutocompleteWithLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters
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
            var comp = Context.Render<MudAutocomplete<string>>(parameters
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
            var comp = Context.Render<MudAutocomplete<string>>(parameters
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
            var comp = Context.Render<MudAutocomplete<string>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required Autocomplete should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredAutocomplete_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required Autocomplete attributes should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredAutocompleteAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<MudAutocomplete<string>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Ensure selecting an option does not reopen the list.
        /// </summary>
        [Test]
        public async Task Autocomplete_SelectingOption_ShouldNot_ReopenList()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;

            // Open the menu
            await autocompleteComponent.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            // Select an option
            await comp.Find("div.mud-list-item").ClickAsync();

            // Assert: Menu should remain closed
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
        }

        /// <summary>
        /// Ensure the menu does not open in read-only mode.
        /// </summary>
        [Test]
        public async Task Autocomplete_User_ShouldNot_OpenMenu_InReadOnlyMode()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.ReadOnly, true)
                .Add(p => p.OpenOnFocus, true));
            var autocomplete = comp.Instance;

            // Attempt to open the menu via focus
            await comp.Find("div.mud-input-control").FocusAsync();

            // Assert: Menu should not open
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());

            // Attempt to open the menu via click
            await comp.Find("div.mud-input-control").MouseDownAsync(new MouseEventArgs());

            // Assert: Menu should not open
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());
        }

        /// <summary>
        /// Ensure the menu does not open in disabled mode.
        /// </summary>
        [Test]
        public async Task Autocomplete_User_ShouldNot_OpenMenu_InDisabledMode()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.Disabled, true)
                .Add(p => p.OpenOnFocus, true));
            var autocomplete = comp.Instance;

            // Attempt to open the menu via focus
            await comp.Find("div.mud-input-control").FocusAsync();

            // Assert: Menu should not open
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());

            // Attempt to open the menu via click
            await comp.Find("div.mud-input-control").MouseDownAsync(new MouseEventArgs());

            // Assert: Menu should not open
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());
        }

        /// <summary>
        /// Ensure that the ItemDisabledTemplate and ItemSelectedTemplate both can display when ItemTemplate isn't provided (null)
        /// </summary>
        [Test]
        public async Task AutocompleteItemTemplateDisplayTest()
        {
            var comp = Context.Render<AutocompleteItemTemplateDisplayTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();

            // Search for a to get Alabama, Alaska, American Samoa,...
            await autocompleteComponent.Find("input").InputAsync("a");

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
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
            var comp = Context.Render<MudAutocomplete<int>>(parameters => parameters
                .Add(p => p.ErrorId, "error-id")
                .Add(p => p.CoerceValue, true)
                .Add(p => p.Converter, new DummyErrorConverter())
                .Add(p => p.Text, "not a number"));

            comp.Instance.ConversionErrorMessage.Should().NotBeNullOrEmpty();
            comp.Find("#error-id").InnerHtml.Should().Be(comp.Instance.ConversionErrorMessage);
        }

        [TestCase(Adornment.Start)]
        [TestCase(Adornment.End)]
        public void Should_render_aria_label_for_adornment_if_provided(Adornment adornment)
        {
            var ariaLabel = "the aria label";
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
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
        public async Task Should_pass_various_aria_describedby_tests(
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

            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
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

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Error, true));
            var secondExpectedAriaDescribedBy = withUserHelperId ? $"{errorId} {helperId}" : errorId;

            // verify error text is rendered
            var errorAction = () => comp.Find($"#{errorId}");
            errorAction.Should().NotThrow();

            comp.Find(inputSelector).GetAttribute("aria-describedby").Should().Be(secondExpectedAriaDescribedBy);
        }
#nullable disable

        [Test]
        public void Autocomplete_Attribute_Should_Exist()
        {
            var comp = Context.Render<MudAutocomplete<string>>();

            comp.Find("input.mud-input-root").GetAttribute("autocomplete").Should().Be("off");
        }

        [Test]
        public void Should_Override_Autocomplete_Attribute_With_UserAttributes()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.UserAttributes, new() { ["autocomplete"] = "on" }));

            comp.Find("input.mud-input-root").GetAttribute("autocomplete").Should().Be("on");
        }

        /// <summary>
        /// https://github.com/MudBlazor/MudBlazor/issues/9495
        /// With `ResetValueOnEmptyText`,
        /// when the input text is cleared,
        /// then the value is set to null and the search func is called
        /// </summary>
        [Test]
        public async Task ResetValueOnEmptyText_WhenTextCleared_ThenSetNullAndTriggerSearch()
        {
            // Arrange

            var comp = Context.Render<AutocompleteResetValueOnEmptyText>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;

            // Act

            await autocompleteComponent.Find("input").InputAsync("");

            // Assert

            autocomplete.ReadValue.Should().Be(null);
            await comp.WaitForAssertionAsync(() => comp.Instance.SearchCount.Should().Be(1));
        }

        [Test]
        public void Should_Render_Classes_Correctly()
        {
            // Arrange
            var inputClass = "custom-input-class";

            // Act
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.InputClass, inputClass)
            );

            // Assert
            comp.Find(".mud-select-input").ClassList.Should().Contain(inputClass);
        }

        [Test]
        public async Task Should_Select_Correct_Item_With_ArrowKeys_And_Not_Wrap_Around()
        {
            var selectedItemIndexPropertyInfo = typeof(MudAutocomplete<string>).GetField("_selectedListItemIndex", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new ArgumentException("Cannot find field named '_selectedListItemIndex' on type 'MudAutocomplete<T>'");

            var component = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = component.FindComponent<MudAutocomplete<string>>();
            var autocompleteInstance = autocompleteComponent.Instance;

            // Focus to open the popup
            await autocompleteComponent.Find("div.mud-input-control").FocusAsync();

            // Ensure popup is open
            await component.WaitForAssertionAsync(() => autocompleteInstance.Open.Should().BeTrue("Input has been focused and should open the popup"));

            // Get the initial matching states (items in the dropdown)
            var matchingStates = component.FindComponents<MudListItem<string>>().ToArray();
            var maxIndex = matchingStates.Length - 1;

            // Define keyboard event args for ArrowDown and ArrowUp
            var arrowDownKeyboardEventArgs = new KeyboardEventArgs { Key = Key.Down.Value, Type = "keydown" };
            var arrowUpKeyboardEventArgs = new KeyboardEventArgs { Key = Key.Up.Value, Type = "keydown" };

            // Scroll down until reaching the last item
            for (var i = 0; i <= maxIndex; i++)
            {
                await autocompleteComponent.Find("input").KeyDownAsync(arrowDownKeyboardEventArgs);
            }

            // Check that the last item is selected
            var lastIndex = (int)selectedItemIndexPropertyInfo.GetValue(autocompleteInstance);
            await component.WaitForAssertionAsync(() => lastIndex.Should().Be(maxIndex, "ArrowDown should reach the last item"));

            // Press ArrowDown again to confirm it does not wrap around
            await autocompleteComponent.Find("input").KeyDownAsync(arrowDownKeyboardEventArgs);
            var noWrapIndex = (int)selectedItemIndexPropertyInfo.GetValue(autocompleteInstance);
            await component.WaitForAssertionAsync(() => noWrapIndex.Should().Be(maxIndex, "ArrowDown should not wrap around past the last item"));

            // Scroll up until reaching the first item
            for (var i = maxIndex; i >= 0; i--)
            {
                await autocompleteComponent.Find("input").KeyDownAsync(arrowUpKeyboardEventArgs);
            }

            // Check that the first item is selected
            var firstIndex = (int)selectedItemIndexPropertyInfo.GetValue(autocompleteInstance);
            await component.WaitForAssertionAsync(() => firstIndex.Should().Be(0, "ArrowUp should reach the first item"));

            // Press ArrowUp again to confirm it does not wrap around
            await autocompleteComponent.Find("input").KeyDownAsync(arrowUpKeyboardEventArgs);
            var noWrapToLastIndex = (int)selectedItemIndexPropertyInfo.GetValue(autocompleteInstance);
            await component.WaitForAssertionAsync(() => noWrapToLastIndex.Should().Be(0, "ArrowUp should not wrap around past the first item"));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task AutoComplete_ShouldHaveOnAdornmentClickBehavior(bool attachDelegate)
        {
            var eventCallbackFactory = new EventCallbackFactory();
            var _delegate = attachDelegate ?
                eventCallbackFactory.Create<MouseEventArgs>(this, (e) => { }) : default;

            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.OnAdornmentClick, _delegate));

            var autocompleteInstance = comp.Instance;
            autocompleteInstance.OnAdornmentClick.HasDelegate.Should().Be(attachDelegate);
            await comp.InvokeAsync(async () => await autocompleteInstance.AdornmentClickHandlerAsync());
            autocompleteInstance.Open.Should().Be(!attachDelegate);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task Autocomplete_OpenOnFocusShouldWork(bool openOnFocus)
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.OpenOnFocus, openOnFocus));
            await comp.Find("input").FocusAsync();

            await comp.WaitForAssertionAsync(() => comp.Instance.Open.Should().Be(openOnFocus, $"OpenOnFocus should set Open to {openOnFocus} after input Focus"));
        }

        [Test]
        public async Task Autocomplete_OpenTwiceInMenu()
        {
            var comp = Context.Render<AutocompleteMenuCloseTest>();
            // Open the menu
            await comp.Find("#menu-open").ClickAsync();
            await comp.WaitForAssertionAsync(() =>
            {
                comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Menu should be open");
                // comp.Find(".mud-overlay").Attributes["style"]?.Value.Should().Contain("1302", "Overlay should be present with 1302 as z-index");
            });

            // Focus on the autocomplete, which opens the autocomplete popover
            await comp.Find(".autocomplete input").FocusAsync();
            await comp.WaitForAssertionAsync(() =>
            {
                comp.FindAll("div.mud-popover-open").Count.Should().Be(2, "Both menu & autocomplete should be open");
                // comp.Find(".mud-overlay").Attributes["style"]?.Value.Should().Contain("z-index:1303");
            });

            // Click on the backdrop, closes the autocomplete
            await comp.Find(".mud-overlay").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1, "Only menu should be open"));
            // comp.Find(".mud-overlay").Attributes["style"]?.Value.Should().Contain("z-index:1302");

            // Click the autocomplete again, this will actually work even before the fix but clicking in this spot is will close the menu before fix.
            await comp.Find(".autocomplete input").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(2, "Both menu & autocomplete should be opened again"));
            // comp.Find(".mud-overlay").Attributes["style"]?.Value.Should().Contain("z-index:1303");
        }

        [Test]
        public async Task Autocomplete_OpenChanged_OpenMenuAsync()
        {
            var comp = Context.Render<AutocompleteOpenChangedTest>();
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Autocomplete.OpenMenuAsync());
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Autocomplete.OpenMenuAsync());
            comp.Instance.OpenedCount.Should().Be(1);
        }

        [Test]
        public async Task Autocomplete_OpenChanged_CloseMenuAsync()
        {
            var comp = Context.Render<AutocompleteOpenChangedTest>();
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Autocomplete.CloseMenuAsync());
            comp.Instance.ClosedCount.Should().Be(0);
        }

        [Test]
        public async Task Autocomplete_OpenChanged_OpenClose()
        {
            var comp = Context.Render<AutocompleteOpenChangedTest>();
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Autocomplete.OpenMenuAsync());
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Autocomplete.CloseMenuAsync());
            comp.Instance.OpenedCount.Should().Be(1);
            comp.Instance.ClosedCount.Should().Be(1);
        }

        [Test]
        public async Task Autocomplete_OpenChanged_SelectOptionAsync()
        {
            var comp = Context.Render<AutocompleteOpenChangedTest>();
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Autocomplete.SelectOptionAsync("Alabama"));
            comp.Instance.OpenedCount.Should().Be(0);
            comp.Instance.ClosedCount.Should().Be(1);
        }

        [Test]
        public async Task Autocomplete_OpenChanged_HandleClearButtonAsync()
        {
            var comp = Context.Render<AutocompleteHandleClearButtonAsyncTest>();
            await Context.Renderer.Dispatcher.InvokeAsync(() => comp.Instance.Autocomplete.HandleClearButtonAsync(new()));
            comp.Instance.OpenedCount.Should().Be(0);
            comp.Instance.ClosedCount.Should().Be(0);
            comp.Instance.ClearCount.Should().Be(1);
        }

        [Test]
        public void PopoverSettings_SetsDefaultValues()
        {
            var auto = Context.Render<MudAutocomplete<string>>();

            auto.Instance.PopoverFixed.Should().BeFalse();
            // When not set, should use global default from PopoverOptions
            auto.Instance.Modal.Should().BeNull();
        }

        [Test]
        public void PopoverSettings_OverridesDefaultValues()
        {
            var auto = Context.Render<MudAutocomplete<string>>(p =>
            {
                p.Add(p => p.PopoverFixed, true);
                p.Add(p => p.Modal, true);
            });

            auto.Instance.PopoverFixed.Should().BeTrue();
            auto.Instance.Modal.Should().BeTrue();
        }

        [Test]
        public void PopoverSettings_UsesGlobalDefaultsFromPopoverOptions()
        {
            // The default PopoverOptions should have OverflowBehavior.FlipOnOpen and ModalOverlay = false
            var auto = Context.Render<MudAutocomplete<string>>();

            // Verify that the component is using the global defaults
            // Modal should be null (using PopoverOptions defaults)
            auto.Instance.Modal.Should().BeNull();
        }
    }
}

