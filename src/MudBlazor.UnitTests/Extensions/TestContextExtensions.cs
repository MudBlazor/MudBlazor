using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;

namespace MudBlazor.UnitTests
{
    public static class TestContextExtensions
    {
        public static void AddMudBlazorServices(this Bunit.TestContext ctx)
        {
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            ctx.Services.AddSingleton<IDialogService>(new DialogService());
            ctx.Services.AddSingleton<ISnackbar>(new SnackbarService(new SnackbarConfiguration() { ShowTransitionDuration = 0, HideTransitionDuration = 0 }));
            ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());

            ctx.Services.AddTransient<IScrollListener, MockScrollListener>();
            ctx.Services.AddTransient<IScrollManager, MockScrollManager>();

            ctx.Services.AddSingleton<IBrowserWindowSizeProvider>(new MockBrowserWindowSizeProvider());

            ctx.Services.AddScoped(sp => new HttpClient());
            ctx.Services.AddOptions();
        }
    }
}
