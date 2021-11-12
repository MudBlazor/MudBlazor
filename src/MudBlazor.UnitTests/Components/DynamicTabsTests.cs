﻿
#pragma warning disable CS1998 // async without await

using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DynamicTabsTests : BunitTest
    {
        [Test]
        public async Task DefaultValues()
        {
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = Context.RenderComponent<MudDynamicTabs>();
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

            ((IHtmlDivElement)comp.Nodes[0]).ClassList.Should().BeEquivalentTo("mud-tabs", "mud-dynamic-tabs");
        }

        [Test]
        public async Task BasicParameters()
        {
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = Context.RenderComponent<SimpleDynamicTabsTest>();
            Console.WriteLine(comp.Markup);

            // three panels three close icons;
            var closeButtons = comp.FindAll(".my-close-icon-class");
            closeButtons.Should().HaveCount(3);

            foreach (var item in closeButtons)
            {
                item.GetAttribute("style").Should().Be("propertyA: 4px");
                item.ClassList.Should().StartWith(new string[] { "mud-button-root" });

                var actual = XElement.Parse($"<test>{item.Children[0].Children[0].InnerHtml}</test>");
                var expected = XElement.Parse($"<test>{Icons.Material.Filled.RestoreFromTrash}</test>");

                actual.Should().BeEquivalentTo(expected);
            }

            var addButtons = comp.FindAll(".my-add-icon-class");

            addButtons.Should().HaveCount(1);
            foreach (var item in addButtons)
            {
                item.GetAttribute("style").Should().Be("propertyB: 6px");
                item.ClassList.Should().StartWith(new string[] { "mud-button-root" });

                var actual = XElement.Parse($"<test>{item.Children[0].Children[0].InnerHtml}</test>");
                var expected = XElement.Parse($"<test>{Icons.Material.Filled.AddAlarm}</test>");

                actual.Should().BeEquivalentTo(expected);

            }
        }

        [Test]
        public async Task BasicParameters_WithToolTips()
        {
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));

            var comp = Context.RenderComponent<SimpleDynamicTabsTestWithToolTips>();
            Console.WriteLine(comp.Markup);

            // three panels three close icons;
            var closeButtons = comp.FindAll(".my-close-icon-class");
            closeButtons.Should().HaveCount(3);

            foreach (var item in closeButtons)
            {
                item.GetAttribute("style").Should().Be("propertyA: 4px");
                item.ClassList.Should().StartWith(new string[] { "mud-button-root" });

                var actual = XElement.Parse($"<test>{item.Children[0].Children[0].InnerHtml}</test>");
                var expected = XElement.Parse($"<test>{Icons.Material.Filled.RestoreFromTrash}</test>");

                actual.Should().BeEquivalentTo(expected);

                var parent = (IHtmlElement)item.Parent;
                parent.Children.Should().HaveCount(2, because: "the button and the empty popover hint");

                await item.ParentElement.TriggerEventAsync("onmouseenter", new MouseEventArgs());
                var popoverId = parent.Children[1].Id.Substring(8);

                var toolTip = comp.Find($"#popovercontent-{popoverId}");

                toolTip.ClassList.Should().Contain(new string[] { "mud-tooltip" });
                toolTip.TextContent.Should().Be("close here");

                await item.ParentElement.TriggerEventAsync("onmouseleave", new MouseEventArgs());

            }

            var addButtons = comp.FindAll(".my-add-icon-class");

            addButtons.Should().HaveCount(1);
            foreach (var item in addButtons)
            {
                item.GetAttribute("style").Should().Be("propertyB: 6px");
                item.ClassList.Should().StartWith(new string[] { "mud-button-root" });

                var actual = XElement.Parse($"<test>{item.Children[0].Children[0].InnerHtml}</test>");
                var expected = XElement.Parse($"<test>{Icons.Material.Filled.AddAlarm}</test>");

                actual.Should().BeEquivalentTo(expected);

                var parent = (IHtmlElement)item.Parent;
                parent.Children.Should().HaveCount(2, because: "the button and the empty popover hint"); ;

                await item.ParentElement.TriggerEventAsync("onmouseenter", new MouseEventArgs());
                var popoverId = parent.Children[1].Id.Substring(8);

                var toolTip = comp.Find($"#popovercontent-{popoverId}");

                toolTip.ClassList.Should().Contain(new string[] { "mud-tooltip" });
                toolTip.TextContent.Should().Be("add here");

                await item.ParentElement.TriggerEventAsync("onmouseleave", new MouseEventArgs());
            }
        }

        [Test]
        public async Task TestInteractions_AddTab()
        {
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));
            var comp = Context.RenderComponent<SimpleDynamicTabsInteractionTest>();

            Console.WriteLine(comp.Markup);

            var addButton = comp.Find(".my-add-icon-class");
            addButton.Click();

            await Task.Delay(5);
            comp.Instance.AddClickCounter.Should().Be(1);
        }

        [Test]
        public async Task TestInteractions_RemoveTab()
        {
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserver), new MockResizeObserver()));
            var comp = Context.RenderComponent<SimpleDynamicTabsInteractionTest>();

            Console.WriteLine(comp.Markup);

            for (var i = 0; i < 3; i++)
            {
                var closeButton = comp.FindAll(".my-close-icon-class")[i];
                closeButton.Click();

                await Task.Delay(5);

                comp.Instance.CloseClicked.Should().HaveCount(i + 1);
            }

        }
    }
}
