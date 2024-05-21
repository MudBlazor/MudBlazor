namespace MudBlazor.Analyzers.Internal
{
    internal class MetadataSymbolComparer : IEqualityComparer<ISymbol?>
    {
        public bool Equals(ISymbol? x, ISymbol? y)
        {
            if (x is null || y is null)
                return false;

            if (x.MetadataName.Equals(y.MetadataName))
                return true;

            return false;
        }

        public int GetHashCode(ISymbol? obj)
        {
            return base.GetHashCode();
        }
    }
}
