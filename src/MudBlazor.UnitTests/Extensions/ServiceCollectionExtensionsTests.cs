// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests;

#nullable enable
[TestFixture]
public class ServiceCollectionExtensionsTests
{
    [Test]
    public void AddMudBlazorDialog_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMudBlazorDialog();
        var serviceProvider = services.BuildServiceProvider();
        var dialogService = serviceProvider.GetService<IDialogService>();

        // Assert
        Assert.IsNotNull(dialogService);
    }

    [Test]
    public void AddMudBlazorSnackBar_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<NavigationManager, MockNavigationManager>()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudBlazorSnackbar();
        var serviceProvider = services.BuildServiceProvider();
        var snackBarService = serviceProvider.GetService<ISnackbar>();

        // Assert
        Assert.IsNotNull(snackBarService);
    }

    [Test]
    public void AddMudBlazorSnackBar_ShouldRegisterServices_WithConfigurationAction()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<NavigationManager, MockNavigationManager>()
            .AddSingleton<IJSRuntime, MockJsRuntime>();
        SnackbarConfiguration? expectedOptions = null;

        // Act
        services.AddMudBlazorSnackbar(options =>
        {
            options.PositionClass = "position_class";
            options.ClearAfterNavigation = true;
            options.ShowCloseIcon = false;
            expectedOptions = options;
        });
        var serviceProvider = services.BuildServiceProvider();
        var snackBarService = serviceProvider.GetService<ISnackbar>();
        var options = serviceProvider.GetRequiredService<IOptions<SnackbarConfiguration>>();
        var actualOptions = options.Value;

        // Assert
        Assert.IsNotNull(snackBarService);
        Assert.AreSame(expectedOptions, actualOptions);
    }


    [Test]
    public void AddMudBlazorResizeListener_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudBlazorResizeListener();
        var serviceProvider = services.BuildServiceProvider();
        var browserViewportService = serviceProvider.GetService<IBrowserViewportService>();
#pragma warning disable CS0618
        var resizeListenerService = serviceProvider.GetService<IResizeListenerService>();
        var breakpointService = serviceProvider.GetService<IBreakpointService>();
        var browserWindowSizeProvider = serviceProvider.GetService<IBrowserWindowSizeProvider>();
        var resizeService = serviceProvider.GetService<IResizeService>();
#pragma warning restore CS0618

        // Assert
        Assert.IsNotNull(browserViewportService);
        Assert.IsNotNull(resizeListenerService);
        Assert.IsNotNull(browserWindowSizeProvider);
        Assert.IsNotNull(resizeService);
        Assert.IsNotNull(breakpointService);
    }

    [Test]
    public void AddMudBlazorResizeListener_ShouldRegisterServices_WithOptionsAction()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IJSRuntime, MockJsRuntime>();
        ResizeOptions? expectedOptions = null;

        // Act
        services.AddMudBlazorResizeListener(options =>
        {
            options.BreakpointDefinitions = new Dictionary<Breakpoint, int>
            {
                { Breakpoint.Lg, 500 }
            };
            options.EnableLogging = true;
            options.NotifyOnBreakpointOnly = false;
            options.ReportRate = 100;
            options.SuppressInitEvent = false;
            expectedOptions = options;
        });
        var serviceProvider = services.BuildServiceProvider();
        var browserViewportService = serviceProvider.GetService<IBrowserViewportService>();
#pragma warning disable CS0618
        var resizeListenerService = serviceProvider.GetService<IResizeListenerService>();
        var breakpointService = serviceProvider.GetService<IBreakpointService>();
        var browserWindowSizeProvider = serviceProvider.GetService<IBrowserWindowSizeProvider>();
        var resizeService = serviceProvider.GetService<IResizeService>();
