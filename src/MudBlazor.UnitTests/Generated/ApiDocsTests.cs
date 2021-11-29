﻿using System.Net.Http;
using System.Threading.Tasks;
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
            ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());
            ctx.Services.AddSingleton<IResizeService>(new MockResizeService());
            ctx.Services.AddSingleton<IBreakpointService>(new MockBreakpointService());
            ctx.Services.AddTransient<IScrollManager, MockScrollManager>();
            ctx.Services.AddTransient<IScrollListener, MockScrollListener>();
            ctx.Services.AddTransient<IJsApiService, MockJsApiServices>();
            ctx.Services.AddTransient<IResizeObserver, MockResizeObserver>();
            ctx.Services.AddTransient<IScrollSpy, MockScrollSpy>();
            ctx.Services.AddTransient<IEventListener, MockEventListener>();
            ctx.Services.AddSingleton<IBrowserWindowSizeProvider>(new MockBrowserWindowSizeProvider());
            ctx.Services.AddSingleton<IDocsNavigationService, DocsNavigationService>();
            ctx.Services.AddSingleton<IMenuService, MenuService>();
            ctx.Services.AddSingleton<IMudPopoverService, MockPopoverService>();
            ctx.Services.AddTransient<IKeyInterceptor, MockKeyInterceptorService>();
            ctx.Services.AddSingleton<IRenderQueueService, RenderQueueService>();
            ctx.Services.AddScoped(sp => new HttpClient());
        }

        // This shows how to test a docs page with incremental rendering. 
        // We are not (yet) testing all docs pages (just the examples), but if we wanted to, this would be the way.
        [Test]
        public async Task AlertPage_Test()
        {
            var comp = ctx.RenderComponent<Docs.Pages.Components.Alert.AlertPage>();
            await ctx.Services.GetService<IRenderQueueService>().WaitUntilEmpty();
            System.Console.WriteLine(comp.Markup);
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();
    }
}
