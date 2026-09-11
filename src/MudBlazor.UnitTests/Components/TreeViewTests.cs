using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.TreeView;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class TreeViewTests : BunitTest
    {
        [Test]
        public async Task TreeView_ClickWhileDisabled_DoesNotChangeSelection()
        {
            var comp = Context.Render<DisabledTreeViewTest>(parameters => parameters.Add(x => x.Disabled, true));
            await comp.Find("div.mud-treeview-item-content").ClickAsync();
            var GetSelectedValue = () => comp.Find("p.selected-value").TrimmedText();
            GetSelectedValue().Should().BeNullOrWhiteSpace();

            await comp.Find("div.mud-treeview-item-content").DoubleClickAsync();
            GetSelectedValue().Should().BeNullOrWhiteSpace();
        }

        [Test]
        public async Task TreeView_ClickWhileActive_DoesChangeSelection()
        {
            var comp = Context.Render<DisabledTreeViewTest>(self => self.Add(x => x.Disabled, false));
            await comp.Find("div.mud-treeview-item-content").ClickAsync();
            var GetSelectedValue = () => comp.Find("p.selected-value").TrimmedText();
            GetSelectedValue().Should().NotBeNullOrWhiteSpace();
            GetSelectedValue().Should().Be("content");

            // To reset
            await comp.Find("div.mud-treeview-item-content").ClickAsync();
            GetSelectedValue().Should().BeNullOrWhiteSpace();

            await comp.Find("div.mud-treeview-item-content").DoubleClickAsync();
            GetSelectedValue().Should().NotBeNull();
        }

        [Test]
        public async Task TreeView_MultiSelectionClickCheckboxWhenDisabled_DoesNotChangeSelection()
        {
            var comp = Context.Render<TreeViewMultiSelectionCheckboxTest>(parameters => parameters.Add(x => x.Disabled, true)
                    .Add(x => x.ReadOnly, false));
            await comp.Find("div.mud-treeview-item-checkbox").ClickAsync();
            var GetSelectedValue = () => comp.Find("ul.selected-values").ChildElementCount;
            GetSelectedValue().Should().Be(0);

            await comp.Find("div.mud-treeview-item-checkbox").DoubleClickAsync();
            GetSelectedValue().Should().Be(0);
        }

        [Test]
        public async Task TreeView_MultiSelectionClickCheckboxWhenReadOnly_DoesNotChangeSelection()
        {
            var comp = Context.Render<TreeViewMultiSelectionCheckboxTest>(parameters => parameters.Add(x => x.ReadOnly, true)
                    .Add(x => x.Disabled, false));
            await comp.Find("div.mud-treeview-item-checkbox").ClickAsync();
            var GetSelectedValue = () => comp.Find("ul.selected-values").ChildElementCount;
            GetSelectedValue().Should().Be(0);

            await comp.Find("div.mud-treeview-item-checkbox").DoubleClickAsync();
            GetSelectedValue().Should().Be(0);
        }

        [Test]
        public async Task TreeView_MultiSelectionClickCheckboxWhenReadOnlyAndDisabled_DoesNotChangeSelection()
        {
            var comp = Context.Render<TreeViewMultiSelectionCheckboxTest>(parameters => parameters.Add(x => x.ReadOnly, true)
                    .Add(x => x.Disabled, true));
            await comp.Find("div.mud-treeview-item-checkbox").ClickAsync();
            var GetSelectedValue = () => comp.Find("ul.selected-values").ChildElementCount;
            GetSelectedValue().Should().Be(0);

            await comp.Find("div.mud-treeview-item-checkbox").DoubleClickAsync();
            GetSelectedValue().Should().Be(0);
        }

        [Test]
        public async Task TreeView_ClickMultiSelectionCheckboxWhileActive_DoesChangeSelection()
        {
            var comp = Context.Render<TreeViewMultiSelectionCheckboxTest>(self => self.Add(x => x.Disabled, false)
                    .Add(x => x.ReadOnly, false));
            await comp.Find("div.mud-treeview-item-checkbox").ClickAsync();
            var GetSelectedValue = () => comp.Find("ul.selected-values").ChildElementCount;
            GetSelectedValue().Should().Be(4);

            // To reset
            await comp.Find("div.mud-treeview-item-checkbox").ClickAsync();
            GetSelectedValue().Should().Be(0);

            await comp.Find("div.mud-treeview-item-checkbox").DoubleClickAsync();
            GetSelectedValue().Should().Be(4);
        }

        [Test]
        [TestCase("item1")]
        [TestCase("item1.1")]
        [TestCase("item1.2")]
        public void TreeViewWithSingleSelection_Should_RespectInitialSelectedValue(string value)
        {
            var comp = Context.Render<SimpleTreeViewTest>(self => self.Add(x => x.SelectedValue, value));
            comp.Find("div.mud-treeview-item-selected").QuerySelector(".mud-treeview-item-label").TrimmedText().Should().Be(value);
        }

        [Test]
        public async Task TreeViewWith_SingleSelection_TwoWayBinding()
        {
            var comp = Context.Render<TreeViewSelectionBindingTest>(self => self.Add(x => x.SelectedValue, "item1.2"));
            // check initial selection
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");

            // select another value on tree1 and check selection has changed on both trees
            await comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClickAsync();
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1");
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");

            // select another value on tree2 and check selection has changed on both trees
            await comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClickAsync();
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1.1");
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");

            // in single selection clicking the same item twice won't de-select it!
            // select same value on tree1 and check selection has NOT changed
            await comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClickAsync();
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1.1");
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
        }

        [Test]
        public async Task TreeViewWith_ToggleSelection_TwoWayBinding()
        {
            var comp = Context.Render<TreeViewSelectionBindingTest>(self => self
                .Add(x => x.SelectedValue, "item1.2")
                .Add(x => x.SelectionMode, SelectionMode.ToggleSelection));
            // check initial selection
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");

            // select another value on tree1 and check selection has changed on both trees
            await comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClickAsync();
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1");
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");

            // select another value on tree2 and check selection has changed on both trees
            await comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClickAsync();
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1.1");
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");

            // in toggle selection clicking the same item twice will de-select it!
            // select same value on tree1 and check selection has been removed
            await comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClickAsync();
            comp.Find("p.selected-value").TrimmedText().Should().BeNullOrWhiteSpace();
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
        }

        [Test]
        public async Task TreeViewWith_MultiSelection_TwoWayBinding()
        {
            var comp = Context.Render<TreeViewSelectionBindingTest>(self => self
                .Add(x => x.SelectedValues, ["item1", "item1.2"])
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            // check initial selection
            comp.Find(".tree1 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-null");
            comp.Find(".tree2 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-null");
            // note: the tristate checkbox is null because not all its children are selected ...
            // ... this doesn't mean that the item's Selected value isn't true. Checking:
            foreach (var item in comp.FindComponents<MudTreeViewItem<string>>().Where(x => x.Instance.Value == "item1"))
            {
                item.Instance.GetState<bool>(nameof(MudTreeViewItem<string>.Selected)).Should().Be(true);
            }
            comp.Find(".tree1 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree2 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree1 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree2 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find("p.selected-values").TrimmedText().Should().Be("item1, item1.2");

            // select another value on tree1 and check selection has changed on both trees
            await comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClickAsync();
            comp.Find(".tree1 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree2 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree1 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree2 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree1 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree2 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find("p.selected-values").TrimmedText().Should().Be("item1, item1.1, item1.2");

            // remove a value on tree2 and check selection has changed on both trees
            await comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClickAsync();
            comp.Find(".tree1 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree2 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree1 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree2 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree1 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree2 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find("p.selected-values").TrimmedText().Should().Be("");
        }

        [Test]
        public async Task TreeViewWith_MultiSelection_ShouldNotAutoSelectParent()
        {
            var comp = Context.Render<TreeViewAutoSelectParentTest>(self => self
                .Add(x => x.SelectedValues, ["item1.2"])
                .Add(x => x.AutoSelectParent, false));
            // check initial selection
            comp.Find(".tree1 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-null");
            comp.Find(".tree1 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree1 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find("p.selected-values").TrimmedText().Should().Be("item1.2");

            // select another value on tree1 and check parent is not selected
            await comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClickAsync();
            comp.Find(".tree1 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-null");
            comp.Find(".tree1 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree1 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find("p.selected-values").TrimmedText().Should().Be("item1.1, item1.2");

            // manually selecting a parent should still work
            await comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClickAsync();
            comp.Find(".tree1 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree1 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree1 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find("p.selected-values").TrimmedText().Should().Be("item1, item1.1, item1.2");

            // removing selection of a child will keep the parent selected
            await comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClickAsync();
            comp.Find(".tree1 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-null");
            comp.Find(".tree1 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree1 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find("p.selected-values").TrimmedText().Should().Be("item1, item1.2");
        }

        [Test]
        public void TreeViewWith_MultiSelection_ShouldCalculateTriStateAcrossAllDescendants()
        {
            var comp = Context.Render<TreeViewTriStateTraversalTest>(self => self
                .Add(x => x.SelectedValues,
                [
                    "selected-leaf",
                    "fully-selected", "fully-selected-child", "fully-selected-grandchild",
                    "selected-parent", "selected-parent-child", "selected-parent-grandchild",
                    "wide", "wide-selected", "wide-selected-2",
                    "deep", "deep-1", "deep-2", "deep-3",
                    "unselected-parent-grandchild"
                ]));

            static string CheckboxState(string className, IRenderedComponent<TreeViewTriStateTraversalTest> component) =>
                component.Find($".{className} .mud-checkbox span").ClassList
                    .Single(x => x is "mud-checkbox-true" or "mud-checkbox-false" or "mud-checkbox-null");

            CheckboxState("selected-leaf", comp).Should().Be("mud-checkbox-true");
            CheckboxState("unselected-leaf", comp).Should().Be("mud-checkbox-false");
            CheckboxState("fully-selected", comp).Should().Be("mud-checkbox-true");
            CheckboxState("fully-unselected", comp).Should().Be("mud-checkbox-false");
            CheckboxState("selected-parent", comp).Should().Be("mud-checkbox-null");
            CheckboxState("unselected-parent", comp).Should().Be("mud-checkbox-null");
            CheckboxState("root", comp).Should().Be("mud-checkbox-null");
            CheckboxState("wide", comp).Should().Be("mud-checkbox-null");
            CheckboxState("deep", comp).Should().Be("mud-checkbox-true");
            CheckboxState("deep-1", comp).Should().Be("mud-checkbox-true");
            CheckboxState("deep-2", comp).Should().Be("mud-checkbox-true");
            CheckboxState("deep-3", comp).Should().Be("mud-checkbox-true");
        }

        [Test]
        public void TreeViewItemSelected_ShouldBeInitializedCorrectly_SingleSelection()
        {
            var comp = Context.Render<TreeViewItemSelectedBindingTest>(self => self.Add(x => x.SelectedValue, "item1.2"));
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1.2");
            comp.Find("p.item1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-2-selected").TrimmedText().Should().Be("True");
        }

        [Test]
        public void InitialValueOfTreeViewItemSelected_Should_InfluenceSelectedValue_SingleSelection()
        {
            var comp = Context.Render<TreeViewItemSelectedBindingTest>(self => self
                .Add(x => x.SelectedValue, "item1.2")
                .Add(x => x.Item1Selected, true));
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1");
            comp.Find("p.item1-selected").TrimmedText().Should().Be("True");
            comp.Find("p.item1-1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-2-selected").TrimmedText().Should().Be("False");
        }

        [Test]
        public void TreeViewItemSelected_ShouldBeInitializedCorrectly_MultiSelection()
        {
            var comp = Context.Render<TreeViewItemSelectedBindingTest>(self => self
                .Add(x => x.SelectedValues, ["item1", "item1.2"])
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            comp.Find("p.selected-values").TrimmedText().Should().Be("item1, item1.2");
            comp.Find("p.item1-selected").TrimmedText().Should().Be("True");
            comp.Find("p.item1-1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-2-selected").TrimmedText().Should().Be("True");
        }

        [Test]
        public void TreeViewItemVisible_RendersWhenVisibleIsTrue()
        {
            var comp = Context.Render<ItemVisibleTreeViewTest>(element =>
            {
                element.Add(x => x.IsElementVisible, true);
            });

            comp.FindAll("li").Should().HaveCount(1);
        }

        [Test]
        public void TreeViewItemVisible_RendersNotWhenVisibleIsFalse()
        {
            var comp = Context.Render<ItemVisibleTreeViewTest>(element =>
            {
                element.Add(x => x.IsElementVisible, false);
            });

            comp.FindAll("li").Should().HaveCount(0);
        }

        [Test]
        public void TreeViewFilterFunc_FindTopElement()
        {
            // Arrange and act
            var searchPhrase = "Trash";
            var comp = Context.Render<TreeViewFilterFuncTest>(element =>
            {
                element.Add(x => x.SearchPhrase, searchPhrase);
                element.Add(x => x.FilterFunc, (e) =>
                {
                    if (string.IsNullOrEmpty(e.Text))
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(e.Text.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase));
                });
            });

            // Assert that the element "Trash" is visible
            comp.Instance.Items.Should().Contain(e => e.Text.Equals("Trash") && e.Visible);

            // Assert that the element "Categories" is invisible
            comp.Instance.Items.Should().Contain(e => e.Text.Equals("Categories") && !e.Visible);

            // Assert that the element "Social" is invisible
            var categoriesNode = comp.Instance.Items.ElementAt(1);
            categoriesNode.Children.Should().Contain(e => e.Text.Equals("Social") && !e.Visible);
        }

        [Test]
        public void TreeViewFilterFunc_FindExpandableAndChildrenElements()
        {
            // Arrange and act
            var searchPhrase = "Categories";
            var comp = Context.Render<TreeViewFilterFuncTest>(element =>
            {
                element.Add(x => x.SearchPhrase, searchPhrase);
                element.Add(x => x.FilterFunc, (e) =>
                {
                    if (string.IsNullOrEmpty(e.Text))
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(e.Text.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase));
                });
            });

            // Assert that the element "Trash" is invisible
            comp.Instance.Items.Should().Contain(e => e.Text.Equals("Trash") && !e.Visible);

            // Assert that the element "Categories" is visible
            comp.Instance.Items.Should().Contain(e => e.Text.Equals("Categories") && e.Visible);

            // Assert that the element "Social" is invisible
            var categoriesNode = comp.Instance.Items.ElementAt(1);
            categoriesNode.Children.Should().Contain(e => e.Text.Equals("Social") && !e.Visible);
        }

        [Test]
        public void TreeViewFilterFunc_FindChildElement()
        {
            // Arrange and act
            var searchPhrase = "Social";
            var comp = Context.Render<TreeViewFilterFuncTest>(element =>
            {
                element.Add(x => x.SearchPhrase, searchPhrase);
                element.Add(x => x.FilterFunc, (e) =>
                {
                    if (string.IsNullOrEmpty(e.Text))
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(e.Text.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase));
                });
            });

            // Assert that the element "Trash" is invisible
            comp.Instance.Items.Should().Contain(e => e.Text.Equals("Trash") && !e.Visible);

            // Assert that the element "Categories" is visible
            comp.Instance.Items.Should().Contain(e => e.Text.Equals("Categories") && e.Visible);

            // Assert that the element "Social" is visible
            var categoriesNode = comp.Instance.Items.ElementAt(1);
            categoriesNode.Children.Should().Contain(e => e.Text.Equals("Social") && e.Visible);
        }

        [Test]
        public void TreeViewFilterFunc_ItemsAreNull()
        {
            // Arrange and act
            var searchPhrase = "Social";
            var comp = Context.Render<TreeViewFilterFuncTest>(element =>
            {
                element.Add(x => x.SearchPhrase, "Social");
                element.Add(x => x.AreItemsPopulated, false);
                element.Add(x => x.FilterFunc, (e) =>
                {
                    if (string.IsNullOrEmpty(e.Text))
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(e.Text.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase));
                });
            });

            // Assert that the items are null
            comp.Instance.Items.Should().BeNull();
        }

        [Test]
        public void TreeViewFilterFunc_FilterFuncIsNull()
        {
            var comp = Context.Render<TreeViewFilterFuncTest>(element =>
            {
                element.Add(x => x.SearchPhrase, "Social");
            });

            // Assert that the element "Trash" is visible
            comp.Instance.Items.Should().Contain(e => e.Text.Equals("Trash") && e.Visible);

            // Assert that the element "Categories" is visible
            comp.Instance.Items.Should().Contain(e => e.Text.Equals("Categories") && e.Visible);

            // Assert that the element "Social" is visible
            var categoriesNode = comp.Instance.Items.ElementAt(1);
            categoriesNode.Children.Should().Contain(e => e.Text.Equals("Social") && e.Visible);
        }

        [Test]
        public void InitialValueOfTreeViewItemSelected_Should_InfluenceSelectedValue_MultiSelection()
        {
            var comp = Context.Render<TreeViewItemSelectedBindingTest>(self => self
                .Add(x => x.SelectedValues, ["item1", "item1.2"])
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Add(x => x.Item11Selected, true));
            comp.Find("p.selected-values").TrimmedText().Should().Be("item1, item1.1, item1.2");
            comp.Find("p.item1-selected").TrimmedText().Should().Be("True");
            comp.Find("p.item1-1-selected").TrimmedText().Should().Be("True");
            comp.Find("p.item1-2-selected").TrimmedText().Should().Be("True");
        }

        /// <summary>
        /// Note: in this test the trees are synchronized solely via their item's Selected parameter
        /// </summary>
        [Test]
        public async Task TreeViewItem_Selected_TwoWayBindingTest_SingleSelection()
        {
            var comp = Context.Render<TreeViewItemSelectedBindingTest>(self => self.Add(x => x.SelectedValue, "item1.2"));
            // check initial selection
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find("p.item1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-2-selected").TrimmedText().Should().Be("True");

            // select another value on tree1 and check selection has changed on both trees
            await comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClickAsync();
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1");
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find("p.item1-selected").TrimmedText().Should().Be("True");
            comp.Find("p.item1-1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-2-selected").TrimmedText().Should().Be("False");

            // select another value on tree2 and check selection has changed on both trees
            await comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClickAsync();
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1.1");
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find("p.item1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-1-selected").TrimmedText().Should().Be("True");
            comp.Find("p.item1-2-selected").TrimmedText().Should().Be("False");
        }

        [Test]
        public async Task TreeViewItem_Selected_TwoWayBindingTest_MultiSelection()
        {
            var comp = Context.Render<TreeViewItemSelectedBindingTest>(self => self
                .Add(x => x.SelectedValues, ["item1", "item1.2"])
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            // check initial selection
            comp.Find(".tree1 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-null");
            comp.Find(".tree2 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-null");
            // note: the tristate checkbox is null because not all its children are selected ...
            // ... this doesn't mean that the item's Selected value isn't true. Checking:
            foreach (var item in comp.FindComponents<MudTreeViewItem<string>>().Where(x => x.Instance.Value == "item1"))
            {
                item.Instance.GetState<bool>(nameof(MudTreeViewItem<string>.Selected)).Should().Be(true);
            }
            comp.Find(".tree1 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree2 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree1 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree2 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find("p.selected-values").TrimmedText().Should().Be("item1, item1.2");
            comp.Find("p.item1-selected").TrimmedText().Should().Be("True");
            comp.Find("p.item1-1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-2-selected").TrimmedText().Should().Be("True");

            // select another value on tree1 and check selection has changed on both trees
            await comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClickAsync();
            comp.Find(".tree1 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree2 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree1 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree2 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree1 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree2 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find("p.selected-values").TrimmedText().Should().Be("item1, item1.1, item1.2");
            comp.Find("p.item1-selected").TrimmedText().Should().Be("True");
            comp.Find("p.item1-1-selected").TrimmedText().Should().Be("True");
            comp.Find("p.item1-2-selected").TrimmedText().Should().Be("True");

            // remove a value on tree2 and check selection has changed on both trees
            await comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClickAsync();
            comp.Find(".tree1 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree2 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree1 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree2 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree1 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree2 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find("p.selected-values").TrimmedText().Should().Be("");
            comp.Find("p.item1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-2-selected").TrimmedText().Should().Be("False");
        }

        [Test]
        public async Task TreeView_WhenDisabled_DoesNotHaveRipple()
        {
            var comp = Context.Render<TreeViewRippleTest>(self => self.Add(x => x.Disabled, true));

            comp.FindAll("div.mud-ripple").Count.Should().Be(0);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Disabled, false));
            comp.FindAll("div.mud-ripple").Count.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task TreeView_WhenRippleDisabled_DoesNotHaveRipple()
        {
            var comp = Context.Render<TreeViewRippleTest>(self => self.Add(x => x.Ripple, false));

            comp.FindAll("div.mud-ripple").Count.Should().Be(0);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Ripple, true));
            comp.FindAll("div.mud-ripple").Count.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task Collapsed_ClickOnArrowButton_CheckClose()
        {
            var comp = Context.Render<TreeViewTest1>();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(10);
            await comp.Find("button.mud-treeview-item-expand-button").ClickAsync();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(1);
            await comp.Find("button.mud-treeview-item-expand-button").ClickAsync();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(0);
            await comp.Find("div.mud-treeview-item-content").ClickAsync();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(0);
        }

        [Test]
        public async Task DoubleClickOnArrowButton_ShouldNotSelectItem()
        {
            var comp = Context.Render<TreeViewTest1>(self => self.Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(10);
            await comp.Find("button.mud-treeview-item-expand-button").ClickAsync();
            comp.FindAll("input.mud-checkbox-input").Count.Should().Be(10);
            comp.Instance.SubItemSelected.Should().BeFalse();
            comp.Instance.Item1Selected.Should().BeFalse();
            // double-click on expand button should not influence selection
            await comp.Find("button.mud-treeview-item-expand-button").DoubleClickAsync();
            comp.Instance.SubItemSelected.Should().BeFalse();
            comp.Instance.Item1Selected.Should().BeFalse();
            await comp.Find("input.mud-checkbox-input").ChangeAsync(true);
            comp.Instance.SubItemSelected.Should().BeTrue();
            comp.Instance.Item1Selected.Should().BeTrue();
            // double-click on expand button should not influence selection
            await comp.Find("button.mud-treeview-item-expand-button").DoubleClickAsync();
            comp.Instance.SubItemSelected.Should().BeTrue();
            comp.Instance.Item1Selected.Should().BeTrue();
        }

        /// <summary>
        /// Only expandable items instantiate a toggle button; leaves render the arrow placeholder as plain markup.
        /// </summary>
        [Test]
        public void LeafItems_DoNotInstantiateToggleButton()
        {
            var comp = Context.Render<TreeViewTest1>();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(10);
            // Every item keeps the arrow slot because it is the indent gutter that aligns leaves with their parents.
            comp.FindAll("div.mud-treeview-item-arrow").Count.Should().Be(10);
            // Four of the ten items have children, so the other six pay for no component.
            comp.FindComponents<MudTreeViewItemToggleButton>().Count.Should().Be(4);
            comp.FindAll("div.mud-treeview-item-arrow button").Count.Should().Be(4);
        }

        /// <summary>
        /// Double-clicking a leaf's arrow placeholder must not reach the item, matching the expand button (#9419).
        /// </summary>
        [Test]
        public async Task DoubleClickOnLeafArrow_ShouldNotSelectItem()
        {
            var comp = Context.Render<TreeViewTest1>(self => self.Add(x => x.SelectionMode, SelectionMode.SingleSelection));
            // Index 2 is "MudBlazor.svg", a leaf, so its arrow is the empty placeholder rather than a button.
            comp.FindAll("div.mud-treeview-item-arrow")[2].QuerySelector("button").Should().BeNull();

            await comp.FindAll("div.mud-treeview-item-arrow")[2].DoubleClickAsync();
            comp.Instance.SelectedValue.Should().BeNull();

            // The same double-click on the item content does select, which is what the placeholder shields against.
            await comp.FindAll("div.mud-treeview-item-content")[2].DoubleClickAsync();
            comp.Instance.SelectedValue.Should().Be("MudBlazor.svg");
        }

        [Test]
        public async Task Collapsed_ClickOnTreeItem_CheckClose()
        {
            var comp = Context.Render<TreeViewTest2>();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(10);
            await comp.Find("button.mud-treeview-item-expand-button").ClickAsync();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(1);
            await comp.Find("button.mud-treeview-item-expand-button").ClickAsync();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(0);
            await comp.Find("div.mud-treeview-item-content").ClickAsync();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(1);
            await comp.Find("div.mud-treeview-item-content").ClickAsync();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(0);
        }

        [Test]
        public async Task Unselected_Select_CheckSelected_Deselect_CheckDeselected()
        {
            var comp = Context.Render<TreeViewTest1>(self => self.Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(10);
            await comp.Find("button.mud-treeview-item-expand-button").ClickAsync();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(1);
            comp.FindAll("input.mud-checkbox-input").Count.Should().Be(10);
            await comp.Find("input.mud-checkbox-input").ChangeAsync(true);
            comp.Instance.SubItemSelected.Should().BeTrue();
            comp.Instance.Item1Selected.Should().BeTrue();
            await comp.FindAll("input.mud-checkbox-input")[2].ChangeAsync(false);
            comp.Instance.SubItemSelected.Should().BeFalse();
            comp.Instance.Item1Selected.Should().BeFalse(); // <-- selecting child updates parent in multi-selection mode
        }

        [Test]
        public async Task Normal_Activate_CheckActivated_ActivateAnother_CheckBoth()
        {
            var comp = Context.Render<TreeViewTest1>(self => self.Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            comp.FindAll(".mud-checkbox-true").Count.Should().Be(0);
            await comp.Find("div.mud-treeview-item-content").ClickAsync();
            comp.Instance.Item1Selected.Should().BeTrue();
            comp.Instance.Item2Selected.Should().BeFalse();
            comp.FindAll(".mud-checkbox-true").Count.Should().Be(4); // item1 + entire sub-tree checked
            await comp.FindAll("div.mud-treeview-item-content")[4].ClickAsync();
            comp.Instance.Item1Selected.Should().BeTrue();
            comp.Instance.Item2Selected.Should().BeTrue();
            comp.FindAll(".mud-checkbox-true").Count.Should().Be(10);  // + item2 + entire sub-tree checked
        }

        [Test]
        public async Task TreeView_WillUnselectItems_WhenNotMultiSelect()
        {
            var comp = Context.Render<TreeViewTest7>();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(0);
            await comp.Find("div.mud-treeview-item-content").ClickAsync();
            comp.Instance.Item1Selected.Should().BeTrue();
            comp.Instance.Item2Selected.Should().BeFalse();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(1);
            await comp.FindAll("div.mud-treeview-item-content")[4].ClickAsync();
            comp.Instance.Item1Selected.Should().BeFalse();
            comp.Instance.Item2Selected.Should().BeTrue();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(1);
        }

        [Test]
        public async Task Normal_Activate_CheckActivated_Deactivate_Check()
        {
            var comp = Context.Render<TreeViewTest1>(self => self.Add(x => x.SelectionMode, SelectionMode.ToggleSelection));
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(0);
            await comp.Find("div.mud-treeview-item-content").ClickAsync();
            comp.Instance.Item1Selected.Should().BeTrue();
            comp.Instance.Item2Selected.Should().BeFalse();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(1);
            await comp.Find("div.mud-treeview-item-content").ClickAsync();
            comp.Instance.Item1Selected.Should().BeFalse();
            comp.Instance.Item2Selected.Should().BeFalse();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(0);
        }

        [Test]
        public void RenderWithTemplate_CheckResult()
        {
            var comp = Context.Render<TreeViewTemplateTest>();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(8);
        }

        /// <summary>
        /// Verifies that flattened virtualized rows expose their depth and sibling position, which the nested lists of a standard tree convey structurally.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_DataItems_ExposesAccessibleHierarchy()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>();

            var root = FindTreeItem(comp, "Root");
            root.GetAttribute("aria-level").Should().Be("1");
            root.GetAttribute("aria-posinset").Should().Be("1");
            root.GetAttribute("aria-setsize").Should().Be("2");

            var childA = FindTreeItem(comp, "Child A");
            childA.GetAttribute("aria-level").Should().Be("2");
            childA.GetAttribute("aria-posinset").Should().Be("1");
            childA.GetAttribute("aria-setsize").Should().Be("2");

            var childB = FindTreeItem(comp, "Child B");
            childB.GetAttribute("aria-level").Should().Be("2");
            childB.GetAttribute("aria-posinset").Should().Be("2");
            childB.GetAttribute("aria-setsize").Should().Be("2");

            await childB.QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();

            comp.WaitForAssertion(() =>
            {
                var grandchild = FindTreeItem(comp, "Grandchild");
                grandchild.GetAttribute("aria-level").Should().Be("3");
                grandchild.GetAttribute("aria-posinset").Should().Be("1");
                grandchild.GetAttribute("aria-setsize").Should().Be("1");
            });
        }

        /// <summary>
        /// Verifies that repeated references to one backing item render distinct rows without failing.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_DuplicateReferences_RenderDistinctRows()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.DuplicateSharedItem, true));
            var sharedRows = FindSharedRows(comp);
            sharedRows.Should().HaveCount(2);

            await sharedRows[1].QuerySelector("div.mud-treeview-item-content")!.ClickAsync();

            comp.Instance.SelectedValue.Should().Be("Shared");
            FindSharedRows(comp).Should().OnlyContain(row => row.QuerySelector("div.mud-treeview-item-content")!.ClassList.Contains("mud-treeview-item-selected"));

            static IReadOnlyList<IElement> FindSharedRows(IRenderedComponent<TreeViewVirtualizationTest> component)
            {
                return component.FindAll("li.mud-treeview-item")
                    .Where(item => item.FirstElementChild?.TextContent.Contains("Shared", StringComparison.Ordinal) == true)
                    .ToList();
            }
        }

        /// <summary>
        /// Verifies that virtualization falls back to standard rendering without a constrained height.
        /// </summary>
        [Test]
        public void TreeViewVirtualize_DataItemsWithoutConstrainedHeight_UsesStandardRenderer()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.Height, (string)null!));

            comp.FindComponents<Virtualize<TreeViewItemContext<string>>>().Should().BeEmpty();
            comp.FindAll("ul.mud-treeview-group").Should().NotBeEmpty();
            comp.Find("li.mud-treeview-item").HasAttribute("aria-level").Should().BeFalse();
        }

        /// <summary>
        /// Verifies that MaxHeight enables flattened virtualized data-item rendering.
        /// </summary>
        [Test]
        public void TreeViewVirtualize_DataItemsWithMaxHeight_UsesFlattenedVisibleItems()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.Height, (string)null!)
                .Add(x => x.MaxHeight, "200px"));

            var virtualize = comp.FindComponent<Virtualize<TreeViewItemContext<string>>>();

            virtualize.Instance.Items.Should().NotBeNull();
            virtualize.Instance.Items!.Select(x => x.Item.Value).Should().Equal("Root", "Child A", "Child B", "Other");
            virtualize.Instance.Items!.Select(x => x.Depth).Should().Equal(0, 1, 1, 0);
        }

        /// <summary>
        /// Verifies that explicit virtualization settings and the dense default item size are passed to the underlying virtualizer.
        /// </summary>
        [Test]
        public void TreeViewVirtualize_DataItems_PassesVirtualizationSettings()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.ItemSize, 40f)
                .Add(x => x.OverscanCount, 8)
                .Add(x => x.MaxItemCount, 500));

            var virtualize = comp.FindComponent<Virtualize<TreeViewItemContext<string>>>();

            virtualize.Instance.ItemSize.Should().Be(40f);
            virtualize.Instance.OverscanCount.Should().Be(8);
            virtualize.Instance.MaxItemCount.Should().Be(500);

            var denseComp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.Dense, true)
                .Add(x => x.ItemSize, 0f));

            denseComp.FindComponent<Virtualize<TreeViewItemContext<string>>>().Instance.ItemSize.Should().Be(26f);
        }

        /// <summary>
        /// Verifies that switching from standard to virtual rendering reads expansion, visibility, and selection from backing data.
        /// </summary>
        [Test]
        public async Task TreeView_DataItems_StandardToVirtual_UsesBackingDataState()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.Virtualize, false)
                .Add(x => x.BindExpanded, false));
            var childB = FindDataItem(comp.Instance.TreeItems, "Child B");
            var childA = FindDataItem(comp.Instance.TreeItems, "Child A");
            var grandchild = FindDataItem(comp.Instance.TreeItems, "Grandchild");

            await FindTreeItem(comp, "Child B").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();

            childB.Expanded.Should().BeFalse();
            FindTreeItem(comp, "Grandchild").Should().NotBeNull();
            childA.Visible = false;
            grandchild.Selected = true;

            await comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Virtualize, true)));

            childB.Expanded.Should().BeFalse();
            GetVirtualizedValues(comp).Should().Equal("Root", "Child B", "Other");
            comp.Instance.SelectedValue.Should().Be("Grandchild");
        }

        /// <summary>
        /// Verifies that switching from virtual to standard rendering preserves expansion state.
        /// </summary>
        [Test]
        public async Task TreeView_DataItems_VirtualToStandard_PreservesExpansion()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>();

            await FindTreeItem(comp, "Child B").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();
            GetVirtualizedValues(comp).Should().Contain("Grandchild");

            await comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Virtualize, false)));

            comp.FindComponents<Virtualize<TreeViewItemContext<string>>>().Should().BeEmpty();
            FindTreeItem(comp, "Grandchild").Should().NotBeNull();
        }

        /// <summary>
        /// Verifies that a moved item keeps its row component and receives its current hierarchy depth.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_DataItems_MovedItemKeepsComponentAndReceivesCurrentDepth()
        {
            var movableItem = new TreeItemData<string> { Value = "Movable", Text = "Movable" };
            var parentItem = new TreeItemData<string> { Value = "Parent", Text = "Parent", Children = [movableItem] };
            var comp = Context.Render<MudTreeView<string>>(parameters => parameters
                .Add(x => x.Items, new List<ITreeItemData<string>> { parentItem, movableItem })
                .Add(x => x.Virtualize, true)
                .Add(x => x.Height, "120px")
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Add(x => x.ItemTemplate, item => builder =>
                {
                    builder.OpenComponent<MudTreeViewItem<string>>(0);
                    builder.AddAttribute(1, nameof(MudTreeViewItem<string>.Value), item.Value);
                    builder.AddAttribute(2, nameof(MudTreeViewItem<string>.Text), item.Text);
                    builder.AddAttribute(3, nameof(MudTreeViewItem<string>.Items), item.Children);
                    builder.CloseComponent();
                }));
            var movableComponent = comp.FindComponents<MudTreeViewItem<string>>()
                .Single(item => item.Instance.CurrentItemData?.Value == "Movable")
                .Instance;

            var movable = FindTreeItem(comp, "Movable");
            movable.GetAttribute("style").Should().Contain("--mud-treeview-item-depth:0");
            movable.GetAttribute("aria-level").Should().Be("1");
            movable.GetAttribute("aria-posinset").Should().Be("2");
            movable.GetAttribute("aria-setsize").Should().Be("2");

            parentItem.Expanded = true;
            await comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Items, new List<ITreeItemData<string>> { parentItem })));

            comp.WaitForAssertion(() =>
            {
                var movedItem = FindTreeItem(comp, "Movable");
                movedItem.GetAttribute("style").Should().Contain("--mud-treeview-item-depth:1");
                movedItem.GetAttribute("aria-level").Should().Be("2");
                movedItem.GetAttribute("aria-posinset").Should().Be("1");
                movedItem.GetAttribute("aria-setsize").Should().Be("1");
            });
            comp.FindComponents<MudTreeViewItem<string>>()
                .Single(item => item.Instance.CurrentItemData?.Value == "Movable")
                .Instance.Should().BeSameAs(movableComponent, "rows are keyed by backing item");
        }

        /// <summary>
        /// Verifies that string data items use their text as the selected value when Value is null.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_DataItems_SelectedValueUsesTextFallbackWhenValueIsNull()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.UseTextOnlyValues, true)
                .Add(x => x.AutoExpand, true));

            await comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.SelectedValue, "Grandchild")));

            comp.WaitForAssertion(() =>
            {
                comp.Instance.SelectedValue.Should().Be("Grandchild");
                GetTreeViewItemContexts(comp).Select(itemContext => itemContext.Item.Text).Should().Contain("Grandchild");
            });
        }

        /// <summary>
        /// Verifies that replacing virtualized items and selecting a replacement in one parameter update uses the replacement hierarchy.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_ItemsReplacementAndSelectionUseCurrentHierarchy()
        {
            var initial = new TreeItemData<string> { Value = "Initial" };
            var replacement = new TreeItemData<string> { Value = "Replacement" };
            var comp = Context.Render<MudTreeView<string>>(parameters => parameters
                .Add(x => x.Items, [initial])
                .Add(x => x.Virtualize, true)
                .Add(x => x.Height, "80px")
                .Add(x => x.ItemTemplate, item => builder =>
                {
                    builder.OpenComponent<MudTreeViewItem<string>>(0);
                    builder.AddAttribute(1, nameof(MudTreeViewItem<string>.Value), item.Value);
                    builder.AddAttribute(2, nameof(MudTreeViewItem<string>.Selected), item.Selected);
                    builder.CloseComponent();
                }));

            await comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Items, [replacement])
                .Add(x => x.SelectedValue, "Replacement")));

            replacement.Selected.Should().BeTrue();
            FindTreeItem(comp, "Replacement").QuerySelector("div.mud-treeview-item-content")!
                .ClassList.Should().Contain("mud-treeview-item-selected");
        }

        /// <summary>
        /// Verifies that selecting a virtualized parent selects all of its descendants.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_MultiSelection_SelectingParentSelectsDescendants()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection));

            await FindTreeItem(comp, "Root").QuerySelector("div.mud-treeview-item-content")!.ClickAsync();

            comp.Instance.SelectedValues.Should().BeEquivalentTo(["Root", "Child A", "Child B", "Grandchild"]);
            GetDataItems(comp.Instance.TreeItems).Should().ContainSingle(item => item.Value == "Grandchild" && item.Selected);

            await FindTreeItem(comp, "Root").QuerySelector("div.mud-treeview-item-content")!.ClickAsync();

            comp.Instance.SelectedValues.Should().BeEmpty();
            GetDataItems(comp.Instance.TreeItems).Should().OnlyContain(item => !item.Selected);
        }

        /// <summary>
        /// Verifies that selecting a duplicated visible root does not update ancestors of its collapsed duplicate occurrence.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_DuplicateReferenceUsesClickedRowForAncestors()
        {
            var shared = new TreeItemData<string> { Value = "Shared", Text = "Shared" };
            var collapsedParent = new TreeItemData<string>
            {
                Value = "Parent",
                Text = "Parent",
                Children = [shared]
            };
            var comp = Context.Render<MudTreeView<string>>(parameters => parameters
                .Add(x => x.Items, [shared, collapsedParent])
                .Add(x => x.Virtualize, true)
                .Add(x => x.Height, "120px")
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Add(x => x.AutoSelectParent, true)
                .Add(x => x.ItemTemplate, item => builder =>
                {
                    builder.OpenComponent<MudTreeViewItem<string>>(0);
                    builder.AddAttribute(1, nameof(MudTreeViewItem<string>.Value), item.Value);
                    builder.AddAttribute(2, nameof(MudTreeViewItem<string>.Text), item.Text);
                    builder.AddAttribute(3, nameof(MudTreeViewItem<string>.Items), item.Children);
                    builder.AddAttribute(4, nameof(MudTreeViewItem<string>.Expanded), item.Expanded);
                    builder.CloseComponent();
                }));

            await FindTreeItem(comp, "Shared").QuerySelector("div.mud-treeview-item-content")!.ClickAsync();

            shared.Selected.Should().BeTrue();
            collapsedParent.Selected.Should().BeFalse();
        }

        /// <summary>
        /// Verifies that virtualized multi-selection invokes each item's SelectedChanged callback.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_MultiSelection_InvokesItemSelectedChanged()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            var initialChangeCount = comp.Instance.ItemSelectedChangedCount;

            await FindTreeItem(comp, "Child A").QuerySelector("div.mud-treeview-item-content")!.ClickAsync();

            comp.Instance.ItemSelectedChangedCount.Should().BeGreaterThan(initialChangeCount);
            FindDataItem(comp.Instance.TreeItems, "Child A").Selected.Should().BeTrue();
        }

        /// <summary>
        /// Verifies that external multi-selection updates collapsed virtualized item data.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_MultiSelection_ExternalSelectionUpdatesCollapsedItemData()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection));

            await comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.SelectedValues, ["Grandchild"])));

            FindDataItem(comp.Instance.TreeItems, "Grandchild").Selected.Should().BeTrue();
            GetDataItems(comp.Instance.TreeItems)
                .Where(item => item.Value != "Grandchild")
                .Should()
                .OnlyContain(item => !item.Selected);
        }

        /// <summary>
        /// Verifies that a row mounting in the same parent render as an external deselection of its item does not overwrite that deselection.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_ExternalDeselectionOfMountingRow_IsReconciled()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Add(x => x.SelectedValues, ["Grandchild"]));
            var grandchild = FindDataItem(comp.Instance.TreeItems, "Grandchild");
            grandchild.Selected.Should().BeTrue();

            await comp.InvokeAsync(() =>
            {
                grandchild.Selected = false;
                FindDataItem(comp.Instance.TreeItems, "Child B").Expanded = true;
                comp.Instance.Rerender();
            });

            comp.WaitForAssertion(() => FindTreeItem(comp, "Grandchild").QuerySelector(".mud-checkbox span")!.ClassList.Should().NotContain("mud-checkbox-true"));
            comp.Instance.SelectedValues.Should().BeEmpty();
        }

        /// <summary>
        /// Verifies that a backing selection change made together with a tree-only refresh, such as expanding all, is reconciled for rows on screen.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_ExternalSelectionWithTreeOnlyRefresh_IsReconciled()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            var tree = comp.FindComponent<MudTreeView<string>>().Instance;
            var childA = FindDataItem(comp.Instance.TreeItems, "Child A");

            await comp.InvokeAsync(async () =>
            {
                childA.Selected = true;
                await tree.ExpandAllAsync();
            });

            comp.WaitForAssertion(() => FindTreeItem(comp, "Child A").QuerySelector(".mud-checkbox span")!.ClassList.Should().Contain("mud-checkbox-true"));
            comp.Instance.SelectedValues.Should().BeEquivalentTo(["Child A"]);
        }

        /// <summary>
        /// Verifies that an ordinary external selection update projects the current hierarchy only once for its render.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_ExternalSelectionProjectsCurrentHierarchyOnce()
        {
            var item = new ProjectionTrackingTreeItemData("Item");
            var comp = Context.Render<MudTreeView<string>>(parameters => parameters
                .Add(x => x.Items, [item])
                .Add(x => x.Virtualize, true)
                .Add(x => x.Height, "80px")
                .Add(x => x.ItemTemplate, itemData => builder =>
                {
                    builder.OpenComponent<MudTreeViewItem<string>>(0);
                    builder.AddAttribute(1, nameof(MudTreeViewItem<string>.Value), itemData.Value);
                    builder.CloseComponent();
                }));
            item.ResetProjectionReadCount();

            await comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.SelectedValue, "Item")));

            item.Selected.Should().BeTrue();
            item.VisibleReadCount.Should().Be(1);
        }

        /// <summary>
        /// Verifies that a folder without a value whose children are all selected shows a checked box in both render modes.
        /// </summary>
        [Test]
        public void TreeView_ValuelessFolderWithAllChildrenSelected_ShowsCheckedInBothModes()
        {
            var standard = RenderFolderTree(Context, virtualize: false);
            var virtualized = RenderFolderTree(Context, virtualize: true);

            GetCheckState(standard).Should().Be("mud-checkbox-true");
            GetCheckState(virtualized).Should().Be("mud-checkbox-true");

            static IRenderedComponent<MudTreeView<int?>> RenderFolderTree(BunitContext context, bool virtualize)
            {
                var folder = new TreeItemData<int?>
                {
                    Text = "Folder",
                    Expanded = true,
                    Children = [new TreeItemData<int?> { Value = 1, Text = "One" }, new TreeItemData<int?> { Value = 2, Text = "Two" }]
                };
                return context.Render<MudTreeView<int?>>(parameters => parameters
                    .Add(x => x.Items, [folder])
                    .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                    .Add(x => x.SelectedValues, [1, 2])
                    .Add(x => x.Virtualize, virtualize)
                    .Add(x => x.Height, "200px")
                    .Add(x => x.ItemTemplate, item => builder =>
                    {
                        builder.OpenComponent<MudTreeViewItem<int?>>(0);
                        builder.AddAttribute(1, nameof(MudTreeViewItem<int?>.Value), item.Value);
                        builder.AddAttribute(2, nameof(MudTreeViewItem<int?>.Text), item.Text);
                        builder.AddAttribute(3, nameof(MudTreeViewItem<int?>.Items), item.Children);
                        builder.AddAttribute(4, nameof(MudTreeViewItem<int?>.Expanded), item.Expanded);
                        builder.CloseComponent();
                    }));
            }

            static string GetCheckState(IRenderedComponent<MudTreeView<int?>> component)
            {
                return FindTreeItem(component, "Folder").QuerySelector(".mud-checkbox span")!.ClassList
                    .Single(className => className.StartsWith("mud-checkbox-", StringComparison.Ordinal));
            }
        }

        /// <summary>
        /// Verifies that a valueless descendant does not make an otherwise fully selected parent indeterminate,
        /// so both render modes agree. A valueless item cannot be selected, so it must not count as unselected.
        /// </summary>
        [Test]
        public void TreeView_ValuelessDescendant_ShowsSameCheckStateInBothModes()
        {
            var standard = RenderLabelChildTree(Context, virtualize: false);
            var virtualized = RenderLabelChildTree(Context, virtualize: true);

            GetParentCheckState(standard).Should().Be("mud-checkbox-true");
            GetParentCheckState(virtualized).Should().Be("mud-checkbox-true");

            static IRenderedComponent<MudTreeView<int?>> RenderLabelChildTree(BunitContext context, bool virtualize)
            {
                var parent = new TreeItemData<int?>
                {
                    Value = 1,
                    Text = "Parent",
                    Expanded = true,
                    Children =
                    [
                        new TreeItemData<int?> { Value = 2, Text = "SelectedChild" },
                        new TreeItemData<int?> { Value = null, Text = "LabelChild" }
                    ]
                };

                return context.Render<MudTreeView<int?>>(parameters => parameters
                    .Add(x => x.Items, [parent])
                    .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                    .Add(x => x.SelectedValues, [1, 2])
                    .Add(x => x.Virtualize, virtualize)
                    .Add(x => x.Height, "200px")
                    .Add(x => x.ItemTemplate, item => builder =>
                    {
                        builder.OpenComponent<MudTreeViewItem<int?>>(0);
                        builder.AddAttribute(1, nameof(MudTreeViewItem<int?>.Value), item.Value);
                        builder.AddAttribute(2, nameof(MudTreeViewItem<int?>.Text), item.Text);
                        builder.AddAttribute(3, nameof(MudTreeViewItem<int?>.Items), item.Children);
                        builder.AddAttribute(4, nameof(MudTreeViewItem<int?>.Expanded), item.Expanded);
                        builder.CloseComponent();
                    }));
            }

            static string GetParentCheckState(IRenderedComponent<MudTreeView<int?>> component)
            {
                return FindTreeItem(component, "Parent").QuerySelector(".mud-checkbox span")!.ClassList
                    .Single(className => className.StartsWith("mud-checkbox-", StringComparison.Ordinal));
            }
        }

        /// <summary>
        /// Verifies that reloading an item on a tree without <see cref="MudTreeView{T}.ServerData"/> keeps the
        /// backing children, because no load will run to restore them.
        /// </summary>
        [Test]
        public async Task TreeView_ReloadAsyncWithoutServerData_KeepsBackingChildren()
        {
            var parent = new TreeItemData<string>
            {
                Value = "parent",
                Text = "Parent",
                Expanded = true,
                Children = [new TreeItemData<string> { Value = "child", Text = "Child" }]
            };

            var comp = Context.Render<MudTreeView<string>>(parameters => parameters
                .Add(x => x.Items, [parent])
                .Add(x => x.ItemTemplate, item => builder =>
                {
                    builder.OpenComponent<MudTreeViewItem<string>>(0);
                    builder.AddAttribute(1, nameof(MudTreeViewItem<string>.Value), item.Value);
                    builder.AddAttribute(2, nameof(MudTreeViewItem<string>.Text), item.Text);
                    builder.AddAttribute(3, nameof(MudTreeViewItem<string>.Items), item.Children);
                    builder.AddAttribute(4, nameof(MudTreeViewItem<string>.Expanded), item.Expanded);
                    builder.CloseComponent();
                }));

            var parentItem = comp.FindComponents<MudTreeViewItem<string>>()
                .Single(x => x.Instance.Text == "Parent").Instance;

            await comp.InvokeAsync(() => parentItem.ReloadAsync());

            parent.Children.Should().ContainSingle("a tree without ServerData cannot restore cleared children");
        }

        /// <summary>
        /// Verifies that virtualized tri-state and parent selection use the complete backing data tree.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_MultiSelection_UsesDataTreeForTriStateAndParentSelection()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Add(x => x.SelectedValues, ["Child A"]));

            FindTreeItem(comp, "Root").QuerySelector(".mud-checkbox span")!.ClassList.Should().Contain("mud-checkbox-null");

            await FindTreeItem(comp, "Child B").QuerySelector("div.mud-treeview-item-content")!.ClickAsync();

            comp.Instance.SelectedValues.Should().BeEquivalentTo(["Root", "Child A", "Child B", "Grandchild"]);
            FindTreeItem(comp, "Root").QuerySelector(".mud-checkbox span")!.ClassList.Should().Contain("mud-checkbox-true");
        }

        /// <summary>
        /// Verifies that a parent render traverses collapsed descendants at most once.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_ParentRender_DoesNotRepeatedlyEnumerateNestedCollapsedDescendants()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.RootExpanded, false));
            await comp.InvokeAsync(comp.Instance.ResetNestedDescendantEnumerationCount);

            await comp.InvokeAsync(comp.Instance.Rerender);

            comp.Instance.NestedDescendantEnumerationCount.Should().BeLessThanOrEqualTo(1);
        }

        /// <summary>
        /// Verifies that a row which mounts with an already selected value does not make the tree re-project or re-render more than an unselected row does.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_SelectedRowMount_DoesNotReprojectTree()
        {
            var unselectedComp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            var selectedComp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Add(x => x.SelectedValues, ["Grandchild"]));

            var unselected = await MeasureExpandAsync(unselectedComp);
            var selected = await MeasureExpandAsync(selectedComp);

            selected.Should().Be(unselected, "the projection already holds the mounted row's selection");

            // Expands the collapsed "Child B" and reports how often the tree rendered and how often it enumerated the collapsed children.
            static async Task<(int TreeRenders, int NestedEnumerations)> MeasureExpandAsync(IRenderedComponent<TreeViewVirtualizationTest> comp)
            {
                var tree = comp.FindComponent<MudTreeView<string>>();
                await comp.InvokeAsync(comp.Instance.ResetNestedDescendantEnumerationCount);
                var renders = tree.RenderCount;

                await FindTreeItem(comp, "Child B").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();
                comp.WaitForAssertion(() => FindTreeItem(comp, "Grandchild").Should().NotBeNull());

                return (tree.RenderCount - renders, comp.Instance.NestedDescendantEnumerationCount);
            }
        }

        /// <summary>
        /// Verifies that a row mounting while the tree is held inside an item's asynchronous selected-changed callback does not break the selection update.
        /// </summary>
        [Test]
        [CancelAfter(5000)]
        public async Task TreeViewVirtualize_RowMountDuringSelectedChangedCallback_CompletesSelectionUpdate()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.AutoExpand, true)
                .Add(x => x.SelectedValue, "Child A"));
            comp.WaitForAssertion(() => FindTreeItem(comp, "Child A").QuerySelector("div.mud-treeview-item-content")!.ClassList.Should().Contain("mud-treeview-item-selected"));
            await comp.InvokeAsync(comp.Instance.GateNextItemSelectedChange);

            // Selecting the collapsed grandchild auto-expands its parent, and the walk which deselects "Child A" stops inside the gated callback.
            var selectionTask = comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.SelectedValue, "Grandchild")));
            await comp.Instance.ItemSelectedChangeStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
            await selectionTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            // The parent render queued by the selection change mounts the grandchild row while the walk is suspended.
            comp.WaitForAssertion(() => FindTreeItem(comp, "Grandchild").Should().NotBeNull());
            await comp.InvokeAsync(comp.Instance.CompleteItemSelectedChange);
            // The resumed walk continues on the renderer's dispatcher, so this runs after it finished.
            await comp.InvokeAsync(() => { });
            if (Context.Renderer.UnhandledException.IsCompleted)
            {
                var exception = await Context.Renderer.UnhandledException;
                Assert.Fail($"The selection walk failed after a row mounted during its callback: {exception.Message}");
            }

            comp.Instance.SelectedValue.Should().Be("Grandchild");
            comp.WaitForAssertion(() => FindTreeItem(comp, "Grandchild").QuerySelector("div.mud-treeview-item-content")!.ClassList.Should().Contain("mud-treeview-item-selected"));
        }

        /// <summary>
        /// Verifies that the item-level expand-all and collapse-all methods work on a virtualized row by updating its backing data.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_ItemExpandAllAndCollapseAll_UpdateBackingData()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>();
            var childB = comp.FindComponents<MudTreeViewItem<string>>().Single(item => item.Instance.CurrentItemData?.Value == "Child B").Instance;

            await comp.InvokeAsync(childB.ExpandAllAsync);

            comp.WaitForAssertion(() => GetVirtualizedValues(comp).Should().Contain("Grandchild"));

            await comp.InvokeAsync(childB.CollapseAllAsync);

            comp.WaitForAssertion(() => GetVirtualizedValues(comp).Should().NotContain("Grandchild"));
        }

        /// <summary>
        /// Verifies that expanding all immediately includes items added in place since the previous render.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_ExpandAllUsesCurrentInPlaceHierarchy()
        {
            var items = new List<ITreeItemData<string>> { new TreeItemData<string> { Value = "Initial" } };
            var addedParent = new TreeItemData<string>
            {
                Value = "Added parent",
                Children = [new TreeItemData<string> { Value = "Added child" }]
            };
            var comp = Context.Render<MudTreeView<string>>(parameters => parameters
                .Add(x => x.Items, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.Height, "120px")
                .Add(x => x.ItemTemplate, item => builder =>
                {
                    builder.OpenComponent<MudTreeViewItem<string>>(0);
                    builder.AddAttribute(1, nameof(MudTreeViewItem<string>.Value), item.Value);
                    builder.AddAttribute(2, nameof(MudTreeViewItem<string>.Items), item.Children);
                    builder.CloseComponent();
                }));
            items.Add(addedParent);

            await comp.InvokeAsync(comp.Instance.ExpandAllAsync);

            addedParent.Expanded.Should().BeTrue();
            comp.WaitForAssertion(() => FindTreeItem(comp, "Added child").Should().NotBeNull());
        }

        /// <summary>
        /// Verifies that filtering immediately includes items added in place since the previous render.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_FilterUsesCurrentInPlaceHierarchy()
        {
            var items = new List<ITreeItemData<string>> { new TreeItemData<string> { Value = "Initial" } };
            var added = new TreeItemData<string> { Value = "Added" };
            var comp = Context.Render<MudTreeView<string>>(parameters => parameters
                .Add(x => x.Items, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.Height, "120px")
                .Add(x => x.FilterFunc, _ => Task.FromResult(false))
                .Add(x => x.ItemTemplate, item => builder =>
                {
                    builder.OpenComponent<MudTreeViewItem<string>>(0);
                    builder.AddAttribute(1, nameof(MudTreeViewItem<string>.Value), item.Value);
                    builder.CloseComponent();
                }));
            items.Add(added);

            await comp.InvokeAsync(comp.Instance.FilterAsync);

            added.Visible.Should().BeFalse();
            comp.WaitForAssertion(() => comp.FindAll("li.mud-treeview-item").Should().BeEmpty());
        }

        /// <summary>
        /// Verifies that server-loaded children are added to the flattened virtualized item list.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_ServerData_AddsLoadedChildrenToFlattenedItems()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>();

            GetVirtualizedValues(comp).Should().Equal("Root");

            await FindTreeItem(comp, "Root").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();

            comp.WaitForAssertion(() => GetVirtualizedValues(comp).Should().Equal("Root", "Loaded 1"));
            FindTreeItem(comp, "Loaded 1").GetAttribute("style").Should().Contain("--mud-treeview-item-depth:1");
        }

        /// <summary>
        /// Verifies that a pending server load stays bound to its originating data item when that item is replaced.
        /// </summary>
        [Test]
        [CancelAfter(5000)]
        public async Task TreeViewVirtualize_ServerData_ReplacedItemStillReceivesItsOwnLoad()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.UseTextFallback, true)
                .Add(x => x.DelayServerData, true));
            var originatingItem = comp.Instance.TreeItems.Single();

            var loadTask = FindTreeItem(comp, "Node A").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();
            await comp.Instance.ServerDataStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            await comp.InvokeAsync(comp.Instance.ReplaceRootWithTextFallback);
            var replacementItem = comp.Instance.TreeItems.Single();
            await comp.InvokeAsync(comp.Instance.CompleteServerData);
            await loadTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            originatingItem.Children.Should().ContainSingle(item => item.Value == "Loaded 1");
            replacementItem.Children.Should().BeNull();
            FindTreeItem(comp, "Node B").QuerySelector("button.mud-treeview-item-expand-button").Should().NotBeNull();
        }

        /// <summary>
        /// Verifies that an asynchronous expanded callback loads the item that initiated it even after its row scrolled away.
        /// </summary>
        [Test]
        [CancelAfter(5000)]
        public async Task TreeViewVirtualize_ServerData_ExpandedCallbackAfterScrollLoadsOriginatingItem()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.UseTextFallback, true)
                .Add(x => x.DelayExpandedChanged, true)
                .Add(x => x.RootCount, 2)
                .Add(x => x.Height, "40px")
                .Add(x => x.OverscanCount, 0)
                .Add(x => x.MaxItemCount, 1));
            var originatingItem = comp.Instance.TreeItems[0];
            var otherItem = comp.Instance.TreeItems[1];

            var expandTask = FindTreeItem(comp, "Node A").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();
            await comp.Instance.ExpandedChangeStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
            await ScrollVirtualizerAsync(comp, spacerSize: 40f, containerSize: 40f);
            FindTreeItem(comp, "Node B").Should().NotBeNull();

            await comp.InvokeAsync(comp.Instance.CompleteExpandedChange);
            await expandTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            comp.Instance.LoadedParentValues.Should().Equal("Node A");
            originatingItem.Children.Should().ContainSingle(item => item.Value == "Loaded 1");
            otherItem.Children.Should().BeNull();
        }

        /// <summary>
        /// Verifies that scrolling keeps the row components of items which stay visible instead of re-creating every row.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_Scroll_KeepsRowComponentsOfItemsStillVisible()
        {
            var items = Enumerable.Range(1, 30)
                .Select(index => (ITreeItemData<string>)new TreeItemData<string> { Value = $"Item {index}", Text = $"Item {index}" })
                .ToList();
            var comp = Context.Render<MudTreeView<string>>(parameters => parameters
                .Add(x => x.Items, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.Height, "80px")
                .Add(x => x.OverscanCount, 1)
                .Add(x => x.ItemTemplate, item => builder =>
                {
                    builder.OpenComponent<MudTreeViewItem<string>>(0);
                    builder.AddAttribute(1, nameof(MudTreeViewItem<string>.Value), item.Value);
                    builder.AddAttribute(2, nameof(MudTreeViewItem<string>.Text), item.Text);
                    builder.CloseComponent();
                }));
            await ScrollVirtualizerAsync(comp, spacerSize: 200f, containerSize: 80f);
            var before = GetRowComponents(comp);

            await ScrollVirtualizerAsync(comp, spacerSize: 240f, containerSize: 80f);

            var after = GetRowComponents(comp);
            var stillVisible = after.Keys.Intersect(before.Keys).ToList();
            stillVisible.Should().NotBeEmpty("the two-row viewport moved by one row");
            stillVisible.Should().OnlyContain(item => ReferenceEquals(after[item], before[item]));

            static Dictionary<ITreeItemData<string>, MudTreeViewItem<string>> GetRowComponents(IRenderedComponent<MudTreeView<string>> component)
            {
                var rows = new Dictionary<ITreeItemData<string>, MudTreeViewItem<string>>(ReferenceEqualityComparer.Instance);
                foreach (var row in component.FindComponents<MudTreeViewItem<string>>())
                {
                    rows.Add(row.Instance.CurrentItemData!, row.Instance);
                }

                return rows;
            }
        }

        /// <summary>
        /// Verifies that an expanded callback can make the originating item ineligible for server loading.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_ServerData_ExpandedCallbackCanCancelLoading()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.DisableExpandOnExpandedChanged, true));

            await FindTreeItem(comp, "Root").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();

            comp.Instance.LoadCount.Should().Be(0);
            comp.Instance.TreeItems[0].Children.Should().BeNull();
        }

        /// <summary>
        /// Verifies that disposing the tree invalidates a pending load before it can update backing data.
        /// </summary>
        [Test]
        [CancelAfter(5000)]
        public async Task TreeViewVirtualize_ServerData_DisposalRejectsPendingCompletion()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.DelayServerData, true));
            var fixture = comp.Instance;
            var originatingItem = fixture.TreeItems.Single();
            var loadTask = FindTreeItem(comp, "Root").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();
            await fixture.ServerDataStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            await Context.DisposeComponentsAsync();
            fixture.CompleteServerData();
            await loadTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            originatingItem.Children.Should().BeNull();
        }

        /// <summary>
        /// Verifies that a server load completing after virtualization is enabled updates the backing data item.
        /// </summary>
        [Test]
        [CancelAfter(5000)]
        public async Task TreeView_ServerData_PendingStandardLoadSurvivesVirtualization()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.Virtualize, false)
                .Add(x => x.DelayServerData, true));

            var loadTask = FindTreeItem(comp, "Root").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();
            await comp.Instance.ServerDataStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
            await comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Virtualize, true)));

            await comp.InvokeAsync(comp.Instance.CompleteServerData);
            await loadTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            comp.Instance.TreeItems[0].Children.Should().ContainSingle(item => item.Value == "Loaded 1");
            GetVirtualizedValues(comp).Should().Equal("Root", "Loaded 1");
            comp.Instance.LoadCount.Should().Be(1);
        }

        /// <summary>
        /// Verifies that a virtual server load completing in standard mode refreshes the replacement tree.
        /// </summary>
        /// <remarks>
        /// A standard tree does not read <see cref="ITreeItemData{T}.Selected"/> from the backing data unless the item template binds it, so the loaded child is not expected to become selected here.
        /// </remarks>
        [Test]
        [CancelAfter(5000)]
        public async Task TreeViewVirtualize_ServerData_PendingLoadRefreshesStandardTree()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.DelayServerData, true));
            var loadTask = FindTreeItem(comp, "Root").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();
            await comp.Instance.ServerDataStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
            await comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Virtualize, false)));

            await comp.InvokeAsync(comp.Instance.CompleteServerData);
            await loadTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            comp.WaitForAssertion(() => FindTreeItem(comp, "Loaded 1").Should().NotBeNull());
            comp.Instance.SelectedValue.Should().BeNull();
            FindTreeItem(comp, "Root").QuerySelector("button.mud-treeview-item-expand-button")!
                .ClassList.Should().NotContain("mud-treeview-item-arrow-load");
        }

        /// <summary>
        /// Verifies that a virtualized item whose backing data is not expandable shows no expand button, since the tree refuses to load its children.
        /// </summary>
        [Test]
        public void TreeViewVirtualize_ServerData_NotExpandableItemShowsNoExpandButton()
        {
            var comp = Context.Render<MudTreeView<string>>(parameters => parameters
                .Add(x => x.Items, [new TreeItemData<string> { Value = "Leaf", Text = "Leaf", Expandable = false }])
                .Add(x => x.Virtualize, true)
                .Add(x => x.Height, "80px")
                .Add(x => x.ServerData, _ => Task.FromResult<IReadOnlyCollection<TreeItemData<string>>>([]))
                .Add(x => x.ItemTemplate, item => builder =>
                {
                    builder.OpenComponent<MudTreeViewItem<string>>(0);
                    builder.AddAttribute(1, nameof(MudTreeViewItem<string>.Value), item.Value);
                    builder.AddAttribute(2, nameof(MudTreeViewItem<string>.Text), item.Text);
                    builder.AddAttribute(3, nameof(MudTreeViewItem<string>.Items), item.Children);
                    builder.CloseComponent();
                }));

            FindTreeItem(comp, "Leaf").QuerySelector("button.mud-treeview-item-expand-button").Should().BeNull();
        }

        /// <summary>
        /// Verifies that a reload superseded while its items callback was pending does not wipe the children a newer reload loaded.
        /// </summary>
        [Test]
        [CancelAfter(5000)]
        public async Task TreeViewVirtualize_ServerData_SupersededReloadDoesNotWipeNewerChildren()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.InitialChildCount, 1)
                .Add(x => x.BindItems, true)
                .Add(x => x.DelayItemsChanged, true)
                .Add(x => x.DelayServerDataIndependently, true));
            var rootItem = comp.FindComponent<MudTreeViewItem<string>>().Instance;
            var rootData = comp.Instance.TreeItems[0];

            var firstReload = comp.InvokeAsync(rootItem.ReloadAsync);
            await comp.Instance.ItemsChangeStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
            var secondReload = comp.InvokeAsync(rootItem.ReloadAsync);
            await comp.Instance.LoadStarted(1).WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
            await comp.InvokeAsync(() => comp.Instance.CompleteServerData(1));
            await comp.Instance.SecondItemsChangeStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            await comp.InvokeAsync(comp.Instance.CompleteItemsChange);
            await Task.WhenAll(firstReload, secondReload).WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            rootData.Children.Should().ContainSingle(item => item.Value == "Loaded 1", "the superseded reload must not wipe the newer load");
        }

        /// <summary>
        /// Verifies that reload supersedes a pending request and ignores its stale completion.
        /// </summary>
        [Test]
        [CancelAfter(5000)]
        public async Task TreeViewVirtualize_ServerData_ReloadSupersedesPendingLoad()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.DelayServerDataIndependently, true));
            var rootItem = comp.FindComponent<MudTreeViewItem<string>>();
            var firstLoadTask = comp.InvokeAsync(rootItem.Instance.TryInvokeServerLoadFunc);
            await comp.Instance.ServerDataStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            var reloadTask = comp.InvokeAsync(rootItem.Instance.ReloadAsync);

            try
            {
                await comp.Instance.LoadStarted(2).WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
                await comp.InvokeAsync(() => comp.Instance.CompleteServerData(2));
                await reloadTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
                comp.Instance.TreeItems[0].Children.Should().ContainSingle(item => item.Value == "Loaded 2");

                await comp.InvokeAsync(() => comp.Instance.CompleteServerData(1));
                await firstLoadTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
                comp.Instance.TreeItems[0].Children.Should().ContainSingle(item => item.Value == "Loaded 2");
            }
            finally
            {
                await comp.InvokeAsync(comp.Instance.CompleteAllServerDataRequests);
            }
        }

        /// <summary>
        /// Verifies that a failure from a superseded request does not escape or replace fresh children.
        /// </summary>
        [Test]
        [CancelAfter(5000)]
        public async Task TreeViewVirtualize_ServerData_ReloadIgnoresSupersededFailure()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.DelayServerDataIndependently, true));
            var rootItem = comp.FindComponent<MudTreeViewItem<string>>();
            var firstLoadTask = FindTreeItem(comp, "Root").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();
            await comp.Instance.ServerDataStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
            var reloadTask = comp.InvokeAsync(rootItem.Instance.ReloadAsync);

            try
            {
                await comp.Instance.LoadStarted(2).WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
                await comp.InvokeAsync(() => comp.Instance.CompleteServerData(2));
                await reloadTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

                await comp.InvokeAsync(() => comp.Instance.FailServerData(1));
                var act = () => firstLoadTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

                await act.Should().NotThrowAsync();
                comp.Instance.TreeItems[0].Children.Should().ContainSingle(item => item.Value == "Loaded 2");

                var currentFailureTask = comp.InvokeAsync(rootItem.Instance.ReloadAsync);
                await comp.Instance.LoadStarted(3).WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
                await comp.InvokeAsync(() => comp.Instance.FailServerData(3));
                var currentFailure = () => currentFailureTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
                await currentFailure.Should().ThrowAsync<InvalidOperationException>();
                comp.WaitForAssertion(() => comp.Find("ul.mud-treeview")
                    .QuerySelector("button.mud-treeview-item-arrow-load")
                    .Should().BeNull());

                var retryRootItem = comp.FindComponent<MudTreeViewItem<string>>();
                var retryTask = comp.InvokeAsync(retryRootItem.Instance.ReloadAsync);
                await comp.Instance.LoadStarted(4).WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
                await comp.InvokeAsync(() => comp.Instance.CompleteServerData(4));
                await retryTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
                comp.WaitForAssertion(() =>
                {
                    comp.Instance.TreeItems[0].Children.Should().ContainSingle(item => item.Value == "Loaded 4");
                    GetVirtualizedValues(comp).Should().Contain("Loaded 4");
                });
            }
            finally
            {
                await comp.InvokeAsync(comp.Instance.CompleteAllServerDataRequests);
            }
        }

        /// <summary>
        /// Verifies that reload recency is based on invocation order when an earlier items callback is delayed.
        /// </summary>
        [Test]
        [CancelAfter(5000)]
        public async Task TreeViewVirtualize_ServerData_LatestReloadInvocationWinsWhenEarlierItemsCallbackIsDelayed()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.BindItems, true)
                .Add(x => x.DelayItemsChanged, true)
                .Add(x => x.DelayServerDataIndependently, true)
                .Add(x => x.InitialChildCount, 1));
            var rootItem = comp.FindComponent<MudTreeViewItem<string>>();
            var earlierReloadTask = comp.InvokeAsync(rootItem.Instance.ReloadAsync);
            await comp.Instance.ItemsChangeStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            var latestReloadTask = comp.InvokeAsync(rootItem.Instance.ReloadAsync);

            try
            {
                await comp.Instance.ServerDataStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
                comp.Instance.LoadCount.Should().Be(1);

                await comp.InvokeAsync(comp.Instance.CompleteItemsChange);
                var firstOutcome = await Task.WhenAny(earlierReloadTask, comp.Instance.LoadStarted(2))
                    .WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

                firstOutcome.Should().BeSameAs(earlierReloadTask);
                comp.Instance.LoadCount.Should().Be(1);

                await comp.InvokeAsync(() => comp.Instance.CompleteServerData(1));
                await Task.WhenAll(earlierReloadTask, latestReloadTask)
                    .WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
                comp.Instance.TreeItems[0].Children.Should().ContainSingle(item => item.Value == "Loaded 1");
            }
            finally
            {
                await comp.InvokeAsync(comp.Instance.CompleteAllServerDataRequests);
            }
        }

        /// <summary>
        /// Verifies that a standard load completing after a virtual reload cannot supersede that reload.
        /// </summary>
        [Test]
        [CancelAfter(5000)]
        public async Task TreeView_ServerData_PendingStandardLoadCannotSupersedeVirtualReload()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.Virtualize, false)
                .Add(x => x.BindItems, true)
                .Add(x => x.DelayItemsChanged, true)
                .Add(x => x.DelayServerDataIndependently, true));
            comp.Instance.TreeItems[0].Children = Array.Empty<TreeItemData<string>>();
            await comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Virtualize, false)));
            var standardRootItem = comp.FindComponent<MudTreeViewItem<string>>();
            var standardLoadTask = comp.InvokeAsync(standardRootItem.Instance.TryInvokeServerLoadFunc);
            await comp.Instance.ServerDataStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            await comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Virtualize, true)));
            var virtualRootItem = comp.FindComponents<MudTreeViewItem<string>>()
                .Single(component => ReferenceEquals(component.Instance.CurrentItemData, comp.Instance.TreeItems[0]));
            var reloadTask = comp.InvokeAsync(virtualRootItem.Instance.ReloadAsync);
            await comp.Instance.ItemsChangeStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            try
            {
                await comp.InvokeAsync(() => comp.Instance.CompleteServerData(1));
                var oldLoadOutcome = await Task.WhenAny(standardLoadTask, comp.Instance.SecondItemsChangeStarted)
                    .WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

                oldLoadOutcome.Should().BeSameAs(standardLoadTask);
                comp.Instance.TreeItems[0].Children.Should().BeEmpty();

                await comp.InvokeAsync(comp.Instance.CompleteItemsChange);
                await comp.Instance.LoadStarted(2).WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
                comp.Instance.LoadCount.Should().Be(2);

                await comp.InvokeAsync(() => comp.Instance.CompleteServerData(2));
                await Task.WhenAll(standardLoadTask, reloadTask)
                    .WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
                comp.Instance.TreeItems[0].Children.Should().ContainSingle(item => item.Value == "Loaded 2");
            }
            finally
            {
                await comp.InvokeAsync(comp.Instance.CompleteItemsChange);
                await comp.InvokeAsync(comp.Instance.CompleteAllServerDataRequests);
            }
        }

        /// <summary>
        /// Verifies that children loaded in standard mode are written to the backing item and reused after switching to virtual rendering.
        /// </summary>
        [Test]
        public async Task TreeView_ServerData_StandardToVirtual_ReusesLoadedBackingChildren()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.Virtualize, false));

            await FindTreeItem(comp, "Root").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();

            comp.Instance.LoadCount.Should().Be(1);
            comp.Instance.TreeItems[0].Children.Should().ContainSingle(item => item.Value == "Loaded 1");
            comp.Instance.TreeItems[0].Expanded = false;

            await comp.InvokeAsync(() => comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Virtualize, true)));

            GetVirtualizedValues(comp).Should().Equal("Root");
            var virtualRootItem = comp.FindComponents<MudTreeViewItem<string>>()
                .Single(component => ReferenceEquals(component.Instance.CurrentItemData, comp.Instance.TreeItems[0]));
            virtualRootItem.FindComponent<MudTreeViewItemToggleButton>().Instance.Expanded.Should().BeFalse();

            await FindTreeItem(comp, "Root").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();

            comp.Instance.LoadCount.Should().Be(1, "the children are already loaded on the backing item");
            comp.Instance.TreeItems[0].Expanded.Should().BeTrue();
            comp.WaitForAssertion(() => GetVirtualizedValues(comp).Should().Equal("Root", "Loaded 1"));
        }

        [Test]
        public async Task TreeViewServer()
        {
            var comp = Context.Render<TreeViewServerTest>();
            string.Join('|', comp.FindAll("div.mud-treeview-item-content").Select(x => x.TextContent)).Should()
                .Be("All Mail|Categories|Social|Updates|Trash");
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(5);
            await comp.FindAll("div.mud-treeview-item-content")[4].ClickAsync();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(5); // <-- nothing loaded as it's not expandable
            await comp.FindAll("div.mud-treeview-item-content")[0].ClickAsync();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(6); // <- loaded one more from server
            await comp.FindAll("div.mud-treeview-item-content")[3].ClickAsync();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(7); // <- loaded another one from server
            // now loading a child of a server loaded item
            await comp.FindAll("div.mud-treeview-item-content")[1].ClickAsync();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(8); // <- loaded another one from server
            string.Join('|', comp.FindAll("div.mud-treeview-item-content").Select(x => x.TextContent)).Should()
                .Be("All Mail|Loaded 1|Loaded 3|Categories|Social|Loaded 2|Updates|Trash");
        }

