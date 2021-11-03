using System;
using System.Net.Http;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;
using Toolbelt.Blazor.HeadElement;

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
            ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());
            ctx.Services.AddSingleton<IResizeService>(new MockResizeService());
            ctx.Services.AddSingleton<IBreakpointService>(new MockBreakpointService());
            ctx.Services.AddTransient<IScrollManager, MockScrollManager>();
            ctx.Services.AddTransient<IScrollListener, MockScrollListener>();
            ctx.Services.AddTransient<IJsApiService, MockJsApiServices>();
            ctx.Services.AddSingleton<IHeadElementHelper>(new MockHeadElementHelper());
            ctx.Services.AddTransient<IResizeObserver, MockResizeObserver>();
            ctx.Services.AddSingleton<IBrowserWindowSizeProvider>(new MockBrowserWindowSizeProvider());
            ctx.Services.AddTransient<IEventListener, EventListener>();
            ctx.Services.AddTransient<IKeyInterceptor, MockKeyInterceptorService>();
            ctx.Services.AddSingleton<IMudPopoverService, MockPopoverService>();

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
            catch(Exception) { /*ignore, may fail because of dispose in the middle of a (second) render pass*/ }
        }
    }
}

