// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Analyzers.Internal;

internal static class SymbolExtensions
{
    public static bool IsEqualTo(this ISymbol? symbol, ISymbol? expectedType)
    {
        if (symbol is null || expectedType is null)
            return false;

        return SymbolEqualityComparer.Default.Equals(expectedType, symbol);
    }

    public static bool IsVisibleOutsideOfAssembly([NotNullWhen(true)] this ISymbol? symbol)
    {
        if (symbol is null)
            return false;

        if (symbol.DeclaredAccessibility != Accessibility.Public &&
            symbol.DeclaredAccessibility != Accessibility.Protected &&
            symbol.DeclaredAccessibility != Accessibility.ProtectedOrInternal)
        {
            return false;
        }

        if (symbol.ContainingType is null)
            return true;

        return IsVisibleOutsideOfAssembly(symbol.ContainingType);
    }

    public static bool IsOperator(this ISymbol? symbol)
    {
        if (symbol is IMethodSymbol methodSymbol)
        {
            return methodSymbol.MethodKind == MethodKind.UserDefinedOperator || methodSymbol.MethodKind == MethodKind.Conversion;
        }

        return false;
    }

    public static bool IsOverrideOrInterfaceImplementation(this ISymbol? symbol)
    {
        if (symbol is IMethodSymbol methodSymbol)
            return methodSymbol.IsOverride || methodSymbol.IsInterfaceImplementation();

        if (symbol is IPropertySymbol propertySymbol)
            return propertySymbol.IsOverride || propertySymbol.IsInterfaceImplementation();

        if (symbol is IEventSymbol eventSymbol)
            return eventSymbol.IsOverride || eventSymbol.IsInterfaceImplementation();

        return false;
    }

    public static bool Override(this IMethodSymbol? symbol, ISymbol? baseSymbol)
    {
        if (baseSymbol is null)
            return false;

        var currentMethod = symbol?.OverriddenMethod;
        while (currentMethod is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(baseSymbol, currentMethod))
                return true;

            currentMethod = currentMethod.OverriddenMethod;
        }

        return false;
    }

    public static bool IsConst(this ISymbol? symbol)
    {
        return symbol is IFieldSymbol field && field.IsConst;
    }

    public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol? symbol)
    {
        while (symbol is not null)
        {
            foreach (var member in symbol.GetMembers())
                yield return member;

            symbol = symbol.BaseType;
        }
    }

    public static bool IsTopLevelStatement(this ISymbol symbol, CancellationToken cancellationToken)
    {
        if (symbol.DeclaringSyntaxReferences.Length == 0)
            return false;

        foreach (var syntaxReference in symbol.DeclaringSyntaxReferences)
        {
            var syntax = syntaxReference.GetSyntax(cancellationToken);
            if (!syntax.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.CompilationUnit))
                return false;
        }

        return true;
    }

    public static bool IsTopLevelStatementsEntryPointMethod(/*[NotNullWhen(true)]*/ this IMethodSymbol? methodSymbol)
    {
        return methodSymbol?.IsStatic == true && methodSymbol.Name switch
        {
            "$Main" => true,
            "<Main>$" => true,
            _ => false
        };
    }

    public static bool IsTopLevelStatementsEntryPointType(/*[NotNullWhen(true)]*/ this INamedTypeSymbol? typeSymbol)
    {
        if (typeSymbol is null)
            return false;

        foreach (var member in typeSymbol.GetMembers())
        {
            if (member.Kind == SymbolKind.Method)
            {
                var method = (IMethodSymbol)member;
                if (method.IsTopLevelStatementsEntryPointMethod())
                    return true;
            }
        }

        return false;
    }

    public static ITypeSymbol? GetSymbolType(this ISymbol symbol)
    {
        return symbol switch
        {
            IParameterSymbol parameter => parameter.Type,
            IFieldSymbol field => field.Type,
            IPropertySymbol { GetMethod: not null } property => property.Type,
            ILocalSymbol local => local.Type,
            _ => null,
        };
    }
}
