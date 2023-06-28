using Microsoft.Extensions.Localization;

namespace MudBlazor
{
    /// <summary>
    /// Customizable localizer service which can be implemented by users to provide custom translations for MudBlazor components.
    /// Custom implementations can be registered like this (scope depends on the implementation):
    /// <code>services.Add{scope}Transient&lt;MudLocalizer, CustomMudLocalizerImpl&gt;()</code>
    /// or
    /// <code>services.TryAdd{scope}Transient&lt;MudLocalizer, CustomMudLocalizerImpl&gt;()</code>
    /// Though 
    /// </summary>
    public class MudLocalizer
    {
        /// <summary>
        /// Gets the translation for the given translation key.
        /// Override this method to provide your custom translations.
        /// </summary>
        /// <param name="key">the translation key to look up</param>
        /// <returns><see cref="LocalizedString"/> with the custom translation. <see cref="LocalizedString.ResourceNotFound"/> should be <c>true</c> if no custom translation is provided for some translation key</returns>
        public virtual LocalizedString this[string key] => new(key, key, true);
    }
}
