using Moq;
using NUnit.Framework;
using System;
using FluentAssertions;
using Microsoft.Extensions.Localization;

namespace MudBlazor.UnitTests.Services.Localization;

#nullable enable
[TestFixture]
public class InternalMudLocalizerTests
{
    [Test]
    public void Constructor_WithNullInterceptor_ShouldThrowArgumentNullException()
    {
        // Arrange
        ILocalizationInterceptor? interceptor = null;

        // Act
        var construct = () => new InternalMudLocalizer(interceptor!);

        // Assert
        construct.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WithValidInterceptor_ShouldNotThrowException()
    {
        // Arrange
        var interceptorMock = new Mock<ILocalizationInterceptor>();

        // Act
        var construct = () => new InternalMudLocalizer(interceptorMock.Object);

        // Assert
        construct.Should().NotThrow();
    }

    [Test]
    public void InternalMudLocalizer_WithCustomInterceptor()
    {
        // Arrange
        var interceptorMock = new Mock<ILocalizationInterceptor>();
        interceptorMock.Setup(mock => mock.Handle("TestKey")).Returns(new LocalizedString("TestKey", "TestValue", resourceNotFound: false));
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock.Object);

        // Act
        var result = internalMudLocalizer["TestKey"];

        // Assert
        result.Value.Should().Be("TestValue").And.BeEquivalentTo(new LocalizedString("TestKey", "TestValue", resourceNotFound: false, searchedLocation: null));
    }
}
