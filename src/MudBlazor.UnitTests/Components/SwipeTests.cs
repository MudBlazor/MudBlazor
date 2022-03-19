
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
    public class SwipeTest : BunitTest
    {
        [Test]
        public async Task SwipeTest_1()
        {
            var comp = Context.RenderComponent<SwipeAreaTest>();
            var swipe = comp.FindComponent<MudSwipeArea>();
            TouchPoint[] _point1 = { new() { ClientY = 0, ClientX = 150 } };
            TouchPoint[] _point1reverse = { new() { ClientY = 0, ClientX = -150 } };
            TouchPoint[] _point2 = { new() { ClientY = 20, ClientX = 0 } };
            TouchPoint[] _point3 = { new() { ClientY = 150, ClientX = 0 } };
            TouchPoint[] _point3reverse = { new() { ClientY = -150, ClientX = 0 } };
            TouchEventArgs tea = new TouchEventArgs() { ChangedTouches = _point1 };
            TouchEventArgs tea2 = new TouchEventArgs() { ChangedTouches = _point2 };
            TouchEventArgs tea3 = new TouchEventArgs() { ChangedTouches = _point3 };
            TouchEventArgs teaReverse = new TouchEventArgs() { ChangedTouches = _point1reverse };
            TouchEventArgs tea3Reverse = new TouchEventArgs() { ChangedTouches = _point3reverse };

            await comp.InvokeAsync(() => swipe.Instance._yDown = 50);
            await comp.InvokeAsync(() => swipe.Instance.OnTouchEnd(new TouchEventArgs()));

            await comp.InvokeAsync(() => swipe.Instance.OnTouchCancel(new TouchEventArgs()));
            comp.WaitForAssertion(() => swipe.Instance._xDown.Should().Be(null));

            await comp.InvokeAsync(() => swipe.Instance.OnTouchEnd(new TouchEventArgs()));
            comp.WaitForAssertion(() => swipe.Instance._xDown.Should().Be(null));

            await comp.InvokeAsync(() => swipe.Instance._xDown = 50);
            await comp.InvokeAsync(() => swipe.Instance._yDown = 50);
            await comp.InvokeAsync(() => swipe.Instance.OnTouchEnd(tea));
            await comp.InvokeAsync(() => swipe.Instance._xDown = 50);
            await comp.InvokeAsync(() => swipe.Instance._yDown = 50);
            await comp.InvokeAsync(() => swipe.Instance.OnTouchEnd(teaReverse));

            await comp.InvokeAsync(() => swipe.Instance._xDown = 50);
            await comp.InvokeAsync(() => swipe.Instance._yDown = 50);
            await comp.InvokeAsync(() => swipe.Instance.OnTouchEnd(tea2));

            await comp.InvokeAsync(() => swipe.Instance._xDown = 50);
            await comp.InvokeAsync(() => swipe.Instance._yDown = 50);
            await comp.InvokeAsync(() => swipe.Instance.OnTouchEnd(tea3));
            await comp.InvokeAsync(() => swipe.Instance._xDown = 50);
            await comp.InvokeAsync(() => swipe.Instance._yDown = 50);
            await comp.InvokeAsync(() => swipe.Instance.OnTouchEnd(tea3Reverse));

        }
    }
}
