
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

            await comp.InvokeAsync(() => swipe.Instance._yDown = 50);
            await comp.InvokeAsync(() => swipe.Instance.OnTouchEnd(new TouchEventArgs()));

            await comp.InvokeAsync(() => swipe.Instance.OnTouchCancel(new TouchEventArgs()));
            comp.WaitForAssertion(() => swipe.Instance._xDown.Should().Be(null));

            await comp.InvokeAsync(() => swipe.Instance.OnTouchEnd(new TouchEventArgs()));
            comp.WaitForAssertion(() => swipe.Instance._xDown.Should().Be(null));

        }
    }
}
