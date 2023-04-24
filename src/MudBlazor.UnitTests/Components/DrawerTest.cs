#pragma warning disable CS1998 // async without await

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DrawerTest : BunitTest
    {
        private Mock<IBreakpointService> _breakpointListenerServiceMock;
        private Action<Breakpoint> _breakpointUpdateCallback;

        public override void Setup()
        {
            base.Setup();
            _breakpointListenerServiceMock = new Mock<IBreakpointService>();

            _breakpointListenerServiceMock
                .Setup(x => x.SubscribeAsync(It.IsAny<Action<Breakpoint>>()))
                .ReturnsAsync(new BreakpointServiceSubscribeResult(Guid.NewGuid(), Breakpoint.Md))
                .Callback<Action<Breakpoint>>(x => _breakpointUpdateCallback = x)
                .Verifiable();

            Context.Services.AddScoped(sp => _breakpointListenerServiceMock.Object);
        }

        [Test]
        public async Task TemporaryClosed_Open_CheckOpened_Close_CheckClosed()
        {
            var comp = Context.RenderComponent<DrawerTest1>(Parameter(nameof(DrawerTest1.Variant), DrawerVariant.Temporary));

            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-temporary").Count.Should().Be(1);
            comp.FindAll("aside+.mud-overlay-drawer").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-temporary").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task TemporaryClosedWithoutOverlay_Open_CheckOverlay()
        {
            var comp = Context.RenderComponent<DrawerTest1>(
                Parameter(nameof(DrawerTest1.Variant), DrawerVariant.Temporary),
                Parameter(nameof(DrawerTest1.DisableOverlay), true));

            comp.Find("button").Click();
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task TemporaryClosedClipped_Open_CheckState()
        {
            var comp = Context.RenderComponent<DrawerTest1>(
                Parameter(nameof(DrawerTest1.Variant), DrawerVariant.Temporary),
                Parameter(nameof(DrawerTest1.ClipMode), DrawerClipMode.Always));

            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer-clipped-always").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-temporary").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task PersistentClosed_Open_CheckOpened_Close_CheckClosed()
        {
            var comp = Context.RenderComponent<DrawerTest1>(Parameter(nameof(DrawerTest1.Variant), DrawerVariant.Persistent));

            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-persistent").Count.Should().Be(1);
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-persistent").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task PersistentClosedClipped_Open_CheckState()
        {
            var comp = Context.RenderComponent<DrawerTest1>(
                Parameter(nameof(DrawerTest1.Variant),
                    DrawerVariant.Persistent), Parameter(nameof(DrawerTest1.ClipMode), DrawerClipMode.Always));

            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer-clipped-always").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-persistent").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task MiniClosed_Open_CheckOpened_Close_CheckClosed()
        {
            var comp = Context.RenderComponent<DrawerTest1>(Parameter(nameof(DrawerTest1.Variant), DrawerVariant.Mini));

            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-mini").Count.Should().Be(1);
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-mini").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task MiniClosedClipped_Open_CheckState()
        {
            var comp = Context.RenderComponent<DrawerTest1>(
                Parameter(nameof(DrawerTest1.Variant), DrawerVariant.Mini),
                Parameter(nameof(DrawerTest1.ClipMode), DrawerClipMode.Always));

            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer-clipped-always").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-mini").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task ResponsiveClosed_Open_CheckOpened_Close_CheckClosed()
        {
            var comp = Context.RenderComponent<DrawerResponsiveTest>();

            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        [TestCase(Breakpoint.Xs)]
        [TestCase(Breakpoint.Sm)]
        public async Task ResponsiveSmallClosed_Open_CheckOpenedAndOverlay(Breakpoint point)
        {
            var comp = Context.RenderComponent<DrawerResponsiveTest>();
            await comp.InvokeAsync(() => _breakpointUpdateCallback(point));

            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll("aside+.mud-drawer-overlay").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [TestCase(Breakpoint.Xs)]
        [TestCase(Breakpoint.Sm)]
        [TestCase(Breakpoint.Md)]
        [TestCase(Breakpoint.Lg)]
        [TestCase(Breakpoint.Xl)]
        public async Task ResponsiveClosed_LargeScreen_SetBreakpoint_Open_CheckState(Breakpoint breakpoint)
        {
            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.Breakpoint), breakpoint));
            await comp.InvokeAsync(() => _breakpointUpdateCallback(Breakpoint.Xl));

            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll("aside+.mud-drawer-overlay").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [TestCase(Breakpoint.Xs)]
        [TestCase(Breakpoint.Sm)]
        [TestCase(Breakpoint.Md)]
        [TestCase(Breakpoint.Lg)]
        [TestCase(Breakpoint.Xl)]
        public async Task ResponsiveClosed_SmallScreen_SetBreakpoint_Open_CheckState(Breakpoint breakpoint)
        {
            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.Breakpoint), breakpoint));
            await comp.InvokeAsync(() => _breakpointUpdateCallback(Breakpoint.Xs));

            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll("aside+.mud-drawer-overlay").Count.Should().Be(breakpoint == Breakpoint.Xs ? 0 : 1);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task ResponsiveClosed_ResizeMultiple_CheckStates()
        {
            var srv = Context.Services.GetService<IResizeListenerService>() as MockResizeListenerService;

            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.PreserveOpenState), true));

            await comp.InvokeAsync(() => _breakpointUpdateCallback(Breakpoint.Lg));

            //open drawer
            comp.Find("button").Click();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            //resize to small, drawer should close
            await comp.InvokeAsync(() => _breakpointUpdateCallback(Breakpoint.Xs));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            //resize to large, drawer should open automatically
            await comp.InvokeAsync(() => _breakpointUpdateCallback(Breakpoint.Lg));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            //close drawer
            comp.Find("button").Click();

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);

            //resize to small, then open drawer
            await comp.InvokeAsync(() => _breakpointUpdateCallback(Breakpoint.Sm));

            comp.Find("button").Click();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll("aside+.mud-drawer-overlay").Count.Should().Be(1);

            //resize to large, drawer should stays open
            await comp.InvokeAsync(() => _breakpointUpdateCallback(Breakpoint.Lg));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll("aside+.mud-drawer-overlay").Count.Should().Be(0);
        }

        /// <summary>
        /// Resize screen to small in two steps: first to SM, then to XS. After restoring the original screen size, the drawer should reopen automatically.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Responsive_ResizeToSmall_RestoreToLarge_CheckStates()
        {
            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.PreserveOpenState), true));
            await comp.InvokeAsync(() => _breakpointUpdateCallback(Breakpoint.Lg));

            //open drawer
            comp.Find("button").Click();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            //resize to small, drawer should close
            await comp.InvokeAsync(() => _breakpointUpdateCallback(Breakpoint.Sm));


            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            //resize to extra small, drawer should close
            await comp.InvokeAsync(() => _breakpointUpdateCallback(Breakpoint.Xs));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            //resize to large, drawer should open automatically
            await comp.InvokeAsync(() => _breakpointUpdateCallback(Breakpoint.Lg));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
        }

        [Test]
        public async Task DrawerContainer_RemoveDrawer_CheckStates()
        {
            var comp = Context.RenderComponent<DrawerContainerTest1>();

            comp.FindAll("div.mud-drawer-open-responsive-md-right").Count.Should().Be(1);

            // Remove drawer
            comp.Find("button").Click();

            comp.FindAll("div.mud-drawer-open-responsive-md-right").Count.Should().Be(0);
        }
    }
}
