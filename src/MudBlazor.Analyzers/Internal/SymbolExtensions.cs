namespace MudBlazor.Analyzers.Internal;

internal static class SymbolExtensions
{
    public static bool IsEqualTo(this ISymbol? symbol, ISymbol? expectedType)
    {
        if (symbol is null || expectedType is null)
            return false;

        return SymbolEqualityComparer.Default.Equals(expectedType, symbol);
    }

    public static bool IsEqualTo(this ISymbol? symbol, ISymbol? expectedType, IEqualityComparer<ISymbol?> comparer)
    {
        if (symbol is null || expectedType is null)
            return false;

        return comparer.Equals(expectedType, symbol);
    }
}
