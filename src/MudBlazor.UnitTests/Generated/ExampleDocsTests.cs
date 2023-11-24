using System;
using System.Net.Http;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Docs.Services;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public partial class ExampleDocsTests
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
            ctx.Services.AddTransient<IEventListenerFactory, MockEventListenerFactory>();
            ctx.Services.AddTransient<IEventListener, MockEventListener>();
            ctx.Services.AddTransient<IKeyInterceptorFactory, MockKeyInterceptorServiceFactory>();
            ctx.Services.AddTransient<IJsEventFactory, MockJsEventFactory>();
#pragma warning disable CS0618
            //TODO: Remove in v7
            ctx.Services.AddSingleton<IMudPopoverService, MockPopoverService>();
#pragma warning restore CS0618
            ctx.Services.AddSingleton<IPopoverService, MockPopoverServiceV2>();
            ctx.Services.AddSingleton<IRenderQueueService, RenderQueueService>();
            ctx.Services.AddTransient<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            ctx.Services.AddTransient<InternalMudLocalizer>();
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
