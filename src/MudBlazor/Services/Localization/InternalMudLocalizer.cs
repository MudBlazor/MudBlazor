using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;

namespace MudBlazor;

#nullable enable
/// <summary>
/// The <see cref="InternalMudLocalizer"/> service forwards translations to the <see cref="ILocalizationInterceptor"/> service.
/// By default, the <see cref="DefaultLocalizationInterceptor"/> is used, though custom implementations can be provided.
/// </summary>
internal sealed class InternalMudLocalizer : IStringLocalizer
{
    private readonly ILocalizationInterceptor _interceptor;
    private readonly Lazy<IStringLocalizer> _defaultLocalizationInterceptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalMudLocalizer"/> class with the specified <paramref name="interceptor"/>.
    /// </summary>
    /// <param name="interceptor">The localization interceptor to use for handling translations.</param>
    public InternalMudLocalizer(ILocalizationInterceptor interceptor)
    {
        ArgumentNullException.ThrowIfNull(interceptor);

        _interceptor = interceptor;
        // This is necessary in case the interceptor is replaced, and creating a ResourceManagerStringLocalizer involves a heavy operation using reflection.
        // Logging is not required for this operation.
        _defaultLocalizationInterceptor = new Lazy<IStringLocalizer>(() => AbstractLocalizationInterceptor.DefaultLanguageResourceReader(NullLoggerFactory.Instance));
    }

    /// <inheritdoc />
    IEnumerable<LocalizedString> IStringLocalizer.GetAllStrings(bool includeParentCultures)
    {
        // We already have access to our IStringLocalizer pointing at LanguageResource.
        if (_interceptor is AbstractLocalizationInterceptor abstractLocalizationInterceptor)
        {
            return abstractLocalizationInterceptor.Localizer.GetAllStrings(includeParentCultures);
        }

        return _defaultLocalizationInterceptor.Value.GetAllStrings(includeParentCultures);
    }

    /// <inheritdoc />
    LocalizedString IStringLocalizer.this[string key] => this[key, Array.Empty<object>()];

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="key">The name of the string resource.</param>
    /// <param name="arguments">The list of arguments to be passed to the string resource.</param>
    /// <returns>The string resource as a <see cref="LocalizedString" />.</returns>
    public LocalizedString this[string key, params object[] arguments]
    {
        get
        {
            var result = _interceptor.Handle(key, arguments);

            // This is a hack to support key renaming without breaking consumers
#pragma warning disable CS0618 // Type or member is obsolete
            var legacyKey = GetLegacyKey(key);
#pragma warning restore CS0618 // Type or member is obsolete
            if (legacyKey == key)
            {
                return result;
            }

            // If the resource was not found, try the legacy key
            if (result.ResourceNotFound || result.Value == key || string.IsNullOrWhiteSpace(result.Value))
            {
                var legacyResult = _interceptor.Handle(legacyKey, arguments);

                if (!legacyResult.ResourceNotFound)
                {
                    return legacyResult;
                }
            }

            return result;
        }
    }

    [Obsolete("To be removed in v8")]
    private static string GetLegacyKey(string key) => key switch
    {
        // Remove these mappings and add renamed keys to breaking changes before the next major release
        "MudDataGrid_EqualSign" => "MudDataGrid.=",
        "MudDataGrid_NotEqualSign" => "MudDataGrid.!=",
        "MudDataGrid_GreaterThanSign" => "MudDataGrid.>",
        "MudDataGrid_GreaterThanOrEqualSign" => "MudDataGrid.>=",
        "MudDataGrid_LessThanSign" => "MudDataGrid.<",
        "MudDataGrid_LessThanOrEqualSign" => "MudDataGrid.<=",
        "MudDataGrid_RefreshData" => "MudDataGrid.RefreshData",
        "MudDataGrid_IsNot" => "MudDataGrid.is not",
        "MudDataGrid_IsNotEmpty" => "MudDataGrid.is not empty",
        "MudDataGrid_Sort" => "MudDataGrid.Sort",
        "MudDataGrid_Save" => "MudDataGrid.Save",
        "MudDataGrid_True" => "MudDataGrid.True",
        "MudDataGrid_False" => "MudDataGrid.False",
        "MudDataGrid_Hide" => "MudDataGrid.Hide",
        "MudDataGrid_HideAll" => "MudDataGrid.HideAll",
        "MudDataGrid_StartsWith" => "MudDataGrid.starts with",
        "MudDataGrid_Equals" => "MudDataGrid.equals",
        "MudDataGrid_NotEquals" => "MudDataGrid.not equals",
        "MudDataGrid_Unsort" => "MudDataGrid.Unsort",
        "MudDataGrid_Columns" => "MudDataGrid.Columns",
        "MudDataGrid_Loading" => "MudDataGrid.Loading",
        "MudDataGrid_MoveUp" => "MudDataGrid.MoveUp",
        "MudDataGrid_MoveDown" => "MudDataGrid.MoveDown",
        "MudDataGrid_Is" => "MudDataGrid.is",
        "MudDataGrid_IsBefore" => "MudDataGrid.is before",
        "MudDataGrid_IsOnOrBefore" => "MudDataGrid.is on or before",
        "MudDataGrid_IsOnOrAfter" => "MudDataGrid.is on or after",
        "MudDataGrid_Filter" => "MudDataGrid.Filter",
        "MudDataGrid_Contains" => "MudDataGrid.contains",
        "MudDataGrid_Value" => "MudDataGrid.Value",
        "MudDataGrid_Group" => "MudDataGrid.Group",
        "MudDataGrid_Apply" => "MudDataGrid.Apply",
        "MudDataGrid_Clear" => "MudDataGrid.Clear",
        "MudDataGrid_ShowAll" => "MudDataGrid.ShowAll",
        "MudDataGrid_Column" => "MudDataGrid.Column",
        "MudDataGrid_Cancel" => "MudDataGrid.Cancel",
        "MudDataGrid_FilterValue" => "MudDataGrid.FilterValue",
        "MudDataGrid_Operator" => "MudDataGrid.Operator",
        "MudDataGrid_IsEmpty" => "MudDataGrid.is empty",
        "MudDataGrid_IsAfter" => "MudDataGrid.is after",
        "MudDataGrid_Ungroup" => "MudDataGrid.Ungroup",
        "MudDataGrid_NotContains" => "MudDataGrid.not contains",
        "MudDataGrid_EndsWith" => "MudDataGrid.ends with",
        "MudDataGrid_CollapseAllGroups" => "MudDataGrid.CollapseAllGroups",
        "MudDataGrid_AddFilter" => "MudDataGrid.AddFilter",
        "MudDataGrid_ExpandAllGroups" => "MudDataGrid.ExpandAllGroups",

        _ => key
    };
}
