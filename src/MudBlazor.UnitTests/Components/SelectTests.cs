#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static MudBlazor.UnitTests.TestComponents.SelectWithEnumTest;

namespace MudBlazor.UnitTests.Components
{

    [TestFixture]
    public class SelectTests
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
        /// Click should open the Menu and selecting a value should update the bindable value.
        /// </summary>
        [Test]
        public void SelectTest1()
        {
            var comp = ctx.RenderComponent<SelectTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check initial state
            select.Instance.Value.Should().BeNullOrEmpty();
            menu.ClassList.Should().NotContain("mud-popover-open");
            // click and check if it has toggled the menu
            input.Click();
            menu.ClassList.Should().Contain("mud-popover-open");
            // now click an item and see the value change
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            // menu should be closed now
            menu.ClassList.Should().NotContain("mud-popover-open");
            select.Instance.Value.Should().Be("2");
            // now we cheat and click the list without opening the menu ;)
            items[0].Click();
            select.Instance.Value.Should().Be("1");
        }

        /// <summary>
        /// Click should not close the menu and selecting multiple values should update the bindable value with a comma separated list.
        /// </summary>
        [Test]
        public async Task MultiSelectTest1()
        {
            var comp = ctx.RenderComponent<MultiSelectTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check initial state
            select.Instance.Value.Should().BeNullOrEmpty();
            menu.ClassList.Should().NotContain("mud-popover-open");
            // click and check if it has toggled the menu
            input.Click();
            menu.ClassList.Should().Contain("mud-popover-open");
            // now click an item and see the value change
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            // menu should still be open now!!
            menu.ClassList.Should().Contain("mud-popover-open");
            select.Instance.Text.Should().Be("2");
            items[0].Click();
            select.Instance.Text.Should().Be("2, 1");
            items[2].Click();
            select.Instance.Text.Should().Be("2, 1, 3");
            items[0].Click();
            select.Instance.Text.Should().Be("2, 3");
            select.Instance.SelectedValues.Count.Should().Be(2);
            select.Instance.SelectedValues.Should().Contain("2");
            select.Instance.SelectedValues.Should().Contain("3");
            //Console.WriteLine(comp.Markup);
            const string @unchecked =
                "M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z";
            const string @checked =
                "M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z";
            var icons = comp.FindAll("div.mud-list-item path").ToArray();
            // check that the correct items are checked
            icons[1].Attributes["d"].Value.Should().Be(@unchecked);
            icons[3].Attributes["d"].Value.Should().Be(@checked);
            icons[5].Attributes["d"].Value.Should().Be(@checked);
            // now check how setting the SelectedValues makes items checked or unchecked
            await comp.InvokeAsync(() =>
            {
                select.Instance.SelectedValues = new HashSet<string>() { "1", "2" };
            });
            icons = comp.FindAll("div.mud-list-item path").ToArray();
            icons[1].Attributes["d"].Value.Should().Be(@checked);
            icons[3].Attributes["d"].Value.Should().Be(@checked);
            icons[5].Attributes["d"].Value.Should().Be(@unchecked);
            Console.WriteLine(comp.Markup);
        }

        /// <summary>
        /// Initial Text should be enums default value
        /// Initial render fragement in input should be the pre-selected value's items's render fragment.
        /// After clicking the second item, the render fragment should update
        /// </summary>
        [Test]
        public async Task SelectWithEnumTest()
        {
            var comp = ctx.RenderComponent<SelectWithEnumTest>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<MyEnum>>();
            select.Instance.Value.Should().Be(default(MyEnum));
            select.Instance.Text.Should().Be(default(MyEnum).ToString());
            await Task.Delay(50);
            comp.Find("div.mud-input-slot").TextContent.Trim().Should().Be("First");
            comp.RenderCount.Should().Be(1);
            //Console.WriteLine(comp.Markup);
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            comp.Find("div.mud-input-slot").TextContent.Trim().Should().Be("Second");
            comp.RenderCount.Should().Be(2);
        }

