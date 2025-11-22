using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Docs.Services;
using MudBlazor.Services;
#pragma warning disable CS0619 // Type or member is obsolete
using MudBlazor.UnitTests.Docs.Mocks;
#pragma warning disable CS0619 // Type or member is obsolete
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;
using MudBlazor.UnitTests.Shared.Extensions;

namespace MudBlazor.UnitTests.Docs.Generated
{
    [TestFixture]
    public partial class ExampleDocsTests
    {
        private Bunit.BunitContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.BunitContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddSingleton(TimeProvider.System);
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            ctx.Services.AddSingleton<IDialogService>(new DialogService());
            ctx.Services.AddSingleton<ISnackbar, SnackbarService>();
            ctx.Services.AddSingleton<IBrowserViewportService>(new MockBrowserViewportService());
            ctx.Services.AddTransient<IScrollManager, MockScrollManager>();
            ctx.Services.AddTransient<IScrollListenerFactory, MockScrollListenerFactory>();
            ctx.Services.AddTransient<IJsApiService, MockJsApiService>();
            ctx.Services.AddTransient<IDocsJsApiService, MockDocsJsApiService>();
            ctx.Services.AddTransient<IResizeObserverFactory, MockResizeObserverFactory>();
            ctx.Services.AddTransient<IEventListenerFactory, MockEventListenerFactory>();
            ctx.Services.AddTransient<IEventListener, MockEventListener>();
            ctx.Services.AddSingleton<IKeyInterceptorService, MockKeyInterceptorService>();
            ctx.Services.AddTransient<IJsEventFactory, MockJsEventFactory>();
            ctx.Services.AddSingleton<IPopoverService, MockPopoverService>();
            ctx.Services.AddScoped<IRenderQueueService, RenderQueueService>();
            ctx.Services.AddScoped<IPointerEventsNoneService, MockPointerEventsNoneService>();
            ctx.Services.AddTransient<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            ctx.Services.AddTransient<InternalMudLocalizer>();
            ctx.Services.AddTransient<ILocalizationEnumInterceptor, DefaultLocalizationEnumInterceptor>();
            ctx.Services.AddTransient<IScrollListener, ScrollListener>();
            ctx.Services.AddTransient<IResizeObserver, ResizeObserver>();
            ctx.Services.AddOptions();
            ctx.Services.AddScoped(sp =>
                new HttpClient(new MockDocsMessageHandler()) { BaseAddress = new Uri("https://localhost/") });
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                ctx.Dispose();
            }
            catch (Exception) { /*ignore, may fail because of dispose in the middle of a (second) render pass*/ }
        }
    }
}
