using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.List;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ListTests : BunitTest
    {

        [Test]
        public async Task ListRender()
        {
            var comp = Context.Render<ListSelectionTest>();
            var listItem = comp.FindComponent<MudListItem<string>>();
            comp.Markup.Should().Contain("Sparkling Water");
            comp.Markup.Should().NotContain("Roger Waters");
            comp.Markup.Should().NotContain("High Hopes");
            await listItem.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Text, "Roger Waters")
                .Add(x => x.SecondaryText, "High Hopes"));
            comp.Markup.Should().NotContain("Sparkling Water");
            comp.Markup.Should().Contain("Roger Waters");
            comp.Markup.Should().Contain("High Hopes");
        }

        /// <summary>
        /// <para>Clicking the drinks selects them. The child lists are updated accordingly, meaning, only ever 1 list item can have the active class.</para>
        /// <para>In this test no item is selected to begin with</para>
        /// </summary>
        [Test]
        public async Task ListSelection()
        {
            var comp = Context.Render<ListSelectionTest>();
            var list = comp.FindComponent<MudList<string>>().Instance;
            list.SelectedValue.Should().Be(null);
            // we have seven choices, none is active
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            // click water
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            list.SelectedValue.Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            await comp.FindAll("div.mud-list-item")[4].ClickAsync();
            list.SelectedValue.Should().Be("Pu'er");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[4].Markup.Should().Contain("mud-selected-item");
            // click Cafe Latte
            await comp.FindAll("div.mud-list-item")[8].ClickAsync();
            list.SelectedValue.Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[8].Markup.Should().Contain("mud-selected-item");
            // click Cafe Latte again which should NOT deselect it because we are in single-selection mode
            await comp.FindAll("div.mud-list-item")[8].ClickAsync();
            list.SelectedValue.Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[8].Markup.Should().Contain("mud-selected-item");
        }

        [Test]
        public async Task ListToggleSelection()
        {
            var comp = Context.Render<ListSelectionTest>(self => self.Add(x => x.SelectionMode, SelectionMode.ToggleSelection));
            var list = comp.FindComponent<MudList<string>>().Instance;
            list.SelectedValue.Should().Be(null);
            // we have seven choices, none is active
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            // click water
            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            list.SelectedValue.Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            await comp.FindAll("div.mud-list-item")[4].ClickAsync();
            list.SelectedValue.Should().Be("Pu'er");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[4].Markup.Should().Contain("mud-selected-item");
            // click Cafe Latte
            await comp.FindAll("div.mud-list-item")[8].ClickAsync();
            list.SelectedValue.Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[8].Markup.Should().Contain("mud-selected-item");
            // click Cafe Latte again which should deselect it because we are in toggle-selection mode
            await comp.FindAll("div.mud-list-item")[8].ClickAsync();
            list.SelectedValue.Should().Be(null);
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            comp.FindComponents<MudListItem<string>>()[8].Markup.Should().NotContain("mud-selected-item");
        }

        [Test]
        public void ListMultiSelectionInitialValues()
        {
            var comp = Context.Render<ListMultiSelectionTest>(self => self.Add(x => x.SelectedValues, ["Milk", "Cafe Latte"]));
            var list = comp.FindComponent<MudList<string>>().Instance;
            comp.Find("p.selected-values").TrimmedText().Should().Be("Cafe Latte, Milk");
            var GetCheckBox = (string text) => comp.FindComponents<MudListItem<string>>().FirstOrDefault(x => x.Instance.Text == text)?.FindComponent<MudCheckBox<bool?>>().Instance;
            GetCheckBox("Milk").ReadValue.Should().Be(true);
            GetCheckBox("Cafe Latte").ReadValue.Should().Be(true);
        }

        [Test]
        public async Task ListMultiSelection_DisabledItems_ShouldDisplaySelectedStateAndNotBeClickable()
        {
            var comp = Context.Render<ListMultiSelectionTest>(self => self.Add(x => x.SelectedValues, ["Apple Juice", "Orange Juice"]));
            var list = comp.FindComponent<MudList<string>>().Instance;
            var GetCheckBox = (string text) => comp.FindComponents<MudListItem<string>>().FirstOrDefault(x => x.Instance.Text == text)?.FindComponent<MudCheckBox<bool?>>().Instance;
            comp.Find("p.selected-values").TrimmedText().Should().Be("Apple Juice, Orange Juice");
            GetCheckBox("Apple Juice").ReadValue.Should().Be(true);
            GetCheckBox("Orange Juice").ReadValue.Should().Be(true);
            // attempt to click disabled items: selection state must not change
            var appleItem = comp.FindComponents<MudListItem<string>>()
                .FirstOrDefault(x => x.Instance.Text == "Apple Juice");
            var orangeItem = comp.FindComponents<MudListItem<string>>()
                .FirstOrDefault(x => x.Instance.Text == "Orange Juice");
            await appleItem.Find("div.mud-list-item").ClickAsync();
            await orangeItem.Find("div.mud-list-item").ClickAsync();
            // after click attempts, disabled items should remain selected
            comp.Find("p.selected-values").TrimmedText().Should().Be("Apple Juice, Orange Juice");
            GetCheckBox("Apple Juice").ReadValue.Should().Be(true);
            GetCheckBox("Orange Juice").ReadValue.Should().Be(true);
        }

        [Test]
        public async Task ListMultiSelectionBinding()
        {
            var comp = Context.Render<ListMultiSelectionBindingTest>();
            var list1 = comp.FindComponents<MudList<string>>().FirstOrDefault(x => x.Instance.Class == "list-1");
            var list2 = comp.FindComponents<MudList<string>>().FirstOrDefault(x => x.Instance.Class == "list-2");
            list1.FindComponents<MudListItem<string>>().Count.Should().Be(8);
            var GetCheckBox = (IRenderedComponent<MudList<string>> list, string text) => list.FindComponents<MudListItem<string>>()
                        .FirstOrDefault(x => x.Instance.Text == text)?.FindComponent<MudCheckBox<bool?>>().Instance;
            var Select = async (IRenderedComponent<MudList<string>> list, string text) =>
                        await list.FindComponents<MudListItem<string>>()
                        .FirstOrDefault(x => x.Instance.Text == text).Find("div.mud-list-item").ClickAsync();
            // click water on list1
            await Select(list1, "Sparkling Water");
            comp.Find("p.selected-values").TrimmedText().Should().Be("Carbonated H²O");
            GetCheckBox(list1, "Milk").ReadValue.Should().Be(false);
            GetCheckBox(list1, "Sparkling Water").ReadValue.Should().Be(true);
            GetCheckBox(list1, "English Tea").ReadValue.Should().Be(false);
            GetCheckBox(list1, "Chinese Tea").ReadValue.Should().Be(false);
            GetCheckBox(list1, "Irish Coffee").ReadValue.Should().Be(false);
            GetCheckBox(list1, "Double Espresso").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Milk").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Sparkling Water").ReadValue.Should().Be(true);
            GetCheckBox(list2, "English Tea").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Chinese Tea").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Irish Coffee").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Double Espresso").ReadValue.Should().Be(false);
            // click Irish on list2
            await Select(list2, "Irish Coffee");
            comp.Find("p.selected-values").TrimmedText().Should().Be("Carbonated H²O, Irish Coffee");
            GetCheckBox(list1, "Milk").ReadValue.Should().Be(false);
            GetCheckBox(list1, "Sparkling Water").ReadValue.Should().Be(true);
            GetCheckBox(list1, "English Tea").ReadValue.Should().Be(false);
            GetCheckBox(list1, "Chinese Tea").ReadValue.Should().Be(false);
            GetCheckBox(list1, "Irish Coffee").ReadValue.Should().Be(true);
            GetCheckBox(list1, "Double Espresso").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Milk").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Sparkling Water").ReadValue.Should().Be(true);
            GetCheckBox(list2, "English Tea").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Chinese Tea").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Irish Coffee").ReadValue.Should().Be(true);
            GetCheckBox(list2, "Double Espresso").ReadValue.Should().Be(false);
            // click off water on list2
            await Select(list2, "Sparkling Water");
            comp.Find("p.selected-values").TrimmedText().Should().Be("Irish Coffee");
            GetCheckBox(list1, "Milk").ReadValue.Should().Be(false);
            GetCheckBox(list1, "Sparkling Water").ReadValue.Should().Be(false);
            GetCheckBox(list1, "English Tea").ReadValue.Should().Be(false);
            GetCheckBox(list1, "Chinese Tea").ReadValue.Should().Be(false);
            GetCheckBox(list1, "Irish Coffee").ReadValue.Should().Be(true);
            GetCheckBox(list1, "Double Espresso").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Milk").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Sparkling Water").ReadValue.Should().Be(false);
            GetCheckBox(list2, "English Tea").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Chinese Tea").ReadValue.Should().Be(false);
            GetCheckBox(list2, "Irish Coffee").ReadValue.Should().Be(true);
            GetCheckBox(list2, "Double Espresso").ReadValue.Should().Be(false);
        }

        /// <summary>
        /// <para>Clicking the drinks selects them. The child lists are updated accordingly, meaning, only ever 1 list item can have the active class.</para>
        /// <para>This test starts with a pre-selected item (by value)</para>
        /// </summary>
        [Test]
        public async Task ListWithPreSelectedValue()
        {
            var comp = Context.Render<ListSelectionInitialValueTest>();
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
        public void ListColor(Color color)
        {
            var comp = Context.Render<ListSelectionInitialValueTest>(x => x.Add(c => c.Color, color));

            var list = comp.FindComponent<MudList<string>>().Instance;
            list.SelectedValue.Should().Be("Sparkling Water");

            var listItemClasses = comp.Find(".mud-selected-item");
            listItemClasses.ClassList.Should().ContainInOrder(new[] { $"mud-{color.ToStringFast(true)}-text", $"mud-{color.ToStringFast(true)}-hover" });
        }

        /// <summary>
        /// The child lists should honor the Dense property of their parent list if not overridden.
        /// </summary>
        [Test]
        [TestCase(true, null, 9)]
        [TestCase(false, null, 0)]
        [TestCase(true, true, 9)]
        [TestCase(false, false, 0)]
        [TestCase(true, false, 5)]
        [TestCase(false, true, 4)]
        public void ListDenseInheritance(bool dense, bool? innerListDense, int expectedDenseClassCount)
        {
            var comp = Context.Render<ListDenseInheritanceTest>(x => x.Add(c => c.Dense, dense).Add(c => c.InnerListDense, innerListDense));

            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-list-item-dense").Count.Should().Be(expectedDenseClassCount); // 7 choices, 2 groups
        }

        [Test]
        public async Task ListItem_HasRipple_WhenRippleIsTrue()
        {
            var comp = Context.Render<ListItemRippleTest>(parameters => parameters.Add(p => p.Ripple, true));
            comp.FindAll("div.mud-ripple").Count.Should().BeGreaterThan(0);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Ripple, false));
            comp.FindAll("div.mud-ripple").Count.Should().Be(0);
        }

        [Test]
        public void ListItemTabIndex()
        {
            var comp = Context.Render<ListItemTabIndexTest>();
            comp.FindAll("div")[1].GetAttribute("tabindex").Should().Be("-1");
        }

        [Test]
        public void ListAccessibility_ReadOnlyUsesListRoles()
        {
            var comp = Context.Render<ListAccessibilityTest>(x => x.Add(c => c.ReadOnly, true));

            var list = comp.Find("div.mud-list");
            list.GetAttribute("role").Should().Be("list");
            list.HasAttribute("aria-multiselectable").Should().BeFalse();

            foreach (var item in comp.FindAll("div.mud-list-item"))
            {
                item.GetAttribute("role").Should().Be("listitem");
                item.HasAttribute("aria-selected").Should().BeFalse();
            }
        }

        [Test]
        public void ListAccessibility_InteractiveUsesListboxRolesAndRovingTabIndex()
        {
            var comp = Context.Render<ListAccessibilityTest>();

            var list = comp.Find("div.mud-list");
            list.GetAttribute("role").Should().Be("listbox");
            list.HasAttribute("aria-multiselectable").Should().BeFalse();

            var items = comp.FindAll("div.mud-list-item");
            items[0].GetAttribute("role").Should().Be("option");
            items[0].GetAttribute("tabindex").Should().Be("0");
            items[1].GetAttribute("tabindex").Should().Be("-1");
            items[2].GetAttribute("tabindex").Should().Be("-1");
        }

        [Test]
        public async Task ListKeyboardNavigation_ArrowDownSkipsDisabledItem()
        {
            var comp = Context.Render<ListAccessibilityTest>(x => x.Add(c => c.IncludeDisabledItem, true));
            var items = comp.FindAll("div.mud-list-item");

            await items[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });

            await comp.WaitForAssertionAsync(() => comp.Find("p.selected-value").TrimmedText().Should().Be("Charlie"));

            items = comp.FindAll("div.mud-list-item");
            items[0].GetAttribute("tabindex").Should().Be("-1");
            items[1].GetAttribute("tabindex").Should().Be("-1");
            items[2].GetAttribute("tabindex").Should().Be("0");
        }

        [Test]
        public async Task ListKeyboardNavigation_ArrowUpMovesToPreviousItem()
        {
            var comp = Context.Render<ListAccessibilityTest>();
            var items = comp.FindAll("div.mud-list-item");

            await items[0].KeyDownAsync(new KeyboardEventArgs { Key = "End" });
            await comp.WaitForAssertionAsync(() => comp.Find("p.selected-value").TrimmedText().Should().Be("Charlie"));

            items = comp.FindAll("div.mud-list-item");
            await items[2].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowUp" });

            await comp.WaitForAssertionAsync(() => comp.Find("p.selected-value").TrimmedText().Should().Be("Bravo"));

            items = comp.FindAll("div.mud-list-item");
            items[1].GetAttribute("tabindex").Should().Be("0");
            items[2].GetAttribute("tabindex").Should().Be("-1");
        }

        [Test]
        public async Task ListKeyboardNavigation_ArrowUpOnFirstItemStaysOnFirstItem()
        {
            var comp = Context.Render<ListAccessibilityTest>();
            var items = comp.FindAll("div.mud-list-item");

            await items[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowUp" });

            await comp.WaitForAssertionAsync(() => comp.Find("p.selected-value").TrimmedText().Should().Be("Alpha"));

            items = comp.FindAll("div.mud-list-item");
            items[0].GetAttribute("tabindex").Should().Be("0");
            items[1].GetAttribute("tabindex").Should().Be("-1");
            items[2].GetAttribute("tabindex").Should().Be("-1");
        }

        [Test]
        public async Task ListKeyboardNavigation_HomeMovesFocusToFirstItem()
        {
            var comp = Context.Render<ListAccessibilityTest>();
            var items = comp.FindAll("div.mud-list-item");

            await items[0].KeyDownAsync(new KeyboardEventArgs { Key = "End" });
            await comp.WaitForAssertionAsync(() => comp.Find("p.selected-value").TrimmedText().Should().Be("Charlie"));

            items = comp.FindAll("div.mud-list-item");
            await items[2].KeyDownAsync(new KeyboardEventArgs { Key = "Home" });

            await comp.WaitForAssertionAsync(() => comp.Find("p.selected-value").TrimmedText().Should().Be("Alpha"));

            items = comp.FindAll("div.mud-list-item");
            items[0].GetAttribute("tabindex").Should().Be("0");
            items[2].GetAttribute("tabindex").Should().Be("-1");
        }

        [Test]
        public async Task ListKeyboardNavigation_EndMovesFocusToLastItem()
        {
            var comp = Context.Render<ListAccessibilityTest>();
            var items = comp.FindAll("div.mud-list-item");

            await items[0].KeyDownAsync(new KeyboardEventArgs { Key = "End" });

            await comp.WaitForAssertionAsync(() => comp.Find("p.selected-value").TrimmedText().Should().Be("Charlie"));

            items = comp.FindAll("div.mud-list-item");
            items[0].GetAttribute("tabindex").Should().Be("-1");
            items[2].GetAttribute("tabindex").Should().Be("0");
        }

        [Test]
        public async Task ListKeyboardNavigation_EnterAndNumpadEnterToggleSelectionInToggleMode()
        {
            var comp = Context.Render<ListAccessibilityTest>(x => x.Add(c => c.SelectionMode, SelectionMode.ToggleSelection));
            var items = comp.FindAll("div.mud-list-item");

            await items[0].KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
            await comp.WaitForAssertionAsync(() => comp.Find("p.selected-value").TrimmedText().Should().Be("Alpha"));

            items = comp.FindAll("div.mud-list-item");
            await items[0].KeyDownAsync(new KeyboardEventArgs { Key = "NumpadEnter" });
            await comp.WaitForAssertionAsync(() => comp.Find("p.selected-value").TrimmedText().Should().BeEmpty());
        }

        [Test]
        public async Task ListKeyboardNavigation_NonTabbableItemDoesNotHandleKeyCommands()
        {
            var comp = Context.Render<ListAccessibilityTest>();
            var items = comp.FindAll("div.mud-list-item");

            await items[1].KeyDownAsync(new KeyboardEventArgs { Key = "End" });

            await comp.WaitForAssertionAsync(() =>
            {
                comp.Find("p.selected-value").TrimmedText().Should().BeEmpty();
                var currentItems = comp.FindAll("div.mud-list-item");
                currentItems[0].GetAttribute("tabindex").Should().Be("0");
                currentItems[1].GetAttribute("tabindex").Should().Be("-1");
                currentItems[2].GetAttribute("tabindex").Should().Be("-1");
            });
        }

        [Test]
        public async Task ListKeyboardNavigation_SpaceTogglesMultiSelectionWithoutTabbableCheckboxes()
        {
            var comp = Context.Render<ListAccessibilityTest>(x => x.Add(c => c.SelectionMode, SelectionMode.MultiSelection));
            var items = comp.FindAll("div.mud-list-item");

            foreach (var checkbox in comp.FindAll("input.mud-checkbox-input"))
            {
                checkbox.GetAttribute("tabindex").Should().Be("-1");
                checkbox.GetAttribute("aria-hidden").Should().Be("true");
            }

            await items[0].KeyDownAsync(new KeyboardEventArgs { Key = " " });
            await comp.WaitForAssertionAsync(() => comp.Find("p.selected-values").TrimmedText().Should().Be("Alpha"));

            await items[0].KeyDownAsync(new KeyboardEventArgs { Key = " " });
            await comp.WaitForAssertionAsync(() => comp.Find("p.selected-values").TrimmedText().Should().BeEmpty());
        }

        [Test]
        public void ListItem_UserProvidedIdOverridesGeneratedElementId()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .AddChildContent<MudListItem<string>>(item => item
                    .Add(x => x.Text, "Custom attrs")
                    .AddUnmatched("id", "custom-id")
                    .AddUnmatched("tabindex", "-1")
                    .AddUnmatched("data-test", "custom-marker"))
                .AddChildContent<MudListItem<string>>(item => item
                    .Add(x => x.Text, "Default item"))
            );

            var customIdItem = comp.Find("div.mud-list-item[data-test='custom-marker']");
            var fallbackItem = comp.FindAll("div.mud-list-item")[1];

            customIdItem.GetAttribute("id").Should().Be("custom-id");
            customIdItem.GetAttribute("tabindex").Should().Be("-1");
            fallbackItem.GetAttribute("id").Should().StartWith("list-item");
        }

        [Test]
        [TestCase(true, null, true)]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, null, false)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void SettingGuttersOnList_Should_OverrideGuttersOnItemsWithoutGuttersSetting(bool listGutters, bool? itemGutters, bool resultingGutters)
        {
            var comp = Context.Render<ListItemGuttersTest>(self => self
                .Add(x => x.ListGutters, listGutters)
                .Add(x => x.ItemGutters, itemGutters)
            );
            (comp.FindAll("div.mud-list-item-gutters").Count > 0).Should().Be(resultingGutters);
        }
    }
}
