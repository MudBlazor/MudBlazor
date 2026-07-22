using System;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MudBlazor.Resources;
using NUnit.Framework;

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
    [SetUICulture("en-US")]
    public void CustomLocalizationInterceptor_EnglishUICulture()
    {
        var interceptorMock = new Mock<ILocalizationInterceptor>();
        interceptorMock.Setup(mock => mock.Handle("MudDataGrid_Clear")).Returns(new LocalizedString("MudDataGrid_Clear", "Reset", false));
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock.Object);

        // Act
        var result = internalMudLocalizer["MudDataGrid_Clear"];

        // Assert
        result.Should().BeEquivalentTo(new LocalizedString("MudDataGrid_Clear", "Reset", false));
    }

    [Test]
    [SetUICulture("de-DE")]
    public void CustomLocalizationInterceptor_NonEnglishUICulture()
    {
        // Assert
        var interceptorMock = new Mock<ILocalizationInterceptor>();
        interceptorMock.Setup(mock => mock.Handle("MudDataGrid_Clear")).Returns(new LocalizedString("MudDataGrid_Clear", "Reset", false));
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock.Object);

        // Act
        var result = internalMudLocalizer["MudDataGrid_Clear"];

        result.Should().BeEquivalentTo(new LocalizedString("MudDataGrid_Clear", "Reset", false));
    }

    [Test]
    [SetUICulture("en-US")]
    public void DefaultLocalizationInterceptor_EnglishUICulture()
    {
        // Arrange
        var interceptorMock = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer: null);
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock);

        // Act & Assert
        internalMudLocalizer["MudDataGrid_Contains"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid_Contains", "contains", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid_IsEmpty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid_IsEmpty", "is empty", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid_IsNotEmpty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid_IsNotEmpty", "is not empty", false, typeof(LanguageResource).FullName));
    }

    [Test]
    [SetUICulture("de-DE")]
    public void DefaultLocalizationInterceptor_NonEnglishUICulture()
    {
        // Arrange
        var interceptorMock = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer: null);
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock);

        // Act & Assert
        internalMudLocalizer["MudDataGrid_Contains"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid_Contains", "contains", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid_IsEmpty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid_IsEmpty", "is empty", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid_IsNotEmpty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid_IsNotEmpty", "is not empty", false, typeof(LanguageResource).FullName));
    }

    [Test]
    [SetUICulture("en-US")]
    public void DefaultLocalizationInterceptor_WithCustomMudLocalizer_EnglishUICulture()
    {
        // Arrange
        var mudLocalizerMock = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizerMock.Setup(mock => mock["MudDataGrid_IsEmpty"]).Returns(new LocalizedString("MudDataGrid_IsEmpty", "XXX", false));
        mudLocalizerMock.Setup(mock => mock["MudDataGrid_IsNotEmpty"]).Returns(new LocalizedString("MudDataGrid_IsNotEmpty", "MudDataGrid_IsNotEmpty", true));
        var interceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizerMock.Object);
        var internalMudLocalizer = new InternalMudLocalizer(interceptor);

        // Act & Assert
        internalMudLocalizer["MudDataGrid_Contains"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid_Contains", "contains", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid_IsEmpty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid_IsEmpty", "is empty", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid_IsNotEmpty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid_IsNotEmpty", "is not empty", false, typeof(LanguageResource).FullName));
    }

    [Test]
    [SetUICulture("de-DE")]
    public void DefaultLocalizationInterceptor_WithCustomMudLocalizer_NonEnglishUICulture()
    {
        // Arrange
        var mudLocalizerMock = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizerMock.Setup(mock => mock["MudDataGrid_IsEmpty"]).Returns(new LocalizedString("MudDataGrid_IsEmpty", "XXX", false));
        mudLocalizerMock.Setup(mock => mock["MudDataGrid_IsNotEmpty"]).Returns(new LocalizedString("MudDataGrid_IsNotEmpty", "MudDataGrid_IsNotEmpty", true));
        var interceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizerMock.Object);
        var internalMudLocalizer = new InternalMudLocalizer(interceptor);

        // Act & Assert
        internalMudLocalizer["MudDataGrid_Contains"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid_Contains", "contains", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid_IsEmpty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid_IsEmpty", "XXX", false));
        internalMudLocalizer["MudDataGrid_IsNotEmpty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid_IsNotEmpty", "is not empty", false, typeof(LanguageResource).FullName));
    }

    [Test]
    [SetUICulture("en-US")]
    public void RenamedKey_ShouldFallbackToLegacyKey()
    {
        // Arrange
        var interceptorMock = new Mock<ILocalizationInterceptor>();
        interceptorMock.Setup(mock => mock.Handle("MudDataGrid_GreaterThanSign")).Returns(new LocalizedString("MudDataGrid_GreaterThanSign", "", true));
        interceptorMock.Setup(mock => mock.Handle("MudDataGrid.>")).Returns(new LocalizedString("MudDataGrid.>", ">", false));
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock.Object);

        // Act
        var result = internalMudLocalizer["MudDataGrid_GreaterThanSign"];

        // Assert
        result.Should().BeEquivalentTo(new LocalizedString("MudDataGrid.>", ">", false));
    }
}
