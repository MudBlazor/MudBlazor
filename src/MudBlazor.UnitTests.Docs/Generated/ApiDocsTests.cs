using AwesomeAssertions;
using Bunit;
using Bunit.TestDoubles;
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
        private BunitContext _ctx;
        private BunitNavigationManager _navigationManager;

        [SetUp]
        public void Setup()
        {
            _ctx = new BunitContext();
            _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            _ctx.Services.AddSingleton(TimeProvider.System);
            _navigationManager = new BunitNavigationManager(_ctx);
            _ctx.Services.AddSingleton<NavigationManager>(_navigationManager);
            _ctx.Services.AddSingleton<IDialogService>(new DialogService());
            _ctx.Services.AddSingleton<ISnackbar, SnackbarService>();
            _ctx.Services.AddSingleton<IBrowserViewportService>(new MockBrowserViewportService());
            _ctx.Services.AddTransient<IScrollManager, MockScrollManager>();
            _ctx.Services.AddTransient<IScrollListenerFactory, MockScrollListenerFactory>();
            _ctx.Services.AddTransient<IJsApiService, MockJsApiService>();
            _ctx.Services.AddTransient<IDocsJsApiService, MockDocsJsApiService>();
            _ctx.Services.AddTransient<IResizeObserverFactory, MockResizeObserverFactory>();
            _ctx.Services.AddTransient<IScrollSpyFactory, MockScrollSpyFactory>();
            _ctx.Services.AddSingleton<IDocsNavigationService, DocsNavigationService>();
            _ctx.Services.AddSingleton<IMenuService, MenuService>();
            _ctx.Services.AddSingleton<IPopoverService, MockPopoverService>();
            _ctx.Services.AddSingleton<IKeyInterceptorService, MockKeyInterceptorService>();
            _ctx.Services.AddTransient<IJsEventFactory, MockJsEventFactory>();
            _ctx.Services.AddScoped<IRenderQueueService, RenderQueueService>();
            _ctx.Services.AddScoped<IPointerEventsNoneService, MockPointerEventsNoneService>();
            _ctx.Services.AddTransient<InternalMudLocalizer>();
            _ctx.Services.AddTransient<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            _ctx.Services.AddTransient<ILocalizationEnumInterceptor, DefaultLocalizationEnumInterceptor>();
            _ctx.Services.AddScoped(sp => new HttpClient());
        }

        // This shows how to test a docs page with incremental rendering.
        // We are not (yet) testing all docs pages (just the examples), but if we wanted to, this would be the way.
        [Test]
        public async Task AlertPage_Test()
        {
            _navigationManager.NavigateTo("/components/alert");
            _ = _ctx.Render<MudBlazor.Docs.Pages.Components.Alert.AlertPage>();
            await _ctx.Services.GetService<IRenderQueueService>().WaitUntilEmpty();
        }

        /// <summary>
        /// An example of a generated API test.
        /// </summary>
        [Test]
        public async Task MudAlert_API_Test_Example()
        {
            _navigationManager.NavigateTo("/components/MudAlert");
            var comp = _ctx.Render<Api>(parameters => parameters.Add(x => x.TypeName, "MudAlert"));
            await _ctx.Services.GetService<IRenderQueueService>().WaitUntilEmpty();
            comp.Find(".mud-breadcrumbs");
            var exampleLink = comp.FindComponents<MudLink>().FirstOrDefault(link => link.Instance.Href.StartsWith("/component"));
            exampleLink.Should().NotBeNull();
        }

        [TearDown]
        public async Task TearDown() => await _ctx.DisposeAsync();
    }
}
