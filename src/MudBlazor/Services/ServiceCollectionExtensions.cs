using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Components;
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
        /// Adds a Snackbar Service as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configuration">Defines SnackbarConfiguration for this instance.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorSnackbar(this IServiceCollection services, SnackbarConfiguration configuration = null)
        {
            if (configuration == null)
                configuration = new SnackbarConfiguration();

            services.TryAddScoped<ISnackbar>(builder =>
                new SnackbarService(builder.GetService<NavigationManager>(), configuration));
            return services;
        }

        /// <summary>
        /// Adds a Snackbar Service as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configuration">Defines SnackbarConfiguration for this instance.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorSnackbar(this IServiceCollection services, Action<SnackbarConfiguration> configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var options = new SnackbarConfiguration();
            configuration(options);
            return AddMudBlazorSnackbar(services, options);
        }

        /// <summary>
        /// Adds a ResizeListener as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines ResizeOptions for this instance</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorResizeListener(this IServiceCollection services, ResizeOptions options = null)
        {
            if (options == null)
                options = new ResizeOptions();
            services.AddMudBlazorResizeListener(o =>
            {
                o = options;
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
        /// Adds JsApi as a transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public static IServiceCollection AddMudBlazorJsApi(this IServiceCollection services)
        {
            services.TryAddTransient<IJsApiService, JsApiService>();
            return services;
        }

        /// <summary>
        /// Adds common services required by MudBlazor components
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configuration">Defines options for all MudBlazor services.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudServices(this IServiceCollection services, MudServicesConfiguration configuration = null)
        {
            if (configuration == null)
                configuration = new MudServicesConfiguration();
            return services
                .AddMudBlazorDialog()
                .AddMudBlazorSnackbar(configuration.SnackbarConfiguration)
                .AddMudBlazorResizeListener(configuration.ResizeOptions)
                .AddMudBlazorScrollManager()
                .AddMudBlazorScrollListener()
                .AddMudBlazorJsApi();
        }

        /// <summary>
        /// Adds common services required by MudBlazor components
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configuration">Defines options for all MudBlazor services.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudServices(this IServiceCollection services, Action<MudServicesConfiguration> configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var options = new MudServicesConfiguration();
            configuration(options);
            return services
                .AddMudBlazorDialog()
                .AddMudBlazorSnackbar(options.SnackbarConfiguration)
                .AddMudBlazorResizeListener(options.ResizeOptions)
                .AddMudBlazorScrollManager()
                .AddMudBlazorScrollListener()
                .AddMudBlazorJsApi();
        }
    }
}
