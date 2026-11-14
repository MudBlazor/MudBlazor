namespace MudBlazor;

/// <summary>
/// Set-based equality comparer for <see cref="IReadOnlyCollection{T}"/> values that ignores element order and duplicates.
/// Two collections are equal when they contain the same distinct elements.
/// Null is only equal to null.
/// <para>Note:</para>
/// <list type="bullet">
///   <item>The order of elements does not affect equality or hash code.</item>
///   <item>Multiple entries of the same value are ignored.</item>
///   <item>Null and empty collections are treated as distinct values.</item>
/// </list>
/// </summary>
/// <typeparam name="T">The type of the elements in the collection.</typeparam>
public class CollectionComparer<T> : IEqualityComparer<IReadOnlyCollection<T>?>
{
    private readonly IEqualityComparer<T> _comparer;

    public CollectionComparer()
        : this(EqualityComparer<T>.Default)
    {
    }

    public CollectionComparer(IEqualityComparer<T> comparer)
    {
        _comparer = comparer;
    }

    /// <inheritdoc/>
    public bool Equals(IReadOnlyCollection<T>? x, IReadOnlyCollection<T>? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        if (x.Count == 0 && y.Count == 0)
        {
            return true;
        }

        var a = new HashSet<T>(x, _comparer);

        return a.SetEquals(y);
    }

    public int GetHashCode(IReadOnlyCollection<T>? obj)
    {
        if (obj is null)
        {
            return 0;
        }
        if (obj.Count == 0)
        {
            // Empty collection seed
            return 0x34502209;
        }

        var hash = 0;
        var seen = new HashSet<T>(_comparer);

        foreach (var item in obj)
        {
            if (seen.Add(item))
            {
                hash ^= item is null ? 0 : _comparer.GetHashCode(item);
            }
        }

        return hash;
    }

    public static readonly CollectionComparer<T> Default = new();
}
