using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace MudBlazor;

/// <summary>
/// Internal localization entry point that delegates translation work to configured interceptors.
/// </summary>
/// <remarks>
/// Components access localization through this class so they can be decoupled from specific resource providers. By default it uses <see cref="DefaultLocalizationInterceptor"/>, but applications can plug in custom interceptors.
/// </remarks>
internal sealed class InternalMudLocalizer
{
    private readonly ILocalizationInterceptor _interceptor;
    private readonly ILocalizationEnumInterceptor _enumInterceptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalMudLocalizer"/> class.
    /// </summary>
    /// <param name="interceptor">The localization interceptor to use for handling translations.</param>
    public InternalMudLocalizer(ILocalizationInterceptor interceptor)
        : this(interceptor, new DefaultLocalizationEnumInterceptor(interceptor))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalMudLocalizer"/> class.
    /// </summary>
    /// <param name="interceptor">The localization interceptor to use for handling translations.</param>
    /// <param name="enumInterceptor">The localization interceptor to use for handling enumeration translations.</param>
    [ActivatorUtilitiesConstructor]
    public InternalMudLocalizer(ILocalizationInterceptor interceptor, ILocalizationEnumInterceptor enumInterceptor)
    {
        ArgumentNullException.ThrowIfNull(interceptor);
        ArgumentNullException.ThrowIfNull(enumInterceptor);

        _interceptor = interceptor;
        _enumInterceptor = enumInterceptor;
    }

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="key">The name of the string resource.</param>
    /// <param name="arguments">The list of arguments to be passed to the string resource.</param>
    /// <returns>The string resource as a <see cref="LocalizedString" />.</returns>
    public LocalizedString this[string key, params object[] arguments] => _interceptor.Handle(key, arguments);

    /// <summary>
    /// Localizes the specified enumeration value.
    /// </summary>
    /// <param name="enumeration">The enumeration value to be localized.</param>
    /// <returns>The localized representation of the enumeration value.</returns>
    public string this[Enum enumeration] => _enumInterceptor.Handle(enumeration);
}
