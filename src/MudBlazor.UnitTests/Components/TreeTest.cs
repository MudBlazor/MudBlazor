using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents.Tree;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class TreeTest
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddMudBlazorServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        /// <summary>
        /// Expand-collapse a node, and check 
        /// </summary>
        [Test]
        public void ExpandTest1()
        {
            var comp = ctx.RenderComponent<TreeTest1>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("li.mud-tree-item").Count.Should().Be(10);
            comp.Find("button").Click();
            comp.FindAll("li.mud-tree-item .mud-collapse-container.mud-collapse-expanded").Count.Should().Be(1);
            comp.Find("button").Click();
            comp.FindAll("li.mud-tree-item .mud-collapse-container.mud-collapse-expanded").Count.Should().Be(0);
            comp.Find("div.mud-tree-item-content").Click();
            comp.FindAll("li.mud-tree-item .mud-collapse-container.mud-collapse-expanded").Count.Should().Be(0);
        }

        /// <summary>
        /// Expand-collapse a node, and check with Expand-On-Click
        /// </summary>
        [Test]
        public void ExpandTest2()
        {
            var comp = ctx.RenderComponent<TreeTest2>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("li.mud-tree-item").Count.Should().Be(10);
            comp.Find("button").Click();
            comp.FindAll("li.mud-tree-item .mud-collapse-container.mud-collapse-expanded").Count.Should().Be(1);
            comp.Find("button").Click();
            comp.FindAll("li.mud-tree-item .mud-collapse-container.mud-collapse-expanded").Count.Should().Be(0);
            comp.Find("div.mud-tree-item-content").Click();
            comp.FindAll("li.mud-tree-item .mud-collapse-container.mud-collapse-expanded").Count.Should().Be(1);
            comp.Find("div.mud-tree-item-content").Click();
            comp.FindAll("li.mud-tree-item .mud-collapse-container.mud-collapse-expanded").Count.Should().Be(0);
        }

        /// <summary>
        /// Select an item, then check it affects itself also all the childitems
        /// </summary>
        [Test]
        public void SelectionTest()
        {
            var comp = ctx.RenderComponent<TreeTest1>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("li.mud-tree-item").Count.Should().Be(10);
            comp.Find("button").Click();
            comp.FindAll("li.mud-tree-item .mud-collapse-container.mud-collapse-expanded").Count.Should().Be(1);
            comp.FindAll("input.mud-checkbox-input").Count.Should().Be(10);
            comp.Find("input.mud-checkbox-input").Change(true);
            comp.Instance.SubItemSelected.Should().BeTrue();
            comp.Instance.ParentItemSelected.Should().BeTrue();
            comp.FindAll("input.mud-checkbox-input")[2].Change(false);
            comp.Instance.SubItemSelected.Should().BeFalse();
            comp.Instance.ParentItemSelected.Should().BeTrue();
        }

        /// <summary>
        /// Activate a node, then activate another. Check the binding values.
        /// </summary>
        [Test]
        public void ActivationTest()
        {
            var comp = ctx.RenderComponent<TreeTest1>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("div.mud-tree-item-content.mud-tree-item-activated").Count.Should().Be(0);
            comp.Find("div.mud-tree-item-content").Click();
            comp.Instance.Item1Activated.Should().BeTrue();
            comp.Instance.Item2Activated.Should().BeFalse();
            comp.FindAll("div.mud-tree-item-content.mud-tree-item-activated").Count.Should().Be(1);
            comp.FindAll("div.mud-tree-item-content")[4].Click();
            comp.Instance.Item1Activated.Should().BeFalse();
            comp.Instance.Item2Activated.Should().BeTrue();
            comp.FindAll("div.mud-tree-item-content.mud-tree-item-activated").Count.Should().Be(1);
        }
    }
}