        /// <summary>
        /// Initially we have a value of 17 which is not in the list. So we render it as text via MudInput
        /// </summary>
        [Test]
        public void SelectUnrepresentableValueTest()
        {
            var comp = ctx.RenderComponent<SelectUnrepresentableValueTest>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<int>>();
            select.Instance.Value.Should().Be(17);
            select.Instance.Text.Should().Be("17");
            comp.FindAll("div.mud-input-slot").Count.Should().Be(0);
            //Console.WriteLine(comp.Markup);
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            comp.Find("div.mud-input-slot").TextContent.Trim().Should().Be("Two");
            select.Instance.Value.Should().Be(2);
            select.Instance.Text.Should().Be("2");
        }

        /// <summary>
        /// Don't show initial value which is not in list because of Strict=true.
        /// </summary>
        [Test]
        public async Task SelectUnrepresentableValueTest2()
        {
            var comp = ctx.RenderComponent<SelectUnrepresentableValueTest2>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<int>>();
            select.Instance.Value.Should().Be(17);
            select.Instance.Text.Should().Be("17");
            await Task.Delay(100);
            // BUT: we have a select with Strict="true" so the Text will not be shown because it is not in the list of selectable values
            comp.FindComponent<MudInput<string>>().Instance.Value.Should().Be(null);
            comp.FindComponent<MudInput<string>>().Instance.InputType.Should().Be(InputType.Hidden);
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            select.Instance.Value.Should().Be(2);
            select.Instance.Text.Should().Be("2");
            comp.FindComponent<MudInput<string>>().Instance.Value.Should().Be("2");
            comp.FindComponent<MudInput<string>>().Instance.InputType.Should().Be(InputType.Text); // because list item has no render fragment, so we show it as text
        }

        /// <summary>
        /// The items have no render fragments, so instead of RF the select must display the converted string value
        /// </summary>
        [Test]
        public void SelectWithoutItemPresentersTest()
        {
            var comp = ctx.RenderComponent<SelectWithoutItemPresentersTest>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<int>>();
            select.Instance.Value.Should().Be(1);
            select.Instance.Text.Should().Be("1");
            comp.FindAll("div.mud-input-slot").Count.Should().Be(0);
            comp.RenderCount.Should().Be(1);
            //Console.WriteLine(comp.Markup);
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            comp.FindAll("div.mud-input-slot").Count.Should().Be(0);
            select.Instance.Value.Should().Be(2);
            select.Instance.Text.Should().Be("2");
            comp.RenderCount.Should().Be(2);
        }

        [Test]
        public void Select_Should_FireTextChangedWithNewValue()
        {
            var comp = ctx.RenderComponent<SelectTest1>();
            Console.WriteLine(comp.Markup);
            var select = comp.FindComponent<MudSelect<string>>();
            string text = null;
            select.SetCallback(s => s.TextChanged, x => text = x);
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check initial state
            select.Instance.Value.Should().BeNullOrEmpty();
            menu.ClassList.Should().NotContain("mud-popover-open");
            // click and check if it has toggled the menu
            input.Click();
            menu.ClassList.Should().Contain("mud-popover-open");
            // now click an item and see the value change
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            // menu should be closed now
            menu.ClassList.Should().NotContain("mud-popover-open");
            select.Instance.Value.Should().Be("2");
            select.Instance.Text.Should().Be("2");
            text.Should().Be("2");
            // now we cheat and click the list without opening the menu ;)
            items[0].Click();
            select.Instance.Value.Should().Be("1");
            select.Instance.Text.Should().Be("1");
            text.Should().Be("1");
        }

