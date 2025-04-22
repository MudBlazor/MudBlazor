using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.SwipeArea;
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
            await comp.InvokeAsync(() => swipe.Instance.OnPointerUp(new PointerEventArgs()));

            await comp.InvokeAsync(() => swipe.Instance.OnPointerCancel(new PointerEventArgs()));
            comp.WaitForAssertion(() => swipe.Instance._xDown.Should().Be(null));

            await comp.InvokeAsync(() => swipe.Instance.OnPointerUp(new PointerEventArgs()));
            comp.WaitForAssertion(() => swipe.Instance._xDown.Should().Be(null));
        }

        [Test]
        public async Task SwipeTest_2()
        {
            var comp = Context.RenderComponent<SwipeAreaOnSwipeEndTest>();
            var swipe = comp.FindComponent<MudSwipeArea>();

            // Swipe below the sensitivity should not make change.

            await comp.InvokeAsync(() => swipe.Instance.OnPointerDown(new PointerEventArgs { ClientX = 0, ClientY = 0 }));
            await comp.InvokeAsync(() => swipe.Instance.OnPointerUp(new PointerEventArgs { ClientX = 20, ClientY = 20 }));

            comp.WaitForAssertion(() => comp.Instance.SwipeDirection.Should().Be(SwipeDirection.None));
            comp.WaitForAssertion(() => comp.Instance.SwipeDelta.Should().Be(null));

            await comp.InvokeAsync(() => swipe.Instance.OnPointerDown(new PointerEventArgs { ClientX = 0, ClientY = 0 }));
            await comp.InvokeAsync(() => swipe.Instance.OnPointerUp(new PointerEventArgs { ClientX = 150, ClientY = 200 }));
            await comp.InvokeAsync(() => swipe.Instance.OnPointerUp(new PointerEventArgs { ClientX = 100, ClientY = 50 }));

            comp.WaitForAssertion(() => comp.Instance.SwipeDirection.Should().Be(SwipeDirection.TopToBottom));
            comp.WaitForAssertion(() => comp.Instance.SwipeDelta.Should().Be(-200));
        }

        [Test]
        public void SwipeTest_PreventDefault_SetTrue()
        {
            var listenerIds = new int[] { 1, 2, 3 };

            var handler = Context.JSInterop.Setup<int[]>(invocation => invocation.Identifier == "mudElementRef.addDefaultPreventingHandlers")
                .SetResult(listenerIds);

            var comp = Context.RenderComponent<MudSwipeArea>(ComponentParameter.CreateParameter("PreventDefault", true));

            comp.WaitForState(() => comp.Instance.PreventDefault);
            comp.Instance._listenerIds.Should().BeEquivalentTo(listenerIds);

            var invocation = handler.VerifyInvoke("mudElementRef.addDefaultPreventingHandlers");
            invocation.Arguments.Count.Should().Be(2);
            invocation.Arguments[1].Should().BeEquivalentTo(new[] { "onpointerdown", "onpointerup", "onpointercancel" });
        }

        [Test]
        public void SwipeTest_PreventDefault_SetFalse()
        {
            var listenerIds = new int[] { 1, 2, 3 };

            Context.JSInterop.Setup<int[]>(invocation => invocation.Identifier == "mudElementRef.addDefaultPreventingHandlers")
                .SetResult(listenerIds);

            var comp = Context.RenderComponent<MudSwipeArea>(ComponentParameter.CreateParameter("PreventDefault", true));

            var handler = Context.JSInterop.SetupVoid(invocation => invocation.Identifier == "mudElementRef.removeDefaultPreventingHandlers")
                .SetVoidResult();

            comp.SetParam("PreventDefault", false);

            comp.Instance.PreventDefault.Should().Be(false);
            comp.Instance._listenerIds.Should().BeNull();

            var invocation = handler.VerifyInvoke("mudElementRef.removeDefaultPreventingHandlers");
            invocation.Arguments.Count.Should().Be(3);
            invocation.Arguments[1].Should().BeEquivalentTo(new[] { "onpointerdown", "onpointerup", "onpointercancel" });
            invocation.Arguments[2].Should().BeEquivalentTo(listenerIds);
        }
    }
}
