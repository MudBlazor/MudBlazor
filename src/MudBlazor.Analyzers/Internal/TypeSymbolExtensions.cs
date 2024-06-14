// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Analyzers.Internal;

// http://source.roslyn.io/#Microsoft.CodeAnalysis.Workspaces/Shared/Extensions/ITypeSymbolExtensions.cs,190b4ed0932458fd,references
internal static class TypeSymbolExtensions
{
    public static IList<INamedTypeSymbol> GetAllInterfacesIncludingThis(this ITypeSymbol type)
    {
        var allInterfaces = type.AllInterfaces;
        if (type is INamedTypeSymbol namedType && namedType.TypeKind == TypeKind.Interface && !allInterfaces.Contains(namedType))
        {
            var result = new List<INamedTypeSymbol>(allInterfaces.Length + 1);
            result.AddRange(allInterfaces);
            result.Add(namedType);
            return result;
        }

        return allInterfaces;
    }

    public static bool InheritsFrom(this ITypeSymbol classSymbol, ITypeSymbol? baseClassType)
    {
        if (baseClassType is null)
            return false;

        var baseType = classSymbol.BaseType;
        while (baseType is not null)
        {
            if (baseClassType.IsEqualTo(baseType))
                return true;

            baseType = baseType.BaseType;
        }

        return false;
    }

    public static bool InheritsFrom(this ITypeSymbol classSymbol, ITypeSymbol? baseClassType, IEqualityComparer<ISymbol?> comparer)
    {
        if (baseClassType is null)
            return false;

        var baseType = classSymbol.BaseType;
        while (baseType is not null)
        {
            if (baseClassType.IsEqualTo(baseType, comparer))
                return true;

            baseType = baseType.BaseType;
        }

        return false;
    }

    public static bool Implements(this ITypeSymbol classSymbol, ITypeSymbol? interfaceType)
    {
        if (interfaceType is null)
            return false;

        return classSymbol.AllInterfaces.Any(interfaceType.IsEqualTo);
    }

    public static bool IsOrImplements(this ITypeSymbol symbol, ITypeSymbol? interfaceType)
    {
        if (interfaceType is null)
            return false;

        return GetAllInterfacesIncludingThis(symbol).Any(interfaceType.IsEqualTo);
    }

    public static AttributeData? GetAttribute(this ISymbol symbol, ITypeSymbol? attributeType, bool inherits = true)
    {
        if (attributeType is null)
            return null;

        if (attributeType.IsSealed)
        {
            inherits = false;
        }

        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is null)
                continue;

            if (inherits)
            {
                if (attribute.AttributeClass.IsOrInheritFrom(attributeType))
                    return attribute;
            }
            else
            {
                if (attributeType.IsEqualTo(attribute.AttributeClass))
                    return attribute;

            }
        }

        return null;
    }

    public static bool HasAttribute(this ISymbol symbol, ITypeSymbol? attributeType, bool inherits = true)
    {
        return GetAttribute(symbol, attributeType, inherits) is not null;
    }

    public static bool IsOrInheritFrom(this ITypeSymbol symbol, ITypeSymbol? expectedType, IEqualityComparer<ISymbol?> comparer)
    {
        if (expectedType is null)
            return false;

        return symbol.IsEqualTo(expectedType, comparer) || symbol.InheritsFrom(expectedType, comparer);
    }

    public static bool IsOrInheritFrom(this ITypeSymbol symbol, ITypeSymbol? expectedType)
    {
        if (expectedType is null)
            return false;

        return symbol.IsEqualTo(expectedType) || symbol.InheritsFrom(expectedType);
    }

    public static bool IsEqualToAny(this ITypeSymbol? symbol, params ITypeSymbol?[]? expectedTypes)
    {
        if (symbol is null || expectedTypes is null)
            return false;

        return expectedTypes.Any(t => t.IsEqualTo(symbol));
    }

    public static bool IsEqualToAny(this ITypeSymbol? symbol, ITypeSymbol? expectedType1)
    {
        if (symbol is null)
            return false;

        if (expectedType1 is not null && symbol.IsEqualTo(expectedType1))
            return true;

        return false;
    }

    public static bool IsEqualToAny(this ITypeSymbol? symbol, ITypeSymbol? expectedType1, ITypeSymbol? expectedType2)
    {
        if (symbol is null)
            return false;

        if (expectedType1 is not null && symbol.IsEqualTo(expectedType1))
            return true;

        if (expectedType2 is not null && symbol.IsEqualTo(expectedType2))
            return true;

        return false;
    }

    public static bool IsEqualToAny(this ITypeSymbol? symbol, ITypeSymbol? expectedType1, ITypeSymbol? expectedType2, ITypeSymbol? expectedType3)
    {
        if (symbol is null)
            return false;

        if (expectedType1 is not null && symbol.IsEqualTo(expectedType1))
            return true;

        if (expectedType2 is not null && symbol.IsEqualTo(expectedType2))
            return true;

        if (expectedType3 is not null && symbol.IsEqualTo(expectedType3))
            return true;

        return false;
    }

    public static bool IsObject(this ITypeSymbol? symbol)
    {
        if (symbol is null)
            return false;

        return symbol.SpecialType == SpecialType.System_Object;
    }

    public static bool IsString(this ITypeSymbol? symbol)
    {
        if (symbol is null)
            return false;

        return symbol.SpecialType == SpecialType.System_String;
    }

    public static bool IsChar(this ITypeSymbol? symbol)
    {
        if (symbol is null)
            return false;

        return symbol.SpecialType == SpecialType.System_Char;
    }

    public static bool IsInt32(this ITypeSymbol? symbol)
    {
        if (symbol is null)
            return false;

        return symbol.SpecialType == SpecialType.System_Int32;
    }

    public static bool IsBoolean(this ITypeSymbol? symbol)
    {
        if (symbol is null)
            return false;

        return symbol.SpecialType == SpecialType.System_Boolean;
    }

    public static bool IsDateTime(this ITypeSymbol? symbol)
    {
        if (symbol is null)
            return false;

        return symbol.SpecialType == SpecialType.System_DateTime;
    }

    public static bool IsEnumeration([NotNullWhen(returnValue: true)] this ITypeSymbol? symbol)
    {
        return symbol is not null && GetEnumerationType(symbol) is not null;
    }

    public static INamedTypeSymbol? GetEnumerationType(this ITypeSymbol? symbol)
    {
        return (symbol as INamedTypeSymbol)?.EnumUnderlyingType;
    }

    public static bool IsNumberType(this ITypeSymbol? symbol)
    {
        if (symbol is null)
            return false;

#pragma warning disable IDE0066 // Convert switch statement to expression
        switch (symbol.SpecialType)
        {
            case SpecialType.System_Int16:
            case SpecialType.System_Int32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt16:
            case SpecialType.System_UInt32:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Decimal:
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
                return true;

            default:
                return false;
        }
#pragma warning restore IDE0066 // Convert switch statement to expression
    }

    [return: NotNullIfNotNull(nameof(typeSymbol))]
    public static ITypeSymbol? GetUnderlyingNullableTypeOrSelf(this ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T && namedTypeSymbol.TypeArguments.Length == 1)
            {
                return namedTypeSymbol.TypeArguments[0];
            }
        }

        return typeSymbol;
    }
}
