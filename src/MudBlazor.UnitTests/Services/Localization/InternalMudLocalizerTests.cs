using AwesomeAssertions;
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
    public void Indexer_ReturnsString_SoBlazorCanCompareItByValue()
    {
        // Blazor only compares parameters by value for known-immutable types, and ChangeDetection.MayHaveChanged returns true for every other reference type.
        // A LocalizedString return would therefore rebuild any component given a localized attribute, on every render of its parent, forever.
        var indexer = typeof(InternalMudLocalizer).GetProperty("Item", new[] { typeof(string), typeof(object[]) });

        indexer.Should().NotBeNull();
        indexer!.PropertyType.Should().Be<string>();
    }

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
    public void Constructor_WithNullEnumInterceptor_ShouldThrowArgumentNullException()
    {
        // Arrange
        var interceptorMock = new Mock<ILocalizationInterceptor>();
        ILocalizationEnumInterceptor? enumInterceptor = null;

        // Act
        var construct = () => new InternalMudLocalizer(interceptorMock.Object, enumInterceptor!);

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
        interceptorMock.Setup(mock => mock.Handle(LanguageResource.MudDataGrid_Clear)).Returns(new LocalizedString(LanguageResource.MudDataGrid_Clear, "Reset", false));
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock.Object);

        // Act
        var result = internalMudLocalizer[LanguageResource.MudDataGrid_Clear];

        // Assert
        result.Should().Be("Reset");
    }

    [Test]
    [SetUICulture("de-DE")]
    public void CustomLocalizationInterceptor_NonEnglishUICulture()
    {
        // Assert
        var interceptorMock = new Mock<ILocalizationInterceptor>();
        interceptorMock.Setup(mock => mock.Handle(LanguageResource.MudDataGrid_Clear)).Returns(new LocalizedString(LanguageResource.MudDataGrid_Clear, "Reset", false));
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock.Object);

        // Act
        var result = internalMudLocalizer[LanguageResource.MudDataGrid_Clear];

        result.Should().Be("Reset");
    }

    [Test]
    [SetUICulture("en-US")]
    public void DefaultLocalizationInterceptor_EnglishUICulture()
    {
        // Arrange
        var interceptorMock = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer: null);
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock);

        // Act & Assert
        internalMudLocalizer[LanguageResource.MudDataGrid_Contains].Should().Be("contains");
        internalMudLocalizer[LanguageResource.MudDataGrid_IsEmpty].Should().Be("is empty");
        internalMudLocalizer[LanguageResource.MudDataGrid_IsNotEmpty].Should().Be("is not empty");
    }

    [Test]
    [SetUICulture("de-DE")]
    public void DefaultLocalizationInterceptor_NonEnglishUICulture()
    {
        // Arrange
        var interceptorMock = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer: null);
        var internalMudLocalizer = new InternalMudLocalizer(interceptorMock);

        // Act & Assert
        internalMudLocalizer[LanguageResource.MudDataGrid_Contains].Should().Be("contains");
        internalMudLocalizer[LanguageResource.MudDataGrid_IsEmpty].Should().Be("is empty");
        internalMudLocalizer[LanguageResource.MudDataGrid_IsNotEmpty].Should().Be("is not empty");
    }

    [Test]
    [SetUICulture("en-US")]
    public void DefaultLocalizationInterceptor_WithCustomMudLocalizer_EnglishUICulture()
    {
        // Arrange
        var mudLocalizerMock = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizerMock.Setup(mock => mock[LanguageResource.MudDataGrid_IsEmpty]).Returns(new LocalizedString(LanguageResource.MudDataGrid_IsEmpty, "XXX", false));
        mudLocalizerMock.Setup(mock => mock[LanguageResource.MudDataGrid_IsNotEmpty]).Returns(new LocalizedString(LanguageResource.MudDataGrid_IsNotEmpty, "MudDataGrid_IsNotEmpty", true));
        var interceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizerMock.Object);
        var internalMudLocalizer = new InternalMudLocalizer(interceptor);

        // Act & Assert
        internalMudLocalizer[LanguageResource.MudDataGrid_Contains].Should().Be("contains");
        internalMudLocalizer[LanguageResource.MudDataGrid_IsEmpty].Should().Be("is empty");
        internalMudLocalizer[LanguageResource.MudDataGrid_IsNotEmpty].Should().Be("is not empty");
    }

    [Test]
    [SetUICulture("de-DE")]
    public void DefaultLocalizationInterceptor_WithCustomMudLocalizer_NonEnglishUICulture()
    {
        // Arrange
        var mudLocalizerMock = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizerMock.Setup(mock => mock[LanguageResource.MudDataGrid_IsEmpty]).Returns(new LocalizedString(LanguageResource.MudDataGrid_IsEmpty, "XXX", false));
        mudLocalizerMock.Setup(mock => mock[LanguageResource.MudDataGrid_IsNotEmpty]).Returns(new LocalizedString(LanguageResource.MudDataGrid_IsNotEmpty, "MudDataGrid_IsNotEmpty", true));
        var interceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizerMock.Object);
        var internalMudLocalizer = new InternalMudLocalizer(interceptor);

        // Act & Assert
        internalMudLocalizer[LanguageResource.MudDataGrid_Contains].Should().Be("contains");
        internalMudLocalizer[LanguageResource.MudDataGrid_IsEmpty].Should().Be("XXX");
        internalMudLocalizer[LanguageResource.MudDataGrid_IsNotEmpty].Should().Be("is not empty");
    }
}
