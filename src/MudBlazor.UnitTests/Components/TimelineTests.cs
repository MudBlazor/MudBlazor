// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class TimelineTests : BunitTest
    {
        /// <summary>
        /// Default Timeline, with five items.
        /// Testing if selection is sync with move commands.
        /// </summary>
        [Test]
        public async Task TimelineTest()
        {
            var comp = Context.RenderComponent<TimelineTest>();
            // print the generated html
            Console.WriteLine(comp.Markup);
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

    }
}
