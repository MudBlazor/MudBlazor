// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Analyzers.Internal;

internal static class MethodSymbolExtensions
{
    private static readonly string[] MsTestNamespaceParts = ["Microsoft", "VisualStudio", "TestTools", "UnitTesting"];
    private static readonly string[] NunitNamespaceParts = ["NUnit", "Framework"];
    private static readonly string[] XunitNamespaceParts = ["Xunit"];

    public static bool IsInterfaceImplementation(this IMethodSymbol symbol)
    {
        if (symbol.ExplicitInterfaceImplementations.Length > 0)
            return true;

        return IsInterfaceImplementation((ISymbol)symbol);
    }

    public static bool IsInterfaceImplementation(this IPropertySymbol symbol)
    {
        if (symbol.ExplicitInterfaceImplementations.Length > 0)
            return true;

        return IsInterfaceImplementation((ISymbol)symbol);
    }

    public static bool IsInterfaceImplementation(this IEventSymbol symbol)
    {
        if (symbol.ExplicitInterfaceImplementations.Length > 0)
            return true;

        return IsInterfaceImplementation((ISymbol)symbol);
    }

    private static bool IsInterfaceImplementation(this ISymbol symbol)
    {
        return GetImplementingInterfaceSymbol(symbol) is not null;
    }

    public static IMethodSymbol? GetImplementingInterfaceSymbol(this IMethodSymbol symbol)
    {
        if (symbol.ExplicitInterfaceImplementations.Any())
            return symbol.ExplicitInterfaceImplementations.First();

        return (IMethodSymbol?)GetImplementingInterfaceSymbol((ISymbol)symbol);
    }

    private static ISymbol? GetImplementingInterfaceSymbol(this ISymbol symbol)
    {
        if (symbol.ContainingType is null)
            return null;

        return symbol.ContainingType.AllInterfaces
            .SelectMany(@interface => @interface.GetMembers())
            .FirstOrDefault(interfaceMember => SymbolEqualityComparer.Default.Equals(symbol, symbol.ContainingType.FindImplementationForInterfaceMember(interfaceMember)));
    }

    public static bool IsUnitTestMethod(this IMethodSymbol methodSymbol)
    {
        var attributes = methodSymbol.GetAttributes();
        foreach (var attribute in attributes)
        {
            var type = attribute.AttributeClass;
            while (type is not null)
            {
                var ns = type.ContainingNamespace;
                if (ns.IsNamespace(MsTestNamespaceParts) ||
                    ns.IsNamespace(NunitNamespaceParts) ||
                    ns.IsNamespace(XunitNamespaceParts))
                {
                    return true;
                }

                type = type.BaseType;
            }
        }

        return false;
    }
}
