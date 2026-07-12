#nullable enable
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.List;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ListTests : BunitTest
    {
        [Test]
        public async Task ListItem_RendersText_AndSecondaryText()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "Sparkling Water")));
            var listItem = comp.FindComponent<MudListItem<string>>();
            comp.Markup.Should().Contain("Sparkling Water");
            comp.FindAll(".mud-list-item-secondary-text").Should().BeEmpty();

            await listItem.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Text, "Latte")
                .Add(x => x.SecondaryText, "with oat milk"));

            comp.Markup.Should().NotContain("Sparkling Water");
            comp.Markup.Should().Contain("Latte");
            comp.Find(".mud-list-item-secondary-text").TextContent.Should().Contain("with oat milk");
        }

        [Test]
        [TestCase(SelectionMode.SingleSelection)]
        [TestCase(SelectionMode.ToggleSelection)]
        public async Task ClickingItems_HighlightsAtMostOne_AndReclickDependsOnMode(SelectionMode mode)
        {
            var comp = Context.Render<ListSelectionTest>(self => self.Add(x => x.SelectionMode, mode));
            var list = comp.FindComponent<MudList<string>>().Instance;

            Task ClickItem(string text) => comp.FindComponents<MudListItem<string>>()
                .Single(x => x.Instance.Text == text).Find("div.mud-list-item").ClickAsync();
            void AssertOnlySelected(string text)
            {
                comp.FindAll("div.mud-selected-item").Should().ContainSingle();
                list.SelectedValue.Should().Be(text);
                comp.FindComponents<MudListItem<string>>().Single(x => x.Instance.Text == text)
                    .Markup.Should().Contain("mud-selected-item");
            }

            list.SelectedValue.Should().BeNull();
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 drinks + 2 nested group headers
            comp.FindAll("div.mud-selected-item").Should().BeEmpty();

            await ClickItem("Sparkling Water");
            AssertOnlySelected("Sparkling Water");

            // selecting a nested item moves the single selection into the child list
            await ClickItem("Pu'er");
            AssertOnlySelected("Pu'er");

            // re-clicking the already-selected item is where the modes diverge
            await ClickItem("Pu'er");
            if (mode == SelectionMode.ToggleSelection)
            {
                list.SelectedValue.Should().BeNull();
                comp.FindAll("div.mud-selected-item").Should().BeEmpty();
            }
            else
            {
                AssertOnlySelected("Pu'er");
            }
        }

        [Test]
        public async Task PreSelectedValue_IsHonored_AndExternalChangesMoveSelection()
        {
            var comp = Context.Render<ListSelectionInitialValueTest>();
            var list = comp.FindComponent<MudList<string>>().Instance;
            list.SelectedValue.Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Should().ContainSingle();

            foreach (var drink in new[] { "Pu'er", "Cafe Latte", "Sparkling Water" })
            {
                await comp.InvokeAsync(() => comp.Instance.SetSelectedValue(drink));
                list.SelectedValue.Should().Be(drink);
                comp.FindAll("div.mud-selected-item").Should().ContainSingle();
                comp.FindComponents<MudListItem<string>>().Single(x => x.Instance.Text == drink)
                    .Markup.Should().Contain("mud-selected-item");
            }

            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue(null));
            list.SelectedValue.Should().BeNull();
            comp.FindAll("div.mud-selected-item").Should().BeEmpty();
        }

        [Test]
        public void MultiSelection_InitialSelectedValues_CheckTheMatchingItems()
        {
            var comp = Context.Render<ListMultiSelectionTest>(self => self.Add(x => x.SelectedValues, ["Milk", "Cafe Latte"]));
            comp.Find("p.selected-values").TrimmedText().Should().Be("Cafe Latte, Milk");
            CheckBoxValue(comp, "Milk").Should().Be(true);
            CheckBoxValue(comp, "Cafe Latte").Should().Be(true);
        }

        [Test]
        public async Task MultiSelection_DisabledItems_IgnoreClicks_WhileEnabledItemsToggle()
        {
            // Apple/Orange Juice are disabled in the fixture; Milk is enabled. All three start selected.
            var comp = Context.Render<ListMultiSelectionTest>(self => self.Add(x => x.SelectedValues, ["Apple Juice", "Orange Juice", "Milk"]));
            var item = (string text) => comp.FindComponents<MudListItem<string>>().Single(x => x.Instance.Text == text);

            item("Apple Juice").Find("div.mud-list-item").ClassList.Should().Contain("mud-list-item-disabled");
            CheckBoxValue(comp, "Apple Juice").Should().Be(true);
            CheckBoxValue(comp, "Milk").Should().Be(true);

            // clicking the enabled, already-selected item deselects it...
            await item("Milk").Find("div.mud-list-item").ClickAsync();
            CheckBoxValue(comp, "Milk").Should().Be(false);

            // ...but clicking the disabled items leaves their selection untouched
            await item("Apple Juice").Find("div.mud-list-item").ClickAsync();
            await item("Orange Juice").Find("div.mud-list-item").ClickAsync();
            CheckBoxValue(comp, "Apple Juice").Should().Be(true);
            CheckBoxValue(comp, "Orange Juice").Should().Be(true);
        }

        [Test]
        public async Task MultiSelection_TwoListsShareBoundCollection()
        {
            var comp = Context.Render<ListMultiSelectionBindingTest>();
            var list1 = comp.FindComponents<MudList<string>>().Single(x => x.Instance.Class == "list-1");
            var list2 = comp.FindComponents<MudList<string>>().Single(x => x.Instance.Class == "list-2");
            list1.FindComponents<MudListItem<string>>().Count.Should().Be(8);

            bool? CheckBox(IRenderedComponent<MudList<string>> list, string text) => list
                .FindComponents<MudListItem<string>>().Single(x => x.Instance.Text == text)
                .FindComponent<MudCheckBox<bool?>>().Instance.ReadValue;
            Task Select(IRenderedComponent<MudList<string>> list, string text) => list
                .FindComponents<MudListItem<string>>().Single(x => x.Instance.Text == text)
                .Find("div.mud-list-item").ClickAsync();

            // selecting on list1 mirrors onto list2 because both bind the same collection
            await Select(list1, "Sparkling Water");
            comp.Find("p.selected-values").TrimmedText().Should().Be("Carbonated H²O");
            CheckBox(list1, "Sparkling Water").Should().Be(true);
            CheckBox(list2, "Sparkling Water").Should().Be(true);

            // selecting on list2 adds to the shared collection without disturbing the first selection
            await Select(list2, "Irish Coffee");
            comp.Find("p.selected-values").TrimmedText().Should().Be("Carbonated H²O, Irish Coffee");
            CheckBox(list1, "Irish Coffee").Should().Be(true);
            CheckBox(list2, "Irish Coffee").Should().Be(true);

            // de-selecting on list2 removes it from both
            await Select(list2, "Sparkling Water");
            comp.Find("p.selected-values").TrimmedText().Should().Be("Irish Coffee");
            CheckBox(list1, "Sparkling Water").Should().Be(false);
            CheckBox(list2, "Sparkling Water").Should().Be(false);
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
        public void SelectedItem_UsesListColorClasses(Color color)
        {
            var comp = Context.Render<ListSelectionInitialValueTest>(x => x.Add(c => c.Color, color));

            var selectedItem = comp.Find(".mud-selected-item");
            selectedItem.ClassList.Should().ContainInOrder(new[] { $"mud-{color.ToStringFast(true)}-text", $"mud-{color.ToStringFast(true)}-hover" });
        }

        [Test]
        [TestCase(true, null, 9)]
        [TestCase(false, null, 0)]
        [TestCase(true, true, 9)]
        [TestCase(false, false, 0)]
        [TestCase(true, false, 5)]
        [TestCase(false, true, 4)]
        public void DenseInheritance_ChildListsFollowParentUnlessOverridden(bool dense, bool? innerListDense, int expectedDenseClassCount)
        {
            var comp = Context.Render<ListDenseInheritanceTest>(x => x.Add(c => c.Dense, dense).Add(c => c.InnerListDense, innerListDense));

            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 drinks + 2 nested group headers
            comp.FindAll("div.mud-list-item-dense").Count.Should().Be(expectedDenseClassCount);
        }

        [Test]
        public async Task Dense_ToggledAfterRender_PropagatesToItemsAndNestedLists()
        {
            var comp = Context.Render<ListDenseInheritanceTest>();
            comp.FindAll("div.mud-list-item-dense").Should().BeEmpty();

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Dense, true));

            comp.FindAll("div.mud-list-item-dense").Count.Should().Be(9);
        }

        [Test]
        public async Task SelectionMode_SwitchedToMultiSelection_MarksContainerMultiselectable()
        {
            var comp = Context.Render<ListSelectionTest>();
            comp.Find("div.mud-list").HasAttribute("aria-multiselectable").Should().BeFalse();

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.SelectionMode, SelectionMode.MultiSelection));

            comp.Find("div.mud-list").GetAttribute("aria-multiselectable").Should().Be("true");
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
        [TestCase(true, null, true)]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, null, false)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void Gutters_OnList_OverrideGuttersOnItemsWithoutTheirOwnSetting(bool listGutters, bool? itemGutters, bool resultingGutters)
        {
            var comp = Context.Render<ListItemGuttersTest>(self => self
                .Add(x => x.ListGutters, listGutters)
                .Add(x => x.ItemGutters, itemGutters)
            );
            comp.FindAll("div.mud-list-item-gutters").Should().HaveCount(resultingGutters ? 1 : 0);
        }

        [Test]
        public void ListItem_Inset_AddsInsetClassToTextWrapper()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "Indented").Add(x => x.Inset, true)));

            comp.Find("div.mud-list-item-text").ClassList.Should().Contain("mud-list-item-text-inset");
        }

        [Test]
        public void ListItem_ChildContent_OverridesTextAndSecondaryText()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .AddChildContent<MudListItem<string>>(item => item
                    .Add(x => x.Text, "ignored-text")
                    .Add(x => x.SecondaryText, "ignored-secondary")
                    .AddChildContent("<span class=\"custom-content\">Custom</span>")));

            comp.Find("span.custom-content").TextContent.Should().Be("Custom");
            comp.Markup.Should().NotContain("ignored");
        }

        [Test]
        public void ListItem_AvatarContent_RendersAvatar_AndIgnoresIcon()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .AddChildContent<MudListItem<string>>(item => item
                    .Add(x => x.Text, "Profile")
                    .Add(x => x.Icon, Icons.Material.Filled.Inbox)
                    .Add(x => x.AvatarContent, "<span class=\"avatar-marker\">AV</span>")));

            comp.Find("div.mud-list-item-avatar").TextContent.Should().Contain("AV");
            comp.FindAll("div.mud-list-item-icon").Should().BeEmpty(); // Icon is ignored when AvatarContent is set
        }

        [Test]
        public async Task Subheader_GuttersAndInset_ToggleTheirClasses()
        {
            var comp = Context.Render<MudListSubheader>(p => p
                .Add(x => x.Inset, true)
                .Add(x => x.Gutters, false)
                .AddChildContent("Drinks"));

            var header = comp.Find("div.mud-list-subheader");
            header.ClassList.Should().Contain("mud-list-subheader-inset");
            header.ClassList.Should().NotContain("mud-list-subheader-gutters");

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Inset, false).Add(x => x.Gutters, true));

            header = comp.Find("div.mud-list-subheader");
            header.ClassList.Should().NotContain("mud-list-subheader-inset");
            header.ClassList.Should().Contain("mud-list-subheader-gutters");
        }

        [Test]
        public void List_WithHref_RendersAnchorWithHrefAndTarget()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .AddChildContent<MudListItem<string>>(item => item
                    .Add(x => x.Text, "Docs")
                    .Add(x => x.Href, "/docs")
                    .Add(x => x.Target, "_blank"))
                .AddChildContent<MudListItem<string>>(item => item
                    .Add(x => x.Text, "Plain")));

            var anchor = comp.Find("a.mud-list-item");
            anchor.GetAttribute("href").Should().Be("/docs");
            anchor.GetAttribute("target").Should().Be("_blank");

            // the item without an Href stays a div
            comp.FindAll("div.mud-list-item").Should().ContainSingle();
        }

        [Test]
        [TestCase(true, null, true)]      // ForceLoad + Href + no Target -> manual NavigateTo
        [TestCase(false, null, false)]    // Href only -> the anchor navigates, no manual call
        [TestCase(true, "_blank", false)] // ForceLoad + Href + Target -> the anchor navigates
        public async Task ListItem_ForceLoad_NavigatesManuallyOnlyWithHrefAndNoTarget(bool forceLoad, string? target, bool shouldNavigate)
        {
            var nav = Context.Services.GetRequiredService<NavigationManager>();
            var comp = Context.Render<MudList<string>>(builder => builder
                .AddChildContent<MudListItem<string>>(item => item
                    .Add(x => x.Text, "Go")
                    .Add(x => x.Href, "/list-target")
                    .Add(x => x.ForceLoad, forceLoad)
                    .Add(x => x.Target, target)));
            var before = nav.Uri;

            await comp.Find("a.mud-list-item").ClickAsync();

            if (shouldNavigate)
            {
                nav.Uri.Should().EndWith("list-target");
            }
            else
            {
                nav.Uri.Should().Be(before);
            }
        }

        [Test]
        public async Task ListItem_OnClickPreventDefault_OnlyInvokesOnClick()
        {
            var clicks = 0;
            var comp = Context.Render<MudList<string>>(builder => builder
                .AddChildContent<MudListItem<string>>(item => item
                    .Add(x => x.Text, "Item")
                    .Add(x => x.Value, "item")
                    .Add(x => x.Href, "/should-not-follow")
                    .Add(x => x.OnClickPreventDefault, true)
                    .Add(x => x.OnClick, () => clicks++)));

            comp.FindAll("a.mud-list-item").Should().BeEmpty(); // forced to a div despite the Href
            await comp.Find("div.mud-list-item").ClickAsync();

            clicks.Should().Be(1);
            comp.FindAll("div.mud-selected-item").Should().BeEmpty(); // selection is suppressed
        }

        [Test]
        public async Task ListItem_WithNestedList_InvokesOnClick_AndExpands()
        {
            var clicks = 0;
            var comp = Context.Render<MudList<string>>(builder => builder
                .AddChildContent<MudListItem<string>>(item => item
                    .Add(x => x.Text, "Parent")
                    .Add(x => x.Value, "parent")
                    .Add(x => x.OnClick, () => clicks++)
                    .Add(x => x.NestedList, nested =>
                    {
                        nested.OpenComponent<MudListItem<string>>(0);
                        nested.AddAttribute(1, nameof(MudListItem<string>.Text), "Child");
                        nested.AddAttribute(2, nameof(MudListItem<string>.Value), "child");
                        nested.CloseComponent();
                    })));

            var parent = comp.FindComponents<MudListItem<string>>().First(x => x.Instance.Text == "Parent");
            parent.Find("div.mud-list-item").GetAttribute("aria-expanded").Should().Be("false");

            await parent.Find("div.mud-list-item").ClickAsync();

            clicks.Should().Be(1); // OnClick fires even though there is a NestedList
            parent.Find("div.mud-list-item").GetAttribute("aria-expanded").Should().Be("true"); // and the nested list still expands
        }

        [Test]
        public async Task Disabled_List_DisablesEveryItem_AndSuppressesSelection()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .Add(x => x.Disabled, true)
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "A").Add(x => x.Value, "A"))
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "B").Add(x => x.Value, "B")));

            comp.FindAll("div.mud-list-item").Should().OnlyContain(item =>
                item.ClassList.Contains("mud-list-item-disabled") && item.GetAttribute("tabindex") == "-1");

            await comp.FindAll("div.mud-list-item")[0].ClickAsync();
            comp.FindAll("div.mud-selected-item").Should().BeEmpty(); // clicks on a disabled list select nothing
        }

        [Test]
        public async Task Disabled_Item_IsNotClickable_ButSiblingsRemainSelectable()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "A").Add(x => x.Value, "A"))
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "B").Add(x => x.Value, "B").Add(x => x.Disabled, true))
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "C").Add(x => x.Value, "C")));
            var item = (string text) => comp.FindComponents<MudListItem<string>>().Single(x => x.Instance.Text == text);

            var disabled = item("B").Find("div.mud-list-item");
            disabled.ClassList.Should().Contain("mud-list-item-disabled");
            disabled.ClassList.Should().NotContain("mud-list-item-clickable");

            await disabled.ClickAsync();
            comp.FindAll("div.mud-selected-item").Should().BeEmpty(); // the disabled item ignores the click

            await item("C").Find("div.mud-list-item").ClickAsync();
            item("C").Find("div.mud-list-item").ClassList.Should().Contain("mud-selected-item"); // enabled siblings still work
        }

        [Test]
        public void MultiSelection_AppliesCustomCheckBoxIconsAndColor()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Add(x => x.CheckBoxColor, Color.Secondary)
                .Add(x => x.CheckedIcon, Icons.Material.Filled.Star)
                .Add(x => x.UncheckedIcon, Icons.Material.Filled.StarBorder)
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "A").Add(x => x.Value, "A"))
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "B").Add(x => x.Value, "B")));

            comp.FindComponents<MudCheckBox<bool?>>().Should().AllSatisfy(checkbox =>
            {
                checkbox.Instance.CheckedIcon.Should().Be(Icons.Material.Filled.Star);
                checkbox.Instance.UncheckedIcon.Should().Be(Icons.Material.Filled.StarBorder);
                checkbox.Instance.Color.Should().Be(Color.Secondary);
                checkbox.Instance.UncheckedColor.Should().Be(Color.Secondary);
            });
        }

        [Test]
        public async Task SingleSelection_ChangingComparer_ReevaluatesSelectedItem()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .Add(x => x.SelectedValue, "apple")
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "APPLE").Add(x => x.Value, "APPLE"))
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "Banana").Add(x => x.Value, "Banana")));

            // case-sensitive default: "apple" matches nothing
            comp.FindAll("div.mud-selected-item").Should().BeEmpty();

            await comp.SetParametersAndRenderAsync(p => p
                .Add(x => x.Comparer, new CaseInsensitiveStringComparer()));

            // now "apple" matches "APPLE"
            comp.FindAll("div.mud-selected-item").Should().ContainSingle();
            comp.FindComponents<MudListItem<string>>().Single(x => x.Instance.Value == "APPLE")
                .Markup.Should().Contain("mud-selected-item");
        }

        [Test]
        public async Task MultiSelection_ChangingComparer_RematchesItemsUnderNewComparer()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Add(x => x.SelectedValues, ["apple"])
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "APPLE").Add(x => x.Value, "APPLE"))
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "Banana").Add(x => x.Value, "Banana")));

            bool? CheckBox(string text) => comp.FindComponents<MudListItem<string>>()
                .Single(x => x.Instance.Text == text).FindComponent<MudCheckBox<bool?>>().Instance.ReadValue;

            // "apple" does not match the "APPLE" item under the default comparer
            CheckBox("APPLE").Should().Be(false);

            await comp.SetParametersAndRenderAsync(p => p
                .Add(x => x.Comparer, new CaseInsensitiveStringComparer()));

            // the looser comparer now matches "apple" to the "APPLE" item
            CheckBox("APPLE").Should().Be(true);
            CheckBox("Banana").Should().Be(false);
        }

        [Test]
        public void Interactive_SingleSelection_UsesListboxRoles_WithoutMultiselectable()
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
        public void Interactive_MultiSelection_MarksContainerMultiselectable()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "A")));

            var list = comp.Find("div.mud-list");
            list.GetAttribute("role").Should().Be("listbox");
            list.GetAttribute("aria-multiselectable").Should().Be("true");
        }

        [Test]
        public async Task SingleSelection_AriaSelected_ReflectsTheSelectedItem()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "A").Add(x => x.Value, "A"))
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "B").Add(x => x.Value, "B")));

            comp.FindAll("div.mud-list-item").Should().OnlyContain(item => item.GetAttribute("aria-selected") == "false");

            await comp.FindAll("div.mud-list-item")[0].ClickAsync();

            var items = comp.FindAll("div.mud-list-item");
            items[0].GetAttribute("aria-selected").Should().Be("true");
            items[1].GetAttribute("aria-selected").Should().Be("false");
        }

        [Test]
        public void ReadOnly_List_UsesPlainListRolesAndNoSelectionState()
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
        public async Task Keyboard_ArrowDown_SkipsDisabledItem()
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
        public async Task Keyboard_ArrowUp_MovesToPreviousItem()
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
        public async Task Keyboard_ArrowUpOnFirstItem_StaysOnFirstItem()
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
        public async Task Keyboard_Home_MovesFocusToFirstItem()
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
        public async Task Keyboard_End_MovesFocusToLastItem()
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
        [TestCase(" ")]
        [TestCase("Enter")]
        public async Task Keyboard_SingleSelection_ActivatesFocusedItem(string key)
        {
            var comp = Context.Render<ListAccessibilityTest>();
            var items = comp.FindAll("div.mud-list-item");

            await items[0].KeyDownAsync(new KeyboardEventArgs { Key = key });

            await comp.WaitForAssertionAsync(() => comp.Find("p.selected-value").TrimmedText().Should().Be("Alpha"));
        }

        [Test]
        public async Task Keyboard_EnterAndNumpadEnter_ToggleSelectionInToggleMode()
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
        public async Task Keyboard_EnterOnAnchorItem_LeavesNavigationToTheBrowser()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .AddChildContent<MudListItem<string>>(item => item.Add(x => x.Text, "Link").Add(x => x.Value, "Link").Add(x => x.Href, "/somewhere")));

            comp.Find("a.mud-list-item").GetAttribute("aria-selected").Should().Be("false");

            await comp.Find("a.mud-list-item").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

            // Enter on an anchor is a no-op in code (the rendered <a> activates itself), so it is not selected
            comp.Find("a.mud-list-item").GetAttribute("aria-selected").Should().Be("false");
        }

        [Test]
        public async Task Keyboard_NonTabbableItem_DoesNotHandleKeyCommands()
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
        public async Task Keyboard_Space_TogglesMultiSelection_WithCheckboxesHiddenFromTabOrder()
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
        public async Task NestedItem_Click_TogglesExpansion_AndRaisesExpandedChanged()
        {
            var comp = Context.Render<ListNestedExpansionTest>();
            var parent = comp.Find("div.mud-list-item");

            // the expand chevron is the icon coloured with the fixture's ExpandIconColor (Secondary)
            string? ExpandIcon() => comp.FindComponents<MudIcon>().Single(i => i.Instance.Color == Color.Secondary).Instance.Icon;

            parent.GetAttribute("aria-expanded").Should().Be("false");
            comp.Find("p.changed-count").TrimmedText().Should().Be("0");
            ExpandIcon().Should().Be(Icons.Material.Filled.Add); // ExpandMoreIcon

            await parent.ClickAsync();

            comp.Find("div.mud-list-item").GetAttribute("aria-expanded").Should().Be("true");
            comp.Find("p.expanded").TrimmedText().Should().Be("True");
            comp.Find("p.changed-count").TrimmedText().Should().Be("1");
            comp.FindAll("div.mud-selected-item").Should().BeEmpty(); // expanding is not selecting
            ExpandIcon().Should().Be(Icons.Material.Filled.Remove); // ExpandLessIcon

            await comp.Find("div.mud-list-item").ClickAsync();
            comp.Find("div.mud-list-item").GetAttribute("aria-expanded").Should().Be("false");
            comp.Find("p.changed-count").TrimmedText().Should().Be("2");
        }

        [Test]
        [TestCase(" ")]
        [TestCase("Enter")]
        public async Task NestedItem_Keyboard_TogglesExpansion(string key)
        {
            var comp = Context.Render<ListNestedExpansionTest>();
            var parent = comp.Find("div.mud-list-item");

            await parent.KeyDownAsync(new KeyboardEventArgs { Key = key });

            await comp.WaitForAssertionAsync(() => comp.Find("div.mud-list-item").GetAttribute("aria-expanded").Should().Be("true"));
            comp.Find("p.expanded").TrimmedText().Should().Be("True");
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
            var fallbackItem = comp.FindComponents<MudListItem<string>>()
                .Single(x => x.Instance.Text == "Default item").Find("div.mud-list-item");

            customIdItem.GetAttribute("id").Should().Be("custom-id");
            customIdItem.GetAttribute("tabindex").Should().Be("-1"); // user value wins over the roving tabindex
            fallbackItem.GetAttribute("id").Should().StartWith("list-item");
        }

        [Test]
        public void List_UserAttributes_ShouldOverrideGeneratedAccessibilityAttributes()
        {
            var comp = Context.Render<MudList<string>>(builder => builder
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .AddUnmatched("role", "group")
                .AddUnmatched("aria-multiselectable", "false"));

            var list = comp.Find("div.mud-list");
            list.GetAttribute("role").Should().Be("group");
            list.GetAttribute("aria-multiselectable").Should().Be("false");
        }

        private static bool? CheckBoxValue(IRenderedComponent<ListMultiSelectionTest> comp, string text) =>
            comp.FindComponents<MudListItem<string>>()
                .Single(x => x.Instance.Text == text)
                .FindComponent<MudCheckBox<bool?>>().Instance.ReadValue;

        private sealed class CaseInsensitiveStringComparer : IEqualityComparer<string?>
        {
            public bool Equals(string? x, string? y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode(string? obj) => obj is null ? 0 : obj.ToLowerInvariant().GetHashCode();
        }
    }
}
