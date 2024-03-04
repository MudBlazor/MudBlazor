using System;
using System.Threading;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// The <see cref="DefaultLocalizationInterceptor"/>  handles translations, providing english as an included default language,
    /// while allowing users to add custom translations without restricting how they can be implemented.
    /// </summary>
    public class DefaultLocalizationInterceptor : AbstractLocalizationInterceptor
    {
        public virtual bool IgnoreDefaultEnglish { get; init; } = false;

        public DefaultLocalizationInterceptor(ILoggerFactory loggerFactory, MudLocalizer? mudLocalizer = null)
            : base(loggerFactory, mudLocalizer)
        {
        }

        public override LocalizedString Handle(string key)
        {
            if (!IgnoreDefaultEnglish)
            {
                // First check whether custom translations are available or the current ui culture is english, then we want to use the internal translations
                var currentCulture = Thread.CurrentThread.CurrentUICulture.Parent.TwoLetterISOLanguageName;
                if (MudLocalizer is null || currentCulture.Equals("en", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Localizer[key];
                }
            }

            return TranslationWithFallback(key);
        }

        protected virtual LocalizedString TranslationWithFallback(string key)
        {
            var defaultTranslation = Localizer[key];
            if (MudLocalizer is null)
            {
                return defaultTranslation;
            }

            // If CurrentUICulture is not english and a custom MudLocalizer service implementation is registered, try to use user provided languages.
            // If no translation was found, fallback to the internal English translation
            var res = MudLocalizer[key];
            return res.ResourceNotFound ? defaultTranslation : res;
        }
    }
}
