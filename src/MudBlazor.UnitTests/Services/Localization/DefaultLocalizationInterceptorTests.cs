using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using MudBlazor.Docs.Extensions;
using MudBlazor.Resources;
using System;
using Microsoft.Extensions.Localization;
using Moq;

namespace MudBlazor.UnitTests.Services.Localization;

#nullable enable
[TestFixture]
public class DefaultLocalizationInterceptorTests
{
    [Test]
    public void Handle_IgnoreDefaultEnglishTrue_NullMudLocalizer_ReturnsDefaultEnglish()
    {
        // Even though we are using IgnoreDefaultEnglish is "true", we still should get default English localization
        // Because we didn't provide custom MudLocalizer, so it will fall back.

        // Arrange
        var resourceManager = LanguageResource.ResourceManager;
        var resourceSet = resourceManager
            .GetResourceSet(CultureInfo.InvariantCulture, true, true)
            .ToEnumerable()
            .ToDictionary(x => x.Key.ToString()!, x => x.Value?.ToString(), StringComparer.Ordinal);

        var defaultLocalizationIInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer: null)
        {
            IgnoreDefaultEnglish = true
        };

        // Act & Result
        foreach (var resource in resourceSet)
        {
            var translation = defaultLocalizationIInterceptor.Handle(resource.Key);

            translation.Value.Should().Be(resource.Value);
        }
    }

    [Test]
    [SetUICulture("en-GB")]
    public void Handle_IgnoreDefaultEnglishFalse_CustomMudLocalizer_EnglishUICulture_ReturnsDefaultEnglish()
    {
        // When IgnoreDefaultEnglish is "false" and CurrentUICulture is "en", we return default English localization
        // Even when custom MudLocalizer is provided

        // Arrange
        var mudLocalizer = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizer.Setup(mock => mock["MudDataGrid.is empty"]).Returns(new LocalizedString("MudDataGrid.is empty", "XXX", false));

        var defaultLocalizationIInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer.Object)
        {
            IgnoreDefaultEnglish = false
        };

        // Act
        var result = defaultLocalizationIInterceptor.Handle("MudDataGrid.is empty");

        // Assert
        result.Value.Should().Be(LanguageResource.MudDataGrid_is_empty);
    }

    [Test]
    [SetUICulture("fr-FR")]
    public void Handle_IgnoreDefaultEnglishFalse_CustomMudLocalizer_NonEnglishUICulture_ReturnsDefaultEnglish()
    {
        // When IgnoreDefaultEnglish is "false" and CurrentUICulture is NOT "en", we return localization provided from custom MudLocalizer

        // Arrange
        var mudLocalizer = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizer.Setup(mock => mock["MudDataGrid.is empty"]).Returns(new LocalizedString("MudDataGrid.is empty", "XXX", false));

        var defaultLocalizationIInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer.Object)
        {
            IgnoreDefaultEnglish = false
        };

        // Act
        var result = defaultLocalizationIInterceptor.Handle("MudDataGrid.is empty");

        // Assert
        result.Value.Should().Be("XXX");
    }
}
