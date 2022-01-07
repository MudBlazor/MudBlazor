using System.Collections.Generic;

public static class LinqExtensions
{
    public static IEnumerable<IEnumerable<T>> GetChunksOf<T>(this IEnumerable<T> source, int count)
    {
        var chunk = new List<T>();
        foreach (var item in source)
        {
            chunk.Add(item);
            if (chunk.Count == count)
            {
                yield return chunk;
                chunk = new List<T>();
            }
        }
        if (chunk.Count != 0)
        {
            yield return chunk;
        }
    }
}