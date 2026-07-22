using System.Globalization;
using Microsoft.Extensions.Localization;

namespace MudBlazor;

#nullable enable
/// <summary>
/// This customizable localizer service allows users to supply custom translations for MudBlazor components.
/// Users can register custom implementations using the following syntax, where the scope depends on the implementation:
/// <c>services.Add{scope}Transient&lt;MudLocalizer, CustomMudLocalizerImpl&gt;()</c>
/// or
/// <c>services.TryAdd{scope}Transient&lt;MudLocalizer, CustomMudLocalizerImpl&gt;()</c>
/// </summary>
public class MudLocalizer
{
    /// <summary>
    /// Retrieves the translation for the specified translation key.
    /// <para/>
    /// <b>NB!</b> Override this method to supply custom translations.
    /// </summary>
    /// <param name="key">The name of the string resource.</param>
    /// <remarks>
    /// The value of  <see cref="LocalizedString.ResourceNotFound"/> should be <c>true</c> if no translation is available for the specified key.
    /// </remarks>
    /// <returns>The string resource as a <see cref="LocalizedString" />.</returns>
    public virtual LocalizedString this[string key] => new(key, key, resourceNotFound: true);

    /// <summary>
    /// Retrieves the translation for the specified translation key.
    /// <para/>
    /// <b>NB!</b> Override this method to supply custom translations.
    /// </summary>
    /// <param name="key">The name of the string resource.</param>
    /// <param name="arguments">The list of arguments to be passed to the string resource</param>
    /// <remarks>
    /// The value of  <see cref="LocalizedString.ResourceNotFound"/> should be <c>true</c> if no translation is available for the specified key.
    /// </remarks>
    /// <returns>The string resource as a <see cref="LocalizedString" />.</returns>
    public virtual LocalizedString this[string key, params object[] arguments]
    {
        get
        {
            // This method was introduced later, in order to maintain backward compatibility, we first invoke the original method without arguments.
            // If arguments are provided, we then format the string accordingly.
            // This approach accommodates two scenarios:
            // 1. Users might have overridden only the original method without arguments.
            // 2. Users might have mocked only the original method without arguments.
            // Therefore, we wrap the result from the original non argument method in a new LocalizedString.
            var anyArguments = arguments.Length > 0;
            var localizedString = this[key];
            if (!anyArguments)
            {
                return localizedString;
            }

            var formatedString = string.Format(localizedString.Value, arguments);
            localizedString = new LocalizedString(localizedString.Name, formatedString, localizedString.ResourceNotFound);

            return localizedString;
        }
    }
}
