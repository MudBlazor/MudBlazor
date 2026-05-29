// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

namespace MudBlazor.Analyzers;

public sealed partial class ParameterStateAnalyzer
{
    /// <summary>
    /// MUD0010: Reading a parameter state property is not allowed.
    /// </summary>
    public const string ReadDiagnosticId = "MUD0010";

    /// <summary>
    /// MUD0011: Writing to a parameter state property is not allowed.
    /// </summary>
    public const string WriteDiagnosticId = "MUD0011";

    /// <summary>
    /// MUD0012: External access of a parameter state property is not allowed.
    /// </summary>
    public const string ExternalAccessDiagnosticId = "MUD0012";

    private const string Category = "Usage";

    private static readonly LocalizableString HelpLinkUrl = new LocalizableResourceString(nameof(Resources.HelpLinkUrl), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString ReadTitle = new LocalizableResourceString(nameof(Resources.MUD0010Title), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString ReadMessageFormat = new LocalizableResourceString(nameof(Resources.MUD0010MessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString ReadDescription = new LocalizableResourceString(nameof(Resources.MUD0010Description), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString WriteTitle = new LocalizableResourceString(nameof(Resources.MUD0011Title), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString WriteMessageFormat = new LocalizableResourceString(nameof(Resources.MUD0011MessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString WriteDescription = new LocalizableResourceString(nameof(Resources.MUD0011Description), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString ExternalAccessTitle = new LocalizableResourceString(nameof(Resources.MUD0012Title), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString ExternalAccessMessageFormat = new LocalizableResourceString(nameof(Resources.MUD0012MessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString ExternalAccessDescription = new LocalizableResourceString(nameof(Resources.MUD0012Description), Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// Diagnostic descriptor for MUD0010: Reading a parameter state property is not allowed.
    /// </summary>
    public static readonly DiagnosticDescriptor ReadDescriptor = new(
        id: ReadDiagnosticId,
        title: ReadTitle,
        messageFormat: ReadMessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: ReadDescription,
        helpLinkUri: HelpLinkUrl.ToString());

    /// <summary>
    /// Diagnostic descriptor for MUD0011: Writing to a parameter state property is not allowed.
    /// </summary>
    public static readonly DiagnosticDescriptor WriteDescriptor = new(
        id: WriteDiagnosticId,
        title: WriteTitle,
        messageFormat: WriteMessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: WriteDescription,
        helpLinkUri: HelpLinkUrl.ToString());

    /// <summary>
    /// Diagnostic descriptor for MUD0012: External access of a parameter state property is not allowed.
    /// </summary>
    public static readonly DiagnosticDescriptor ExternalAccessDescriptor = new(
        id: ExternalAccessDiagnosticId,
        title: ExternalAccessTitle,
        messageFormat: ExternalAccessMessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: ExternalAccessDescription,
        helpLinkUri: HelpLinkUrl.ToString());

    private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue = ImmutableArray.Create(
        ReadDescriptor,
        WriteDescriptor,
        ExternalAccessDescriptor);
}
