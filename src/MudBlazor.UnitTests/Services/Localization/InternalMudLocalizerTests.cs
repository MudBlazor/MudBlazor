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
        interceptorMock.Setup(mock => mock.Handle("MudDataGrid.Clear")).Returns(new LocalizedString("MudDataGrid.Clear", "Reset", false));
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock.Object);

        // Act
        var result = internalMudLocalizer["MudDataGrid.Clear"];

        // Assert
        result.Should().BeEquivalentTo(new LocalizedString("MudDataGrid.Clear", "Reset", false));
    }

    [Test]
    [SetUICulture("de-DE")]
    public void CustomLocalizationInterceptor_NonEnglishUICulture()
    {
        // Assert
        var interceptorMock = new Mock<ILocalizationInterceptor>();
        interceptorMock.Setup(mock => mock.Handle("MudDataGrid.Clear")).Returns(new LocalizedString("MudDataGrid.Clear", "Reset", false));
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock.Object);

        // Act
        var result = internalMudLocalizer["MudDataGrid.Clear"];

        result.Should().BeEquivalentTo(new LocalizedString("MudDataGrid.Clear", "Reset", false));
    }

    [Test]
    [SetUICulture("en-US")]
    public void DefaultLocalizationInterceptor_EnglishUICulture()
    {
        // Arrange
        var interceptorMock = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer: null);
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock);

        // Act & Assert
        internalMudLocalizer["MudDataGrid.contains"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid.contains", "contains", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid.is empty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid.is empty", "is empty", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid.is not empty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid.is not empty", "is not empty", false, typeof(LanguageResource).FullName));
    }

    [Test]
    [SetUICulture("de-DE")]
    public void DefaultLocalizationInterceptor_NonEnglishUICulture()
    {
        // Arrange
        var interceptorMock = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer: null);
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock);

        // Act & Assert
        internalMudLocalizer["MudDataGrid.contains"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid.contains", "contains", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid.is empty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid.is empty", "is empty", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid.is not empty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid.is not empty", "is not empty", false, typeof(LanguageResource).FullName));
    }

    [Test]
    [SetUICulture("en-US")]
    public void DefaultLocalizationInterceptor_WithCustomMudLocalizer_EnglishUICulture()
    {
        // Arrange
        var mudLocalizerMock = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizerMock.Setup(mock => mock["MudDataGrid.is empty"]).Returns(new LocalizedString("MudDataGrid.is empty", "XXX", false));
        mudLocalizerMock.Setup(mock => mock["MudDataGrid.is not empty"]).Returns(new LocalizedString("MudDataGrid.is not empty", "MudDataGrid.is not empty", true));
        var interceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizerMock.Object);
        var internalMudLocalizer = new InternalMudLocalizer(interceptor);

        // Act & Assert
        internalMudLocalizer["MudDataGrid.contains"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid.contains", "contains", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid.is empty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid.is empty", "is empty", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid.is not empty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid.is not empty", "is not empty", false, typeof(LanguageResource).FullName));
    }

    [Test]
    [SetUICulture("de-DE")]
    public void DefaultLocalizationInterceptor_WithCustomMudLocalizer_NonEnglishUICulture()
    {
        // Arrange
        var mudLocalizerMock = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizerMock.Setup(mock => mock["MudDataGrid.is empty"]).Returns(new LocalizedString("MudDataGrid.is empty", "XXX", false));
        mudLocalizerMock.Setup(mock => mock["MudDataGrid.is not empty"]).Returns(new LocalizedString("MudDataGrid.is not empty", "MudDataGrid.is not empty", true));
        var interceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizerMock.Object);
        var internalMudLocalizer = new InternalMudLocalizer(interceptor);

        // Act & Assert
        internalMudLocalizer["MudDataGrid.contains"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid.contains", "contains", false, typeof(LanguageResource).FullName));
        internalMudLocalizer["MudDataGrid.is empty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid.is empty", "XXX", false));
        internalMudLocalizer["MudDataGrid.is not empty"].Should().BeEquivalentTo(new LocalizedString("MudDataGrid.is not empty", "is not empty", false, typeof(LanguageResource).FullName));
    }
}
