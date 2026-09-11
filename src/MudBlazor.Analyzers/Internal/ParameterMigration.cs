// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

namespace MudBlazor.Analyzers.Internal;

/// <summary>
/// A component parameter that an earlier MudBlazor major version removed, together with the guidance <c>MUD0002</c> appends when the old name still appears on the component that used to declare it.
/// </summary>
/// <remarks>
/// This is a lookup table, not a migration tool.
/// The analyzer never reads, evaluates, or rewrites the attribute's value; it only explains what the replacement parameter is so the reader does not have to search the migration guides.
/// </remarks>
internal sealed class ParameterMigration
{
    private ParameterMigration(string componentMetadataName, string parameterName, LocalizableString hint)
    {
        ComponentMetadataName = componentMetadataName;
        ParameterName = parameterName;
        Hint = hint;
    }

    /// <summary>
    /// The fully qualified metadata name of the component that declared the removed parameter.
    /// </summary>
    /// <remarks>
    /// Resolved through the compilation so the hint follows type identity rather than the display tag name, which several unrelated components share.
    /// </remarks>
    internal string ComponentMetadataName { get; }

    /// <summary>
    /// The removed parameter's name, matched case-insensitively like the rest of MUD0002.
    /// </summary>
    internal string ParameterName { get; }

    /// <summary>
    /// The replacement guidance appended to the diagnostic message.
    /// </summary>
    internal LocalizableString Hint { get; }

    /// <summary>
    /// Every removed parameter MUD0002 can explain.
    /// </summary>
    /// <remarks>
    /// Each entry is verified against the release that removed the parameter and against the guide it links to, so keep the list short and add an entry only when both still hold.
    /// </remarks>
    internal static ImmutableArray<ParameterMigration> All { get; } = ImmutableArray.Create(
        Create("MudBlazor.MudButton", "Link", nameof(Resources.MUD0002MigrationHintMudButtonLink)),
        Create("MudBlazor.MudButton", "DisableRipple", nameof(Resources.MUD0002MigrationHintMudButtonDisableRipple)),
        Create("MudBlazor.MudTextField`1", "AutoGrow", nameof(Resources.MUD0002MigrationHintMudTextFieldAutoGrow)));

    private static ParameterMigration Create(string componentMetadataName, string parameterName, string resourceName) =>
        new(componentMetadataName, parameterName, new LocalizableResourceString(resourceName, Resources.ResourceManager, typeof(Resources)));
}
