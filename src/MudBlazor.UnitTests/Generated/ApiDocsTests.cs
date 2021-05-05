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
            ctx.Services.AddTransient<IScrollManager, MockScrollManager>();
            ctx.Services.AddTransient<IScrollListener, MockScrollListener>();
            ctx.Services.AddTransient<IJsApiService, MockJsApiServices>();
            ctx.Services.AddTransient<IResizeObserver, MockResizeObserver>();
            ctx.Services.AddSingleton<IHeadElementHelper>(new MockHeadElementHelper());
            ctx.Services.AddSingleton<IBrowserWindowSizeProvider>(new MockBrowserWindowSizeProvider());
            ctx.Services.AddScoped(sp => new HttpClient());
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();
    }
}
