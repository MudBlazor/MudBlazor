
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
            var comp = Context.RenderComponent<DisabledTreeViewTest>(new ComponentParameter[] { Parameter(nameof(MudTreeView<string>.Disabled), true) });
            comp.Find("div.mud-treeview-item-content").Click();
            comp.Instance.SelectedValue.Should().BeNull();

            comp.Find("div.mud-treeview-item-content").DoubleClick();
            comp.Instance.SelectedValue.Should().BeNull();
        }

        [Test]
        public void TreeView_ClickWhileActive_DoesChangeSelection()
        {
            var comp = Context.RenderComponent<DisabledTreeViewTest>(new ComponentParameter[] { Parameter(nameof(MudTreeView<string>.Disabled), false) });
            comp.Find("div.mud-treeview-item-content").Click();
            comp.Instance.SelectedValue.Should().NotBeNull();

            // To reset
            comp.Find("div.mud-treeview-item-content").Click();
            comp.Instance.SelectedValue.Should().BeNull();

            comp.Find("div.mud-treeview-item-content").DoubleClick();
            comp.Instance.SelectedValue.Should().NotBeNull();
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

        [Test]
        public async Task TreeView_OtherTest()
        {
            var comp = Context.RenderComponent<TreeViewTest2>();
            var item = comp.FindComponent<MudTreeViewItem<string>>();
#pragma warning disable BL0005
            await comp.InvokeAsync(() => item.Instance.Disabled = true);
            comp.WaitForAssertion(() => item.Instance.Selected.Should().BeFalse());
            await comp.InvokeAsync(() => item.Instance.Selected = true);
            comp.WaitForAssertion(() => item.Instance.Selected.Should().BeTrue());
            await comp.InvokeAsync(() => item.Instance.SelectItem(true));
            comp.WaitForAssertion(() => item.Instance.Selected.Should().BeTrue());
            comp.WaitForAssertion(() => item.Instance.ArrowExpanded.Should().BeFalse());
            await comp.InvokeAsync(() => item.Instance.ArrowExpanded = true);
            comp.WaitForAssertion(() => item.Instance.ArrowExpanded.Should().BeTrue());
            await comp.InvokeAsync(() => item.Instance.ArrowExpanded = true);
            comp.WaitForAssertion(() => item.Instance.ArrowExpanded.Should().BeTrue());

            comp.WaitForAssertion(() => item.Instance.Expanded.Should().BeTrue());
            await comp.InvokeAsync(() => item.Instance.OnItemExpanded(true));
            comp.WaitForAssertion(() => item.Instance.Expanded.Should().BeTrue());

            await comp.InvokeAsync(() => item.Instance.Select(false));
        }

        [Test]
        public async Task TreeViewItem_DoubleClick_CheckExpanded()
        {
            var comp = Context.RenderComponent<TreeViewTest3>();
            bool itemIsExpanded = false;

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

            await comp.InvokeAsync(() => comp.Instance.SelectFirst());
            comp.WaitForAssertion(() => comp.Instance.selectedValue.Should().Be("content"));

            await comp.InvokeAsync(() => comp.Instance.SelectSecond());
            comp.WaitForAssertion(() => comp.Instance.selectedValue.Should().Be("src"));

            await comp.InvokeAsync(() => comp.Instance.DeselectSecond());
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
        
    }
}
