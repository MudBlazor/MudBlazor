using Microsoft.Extensions.DependencyInjection;
using System;
using MudBlazor.Services;

namespace MudBlazor.Services
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a ResizeListener as a Scoped instance.
        /// </summary>
        /// <param name="configure">Defines settings for this instance.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorResizeListener(this IServiceCollection services, Action<ResizeOptions> configure)
        {
            services.AddScoped<ResizeListenerService>();
            services.Configure(configure);
            return services;
        }

        public static IServiceCollection AddMudBlazorResizeListener(this IServiceCollection services)
        {
            services.AddMudBlazorResizeListener(options =>
            {
                options.ReportRate = 300;
                options.EnableLogging = true;
                options.SuppressInitEvent = false;
            });
            return services;
        }
    }
}