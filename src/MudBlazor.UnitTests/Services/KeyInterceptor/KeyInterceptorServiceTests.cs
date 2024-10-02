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
    public async Task OnKeyUp_ShouldNotifyObservers()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var observer = new KeyInterceptorObserverMock("observer1");
        var options = new KeyInterceptorOptions();
        var service = new KeyInterceptorService(NullLogger<KeyInterceptorService>.Instance, jsRuntimeMock.Object);
        await service.SubscribeAsync(observer, options);
        var expectedEventArgs = new KeyboardEventArgs { Key = "ArrowUp", Type = "keyup" };

        // Act
        await service.OnKeyUp(observer.ElementId, expectedEventArgs);

        // Assert
        service.ObserversCount.Should().Be(1);
        observer.Notifications.Count.Should().Be(1);
        observer.Notifications.Should().ContainSingle().Which.Should().BeEquivalentTo(expectedEventArgs);
    }
}
