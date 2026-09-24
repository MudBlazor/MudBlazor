using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Extensions;
using MudBlazor.Resources;
using MudBlazor.UnitTests.Dummy;
using MudBlazor.UnitTests.TestComponents.Select;
using MudBlazor.UnitTests.TestData;
using NUnit.Framework;
using static MudBlazor.UnitTests.TestComponents.Select.SelectWithEnumTest;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class SelectTests : BunitTest
    {
        /// <summary>
        /// Select owns popup keyboard navigation without installing an interceptor for every option.
        /// </summary>
        [Test]
        public async Task SelectOptions_DoNotRegisterKeyInterceptors()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<SelectTest1>();
            var observersWhileClosed = keyInterceptorService.ObserversCount;

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindComponents<MudListItem<string>>().Should().HaveCount(4));

            comp.FindComponents<MudListItem<string>>().Should().OnlyContain(x => !x.Instance.KeyboardEnabled);
            keyInterceptorService.ObserversCount.Should().Be(observersWhileClosed);
        }

        /// <summary>
        /// A select with nothing to present must build its option list once, not twice (#13519).
        /// </summary>
        /// <remarks>
        /// While the select is closed only the hidden shadow items are built.
        /// The select renders again while mounting, and that render must not rebuild them.
        /// </remarks>
        [Test]
        public void Select_WithoutResolvableValue_BuildsItemsOnce()
        {
            var passes = 0;
            Context.Render<MudSelect<string>>(parameters => parameters
                .Add(x => x.ChildContent, builder =>
                {
                    passes++;
                    builder.OpenComponent<MudSelectItem<string>>(0);
                    builder.AddComponentParameter(1, nameof(MudSelectItem<string>.Value), "Espresso");
                    builder.CloseComponent();
                }));

            passes.Should().Be(1);
        }

        /// <summary>
        /// Opening and closing only change the select's own state, so they build the popup options once and never rebuild the hidden ones (#13519).
        /// </summary>
        [Test]
        public async Task Select_OpenAndClose_DoNotRebuildOptions()
        {
            var passes = 0;
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(x => x.ChildContent, builder =>
                {
                    passes++;
                    builder.OpenComponent<MudSelectItem<string>>(0);
                    builder.AddComponentParameter(1, nameof(MudSelectItem<string>.Value), "Espresso");
                    builder.CloseComponent();
                    builder.OpenComponent<MudSelectItem<string>>(2);
                    builder.AddComponentParameter(3, nameof(MudSelectItem<string>.Value), "Latte");
                    builder.CloseComponent();
                }));
            passes.Should().Be(1);

            await comp.InvokeAsync(() => comp.Instance.OpenMenu());
            await provider.WaitForAssertionAsync(() => provider.FindAll("div.mud-list-item").Should().HaveCount(2));
            passes.Should().Be(3, "opening refreshes the shadow items once and the popup builds its own copy");

            await comp.InvokeAsync(() => comp.Instance.CloseMenu());
            passes.Should().Be(3, "closing does not rebuild either copy");
        }

        /// <summary>
        /// An option added without a parent render still shows its content in the input after it is picked.
        /// </summary>
        [Test]
        public async Task Select_OptionAddedWithoutParentRender_ShowsContentAfterPick()
        {
            var items = new List<string> { "Espresso", "Latte" };
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(x => x.Strict, true)
                .Add(x => x.ChildContent, builder =>
                {
                    foreach (var item in items)
                    {
                        builder.OpenComponent<MudSelectItem<string>>(0);
                        builder.AddComponentParameter(1, nameof(MudSelectItem<string>.Value), item);
                        builder.AddComponentParameter(2, nameof(MudSelectItem<string>.ChildContent), (RenderFragment)(content => content.AddContent(0, $"{item} option")));
                        builder.CloseComponent();
                    }
                }));

            items.Add("Cortado");
            await comp.InvokeAsync(() => comp.Instance.OpenMenu());
            await provider.WaitForAssertionAsync(() => provider.FindAll("div.mud-list-item").Should().HaveCount(3));
            await provider.FindAll("div.mud-list-item")[2].ClickAsync();

            await comp.WaitForAssertionAsync(() => comp.Find(".mud-input-control .mud-input").TextContent.Should().Contain("Cortado option"));
        }

        /// <summary>
        /// A render from the parent still rebuilds the options, even when it passes the same ChildContent delegate.
        /// </summary>
        [Test]
        public async Task Select_ParentRender_RebuildsOptionsWithSameChildContent()
        {
            var text = "Espresso";
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(x => x.ChildContent, builder =>
                {
                    builder.OpenComponent<MudSelectItem<string>>(0);
                    builder.AddComponentParameter(1, nameof(MudSelectItem<string>.Value), "coffee");
                    builder.AddComponentParameter(2, nameof(MudSelectItem<string>.ChildContent), (RenderFragment)(content => content.AddContent(0, text)));
                    builder.CloseComponent();
                }));

            await comp.InvokeAsync(() => comp.Instance.OpenMenu());
            await provider.WaitForAssertionAsync(() => provider.Find("div.mud-list-item").TextContent.Trim().Should().Be("Espresso"));

            text = "Latte";
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Dense, true));

            await provider.WaitForAssertionAsync(() => provider.Find("div.mud-list-item").TextContent.Trim().Should().Be("Latte"));
        }

        /// <summary>
        /// A select whose value resolves to an item still renders that item's content, which is why the extra render on mount exists.
        /// </summary>
        [Test]
        public void Select_WithResolvableValue_RendersTheSelectedItemContent()
        {
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(x => x.Value, "Espresso")
                .Add(x => x.ChildContent, builder =>
                {
                    builder.OpenComponent<MudSelectItem<string>>(0);
                    builder.AddComponentParameter(1, nameof(MudSelectItem<string>.Value), "Espresso");
                    builder.AddComponentParameter(2, nameof(MudSelectItem<string>.ChildContent), (RenderFragment)(content => content.AddContent(0, "A short black")));
                    builder.CloseComponent();
                }));

            comp.Find("div.mud-input-slot").TextContent.Should().Contain("A short black");
        }

        /// <summary>
        /// Each class parameter lands on the element it styles: the root, the input control, the input, the popover and the list.
        /// </summary>
        [Test]
        public async Task Select_ClassParameters_ApplyToTheirElements()
        {
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(x => x.OuterClass, "my-outer-class")
                .Add(x => x.Class, "my-main-class")
                .Add(x => x.InputClass, "my-input-class")
                .Add(x => x.PopoverClass, "my-popover-class")
                .Add(x => x.ListClass, "my-list-class")
                .AddChildContent<MudSelectItem<string>>(item => item.Add(x => x.Value, "Espresso")));

            comp.Find($"#{comp.Instance.ElementId}").ClassList.Should().Contain("my-outer-class");
            comp.Find(".mud-input-control").ClassList.Should().Contain("my-main-class");
            comp.Find(".mud-select-input").ClassList.Should().Contain("my-input-class");
            provider.Find("div.mud-popover").ClassList.Should().Contain("my-popover-class");

            await comp.InvokeAsync(() => comp.Instance.OpenMenu());
            await provider.WaitForAssertionAsync(() => provider.Find(".mud-list").ClassList.Should().Contain("my-list-class"));
        }

        /// <summary>
        /// Click should open the Menu and selecting a value should update the bindable value.
        /// </summary>
        [Test]
        public async Task SelectTest1()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            select.Instance.ReadValue.Should().BeNull();
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            select.Instance.ReadValue.Should().Be("2");

            await PickOptionAsync(comp, 0);
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("1"));
        }

        [Test]
        public async Task Select_ModelessOverlay_IgnoresActivatorRootForAutoCloseHitTesting()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            var overlay = comp.Find("div.mud-overlay");
            overlay.GetAttribute("data-modeless-ignore-element-id").Should().Be(select.Instance.ElementId);
        }

        [Test]
        public async Task SelectTestCustomToString()
        {
            var comp = Context.Render<SelectCustomToStringTest>();
            IElement Input() => comp.Find("input[value]");
            Input().GetAttribute("value").Should().Be("Diavolo");

            await Input().MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Select(x => x.TextContent)
                .Should().Equal("Cardinale", "Diavolo", "Margarita", "Spinaci"));

            await comp.FindAll("div.mud-list-item")[2].ClickAsync();
            await comp.WaitForAssertionAsync(() => Input().GetAttribute("value").Should().Be("Margarita"));
        }

        [Test]
        public async Task Select_KeyDown_WhileClosed()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<SelectFocusAndTypeTest>();
            var select = comp.FindComponent<MudSelect<string>>();

            //typing while closed selects the first match without opening the menu
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "t", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Tennessee"));

            //cycle through matching results
            timeProvider.Advance(select.Instance.QuickSearchInterval + TimeSpan.FromMilliseconds(10));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "t", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Texas"));
            timeProvider.Advance(select.Instance.QuickSearchInterval + TimeSpan.FromMilliseconds(10));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "t", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Tennessee"));

            //multi-string search
            timeProvider.Advance(select.Instance.QuickSearchInterval + TimeSpan.FromMilliseconds(10));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "c", Type = "keydown" }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "o", Type = "keydown" }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "l", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Colorado"));

            //paused search
            timeProvider.Advance(select.Instance.QuickSearchInterval + TimeSpan.FromMilliseconds(10));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "i", Type = "keydown" }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "o", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Iowa"));

            timeProvider.Advance(select.Instance.QuickSearchInterval + TimeSpan.FromMilliseconds(10));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "i", Type = "keydown" }));
            timeProvider.Advance(select.Instance.QuickSearchInterval + TimeSpan.FromMilliseconds(10));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "o", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Ohio"));
        }

        /// <summary>
        /// Click should not close the menu and selecting multiple values should update the bindable value with a comma separated list.
        /// </summary>
        [Test]
        public async Task MultiSelectTest1()
        {
            var comp = Context.Render<MultiSelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            select.Instance.ReadValue.Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() =>
                comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() =>
                comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() =>
                comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("2"));
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("2, 1"));
            await comp.FindAll("div.mud-list-item")[2].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("2, 1, 3"));
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("2, 3"));
            select.Instance.GetState(x => x.SelectedValues).Count.Should().Be(2);
            select.Instance.GetState(x => x.SelectedValues).Should().Contain("2");
            select.Instance.GetState(x => x.SelectedValues).Should().Contain("3");
            const string @unchecked =
                "M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z";
            const string @checked =
                "M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z";
            await comp.WaitForAssertionAsync(() =>
                comp.FindAll("div.mud-list-item path")[1].Attributes["d"].Value.Should().Be(@unchecked));
            await comp.WaitForAssertionAsync(() =>
                comp.FindAll("div.mud-list-item path")[3].Attributes["d"].Value.Should().Be(@checked));
            await comp.WaitForAssertionAsync(() =>
                comp.FindAll("div.mud-list-item path")[5].Attributes["d"].Value.Should().Be(@checked));
            await select.SetParametersAndRenderAsync(parameter => parameter.Add(x => x.SelectedValues, new HashSet<string>() { "1", "2" }));
            await comp.WaitForAssertionAsync(() =>
                comp.FindAll("div.mud-list-item path")[1].Attributes["d"].Value.Should().Be(@checked));
            await comp.WaitForAssertionAsync(() =>
                comp.FindAll("div.mud-list-item path")[3].Attributes["d"].Value.Should().Be(@checked));
            await comp.WaitForAssertionAsync(() =>
                comp.FindAll("div.mud-list-item path")[5].Attributes["d"].Value.Should().Be(@unchecked));
        }

        [Test]
        public async Task MultiSelectWithValueContainZero()
        {
            var comp = Context.Render<MultiSelectWithValueContainZeroTest>();
            IReadOnlyList<IElement> Inputs() => comp.FindAll("input");
            Inputs().Should().HaveCount(3);
            Inputs()[0].GetAttribute("value").Should().Be("Value2");

            // Both enum selects share one binding, so toggling in the second one updates the first.
            await Inputs()[1].MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll(".mud-list-item").Should().HaveCount(3));
            for (var i = 0; i < 3; i++)
            {
                await comp.FindAll(".mud-list-item")[i].ClickAsync();
            }

            await comp.WaitForAssertionAsync(() => Inputs()[0].GetAttribute("value").Should().Be("Value3, Value1"));
            Inputs()[1].GetAttribute("value").Should().Be("Value3; Value1");
        }

        /// <summary>
        /// Initial Text should be enums default value
        /// Initial render fragment in input should be the pre-selected value's items's render fragment.
        /// After clicking the second item, the render fragment should update
        /// </summary>
        [Test]
        public async Task SelectWithEnum()
        {
            var comp = Context.Render<SelectWithEnumTest>();
            var select = comp.FindComponent<MudSelect<MyEnum>>();
            select.Instance.ReadValue.Should().Be(default(MyEnum));
            select.Instance.ReadText.Should().Be(default(MyEnum).ToString());
            comp.Find("input").GetAttribute("value").Should().Be("First");

            await PickOptionAsync(comp, 1);

            await comp.WaitForAssertionAsync(() => comp.Find("input").GetAttribute("value").Should().Be("Second"));
        }

        /// <summary>
        /// Initially no member is selected; clicking the first member checks only that member.
        /// </summary>
        [Test]
        public async Task MultiSelectWithEnum()
        {
            var comp = Context.Render<MultiSelectWithEnumTest>();
            var select = comp.FindComponent<MudSelect<MultiSelectWithEnumTest.MyEnum>>();
            select.Instance.GetState(x => x.SelectedValues).Should().BeEmpty();

            await OpenAsync(comp);
            comp.FindAll("div.mud-list-item").Select(CheckboxState).Should().OnlyContain(state => state == "unchecked");

            await comp.FindAll("div.mud-list-item")[0].ClickAsync();

            await comp.WaitForAssertionAsync(() => comp.Find("input").GetAttribute("value").Should().Be("First"));
            select.Instance.GetState(x => x.SelectedValues).Should().Equal(MultiSelectWithEnumTest.MyEnum.First);
            await comp.WaitForAssertionAsync(() =>
            {
                var states = comp.FindAll("div.mud-list-item").Select(CheckboxState).ToList();
                states[0].Should().Be("checked");
                states.Skip(1).Should().OnlyContain(state => state == "unchecked");
            });
        }

        [Test]
        public async Task MultiSelect_ChildlessEnumItems_ShouldUpdateCheckboxImmediately()
        {
            var comp = Context.Render<MultiSelectChildlessEnumToStringFuncTest>();
            var select = comp.FindComponent<MudSelect<MultiSelectChildlessEnumToStringFuncTest.Pizza>>();

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().Be(4));
            CheckboxState(comp.FindAll("div.mud-list-item")[0]).Should().Be("unchecked");

            await comp.FindAll("div.mud-list-item")[1].ClickAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("Diavolo"));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Select(CheckboxState)
                .Should().Equal("unchecked", "checked", "unchecked", "unchecked"));
        }

        [Test]
        public async Task MultiSelect_ChildlessStringItems_ShouldUpdateCheckboxImmediately()
        {
            var comp = Context.Render<MultiSelectChildlessStringTest>();
            var select = comp.FindComponent<MudSelect<string>>();

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().Be(4));
            CheckboxState(comp.FindAll("div.mud-list-item")[2]).Should().Be("unchecked");

            await comp.FindAll("div.mud-list-item")[2].ClickAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("Margarita"));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Select(CheckboxState)
                .Should().Equal("unchecked", "unchecked", "checked", "unchecked"));
        }

        [Test]
        public async Task MultiSelect_SelectAll_ShouldUpdateChildlessItemCheckboxesImmediately()
        {
            var comp = Context.Render<MultiSelectChildlessSelectAllTest>();
            var select = comp.FindComponent<MudSelect<string>>();

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().Be(5));

            await comp.FindAll("div.mud-list-item")[0].ClickAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().HaveCount(4));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("Cardinale, Diavolo, Margarita, Spinaci"));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Skip(1).Select(CheckboxState)
                .Should().OnlyContain(state => state == "checked"));
        }

        /// <summary>
        /// Initially we have a value of 17 which is not in the list. So we render it as text via MudInput
        /// </summary>
        [Test]
        public async Task SelectUnrepresentableValue()
        {
            var comp = Context.Render<SelectUnrepresentableValueTest>();
            var select = comp.FindComponent<MudSelect<int>>();
            select.Instance.ReadValue.Should().Be(17);
            select.Instance.ReadText.Should().Be("17");
            comp.Find("input").GetAttribute("value").Should().Be("17");

            await PickOptionAsync(comp, 1);

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-input-slot").TextContent.Trim().Should().Be("Two"));
            select.Instance.ReadValue.Should().Be(2);
            select.Instance.ReadText.Should().Be("2");
        }

        /// <summary>
        /// Don't show initial value which is not in list because of Strict=true.
        /// </summary>
        [Test]
        public async Task SelectUnrepresentableValueTest2()
        {
            var comp = Context.Render<SelectUnrepresentableValueTest2>();
            var select = comp.FindComponent<MudSelect<int>>();
            select.Instance.ReadValue.Should().Be(17);
            select.Instance.ReadText.Should().Be("17");
            await comp.WaitForAssertionAsync(() => comp.FindComponent<MudInput<string>>().Instance.ReadValue.Should().BeNull());
            comp.FindComponent<MudInput<string>>().Instance.InputType.Should().Be(InputType.Hidden);

            await PickOptionAsync(comp, 1);

            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be(2));
            select.Instance.ReadText.Should().Be("2");
            comp.FindComponent<MudInput<string>>().Instance.ReadValue.Should().Be("2");
            // The picked option has no child content, so the value is shown as plain text.
            comp.FindComponent<MudInput<string>>().Instance.InputType.Should().Be(InputType.Text);
        }

        /// <summary>
        /// When the select has a null value, the text should be displayed, and the mud-shrink class should be applied.
        /// </summary>
        [Test]
        public async Task SelectNullValue()
        {
            var comp = Context.Render<SelectNullValueTest>();
            var select = comp.FindComponent<MudSelect<int?>>();
            select.Instance.ReadValue.Should().BeNull();
            select.Find("div.mud-input-slot").TextContent.Should().Be("None");
            select.FindAll(".mud-shrink").Should().NotBeEmpty();

            await PickOptionAsync(comp, 1);
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be(1));
            select.Find("div.mud-input-slot").TextContent.Should().Be("One");

            await PickOptionAsync(comp, 0);
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().BeNull());
            select.Find("div.mud-input-slot").TextContent.Should().Be("None");
            select.FindAll(".mud-shrink").Should().NotBeEmpty();
        }

        /// <summary>
        /// The items have no render fragments, so instead of RF the select must display the converted string value
        /// </summary>
        [Test]
        public async Task SelectWithoutItemPresenters()
        {
            var comp = Context.Render<SelectWithoutItemPresentersTest>();
            var select = comp.FindComponent<MudSelect<int>>();
            select.Instance.ReadValue.Should().Be(1);
            select.Instance.ReadText.Should().Be("1");
            comp.Find("div.mud-input-slot").GetAttribute("style").Should().Contain("display:none");

            await PickOptionAsync(comp, 1);

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            comp.Find("div.mud-input-slot").GetAttribute("style").Should().Contain("display:none");
            select.Instance.ReadValue.Should().Be(2);
            select.Instance.ReadText.Should().Be("2");
        }

        /// <summary>
        /// SingleSelect: TextChanged should be fired before SelectedValuesChanged
        /// We test this by checking the counter. The event which should be fired first must always
        /// find an even counter value, the second must always find an odd value.
        /// </summary>
        [Test]
        public async Task SingleSelect_Should_FireTextChangedBeforeSelectedValuesChanged()
        {
            var events = new List<string>();
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            await select.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.TextChanged, text => events.Add($"TextChanged: {text}"))
                .Add(x => x.SelectedValuesChanged, values => events.Add($"SelectedValuesChanged: {string.Join(", ", values!)}")));

            await PickOptionAsync(comp, 1);
            await comp.WaitForAssertionAsync(() => events.Should().Equal("TextChanged: 2", "SelectedValuesChanged: 2"));
            select.Instance.ReadValue.Should().Be("2");

            await PickOptionAsync(comp, 0);
            await comp.WaitForAssertionAsync(() => events.Should().Equal(
                "TextChanged: 2", "SelectedValuesChanged: 2",
                "TextChanged: 1", "SelectedValuesChanged: 1"));
            select.Instance.ReadValue.Should().Be("1");
        }

        /// <summary>
        /// MultiSelect: TextChanged should be fired before SelectedValuesChanged
        /// We test this by checking the counter. The event which should be fired first must always
        /// find an even counter value, the second must always find an odd value.
        /// </summary>
        [Test]
        public async Task MultiSelect_Should_FireTextChangedBeforeSelectedValuesChanged()
        {
            var events = new List<string>();
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.MultiSelection, true));
            await select.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.TextChanged, text => events.Add($"TextChanged: {text}"))
                .Add(x => x.SelectedValuesChanged, values => events.Add($"SelectedValuesChanged: {string.Join(", ", values!)}")));

            await OpenAsync(comp);
            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => events.Should().Equal("TextChanged: 2", "SelectedValuesChanged: 2"));

            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => events.Should().Equal(
                "TextChanged: 2", "SelectedValuesChanged: 2",
                "TextChanged: 2, 1", "SelectedValuesChanged: 2, 1"));
            select.Instance.ReadValue.Should().Be("2, 1");
        }

        [Test]
        public async Task Select_Should_FireOnBlur()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            var blurCount = 0;
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.OnBlur, () => blurCount++));

            await comp.InvokeAsync(() => select.Instance.OpenMenu());
            await comp.InvokeAsync(() => select.Instance.CloseMenu());
            blurCount.Should().Be(0);

            await comp.Find($"#{select.Instance.ElementId}").TriggerEventAsync("onfocusout", new FocusEventArgs());
            blurCount.Should().Be(1);
        }

        [Test]
        public async Task Select_OnBlurShouldFireOnceOnFocusLoss()
        {
            var calls = 0;
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(p => p.OnBlur, _ => calls++));

            await comp.Find("input").BlurAsync();
            await comp.Find($"#{comp.Instance.ElementId}").TriggerEventAsync("onfocusout", new FocusEventArgs());

            calls.Should().Be(1);
        }

        [Test]
        public async Task Disabled_SelectItem_Should_Be_Respected()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            await OpenAsync(comp);
            comp.FindAll("div.mud-list-item-disabled").Should().ContainSingle();

            await comp.Find("div.mud-list-item-disabled").ClickAsync();

            select.Instance.ReadValue.Should().BeNull();
            comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open");
        }

        [Test]
        public async Task MultiSelect_ShouldCallValidationFunc()
        {
            var comp = Context.Render<MultiSelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            string validatedValue = null;
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, new Func<string, bool>(value =>
            {
                validatedValue = value;
                return true;
            })));

            await OpenAsync(comp);
            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => validatedValue.Should().Be("2"));
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => validatedValue.Should().Be("2, 1"));
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => validatedValue.Should().Be("2"));
        }

        /// <summary>
        /// Programmatic selected values changes convert each value once for custom multi-selection text.
        /// </summary>
        [Test]
        public async Task MultiSelect_CustomText_ProgrammaticSelection_ConvertsEachValueOnce()
        {
            var conversionCount = 0;
            var countingConversions = false;
            IReadOnlyList<string> capturedValues = null;
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(x => x.MultiSelection, true)
                .Add(x => x.ToStringFunc, value =>
                {
                    if (countingConversions)
                    {
                        conversionCount++;
                    }

                    return value is "null" or null ? null : value;
                })
                .Add(x => x.MultiSelectionTextFunc, values =>
                {
                    capturedValues = values;
                    countingConversions = false;
                    return string.Join("|", values.Select(value => value ?? "<null>"));
                }));
            var select = comp.Instance;

            conversionCount = 0;
            countingConversions = true;
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SelectedValues, new[] { "one", "null", (string)null }));

            conversionCount.Should().Be(3);
            capturedValues.Should().Equal("one", null, null);
            select.ReadText.Should().Be("one|<null>|<null>");
        }

        /// <summary>
        /// Select All converts each value once for custom multi-selection text.
        /// </summary>
        [Test]
        public async Task MultiSelect_CustomText_SelectAll_ConvertsEachValueOnce()
        {
            var conversionCount = 0;
            var countingConversions = false;
            IReadOnlyList<string> capturedValues = null;
            var localizer = Context.Services.GetRequiredService<InternalMudLocalizer>();
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectAll, true)
                .Add(x => x.ToStringFunc, value =>
                {
                    if (countingConversions)
                    {
                        conversionCount++;
                    }

                    return value is "null" or null ? null : value;
                })
                .Add(x => x.MultiSelectionTextFunc, values =>
                {
                    capturedValues = values;
                    countingConversions = false;
                    return string.Join("|", values.Select(value => value ?? "<null>"));
                })
                .AddChildContent<MudSelectItem<string>>(item => item.Add(x => x.Value, "one"))
                .AddChildContent<MudSelectItem<string>>(item => item.Add(x => x.Value, "two")));
            var select = comp.Instance;

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await provider.WaitForAssertionAsync(() => provider.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            var selectAllItem = provider.FindComponent<MudListItem<string>>();
            selectAllItem.Instance.Text.Should().Be(localizer[LanguageResource.MudSelect_SelectAll]);

            conversionCount = 0;
            countingConversions = true;
            await provider.FindAll("div.mud-list-item")[0].ClickAsync();

            conversionCount.Should().Be(2);
            capturedValues.Should().Equal("one", "two");
            select.ReadText.Should().Be("one|two");
            select.SelectAllText.Should().Be("");
        }

        /// <summary>
        /// Multi-selection text updates convert each value once when MultiSelection changes.
        /// </summary>
        [Test]
        public async Task MultiSelect_CustomText_TextUpdate_ConvertsEachValueOnce()
        {
            var conversionCount = 0;
            var countingConversions = false;
            IReadOnlyList<string> capturedValues = null;
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, new[] { "one", "null", (string)null })
                .Add(x => x.ToStringFunc, value =>
                {
                    if (countingConversions)
                    {
                        conversionCount++;
                    }

                    return value is "null" or null ? null : value;
                })
                .Add(x => x.MultiSelectionTextFunc, values =>
                {
                    capturedValues = values;
                    countingConversions = false;
                    return string.Join("|", values.Select(value => value ?? "<null>"));
                }));

            conversionCount = 0;
            countingConversions = true;
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.MultiSelection, false));
            conversionCount = 0;
            countingConversions = true;
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.MultiSelection, true));

            conversionCount.Should().Be(3);
            capturedValues.Should().Equal("one", null, null);
            comp.Instance.ReadText.Should().Be("one|<null>|<null>");
        }

        [Test]
        public async Task MultiSelect_SelectAll()
        {
            var comp = Context.Render<MultiSelectTest2>();
            var select = comp.FindComponent<MudSelect<string>>();
            string validatedValue = null;
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, new Func<string, bool>(value =>
            {
                validatedValue = value;
                return true;
            })));

            await OpenAsync(comp);
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("FirstA^SecondA^ThirdA"));
            validatedValue.Should().Be("FirstA^SecondA^ThirdA");
        }

        [Test]
        public async Task MultiSelect_SelectAll2()
        {
            var comp = Context.Render<MultiSelectTest3>();
            var select = comp.FindComponent<MudSelect<string>>();
            select.Instance.SelectAllText.Should().Be("Select all felines");

            await OpenAsync(comp);

            var options = comp.FindAll("div.mud-list-item");
            options[0].TextContent.Should().Contain("Select all felines");
            options.Select(CheckboxState).Should().HaveCount(8).And.OnlyContain(state => state == "checked");
            comp.FindComponents<MudSelectItem<string>>().Where(x => x.Instance.HideContent)
                .Should().NotBeEmpty().And.OnlyContain(item => item.FindComponents<MudListItem<string>>().Count == 0);
        }

        [Test]
        public async Task MultiSelect_SelectAll3()
        {
            var comp = Context.Render<MultiSelectTest4>();

            await OpenAsync(comp);

            var selectAll = comp.FindAll("div.mud-list-item")[0];
            selectAll.TextContent.Should().Contain("Select all felines");
            CheckboxState(selectAll).Should().Be("unchecked");
        }

        [Test]
        public async Task MultiSelect_SelectAll4()
        {
            var comp = Context.Render<MultiSelectTest7>();
            var select = comp.FindComponent<MudSelect<string>>();
            await OpenAsync(comp);
            select.Instance.GetState(x => x.SelectedValues).Should().BeEmpty();

            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().Equal("FirstA", "SecondA", "ThirdA"));

            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().BeEmpty());
        }

        /// <summary>
        /// SelectAll preserves disabled selected items when selecting and deselecting enabled items (#11236).
        /// </summary>
        [Test]
        public async Task MultiSelect_SelectAll_PreservesDisabledSelectedItems()
        {
            var comp = Context.Render<MultiSelectTest7>();
            var select = comp.FindComponent<MudSelect<string>>();
            await select.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Comparer, StringComparer.OrdinalIgnoreCase)
                .Add(x => x.SelectedValues, ["FOURTHA"]));

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues)
                .Should().BeEquivalentTo(["FirstA", "SecondA", "ThirdA", "FOURTHA"]));

            await comp.FindAll("div.mud-list-item")[0].ClickAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues)
                .Should().BeEquivalentTo(["FOURTHA"]));
        }

        [Test]
        public async Task SingleSelect_Should_CallValidationFunc()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            string validatedValue = null;
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, new Func<string, bool>(value =>
            {
                validatedValue = value;
                return true;
            })));

            await PickOptionAsync(comp, 1);
            await comp.WaitForAssertionAsync(() => validatedValue.Should().Be("2"));

            await PickOptionAsync(comp, 0);
            await comp.WaitForAssertionAsync(() => validatedValue.Should().Be("1"));
        }

        /// <summary>
        /// We filled the multiselect with initial selected values, that must
        /// show in the value of the input as a comma separated list of strings
        /// </summary>
        [Test]
        public void MultiSelect_Initial_Values()
        {
            var comp = Context.Render<MultiSelectWithInitialValuesTest>();

            comp.Find("input").GetAttribute("value").Should().Be("FirstA, SecondA");
        }

        /// <summary>
        /// We filled the multiselect with initial selected values.
        /// Then the returned text in the selection is customized.
        /// </summary>
        [Test]
        public void MultiSelectCustomizedText()
        {
            var comp = Context.Render<MultiSelectCustomizedTextTest>();

            comp.Find("input").GetAttribute("value").Should().Be("Selected values: FirstA, SecondA");
        }

        [Test]
        public async Task SelectClearable()
        {
            var comp = Context.Render<SelectClearableTest>();
            var select = comp.FindComponent<MudSelect<string>>();
            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();

            await PickOptionAsync(comp, 1);
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("2"));
            comp.FindAll(".mud-input-clear-button").Should().ContainSingle();

            await comp.Find(".mud-input-clear-button").ClickAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().BeNull());
            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();
            comp.Instance.ClearButtonClicked.Should().BeTrue();
        }

        [Test]
        public async Task SelectClearable_NonNullableEnum_HiddenWhileValueIsDefault()
        {
            var comp = Context.Render<MudSelect<MyEnum>>(p => p
                .Add(x => x.Clearable, true)
                .Add(x => x.Value, MyEnum.First));

            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Value, MyEnum.Second));
            comp.FindAll(".mud-input-clear-button").Should().ContainSingle();

            // Returning to the default hides it again, since clearing the default would be a no-op.
            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Value, MyEnum.First));
            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();
        }

        [Test]
        public async Task SelectClearable_NonNullableInt_HiddenWhileValueIsDefault()
        {
            var comp = Context.Render<MudSelect<int>>(p => p
                .Add(x => x.Clearable, true)
                .Add(x => x.Value, 0));

            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Value, 2));
            comp.FindAll(".mud-input-clear-button").Should().ContainSingle();
        }

        [Test]
        public async Task SelectClearable_NullableValueType_ShownForZero()
        {
            var comp = Context.Render<MudSelect<int?>>(p => p
                .Add(x => x.Clearable, true)
                .Add(x => x.Value, (int?)null));

            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Value, (int?)0));
            comp.FindAll(".mud-input-clear-button").Should().ContainSingle();
        }

        /// <summary>
        /// Reselect an already selected value should not call SelectedValuesChanged event.
        /// </summary>
        [Test]
        public async Task SelectReselect()
        {
            var comp = Context.Render<ReselectValueTest>();
            var select = comp.FindComponent<MudSelect<string>>();
            select.Instance.ReadValue.Should().Be("Apple");

            await PickOptionAsync(comp, 1);
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Orange"));
            comp.Instance.ChangeCount.Should().Be(1);

            await PickOptionAsync(comp, 1);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            select.Instance.ReadValue.Should().Be("Orange");
            comp.Instance.ChangeCount.Should().Be(1);
        }

        [Test]
        public async Task Select_Should_Validate_Data_Attribute_Fail()
        {
            var comp = Context.Render<SelectValidationDataAttrTest>();
            var select = comp.FindComponent<MudSelect<string>>().Instance;

            await comp.InvokeAsync(() => select.SelectOption("Quux"));
            select.Value.Should().Be("Quux");
            select.ReadText.Should().Be("Quux");

            await comp.InvokeAsync(() => select.ValidateAsync());
            select.ValidationErrors.Should().Equal("Should not be longer than 3");
        }

        [Test]
        public async Task Select_Should_Validate_Data_Attribute_Success()
        {
            var comp = Context.Render<SelectValidationDataAttrTest>();
            var select = comp.FindComponent<MudSelect<string>>().Instance;

            await comp.InvokeAsync(() => select.SelectOption("Qux"));
            select.Value.Should().Be("Qux");
            select.ReadText.Should().Be("Qux");

            await comp.InvokeAsync(() => select.ValidateAsync());
            select.ValidationErrors.Should().BeEmpty();
        }

        /// <summary>
        /// Tests the required property.
        /// </summary>
        [Test]
        public async Task Select_Should_SetRequiredTrue()
        {
            var localizer = Context.Services.GetRequiredService<InternalMudLocalizer>();
            var comp = Context.Render<SelectRequiredTest>();
            var select = comp.FindComponent<MudSelect<string>>().Instance;

            await comp.InvokeAsync(() => select.ValidateAsync());

            select.ValidationErrors.Should().Equal(localizer[LanguageResource.MudFormComponent_Required]);
        }

        /// <summary>
        /// Required MudSelect should show validation error on focus loss without a value selected.
        /// </summary>
        [Test]
        public async Task Select_Required_Should_ShowValidationError_OnFocusOut()
        {
            var localizer = Context.Services.GetRequiredService<InternalMudLocalizer>();
            var comp = Context.Render<SelectRequiredTest>();
            var select = comp.FindComponent<MudSelect<string>>().Instance;
            select.HasErrors.Should().BeFalse();
            select.Touched.Should().BeFalse();

            await comp.Find($"#{select.ElementId}").TriggerEventAsync("onfocusout", new FocusEventArgs());

            select.Touched.Should().BeTrue();
            select.HasErrors.Should().BeTrue();
            select.ValidationErrors.First().Should().Be(localizer[LanguageResource.MudFormComponent_Required]);
        }

        /// <summary>
        /// #11796: in multi-selection the Validation function runs after SelectedValues commits, so it observes the new selection - once per click.
        /// </summary>
        [Test]
        public async Task MultiSelect_Validation_RunsAfterSelectedValuesCommit()
        {
            var comp = Context.Render<SelectMultiSelectionValidationOrderTest>();

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().Be(3));
            comp.Instance.ObservedCounts.Clear();

            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            comp.Instance.ObservedCounts.Should().Equal(new[] { 1 },
                "validation runs once per selection and sees the committed binding (#11796)");

            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            comp.Instance.ObservedCounts.Should().Equal(new[] { 1, 2 });

            // deselect the first option again
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            comp.Instance.ObservedCounts.Should().Equal(new[] { 1, 2, 1 });

            // the Clearable X button must also validate against the committed (now empty) binding
            await comp.Find(".mud-input-clear-button").ClickAsync();
            comp.Instance.ObservedCounts.Should().Equal(new[] { 1, 2, 1, 0 });
        }

        /// <summary>
        /// Selected option should be hilighted when drop-down opens
        /// </summary>
        [Test]
        public async Task Select_Should_HilightSelectedValue()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();

            await OpenAsync(comp);
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-selected-item").Should().BeEmpty());
            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("2"));

            await OpenAsync(comp);
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-selected-item").Should().ContainSingle());
            comp.FindAll("div.mud-list-item")[1].ClassList.Should().Contain("mud-selected-item");

            await comp.InvokeAsync(() => select.Instance.CloseMenu());
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, null));
            await comp.InvokeAsync(() => select.Instance.OpenMenu());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-selected-item").Should().BeEmpty());
        }

        /// <summary>
        /// Initially selected option should be hilighted when drop-down opens
        /// </summary>
        [Test]
        public async Task Select_Should_HilightInitiallySelectedValue()
        {
            var comp = Context.Render<SelectTest2>();
            var select = comp.FindComponent<MudSelect<string>>();
            select.Instance.ReadValue.Should().Be("2");

            await OpenAsync(comp);
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-selected-item").Should().ContainSingle());
            comp.FindAll("div.mud-list-item")[1].ClassList.Should().Contain("mud-selected-item");

            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("1"));

            await OpenAsync(comp);
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-selected-item").Should().ContainSingle());
            comp.FindAll("div.mud-list-item")[0].ClassList.Should().Contain("mud-selected-item");
        }

        [Test]
        public async Task Select_Should_ScrollToInitiallySelectedValue_WhenOpened()
        {
            var comp = Context.Render<SelectTest2>();

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => Context.JSInterop.VerifyInvoke("mudScrollManager.scrollToListItem"));
        }

        [Test]
        public async Task Select_Should_AllowReloadingItems()
        {
            var comp = Context.Render<ReloadSelectItemsTest>();
            var select = comp.FindComponent<MudSelect<string>>();

            async Task<string> PickAsync(int index)
            {
                await PickOptionAsync(comp, index);
                await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
                return select.Instance.ReadValue;
            }

            (await PickAsync(0)).Should().Be("American Samoa");
            (await PickAsync(1)).Should().Be("Arizona");
            (await PickAsync(2)).Should().Be("Arkansas");

            await comp.Find(".reload").ClickAsync();

            (await PickAsync(0)).Should().Be("Alabama");
            (await PickAsync(1)).Should().Be("Alaska");
            (await PickAsync(2)).Should().Be("American Samoa");
        }

        [Test]
        public async Task Select_ToggleOpenCloseMenuMethods()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();

            await comp.InvokeAsync(() => select.Instance.ToggleMenu());
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));
            await comp.InvokeAsync(() => select.Instance.ToggleMenu());
            comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open");

            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, false));
            await comp.InvokeAsync(() => select.Instance.ToggleMenu());
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));

            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));
            await comp.InvokeAsync(() => select.Instance.ToggleMenu());
            await comp.InvokeAsync(() => select.Instance.OpenMenu());
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
        }

        [Test]
        public async Task Select_KeyboardNavigation_SingleSelect()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<SelectTest1>();
            // print the generated html
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Escape", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = " ", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            //If we didn't select an item with mouse or arrow keys yet, value should remains null.
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be(null));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowDown", AltKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowUp", AltKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            //If dropdown is closed, arrow key should not set a value.
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowDown", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be(null));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "NumpadEnter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowUp", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowDown", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("1"));
            //End key should not select the last disabled item
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "End", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("3"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowUp", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("2"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Home", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("1"));
            //Arrow up should select still the first item
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowUp", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("1"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "End", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowDown", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("3"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "2", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("2"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "2", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("2"));
            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().HaveCount(1));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = " ", Type = "keydown", }));
            comp.Render(); // <-- this is necessary for reliable passing of the test
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
        }

        [Test]
        public async Task Select_SelectionOnEnter_ShouldOnlyChangeOnEnter()
        {
            var keys = Context.AddKeyInterceptorService();
            var comp = Context.Render<SelectTest3>();
            var select = comp.FindComponent<MudSelect<string>>();

            await PressAsync(select, keys, "ArrowDown", altKey: true);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await PressAsync(select, keys, "ArrowDown");
            await PressAsync(select, keys, "ArrowDown");
            await PressAsync(select, keys, "ArrowUp");
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-selected-item").Single().TextContent.Trim().Should().Be("1"));
            select.Instance.Value.Should().BeNull();

            await PressAsync(select, keys, "Enter");

            await comp.WaitForAssertionAsync(() => select.Instance.Value.Should().Be("1"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
        }

        [Test]
        public async Task Select_KeyboardNavigation_MultiSelect()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<MultiSelectTest3>();
            // print the generated html
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = " ", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "a", CtrlKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("0 feline has been selected"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "A", CtrlKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("7 felines have been selected"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("6 felines have been selected"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "A", CtrlKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("7 felines have been selected"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Escape", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().Contain("Jaguar"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Home", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "NumpadEnter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().NotContain("Jaguar"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().Contain("Leopard"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "End", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().NotContain("Tiger"));

            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().NotContain("Tiger"));

            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, false));
            //Test the keyup event
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyUp(select.Instance.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keyup", }));
            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().NotContain("Tiger"));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Tab", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyUp(select.Instance.ElementId, new KeyboardEventArgs { Key = "Tab" }));
            comp.Render(); // <-- this is necessary for reliable passing of the test
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
        }

        [Test]
        public async Task Select_KeyboardNavigation_MultiSelect_Focus()
        {
            var comp = Context.Render<MultiSelectTest6>();
            var select = comp.FindComponent<MudSelect<string>>();
            await OpenAsync(comp);
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.FindAll("div.mud-list-item")[2].ClickAsync();

            await comp.Find(".mud-select").FocusOutAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("Alaska, Alabama, American Samoa"));
            select.Instance.GetState(x => x.Open).Should().BeTrue();
            Context.JSInterop.VerifyFocusAsyncInvoke();
        }

        [Test]
        public async Task Select_ItemlessSelect()
        {
            var keys = Context.AddKeyInterceptorService();
            var comp = Context.Render<MudSelect<string>>();

            foreach (var key in new[] { " ", "ArrowDown", "Home", "End", "Enter" })
            {
                await PressAsync(comp, keys, key);
            }

            comp.Instance.GetState(x => x.SelectedValues).Should().BeEmpty();
            comp.Instance.ReadValue.Should().BeNull();
        }

        [Test]
        public async Task MultiSelectWithCustomComparer()
        {
            var comp = Context.Render<MultiSelectWithCustomComparerTest>();

            await comp.Find("#set-selection-button").ClickAsync();
            comp.Find("input").GetAttribute("value").Should().Be("Selected Cafe Latte, Selected Espresso");

            await OpenAsync(comp);
            comp.FindAll("div.mud-list-item").Select(CheckboxState).Should().Equal("unchecked", "checked", "checked", "unchecked");
        }

        [Test(Description = "https://github.com/MudBlazor/MudBlazor/issues/13106")]
        public async Task MultiSelectWithCustomComparer_InitialSelectionPreservedOnFirstRender()
        {
            var comp = Context.Render<MultiSelectComparerInitialTest>();

            // The parent's bound collection must still contain both preselected items.
            comp.Instance._selected.Select(c => c!.Key).Should().BeEquivalentTo("lat", "esp");

            // The input text reflects the preselected values' names, which proves the comparer matched on Key.
            comp.Find("input").GetAttribute("value").Should().Be("Preselected Latte, Preselected Espresso");

            // Cappuccino, Cafe Latte (Key="lat"), Espresso (Key="esp"), Irish Coffee.
            await OpenAsync(comp);
            comp.FindAll("div.mud-list-item").Select(CheckboxState).Should().Equal("unchecked", "checked", "checked", "unchecked");
        }

        [Test(Description = "https://github.com/MudBlazor/MudBlazor/issues/13106")]
        public async Task MultiSelectWithCustomComparer_InitialBindPreservedOnFirstRender()
        {
            var comp = Context.Render<MultiSelectWithCustomComparerInitialBindTest>();

            comp.Instance.SelectedItems.Should().BeEquivalentTo("test1");
            comp.Find("input").GetAttribute("value").Should().Be("test1");

            await OpenAsync(comp);
            comp.FindAll("div.mud-list-item").Select(CheckboxState).Should().Equal("checked", "unchecked", "unchecked");
        }

        [Test(Description = "A custom Comparer must drive value->item resolution for highlight/active-descendant, not just selection state.")]
        public async Task SingleSelectWithCustomComparer_HighlightsKeyEqualItem()
        {
            var comp = Context.Render<SingleSelectComparerHighlightTest>();

            await comp.Find("div.mud-input-control").MouseDownAsync();

            await comp.WaitForAssertionAsync(() =>
            {
                var input = comp.Find("input");
                var latte = comp.FindAll("div.mud-list-item").Single(item => item.TextContent.Contains("Cafe Latte"));

                // The bound value is a different Coffee instance with the same Key ("lat") as "Cafe Latte".
                // Without honoring the comparer the dictionary lookup misses, so no item is highlighted and aria-activedescendant is omitted.
                input.GetAttribute("aria-activedescendant").Should().Be(latte.Id);
                latte.ClassList.Should().Contain("mud-selected-item");
            });
        }

        [Test(Description = "A custom Comparer must drive value->item resolution for the selected-value template (shadow lookup).")]
        public async Task SingleSelectWithCustomComparer_RendersKeyEqualItemTemplate()
        {
            var comp = Context.Render<SingleSelectComparerPresenterTest>();

            // The bound value is a different Coffee instance with the same Key ("lat") as "Cafe Latte".
            // Resolving it to the matching item's ChildContent requires honoring the comparer.
            comp.Find("div.mud-select-input").TextContent.Should().Contain("Latte template");
        }

        [Test(Description = "A custom Comparer that matches no item resolves to no highlight rather than mis-highlighting.")]
        public async Task SingleSelectWithCustomComparer_NoMatch_HighlightsNothing()
        {
            var comp = Context.Render<SingleSelectComparerNoMatchTest>();

            await comp.Find("div.mud-input-control").MouseDownAsync();

            await comp.WaitForAssertionAsync(() =>
            {
                comp.FindAll("div.mud-list-item").Should().NotBeEmpty();
                comp.FindAll("div.mud-selected-item").Should().BeEmpty();
                comp.Find("input").HasAttribute("aria-activedescendant").Should().BeFalse();
            });
        }

        [Test]
        public async Task Select_Item_Collection_Should_Match_Number_Of_Select_Options()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();

            await OpenAsync(comp);

            select.Instance.Items.Select(x => x.Value).Should().Equal("1", "2", "3", "4");
        }

        /// <summary>
        /// When MultiSelection and Required are True with no selected values, required validation should fail.
        /// </summary>
        [Test]
        public async Task MultiSelectWithRequiredValue()
        {
            var localizer = Context.Services.GetRequiredService<InternalMudLocalizer>();
            var required = localizer[LanguageResource.MudFormComponent_Required];
            var comp = Context.Render<MultiSelectTestRequiredValue>();
            var stringSelect = comp.FindComponent<MudSelect<string>>().Instance;
            var objectSelect = comp.FindComponent<MudSelect<MultiSelectTestRequiredValue.TestClass>>().Instance;

            await comp.InvokeAsync(() => stringSelect.ValidateAsync());
            stringSelect.ValidationErrors.Should().Equal(required);
            await comp.InvokeAsync(() => objectSelect.ValidateAsync());
            objectSelect.ValidationErrors.Should().Equal(required);

            await comp.FindAll("div.mud-input-control")[0].MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Should().HaveCount(3));
            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.InvokeAsync(() => stringSelect.ValidateAsync());
            stringSelect.ValidationErrors.Should().BeEmpty();

            await comp.FindAll("div.mud-input-control")[0].MouseDownAsync();
            await comp.FindAll("div.mud-input-control")[1].MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Should().HaveCount(2));
            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.InvokeAsync(() => objectSelect.ValidateAsync());
            objectSelect.ValidationErrors.Should().BeEmpty();
        }

        [Test]
        public async Task MultiSelectClearAndReset()
        {
            var localizer = Context.Services.GetRequiredService<InternalMudLocalizer>();
            var required = localizer[LanguageResource.MudFormComponent_Required];
            var comp = Context.Render<MultiSelectTestRequiredValue>();
            var stringSelect = comp.FindComponent<MudSelect<string>>().Instance;
            var objectSelect = comp.FindComponent<MudSelect<MultiSelectTestRequiredValue.TestClass>>().Instance;

            async Task SelectSecondOptionAsync(int selectIndex)
            {
                await comp.FindAll("div.mud-input-control")[selectIndex].MouseDownAsync();
                await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(1));
                await comp.FindAll("div.mud-list-item")[1].ClickAsync();
                await comp.FindAll("div.mud-input-control")[selectIndex].MouseDownAsync();
            }

            await SelectSecondOptionAsync(0);
            stringSelect.Value.Should().Be("2");
            stringSelect.GetState(x => x.SelectedValues).Should().Equal("2");
            await comp.Find("#clear-string").ClickAsync();
            stringSelect.Value.Should().BeNullOrEmpty();
            stringSelect.GetState(x => x.SelectedValues).Should().BeEmpty();
            stringSelect.ValidationErrors.Should().Equal(required);

            await SelectSecondOptionAsync(0);
            await comp.Find("#reset-string").ClickAsync();
            stringSelect.Value.Should().BeNullOrEmpty();
            stringSelect.GetState(x => x.SelectedValues).Should().BeEmpty();
            stringSelect.ValidationErrors.Should().BeEmpty();

            await SelectSecondOptionAsync(1);
            objectSelect.SelectedValues.Select(x => x.Name).Should().Equal("Customer");
            await comp.Find("#clear-object").ClickAsync();
            objectSelect.SelectedValues.Should().BeEmpty();
            objectSelect.ValidationErrors.Should().Equal(required);

            await SelectSecondOptionAsync(1);
            await comp.Find("#reset-object").ClickAsync();
            objectSelect.SelectedValues.Should().BeEmpty();
            objectSelect.ValidationErrors.Should().BeEmpty();
        }

        /// <summary>
        /// When MultiSelect attribute goes after SelectedValues, text should contain all selected values.
        /// </summary>
        [Test]
        public async Task MultiSelectAttributesOrder()
        {
            var comp = Context.Render<MultiSelectTest5>();
            var selectComponent = comp.FindComponent<MudSelect<string>>();
            var select = selectComponent.Instance;
            select.GetState(x => x.SelectedValues).Count.Should().Be(2);
            select.ReadText.Should().Be("Programista, test");

            await selectComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SelectedValues, new List<string> { "test" }));

            select.GetState(x => x.SelectedValues).Count.Should().Be(1);
            select.ReadText.Should().Be("test");
        }

        /// <summary>
        /// A select component with a label should auto-generate an id and use that id on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void SelectWithLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.Render<MudSelect<string>>(parameters
                => parameters.Add(p => p.Label, "Test Label"));

            comp.Find("input").Id.Should().NotBeNullOrEmpty();
            comp.Find("label").GetAttribute("for").Should().Be(comp.Find("input").Id);
        }

        /// <summary>
        /// A select component with a label and UserAttributesId should use the UserAttributesId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void SelectWithLabelAndUserAttributesId_Should_UseUserAttributesIdForInputAndAccompanyingLabel()
        {
            var expectedId = "userattributes-id";
            var comp = Context.Render<MudSelect<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes, new Dictionary<string, object>
                    {
                        { "Id", expectedId }
                    }));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").GetAttribute("for").Should().Be(expectedId);
        }

        /// <summary>
        /// A select component with a label, a UserAttributesId, and an InputId should use the InputId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void SelectWithLabelAndUserAttributesIdAndInputId_Should_UseInputIdForInputAndAccompanyingLabel()
        {
            var expectedId = "input-id";
            var comp = Context.Render<MudSelect<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes, new Dictionary<string, object>
                    {
                        { "Id", "userattributes-id" }
                    })
                    .Add(p => p.InputId, expectedId));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").GetAttribute("for").Should().Be(expectedId);
        }

        /// <summary>
        /// Optional Select should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalSelect_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.Render<MudSelect<string>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required Select should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredSelect_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required Select attributes should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredSelectAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<MudSelect<string>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        [Test]
        public void Should_render_conversion_error_message()
        {
            var comp = Context.Render<MudSelect<int>>(parameters => parameters
                .Add(p => p.ErrorId, "error-id")
                .Add(p => p.Text, "not a number")
                .Add(p => p.Converter, new DummyErrorConverter()));

            comp.Instance.ConversionErrorMessage.Should().NotBeNullOrEmpty();
            comp.Find("#error-id").InnerHtml.Should().Be(comp.Instance.ConversionErrorMessage);
            comp.Find("input").GetAttribute("aria-describedby").Should().Be("error-id");
            comp.Find("input").GetAttribute("aria-invalid").Should().Be("true");
        }

        [TestCase(Adornment.Start)]
        [TestCase(Adornment.End)]
        public void Should_render_aria_label_for_adornment_if_provided(Adornment adornment)
        {
            var ariaLabel = "the aria label";
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(p => p.Adornment, adornment)
                .Add(p => p.AdornmentIcon, Icons.Material.Filled.Accessibility)
                .Add(p => p.AdornmentAriaLabel, ariaLabel));

            comp.Find(".mud-input-adornment-icon").GetAttribute("aria-label").Should().Be(ariaLabel);
        }

