using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MudBlazor.Resources;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Provides a base implementation for <see cref="ILocalizationInterceptor"/>.
/// </summary>
public abstract class AbstractLocalizationInterceptor : ILocalizationInterceptor
{
    /// <summary>
    /// Gets the <see cref="IStringLocalizer"/> for internal translations.
    /// </summary>
    protected internal IStringLocalizer Localizer { get; }

    /// <summary>
    /// Gets the custom <see cref="MudBlazor.MudLocalizer"/> for additional translations, if provided.
    /// </summary>
    protected internal MudLocalizer? MudLocalizer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractLocalizationInterceptor"/> class.
    /// This creates an ResX reader for builtin <see cref="LanguageResource"/> with the default <see cref="LocalizationOptions"/>.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="mudLocalizer">The optional custom MudLocalizer.</param>
    /// <remarks>
    /// For more custom options use <see cref="AbstractLocalizationInterceptor(IStringLocalizer,MudBlazor.MudLocalizer)"/> constuctor.
    /// </remarks>
    protected AbstractLocalizationInterceptor(ILoggerFactory loggerFactory, MudLocalizer? mudLocalizer = null)
        : this(DefaultLanguageResourceReader(loggerFactory), mudLocalizer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractLocalizationInterceptor"/> class.
    /// </summary>
    /// <param name="localizer">The instance of <see cref="IStringLocalizer"/>.</param>
    /// <param name="mudLocalizer">The optional custom MudLocalizer.</param>
    protected AbstractLocalizationInterceptor(IStringLocalizer localizer, MudLocalizer? mudLocalizer = null)
    {
        Localizer = localizer;
        MudLocalizer = mudLocalizer;
    }

    /// <inheritdoc />
    public abstract LocalizedString Handle(string key, params object[] arguments);

    internal static IStringLocalizer DefaultLanguageResourceReader(ILoggerFactory loggerFactory)
    {
        var options = Options.Create(new LocalizationOptions());
        var factory = new ResourceManagerStringLocalizerFactory(options, loggerFactory);

        return factory.Create(typeof(LanguageResource));
    }
}
