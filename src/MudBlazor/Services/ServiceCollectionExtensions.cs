using System;
using System.Diagnostics.CodeAnalysis;
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
            configuration ??= new SnackbarConfiguration();

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
            options ??= new ResizeOptions();
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
            services.TryAddScoped<IResizeService, ResizeService>();
            services.TryAddScoped<IBreakpointService, BreakpointService>();
            services.Configure(options);
            return services;
        }

        /// <summary>
        /// Adds a IResizeObserver as a Transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines ResizeObserverOptions for this instance</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorResizeObserver(this IServiceCollection services, Action<ResizeObserverOptions> options)
        {
            services.TryAddTransient<IResizeObserver, ResizeObserver>();
            services.Configure(options);
            return services;
        }

        /// <summary>
        /// Adds a IResizeObserver as a Transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines ResizeObserverOptions for this instance</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorResizeObserver(this IServiceCollection services, ResizeObserverOptions options = null)
        {
            options ??= new ResizeObserverOptions();
            services.AddMudBlazorResizeObserver(o =>
            {
                o = options;
            });
            return services;
        }

        /// <summary>
        /// Adds a IResizeObserverFactory as a scoped dependency.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines ResizeObserverOptions for this instance</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorResizeObserverFactory(this IServiceCollection services, Action<ResizeObserverOptions> options)
        {
            services.TryAddScoped<IResizeObserverFactory, ResizeObserverFactory>();
            services.Configure(options);
            return services;
        }

        /// <summary>
        /// Adds a IResizeObserverFactory as a scoped dependency.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines ResizeObserverOptions for this instance</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorResizeObserverFactory(this IServiceCollection services, ResizeObserverOptions options = null)
        {
            options ??= new ResizeObserverOptions();
            services.AddMudBlazorResizeObserverFactory(o =>
            {
                o = options;
            });
            return services;
        }

        /// <summary>
        /// Adds IKeyInterceptor as a Transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorKeyInterceptor(this IServiceCollection services)
        {
            services.TryAddTransient<IKeyInterceptor, KeyInterceptor>();
            services.TryAddScoped<IKeyInterceptorFactory, KeyInterceptorFactory>();

            return services;
        }

        /// <summary>
        /// Adds JsEvent as a Transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorJsEvent(this IServiceCollection services)
        {
            services.TryAddTransient<IJsEvent, JsEvent>();
            services.TryAddScoped<IJsEventFactory, JsEventFactory>();

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
        /// Adds ScrollManager as a transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines PopoverOptions for the application/user</param>
        public static IServiceCollection AddMudPopoverService(this IServiceCollection services, Action<PopoverOptions> options)
        {
            services.Configure(options);
#pragma warning disable CS0618
            //TODO: Remove in v7.
            services.TryAddScoped<IMudPopoverService, MudPopoverService>();
#pragma warning restore CS0618
            services.TryAddScoped<IPopoverService, PopoverService>();
            return services;
        }

        /// <summary>
        /// Adds ScrollManager as a transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines PopoverOptions for the application/user</param>
        public static IServiceCollection AddMudPopoverService(this IServiceCollection services, PopoverOptions options)
        {
            options ??= new PopoverOptions();
            services.AddMudPopoverService(o =>
            {
                o = options;
            });
            return services;
        }

        /// <summary>
        /// Adds ScrollListener as a transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public static IServiceCollection AddMudBlazorScrollListener(this IServiceCollection services)
        {
            services.TryAddTransient<IScrollListener, ScrollListener>();
            services.TryAddScoped<IScrollListenerFactory, ScrollListenerFactory>();

            return services;
        }


        /// <summary>
        /// Adds ScrollSpy as a transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public static IServiceCollection AddMudBlazorScrollSpy(this IServiceCollection services)
        {
            services.TryAddTransient<IScrollSpy, ScrollSpy>();
            services.TryAddScoped<IScrollSpyFactory, ScrollSpyFactory>();
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
        /// Adds IEventListener as a transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public static IServiceCollection AddMudEventManager(this IServiceCollection services)
        {
            services.TryAddTransient<IEventListener, EventListener>();
            services.TryAddScoped<IEventListenerFactory, EventListenerFactory>();

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
            configuration ??= new MudServicesConfiguration();
            return services
                .AddMudBlazorDialog()
                .AddMudBlazorSnackbar(configuration.SnackbarConfiguration)
                .AddMudBlazorResizeListener(configuration.ResizeOptions)
                .AddMudBlazorResizeObserver(configuration.ResizeObserverOptions)
                .AddMudBlazorResizeObserverFactory()
                .AddMudBlazorKeyInterceptor()
                .AddMudBlazorJsEvent()
                .AddMudBlazorScrollManager()
                .AddMudBlazorScrollListener()
                .AddMudBlazorJsApi()
                .AddMudBlazorScrollSpy()
                .AddMudPopoverService(configuration.PopoverOptions)
                .AddMudEventManager();
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
                .AddMudBlazorResizeObserver(options.ResizeObserverOptions)
                .AddMudBlazorResizeObserverFactory(options.ResizeObserverOptions)
                .AddMudBlazorKeyInterceptor()
                .AddMudBlazorJsEvent()
                .AddMudBlazorScrollManager()
                .AddMudBlazorScrollListener()
                .AddMudBlazorJsApi()
                .AddMudPopoverService(options.PopoverOptions)
                .AddMudBlazorScrollSpy()
                .AddMudEventManager();
        }
    }
}
