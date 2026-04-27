using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Shared.Search;

public interface IFuzzySearchService
{
    Task<FuzzySearchResponse<T>> SearchAsync<T>(string query, IReadOnlyList<FuzzySearchEntry<T>> entries, FuzzySearchOptions? options = null, CancellationToken cancellationToken = default);
}

public sealed record FuzzySearchOptions
{
    public int Threshold { get; init; } = 35;

    public int? Limit { get; init; }
}

public sealed record FuzzySearchResponse<T>(IReadOnlyList<T> Results, int TotalMatchCount);
