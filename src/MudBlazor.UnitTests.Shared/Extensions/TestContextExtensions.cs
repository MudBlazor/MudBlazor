using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Shared.Extensions
{
    public static class TestContextExtensions
    {
        public static void AddTestServices(this BunitContext ctx)
        {
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddSingleton<NavigationManager>(new BunitNavigationManager(ctx));
            ctx.Services.AddMudServices(options =>
            {
                options.SnackbarConfiguration.ShowTransitionDuration = 0;
                options.SnackbarConfiguration.HideTransitionDuration = 0;
                options.PopoverOptions.CheckForPopoverProvider = false;
            });
            ctx.Services.AddScoped(sp => new HttpClient());
            ctx.Services.AddOptions();
        }

        /// <summary>
        /// Replaces the default time provider with a fake provider for the current bUnit context.
        /// </summary>
        public static FakeTimeProvider AddFakeTimeProvider(this BunitContext ctx)
        {
            var timeProvider = new FakeTimeProvider();
            ctx.Services.RemoveAll<TimeProvider>();
            ctx.Services.AddSingleton<TimeProvider>(timeProvider);

            return timeProvider;
        }
    }
}