        /// <summary>
        /// SingleSelect: SelectedValuesChanged should be fired before TextChanged
        /// We test this by checking the counter. The event which should be fired first must always
        /// find an even counter value, the second must always find an odd value.
        /// </summary>
        [Test]
        public void SingleSelect_Should_FireTextChangedBeforeSelectedValuesChanged()
        {
            var comp = ctx.RenderComponent<SelectTest1>();
            Console.WriteLine(comp.Markup);
            var select = comp.FindComponent<MudSelect<string>>();
            string text = null;
            HashSet<string> selectedValues = null;
            var eventCounter = 0;
            var textChangedCount = 0;
            var selectedValuesChangedCount = 0;
            select.SetCallback(s => s.TextChanged, x =>
              {
                  textChangedCount = eventCounter++;
                  text = x;
              });
            select.SetCallback(s => s.SelectedValuesChanged, x =>
              {
                  selectedValuesChangedCount = eventCounter++;
                  selectedValues = x;
              });
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check initial state
            select.Instance.Value.Should().BeNullOrEmpty();
            menu.ClassList.Should().NotContain("mud-popover-open");
            // click and check if it has toggled the menu
            input.Click();
            menu.ClassList.Should().Contain("mud-popover-open");
            // now click an item and see the value change
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            // menu should be closed now
            menu.ClassList.Should().NotContain("mud-popover-open");
            select.Instance.Value.Should().Be("2");
            select.Instance.Text.Should().Be("2");
            text.Should().Be("2");
            selectedValuesChangedCount.Should().Be(1);
            textChangedCount.Should().Be(0);
            string.Join(",", selectedValues).Should().Be("2");
            // now we cheat and click the list without opening the menu ;)
            items[0].Click();
            select.Instance.Value.Should().Be("1");
            select.Instance.Text.Should().Be("1");
            text.Should().Be("1");
            string.Join(",", selectedValues).Should().Be("1");
            selectedValuesChangedCount.Should().Be(3);
            textChangedCount.Should().Be(2);
        }

        /// <summary>
        /// MultiSelect: SelectedValuesChanged should be fired before TextChanged
        /// We test this by checking the counter. The event which should be fired first must always
        /// find an even counter value, the second must always find an odd value.
        /// </summary>
        [Test]
        public void MulitSelect_Should_FireTextChangedBeforeSelectedValuesChanged()
        {
            var comp = ctx.RenderComponent<SelectTest1>();
            Console.WriteLine(comp.Markup);
            var select = comp.FindComponent<MudSelect<string>>();
            string text = null;
            HashSet<string> selectedValues = null;
            var eventCounter = 0;
            var textChangedCount = 0;
            var selectedValuesChangedCount = 0;
            select.SetParam(s => s.MultiSelection, true);
            select.SetCallback(s => s.TextChanged, x =>
              {
                  textChangedCount = eventCounter++;
                  text = x;
              });
            select.SetCallback(s => s.SelectedValuesChanged, x =>
              {
                  selectedValuesChangedCount = eventCounter++;
                  selectedValues = x;
              });
            var items = comp.FindAll("div.mud-list-item").ToArray();
            // click list item
            items[1].Click();
            select.Instance.Value.Should().Be("2");
            select.Instance.Text.Should().Be("2");
            text.Should().Be("2");
            selectedValuesChangedCount.Should().Be(1);
            textChangedCount.Should().Be(0);
            string.Join(",", selectedValues).Should().Be("2");
            // click another list item
            items = comp.FindAll("div.mud-list-item").ToArray();
            items[0].Click();
            select.Instance.Value.Should().Be("2, 1");
            select.Instance.Text.Should().Be("2, 1");
            text.Should().Be("2, 1");
            string.Join(",", selectedValues).Should().Be("2,1");
            selectedValuesChangedCount.Should().Be(3);
            textChangedCount.Should().Be(2);
        }

        [Test]
        public void Select_Should_FireOnBlur()
        {
            var comp = ctx.RenderComponent<SelectTest1>();
            Console.WriteLine(comp.Markup);
            var select = comp.FindComponent<MudSelect<string>>();
            var eventCounter = 0;
            select.SetCallback(s => s.OnBlur, x => eventCounter++);
            comp.InvokeAsync(() =>
            {
                select.Instance.OpenMenu();
                select.Instance.CloseMenu();
            });
            eventCounter.Should().Be(1);
        }

        [Test]
        public void Disabled_SelectItem_Should_Be_Respected()
        {
            var comp = ctx.RenderComponent<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("div.mud-list-item-disabled").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item-disabled")[0].Click();
            select.Instance.Value.Should().BeNull();
        }

