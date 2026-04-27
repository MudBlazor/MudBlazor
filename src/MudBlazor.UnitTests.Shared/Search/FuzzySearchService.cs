using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FuzzySharp;

namespace MudBlazor.UnitTests.Shared.Search;

public sealed class FuzzySearchService : IFuzzySearchService
{
    private const int YieldFrequency = 32;

    public async Task<FuzzySearchResponse<T>> SearchAsync<T>(string query, IReadOnlyList<FuzzySearchEntry<T>> entries, FuzzySearchOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || entries.Count == 0)
        {
            return new FuzzySearchResponse<T>([], 0);
        }

        var normalizedQuery = query.Trim().ToLowerInvariant();
        var threshold = options?.Threshold ?? 35;
        var limit = options?.Limit;

        List<(FuzzySearchEntry<T> Entry, int Score)>? results = limit is null ? new(entries.Count) : null;
        List<(FuzzySearchEntry<T> Entry, int Score)>? topResults = limit is null ? null : new(Math.Min(limit.Value, entries.Count));
        var totalMatchCount = 0;

        for (var i = 0; i < entries.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entry = entries[i];
            var score = GetSearchScore(normalizedQuery, entry);
            if (score >= threshold)
            {
                totalMatchCount++;

                if (results is not null)
                {
                    results.Add((entry, score));
                }
                else
                {
                    InsertTopK(topResults!, (entry, score), limit!.Value);
                }
            }

            if ((i + 1) % YieldFrequency == 0)
            {
                await Task.Yield();
            }
        }

        var orderedResults = (results ?? topResults ?? [])
            .OrderByDescending(static x => x.Score)
            .ThenBy(static x => x.Entry.SortKey, StringComparer.OrdinalIgnoreCase)
            .Select(static x => x.Entry.Item)
            .ToArray();

        return new FuzzySearchResponse<T>(orderedResults, totalMatchCount);
    }

    private static int GetSearchScore<T>(string query, FuzzySearchEntry<T> entry)
    {
        var bestScore = 0;

        foreach (var dataPoint in entry.DataPoints)
        {
            var fieldScore = GetFieldScore(query, dataPoint.Text);
            if (fieldScore == 0)
            {
                continue;
            }

            var weightedScore = (int)Math.Round(fieldScore * dataPoint.Weight);
            if (weightedScore > bestScore)
            {
                bestScore = weightedScore;
            }
        }

        return bestScore;
    }

    private static int GetFieldScore(string query, string candidate)
    {
        if (candidate.Equals(query, StringComparison.Ordinal))
        {
            return 100;
        }

        if (candidate.StartsWith(query, StringComparison.Ordinal))
        {
            return 96;
        }

        if (candidate.Contains(query, StringComparison.Ordinal))
        {
            return 90;
        }

        var ratio = Fuzz.Ratio(candidate, query);
        var partialRatio = Fuzz.PartialRatio(candidate, query);
        var tokenScore = Fuzz.PartialTokenSortRatio(candidate, query);

        return (int)Math.Round(ratio * 0.50 + partialRatio * 0.35 + tokenScore * 0.15);
    }

    private static void InsertTopK<T>(List<(FuzzySearchEntry<T> Entry, int Score)> list, (FuzzySearchEntry<T> Entry, int Score) candidate, int limit)
    {
        if (limit <= 0)
        {
            return;
        }

        if (list.Count == 0)
        {
            list.Add(candidate);
            return;
        }

        var inserted = false;
        for (var i = 0; i < list.Count; i++)
        {
            var scoreComparison = candidate.Score.CompareTo(list[i].Score);
            if (scoreComparison > 0
                || (scoreComparison == 0
                    && StringComparer.OrdinalIgnoreCase.Compare(candidate.Entry.SortKey, list[i].Entry.SortKey) < 0))
            {
                list.Insert(i, candidate);
                inserted = true;
                break;
            }
        }

        if (!inserted && list.Count < limit)
        {
            list.Add(candidate);
        }

        if (list.Count > limit)
        {
            list.RemoveAt(list.Count - 1);
        }
    }
}
