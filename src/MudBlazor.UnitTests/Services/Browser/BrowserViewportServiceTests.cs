// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.UnitTests.Services.Browser.Mocks;
using MudBlazor.UnitTests.TestData;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.Browser;

#nullable enable
[TestFixture]
public class BrowserViewportServiceTests
{
    [Test]
    public async Task SubscribeAsync_WithObserver_NoFireImmediately()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var observer = new BrowserViewportObserverMock();

        // Act
        await service.SubscribeAsync(observer, fireImmediately: false);

        // Assert
        observer.Notifications.Count.Should().Be(0);
        service.ObserversCount.Should().Be(1);
        jsRuntimeMock.Verify(x => x.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<object[]>()), Times.Never);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_WithAction_NoFireImmediately()
    {
        // Arrange
        var observerId = Guid.NewGuid();
        var lambdaInvokedCount = 0;
        var jsRuntimeMock = new Mock<IJSRuntime>();
        void Lambda(BrowserViewportEventArgs args) => lambdaInvokedCount++;
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);

        // Act
        await service.SubscribeAsync(observerId, Lambda, fireImmediately: false);

        // Assert
        lambdaInvokedCount.Should().Be(0);
        service.ObserversCount.Should().Be(1);
        jsRuntimeMock.Verify(x => x.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<object[]>()), Times.Never);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_WithFunc_NoFireImmediately()
    {
        // Arrange
        var observerId = Guid.NewGuid();
        var observerNotifications = new List<BrowserViewportEventArgs>();
        var jsRuntimeMock = new Mock<IJSRuntime>();

        Task LambdaAsync(BrowserViewportEventArgs args)
        {
            observerNotifications.Add(args);

            return Task.CompletedTask;
        }

        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);

        // Act
        await service.SubscribeAsync(observerId, LambdaAsync, fireImmediately: false);

        // Assert
        observerNotifications.Count.Should().Be(0);
        service.ObserversCount.Should().Be(1);
        jsRuntimeMock.Verify(x => x.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<object[]>()), Times.Never);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_WithObserver_FireImmediately()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var observer = new BrowserViewportObserverMock();

        // Act
        await service.SubscribeAsync(observer, fireImmediately: true);

        // Assert
        var firstNotification = observer.Notifications[0];
        firstNotification.IsImmediate.Should().BeTrue();
        observer.Notifications.Count.Should().Be(1);
        service.ObserversCount.Should().Be(1);
        jsRuntimeMock.Verify(x => x.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<object[]>()), Times.Exactly(2));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_WithAction_FireImmediately()
    {
        // Arrange
        var observerId = Guid.NewGuid();
        var observerNotifications = new List<BrowserViewportEventArgs>();
        var jsRuntimeMock = new Mock<IJSRuntime>();
        void Lambda(BrowserViewportEventArgs args) => observerNotifications.Add(args);
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);

        // Act
        await service.SubscribeAsync(observerId, Lambda, fireImmediately: true);

        // Assert
        var firstNotification = observerNotifications[0];
        firstNotification.IsImmediate.Should().BeTrue();
        observerNotifications.Count.Should().Be(1);
        service.ObserversCount.Should().Be(1);
        jsRuntimeMock.Verify(x => x.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<object[]>()), Times.Exactly(2));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_WithFunc_FireImmediately()
    {
        // Arrange
        var observerId = Guid.NewGuid();
        var observerNotifications = new List<BrowserViewportEventArgs>();
        var jsRuntimeMock = new Mock<IJSRuntime>();

        Task LambdaAsync(BrowserViewportEventArgs args)
        {
            observerNotifications.Add(args);

            return Task.CompletedTask;
        }

        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);

        // Act
        await service.SubscribeAsync(observerId, LambdaAsync, fireImmediately: true);

        // Assert
        var firstNotification = observerNotifications[0];
        firstNotification.IsImmediate.Should().BeTrue();
        observerNotifications.Count.Should().Be(1);
        service.ObserversCount.Should().Be(1);
        jsRuntimeMock.Verify(x => x.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<object[]>()), Times.Exactly(2));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_ReSubscribeWithSameObserverIdentifier()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var observer = new BrowserViewportObserverMock();

        // Act
        await service.SubscribeAsync(observer, fireImmediately: true);
        await service.SubscribeAsync(observer, fireImmediately: true);
        await service.SubscribeAsync(observer, fireImmediately: true);

        // Assert
        var firstNotification = observer.Notifications[0];
        firstNotification.IsImmediate.Should().BeTrue();
        observer.Notifications.Count.Should().Be(1);
        service.ObserversCount.Should().Be(1);
        jsRuntimeMock.Verify(x => x.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<object[]>()), Times.Exactly(2));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_ReSubscribeWithSameObserverIdentifierButDifferentOptions()
    {
        // Note: After the first subscription, passing other options or modifying the reference will have no effect. This scenario is not supported in our codebase and in JavaScript, as documented in the Subscribe method.
        // The BrowserViewportSubscription subscription will use the first set of options passed, which may be mutated after the GetDefaultOrUserDefinedBreakpointDefinition method (if necessary for user-defined breakpoints).
        // The original reference will remain untouched.
        // This test is "quirky", but it is essential to document and test it since it is easy to introduce errors when working with references.

        // Arrange
        var observerId = Guid.NewGuid();
        var observerNotifications = new List<BrowserViewportEventArgs>();
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var options1 = new ResizeOptions { ReportRate = 1, EnableLogging = true };
        var options2 = new ResizeOptions { ReportRate = 2, SuppressInitEvent = false };
        var options3 = new ResizeOptions { ReportRate = 3, NotifyOnBreakpointOnly = false };
        void Lambda(BrowserViewportEventArgs args) => observerNotifications.Add(args);

        // Act
        await service.SubscribeAsync(observerId, Lambda, options: options1, fireImmediately: true);
        await service.SubscribeAsync(observerId, Lambda, options: options2, fireImmediately: true);
        await service.SubscribeAsync(observerId, Lambda, options: options3, fireImmediately: true);

        // Assert
        var firstNotification = observerNotifications[0];
        firstNotification.IsImmediate.Should().BeTrue();
        var options1Mutated = options1.Clone();
        // This is the "real" options that goes inside "mudResizeListenerFactory.listenForResize"
        options1Mutated.BreakpointDefinitions = BreakpointGlobalOptions.GetDefaultOrUserDefinedBreakpointDefinition(options1Mutated);
        // BrowserViewportSubscription holds this information on what was the real options that were passed to the "mudResizeListenerFactory.listenForResize"
        var innerObserverOptions = service.GetInternalSubscription(observerId)?.Options;
        observerNotifications.Count.Should().Be(1);
        service.ObserversCount.Should().Be(1);
        innerObserverOptions.Should().Be(options1Mutated);
        jsRuntimeMock.Verify(x => x.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<object[]>()), Times.Exactly(2));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_NoServiceAndObserverOptionsMutation()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var mainOptions = new ResizeOptions();
        var mainOptionsClone = mainOptions.Clone();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object, new OptionsWrapper<ResizeOptions>(mainOptions));
        var observerOptions1 = new ResizeOptions { ReportRate = 1 };
        var observerOptions1Clone = observerOptions1.Clone();
        ResizeOptions? observerOptions2 = null;
        var observer1 = new BrowserViewportObserverMock(observerOptions1);
        var observer2 = new BrowserViewportObserverMock(observerOptions2);

        // Act
        await service.SubscribeAsync(observer1, fireImmediately: false);
        await service.SubscribeAsync(observer2, fireImmediately: false);

        // Assert
        // If this fails properties mutated, this shouldn't happen
        mainOptions.Should().Be(mainOptionsClone);
        mainOptions.Should().NotBeSameAs(mainOptionsClone);
        observerOptions2.Should().BeNull();
        observer2.ResizeOptions.Should().BeNull();
        // The object reference should stay same, otherwise means the instance was replaced
        observer2.ResizeOptions.Should().BeSameAs(observerOptions2);
        // If this fails properties mutated, this shouldn't happen
        observerOptions1.Should().Be(observerOptions1Clone);
    }

    [Test]
    public async Task SubscribeAsync_DifferentObserversWithSameOptions_ShouldHaveOneJSListener()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var observerOptions = new ResizeOptions();
        var observer1 = new BrowserViewportObserverMock(observerOptions);
        var observer2 = new BrowserViewportObserverMock(observerOptions);

        // Act
        await service.SubscribeAsync(observer1, fireImmediately: false);
        await service.SubscribeAsync(observer2, fireImmediately: false);

        // Assert
        service.ObserversCount.Should().Be(2);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_DifferentObserversWithDifferentOptions_ShouldHaveMultipleJSListener()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var observerOptions1 = new ResizeOptions { ReportRate = 1, EnableLogging = true };
        var observerOptions2 = new ResizeOptions { ReportRate = 2, SuppressInitEvent = false };
        var observer1 = new BrowserViewportObserverMock(observerOptions1);
        var observer2 = new BrowserViewportObserverMock(observerOptions2);

        // Act
        await service.SubscribeAsync(observer1, fireImmediately: false);
        await service.SubscribeAsync(observer2, fireImmediately: false);

        // Assert
        service.ObserversCount.Should().Be(2);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize", It.IsAny<object[]>()), Times.Exactly(2));
    }


    [Test]
    public async Task SubscribeAsync_RaiseOnResized_FireImmediately()
    {
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var observer = new BrowserViewportObserverMock();

        // Act
        await service.SubscribeAsync(observer, fireImmediately: true);
        var subscription = service.GetInternalSubscription(observer);
        Debug.Assert(subscription is not null, nameof(subscription) + " != null");
        await service.RaiseOnResized(new BrowserWindowSize { Width = 600, Height = 400 }, Breakpoint.Sm, subscription.JavaScriptListenerId);
        await service.RaiseOnResized(new BrowserWindowSize { Width = 960, Height = 720 }, Breakpoint.Md, subscription.JavaScriptListenerId);
        await service.RaiseOnResized(new BrowserWindowSize { Width = 1280, Height = 1024 }, Breakpoint.Sm, subscription.JavaScriptListenerId);

        // Assert
        observer.Notifications[0].IsImmediate.Should().BeTrue();
        observer.Notifications[1].IsImmediate.Should().BeFalse();
        observer.Notifications[2].IsImmediate.Should().BeFalse();
        observer.Notifications[3].IsImmediate.Should().BeFalse();
        observer.Notifications.Count.Should().Be(4);
    }

    [Test]
    public async Task RaiseOnResized_NotifyObservers_SameJavaScriptListener()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var observer1 = new BrowserViewportObserverMock();
        var observer2 = new BrowserViewportObserverMock();
        await service.SubscribeAsync(observer1, fireImmediately: false);
        await service.SubscribeAsync(observer2, fireImmediately: false);
        var subscription1 = service.GetInternalSubscription(observer1);
        var subscription2 = service.GetInternalSubscription(observer2);
        Debug.Assert(subscription1 is not null, nameof(subscription1) + " != null");
        Debug.Assert(subscription2 is not null, nameof(subscription2) + " != null");

        // Act
        // Assuming they both have same JavaScriptListenerId they both should get notification
        await service.RaiseOnResized(new BrowserWindowSize { Width = 600, Height = 400 }, Breakpoint.Sm, subscription1.JavaScriptListenerId);
        await service.RaiseOnResized(new BrowserWindowSize { Width = 960, Height = 720 }, Breakpoint.Md, subscription1.JavaScriptListenerId);
        await service.RaiseOnResized(new BrowserWindowSize { Width = 1280, Height = 1024 }, Breakpoint.Sm, subscription1.JavaScriptListenerId);

        // Assert
        observer1.Notifications[0].IsImmediate.Should().BeFalse();
        observer1.Notifications[1].IsImmediate.Should().BeFalse();
        observer1.Notifications[2].IsImmediate.Should().BeFalse();
        observer2.Notifications[0].IsImmediate.Should().BeFalse();
        observer2.Notifications[1].IsImmediate.Should().BeFalse();
        observer2.Notifications[2].IsImmediate.Should().BeFalse();
        subscription2.JavaScriptListenerId.Should().Be(subscription1.JavaScriptListenerId);
        observer1.Notifications.Count.Should().Be(3);
        observer2.Notifications.Count.Should().Be(3);
    }

    [Test]
    public async Task RaiseOnResized_NotifyObservers_DifferentJavaScriptListener()
    {
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var option1 = new ResizeOptions { ReportRate = 1, EnableLogging = true };
        var option2 = new ResizeOptions { ReportRate = 2, NotifyOnBreakpointOnly = false };
        var observer1 = new BrowserViewportObserverMock(option1);
        var observer2 = new BrowserViewportObserverMock(option2);
        await service.SubscribeAsync(observer1, fireImmediately: false);
        await service.SubscribeAsync(observer2, fireImmediately: false);
        var subscription1 = service.GetInternalSubscription(observer1);
        var subscription2 = service.GetInternalSubscription(observer2);
        Debug.Assert(subscription1 is not null, nameof(subscription1) + " != null");
        Debug.Assert(subscription2 is not null, nameof(subscription2) + " != null");

        // Act
        // Assuming they both have different JavaScriptListenerId meaning only observer1 should get notifications
        await service.RaiseOnResized(new BrowserWindowSize { Width = 600, Height = 400 }, Breakpoint.Sm, subscription1.JavaScriptListenerId);
        await service.RaiseOnResized(new BrowserWindowSize { Width = 960, Height = 720 }, Breakpoint.Md, subscription1.JavaScriptListenerId);
        await service.RaiseOnResized(new BrowserWindowSize { Width = 1280, Height = 1024 }, Breakpoint.Sm, subscription1.JavaScriptListenerId);

        // Assert
        observer1.Notifications[0].IsImmediate.Should().BeFalse();
        observer1.Notifications[1].IsImmediate.Should().BeFalse();
        observer1.Notifications[2].IsImmediate.Should().BeFalse();
        subscription2.JavaScriptListenerId.Should().NotBe(subscription1.JavaScriptListenerId);
        observer1.Notifications.Count.Should().Be(3);
        observer2.Notifications.Count.Should().Be(0);
    }

    [Test]
    public async Task UnsubscribeAsync_UnsubscribeObserver()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var observer = new BrowserViewportObserverMock();
        await service.SubscribeAsync(observer, fireImmediately: false);

        // Act
        await service.UnsubscribeAsync(observer);

        // Assert
        service.ObserversCount.Should().Be(0);
    }

    [Test]
    public async Task UnsubscribeAsync_DifferentObserversWithSameOptions_CancelLastJSListener()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var observerOptions = new ResizeOptions();
        var observer1 = new BrowserViewportObserverMock(observerOptions);
        var observer2 = new BrowserViewportObserverMock(observerOptions);
        await service.SubscribeAsync(observer1, fireImmediately: false);
        await service.SubscribeAsync(observer2, fireImmediately: false);

        // Act
        await service.UnsubscribeAsync(observer1);
        await service.UnsubscribeAsync(observer2);

        // Assert
        service.ObserversCount.Should().Be(0);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.cancelListener", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task UnsubscribeAsync_DifferentObserversWithDifferentOptions_CancelMultipleJSListener()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var observerOptions1 = new ResizeOptions { ReportRate = 1, EnableLogging = true };
        var observerOptions2 = new ResizeOptions { ReportRate = 2, SuppressInitEvent = false };
        var observer1 = new BrowserViewportObserverMock(observerOptions1);
        var observer2 = new BrowserViewportObserverMock(observerOptions2);
        await service.SubscribeAsync(observer1, fireImmediately: false);
        await service.SubscribeAsync(observer2, fireImmediately: false);

        // Act
        await service.UnsubscribeAsync(observer1);
        await service.UnsubscribeAsync(observer2);

        // Assert
        service.ObserversCount.Should().Be(0);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.cancelListener", It.IsAny<object[]>()), Times.Exactly(2));
    }

    [Test]
    [TestCase(2560, 1440, Breakpoint.Xxl)]
    [TestCase(1920, 1080, Breakpoint.Xl)]
    [TestCase(1280, 1024, Breakpoint.Lg)]
    [TestCase(960, 720, Breakpoint.Md)]
    [TestCase(600, 400, Breakpoint.Sm)]
    [TestCase(0, 0, Breakpoint.Xs)]
    public async Task GetCurrentBreakpointAsync_WithWindowSize_ReturnsBreakpoint(int width, int height, Breakpoint expectedBreakpoint)
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        jsRuntimeMock
            .Setup(x => x.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<object[]>()))
            .ReturnsAsync(new BrowserWindowSize { Width = width, Height = height });

        // Act
        var result = await service.GetCurrentBreakpointAsync();

        // Assert
        result.Should().Be(expectedBreakpoint);
    }

    [Test]
    [TestCaseSource(typeof(BreakpointWithinReferenceSizeTestCase), nameof(BreakpointWithinReferenceSizeTestCase.AllCombinations))]
    public async Task IsBreakpointWithinReferenceSizeAsync_ReturnsExpectedResult_AllCombinations(Breakpoint breakpoint, Breakpoint referenceBreakpoint, bool expectedResult)
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);

        // Act
        var result = await service.IsBreakpointWithinReferenceSizeAsync(breakpoint, referenceBreakpoint);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Test]
    [TestCase(Breakpoint.None, false)]
    [TestCase(Breakpoint.Always, true)]
    [TestCase(Breakpoint.Xs, false)]
    [TestCase(Breakpoint.Sm, true)]
    [TestCase(Breakpoint.Md, false)]
    [TestCase(Breakpoint.Lg, false)]
    [TestCase(Breakpoint.Xl, false)]
    [TestCase(Breakpoint.Xxl, false)]
    public async Task IsBreakpointWithinWindowSizeAsync_ReturnsExpectedResult(Breakpoint breakpoint, bool expectedResult)
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        jsRuntimeMock
            .Setup(x => x.InvokeAsync<BrowserWindowSize>("mudResizeListener.getBrowserWindowSize", It.IsAny<object[]>()))
            .ReturnsAsync(new BrowserWindowSize
            {
                // This will return Sm size
                Width = 600,
                Height = 400
            });

        // Act
        var result = await service.IsBreakpointWithinWindowSizeAsync(breakpoint);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Test]
    public async Task IsMediaQueryMatchAsync_MatchesMediaQuery()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        jsRuntimeMock
            .Setup(x => x.InvokeAsync<bool>("mudResizeListener.matchMedia", It.IsAny<object[]>()))
            .ReturnsAsync((string _, object[] args) =>
            {
                var mediaQuery = args[0] as string;

                return string.Equals(mediaQuery, "(max-width: 700px)", StringComparison.OrdinalIgnoreCase);
            });

        // Act
        var result1 = await service.IsMediaQueryMatchAsync("(max-width: 700px)");
        var result2 = await service.IsMediaQueryMatchAsync("random");

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }

    [Test]
    public async Task DisposeAsync_ShouldCancelOneListenerWhenSameOptions()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var options1 = new ResizeOptions();
        var options2 = new ResizeOptions();
        // should use default which is same as new ResizeOptions()
        ResizeOptions? options3 = null;

        await service.SubscribeAsync(new BrowserViewportObserverMock(new ResizeOptions()));
        await service.SubscribeAsync(new BrowserViewportObserverMock(options1));
        await service.SubscribeAsync(new BrowserViewportObserverMock(options2));
        await service.SubscribeAsync(new BrowserViewportObserverMock(options3));

        // Act
        await service.DisposeAsync();

        // Assert
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.dispose", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task DisposeAsync_ShouldCancelMultipleListenersWithDifferentOptions()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock.Object);
        var options1 = new ResizeOptions { ReportRate = 500, EnableLogging = true };
        var options2 = new ResizeOptions { ReportRate = 50, SuppressInitEvent = false };
        ResizeOptions? options3 = null;

        await service.SubscribeAsync(new BrowserViewportObserverMock(options1));
        await service.SubscribeAsync(new BrowserViewportObserverMock(options2));
        await service.SubscribeAsync(new BrowserViewportObserverMock(options3));

        // Act
        await service.DisposeAsync();

        // Assert
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.dispose", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task DisposeAsync_ShouldClearAllObservers()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock);

        await service.SubscribeAsync(new BrowserViewportObserverMock());
        await service.SubscribeAsync(new BrowserViewportObserverMock());
        await service.SubscribeAsync(new BrowserViewportObserverMock());
        await service.SubscribeAsync(new BrowserViewportObserverMock());
        await service.SubscribeAsync(new BrowserViewportObserverMock());
        var beforeObserversCount = service.ObserversCount;

        // Act
        await service.DisposeAsync();
        var afterObserversCount = service.ObserversCount;

        // Assert
        beforeObserversCount.Should().Be(5);
        afterObserversCount.Should().Be(0);
    }

    [Test]
    public async Task DisposeAsync_SubscribeShouldBeIgnored()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = new BrowserViewportService(NullLogger<BrowserViewportService>.Instance, jsRuntimeMock);
        var observer = new BrowserViewportObserverMock();

        // Act
        await service.DisposeAsync();
        await service.SubscribeAsync(observer);

        // Asset
        observer.Notifications.Count.Should().Be(0);
        service.ObserversCount.Should().Be(0);
    }
}
