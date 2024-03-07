using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MudBlazor.Services
{
#nullable enable
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
        /// <param name="options">Defines SnackbarConfiguration for this instance.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        [Obsolete("Use overload with Action<SnackbarConfiguration>. This will be removed in v7.")]
        public static IServiceCollection AddMudBlazorSnackbar(this IServiceCollection services, SnackbarConfiguration options)
        {
            services.AddMudBlazorSnackbar(snackBarConfiguration =>
            {
                snackBarConfiguration.ClearAfterNavigation = options.ClearAfterNavigation;
                snackBarConfiguration.MaxDisplayedSnackbars = options.MaxDisplayedSnackbars;
                snackBarConfiguration.NewestOnTop = options.NewestOnTop;
                snackBarConfiguration.PositionClass = options.PositionClass;
                snackBarConfiguration.PreventDuplicates = options.PreventDuplicates;
                snackBarConfiguration.MaximumOpacity = options.MaximumOpacity;
                snackBarConfiguration.ShowTransitionDuration = options.ShowTransitionDuration;
                snackBarConfiguration.VisibleStateDuration = options.VisibleStateDuration;
                snackBarConfiguration.HideTransitionDuration = options.HideTransitionDuration;
                snackBarConfiguration.ShowCloseIcon = options.ShowCloseIcon;
                snackBarConfiguration.RequireInteraction = options.RequireInteraction;
                snackBarConfiguration.BackgroundBlurred = options.BackgroundBlurred;
                snackBarConfiguration.SnackbarVariant = options.SnackbarVariant;
            });

            return services;
        }

        /// <summary>
        /// Adds a Snackbar Service as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorSnackbar(this IServiceCollection services)
        {
            services.TryAddScoped<ISnackbar, SnackbarService>();

            return services;
        }

        /// <summary>
        /// Adds a Snackbar Service as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines SnackbarConfiguration for this instance.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorSnackbar(this IServiceCollection services, Action<SnackbarConfiguration> options)
        {
            services.AddMudBlazorSnackbar();
            services.Configure(options);

            return services;
        }

        /// <summary>
        /// Adds a ResizeListener as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines ResizeOptions for this instance</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        [Obsolete("Use overload with Action<ResizeOptions>. This will be removed in v7.")]
        public static IServiceCollection AddMudBlazorResizeListener(this IServiceCollection services, ResizeOptions options)
        {
            services.AddMudBlazorResizeListener(resizeOptions =>
            {
                resizeOptions.BreakpointDefinitions = options.BreakpointDefinitions;
                resizeOptions.EnableLogging = options.EnableLogging;
                resizeOptions.NotifyOnBreakpointOnly = options.NotifyOnBreakpointOnly;
                resizeOptions.ReportRate = options.ReportRate;
                resizeOptions.SuppressInitEvent = options.SuppressInitEvent;
            });

            return services;
        }

        /// <summary>
        /// Adds a ResizeListener as a Scoped instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorResizeListener(this IServiceCollection services)
        {
#pragma warning disable CS0618
            services.TryAddScoped<IResizeListenerService, ResizeListenerService>();
            services.TryAddScoped<IResizeService, ResizeService>();
            services.TryAddScoped<IBreakpointService, BreakpointService>();
            services.TryAddScoped<IBrowserWindowSizeProvider, BrowserWindowSizeProvider>();
#pragma warning restore CS0618
            services.TryAddScoped<IBrowserViewportService, BrowserViewportService>();

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
            services.AddMudBlazorResizeListener();
            services.Configure(options);

            return services;
        }

        /// <summary>
        /// Adds a IResizeObserver as a Transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorResizeObserver(this IServiceCollection services)
        {
            services.TryAddTransient<IResizeObserver, ResizeObserver>();

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
            services.AddMudBlazorResizeObserver();
            services.Configure(options);

            return services;
        }

        /// <summary>
        /// Adds a IResizeObserver as a Transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines ResizeObserverOptions for this instance</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        [Obsolete("Use overload with Action<ResizeObserverOptions>. This will be removed in v7.")]
        public static IServiceCollection AddMudBlazorResizeObserver(this IServiceCollection services, ResizeObserverOptions options)
        {
            services.AddMudBlazorResizeObserver(observerOptions =>
            {
                observerOptions.EnableLogging = options.EnableLogging;
                observerOptions.ReportRate = options.ReportRate;
            });

            return services;
        }

        /// <summary>
        /// Adds a IResizeObserverFactory as a scoped dependency.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudBlazorResizeObserverFactory(this IServiceCollection services)
        {
            services.TryAddScoped<IResizeObserverFactory, ResizeObserverFactory>();

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
            services.AddMudBlazorResizeObserverFactory();
            services.Configure(options);

            return services;
        }

        /// <summary>
        /// Adds a IResizeObserverFactory as a scoped dependency.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines ResizeObserverOptions for this instance</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        [Obsolete("Use overload with Action<ResizeObserverOptions>. This will be removed in v7.")]
        public static IServiceCollection AddMudBlazorResizeObserverFactory(this IServiceCollection services, ResizeObserverOptions options)
        {
            services.AddMudBlazorResizeObserverFactory(observerOptions =>
            {
                observerOptions.EnableLogging = options.EnableLogging;
                observerOptions.ReportRate = options.ReportRate;
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
        public static IServiceCollection AddMudPopoverService(this IServiceCollection services)
        {
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
        public static IServiceCollection AddMudPopoverService(this IServiceCollection services, Action<PopoverOptions> options)
        {
            services.AddMudPopoverService();
            services.Configure(options);

            return services;
        }

        /// <summary>
        /// Adds ScrollManager as a transient instance.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="options">Defines PopoverOptions for the application/user</param>
        [Obsolete("Use overload with Action<PopoverOptions>. This will be removed in v7.")]
        public static IServiceCollection AddMudPopoverService(this IServiceCollection services, PopoverOptions options)
        {
            services.AddMudPopoverService(popoverOptions =>
            {
                popoverOptions.ContainerClass = options.ContainerClass;
                popoverOptions.FlipMargin = options.FlipMargin;
                popoverOptions.QueueDelay = options.QueueDelay;
                popoverOptions.ThrowOnDuplicateProvider = options.ThrowOnDuplicateProvider;
                popoverOptions.Mode = options.Mode;
                popoverOptions.PoolSize = options.PoolSize;
                popoverOptions.PoolInitialFill = options.PoolInitialFill;
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
        /// Adds the services required for translations.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public static IServiceCollection AddMudLocalization(this IServiceCollection services)
        {
            services.TryAddTransient<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            services.TryAddTransient<InternalMudLocalizer>();

            return services;
        }

        /// <summary>
        /// Replaces the default <see cref="ILocalizationInterceptor"/> with custom implementation.
        /// </summary>
        /// <typeparam name="TInterceptor">Custom <see cref="ILocalizationInterceptor"/> implentation.</typeparam>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddLocalizationInterceptor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TInterceptor>(this IServiceCollection services) where TInterceptor : class, ILocalizationInterceptor
        {
            services.Replace(ServiceDescriptor.Transient<ILocalizationInterceptor, TInterceptor>());

            return services;
        }

        /// <summary>
        /// Replaces the default <see cref="ILocalizationInterceptor"/> with custom implementation.
        /// </summary>
        /// <typeparam name="TInterceptor">Custom <see cref="ILocalizationInterceptor"/> implentation.</typeparam>
        /// <param name="services">IServiceCollection</param>
        /// <param name="implementationFactory">A factory to create new instances of the <see cref="ILocalizationInterceptor"/> implementation.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddLocalizationInterceptor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TInterceptor>(this IServiceCollection services, Func<IServiceProvider, TInterceptor> implementationFactory) where TInterceptor : class, ILocalizationInterceptor
        {
            services.Replace(ServiceDescriptor.Transient<ILocalizationInterceptor>(implementationFactory));

            return services;
        }

        /// <summary>
        /// Adds common services required by MudBlazor components
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configuration">Defines options for all MudBlazor services.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        [Obsolete("Use overload with Action<MudServicesConfiguration>. This will be removed in v7.")]
        public static IServiceCollection AddMudServices(this IServiceCollection services, MudServicesConfiguration configuration)
        {
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
                .AddMudEventManager()
                .AddMudLocalization();
        }

        /// <summary>
        /// Adds common services required by MudBlazor components
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudServices(this IServiceCollection services)
        {
            return services
                .AddMudBlazorDialog()
                .AddMudBlazorSnackbar()
                .AddMudBlazorResizeListener()
                .AddMudBlazorResizeObserver()
                .AddMudBlazorResizeObserverFactory()
                .AddMudBlazorKeyInterceptor()
                .AddMudBlazorJsEvent()
                .AddMudBlazorScrollManager()
                .AddMudBlazorScrollListener()
                .AddMudBlazorJsApi()
                .AddMudBlazorScrollSpy()
                .AddMudPopoverService()
                .AddMudEventManager()
                .AddMudLocalization();
        }

        /// <summary>
        /// Adds common services required by MudBlazor components
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configuration">Defines options for all MudBlazor services.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMudServices(this IServiceCollection services, Action<MudServicesConfiguration> configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var options = new MudServicesConfiguration();
            configuration(options);

            return services
                .AddMudBlazorDialog()
                .AddMudBlazorSnackbar(snackBarConfiguration =>
                {
                    snackBarConfiguration.ClearAfterNavigation = options.SnackbarConfiguration.ClearAfterNavigation;
                    snackBarConfiguration.MaxDisplayedSnackbars = options.SnackbarConfiguration.MaxDisplayedSnackbars;
                    snackBarConfiguration.NewestOnTop = options.SnackbarConfiguration.NewestOnTop;
                    snackBarConfiguration.PositionClass = options.SnackbarConfiguration.PositionClass;
                    snackBarConfiguration.PreventDuplicates = options.SnackbarConfiguration.PreventDuplicates;
                    snackBarConfiguration.MaximumOpacity = options.SnackbarConfiguration.MaximumOpacity;
                    snackBarConfiguration.ShowTransitionDuration = options.SnackbarConfiguration.ShowTransitionDuration;
                    snackBarConfiguration.VisibleStateDuration = options.SnackbarConfiguration.VisibleStateDuration;
                    snackBarConfiguration.HideTransitionDuration = options.SnackbarConfiguration.HideTransitionDuration;
                    snackBarConfiguration.ShowCloseIcon = options.SnackbarConfiguration.ShowCloseIcon;
                    snackBarConfiguration.RequireInteraction = options.SnackbarConfiguration.RequireInteraction;
                    snackBarConfiguration.BackgroundBlurred = options.SnackbarConfiguration.BackgroundBlurred;
                    snackBarConfiguration.SnackbarVariant = options.SnackbarConfiguration.SnackbarVariant;
                })
                .AddMudBlazorResizeListener(resizeOptions =>
                {
                    resizeOptions.BreakpointDefinitions = options.ResizeOptions.BreakpointDefinitions;
                    resizeOptions.EnableLogging = options.ResizeOptions.EnableLogging;
                    resizeOptions.NotifyOnBreakpointOnly = options.ResizeOptions.NotifyOnBreakpointOnly;
                    resizeOptions.ReportRate = options.ResizeOptions.ReportRate;
                    resizeOptions.SuppressInitEvent = options.ResizeOptions.SuppressInitEvent;
                })
                .AddMudBlazorResizeObserver(observerOptions =>
                {
                    observerOptions.EnableLogging = options.ResizeObserverOptions.EnableLogging;
                    observerOptions.ReportRate = options.ResizeObserverOptions.ReportRate;
                })
                .AddMudBlazorResizeObserverFactory(observerOptions =>
                {
                    observerOptions.EnableLogging = options.ResizeObserverOptions.EnableLogging;
                    observerOptions.ReportRate = options.ResizeObserverOptions.ReportRate;
                })
                .AddMudBlazorKeyInterceptor()
                .AddMudBlazorJsEvent()
                .AddMudBlazorScrollManager()
                .AddMudBlazorScrollListener()
                .AddMudBlazorJsApi()
                .AddMudPopoverService(popoverOptions =>
                {
                    popoverOptions.ContainerClass = options.PopoverOptions.ContainerClass;
                    popoverOptions.FlipMargin = options.PopoverOptions.FlipMargin;
                    popoverOptions.QueueDelay = options.PopoverOptions.QueueDelay;
                    popoverOptions.ThrowOnDuplicateProvider = options.PopoverOptions.ThrowOnDuplicateProvider;
                    popoverOptions.Mode = options.PopoverOptions.Mode;
                    popoverOptions.PoolSize = options.PopoverOptions.PoolSize;
                    popoverOptions.PoolInitialFill = options.PopoverOptions.PoolInitialFill;
                })
                .AddMudBlazorScrollSpy()
                .AddMudEventManager()
                .AddMudLocalization();
        }
    }
}
