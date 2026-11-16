using System.ComponentModel;
using AngleSharp.Css.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Moq;
using MudBlazor.Services;
using MudBlazor.UnitTests.TestComponents.Drawer;
using NUnit.Framework;

#nullable enable
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DrawerTest : BunitTest
    {
        private static BrowserViewportService GetBrowserViewportService(BrowserWindowSize browserWindowSize)
        {
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var browserViewportService = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
            // Sets the initial browser size aka simulating the windows size when the website was opened for the first time
            jsRuntimeMock
                .Setup(expression => expression.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<CancellationToken>(), It.IsAny<object[]>()))
                .ReturnsAsync(browserWindowSize)
                .Verifiable();

            return browserViewportService;
        }

        private BrowserViewportService AddBrowserViewportService(BrowserWindowSize browserWindowSize)
        {
            var service = GetBrowserViewportService(browserWindowSize);

            Context.Services.AddScoped<IBrowserViewportService>(_ => service);

            return service;
        }

        private BrowserViewportService AddBrowserViewportService(int height = 640, int width = 960) => AddBrowserViewportService(new BrowserWindowSize { Height = height, Width = width });

        private static BrowserWindowSize BreakpointBrowserAssociatedSize(Breakpoint breakpoint)
        {
            return breakpoint switch
            {
                Breakpoint.Xs or Breakpoint.None => new BrowserWindowSize { Height = 0, Width = 0 },
                Breakpoint.Sm or Breakpoint.SmAndDown or Breakpoint.SmAndUp => new BrowserWindowSize { Height = 400, Width = 600 },
                Breakpoint.Md or Breakpoint.MdAndDown or Breakpoint.MdAndUp => new BrowserWindowSize { Height = 640, Width = 960 },
                Breakpoint.Lg or Breakpoint.LgAndDown or Breakpoint.LgAndUp => new BrowserWindowSize { Height = 720, Width = 1280 },
                Breakpoint.Xl or Breakpoint.XlAndDown or Breakpoint.XlAndUp => new BrowserWindowSize { Height = 1080, Width = 1920 },
                Breakpoint.Xxl or Breakpoint.Always => new BrowserWindowSize { Height = 1440, Width = 2560 },
                _ => throw new InvalidEnumArgumentException("Not acceptable breakpoint")
            };
        }

        [Test]
        public async Task TemporaryClosed_Open_CheckOpened_Close_CheckClosedAsync()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerTest1>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Temporary));

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-temporary").Count.Should().Be(1);
            comp.FindAll(".mud-overlay-drawer").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-temporary").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Temporary_OverlayAutoClose(bool overlayAutoClose)
        {
            var comp = Context.Render<DrawerTest1>(parameters => parameters
                .Add(parameter => parameter.Variant, DrawerVariant.Temporary)
                .Add(parameter => parameter.OverlayAutoClose, overlayAutoClose));

            // Open the drawer
            await comp.Find("#toggle-drawer-button").ClickAsync();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-temporary").Count.Should().Be(1);
            comp.FindAll(".mud-overlay-drawer").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Clicking on the overlay
            await comp.Find("div.mud-overlay").ClickAsync(new MouseEventArgs());

            if (overlayAutoClose)
            {
                // Drawer should close
                comp.FindAll("aside.mud-drawer--open.mud-drawer-temporary").Count.Should().Be(0);
                comp.FindAll("aside.mud-drawer--closed.mud-drawer-temporary").Count.Should().Be(1);
                comp.FindAll(".mud-overlay-drawer").Count.Should().Be(0);
                comp.Instance.Drawer.Open.Should().BeFalse();
            }
            else
            {
                // Drawer should stay open
                comp.FindAll("aside.mud-drawer--open.mud-drawer-temporary").Count.Should().Be(1);
                comp.FindAll("aside.mud-drawer--closed.mud-drawer-temporary").Count.Should().Be(0);
                comp.FindAll(".mud-overlay-drawer").Count.Should().Be(1);
                comp.Instance.Drawer.Open.Should().BeTrue();
            }
        }

        [Test]
        public async Task TemporaryClosedWithoutOverlay_Open_CheckOverlayAsync()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerTest1>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Temporary)
                .Add(x => x.Overlay, false));

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task TemporaryClosedClipped_Open_CheckStateAsync()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerTest1>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Temporary)
                .Add(x => x.ClipMode, DrawerClipMode.Always));

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer-clipped-always").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-temporary").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task PersistentClosed_Open_CheckOpened_Close_CheckClosedAsync()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerTest1>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Persistent));

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-persistent").Count.Should().Be(1);
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-persistent").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task PersistentClosedClipped_Open_CheckStateAsync()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerTest1>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Persistent)
                .Add(x => x.ClipMode, DrawerClipMode.Always));

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer-clipped-always").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-persistent").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task MiniClosed_Open_CheckOpened_Close_CheckClosedAsync()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerTest1>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini));

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-mini").Count.Should().Be(1);
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-mini").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task MiniClosedClipped_Open_CheckStateAsync()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerTest1>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini)
                .Add(x => x.ClipMode, DrawerClipMode.Always));

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer-clipped-always").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-mini").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task MiniSmallScreen_UsesTemporaryVariantClassesAndOverlay()
        {
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Xs));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-temporary").Count.Should().Be(1);
            comp.Find("div.mud-layout").ClassName.Should().Contain("mud-drawer-close-temporary-left");
            comp.Find("div.mud-layout").ClassName.Should().NotContain("mud-drawer-close-mini");

            await comp.Find("#toggle-drawer-button").ClickAsync();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-temporary").Count.Should().Be(1);
            comp.FindAll(".mud-drawer-overlay").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
        }

        [Test]
        public async Task MiniSmallScreen_NavigationClosesDrawer()
        {
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Xs));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini));

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.Instance.Drawer.Open.Should().BeTrue();

            await comp.InvokeAsync(() => ((MudBlazor.Interfaces.INavigationEventReceiver)comp.Instance.Drawer).OnNavigation());

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-temporary").Count.Should().Be(1);
            comp.FindAll(".mud-drawer-overlay").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        [TestCase(Breakpoint.None)]
        [TestCase(Breakpoint.Always)]
        public async Task MiniSentinelBreakpoint_StaysMiniOnLargeScreen(Breakpoint breakpoint)
        {
            // Breakpoint.None and Breakpoint.Always are sentinels, not real widths, so a mini drawer using them must
            // keep its mini behavior on every screen size instead of degrading to a temporary overlay drawer.
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini)
                .Add(x => x.Breakpoint, breakpoint));

            comp.FindAll("aside.mud-drawer-mini").Count.Should().Be(1);
            comp.FindAll("aside.mud-drawer-temporary").Count.Should().Be(0);
            comp.FindAll(".mud-drawer-overlay").Count.Should().Be(0);

            // Toggling forces the drawer to re-render after the viewport breakpoint has been resolved; a sentinel
            // breakpoint must never flip a mini drawer into temporary mode.
            await comp.Find("#toggle-drawer-button").ClickAsync();

            comp.FindAll("aside.mud-drawer-mini").Count.Should().Be(1);
            comp.FindAll("aside.mud-drawer-temporary").Count.Should().Be(0);
        }

        [Test]
        [TestCase(Breakpoint.Lg)]
        [TestCase(Breakpoint.Xl)]
        public async Task MiniLargeScreen_StaysMiniRailWithoutOverlay(Breakpoint viewport)
        {
            // Above its breakpoint a mini drawer stays a docked rail with no overlay and keeps the content offset.
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(viewport));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini));

            comp.FindAll("aside.mud-drawer-mini").Count.Should().Be(1);
            comp.Find("div.mud-layout").ClassName.Should().Contain("mini-md-left");

            await comp.Find("#toggle-drawer-button").ClickAsync();

            // Opening a desktop mini drawer keeps it a mini rail and never shows an overlay.
            comp.FindAll("aside.mud-drawer--open.mud-drawer-mini").Count.Should().Be(1);
            comp.FindAll(".mud-drawer-overlay").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
        }

        [Test]
        public async Task MiniLargeScreen_OpenMiniOnHover_OpensAndCloses()
        {
            // On a large screen the mini drawer is still a real mini drawer, so hover-to-expand keeps working.
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini)
                .Add(x => x.OpenMiniOnHover, true));

            comp.Instance.Drawer.Open.Should().BeFalse();

            await comp.Find("aside.mud-drawer").TriggerEventAsync("onpointerenter", new PointerEventArgs());
            comp.Instance.Drawer.Open.Should().BeTrue();

            await comp.Find("aside.mud-drawer").TriggerEventAsync("onpointerleave", new PointerEventArgs());
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public void MiniSmallScreen_OpenMiniOnHover_DoesNotWireHover()
        {
            // Below the breakpoint the drawer behaves like Temporary, so hover must not open it - and the handler is not wired at all, which on Blazor Server is what keeps the pointer off the circuit.
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Xs));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini)
                .Add(x => x.OpenMiniOnHover, true));

            comp.Instance.Drawer.Open.Should().BeFalse();

            var aside = comp.Find("aside.mud-drawer");
            Assert.ThrowsAsync<MissingEventHandlerException>(async () =>
                await aside.TriggerEventAsync("onpointerenter", new PointerEventArgs()));
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        [TestCase(DrawerVariant.Responsive)]
        [TestCase(DrawerVariant.Persistent)]
        [TestCase(DrawerVariant.Temporary)]
        [TestCase(DrawerVariant.Mini)]
        public void HoverHandlersAreNotWiredWhenHoverCannotOpenTheDrawer(DrawerVariant variant)
        {
            // Nothing but a mini drawer with OpenMiniOnHover can act on these, and a handler that is registered anyway costs a Blazor Server round-trip on every pointer enter and leave of the drawer.
            // DrawerVariant.Mini is included without OpenMiniOnHover, which is the default.
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Variant, variant));

            var aside = comp.Find("aside.mud-drawer");

            Assert.ThrowsAsync<MissingEventHandlerException>(async () =>
                await aside.TriggerEventAsync("onpointerenter", new PointerEventArgs()));
            Assert.ThrowsAsync<MissingEventHandlerException>(async () =>
                await aside.TriggerEventAsync("onpointerleave", new PointerEventArgs()));
        }

        [Test]
        public async Task Mini_ResizeAcrossBreakpoint_SwitchesVariantClasses()
        {
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini));
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;

            // Large screen: mini rail with the mini content offset. (The open/close prefix depends on auto-open
            // timing, so assert on the variant token only.)
            comp.FindAll("aside.mud-drawer-mini").Count.Should().Be(1);
            comp.Find("div.mud-layout").ClassName.Should().Contain("mini-md-left");
            comp.Find("div.mud-layout").ClassName.Should().NotContain("temporary");

            // Resize below the breakpoint: the drawer becomes temporary and the mini offset is dropped.
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer-temporary").Count.Should().Be(1);
            comp.FindAll("aside.mud-drawer-mini").Count.Should().Be(0);
            comp.Find("div.mud-layout").ClassName.Should().Contain("temporary-left");
            comp.Find("div.mud-layout").ClassName.Should().NotContain("mini");

            // Resize back above the breakpoint: the drawer returns to the mini rail.
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer-mini").Count.Should().Be(1);
            comp.FindAll("aside.mud-drawer-temporary").Count.Should().Be(0);
            comp.Find("div.mud-layout").ClassName.Should().Contain("mini-md-left");
            comp.Find("div.mud-layout").ClassName.Should().NotContain("temporary");
        }

        [Test]
        public async Task MiniLargeScreen_NavigationDoesNotCloseDesktopRail()
        {
            // The "And-up" aliases normalize to a real breakpoint; navigating on a desktop mini rail must not close it.
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini)
                .Add(x => x.Breakpoint, Breakpoint.MdAndUp));

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-mini").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            await comp.InvokeAsync(() => ((MudBlazor.Interfaces.INavigationEventReceiver)comp.Instance.Drawer).OnNavigation());

            comp.FindAll("aside.mud-drawer--open.mud-drawer-mini").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
        }

        [Test]
        public async Task ResponsiveClosed_Open_CheckOpened_Close_CheckClosedAsync()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerResponsiveTest>();

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        [TestCase(Breakpoint.Xs)]
        [TestCase(Breakpoint.Sm)]
        public async Task ResponsiveSmallClosed_Open_CheckOpenedAndOverlayAsync(Breakpoint point)
        {
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(point));
            var comp = Context.Render<DrawerResponsiveTest>();

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll(".mud-drawer-overlay").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [TestCase(Breakpoint.SmAndDown, "sm")]
        [TestCase(Breakpoint.SmAndUp, "sm")]
        [TestCase(Breakpoint.MdAndDown, "md")]
        [TestCase(Breakpoint.MdAndUp, "md")]
        [TestCase(Breakpoint.LgAndDown, "lg")]
        [TestCase(Breakpoint.LgAndUp, "lg")]
        [TestCase(Breakpoint.XlAndDown, "xl")]
        [TestCase(Breakpoint.XlAndUp, "xl")]
        public async Task CompositeBreakpoint_UsesNormalizedCssClasses(Breakpoint breakpoint, string normalizedBreakpoint)
        {
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Xs));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Breakpoint, breakpoint));

            await comp.Find("#toggle-drawer-button").ClickAsync();

            comp.Find("aside.mud-drawer").ClassList.Should().Contain($"mud-drawer-{normalizedBreakpoint}");
            comp.Find("div.mud-layout").ClassList.Should().Contain($"mud-drawer-open-responsive-{normalizedBreakpoint}-left");
            comp.Find(".mud-drawer-overlay").ClassList.Should().Contain($"mud-drawer-overlay-{normalizedBreakpoint}");
        }

        [TestCase(Breakpoint.Xs)]
        [TestCase(Breakpoint.Sm)]
        [TestCase(Breakpoint.SmAndDown)]
        [TestCase(Breakpoint.SmAndUp)]
        [TestCase(Breakpoint.Md)]
        [TestCase(Breakpoint.MdAndDown)]
        [TestCase(Breakpoint.MdAndUp)]
        [TestCase(Breakpoint.Lg)]
        [TestCase(Breakpoint.LgAndDown)]
        [TestCase(Breakpoint.LgAndUp)]
        [TestCase(Breakpoint.Xl)]
        [TestCase(Breakpoint.XlAndDown)]
        [TestCase(Breakpoint.XlAndUp)]
        public async Task ResponsiveClosed_StartLargeScreen_SetBreakpoint_Open_CheckStateAsync(Breakpoint breakpoint)
        {
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Xl));
            var providerComp = Context.Render<MudPopoverProvider>();
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Breakpoint, breakpoint));

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll(".mud-drawer-overlay").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [TestCase(Breakpoint.Xs)]
        [TestCase(Breakpoint.Sm)]
        [TestCase(Breakpoint.SmAndDown)]
        [TestCase(Breakpoint.SmAndUp)]
        [TestCase(Breakpoint.Md)]
        [TestCase(Breakpoint.MdAndDown)]
        [TestCase(Breakpoint.MdAndUp)]
        [TestCase(Breakpoint.Lg)]
        [TestCase(Breakpoint.LgAndDown)]
        [TestCase(Breakpoint.LgAndUp)]
        [TestCase(Breakpoint.Xl)]
        [TestCase(Breakpoint.XlAndDown)]
        [TestCase(Breakpoint.XlAndUp)]
        public async Task ResponsiveClosed_StartSmallScreen_SetBreakpoint_Open_CheckStateAsync(Breakpoint breakpoint)
        {
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Xs));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Breakpoint, breakpoint));

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll(".mud-drawer-overlay").Count.Should().Be(breakpoint == Breakpoint.Xs ? 0 : 1);
            comp.Instance.Drawer.Open.Should().BeTrue();
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task ResponsiveClosed_ResizeMultiple_CheckStates()
        {
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerResponsiveTest>();
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;

            // Open drawer
            await comp.Find("#toggle-drawer-button").ClickAsync();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Resize to small, drawer should close
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to large, drawer should open automatically
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Close drawer
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.Instance.Drawer.Open.Should().BeFalse();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);

            // Resize to small, then open drawer
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 400, Width = 600 }, Breakpoint.Sm, subscription.JavaScriptListenerId));

            // Open drawer
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll(".mud-drawer-overlay").Count.Should().Be(1);

            // Resize to large, drawer should stays open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll(".mud-drawer-overlay").Count.Should().Be(0);
        }

        /// <summary>
        /// Resize screen to small in two steps: first to SM, then to XS. After restoring the original screen size, the drawer should reopen automatically.
        /// </summary>
        [Test]
        public async Task Responsive_ResizeToSmall_RestoreToLarge_CheckStates()
        {
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerResponsiveTest>();
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;

            // Open drawer
            await comp.Find("#toggle-drawer-button").ClickAsync();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Resize to small, drawer should close
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 400, Width = 600 }, Breakpoint.Sm, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to extra small, drawer should close
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to large, drawer should open automatically
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
        }

        /// <summary>
        /// Resize screen from small to big. Once the screen is large enough, the drawer should open automatically.
        /// </summary>
        [Test]
        public async Task Responsive_ResizeFromSmall_ToLarge_CheckStates()
        {
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Xs));
            var comp = Context.Render<DrawerResponsiveTest>();
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;

            // Drawer should be closed
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to small, drawer should stay closed
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 400, Width = 600 }, Breakpoint.Sm, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize above breakpoint - drawer should open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
        }

        [Test]
        public async Task Responsive_AlwaysOpen_BreakpointAlways()
        {
            var breakpoint = Breakpoint.Always;
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(breakpoint));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Breakpoint, breakpoint));
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;

            // Initial state
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Resize to small, drawer should be open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Resize to large, drawer should be open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Resize to extra extra large, drawer should be open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 1440, Width = 2560 }, Breakpoint.Xxl, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Resize to large, drawer should be open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Resize to small, drawer should be open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Close drawer manually to check if it opens again
            await comp.Find("#toggle-drawer-button").ClickAsync();

            // Resize to small, drawer should be open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Resize to large, drawer should be open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Resize to extra extra large, drawer should be open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 1440, Width = 2560 }, Breakpoint.Xxl, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Resize to large, drawer should be open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            // Resize to small, drawer should be open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
        }

        [Test]
        public async Task Responsive_AlwaysClose_BreakpointNone()
        {
            var breakpoint = Breakpoint.None;
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(breakpoint));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Breakpoint, breakpoint));
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;

            // Initial state
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to small, drawer should be closed
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to large, drawer should be closed
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to extra extra large, drawer should be closed
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 1440, Width = 2560 }, Breakpoint.Xxl, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to large, drawer should be closed
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to small, drawer should be closed
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Open drawer manually to check if it closes again
            await comp.Find("#toggle-drawer-button").ClickAsync();

            // Resize to small, drawer should be closed
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to large, drawer should be closed
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to extra extra large, drawer should be closed
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 1440, Width = 2560 }, Breakpoint.Xxl, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to large, drawer should be closed
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            // Resize to small, drawer should be closed
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task DrawerContainer_RemoveDrawer_CheckStatesAsync()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerContainerTest1>();

            comp.FindAll("div.mud-drawer-open-responsive-md-right").Count.Should().Be(1);

            // Remove drawer
            await comp.Find("#hide-drawer-button").ClickAsync();

            comp.FindAll("div.mud-drawer-open-responsive-md-right").Count.Should().Be(0);
        }

        [Test]
        public void PersistentWithoutBreakpointCallback_DoesNotSubscribeToViewport()
        {
            var browserViewportService = AddBrowserViewportService();
            var comp = Context.Render<DrawerTest1>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Persistent));

            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance).Should().BeNull();
        }

        [Test]
        public async Task PersistentWithBreakpointCallback_SubscribesAndRaisesCallback()
        {
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerBreakpointEventTest>();
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance);

            subscription.Should().NotBeNull();
            comp.Instance.LastBreakpoint.Should().Be(Breakpoint.Lg);
            comp.Instance.BreakpointNotifications.Should().Be(1);

            await comp.InvokeAsync(async () =>
                await browserViewportService.RaiseOnResized(
                    new BrowserWindowSize { Height = 0, Width = 0 },
                    Breakpoint.Xs,
                    subscription!.JavaScriptListenerId));

            comp.Instance.LastBreakpoint.Should().Be(Breakpoint.Xs);
            comp.Instance.BreakpointNotifications.Should().Be(2);
        }

        /// <summary>
        /// A breakpoint callback assigned after a responsive drawer subscribed receives the current breakpoint exactly once (#11541).
        /// </summary>
        [Test]
        public async Task ResponsiveBreakpointCallbackAddedLater_ReceivesCurrentBreakpointOnce()
        {
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerBreakpointEventTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Responsive)
                .Add(x => x.ObserveBreakpoint, false));
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();

            browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance).Should().NotBeNull();
            comp.Instance.BreakpointNotifications.Should().Be(0);

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.ObserveBreakpoint, true));

            comp.Instance.LastBreakpoint.Should().Be(Breakpoint.Lg);
            comp.Instance.BreakpointNotifications.Should().Be(1);

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.ObserveBreakpoint, true));

            comp.Instance.BreakpointNotifications.Should().Be(1);
        }

        /// <summary>
        /// Removing the breakpoint callback from a persistent drawer clears its viewport subscription (#11541).
        /// </summary>
        [Test]
        public async Task PersistentBreakpointCallbackRemoved_UnsubscribesFromViewport()
        {
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerBreakpointEventTest>();
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();

            browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance).Should().NotBeNull();

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.ObserveBreakpoint, false));

            browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance).Should().BeNull();
        }

        /// <summary>
        /// Switching a responsive drawer without a breakpoint callback to persistent clears its viewport subscription (#11541).
        /// </summary>
        [Test]
        public async Task ResponsiveWithoutBreakpointCallbackSwitchedToPersistent_UnsubscribesFromViewport()
        {
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerBreakpointEventTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Responsive)
                .Add(x => x.ObserveBreakpoint, false));
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();

            browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance).Should().NotBeNull();

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Persistent));

            browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance).Should().BeNull();
        }

        /// <summary>
        /// A drawer removed by its breakpoint callback during the immediate viewport notification does not stay subscribed (#11541).
        /// </summary>
        [Test]
        public void BreakpointCallbackRemovesDrawerDuringSubscription_UnsubscribesFromViewport()
        {
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerBreakpointEventTest>(parameters => parameters
                .Add(x => x.RemoveDrawerOnBreakpoint, true));

            comp.Instance.BreakpointNotifications.Should().Be(1);
            comp.FindComponents<MudDrawer>().Should().BeEmpty();
            comp.WaitForAssertion(() => browserViewportService.ObserversCount.Should().Be(0));
        }

        /// <summary>
        /// A breakpoint callback that closes a responsive drawer is not overridden by the drawer opening itself above its breakpoint (#11541).
        /// </summary>
        [Test]
        public async Task ResponsiveBreakpointCallbackClosesDrawer_IsNotOverriddenByResponsiveOpen()
        {
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Xs));
            var comp = Context.Render<DrawerBreakpointEventTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Responsive)
                .Add(x => x.CloseOnBreakpoint, Breakpoint.Lg));
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;

            comp.FindAll("aside.mud-drawer--closed").Count.Should().Be(1);

            await comp.InvokeAsync(async () =>
                await browserViewportService.RaiseOnResized(
                    new BrowserWindowSize { Height = 720, Width = 1280 },
                    Breakpoint.Lg,
                    subscription.JavaScriptListenerId));

            comp.Instance.LastBreakpoint.Should().Be(Breakpoint.Lg);
            comp.FindAll("aside.mud-drawer--closed").Count.Should().Be(1);
            comp.FindAll("aside.mud-drawer--open").Count.Should().Be(0);
        }

        [Test, Combinatorial]
        public async Task NonResponsiveKeepInitialOpen_AllBreakpointsAsync(
            [Values(
                Breakpoint.None,
                Breakpoint.Xs,
                Breakpoint.Sm,
                Breakpoint.SmAndDown,
                Breakpoint.SmAndUp,
                Breakpoint.Md,
                Breakpoint.MdAndDown,
                Breakpoint.MdAndUp,
                Breakpoint.Lg,
                Breakpoint.LgAndDown,
                Breakpoint.LgAndUp,
                Breakpoint.Xl,
                Breakpoint.XlAndDown,
                Breakpoint.XlAndUp,
                Breakpoint.Always
            )] Breakpoint breakpoint,
            [Values(
                true,
                false
            )] bool initialState)
        {
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(breakpoint));
            var comp = Context.Render<DrawerNonResponsiveTest>(parameters => parameters
                .Add(x => x.InitialOpenState, initialState));

            var expectedDrawerCount = initialState ? 1 : 0;

            comp.FindAll("aside.mud-drawer--open.mud-drawer-temporary").Count.Should().Be(expectedDrawerCount);
            comp.FindAll(".mud-drawer-overlay").Count.Should().Be(expectedDrawerCount);
            comp.Instance.Drawer.Open.Should().Be(initialState);

            // Make sure that we can toggle the drawer without issues
            await comp.Find("#toggle-drawer-button").ClickAsync();

            var expectedToggledDrawerCount = initialState ? 0 : 1;

            comp.FindAll("aside.mud-drawer--open.mud-drawer-temporary").Count.Should().Be(expectedToggledDrawerCount);
            comp.FindAll(".mud-drawer-overlay").Count.Should().Be(expectedToggledDrawerCount);
            comp.Instance.Drawer.Open.Should().Be(!initialState);
        }

        [Test]
        public void DrawerPersistentTop_HeightTest()
        {
            var drawerHeight = "300px";
            var comp = Context.Render<DrawerPersistentTest>(parameters => parameters
                .Add(x => x.Anchor, Anchor.Top)
                .Add(x => x.DrawerHeight, drawerHeight));

            var asideDrawer = comp.Find("aside.mud-drawer");
            var styles = asideDrawer.GetStyle().ToList();
            styles.Single(a => a.Name == "--mud-drawer-height").Value.Should().Be(drawerHeight);
        }

        /// <summary>
        /// Test for issue #6791: Verifies that changing <see cref="MudDrawer.Width"/> updates the container's width variable without a further render.
        /// </summary>
        [Test]
        [TestCase(Anchor.Start, "--mud-drawer-width-left")]
        [TestCase(Anchor.End, "--mud-drawer-width-right")]
        public async Task DrawerContainer_WidthChanged_UpdatesContainerVariable(Anchor anchor, string variable)
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerContainerSizeTest>(parameters => parameters
                .Add(x => x.Anchor, anchor)
                .Add(x => x.Width, "150px"));

            comp.Find("#drawer-container").GetStyle().Single(a => a.Name == variable).Value.Should().Be("150px");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Width, "250px"));

            comp.Find("#drawer-container").GetStyle().Single(a => a.Name == variable).Value.Should().Be("250px");
        }

        /// <summary>
        /// Test for issue #6791: Verifies that changing <see cref="MudDrawer.MiniWidth"/> updates the container's mini width variable without a further render.
        /// </summary>
        [Test]
        public async Task DrawerContainer_MiniWidthChanged_UpdatesContainerVariable()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerContainerSizeTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini)
                .Add(x => x.MiniWidth, "56px"));

            comp.Find("#drawer-container").GetStyle().Single(a => a.Name == "--mud-drawer-width-mini-left").Value.Should().Be("56px");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.MiniWidth, "72px"));

            comp.Find("#drawer-container").GetStyle().Single(a => a.Name == "--mud-drawer-width-mini-left").Value.Should().Be("72px");
        }

        /// <summary>
        /// Test for issue #6791: Verifies that changing <see cref="MudDrawer.Height"/> updates the container's height variable without a further render.
        /// </summary>
        [Test]
        public async Task DrawerContainer_HeightChanged_UpdatesContainerVariable()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerContainerSizeTest>(parameters => parameters
                .Add(x => x.Anchor, Anchor.Top)
                .Add(x => x.Height, "150px"));

            comp.Find("#drawer-container").GetStyle().Single(a => a.Name == "--mud-drawer-height-top").Value.Should().Be("150px");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Height, "250px"));

            comp.Find("#drawer-container").GetStyle().Single(a => a.Name == "--mud-drawer-height-top").Value.Should().Be("250px");
        }

        /// <summary>
        /// Test for issue #6791: Verifies that a fixed drawer inside a <see cref="MudLayout"/>, where the layout is the only source of the width, updates that width when <see cref="MudDrawer.Width"/> changes.
        /// </summary>
        [Test]
        public async Task DrawerLayout_FixedDrawerWidthChanged_UpdatesLayoutVariable()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerLayoutSizeTest>(parameters => parameters
                .Add(x => x.Width, "150px"));

            // A fixed non-temporary drawer emits no width of its own, so the layout carries the only width.
            comp.Find("aside.mud-drawer").GetStyle().ToList().Should().NotContain(a => a.Name == "--mud-drawer-width");
            comp.Find("#drawer-layout").GetStyle().Single(a => a.Name == "--mud-drawer-width-left").Value.Should().Be("150px");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Width, "250px"));

            comp.Find("#drawer-layout").GetStyle().Single(a => a.Name == "--mud-drawer-width-left").Value.Should().Be("250px");
        }

        /// <summary>
        /// Test for issue #6791: Verifies that repeatedly changing <see cref="MudDrawer.Width"/> costs the container a bounded number of renders instead of looping.
        /// </summary>
        [Test]
        public async Task DrawerContainer_WidthChanged_RendersContainerBoundedNumberOfTimes()
        {
            // A registered-parameter change on the drawer costs the container 5 renders, and a set that changes nothing costs 2.
            // These bounds leave room for renderer batching to shift a little and only have to catch the drawer and the container re-rendering each other without settling.
            const int MaxRendersPerChange = 8;
            const int MaxRendersPerNoOp = 4;

            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerContainerSizeTest>(parameters => parameters
                .Add(x => x.Width, "150px"));
            var container = comp.FindComponent<MudDrawerContainer>();

            foreach (var width in new[] { "250px", "350px", "450px" })
            {
                var before = container.RenderCount;
                await comp.SetParametersAndRenderAsync(parameters => parameters
                    .Add(x => x.Width, width));

                (container.RenderCount - before).Should().BePositive().And.BeLessThanOrEqualTo(MaxRendersPerChange);
            }

            var beforeNoOp = container.RenderCount;
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Width, "450px"));

            (container.RenderCount - beforeNoOp).Should().BeLessThanOrEqualTo(MaxRendersPerNoOp);
        }

        /// <summary>
        /// Test for issue #3378: Verifies that the mud-drawer--initial class is removed after first interaction.
        /// This class is used to skip the initial CSS transition when the drawer first renders.
        /// </summary>
        [Test]
        public async Task DrawerInTabs_ShouldRemoveInitialClassAfterFirstInteractionAsync()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerInTabsTest>();

            // Drawer should be closed initially
            comp.FindAll("aside.mud-drawer--closed").Count.Should().Be(1);

            // Open the drawer
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--open").Count.Should().Be(1);

            // Close the drawer
            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.FindAll("aside.mud-drawer--closed").Count.Should().Be(1);

            // Verify the drawer loses the initial class after first interaction
            // This ensures the CSS transition will be applied (not skipped)
            comp.FindAll("aside.mud-drawer--initial").Count.Should().Be(0);
        }

        /// <summary>
        /// A closed off-screen drawer is inert until opened.
        /// </summary>
        [Test]
        [TestCase(DrawerVariant.Temporary)]
        [TestCase(DrawerVariant.Persistent)]
        [TestCase(DrawerVariant.Responsive)]
        public async Task ClosedDrawer_IsInertUntilOpened(DrawerVariant variant)
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<DrawerTest1>(parameters => parameters
                .Add(x => x.Variant, variant));

            comp.Find("aside.mud-drawer--closed").HasAttribute("inert").Should().BeTrue();

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.Find("aside.mud-drawer--open").HasAttribute("inert").Should().BeFalse();

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.Find("aside.mud-drawer--closed").HasAttribute("inert").Should().BeTrue();
        }

        /// <summary>
        /// A closed mini drawer docked above its breakpoint keeps its visible icon rail interactive.
        /// </summary>
        [Test]
        public async Task MiniLargeScreen_ClosedRailIsNotInert()
        {
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini));

            comp.Find("aside.mud-drawer--closed.mud-drawer-mini").HasAttribute("inert").Should().BeFalse();

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.Find("aside.mud-drawer--open.mud-drawer-mini").HasAttribute("inert").Should().BeFalse();
        }

        /// <summary>
        /// A closed mini drawer below its breakpoint renders as a temporary drawer and is inert until opened.
        /// </summary>
        [Test]
        public async Task MiniSmallScreen_ClosedDrawerIsInertUntilOpened()
        {
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Xs));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini));

            comp.Find("aside.mud-drawer--closed.mud-drawer-temporary").HasAttribute("inert").Should().BeTrue();

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.Find("aside.mud-drawer--open.mud-drawer-temporary").HasAttribute("inert").Should().BeFalse();
        }

        /// <summary>
        /// Resizing a mini drawer across its breakpoint makes it inert only while it renders as a closed temporary drawer.
        /// </summary>
        [Test]
        public async Task Mini_ResizeAcrossBreakpoint_TogglesInert()
        {
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.Render<DrawerResponsiveTest>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Mini));
            var subscription = browserViewportService.GetInternalSubscription(comp.FindComponent<MudDrawer>().Instance)!;

            comp.Find("aside.mud-drawer--closed.mud-drawer-mini").HasAttribute("inert").Should().BeFalse();

            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 0, Width = 0 }, Breakpoint.Xs, subscription.JavaScriptListenerId));
            comp.Find("aside.mud-drawer--closed.mud-drawer-temporary").HasAttribute("inert").Should().BeTrue();

            // Returning above the breakpoint opens the mini drawer, so close it to check the docked rail again.
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));
            comp.Find("aside.mud-drawer--open.mud-drawer-mini").HasAttribute("inert").Should().BeFalse();

            await comp.Find("#toggle-drawer-button").ClickAsync();
            comp.Find("aside.mud-drawer--closed.mud-drawer-mini").HasAttribute("inert").Should().BeFalse();
        }

        /// <summary>
        /// A caller's own <c>inert</c> in <see cref="MudComponentBase.UserAttributes"/> overrides the generated value and renders once.
        /// </summary>
        [Test]
        public void UserAttributesInert_OverridesGeneratedValue()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<MudDrawer>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Persistent)
                .Add(x => x.Open, true)
                .AddUnmatched("inert", true));

            var aside = comp.Find("aside.mud-drawer--open");
            aside.HasAttribute("inert").Should().BeTrue();
            aside.Attributes.Count(a => a.Name == "inert").Should().Be(1);
        }

        /// <summary>
        /// A caller's boolean false <c>inert</c> removes the attribute from a closed off-screen drawer.
        /// </summary>
        [Test]
        public void UserAttributesInertFalse_OptsOutOfGeneratedValue()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<MudDrawer>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Persistent)
                .AddUnmatched("inert", false));

            comp.Find("aside.mud-drawer--closed").HasAttribute("inert").Should().BeFalse();
        }

        /// <summary>
        /// The string "false" is not the boolean opt-out, so a closed drawer still renders the <c>inert</c> attribute.
        /// </summary>
        [Test]
        public void UserAttributesInertStringFalse_StillRendersAttribute()
        {
            _ = AddBrowserViewportService();
            var comp = Context.Render<MudDrawer>(parameters => parameters
                .Add(x => x.Variant, DrawerVariant.Persistent)
                .AddUnmatched("inert", "false"));

            var aside = comp.Find("aside.mud-drawer--closed");
            aside.HasAttribute("inert").Should().BeTrue();
            aside.GetAttribute("inert").Should().Be("false");
        }
    }
}
