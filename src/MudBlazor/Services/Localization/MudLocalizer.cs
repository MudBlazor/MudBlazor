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
    public virtual LocalizedString this[string key] => new(key, key, true);
}
