using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ToolTipTests : BunitTest
    {
        [Test]
        public void DefaultValue()
        {
            var toolTip = new MudTooltip();

            toolTip.Color.Should().Be(Color.Default);
            toolTip.Text.Should().BeEmpty();
            toolTip.Arrow.Should().BeFalse();
            toolTip.Duration.Should().Be(251);
            toolTip.Delay.Should().Be(0);
            toolTip.Placement.Should().Be(Placement.Bottom);
            toolTip.Inline.Should().BeTrue();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task RenderContent(bool usingFocusout)
        {
            var comp = Context.RenderComponent<TooltipWithTextTest>(p => p.Add(
                x => x.TooltipTextContent, "my tooltip content text"
                ));

            var tooltipComp = comp.FindComponent<MudTooltip>().Instance;

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

            //not visible by default
            tooltipComp.GetState(x => x.Visible).Should().BeFalse();

            //trigger pointerover

            await button.ParentElement.TriggerEventAsync("onpointerenter", new PointerEventArgs());

            //content should be visible
            popoverContentNode.TextContent.Should().Be("my tooltip content text");
            popoverContentNode.ClassList.Should().Contain("d-flex");

            tooltipComp.GetState(x => x.Visible).Should().BeTrue();

            //trigger pointerleave
            if (!usingFocusout)
            {
                await button.ParentElement.TriggerEventAsync("onpointerleave", new PointerEventArgs());
            }
            else
            {
                button.ParentElement.FocusOut();
            }
            //no content should be visible
            popoverContentNode.Children.Should().BeEmpty();

            tooltipComp.GetState(x => x.Visible).Should().BeFalse();
        }

        [Test]
        public void NoPopoverIfThereIsNoContent()
        {
            var comp = Context.RenderComponent<TooltipWithTextTest>(p => p.Add(
                x => x.TooltipTextContent, null
                ));

            // content should always be visible
            var button = comp.Find("button");
            button.TextContent.Should().Be("My Buttion");

            button.ParentElement.ClassList.Should().Contain("mud-tooltip-root");

            //popover should be but not having a content
            button.ParentElement.Children.Should().ContainSingle();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task RenderTooltipFragment(bool usingFocusout)
        {
            var comp = Context.RenderComponent<TooltipWithRenderFragmentContentTest>();

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

            //trigger pointerover

            await button.ParentElement.TriggerEventAsync("onpointerenter", new PointerEventArgs());

            //content should be visible
            popoverContentNode.ClassList.Should().Contain("mud-tooltip");
            popoverContentNode.ClassList.Should().Contain("d-flex");

            comp.Find(".my-customer-paper").Children[0].TextContent.Should().Be("My content");

            //trigger pointerleave
            if (!usingFocusout)
            {
                await button.ParentElement.TriggerEventAsync("onpointerleave", new PointerEventArgs());
            }
            else
            {
                button.ParentElement.FocusOut();
            }
            //no content should be visible
            popoverContentNode.Children.Should().BeEmpty();
        }

        [Test]
        [TestCase(false, new[] { "mud-tooltip-root" })]
        [TestCase(true, new[] { "mud-tooltip-root", "mud-tooltip-inline" })]
        public void ContainerClass_PropertyRelations(bool inlineValue, string[] expectedClasses)
        {
            var comp = Context.RenderComponent<ToolTipContainerPropertyTest>(p =>
            p.Add(x => x.Inline, inlineValue));

            comp.Nodes.Last().Should().BeAssignableTo<IHtmlElement>();

            var container = comp.Nodes.Last() as IHtmlElement;

            container.ClassList.Should().BeEquivalentTo(expectedClasses);
        }

        [Test]
        public async Task InnerClass_ChildContentWrapper()
        {
            var comp = Context.RenderComponent<ToolTipPopoverClassPropertyTest>();

            var button = comp.Find("button");
            await button.ParentElement.TriggerEventAsync("onpointerenter", new PointerEventArgs());

            var wrapperDivNode = comp.Find("#my-tooltip-content").ParentElement;

            wrapperDivNode.ClassList.Should().BeEquivalentTo(new[] { "d-block" });
        }

        [Test]
        [TestCase(false, new[] { "mud-tooltip" })]
        [TestCase(true, new[] { "mud-tooltip", "mud-tooltip-arrow" })]
        public async Task PopoverClass_PropertyArrow(bool arrowValue, string[] expectedClasses)
        {
            var comp = Context.RenderComponent<ToolTipPopoverClassPropertyTest>(p =>
            p.Add(x => x.Arrow, arrowValue));

            var button = comp.Find("button");
            await button.ParentElement.TriggerEventAsync("onpointerenter", new PointerEventArgs());

            var popoverContentNode = comp.Find("#my-tooltip-content").ParentElement.ParentElement;

            popoverContentNode.ClassList.Should().Contain(expectedClasses);
        }

        [Test]
        [TestCase(Color.Default, new[] { "mud-tooltip", "mud-tooltip-default" })]
        [TestCase(Color.Tertiary, new[] { "mud-tooltip", "mud-theme-tertiary" })]
        [TestCase(Color.Success, new[] { "mud-tooltip", "mud-theme-success" })]
        [TestCase(Color.Dark, new[] { "mud-tooltip", "mud-theme-dark" })]
        public async Task PopoverClass_PropertyColor(Color colorValue, string[] expectedClasses)
        {
            var comp = Context.RenderComponent<ToolTipPopoverClassPropertyTest>(p =>
            p.Add(x => x.Color, colorValue));

            var button = comp.Find("button");
            await button.ParentElement.TriggerEventAsync("onpointerenter", new PointerEventArgs());

            var popoverContentNode = comp.Find("#my-tooltip-content").ParentElement.ParentElement;

            popoverContentNode.ClassList.Should().Contain(expectedClasses);
        }

        [Test]
        [TestCase(Color.Default, false, new[] { "mud-tooltip", "mud-tooltip-default", })]
        [TestCase(Color.Default, true, new[] { "mud-tooltip", "mud-tooltip-default", "mud-tooltip-arrow" })]
        [TestCase(Color.Success, true, new[] { "mud-tooltip", "mud-theme-success", "mud-tooltip-arrow", "mud-border-success" })]
        [TestCase(Color.Success, false, new[] { "mud-tooltip", "mud-theme-success" })]
        public async Task PopoverClass_PropertyColorAndArrow(Color colorValue, bool arrowValue, string[] expectedClasses)
        {
            var comp = Context.RenderComponent<ToolTipPopoverClassPropertyTest>(p =>
            {
                p.Add(x => x.Color, colorValue);
                p.Add(x => x.Arrow, arrowValue);
            });

            var button = comp.Find("button");
            await button.ParentElement.TriggerEventAsync("onpointerenter", new PointerEventArgs());

            var popoverContentNode = comp.Find("#my-tooltip-content").ParentElement.ParentElement;

            popoverContentNode.ClassList.Should().Contain(expectedClasses);
        }

        [Test]
        [TestCase(Placement.Bottom, false, new[] { "mud-tooltip", "mud-tooltip-bottom-center", "mud-popover-anchor-bottom-center", "mud-popover-top-center" })]
        [TestCase(Placement.Bottom, true, new[] { "mud-tooltip", "mud-tooltip-bottom-center", "mud-popover-anchor-bottom-center", "mud-popover-top-center" })]
        [TestCase(Placement.Top, false, new[] { "mud-tooltip", "mud-tooltip-top-center", "mud-popover-anchor-top-center", "mud-popover-bottom-center" })]
        [TestCase(Placement.Top, true, new[] { "mud-tooltip", "mud-tooltip-top-center", "mud-popover-anchor-top-center", "mud-popover-bottom-center" })]
        [TestCase(Placement.Left, false, new[] { "mud-tooltip", "mud-tooltip-center-left", "mud-popover-anchor-center-left", "mud-popover-center-right" })]
        [TestCase(Placement.Left, true, new[] { "mud-tooltip", "mud-tooltip-center-left", "mud-popover-anchor-center-left", "mud-popover-center-right" })]
        [TestCase(Placement.Start, false, new[] { "mud-tooltip", "mud-tooltip-center-left", "mud-popover-anchor-center-left", "mud-popover-center-right" })]
        [TestCase(Placement.Start, true, new[] { "mud-tooltip", "mud-tooltip-center-right", "mud-popover-anchor-center-right", "mud-popover-center-left" })]
        [TestCase(Placement.Right, false, new[] { "mud-tooltip", "mud-tooltip-center-right", "mud-popover-anchor-center-right", "mud-popover-center-left" })]
        [TestCase(Placement.Right, true, new[] { "mud-tooltip", "mud-tooltip-center-right", "mud-popover-anchor-center-right", "mud-popover-center-left" })]
        [TestCase(Placement.End, false, new[] { "mud-tooltip", "mud-tooltip-center-right", "mud-popover-anchor-center-right", "mud-popover-center-left" })]
        [TestCase(Placement.End, true, new[] { "mud-tooltip", "mud-tooltip-center-left", "mud-popover-anchor-center-left", "mud-popover-center-right" })]

        public async Task PopoverClass_Placement(Placement placementValue, bool rtlValue, string[] expectedClasses)
        {
            var comp = Context.RenderComponent<ToolTipPlacementPropertyTest>(p =>
            {
                p.Add(x => x.Placement, placementValue);
                p.Add(x => x.RightToLeft, rtlValue);
            });

            var button = comp.Find("button");
            await button.ParentElement.TriggerEventAsync("onpointerenter", new PointerEventArgs());

            var popoverContentNode = comp.Find("#my-tooltip-content").ParentElement.ParentElement;

            popoverContentNode.ClassList.Should().Contain(expectedClasses);
        }

        [Test]
        public async Task Tooltip_On_Focus()
        {
            var comp = Context.RenderComponent<ToolTipPlacementPropertyTest>();

            var button = comp.Find("button");
            await button.ParentElement.TriggerEventAsync("onfocusin", new FocusEventArgs());

            var popoverContentNode = comp.Find("#my-tooltip-content").ParentElement;

            popoverContentNode.Should().NotBeNull();
        }

        [Test]
        public async Task Tooltip_On_Click()
        {
            var comp = Context.RenderComponent<TooltipClickTest>();
            var tooltipComp = comp.FindComponent<MudTooltip>().Instance;
            tooltipComp.Visible.Should().BeFalse();
            var button = comp.Find("button");
            await button.ParentElement.TriggerEventAsync("onpointerup", new PointerEventArgs());

            var popoverContentNode = comp.Find("#my-tooltip-content").ParentElement;
            tooltipComp.GetState(x => x.Visible).Should().BeTrue();
            popoverContentNode.Should().NotBeNull();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Visible_ByDefault(bool usingFocusout)
        {
            var comp = Context.RenderComponent<TooltipVisiblePropTest>(p =>
            {
                p.Add(x => x.TooltipVisible, true);
            });
            var tooltipComp = comp.FindComponent<MudTooltip>().Instance;

            comp.Instance.TooltipVisible.Should().BeTrue();
            tooltipComp.Visible.Should().BeTrue(); //tooltip is visible by default in this case

            var button = comp.Find("button");

            if (!usingFocusout)
            {
                await button.ParentElement.TriggerEventAsync("onpointerleave", new PointerEventArgs());
            }
            else
            {
                button.ParentElement.FocusOut();
            }

            tooltipComp.Visible.Should().BeFalse();
            comp.Instance.TooltipVisible.Should().BeFalse();
        }

        [Test]
        public async Task Tooltip_Style_Respected()
        {
            var comp = Context.RenderComponent<TestComponents.Tooltip.TooltipStylingTest>();
            var tooltipComp = comp.FindComponent<MudTooltip>().Instance;
            var button = comp.Find("button");
            await button.ParentElement.TriggerEventAsync("onpointerup", new PointerEventArgs());

            tooltipComp.Style.Should().Contain("background-color").And.Contain("orangered");
        }


        [Test]
        public void Tooltip_Disabled_Default_False()
        {
            var comp = Context.RenderComponent<TooltipDisabledPropertyTest>();
            var tooltipComp = comp.FindComponent<MudTooltip>().Instance;
            tooltipComp.Disabled.Should().BeFalse();
        }

        [Test]
        public async Task Tooltip_Disabled_Button_OnFocusIn_NoPopover()
        {
            var comp = Context.RenderComponent<TooltipDisabledPropertyTest>(p =>
            {
                p.Add(x => x.TooltipDisabled, true);
            });

            var button = comp.Find("button");
            await button.ParentElement.TriggerEventAsync("onfocusin", new FocusEventArgs());
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }

        [Test]
        public async Task Tooltip_Disabled_Button_OnPointerEnter_NoPopover()
        {
            var comp = Context.RenderComponent<TooltipDisabledPropertyTest>(p =>
            {
                p.Add(x => x.TooltipDisabled, true);
            });

            var button = comp.Find("button");
            await button.ParentElement.TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
        }
    }
}
