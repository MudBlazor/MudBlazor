// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FluentAssertions;
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
        dialogService.Should().NotBeNull();
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
        snackBarService.Should().NotBeNull();
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
        snackBarService.Should().NotBeNull();
        actualOptions.Should().BeSameAs(expectedOptions);
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

        // Assert
        browserViewportService.Should().NotBeNull();
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
        var options = serviceProvider.GetRequiredService<IOptions<ResizeOptions>>();
        var actualOptions = options.Value;

        // Assert
        browserViewportService.Should().NotBeNull();
        expectedOptions.Should().NotBeNull();
        actualOptions.Should().BeSameAs(expectedOptions);
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
        resizeObserver.Should().NotBeNull();
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
        resizeObserver.Should().NotBeNull();
        expectedOptions.Should().NotBeNull();
        actualOptions.Should().BeSameAs(expectedOptions);
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
        resizeObserverFactory.Should().NotBeNull();
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
        resizeObserverFactory.Should().NotBeNull();
        expectedOptions.Should().NotBeNull();
        actualOptions.Should().BeSameAs(expectedOptions);
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
        keyInterceptor.Should().NotBeNull();
        keyInterceptorFactory.Should().NotBeNull();
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
        jsEvent.Should().NotBeNull();
        jsEventFactory.Should().NotBeNull();
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
        scrollManager.Should().NotBeNull();
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
        mudPopoverService.Should().NotBeNull();
        popoverService.Should().NotBeNull();
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
            options.PoolSize = 200;
            options.PoolInitialFill = 10;
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
        mudPopoverService.Should().NotBeNull();
        popoverService.Should().NotBeNull();
        expectedOptions.Should().NotBeNull();
        actualOptions.Should().BeSameAs(expectedOptions);
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
        scrollListener.Should().NotBeNull();
        scrollListenerFactory.Should().NotBeNull();
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
        scrollSpy.Should().NotBeNull();
        scrollSpyFactory.Should().NotBeNull();
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
        jsApiService.Should().NotBeNull();
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
        eventListener.Should().NotBeNull();
        eventListenerFactory.Should().NotBeNull();
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
        var localizationInterceptor = serviceProvider.GetService<ILocalizationInterceptor>();

        // Assert
        mudLocalizer.Should().NotBeNull();
        localizationInterceptor.Should().NotBeNull();
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
        var localizationInterceptor = serviceProvider.GetService<ILocalizationInterceptor>();

        // Assert
        dialogService.Should().NotBeNull();
        snackBarService.Should().NotBeNull();
        browserViewportService.Should().NotBeNull();
        resizeObserver.Should().NotBeNull();
        resizeObserverFactory.Should().NotBeNull();
        keyInterceptor.Should().NotBeNull();
        keyInterceptorFactory.Should().NotBeNull();
        jsEvent.Should().NotBeNull();
        jsEventFactory.Should().NotBeNull();
        scrollManager.Should().NotBeNull();
        mudPopoverService.Should().NotBeNull();
        popoverService.Should().NotBeNull();
        scrollListener.Should().NotBeNull();
        scrollListenerFactory.Should().NotBeNull();
        scrollSpy.Should().NotBeNull();
        scrollSpyFactory.Should().NotBeNull();
        jsApiService.Should().NotBeNull();
        eventListener.Should().NotBeNull();
        eventListenerFactory.Should().NotBeNull();
        mudLocalizer.Should().NotBeNull();
        localizationInterceptor.Should().NotBeNull();
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
            options.PopoverOptions.PoolSize = 300;
            options.PopoverOptions.PoolInitialFill = 5;

            expectedOptions = options;
        });
        var serviceProvider = services.BuildServiceProvider();
        var dialogService = serviceProvider.GetService<IDialogService>();
        var snackBarService = serviceProvider.GetService<ISnackbar>();
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
        var localizationInterceptor = serviceProvider.GetService<ILocalizationInterceptor>();
        var snackBarOptions = serviceProvider.GetRequiredService<IOptions<SnackbarConfiguration>>();
        var resizeOptions = serviceProvider.GetRequiredService<IOptions<ResizeOptions>>();
        var resizeObserverOptions = serviceProvider.GetRequiredService<IOptions<ResizeObserverOptions>>();
        var popoverOptions = serviceProvider.GetRequiredService<IOptions<PopoverOptions>>();
        var actualSnackBarOptions = snackBarOptions.Value;
        var actualResizeOptions = resizeOptions.Value;
        var actualResizeObserverOptions = resizeObserverOptions.Value;
        var actualPopoverOptions = popoverOptions.Value;

        // Assert
        dialogService.Should().NotBeNull();
        snackBarService.Should().NotBeNull();
        browserViewportService.Should().NotBeNull();
        resizeObserver.Should().NotBeNull();
        resizeObserverFactory.Should().NotBeNull();
        keyInterceptor.Should().NotBeNull();
        keyInterceptorFactory.Should().NotBeNull();
        jsEvent.Should().NotBeNull();
        jsEventFactory.Should().NotBeNull();
        scrollManager.Should().NotBeNull();
        mudPopoverService.Should().NotBeNull();
        popoverService.Should().NotBeNull();
        scrollListener.Should().NotBeNull();
        scrollListenerFactory.Should().NotBeNull();
        scrollSpy.Should().NotBeNull();
        scrollSpyFactory.Should().NotBeNull();
        jsApiService.Should().NotBeNull();
        eventListener.Should().NotBeNull();
        eventListenerFactory.Should().NotBeNull();
        mudLocalizer.Should().NotBeNull();
        localizationInterceptor.Should().NotBeNull();

        // We can't check reference here, instead we need to check each value
        actualPopoverOptions.QueueDelay.Should().Be(expectedOptions!.PopoverOptions.QueueDelay);
        actualPopoverOptions.ContainerClass.Should().Be(expectedOptions.PopoverOptions.ContainerClass);
        actualPopoverOptions.FlipMargin.Should().Be(expectedOptions.PopoverOptions.FlipMargin);
        actualPopoverOptions.ThrowOnDuplicateProvider.Should().Be(expectedOptions.PopoverOptions.ThrowOnDuplicateProvider);
        actualPopoverOptions.Mode.Should().Be(expectedOptions.PopoverOptions.Mode);
        actualPopoverOptions.PoolSize.Should().Be(expectedOptions.PopoverOptions.PoolSize);
        actualPopoverOptions.PoolInitialFill.Should().Be(expectedOptions.PopoverOptions.PoolInitialFill);

        actualResizeObserverOptions.EnableLogging.Should().Be(expectedOptions.ResizeObserverOptions.EnableLogging);
        actualResizeObserverOptions.ReportRate.Should().Be(expectedOptions.ResizeObserverOptions.ReportRate);

        actualResizeOptions.BreakpointDefinitions.Should().BeSameAs(expectedOptions.ResizeOptions.BreakpointDefinitions);
        actualResizeOptions.EnableLogging.Should().Be(expectedOptions.ResizeOptions.EnableLogging);
        actualResizeOptions.NotifyOnBreakpointOnly.Should().Be(expectedOptions.ResizeOptions.NotifyOnBreakpointOnly);
        actualResizeOptions.ReportRate.Should().Be(expectedOptions.ResizeOptions.ReportRate);
        actualResizeOptions.SuppressInitEvent.Should().Be(expectedOptions.ResizeOptions.SuppressInitEvent);

        actualSnackBarOptions.ClearAfterNavigation.Should().Be(expectedOptions.SnackbarConfiguration.ClearAfterNavigation);
        actualSnackBarOptions.MaxDisplayedSnackbars.Should().Be(expectedOptions.SnackbarConfiguration.MaxDisplayedSnackbars);
        actualSnackBarOptions.NewestOnTop.Should().Be(expectedOptions.SnackbarConfiguration.NewestOnTop);
        actualSnackBarOptions.PositionClass.Should().Be(expectedOptions.SnackbarConfiguration.PositionClass);
        actualSnackBarOptions.PreventDuplicates.Should().Be(expectedOptions.SnackbarConfiguration.PreventDuplicates);
        actualSnackBarOptions.MaximumOpacity.Should().Be(expectedOptions.SnackbarConfiguration.MaximumOpacity);
        actualSnackBarOptions.ShowTransitionDuration.Should().Be(expectedOptions.SnackbarConfiguration.ShowTransitionDuration);
        actualSnackBarOptions.VisibleStateDuration.Should().Be(expectedOptions.SnackbarConfiguration.VisibleStateDuration);
        actualSnackBarOptions.HideTransitionDuration.Should().Be(expectedOptions.SnackbarConfiguration.HideTransitionDuration);
        actualSnackBarOptions.ShowCloseIcon.Should().Be(expectedOptions.SnackbarConfiguration.ShowCloseIcon);
        actualSnackBarOptions.RequireInteraction.Should().Be(expectedOptions.SnackbarConfiguration.RequireInteraction);
        actualSnackBarOptions.BackgroundBlurred.Should().Be(expectedOptions.SnackbarConfiguration.BackgroundBlurred);
        actualSnackBarOptions.SnackbarVariant.Should().Be(expectedOptions.SnackbarConfiguration.SnackbarVariant);
    }
}
