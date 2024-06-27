// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.SourceGenerator;

internal static class DiagnosticHelper
{
    public static Diagnostic CreateDescriptionWarning(string enumName)
    {
        var diagnosticDescriptor = new DiagnosticDescriptor(
            id: "MUD0101",
            title: "Inconsistent usage of DescriptionAttribute",
            messageFormat: $"Enum {enumName} has inconsistent usage of DescriptionAttribute",
            category: "MudBlazor.SourceGenerator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
        return Diagnostic.Create(diagnosticDescriptor, Location.None);
    }
}
