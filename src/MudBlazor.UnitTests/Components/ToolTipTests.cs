
using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ToolTipTests : BunitTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(true)]
        public async Task RenderContent(bool usingFocusout)
        {
            var comp = Context.RenderComponent<TooltipWithTextTest>(p => p.Add(
                x => x.TooltipTextContent, "my tooltip content text"
                ));
            Console.WriteLine(comp.Markup);

            // content should always be visible
            var button = comp.Find("button");
            button.TextContent.Should().Be("My Buttion");

            button.ParentElement.ClassList.Should().Contain("mud-tooltip-root");

            //the button [0] and [1] the popover npde
            button.ParentElement.Children.Should().HaveCount(2);

            var popoverNode = button.ParentElement.Children[1];
            popoverNode.Id.Should().StartWith("popover-");

            var popoverContentNode = comp.Find($"#popovercontent-{popoverNode.Id.Substring(8)}");

            //no content for the popover node
            popoverContentNode.Children.Should().BeEmpty();

            //trigger mouseover

            await button.ParentElement.TriggerEventAsync("onmouseenter", new MouseEventArgs());

            //content should be visible
            popoverContentNode.Children.Should().ContainSingle();
            popoverContentNode.Children[0].GetAttribute("role").Should().Be("tooltip");
            popoverContentNode.Children[0].TextContent.Should().Be("my tooltip content text");

            //trigger mouseleave
            if (usingFocusout == false)
            {
                await button.ParentElement.TriggerEventAsync("onmouseleave", new MouseEventArgs());
            }
            else
            {
                button.ParentElement.FocusOut();
            }
            //no content should be visible
            popoverContentNode.Children.Should().BeEmpty();
        }

        [Test]
        public void NoPopoverIfThereIsNoContent()
        {
            var comp = Context.RenderComponent<TooltipWithTextTest>(p => p.Add(
                x => x.TooltipTextContent, null
                ));
            Console.WriteLine(comp.Markup);

            // content should always be visible
            var button = comp.Find("button");
            button.TextContent.Should().Be("My Buttion");

            button.ParentElement.ClassList.Should().Contain("mud-tooltip-root");

            //popover should be but not having a content
            button.ParentElement.Children.Should().ContainSingle();
        }

        [Test]
        [TestCase(true)]
        [TestCase(true)]
        public async Task RenderTooltipFragement(bool usingFocusout)
        {
            var comp = Context.RenderComponent<TooltipWithRenderFragmentContentTest>();
            Console.WriteLine(comp.Markup);

            // content should always be visible
            var button = comp.Find("button");
            button.TextContent.Should().Be("My Buttion");

            button.ParentElement.ClassList.Should().Contain("mud-tooltip-root");

            //the button [0] and [1] the popover node
            button.ParentElement.Children.Should().HaveCount(2);

            var popoverNode = button.ParentElement.Children[1];
            popoverNode.Id.Should().StartWith("popover-");

            var popoverContentNode = comp.Find($"#popovercontent-{popoverNode.Id.Substring(8)}");

            //no content for the popover node
            popoverContentNode.Children.Should().BeEmpty();

            //trigger mouseover

            await button.ParentElement.TriggerEventAsync("onmouseenter", new MouseEventArgs());

            //content should be visible
            popoverContentNode.Children.Should().ContainSingle();
            popoverContentNode.Children[0].GetAttribute("role").Should().Be("tooltip");

            comp.Find(".my-customer-paper").Children[0].TextContent.Should().Be("My content");

            //trigger mouseleave
            if (usingFocusout == false)
            {
                await button.ParentElement.TriggerEventAsync("onmouseleave", new MouseEventArgs());
            }
            else
            {
                button.ParentElement.FocusOut();
            }
            //no content should be visible
            popoverContentNode.Children.Should().BeEmpty();
        }
    }
}
