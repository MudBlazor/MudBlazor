using System.Xml.Linq;
using AngleSharp.Html.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.TestComponents.Tabs;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DynamicTabsTests : BunitTest
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            Context.Services.Add(new ServiceDescriptor(typeof(IResizeObserverFactory), new MockResizeObserverFactory()));
        }

        [Test]
        public void DefaultValues()
        {
            var comp = Context.Render<MudDynamicTabs>();
            var tabs = comp.Instance;

            tabs.Header.Should().NotBeNull();
            tabs.TabPanelHeader.Should().NotBeNull();

            tabs.HeaderPosition.Should().Be(TabHeaderPosition.After);
            tabs.TabPanelHeaderPosition.Should().Be(TabHeaderPosition.After);

            tabs.AddTabIcon.Should().Be(Icons.Material.Filled.Add);
            tabs.CloseTabIcon.Should().Be(Icons.Material.Filled.Close);

            tabs.AddIconClass.Should().BeNullOrEmpty();
#pragma warning disable CS0618 // Type or member is obsolete
            tabs.AddIconStyle.Should().BeNullOrEmpty();
#pragma warning restore CS0618 // Type or member is obsolete
            tabs.AddIconToolTip.Should().BeNullOrEmpty();

            tabs.CloseIconClass.Should().BeNullOrEmpty();
#pragma warning disable CS0618 // Type or member is obsolete
            tabs.CloseIconStyle.Should().BeNullOrEmpty();
#pragma warning restore CS0618 // Type or member is obsolete
            tabs.CloseIconToolTip.Should().BeNullOrEmpty();

            comp.Nodes.Should().ContainSingle();
            comp.Nodes[0].Should().BeAssignableTo<IHtmlDivElement>();

            ((IHtmlDivElement)comp.Nodes[0]).ClassList.Should().BeEquivalentTo("mud-tabs", "mud-dynamic-tabs");
        }

        [Test]
        public void BasicParameters()
        {
            var comp = Context.Render<SimpleDynamicTabsTest>();

            // three panels three close icons;
            var closeButtons = comp.FindAll(".my-close-icon-class");
            closeButtons.Should().HaveCount(3);

            foreach (var item in closeButtons)
            {
                item.ClassList.Should().StartWith(new string[] { "mud-button-root" });

                var actual = XElement.Parse($"<test>{item.Children[0].Children[0].InnerHtml}</test>");
                var expected = XElement.Parse($"<test>{Icons.Material.Filled.RestoreFromTrash}</test>");

                actual.Should().BeEquivalentTo(expected);
            }

            var addButtons = comp.FindAll(".my-add-icon-class");

            addButtons.Should().HaveCount(1);
            foreach (var item in addButtons)
            {
                item.ClassList.Should().StartWith(new string[] { "mud-button-root" });

                var actual = XElement.Parse($"<test>{item.Children[0].Children[0].InnerHtml}</test>");
                var expected = XElement.Parse($"<test>{Icons.Material.Filled.AddAlarm}</test>");

                actual.Should().BeEquivalentTo(expected);

            }
        }

        [Test]
        public async Task BasicParameters_WithToolTips()
        {
            var comp = Context.Render<SimpleDynamicTabsTestWithToolTips>();

            // three panels three close icons;
            var closeButtons = comp.FindAll(".my-close-icon-class");
            closeButtons.Should().HaveCount(3);

            foreach (var item in closeButtons)
            {
                item.ClassList.Should().StartWith(["mud-button-root"]);

                var actual = XElement.Parse($"<test>{item.Children[0].Children[0].InnerHtml}</test>");
                var expected = XElement.Parse($"<test>{Icons.Material.Filled.RestoreFromTrash}</test>");

                actual.Should().BeEquivalentTo(expected);

                var parent = (IHtmlElement)item.Parent;
                parent.Children.Should().HaveCount(2, because: "the button and the empty popover hint since it's not active");

                await item.ParentElement.TriggerEventAsync("onpointerenter", new PointerEventArgs());

                var popoverId = parent.Children[1].Id.Substring(8);

                var toolTip = comp.Find($"#popovercontent-{popoverId}");

                toolTip.ClassList.Should().Contain(["mud-tooltip"]);
                toolTip.TextContent.Should().Be("close here");

                await item.ParentElement.TriggerEventAsync("onpointerleave", new PointerEventArgs());

            }

            var addButtons = comp.FindAll(".my-add-icon-class");

            addButtons.Should().HaveCount(1);
            foreach (var item in addButtons)
            {
                item.ClassList.Should().StartWith(["mud-button-root"]);

                var actual = XElement.Parse($"<test>{item.Children[0].Children[0].InnerHtml}</test>");
                var expected = XElement.Parse($"<test>{Icons.Material.Filled.AddAlarm}</test>");

                actual.Should().BeEquivalentTo(expected);

                var parent = (IHtmlElement)item.Parent;
                parent.Children.Should().HaveCount(2, because: "the button and the empty popover hint"); ;

                await item.ParentElement.TriggerEventAsync("onpointerenter", new PointerEventArgs());

                var popoverId = parent.Children[1].Id.Substring(8);

                var toolTip = comp.Find($"#popovercontent-{popoverId}");

                toolTip.ClassList.Should().Contain(["mud-tooltip"]);
                toolTip.TextContent.Should().Be("add here");

                await item.ParentElement.TriggerEventAsync("onpointerleave", new PointerEventArgs());
            }
        }

        [Test]
        public async Task TestInteractions_AddTab()
        {
            var comp = Context.Render<SimpleDynamicTabsInteractionTest>();

            var addButton = comp.Find(".my-add-icon-class");
            await addButton.ClickAsync();

            await Task.Delay(5);
            comp.Instance.AddClickCounter.Should().Be(1);
        }

        [Test]
        public async Task TestInteractions_RemoveTab()
        {
            var comp = Context.Render<SimpleDynamicTabsInteractionTest>();

            for (var i = 0; i < 3; i++)
            {
                var closeButton = comp.FindAll(".my-close-icon-class")[i];
                await closeButton.ClickAsync();

                await Task.Delay(5);

                comp.Instance.CloseClicked.Should().HaveCount(i + 1);
            }
        }
    }
}
