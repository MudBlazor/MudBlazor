using System;
using System.Net.Http;
using Bunit;
using Bunit.Rendering;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Charts;
using MudBlazor.Docs.Examples;
using MudBlazor.Docs.Components;
using MudBlazor.Docs.Wireframes;
using MudBlazor.Internal;
using MudBlazor.Services;
using MudBlazor.UnitTests;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;
using Toolbelt.Blazor.HeadElement;
using ComponentParameter = Bunit.ComponentParameter;

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
            ctx.Services.AddSingleton<ISnackbar>(new SnackbarService());
            ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());
            ctx.Services.AddTransient<IScrollManager, MockScrollManager>();
            ctx.Services.AddTransient<IScrollListener, MockScrollListener>();
            ctx.Services.AddSingleton<IHeadElementHelper>(new MockHeadElementHelper());
            ctx.Services.AddSingleton<IBrowserWindowSizeProvider>(new MockBrowserWindowSizeProvider());
            ctx.Services.AddSingleton<IDomService>(new MockDomService());
            ctx.Services.AddScoped(sp => new HttpClient());
        }
        
        [TearDown]
        public void TearDown() => ctx.Dispose();
    }
}