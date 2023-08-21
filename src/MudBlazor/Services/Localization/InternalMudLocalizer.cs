using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MudBlazor.Resources;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// The <see cref="InternalMudLocalizer"/> service forwards translations to the <see cref="ILocalizationInterceptor"/> service.
    /// By default the <see cref="DefaultLocalizationInterceptor"/> is used, though custom implementations can be provided.
    /// </summary>
    internal sealed class InternalMudLocalizer
    {
        private readonly ILocalizationInterceptor _interceptor;
        private readonly IStringLocalizer _localizer;
        private readonly MudLocalizer? _mudLocalizer;

        public InternalMudLocalizer(ILoggerFactory loggerFactory, MudLocalizer? mudLocalizer = null, ILocalizationInterceptor? interceptor = null)
        {
            var options = Options.Create(new LocalizationOptions());
            var factory = new ResourceManagerStringLocalizerFactory(options, loggerFactory);
            _localizer = factory.Create(typeof(LanguageResource));
            _mudLocalizer = mudLocalizer;
            
            _interceptor = interceptor ?? new DefaultLocalizationInterceptor();
        }

        /// <summary>
        /// Gets the translation for the given translation key from the <see cref="ILocalizationInterceptor"/>.
        /// </summary>
        /// <param name="key">the translation key to look up</param>
        /// <returns>The string resource as a <see cref="LocalizedString"/>.</returns>
        public LocalizedString this[string key]
        {
            get
            {
                return _interceptor.Handle(key, _localizer, _mudLocalizer);
            }
        }
    }
}
