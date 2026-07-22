using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MudBlazor.Resources;

namespace MudBlazor;

/// <summary>
/// Base class for localization interceptors that can swap or augment MudBlazor translations.
/// </summary>
/// <remarks>
/// Derive from this when you need custom resource sources or fallback logic beyond the defaults provided by <see cref="DefaultLocalizationInterceptor"/>.
/// </remarks>
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

        // MudBlazor ships no satellite assemblies, so look up the built-in English strings under the invariant culture to avoid probing for a non-existent MudBlazor.resources satellite under non-English UI cultures (#13461).
        return new InvariantLanguageResourceLocalizer(factory.Create(typeof(LanguageResource)));
    }

    /// <summary>
    /// Resolves the built-in English resources under the invariant culture. Only <see cref="CultureInfo.CurrentUICulture"/> is pinned, so <see cref="CultureInfo.CurrentCulture"/> still formats arguments in the user's culture.
    /// </summary>
    private sealed class InvariantLanguageResourceLocalizer(IStringLocalizer inner) : IStringLocalizer
    {
        public LocalizedString this[string name]
            => ReadInvariant(() => inner[name]);

        public LocalizedString this[string name, params object[] arguments]
            => ReadInvariant(() => inner[name, arguments]);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            => ReadInvariant(() => inner.GetAllStrings(includeParentCultures).ToList());

        private static T ReadInvariant<T>(Func<T> read)
        {
            // Synchronous swap/restore with no await in between, so the culture never leaks to another flow.
            var previous = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            try
            {
                return read();
            }
            finally
            {
                CultureInfo.CurrentUICulture = previous;
            }
        }
    }
}