#nullable enable
        [Test]
        public async Task TreeViewServerData_BindsItems()
        {
            var comp = Context.Render<TreeViewServerTest>();
            var target = comp.FindComponents<MudTreeViewItem<string?>>()
                .First(x => x.Instance.Value == "All Mail");
            await comp.InvokeAsync(target.Instance.ReloadAsync);

            var root = comp.Instance.TreeItems.First(x => x.Value == "All Mail");
            root.Children.Should().NotBeNull();
            root.Children!.Should().HaveCount(1);
            root.Children.First().Value.Should().StartWith("Loaded");
        }
#nullable disable

        [Test]
        public async Task TreeViewItem_ShouldBeAbleTo_ReloadInCollapsedState()
        {
            var comp = Context.Render<TreeViewServerTest2>();
            var treeviewItem = comp.FindComponents<MudTreeViewItem<string>>().FirstOrDefault();
            treeviewItem!.Instance.GetState<bool>(nameof(MudTreeViewItem<string>.Expanded)).Should().Be(false);
            comp.FindAll("div.mud-treeview-item-arrow button").Count.Should().Be(2);
            // expand first tree
            await comp.Find("div.mud-treeview-item-arrow button").ClickAsync();
            comp.FindAll("div.mud-treeview-item-arrow button").Count.Should().Be(3);
            // collapse first tree again
            await comp.Find("div.mud-treeview-item-arrow button").ClickAsync();
            treeviewItem.Instance.GetState<bool>(nameof(MudTreeViewItem<string>.Expanded)).Should().Be(false);
            // reload first tree in collapsed state
            var reloadTask = Task.CompletedTask;
            await comp.InvokeAsync(() => reloadTask = treeviewItem.Instance.ReloadAsync());
            await reloadTask;
            comp.FindAll("div.mud-treeview-item-arrow button").Count.Should().Be(3);
        }

        [Test]
        public void TreeViewTreeItemData()
        {
            // test default values
            new TreeItemData<int>().Expanded.Should().Be(false);
            new TreeItemData<int>().Selected.Should().Be(false);
            new TreeItemData<int>().Expandable.Should().Be(true);
            new TreeItemData<int>().Visible.Should().Be(true);
            new TreeItemData<int>().Text.Should().Be(null);
            new TreeItemData<int>().Icon.Should().Be(null);
            new TreeItemData<int>().HasChildren.Should().Be(false);
            new TreeItemData<int>().Children.Should().BeNull();

            var data = new TreeItemData<string>()
            {
                Value = "val",
                Icon = "i",
                Text = "t",
                Expandable = false,
                Expanded = true,
                Visible = true,
                Selected = true,
                Children = [new TreeItemData<string>()]
            };
            data.Value.Should().Be("val");
            data.Icon.Should().Be("i");
            data.Text.Should().Be("t");
            data.Expandable.Should().Be(false);
            data.Expanded.Should().Be(true);
            data.Visible.Should().Be(true);
            data.Selected.Should().Be(true);
            data.HasChildren.Should().Be(true);
            data.Children.Count.Should().Be(1);
            new TreeItemData<int> { Value = 17 }.Should().Be(new TreeItemData<int> { Value = 17 });
            new TreeItemData<int> { Value = 17 }.Should().NotBe(new TreeItemData<int> { Value = 77 });
            new TreeItemData<int> { Value = 17 }.GetHashCode().Should().Be(17.GetHashCode());
            Equals(new TreeItemData<int> { Value = 17 }, new TreeItemData<int> { Value = 17 }).Should().Be(true);
            Equals(new TreeItemData<int> { Value = 17 }, new TreeItemData<int> { Value = 18 }).Should().Be(false);
            Equals(new TreeItemData<int> { Value = 17 }, null).Should().Be(false);
            var x = new TreeItemData<int> { Value = 17 };
            Equals(x, x).Should().Be(true);
            x.Equals(x).Should().Be(true);
            x.Equals(null).Should().Be(false);
            new TreeItemData<string>().GetHashCode().Should().Be(0);
            new TreeItemData<string>().Equals(new TreeItemData<string>()).Should().Be(true);
            new TreeItemData<int>().Value.Should().Be(default);
        }

        [Test]
        public async Task TreeViewItem_DoubleClick_CheckExpanded()
        {
            var comp = Context.Render<TreeViewTest3>();
            var itemExpanded = false;

            var item = comp.FindComponent<MudTreeViewItem<string>>();
            await item.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.OnDoubleClick, new EventCallback<MouseEventArgs>(null, (Action)(() => itemExpanded = !itemExpanded))));

            comp.FindAll("li.mud-treeview-item").Count.Should().Be(10);

            await comp.Find("div.mud-treeview-item-content").DoubleClickAsync();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(1);
            itemExpanded.Should().BeTrue();

            await comp.Find("div.mud-treeview-item-content").DoubleClickAsync();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(0);
            itemExpanded.Should().BeFalse();
        }

        [Test]
        public async Task TreeViewItem_DoubleClick_CheckSelected()
        {
            var comp = Context.Render<TreeViewTest3>();
            string selectedItem = null;

            var tree = comp.FindComponent<MudTreeView<string>>();

            await tree.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SelectedValueChanged, new EventCallback<string>(null, (Action<string>)((s) => selectedItem = s))));

            await comp.Find("div.mud-treeview-item-content").DoubleClickAsync();
            selectedItem.Should().Be("content");
        }

        [Test]
        public async Task TreeViewItem_ProgrammaticallySelect()
        {
            var comp = Context.Render<TreeViewTest4>();
            var treeView = comp.FindComponent<MudTreeView<string>>();

            await comp.InvokeAsync(() => comp.Instance.ClickFirst());
            await comp.WaitForAssertionAsync(() => comp.Instance.SelectedValue.Should().Be("content"));

            await comp.InvokeAsync(() => comp.Instance.ClickSecond());
            await comp.WaitForAssertionAsync(() => comp.Instance.SelectedValue.Should().Be("src"));

            await comp.InvokeAsync(() => comp.Instance.ClickSecond());
            await comp.WaitForAssertionAsync(() => comp.Instance.SelectedValue.Should().Be(null));
        }

        [Test]
        public async Task TreeViewItem_BodyContent()
        {
            var comp = Context.Render<TreeViewTest5>();
            var treeView = comp.FindComponent<MudTreeView<string>>();
            var treeViewItem = comp.FindComponents<MudTreeViewItem<string>>()[2];

            comp.FindAll("ul.mud-treeview").Count.Should().Be(5);
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(4);
            comp.FindAll("div.mud-treeview-item-content").Count.Should().Be(4);
            comp.FindAll("div.mud-treeview-item-arrow").Count.Should().Be(4);
            comp.FindAll("div.mud-treeview-item-icon").Count.Should().Be(4);
            comp.FindAll("div.mud-treeview-item-bodycontent").Count.Should().Be(4);
            comp.FindAll("button.mud-treeview-item-arrow-expand").Count.Should().Be(4);
            comp.FindAll("p.mud-typography")[0].InnerHtml.MarkupMatches("This is item one");
            comp.FindAll("p.mud-typography")[1].InnerHtml.MarkupMatches("This is item two");
            comp.FindAll("p.mud-typography")[2].InnerHtml.MarkupMatches("This is item three");
            comp.FindAll("p.mud-typography")[3].InnerHtml.MarkupMatches("This is item six");
            comp.FindAll("div").Count.Should().Be(32);
            comp.FindAll("button").Count.Should().Be(12);

            // Test updating the treeview root.
            comp.Instance.SimulateUpdateRoot = true;
            var items = await comp.Instance.LoadServerData(null);
            await treeView.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Items, items));
            comp.FindAll("p.mud-typography")[1].InnerHtml.MarkupMatches("This is item 2");

            // Test reloading the treeview item.
            await comp.FindAll("button.mud-treeview-item-arrow-expand")[2].ClickAsync();
            comp.FindAll("p.mud-typography")[3].InnerHtml.MarkupMatches("This is item four");
            comp.FindAll("p.mud-typography")[4].InnerHtml.MarkupMatches("This is item five");

            comp.Instance.SimulateUpdate3 = true;
            await treeViewItem.InvokeAsync(treeViewItem.Instance.ReloadAsync);
            comp.FindAll("p.mud-typography")[3].InnerHtml.MarkupMatches("This is item 4");
            comp.FindAll("p.mud-typography")[4].InnerHtml.MarkupMatches("This is item 5");
        }

        [Test]
        public async Task TreeView_SetSelectedValue_SetsSelectedValue()
        {
            var comp = Context.Render<TreeViewTest6>();

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.SelectedValue, "logo.png"));

            comp.Instance.SelectedValue.Should().Be("logo.png");
        }

        [Test]
        public async Task TreeView_SetSelectedValue_IsSetNullWhenNotFound()
        {
            var comp = Context.Render<TreeViewTest6>();

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.SelectedValue, "logo.png"));

            comp.Instance.SelectedValue.Should().Be("logo.png");

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.SelectedValue, "xxxxxx"));

            comp.Instance.SelectedValue.Should().Be(default);
        }

        [Test]
        public async Task TreeView_SetSelectedValue_IsSetNullWhenInitialValueIsInvalid()
        {
            var comp = Context.Render<TreeViewTest6>();

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.SelectedValue, "xxxxxx"));

            comp.Instance.SelectedValue.Should().Be(default);
        }

        [Test]
        public async Task TreeView_SelectedValue_ShouldUseComparer()
        {
            // test tree with items ("Ax", "Bx", "Cx", "Dx")
            var comp = Context.Render<TreeViewCompareTest>();
            string GetSelectedValue() => comp.Find("p.selected-value").TrimmedText();

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.SelectedValue, "Ax"));
            GetSelectedValue().Should().Be("Ax");

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.SelectedValue, "Bx"));
            GetSelectedValue().Should().Be("Bx");

            // setting A will not select anything because it isn't a valid value with the default comparer
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.SelectedValue, "A"));
            GetSelectedValue().Should().BeNullOrWhiteSpace();

            // set the comparer to a value that will only check the first character of the string
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Comparer,
                new DelegateEqualityComparer<string>(
                    (x, y) =>
                    {
                        if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
                        {
                            return true;
                        }

                        if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
                        {
                            return false;
                        }

                        return x[0] == y[0];
                    },
                    obj =>
                    {
                        if (string.IsNullOrEmpty(obj))
                        {
                            return 0;
                        }

                        return obj[0].GetHashCode();
                    }
                )
            ));
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.SelectedValue, "A"));
            GetSelectedValue().Should().StartWith("A");
        }

        /// <summary>
        /// This test checks that when multiple values are selected and the compare parameter is updated,
        /// selected values are updated correctly.
        /// </summary>
        [Test]
        public async Task TreeView_SelectedValues_ShouldUseComparer()
        {
            // tree containing two children with values AA and AC
            var comp = Context.Render<TreeViewComparerMultiSelectTest>(self => self.Add(x => x.SelectedValues, ["AA"]));

            comp.Instance.Item1Selected.Should().BeTrue();
            comp.Instance.Item2Selected.Should().BeFalse();

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Comparer,
                new DelegateEqualityComparer<string>(
                    (x, y) =>
                    {
                        if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
                        {
                            return true;
                        }

                        if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
                        {
                            return false;
                        }

                        return x[0] == y[0];
                    },
                    obj =>
                    {
                        if (string.IsNullOrEmpty(obj))
                        {
                            return 0;
                        }

                        return obj[0].GetHashCode();
                    }
                )
            ));

            comp.Instance.Item1Selected.Should().BeTrue();
            comp.Instance.Item2Selected.Should().BeTrue();
        }

        /// <summary>
        /// Tests binding Selected and Expanded parameters, especially selection with items that have only Text and no Value
        ///
        /// NOTE: we can only check the component parameters directly here because they are two-way bound!
        /// </summary>
        [Test]
        public async Task TreeViewItem_TwoWayBinding()
        {
            var comp = Context.Render<TreeViewItemBindingTest>();
            // check initial selection
            comp.Find(".item-config .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-launch .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".item-tasks .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-images .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-logo .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            // expanded
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "config").Expanded.Should().Be(false);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "images").Expanded.Should().Be(false);
            // selected
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "launch.json").Selected.Should().Be(true);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "tasks.json").Selected.Should().Be(false);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "logo.png").Selected.Should().Be(false);
            // switches
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-config").ReadValue.Should().Be(false);
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-images").ReadValue.Should().Be(false);
            // checkboxes
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-launch").ReadValue.Should().Be(true);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-tasks").ReadValue.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-logo").ReadValue.Should().Be(false);

            // select logo.png via tree item
            await comp.Find(".item-logo .mud-treeview-item-content").ClickAsync();
            // selection visualization
            comp.Find(".item-config .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-launch .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-tasks .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-images .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-logo .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            // expanded
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "config").Expanded.Should().Be(false);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "images").Expanded.Should().Be(false);
            // selected
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "launch.json").Selected.Should().Be(false);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "tasks.json").Selected.Should().Be(false);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "logo.png").Selected.Should().Be(true);
            // switches
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-config").ReadValue.Should().Be(false);
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-images").ReadValue.Should().Be(false);
            // checkboxes
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-launch").ReadValue.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-tasks").ReadValue.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-logo").ReadValue.Should().Be(true);

            // expand config via tree item
            await comp.Find(".item-config .mud-treeview-item-arrow button").ClickAsync();
            // selection visualization
            comp.Find(".item-config .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-launch .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-tasks .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-images .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-logo .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            // expanded
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "config").Expanded.Should().Be(true);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "images").Expanded.Should().Be(false);
            // selected
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "launch.json").Selected.Should().Be(false);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "tasks.json").Selected.Should().Be(false);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "logo.png").Selected.Should().Be(true);
            // switches
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-config").ReadValue.Should().Be(true);
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-images").ReadValue.Should().Be(false);
            // checkboxes
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-launch").ReadValue.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-tasks").ReadValue.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-logo").ReadValue.Should().Be(true);

            // collapse config via switch
            await comp.Find(".switch-config input").ChangeAsync(false);
            // selection visualization
            comp.Find(".item-config .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-launch .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-tasks .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-images .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-logo .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            // expanded
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "config").Expanded.Should().Be(false);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "images").Expanded.Should().Be(false);
            // selected
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "launch.json").Selected.Should().Be(false);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "tasks.json").Selected.Should().Be(false);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "logo.png").Selected.Should().Be(true);
            // switches
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-config").ReadValue.Should().Be(false);
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-images").ReadValue.Should().Be(false);
            // checkboxes
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-launch").ReadValue.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-tasks").ReadValue.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-logo").ReadValue.Should().Be(true);

            // select launch.json via checkbox
            await comp.Find(".checkbox-launch input").ChangeAsync(true);
            // selection visualization
            comp.Find(".item-config .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-launch .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".item-tasks .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-images .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".item-logo .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            // expanded
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "config").Expanded.Should().Be(false);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "images").Expanded.Should().Be(false);
            // selected
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "launch.json").Selected.Should().Be(true);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "tasks.json").Selected.Should().Be(false);
            comp.FindComponents<MudTreeViewItem<string>>().Select(x => x.Instance).First(x => x.Text == "logo.png").Selected.Should().Be(false);
            // switches
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-config").ReadValue.Should().Be(false);
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-images").ReadValue.Should().Be(false);
            // checkboxes
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-launch").ReadValue.Should().Be(true);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-tasks").ReadValue.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-logo").ReadValue.Should().Be(false);
        }

        [Test]
        public async Task TreeViewAutoExpansion()
        {
            var comp = Context.Render<TreeViewAutoExpandTest>(self => self.Add(x => x.AutoExpand, true));
            var isExpanded = (string value) => comp.FindComponents<MudTreeViewItem<string>>()
                .FirstOrDefault(x => x.Instance.Value == value)?.Instance.GetState<bool>(nameof(MudTreeViewItem<string>.Expanded));
            var select = (string value) => comp.FindComponents<MudChip<string>>().FirstOrDefault(x => x.Instance.Text == value)?.Find("button.mud-chip").ClickAsync();
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            // select and check that along the path to the value all parents were expanded, nothing else
            await select("tasks.json");
            isExpanded("C:").Should().Be(true);
            isExpanded("config").Should().Be(true);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            // reset all to collapsed and check
            await comp.Find("button.collapse-all").ClickAsync();
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            // select and check that along the path to the value all parents were expanded, nothing else
            // here images itself must not be expanded, only its parent
            await select("images");
            isExpanded("C:").Should().Be(true);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
        }

        [Test]
        public async Task TreeViewAutoExpansion_ShouldNot_ExpandNonExpandableItems()
        {
            var comp = Context.Render<TreeViewAutoExpandTest>(self => self.Add(x => x.AutoExpand, true).Add(x => x.ConfigCanExpand, false));
            var isExpanded = (string value) => comp.FindComponents<MudTreeViewItem<string>>()
                .FirstOrDefault(x => x.Instance.Value == value)?.Instance.GetState<bool>(nameof(MudTreeViewItem<string>.Expanded));
            var select = (string value) => comp.FindComponents<MudChip<string>>().FirstOrDefault(x => x.Instance.Text == value)?.Find("button.mud-chip").ClickAsync();
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            // select and check that along the path to the value all parents were expanded, nothing else
            await select("tasks.json");
            isExpanded("C:").Should().Be(true);
            isExpanded("config").Should().Be(false); // <--- shouldn't be expanded because it can't
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            // reset all to collapsed and check
            await comp.Find("button.collapse-all").ClickAsync();
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            // select and check that along the path to the value all parents were expanded, nothing else
            // here images itself must not be expanded, only its parent
            await select("logo.png");
            isExpanded("C:").Should().Be(true);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(true);
            isExpanded("logo.png").Should().Be(false);
        }

        [Test]
        public async Task TreeViewExpandAllCollapseAll()
        {
            var comp = Context.Render<TreeViewAutoExpandTest>(self => self.Add(x => x.AutoExpand, false));
            var isExpanded = (string value) => comp.FindComponents<MudTreeViewItem<string>>()
                .FirstOrDefault(x => x.Instance.Value == value)?.Instance.GetState<bool>(nameof(MudTreeViewItem<string>.Expanded));
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            await comp.Find("button.expand-all").ClickAsync();
            isExpanded("C:").Should().Be(true);
            isExpanded("config").Should().Be(true);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(true);
            isExpanded("logo.png").Should().Be(false);
            await comp.Find("button.collapse-all").ClickAsync();
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
        }

        [Test]
        public async Task TreeViewExpandAll_ShouldNot_ExpandNonExpandableItems()
        {
            var comp = Context.Render<TreeViewAutoExpandTest>(self => self.Add(x => x.ConfigCanExpand, false));
            var isExpanded = (string value) => comp.FindComponents<MudTreeViewItem<string>>()
                .FirstOrDefault(x => x.Instance.Value == value)?.Instance.GetState<bool>(nameof(MudTreeViewItem<string>.Expanded));
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            await comp.Find("button.expand-all").ClickAsync();
            isExpanded("C:").Should().Be(true);
            isExpanded("config").Should().Be(false); // <--- shouldn't be expanded because it can't
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(true);
            isExpanded("logo.png").Should().Be(false);
            await comp.Find("button.collapse-all").ClickAsync();
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
        }

        [Test]
        public void TreeViewItem_SetParameters_ValueIsSetNull_WhenTextUnset_RootServerDataIsSet_Throw()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                var comp = Context.Render<TreeViewTest8>();
                comp.FindAll("li.mud-treeview-item").Count.Should().Be(4);
            });

