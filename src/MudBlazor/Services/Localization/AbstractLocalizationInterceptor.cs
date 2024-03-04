using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MudBlazor.Resources;

namespace MudBlazor;

#nullable enable
public abstract class AbstractLocalizationInterceptor : ILocalizationInterceptor
{
    protected IStringLocalizer Localizer { get; }

    protected MudLocalizer? MudLocalizer { get; }

    protected AbstractLocalizationInterceptor(ILoggerFactory loggerFactory, MudLocalizer? mudLocalizer = null)
    {
        var options = Options.Create(new LocalizationOptions());
        var factory = new ResourceManagerStringLocalizerFactory(options, loggerFactory);
        Localizer = factory.Create(typeof(LanguageResource));
        MudLocalizer = mudLocalizer;
    }

    public abstract LocalizedString Handle(string key);
}