#pragma warning restore CS0618
        var options = serviceProvider.GetRequiredService<IOptions<ResizeOptions>>();
        var actualOptions = options.Value;

        // Assert
        Assert.IsNotNull(browserViewportService);
        Assert.IsNotNull(resizeListenerService);
        Assert.IsNotNull(browserWindowSizeProvider);
        Assert.IsNotNull(breakpointService);
        Assert.IsNotNull(resizeService);
        Assert.IsNotNull(expectedOptions);
        Assert.AreSame(expectedOptions, actualOptions);
    }

    [Test]
    public void AddMudBlazorResizeObserver_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudBlazorResizeObserver();
        var serviceProvider = services.BuildServiceProvider();
        var resizeObserver = serviceProvider.GetService<IResizeObserver>();

        // Assert
        Assert.IsNotNull(resizeObserver);
    }

    [Test]
    public void AddMudBlazorResizeObserver_ShouldRegisterServices_WithOptionsAction()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<IJSRuntime, MockJsRuntime>();
        ResizeObserverOptions? expectedOptions = null;

        // Act
        services.AddMudBlazorResizeObserver(options =>
        {
            options.EnableLogging = true;
            options.ReportRate = 500;
            expectedOptions = options;
        });
        var serviceProvider = services.BuildServiceProvider();
        var resizeObserver = serviceProvider.GetService<IResizeObserver>();
        var options = serviceProvider.GetRequiredService<IOptions<ResizeObserverOptions>>();
        var actualOptions = options.Value;

        // Assert
        Assert.IsNotNull(resizeObserver);
        Assert.IsNotNull(expectedOptions);
        Assert.AreSame(expectedOptions, actualOptions);
    }

    [Test]
    public void AddMudBlazorResizeObserverFactory_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMudBlazorResizeObserverFactory();
        var serviceProvider = services.BuildServiceProvider();
        var resizeObserverFactory = serviceProvider.GetService<IResizeObserverFactory>();

        // Assert
        Assert.IsNotNull(resizeObserverFactory);
    }

    [Test]
    public void AddMudBlazorResizeObserverFactory_ShouldRegisterServices_WithOptionsAction()
    {
        // Arrange
        var services = new ServiceCollection();
        ResizeObserverOptions? expectedOptions = null;

        // Act
        services.AddMudBlazorResizeObserverFactory(options =>
        {
            options.EnableLogging = true;
            options.ReportRate = 500;
            expectedOptions = options;
        });
        var serviceProvider = services.BuildServiceProvider();
        var resizeObserverFactory = serviceProvider.GetService<IResizeObserverFactory>();
        var options = serviceProvider.GetRequiredService<IOptions<ResizeObserverOptions>>();
        var actualOptions = options.Value;

        // Assert
        Assert.IsNotNull(resizeObserverFactory);
        Assert.IsNotNull(expectedOptions);
        Assert.AreSame(expectedOptions, actualOptions);
    }

    [Test]
    public void AddMudBlazorKeyInterceptor_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudBlazorKeyInterceptor();
        var serviceProvider = services.BuildServiceProvider();
        var keyInterceptor = serviceProvider.GetService<IKeyInterceptor>();
        var keyInterceptorFactory = serviceProvider.GetService<IKeyInterceptorFactory>();

        // Assert
        Assert.IsNotNull(keyInterceptor);
        Assert.IsNotNull(keyInterceptorFactory);
    }

    [Test]
    public void AddMudBlazorJsEvent_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudBlazorJsEvent();
        var serviceProvider = services.BuildServiceProvider();
        var jsEvent = serviceProvider.GetService<IJsEvent>();
        var jsEventFactory = serviceProvider.GetService<IJsEventFactory>();

        // Assert
        Assert.IsNotNull(jsEvent);
        Assert.IsNotNull(jsEventFactory);
    }

    [Test]
    public void AddMudBlazorScrollManager_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudBlazorScrollManager();
        var serviceProvider = services.BuildServiceProvider();
        var scrollManager = serviceProvider.GetService<IScrollManager>();

        // Assert
        Assert.IsNotNull(scrollManager);
    }

    [Test]
    public void AddMudPopoverService_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudPopoverService();
        var serviceProvider = services.BuildServiceProvider();
#pragma warning disable CS0618
        var mudPopoverService = serviceProvider.GetService<IMudPopoverService>();
