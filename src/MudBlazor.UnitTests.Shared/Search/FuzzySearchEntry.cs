using System.Collections.Generic;
using System.Linq;

namespace MudBlazor.UnitTests.Shared.Search;

public sealed class FuzzySearchEntry<T>
{
    internal IReadOnlyList<IndexedFuzzySearchDataPoint> DataPoints { get; }

    public T Item { get; }

    public string SortKey { get; }

    public FuzzySearchEntry(T item, string sortKey, IEnumerable<FuzzySearchDataPoint> dataPoints)
    {
        Item = item;
        SortKey = sortKey;
        DataPoints = dataPoints
            .Where(static x => !string.IsNullOrWhiteSpace(x.Text))
            .Select(static x => new IndexedFuzzySearchDataPoint(x.Text.Trim().ToLowerInvariant(), x.Weight))
            .ToArray();
    }

    internal sealed record IndexedFuzzySearchDataPoint(string Text, double Weight);
}

public sealed record FuzzySearchDataPoint(string Text, double Weight = 1.0);
