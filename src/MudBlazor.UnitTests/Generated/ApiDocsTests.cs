using System.Net.Http;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Docs.Services;
using MudBlazor.Interop;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public partial class ApiDocsTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            ctx.Services.AddSingleton<IDialogService>(new DialogService());
            ctx.Services.AddSingleton<ISnackbar, SnackbarService>();
#pragma warning disable CS0618
            //TODO: Remove in v7
            ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());
            ctx.Services.AddSingleton<IBreakpointService>(new MockBreakpointService());
            ctx.Services.AddSingleton<IResizeService>(new MockResizeService());
            ctx.Services.AddSingleton<IBrowserWindowSizeProvider>(new MockBrowserWindowSizeProvider());
#pragma warning restore CS0618
            ctx.Services.AddSingleton<IBrowserViewportService>(new MockBrowserViewportService());
            ctx.Services.AddTransient<IScrollManager, MockScrollManager>();
            ctx.Services.AddTransient<IScrollListenerFactory, MockScrollListenerFactory>();
            ctx.Services.AddTransient<IJsApiService, MockJsApiService>();
            ctx.Services.AddTransient<IDocsJsApiService, MockDocsJsApiService>();
            ctx.Services.AddTransient<IResizeObserverFactory, MockResizeObserverFactory>();
            ctx.Services.AddTransient<IScrollSpyFactory, MockScrollSpyFactory>();
            ctx.Services.AddTransient<IEventListenerFactory, MockEventListenerFactory>();
            ctx.Services.AddTransient<IEventListener, MockEventListener>();
            ctx.Services.AddSingleton<IDocsNavigationService, DocsNavigationService>();
            ctx.Services.AddSingleton<IMenuService, MenuService>();
#pragma warning disable CS0618
            //TODO: Remove in v7.
            ctx.Services.AddSingleton<IMudPopoverService, MockPopoverService>();
#pragma warning restore CS0618
            ctx.Services.AddSingleton<IPopoverService, MockPopoverServiceV2>();
            ctx.Services.AddTransient<IKeyInterceptorFactory, MockKeyInterceptorServiceFactory>();
            ctx.Services.AddTransient<IJsEventFactory, MockJsEventFactory>();
            ctx.Services.AddSingleton<IRenderQueueService, RenderQueueService>();
            ctx.Services.AddTransient<InternalMudLocalizer>();
            ctx.Services.AddScoped(sp => new HttpClient());
        }

        // This shows how to test a docs page with incremental rendering.
        // We are not (yet) testing all docs pages (just the examples), but if we wanted to, this would be the way.
        [Test]
        public async Task AlertPage_Test()
        {
            var comp = ctx.RenderComponent<Docs.Pages.Components.Alert.AlertPage>();
            await ctx.Services.GetService<IRenderQueueService>().WaitUntilEmpty();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();
    }
}