#pragma warning restore CS0618
        var popoverService = serviceProvider.GetService<IPopoverService>();

        // Assert
        Assert.IsNotNull(mudPopoverService);
        Assert.IsNotNull(popoverService);
    }

    [Test]
    public void AddMudPopoverService_ShouldRegisterServices_WithOptionsAction()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IJSRuntime, MockJsRuntime>();
        PopoverOptions? expectedOptions = null;

        // Act
        services.AddMudPopoverService(options =>
        {
            options.QueueDelay = TimeSpan.FromSeconds(5);
            options.ContainerClass = "container_class";
            options.FlipMargin = 100;
            options.ThrowOnDuplicateProvider = false;
            options.Mode = PopoverMode.Legacy;
            expectedOptions = options;
        });
        var serviceProvider = services.BuildServiceProvider();
#pragma warning disable CS0618
        var mudPopoverService = serviceProvider.GetService<IMudPopoverService>();
#pragma warning restore CS0618
        var popoverService = serviceProvider.GetService<IPopoverService>();
        var options = serviceProvider.GetRequiredService<IOptions<PopoverOptions>>();
        var actualOptions = options.Value;

        // Assert
        Assert.IsNotNull(mudPopoverService);
        Assert.IsNotNull(popoverService);
        Assert.IsNotNull(expectedOptions);
        Assert.AreSame(expectedOptions, actualOptions);
    }

    [Test]
    public void AddMudBlazorScrollListener_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudBlazorScrollListener();
        var serviceProvider = services.BuildServiceProvider();
        var scrollListener = serviceProvider.GetService<IScrollListener>();
        var scrollListenerFactory = serviceProvider.GetService<IScrollListenerFactory>();

        // Assert
        Assert.IsNotNull(scrollListener);
        Assert.IsNotNull(scrollListenerFactory);
    }

    [Test]
    public void AddMudBlazorScrollSpy_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudBlazorScrollSpy();
        var serviceProvider = services.BuildServiceProvider();
        var scrollSpy = serviceProvider.GetService<IScrollSpy>();
        var scrollSpyFactory = serviceProvider.GetService<IScrollSpyFactory>();

        // Assert
        Assert.IsNotNull(scrollSpy);
        Assert.IsNotNull(scrollSpyFactory);
    }

    [Test]
    public void AddMudBlazorJsApi_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudBlazorJsApi();
        var serviceProvider = services.BuildServiceProvider();
        var jsApiService = serviceProvider.GetService<IJsApiService>();

        // Assert
        Assert.IsNotNull(jsApiService);
    }

    [Test]
    public void AddMudEventManager_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudEventManager();
        var serviceProvider = services.BuildServiceProvider();
        var eventListener = serviceProvider.GetService<IEventListener>();
        var eventListenerFactory = serviceProvider.GetService<IEventListenerFactory>();

        // Assert
        Assert.IsNotNull(eventListener);
        Assert.IsNotNull(eventListenerFactory);
    }

    [Test]
    public void AddMudLocalization_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudLocalization();
        var serviceProvider = services.BuildServiceProvider();
        var mudLocalizer = serviceProvider.GetService<InternalMudLocalizer>();

        // Assert
        Assert.IsNotNull(mudLocalizer);
    }

    [Test]
    public void AddMudServices_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<NavigationManager, MockNavigationManager>()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        // Act
        services.AddMudServices();
        var serviceProvider = services.BuildServiceProvider();
        var dialogService = serviceProvider.GetService<IDialogService>();
        var snackBarService = serviceProvider.GetService<ISnackbar>();
#pragma warning disable CS0618
        var resizeListenerService = serviceProvider.GetService<IResizeListenerService>();
        var breakpointService = serviceProvider.GetService<IBreakpointService>();
        var browserWindowSizeProvider = serviceProvider.GetService<IBrowserWindowSizeProvider>();
        var resizeService = serviceProvider.GetService<IResizeService>();
