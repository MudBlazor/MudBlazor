using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MudBlazor.Resources;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Provides a base implementation for <see cref="ILocalizationInterceptor"/>.
/// </summary>
public abstract class AbstractLocalizationInterceptor : ILocalizationInterceptor
{
    /// <summary>
    /// Gets the <see cref="IStringLocalizer"/> for internal translations.
    /// </summary>
    protected IStringLocalizer Localizer { get; }

    /// <summary>
    /// Gets the custom <see cref="MudBlazor.MudLocalizer"/> for additional translations, if provided.
    /// </summary>
    protected MudLocalizer? MudLocalizer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractLocalizationInterceptor"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="mudLocalizer">The optional custom MudLocalizer.</param>
    protected AbstractLocalizationInterceptor(ILoggerFactory loggerFactory, MudLocalizer? mudLocalizer = null)
    {
        var options = Options.Create(new LocalizationOptions());
        var factory = new ResourceManagerStringLocalizerFactory(options, loggerFactory);
        Localizer = factory.Create(typeof(LanguageResource));
        MudLocalizer = mudLocalizer;
    }

    /// <inheritdoc />
    public abstract LocalizedString Handle(string key);
}
