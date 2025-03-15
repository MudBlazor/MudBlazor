// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.Overlay;

[TestFixture]
public class OverlayPointerDownListenerTests
{
    [Test]
    public void IsStarted_ShouldBeFalse_Initially()
    {
        // Arrange
        var jsRuntimeMock = new MockJsRuntime();

        // Act
        var service = new OverlayPointerDownListener("elementId", jsRuntimeMock);

        // Assert
        service.IsStarted.Should().BeFalse();
    }

    [Test]
    public void IsStarted_ShouldBeTrue_AfterStartAsync()
    {
        // Arrange
        var jsRuntimeMock = new MockJsRuntime();
        var service = new OverlayPointerDownListener("elementId", jsRuntimeMock);

        // Act
        service.StartAsync().GetAwaiter().GetResult();

        // Assert
        service.IsStarted.Should().BeTrue();
    }

    [Test]
    public async Task IsStarted_ShouldBeFalse_AfterStopAsync()
    {
        // Arrange
        var jsRuntimeMock = new MockJsRuntime();
        var service = new OverlayPointerDownListener("elementId", jsRuntimeMock);

        // Act
        await service.StartAsync();
        await service.StopAsync();

        // Assert
        service.IsStarted.Should().BeFalse();
    }

    [Test]
    public void OnPointerDown_ShouldInvoke_WhenRaiseOnPointerDownIsCalled()
    {
        // Arrange
        var jsRuntimeMock = new MockJsRuntime();
        var service = new OverlayPointerDownListener("elementId", jsRuntimeMock);
        bool wasCalled = false;
        service.OnPointerDown += (sender, args) => wasCalled = true;

        // Act
        service.RaiseOnPointerDown();

        // Assert
        wasCalled.Should().BeTrue();
    }
}
