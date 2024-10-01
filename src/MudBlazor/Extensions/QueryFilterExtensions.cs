using System.Collections.Generic;
using System.Linq;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Provides extension methods for filtering queryable data sources using filter definitions.
/// </summary>
/// <remarks>
/// This class contains methods that extend <see cref="IQueryable{T}"/> with filtering capabilities based on a collection of
/// <see cref="IFilterDefinition{T}"/> instances. These extensions allow for dynamic and customizable filtering of data.
/// </remarks>
public static class QueryFilterExtensions
{
    /// <summary>
    /// Filters a sequence of values based on a collection of filter definitions.
    /// </summary>
    /// <param name="source">An <see cref="IQueryable{T}"/> to filter.</param>
    /// <param name="filterDefinitions">A collection of <see cref="IFilterDefinition{T}"/> to apply.</param>
    /// <param name="caseSensitivity">The filtering case sensitivity. Defaults to <see cref="DataGridFilterCaseSensitivity.Ignore"/> for best compatibility with database providers.</param>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <returns>An <see cref="IQueryable{T}"/> that contains elements from the input sequence that satisfy the filter definitions.</returns>
    public static IQueryable<T> Where<T>(
        this IQueryable<T> source,
        IEnumerable<IFilterDefinition<T>> filterDefinitions,
        DataGridFilterCaseSensitivity caseSensitivity = DataGridFilterCaseSensitivity.Ignore)
    {
        var filterOptions = new FilterOptions { FilterCaseSensitivity = caseSensitivity };
        return filterDefinitions.Aggregate(source, (current, filter) => current.Where(FilterExpressionGenerator.GenerateExpression(filter, filterOptions)));
    }
}
