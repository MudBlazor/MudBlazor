// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.UnitTests.Services.PointerEvents.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.PointerEvents;

#nullable enable

[TestFixture]
public class PointerEventsNoneServiceTests
{
    [Test]
    public async Task SubscribeAsync_WithObserver_ShouldSubscribe()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer = new PointerEventsNoneObserverMock("observer1");
        var options = new PointerEventsNoneOptions();
        var service = new PointerEventsNoneService(NullLogger<PointerEventsNoneService>.Instance, jsRuntimeMock.Object);

        // Act
        await service.SubscribeAsync(observer, options);

        // Assert
        observer.Notifications.Count.Should().Be(0);
        service.ObserversCount.Should().Be(1);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudPointerEventsNone.listenForPointerEvents", It.IsAny<CancellationToken>(), It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_ReSubscribeWithSameObserverIdentifier()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer = new PointerEventsNoneObserverMock("observer1");
        var options = new PointerEventsNoneOptions();
        var service = new PointerEventsNoneService(NullLogger<PointerEventsNoneService>.Instance, jsRuntimeMock.Object);

        // Act
        await service.SubscribeAsync(observer, options);
        await service.SubscribeAsync(observer, options);
        await service.SubscribeAsync(observer, options);

        // Assert
        observer.Notifications.Count.Should().Be(0);
        service.ObserversCount.Should().Be(1);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudPointerEventsNone.listenForPointerEvents", It.IsAny<CancellationToken>(), It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_MultipleObservers_ShouldNotifyCorrectObserver()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer1 = new PointerEventsNoneObserverMock("observer1");
        var observer2 = new PointerEventsNoneObserverMock("observer2");
        var service = new PointerEventsNoneService(NullLogger<PointerEventsNoneService>.Instance, jsRuntimeMock.Object);

        await service.SubscribeAsync(observer1, new());
        await service.SubscribeAsync(observer2, new());

        // Act
        await service.RaiseOnPointerDown([observer2.ElementId]);

        // Assert
        service.ObserversCount.Should().Be(2);
        observer1.Notifications.Count.Should().Be(0);
        observer2.Notifications.Count.Should().Be(1);
        observer2.Notifications.Should().ContainSingle().Which.Should().BeEquivalentTo((observer2.ElementId, EventArgs.Empty));
    }

    [Test]
    public async Task SubscribeAsync_Overloads()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new PointerEventsNoneService(NullLogger<PointerEventsNoneService>.Instance, jsRuntimeMock.Object);

        // Act
        await service.SubscribeAsync("observer1", new());
        await service.SubscribeAsync("observer2", new(), PointerEventsNoneObserver.PointerDownIgnore(), PointerEventsNoneObserver.PointerUpIgnore());

        // Assert
        service.ObserversCount.Should().Be(2);
    }

    [Test]
    public async Task RaiseOnPointerDown_ShouldNotifyObservers()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer = new PointerEventsNoneObserverMock("observer1");
        var service = new PointerEventsNoneService(NullLogger<PointerEventsNoneService>.Instance, jsRuntimeMock.Object);
        await service.SubscribeAsync(observer, new());

        // Act
        await service.RaiseOnPointerDown([observer.ElementId]);

        // Assert
        service.ObserversCount.Should().Be(1);
        observer.Notifications.Count.Should().Be(1);
        observer.Notifications.Should().ContainSingle().Which.Should().BeEquivalentTo((observer.ElementId, EventArgs.Empty));
    }

    [Test]
    public async Task RaiseOnPointerUp_ShouldNotifyObservers()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer = new PointerEventsNoneObserverMock("observer1");
        var service = new PointerEventsNoneService(NullLogger<PointerEventsNoneService>.Instance, jsRuntimeMock.Object);
        await service.SubscribeAsync(observer, new());

        // Act
        await service.RaiseOnPointerUp([observer.ElementId]);

        // Assert
        service.ObserversCount.Should().Be(1);
        observer.Notifications.Count.Should().Be(1);
        observer.Notifications.Should().ContainSingle().Which.Should().BeEquivalentTo((observer.ElementId, EventArgs.Empty));
    }

    [Test]
    public async Task UnsubscribeAsync_UnsubscribeObserver()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer = new PointerEventsNoneObserverMock("observer1");
        var service = new PointerEventsNoneService(NullLogger<PointerEventsNoneService>.Instance, jsRuntimeMock.Object);
        await service.SubscribeAsync(observer, new());

        // Act
        await service.UnsubscribeAsync(observer);

        // Assert
        service.ObserversCount.Should().Be(0);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudPointerEventsNone.listenForPointerEvents", It.IsAny<CancellationToken>(), It.IsAny<object[]>()), Times.Once);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudPointerEventsNone.cancelListener", It.IsAny<CancellationToken>(), It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task DisposeAsync_ShouldClearAllObservers()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new PointerEventsNoneService(NullLogger<PointerEventsNoneService>.Instance, jsRuntimeMock.Object);
        await service.SubscribeAsync("observer1", new());
        await service.SubscribeAsync("observer2", new());
        await service.SubscribeAsync("observer3", new());
        await service.SubscribeAsync("observer4", new());
        await service.SubscribeAsync("observer5", new());
        var beforeObserversCount = service.ObserversCount;

        // Act
        await service.DisposeAsync();
        var afterObserversCount = service.ObserversCount;

        // Assert
        beforeObserversCount.Should().Be(5);
        afterObserversCount.Should().Be(0);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudPointerEventsNone.listenForPointerEvents", It.IsAny<CancellationToken>(), It.IsAny<object[]>()), Times.Exactly(5));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudPointerEventsNone.dispose", It.IsAny<CancellationToken>(), It.IsAny<object[]>()), Times.Once);
    }
}
