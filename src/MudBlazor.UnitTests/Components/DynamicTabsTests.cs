#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Interop;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{

    [TestFixture]
    public class DynamicTabsTests
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
        public async Task DefaultValues()
        {
            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = ctx.RenderComponent<MudDynamicTabs>();
            var tabs = comp.Instance;

            tabs.Header.Should().NotBeNull();
            tabs.TabPanelHeader.Should().NotBeNull();

            tabs.HeaderPosition.Should().Be(TabHeaderPosition.After);
            tabs.TabPanelHeaderPosition.Should().Be(TabHeaderPosition.After);

            tabs.AddTabIcon.Should().Be(Icons.Material.Filled.Add);
            tabs.CloseTabIcon.Should().Be(Icons.Material.Filled.Close);

            tabs.AddIconClass.Should().BeNullOrEmpty();
            tabs.AddIconStyle.Should().BeNullOrEmpty();
            tabs.AddIconToolTip.Should().BeNullOrEmpty();

            tabs.CloseIconClass.Should().BeNullOrEmpty();
            tabs.CloseIconStyle.Should().BeNullOrEmpty();
            tabs.CloseIconToolTip.Should().BeNullOrEmpty();

            comp.Nodes.Should().ContainSingle();
            comp.Nodes[0].Should().BeAssignableTo<IHtmlDivElement>();

            (comp.Nodes[0] as IHtmlDivElement).ClassList.Should().BeEquivalentTo(new[] { "mud-tabs", "mud-dynamic-tabs" });
        }

        [Test]
        public async Task BasicParameters()
        {
            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = ctx.RenderComponent<SimpleDynamicTabsTest>();
            Console.WriteLine(comp.Markup);

            // three panels three close icons;
            var closeButtons = comp.FindAll(".my-close-icon-class");
            closeButtons.Should().HaveCount(3);

            foreach (var item in closeButtons)
            {
                item.GetAttribute("style").Should().Be("propertyA: 4px");
                item.ClassList.Should().StartWith(new string[] { "mud-button-root" });

                XElement actual = XElement.Parse($"<test>{item.Children[0].Children[0].InnerHtml}</test>");
                XElement expected = XElement.Parse($"<test>{Icons.Material.Filled.RestoreFromTrash}</test>");

                actual.Should().BeEquivalentTo(expected);
            }

            var addButtons = comp.FindAll(".my-add-icon-class");

            addButtons.Should().HaveCount(1);
            foreach (var item in addButtons)
            {
                item.GetAttribute("style").Should().Be("propertyB: 6px");
                item.ClassList.Should().StartWith(new string[] { "mud-button-root" });

                XElement actual = XElement.Parse($"<test>{item.Children[0].Children[0].InnerHtml}</test>");
                XElement expected = XElement.Parse($"<test>{Icons.Material.Filled.AddAlarm}</test>");

                actual.Should().BeEquivalentTo(expected);

            }
        }

        [Test]
        public async Task BasicParameters_WithToolTips()
        {
            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = ctx.RenderComponent<SimpleDynamicTabsTestWithToolTips>();
            Console.WriteLine(comp.Markup);

            // three panels three close icons;
            var closeButtons = comp.FindAll(".my-close-icon-class");
            closeButtons.Should().HaveCount(3);

            foreach (var item in closeButtons)
            {
                item.GetAttribute("style").Should().Be("propertyA: 4px");
                item.ClassList.Should().StartWith(new string[] { "mud-button-root" });

                XElement actual = XElement.Parse($"<test>{item.Children[0].Children[0].InnerHtml}</test>");
                XElement expected = XElement.Parse($"<test>{Icons.Material.Filled.RestoreFromTrash}</test>");

                actual.Should().BeEquivalentTo(expected);

                var parent = (IHtmlElement)item.Parent;
                parent.Children.Should().HaveCount(2);

                var toolTip = parent.Children[1];
                toolTip.ClassList.Should().StartWith(new string[] { "mud-tooltip" });
                toolTip.TextContent.Should().Be("close here");
            }

            var addButtons = comp.FindAll(".my-add-icon-class");

            addButtons.Should().HaveCount(1);
            foreach (var item in addButtons)
            {
                item.GetAttribute("style").Should().Be("propertyB: 6px");
                item.ClassList.Should().StartWith(new string[] { "mud-button-root" });

                XElement actual = XElement.Parse($"<test>{item.Children[0].Children[0].InnerHtml}</test>");
                XElement expected = XElement.Parse($"<test>{Icons.Material.Filled.AddAlarm}</test>");

                actual.Should().BeEquivalentTo(expected);

                var parent = (IHtmlElement)item.Parent;
                parent.Children.Should().HaveCount(2);

                var toolTip = parent.Children[1];
                toolTip.ClassList.Should().StartWith(new string[] { "mud-tooltip" });
                toolTip.TextContent.Should().Be("add here");
            }
        }

        [Test]
        public async Task TestInteractions_AddTab()
        {
            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));
            var comp = ctx.RenderComponent<SimpleDynamicTabsInteractionTest>();

            Console.WriteLine(comp.Markup);

            var addButton = comp.Find(".my-add-icon-class");
            addButton.Click();

            await Task.Delay(5);
            comp.Instance.AddClickCounter.Should().Be(1);
        }

        [Test]
        public async Task TestInteractions_RemoveTab()
        {
            ctx.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));
            var comp = ctx.RenderComponent<SimpleDynamicTabsInteractionTest>();

            Console.WriteLine(comp.Markup);

            for (int i = 0; i < 3; i++)
            {
                var closeButton = comp.FindAll(".my-close-icon-class")[i];
                closeButton.Click();

                await Task.Delay(5);

                comp.Instance.CloseClicked.Should().HaveCount(i + 1);
            }

        }
    }
}