        [Test]
        public async Task MultiSelect_ShouldCallValidationFunc()
        {
            var comp = ctx.RenderComponent<MultiSelectTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            string validatedValue = null;
            select.SetParam(x => x.Validation, (object)new Func<string, bool>(value =>
              {
                  validatedValue = value; // NOTE: select does only update the value for T string
                return true;
              }));
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check initial state
            select.Instance.Value.Should().BeNullOrEmpty();
            menu.ClassList.Should().NotContain("mud-popover-open");
            // click and check if it has toggled the menu
            input.Click();
            menu.ClassList.Should().Contain("mud-popover-open");
            // now click an item and see the value change
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            // menu should still be open now!!
            menu.ClassList.Should().Contain("mud-popover-open");
            select.Instance.Text.Should().Be("2");
            validatedValue.Should().Be("2");
            items[0].Click();
            select.Instance.Text.Should().Be("2, 1");
            validatedValue.Should().Be("2, 1");
            items[2].Click();
            select.Instance.Text.Should().Be("2, 1, 3");
            validatedValue.Should().Be("2, 1, 3");
            items[0].Click();
            select.Instance.Text.Should().Be("2, 3");
            validatedValue.Should().Be("2, 3");
        }

        [Test]
        public void SingleSelect_Should_CallValidatonFunc()
        {
            var comp = ctx.RenderComponent<SelectTest1>();
            Console.WriteLine(comp.Markup);
            var select = comp.FindComponent<MudSelect<string>>();
            string validatedValue = null;
            select.SetParam(x => x.Validation, (object)new Func<string, bool>(value =>
            {
                validatedValue = value; // NOTE: select does only update the value for T string
                return true;
            }));
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check initial state
            select.Instance.Value.Should().BeNullOrEmpty();
            menu.ClassList.Should().NotContain("mud-popover-open");
            // click and check if it has toggled the menu
            input.Click();
            menu.ClassList.Should().Contain("mud-popover-open");
            // now click an item and see the value change
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            // menu should be closed now
            menu.ClassList.Should().NotContain("mud-popover-open");
            select.Instance.Value.Should().Be("2");
            select.Instance.Text.Should().Be("2");
            validatedValue.Should().Be("2");
            // now we cheat and click the list without opening the menu ;)
            items[0].Click();
            select.Instance.Value.Should().Be("1");
            select.Instance.Text.Should().Be("1");
            validatedValue.Should().Be("1");
        }


        #region DataAttribute validation
        [Test]
        public async Task TextField_Should_Validate_Data_Attribute_Fail()
        {
            var comp = ctx.RenderComponent<SelectValidationDataAttrTest>();
            Console.WriteLine(comp.Markup);
            var selectcomp = comp.FindComponent<MudSelect<string>>();
            var select = selectcomp.Instance;
            // Select invalid option
            await comp.InvokeAsync(() => select.SelectOption("Quux"));
            // check initial state
            select.Value.Should().Be("Quux");
            select.Text.Should().Be("Quux");
            // check validity
            await comp.InvokeAsync(() => select.Validate());
            select.ValidationErrors.Should().NotBeEmpty();
            select.ValidationErrors.Should().HaveCount(1);
            select.ValidationErrors[0].Should().Equals("Should not be longer than 3");
        }

        [Test]
        public async Task TextField_Should_Validate_Data_Attribute_Success()
        {
            var comp = ctx.RenderComponent<SelectValidationDataAttrTest>();
            Console.WriteLine(comp.Markup);
            var selectcomp = comp.FindComponent<MudSelect<string>>();
            var select = selectcomp.Instance;
            // Select valid option
            await comp.InvokeAsync(() => select.SelectOption("Qux"));
            // check initial state
            select.Value.Should().Be("Qux");
            select.Text.Should().Be("Qux");
            // check validity
            await comp.InvokeAsync(() => select.Validate());
            select.ValidationErrors.Should().BeEmpty();
        }
        #endregion
    }
}
