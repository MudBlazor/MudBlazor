using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;

namespace MudBlazor;

#nullable enable
/// <summary>
/// The <see cref="InternalMudLocalizer"/> service forwards translations to the <see cref="ILocalizationInterceptor"/> service.
/// By default, the <see cref="DefaultLocalizationInterceptor"/> is used, though custom implementations can be provided.
/// </summary>
internal sealed class InternalMudLocalizer : IStringLocalizer
{
    private readonly ILocalizationInterceptor _interceptor;
    private readonly Lazy<IStringLocalizer> _defaultLocalizationInterceptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalMudLocalizer"/> class with the specified <paramref name="interceptor"/>.
    /// </summary>
    /// <param name="interceptor">The localization interceptor to use for handling translations.</param>
    public InternalMudLocalizer(ILocalizationInterceptor interceptor)
    {
        ArgumentNullException.ThrowIfNull(interceptor);

        _interceptor = interceptor;
        // This is necessary in case the interceptor is replaced, and creating a ResourceManagerStringLocalizer involves a heavy operation using reflection.
        // Logging is not required for this operation.
        _defaultLocalizationInterceptor = new Lazy<IStringLocalizer>(() => AbstractLocalizationInterceptor.DefaultLanguageResourceReader(NullLoggerFactory.Instance));
    }

    /// <inheritdoc />
    IEnumerable<LocalizedString> IStringLocalizer.GetAllStrings(bool includeParentCultures)
    {
        // We already have access to our IStringLocalizer pointing at LanguageResource.
        if (_interceptor is AbstractLocalizationInterceptor abstractLocalizationInterceptor)
        {
            return abstractLocalizationInterceptor.Localizer.GetAllStrings(includeParentCultures);
        }

        return _defaultLocalizationInterceptor.Value.GetAllStrings(includeParentCultures);
    }

    /// <inheritdoc />
    LocalizedString IStringLocalizer.this[string key] => this[key, Array.Empty<object>()];

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="key">The name of the string resource.</param>
    /// <param name="arguments">The list of arguments to be passed to the string resource.</param>
    /// <returns>The string resource as a <see cref="LocalizedString" />.</returns>
    public LocalizedString this[string key, params object[] arguments] => _interceptor.Handle(key, arguments);
}
