
using System;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class TreeViewTest : BunitTest
    {
        [Test]
        public void Collapsed_ClickOnArrowButton_CheckClose()
        {
            var comp = Context.RenderComponent<TreeViewTest1>();
            Console.WriteLine(comp.Markup);
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
            Console.WriteLine(comp.Markup);
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
            Console.WriteLine(comp.Markup);
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
            Console.WriteLine(comp.Markup);
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
            Console.WriteLine(comp.Markup);
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
            Console.WriteLine(comp.Markup);
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(8);
        }

        [Test]
        public void TreeViewServerTest()
        {
            var comp = Context.RenderComponent<TreeViewServerTest>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(4);
            comp.FindAll("div.mud-treeview-item-content")[0].Click();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(4);
            comp.FindAll("div.mud-treeview-item-content")[2].Click();
            comp.FindAll("li.mud-treeview-item").Count.Should().Be(8);
        }
    }
}
