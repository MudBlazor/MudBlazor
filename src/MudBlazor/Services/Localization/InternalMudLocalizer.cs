using Microsoft.Extensions.Localization;
using System;

namespace MudBlazor;

#nullable enable
/// <summary>
/// The <see cref="InternalMudLocalizer"/> service forwards translations to the <see cref="ILocalizationInterceptor"/> service.
/// By default, the <see cref="DefaultLocalizationInterceptor"/> is used, though custom implementations can be provided.
/// </summary>
internal sealed class InternalMudLocalizer
{
    private readonly ILocalizationInterceptor _interceptor;

    public InternalMudLocalizer(ILocalizationInterceptor interceptor)
    {
        ArgumentNullException.ThrowIfNull(interceptor);

        _interceptor = interceptor;
    }

    /// <summary>
    /// Gets the translation for the given translation key from the <see cref="ILocalizationInterceptor"/>.
    /// </summary>
    /// <param name="key">The translation key to look up.</param>
    /// <returns>The string resource as a <see cref="LocalizedString"/>.</returns>
    public LocalizedString this[string key] => _interceptor.Handle(key);
}