#nullable enable
            MudTreeView<string>? nullInstanceTree = null;
            MudTreeViewItem<string>? nullInstanceItem = null;
#nullable disable

            exception.Message.Should().Be($"'{nameof(MudTreeView<string>)}.{nameof(nullInstanceTree.ServerData)}' requires '{nameof(nullInstanceTree.ItemTemplate)}.{nameof(MudTreeViewItem<string>)}.{nameof(nullInstanceItem.Value)}' to be supplied.");
        }

        [Test]
        public async Task TreeView_ClickItemWhileActive_DoesChangeSelection()
        {
            var comp = Context.Render<ItemSelectableTreeViewTest>();

            var parentItemButton = comp.Find(".parent-item button.mud-treeview-item-expand-button");
            var parentItemContent = comp.Find(".parent-item > div.mud-treeview-item-content");

            var GetSelectedValue = () => comp.Find("p.selected-value").TrimmedText();
            var GetItemExpandedValue = () => comp
                .FindComponent<MudTreeViewItem<string>>()
                .Instance
                .GetState<bool>(nameof(MudTreeViewItem<string>.Expanded));

            await parentItemContent.ClickAsync();

            GetSelectedValue().Should().Be("content");

            GetItemExpandedValue().Should().Be(false);
            await parentItemButton.ClickAsync();
            GetItemExpandedValue().Should().Be(true);
        }

        [Test]
        public async Task TreeView_ClickItemWhileReadOnly_DoesNotChangeSelection()
        {
            var comp = Context.Render<ItemSelectableTreeViewTest>(self => self.Add(x => x.ParentItemReadOnly, true));

            var parentItemButton = comp.Find(".parent-item button.mud-treeview-item-expand-button");
            var parentItemContent = comp.Find(".parent-item > div.mud-treeview-item-content");

            var GetSelectedValue = () => comp.Find("p.selected-value").TrimmedText();
            var GetItemExpandedValue = () => comp
                .FindComponent<MudTreeViewItem<string>>()
                .Instance
                .GetState<bool>(nameof(MudTreeViewItem<string>.Expanded));

            await parentItemContent.ClickAsync();
            GetSelectedValue().Should().BeNullOrWhiteSpace();

            GetItemExpandedValue().Should().Be(false);
            await parentItemButton.ClickAsync();
            GetItemExpandedValue().Should().Be(true);
        }

        [Test]
        public async Task TreeView_ClickItemWhileDisabled_DoesNotChangeSelectionAndExpanded()
        {
            var comp = Context.Render<ItemSelectableTreeViewTest>(self => self.Add(x => x.ParentItemDisabled, true));

            var parentItemButton = comp.Find(".parent-item button.mud-treeview-item-expand-button");
            var parentItemContent = comp.Find(".parent-item > div.mud-treeview-item-content");

            var GetSelectedValue = () => comp.Find("p.selected-value").TrimmedText();
            var GetItemExpandedValue = () => comp
                .FindComponent<MudTreeViewItem<string>>()
                .Instance
                .GetState<bool>(nameof(MudTreeViewItem<string>.Expanded));

            await parentItemContent.ClickAsync();
            GetSelectedValue().Should().BeNullOrWhiteSpace();

            GetItemExpandedValue().Should().Be(false);
            await parentItemButton.ClickAsync();
            GetItemExpandedValue().Should().Be(false);
        }

        [Test]
        public async Task TreeView_ClickHeterogeneousTreeElement_ShouldNotThrow()
        {
            var comp = Context.Render<TreeViewHeterogeneous>();
            var l2 = comp.Find(".L2 > div.mud-treeview-item-content");
            var act = () => l2.ClickAsync();
            await act.Should().NotThrowAsync();
        }

        [Test(Description = "https://github.com/MudBlazor/MudBlazor/issues/12833")]
        public async Task TreeView_NewItem_ShouldBeSelected()
        {
            var comp = Context.Render<TreeViewNewItemSelectTest>();
            comp.Instance.SelectedValue.Should().NotBeNull();
            comp.Instance.SelectedValue!.Name.Should().Be("2");
            await comp.Find("#add_item").ClickAsync();
            comp.Instance.SelectedValue.Should().NotBeNull();
            comp.Instance.SelectedValue!.Name.Should().Be("4");
        }

        [Test(Description = "https://github.com/MudBlazor/MudBlazor/issues/12849")]
        public async Task TreeView_ServerData_Reset()
        {
            var comp = Context.Render<TreeViewServerDataResetTest>();
            var arrows = () => comp.FindAll("button.mud-treeview-item-expand-button");
            var itemContents = () => comp.FindAll("div.mud-treeview-item-content").Select(x => x.TextContent);

            arrows().Count.Should().Be(4);
            await arrows()[1].ClickAsync();
            comp.WaitForAssertion(() => itemContents().Should().Contain("More Spam (1)"));
            await comp.Find("#btn_reset").ClickAsync();
            comp.WaitForAssertion(() =>
            {
                arrows().Count.Should().Be(4);
                itemContents().Should().NotContain("More Spam (1)");
            });

            await arrows()[1].ClickAsync();
            comp.WaitForAssertion(() => itemContents().Should().Contain("More Spam (6)"));
        }

        /// <summary>
        /// A standard tree must keep a bound selected value whose item has not been loaded yet when another branch loads.
        /// </summary>
        /// <remarks>
        /// Reconciling selection from the backing <see cref="ITreeItemData{T}.Selected"/> flags is a virtualized concern.
        /// A standard tree keeps its selection in the item components, so a server load must not prune the pending value.
        /// </remarks>
        [Test]
        public async Task TreeView_ServerData_PendingSelectedValueSurvivesUnrelatedLoad()
        {
            var comp = Context.Render<TreeViewServerPendingSelectionTest>();
            comp.Instance.SelectedValue.Should().Be("B1");

            await FindTreeItem(comp, "A").QuerySelector("div.mud-treeview-item-arrow button")!.ClickAsync();
            comp.WaitForAssertion(() => comp.FindAll("li.mud-treeview-item").Should().Contain(x => x.TextContent.Contains("A1", StringComparison.Ordinal)));

            comp.Render();

            comp.Instance.SelectedValueChangedCount.Should().Be(0, "no user selection happened");
            comp.Instance.SelectedValue.Should().Be("B1");
        }

        /// <summary>
        /// Verifies that a server load completing in a standard tree re-renders only the loading item, not unrelated items from the root.
        /// </summary>
        [Test]
        [CancelAfter(5000)]
        public async Task TreeView_ServerData_LoadCompletionDoesNotRenderUnrelatedItems()
        {
            var comp = Context.Render<TreeViewVirtualizationServerTest>(parameters => parameters
                .Add(x => x.Virtualize, false)
                .Add(x => x.UseTextFallback, true)
                .Add(x => x.RootCount, 2)
                .Add(x => x.DelayServerData, true));
            var unrelatedItem = comp.FindComponents<MudTreeViewItem<string>>().Single(item => item.Instance.CurrentItemData?.Text == "Node B");
            var loadTask = FindTreeItem(comp, "Node A").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();
            await comp.Instance.ServerDataStarted.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);
            comp.WaitForAssertion(() => FindTreeItem(comp, "Node A").QuerySelector("button.mud-treeview-item-expand-button")!.ClassList.Should().Contain("mud-treeview-item-arrow-load"));
            var renders = unrelatedItem.RenderCount;

            await comp.InvokeAsync(comp.Instance.CompleteServerData);
            await loadTask.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            comp.WaitForAssertion(() => FindTreeItem(comp, "Loaded 1").Should().NotBeNull());
            unrelatedItem.RenderCount.Should().Be(renders, "only the loading item changed");
        }

        /// <summary>
        /// A virtualized tree must keep a bound selected value whose item has not been loaded yet, and select the item once it loads.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_ServerData_PendingSelectedValueSurvivesUntilLoaded()
        {
            var comp = Context.Render<TreeViewServerPendingSelectionTest>(parameters => parameters
                .Add(x => x.Virtualize, true)
                .Add(x => x.Height, "200px"));
            comp.Instance.SelectedValue.Should().Be("B1");

            await FindTreeItem(comp, "A").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();
            comp.WaitForAssertion(() => FindTreeItem(comp, "A1").Should().NotBeNull());

            comp.Instance.SelectedValue.Should().Be("B1", "an unrelated load must not prune the pending value");

            await FindTreeItem(comp, "B").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();

            comp.WaitForAssertion(() => FindTreeItem(comp, "B1").QuerySelector("div.mud-treeview-item-content")!.ClassList.Should().Contain("mud-treeview-item-selected"));
            comp.Instance.SelectedValue.Should().Be("B1");
            comp.Instance.SelectedValueChangedCount.Should().Be(0, "no user selection happened");
        }

        /// <summary>
        /// A virtualized tree must keep the selection of a loaded child while its parent reloads, and re-select the child once it is loaded again.
        /// </summary>
        [Test]
        public async Task TreeViewVirtualize_ServerData_ReloadKeepsSelectedChild()
        {
            var comp = Context.Render<TreeViewServerPendingSelectionTest>(parameters => parameters
                .Add(x => x.Virtualize, true)
                .Add(x => x.Height, "200px")
                .Add(x => x.SelectedValue, (string)null!));
            await FindTreeItem(comp, "A").QuerySelector("button.mud-treeview-item-expand-button")!.ClickAsync();
            comp.WaitForAssertion(() => FindTreeItem(comp, "A1").Should().NotBeNull());
            await FindTreeItem(comp, "A1").QuerySelector("div.mud-treeview-item-content")!.ClickAsync();
            comp.Instance.SelectedValue.Should().Be("A1");
            var parentItem = comp.FindComponents<MudTreeViewItem<string>>().Single(item => item.Instance.CurrentItemData?.Value == "A").Instance;

            await comp.InvokeAsync(parentItem.ReloadAsync);

            comp.Instance.SelectedValue.Should().Be("A1", "the reload must not prune the selected child");
            comp.WaitForAssertion(() => FindTreeItem(comp, "A1").QuerySelector("div.mud-treeview-item-content")!.ClassList.Should().Contain("mud-treeview-item-selected"));
        }

        /// <summary>
        /// A virtualized tree with <see cref="MudTreeView{T}.AutoExpand"/> must keep a branch collapsed after the user collapses it.
        /// </summary>
        /// <remarks>
        /// Auto-expansion follows selection transitions. A parent re-render alone must not expand the ancestors of the selected item again.
        /// </remarks>
        [Test]
        public async Task TreeViewVirtualize_AutoExpand_UserCollapseSurvivesParentRender()
        {
            var comp = Context.Render<TreeViewVirtualizationTest>(parameters => parameters
                .Add(x => x.AutoExpand, true)
                .Add(x => x.SelectedValue, "Grandchild"));

            var childB = FindDataItem(comp.Instance.TreeItems, "Child B");
            comp.WaitForAssertion(() => childB.Expanded.Should().BeTrue());

            await FindTreeItem(comp, "Child B").QuerySelector("div.mud-treeview-item-arrow button")!.ClickAsync();

            comp.WaitForAssertion(() => childB.Expanded.Should().BeFalse());

            await comp.InvokeAsync(comp.Instance.Rerender);

            childB.Expanded.Should().BeFalse("the user collapsed this branch and the selection did not change");
            GetVirtualizedValues(comp).Should().NotContain("Grandchild");
        }

        /// <summary>
        /// A tree rendered inside a virtualized row must not inherit the row's context.
        /// </summary>
        /// <remarks>
        /// Without the reset, the inner items would treat themselves as virtualized rows of the outer tree, skipping their children and reporting the outer row's hierarchy.
        /// </remarks>
        [Test]
        public void TreeViewVirtualize_NestedTreeInRow_DoesNotInheritRowContext()
        {
            var comp = Context.Render<TreeViewVirtualizedNestedTest>();

            var innerItems = comp.FindAll("ul.nested-tree li.mud-treeview-item");
            innerItems.Should().HaveCountGreaterThan(0);
            innerItems.Should().OnlyContain(item => !item.ClassList.Contains("mud-treeview-item-virtualized"));
            innerItems.Should().OnlyContain(item => !item.HasAttribute("aria-level"));
            innerItems.Should().Contain(item => item.TextContent.Contains("Root inner child", StringComparison.Ordinal), "the inner item renders its own children");
        }

        /// <summary>
        /// A virtualized tree nested inside another tree shares the outer root's state, so it must fall back to standard rendering.
        /// </summary>
        /// <remarks>
        /// Nested trees render their groups through the root tree's template, so the inner parent is deliberately not expanded here.
        /// </remarks>
        [Test]
        public void TreeViewVirtualize_NestedInsideAnotherTree_FallsBackToStandardRendering()
        {
            var provider = new MockLoggerProvider();
            var logger = provider.CreateLogger(GetType().FullName) as MockLogger;
            Context.Services.AddLogging(builder => builder.ClearProviders().AddProvider(provider));
            var comp = Context.Render<TreeViewNestedVirtualizeTest>();

            var innerItems = comp.FindAll("ul.nested-tree li.mud-treeview-item");
            innerItems.Should().HaveCount(2);
            innerItems.Should().OnlyContain(item => !item.ClassList.Contains("mud-treeview-item-virtualized"));
            comp.FindComponents<Virtualize<TreeViewItemContext<string>>>().Should().BeEmpty();
            logger!.GetEntries().Should().ContainSingle(entry => entry.Level == LogLevel.Warning && entry.Message.Contains("nested inside another tree", StringComparison.Ordinal));
        }

        /// <summary>
        /// Verifies that turning on the root's <see cref="MudTreeView{T}.Disabled"/> at runtime reaches items at every depth.
        /// </summary>
        [Test]
        public async Task TreeView_RuntimeDisabled_ShouldReachEveryDepth()
        {
            var comp = Context.Render<TreeViewRuntimeRootParametersTest>();
            comp.Find(".lvl3.mud-treeview-item").ClassList.Should().NotContain("mud-treeview-item-disabled");

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));

            comp.Find(".lvl1.mud-treeview-item").ClassList.Should().Contain("mud-treeview-item-disabled");
            comp.Find(".lvl2.mud-treeview-item").ClassList.Should().Contain("mud-treeview-item-disabled");
            comp.Find(".lvl3.mud-treeview-item").ClassList.Should().Contain("mud-treeview-item-disabled");
            foreach (var input in comp.FindAll(".mud-treeview-item-checkbox input"))
            {
                input.HasAttribute("disabled").Should().BeTrue();
            }
        }

        /// <summary>
        /// Verifies that turning on the root's <see cref="MudTreeView{T}.ReadOnly"/> at runtime reaches items at every depth.
        /// </summary>
        [Test]
        public async Task TreeView_RuntimeReadOnly_ShouldReachEveryDepth()
        {
            var comp = Context.Render<TreeViewRuntimeRootParametersTest>();
            comp.Find(".lvl3 .mud-treeview-item-content").ClassList.Should().Contain("cursor-pointer");

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ReadOnly, true));

            comp.Find(".lvl1 .mud-treeview-item-content").ClassList.Should().NotContain("cursor-pointer");
            comp.Find(".lvl2 .mud-treeview-item-content").ClassList.Should().NotContain("cursor-pointer");
            comp.Find(".lvl3 .mud-treeview-item-content").ClassList.Should().NotContain("cursor-pointer");
        }

        /// <summary>
        /// Verifies that switching the root's <see cref="MudTreeView{T}.SelectionMode"/> at runtime gives every item a checkbox.
        /// </summary>
        [Test]
        public async Task TreeView_RuntimeSelectionMode_ShouldReachEveryDepth()
        {
            var comp = Context.Render<TreeViewRuntimeRootParametersTest>(parameters => parameters
                .Add(x => x.SelectionMode, SelectionMode.SingleSelection));
            comp.FindAll(".mud-treeview-item-checkbox").Should().BeEmpty();

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection));

            comp.FindAll(".mud-treeview-item-checkbox").Count.Should().Be(3);
        }
        /// <summary>
        /// Mounting a tree must render each item once, not twice.
        /// </summary>
        /// <remarks>
        /// Every top-level item asks the tree to refresh its selection state while it initialises, and the tree used to render unconditionally in response.
        /// Nothing is selected in this tree, so the second pass produced exactly the markup the first one already had.
        /// </remarks>
        [Test]
        public void TreeView_OnMount_RendersEachItemOnce()
        {
            var renders = 0;
            Context.Render<MudTreeView<string>>(parameters => parameters
                .Add(x => x.ChildContent, BuildItems(_ => renders++)));

            renders.Should().Be(3);
        }

        /// <summary>
        /// Mounting a multi-selection tree must also render each item once.
        /// </summary>
        /// <remarks>
        /// The tri-state checkbox is derived from the sub-items, so a multi-selection item can go stale without its own state changing.
        /// It is enough to render when that derived value stops matching what was rendered, which nothing selected never does.
        /// </remarks>
        [Test]
        public void TreeView_MultiSelection_OnMount_RendersEachItemOnce()
        {
            var renders = 0;
            Context.Render<MudTreeView<string>>(parameters => parameters
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Add(x => x.ChildContent, BuildItems(_ => renders++)));

            renders.Should().Be(3);
        }

        /// <summary>
        /// Selecting an item must re-render only that item, not every sibling.
        /// </summary>
        /// <remarks>
        /// The tree refreshes the selection state of every item whenever the selection changes.
        /// A sibling whose own state did not change has nothing new to show, and this guards the click path that the mount tests above do not reach.
        /// </remarks>
        [Test]
        public void TreeView_SelectOneSibling_RendersOnlyChangedItem()
        {
            var renders = new int[3];
            var comp = Context.Render<MudTreeView<string>>(parameters => parameters
                .Add(x => x.ChildContent, BuildItems(index => renders[index]++)));

            var afterMount = (int[])renders.Clone();
            comp.FindAll(".mud-treeview-item-content")[0].Click();

            renders[0].Should().Be(afterMount[0] + 1, "the selected item has a new state to show");
            renders[1].Should().Be(afterMount[1], "sibling 1 did not change and should not have re-rendered");
            renders[2].Should().Be(afterMount[2], "sibling 2 did not change and should not have re-rendered");
        }

        /// <summary>
        /// Builds three sibling items whose body content reports every time it is rendered.
        /// </summary>
        /// <param name="onRender">Called with the item's index once per render of that item.</param>
        private static RenderFragment BuildItems(Action<int> onRender) => builder =>
        {
            for (var i = 0; i < 3; i++)
            {
                var index = i;
                builder.OpenComponent<MudTreeViewItem<string>>(index * 3);
                builder.AddComponentParameter((index * 3) + 1, nameof(MudTreeViewItem<string>.Value), $"item{index}");
                builder.AddComponentParameter((index * 3) + 2, nameof(MudTreeViewItem<string>.BodyContent), (RenderFragment<MudTreeViewItem<string>>)(item => content =>
                {
                    onRender(index);
                    content.AddContent(0, item.Value);
                }));
                builder.CloseComponent();
            }
        };

        /// <summary>
        /// Finds the rendered tree item whose own content contains the specified text.
        /// </summary>
        private static IElement FindTreeItem<TComponent>(IRenderedComponent<TComponent> component, string text)
            where TComponent : IComponent
        {
            return component.FindAll("li.mud-treeview-item")
                .Single(item => item.FirstElementChild?.TextContent.Contains(text, StringComparison.Ordinal) == true);
        }

        /// <summary>
        /// Simulates the browser reporting the virtualizer's scroll position as the space above the viewport and the viewport height, in pixels.
        /// </summary>
        private static Task ScrollVirtualizerAsync(IRenderedComponent<IComponent> component, float spacerSize, float containerSize)
        {
            var virtualize = component.FindComponent<Virtualize<TreeViewItemContext<string>>>().Instance;
            var method = virtualize.GetType()
                .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Single(candidate => candidate.Name.EndsWith("OnBeforeSpacerVisible", StringComparison.Ordinal));

            return component.InvokeAsync(() => method.Invoke(virtualize, [spacerSize, spacerSize, containerSize]));
        }

        /// <summary>
        /// Gets the values from the flattened virtualized item contexts.
        /// </summary>
        private static IReadOnlyList<string> GetVirtualizedValues(IRenderedComponent<IComponent> component)
        {
            return GetTreeViewItemContexts(component).Select(itemContext => itemContext.Item.Value ?? string.Empty).ToList();
        }

        /// <summary>
        /// Gets the flattened item contexts rendered by <see cref="MudVirtualize{T}"/>.
        /// </summary>
        private static ICollection<TreeViewItemContext<string>> GetTreeViewItemContexts(IRenderedComponent<IComponent> component)
        {
            return component.FindComponent<Virtualize<TreeViewItemContext<string>>>()
                .Instance
                .Items!;
        }

        /// <summary>
        /// Finds an item in a hierarchical data tree by value.
        /// </summary>
        private static ITreeItemData<string> FindDataItem(IEnumerable<ITreeItemData<string>> items, string value)
        {
            return GetDataItems(items).Single(item => item.Value == value);
        }

        /// <summary>
        /// Enumerates every item in a hierarchical data tree.
        /// </summary>
        private static IEnumerable<ITreeItemData<string>> GetDataItems(IEnumerable<ITreeItemData<string>> items)
        {
            foreach (var item in items)
            {
                yield return item;

                if (!item.HasChildren)
                {
                    continue;
                }

                foreach (var child in GetDataItems(item.Children))
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// Counts reads of <see cref="Visible"/> so a test can assert how many times the tree projects the hierarchy.
        /// </summary>
        private sealed class ProjectionTrackingTreeItemData(string value) : ITreeItemData<string>
        {
            private bool _visible = true;

            public string Text { get; set; }

            public string Icon { get; set; }

            public string Value { get; } = value;

            public bool Expanded { get; set; }

            public bool Expandable { get; set; } = true;

            public bool Selected { get; set; }

            public bool Visible
            {
                get
                {
                    VisibleReadCount++;
                    return _visible;
                }
                set => _visible = value;
            }

            public IReadOnlyCollection<ITreeItemData<string>> Children { get; set; }

            public bool HasChildren => Children is { Count: > 0 };

            public int VisibleReadCount { get; private set; }

            public void ResetProjectionReadCount() => VisibleReadCount = 0;
        }

    }
}
