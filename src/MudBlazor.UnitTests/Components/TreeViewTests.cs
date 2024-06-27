using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.TreeView;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class TreeViewTests : BunitTest
    {
        [Test]
        public void TreeView_ClickWhileDisabled_DoesNotChangeSelection()
        {
            var comp = Context.RenderComponent<DisabledTreeViewTest>([Parameter(nameof(MudTreeView<string>.Disabled), true)]);
            comp.Find("div.mud-treeview-item-content").Click();
            var GetSelectedValue = () => comp.Find("p.selected-value").TrimmedText();
            GetSelectedValue().Should().BeNullOrWhiteSpace();

            comp.Find("div.mud-treeview-item-content").DoubleClick();
            GetSelectedValue().Should().BeNullOrWhiteSpace();
        }

        [Test]
        public void TreeView_ClickWhileActive_DoesChangeSelection()
        {
            var comp = Context.RenderComponent<DisabledTreeViewTest>(self => self.Add(x => x.Disabled, false));
            comp.Find("div.mud-treeview-item-content").Click();
            var GetSelectedValue = () => comp.Find("p.selected-value").TrimmedText();
            GetSelectedValue().Should().NotBeNullOrWhiteSpace();
            GetSelectedValue().Should().Be("content");

            // To reset
            comp.Find("div.mud-treeview-item-content").Click();
            GetSelectedValue().Should().BeNullOrWhiteSpace();

            comp.Find("div.mud-treeview-item-content").DoubleClick();
            GetSelectedValue().Should().NotBeNull();
        }

        [Test]
        [TestCase("item1")]
        [TestCase("item1.1")]
        [TestCase("item1.2")]
        public void TreeViewWithSingleSelection_Should_RespectInitialSelectedValue(string value)
        {
            var comp = Context.RenderComponent<SimpleTreeViewTest>(self => self.Add(x => x.SelectedValue, value));
            comp.Find("div.mud-treeview-item-selected").QuerySelector(".mud-treeview-item-label").TrimmedText().Should().Be(value);
        }

        [Test]
        public void TreeViewWith_SingleSelection_TwoWayBindingTest()
        {
            var comp = Context.RenderComponent<TreeViewSelectionBindingTest>(self => self.Add(x => x.SelectedValue, "item1.2"));
            // check initial selection
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");

            // select another value on tree1 and check selection has changed on both trees
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").Click();
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1");
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");

            // select another value on tree2 and check selection has changed on both trees
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").Click();
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1.1");
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");

            // in single selection clicking the same item twice won't de-select it!
            // select same value on tree1 and check selection has NOT changed
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").Click();
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1.1");
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
        }

        [Test]
        public void TreeViewWith_ToggleSelection_TwoWayBindingTest()
        {
            var comp = Context.RenderComponent<TreeViewSelectionBindingTest>(self => self
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
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").Click();
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1");
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");

            // select another value on tree2 and check selection has changed on both trees
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").Click();
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1.1");
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().Contain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");

            // in toggle selection clicking the same item twice will de-select it!
            // select same value on tree1 and check selection has been removed
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").Click();
            comp.Find("p.selected-value").TrimmedText().Should().BeNullOrWhiteSpace();
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-1 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree1 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
            comp.Find(".tree2 .item-1-2 .mud-treeview-item-content").ClassList.Should().NotContain("mud-treeview-item-selected");
        }

        [Test]
        public void TreeViewWith_MultiSelection_TwoWayBindingTest()
        {
            var comp = Context.RenderComponent<TreeViewSelectionBindingTest>(self => self
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
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").Click();
            comp.Find(".tree1 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree2 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree1 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree2 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree1 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find(".tree2 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find("p.selected-values").TrimmedText().Should().Be("item1, item1.1, item1.2");

            // remove a value on tree2 and check selection has changed on both trees
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").Click();
            comp.Find(".tree1 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree2 .item-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree1 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree2 .item-1-1 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree1 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find(".tree2 .item-1-2 .mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find("p.selected-values").TrimmedText().Should().Be("");
        }

        [Test]
        public void TreeViewItemSelected_ShouldBeInitializedCorrectly_SingleSelection()
        {
            var comp = Context.RenderComponent<TreeViewItemSelectedBindingTest>(self => self.Add(x => x.SelectedValue, "item1.2"));
            comp.Find("p.selected-value").TrimmedText().Should().Be("item1.2");
            comp.Find("p.item1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-2-selected").TrimmedText().Should().Be("True");
        }

        [Test]
        public void InitialValueOfTreeViewItemSelected_Should_InfluenceSelectedValue_SingleSelection()
        {
            var comp = Context.RenderComponent<TreeViewItemSelectedBindingTest>(self => self
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
            var comp = Context.RenderComponent<TreeViewItemSelectedBindingTest>(self => self
                .Add(x => x.SelectedValues, ["item1", "item1.2"])
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            comp.Find("p.selected-values").TrimmedText().Should().Be("item1, item1.2");
            comp.Find("p.item1-selected").TrimmedText().Should().Be("True");
            comp.Find("p.item1-1-selected").TrimmedText().Should().Be("False");
            comp.Find("p.item1-2-selected").TrimmedText().Should().Be("True");
        }

        [Test]
        public void InitialValueOfTreeViewItemSelected_Should_InfluenceSelectedValue_MultiSelection()
        {
            var comp = Context.RenderComponent<TreeViewItemSelectedBindingTest>(self => self
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
        public void TreeViewItem_Selected_TwoWayBindingTest_SingleSelection()
        {
            var comp = Context.RenderComponent<TreeViewItemSelectedBindingTest>(self => self.Add(x => x.SelectedValue, "item1.2"));
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
            comp.Find(".tree1 .item-1 .mud-treeview-item-content").Click();
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
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").Click();
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
        public void TreeViewItem_Selected_TwoWayBindingTest_MultiSelection()
        {
            var comp = Context.RenderComponent<TreeViewItemSelectedBindingTest>(self => self
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
            comp.Find(".tree1 .item-1-1 .mud-treeview-item-content").Click();
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
            comp.Find(".tree2 .item-1 .mud-treeview-item-content").Click();
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
        public void TreeView_WhenDisabled_DoesNotHaveRipple()
        {
            var comp = Context.RenderComponent<TreeViewRippleTest>(self => self.Add(x => x.Disabled, true));

            comp.FindAll("div.mud-ripple").Count.Should().Be(0);

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Disabled, false));
            comp.FindAll("div.mud-ripple").Count.Should().BeGreaterThan(0);
        }

        [Test]
        public void TreeView_WhenRippleDisabled_DoesNotHaveRipple()
        {
            var comp = Context.RenderComponent<TreeViewRippleTest>(self => self.Add(x => x.Ripple, false));

            comp.FindAll("div.mud-ripple").Count.Should().Be(0);

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Ripple, true));
            comp.FindAll("div.mud-ripple").Count.Should().BeGreaterThan(0);
        }

        [Test]
        public void Collapsed_ClickOnArrowButton_CheckClose()
        {
            var comp = Context.RenderComponent<TreeViewTest1>();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(10);
            comp.Find("button").Click();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(1);
            comp.Find("button").Click();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(0);
            comp.Find("div.mud-treeview-item-content").Click();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(0);
        }

        [Test]
        public void Collapsed_ClickOnTreeItem_CheckClose()
        {
            var comp = Context.RenderComponent<TreeViewTest2>();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(10);
            comp.Find("button").Click();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(1);
            comp.Find("button").Click();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(0);
            comp.Find("div.mud-treeview-item-content").Click();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(1);
            comp.Find("div.mud-treeview-item-content").Click();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(0);
        }

        [Test]
        public void Unselected_Select_CheckSelected_Deselect_CheckDeselected()
        {
            var comp = Context.RenderComponent<TreeViewTest1>(self => self.Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(10);
            comp.Find("button").Click();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(1);
            comp.FindAll("input.mud-checkbox-input").Count.Should().Be(10);
            comp.Find("input.mud-checkbox-input").Change(true);
            comp.Instance.SubItemSelected.Should().BeTrue();
            comp.Instance.Item1Selected.Should().BeTrue();
            comp.FindAll("input.mud-checkbox-input")[2].Change(false);
            comp.Instance.SubItemSelected.Should().BeFalse();
            comp.Instance.Item1Selected.Should().BeFalse(); // <-- selecting child updates parent in multi-selection mode
        }

        [Test]
        public void Normal_Activate_CheckActivated_ActivateAnother_CheckBoth()
        {
            var comp = Context.RenderComponent<TreeViewTest1>(self => self.Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            comp.FindAll(".mud-checkbox-true").Count.Should().Be(0);
            comp.Find("div.mud-treeview-item-content").Click();
            comp.Instance.Item1Selected.Should().BeTrue();
            comp.Instance.Item2Selected.Should().BeFalse();
            comp.FindAll(".mud-checkbox-true").Count.Should().Be(4); // item1 + entire sub-tree checked
            comp.FindAll("div.mud-treeview-item-content")[4].Click();
            comp.Instance.Item1Selected.Should().BeTrue();
            comp.Instance.Item2Selected.Should().BeTrue();
            comp.FindAll(".mud-checkbox-true").Count.Should().Be(10);  // + item2 + entire sub-tree checked
        }

        [Test]
        public void TreeView_WillUnselectItems_WhenNotMultiSelect()
        {
            var comp = Context.RenderComponent<TreeViewTest7>();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(0);
            comp.Find("div.mud-treeview-item-content").Click();
            comp.Instance.Item1Selected.Should().BeTrue();
            comp.Instance.Item2Selected.Should().BeFalse();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(1);
            comp.FindAll("div.mud-treeview-item-content")[4].Click();
            comp.Instance.Item1Selected.Should().BeFalse();
            comp.Instance.Item2Selected.Should().BeTrue();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(1);
        }

        [Test]
        public void Normal_Activate_CheckActivated_Deactivate_Check()
        {
            var comp = Context.RenderComponent<TreeViewTest1>(self => self.Add(x => x.SelectionMode, SelectionMode.ToggleSelection));
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(0);
            comp.Find("div.mud-treeview-item-content").Click();
            comp.Instance.Item1Selected.Should().BeTrue();
            comp.Instance.Item2Selected.Should().BeFalse();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(1);
            comp.Find("div.mud-treeview-item-content").Click();
            comp.Instance.Item1Selected.Should().BeFalse();
            comp.Instance.Item2Selected.Should().BeFalse();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(0);
        }

        [Test]
        public void RenderWithTemplate_CheckResult()
        {
            var comp = Context.RenderComponent<TreeViewTemplateTest>();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(8);
        }

        [Test]
        public void TreeViewServerTest()
        {
            var comp = Context.RenderComponent<TreeViewServerTest>();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(4);
            comp.FindAll("div.mud-treeview-item-content")[0].Click();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(4);
            comp.FindAll("div.mud-treeview-item-content")[3]
                .GetElementsByClassName("mud-treeview-item-arrow")[0]
                .ChildElementCount.Should().Be(0);
            comp.FindAll("div.mud-treeview-item-content")[2].Click();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(8);
        }

        [Test]
        public void TreeViewTreeItemDataTest()
        {
            // test default values
            new TreeItemData<int>().Expanded.Should().Be(false);
            new TreeItemData<int>().Selected.Should().Be(false);
            new TreeItemData<int>().Expandable.Should().Be(true);
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
                Selected = true,
                Children = [new TreeItemData<string>()]
            };
            data.Value.Should().Be("val");
            data.Icon.Should().Be("i");
            data.Text.Should().Be("t");
            data.Expandable.Should().Be(false);
            data.Expanded.Should().Be(true);
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
        public void TreeViewItem_DoubleClick_CheckExpanded()
        {
            var comp = Context.RenderComponent<TreeViewTest3>();
            var itemExpanded = false;

            var item = comp.FindComponent<MudTreeViewItem<string>>();
            item.SetParametersAndRender(Parameter(nameof(MudTreeViewItem<string>.OnDoubleClick), new EventCallback<MouseEventArgs>(null, (Action)(() => itemExpanded = !itemExpanded))));

            comp.FindAll("li.mud-treeview-item").Count.Should().Be(10);

            comp.Find("div.mud-treeview-item-content").DoubleClick();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(1);
            itemExpanded.Should().BeTrue();

            comp.Find("div.mud-treeview-item-content").DoubleClick();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(0);
            itemExpanded.Should().BeFalse();
        }

        [Test]
        public void TreeViewItem_DoubleClick_CheckSelected()
        {
            var comp = Context.RenderComponent<TreeViewTest3>();
            string selectedItem = null;

            var tree = comp.FindComponent<MudTreeView<string>>();

            tree.SetParametersAndRender(Parameter(nameof(MudTreeView<string>.SelectedValueChanged), new EventCallback<string>(null, (Action<string>)((s) => selectedItem = s))));

            comp.Find("div.mud-treeview-item-content").DoubleClick();
            selectedItem.Should().Be("content");
        }

        [Test]
        public async Task TreeViewItem_ProgrammaticallySelect()
        {
            var comp = Context.RenderComponent<TreeViewTest4>();
            var treeView = comp.FindComponent<MudTreeView<string>>();

            await comp.InvokeAsync(() => comp.Instance.ClickFirst());
            comp.WaitForAssertion(() => comp.Instance.selectedValue.Should().Be("content"));

            await comp.InvokeAsync(() => comp.Instance.ClickSecond());
            comp.WaitForAssertion(() => comp.Instance.selectedValue.Should().Be("src"));

            await comp.InvokeAsync(() => comp.Instance.ClickSecond());
            comp.WaitForAssertion(() => comp.Instance.selectedValue.Should().Be(null));
        }


        [Test]
        public async Task TreeViewItem_BodyContent()
        {
            var comp = Context.RenderComponent<TreeViewTest5>();
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
            treeView.SetParam("Items", await comp.Instance.LoadServerData(null));
            comp.FindAll("p.mud-typography")[1].InnerHtml.MarkupMatches("This is item 2");

            // Test reloading the treeview item.
            comp.FindAll("button.mud-treeview-item-arrow-expand")[2].Click();
            comp.FindAll("p.mud-typography")[3].InnerHtml.MarkupMatches("This is item four");
            comp.FindAll("p.mud-typography")[4].InnerHtml.MarkupMatches("This is item five");

            comp.Instance.SimulateUpdate3 = true;
            await treeViewItem.InvokeAsync(treeViewItem.Instance.ReloadAsync);
            comp.FindAll("p.mud-typography")[3].InnerHtml.MarkupMatches("This is item 4");
            comp.FindAll("p.mud-typography")[4].InnerHtml.MarkupMatches("This is item 5");
        }

        [Test]
        public void TreeView_SetSelectedValue_SetsSelectedValue()
        {
            var comp = Context.RenderComponent<TreeViewTest6>();

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.SelectedValue, "logo.png"));

            comp.Instance.SelectedValue.Should().Be("logo.png");
        }

        [Test]
        public void TreeView_SetSelectedValue_IsSetNullWhenNotFound()
        {
            var comp = Context.RenderComponent<TreeViewTest6>();

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.SelectedValue, "logo.png"));

            comp.Instance.SelectedValue.Should().Be("logo.png");

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.SelectedValue, "xxxxxx"));

            comp.Instance.SelectedValue.Should().Be(default);
        }

        [Test]
        public void TreeView_SetSelectedValue_IsSetNullWhenInitialValueIsInvalid()
        {
            var comp = Context.RenderComponent<TreeViewTest6>();

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.SelectedValue, "xxxxxx"));

            comp.Instance.SelectedValue.Should().Be(default);
        }

        [Test]
        public void TreeView_SelectedValue_ShouldUseComparer()
        {
            // test tree with items ("Ax", "Bx", "Cx", "Dx")
            var comp = Context.RenderComponent<TreeViewCompareTest>();
            string GetSelectedValue() => comp.Find("p.selected-value").TrimmedText();

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.SelectedValue, "Ax"));
            GetSelectedValue().Should().Be("Ax");

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.SelectedValue, "Bx"));
            GetSelectedValue().Should().Be("Bx");

            // setting A will not select anything because it isn't a valid value with the default comparer
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.SelectedValue, "A"));
            GetSelectedValue().Should().BeNullOrWhiteSpace();

            // set the comparer to a value that will only check the first character of the string
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Comparer,
                new DelegateEqualityComparer<string>(
                    (x, y) =>
                    {
                        if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
                            return true;
                        if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
                            return false;
                        return x[0] == y[0];
                    },
                    obj =>
                    {
                        if (string.IsNullOrEmpty(obj))
                            return 0;
                        return obj[0].GetHashCode();
                    }
                )
            ));
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.SelectedValue, "A"));
            GetSelectedValue().Should().StartWith("A");
        }

        /// <summary>
        /// This test checks that when multiple values are selected and the compare parameter is updated,
        /// selected values are updated correctly. 
        /// </summary>
        [Test]
        public void TreeView_SelectedValues_ShouldUseComparer()
        {
            // tree containing two children with values AA and AC
            var comp = Context.RenderComponent<TreeViewComparerMultiSelectTest>(self => self.Add(x => x.SelectedValues, ["AA"]));

            comp.Instance.Item1Selected.Should().BeTrue();
            comp.Instance.Item2Selected.Should().BeFalse();

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Comparer,
                new DelegateEqualityComparer<string>(
                    (x, y) =>
                    {
                        if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
                            return true;
                        if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
                            return false;
                        return x[0] == y[0];
                    },
                    obj =>
                    {
                        if (string.IsNullOrEmpty(obj))
                            return 0;
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
        public void TreeViewItem_TwoWayBindingTest()
        {
            var comp = Context.RenderComponent<TreeViewItemBindingTest>();
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
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-config").Value.Should().Be(false);
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-images").Value.Should().Be(false);
            // checkboxes
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-launch").Value.Should().Be(true);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-tasks").Value.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-logo").Value.Should().Be(false);

            // select logo.png via tree item
            comp.Find(".item-logo .mud-treeview-item-content").Click();
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
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-config").Value.Should().Be(false);
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-images").Value.Should().Be(false);
            // checkboxes
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-launch").Value.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-tasks").Value.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-logo").Value.Should().Be(true);

            // expand config via tree item
            comp.Find(".item-config .mud-treeview-item-arrow button").Click();
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
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-config").Value.Should().Be(true);
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-images").Value.Should().Be(false);
            // checkboxes
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-launch").Value.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-tasks").Value.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-logo").Value.Should().Be(true);

            // collapse config via switch
            comp.Find(".switch-config input").Change(false);
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
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-config").Value.Should().Be(false);
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-images").Value.Should().Be(false);
            // checkboxes
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-launch").Value.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-tasks").Value.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-logo").Value.Should().Be(true);

            // select launch.json via checkbox
            comp.Find(".checkbox-launch input").Change(true);
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
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-config").Value.Should().Be(false);
            comp.FindComponents<MudSwitch<bool>>().Select(x => x.Instance).First(x => x.Class == "switch-images").Value.Should().Be(false);
            // checkboxes
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-launch").Value.Should().Be(true);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-tasks").Value.Should().Be(false);
            comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).First(x => x.Class == "checkbox-logo").Value.Should().Be(false);
        }

        [Test]
        public void TreeViewAutoExpansionTest()
        {
            var comp = Context.RenderComponent<TreeViewAutoExpandTest>(self => self.Add(x => x.AutoExpand, true));
            var isExpanded = (string value) => comp.FindComponents<MudTreeViewItem<string>>()
                .FirstOrDefault(x => x.Instance.Value == value)?.Instance.GetState<bool>(nameof(MudTreeViewItem<string>.Expanded));
            var select = (string value) => comp.FindComponents<MudChip<string>>().FirstOrDefault(x => x.Instance.Text == value)?.Find("div").Click();
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            // select and check that along the path to the value all parents were expanded, nothing else
            select("tasks.json");
            isExpanded("C:").Should().Be(true);
            isExpanded("config").Should().Be(true);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            // reset all to collapsed and check
            comp.Find("button.collapse-all").Click();
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            // select and check that along the path to the value all parents were expanded, nothing else
            // here images itself must not be expanded, only its parent
            select("images");
            isExpanded("C:").Should().Be(true);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
        }

        [Test]
        public void TreeViewAutoExpansion_ShouldNot_ExpandNonExpandableItems()
        {
            var comp = Context.RenderComponent<TreeViewAutoExpandTest>(self => self.Add(x => x.AutoExpand, true).Add(x => x.ConfigCanExpand, false));
            var isExpanded = (string value) => comp.FindComponents<MudTreeViewItem<string>>()
                .FirstOrDefault(x => x.Instance.Value == value)?.Instance.GetState<bool>(nameof(MudTreeViewItem<string>.Expanded));
            var select = (string value) => comp.FindComponents<MudChip<string>>().FirstOrDefault(x => x.Instance.Text == value)?.Find("div").Click();
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            // select and check that along the path to the value all parents were expanded, nothing else
            select("tasks.json");
            isExpanded("C:").Should().Be(true);
            isExpanded("config").Should().Be(false); // <--- shouldn't be expanded because it can't
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            // reset all to collapsed and check
            comp.Find("button.collapse-all").Click();
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            // select and check that along the path to the value all parents were expanded, nothing else
            // here images itself must not be expanded, only its parent
            select("logo.png");
            isExpanded("C:").Should().Be(true);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(true);
            isExpanded("logo.png").Should().Be(false);
        }

        [Test]
        public void TreeViewExpandAllCollapseAllTest()
        {
            var comp = Context.RenderComponent<TreeViewAutoExpandTest>(self => self.Add(x => x.AutoExpand, false));
            var isExpanded = (string value) => comp.FindComponents<MudTreeViewItem<string>>()
                .FirstOrDefault(x => x.Instance.Value == value)?.Instance.GetState<bool>(nameof(MudTreeViewItem<string>.Expanded));
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            comp.Find("button.expand-all").Click();
            isExpanded("C:").Should().Be(true);
            isExpanded("config").Should().Be(true);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(true);
            isExpanded("logo.png").Should().Be(false);
            comp.Find("button.collapse-all").Click();
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
        }

        [Test]
        public void TreeViewExpandAll_ShouldNot_ExpandNonExpandableItems()
        {
            var comp = Context.RenderComponent<TreeViewAutoExpandTest>(self => self.Add(x => x.ConfigCanExpand, false));
            var isExpanded = (string value) => comp.FindComponents<MudTreeViewItem<string>>()
                .FirstOrDefault(x => x.Instance.Value == value)?.Instance.GetState<bool>(nameof(MudTreeViewItem<string>.Expanded));
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
            comp.Find("button.expand-all").Click();
            isExpanded("C:").Should().Be(true);
            isExpanded("config").Should().Be(false); // <--- shouldn't be expanded because it can't
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(true);
            isExpanded("logo.png").Should().Be(false);
            comp.Find("button.collapse-all").Click();
            isExpanded("C:").Should().Be(false);
            isExpanded("config").Should().Be(false);
            isExpanded("launch.json").Should().Be(false);
            isExpanded("tasks.json").Should().Be(false);
            isExpanded("images").Should().Be(false);
            isExpanded("logo.png").Should().Be(false);
        }

        [Test]
        public void TreeViewItem_SetParameters_ValueIsSetNull_WhenTextUnset_RootServerdataIsSet_Throw()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                var comp = Context.RenderComponent<TreeViewTest8>();
                comp.SetParametersAndRender();
                comp.FindAll("li.mud-treeview-item").Count.Should().Be(4);
            });

#nullable enable 
            MudTreeView<string>? nullInstanceTree = null;
            MudTreeViewItem<string>? nullInstanceItem = null;
#nullable disable

            exception.Message.Should().Be($"'{nameof(MudTreeView<string>)}.{nameof(nullInstanceTree.ServerData)}' requires '{nameof(nullInstanceTree.ItemTemplate)}.{nameof(MudTreeViewItem<string>)}.{nameof(nullInstanceItem.Value)}' to be supplied.");
        }
    }
}
