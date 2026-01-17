using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.SwipeArea;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class SwipeTest : BunitTest
    {
        [Test]
        public async Task Swipe_1()
        {
            var comp = Context.Render<SwipeAreaTest>();
            var swipe = comp.FindComponent<MudSwipeArea>();

            await comp.InvokeAsync(() => swipe.Instance._yDown = 50);
            await comp.InvokeAsync(() => swipe.Instance.OnPointerUpAsync(new PointerEventArgs()));

            await comp.InvokeAsync(() => swipe.Instance.OnPointerCancelAsync(new PointerEventArgs()));
            await comp.WaitForAssertionAsync(() => swipe.Instance._xDown.Should().Be(null));

            await comp.InvokeAsync(() => swipe.Instance.OnPointerUpAsync(new PointerEventArgs()));
            await comp.WaitForAssertionAsync(() => swipe.Instance._xDown.Should().Be(null));
        }

        [Test]
        public async Task Swipe_2()
        {
            var comp = Context.Render<SwipeAreaOnSwipeEndTest>();
            var swipe = comp.FindComponent<MudSwipeArea>();

            // Swipe below the sensitivity should not make change.

            await comp.InvokeAsync(() => swipe.Instance.OnPointerDown(new PointerEventArgs { ClientX = 0, ClientY = 0 }));
            await comp.InvokeAsync(() => swipe.Instance.OnPointerUpAsync(new PointerEventArgs { ClientX = 20, ClientY = 20 }));

            await comp.WaitForAssertionAsync(() => comp.Instance.SwipeDirection.Should().Be(SwipeDirection.None));
            await comp.WaitForAssertionAsync(() => comp.Instance.SwipeDelta.Should().Be(null));

            await comp.InvokeAsync(() => swipe.Instance.OnPointerDown(new PointerEventArgs { ClientX = 0, ClientY = 0 }));
            await comp.InvokeAsync(() => swipe.Instance.OnPointerUpAsync(new PointerEventArgs { ClientX = 150, ClientY = 200 }));
            await comp.InvokeAsync(() => swipe.Instance.OnPointerUpAsync(new PointerEventArgs { ClientX = 100, ClientY = 50 }));

            await comp.WaitForAssertionAsync(() => comp.Instance.SwipeDirection.Should().Be(SwipeDirection.TopToBottom));
            await comp.WaitForAssertionAsync(() => comp.Instance.SwipeDelta.Should().Be(-200));
        }

        [Test]
        public void Swipe_PreventDefault_SetTrue()
        {
            var listenerIds = new int[] { 1, 2, 3, 4, 5 };

            var handler = Context.JSInterop.Setup<int[]>(invocation => invocation.Identifier == "mudElementRef.addDefaultPreventingHandlers")
                .SetResult(listenerIds);

            var comp = Context.Render<MudSwipeArea>(parameters => parameters.Add(p => p.PreventDefault, true));

            comp.WaitForState(() => comp.Instance.PreventDefault);
            comp.Instance._listenerIds.Should().BeEquivalentTo(listenerIds);

            var invocation = handler.VerifyInvoke("mudElementRef.addDefaultPreventingHandlers");
            invocation.Arguments.Count.Should().Be(2);
            invocation.Arguments[1].Should().BeEquivalentTo(new[] { "onpointerdown", "onpointerup", "onpointercancel", "onpointermove", "onpointerleave" });
        }

        [Test]
        public async Task Swipe_PreventDefault_SetFalse()
        {
            var listenerIds = new int[] { 1, 2, 3, 4, 5 };

            Context.JSInterop.Setup<int[]>(invocation => invocation.Identifier == "mudElementRef.addDefaultPreventingHandlers")
                .SetResult(listenerIds);

            var comp = Context.Render<MudSwipeArea>(parameters => parameters.Add(p => p.PreventDefault, true));

            var handler = Context.JSInterop.SetupVoid(invocation => invocation.Identifier == "mudElementRef.removeDefaultPreventingHandlers")
                .SetVoidResult();

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.PreventDefault, false));

            comp.Instance.PreventDefault.Should().Be(false);
            comp.Instance._listenerIds.Should().BeNull();

            var invocation = handler.VerifyInvoke("mudElementRef.removeDefaultPreventingHandlers");
            invocation.Arguments.Count.Should().Be(3);
            invocation.Arguments[1].Should().BeEquivalentTo(new[] { "onpointerdown", "onpointerup", "onpointercancel", "onpointermove", "onpointerleave" });
            invocation.Arguments[2].Should().BeEquivalentTo(listenerIds);
        }
    }
}
