// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.Overlay;

[TestFixture]
public class OverlayPointerDownListenerFactoryTests
{
    [Test]
    public void Create_ShouldReturnNewOverlayPointerDownListener()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<IJSRuntime, MockJsRuntime>();

        var serviceProvider = services.BuildServiceProvider();
        var factory = new OverlayPointerDownListenerFactory(serviceProvider);

        // Act
        var listener = factory.Create("elementId");
        
        // Assert
        listener.Should().NotBeNull();
        listener.Should().BeOfType<OverlayPointerDownListener>();
    }
}
