using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MudBlazor.Docs.Extensions;
using MudBlazor.Resources;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.Localization;

#nullable enable
[TestFixture(Description = "Similar to InternalMudLocalizerTests but more narrow focusing on IgnoreDefaultEnglish cases.")]
public class DefaultLocalizationInterceptorTests
{
    [Test]
    public void IgnoreDefaultEnglishTrue_NullMudLocalizer_ReturnsDefaultEnglish()
    {
        // Even though IgnoreDefaultEnglish is "true", we still should get default English localization
        // Because we didn't provide custom MudLocalizer, so it will fall back.

        // Arrange
        var defaultLocalizationIInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer: null)
        {
            IgnoreDefaultEnglish = true
        };
        var internalMudLocalizer = new InternalMudLocalizer(defaultLocalizationIInterceptor);
        var resourceManager = LanguageResource.ResourceManager;
        var resourceSet = resourceManager
            .GetResourceSet(CultureInfo.InvariantCulture, true, true)
            .ToEnumerable()
            .ToDictionary(x => x.Key.ToString()!, x => x.Value?.ToString(), StringComparer.Ordinal);

        // Act & Result
        foreach (var resource in resourceSet)
        {
            var translation = internalMudLocalizer[resource.Key];

            translation.Value.Should().Be(resource.Value);
        }
    }

    [Test]
    [SetUICulture("en-US")]
    public void IgnoreDefaultEnglishFalse_CustomMudLocalizer_EnglishUICulture_ReturnsDefaultEnglish()
    {
        // When IgnoreDefaultEnglish is "false" and CurrentUICulture is "en", we return default English localization
        // Even when custom MudLocalizer is provided

        // Arrange
        var mudLocalizer = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizer.Setup(mock => mock["MudDataGrid.is empty"]).Returns(new LocalizedString("MudDataGrid.is empty", "XXX", resourceNotFound: false));
        var defaultLocalizationIInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer.Object)
        {
            IgnoreDefaultEnglish = false
        };
        var internalMudLocalizer = new InternalMudLocalizer(defaultLocalizationIInterceptor);

        // Act
        var result = internalMudLocalizer["MudDataGrid.is empty"];

        // Assert
        result.Value.Should().Be(LanguageResource.MudDataGrid_is_empty, "We default to english despite MudLocalizer, the value should be the one from the LanguageResource.");
    }

    [Test]
    [SetUICulture("fr-FR")]
    public void IgnoreDefaultEnglishFalse_CustomMudLocalizer_NonEnglishUICulture_ReturnsDefaultEnglish()
    {
        // When IgnoreDefaultEnglish is "false" and CurrentUICulture is NOT "en", we return localization provided from custom MudLocalizer

        // Arrange
        var mudLocalizer = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizer.Setup(mock => mock["MudDataGrid.is empty"]).Returns(new LocalizedString("MudDataGrid.is empty", "XXX", resourceNotFound: false));
        var defaultLocalizationIInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer.Object)
        {
            IgnoreDefaultEnglish = false
        };
        var internalMudLocalizer = new InternalMudLocalizer(defaultLocalizationIInterceptor);

        // Act
        var result = internalMudLocalizer["MudDataGrid.is empty"];

        // Assert
        result.Value.Should().Be("XXX", "The UICulture is not English therefore the value is the one from the Mock.");
    }
}
