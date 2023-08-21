using System;
using System.Threading;
using Microsoft.Extensions.Localization;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// The <see cref="DefaultLocalizationInterceptor"/>  handles translations, providing english as an included default language,
    /// while allowing users to add custom translations without restricting how they can be implemented.
    /// </summary>
    internal class DefaultLocalizationInterceptor : ILocalizationInterceptor
    {
        public LocalizedString Handle(string key, IStringLocalizer localizer, MudLocalizer? mudLocalizer)
        {
            // First check whether custom translations are available or the current ui culture is english, then we want to use the internal translations
            var currentCulture = Thread.CurrentThread.CurrentUICulture.Parent.TwoLetterISOLanguageName;
            if (mudLocalizer == null || currentCulture.Equals("en", StringComparison.InvariantCultureIgnoreCase))
            {
                return localizer[key];
            }

            // If CurrentUICulture is not english and a custom MudLocalizer service implementation is registered, try to use user provided languages.
            // If no translation was found, fall back to the internal english translation
            var res = mudLocalizer[key];
            return res.ResourceNotFound ? localizer[key] : res;
        }
    }
}
