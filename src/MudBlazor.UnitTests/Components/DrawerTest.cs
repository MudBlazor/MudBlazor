

#pragma warning disable CS1998 // async without await

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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
        public override void Setup()
        {
            base.Setup();
            Context.Services.AddScoped<IResizeListenerService, MockResizeListenerService>();
        }

        [Test]
        public async Task TemporaryClosed_Open_CheckOpened_Close_CheckClosed()
        {
            var comp = Context.RenderComponent<DrawerTest1>(Parameter(nameof(DrawerTest1.Variant), DrawerVariant.Temporary));

            Console.WriteLine(comp.Markup);

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

            Console.WriteLine(comp.Markup);

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

            Console.WriteLine(comp.Markup);

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

            Console.WriteLine(comp.Markup);

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

            Console.WriteLine(comp.Markup);

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

            Console.WriteLine(comp.Markup);

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

            Console.WriteLine(comp.Markup);

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

            Console.WriteLine(comp.Markup);

            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll("aside+mud-overlay-drawer").Count.Should().Be(0);
            comp.Instance.Drawer.Open.Should().BeTrue();
            comp.Find("button").Click();
            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();
        }

        [Test]
        public async Task ResponsiveSmallClosed_Open_CheckOpenedAndOverlay()
        {
            (Context.Services.GetService<IResizeListenerService>() as MockResizeListenerService)?.ApplyScreenSize(500, 768);

            var comp = Context.RenderComponent<DrawerResponsiveTest>();

            Console.WriteLine(comp.Markup);

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
            (Context.Services.GetService<IResizeListenerService>() as MockResizeListenerService)?.ApplyScreenSize(1920, 1080);

            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.Breakpoint), breakpoint));

            Console.WriteLine(comp.Markup);

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
            (Context.Services.GetService<IResizeListenerService>() as MockResizeListenerService)?.ApplyScreenSize(400, 300);

            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.Breakpoint), breakpoint));

            Console.WriteLine(comp.Markup);

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
            srv?.ApplyScreenSize(1280, 768);

            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.PreserveOpenState), true));

            Console.WriteLine(comp.Markup);

            //open drawer
            comp.Find("button").Click();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            //resize to small, drawer should close
            srv?.ApplyScreenSize(600, 768);

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            //resize to large, drawer should open automatically
            srv?.ApplyScreenSize(1024, 768);

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            //close drawer
            comp.Find("button").Click();

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);

            //resize to small, then open drawer
            srv?.ApplyScreenSize(600, 768);
            comp.Find("button").Click();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.FindAll("aside+.mud-drawer-overlay").Count.Should().Be(1);

            //resize to large, drawer should stays open
            srv?.ApplyScreenSize(1024, 768);

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
            var srv = Context.Services.GetService<IResizeListenerService>() as MockResizeListenerService;
            srv?.ApplyScreenSize(1280, 768);

            var comp = Context.RenderComponent<DrawerResponsiveTest>(Parameter(nameof(DrawerResponsiveTest.PreserveOpenState), true));

            Console.WriteLine(comp.Markup);

            //open drawer
            comp.Find("button").Click();

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();

            //resize to small, drawer should close
            srv?.ApplyScreenSize(800, 768);

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            //resize to extra small, drawer should close
            srv?.ApplyScreenSize(400, 768);

            comp.FindAll("aside.mud-drawer--closed.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeFalse();

            //resize to large, drawer should open automatically
            srv?.ApplyScreenSize(1024, 768);

            comp.FindAll("aside.mud-drawer--open.mud-drawer-responsive").Count.Should().Be(1);
            comp.Instance.Drawer.Open.Should().BeTrue();
        }
    }
}
