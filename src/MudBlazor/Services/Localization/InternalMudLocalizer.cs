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
        private readonly ILocalizationInterceptor _interceptor;

        public InternalMudLocalizer(ILoggerFactory loggerFactory, MudLocalizer? mudLocalizer = null, ILocalizationInterceptor? interceptor = null)
        {
            var options = Options.Create(new LocalizationOptions());
            var factory = new ResourceManagerStringLocalizerFactory(options, loggerFactory);
            var localizer = factory.Create(typeof(LanguageResource));
            
            _interceptor = interceptor ?? new DefaultLocalizationInterceptor(localizer, mudLocalizer);
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
                return _interceptor.Handle(key);
            }
        }
    }
}