#pragma warning restore CS0618
        var browserViewportService = serviceProvider.GetService<IBrowserViewportService>();
        var resizeObserver = serviceProvider.GetService<IResizeObserver>();
        var resizeObserverFactory = serviceProvider.GetService<IResizeObserverFactory>();
        var keyInterceptor = serviceProvider.GetService<IKeyInterceptor>();
        var keyInterceptorFactory = serviceProvider.GetService<IKeyInterceptorFactory>();
        var jsEvent = serviceProvider.GetService<IJsEvent>();
        var jsEventFactory = serviceProvider.GetService<IJsEventFactory>();
        var scrollManager = serviceProvider.GetService<IScrollManager>();
#pragma warning disable CS0618
        var mudPopoverService = serviceProvider.GetService<IMudPopoverService>();
#pragma warning restore CS0618
        var popoverService = serviceProvider.GetService<IPopoverService>();
        var scrollListener = serviceProvider.GetService<IScrollListener>();
        var scrollListenerFactory = serviceProvider.GetService<IScrollListenerFactory>();
        var scrollSpy = serviceProvider.GetService<IScrollSpy>();
        var scrollSpyFactory = serviceProvider.GetService<IScrollSpyFactory>();
        var jsApiService = serviceProvider.GetService<IJsApiService>();
        var eventListener = serviceProvider.GetService<IEventListener>();
        var eventListenerFactory = serviceProvider.GetService<IEventListenerFactory>();
        var mudLocalizer = serviceProvider.GetService<InternalMudLocalizer>();

        // Assert
        Assert.IsNotNull(dialogService);
        Assert.IsNotNull(snackBarService);
        Assert.IsNotNull(resizeListenerService);
        Assert.IsNotNull(browserViewportService);
        Assert.IsNotNull(browserWindowSizeProvider);
        Assert.IsNotNull(resizeService);
        Assert.IsNotNull(breakpointService);
        Assert.IsNotNull(resizeObserver);
        Assert.IsNotNull(resizeObserverFactory);
        Assert.IsNotNull(keyInterceptor);
        Assert.IsNotNull(keyInterceptorFactory);
        Assert.IsNotNull(jsEvent);
        Assert.IsNotNull(jsEventFactory);
        Assert.IsNotNull(scrollManager);
        Assert.IsNotNull(mudPopoverService);
        Assert.IsNotNull(popoverService);
        Assert.IsNotNull(scrollListener);
        Assert.IsNotNull(scrollListenerFactory);
        Assert.IsNotNull(scrollSpy);
        Assert.IsNotNull(scrollSpyFactory);
        Assert.IsNotNull(jsApiService);
        Assert.IsNotNull(eventListener);
        Assert.IsNotNull(eventListenerFactory);
        Assert.IsNotNull(mudLocalizer);
    }

    [Test]
    public void AddMudServices_ShouldRegisterAllServices_WithOptionsAction()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<NavigationManager, MockNavigationManager>()
            .AddSingleton<IJSRuntime, MockJsRuntime>();
        MudServicesConfiguration? expectedOptions = null;

        // Act
        services.AddMudServices(options =>
        {
            // SnackBarConfiguration
            options.SnackbarConfiguration.ClearAfterNavigation = true;
            options.SnackbarConfiguration.MaxDisplayedSnackbars = 1;
            options.SnackbarConfiguration.NewestOnTop = true;
            options.SnackbarConfiguration.PositionClass = "position_class";
            options.SnackbarConfiguration.PreventDuplicates = true;
            options.SnackbarConfiguration.MaximumOpacity = 2;
            options.SnackbarConfiguration.ShowTransitionDuration = 3;
            options.SnackbarConfiguration.VisibleStateDuration = 4;
            options.SnackbarConfiguration.HideTransitionDuration = 5;
            options.SnackbarConfiguration.ShowCloseIcon = false;
            options.SnackbarConfiguration.RequireInteraction = true;
            options.SnackbarConfiguration.BackgroundBlurred = true;
            options.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;

            // ResizeOptions
            options.ResizeOptions.BreakpointDefinitions = new Dictionary<Breakpoint, int>
            {
                { Breakpoint.Lg, 500 }
            };
            options.ResizeOptions.EnableLogging = true;
            options.ResizeOptions.NotifyOnBreakpointOnly = false;
            options.ResizeOptions.ReportRate = 100;
            options.ResizeOptions.SuppressInitEvent = false;

            // ResizeObserverOptions
            options.ResizeObserverOptions.EnableLogging = true;
            options.ResizeObserverOptions.ReportRate = 500;

            // PopoverOptions
            options.PopoverOptions.QueueDelay = TimeSpan.FromSeconds(5);
            options.PopoverOptions.ContainerClass = "container_class";
            options.PopoverOptions.FlipMargin = 100;
            options.PopoverOptions.ThrowOnDuplicateProvider = false;
            options.PopoverOptions.Mode = PopoverMode.Legacy;

            expectedOptions = options;
        });
        var serviceProvider = services.BuildServiceProvider();
        var dialogService = serviceProvider.GetService<IDialogService>();
        var snackBarService = serviceProvider.GetService<ISnackbar>();
