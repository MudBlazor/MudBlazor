#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents.Tabs;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class TabsTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public async Task AddingAndRemovingTabPanels()
        {
            var comp = ctx.RenderComponent<TabsAddingRemovingTabsTest>();
            Console.WriteLine(comp.Markup);
            comp.Find("div.mud-tabs-panels").InnerHtml.Trim().Should().BeEmpty();
            comp.FindAll("div.mud-tab").Should().BeEmpty();
            // add a panel
            comp.FindAll("button")[0].Click();
            Console.WriteLine("\n" + comp.Markup);
            comp.Find("div.mud-tabs-panels").InnerHtml.Trim().Should().NotBeEmpty();
            comp.FindAll("div.mud-tab").Count.Should().Be(1);
            comp.FindAll("p.mud-typography").Count.Should().Be(1);
            // add another
            comp.FindAll("button")[0].Click();
            Console.WriteLine("\n" + comp.Markup);
            comp.FindAll("div.mud-tab").Count.Should().Be(2);
            comp.FindAll("p.mud-typography").Count.Should().Be(1, because: "Only the current tab panel is displayed");
            // we are now on tab 0
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Tab 0");
            // switch to tab1
            comp.FindAll("div.mud-tab")[1].Click();
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Tab 1");
            // remove tab1
            comp.FindAll("button")[1].Click();
            comp.FindAll("div.mud-tab").Count.Should().Be(1);
            comp.FindAll("p.mud-typography").Count.Should().Be(1);
            // we should be on tab0 again
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Tab 0");
            // remove another
            comp.FindAll("button")[1].Click();
            comp.Find("div.mud-tabs-panels").InnerHtml.Trim().Should().BeEmpty();
            comp.FindAll("div.mud-tab").Should().BeEmpty();
        }

        [Test]
        public async Task KeepTabsAliveTest()
        {
            var comp = ctx.RenderComponent<TabsKeepAliveTest>();
            Console.WriteLine(comp.Markup);
            Console.WriteLine($"{comp.Instance.content1.ComponentName} Counter : {comp.Instance.content1.Counter}");
            Console.WriteLine($"{comp.Instance.content2.ComponentName} Counter : {comp.Instance.content2.Counter}");
            Console.WriteLine($"{comp.Instance.content3.ComponentName} Counter : {comp.Instance.content3.Counter}");
            Console.WriteLine("=======================");

            comp.Instance.IncrementCountOfTab(1);
            comp.Instance.content1.Counter.Should().Be(1);
            comp.Instance.tabs.ActivatePanel(1);
            comp.Instance.content1.Counter.Should().Be(1);
            Console.WriteLine(comp.Markup);
            Console.WriteLine($"Active Panel index: {comp.Instance.tabs.ActivePanelIndex}");
            Console.WriteLine($"{comp.Instance.content1.ComponentName} Counter : {comp.Instance.content1.Counter}");
            Console.WriteLine($"{comp.Instance.content2.ComponentName} Counter : {comp.Instance.content2.Counter}");
            Console.WriteLine($"{comp.Instance.content3.ComponentName} Counter : {comp.Instance.content3.Counter}");
            Console.WriteLine("=======================");

            comp.Instance.IncrementCountOfTab(2);
            comp.Instance.IncrementCountOfTab(2);
            comp.Instance.content1.Counter.Should().Be(1);
            comp.Instance.content2.Counter.Should().Be(2);
            comp.Instance.tabs.ActivatePanel(2);
            comp.Instance.content1.Counter.Should().Be(1);
            comp.Instance.content2.Counter.Should().Be(2);
            comp.Instance.content3.Counter.Should().Be(0);
            Console.WriteLine(comp.Markup);
            Console.WriteLine($"Active Panel index: {comp.Instance.tabs.ActivePanelIndex}");
            Console.WriteLine($"{comp.Instance.content1.ComponentName} Counter : {comp.Instance.content1.Counter}");
            Console.WriteLine($"{comp.Instance.content2.ComponentName} Counter : {comp.Instance.content2.Counter}");
            Console.WriteLine($"{comp.Instance.content3.ComponentName} Counter : {comp.Instance.content3.Counter}");
            Console.WriteLine("=======================");

            comp.Instance.IncrementCountOfTab(3);
            comp.Instance.content1.Counter.Should().Be(1);
            comp.Instance.content2.Counter.Should().Be(2);
            comp.Instance.content3.Counter.Should().Be(1);
            comp.Instance.tabs.ActivatePanel(0);
            comp.Instance.content1.Counter.Should().Be(1);
            comp.Instance.content2.Counter.Should().Be(2);
            comp.Instance.content3.Counter.Should().Be(1);
            Console.WriteLine(comp.Markup);
            Console.WriteLine($"Active Panel index: {comp.Instance.tabs.ActivePanelIndex}");
            Console.WriteLine($"{comp.Instance.content1.ComponentName} Counter : {comp.Instance.content1.Counter}");
            Console.WriteLine($"{comp.Instance.content2.ComponentName} Counter : {comp.Instance.content2.Counter}");
            Console.WriteLine($"{comp.Instance.content3.ComponentName} Counter : {comp.Instance.content3.Counter}");

        }


    }

}
