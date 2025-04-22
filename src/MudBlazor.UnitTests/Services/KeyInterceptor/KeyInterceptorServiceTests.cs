// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.Services;
using MudBlazor.UnitTests.Services.KeyInterceptor.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.KeyInterceptor;

#nullable enable
[TestFixture]
public class KeyInterceptorServiceTests
{
    [Test]
    public async Task SubscribeAsync_WithObserver_ShouldSubscribe()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer = new KeyInterceptorObserverMock("observer1");
        var options = new KeyInterceptorOptions();
        var service = new KeyInterceptorService(NullLogger<KeyInterceptorService>.Instance, jsRuntimeMock.Object);

        // Act
        await service.SubscribeAsync(observer, options);

        // Assert
        observer.Notifications.Count.Should().Be(0);
        service.ObserversCount.Should().Be(1);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudKeyInterceptor.connect", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_ReSubscribeWithSameObserverIdentifier()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer = new KeyInterceptorObserverMock("observer1");
        var options = new KeyInterceptorOptions();
        var service = new KeyInterceptorService(NullLogger<KeyInterceptorService>.Instance, jsRuntimeMock.Object);

        // Act
        await service.SubscribeAsync(observer, options);
        await service.SubscribeAsync(observer, options);
        await service.SubscribeAsync(observer, options);

        // Assert
        observer.Notifications.Count.Should().Be(0);
        service.ObserversCount.Should().Be(1);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudKeyInterceptor.connect", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task SubscribeAsync_MultipleObservers_ShouldNotifyCorrectObserver()
    {
        // Arrange
        var expectedEventArgs = new KeyboardEventArgs { Key = "ArrowUp", Type = "keydown" };
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer1 = new KeyInterceptorObserverMock("observer1");
        var observer2 = new KeyInterceptorObserverMock("observer2");
        var service = new KeyInterceptorService(NullLogger<KeyInterceptorService>.Instance, jsRuntimeMock.Object);

        await service.SubscribeAsync(observer1, new KeyInterceptorOptions());
        await service.SubscribeAsync(observer2, new KeyInterceptorOptions());

        // Act
        await service.OnKeyDown(observer2.ElementId, expectedEventArgs);

        // Assert
        service.ObserversCount.Should().Be(2);
        observer1.Notifications.Count.Should().Be(0);
        observer2.Notifications.Count.Should().Be(1);
        observer2.Notifications.Should().ContainSingle().Which.Should().BeEquivalentTo((observer2.ElementId, expectedEventArgs));
    }

    [Test]
    public async Task SubscribeAsync_Overloads()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new KeyInterceptorService(NullLogger<KeyInterceptorService>.Instance, jsRuntimeMock.Object);
        void OnKeyDownAction(KeyboardEventArgs args) { }
        void OnKeyUpAction(KeyboardEventArgs args) { }
        Task OnKeyDownTask(KeyboardEventArgs args) => Task.CompletedTask;
        Task OnKeyUpTask(KeyboardEventArgs args) => Task.CompletedTask;

        // Act
        await service.SubscribeAsync("observer1", new KeyInterceptorOptions(), OnKeyDownAction, OnKeyUpAction);
        await service.SubscribeAsync("observer2", new KeyInterceptorOptions(), OnKeyDownTask, OnKeyUpTask);
        await service.SubscribeAsync("observer3", new KeyInterceptorOptions(), KeyObserver.KeyDownIgnore(), KeyObserver.KeyUpIgnore());
        await service.SubscribeAsync(new KeyInterceptorObserverMock("observer4"), new KeyInterceptorOptions());

        // Assert
        service.ObserversCount.Should().Be(4);
    }

    [Test]
    public async Task UpdateKeyAsync_ShouldCallJavaScript()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer = new KeyInterceptorObserverMock("observer1");
        var service = new KeyInterceptorService(NullLogger<KeyInterceptorService>.Instance, jsRuntimeMock.Object);

        // Act
        await service.UpdateKeyAsync(observer, new("Escape", stopDown: "key+none"));

        // Assert
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudKeyInterceptor.updatekey", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task OnKeyDown_ShouldNotifyObservers()
    {
        // Arrange
        var expectedEventArgs = new KeyboardEventArgs { Key = "ArrowUp", Type = "keydown" };
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer = new KeyInterceptorObserverMock("observer1");
        var options = new KeyInterceptorOptions("target", enableLogging: true);
        var service = new KeyInterceptorService(NullLogger<KeyInterceptorService>.Instance, jsRuntimeMock.Object);
        await service.SubscribeAsync(observer, options);

        // Act
        await service.OnKeyDown(observer.ElementId, expectedEventArgs);

        // Assert
        service.ObserversCount.Should().Be(1);
        observer.Notifications.Count.Should().Be(1);
        observer.Notifications.Should().ContainSingle().Which.Should().BeEquivalentTo((observer.ElementId, expectedEventArgs));
    }

    [Test]
    public async Task OnKeyUp_ShouldNotifyObservers()
    {
        // Arrange
        var expectedEventArgs = new KeyboardEventArgs { Key = "ArrowUp", Type = "keyup" };
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer = new KeyInterceptorObserverMock("observer1");
        var options = new KeyInterceptorOptions();
        var service = new KeyInterceptorService(NullLogger<KeyInterceptorService>.Instance, jsRuntimeMock.Object);
        await service.SubscribeAsync(observer, options);

        // Act
        await service.OnKeyUp(observer.ElementId, expectedEventArgs);

        // Assert
        service.ObserversCount.Should().Be(1);
        observer.Notifications.Count.Should().Be(1);
        observer.Notifications.Should().ContainSingle().Which.Should().BeEquivalentTo((observer.ElementId, expectedEventArgs));
    }

    [Test]
    public async Task UnsubscribeAsync_UnsubscribeObserver()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer = new KeyInterceptorObserverMock("observer1");
        var options = new KeyInterceptorOptions();
        var service = new KeyInterceptorService(NullLogger<KeyInterceptorService>.Instance, jsRuntimeMock.Object);
        await service.SubscribeAsync(observer, options);

        // Act
        await service.UnsubscribeAsync(observer);

        // Assert
        service.ObserversCount.Should().Be(0);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudKeyInterceptor.connect", It.IsAny<object[]>()), Times.Once);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudKeyInterceptor.disconnect", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task DisposeAsync_ShouldClearAllObservers()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new KeyInterceptorService(NullLogger<KeyInterceptorService>.Instance, jsRuntimeMock.Object);
        await service.SubscribeAsync(new KeyInterceptorObserverMock("observer1"), new KeyInterceptorOptions());
        await service.SubscribeAsync(new KeyInterceptorObserverMock("observer2"), new KeyInterceptorOptions());
        await service.SubscribeAsync(new KeyInterceptorObserverMock("observer3"), new KeyInterceptorOptions());
        await service.SubscribeAsync(new KeyInterceptorObserverMock("observer4"), new KeyInterceptorOptions());
        await service.SubscribeAsync(new KeyInterceptorObserverMock("observer5"), new KeyInterceptorOptions());
        var beforeObserversCount = service.ObserversCount;

        // Act
        await service.DisposeAsync();
        var afterObserversCount = service.ObserversCount;

        // Assert
        beforeObserversCount.Should().Be(5);
        afterObserversCount.Should().Be(0);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudKeyInterceptor.connect", It.IsAny<object[]>()), Times.Exactly(5));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudKeyInterceptor.disconnect", It.IsAny<object[]>()), Times.Exactly(5));
    }
}
