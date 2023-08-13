using System;
using System.Threading;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MudBlazor.Resources;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// The <see cref="InternalMudLocalizer"/> service handles translations, providing english as an included default language,
    /// while allowing users to add custom translations without restricting how they can be implemented.
    /// </summary>
    internal sealed class InternalMudLocalizer
    {
        private readonly IStringLocalizer _localizer;
        private readonly MudLocalizer? _mudLocalizer;

        public InternalMudLocalizer(ILoggerFactory loggerFactory, MudLocalizer? mudLocalizer = null)
        {
            var factory = new ResourceManagerStringLocalizerFactory(Options.Create(new LocalizationOptions()), loggerFactory);
            _localizer = factory.Create(typeof(LanguageResource));
            _mudLocalizer = mudLocalizer;
        }

        /// <summary>
        /// Gets the translation for the given translation key.
        /// </summary>
        /// <param name="key">the translation key to look up</param>
        /// <returns>The string resource as a <see cref="LocalizedString"/>.</returns>
        public LocalizedString this[string key]
        {
            get
            {
                // First check whether custom translations are available or the current ui culture is english, then we want to use the internal translations
                var currentCulture = Thread.CurrentThread.CurrentUICulture.Parent.TwoLetterISOLanguageName;
                if (_mudLocalizer == null || currentCulture.Equals("en", StringComparison.InvariantCultureIgnoreCase))
                {
                    return _localizer[key];
                }

                // If CurrentUICulture is not english and a custom MudLocalizer service implementation is registered, try to use user provided languages.
                // If no translation was found, fall back to the internal english translation
                var res = _mudLocalizer[key];
                return res.ResourceNotFound ? _localizer[key] : res;
            }
        }
    }
}
