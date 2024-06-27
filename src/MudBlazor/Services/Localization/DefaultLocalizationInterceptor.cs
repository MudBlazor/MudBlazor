using System;
using System.Threading;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace MudBlazor;

#nullable enable
/// <summary>
/// The <see cref="DefaultLocalizationInterceptor"/> manages translations, incorporating English as the default language,
/// facilitating the addition of custom translations without imposing limitations on their implementation.
/// </summary>
public class DefaultLocalizationInterceptor : AbstractLocalizationInterceptor
{
    /// <summary>
    /// Gets or sets a value indicating whether to ignore default English translations.
    /// </summary>
    public virtual bool IgnoreDefaultEnglish { get; init; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultLocalizationInterceptor"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="mudLocalizer">The optional custom <see cref="MudLocalizer"/>.</param>
    public DefaultLocalizationInterceptor(ILoggerFactory loggerFactory, MudLocalizer? mudLocalizer = null)
        : base(loggerFactory, mudLocalizer)
    {
    }

    /// <inheritdoc />
    public override LocalizedString Handle(string key, params object[] arguments)
    {
        if (!IgnoreDefaultEnglish)
        {
            // First check whether custom translations are available or the current ui culture is english, then we want to use the internal translations
            var currentCulture = Thread.CurrentThread.CurrentUICulture.Parent.TwoLetterISOLanguageName;
            if (MudLocalizer is null || currentCulture.Equals("en", StringComparison.InvariantCultureIgnoreCase))
            {
                return Localizer[key, arguments];
            }
        }

        return TranslationWithFallback(key);
    }

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="key">The name of the string resource</param>
    /// <returns>The string resource as a <see cref="LocalizedString" />.</returns>
    /// <remarks>
    /// This method is called when the default English translation is ignored or unavailable, and a custom MudLocalizer service implementation is registered.
    /// It attempts to use user-provided languages, falling back to the internal English translation if MudLocalizer is missing or no resource is found.
    /// </remarks>
    protected virtual LocalizedString TranslationWithFallback(string key)
    {
        var defaultTranslation = Localizer[key];
        if (MudLocalizer is null)
        {
            return defaultTranslation;
        }

        // If CurrentUICulture is not english and a custom MudLocalizer service implementation is registered, try to use user provided languages.
        // If no translation was found, fallback to the internal English translation
        var res = MudLocalizer[key];
        return res.ResourceNotFound ? defaultTranslation : res;
    }
}
