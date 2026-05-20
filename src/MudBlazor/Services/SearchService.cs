// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor;

#nullable enable

/// <inheritdoc cref="ISearchService"/>
internal sealed class SearchService : ISearchService
{
    // Characters that delimit word tokens inside a search keyword.
    private static ReadOnlySpan<char> TokenSeparators => " -.,_/<>()";

    // Stack-allocation budget for Levenshtein rows and Range arrays.
    private const int StackAllocLimit = 64;

    // Maximum token count stored on the stack per query/target string.
    // Tokens beyond this count are silently ignored, which is fine for
    // typical component-search inputs that rarely exceed a handful of words.
    private const int MaxQueryTokens = 16;
    private const int MaxTargetTokens = 64;

    /// <summary>
    /// The minimum score (0–100) for a result to be considered relevant.
    /// </summary>
    internal const int MinScore = 65;

    /// <inheritdoc />
    public IReadOnlyList<T> Search<T>(IEnumerable<T> items, Func<T, IEnumerable<string>> getKeywords, string query) where T : notnull
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var q = query.ToLowerInvariant();
        var scores = new Dictionary<T, int>();

        foreach (var item in items)
        {
            foreach (var keyword in getKeywords(item))
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    continue;

                var score = ComputeScore(keyword.ToLowerInvariant().AsSpan(), q.AsSpan());
                if (score < MinScore)
                    continue;

                if (!scores.TryGetValue(item, out var best) || score > best)
                    scores[item] = score;
            }
        }

        return [.. scores.OrderByDescending(x => x.Value).Select(x => x.Key)];
    }

    /// <summary>
    /// Returns a score from 0–100 for how closely <paramref name="target"/> matches <paramref name="query"/>.
    /// Both spans must already be lower-cased.
    /// </summary>
    internal static int ComputeScore(ReadOnlySpan<char> target, ReadOnlySpan<char> query)
    {
        if (target.IsEmpty || query.IsEmpty)
            return 0;

        if (target.SequenceEqual(query))
            return 100;

        var best = 0;

        // Stage 1 – prefix: the full target starts with the query string.
        if (target.StartsWith(query))
            best = Math.Max(best, PrefixScore(target.Length, query.Length));

        // Stage 2 – substring: query appears verbatim anywhere inside target.
        if (target.IndexOf(query) >= 0)
            best = Math.Max(best, 70);

        // Stage 3 – token set: split both sides into words and match each query
        // token to the nearest target token (any order, prefix + typo tolerance).
        Span<Range> qRanges = stackalloc Range[MaxQueryTokens];
        Span<Range> tRanges = stackalloc Range[MaxTargetTokens];
        var qCount = query.SplitAny(qRanges, TokenSeparators, StringSplitOptions.RemoveEmptyEntries);
        var tCount = target.SplitAny(tRanges, TokenSeparators, StringSplitOptions.RemoveEmptyEntries);

        if (qCount > 0 && tCount > 0)
        {
            var ts = TokenSetScore(
                query, qRanges[..qCount],
                target, tRanges[..tCount]);
            best = Math.Max(best, ts);
        }

        // Stage 4 – concat: join the query tokens without spaces and try matching
        // as one string (e.g. "auto complte" → "autocomplete").
        if (qCount > 1)
        {
            Span<char> concat = stackalloc char[StackAllocLimit];
            var concatLen = BuildConcat(query, qRanges[..qCount], concat);
            if (concatLen > 0)
            {
                var cs = SinglePairScore(concat[..concatLen], target);
                best = Math.Max(best, cs);
            }
        }

        return best;
    }

    // Score for a target that begins with the full query; longer surplus = lower score.
    private static int PrefixScore(int targetLen, int queryLen) =>
        Math.Max(65, 100 - ((targetLen - queryLen) * 3));

    // Match every query token to its best target token (any order).
    // A mild coverage factor rewards queries that are more specific relative to the target.
    private static int TokenSetScore(
        ReadOnlySpan<char> queryStr, ReadOnlySpan<Range> qRanges,
        ReadOnlySpan<char> targetStr, ReadOnlySpan<Range> tRanges)
    {
        var totalScore = 0;
        var totalLen = 0;

        foreach (var qr in qRanges)
        {
            var qt = queryStr[qr];
            if (qt.IsEmpty)
                continue;

            var best = 0;
            foreach (var tr in tRanges)
            {
                var tt = targetStr[tr];
                if (tt.IsEmpty)
                    continue;

                var s = TokenPairScore(qt, tt);
                if (s > best)
                    best = s;
            }

            if (best == 0)
                return 0; // This query token has no reasonable match anywhere.

            totalScore += best * qt.Length;
            totalLen += qt.Length;
        }

        if (totalLen == 0)
            return 0;

        var avg = totalScore / totalLen;

        // Mild coverage factor: prefer a target with the same number of tokens as
        // the query (e.g. "date picker" over "date range picker" for "date pikr").
        var coverage = Math.Min(1.0, (double)qRanges.Length / tRanges.Length);
        return (int)(avg * (0.85 + (0.15 * coverage)));
    }

    // Score for one query-token vs one target-token.
    private static int TokenPairScore(ReadOnlySpan<char> qt, ReadOnlySpan<char> tt)
    {
        if (tt.SequenceEqual(qt))
            return 100;

        // The target token starts with the query token (user typed a partial word).
        if (tt.StartsWith(qt))
            return Math.Max(50, qt.Length * 100 / tt.Length);

        // Typo tolerance: allow a limited number of edits proportional to length.
        var maxLen = Math.Max(qt.Length, tt.Length);
        var maxDist = maxLen <= 3 ? 0 : maxLen <= 5 ? 1 : maxLen <= 8 ? 2 : 3;

        if (maxDist > 0)
        {
            var dist = EditDistance(qt, tt);
            if (dist <= maxDist)
                return (maxLen - dist) * 100 / maxLen;
        }

        return 0;
    }

    // Score for a single pair of strings (used for the concat stage).
    // Only checks target.StartsWith(queryConcat), not the reverse, to avoid
    // short target words falsely matching a long concatenated query.
    private static int SinglePairScore(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
    {
        if (a.SequenceEqual(b))
            return 100;

        if (b.StartsWith(a))
            return PrefixScore(b.Length, a.Length);

        var maxLen = Math.Max(a.Length, b.Length);
        var maxDist = maxLen <= 5 ? 1 : maxLen <= 8 ? 2 : 3;
        var dist = EditDistance(a, b);
        if (dist <= maxDist)
            return (maxLen - dist) * 100 / maxLen;

        return 0;
    }

    // Concatenate the tokens of <paramref name="source"/> into <paramref name="dest"/>.
    // Returns the number of characters written, or 0 if <paramref name="dest"/> is too small.
    private static int BuildConcat(ReadOnlySpan<char> source, ReadOnlySpan<Range> ranges, Span<char> dest)
    {
        var pos = 0;
        foreach (var r in ranges)
        {
            var tok = source[r];
            if (pos + tok.Length > dest.Length)
                return 0;
            tok.CopyTo(dest[pos..]);
            pos += tok.Length;
        }
        return pos;
    }

    // Two-row Levenshtein with Damerau transposition; uses stackalloc for small inputs.
    private static int EditDistance(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
    {
        if (a.IsEmpty) return b.Length;
        if (b.IsEmpty) return a.Length;

        var n = b.Length + 1;
        Span<int> prev = n <= StackAllocLimit ? stackalloc int[n] : new int[n];
        Span<int> curr = n <= StackAllocLimit ? stackalloc int[n] : new int[n];

        for (var j = 0; j < n; j++)
            prev[j] = j;

        for (var i = 0; i < a.Length; i++)
        {
            curr[0] = i + 1;
            for (var j = 0; j < b.Length; j++)
            {
                var cost = a[i] == b[j] ? 0 : 1;
                curr[j + 1] = Math.Min(
                    Math.Min(curr[j] + 1, prev[j + 1] + 1),
                    prev[j] + cost);

                // Damerau: count adjacent transpositions as a single edit.
                if (i > 0 && j > 0 && a[i] == b[j - 1] && a[i - 1] == b[j])
                    curr[j + 1] = Math.Min(curr[j + 1], prev[j - 1] + 1);
            }

            // Swap rows without allocation.
            var tmp = prev;
            prev = curr;
            curr = tmp;
        }

        return prev[b.Length];
    }
}
