using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class TreeViewTest : BunitTest
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
            var comp = Context.RenderComponent<TreeViewTest1>();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(10);
            comp.Find("button").Click();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(1);
            comp.FindAll("input.mud-checkbox-input").Count.Should().Be(10);
            comp.Find("input.mud-checkbox-input").Change(true);
            comp.Instance.SubItemSelected.Should().BeTrue();
            comp.Instance.ParentItemSelected.Should().BeTrue();
            comp.FindAll("input.mud-checkbox-input")[2].Change(false);
            comp.Instance.SubItemSelected.Should().BeFalse();
            comp.Instance.ParentItemSelected.Should().BeTrue();
        }

        [Test]
        public void Normal_Activate_CheckActivated_ActivateAnother_CheckBoth()
        {
            var comp = Context.RenderComponent<TreeViewTest1>();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(0);
            comp.Find("div.mud-treeview-item-content").Click();
            comp.Instance.Item1Activated.Should().BeTrue();
            comp.Instance.Item2Activated.Should().BeFalse();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(1);
            comp.FindAll("div.mud-treeview-item-content")[4].Click();
            comp.Instance.Item1Activated.Should().BeTrue();
            comp.Instance.Item2Activated.Should().BeTrue();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(2);
        }

        [Test]
        public void TreeView_WillUnselectItems_WhenNotMultiSelect()
        {
            var comp = Context.RenderComponent<TreeViewTest7>();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(0);
            comp.Find("div.mud-treeview-item-content").Click();
            comp.Instance.Item1Activated.Should().BeTrue();
            comp.Instance.Item2Activated.Should().BeFalse();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(1);
            comp.FindAll("div.mud-treeview-item-content")[4].Click();
            comp.Instance.Item1Activated.Should().BeFalse();
            comp.Instance.Item2Activated.Should().BeTrue();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(1);
        }

        [Test]
        public void Normal_Activate_CheckActivated_Deactivate_Check()
        {
            var comp = Context.RenderComponent<TreeViewTest1>();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(0);
            comp.Find("div.mud-treeview-item-content").Click();
            comp.Instance.Item1Activated.Should().BeTrue();
            comp.Instance.Item2Activated.Should().BeFalse();
            comp.FindAll("div.mud-treeview-item-content.mud-treeview-item-selected").Count.Should().Be(1);
            comp.Find("div.mud-treeview-item-content").Click();
            comp.Instance.Item1Activated.Should().BeFalse();
            comp.Instance.Item2Activated.Should().BeFalse();
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
#pragma warning disable BL0005
        //        [Test]
        //        public async Task TreeView_OtherTest()
        //        {
        //            var comp = Context.RenderComponent<TreeViewTest2>();
        //            var item = comp.FindComponent<MudTreeViewItem<string>>();
        //            await comp.InvokeAsync(() => item.Instance.Disabled = true);
        //            comp.WaitForAssertion(() => item.Instance.Selected.Should().BeFalse());
        //            await comp.InvokeAsync(() => item.Instance.Selected = true);
        //            comp.WaitForAssertion(() => item.Instance.Selected.Should().BeTrue());
        //            await comp.InvokeAsync(() => item.Instance.SelectItem(true));
        //            comp.WaitForAssertion(() => item.Instance.Selected.Should().BeTrue());
        //            comp.WaitForAssertion(() => item.Instance.ArrowExpanded.Should().BeFalse());
        //            await comp.InvokeAsync(() => item.Instance.ArrowExpanded = true);
        //            comp.WaitForAssertion(() => item.Instance.ArrowExpanded.Should().BeTrue());
        //            await comp.InvokeAsync(() => item.Instance.ArrowExpanded = true);
        //            comp.WaitForAssertion(() => item.Instance.ArrowExpanded.Should().BeTrue());

        //            comp.WaitForAssertion(() => item.Instance.Expanded.Should().BeTrue());
        //            await comp.InvokeAsync(() => item.Instance.OnItemExpanded(true));
        //            comp.WaitForAssertion(() => item.Instance.Expanded.Should().BeTrue());

        //            await comp.InvokeAsync(() => item.Instance.Select(false));
        //        }

        [Test]
        public async Task TreeViewItem_DoubleClick_CheckExpanded()
        {
            var comp = Context.RenderComponent<TreeViewTest3>();
            var itemIsExpanded = false;

            var item = comp.FindComponent<MudTreeViewItem<string>>();
            await item.InvokeAsync(() =>
                item.Instance.OnDoubleClick =
                    new EventCallback<MouseEventArgs>(null, (Action)(() => itemIsExpanded = !itemIsExpanded)));

            comp.FindAll("li.mud-treeview-item").Count.Should().Be(10);

            comp.Find("div.mud-treeview-item-content").DoubleClick();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(1);
            itemIsExpanded.Should().BeTrue();

            comp.Find("div.mud-treeview-item-content").DoubleClick();
            comp.FindAll("li.mud-treeview-item .mud-collapse-container.mud-collapse-entering").Count.Should().Be(0);
            itemIsExpanded.Should().BeFalse();
        }

        [Test]
        public async Task TreeViewItem_DoubleClick_CheckSelected()
        {
            var comp = Context.RenderComponent<TreeViewTest3>();
            string selectedItem = null;

            var tree = comp.FindComponent<MudTreeView<string>>();

            await tree.InvokeAsync(() =>
                tree.Instance.SelectedValueChanged =
                    new EventCallback<string>(null, (Action<string>)((s) => selectedItem = s)));

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
            var treeView = comp.FindComponent<MudTreeView<TreeViewTest5.TreeItemData>>();
            var treeViewItem = comp.FindComponents<MudTreeViewItem<TreeViewTest5.TreeItemData>>()[2];

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
        public void TreeView_SelectedValue_ShouldUseCompareParameter()
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
        public async Task TreeView_SelectedValues_ShouldUseCompareParameter()
        {
            // tree containing two children with values AA and AB
            var comp = Context.RenderComponent<TreeViewCompareMultiSelectTest>();

            await comp.InvokeAsync(() => comp.Instance.Item1.SelectedChanged.InvokeAsync(true));
            await comp.InvokeAsync(() => comp.Instance.Item2.SelectedChanged.InvokeAsync(true));

            comp.Instance.Item1Selected.Should().BeTrue();
            comp.Instance.Item2Selected.Should().BeTrue();

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Compare,
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
            comp.Instance.Item2Selected.Should().BeFalse();
        }

        [Test]
        public void MudTreeViewItemComparer_ShouldReturnTrueWhenBothNull()
        {
            var comparer = new MudTreeViewItemComparer<string>(EqualityComparer<string>.Default);

            comparer.Equals(null, null).Should().BeTrue();
        }

        [Test]
        public void MudTreeViewItemComparer_ShouldReturnFalseWhenOneNull()
        {
            var comparer = new MudTreeViewItemComparer<string>(EqualityComparer<string>.Default);

            var treeItem = new MudTreeViewItem<string> { Value = "value" };

            comparer.Equals(treeItem, null).Should().BeFalse();
            comparer.Equals(null, treeItem).Should().BeFalse();
        }
    }
}
