﻿using System;
using System.Net.Http;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;
using RichardSzalay.MockHttp;
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
            ctx.Services.AddSingleton<ISnackbar>(new SnackbarService());
            ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());
            ctx.Services.AddTransient<IScrollManager, MockScrollManager>();
            ctx.Services.AddTransient<IScrollListener, MockScrollListener>();
            ctx.Services.AddTransient<IJsApiService, MockJsApiServices>();
            ctx.Services.AddSingleton<IHeadElementHelper>(new MockHeadElementHelper());
            ctx.Services.AddSingleton<IBrowserWindowSizeProvider>(new MockBrowserWindowSizeProvider());
            ctx.Services.AddOptions();
            ctx.Services.AddScoped(sp => 
                new HttpClient(new MockDocsMessageHandler()) { BaseAddress = new Uri("https://localhost/") });
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();
    }
}

