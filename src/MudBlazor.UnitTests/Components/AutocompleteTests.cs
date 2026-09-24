// Copyright (c) mudblazor 2021
// License MIT

using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Resources;
using MudBlazor.UnitTests.Dummy;
using MudBlazor.UnitTests.TestComponents.Autocomplete;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class AutocompleteTests : BunitTest
    {
        /// <summary>
        /// Autocomplete owns result keyboard navigation without installing an interceptor for every result.
        /// </summary>
        [Test]
        public async Task AutocompleteResults_DoNotRegisterKeyInterceptors()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.SearchFunc, (_, _) => Task.FromResult<IEnumerable<string>>(["one", "two", "three"])));

            await comp.Find("input").InputAsync(new ChangeEventArgs { Value = "o" });
            await provider.WaitForAssertionAsync(() => provider.FindComponents<MudListItem<string>>().Should().HaveCount(3));

            provider.FindComponents<MudListItem<string>>().Should().OnlyContain(x => !x.Instance.KeyboardEnabled);
            keyInterceptorService.ObserversCount.Should().Be(0);
        }

        /// <summary>
        /// The adornment button toggles the full result list, and typing filters it through the converter.
        /// </summary>
        [Test]
        public async Task Autocomplete_ConverterWithStrict_FiltersResults()
        {
            var comp = Context.Render<AutocompleteConverterStrictTest>();
            var autocomplete = comp.FindComponent<MudAutocomplete<AutocompleteConverterStrictTest.ConverterElement>>();
            IElement Popover() => comp.Find("div.mud-popover");
            Popover().ClassList.Should().NotContain("mud-popover-open");

            await autocomplete.Find(".mud-input-adornment-icon-button").ClickAsync();
            await comp.WaitForAssertionAsync(() => Popover().ClassList.Should().Contain("mud-popover-open"));
            comp.FindComponents<MudListItem<AutocompleteConverterStrictTest.ConverterElement>>().Should().HaveCount(10, "MaxItems defaults to 10");

            await autocomplete.Find(".mud-input-adornment-icon-button").ClickAsync();
            await comp.WaitForAssertionAsync(() => Popover().ClassList.Should().NotContain("mud-popover-open"));

            await autocomplete.Find("input").InputAsync("he");
            await comp.WaitForAssertionAsync(() => Popover().ClassList.Should().Contain("mud-popover-open"));
            comp.FindComponents<MudListItem<AutocompleteConverterStrictTest.ConverterElement>>().Should().HaveCount(4);
        }

        /// <summary>
        /// With CoerceValue and a custom converter, typed text is converted back into a value.
        /// </summary>
        [Test]
        public async Task AutocompleteCoerceValue_WithCustomConverter_UsesConvertBack()
        {
            var comp = Context.Render<AutocompleteConverterStrictTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<AutocompleteConverterStrictTest.ConverterElement>>();

            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.CoerceText, false)
                .Add(x => x.Immediate, true)
                .Add(x => x.DebounceInterval, 0));

            await autocompleteComponent.Find("input").InputAsync("Oxygen");

            await autocompleteComponent.WaitForAssertionAsync(() =>
            {
                autocompleteComponent.Instance.ReadText.Should().Be("Oxygen");
                autocompleteComponent.Instance.ReadValue.Should().NotBeNull();
                autocompleteComponent.Instance.ReadValue!.Name.Should().Be("Oxygen");
                autocompleteComponent.Instance.ConversionError.Should().BeFalse();
            });
        }

        /// <summary>
        /// The initial value is shown without opening the menu; typing opens matching results and clicking one sets the value.
        /// </summary>
        [Test]
        public async Task Autocomplete_ClickingResult_SetsValue()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");

            await comp.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            comp.Find("div.mud-popover").ClassList.Should().Contain("autocomplete-popover-class");
            comp.FindAll("div.mud-list-item").Select(x => x.TextContent.Trim()).Should().Equal("California");

            await comp.Find("div.mud-list-item").ClickAsync();

            await comp.WaitForAssertionAsync(() => autocomplete.ReadValue.Should().Be("California"));
            autocomplete.ReadText.Should().Be("California");
        }

        /// <summary>
        /// The modeless overlay ignores the autocomplete's own root, so pressing the autocomplete does not count as a click outside.
        /// </summary>
        [Test]
        public async Task Autocomplete_ModelessOverlay_IgnoresActivatorRootForAutoCloseHitTesting()
        {
            var comp = Context.Render<AutocompleteTest1>();

            await comp.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            var overlay = comp.Find("div.mud-overlay");
            overlay.GetAttribute("data-modeless-ignore-element-id").Should().Be(comp.Find("div.mud-autocomplete").Id);
        }

        /// <summary>
        /// With MinCharacters, focusing does not open the menu, typing enough characters does, and deleting below the minimum closes it again.
        /// </summary>
        [Test]
        public async Task Autocomplete_MinCharacters_OpensOnlyAtThreshold()
        {
            var comp = Context.Render<AutocompleteTest2>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            await comp.Find("div.mud-input-control").FocusAsync();
            autocomplete.Open.Should().BeFalse();

            await comp.Find("input").InputAsync("ala");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await comp.Find("input").InputAsync("al");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
        }

        /// <summary>
        /// ToStringFunc formats the initial value's text.
        /// </summary>
        [Test]
        public void Autocomplete_ToStringFunc_FormatsInitialText()
        {
            var comp = Context.Render<AutocompleteTest3>();

            comp.FindComponent<MudAutocomplete<AutocompleteTest3.State>>().Instance.ReadText.Should().Be("Assam");
        }

        /// <summary>
        /// Disposing the autocomplete cancels a search that is still running.
        /// </summary>
        [Test]
        public async Task Autocomplete_Dispose_CancelsPendingSearch()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            var comp = Context.Render<AutocompleteTest8>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>();

            await autocomplete.Find("input").InputAsync("Cal");
            timeProvider.Advance(TimeSpan.FromMilliseconds(autocomplete.Instance.DebounceInterval));
            comp.Instance.HasBeenDisposed.Should().BeFalse();

            comp.Instance.MustBeShown = false;
            comp.Render();

            await comp.WaitForAssertionAsync(() => comp.Instance.HasBeenDisposed.Should().BeTrue());
        }

        /// <summary>
        /// Without a ToStringFunc, the value's own ToString formats the initial text.
        /// </summary>
        [Test]
        public void Autocomplete_ValueToString_FormatsInitialText()
        {
            var comp = Context.Render<AutocompleteTest4>();

            comp.FindComponent<MudAutocomplete<AutocompleteTest4.State>>().Instance.ReadText.Should().Be("Assam");
        }

        /// <summary>
        /// With CoerceText, leaving text that matches nothing restores the text of the current value.
        /// </summary>
        [Test]
        public async Task Autocomplete_CoerceText_RestoresValueTextOnBlur()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");

            // The search finds nothing for this text, so the menu stays closed.
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, "Austria"));
            autocomplete.Open.Should().BeFalse();

            await comp.Find("input").BlurAsync(new FocusEventArgs());

            await autocompleteComponent.WaitForAssertionAsync(() => autocomplete.ReadText.Should().Be("Alabama"));
            autocomplete.ReadValue.Should().Be("Alabama");
        }

        /// <summary>
        /// With CoerceValue, text that matches nothing becomes the value when the menu is toggled, so it can be validated.
        /// </summary>
        [Test]
        public async Task Autocomplete_CoerceValue_AppliesUnmatchedTextOnMenuToggle()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.CoerceValue, true));
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, "Austria"));

            // The menu does not open for text that matches nothing, but toggling it still coerces the value.
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);

            await comp.WaitForAssertionAsync(() => autocomplete.ReadValue.Should().Be("Austria"));
            autocomplete.ReadText.Should().Be("Austria");
        }

        /// <summary>
        /// With Immediate and CoerceValue, text that matches nothing becomes the value as soon as it changes (#5993).
        /// </summary>
        [Test]
        public async Task Autocomplete_ImmediateCoerceValue_AppliesUnmatchedTextAsTyped()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.CoerceValue, true)
                .Add(x => x.CoerceText, false)
                .Add(x => x.Immediate, true));

            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, "Austria"));

            await comp.WaitForAssertionAsync(() => autocomplete.ReadValue.Should().Be("Austria"));
            autocomplete.ReadText.Should().Be("Austria");
        }

        /// <summary>
        /// Clearing the text of an object-valued autocomplete with immediate value coercion clears the value without a conversion error.
        /// </summary>
        [Test]
        public async Task AutocompleteImmediateCoerceValue_WithToStringFuncAndObjectValue_DoesNotThrowOnClear()
        {
            var comp = Context.Render<MudAutocomplete<CoerceValueElement>>(parameters =>
            {
                parameters.Add(a => a.Value, new CoerceValueElement { Name = "Helium" });
                parameters.Add(a => a.CoerceValue, true);
                parameters.Add(a => a.CoerceText, false);
                parameters.Add(a => a.Immediate, true);
                parameters.Add(a => a.DebounceInterval, 0);
                parameters.Add(a => a.ToStringFunc, static x => x?.Name);
            });

            await comp.WaitForAssertionAsync(() =>
            {
                comp.Instance.ReadText.Should().Be("Helium");
                comp.Instance.ReadValue.Should().NotBeNull();
            });

            await comp.Find("input").InputAsync(string.Empty);

            await comp.WaitForAssertionAsync(() =>
            {
                comp.Instance.ReadText.Should().BeEmpty();
                comp.Instance.ReadValue.Should().BeNull();
                comp.Instance.ConversionError.Should().BeFalse();
            });
        }

        /// <summary>
        /// Without a debounce, Immediate value coercion sets the value and opens the menu on the first keystroke.
        /// </summary>
        [Test]
        public async Task OnTextChanged_WithCoerceValueAndNotCoerceTextAndImmediateNotDebounce_SetValueAndOpenMenuImmediately()
        {
            var valueChangedCount = 0;
            var comp = Context.Render<AutocompleteStates>(parameters =>
            {
                parameters.Add(p => p.DebounceInterval, 0);
                parameters.Add(p => p.CoerceText, false);
                parameters.Add(p => p.CoerceValue, true);
                parameters.Add(p => p.Immediate, true);
                parameters.Add(p => p.ValueChanged, v => valueChangedCount++);
            });
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
            autocomplete.ReadValue.Should().BeNull();
            autocomplete.ReadText.Should().BeNull();
            comp.Instance.SearchFuncCallCount.Should().Be(0);

            await comp.Find("input").InputAsync("Al");

            autocomplete.Open.Should().BeTrue();
            comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open");
            autocomplete.ReadText.Should().Be("Al");
            autocomplete.ReadValue.Should().Be("Al");
            valueChangedCount.Should().Be(1);
        }

        /// <summary>
        /// With a debounce, Immediate value coercion sets the value on the first keystroke but opens the menu only after the interval.
        /// </summary>
        [Test]
        public async Task OnTextChanged_CoerceValueAndNotCoerceTextAndImmediateAndDebounce_SetValueImmediatelyButDelaysMenuOpening()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            var valueChangedCount = 0;
            var comp = Context.Render<AutocompleteStates>(parameters =>
            {
                parameters.Add(p => p.DebounceInterval, 500);
                parameters.Add(p => p.CoerceText, false);
                parameters.Add(p => p.CoerceValue, true);
                parameters.Add(p => p.Immediate, true);
                parameters.Add(p => p.ValueChanged, v => valueChangedCount++);
            });
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            await comp.Find("input").InputAsync("Al");

            autocomplete.Open.Should().BeFalse();
            comp.Instance.SearchFuncCallCount.Should().Be(0);
            autocomplete.ReadText.Should().Be("Al");
            autocomplete.ReadValue.Should().Be("Al");
            valueChangedCount.Should().Be(1);

            timeProvider.Advance(TimeSpan.FromMilliseconds(autocomplete.DebounceInterval));

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            autocomplete.ReadText.Should().Be("Al");
            autocomplete.ReadValue.Should().Be("Al");
            valueChangedCount.Should().Be(1);
        }

        /// <summary>
        /// OnDebounceIntervalElapsed fires once with the typed text when the debounce interval ends.
        /// </summary>
        [Test]
        public async Task OnTextChanged_WithDebounce_InvokesOnDebounceIntervalElapsed()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            var debouncedTexts = new List<string>();
            var comp = Context.Render<AutocompleteStates>(parameters =>
            {
                parameters.Add(p => p.DebounceInterval, 500);
                parameters.Add(p => p.OnDebounceIntervalElapsed, debouncedTexts.Add);
            });
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            await comp.Find("input").InputAsync("Al");
            debouncedTexts.Should().BeEmpty();

            timeProvider.Advance(TimeSpan.FromMilliseconds(autocomplete.DebounceInterval));

            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());
            debouncedTexts.Should().Equal("Al");
        }

        /// <summary>
        /// With CoerceValue but not Immediate, typed text becomes the value only when the input loses focus.
        /// </summary>
        [Test]
        public async Task CoerceValueAndNotCoerceTextAndNotImmediate_ValueSetOnBlur()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters =>
            {
                parameters.Add(a => a.CoerceValue, true);
                parameters.Add(a => a.CoerceText, false);
                parameters.Add(a => a.Immediate, false);
                parameters.Add(a => a.DebounceInterval, 0);
            });

            await comp.Find("input").InputAsync("ABC");

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().BeNull();

            await comp.Find("input").BlurAsync();

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().Be("ABC");
        }

        /// <summary>
        /// Coercing a value on blur without a converter leaves an object-valued autocomplete's value empty, without a conversion error.
        /// </summary>
        [Test]
        public async Task CoerceValueWithToStringFuncAndObjectValue_DoesNotThrowOnBlur()
        {
            var comp = Context.Render<MudAutocomplete<CoerceValueElement>>(parameters =>
            {
                parameters.Add(a => a.CoerceValue, true);
                parameters.Add(a => a.CoerceText, false);
                parameters.Add(a => a.Immediate, false);
                parameters.Add(a => a.DebounceInterval, 0);
                parameters.Add(a => a.ToStringFunc, static x => x?.Name);
            });

            await comp.Find("input").InputAsync("Hydrogen");
            await comp.Find("input").BlurAsync();

            await comp.WaitForAssertionAsync(() =>
            {
                comp.Instance.ReadText.Should().Be("Hydrogen");
                comp.Instance.ReadValue.Should().BeNull();
                comp.Instance.ConversionError.Should().BeFalse();
            });
        }

        /// <summary>
        /// Coercing a value on blur relies on neither ToStringFunc nor the value's ToString, so an object-valued autocomplete stays empty without a conversion error.
        /// </summary>
        [Test]
        public async Task CoerceValueWithObjectToStringAndNoToStringFunc_DoesNotThrowOnBlur()
        {
            var comp = Context.Render<MudAutocomplete<CoerceValueElement>>(parameters =>
            {
                parameters.Add(a => a.CoerceValue, true);
                parameters.Add(a => a.CoerceText, false);
                parameters.Add(a => a.Immediate, false);
                parameters.Add(a => a.DebounceInterval, 0);
            });

            await comp.Find("input").InputAsync("Lithium");
            await comp.Find("input").BlurAsync();

            await comp.WaitForAssertionAsync(() =>
            {
                comp.Instance.ReadText.Should().Be("Lithium");
                comp.Instance.ReadValue.Should().BeNull();
                comp.Instance.ConversionError.Should().BeFalse();
            });
        }

        /// <summary>
        /// Without CoerceValue, typed text never becomes the value, not even when the input loses focus.
        /// </summary>
        [Test]
        public async Task NotCoerceValueAndNotCoerceTextAndNotImmediate_ValueNotSetOnBlur()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters =>
            {
                parameters.Add(a => a.CoerceValue, false);
                parameters.Add(a => a.CoerceText, false);
                parameters.Add(a => a.Immediate, false);
                parameters.Add(a => a.DebounceInterval, 0);
            });

            await comp.Find("input").InputAsync("ABC");
            await comp.Find("input").BlurAsync();

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().BeNull();
        }

        /// <summary>
        /// With CoerceValue but not Immediate, Enter turns typed text into the value.
        /// </summary>
        [Test]
        public async Task CoerceValueAndNotCoerceTextAndNotImmediate_ValueSetOnEnter()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters =>
            {
                parameters.Add(a => a.CoerceValue, true);
                parameters.Add(a => a.CoerceText, false);
                parameters.Add(a => a.Immediate, false);
                parameters.Add(a => a.DebounceInterval, 0);
            });

            await comp.Find("input").InputAsync("ABC");

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().BeNull();

            await comp.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().Be("ABC");
        }

        /// <summary>
        /// Without CoerceValue, Enter does not turn typed text into the value.
        /// </summary>
        [Test]
        public async Task NotCoerceValueAndNotCoerceTextAndNotImmediate_ValueNotSetOnEnter()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters =>
            {
                parameters.Add(a => a.CoerceValue, false);
                parameters.Add(a => a.CoerceText, false);
                parameters.Add(a => a.Immediate, false);
                parameters.Add(a => a.DebounceInterval, 0);
            });

            await comp.Find("input").InputAsync("ABC");
            await comp.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

            comp.Instance.ReadText.Should().Be("ABC");
            comp.Instance.ReadValue.Should().BeNull();
        }

        /// <summary>
        /// Without CoerceText, closing the menu keeps unmatched text and leaves the value unchanged.
        /// </summary>
        [Test]
        public async Task Autocomplete_CoerceTextOff_KeepsTypedTextOnClose()
        {
            var comp = Context.Render<AutocompleteTestCoersionAndBlur>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.CoerceText, false));
            autocomplete.ReadValue.Should().Be("Alabama");

            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, "Austria"));
            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);

            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Austria");
        }

        /// <summary>
        /// With CoerceText, Tab away from unmatched text restores the text of the current value.
        /// </summary>
        [Test]
        public async Task Autocomplete_CoerceText_RestoresValueTextOnTab()
        {
            var comp = Context.Render<AutocompleteTestCoersionAndBlur>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.CoerceText, true));
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, "Austria"));
            autocomplete.ReadText.Should().Be("Austria");

            await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "Tab" });

            autocomplete.ReadValue.Should().Be("Alabama");
            autocomplete.ReadText.Should().Be("Alabama");
        }

        /// <summary>
        /// With ResetValueOnEmptyText, emptying the text clears the value, and Tab does not restore it.
        /// </summary>
        [Test]
        public async Task Autocomplete_ResetValueOnEmptyText_ClearsValueWhenTextEmptied()
        {
            var comp = Context.Render<AutocompleteTestCoersionAndBlur>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.CoerceText, true)
                .Add(x => x.ResetValueOnEmptyText, true));
            autocomplete.ReadValue.Should().Be("Alabama");

            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, ""));
            autocomplete.ReadText.Should().BeNull();

            await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "Tab" });

            autocomplete.ReadValue.Should().BeNull();
            autocomplete.ReadText.Should().BeNull();
        }

        /// <summary>
        /// A SearchFunc that returns null, or a null task, does not throw.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_TolerateNullFromSearchFunc()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.SearchFunc, (_, _) => Task.FromResult<IEnumerable<string>>(null)));

            var setText1 = async () => await comp.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, "Do not throw"));
            var setSearchFunc = async () => await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SearchFunc, new Func<string, CancellationToken, Task<IEnumerable<string>>>((_, _) => null)));
            var setText2 = async () => await comp.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.Text, "Don't throw here neither"));

            await setText1.Should().NotThrowAsync();
            await setSearchFunc.Should().NotThrowAsync();
            await setText2.Should().NotThrowAsync();
        }

        /// <summary>
        /// A read-only autocomplete hides its clear button.
        /// </summary>
        [Test]
        public async Task Autocomplete_ReadOnly_HidesClearButton()
        {
            var comp = Context.Render<MudAutocomplete<string>>(p => p
                .Add(x => x.Text, "some value")
                .Add(x => x.Clearable, true)
                .Add(x => x.ReadOnly, false));

            comp.FindAll(".mud-input-clear-button").Should().ContainSingle();

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ReadOnly, true));
            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();
        }

        /// <summary>
        /// A read-only autocomplete marks its input with the mud-readonly class.
        /// </summary>
        [Test]
        public async Task Autocomplete_ReadOnly_AddsReadonlyClass()
        {
            var comp = Context.Render<MudAutocomplete<string>>(p => p
                .Add(x => x.ReadOnly, false));

            comp.Find(".mud-select-input").ClassList.Should().NotContain("mud-readonly");

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ReadOnly, true));
            comp.Find(".mud-select-input").ClassList.Should().Contain("mud-readonly");
        }

        /// <summary>
        /// MaxItems caps the results, and MoreItemsTemplate says that some were left out.
        /// </summary>
        [Test]
        public async Task Autocomplete_MoreItemsTemplate_ShownWhenResultsExceedMaxItems()
        {
            var comp = Context.Render<AutocompleteTest6>();

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            comp.FindAll("div.mud-list-item").Should().HaveCount(10);
            comp.Find("div.mud-popover .mud-autocomplete-more-items").TextContent.Should().Contain("Not all items are shown");
        }

        /// <summary>
        /// NoItemsTemplate opens the menu with its message when the search finds nothing.
        /// </summary>
        [Test]
        public async Task Autocomplete_NoItemsTemplate_ShownWhenNothingMatches()
        {
            var comp = Context.Render<AutocompleteTest7>();

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            comp.FindAll("div.mud-list-item").Should().BeEmpty();
            comp.Find("div.mud-popover .mud-autocomplete-no-items").TextContent.Should().Contain("No items found, try another search");
        }

        /// <summary>
        /// Enter selects the highlighted result, shows it in the input and closes the menu.
        /// </summary>
        [Test]
        public async Task Autocomplete_Enter_SelectsHighlightedResultAndCloses()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            await comp.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await comp.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

            await comp.WaitForAssertionAsync(() => comp.Find("input").GetAttribute("value").Should().Be("California"));
            autocomplete.Open.Should().BeFalse();
        }

        /// <summary>
        /// A value supplied during the parent's asynchronous initialization is shown once it arrives (#1235).
        /// </summary>
        [Test]
        public async Task Autocomplete_Initialize_Value_on_SetParameters()
        {
            var comp = Context.Render<AutocompleteSetParametersInitialization>();

            await comp.WaitForAssertionAsync(() => comp.Find("input").GetAttribute("value").Should().Be("One"));
        }

        /// <summary>
        /// When T is a complex type and Text is explicitly set without a Value, the initial Text is kept (#12900).
        /// </summary>
        [Test]
        public void Autocomplete_ComplexType_Should_Preserve_Initial_Text()
        {
            var comp = Context.Render<AutocompleteInitialTextComplexTypeTest>();

            comp.Find("input").GetAttribute("value").Should().Be("InitialValue");
        }

        /// <summary>
        /// Blurring the input raises OnBlur (#1415).
        /// </summary>
        [Test]
        public async Task Autocomplete_OnBlurShouldBeCalled()
        {
            var calls = 0;
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.OnBlur, _ => calls++));

            await comp.Find("input").BlurAsync();

            calls.Should().Be(1);
        }

        /// <summary>
        /// A required autocomplete validates when the user leaves it with the menu closed (#5489).
        /// </summary>
        [Test]
        public async Task Autocomplete_Required_ValidatesOnBlur()
        {
            var localizer = Context.Services.GetRequiredService<InternalMudLocalizer>();
            var comp = Context.Render<MudAutocomplete<string>>(a => a
                .Add(x => x.Required, true)
                .Add(x => x.RequiredError, localizer[LanguageResource.MudFormComponent_Required])
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.SearchFunc, (s, t) => Task.FromResult<IEnumerable<string>>(new[] { "a", "b" })));
            var ac = comp.Instance;

            ac.Touched.Should().BeFalse();
            ac.HasErrors.Should().BeFalse();

            await comp.Find("input").BlurAsync();
            await comp.WaitForAssertionAsync(() =>
            {
                ac.Touched.Should().BeTrue("leaving an empty required autocomplete validates it (#5489)");
                ac.HasErrors.Should().BeTrue();
                ac.ValidationErrors.Should().Contain(localizer[LanguageResource.MudFormComponent_Required]);
            });
        }

        /// <summary>
        /// A required autocomplete pre-filled with a value is valid on blur (#9425).
        /// </summary>
        [Test]
        public async Task Autocomplete_Required_PreFilledValue_IsValidOnBlur()
        {
            var localizer = Context.Services.GetRequiredService<InternalMudLocalizer>();
            var comp = Context.Render<MudAutocomplete<string>>(a => a
                .Add(x => x.Required, true)
                .Add(x => x.RequiredError, localizer[LanguageResource.MudFormComponent_Required])
                .Add(x => x.Value, "a")
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.SearchFunc, (s, t) => Task.FromResult<IEnumerable<string>>(new[] { "a", "b" })));
            var ac = comp.Instance;

            await comp.Find("input").BlurAsync();
            await comp.WaitForAssertionAsync(() =>
            {
                ac.Touched.Should().BeTrue("leaving the field marks it touched (#9425)");
                ac.HasErrors.Should().BeFalse("a pre-filled required autocomplete is valid on blur (#9425)");
                ac.ValidationErrors.Should().BeEmpty();
            });
        }

        /// <summary>
        /// A required autocomplete validates on blur even when Immediate is disabled (#5489).
        /// </summary>
        [Test]
        public async Task Autocomplete_Required_NonImmediate_ValidatesOnBlur()
        {
            var localizer = Context.Services.GetRequiredService<InternalMudLocalizer>();
            var comp = Context.Render<MudAutocomplete<string>>(a => a
                .Add(x => x.Required, true)
                .Add(x => x.RequiredError, localizer[LanguageResource.MudFormComponent_Required])
                .Add(x => x.Immediate, false)
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.SearchFunc, (s, t) => Task.FromResult<IEnumerable<string>>(new[] { "a", "b" })));
            var ac = comp.Instance;

            ac.Touched.Should().BeFalse();
            ac.HasErrors.Should().BeFalse();

            await comp.Find("input").BlurAsync();
            await comp.WaitForAssertionAsync(() =>
            {
                ac.Touched.Should().BeTrue("leaving an empty required autocomplete validates it, even when Immediate is off (#5489)");
                ac.HasErrors.Should().BeTrue();
                ac.ValidationErrors.Should().Contain(localizer[LanguageResource.MudFormComponent_Required]);
            });
        }

        /// <summary>
        /// The clear button appears while there is text and disappears when the text is cleared by the button or by typing.
        /// </summary>
        [Test]
        public async Task Autocomplete_ClearButton_ShownOnlyWithText()
        {
            var comp = Context.Render<AutocompleteTestClearable>();
            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();

            await comp.Find("input").InputAsync("text");
            await comp.WaitForAssertionAsync(() => comp.FindAll(".mud-input-clear-button").Should().ContainSingle());
            await comp.Find(".mud-input-clear-button").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll(".mud-input-clear-button").Should().BeEmpty());

            await comp.Find("input").InputAsync("text");
            await comp.WaitForAssertionAsync(() => comp.FindAll(".mud-input-clear-button").Should().ContainSingle());
            await comp.Find("input").InputAsync(string.Empty);
            await comp.WaitForAssertionAsync(() => comp.FindAll(".mud-input-clear-button").Should().BeEmpty());
        }

        /// <summary>
        /// A data annotation on the bound property rejects an invalid value.
        /// </summary>
        [Test]
        public async Task Autocomplete_DataAnnotation_InvalidValue_FailsValidation()
        {
            var comp = Context.Render<AutocompleteValidationDataAttrTest>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            await comp.InvokeAsync(() => autocomplete.SelectOptionAsync("Quux"));
            autocomplete.ReadValue.Should().Be("Quux");
            autocomplete.ReadText.Should().Be("Quux");

            await comp.InvokeAsync(autocomplete.ValidateAsync);
            autocomplete.ValidationErrors.Should().Equal("Should not be longer than 3");
        }

        /// <summary>
        /// A data annotation on the bound property accepts a valid value.
        /// </summary>
        [Test]
        public async Task Autocomplete_DataAnnotation_ValidValue_PassesValidation()
        {
            var comp = Context.Render<AutocompleteValidationDataAttrTest>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            await comp.InvokeAsync(() => autocomplete.SelectOptionAsync("Qux"));
            autocomplete.ReadValue.Should().Be("Qux");
            autocomplete.ReadText.Should().Be("Qux");

            await comp.InvokeAsync(autocomplete.ValidateAsync);
            autocomplete.ValidationErrors.Should().BeEmpty();
        }

        /// <summary>
        /// A required autocomplete without a value fails validation with the localized required message.
        /// </summary>
        [Test]
        public async Task Autocomplete_Required_WithoutValue_FailsValidation()
        {
            var localizer = Context.Services.GetRequiredService<InternalMudLocalizer>();
            var comp = Context.Render<AutocompleteRequiredTest>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            await comp.InvokeAsync(autocomplete.ValidateAsync);

            autocomplete.ValidationErrors.Should().Equal(localizer[LanguageResource.MudFormComponent_Required]);
        }

        /// <summary>
        /// Tab closes the menu without selecting the highlighted result by default (#1761).
        /// </summary>
        [Test]
        public async Task Autocomplete_Tab_ClosesWithoutSelecting()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            await comp.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "Tab" });

            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());
            autocomplete.ReadValue.Should().Be("Alabama");
        }

        /// <summary>
        /// With SelectValueOnTab, Tab selects the highlighted result as it closes the menu.
        /// </summary>
        [Test]
        public async Task Autocomplete_SelectValueOnTab_TabSelectsHighlightedResult()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SelectValueOnTab, true));
            var autocomplete = autocompleteComponent.Instance;

            await comp.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "Tab" });

            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());
            autocomplete.ReadValue.Should().Be("California");
        }

        /// <summary>
        /// Blurring the input keeps the menu open, because clicking a result blurs the input before the click lands.
        /// </summary>
        /// <remarks>
        /// A regression here made results unclickable and caused PR #1807 to be reverted.
        /// </remarks>
        [Test]
        public async Task Autocomplete_Should_NotCloseDropdownOnInputBlur()
        {
            var comp = Context.Render<AutocompleteTest1>();

            await comp.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await comp.Find("input").BlurAsync();

            comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open");
        }

        /// <summary>
        /// ClearAsync and ResetAsync close the menu and empty the text and value, whether the menu was opened by code or by typing.
        /// </summary>
        [TestCase(false)]
        [TestCase(true)]
        public async Task Autocomplete_ClearOrReset_ClosesMenuAndEmptiesTextAndValue(bool reset)
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.CoerceValue, true));
            var autocomplete = autocompleteComponent.Instance;
            Task ClearOrResetAsync() => comp.InvokeAsync(() => reset ? autocomplete.ResetAsync() : autocomplete.ClearAsync());

            await comp.InvokeAsync(autocomplete.ToggleMenuAsync);
            await ClearOrResetAsync();

            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
            autocomplete.ReadValue.Should().BeNull();
            autocomplete.ReadText.Should().BeEmpty();

            await comp.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Should().ContainSingle());
            await ClearOrResetAsync();

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            autocomplete.ReadValue.Should().BeNull();
            autocomplete.ReadText.Should().BeEmpty();
        }

        /// <summary>
        /// Calling ClearAsync from ValueChanged leaves the text empty instead of showing the clicked result.
        /// </summary>
        [Test]
        public async Task Autocomplete_ClearAsyncFromValueChanged_LeavesTextEmpty()
        {
            var comp = Context.Render<AutocompleteDisabledItemsTest>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>();
            await autocomplete.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ValueChanged, () => autocomplete.Instance.ClearAsync()));

            await autocomplete.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => autocomplete.Instance.Open.Should().BeTrue());

            await ListItem(comp, "Alaska").ClickAsync();

            await comp.WaitForAssertionAsync(() => autocomplete.Instance.ReadText.Should().BeEmpty());
        }

        /// <summary>
        /// Generates every combination of the parameters that ResetAsync must ignore.
        /// </summary>
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
        /// ResetAsync leaves the menu closed, the text empty and the value null, whatever the coercion settings, without searching.
        /// </summary>
        [TestCaseSource(nameof(ResetAsyncParameters))]
        public async Task ResetAsync_WithoutValue_SoTextEmptyAndValueNull(bool resetValueOnEmptyText, bool coerceText, bool coerceValue, bool immediate)
        {
            var comp = Context.Render<AutocompleteStates>(parameters =>
            {
                parameters.Add(a => a.ResetValueOnEmptyText, resetValueOnEmptyText);
                parameters.Add(a => a.DebounceInterval, 0);
                parameters.Add(a => a.CoerceText, coerceText);
                parameters.Add(a => a.CoerceValue, coerceValue);
                parameters.Add(a => a.Immediate, immediate);
            });
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;
            autocomplete.ReadValue.Should().BeNull();
            autocomplete.ReadText.Should().BeNull();

            await comp.InvokeAsync(autocomplete.ResetAsync);

            autocomplete.Open.Should().BeFalse();
            autocomplete.ReadValue.Should().BeNull();
            autocomplete.ReadText.Should().BeEmpty();
            comp.Instance.SearchFuncCallCount.Should().Be(0);
        }

        /// <summary>
        /// ResetAsync clears an existing value and leaves the menu closed and the text empty, whatever the coercion settings, without searching.
        /// </summary>
        [TestCaseSource(nameof(ResetAsyncParameters))]
        public async Task ResetAsync_WithValue_SoTextEmptyAndValueNull(bool resetValueOnEmptyText, bool coerceText, bool coerceValue, bool immediate)
        {
            var comp = Context.Render<AutocompleteStates>(parameters =>
            {
                parameters.Add(a => a.Value, "Idaho");
                parameters.Add(a => a.ResetValueOnEmptyText, resetValueOnEmptyText);
                parameters.Add(a => a.DebounceInterval, 0);
                parameters.Add(a => a.CoerceText, coerceText);
                parameters.Add(a => a.CoerceValue, coerceValue);
                parameters.Add(a => a.Immediate, immediate);
            });
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;
            autocomplete.ReadValue.Should().Be("Idaho");
            autocomplete.ReadText.Should().Be("Idaho");

            await comp.InvokeAsync(autocomplete.ResetAsync);

            autocomplete.Open.Should().BeFalse();
            autocomplete.ReadValue.Should().BeNull();
            autocomplete.ReadText.Should().BeEmpty();
            comp.Instance.SearchFuncCallCount.Should().Be(0);
        }

        /// <summary>
        /// Keeps disabled-item state aligned when unlimited results are replaced.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Update_DisabledItems_When_UnlimitedResultsChange()
        {
            var firstResults = new[] { "Enabled 1", "Disabled 1", "Enabled 2" };
            var secondResults = new[] { "Disabled 2", "Enabled 3" };
            var provider = Context.Render<MudPopoverProvider>();
            var autocomplete = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.MaxItems, null)
                .Add(x => x.ItemDisabledFunc, item => item.StartsWith("Disabled", StringComparison.Ordinal))
                .Add(x => x.SearchFunc, (value, _) => Task.FromResult<IEnumerable<string>>(
                    value == "empty" ? [] : value == "second" ? secondResults : firstResults)));

            await autocomplete.Find("input").InputAsync("first");
            await provider.WaitForAssertionAsync(() =>
            {
                provider.FindComponents<MudListItem<string>>().Count.Should().Be(3);
                provider.FindComponents<MudListItem<string>>().Count(x => x.Instance.Disabled).Should().Be(1);
            });

            await autocomplete.Find("input").InputAsync("empty");
            await provider.WaitForAssertionAsync(() => provider.FindComponents<MudListItem<string>>().Should().BeEmpty());

            await autocomplete.Find("input").InputAsync("second");
            await provider.WaitForAssertionAsync(() =>
            {
                provider.FindComponents<MudListItem<string>>().Count.Should().Be(2);
                provider.FindComponents<MudListItem<string>>().Count(x => x.Instance.Disabled).Should().Be(1);
                provider.FindComponents<MudListItem<string>>().Single(x => x.Markup.Contains("Disabled 2")).Instance.Disabled.Should().BeTrue();
                provider.FindComponents<MudListItem<string>>().Single(x => x.Markup.Contains("Enabled 3")).Instance.Disabled.Should().BeFalse();
            });

            await autocomplete.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await autocomplete.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });
            await autocomplete.WaitForAssertionAsync(() => autocomplete.Instance.ReadValue.Should().Be("Enabled 3"));
        }

        /// <summary>
        /// Disabled results can be neither clicked nor reached with the arrow keys.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Not_Select_Disabled_Item()
        {
            // States containing an 'o', such as American Samoa and Arizona, are disabled.
            var comp = Context.Render<AutocompleteDisabledItemsTest>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>();
            await autocomplete.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => autocomplete.Instance.Open.Should().BeTrue());

            await ListItem(comp, "American Samoa").ClickAsync();
            autocomplete.Instance.Value.Should().BeNull("a disabled result cannot be clicked");

            await ListItem(comp, "Alaska").ClickAsync();
            await comp.WaitForAssertionAsync(() => autocomplete.Instance.Value.Should().Be("Alaska"));

            // Emptying the text lists every state again, with the first one highlighted.
            await autocomplete.Find("input").InputAsync(string.Empty);
            await comp.WaitForAssertionAsync(() => HighlightedText(comp).Should().StartWith("Alabama"));

            await autocomplete.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await autocomplete.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await comp.WaitForAssertionAsync(() => HighlightedText(comp).Should().StartWith("Arkansas", "the disabled American Samoa and Arizona are skipped"));

            await autocomplete.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });
            autocomplete.Instance.Value.Should().Be("Arkansas");
        }

        /// <summary>
        /// Switching the parent's bound object shows that object's value, and a selection is written back to the object it came from.
        /// </summary>
        [Test]
        public async Task Autocomplete_BoundValueChange_UpdatesText()
        {
            var comp = Context.Render<AutocompleteChangeBoundObjectTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            IElement Input() => autocompleteComponent.Find("input");

            async Task ToggleBoundObjectAsync(string expected)
            {
                await comp.Find(".toggle-value-button").ClickAsync();
                await comp.WaitForAssertionAsync(() => Input().GetAttribute("value").Should().Be(expected));
                autocomplete.ReadValue.Should().Be(expected);
                autocomplete.ReadText.Should().Be(expected);
            }

            Input().GetAttribute("value").Should().Be("Florida");
            await ToggleBoundObjectAsync("Georgia");

            await Input().InputAsync("Alabam");
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());
            await Input().KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });
            await comp.WaitForAssertionAsync(() => autocomplete.ReadValue.Should().Be("Alabama"));

            await ToggleBoundObjectAsync("Florida");
            await ToggleBoundObjectAsync("Alabama");
        }

        /// <summary>
        /// Each opening key opens a closed menu without changing the value.
        /// </summary>
        [TestCase("ArrowDown", false)]
        [TestCase("ArrowUp", false)]
        [TestCase("Enter", true)]
        [TestCase("NumpadEnter", true)]
        public async Task Autocomplete_Key_OpensClosedMenu(string key, bool onKeyUp)
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.SearchFunc, SearchStatesAsync));

            if (onKeyUp)
            {
                await comp.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = key });
            }
            else
            {
                await comp.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = key });
            }

            comp.Instance.Open.Should().BeTrue();
            comp.Instance.ReadValue.Should().BeNull();
        }

        /// <summary>
        /// Each closing key closes an open menu without selecting the highlighted result.
        /// </summary>
        [TestCase("Escape", false, true)]
        [TestCase("ArrowUp", true, false)]
        [TestCase("Tab", false, false)]
        public async Task Autocomplete_Key_ClosesOpenMenuWithoutSelecting(string key, bool altKey, bool onKeyUp)
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.SearchFunc, SearchStatesAsync));
            await comp.InvokeAsync(comp.Instance.OpenMenuAsync);
            comp.Instance.Open.Should().BeTrue();

            var args = new KeyboardEventArgs { Key = key, AltKey = altKey };
            if (onKeyUp)
            {
                await comp.Find("input").KeyUpAsync(args);
            }
            else
            {
                await comp.Find("input").KeyDownAsync(args);
            }

            comp.Instance.Open.Should().BeFalse();
            comp.Instance.ReadValue.Should().BeNull();
        }

        /// <summary>
        /// Ctrl+Shift+Backspace resets the value and the text.
        /// </summary>
        [Test]
        public async Task Autocomplete_CtrlShiftBackspace_ResetsValue()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.Value, "Alabama")
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.SearchFunc, SearchStatesAsync));

            await comp.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Backspace", CtrlKey = true, ShiftKey = true });

            comp.Instance.ReadValue.Should().BeNull();
            comp.Instance.ReadText.Should().BeEmpty();
        }

        /// <summary>
        /// A SearchFunc that completes synchronously still opens the menu with every result.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Support_Sync_Search()
        {
            var root = Context.Render<AutocompleteSyncTest>();
            var popoverProvider = root.FindComponent<MudPopoverProvider>();
            var autocomplete = root.FindComponent<MudAutocomplete<string>>();
            var popover = autocomplete.FindComponent<MudPopover>();
            popover.Instance.Open.Should().BeFalse();

            await autocomplete.Find("div.mud-input-control").FocusAsync();

            await popoverProvider.WaitForAssertionAsync(() =>
            {
                popover.Instance.Open.Should().BeTrue();
                popoverProvider.FindComponents<MudListItem<string>>().Should().HaveCount(AutocompleteSyncTest.Items.Length);
            });
        }

        /// <summary>
        /// Changing AdornmentIcon updates the icon without reopening the autocomplete.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_ChangeAdornmentIcon()
        {
            var comp = Context.Render<AutocompleteAdornmentChange>(parameters => parameters.Add(x => x.Icon, Icons.Material.Filled.Abc));
            var markupBefore = comp.Find("svg.mud-icon-root").InnerHtml;

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Icon, Icons.Material.Filled.Remove));

            comp.Find("svg.mud-icon-root").InnerHtml.Should().NotBe(markupBefore);
        }

        /// <summary>
        /// No progress indicator shows while searching unless ShowProgressIndicator is set.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_NotIndicateLoadingByDefault()
        {
            var (searchStarted, searchCompletion, searchFunc) = CreateControlledSearch();
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SearchFunc, searchFunc));

            var inputTask = autocompleteComponent.Find("input").InputAsync("Calif");
            await searchStarted.Task.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress"));
            comp.FindAll("div.mud-autocomplete .progress-indicator-circular").Should().BeEmpty();

            searchCompletion.SetResult(["California"]);
            await inputTask;
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
        }

        /// <summary>
        /// ShowProgressIndicator shows a circular indicator while searching and removes it when the results arrive.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_IndicateLoadingWithCircularProgressIndicator()
        {
            var (searchStarted, searchCompletion, searchFunc) = CreateControlledSearch();
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.SearchFunc, searchFunc)
                .Add(x => x.ShowProgressIndicator, true));
            comp.FindAll(".progress-indicator-circular").Should().BeEmpty();

            var inputTask = autocompleteComponent.Find("input").InputAsync("Calif");
            await searchStarted.Task.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").ClassList.Should().Contain("mud-autocomplete--with-progress"));
            comp.FindAll("div.mud-autocomplete .progress-indicator-circular").Should().ContainSingle();

            searchCompletion.SetResult(["California"]);
            await inputTask;
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress");
            comp.FindAll("div.mud-autocomplete .progress-indicator-circular").Should().BeEmpty();
        }

        /// <summary>
        /// The circular progress indicator makes room for an end adornment while searching.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_IndicateLoadingWithCircularProgressIndicatorAndAdornmentAdjustment()
        {
            var (searchStarted, searchCompletion, searchFunc) = CreateControlledSearch();
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.SearchFunc, searchFunc)
                .Add(x => x.ShowProgressIndicator, true)
                .Add(x => x.AdornmentIcon, Icons.Material.Filled.Info)
                .Add(x => x.Adornment, Adornment.End));

            var inputTask = autocompleteComponent.Find("input").InputAsync("Calif");
            await searchStarted.Task.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            await comp.WaitForAssertionAsync(() => comp.Find("div.progress-indicator-circular").ClassList.Should().Contain("progress-indicator-circular--with-adornment"));

            searchCompletion.SetResult(["California"]);
            await inputTask;
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            comp.FindAll("div.mud-autocomplete .progress-indicator-circular").Should().BeEmpty();
        }

        /// <summary>
        /// ProgressIndicatorTemplate replaces the circular indicator while searching.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_IndicateLoadingWithCustomProgressIndicator()
        {
            var (searchStarted, searchCompletion, searchFunc) = CreateControlledSearch();
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.SearchFunc, searchFunc)
                .Add(x => x.ShowProgressIndicator, true)
                .Add(p => p.ProgressIndicatorTemplate, builder => builder.AddContent(0, "Loading...")));
            comp.Markup.Should().NotContain("Loading...");

            var inputTask = autocompleteComponent.Find("input").InputAsync("Calif");
            await searchStarted.Task.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").TextContent.Should().Contain("Loading..."));
            comp.Find("div.mud-autocomplete").ClassList.Should().Contain("mud-autocomplete--with-progress");

            searchCompletion.SetResult(["California"]);
            await inputTask;
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-autocomplete").TextContent.Should().NotContain("Loading..."));
            comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress");
        }

        /// <summary>
        /// ProgressIndicatorInPopoverTemplate opens the menu and shows inside it while searching.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_IndicateLoadingWithProgressIndicatorInsidePopover()
        {
            var (searchStarted, searchCompletion, searchFunc) = CreateControlledSearch();
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.SearchFunc, searchFunc)
                .Add(x => x.ShowProgressIndicator, true)
                .Add(p => p.ProgressIndicatorInPopoverTemplate, builder => builder.AddContent(0, "Loading...")));
            comp.Markup.Should().NotContain("Loading...");

            var inputTask = autocompleteComponent.Find("input").InputAsync("Calif");
            await searchStarted.Task.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").TextContent.Should().Contain("Loading..."));
            comp.Find("div.mud-autocomplete").ClassList.Should().Contain("mud-autocomplete--with-progress");

            searchCompletion.SetResult(["California"]);
            await inputTask;
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").TextContent.Should().NotContain("Loading..."));
            comp.Find("div.mud-autocomplete").ClassList.Should().NotContain("mud-autocomplete--with-progress");
        }

        /// <summary>
        /// A new search cancels the pending one, and only the new search's results are shown.
        /// </summary>
        [Test]
        public async Task Autocomplete_NewSearch_CancelsPendingSearch()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            var tokens = new List<CancellationToken>();
            var first = new TaskCompletionSource<IEnumerable<string>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var second = new TaskCompletionSource<IEnumerable<string>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.DebounceInterval, 100)
                .Add(x => x.SearchFunc, (_, token) =>
                {
                    tokens.Add(token);
                    return tokens.Count == 1 ? first.Task : second.Task;
                }));

            await comp.Find("input").InputAsync("Foo");
            timeProvider.Advance(TimeSpan.FromMilliseconds(100));
            await comp.WaitForAssertionAsync(() => tokens.Should().ContainSingle());
            tokens[0].IsCancellationRequested.Should().BeFalse();

            await comp.Find("input").InputAsync("Bar");
            timeProvider.Advance(TimeSpan.FromMilliseconds(100));
            await comp.WaitForAssertionAsync(() => tokens.Should().HaveCount(2));
            tokens[0].IsCancellationRequested.Should().BeTrue();

            first.SetCanceled();
            second.SetResult(["Bar"]);

            await provider.WaitForAssertionAsync(() => provider.FindAll("div.mud-list-item").Select(x => x.TextContent.Trim()).Should().Equal("Bar"));
        }

        /// <summary>
        /// FullWidth adds the full-width class to the autocomplete.
        /// </summary>
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

        /// <summary>
        /// Typing raises TextChanged with the typed text.
        /// </summary>
        [Test]
        public async Task Autocomplete_Typing_RaisesTextChanged()
        {
            var texts = new List<string>();
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.TextChanged, texts.Add));

            await comp.Find("input").InputAsync("testText");

            texts.Should().Equal("testText");
        }

        /// <summary>
        /// With Strict off, reopening the menu lists results around the selected value and highlights only that value, whether it is formatted by ToStringFunc or ToString.
        /// </summary>
        [TestCase(0, TestName = "AutocompleteStrictFalse(ToStringFunc)")]
        [TestCase(1, TestName = "AutocompleteStrictFalse(ToString)")]
        public async Task AutocompleteStrictFalse(int index)
        {
            var comp = Context.Render<AutocompleteStrictFalseTest>();
            var autocompleteComponent = comp.FindComponents<MudAutocomplete<AutocompleteStrictFalseTest.State>>()[index];
            var autocomplete = autocompleteComponent.Instance;
            IElement Popover() => comp.FindAll("div.mud-popover")[index];
            IReadOnlyList<IElement> Results() => Popover().QuerySelectorAll("div.mud-list-item").ToList();

            async Task SelectAndReopenAsync(string state)
            {
                await autocompleteComponent.Find("input").InputAsync(state);
                await comp.WaitForAssertionAsync(() => Popover().ClassList.Should().Contain("mud-popover-open"));
                await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });
                autocomplete.ReadText.Should().Be(state);
                autocomplete.ReadValue.StateName.Should().Be(state);

                // Enter closes the menu, so open it again to see the unfiltered list.
                await comp.InvokeAsync(autocomplete.OpenMenuAsync);
                await comp.WaitForAssertionAsync(() => Popover().ClassList.Should().Contain("mud-popover-open"));
            }

            await SelectAndReopenAsync("California");
            Results().Should().HaveCount(10);
            Results().ToList().FindIndex(x => x.TextContent.Contains("California")).Should().Be(5, "the selected value is centered");
            await comp.WaitForAssertionAsync(() => Results().Single(x => x.TextContent.Contains("California")).ClassList.Should().Contain("mud-selected-item"));

            await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Escape" });

            await SelectAndReopenAsync("Virginia");
            Results().Should().HaveCount(10);
            Results().Count(x => x.TextContent.Contains("Virginia")).Should().Be(2, "Virginia and West Virginia are both listed");
            Results().ToList().FindIndex(x => x.TextContent.Contains("Virginia")).Should().Be(5);
            Results().Count(x => x.ClassList.Contains("mud-selected-item")).Should().Be(1);
        }

        /// <summary>
        /// With Strict off and an enum whose default is a valid item, only the selected item is highlighted, not also the default-valued one (#13358).
        /// </summary>
        [Test]
        public async Task AutocompleteStrictFalse_ValueType_HighlightsOnlySelectedItem()
        {
            var comp = Context.Render<AutocompleteEnumStrictFalseTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<AutocompleteEnumStrictFalseTest.TestEnum>>();
            var autocomplete = autocompleteComponent.Instance;

            // Select Third (2), which is not the default First (0).
            await autocompleteComponent.Find("input").InputAsync("Third");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });
            await comp.WaitForAssertionAsync(() => autocomplete.Value.Should().Be(AutocompleteEnumStrictFalseTest.TestEnum.Third));

            // Reopening lists every item with Third selected.
            await comp.InvokeAsync(autocomplete.OpenMenuAsync);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item.mud-selected-item")
                .Should().ContainSingle().Which.TextContent.Should().Contain("Third"));
        }

        /// <summary>
        /// When the selected value drops out of refreshed results, no row is highlighted, not even the default-valued one (#13358).
        /// </summary>
        [Test]
        public async Task AutocompleteStrictFalse_ValueType_SelectedItemRemovedOnRefresh_HighlightsNothing()
        {
            var comp = Context.Render<AutocompleteEnumStrictFalseTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<AutocompleteEnumStrictFalseTest.TestEnum>>();
            var autocomplete = autocompleteComponent.Instance;

            await autocompleteComponent.Find("input").InputAsync("Third");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });
            await comp.WaitForAssertionAsync(() => autocomplete.Value.Should().Be(AutocompleteEnumStrictFalseTest.TestEnum.Third));

            // Drop the selected item from the source, so the refreshed results no longer contain it while First (0) remains.
            comp.Instance.Source.Remove(AutocompleteEnumStrictFalseTest.TestEnum.Third);

            await comp.InvokeAsync(autocomplete.OpenMenuAsync);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await comp.WaitForAssertionAsync(() =>
            {
                comp.FindAll("div.mud-list-item").Should().HaveCount(4, "First, Second, Fourth and Fifth remain");
                comp.FindAll("div.mud-list-item.mud-selected-item").Should().BeEmpty();
            });
        }

        /// <summary>
        /// A typed MudListItem in BeforeItemsTemplate inherits the list's Dense setting and is not selected just because its value is default(T).
        /// </summary>
        [Test]
        public async Task Autocomplete_TypedListItemInBeforeItemsTemplate_InheritsListCascade()
        {
            var comp = Context.Render<AutocompleteBeforeItemsListItemTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<AutocompleteBeforeItemsListItemTest.Season>>();

            await comp.InvokeAsync(autocompleteComponent.Instance.OpenMenuAsync);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await comp.WaitForAssertionAsync(() =>
            {
                var beforeItem = comp.Find("div.before-item");
                beforeItem.ClassList.Should().Contain("mud-list-item-dense");
                beforeItem.ClassList.Should().NotContain("mud-selected-item");
            });
        }

        /// <summary>
        /// Typing without a SearchFunc does not throw and opens no results.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Not_Throw_When_SearchFunc_Is_Null()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters.Add(x => x.DebounceInterval, 0));

            await comp.Find("input").InputAsync("Foo");

            comp.Instance.Open.Should().BeFalse();
        }

        /// <summary>
        /// Key presses on the input raise OnKeyDown and OnKeyUp.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Raise_KeyDown_KeyUp_Event()
        {
            var events = new List<string>();
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.OnKeyDown, args => events.Add($"down {args.Key}"))
                .Add(p => p.OnKeyUp, args => events.Add($"up {args.Key}")));

            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "a" });
            await comp.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "a" });

            events.Should().Equal("down a", "up a");
        }

        /// <summary>
        /// A key press that re-renders the autocomplete keeps the typed text when the value is unchanged.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_PreserveText_OnKeyRerender_WhenValueIsUnchanged()
        {
            var comp = Context.Render<AutocompleteKeyDownRerenderTextTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<AutocompleteKeyDownRerenderTextTest.User>>();

            await autocompleteComponent.Find("input").InputAsync("U");

            await comp.WaitForAssertionAsync(() =>
            {
                autocompleteComponent.Instance.ReadText.Should().Be("U");
                autocompleteComponent.Find("input").GetAttribute("value").Should().Be("U");
            });

            await autocompleteComponent.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "s", Type = "keydown" });

            await comp.WaitForAssertionAsync(() =>
            {
                autocompleteComponent.Instance.ReadText.Should().Be("U");
                autocompleteComponent.Find("input").GetAttribute("value").Should().Be("U");
            });
        }

        /// <summary>
        /// With Strict off, reopening highlights the selected result even when a disabled result is listed (#6412).
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Highlight_Selected_Item_After_Disabled()
        {
            var comp = Context.Render<AutocompleteStrictFalseSelectedHighlight>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;

            await autocompleteComponent.Find("input").InputAsync("peach");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await autocompleteComponent.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });
            autocomplete.ReadValue.Should().Be("peach");

            await comp.InvokeAsync(autocomplete.OpenMenuAsync);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            comp.Find("div.mud-list-item-disabled").TextContent.Should().Contain("carrot");
            comp.Find("div.mud-list-item.mud-selected-item").TextContent.Should().Contain("peach");
        }

        /// <summary>
        /// Calling ResetAsync from ValueChanged leaves the text empty instead of showing the clicked result (#6475).
        /// </summary>
        [Test]
        public async Task Autocomplete_ResetAsyncFromValueChanged_LeavesTextEmpty()
        {
            var comp = Context.Render<AutocompleteResetTest>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await ListItem(comp, "Test").ClickAsync();

            await comp.WaitForAssertionAsync(() => autocomplete.ReadText.Should().BeEmpty());
        }

        /// <summary>
        /// BeforeItemsTemplate and AfterItemsTemplate render around the results.
        /// </summary>
        [Test]
        public async Task Autocomplete_BeforeAndAfterItemsTemplates_ShownWithResults()
        {
            var comp = Context.Render<AutocompleteListPartsTest>();

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            comp.Find("div.mud-popover .mud-autocomplete-before-items").TextContent.Should().Contain("StartList_Content");
            comp.Find("div.mud-popover .mud-autocomplete-after-items").TextContent.Should().Contain("EndList_Content");
        }

        /// <summary>
        /// BeforeItemsTemplate alone does not open the menu when the search returns nothing.
        /// </summary>
        [Test]
        public async Task Autocomplete_BeforeItemsTemplate_DoesNotOpenEmptyResults()
        {
            var comp = Context.Render<AutocompleteListStartRendersTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var returnedItemsCount = -1;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.ReturnedItemsCountChanged, count => returnedItemsCount = count));

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => returnedItemsCount.Should().Be(0));

            autocompleteComponent.Instance.Open.Should().BeFalse();
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
        }

        /// <summary>
        /// AfterItemsTemplate alone does not open the menu when the search returns nothing.
        /// </summary>
        [Test]
        public async Task Autocomplete_AfterItemsTemplate_DoesNotOpenEmptyResults()
        {
            var comp = Context.Render<AutocompleteListEndRendersTest>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var returnedItemsCount = -1;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters.Add(a => a.ReturnedItemsCountChanged, count => returnedItemsCount = count));

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => returnedItemsCount.Should().Be(0));

            autocompleteComponent.Instance.Open.Should().BeFalse();
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
        }

        /// <summary>
        /// The menu closes instead of staying invisibly open when a search returns no items and no NoItemsTemplate is set (#13360).
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_StayClosed_When_SearchReturnsNoItems()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            await comp.Find("input").InputAsync("Calif");
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeTrue());

            await comp.Find("input").InputAsync("Califxyz");
            await comp.WaitForAssertionAsync(() => autocomplete.Open.Should().BeFalse());
            comp.FindAll(".mud-overlay").Should().BeEmpty();
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
        }

        /// <summary>
        /// Clicking the clear button after a search that returned no items does not open the menu (#13360).
        /// </summary>
        [Test]
        public async Task Autocomplete_Clear_AfterEmptySearch_Should_NotOpenMenu()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.Clearable, true)
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.SearchFunc, SearchStatesAsync));

            await comp.Find("input").InputAsync("xyz");
            comp.Instance.Open.Should().BeFalse();

            var clearButton = comp.Find("button.mud-input-clear-button");
            await clearButton.MouseDownAsync(new MouseEventArgs());
            await clearButton.ClickAsync(new MouseEventArgs());

            comp.Instance.Open.Should().BeFalse();
            comp.Instance.ReadText.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Pressing Enter while the menu is closed after an empty search still coerces the value when CoerceValue is on (#13360).
        /// </summary>
        [Test]
        public async Task Autocomplete_Enter_AfterEmptySearch_Should_CoerceValue()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocompleteComponent = comp.FindComponent<MudAutocomplete<string>>();
            var autocomplete = autocompleteComponent.Instance;
            await autocompleteComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.CoerceValue, true)
                .Add(x => x.Immediate, false));

            await comp.Find("input").InputAsync("Austria");
            autocomplete.Open.Should().BeFalse();

            await comp.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

            await comp.WaitForAssertionAsync(() => autocomplete.ReadValue.Should().Be("Austria"));
        }

        /// <summary>
        /// Each class parameter lands on the element it styles: the input, the popover, the list and every result.
        /// </summary>
        [Test]
        public async Task Autocomplete_ClassParameters_ApplyToTheirElements()
        {
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.InputClass, "my-input-class")
                .Add(x => x.PopoverClass, "my-popover-class")
                .Add(x => x.ListClass, "my-list-class")
                .Add(x => x.ListItemClass, "my-list-item-class")
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.SearchFunc, SearchStatesAsync));

            comp.Find(".mud-select-input").ClassList.Should().Contain("my-input-class");
            provider.Find("div.mud-popover").ClassList.Should().Contain("my-popover-class");

            await comp.InvokeAsync(comp.Instance.OpenMenuAsync);

            await provider.WaitForAssertionAsync(() => provider.Find(".mud-list").ClassList.Should().Contain("my-list-class"));
            provider.FindAll("div.mud-list-item").Should().HaveCount(States.Length)
                .And.OnlyContain(item => item.ClassList.Contains("my-list-item-class"));
        }

        /// <summary>
        /// A click opens the menu even when OpenOnFocus is off.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_OpenMenuOnFocus_AlwaysOnClick()
        {
            var comp = Context.Render<AutocompleteFocusTest>(parameters => parameters.Add(a => a.OpenOnFocus, false));

            // The browser focuses the input before the mousedown lands.
            await comp.Find("div.mud-input-control").FocusAsync();
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");

            await comp.Find("input.mud-input-root").MouseDownAsync(new MouseEventArgs());

            // OpenOnFocus was added later to opt in to v6 behavior, so it does not apply to clicks.
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
        }

        /// <summary>
        /// ReturnedItemsCountChanged reports how many results each search found.
        /// </summary>
        [Test]
        public async Task Autocomplete_ReturnedItemsCount_Should_Be_Accurate()
        {
            static Task<IEnumerable<string>> Search(string value, CancellationToken token)
            {
                var values = new[] { "Lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit" };
                return Task.FromResult(values.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
            }

            int? count = null;
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.SearchFunc, Search)
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.ReturnedItemsCountChanged, value => count = value));

            await comp.Find("input").InputAsync("Lorem");
            count.Should().Be(1);

            await comp.Find("input").InputAsync("ip");
            count.Should().Be(2);

            await comp.Find("input").InputAsync("xyz");
            count.Should().Be(0);
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
            comp.Find("label").GetAttribute("for").Should().Be(comp.Find("input").Id);
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
            comp.Find("label").GetAttribute("for").Should().Be(expectedId);
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
            comp.Find("label").GetAttribute("for").Should().Be(expectedId);
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
        /// Clicking a result closes the menu and does not reopen it.
        /// </summary>
        [Test]
        public async Task Autocomplete_SelectingOption_ShouldNot_ReopenList()
        {
            var comp = Context.Render<AutocompleteTest1>();

            await comp.Find("div.mud-input-control").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await comp.Find("div.mud-list-item").ClickAsync();

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
        }

        /// <summary>
        /// A read-only autocomplete opens neither on focus nor on click.
        /// </summary>
        [Test]
        public async Task Autocomplete_User_ShouldNot_OpenMenu_InReadOnlyMode()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.ReadOnly, true)
                .Add(p => p.OpenOnFocus, true)
                .Add(p => p.SearchFunc, SearchStatesAsync));

            await comp.Find("div.mud-input-control").FocusAsync();
            comp.Instance.Open.Should().BeFalse();

            await comp.Find("div.mud-input-control").MouseDownAsync(new MouseEventArgs());
            comp.Instance.Open.Should().BeFalse();
        }

        /// <summary>
        /// A disabled autocomplete opens neither on focus nor on click.
        /// </summary>
        [Test]
        public async Task Autocomplete_User_ShouldNot_OpenMenu_InDisabledMode()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.Disabled, true)
                .Add(p => p.OpenOnFocus, true)
                .Add(p => p.SearchFunc, SearchStatesAsync));

            await comp.Find("div.mud-input-control").FocusAsync();
            comp.Instance.Open.Should().BeFalse();

            await comp.Find("div.mud-input-control").MouseDownAsync(new MouseEventArgs());
            comp.Instance.Open.Should().BeFalse();
        }

        /// <summary>
        /// ItemDisabledTemplate and ItemSelectedTemplate apply even without an ItemTemplate.
        /// </summary>
        [Test]
        public async Task AutocompleteItemTemplateDisplay()
        {
            var comp = Context.Render<AutocompleteItemTemplateDisplayTest>();

            // Any state with an 'l' is disabled, and American Samoa is the selected state.
            await comp.Find("input").InputAsync("a");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            var results = comp.FindAll("div.mud-list-item");
            results[0].TextContent.Should().Contain("Alabama Disabled State");
            results[2].TextContent.Should().Contain("American Samoa Selected State");
        }

        /// <summary>
        /// A conversion error renders its message and marks the input invalid, described by the error.
        /// </summary>
        [Test]
        public void Autocomplete_ConversionError_RendersMessageAndMarksInputInvalid()
        {
            var comp = Context.Render<MudAutocomplete<int>>(parameters => parameters
                .Add(p => p.ErrorId, "error-id")
                .Add(p => p.CoerceValue, true)
                .Add(p => p.Converter, new DummyErrorConverter())
                .Add(p => p.Text, "not a number"));

            comp.Instance.ConversionErrorMessage.Should().NotBeNullOrEmpty();
            comp.Find("#error-id").InnerHtml.Should().Be(comp.Instance.ConversionErrorMessage);
            comp.Find("input").GetAttribute("aria-describedby").Should().Be("error-id");
            comp.Find("input").GetAttribute("aria-invalid").Should().Be("true");
        }

        /// <summary>
        /// AdornmentAriaLabel names the adornment button at either end.
        /// </summary>
        [TestCase(Adornment.Start)]
        [TestCase(Adornment.End)]
        public void Autocomplete_AdornmentAriaLabel_NamesAdornmentButton(Adornment adornment)
        {
            var ariaLabel = "the aria label";
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.Adornment, adornment)
                .Add(p => p.AdornmentIcon, Icons.Material.Filled.Accessibility)
                .Add(p => p.AdornmentAriaLabel, ariaLabel));

            comp.Find(".mud-input-adornment-icon-button").GetAttribute("aria-label").Should().Be(ariaLabel);
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
        public async Task Autocomplete_AriaDescribedBy_ReferencesHelperAndError(
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

            if (withUserHelperId is false && withHelperText)
            {
                comp.FindAll($"#{inputId}-helper-text").Should().ContainSingle();
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

            comp.FindAll($"#{errorId}").Should().ContainSingle();
            comp.Find(inputSelector).GetAttribute("aria-describedby").Should().Be(secondExpectedAriaDescribedBy);
        }
#nullable disable

        /// <summary>
        /// The input turns off the browser's own autocomplete so it does not cover the results.
        /// </summary>
        [Test]
        public void Autocomplete_DisablesBrowserAutocomplete()
        {
            var comp = Context.Render<MudAutocomplete<string>>();

            comp.Find("input.mud-input-root").GetAttribute("autocomplete").Should().Be("off");
        }

        /// <summary>
        /// A caller-supplied autocomplete attribute overrides the default.
        /// </summary>
        [Test]
        public void Autocomplete_UserAttributes_OverrideBrowserAutocomplete()
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.UserAttributes, new() { ["autocomplete"] = "on" }));

            comp.Find("input.mud-input-root").GetAttribute("autocomplete").Should().Be("on");
        }

        /// <summary>
        /// With ResetValueOnEmptyText, emptying the text sets the value to null and searches again (#9495).
        /// </summary>
        [Test]
        public async Task ResetValueOnEmptyText_WhenTextCleared_ThenSetNullAndTriggerSearch()
        {
            var comp = Context.Render<AutocompleteResetValueOnEmptyText>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>().Instance;

            await comp.Find("input").InputAsync("");

            autocomplete.ReadValue.Should().BeNull();
            await comp.WaitForAssertionAsync(() => comp.Instance.SearchCount.Should().Be(1));
        }

        /// <summary>
        /// Arrow keys move the highlight to the last and first result without wrapping around.
        /// </summary>
        [Test]
        public async Task Autocomplete_ArrowKeys_MoveHighlightWithoutWrapping()
        {
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.SearchFunc, SearchStatesAsync));
            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            var lastIndex = States.Length - 1;
            await provider.WaitForAssertionAsync(() => provider.FindAll("div.mud-list-item").Should().HaveCount(States.Length));
            HighlightedIndex(provider).Should().Be(0);

            async Task PressAsync(string key, int times)
            {
                for (var i = 0; i < times; i++)
                {
                    await comp.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = key });
                }
            }

            await PressAsync("ArrowDown", lastIndex);
            HighlightedIndex(provider).Should().Be(lastIndex);
            await PressAsync("ArrowDown", 1);
            HighlightedIndex(provider).Should().Be(lastIndex, "ArrowDown does not wrap to the first result");

            await PressAsync("ArrowUp", lastIndex);
            HighlightedIndex(provider).Should().Be(0);
            await PressAsync("ArrowUp", 1);
            HighlightedIndex(provider).Should().Be(0, "ArrowUp does not wrap to the last result");
        }

        /// <summary>
        /// Arrowing past disabled results scrolls to the highlighted result even when its position exceeds the number of enabled results.
        /// </summary>
        [Test]
        public async Task Autocomplete_ArrowKeys_ScrollToResultAfterDisabledResults()
        {
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.ItemDisabledFunc, state => state is "Alaska" or "Arizona")
                .Add(x => x.SearchFunc, SearchStatesAsync));
            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await provider.WaitForAssertionAsync(() => provider.FindAll("div.mud-list-item").Should().HaveCount(States.Length));

            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });

            HighlightedText(provider).Should().Be("Arkansas");
            Context.JSInterop.Invocations["mudScrollManager.scrollToListItem"]
                .Select(invocation => (string)invocation.Arguments[0])
                .Should().ContainSingle(id => id.EndsWith("_item3"));
        }

        /// <summary>
        /// Clicking the adornment toggles the menu and focuses the input, unless OnAdornmentClick handles the click.
        /// </summary>
        [TestCase(false)]
        [TestCase(true)]
        public async Task Autocomplete_AdornmentClick_TogglesMenuUnlessHandled(bool attachDelegate)
        {
            var handled = 0;
            var comp = Context.Render<MudAutocomplete<string>>(parameters =>
            {
                parameters.Add(p => p.SearchFunc, SearchStatesAsync);
                if (attachDelegate)
                {
                    parameters.Add(p => p.OnAdornmentClick, () => handled++);
                }
            });

            await comp.Find(".mud-input-adornment-icon-button").ClickAsync();

            comp.Instance.Open.Should().Be(!attachDelegate);
            handled.Should().Be(attachDelegate ? 1 : 0);
            Context.JSInterop.Invocations["Blazor._internal.domWrapper.focus"].Should().HaveCount(attachDelegate ? 0 : 1);
        }

        /// <summary>
        /// OpenOnFocus decides whether focusing the input opens the menu.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public async Task Autocomplete_OpenOnFocus_ControlsOpeningOnFocus(bool openOnFocus)
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.OpenOnFocus, openOnFocus)
                .Add(p => p.SearchFunc, SearchStatesAsync));

            await comp.Find("input").FocusAsync();

            await comp.WaitForAssertionAsync(() => comp.Instance.Open.Should().Be(openOnFocus));
        }

        /// <summary>
        /// An autocomplete inside an open menu can be opened, closed by its overlay, and opened again without closing the menu.
        /// </summary>
        [Test]
        public async Task Autocomplete_OpenTwiceInMenu()
        {
            var comp = Context.Render<AutocompleteMenuCloseTest>();

            await comp.Find("#menu-open").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Should().ContainSingle("the menu is open"));

            await comp.Find(".autocomplete input").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Should().HaveCount(2, "the menu and the autocomplete are open"));

            await comp.Find(".mud-overlay").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Should().ContainSingle("only the menu is open"));

            await comp.Find(".autocomplete input").FocusAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Should().HaveCount(2, "the autocomplete opens again inside the menu"));
        }

        /// <summary>
        /// OpenChanged fires once for opening, even when the menu is opened twice.
        /// </summary>
        [Test]
        public async Task Autocomplete_OpenChanged_OpenMenu()
        {
            var comp = Context.Render<AutocompleteOpenChangedTest>();

            await comp.InvokeAsync(() => comp.Instance.Autocomplete.OpenMenuAsync());
            await comp.InvokeAsync(() => comp.Instance.Autocomplete.OpenMenuAsync());

            comp.Instance.OpenedCount.Should().Be(1);
        }

        /// <summary>
        /// Closing a menu that is already closed does not fire OpenChanged.
        /// </summary>
        [Test]
        public async Task Autocomplete_OpenChanged_CloseMenu()
        {
            var comp = Context.Render<AutocompleteOpenChangedTest>();

            await comp.InvokeAsync(() => comp.Instance.Autocomplete.CloseMenuAsync());

            comp.Instance.ClosedCount.Should().Be(0);
        }

        /// <summary>
        /// Opening then closing fires OpenChanged once for each.
        /// </summary>
        [Test]
        public async Task Autocomplete_OpenChanged_OpenClose()
        {
            var comp = Context.Render<AutocompleteOpenChangedTest>();

            await comp.InvokeAsync(() => comp.Instance.Autocomplete.OpenMenuAsync());
            await comp.InvokeAsync(() => comp.Instance.Autocomplete.CloseMenuAsync());

            comp.Instance.OpenedCount.Should().Be(1);
            comp.Instance.ClosedCount.Should().Be(1);
        }

        /// <summary>
        /// Selecting an option from code fires OpenChanged once for closing, without opening first.
        /// </summary>
        [Test]
        public async Task Autocomplete_OpenChanged_SelectOption()
        {
            var comp = Context.Render<AutocompleteOpenChangedTest>();

            await comp.InvokeAsync(() => comp.Instance.Autocomplete.SelectOptionAsync("Alabama"));

            comp.Instance.OpenedCount.Should().Be(0);
            comp.Instance.ClosedCount.Should().Be(1);
        }

        /// <summary>
        /// The clear button clears a closed autocomplete without opening or closing it.
        /// </summary>
        [Test]
        public async Task Autocomplete_OpenChanged_HandleClearButton()
        {
            var comp = Context.Render<AutocompleteHandleClearButtonAsyncTest>();

            // The clear button's mousedown focuses the autocomplete first.
            await comp.Find("button.mud-input-clear-button").MouseDownAsync();
            await comp.InvokeAsync(() => comp.Instance.Autocomplete.HandleClearButtonAsync(new()));

            comp.Instance.OpenedCount.Should().Be(0);
            comp.Instance.ClosedCount.Should().Be(0);
            comp.Instance.ClearCount.Should().Be(1);
        }

        /// <summary>
        /// The clear button keeps an open menu open.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Remain_Open_On_ClearButton_Usage()
        {
            var comp = Context.Render<AutocompleteHandleClearButtonAsyncTest>();
            await comp.InvokeAsync(() => comp.Instance.Autocomplete.OpenMenuAsync());

            await comp.Find("button.mud-input-clear-button").MouseDownAsync();
            await comp.InvokeAsync(() => comp.Instance.Autocomplete.HandleClearButtonAsync(new()));

            comp.Instance.OpenedCount.Should().Be(1);
            comp.Instance.ClosedCount.Should().Be(0);
            comp.Instance.ClearCount.Should().Be(1);
        }

        /// <summary>
        /// The clear button keeps a menu that was opened and closed again closed.
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Remain_Closed_On_ClearButton_Usage()
        {
            var comp = Context.Render<AutocompleteHandleClearButtonAsyncTest>();
            await comp.InvokeAsync(() => comp.Instance.Autocomplete.OpenMenuAsync());
            await comp.InvokeAsync(() => comp.Instance.Autocomplete.CloseMenuAsync());

            await comp.Find("button.mud-input-clear-button").MouseDownAsync();
            await comp.InvokeAsync(() => comp.Instance.Autocomplete.HandleClearButtonAsync(new()));

            comp.Instance.OpenedCount.Should().Be(1);
            comp.Instance.ClosedCount.Should().Be(1);
            comp.Instance.ClearCount.Should().Be(1);
        }

        /// <summary>
        /// Clicking the clear button without a preceding mousedown, as with element.click(), must not open the menu (follow-up to #13529).
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Remain_Closed_On_Programmatic_ClearButton_Click()
        {
            var comp = Context.Render<AutocompleteHandleClearButtonAsyncTest>(parameters => parameters
                .Add(x => x.DebounceInterval, 0));

            await comp.Find("button.mud-input-clear-button").ClickAsync(new());

            comp.Instance.Autocomplete.Open.Should().BeFalse();
            comp.Instance.OpenedCount.Should().Be(0);
            comp.Instance.ClosedCount.Should().Be(0);
            comp.Instance.ClearCount.Should().Be(1);
        }

        /// <summary>
        /// A full pointer interaction on the clear button, mousedown followed by click through the input's own handler, must not open a closed menu (follow-up to #13529).
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Remain_Closed_On_Pointer_ClearButton_Interaction()
        {
            var comp = Context.Render<AutocompleteHandleClearButtonAsyncTest>(parameters => parameters
                .Add(x => x.DebounceInterval, 0));

            var button = comp.Find("button.mud-input-clear-button");
            await button.MouseDownAsync(new());
            await button.ClickAsync(new());

            comp.Instance.Autocomplete.Open.Should().BeFalse();
            comp.Instance.OpenedCount.Should().Be(0);
            comp.Instance.ClosedCount.Should().Be(0);
            comp.Instance.ClearCount.Should().Be(1);
        }

        /// <summary>
        /// Enter activates the clear button on keydown and its keyup lands on the input focused by the clear handler; that stray keyup must not reopen the menu, while a subsequent full Enter keystroke still opens it (follow-up to #13529).
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Remain_Closed_On_Enter_ClearButton_Activation()
        {
            var comp = Context.Render<AutocompleteHandleClearButtonAsyncTest>(parameters => parameters
                .Add(x => x.DebounceInterval, 0));
            var input = comp.Find("input");

            // Enter on the focused clear button: the click fires on keydown with no mousedown,
            // then the keyup lands on the input because the clear handler moved focus there.
            await comp.Find("button.mud-input-clear-button").ClickAsync(new());
            await input.KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

            comp.Instance.Autocomplete.Open.Should().BeFalse();
            comp.Instance.OpenedCount.Should().Be(0);
            comp.Instance.ClearCount.Should().Be(1);

            // A genuine Enter keystroke on the input still opens the menu.
            await input.KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
            await input.KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

            comp.Instance.Autocomplete.Open.Should().BeTrue();
            comp.Instance.OpenedCount.Should().Be(1);
        }

        /// <summary>
        /// The stray Enter keyup must stay suppressed even when it arrives while an asynchronous clear callback is still pending (follow-up to #13529).
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Remain_Closed_On_Enter_ClearButton_Activation_With_Pending_Callback()
        {
            var comp = Context.Render<AutocompleteHandleClearButtonAsyncTest>(parameters => parameters
                .Add(x => x.DebounceInterval, 0)
                .Add(x => x.GateClear, true));

            // Enter fires the click on keydown; the gated callback keeps the clear transaction pending.
            var clickTask = comp.Find("button.mud-input-clear-button").ClickAsync(new());
            comp.Instance.ClearCount.Should().Be(1, "the click must have reached the gated clear callback");

            // The keyup lands on the input while the clear callback is still awaited.
            await comp.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

            comp.Instance.ClearGate.SetResult();
            await clickTask;

            comp.Instance.Autocomplete.Open.Should().BeFalse();
            comp.Instance.OpenedCount.Should().Be(0);
        }

        /// <summary>
        /// Enter on the clear button while the menu is open must not let the stray keyup select the highlighted item and restore the cleared value (follow-up to #13529).
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Not_Select_On_Enter_ClearButton_Activation_While_Open()
        {
            var comp = Context.Render<AutocompleteHandleClearButtonAsyncTest>(parameters => parameters
                .Add(x => x.DebounceInterval, 0));
            await comp.InvokeAsync(() => comp.Instance.Autocomplete.OpenMenuAsync());

            await comp.Find("button.mud-input-clear-button").ClickAsync(new());
            await comp.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

            comp.Instance.Autocomplete.Open.Should().BeTrue();
            comp.Instance.Autocomplete.Value.Should().BeNull();
            comp.Instance.ClearCount.Should().Be(1);
        }

        /// <summary>
        /// A full pointer interaction on the clear button while the menu is open must keep it open (#13528).
        /// </summary>
        [Test]
        public async Task Autocomplete_Should_Remain_Open_On_Pointer_ClearButton_Interaction()
        {
            var comp = Context.Render<AutocompleteHandleClearButtonAsyncTest>(parameters => parameters
                .Add(x => x.DebounceInterval, 0));
            await comp.InvokeAsync(() => comp.Instance.Autocomplete.OpenMenuAsync());

            var button = comp.Find("button.mud-input-clear-button");
            await button.MouseDownAsync(new());
            await button.ClickAsync(new());

            comp.Instance.Autocomplete.Open.Should().BeTrue();
            comp.Instance.OpenedCount.Should().Be(1);
            comp.Instance.ClosedCount.Should().Be(0);
            comp.Instance.ClearCount.Should().Be(1);
        }

        /// <summary>
        /// PopoverFixed renders the results with fixed positioning.
        /// </summary>
        [Test]
        public void Autocomplete_PopoverFixed_RendersFixedPopover()
        {
            var provider = Context.Render<MudPopoverProvider>();
            Context.Render<MudAutocomplete<string>>(parameters => parameters.Add(x => x.PopoverClass, "default-popover"));
            Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.PopoverClass, "fixed-popover")
                .Add(x => x.PopoverFixed, true));

            provider.Find(".default-popover").ClassList.Should().NotContain("mud-popover-fixed");
            provider.Find(".fixed-popover").ClassList.Should().Contain("mud-popover-fixed");
        }

        /// <summary>
        /// Without Modal, the overlay follows the global ModalOverlay option, and Modal overrides it.
        /// </summary>
        [TestCase(null, false, false)]
        [TestCase(null, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, false)]
        public async Task Autocomplete_Modal_FallsBackToGlobalModalOverlay(bool? modal, bool globalModalOverlay, bool expectModal)
        {
            Context.Services.Configure<PopoverOptions>(options => options.ModalOverlay = globalModalOverlay);
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.Modal, modal)
                .Add(x => x.SearchFunc, SearchStatesAsync));

            await comp.InvokeAsync(comp.Instance.OpenMenuAsync);

            var overlayStyle = provider.Find("div.mud-overlay").GetAttribute("style") ?? string.Empty;
            overlayStyle.Contains("pointer-events:none").Should().Be(!expectModal, "a modeless overlay lets pointer events through");
        }

        /// <summary>
        /// Creates a search function that stays pending until the test completes it, allowing deterministic loading-state assertions.
        /// </summary>
        private static (TaskCompletionSource<bool> Started, TaskCompletionSource<IEnumerable<string>> Completion, Func<string, CancellationToken, Task<IEnumerable<string>>> SearchFunc) CreateControlledSearch()
        {
            var searchStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var searchCompletion = new TaskCompletionSource<IEnumerable<string>>(TaskCreationOptions.RunContinuationsAsynchronously);

            Task<IEnumerable<string>> SearchFunc(string _, CancellationToken __)
            {
                searchStarted.TrySetResult(true);
                return searchCompletion.Task;
            }

            return (searchStarted, searchCompletion, SearchFunc);
        }

        private sealed class CoerceValueElement
        {
            public string Name { get; set; } = string.Empty;

            public override string ToString() => Name;
        }

        /// <summary>
        /// AutoFocus focuses the autocomplete without scrolling the page.
        /// </summary>
        [Test]
        public void AutoFocus_ShouldFocusWithoutScrolling()
        {
            Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.AutoFocus, true));

            var focusInvocation = Context.JSInterop.Invocations["Blazor._internal.domWrapper.focus"].Single();
            var preventScroll = focusInvocation.Arguments.OfType<bool>().Single();
            preventScroll.Should().BeTrue();
        }

        /// <summary>
        /// FocusAsync focuses the autocomplete and lets the browser scroll it into view.
        /// </summary>
        [Test]
        public async Task FocusAsync_ShouldFocusWithScrolling()
        {
            var comp = Context.Render<MudAutocomplete<string>>();

            await comp.InvokeAsync(async () => await comp.Instance.FocusAsync());

            var focusInvocation = Context.JSInterop.Invocations["Blazor._internal.domWrapper.focus"].Single();
            var preventScroll = focusInvocation.Arguments.OfType<bool>().Single();
            preventScroll.Should().BeFalse();
        }

        /// <summary>
        /// The input is a combobox that references the suggestion list and the highlighted option only while the list is open (#13214).
        /// </summary>
        [Test]
        public async Task Autocomplete_ExposesComboboxSemantics()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>();
            IElement Input() => autocomplete.Find("input");

            Input().GetAttribute("role").Should().Be("combobox");
            Input().GetAttribute("aria-autocomplete").Should().Be("list");
            Input().GetAttribute("aria-haspopup").Should().Be("listbox");
            Input().GetAttribute("aria-expanded").Should().Be("false");
            Input().HasAttribute("aria-controls").Should().BeFalse();
            Input().HasAttribute("aria-activedescendant").Should().BeFalse();

            await Input().KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));

            var list = comp.Find("div.mud-list");
            list.GetAttribute("role").Should().Be("listbox");
            list.Id.Should().NotBeNullOrEmpty();
            Input().GetAttribute("aria-expanded").Should().Be("true");
            Input().GetAttribute("aria-controls").Should().Be(list.Id);

            var highlighted = comp.Find("div.mud-list-item.mud-selected-item");
            highlighted.GetAttribute("role").Should().Be("option");
            highlighted.GetAttribute("aria-selected").Should().Be("true");
            Input().GetAttribute("aria-activedescendant").Should().Be(highlighted.Id);
            comp.FindAll("div.mud-list-item:not(.mud-selected-item)").Should().OnlyContain(item => item.GetAttribute("aria-selected") == "false");

            await Input().KeyUpAsync(new KeyboardEventArgs { Key = "Escape" });
            await comp.WaitForAssertionAsync(() => Input().GetAttribute("aria-expanded").Should().Be("false"));
            Input().HasAttribute("aria-controls").Should().BeFalse();
            Input().HasAttribute("aria-activedescendant").Should().BeFalse();
        }

        /// <summary>
        /// Caller-supplied combobox attributes take precedence over the generated fallbacks.
        /// </summary>
        [Test]
        public async Task Autocomplete_CallerAriaAttributes_Win()
        {
            var comp = Context.Render<AutocompleteTest1>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>();
            await autocomplete.SetParametersAndRenderAsync(parameters => parameters.AddUnmatched("aria-autocomplete", "both"));

            autocomplete.Find("input").GetAttribute("aria-autocomplete").Should().Be("both");
            autocomplete.Find("input").GetAttribute("role").Should().Be("combobox");
        }

        /// <summary>
        /// Case-colliding caller-supplied attributes do not throw during rendering.
        /// </summary>
        [Test]
        public void Autocomplete_CaseCollidingUserAttributes_DoNotThrow()
        {
            var userAttributes = new Dictionary<string, object>
            {
                ["ARIA-AUTOCOMPLETE"] = "both",
                ["aria-autocomplete"] = "list"
            };

            var action = () => Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.UserAttributes, userAttributes));

            action.Should().NotThrow();
        }

        /// <summary>
        /// Typing again within the debounce interval restarts it, so only the latest text is searched.
        /// </summary>
        [Test]
        public async Task Autocomplete_TypingWithinDebounceInterval_SearchesOnlyLatestText()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            var searches = new List<string>();
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(x => x.DebounceInterval, 500)
                .Add(x => x.SearchFunc, (text, _) =>
                {
                    searches.Add(text);
                    return Task.FromResult<IEnumerable<string>>(States);
                }));

            await comp.Find("input").InputAsync("A");
            timeProvider.Advance(TimeSpan.FromMilliseconds(300));
            await comp.Find("input").InputAsync("Al");
            timeProvider.Advance(TimeSpan.FromMilliseconds(300));
            searches.Should().BeEmpty("the second keystroke restarted the interval");

            timeProvider.Advance(TimeSpan.FromMilliseconds(200));

            await comp.WaitForAssertionAsync(() => searches.Should().Equal("Al"));
        }

        /// <summary>
        /// SelectOnActivation selects the input text when the input first receives focus.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public async Task Autocomplete_SelectOnActivation_SelectsTextOnFocus(bool selectOnActivation)
        {
            var comp = Context.Render<MudAutocomplete<string>>(parameters => parameters
                .Add(p => p.Value, "Alabama")
                .Add(p => p.OpenOnFocus, false)
                .Add(p => p.SelectOnActivation, selectOnActivation));

            await comp.Find("input").FocusAsync();

            Context.JSInterop.Invocations["mudElementRef.select"].Should().HaveCount(selectOnActivation ? 1 : 0);
        }

        /// <summary>
        /// Searches <see cref="States"/> by case-insensitive substring, completing synchronously.
        /// </summary>
        private static Task<IEnumerable<string>> SearchStatesAsync(string text, CancellationToken token)
        {
            return Task.FromResult(States.Where(state => state.Contains(text ?? string.Empty, StringComparison.OrdinalIgnoreCase)));
        }

        private static readonly string[] States = ["Alabama", "Alaska", "Arizona", "Arkansas"];


        private static IElement ListItem<TComponent>(IRenderedComponent<TComponent> comp, string text)
            where TComponent : IComponent
        {
            return comp.FindAll("div.mud-list-item").First(item => item.TextContent.Contains(text));
        }


        private static string HighlightedText<TComponent>(IRenderedComponent<TComponent> comp)
            where TComponent : IComponent
        {
            return comp.Find("div.mud-list-item.mud-selected-item").TextContent.Trim();
        }


        private static int HighlightedIndex<TComponent>(IRenderedComponent<TComponent> comp)
            where TComponent : IComponent
        {
            return comp.FindAll("div.mud-list-item").ToList().FindIndex(item => item.ClassList.Contains("mud-selected-item"));
        }
    }
}
