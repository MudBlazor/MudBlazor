using System.Globalization;
using AwesomeAssertions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MudBlazor.Resources;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.Localization;

#nullable enable
[TestFixture(Description = "Similar to InternalMudLocalizerTests but more narrow focusing on IgnoreDefaultEnglish cases.")]
public class DefaultLocalizationInterceptorTests
{
    [Test]
    public void NullMudLocalizer_IgnoreDefaultEnglishTrue_ReturnsDefaultEnglish()
    {
        // Even though IgnoreDefaultEnglish is "true", we still should get default English localization
        // Because we didn't provide custom MudLocalizer, so it will fall back.

        // Arrange
        var defaultLocalizationInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer: null)
        {
            IgnoreDefaultEnglish = true
        };
        var internalMudLocalizer = new InternalMudLocalizer(defaultLocalizationInterceptor);
        var resourceManager = LanguageResource.ResourceManager;
        var resourceSet = resourceManager
            .GetResourceSet(CultureInfo.InvariantCulture, true, true)
            .ToEnumerable()
            .ToDictionary(x => (string)x.Key, x => x.Value?.ToString(), StringComparer.Ordinal);

        // Act & Result
        foreach (var resource in resourceSet)
        {
            var translation = internalMudLocalizer[resource.Key];

            translation.Value.Should().Be(resource.Value);
        }
    }

    [Test]
    [SetUICulture("en-US")]
    public void CustomMudLocalizer_IgnoreDefaultEnglishFalse_EnglishUICulture_ReturnsDefaultEnglish()
    {
        // When IgnoreDefaultEnglish is "false" and CurrentUICulture is "en", we return default English localization
        // Even when custom MudLocalizer is provided

        // Arrange
        var mudLocalizer = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizer.Setup(mock => mock[LanguageResource.MudDataGrid_IsEmpty]).Returns(new LocalizedString(LanguageResource.MudDataGrid_IsEmpty, "XXX", resourceNotFound: false));
        var defaultLocalizationInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer.Object)
        {
            IgnoreDefaultEnglish = false
        };
        var internalMudLocalizer = new InternalMudLocalizer(defaultLocalizationInterceptor);

        // Act
        var result = internalMudLocalizer[LanguageResource.MudDataGrid_IsEmpty];

        // Assert
        result.Value.Should().Be(LanguageResource.ResourceManager.GetString(LanguageResource.MudDataGrid_IsEmpty), "We default to english despite MudLocalizer, the value should be the one from the LanguageResource.");
    }

    [Test]
    [SetUICulture("fr-FR")]
    public void CustomMudLocalizer_IgnoreDefaultEnglishFalse_NonEnglishUICulture_ReturnsCustomLocalization()
    {
        // When IgnoreDefaultEnglish is "false" and CurrentUICulture is NOT "en", we return localization provided from custom MudLocalizer

        // Arrange
        var mudLocalizer = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizer.Setup(mock => mock[LanguageResource.MudDataGrid_IsEmpty]).Returns(new LocalizedString(LanguageResource.MudDataGrid_IsEmpty, "XXX", resourceNotFound: false));
        var defaultLocalizationInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer.Object)
        {
            IgnoreDefaultEnglish = false
        };
        var internalMudLocalizer = new InternalMudLocalizer(defaultLocalizationInterceptor);

        // Act
        var result = internalMudLocalizer[LanguageResource.MudDataGrid_IsEmpty];

        // Assert
        result.Value.Should().Be("XXX", "The UICulture is not English therefore the value is the one from the Mock.");
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    [SetUICulture("fr-FR")]
    public void CustomMudLocalizer_IgnoreDefaultEnglishFalse_TemplateTranslation_ReturnsFormattedString(bool ignoreDefaultEnglish)
    {
        // Regardless of the IgnoreDefaultEnglish setting, the custom localization should be returned.
        // This is because custom MudLocalizer was provided, and we have an existing key.

        // Arrange
        var mudLocalizer = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizer.Setup(mock => mock["TemplateString"]).Returns(new LocalizedString("TemplateString", "Bonjour {0}!", resourceNotFound: false));
        var defaultLocalizationInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer.Object)
        {
            IgnoreDefaultEnglish = ignoreDefaultEnglish
        };
        var internalMudLocalizer = new InternalMudLocalizer(defaultLocalizationInterceptor);

        // Act
        var result = internalMudLocalizer["TemplateString", "le monde"];

        // Assert
        result.Value.Should().Be("Bonjour le monde!", "The value should be the template string with the provided parameter.");
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    [SetUICulture("en-US")]
    public void CustomMudLocalizer_DefaultLocalizationFallback_TemplateTranslation_ReturnsFormattedString(bool ignoreDefaultEnglish)
    {
        // Regardless of the IgnoreDefaultEnglish setting, the default English localization should be returned.
        // Although a custom MudLocalizer is provided, the key does not exist, resulting in resourceNotFound being "true".
        // Consequently, this causes a fallback to the default English localization.

        // Arrange
        var mudLocalizer = new Mock<MudLocalizer> { CallBase = true };
        var defaultLocalizationInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer.Object)
        {
            IgnoreDefaultEnglish = ignoreDefaultEnglish
        };
        var internalMudLocalizer = new InternalMudLocalizer(defaultLocalizationInterceptor);

        // Act

        var result = internalMudLocalizer[LanguageResource.MudBaseDatePicker_PrevMonth, "2024"];

        // Assert
        var expectedValue = GetResourceString(LanguageResource.MudBaseDatePicker_PrevMonth, "2024");
        result.Value.Should().Be(expectedValue);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    [SetUICulture("en-US")]
    public void CustomMudLocalizer_DefaultLocalizationFallback_KeyTranslation_ReturnsString(bool ignoreDefaultEnglish)
    {
        // Regardless of the IgnoreDefaultEnglish setting, the default English localization should be returned.
        // Although a custom MudLocalizer is provided, the key does not exist, resulting in resourceNotFound being "true".
        // Consequently, this causes a fallback to the default English localization.

        // Arrange
        var mudLocalizer = new Mock<MudLocalizer> { CallBase = true };
        var defaultLocalizationInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer.Object)
        {
            IgnoreDefaultEnglish = ignoreDefaultEnglish
        };
        var internalMudLocalizer = new InternalMudLocalizer(defaultLocalizationInterceptor);

        // Act
        var result = internalMudLocalizer[LanguageResource.MudColorPicker_ModeSwitch];

        // Assert
        var expectedValue = GetResourceString(LanguageResource.MudColorPicker_ModeSwitch);
        result.Value.Should().Be(expectedValue);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    [SetUICulture("en-US")]
    public void NullMudLocalizer_DefaultLocalizationFallback_TemplateTranslation_ReturnsFormattedString(bool ignoreDefaultEnglish)
    {
        // Regardless of the IgnoreDefaultEnglish setting, the default English localization should be returned.
        // This is because no custom MudLocalizer was provided, causing a fallback to the default English localization even when IgnoreDefaultEnglish is "true".

        // Arrange
        var defaultLocalizationIInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer: null)
        {
            IgnoreDefaultEnglish = ignoreDefaultEnglish
        };
        var internalMudLocalizer = new InternalMudLocalizer(defaultLocalizationIInterceptor);

        // Act
        var result = internalMudLocalizer[LanguageResource.MudBaseDatePicker_PrevMonth, "2024"];

        // Assert
        var expectedValue = GetResourceString(LanguageResource.MudBaseDatePicker_PrevMonth, "2024");
        result.Value.Should().Be(expectedValue, "The value should be the template string with the provided parameter.");
    }

    [Test]
    [NonParallelizable]
    [SetUICulture("sv-SE")]
    public void DefaultEnglishLookup_NonEnglishUICulture_DoesNotProbeSatelliteAssembly()
    {
        // Regression test for #13461. Reading the built-in English resources under a non-English UI culture must not make the ResourceManager probe for a (non-existent) MudBlazor.resources satellite assembly.
        // On the Blazor WebAssembly runtime each probe throws a first-chance FileNotFoundException; on the runtime used by the test host the same probe surfaces as an AssemblyResolve request.
        // A value-only assertion is not enough: the English fallback resolves correctly either way, so the test asserts that the probe never happens.

        // Arrange
        var defaultLocalizationInterceptor = new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer: null);
        var internalMudLocalizer = new InternalMudLocalizer(defaultLocalizationInterceptor);
        var satelliteProbes = 0;
        ResolveEventHandler handler = (_, args) =>
        {
            if (args.Name.StartsWith("MudBlazor.resources", StringComparison.OrdinalIgnoreCase))
            {
                Interlocked.Increment(ref satelliteProbes);
            }

            return null;
        };
        AppDomain.CurrentDomain.AssemblyResolve += handler;

        // Act
        LocalizedString result;
        try
        {
            result = internalMudLocalizer[LanguageResource.MudDataGrid_Clear];
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= handler;
        }

        // Assert
        satelliteProbes.Should().Be(0, "the invariant-culture lookup must not probe for a culture-specific satellite assembly");
        result.ResourceNotFound.Should().BeFalse();
        result.Value.Should().Be("Clear", "the built-in English fallback is still returned under a non-English UI culture");
    }

    private static string GetResourceString(string key, params object[] parameters)
    {
        var resourceString = LanguageResource.GetResourceString(key) ?? string.Empty;

        return string.Format(CultureInfo.CurrentUICulture, resourceString, parameters);
    }
}
