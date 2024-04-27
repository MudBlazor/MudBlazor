using System;
using Microsoft.Extensions.Localization;

namespace MudBlazor;

#nullable enable
/// <summary>
/// The <see cref="InternalMudLocalizer"/> service forwards translations to the <see cref="ILocalizationInterceptor"/> service.
/// By default, the <see cref="DefaultLocalizationInterceptor"/> is used, though custom implementations can be provided.
/// </summary>
internal sealed class InternalMudLocalizer
{
    private readonly ILocalizationInterceptor _interceptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalMudLocalizer"/> class with the specified <paramref name="interceptor"/>.
    /// </summary>
    /// <param name="interceptor">The localization interceptor to use for handling translations.</param>
    public InternalMudLocalizer(ILocalizationInterceptor interceptor)
    {
        ArgumentNullException.ThrowIfNull(interceptor);

        _interceptor = interceptor;
    }

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="key">The name of the string resource.</param>
    /// <param name="arguments">The list of arguments to be passed to the string resource.</param>
    /// <returns>The string resource as a <see cref="LocalizedString" />.</returns>
    public LocalizedString this[string key, params string[] arguments] => _interceptor.Handle(key, arguments);
}
