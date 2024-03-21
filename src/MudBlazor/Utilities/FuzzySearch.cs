using System;

namespace MudBlazor.Utilities
{
    public static class FuzzySearch
    {
        // Modified from https://www.c-sharpcorner.com/article/fuzzy-search-in-c-sharp.
        public static int LevenshteinDistance(ReadOnlySpan<char> source1, ReadOnlySpan<char> source2)
        {
            if (source1.Length == 0)
            {
                return source2.Length;
            }

            if (source2.Length == 0)
            {
                return source1.Length;
            }

            if (source1.Length > source2.Length)
            {
                var tmp = source1;
                source1 = source2;
                source2 = tmp;
            }

            Span<int> distances = stackalloc int[source1.Length + 1];

            for (var i = 0; i <= source1.Length; i++)
            {
                distances[i] = i;
            }

            for (var j = 1; j <= source2.Length; j++)
            {
                var previousDiagonal = distances[0];
                distances[0] = j;

                for (var i = 1; i <= source1.Length; i++)
                {
                    var current = distances[i];
                    distances[i] = Math.Min(
                        Math.Min(distances[i - 1] + 1, distances[i] + 1),
                        previousDiagonal + (source1[i - 1] == source2[j - 1] ? 0 : 1)
                    );
                    previousDiagonal = current;
                }
            }

            return distances[source1.Length];
        }
    }
}
