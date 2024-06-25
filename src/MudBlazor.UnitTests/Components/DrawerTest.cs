using System.ComponentModel;
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

#nullable enable
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DrawerTest : BunitTest
    {
        private BrowserViewportService GetBrowserViewportService(BrowserWindowSize browserWindowSize)
        {
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var browserViewportService = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
            // Sets the initial browser size aka simulating the windows size when the website was opened for the first time
            jsRuntimeMock
                .Setup(expression => expression.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<object[]>()))
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

        private BrowserWindowSize BreakpointBrowserAssociatedSize(Breakpoint breakpoint)
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
        public void TemporaryClosed_Open_CheckOpened_Close_CheckClosed()
        {
            _ = AddBrowserViewportService();
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
        public void TemporaryClosedWithoutOverlay_Open_CheckOverlay()
        {
            _ = AddBrowserViewportService();
            var comp = Context.RenderComponent<DrawerTest1>(
                Parameter(nameof(DrawerTest1.Variant), DrawerVariant.Temporary),
                Parameter(nameof(DrawerTest1.Overlay), false));

            comp.Find("button").Click();
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public void TemporaryClosedClipped_Open_CheckState()
        {
            _ = AddBrowserViewportService();
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
        public void PersistentClosed_Open_CheckOpened_Close_CheckClosed()
        {
            _ = AddBrowserViewportService();
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
        public void PersistentClosedClipped_Open_CheckState()
        {
            _ = AddBrowserViewportService();
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
        public void MiniClosed_Open_CheckOpened_Close_CheckClosed()
        {
            _ = AddBrowserViewportService();
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
        public void MiniClosedClipped_Open_CheckState()
        {
            _ = AddBrowserViewportService();
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
        public void ResponsiveClosed_Open_CheckOpened_Close_CheckClosed()
        {
            _ = AddBrowserViewportService();
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
        public void ResponsiveSmallClosed_Open_CheckOpenedAndOverlay(Breakpoint point)
        {
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(point));
            var comp = Context.RenderComponent<DrawerResponsiveTest>();

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
        public void ResponsiveClosed_StartLargeScreen_SetBreakpoint_Open_CheckState(Breakpoint breakpoint)
        {
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Xl));
            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.Breakpoint), breakpoint));

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
        public void ResponsiveClosed_StartSmallScreen_SetBreakpoint_Open_CheckState(Breakpoint breakpoint)
        {
            _ = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Xs));
            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.Breakpoint), breakpoint));

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
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.RenderComponent<DrawerResponsiveTest>();
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;

            // Open drawer
            comp.Find("button").Click();

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
            comp.Find("button").Click();
            comp.Instance.Drawer.Open.Should().BeFalse();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);

            // Resize to small, then open drawer
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 400, Width = 600 }, Breakpoint.Sm, subscription.JavaScriptListenerId));

            // Open drawer
            comp.Find("button").Click();
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll("aside+.mud-drawer-overlay").Count.Should().Be(1);

            // Resize to large, drawer should stays open
            await comp.InvokeAsync(async () => await browserViewportService.RaiseOnResized(new BrowserWindowSize { Height = 720, Width = 1280 }, Breakpoint.Lg, subscription.JavaScriptListenerId));

            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll("aside+.mud-drawer-overlay").Count.Should().Be(0);
        }

        /// <summary>
        /// Resize screen to small in two steps: first to SM, then to XS. After restoring the original screen size, the drawer should reopen automatically.
        /// </summary>
        [Test]
        public async Task Responsive_ResizeToSmall_RestoreToLarge_CheckStates()
        {
            var browserViewportService = AddBrowserViewportService(BreakpointBrowserAssociatedSize(Breakpoint.Lg));
            var comp = Context.RenderComponent<DrawerResponsiveTest>();
            var mudDrawerComponent = comp.FindComponent<MudDrawer>();
            var subscription = browserViewportService.GetInternalSubscription(mudDrawerComponent.Instance)!;

            // Open drawer
            comp.Find("button").Click();

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
            var comp = Context.RenderComponent<DrawerResponsiveTest>();
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
            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.Breakpoint), breakpoint));
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
            comp.Find("button").Click();

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
            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.Breakpoint), breakpoint));
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
            comp.Find("button").Click();

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
        public void DrawerContainer_RemoveDrawer_CheckStates()
        {
            _ = AddBrowserViewportService();
            var comp = Context.RenderComponent<DrawerContainerTest1>();

            comp.FindAll("div.mud-drawer-open-responsive-md-right").Count.Should().Be(1);

            // Remove drawer
            comp.Find("button").Click();

            comp.FindAll("div.mud-drawer-open-responsive-md-right").Count.Should().Be(0);
        }
    }
}
