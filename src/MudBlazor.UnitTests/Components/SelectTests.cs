using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
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
        [Test]
        public async Task Select_CheckListClass()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<SelectRequiredTest>();
            var select = comp.FindComponent<MudSelect<string>>();
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Enter" }));
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ListClass, "my-list-class"));
            await comp.WaitForAssertionAsync(() => comp.Markup.Should().Contain("my-list-class"));
        }

        [Test]
        public async Task Select_CheckLayerClass()
        {
            var comp = Context.Render<MudSelect<string>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.OuterClass, "my-outer-class")
                .Add(x => x.Class, "my-main-class")
                .Add(x => x.InputClass, "my-input-class"));
            await comp.WaitForAssertionAsync(() => comp.Markup.Should().Contain("my-outer-class"));
            await comp.WaitForAssertionAsync(() => comp.Markup.Should().Contain("my-main-class"));
            await comp.WaitForAssertionAsync(() => comp.Markup.Should().Contain("my-input-class"));
        }

        /// <summary>
        /// Select id should propagate to label for attribute
        /// </summary>
        [Test]
        public void SelectLabelFor()
        {
            var comp = Context.Render<SelectRequiredTest>();
            var label = comp.FindAll(".mud-input-label");
            label[0].Attributes.GetNamedItem("for")?.Value.Should().Be("selectLabelTest");
        }

        /// <summary>
        /// Click should open the Menu and selecting a value should update the bindable value.
        /// </summary>
        [Test]
        public async Task SelectTest1()
        {
            var comp = Context.Render<SelectTest1>();
            // print the generated html
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            IElement Menu() => comp.Find("div.mud-popover");
            IElement Input() => comp.Find("div.mud-input-control");
            // check popover class
            Menu().ClassList.Should().Contain("select-popover-class");
            // check initial state
            select.Instance.ReadValue.Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() => Menu().ClassList.Should().NotContain("mud-popover-open"));
            // click and check if it has toggled the menu
            await Input().MouseDownAsync();
            await comp.WaitForAssertionAsync(() => Menu().ClassList.Should().Contain("mud-popover-open"));
            // now click an item and see the value change
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            IReadOnlyList<IElement> Items() => comp.FindAll("div.mud-list-item");
            await Items()[1].ClickAsync();
            // menu should be closed now
            await comp.WaitForAssertionAsync(() => Menu().ClassList.Should().NotContain("mud-popover-open"));
            select.Instance.ReadValue.Should().Be("2");
            // now we cheat and click the list without opening the menu ;)

            await Input().MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));

            await Items()[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("1"));
            //Check user on blur implementation works
            IElement Switch() => comp.Find("#switch");
            await Switch().ChangeAsync(true);
            await comp.WaitForAssertionAsync(() => Switch().HasAttribute("checked").Should().BeTrue());
            await comp.InvokeAsync(() => select.Instance.OnBlurAsync(new FocusEventArgs()));
            await comp.WaitForAssertionAsync(() => Switch().HasAttribute("checked").Should().BeFalse());
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
            var select = comp.FindComponent<MudSelect<SelectCustomToStringTest.Pizza>>();
            var menu = comp.Find("div.mud-popover");
            IElement Input() => comp.Find("input[value]");
            // check popover class
            menu.ClassList.Should().Contain("select-popover-class");
            // check initial state
            select.Instance.ReadValue.Should().NotBeNull();
            Input().GetAttribute("value").Should().Be("Diavolo");
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            // click and check if it has toggled the menu
            await Input().MouseDownAsync();
            menu.ClassList.Should().Contain("mud-popover-open");
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[0].TextContent.Should().Be("Cardinale");
            items[1].TextContent.Should().Be("Diavolo");
            items[2].TextContent.Should().Be("Margarita");
            items[3].TextContent.Should().Be("Spinaci");
            await items[2].ClickAsync();
            Input().GetAttribute("value").Should().Be("Margarita");
        }

        [Test]
        [NonParallelizable]
        public async Task Select_KeyDown_WhileClosed()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<SelectFocusAndTypeTest>();
            var select = comp.FindComponent<MudSelect<string>>();

            //open menu on keydown
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "t", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Tennessee"));

            //cycle through matching results
            await Task.Delay(210);
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "t", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Texas"));
            await Task.Delay(210);
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "t", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Tennessee"));

            //multi-string search
            await Task.Delay(210);
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "c", Type = "keydown" }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "o", Type = "keydown" }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "l", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Colorado"));

            //paused search
            await Task.Delay(210);
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "i", Type = "keydown" }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "o", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Iowa"));

            await Task.Delay(210);
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "i", Type = "keydown" }));
            await Task.Delay(210);
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
            var inputs = comp.FindAll("input");
            inputs.Count.Should().Be(3);
            inputs[1].GetAttribute("value").Should().Be("Value2");
            await inputs[1].MouseDownAsync();
            await Task.Delay(500);
            var listItems = comp.FindAll(".mud-list-item");
            foreach (var listItem in listItems)
            {
                await listItem.ClickAsync();
            }

            inputs = comp.FindAll("input");
            inputs[0].GetAttribute("value").Should().Be("Value3, Value1");
            inputs[1].GetAttribute("value").Should().Be("Value3; Value1");
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
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<MyEnum>>();
            var input = comp.Find("div.mud-input-control");

            select.Instance.ReadValue.Should().Be(default(MyEnum));
            select.Instance.ReadText.Should().Be(default(MyEnum).ToString());

            comp.Find("input").Attributes["value"]?.Value.Should().Be("First");
            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            var items = comp.FindAll("div.mud-list-item").ToArray();
            await items[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("input").Attributes["value"]?.Value.Should().Be("Second"));
        }

        /// <summary>
        /// Initial Text should be enums default value
        /// Initial render fragment in input should be the pre-selected value's items's render fragment.
        /// After clicking the second item, the render fragment should update
        /// </summary>
        [Test]
        public async Task MultiSelectWithEnum()
        {
            var comp = Context.Render<MultiSelectWithEnumTest>();
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<MultiSelectWithEnumTest.MyEnum>>();
            var input = comp.Find("div.mud-input-control");

            select.Instance.GetState(x => x.SelectedValues).Should().BeEmpty();

            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            var items = comp.FindAll("div.mud-list-item").ToArray();

            const string @unchecked =
                "M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z";
            const string @checked =
                "M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z";
            // Validate that none of the items are selected
            comp.FindAll("div.mud-list-item path:not(:first-child)").Should().AllSatisfy(item =>
                item.Attributes["d"]!.Value.Should().Be(@unchecked)
            );
            // Select the first item
            await items[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("input").Attributes["value"]?.Value.Should().Be("First"));
            await comp.WaitForAssertionAsync(() =>
                select.Instance.GetState(x => x.SelectedValues).Should().OnlyContain(item => item == MultiSelectWithEnumTest.MyEnum.First)
            );
            await comp.WaitForAssertionAsync(() =>
            {
                // Assert that the first item is checked
                comp.FindAll("div.mud-list-item path:not(:first-child)")[0].Attributes["d"]!.Value.Should().Be(@checked);
                // Remaining items should be unchecked
                comp.FindAll("div.mud-list-item:not(:first-child) path:not(:first-child)").Should().AllSatisfy(item =>
                    item.Attributes["d"]!.Value.Should().Be(@unchecked)
                );
            });
        }

        [Test]
        public async Task MultiSelect_ChildlessEnumItems_ShouldUpdateCheckboxImmediately()
        {
            const string uncheckedIcon =
                "M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z";
            const string checkedIcon =
                "M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z";

            var comp = Context.Render<MultiSelectChildlessEnumToStringFuncTest>();
            var select = comp.FindComponent<MudSelect<MultiSelectChildlessEnumToStringFuncTest.Pizza>>();

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().Be(4));

            IReadOnlyList<IElement> Items() => comp.FindAll("div.mud-list-item");

            await comp.WaitForAssertionAsync(() => GetCheckboxPath(Items()[0]).Should().Be(uncheckedIcon));

            await Items()[1].ClickAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("Diavolo"));
            await comp.WaitForAssertionAsync(() => GetCheckboxPath(Items()[1]).Should().Be(checkedIcon));
            await comp.WaitForAssertionAsync(() => GetCheckboxPath(Items()[0]).Should().Be(uncheckedIcon));
        }

        [Test]
        public async Task MultiSelect_ChildlessStringItems_ShouldUpdateCheckboxImmediately()
        {
            const string uncheckedIcon =
                "M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z";
            const string checkedIcon =
                "M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z";

            var comp = Context.Render<MultiSelectChildlessStringTest>();
            var select = comp.FindComponent<MudSelect<string>>();

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().Be(4));

            IReadOnlyList<IElement> Items() => comp.FindAll("div.mud-list-item");

            await comp.WaitForAssertionAsync(() => GetCheckboxPath(Items()[2]).Should().Be(uncheckedIcon));

            await Items()[2].ClickAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("Margarita"));
            await comp.WaitForAssertionAsync(() => GetCheckboxPath(Items()[2]).Should().Be(checkedIcon));
            await comp.WaitForAssertionAsync(() => GetCheckboxPath(Items()[0]).Should().Be(uncheckedIcon));
        }

        [Test]
        public async Task MultiSelect_SelectAll_ShouldUpdateChildlessItemCheckboxesImmediately()
        {
            const string checkedIcon =
                "M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z";

            var comp = Context.Render<MultiSelectChildlessSelectAllTest>();
            var select = comp.FindComponent<MudSelect<string>>();

            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().Be(5));

            IReadOnlyList<IElement> Items() => comp.FindAll("div.mud-list-item");

            await Items()[0].ClickAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().HaveCount(4));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("Cardinale, Diavolo, Margarita, Spinaci"));
            await comp.WaitForAssertionAsync(() => Items().Skip(1).Should().AllSatisfy(item => GetCheckboxPath(item).Should().Be(checkedIcon)));
        }

        /// <summary>
        /// Initially we have a value of 17 which is not in the list. So we render it as text via MudInput
        /// </summary>
        [Test]
        public async Task SelectUnrepresentableValue()
        {
            var comp = Context.Render<SelectUnrepresentableValueTest>();
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<int>>();
            var input = comp.Find("div.mud-input-control");
            select.Instance.ReadValue.Should().Be(17);
            select.Instance.ReadText.Should().Be("17");
            comp.Find("input").Attributes["value"]?.Value.Should().Be("17");
            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            var items = comp.FindAll("div.mud-list-item").ToArray();
            await items[1].ClickAsync();
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
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<int>>();
            var input = comp.Find("div.mud-input-control");

            select.Instance.ReadValue.Should().Be(17);
            select.Instance.ReadText.Should().Be("17");
            await Task.Delay(100);
            // BUT: we have a select with Strict="true" so the Text will not be shown because it is not in the list of selectable values
            comp.FindComponent<MudInput<string>>().Instance.ReadValue.Should().Be(null);
            comp.FindComponent<MudInput<string>>().Instance.InputType.Should().Be(InputType.Hidden);
            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            var items = comp.FindAll("div.mud-list-item").ToArray();
            await items[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be(2));
            select.Instance.ReadText.Should().Be("2");
            comp.FindComponent<MudInput<string>>().Instance.ReadValue.Should().Be("2");
            comp.FindComponent<MudInput<string>>().Instance.InputType.Should().Be(InputType.Text); // because list item has no render fragment, so we show it as text
        }

        /// <summary>
        /// When the select has a null value, the text should be displayed, and the mud-shrink class should be applied.
        /// </summary>
        [Test]
        public async Task SelectNullValue()
        {
            var comp = Context.Render<SelectNullValueTest>();
            var select = comp.FindComponent<MudSelect<int?>>();

            // Initial state: null value
            select.Instance.ReadValue.Should().Be(null);
            select.Find("div.mud-input-slot").TextContent.Should().Be("None");
            select.Markup.Should().Contain("mud-shrink");

            // Open menu and select a non-null value
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            await comp.FindAll("div.mud-list-item").ToArray()[1].ClickAsync(); // Select "One" (value = 1)

            // Verify non-null value
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be(1));
            select.Find("div.mud-input-slot").TextContent.Should().Be("One");
            select.Markup.Should().Contain("mud-shrink");

            // Open menu again and select null value
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            await comp.FindAll("div.mud-list-item").ToArray()[0].ClickAsync(); // Select "None" (value = null)

            // Verify back to null value
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be(null));
            select.Find("div.mud-input-slot").TextContent.Should().Be("None");
            select.Markup.Should().Contain("mud-shrink");
        }

        /// <summary>
        /// RegisterShadowItem should not throw when the item parameter is null.
        /// </summary>
        [Test]
        public void SelectRegisterShadowItemNull()
        {
            var comp = Context.Render<SelectNullValueTest>();
            var select = comp.FindComponent<MudSelect<int?>>();
            IMudSelect mudSelect = select.Instance;
            var context = (MudSelectContext<int?>)mudSelect.SelectContext;

            var registerAction = () => context.RegisterShadowItem(null);

            registerAction.Should().NotThrow();
        }

        /// <summary>
        /// RegisterShadowItem should not throw when the item's Value property is null.
        /// </summary>
        [Test]
        public void SelectRegisterShadowItemWithNullValue()
        {
            var comp = Context.Render<SelectNullValueTest>();
            var select = comp.FindComponent<MudSelect<int?>>();
            var itemWithNullValue = Context.Render<MudSelectItem<int?>>(parameters => parameters.Add(x => x.Value, null));
            IMudSelect mudSelect = select.Instance;
            var context = (MudSelectContext<int?>)mudSelect.SelectContext;

            var registerAction = () => context.RegisterShadowItem(itemWithNullValue.Instance);

            registerAction.Should().NotThrow();
        }

        /// <summary>
        /// UnregisterShadowItem should not throw when the item parameter is null.
        /// </summary>
        [Test]
        public void SelectUnregisterShadowItemNull()
        {
            var comp = Context.Render<SelectNullValueTest>();
            var select = comp.FindComponent<MudSelect<int?>>();
            IMudSelect mudSelect = select.Instance;
            var context = (MudSelectContext<int?>)mudSelect.SelectContext;

            var unregisterAction = () => context.UnregisterShadowItem(null);

            unregisterAction.Should().NotThrow();
        }

        /// <summary>
        /// UnregisterShadowItem should not throw when the item's Value property is null.
        /// </summary>
        [Test]
        public void SelectUnregisterShadowItemWithNullValue()
        {
            var comp = Context.Render<SelectNullValueTest>();
            var select = comp.FindComponent<MudSelect<int?>>();
            var itemWithNullValue = Context.Render<MudSelectItem<int?>>(parameters => parameters.Add(x => x.Value, null));
            IMudSelect mudSelect = select.Instance;
            var context = (MudSelectContext<int?>)mudSelect.SelectContext;

            context.RegisterShadowItem(itemWithNullValue.Instance);
            var unregisterAction = () => context.UnregisterShadowItem(itemWithNullValue.Instance);

            unregisterAction.Should().NotThrow();
        }

        /// <summary>
        /// The items have no render fragments, so instead of RF the select must display the converted string value
        /// </summary>
        [Test]
        public async Task SelectWithoutItemPresenters()
        {
            var comp = Context.Render<SelectWithoutItemPresentersTest>();
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<int>>();
            var input = comp.Find("div.mud-input-control");

            select.Instance.ReadValue.Should().Be(1);
            select.Instance.ReadText.Should().Be("1");
            comp.Find("div.mud-input-slot").Attributes["style"].Value.Should().Contain("display:none");

            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            var items = comp.FindAll("div.mud-list-item").ToArray();
            await items[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-input-slot").Attributes["style"].Value.Should().Contain("display:none"));
            select.Instance.ReadValue.Should().Be(2);
            select.Instance.ReadText.Should().Be("2");
        }

        [Test]
        public async Task Select_Should_FireTextChangedWithNewValue()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            string text = null;
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(s => s.TextChanged, (Action<string>)(x => text = x)));
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check initial state
            select.Instance.ReadValue.Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            // click and check if it has toggled the menu
            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            menu.ClassList.Should().Contain("mud-popover-open");
            // now click an item and see the value change
            var items = comp.FindAll("div.mud-list-item");
            await items[1].ClickAsync();
            // menu should be closed now
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("2"));
            select.Instance.ReadText.Should().Be("2");
            text.Should().Be("2");

            //open the menu again
            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            items = comp.FindAll("div.mud-list-item");

            await items[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("1"));
            select.Instance.ReadText.Should().Be("1");
            text.Should().Be("1");
        }

        /// <summary>
        /// SingleSelect: SelectedValuesChanged should be fired before TextChanged
        /// We test this by checking the counter. The event which should be fired first must always
        /// find an even counter value, the second must always find an odd value.
        /// </summary>
        [Test]
        public async Task SingleSelect_Should_FireTextChangedBeforeSelectedValuesChanged()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            string text = null;
            IEnumerable<string> selectedValues = null;
            var eventCounter = 0;
            var textChangedCount = 0;
            var selectedValuesChangedCount = 0;
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(s => s.TextChanged, x =>
            {
                textChangedCount = eventCounter++;
                text = x;
            }));
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(s => s.SelectedValuesChanged, x =>
            {
                selectedValuesChangedCount = eventCounter++;
                selectedValues = x;
            }));
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check initial state
            select.Instance.ReadValue.Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            // click and check if it has toggled the menu
            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            menu.ClassList.Should().Contain("mud-popover-open");
            // now click an item and see the value change
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            var items = comp.FindAll("div.mud-list-item");
            await items[1].ClickAsync();
            // menu should be closed now
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("2"));
            select.Instance.ReadText.Should().Be("2");
            text.Should().Be("2");
            selectedValuesChangedCount.Should().Be(1);
            textChangedCount.Should().Be(0);
            string.Join(",", selectedValues).Should().Be("2");

            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            items = comp.FindAll("div.mud-list-item").ToArray();

            await items[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("1"));
            select.Instance.ReadText.Should().Be("1");
            text.Should().Be("1");
            string.Join(",", selectedValues).Should().Be("1");
            await comp.WaitForAssertionAsync(() => selectedValuesChangedCount.Should().Be(3));
            await comp.WaitForAssertionAsync(() => textChangedCount.Should().Be(2));
        }

        /// <summary>
        /// MultiSelect: SelectedValuesChanged should be fired before TextChanged
        /// We test this by checking the counter. The event which should be fired first must always
        /// find an even counter value, the second must always find an odd value.
        /// </summary>
        [Test]
        public async Task MulitSelect_Should_FireTextChangedBeforeSelectedValuesChanged()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            string text = null;
            IEnumerable<string> selectedValues = null;
            var eventCounter = 0;
            var textChangedCount = 0;
            var selectedValuesChangedCount = 0;
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(s => s.MultiSelection, true));
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(s => s.TextChanged, (Action<string>)(x =>
              {
                  textChangedCount = eventCounter++;
                  text = x;
              })));
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(s => s.SelectedValuesChanged, (Action<IReadOnlyCollection<string>>)(x =>
              {
                  selectedValuesChangedCount = eventCounter++;
                  selectedValues = x;
              })));

            var selectElement = comp.Find("div.mud-input-control");
            await selectElement.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            var items = comp.FindAll("div.mud-list-item").ToArray();
            // click list item
            await items[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("2"));
            select.Instance.ReadText.Should().Be("2");
            text.Should().Be("2");
            selectedValuesChangedCount.Should().Be(1);
            textChangedCount.Should().Be(0);
            string.Join(",", selectedValues).Should().Be("2");
            // click another list item
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            items = comp.FindAll("div.mud-list-item").ToArray();
            await items[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("2, 1"));
            select.Instance.ReadText.Should().Be("2, 1");
            text.Should().Be("2, 1");
            string.Join(",", selectedValues).Should().Be("2,1");
            selectedValuesChangedCount.Should().Be(3);
            textChangedCount.Should().Be(2);
        }

        [Test]
        public async Task Select_Should_FireOnBlur()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            var eventCounter = 0;
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(s => s.OnBlur, () => eventCounter++));
            await comp.InvokeAsync(async () =>
            {
                await select.Instance.OpenMenu();
                await select.Instance.CloseMenu();
            });
            eventCounter.Should().Be(1);
        }

        [Test]
        public async Task Disabled_SelectItem_Should_Be_Respected()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();

            var selectElement = comp.Find("div.mud-input-control");
            await selectElement.MouseDownAsync();

            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item-disabled").Count.Should().Be(1));
            await comp.FindAll("div.mud-list-item-disabled")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().BeNull());
        }

        [Test]
        public async Task MultiSelect_ShouldCallValidationFunc()
        {
            var comp = Context.Render<MultiSelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            string validatedValue = null;
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, new Func<string, bool>(value =>
            {
                validatedValue = value; // NOTE: select does only update the value for T string
                return true;
            })));
            select.Instance.ReadValue.Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() =>
                comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            await comp.WaitForAssertionAsync(() =>
                comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() =>
                comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("2"));
            validatedValue.Should().Be("2");
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("2, 1"));
            validatedValue.Should().Be("2, 1");
            await comp.FindAll("div.mud-list-item")[2].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("2, 1, 3"));
            validatedValue.Should().Be("2, 1, 3");
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("2, 3"));
            validatedValue.Should().Be("2, 3");
        }

        [Test]
        public async Task MultiSelect_SelectAll()
        {
            var comp = Context.Render<MultiSelectTest2>();
            // select element needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            string validatedValue = null;
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, (object)new Func<string, bool>(value =>
            {
                validatedValue = value; // NOTE: select does only update the value for T string
                return true;
            })));
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // Open the menu
            await input.MouseDownAsync();
            menu.ClassList.Should().Contain("mud-popover-open");
            // now click the first checkbox
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            // validate the result. all items should be selected
            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("FirstA^SecondA^ThirdA"));
            validatedValue.Should().Be("FirstA^SecondA^ThirdA");
        }

        [Test]
        public async Task MultiSelect_SelectAll2()
        {
            var comp = Context.Render<MultiSelectTest3>();
            // select element needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // Open the menu
            await input.MouseDownAsync();
            menu.ClassList.Should().Contain("mud-popover-open");

            // get the first (select all item) and check if it is selected
            var selectAllItem = comp.FindComponent<MudListItem<string>>();
            selectAllItem.Instance.Icon.Should().Be("<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z\"/>");

            // Check that all normal select items are actually selected
            var items = comp.FindComponents<MudSelectItem<string>>().Where(x => x.Instance.HideContent == false).ToArray();

            items.Should().HaveCount(7);
            foreach (var item in items)
            {
                item.Instance.Selected.Should().BeTrue();
                item.FindComponent<MudListItem<string>>().Instance.Icon.Should().Be("<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z\"/>");
            }

            // Check shadow items
            var shadowItems = comp.FindComponents<MudSelectItem<string>>().Where(x => x.Instance.HideContent).ToArray();
            foreach (var item in shadowItems)
            {
                // shadow items don't render, their state is irrelevant, all they do is provide render fragments to the select
                Assert.Throws<Bunit.Rendering.ComponentNotFoundException>(() => item.FindComponent<MudListItem<string>>());
            }
        }

        [Test]
        public async Task MultiSelect_SelectAll3()
        {
            var comp = Context.Render<MultiSelectTest4>();
            // select element needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // Open the menu
            await input.MouseDownAsync();
            menu.ClassList.Should().Contain("mud-popover-open");
            // Check that the icon corresponds to an unchecked checkbox
            var mudListItem = comp.FindComponent<MudListItem<string>>();
            mudListItem.Instance.Icon.Should().Be("<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z\"/>");
        }

        [Test]
        public async Task MultiSelect_SelectAll4()
        {
            var comp = Context.Render<MultiSelectTest7>();
            // select element needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // Open the menu
            await input.MouseDownAsync();
            menu.ClassList.Should().Contain("mud-popover-open");
            // now click the first checkbox to select all
            var items = comp.FindAll("div.mud-list-item").ToArray();
            select.Instance.GetState(x => x.SelectedValues).Should().HaveCount(0);
            await items[0].ClickAsync();
            // validate the result. all items that are not disabled should be selected
            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().HaveCount(3));
            select.Instance.GetState(x => x.SelectedValues).ElementAt(0).Should().Be("FirstA");
            select.Instance.GetState(x => x.SelectedValues).ElementAt(1).Should().Be("SecondA");
            select.Instance.GetState(x => x.SelectedValues).ElementAt(2).Should().Be("ThirdA");
            // now click the first checkbox again to unselect all
            await items[0].ClickAsync();
            // validate the result. all items should be un-selected
            await comp.WaitForAssertionAsync(() => select.Instance.GetState(x => x.SelectedValues).Should().HaveCount(0));
        }

        [Test]
        public async Task SingleSelect_Should_CallValidationFunc()
        {
            var comp = Context.Render<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            string validatedValue = null;
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, (object)new Func<string, bool>(value =>
            {
                validatedValue = value; // NOTE: select does only update the value for T string
                return true;
            })));
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check initial state
            select.Instance.ReadValue.Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            // click and check if it has toggled the menu
            await input.MouseDownAsync();
            menu.ClassList.Should().Contain("mud-popover-open");
            // now click an item and see the value change
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));

            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            // menu should be closed now
            await comp.WaitForAssertionAsync(() => menu.ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("2"));
            select.Instance.ReadText.Should().Be("2");
            validatedValue.Should().Be("2");

            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("1"));
            select.Instance.ReadText.Should().Be("1");
            validatedValue.Should().Be("1");
        }

        /// <summary>
        /// We filled the multiselect with initial selected values, that must
        /// show in the value of the input as a comma separated list of strings
        /// </summary>
        [Test]
        public void MultiSelect_Initial_Values()
        {
            var comp = Context.Render<MultiSelectWithInitialValuesTest>();
            // print the generated html

            // select the input of the select
            var input = comp.Find("input");
            //the value of the input
            var value = input.Attributes.First(a => a.LocalName == "value").Value;
            value.Should().Be("FirstA, SecondA");
        }

        /// <summary>
        /// We filled the multiselect with initial selected values.
        /// Then the returned text in the selection is customized.
        /// </summary>
        [Test]
        public void MultiSelectCustomizedText()
        {
            var comp = Context.Render<MultiSelectCustomizedTextTest>();

            // Select the input of the select
            var input = comp.Find("input");

            // The value of the input
            var value = input.Attributes.First(a => a.LocalName == "value").Value;

            // Value is equal to the customized values returned by the method
            value.Should().Be("Selected values: FirstA, SecondA");
        }

        [Test]
        public async Task SelectClearable()
        {
            var comp = Context.Render<SelectClearableTest>();
            var select = comp.FindComponent<MudSelect<string>>();

            // Initial state – no clear button
            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();

            // Open select
            await comp.InvokeAsync(async () =>
            {
                var input = comp.Find("div.mud-input-control");
                await input.MouseDownAsync();
            });

            // Wait for items to render
            await comp.WaitForAssertionAsync(() =>
                comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));

            // Select second item
            await comp.InvokeAsync(async () =>
            {
                var items = comp.FindAll("div.mud-list-item");
                await items[1].ClickAsync();
            });

            // Popover closes
            await comp.WaitForAssertionAsync(() =>
                comp.Find("div.mud-popover")
                    .ClassList.Should().NotContain("mud-popover-open"));

            // Value is set
            select.Instance.ReadValue.Should().Be("2");

            // Clear button appears
            comp.FindAll(".mud-input-clear-button").Should().ContainSingle();

            // Click clear button
            var clearButton = comp.Find(".mud-input-clear-button");
            await clearButton.ClickAsync();

            // Value cleared
            await comp.WaitForAssertionAsync(() =>
                select.Instance.ReadValue.Should().BeNullOrEmpty());

            // Clear button removed
            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();

            // Clear handler invoked
            comp.Instance.ClearButtonClicked.Should().BeTrue();
        }

        /// <summary>
        /// Reselect an already selected value should not call SelectedValuesChanged event.
        /// </summary>
        [Test]
        public async Task SelectReselect()
        {
            var comp = Context.Render<ReselectValueTest>();
            // print the generated html
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");

            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            select.Instance.ReadValue.Should().Be("Apple");

            // now click an item and see the value change
            var items = comp.FindAll("div.mud-list-item").ToArray();
            await items[1].ClickAsync();

            // menu should be closed now
            await comp.WaitForAssertionAsync(() => menu.ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Orange"));
            comp.Instance.ChangeCount.Should().Be(1);

            // now click an item and see the value change
            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            items = comp.FindAll("div.mud-list-item").ToArray();
            await items[1].ClickAsync();

            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Orange"));
            comp.Instance.ChangeCount.Should().Be(1);

        }

        #region DataAttribute validation
        [Test]
        public async Task TextField_Should_Validate_Data_Attribute_Fail()
        {
            var comp = Context.Render<SelectValidationDataAttrTest>();
            var selectcomp = comp.FindComponent<MudSelect<string>>();
            var select = selectcomp.Instance;
            // Select invalid option
            await comp.InvokeAsync(() => select.SelectOption("Quux"));
            // check initial state
            select.Value.Should().Be("Quux");
            select.ReadText.Should().Be("Quux");
            // check validity
            await comp.InvokeAsync(() => select.ValidateAsync());
            select.ValidationErrors.Should().NotBeEmpty();
            select.ValidationErrors.Should().HaveCount(1);
            select.ValidationErrors[0].Should().Be("Should not be longer than 3");
        }

        [Test]
        public async Task TextField_Should_Validate_Data_Attribute_Success()
        {
            var comp = Context.Render<SelectValidationDataAttrTest>();
            var selectcomp = comp.FindComponent<MudSelect<string>>();
            var select = selectcomp.Instance;
            // Select valid option
            await comp.InvokeAsync(() => select.SelectOption("Qux"));
            // check initial state
            select.Value.Should().Be("Qux");
            select.ReadText.Should().Be("Qux");
            // check validity
            await comp.InvokeAsync(() => select.ValidateAsync());
            select.ValidationErrors.Should().BeEmpty();
        }
        #endregion

        /// <summary>
        /// Tests the required property.
        /// </summary>
        [Test]
        public async Task Select_Should_SetRequiredTrue()
        {
            var comp = Context.Render<SelectRequiredTest>();
            var select = comp.FindComponent<MudSelect<string>>().Instance;
            select.Required.Should().BeTrue();
            await comp.InvokeAsync(() => select.ValidateAsync());
            select.ValidationErrors.First().Should().Be("Required");
        }

        /// <summary>
        /// Selected option should be hilighted when drop-down opens
        /// </summary>
        [Test]
        public async Task Select_Should_HilightSelectedValue()
        {
            var comp = Context.Render<SelectTest1>();
            // print the generated html
            var select = comp.FindComponent<MudSelect<string>>();
            var input = comp.Find("div.mud-input-control");

            comp.Find("div.mud-popover").ClassList.Should().Contain("select-popover-class");
            select.Instance.ReadValue.Should().BeNullOrEmpty();
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
            // open the select
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            // no option should be hilited
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(0));
            // now click an item and see the value change
            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("2"));
            // open again and check hilited option
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            // Nr 2 should be hilited
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(1));
            comp.FindAll("div.mud-list-item")[1].ToMarkup().Should().Contain("mud-selected-item");
            await comp.InvokeAsync(() => select.Instance.CloseMenu());
            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, null));
            await comp.InvokeAsync(() => select.Instance.OpenMenu());
            // no option should be hilited
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(0));
        }

        /// <summary>
        /// Initially selected option should be hilighted when drop-down opens
        /// </summary>
        [Test]
        public async Task Select_Should_HilightInitiallySelectedValue()
        {
            var comp = Context.Render<SelectTest2>();
            // print the generated html
            var select = comp.FindComponent<MudSelect<string>>();
            comp.Find("div.mud-popover").ClassList.Should().Contain("select-popover-class");
            select.Instance.ReadValue.Should().Be("2");
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
            // open the select
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            // Nr 2 should be highlighted
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(1));
            comp.FindAll("div.mud-list-item")[1].ToMarkup().Should().Contain("mud-selected-item");
            // now click an item and see the value change
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("1"));
            // open again and check highlighted option
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            // Nr 1 should be highlighted
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(1));
            comp.FindAll("div.mud-list-item")[0].ToMarkup().Should().Contain("mud-selected-item");
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
        }

        [Test]
        public async Task Select_Should_AllowReloadingItems()
        {
            var comp = Context.Render<ReloadSelectItemsTest>();
            var select = comp.FindComponent<MudSelect<string>>();
            // normal, without reloading
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("American Samoa"));
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Arizona"));
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.FindAll("div.mud-list-item")[2].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Arkansas"));
            // reloading!
            await comp.Find(".reload").ClickAsync();
            // check again, different values expected now
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Alabama"));
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.FindAll("div.mud-list-item")[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("Alaska"));
            await comp.Find("div.mud-input-control").MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.FindAll("div.mud-list-item")[2].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.ReadValue.Should().Be("American Samoa"));
        }

        [Test]
        public async Task Select_ToggleOpenCloseMenuMethods()
        {
            var comp = Context.Render<SelectTest1>();
            // print the generated html
            // select elements needed for the test

            var select = comp.FindComponent<MudSelect<string>>();

            await comp.InvokeAsync(() => select.Instance.ToggleMenu());
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));
            await comp.InvokeAsync(() => select.Instance.ToggleMenu());
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.WaitForAssertionAsync(() => select.Instance.Items.Count.Should().Be(4));

            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, false));
            await comp.InvokeAsync(() => select.Instance.ToggleMenu());
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));

            await select.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));
            await comp.InvokeAsync(() => select.Instance.ToggleMenu());
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));

            await comp.InvokeAsync(() => select.Instance.OpenMenu());
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
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
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<SelectTest3>();
            // print the generated html
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowDown", AltKey = true, Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));

            // ArrowDown should move the highlight but NOT change the value
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowDown", Type = "keydown" })); // Move to "1"
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowDown", Type = "keydown" })); // Move to "2"
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowUp", Type = "keydown" })); // Move to "1"

            // Value is still null/default even though we moved focus
            await comp.WaitForAssertionAsync(() => select.Instance.Value.Should().BeNull());

            // Confirm selection with Enter
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(select.Instance.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown" }));

            // Now the value should be "1" and popover should close
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
            var mudSelectElement = comp.Find(".mud-select");
            await comp.Find("div.mud-input-control").MouseDownAsync();
            select.Instance.GetState(x => x.Open).Should().BeTrue();
            var items = comp.FindAll("div.mud-list-item").ToArray();
            await items[0].ClickAsync();
            await items[2].ClickAsync();
            //emulate focus out
            await mudSelectElement.FocusOutAsync();
            await comp.WaitForAssertionAsync(() => select.Instance.ReadText.Should().Be("Alaska, Alabama, American Samoa"));
            //check if we received focus event from the MudSelect.OnFocusOutAsync
            Context.JSInterop.VerifyFocusAsyncInvoke();
        }

        [Test]
        public async Task Select_ItemlessSelect()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<MudSelect<string>>();

            // print the generated html

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(comp.Instance.ElementId, new KeyboardEventArgs { Key = " ", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(comp.Instance.ElementId, new KeyboardEventArgs { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(comp.Instance.ElementId, new KeyboardEventArgs { Key = "Home", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(comp.Instance.ElementId, new KeyboardEventArgs { Key = "End", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(comp.Instance.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Instance.GetState(x => x.SelectedValues).Should().HaveCount(0));
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(null));
        }

        [Test]
        public async Task MultiSelectWithCustomComparer()
        {
            var comp = Context.Render<MultiSelectWithCustomComparerTest>();
            // print the generated html
            // Click select button
            await comp.Find("#set-selection-button").ClickAsync();
            // Check input text
            comp.Find("input").GetAttribute("value").Should().Be("Selected Cafe Latte, Selected Espresso");
            // Click to render the menu
            await comp.Find("div.mud-input-control").MouseDownAsync();
            // Check check marks
            const string @unchecked =
                "M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z";
            const string @checked =
                "M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z";
            var icons = comp.FindAll("div.mud-list-item path").ToArray();
            icons[1].Attributes["d"].Value.Should().Be(@unchecked);
            icons[3].Attributes["d"].Value.Should().Be(@checked);
            icons[5].Attributes["d"].Value.Should().Be(@checked);
            icons[7].Attributes["d"].Value.Should().Be(@unchecked);
        }

        [Test]
        public async Task Select_Item_Collection_Should_Match_Number_Of_Select_Options()
        {
            var comp = Context.Render<SelectTest1>();
            var sut = comp.FindComponent<MudSelect<string>>();

            var input = comp.Find("div.mud-input-control");
            await input.MouseDownAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));

            sut.Instance.Items.Should().HaveCountGreaterThanOrEqualTo(4);
        }

        /// <summary>
        /// When MultiSelection and Required are True with no selected values, required validation should fail.
        /// </summary>
        [Test]
        public async Task MultiSelectWithRequiredValue()
        {
            //1a. Check When SelectedItems is empty - Validation Should Fail
            //Check on String type
            var comp = Context.Render<MultiSelectTestRequiredValue>();
            var select = comp.FindComponent<MudSelect<string>>().Instance;
            select.Required.Should().BeTrue();
            await comp.InvokeAsync(() => select.ValidateAsync());
            select.ValidationErrors.First().Should().Be("Required");

            //1b. Check on T type - MultiSelect of T(e.g. class object)
            var selectWithT = comp.FindComponent<MudSelect<MultiSelectTestRequiredValue.TestClass>>().Instance;
            selectWithT.Required.Should().BeTrue();
            await comp.InvokeAsync(() => selectWithT.ValidateAsync());
            selectWithT.ValidationErrors.First().Should().Be("Required");

            //2a. Now check when SelectedItems is greater than one - Validation Should Pass
            var inputs = comp.FindAll("div.mud-input-control");
            await inputs[0].MouseDownAsync();//The 2nd one is the
            var items = comp.FindAll("div.mud-list-item").ToArray();
            await items[1].ClickAsync();
            await comp.InvokeAsync(() => select.ValidateAsync());
            select.ValidationErrors.Count.Should().Be(0);

            //2b.
            await inputs[1].MouseDownAsync();//selectWithT
            //wait for render and it will find 5 items from the component
            comp.WaitForState(() => comp.FindAll("div.mud-list-item").Count == 5);
            items = comp.FindAll("div.mud-list-item").ToArray();
            await items[3].ClickAsync();
            await comp.InvokeAsync(() => selectWithT.ValidateAsync());
            selectWithT.ValidationErrors.Count.Should().Be(0);
        }

        [Test]
        public async Task MultiSelectClearAndReset()
        {
            var comp = Context.Render<MultiSelectTestRequiredValue>();
            var select = comp.FindComponent<MudSelect<string>>().Instance;
            select.Required.Should().BeTrue();
            await comp.InvokeAsync(() => select.ValidateAsync());
            select.ValidationErrors.First().Should().Be("Required");

            await comp.Find("#clear-string").ClickAsync();
            select.ValidationErrors.First().Should().Be("Required");

            await comp.Find("#reset-string").ClickAsync();
            select.ValidationErrors.Should().BeEmpty();

            //test clearing string values
            var inputs = comp.FindAll("div.mud-input-control");
            await inputs[0].MouseDownAsync();

            var items = comp.FindAll("div.mud-list-item").ToArray();
            await items[1].ClickAsync();
            await inputs[0].MouseDownAsync();
            select.Value.Should().Be("2");
            select.GetState(x => x.SelectedValues).Should().Contain("2");

            await comp.Find("#clear-string").ClickAsync();

            select.Value.Should().BeNullOrEmpty();
            select.GetState(x => x.SelectedValues).Should().BeEmpty();
            select.ValidationErrors.First().Should().Be("Required");

            //test resetting string values
            inputs = comp.FindAll("div.mud-input-control");
            await inputs[0].MouseDownAsync();
            items = comp.FindAll("div.mud-list-item").ToArray();
            await items[1].ClickAsync();
            await inputs[0].MouseDownAsync();
            select.Value.Should().Be("2");
            select.GetState(x => x.SelectedValues).Should().Contain("2");

            await comp.Find("#reset-string").ClickAsync();

            select.Value.Should().BeNullOrEmpty();
            select.GetState(x => x.SelectedValues).Should().BeEmpty();
            select.ValidationErrors.Should().BeEmpty();

            //test clearing object values
            var select2 = comp.FindComponent<MudSelect<MultiSelectTestRequiredValue.TestClass>>().Instance;
            select2.Required.Should().BeTrue();
            await comp.InvokeAsync(() => select2.ValidateAsync());
            select2.ValidationErrors.First().Should().Be("Required");

            await comp.Find("#clear-object").ClickAsync();
            select2.ValidationErrors.First().Should().Be("Required");

            await comp.Find("#reset-object").ClickAsync();
            select2.ValidationErrors.Should().BeEmpty();

            inputs = comp.FindAll("div.mud-input-control");
            await inputs[1].MouseDownAsync();

            items = comp.FindAll("div.mud-list-item").ToArray();
            await items[1].ClickAsync();
            await inputs[1].MouseDownAsync();
            select2.SelectedValues.Select(x => x.Name).Should().Contain("Customer");

            await comp.Find("#clear-object").ClickAsync();

            select2.SelectedValues.Should().BeEmpty();
            select2.ValidationErrors.First().Should().Be("Required");

            //test resetting object values
            inputs = comp.FindAll("div.mud-input-control");
            await inputs[1].MouseDownAsync();
            items = comp.FindAll("div.mud-list-item").ToArray();
            await items[1].ClickAsync();
            await inputs[1].MouseDownAsync();
            select2.SelectedValues.Select(x => x.Name).Should().Contain("Customer");

            await comp.Find("#reset-object").ClickAsync();

            select2.SelectedValues.Should().BeEmpty();
            select2.ValidationErrors.Should().BeEmpty();
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
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(comp.Find("input").Id);
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
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
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
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
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

            comp.Find(".mud-input-adornment-icon").Attributes.GetNamedItem("aria-label")!.Value.Should().Be(ariaLabel);
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

        [Test]
        public async Task ReadOnlyShouldNotHaveClearButton()
        {
            var comp = Context.Render<MudSelect<string>>(p => p
                .Add(x => x.Text, "some value")
                .Add(x => x.Clearable, true)
                .Add(x => x.ReadOnly, false));

            comp.FindAll(".mud-input-clear-button").Count.Should().Be(1);

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ReadOnly, true)); //no clear button when readonly
            comp.FindAll(".mud-input-clear-button").Count.Should().Be(0);
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

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));

            //Open restricted popover
            await comp.Find("#restricted-select").MouseDownAsync();

            //confirm relative width class
            comp.Find(".restricted").ClassList.Should().Contain("mud-popover-open").And.Contain("mud-popover-relative-width");

            //close popover
            await comp.Find("#restricted-select").MouseDownAsync();

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));

            //Open expanded popover
            await comp.Find("#expanded-select").MouseDownAsync();

            //confirm relative width class not applied
            comp.Find(".expanded").ClassList.Should().Contain("mud-popover-open").And.NotContain("mud-popover-relative-width");
        }

        [Test]
        public async Task SelectFitContent()
        {
            var comp = Context.Render<SelectFitContentTest>();

            //default values
            comp.Instance.FullWidth.Should().BeFalse();
            comp.Instance.FitContent.Should().BeFalse();

            var select = comp.Find(".mud-select");

            select.ClassList.Should().NotContain("mud-width-content");

            //set fit content
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(c => c.FitContent, true));

            comp.Instance.FullWidth.Should().BeFalse();
            comp.Instance.FitContent.Should().BeTrue();

            select.ClassList.Should().Contain("mud-width-content");

            var filler = comp.Find(".mud-select-filler");

            filler.ClassList.Should().Contain("d-inline-block").And.Contain("mx-4");
            filler.TextContent.Trim().Should().Be("Federated States of Micronesia");

            //set full width
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(c => c.FullWidth, true));

            comp.Instance.FullWidth.Should().BeTrue();
            comp.Instance.FitContent.Should().BeTrue();

            select.ClassList.Should().NotContain("mud-width-content");
        }

        [Test]
        public void SelectFitContent_InitiallyEnabled()
        {
            var comp = Context.Render<SelectFitContentTest>(parameters => parameters
                .Add(x => x.FitContent, true));

            comp.Instance.FullWidth.Should().BeFalse();
            comp.Instance.FitContent.Should().BeTrue();

            var select = comp.Find(".mud-select");
            select.ClassList.Should().Contain("mud-width-content");

            var filler = comp.Find(".mud-select-filler");
            filler.TextContent.Trim().Should().Be("Federated States of Micronesia");
        }

        [TestCaseSource(typeof(MouseEventArgsTestCase), nameof(MouseEventArgsTestCase.AllCombinations))]
        [Test]
        public async Task Select_HandleMouseDown(MouseEventArgs args)
        {
            var comp = Context.Render<MudSelect<string>>(p => p
                .Add(x => x.Text, "some value")
                .Add(x => x.Clearable, true)
                .Add(x => x.ReadOnly, false));

            var instance = comp.Instance;

            instance.GetState(x => x.Open).Should().BeFalse();

            await comp.InvokeAsync(async () => await instance.HandleMouseDown(args));

            switch (args.Button)
            {
                case 0:
                    instance.GetState(x => x.Open).Should().BeTrue();
                    break;
                case 1:
                case 2:
                    instance.GetState(x => x.Open).Should().BeFalse();
                    break;
            }
        }

        [Test]
        public async Task SelectMultiSelectFieldChanged()
        {
            var comp = Context.Render<SelectMultiSelectFieldChangedTest>();

            //default values
            comp.Instance.FormFieldChangedEventArgs.Should().BeNull();

            //open the popover
            var input = comp.Find("div.mud-input-control");
            await input.MouseDownAsync();

            //click an item and see the value change
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0));
            await comp.Find(".mud-list-item").ClickAsync();

            await comp.WaitForAssertionAsync(() => comp.Instance.FormFieldChangedEventArgs.Should().NotBeNull());
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().BeEquivalentTo(comp.Instance.States.Take(2).Reverse());
        }

        [Test]
        public async Task SelectOpenTwoWay()
        {
            var comp = Context.Render<SelectOpenTwoBindTest>();
            var selectComponentInsaInstance = comp.FindComponent<MudSelect<string>>().Instance;
            IElement SwitchElement() => comp.Find("#switch");

            var input = comp.Find("div.mud-input-control");
            // Open the menu
            await input.MouseDownAsync();

            SwitchElement().HasAttribute("checked").Should().BeTrue();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            comp.Instance.Open.Should().BeTrue();
            selectComponentInsaInstance.GetState(x => x.Open).Should().BeTrue();

            // Close the menu
            var items = comp.FindAll("div.mud-list-item");
            await items[1].ClickAsync();

            SwitchElement().HasAttribute("checked").Should().BeFalse();
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            comp.Instance.Open.Should().BeFalse();
            selectComponentInsaInstance.GetState(x => x.Open).Should().BeFalse();

            // Open the menu using the switch
            await SwitchElement().ChangeAsync(true);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            comp.Instance.Open.Should().BeTrue();
            selectComponentInsaInstance.GetState(x => x.Open).Should().BeTrue();

            // Close the menu using the switch
            await SwitchElement().ChangeAsync(false);
            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            comp.Instance.Open.Should().BeFalse();
            selectComponentInsaInstance.GetState(x => x.Open).Should().BeFalse();
        }

        [Test]
        public void PopoverSettings_SetsDefaultValues()
        {
            var select = Context.Render<MudSelect<string>>();

            select.Instance.PopoverFixed.Should().BeFalse();
            // When not set, should use global default from PopoverOptions
            select.Instance.Modal.Should().BeNull();
        }

        [Test]
        public void PopoverSettings_OverridesDefaultValues()
        {
            var select = Context.Render<MudSelect<string>>(parameters =>
            {
                parameters.Add(x => x.PopoverFixed, true);
                parameters.Add(x => x.Modal, true);
            });

            select.Instance.PopoverFixed.Should().BeTrue();
            select.Instance.Modal.Should().BeTrue();
        }

        [Test]
        public void PopoverSettings_UsesGlobalDefaultsFromPopoverOptions()
        {
            // The default PopoverOptions should have OverflowBehavior.FlipAlways and ModalOverlay = false
            var select = Context.Render<MudSelect<string>>();

            // Verify that the component is using the global defaults
            // Modal should be null (using PopoverOptions defaults)
            select.Instance.Modal.Should().BeNull();
        }

        [Test]
        public async Task Select_ToStringFunc_ShouldTakePrecedenceOverChildContent()
        {
            var comp = Context.Render<SelectPrecedenceTest>();
            var selectComponent = comp.FindComponent<MudSelect<string>>();
            var select = selectComponent.Instance;

            // 1. Initially item1 is selected. ToStringFunc returns null for item1.
            // Should fall back to RenderFragment.
            var displaySlots = comp.FindAll("div.mud-input-slot");
            var displaySlot = displaySlots.FirstOrDefault(x => x.GetAttribute("style")?.Contains("display:inline") == true || x.GetAttribute("style")?.Contains("display: inline") == true);
            displaySlot.Should().NotBeNull("initially it should fall back to RenderFragment");
            displaySlot.InnerHtml.Should().Contain("custom-render");
            displaySlot.TextContent.Trim().Should().Be("Item 1 Rendered");

            // 2. Select item2. ToStringFunc returns "ITEM2" (not null).
            // Should use ToStringFunc and NOT RenderFragment.
            await comp.InvokeAsync(() => select!.SelectOption("item2"));
            comp.Render();

            displaySlots = comp.FindAll("div.mud-input-slot");
            displaySlot = displaySlots.FirstOrDefault(x => x.GetAttribute("style")?.Contains("display:inline") == true || x.GetAttribute("style")?.Contains("display: inline") == true);
            displaySlot.Should().BeNull("because ToStringFunc should take precedence over RenderFragment when it returns a non-null value");

            var input = comp.Find("input");
            input.GetAttribute("value").Should().Be("ITEM2");
        }

        [Test]
        public async Task Select_FitContent_ShouldPrioritizeToStringFunc()
        {
            var comp = Context.Render<SelectPrecedenceTest>();
            var selectComponent = comp.FindComponent<MudSelect<string>>();

            // Remove label to avoid it being the longest
            await selectComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.FitContent, true)
                .Add(x => x.Label, null)
                .Add(x => x.ToStringFunc, new Func<string?, string?>(x => x == "item2" ? "VERY LONG ITEM 2" : null)));

            // item1 -> null -> "Item 1 Rendered" (15 chars)
            // item2 -> "VERY LONG ITEM 2" (16 chars)

            // item2 is longest. ToStringFunc is NOT null for item2.
            // filler should use "VERY LONG ITEM 2" and NOT RenderFragment.

            var filler = comp.Find(".mud-select-filler");
            filler.TextContent.Should().Contain("VERY LONG ITEM 2");
            filler.InnerHtml.Should().NotContain("custom-render");

            // Now make item1 longest via ToStringFunc
            await selectComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ToStringFunc, new Func<string?, string?>(x => x == "item1" ? "EXTREMELY LONG ITEM 1" : "ITEM 2")));

            // Trigger recalculation of _longestItem by toggling FitContent
            await selectComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.FitContent, false));
            await selectComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.FitContent, true));

            filler = comp.Find(".mud-select-filler");
            filler.TextContent.Should().Contain("EXTREMELY LONG ITEM 1");
            filler.InnerHtml.Should().NotContain("custom-render");
        }

        private static string GetCheckboxPath(IElement item)
        {
            return item.QuerySelectorAll("path").Last().GetAttribute("d")!;
        }
    }
}
