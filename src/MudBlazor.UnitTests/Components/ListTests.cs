using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.List;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ListTests : BunitTest
    {

        [Test]
        public void ListRenderTest()
        {
            var comp = Context.RenderComponent<ListSelectionTest>();
            var listItem = comp.FindComponent<MudListItem<string>>();
            comp.Markup.Should().Contain("Sparkling Water");
            comp.Markup.Should().NotContain("Roger Waters");
            comp.Markup.Should().NotContain("High Hopes");
            listItem.SetParam("Text", "Roger Waters");
            listItem.SetParam("SecondaryText", "High Hopes");
            comp.Markup.Should().NotContain("Sparkling Water");
            comp.Markup.Should().Contain("Roger Waters");
            comp.Markup.Should().Contain("High Hopes");
        }

        /// <summary>
        /// <para>Clicking the drinks selects them. The child lists are updated accordingly, meaning, only ever 1 list item can have the active class.</para>
        /// <para>In this test no item is selected to begin with</para>
        /// </summary>
        [Test]
        public void ListSelectionTest()
        {
            var comp = Context.RenderComponent<ListSelectionTest>();
            var list = comp.FindComponent<MudList<string>>().Instance;
            list.SelectedValue.Should().Be(null);
            // we have seven choices, none is active
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            // click water
            comp.FindAll("div.mud-list-item")[0].Click();
            list.SelectedValue.Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            comp.FindAll("div.mud-list-item")[4].Click();
            list.SelectedValue.Should().Be("Pu'er");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[4].Markup.Should().Contain("mud-selected-item");
            // click Cafe Latte
            comp.FindAll("div.mud-list-item")[8].Click();
            list.SelectedValue.Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[8].Markup.Should().Contain("mud-selected-item");
            // click Cafe Latte again which should NOT deselect it because we are in single-selection mode
            comp.FindAll("div.mud-list-item")[8].Click();
            list.SelectedValue.Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[8].Markup.Should().Contain("mud-selected-item");
        }

        [Test]
        public void ListToggleSelectionTest()
        {
            var comp = Context.RenderComponent<ListSelectionTest>(self => self.Add(x => x.SelectionMode, SelectionMode.ToggleSelection));
            var list = comp.FindComponent<MudList<string>>().Instance;
            list.SelectedValue.Should().Be(null);
            // we have seven choices, none is active
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            // click water
            comp.FindAll("div.mud-list-item")[0].Click();
            list.SelectedValue.Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            comp.FindAll("div.mud-list-item")[4].Click();
            list.SelectedValue.Should().Be("Pu'er");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[4].Markup.Should().Contain("mud-selected-item");
            // click Cafe Latte
            comp.FindAll("div.mud-list-item")[8].Click();
            list.SelectedValue.Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[8].Markup.Should().Contain("mud-selected-item");
            // click Cafe Latte again which should deselect it because we are in toggle-selection mode
            comp.FindAll("div.mud-list-item")[8].Click();
            list.SelectedValue.Should().Be(null);
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            comp.FindComponents<MudListItem<string>>()[8].Markup.Should().NotContain("mud-selected-item");
        }

        [Test]
        public void ListMultiSelectionInitialValuesTest()
        {
            var comp = Context.RenderComponent<ListMultiSelectionTest>(self => self.Add(x => x.SelectedValues, ["Milk", "Cafe Latte"]));
            var list = comp.FindComponent<MudList<string>>().Instance;
            comp.Find("p.selected-values").TrimmedText().Should().Be("Cafe Latte, Milk");
            var GetCheckBox = (string text) => comp.FindComponents<MudListItem<string>>().FirstOrDefault(x => x.Instance.Text == text)?.FindComponent<MudCheckBox<bool?>>().Instance;
            GetCheckBox("Milk").Value.Should().Be(true);
            GetCheckBox("Cafe Latte").Value.Should().Be(true);
        }

        [Test]
        public void ListMultiSelectionBindingTest()
        {
            var comp = Context.RenderComponent<ListMultiSelectionBindingTest>();
            var list1 = comp.FindComponents<MudList<string>>().FirstOrDefault(x => x.Instance.Class == "list-1");
            var list2 = comp.FindComponents<MudList<string>>().FirstOrDefault(x => x.Instance.Class == "list-2");
            list1.FindComponents<MudListItem<string>>().Count.Should().Be(8);
            var GetCheckBox = (IRenderedComponent<MudList<string>> list, string text) => list.FindComponents<MudListItem<string>>()
                        .FirstOrDefault(x => x.Instance.Text == text)?.FindComponent<MudCheckBox<bool?>>().Instance;
            var Select = (IRenderedComponent<MudList<string>> list, string text) => list.FindComponents<MudListItem<string>>()
                        .FirstOrDefault(x => x.Instance.Text == text)?.Find("div.mud-list-item").Click();
            // click water on list1
            Select(list1, "Sparkling Water");
            comp.Find("p.selected-values").TrimmedText().Should().Be("Carbonated H²O");
            GetCheckBox(list1, "Milk").Value.Should().Be(false);
            GetCheckBox(list1, "Sparkling Water").Value.Should().Be(true);
            GetCheckBox(list1, "English Tea").Value.Should().Be(false);
            GetCheckBox(list1, "Chinese Tea").Value.Should().Be(false);
            GetCheckBox(list1, "Irish Coffee").Value.Should().Be(false);
            GetCheckBox(list1, "Double Espresso").Value.Should().Be(false);
            GetCheckBox(list2, "Milk").Value.Should().Be(false);
            GetCheckBox(list2, "Sparkling Water").Value.Should().Be(true);
            GetCheckBox(list2, "English Tea").Value.Should().Be(false);
            GetCheckBox(list2, "Chinese Tea").Value.Should().Be(false);
            GetCheckBox(list2, "Irish Coffee").Value.Should().Be(false);
            GetCheckBox(list2, "Double Espresso").Value.Should().Be(false);
            // click Irish on list2
            Select(list2, "Irish Coffee");
            comp.Find("p.selected-values").TrimmedText().Should().Be("Carbonated H²O, Irish Coffee");
            GetCheckBox(list1, "Milk").Value.Should().Be(false);
            GetCheckBox(list1, "Sparkling Water").Value.Should().Be(true);
            GetCheckBox(list1, "English Tea").Value.Should().Be(false);
            GetCheckBox(list1, "Chinese Tea").Value.Should().Be(false);
            GetCheckBox(list1, "Irish Coffee").Value.Should().Be(true);
            GetCheckBox(list1, "Double Espresso").Value.Should().Be(false);
            GetCheckBox(list2, "Milk").Value.Should().Be(false);
            GetCheckBox(list2, "Sparkling Water").Value.Should().Be(true);
            GetCheckBox(list2, "English Tea").Value.Should().Be(false);
            GetCheckBox(list2, "Chinese Tea").Value.Should().Be(false);
            GetCheckBox(list2, "Irish Coffee").Value.Should().Be(true);
            GetCheckBox(list2, "Double Espresso").Value.Should().Be(false);
            // click off water on list2
            Select(list2, "Sparkling Water");
            comp.Find("p.selected-values").TrimmedText().Should().Be("Irish Coffee");
            GetCheckBox(list1, "Milk").Value.Should().Be(false);
            GetCheckBox(list1, "Sparkling Water").Value.Should().Be(false);
            GetCheckBox(list1, "English Tea").Value.Should().Be(false);
            GetCheckBox(list1, "Chinese Tea").Value.Should().Be(false);
            GetCheckBox(list1, "Irish Coffee").Value.Should().Be(true);
            GetCheckBox(list1, "Double Espresso").Value.Should().Be(false);
            GetCheckBox(list2, "Milk").Value.Should().Be(false);
            GetCheckBox(list2, "Sparkling Water").Value.Should().Be(false);
            GetCheckBox(list2, "English Tea").Value.Should().Be(false);
            GetCheckBox(list2, "Chinese Tea").Value.Should().Be(false);
            GetCheckBox(list2, "Irish Coffee").Value.Should().Be(true);
            GetCheckBox(list2, "Double Espresso").Value.Should().Be(false);
        }

        /// <summary>
        /// <para>Clicking the drinks selects them. The child lists are updated accordingly, meaning, only ever 1 list item can have the active class.</para>
        /// <para>This test starts with a pre-selected item (by value)</para>
        /// </summary>
        [Test]
        public async Task ListWithPreSelectedValueTest()
        {
            var comp = Context.RenderComponent<ListSelectionInitialValueTest>();
            var list = comp.FindComponent<MudList<string>>().Instance;
            list.SelectedValue.Should().Be("Sparkling Water");
            // we have seven choices, 1 is active because of the initial value of SelectedValue
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            // set Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue("Pu'er"));
            list.SelectedValue.Should().Be("Pu'er");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[4].Markup.Should().Contain("mud-selected-item");
            // set Cafe Latte via changing SelectedValue
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue("Cafe Latte"));
            list.SelectedValue.Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[8].Markup.Should().Contain("mud-selected-item");
            // set water
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue("Sparkling Water"));
            list.SelectedValue.Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            // set nothing
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue(null));
            list.SelectedValue.Should().Be(null);
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
        }

        [Test]
        [TestCase(Color.Default)]
        [TestCase(Color.Primary)]
        [TestCase(Color.Secondary)]
        [TestCase(Color.Tertiary)]
        [TestCase(Color.Info)]
        [TestCase(Color.Success)]
        [TestCase(Color.Warning)]
        [TestCase(Color.Error)]
        [TestCase(Color.Dark)]
        public void ListColorTest(Color color)
        {
            var comp = Context.RenderComponent<ListSelectionInitialValueTest>(x => x.Add(c => c.Color, color));

            var list = comp.FindComponent<MudList<string>>().Instance;
            list.SelectedValue.Should().Be("Sparkling Water");

            var listItemClasses = comp.Find(".mud-selected-item");
            listItemClasses.ClassList.Should().ContainInOrder(new[] { $"mud-{color.ToDescriptionString()}-text", $"mud-{color.ToDescriptionString()}-hover" });
        }

        /// <summary>
        /// The child lists should honor the Dense property of their parent list if not overriden.
        /// </summary>
        [Test]
        [TestCase(true, null, 9)]
        [TestCase(false, null, 0)]
        [TestCase(true, true, 9)]
        [TestCase(false, false, 0)]
        [TestCase(true, false, 5)]
        [TestCase(false, true, 4)]
        public void ListDenseInheritanceTest(bool dense, bool? innerListDense, int expectedDenseClassCount)
        {
            var comp = Context.RenderComponent<ListDenseInheritanceTest>(x => x.Add(c => c.Dense, dense).Add(c => c.InnerListDense, innerListDense));

            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-list-item-dense").Count.Should().Be(expectedDenseClassCount); // 7 choices, 2 groups
        }

        [Test]
        public void ListItem_HasRipple_WhenRippleIsTrue()
        {
            var comp = Context.RenderComponent<ListItemRippleTest>(parameters => parameters.Add(p => p.Ripple, true));
            comp.FindAll("div.mud-ripple").Count.Should().BeGreaterThan(0);

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Ripple, false));
            comp.FindAll("div.mud-ripple").Count.Should().Be(0);
        }
    }
}
