using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MudBlazor.Services
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a Dialog Service as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorDialog(this IServiceCollection services)
        {
            services.TryAddScoped<IDialogService, DialogService>();
            return services;
        }

        /// <summary>
        /// Adds a Snackbar Service as a Scoped instance with default options.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorSnackbar(this IServiceCollection services)
        {
            return AddMudBlazorSnackbar(services, new SnackbarConfiguration());
        }

        /// <summary>
        /// Adds a Snackbar Service as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configurationFunction">Defines a function that is passed a default configuration for further customisation for this instance.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorSnackbar(this IServiceCollection services, Action<SnackbarConfiguration> configurationFunction)
        {
            if (configurationFunction == null) throw new ArgumentNullException(nameof(configurationFunction));

            var options = new SnackbarConfiguration();
            configurationFunction(options);

            return AddMudBlazorSnackbar(services, options);
        }

        /// <summary>
        /// Adds a Snackbar Service as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configuration">Defines SnackbarConfiguration for this instance.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorSnackbar(this IServiceCollection services, SnackbarConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            services.TryAddScoped<ISnackbar>(builder => new SnackbarService(configuration));
            return services;
        }

        /// <summary>
        /// Adds a ResizeListener as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
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

        /// <summary>
        /// Adds a ResizeListener as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines ResizeOptions for this instance</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorResizeListener(this IServiceCollection services, Action<ResizeOptions> options)
        {
            services.TryAddScoped<IResizeListenerService, ResizeListenerService>();
            services.TryAddScoped<IBrowserWindowSizeProvider, BrowserWindowSizeProvider>();
            services.Configure(options);
            return services;
        }

        /// <summary>
        /// Adds ScrollManager as a transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public static IServiceCollection AddMudBlazorScrollManager(this IServiceCollection services)
        {
            services.TryAddTransient<IScrollManager, ScrollManager>();
            return services;
        }

        /// <summary>
        /// Adds ScrollListener as a transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public static IServiceCollection AddMudBlazorScrollListener(this IServiceCollection services)
        {
            services.TryAddTransient<IScrollListener, ScrollListener>();
            return services;
        }

        /// <summary>
        /// Adds common services required by MudBlazor components
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazor(this IServiceCollection services)
        {
            return services
                .AddMudBlazorDialog()
                .AddMudBlazorSnackbar()
                .AddMudBlazorResizeListener()
                .AddMudBlazorScrollManager()
                .AddMudBlazorScrollListener();
        }
    }
}