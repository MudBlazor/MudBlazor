// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

#nullable enable
[TestFixture]
public class EventListenerServiceTests
{
    private Mock<IJSRuntime> _jsRuntimeMock = null!;
    private EventListenerService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _jsRuntimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
        _service = new EventListenerService(NullLogger<EventListenerService>.Instance, _jsRuntimeMock.Object);
    }

    [TearDown]
    public async Task TearDown()
    {
        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.unsubscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);
        await _service.DisposeAsync();
    }

    [Test]
    public async Task SubscribeAsync_WithAsyncCallback_ShouldSubscribe()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "click";
        var elementId = "my-element";
        var throttleInterval = 20;
        var projectionName = "mynamespace.myfunction";
        Func<object, Task> callback = x => Task.CompletedTask;

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.Is<object[]>(z =>
                (string)z[0] == eventName &&
                (string)z[1] == elementId &&
                (string?)z[2] == projectionName &&
                (int)z[3] == throttleInterval &&
                (Guid)z[4] == subscriptionId &&
                z[5] is string[] &&
                z[6] is DotNetObjectReference<EventListenerService>
            ))).ReturnsAsync(Mock.Of<IJSVoidResult>);

        // Act
        await _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, projectionName, throttleInterval, callback);

        // Assert
        _service.SubscriptionCount.Should().Be(1);
        _jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_WithSyncCallback_ShouldSubscribe()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "click";
        var elementId = "my-element";
        var throttleInterval = 0;
        var callbackInvoked = false;
        Action<object> callback = x => { callbackInvoked = true; };

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);

        // Act
        await _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, null, throttleInterval, callback);

        // Assert
        _service.SubscriptionCount.Should().Be(1);
        callbackInvoked.Should().BeFalse(); // Not invoked yet
    }

    [Test]
    public async Task SubscribeAsync_MultipleSubscriptions_ShouldAllBeTracked()
    {
        // Arrange
        var eventName = "click";
        var throttleInterval = 0;
        Func<object, Task> callback1 = x => Task.CompletedTask;
        Func<object, Task> callback2 = x => Task.CompletedTask;
        Func<object, Task> callback3 = x => Task.CompletedTask;

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);

        // Act
        await _service.SubscribeAsync<MouseEventArgs>(Guid.NewGuid(), eventName, "element1", null, throttleInterval, callback1);
        await _service.SubscribeAsync<MouseEventArgs>(Guid.NewGuid(), eventName, "element2", null, throttleInterval, callback2);
        await _service.SubscribeAsync<MouseEventArgs>(Guid.NewGuid(), eventName, "element3", null, throttleInterval, callback3);

        // Assert
        _service.SubscriptionCount.Should().Be(3);
    }

    [Test]
    public async Task SubscribeAsync_ReSubscribeWithSameId_ShouldNotAddDuplicate()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "click";
        var elementId = "my-element";
        Func<object, Task> callback = x => Task.CompletedTask;

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);

        // Act
        await _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, null, 0, callback);
        await _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, null, 0, callback);
        await _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, null, 0, callback);

        // Assert
        _service.SubscriptionCount.Should().Be(1);
        _jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_AndCallback_ShouldInvokeCallback()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "click";
        var elementId = "my-element";
        var callbackCalled = false;
        var offsetX = 200.24;
        var offsetY = 12425.2;

        Func<object, Task> callback = x =>
        {
            x.Should().BeAssignableTo<MouseEventArgs>();
            var args = (MouseEventArgs)x;
            args.OffsetX.Should().Be(offsetX);
            args.OffsetY.Should().Be(offsetY);
            callbackCalled = true;
            return Task.CompletedTask;
        };

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);

        // Act
        await _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, null, 0, callback);
        await _service.OnEventOccur(subscriptionId, System.Text.Json.JsonSerializer.Serialize(new
        {
            offsetX = offsetX,
            offsetY = offsetY,
        }));

        // Assert
        callbackCalled.Should().BeTrue();
    }

    [Test]
    public async Task SubscribeAsync_WithSyncCallback_AndCallback_ShouldInvokeCallback()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "click";
        var elementId = "my-element";
        var callbackCalled = false;
        var offsetX = 100.0;
        var offsetY = 200.0;

        Action<object> callback = x =>
        {
            x.Should().BeAssignableTo<MouseEventArgs>();
            var args = (MouseEventArgs)x;
            args.OffsetX.Should().Be(offsetX);
            args.OffsetY.Should().Be(offsetY);
            callbackCalled = true;
        };

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);

        // Act
        await _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, null, 0, callback);
        await _service.OnEventOccur(subscriptionId, System.Text.Json.JsonSerializer.Serialize(new
        {
            offsetX = offsetX,
            offsetY = offsetY,
        }));

        // Assert
        callbackCalled.Should().BeTrue();
    }

    [Test]
    public async Task SubscribeGlobalAsync_WithAsyncCallback_ShouldSubscribe()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "resize";
        var throttleInterval = 100;
        Func<object, Task> callback = x => Task.CompletedTask;

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribeGlobal", It.Is<object[]>(z =>
                (string)z[0] == eventName &&
                (int)z[1] == throttleInterval &&
                (Guid)z[2] == subscriptionId &&
                z[3] is string[] &&
                z[4] is DotNetObjectReference<EventListenerService>
            ))).ReturnsAsync(Mock.Of<IJSVoidResult>);

        // Act
        await _service.SubscribeGlobalAsync<MouseEventArgs>(subscriptionId, eventName, throttleInterval, callback);

        // Assert
        _service.SubscriptionCount.Should().Be(1);
    }

    [Test]
    public async Task SubscribeGlobalAsync_WithSyncCallback_ShouldSubscribe()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "resize";
        var throttleInterval = 100;
        var callbackInvoked = false;
        Action<object> callback = x => { callbackInvoked = true; };

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribeGlobal", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);

        // Act
        await _service.SubscribeGlobalAsync<MouseEventArgs>(subscriptionId, eventName, throttleInterval, callback);

        // Assert
        _service.SubscriptionCount.Should().Be(1);
        callbackInvoked.Should().BeFalse(); // Not invoked yet
    }

    [Test]
    public async Task SubscribeAsync_WithObserver_ShouldSubscribe()
    {
        // Arrange
        var observer = new TestEventObserver(Guid.NewGuid());
        var eventName = "click";
        var elementId = "my-element";
        var throttleInterval = 0;
        var eventType = typeof(MouseEventArgs);
        var eventProperties = new[] { "offsetX", "offsetY" };

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);

        // Act
        await _service.SubscribeAsync(observer, eventName, elementId, null, throttleInterval, eventType, eventProperties);

        // Assert
        _service.SubscriptionCount.Should().Be(1);
    }

    [Test]
    public async Task UnsubscribeAsync_WithObserver_ShouldUnsubscribe()
    {
        // Arrange
        var observer = new TestEventObserver(Guid.NewGuid());
        var eventName = "click";
        var elementId = "my-element";
        var eventType = typeof(MouseEventArgs);
        var eventProperties = new[] { "offsetX", "offsetY" };

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);
        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.unsubscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);

        await _service.SubscribeAsync(observer, eventName, elementId, null, 0, eventType, eventProperties);

        // Act
        await _service.UnsubscribeAsync(observer);

        // Assert
        _service.SubscriptionCount.Should().Be(0);
    }

    [Test]
    public async Task UnsubscribeAsync_WithSubscriptionId_ShouldUnsubscribe()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "click";
        var elementId = "my-element";
        Func<object, Task> callback = x => Task.CompletedTask;

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);
        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.unsubscribe", It.Is<object[]>(z =>
            z.Length == 1 &&
            (Guid)z[0] == subscriptionId
        ))).ReturnsAsync(Mock.Of<IJSVoidResult>);

        await _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, null, 0, callback);

        // Act
        await _service.UnsubscribeAsync(subscriptionId);

        // Assert
        _service.SubscriptionCount.Should().Be(0);
    }

    [Test]
    public async Task UnsubscribeAsync_NonExistentSubscription_ShouldNotThrow()
    {
        // Act & Assert
        await _service.UnsubscribeAsync(Guid.NewGuid());
        // No exception should be thrown
    }

    [Test]
    public async Task OnEventOccur_WithNonExistentSubscription_ShouldNotThrow()
    {
        // Act & Assert
        await _service.OnEventOccur(Guid.NewGuid(), "{}");
        // No exception should be thrown
    }

    [Test]
    public async Task OnEventOccur_WithNullEventData_ShouldNotInvokeCallback()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "click";
        var elementId = "element1";
        var callbackCalled = false;
        Func<object, Task> callback = x =>
        {
            callbackCalled = true;
            return Task.CompletedTask;
        };

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);

        await _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, null, 0, callback);

        // Act - Send empty/null data that deserializes to null
        await _service.OnEventOccur(subscriptionId, "null");

        // Assert
        callbackCalled.Should().BeFalse();
    }

    [Test]
    public async Task DisposeAsync_WithMultipleSubscriptions_ShouldUnsubscribeAll()
    {
        // Arrange
        var eventName = "click";
        var throttleInterval = 0;
        Func<object, Task> callback = x => Task.CompletedTask;

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);
        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.unsubscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);

        for (var i = 0; i < 10; i++)
        {
            await _service.SubscribeAsync<MouseEventArgs>(Guid.NewGuid(), eventName, $"element-{i}", null, throttleInterval, callback);
        }

        _service.SubscriptionCount.Should().Be(10);

        // Act
        await _service.DisposeAsync();

        // Assert
        _service.SubscriptionCount.Should().Be(0);
        _jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()), Times.Exactly(10));
        _jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.unsubscribe", It.IsAny<object[]>()), Times.Exactly(10));
    }

    [Test]
    public async Task DisposeAsync_CalledTwice_ShouldNotThrow()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "click";
        var elementId = "element1";
        Func<object, Task> callback = x => Task.CompletedTask;

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.subscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);
        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudThrottledEventManager.unsubscribe", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>);

        await _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, null, 0, callback);

        // Act & Assert
        await _service.DisposeAsync();
        await _service.DisposeAsync(); // Should not throw
    }

    [Test]
    public async Task SubscribeAsync_AfterDispose_ShouldNotSubscribe()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "click";
        var elementId = "element1";
        Func<object, Task> callback = x => Task.CompletedTask;

        await _service.DisposeAsync();

        // Act
        await _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, null, 0, callback);

        // Assert
        _service.SubscriptionCount.Should().Be(0);
    }

    [Test]
    public async Task SubscribeGlobalAsync_AfterDispose_ShouldNotSubscribe()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "resize";
        Func<object, Task> callback = x => Task.CompletedTask;

        await _service.DisposeAsync();

        // Act
        await _service.SubscribeGlobalAsync<MouseEventArgs>(subscriptionId, eventName, 0, callback);

        // Assert
        _service.SubscriptionCount.Should().Be(0);
    }

    [Test]
    public async Task UnsubscribeAsync_AfterDispose_ShouldNotThrow()
    {
        // Arrange
        await _service.DisposeAsync();

        // Act & Assert
        await _service.UnsubscribeAsync(Guid.NewGuid());
        // No exception should be thrown
    }

    [Test]
    public void SubscribeAsync_WithNullCallback_ShouldThrowArgumentNullException()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "click";
        var elementId = "element1";
        Func<object, Task>? callback = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, null, 0, callback!));
    }

    [Test]
    public void SubscribeAsync_WithNullSyncCallback_ShouldThrowArgumentNullException()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "click";
        var elementId = "element1";
        Action<object>? callback = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.SubscribeAsync<MouseEventArgs>(subscriptionId, eventName, elementId, null, 0, callback!));
    }

    [Test]
    public void SubscribeGlobalAsync_WithNullCallback_ShouldThrowArgumentNullException()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "resize";
        Func<object, Task>? callback = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.SubscribeGlobalAsync<MouseEventArgs>(subscriptionId, eventName, 0, callback!));
    }

    [Test]
    public void SubscribeGlobalAsync_WithNullSyncCallback_ShouldThrowArgumentNullException()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var eventName = "resize";
        Action<object>? callback = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.SubscribeGlobalAsync<MouseEventArgs>(subscriptionId, eventName, 0, callback!));
    }

    [Test]
    public void SubscribeAsync_WithNullObserver_ShouldThrowArgumentNullException()
    {
        // Arrange
        var eventName = "click";
        var elementId = "element1";
        var eventType = typeof(MouseEventArgs);
        var eventProperties = new[] { "offsetX", "offsetY" };
        IEventListenerObserver? observer = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.SubscribeAsync(observer!, eventName, elementId, null, 0, eventType, eventProperties));
    }

    [Test]
    public void SubscribeGlobalAsync_WithNullObserver_ShouldThrowArgumentNullException()
    {
        // Arrange
        var eventName = "resize";
        var eventType = typeof(MouseEventArgs);
        var eventProperties = new[] { "offsetX", "offsetY" };
        IEventListenerObserver? observer = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.SubscribeGlobalAsync(observer!, eventName, 0, eventType, eventProperties));
    }

    [Test]
    public void UnsubscribeAsync_WithNullObserver_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEventListenerObserver? observer = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UnsubscribeAsync(observer!));
    }

    private sealed class TestEventObserver : IEventListenerObserver
    {
        public Guid SubscriptionId { get; }
        public List<object> Notifications { get; } = new();

        public TestEventObserver(Guid subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }

        public Task NotifyEventOccurredAsync(object eventArgs)
        {
            Notifications.Add(eventArgs);
            return Task.CompletedTask;
        }
    }
}
