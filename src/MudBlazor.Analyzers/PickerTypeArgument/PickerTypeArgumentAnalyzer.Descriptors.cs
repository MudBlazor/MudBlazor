// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

namespace MudBlazor.Analyzers;

public sealed partial class PickerTypeArgumentAnalyzer
{
    /// <summary>
    /// MUD0003: Unsupported T on MudDatePicker, MudDateRangePicker, or DateRange.
    /// </summary>
    public const string DiagnosticId = "MUD0003";

    private const string Category = "Usage";

    private static readonly LocalizableString HelpLinkUrl = new LocalizableResourceString(nameof(Resources.HelpLinkUrl), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.MUD0003Title), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.MUD0003MessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.MUD0003Description), Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// Diagnostic descriptor for MUD0003: Unsupported T on date picker components.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: HelpLinkUrl.ToString());

    private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue = ImmutableArray.Create(Descriptor);
}
