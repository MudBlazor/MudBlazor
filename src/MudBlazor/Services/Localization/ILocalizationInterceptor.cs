using Microsoft.Extensions.Localization;
using MudBlazor.Services;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Customizable localization interceptor which can be implemented by users to customize existing localizations,
/// add e.g. logging of missing translations or even completely modify how localizations are chosen.
/// Custom implementations can be registered like this <see cref="ServiceCollectionExtensions.AddLocalizationInterceptor{TInterceptor}"/>.
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
