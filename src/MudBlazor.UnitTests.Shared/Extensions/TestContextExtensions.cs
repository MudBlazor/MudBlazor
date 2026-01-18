using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using MudBlazor.Services;
using MudBlazor.UnitTests.Shared.Mocks;

namespace MudBlazor.UnitTests.Shared.Extensions
{
    public static class TestContextExtensions
    {
        public static void AddTestServices(this BunitContext ctx)
        {
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            var timeProvider = new FakeTimeProvider();
            timeProvider.SetUtcNow(DateTime.UtcNow);
            ctx.Services.AddSingleton(timeProvider);
            ctx.Services.AddSingleton<TimeProvider>(sp => sp.GetRequiredService<FakeTimeProvider>());
            ctx.Services.AddMudServices(options =>
            {
                options.SnackbarConfiguration.ShowTransitionDuration = 0;
                options.SnackbarConfiguration.HideTransitionDuration = 0;
                options.PopoverOptions.CheckForPopoverProvider = false;
            });
            ctx.Services.AddScoped(sp => new HttpClient());
            ctx.Services.AddOptions();
        }

        public static void AdvanceTime(this BunitContext ctx, int milliseconds)
        {
            var timeProvider = ctx.Services.GetRequiredService<TimeProvider>();
            if (timeProvider is not FakeTimeProvider fakeTimeProvider)
            {
                throw new InvalidOperationException("TimeProvider must be a FakeTimeProvider to advance time.");
            }

            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(milliseconds));
        }
    }
}
