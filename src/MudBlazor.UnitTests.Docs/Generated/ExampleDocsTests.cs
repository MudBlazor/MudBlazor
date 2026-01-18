using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Docs.Services;
using MudBlazor.Services;
using MudBlazor.UnitTests.Docs.Mocks;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Generated
{
    [TestFixture]
    public partial class ExampleDocsTests
    {
        private BunitContext _ctx;

        private BunitContext Ctx => _ctx ?? throw new InvalidOperationException("Bunit context has not been initialized for this test.");

        private BunitContext ctx => Ctx;

        [SetUp]
        public void Setup()
        {
            _ctx = CreateContext();
        }

        [TearDown]
        public void TearDown()
        {
            var currentCtx = _ctx;
            if (currentCtx == null)
            {
                return;
            }

            try
            {
                currentCtx.Dispose();
            }
            catch (Exception) { /*ignore, may fail because of dispose in the middle of a (second) render pass*/ }
            finally
            {
                _ctx = null;
            }
        }

        private static BunitContext CreateContext()
        {
            var context = new BunitContext();
            context.JSInterop.Mode = JSRuntimeMode.Loose;
            context.Services.AddSingleton(TimeProvider.System);
            context.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            context.Services.AddSingleton<IDialogService>(new DialogService());
            context.Services.AddSingleton<ISnackbar, SnackbarService>();
            context.Services.AddSingleton<IBrowserViewportService>(new MockBrowserViewportService());
            context.Services.AddTransient<IScrollManager, MockScrollManager>();
            context.Services.AddTransient<IScrollListenerFactory, MockScrollListenerFactory>();
            context.Services.AddTransient<IJsApiService, MockJsApiService>();
            context.Services.AddTransient<IDocsJsApiService, MockDocsJsApiService>();
            context.Services.AddTransient<IResizeObserverFactory, MockResizeObserverFactory>();
            context.Services.AddTransient<IEventListenerFactory, MockEventListenerFactory>();
            context.Services.AddTransient<IEventListener, MockEventListener>();
            context.Services.AddSingleton<IKeyInterceptorService, MockKeyInterceptorService>();
            context.Services.AddTransient<IJsEventFactory, MockJsEventFactory>();
            context.Services.AddSingleton<IPopoverService, MockPopoverService>();
            context.Services.AddScoped<IRenderQueueService, RenderQueueService>();
            context.Services.AddScoped<IPointerEventsNoneService, MockPointerEventsNoneService>();
            context.Services.AddTransient<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            context.Services.AddTransient<InternalMudLocalizer>();
            context.Services.AddTransient<ILocalizationEnumInterceptor, DefaultLocalizationEnumInterceptor>();
            context.Services.AddTransient<IScrollListener, ScrollListener>();
            context.Services.AddTransient<IResizeObserver, ResizeObserver>();
            context.Services.AddOptions();
            context.Services.AddScoped(sp =>
                new HttpClient(new MockDocsMessageHandler()) { BaseAddress = new Uri("https://localhost/") });
            return context;
        }
    }
}
