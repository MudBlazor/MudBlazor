using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Docs.Pages.Api;
using MudBlazor.Docs.Services;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Generated
{
    [TestFixture]
    public partial class ApiDocsTests
    {
        // This shows how to test a docs page with incremental rendering.
        // We are not (yet) testing all docs pages (just the examples), but if we wanted to, this would be the way.
        [Test]
        public async Task AlertPage_Test()
        {
            await using var ctx = CreateContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager("https://localhost:2112/", "https://localhost:2112/components/alert"));
            var comp = ctx.Render<MudBlazor.Docs.Pages.Components.Alert.AlertPage>();
            await ctx.Services.GetService<IRenderQueueService>().WaitUntilEmpty();
        }

        /// <summary>
        /// An example of a generated API test.
        /// </summary>
        [Test]
        public async Task MudAlert_API_Test_Example()
        {
            await using var ctx = CreateContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager("https://localhost:2112/", "https://localhost:2112/components/MudAlert"));
            var comp = ctx.Render<Api>(parameters => parameters.Add(x => x.TypeName, "MudAlert"));
            await ctx.Services.GetService<IRenderQueueService>().WaitUntilEmpty();
            comp.Markup.Should().NotContain("Sorry, the type").And.NotContain("could not be found");
            var exampleLink = comp.FindComponents<MudLink>().FirstOrDefault(link => link.Instance.Href.StartsWith("/component"));
            exampleLink.Should().NotBeNull();
        }

        private static BunitContext CreateContext()
        {
            var context = new BunitContext();
            context.JSInterop.Mode = JSRuntimeMode.Loose;
            context.Services.AddSingleton(TimeProvider.System);
            context.Services.AddSingleton<IDialogService>(new DialogService());
            context.Services.AddSingleton<ISnackbar, SnackbarService>();
            context.Services.AddSingleton<IBrowserViewportService>(new MockBrowserViewportService());
            context.Services.AddTransient<IScrollManager, MockScrollManager>();
            context.Services.AddTransient<IScrollListenerFactory, MockScrollListenerFactory>();
            context.Services.AddTransient<IJsApiService, MockJsApiService>();
            context.Services.AddTransient<IDocsJsApiService, MockDocsJsApiService>();
            context.Services.AddTransient<IResizeObserverFactory, MockResizeObserverFactory>();
            context.Services.AddTransient<IScrollSpyFactory, MockScrollSpyFactory>();
            context.Services.AddTransient<IEventListenerFactory, MockEventListenerFactory>();
            context.Services.AddTransient<IEventListener, MockEventListener>();
            context.Services.AddSingleton<IDocsNavigationService, DocsNavigationService>();
            context.Services.AddSingleton<IMenuService, MenuService>();
            context.Services.AddSingleton<IPopoverService, MockPopoverService>();
            context.Services.AddSingleton<IKeyInterceptorService, MockKeyInterceptorService>();
            context.Services.AddTransient<IJsEventFactory, MockJsEventFactory>();
            context.Services.AddScoped<IRenderQueueService, RenderQueueService>();
            context.Services.AddScoped<IPointerEventsNoneService, MockPointerEventsNoneService>();
            context.Services.AddTransient<InternalMudLocalizer>();
            context.Services.AddTransient<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            context.Services.AddTransient<ILocalizationEnumInterceptor, DefaultLocalizationEnumInterceptor>();
            context.Services.AddScoped(sp => new HttpClient());
            return context;
        }
    }
}
