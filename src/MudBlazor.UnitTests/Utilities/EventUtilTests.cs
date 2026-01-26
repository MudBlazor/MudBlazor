// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class EventUtilTests : BunitTest
    {
        [Test]
        public async Task EventUtil_ShouldPreventRenderCycle()
        {
            var comp = Context.Render<EventUtil1Test>();
            comp.Find("#clicks").TrimmedText().Should().Be("Clicks: 0/0/0");
            comp.RenderCount.Should().Be(1);
            // normal click handler causes a re-render automatically (normal Blazor behavior)
            await comp.Find("#btn1").ClickAsync(new MouseEventArgs() { ScreenX = 66, ScreenY = 99 });
            comp.RenderCount.Should().Be(2);
            comp.Find("#clicks").TrimmedText().Should().Be("Clicks: 1/0/0");
            comp.Instance.NumClicks1.Should().Be(1);
            comp.Instance.NumClicks2.Should().Be(0);
            comp.Instance.NumClicks3.Should().Be(0);
            comp.Instance.ScreenX.Should().Be(0);
            comp.Instance.ScreenY.Should().Be(0);
            // now the EventUtil handler with render-suppression (no args)
            await comp.Find("#btn2").ClickAsync(new MouseEventArgs() { ScreenX = 66, ScreenY = 99 });
            comp.RenderCount.Should().Be(2);
            comp.Find("#clicks").TrimmedText().Should().Be("Clicks: 1/0/0"); // no update to the component due render suppression
            comp.Instance.NumClicks1.Should().Be(1);
            comp.Instance.NumClicks2.Should().Be(1);
            comp.Instance.NumClicks3.Should().Be(0);
            comp.Instance.ScreenX.Should().Be(0);
            comp.Instance.ScreenY.Should().Be(0);
            // now the EventUtil handler with render-suppression (with mouse args)
            await comp.Find("#btn3").ClickAsync(new MouseEventArgs() { ScreenX = 17, ScreenY = 27 });
            comp.RenderCount.Should().Be(2);
            comp.Find("#clicks").TrimmedText().Should().Be("Clicks: 1/0/0"); // no update to the component due render suppression
            comp.Instance.NumClicks1.Should().Be(1);
            comp.Instance.NumClicks2.Should().Be(1);
            comp.Instance.NumClicks3.Should().Be(1);
            comp.Instance.ScreenX.Should().Be(17);
            comp.Instance.ScreenY.Should().Be(27);
            // click the first button to re-render
            await comp.Find("#btn1").ClickAsync(new MouseEventArgs() { ScreenX = 66, ScreenY = 99 });
            comp.RenderCount.Should().Be(3);
            comp.Find("#clicks").TrimmedText().Should().Be("Clicks: 2/1/1");
            comp.Instance.ScreenX.Should().Be(17);
            comp.Instance.ScreenY.Should().Be(27);
            // now test the async versions of the utility
            await comp.Find("#btn4").ClickAsync(new MouseEventArgs() { ScreenX = 66, ScreenY = 99 });
            comp.RenderCount.Should().Be(3);
            comp.Instance.NumClicks1.Should().Be(2);
            comp.Instance.NumClicks2.Should().Be(1);
            comp.Instance.NumClicks3.Should().Be(1);
            comp.Instance.NumClicks4.Should().Be(1);
            comp.Instance.NumClicks5.Should().Be(0);
            comp.Instance.ScreenX.Should().Be(17);
            comp.Instance.ScreenY.Should().Be(27);
            await comp.Find("#btn5").ClickAsync(new MouseEventArgs() { ScreenX = 66, ScreenY = 99 });
            comp.RenderCount.Should().Be(3);
            comp.Instance.NumClicks1.Should().Be(2);
            comp.Instance.NumClicks2.Should().Be(1);
            comp.Instance.NumClicks3.Should().Be(1);
            comp.Instance.NumClicks4.Should().Be(1);
            comp.Instance.NumClicks5.Should().Be(1);
            comp.Instance.ScreenX.Should().Be(66);
            comp.Instance.ScreenY.Should().Be(99);
        }
    }
}
