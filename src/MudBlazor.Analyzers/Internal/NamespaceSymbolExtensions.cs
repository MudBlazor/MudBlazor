// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Analyzers.Internal;

internal static class NamespaceSymbolExtensions
{
    public static bool IsNamespace(this INamespaceSymbol namespaceSymbol, string[] namespaceParts)
    {
        for (var i = namespaceParts.Length - 1; i >= 0; i--)
        {
            if (namespaceSymbol is null || namespaceSymbol.IsGlobalNamespace)
                return false;

            if (!string.Equals(namespaceParts[i], namespaceSymbol.Name, System.StringComparison.Ordinal))
                return false;

            namespaceSymbol = namespaceSymbol.ContainingNamespace;
        }

        return namespaceSymbol is null || namespaceSymbol.IsGlobalNamespace;
    }
}
