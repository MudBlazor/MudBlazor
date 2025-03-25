using System.Linq.Expressions;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Provides extension methods for sorting and for <see cref="MudTable{T}"/>.
/// </summary>
public static class TableExtensions
{
    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order according to a key.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by the function represented by keySelector.</typeparam>
    /// <param name="source">An IEnumerable&lt;T&gt; to order.</param>
    /// <param name="direction">The direction in which to sort the elements (ascending or descending).</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An IOrderedEnumerable&lt;TSource&gt; whose elements are sorted according to a key.</returns>
    public static IOrderedEnumerable<TSource> OrderByDirection<TSource, TKey>(this IEnumerable<TSource> source, SortDirection direction, Func<TSource, TKey> keySelector)
    {
        return direction == SortDirection.Descending
            ? source.OrderByDescending(keySelector)
            : source.OrderBy(keySelector);
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order according to a key.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by the function represented by keySelector.</typeparam>
    /// <param name="source">An IQueryable&lt;T&gt; to order.</param>
    /// <param name="direction">The direction in which to sort the elements (ascending or descending).</param>
    /// <param name="keySelector">An expression to extract a key from an element.</param>
    /// <returns>An IOrderedQueryable&lt;TSource&gt; whose elements are sorted according to a key.</returns>
    public static IOrderedQueryable<TSource> OrderByDirection<TSource, TKey>(this IQueryable<TSource> source, SortDirection direction, Expression<Func<TSource, TKey>> keySelector)
    {
        return direction == SortDirection.Descending
            ? source.OrderByDescending(keySelector)
            : source.OrderBy(keySelector);
    }

    public static bool EditButtonDisabled<T>(this TableContext? context, T item) => (context?.Table?.IsEditRowSwitchingBlocked ?? false) && context.Table._editingItem is not null && !ReferenceEquals(context.Table._editingItem, item);
}
