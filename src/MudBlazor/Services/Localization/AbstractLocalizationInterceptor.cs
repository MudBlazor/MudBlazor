using System.Collections.Concurrent;
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
        // Reading always happens under the invariant culture, so a key's value never changes and can be cached.
        // Only found keys are cached, because callers also pass arbitrary strings such as a conversion exception message and those must not accumulate.
        private readonly ConcurrentDictionary<string, LocalizedString> _cache = new(StringComparer.Ordinal);

        // The swap is repeated per member rather than shared through a Func, which would allocate a closure on every lookup.
        // Components read localized strings while rendering, so this runs in the render loop.
        // Each swap and restore is synchronous with no await in between, so the culture never leaks to another flow.
        public LocalizedString this[string name]
        {
            get
            {
                if (_cache.TryGetValue(name, out var cached))
                {
                    return cached;
                }

                var previous = CultureInfo.CurrentUICulture;
                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
                try
                {
                    var localized = inner[name];
                    if (!localized.ResourceNotFound)
                    {
                        _cache[name] = localized;
                    }

                    return localized;
                }
                finally
                {
                    CultureInfo.CurrentUICulture = previous;
                }
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var previous = CultureInfo.CurrentUICulture;
                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
                try
                {
                    return inner[name, arguments];
                }
                finally
                {
                    CultureInfo.CurrentUICulture = previous;
                }
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var previous = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            try
            {
                return inner.GetAllStrings(includeParentCultures).ToList();
            }
            finally
            {
                CultureInfo.CurrentUICulture = previous;
            }
        }
    }
}
