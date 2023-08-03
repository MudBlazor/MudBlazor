#pragma warning disable CS1998 // async without await

using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Moq;
using MudBlazor.Services;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DrawerTest : BunitTest
    {
        private BrowserViewportService _browserViewportService;

        public override void Setup()
        {
            base.Setup();
            var jsRuntimeMock = new Mock<IJSRuntime>();
            _browserViewportService = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
            // Initial browser size is Md
            jsRuntimeMock
                .Setup(expression => expression.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<object[]>()))
                .ReturnsAsync(new BrowserWindowSize { Height = 640, Width = 960 })
                .Verifiable();

            Context.Services.AddScoped<IBrowserViewportService>(_ => _browserViewportService);
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
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = _browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;
            await comp.InvokeAsync(async () => await _browserViewportService.RaiseOnResized(new BrowserWindowSize(), point, subscription.JavaScriptListenerId));

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
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = _browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;
            await comp.InvokeAsync(async () => await _browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 1080, Width = 1920 }, Breakpoint.Xl, subscription.JavaScriptListenerId));

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
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = _browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;
            await comp.InvokeAsync(async () => await _browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

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
            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.PreserveOpenState), true));
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = _browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;
            await comp.InvokeAsync(async () => await _browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            //open drawer
            comp.Find("button").Click();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            //resize to small, drawer should close
            await comp.InvokeAsync(async () => await _browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            //resize to large, drawer should open automatically
            await comp.InvokeAsync(async () => await _browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            //close drawer
            comp.Find("button").Click();

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);

            //resize to small, then open drawer
            await comp.InvokeAsync(async () => await _browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 400, Width = 600 }, Breakpoint.Sm, subscription.JavaScriptListenerId));

            comp.Find("button").Click();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll("aside+.mud-drawer-overlay").Count.Should().Be(1);

            //resize to large, drawer should stays open
            await comp.InvokeAsync(async () => await _browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

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
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = _browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;
            await comp.InvokeAsync(async () => await _browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280}, Breakpoint.Lg, subscription.JavaScriptListenerId));

            //open drawer
            comp.Find("button").Click();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            //resize to small, drawer should close
            await comp.InvokeAsync(async () => await _browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 400, Width = 600 }, Breakpoint.Sm, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            //resize to extra small, drawer should close
            await comp.InvokeAsync(async () => await _browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            //resize to large, drawer should open automatically
            await comp.InvokeAsync(async () => await _browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

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
