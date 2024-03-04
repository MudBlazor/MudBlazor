using Microsoft.Extensions.Localization;

namespace MudBlazor
{
    #nullable enable
    /// <summary>
    /// Customizable localization interceptor service which can be implemented by users to customize existing localizations,
    /// add e.g. logging of missing translations or even completely modify how localizations are chosen.
    /// Custom implementations can be registered like this (scope depends on the implementation):
    /// <code>services.Add{scope}Transient&lt;ILocalizationInterceptor, CustomMudLocalizerImpl&gt;()</code>
    /// or
    /// <code>services.TryAdd{scope}Transient&lt;ILocalizationInterceptor, CustomMudLocalizerImpl&gt;()</code>
    /// </summary>
    public interface ILocalizationInterceptor
    {
        /// <summary>
        /// Gets the translation for the given translation key.
        /// Implement logic for choosing translations, overrides etc here.
        /// </summary>
        /// <param name="key">The translation key to look up</param>
        /// <remarks>
        /// The <see cref="LocalizedString.ResourceNotFound"/> should be <c>true</c> if no translation is provided for some translation key.
        /// </remarks>
        /// <returns><see cref="LocalizedString"/> with the translation.</returns>
        LocalizedString Handle(string key);
    }
}