#nullable enable
        /// <summary>
        /// Verifies that a select field with various configurations renders the expected <c>aria-describedby</c> attribute.
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

            var comp = Context.Render<MudSelect<string>>(parameters => parameters
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

        [Test]
        public async Task ReadOnlyShouldNotHaveClearButton()
        {
            var comp = Context.Render<MudSelect<string>>(p => p
                .Add(x => x.Text, "some value")
                .Add(x => x.Clearable, true)
                .Add(x => x.ReadOnly, false));

            comp.FindAll(".mud-input-clear-button").Should().ContainSingle();

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ReadOnly, true));
            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();
        }

        [Test]
        public async Task ReadOnlyShouldHaveMudReadonlyClass()
        {
            var comp = Context.Render<MudSelect<string>>(p => p
                .Add(x => x.ReadOnly, false));

            comp.Find(".mud-select-input").ClassList.Should().NotContain("mud-readonly");

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ReadOnly, true));
            comp.Find(".mud-select-input").ClassList.Should().Contain("mud-readonly");
        }

        [Test]
        public async Task SelectPopoverFullWidth()
        {
            var comp = Context.Render<SelectPopoverRelativeWidthTest>();

            await comp.Find("#restricted-select").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find(".restricted").ClassList.Should().Contain("mud-popover-open"));
            comp.Find(".restricted").ClassList.Should().Contain("mud-popover-relative-width");

            await comp.Find("#restricted-select").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find(".restricted").ClassList.Should().NotContain("mud-popover-open"));

            await comp.Find("#expanded-select").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find(".expanded").ClassList.Should().Contain("mud-popover-open"));
            comp.Find(".expanded").ClassList.Should().NotContain("mud-popover-relative-width");
        }

        [Test]
        public async Task SelectFitContent()
        {
            var comp = Context.Render<SelectFitContentTest>();
            comp.Find(".mud-select").ClassList.Should().NotContain("mud-width-content");

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(c => c.FitContent, true));

            comp.Find(".mud-select").ClassList.Should().Contain("mud-width-content");
            var filler = comp.Find(".mud-select-filler");
            filler.ClassList.Should().Contain("d-inline-block").And.Contain("mx-4");
            filler.TextContent.Trim().Should().Be("Federated States of Micronesia");

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(c => c.FullWidth, true));

            comp.Find(".mud-select").ClassList.Should().NotContain("mud-width-content");
        }

        [Test]
        public void SelectFitContent_InitiallyEnabled()
        {
            var comp = Context.Render<SelectFitContentTest>(parameters => parameters
                .Add(x => x.FitContent, true));

            comp.Find(".mud-select").ClassList.Should().Contain("mud-width-content");
            comp.Find(".mud-select-filler").TextContent.Trim().Should().Be("Federated States of Micronesia");
        }

        [TestCaseSource(typeof(MouseEventArgsTestCase), nameof(MouseEventArgsTestCase.AllCombinations))]
        public async Task Select_HandleMouseDown(MouseEventArgs args)
        {
            var comp = Context.Render<MudSelect<string>>();

            await comp.InvokeAsync(() => comp.Instance.HandleMouseDown(args));

            comp.Instance.GetState(x => x.Open).Should().Be(args.Button == 0);
        }

        [Test]
        public async Task SelectMultiSelectFieldChanged()
        {
            var comp = Context.Render<SelectMultiSelectFieldChangedTest>();
            comp.Instance.FormFieldChangedEventArgs.Should().BeNull();

            await OpenAsync(comp);
            await comp.Find(".mud-list-item").ClickAsync();

            await comp.WaitForAssertionAsync(() => comp.Instance.FormFieldChangedEventArgs.Should().NotBeNull());
            comp.Instance.FormFieldChangedEventArgs!.NewValue.Should().BeEquivalentTo(comp.Instance.States.Take(2).Reverse());
        }

        [Test]
        public async Task SelectOpenTwoWay()
        {
            var comp = Context.Render<SelectOpenTwoBindTest>();
            var select = comp.FindComponent<MudSelect<string>>().Instance;
            IElement Switch() => comp.Find("#switch");

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            comp.Instance.Open.Should().BeTrue();
            Switch().HasAttribute("checked").Should().BeTrue();

            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            comp.Instance.Open.Should().BeFalse();
            Switch().HasAttribute("checked").Should().BeFalse();

            await Switch().ChangeAsync(true);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            select.GetState(x => x.Open).Should().BeTrue();

            await Switch().ChangeAsync(false);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            select.GetState(x => x.Open).Should().BeFalse();
        }

        /// <summary>
        /// PopoverFixed renders the dropdown with fixed positioning.
        /// </summary>
        [Test]
        public void Select_PopoverFixed_RendersFixedPopover()
        {
            var provider = Context.Render<MudPopoverProvider>();
            Context.Render<MudSelect<string>>(parameters => parameters.Add(x => x.PopoverClass, "default-popover"));
            Context.Render<MudSelect<string>>(parameters => parameters
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
        public async Task Select_Modal_FallsBackToGlobalModalOverlay(bool? modal, bool globalModalOverlay, bool expectModal)
        {
            Context.Services.Configure<PopoverOptions>(options => options.ModalOverlay = globalModalOverlay);
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudSelect<string>>(parameters => parameters.Add(x => x.Modal, modal));

            await comp.InvokeAsync(() => comp.Instance.OpenMenu());

            var overlayStyle = provider.Find("div.mud-overlay").GetAttribute("style") ?? string.Empty;
            overlayStyle.Contains("pointer-events:none").Should().Be(!expectModal, "a modeless overlay lets pointer events through");
        }

        [Test]
        public async Task Select_ToStringFunc_ShouldTakePrecedenceOverChildContent()
        {
            var comp = Context.Render<SelectPrecedenceTest>();
            var select = comp.FindComponent<MudSelect<string>>().Instance;

            // ToStringFunc returns null for item1, so the item's content is shown.
            var presenter = comp.Find("div.mud-input-slot[style*='display:inline']");
            presenter.QuerySelector(".custom-render").Should().NotBeNull();
            presenter.TextContent.Trim().Should().Be("Item 1 Rendered");

            // ToStringFunc returns "ITEM2" for item2, so the text wins over the content.
            await comp.InvokeAsync(() => select.SelectOption("item2"));

            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-input-slot[style*='display:inline']").Should().BeEmpty());
            comp.Find("input").GetAttribute("value").Should().Be("ITEM2");
        }

        [Test]
        public async Task Select_FitContent_ShouldPrioritizeToStringFunc()
        {
            var comp = Context.Render<SelectPrecedenceTest>();
            var selectComponent = comp.FindComponent<MudSelect<string>>();

            // Remove the label so it cannot be the longest text.
            await selectComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.FitContent, true)
                .Add(x => x.Label, null)
                .Add(x => x.ToStringFunc, new Func<string?, string?>(x => x == "item2" ? "VERY LONG ITEM 2" : null)));

            // item1 falls back to its "Item 1 Rendered" content (15 chars), item2 formats as "VERY LONG ITEM 2" (16 chars).
            comp.Find(".mud-select-filler").TextContent.Should().Contain("VERY LONG ITEM 2");
            comp.Find(".mud-select-filler").InnerHtml.Should().NotContain("custom-render");

            await selectComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ToStringFunc, new Func<string?, string?>(x => x == "item1" ? "EXTREMELY LONG ITEM 1" : "ITEM 2")));

            // The longest option is only measured again when FitContent turns on.
            await selectComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.FitContent, false));
            await selectComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.FitContent, true));

            comp.Find(".mud-select-filler").TextContent.Should().Contain("EXTREMELY LONG ITEM 1");
            comp.Find(".mud-select-filler").InnerHtml.Should().NotContain("custom-render");
        }

        [Test]
        public async Task Select_CustomItemRenderFragment()
        {
            var comp = Context.Render<CustomItemRenderFragmentTest>();
            comp.Find(".mud-select-input").TextContent.Should().Contain("Initial Item 1");

            await comp.Find("#switch_values").ClickAsync();

            await comp.WaitForAssertionAsync(() => comp.Find(".mud-select-input").TextContent.Should().Contain("Item 1").And.NotContain("Initial"));
        }

        [Test]
        public async Task Select_ShouldExposeComboboxSemantics_OnInput()
        {
            var comp = Context.Render<MultiSelectTest6>();

            var input = comp.Find("input");
            input.GetAttribute("role").Should().Be("combobox");
            input.GetAttribute("aria-haspopup").Should().Be("listbox");
            input.GetAttribute("aria-expanded").Should().Be("false");
            input.GetAttribute("aria-label").Should().Be("US States");

            await comp.Find("div.mud-input-control").MouseDownAsync();

            await comp.WaitForAssertionAsync(() =>
            {
                var openInput = comp.Find("input");
                openInput.GetAttribute("aria-expanded").Should().Be("true");
                openInput.GetAttribute("aria-activedescendant").Should().NotBeNullOrWhiteSpace();

                var listboxId = openInput.GetAttribute("aria-controls");
                listboxId.Should().NotBeNullOrWhiteSpace();

                var listbox = comp.Find($"#{listboxId}");
                listbox.GetAttribute("role").Should().Be("listbox");
                listbox.GetAttribute("aria-multiselectable").Should().Be("true");
            });
        }

        [Test]
        public async Task Select_ShouldExposeComboboxSemantics_OnCustomPresenter()
        {
            var comp = Context.Render<SelectPrecedenceTest>();

            var display = comp.Find("div.mud-select-input[tabindex='0']");
            display.GetAttribute("role").Should().Be("combobox");
            display.GetAttribute("aria-haspopup").Should().Be("listbox");
            display.GetAttribute("aria-expanded").Should().Be("false");
            display.GetAttribute("aria-label").Should().Be("Select");

            await comp.Find("div.mud-input-control").MouseDownAsync();

            await comp.WaitForAssertionAsync(() =>
            {
                var openDisplay = comp.Find("div.mud-select-input[tabindex='0']");
                openDisplay.GetAttribute("aria-expanded").Should().Be("true");
                openDisplay.GetAttribute("aria-controls").Should().NotBeNullOrWhiteSpace();
            });
        }

        [Test]
        public void Select_UserAttributes_ShouldOverrideGeneratedAccessibilityAttributes()
        {
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(x => x.Label, "US States")
                .AddUnmatched("role", "button")
                .AddUnmatched("aria-autocomplete", "list")
                .AddUnmatched("aria-controls", "custom-listbox")
                .AddUnmatched("aria-expanded", "mixed")
                .AddUnmatched("aria-haspopup", "dialog")
                .AddUnmatched("aria-label", "Custom label")
                .AddUnmatched("aria-activedescendant", "custom-option"));

            var input = comp.Find("input");
            input.GetAttribute("role").Should().Be("button");
            input.GetAttribute("aria-autocomplete").Should().Be("list");
            input.GetAttribute("aria-controls").Should().Be("custom-listbox");
            input.GetAttribute("aria-expanded").Should().Be("mixed");
            input.GetAttribute("aria-haspopup").Should().Be("dialog");
            input.GetAttribute("aria-label").Should().Be("Custom label");
            input.GetAttribute("aria-activedescendant").Should().Be("custom-option");
        }

        [Test]
        public async Task Select_UserAttributes_ForwardedToSelectedValuePresenter()
        {
            // A value is preselected and its item renders child content, so the hidden input is swapped for the focusable presenter div.
            var comp = Context.Render<SelectOnFocusTest>();
            IElement Presenter() => comp.Find("div.mud-select-input[tabindex='0']");

            Presenter().GetAttribute("data-testid").Should().Be("vehicle-select");

            comp.Find("#focus-count").TextContent.Should().Be("0");
            await Presenter().TriggerEventAsync("onfocus", new FocusEventArgs());
            await comp.WaitForAssertionAsync(() => comp.Find("#focus-count").TextContent.Should().Be("1"));
        }

        [Test]
        public void Select_PresenterAttributes_DoNotDuplicateIdOrFocusDisabled()
        {
            var comp = Context.Render<SelectPresenterAttributeTest>();

            comp.FindAll("[id='select-with-id']").Should().ContainSingle();
            comp.FindAll("div.mud-select-input[id='select-with-id']").Should().BeEmpty();

            comp.Find("div.mud-select-input[data-scenario='disabled']").HasAttribute("tabindex").Should().BeFalse();
        }

        [Test]
        public async Task Select_MultiSelect_ShouldKeepSelectionStateIndependentOfActiveDescendant()
        {
            var comp = Context.Render<MultiSelectTest6>();

            await comp.Find("div.mud-input-control").MouseDownAsync();

            await comp.WaitForAssertionAsync(() =>
            {
                var input = comp.Find("input");
                var alabama = comp.FindAll("div.mud-list-item").Single(item => item.TextContent.Contains("Alabama"));
                var alaska = comp.FindAll("div.mud-list-item").Single(item => item.TextContent.Contains("Alaska"));

                input.GetAttribute("aria-activedescendant").Should().Be(alabama.Id);
                alabama.GetAttribute("aria-selected").Should().Be("false");
                alaska.GetAttribute("aria-selected").Should().Be("true");
            });
        }

        [Test]
        public void AutoFocus_ShouldFocusWithoutScrolling()
        {
            Context.Render<MudSelect<string>>(parameters => parameters
                .Add(p => p.AutoFocus, true));

            var focusInvocation = Context.JSInterop.Invocations["Blazor._internal.domWrapper.focus"].Single();
            var preventScroll = focusInvocation.Arguments.OfType<bool>().Single();
            preventScroll.Should().BeTrue();
        }

        [Test]
        public async Task FocusAsync_ShouldFocusWithScrolling()
        {
            var comp = Context.Render<MudSelect<string>>();

            await comp.InvokeAsync(async () => await comp.Instance.FocusAsync());

            var focusInvocation = Context.JSInterop.Invocations["Blazor._internal.domWrapper.focus"].Single();
            var preventScroll = focusInvocation.Arguments.OfType<bool>().Single();
            preventScroll.Should().BeFalse();
        }

        /// <summary>
        /// A closed select omits aria-controls because its listbox is not rendered (#13760).
        /// </summary>
        [Test]
        public void Select_ShouldNotReferenceListbox_WhileClosed()
        {
            var comp = Context.Render<MudSelect<string>>(parameters => parameters.Add(p => p.Label, "Closed select"));

            var input = comp.Find("input");
            input.GetAttribute("aria-expanded").Should().Be("false");
            input.HasAttribute("aria-controls").Should().BeFalse();
        }

        /// <summary>
        /// ValueChanged is raised once for each new pick and not at all for a reselected or disabled option.
        /// </summary>
        [Test]
        public async Task Select_ValueChanged_RaisedOncePerNewValue()
        {
            var comp = Context.Render<SelectEventCountTest>();

            await PickOptionAsync(comp, 0);
            await comp.WaitForAssertionAsync(() => comp.Instance.ValueChangeCount.Should().Be(1));

            await PickOptionAsync(comp, 1);
            await comp.WaitForAssertionAsync(() => comp.Instance.ValueChangeCount.Should().Be(2));

            await PickOptionAsync(comp, 1);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));

            await PickOptionAsync(comp, 3);
            comp.Instance.ValueChangeCount.Should().Be(2, "reselecting the current option and clicking a disabled one change nothing");
            comp.Instance.ValuesChangeCount.Should().Be(2);
        }

        /// <summary>
        /// Toggling options in a multi-select raises SelectedValuesChanged once per click.
        /// </summary>
        [Test]
        public async Task MultiSelect_SelectedValuesChanged_RaisedOncePerToggle()
        {
            var comp = Context.Render<SelectEventCountTest>(parameters => parameters.Add(x => x.MultiSelection, true));

            await OpenAsync(comp);
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.ValuesChangeCount.Should().Be(1));

            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.ValuesChangeCount.Should().Be(2));

            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.ValuesChangeCount.Should().Be(3));

            await comp.FindAll("div.mud-list-item")[3].ClickAsync();
            comp.Instance.ValuesChangeCount.Should().Be(3, "a disabled option cannot be toggled");
        }

        /// <summary>
        /// Select all is indeterminate while only some options are selected, and clicking it then selects every option.
        /// </summary>
        [Test]
        public async Task MultiSelect_SelectAll_IndeterminateWhenSomeOptionsSelected()
        {
            var comp = Context.Render<MultiSelectTest4>();
            var select = comp.FindComponent<MudSelect<string>>();
            await OpenAsync(comp);

            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => CheckboxState(comp.FindAll("div.mud-list-item")[0]).Should().Be("indeterminate"));

            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().HaveCount(7));
            CheckboxState(comp.FindAll("div.mud-list-item")[0]).Should().Be("checked");
        }

        /// <summary>
        /// ClearAsync resets the value, the text and the selection, and raises the change callbacks.
        /// </summary>
        [Test]
        public async Task Select_ClearAsync_ResetsValueTextAndSelection()
        {
            string? boundValue = null;
            IReadOnlyCollection<string?>? boundValues = null;
            var comp = Context.Render<MudSelect<string>>(parameters => parameters
                .Add(x => x.ValueChanged, value => boundValue = value)
                .Add(x => x.SelectedValuesChanged, values => boundValues = values)
                .AddChildContent<MudSelectItem<string>>(item => item.Add(x => x.Value, "Espresso"))
                .AddChildContent<MudSelectItem<string>>(item => item.Add(x => x.Value, "Latte")));
            await comp.InvokeAsync(() => comp.Instance.SelectOption("Latte"));
            boundValue.Should().Be("Latte");
            boundValues.Should().Equal("Latte");

            await comp.InvokeAsync(() => comp.Instance.ClearAsync());

            comp.Instance.ReadValue.Should().BeNull();
            comp.Instance.ReadText.Should().BeNull();
            comp.Find("input").GetAttribute("value").Should().BeNullOrEmpty();
            boundValue.Should().BeNull();
            boundValues.Should().BeEmpty();
        }

        /// <summary>
        /// ClearAsync on a multi-select deselects every option.
        /// </summary>
        [Test]
        public async Task MultiSelect_ClearAsync_DeselectsEveryOption()
        {
            var comp = Context.Render<MultiSelectTest3>();
            var select = comp.FindComponent<MudSelect<string>>();
            select.Instance.GetState(x => x.SelectedValues).Should().HaveCount(7);

            await comp.InvokeAsync(() => select.Instance.ClearAsync());
            await OpenAsync(comp);

            select.Instance.GetState(x => x.SelectedValues).Should().BeEmpty();
            comp.FindAll("div.mud-list-item").Select(CheckboxState).Should().OnlyContain(state => state == "unchecked");
        }

        /// <summary>
        /// A read-only select opens neither by mouse, keyboard nor OpenMenu.
        /// </summary>
        [Test]
        public async Task Select_ReadOnly_DoesNotOpen()
        {
            var keys = Context.AddKeyInterceptorService();
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ReadOnly, true));

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await PressAsync(select, keys, "Enter");
            await PressAsync(select, keys, "ArrowDown");
            await comp.InvokeAsync(() => select.Instance.OpenMenu());

            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
            select.Instance.ReadValue.Should().BeNull();
        }

        /// <summary>
        /// An option whose value is null can be removed from the select without breaking the remaining options.
        /// </summary>
        [Test]
        public async Task Select_NullValuedOption_CanBeRemoved()
        {
            var values = new List<int?> { null, 1 };
            var provider = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<MudSelect<int?>>(parameters => parameters
                .Add(x => x.ChildContent, builder =>
                {
                    foreach (var value in values)
                    {
                        builder.OpenComponent<MudSelectItem<int?>>(0);
                        builder.SetKey(value ?? -1);
                        builder.AddComponentParameter(1, nameof(MudSelectItem<int?>.Value), value);
                        builder.CloseComponent();
                    }
                }));

            values.Remove(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Dense, true));

            await comp.InvokeAsync(() => comp.Instance.OpenMenu());
            await provider.WaitForAssertionAsync(() => provider.FindAll("div.mud-list-item").Should().ContainSingle());
            await provider.Find("div.mud-list-item").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(1));
        }

        /// <summary>
        /// Opens the select with a primary-button press and waits for its options to render.
        /// </summary>
        /// <remarks>
        /// Only for components hosting a single select; pressing an open select closes it.
        /// </remarks>
        private static async Task OpenAsync<TComponent>(IRenderedComponent<TComponent> comp)
            where TComponent : IComponent
        {
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Should().NotBeEmpty());
        }

        /// <summary>
        /// Opens the select and clicks the option at <paramref name="index"/>.
        /// </summary>
        private static async Task PickOptionAsync<TComponent>(IRenderedComponent<TComponent> comp, int index)
            where TComponent : IComponent
        {
            await OpenAsync(comp);
            await comp.FindAll("div.mud-list-item")[index].ClickAsync();
        }

        /// <summary>
        /// Sends a keydown the way the key interceptor reports a key pressed on the select.
        /// </summary>
        private static Task PressAsync<T>(IRenderedComponent<MudSelect<T>> select, KeyInterceptorService keys, string key, bool altKey = false, bool ctrlKey = false)
        {
            return select.InvokeAsync(() => keys.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = key, AltKey = altKey, CtrlKey = ctrlKey, Type = "keydown" }));
        }

        /// <summary>
        /// Names the checkbox icon a multi-select option renders: checked, unchecked or indeterminate.
        /// </summary>
        private static string CheckboxState(IElement option)
        {
            var path = $"d=\"{option.QuerySelectorAll("path").Last().GetAttribute("d")}\"";

            if (Icons.Material.Filled.CheckBox.Contains(path))
            {
                return "checked";
            }

            if (Icons.Material.Filled.CheckBoxOutlineBlank.Contains(path))
            {
                return "unchecked";
            }

            return Icons.Material.Filled.IndeterminateCheckBox.Contains(path) ? "indeterminate" : $"unknown icon {path}";
        }
    }
}
