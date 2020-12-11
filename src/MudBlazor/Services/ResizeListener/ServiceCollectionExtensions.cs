using Microsoft.Extensions.DependencyInjection;
using System;
using MudBlazor.Services;
using MudBlazor.Providers;

namespace MudBlazor.Services
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a ResizeListener as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configure">Defines settings for this instance.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorResizeListener(this IServiceCollection services, Action<ResizeOptions> configure)
        {
            services.AddScoped<IResizeListenerService, ResizeListenerService>();
            services.AddScoped<IBrowserWindowSizeProvider, BrowserWindowSizeProvider>();
            services.Configure(configure);
            return services;
        }

        public static IServiceCollection AddMudBlazorResizeListener(this IServiceCollection services)
        {
            services.AddMudBlazorResizeListener(options =>
            {
                options.ReportRate = 100; // ms delay
                options.EnableLogging = true;
                options.SuppressInitEvent = false;
            });
            return services;
        }
    }
}