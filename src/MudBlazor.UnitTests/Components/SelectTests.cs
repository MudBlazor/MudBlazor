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
    public class SelectTests : BunitTest
    {
        /// <summary>
        /// Click should open the Menu and selecting a value should update the bindable value.
        /// </summary>
        [Test]
        public void SelectTest1()
        {
            var comp = Context.RenderComponent<SelectTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check popover class
            menu.ClassList.Should().Contain("select-popover-class");
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

            input.Click();
            items = comp.FindAll("div.mud-list-item").ToArray();

            items[0].Click();
            select.Instance.Value.Should().Be("1");
        }

        /// <summary>
        /// Click should not close the menu and selecting multiple values should update the bindable value with a comma separated list.
        /// </summary>
        [Test]
        public async Task MultiSelectTest1()
        {
            var comp = Context.RenderComponent<MultiSelectTest1>();
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
            select.Instance.SelectedValues.Count().Should().Be(2);
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
        /// Initial render fragment in input should be the pre-selected value's items's render fragment.
        /// After clicking the second item, the render fragment should update
        /// </summary>
        [Test]
        public async Task SelectWithEnumTest()
        {
            var comp = Context.RenderComponent<SelectWithEnumTest>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<MyEnum>>();
            var input = comp.Find("div.mud-input-control");

            select.Instance.Value.Should().Be(default(MyEnum));
            select.Instance.Text.Should().Be(default(MyEnum).ToString());
            await Task.Delay(50);

            comp.Find("div.mud-input-slot").TextContent.Trim().Should().Be("First");
            comp.RenderCount.Should().Be(1);

            //Console.WriteLine(comp.Markup);
            input.Click();
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            comp.Find("div.mud-input-slot").TextContent.Trim().Should().Be("Second");
            comp.RenderCount.Should().Be(3);
        }

        /// <summary>
        /// Initially we have a value of 17 which is not in the list. So we render it as text via MudInput
        /// </summary>
        [Test]
        public void SelectUnrepresentableValueTest()
        {
            var comp = Context.RenderComponent<SelectUnrepresentableValueTest>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<int>>();
            var input = comp.Find("div.mud-input-control");

            select.Instance.Value.Should().Be(17);
            select.Instance.Text.Should().Be("17");
            comp.FindAll("div.mud-input-slot").Count.Should().Be(0);
            //Console.WriteLine(comp.Markup);
            input.Click();

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
            var comp = Context.RenderComponent<SelectUnrepresentableValueTest2>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<int>>();
            var input = comp.Find("div.mud-input-control");

            select.Instance.Value.Should().Be(17);
            select.Instance.Text.Should().Be("17");
            await Task.Delay(100);
            // BUT: we have a select with Strict="true" so the Text will not be shown because it is not in the list of selectable values
            comp.FindComponent<MudInput<string>>().Instance.Value.Should().Be(null);
            comp.FindComponent<MudInput<string>>().Instance.InputType.Should().Be(InputType.Hidden);
            input.Click();

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
            var comp = Context.RenderComponent<SelectWithoutItemPresentersTest>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<int>>();
            var input = comp.Find("div.mud-input-control");

            select.Instance.Value.Should().Be(1);
            select.Instance.Text.Should().Be("1");
            comp.FindAll("div.mud-input-slot").Count.Should().Be(0);
            comp.RenderCount.Should().Be(1);
            //Console.WriteLine(comp.Markup);

            input.Click();
            comp.RenderCount.Should().Be(2);

            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            comp.FindAll("div.mud-input-slot").Count.Should().Be(0);
            select.Instance.Value.Should().Be(2);
            select.Instance.Text.Should().Be("2");
            comp.RenderCount.Should().Be(3);
        }

        [Test]
        public void Select_Should_FireTextChangedWithNewValue()
        {
            var comp = Context.RenderComponent<SelectTest1>();
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

            //open the menu again
            input.Click();
            items = comp.FindAll("div.mud-list-item").ToArray();

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
            var comp = Context.RenderComponent<SelectTest1>();
            Console.WriteLine(comp.Markup);
            var select = comp.FindComponent<MudSelect<string>>();
            string text = null;
            IEnumerable<string> selectedValues = null;
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

            input.Click();
            items = comp.FindAll("div.mud-list-item").ToArray();

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
            var comp = Context.RenderComponent<SelectTest1>();
            Console.WriteLine(comp.Markup);
            var select = comp.FindComponent<MudSelect<string>>();
            string text = null;
            IEnumerable<string> selectedValues = null;
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

            var selectElement = comp.Find("div.mud-input-control");
            selectElement.Click();

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
            var comp = Context.RenderComponent<SelectTest1>();
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
            var comp = Context.RenderComponent<SelectTest1>();
            var select = comp.FindComponent<MudSelect<string>>();
            Console.WriteLine(comp.Markup);

            var selectElement = comp.Find("div.mud-input-control");
            selectElement.Click();

            comp.FindAll("div.mud-list-item-disabled").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item-disabled")[0].Click();
            select.Instance.Value.Should().BeNull();
        }

        [Test]
        public async Task MultiSelect_ShouldCallValidationFunc()
        {
            var comp = Context.RenderComponent<MultiSelectTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            string validatedValue = null;
            select.SetParam(x => x.Validation, new Func<string, bool>(value =>
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
        public void MultiSelect_SelectAll()
        {
            var comp = Context.RenderComponent<MultiSelectTest2>();
            // select element needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            string validatedValue = null;
            select.SetParam(x => x.Validation, (object)new Func<string, bool>(value =>
            {
                validatedValue = value; // NOTE: select does only update the value for T string
                return true;
            }));
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // Open the menu
            input.Click();
            menu.ClassList.Should().Contain("mud-popover-open");
            // now click the first checkbox
            comp.FindAll("div.mud-list-item")[0].Click();
            // validate the result. all items should be selected
            select.Instance.Text.Should().Be("FirstA^SecondA^ThirdA");
            validatedValue.Should().Be("FirstA^SecondA^ThirdA");
        }

        [Test]
        public void MultiSelect_SelectAll2()
        {
            var comp = Context.RenderComponent<MultiSelectTest3>();
            // select element needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // Open the menu
            input.Click();
            menu.ClassList.Should().Contain("mud-popover-open");

            // get the first (select all item) and check if it is selected
            var selectAllItem = comp.FindComponent<MudListItem>();
            selectAllItem.Instance.Icon.Should().Be("<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z\"/>");

            // Check that all select items are actually selected
            var mudListItems = comp.FindComponents<MudSelectItem<string>>();

            mudListItems.Should().HaveCount(7 * 2);
            foreach (var item in mudListItems.Take(7))
            {
                item.Instance.IsSelected.Should().BeTrue();
                item.FindComponent<MudListItem>().Instance.Icon.Should().Be("<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z\"/>");
            }

            foreach (var item in mudListItems.Skip(7))
            {
                item.Instance.IsSelected.Should().BeTrue();
                Assert.Throws<Bunit.Rendering.ComponentNotFoundException>(() => item.FindComponent<MudListItem>());
            }
        }

        [Test]
        public void MultiSelect_SelectAll3()
        {
            var comp = Context.RenderComponent<MultiSelectTest4>();
            // select element needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // Open the menu
            input.Click();
            menu.ClassList.Should().Contain("mud-popover-open");
            // Check that the icon corresponds to an unchecked checkbox
            var mudListItem = comp.FindComponent<MudListItem>();
            mudListItem.Instance.Icon.Should().Be("<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z\"/>");
        }

        [Test]
        public void SingleSelect_Should_CallValidationFunc()
        {
            var comp = Context.RenderComponent<SelectTest1>();
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

            input.Click();
            items = comp.FindAll("div.mud-list-item").ToArray();
            items[0].Click();

            select.Instance.Value.Should().Be("1");
            select.Instance.Text.Should().Be("1");
            validatedValue.Should().Be("1");
        }

        /// <summary>
        /// We filled the multiselect with initial selected values, that must
        /// show in the value of the input as a comma separated list of strings
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task MultiSelect_Initial_Values()
        {
            var comp = Context.RenderComponent<MultiSelectWithInitialValues>();
            // print the generated html
            Console.WriteLine(comp.Markup);

            // select the input of the select
            var input = comp.Find("input");
            //the value of the input
            var value = input.Attributes.Where(a => a.LocalName == "value").First().Value;
            value.Should().Be("FirstA, SecondA");
        }

        /// <summary>
        /// We filled the multiselect with initial selected values.
        /// Then the returned text in the selection is customized.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task MultiSelectCustomizedTextTest()
        {
            var comp = Context.RenderComponent<MultiSelectCustomizedTextTest>();

            // Select the input of the select
            var input = comp.Find("input");

            // The value of the input
            var value = input.Attributes.Where(a => a.LocalName == "value").First().Value;

            // Value is equal to the customized values returned by the method
            value.Should().Be("Selected values: FirstA, SecondA");
        }

        [Test]
        public async Task SelectClearableTest()
        {
            var comp = Context.RenderComponent<SelectClearableTest>();
            var select = comp.FindComponent<MudSelect<string>>();
            var input = comp.Find("div.mud-input-control");

            // No button when initialized
            comp.FindAll("button").Should().BeEmpty();

            input.Click();
            // Button shows after selecting item
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            select.Instance.Value.Should().Be("2");
            comp.Find("button").Should().NotBeNull();
            // Selection cleared and button removed after clicking clear button
            comp.Find("button").Click();
            select.Instance.Value.Should().BeNullOrEmpty();
            comp.FindAll("button").Should().BeEmpty();
            // Clear button click handler should have been invoked
            comp.Instance.ClearButtonClicked.Should().BeTrue();
        }

        /// <summary>
        /// Reselect an already selected value should not call SelectedValuesChanged event.
        /// </summary>
        [Test]
        public void SelectReselectTest()
        {
            var comp = Context.RenderComponent<ReselectValueTest>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var select = comp.FindComponent<MudSelect<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");

            input.Click();
            select.Instance.Value.Should().Be("Apple");

            // now click an item and see the value change
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();

            // menu should be closed now
            menu.ClassList.Should().NotContain("mud-popover-open");
            select.Instance.Value.Should().Be("Orange");
            comp.Instance.ChangeCount.Should().Be(1);

            // now click an item and see the value change
            input.Click();

            items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();

            select.Instance.Value.Should().Be("Orange");
            comp.Instance.ChangeCount.Should().Be(1);

        }

        #region DataAttribute validation
        [Test]
        public async Task TextField_Should_Validate_Data_Attribute_Fail()
        {
            var comp = Context.RenderComponent<SelectValidationDataAttrTest>();
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
            var comp = Context.RenderComponent<SelectValidationDataAttrTest>();
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

        /// <summary>
        /// Tests the required property.
        /// </summary>
        [Test]
        public async Task Select_Should_SetRequiredTrue()
        {
            var comp = Context.RenderComponent<SelectRequiredTest>();
            var select = comp.FindComponent<MudSelect<string>>().Instance;
            select.Required.Should().BeTrue();
            await comp.InvokeAsync(() => select.Validate());
            select.ValidationErrors.First().Should().Be("Required");
        }

        /// <summary>
        /// Selected option should be hilighted when drop-down opens
        /// </summary>
        [Test]
        public async Task Select_Should_HilightSelectedValue()
        {
            var comp = Context.RenderComponent<SelectTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            var select = comp.FindComponent<MudSelect<string>>();
            var input = comp.Find("div.mud-input-control");

            comp.Find("div.mud-popover").ClassList.Should().Contain("select-popover-class");
            select.Instance.Value.Should().BeNullOrEmpty();
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
            // open the select
            comp.Find("div.mud-input-control").Click();
            comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open");
            // no option should be hilited
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            // now click an item and see the value change
            comp.FindAll("div.mud-list-item")[1].Click();
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
            select.Instance.Value.Should().Be("2");
            // open again and check hilited option
            comp.Find("div.mud-input-control").Click();
            comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open");
            // Nr 2 should be hilited
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item")[1].ToMarkup().Should().Contain("mud-selected-item");
            await comp.InvokeAsync(() => select.Instance.CloseMenu());
            select.SetParam(nameof(MudSelect<string>.Value), null);
            await comp.InvokeAsync(() => select.Instance.OpenMenu());
            // no option should be hilited
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
        }

        /// <summary>
        /// Initially selected option should be hilighted when drop-down opens
        /// </summary>
        [Test]
        public void Select_Should_HilightInitiallySelectedValue()
        {
            var comp = Context.RenderComponent<SelectTest2>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            var select = comp.FindComponent<MudSelect<string>>();
            comp.Find("div.mud-popover").ClassList.Should().Contain("select-popover-class");
            select.Instance.Value.Should().Be("2");
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
            // open the select
            comp.Find("div.mud-input-control").Click();
            comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open");
            // Nr 2 should be hilited
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item")[1].ToMarkup().Should().Contain("mud-selected-item");
            // now click an item and see the value change
            comp.FindAll("div.mud-list-item")[0].Click();
            comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open");
            select.Instance.Value.Should().Be("1");
            // open again and check hilited option
            comp.Find("div.mud-input-control").Click();
            comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open");
            // Nr 1 should be hilited
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindAll("div.mud-list-item")[0].ToMarkup().Should().Contain("mud-selected-item");
        }
    }
}