#pragma warning disable CS0618
        var breakpointService = serviceProvider.GetService<IBreakpointService>();
        var resizeListenerService = serviceProvider.GetService<IResizeListenerService>();
        var browserWindowSizeProvider = serviceProvider.GetService<IBrowserWindowSizeProvider>();
        var resizeService = serviceProvider.GetService<IResizeService>();
#pragma warning restore CS0618
        var browserViewportService = serviceProvider.GetService<IBrowserViewportService>();
        var resizeObserver = serviceProvider.GetService<IResizeObserver>();
        var resizeObserverFactory = serviceProvider.GetService<IResizeObserverFactory>();
        var keyInterceptor = serviceProvider.GetService<IKeyInterceptor>();
        var keyInterceptorFactory = serviceProvider.GetService<IKeyInterceptorFactory>();
        var jsEvent = serviceProvider.GetService<IJsEvent>();
        var jsEventFactory = serviceProvider.GetService<IJsEventFactory>();
        var scrollManager = serviceProvider.GetService<IScrollManager>();
#pragma warning disable CS0618
        var mudPopoverService = serviceProvider.GetService<IMudPopoverService>();
#pragma warning restore CS0618
        var popoverService = serviceProvider.GetService<IPopoverService>();
        var scrollListener = serviceProvider.GetService<IScrollListener>();
        var scrollListenerFactory = serviceProvider.GetService<IScrollListenerFactory>();
        var scrollSpy = serviceProvider.GetService<IScrollSpy>();
        var scrollSpyFactory = serviceProvider.GetService<IScrollSpyFactory>();
        var jsApiService = serviceProvider.GetService<IJsApiService>();
        var eventListener = serviceProvider.GetService<IEventListener>();
        var eventListenerFactory = serviceProvider.GetService<IEventListenerFactory>();
        var mudLocalizer = serviceProvider.GetService<InternalMudLocalizer>();
        var snackBarOptions = serviceProvider.GetRequiredService<IOptions<SnackbarConfiguration>>();
        var resizeOptions = serviceProvider.GetRequiredService<IOptions<ResizeOptions>>();
        var resizeObserverOptions = serviceProvider.GetRequiredService<IOptions<ResizeObserverOptions>>();
        var popoverOptions = serviceProvider.GetRequiredService<IOptions<PopoverOptions>>();
        var actualSnackBarOptions = snackBarOptions.Value;
        var actualResizeOptions = resizeOptions.Value;
        var actualResizeObserverOptions = resizeObserverOptions.Value;
        var actualPopoverOptions = popoverOptions.Value;

        // Assert
        Assert.IsNotNull(dialogService);
        Assert.IsNotNull(snackBarService);
        Assert.IsNotNull(resizeListenerService);
        Assert.IsNotNull(browserViewportService);
        Assert.IsNotNull(browserWindowSizeProvider);
        Assert.IsNotNull(resizeService);
        Assert.IsNotNull(breakpointService);
        Assert.IsNotNull(resizeObserver);
        Assert.IsNotNull(resizeObserverFactory);
        Assert.IsNotNull(keyInterceptor);
        Assert.IsNotNull(keyInterceptorFactory);
        Assert.IsNotNull(jsEvent);
        Assert.IsNotNull(jsEventFactory);
        Assert.IsNotNull(scrollManager);
        Assert.IsNotNull(mudPopoverService);
        Assert.IsNotNull(popoverService);
        Assert.IsNotNull(scrollListener);
        Assert.IsNotNull(scrollListenerFactory);
        Assert.IsNotNull(scrollSpy);
        Assert.IsNotNull(scrollSpyFactory);
        Assert.IsNotNull(jsApiService);
        Assert.IsNotNull(eventListener);
        Assert.IsNotNull(eventListenerFactory);
        Assert.IsNotNull(mudLocalizer);

        // We can't check reference here, instead we need to check each value
        Assert.AreEqual(expectedOptions!.PopoverOptions.QueueDelay, actualPopoverOptions.QueueDelay);
        Assert.AreEqual(expectedOptions.PopoverOptions.ContainerClass, actualPopoverOptions.ContainerClass);
        Assert.AreEqual(expectedOptions.PopoverOptions.FlipMargin, actualPopoverOptions.FlipMargin);
        Assert.AreEqual(expectedOptions.PopoverOptions.ThrowOnDuplicateProvider, actualPopoverOptions.ThrowOnDuplicateProvider);
        Assert.AreEqual(expectedOptions.PopoverOptions.Mode, actualPopoverOptions.Mode);

        Assert.AreEqual(expectedOptions.ResizeObserverOptions.EnableLogging, actualResizeObserverOptions.EnableLogging);
        Assert.AreEqual(expectedOptions.ResizeObserverOptions.ReportRate, actualResizeObserverOptions.ReportRate);

        Assert.AreEqual(expectedOptions.ResizeOptions.BreakpointDefinitions, actualResizeOptions.BreakpointDefinitions);
        Assert.AreEqual(expectedOptions.ResizeOptions.EnableLogging, actualResizeOptions.EnableLogging);
        Assert.AreEqual(expectedOptions.ResizeOptions.NotifyOnBreakpointOnly, actualResizeOptions.NotifyOnBreakpointOnly);
        Assert.AreEqual(expectedOptions.ResizeOptions.ReportRate, actualResizeOptions.ReportRate);
        Assert.AreEqual(expectedOptions.ResizeOptions.SuppressInitEvent, actualResizeOptions.SuppressInitEvent);

        Assert.AreEqual(expectedOptions.SnackbarConfiguration.ClearAfterNavigation, actualSnackBarOptions.ClearAfterNavigation);
        Assert.AreEqual(expectedOptions.SnackbarConfiguration.MaxDisplayedSnackbars, actualSnackBarOptions.MaxDisplayedSnackbars);
        Assert.AreEqual(expectedOptions.SnackbarConfiguration.NewestOnTop, actualSnackBarOptions.NewestOnTop);
        Assert.AreEqual(expectedOptions.SnackbarConfiguration.PositionClass, actualSnackBarOptions.PositionClass);
        Assert.AreEqual(expectedOptions.SnackbarConfiguration.PreventDuplicates, actualSnackBarOptions.PreventDuplicates);
        Assert.AreEqual(expectedOptions.SnackbarConfiguration.MaximumOpacity, actualSnackBarOptions.MaximumOpacity);
        Assert.AreEqual(expectedOptions.SnackbarConfiguration.ShowTransitionDuration, actualSnackBarOptions.ShowTransitionDuration);
        Assert.AreEqual(expectedOptions.SnackbarConfiguration.VisibleStateDuration, actualSnackBarOptions.VisibleStateDuration);
        Assert.AreEqual(expectedOptions.SnackbarConfiguration.HideTransitionDuration, actualSnackBarOptions.HideTransitionDuration);
        Assert.AreEqual(expectedOptions.SnackbarConfiguration.ShowCloseIcon, actualSnackBarOptions.ShowCloseIcon);
        Assert.AreEqual(expectedOptions.SnackbarConfiguration.RequireInteraction, actualSnackBarOptions.RequireInteraction);
        Assert.AreEqual(expectedOptions.SnackbarConfiguration.BackgroundBlurred, actualSnackBarOptions.BackgroundBlurred);
        Assert.AreEqual(expectedOptions.SnackbarConfiguration.SnackbarVariant, actualSnackBarOptions.SnackbarVariant);
    }
}
