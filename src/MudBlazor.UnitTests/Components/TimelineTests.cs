// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using AngleSharp.Css.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class TimelineTests : BunitTest
    {
        [Test]
        public void TimelineTest_DefaultValues()
        {
            var comp = Context.RenderComponent<MudTimeline>();

            comp.Instance.TimelineOrientation.Should().Be(TimelineOrientation.Vertical);
            comp.Instance.TimelinePosition.Should().Be(TimelinePosition.Alternate);
            comp.Instance.TimelineAlign.Should().Be(TimelineAlign.Default);
            comp.Instance.Reverse.Should().Be(false);
            comp.Instance.Modifiers.Should().Be(true);

        }

        /// <summary>
        /// Default Timeline, with five items.
        /// Testing if selection is sync with move commands.
        /// </summary>
        [Test]
        public async Task TimelineTest()
        {
            var comp = Context.RenderComponent<TimelineTest>();
            // print the generated html
            //// select elements needed for the test
            var timeline = comp.FindComponent<MudTimeline>().Instance;
            //// validating some renders
            timeline.Should().NotBeNull();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-timeline").Count.Should().Be(1));
            comp.FindAll("div.mud-timeline-item").Count.Should().Be(5);
            var items = comp.FindComponents<MudTimelineItem>();
            items.Count.Should().Be(5);
            //// changing current index
            for (var i = 1; i <= 4; i++)
            {
                await comp.InvokeAsync(() => timeline.MoveTo(i));
                timeline.SelectedIndex.Should().Be(i);
                timeline.SelectedContainer.Should().Be(items[i].Instance);
            }
            await comp.InvokeAsync(() => timeline.MoveTo(0));
            timeline.SelectedIndex.Should().Be(0);
            timeline.SelectedContainer.Should().Be(items[0].Instance);
        }

        [Test]

        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.Alternate, false, new[] { "mud-timeline-horizontal", "mud-timeline-position-alternate" })]
        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.Start, false, new[] { "mud-timeline-horizontal", "mud-timeline-position-alternate" })]
        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.Left, false, new[] { "mud-timeline-horizontal", "mud-timeline-position-alternate" })]
        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.Right, false, new[] { "mud-timeline-horizontal", "mud-timeline-position-alternate" })]
        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.End, false, new[] { "mud-timeline-horizontal", "mud-timeline-position-alternate" })]

        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.Top, false, new[] { "mud-timeline-horizontal", "mud-timeline-position-top" })]
        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.Bottom, false, new[] { "mud-timeline-horizontal", "mud-timeline-position-bottom" })]

        [TestCase(TimelineOrientation.Vertical, TimelinePosition.Alternate, false, new[] { "mud-timeline-vertical", "mud-timeline-position-alternate" })]
        [TestCase(TimelineOrientation.Vertical, TimelinePosition.Top, false, new[] { "mud-timeline-vertical", "mud-timeline-position-alternate" })]
        [TestCase(TimelineOrientation.Vertical, TimelinePosition.Bottom, false, new[] { "mud-timeline-vertical", "mud-timeline-position-alternate" })]

        [TestCase(TimelineOrientation.Vertical, TimelinePosition.Start, false, new[] { "mud-timeline-vertical", "mud-timeline-position-start" })]
        [TestCase(TimelineOrientation.Vertical, TimelinePosition.End, false, new[] { "mud-timeline-vertical", "mud-timeline-position-end" })]

        [TestCase(TimelineOrientation.Vertical, TimelinePosition.Left, false, new[] { "mud-timeline-vertical", "mud-timeline-position-start" })]
        [TestCase(TimelineOrientation.Vertical, TimelinePosition.Right, false, new[] { "mud-timeline-vertical", "mud-timeline-position-end" })]

        //RTL to true

        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.Alternate, true, new[] { "mud-timeline-horizontal", "mud-timeline-position-alternate" })]
        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.Start, true, new[] { "mud-timeline-horizontal", "mud-timeline-position-alternate" })]
        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.Left, true, new[] { "mud-timeline-horizontal", "mud-timeline-position-alternate" })]
        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.Right, true, new[] { "mud-timeline-horizontal", "mud-timeline-position-alternate" })]
        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.End, true, new[] { "mud-timeline-horizontal", "mud-timeline-position-alternate" })]

        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.Top, true, new[] { "mud-timeline-horizontal", "mud-timeline-position-top" })]
        [TestCase(TimelineOrientation.Horizontal, TimelinePosition.Bottom, true, new[] { "mud-timeline-horizontal", "mud-timeline-position-bottom" })]

        [TestCase(TimelineOrientation.Vertical, TimelinePosition.Alternate, true, new[] { "mud-timeline-vertical", "mud-timeline-position-alternate" })]
        [TestCase(TimelineOrientation.Vertical, TimelinePosition.Top, true, new[] { "mud-timeline-vertical", "mud-timeline-position-alternate" })]
        [TestCase(TimelineOrientation.Vertical, TimelinePosition.Bottom, true, new[] { "mud-timeline-vertical", "mud-timeline-position-alternate" })]

        [TestCase(TimelineOrientation.Vertical, TimelinePosition.Start, true, new[] { "mud-timeline-vertical", "mud-timeline-position-start" })]
        [TestCase(TimelineOrientation.Vertical, TimelinePosition.End, true, new[] { "mud-timeline-vertical", "mud-timeline-position-end" })]

        [TestCase(TimelineOrientation.Vertical, TimelinePosition.Left, true, new[] { "mud-timeline-vertical", "mud-timeline-position-end" })]
        [TestCase(TimelineOrientation.Vertical, TimelinePosition.Right, true, new[] { "mud-timeline-vertical", "mud-timeline-position-start" })]

        public void TimelineTest_Position(TimelineOrientation orientation, TimelinePosition position, bool rtl, string[] expectedClass)
        {
            var comp = Context.RenderComponent<TimelineTest>(p => p.AddCascadingValue("RightToLeft", rtl));

            var timeline = comp.FindComponent<MudTimeline>();

            timeline.SetParametersAndRender(p =>
            {
                p.Add(x => x.TimelineOrientation, orientation);
                p.Add(x => x.TimelinePosition, position);
            });


            timeline.Nodes.Should().ContainSingle();
            timeline.Nodes[0].Should().BeAssignableTo<IHtmlDivElement>();

            (timeline.Nodes[0] as IHtmlDivElement).ClassList.Should().Contain(expectedClass);
        }

        [Test]
        public void TimelineTest_SelectItem()
        {
            var comp = Context.RenderComponent<TimelineTest>();

            var itemsDiv = comp.FindAll(".mud-timeline-item");

            itemsDiv.Should().HaveCount(5);

            for (var i = 0; i < 5; i++)
            {
                itemsDiv[i].Click();

                comp.Instance.SelectedIndex.Should().Be(i);
            }
        }

        [Test]
        public void TimelineTest_DotStyles()
        {
            var comp = Context.RenderComponent<TimelineTest>();
            var firstItem = comp.FindComponent<MudTimelineItem>();
            comp.Find("div.mud-timeline-item-dot-inner").GetStyle()["background-color"].Should().Be("");

            firstItem.SetParametersAndRender(p =>
            {
                p.Add(t => t.DotStyle, "background-color: #ff0000");
            });

            comp.Find("div.mud-timeline-item-dot-inner").GetStyle()["background-color"].Should().Be("rgba(255, 0, 0, 1)");
        }
    }
}
