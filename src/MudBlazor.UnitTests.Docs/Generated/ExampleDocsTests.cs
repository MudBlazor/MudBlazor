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

        [SetUp]
        public void Setup()
        {
            _ctx = new BunitContext();
            _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            _ctx.Services.AddSingleton(TimeProvider.System);
            _ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            _ctx.Services.AddSingleton<IDialogService>(new DialogService());
            _ctx.Services.AddSingleton<ISnackbar, SnackbarService>();
            _ctx.Services.AddSingleton<IBrowserViewportService>(new MockBrowserViewportService());
            _ctx.Services.AddTransient<IScrollManager, MockScrollManager>();
            _ctx.Services.AddTransient<IScrollListenerFactory, MockScrollListenerFactory>();
            _ctx.Services.AddTransient<IJsApiService, MockJsApiService>();
            _ctx.Services.AddTransient<IDocsJsApiService, MockDocsJsApiService>();
            _ctx.Services.AddTransient<IResizeObserverFactory, MockResizeObserverFactory>();
            _ctx.Services.AddTransient<IEventListenerFactory, MockEventListenerFactory>();
            _ctx.Services.AddTransient<IEventListener, MockEventListener>();
            _ctx.Services.AddSingleton<IKeyInterceptorService, MockKeyInterceptorService>();
            _ctx.Services.AddTransient<IJsEventFactory, MockJsEventFactory>();
            _ctx.Services.AddSingleton<IPopoverService, MockPopoverService>();
            _ctx.Services.AddScoped<IRenderQueueService, RenderQueueService>();
            _ctx.Services.AddScoped<IPointerEventsNoneService, MockPointerEventsNoneService>();
            _ctx.Services.AddTransient<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            _ctx.Services.AddTransient<InternalMudLocalizer>();
            _ctx.Services.AddTransient<ILocalizationEnumInterceptor, DefaultLocalizationEnumInterceptor>();
            _ctx.Services.AddTransient<IScrollListener, ScrollListener>();
            _ctx.Services.AddTransient<IResizeObserver, ResizeObserver>();
            _ctx.Services.AddOptions();
            _ctx.Services.AddScoped(sp =>
                new HttpClient(new MockDocsMessageHandler()) { BaseAddress = new Uri("https://localhost/") });
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _ctx.Dispose();
            }
            catch (Exception) { /*ignore, may fail because of dispose in the middle of a (second) render pass*/ }
        }
    }
}
