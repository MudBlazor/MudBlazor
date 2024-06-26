using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Provides extension methods for sorting queryable data sources using sort definitions.
/// </summary>
/// <remarks>
/// This class contains methods that extend <see cref="IQueryable{T}"/> with sorting capabilities based on a collection of
/// <see cref="SortDefinition{T}"/> instances. These extensions allow for dynamic and customizable sorting of data.
/// </remarks>
public static class QuerySortExtensions
{
    private static readonly MethodInfo _orderByMethod = typeof(Queryable).GetMethods()
        .Where(m => m is { Name: nameof(Queryable.OrderBy), IsGenericMethodDefinition: true })
        .Single(m => m.GetParameters().Length == 2);

    private static readonly MethodInfo _orderByDescendingMethod = typeof(Queryable).GetMethods()
        .Where(m => m is { Name: nameof(Queryable.OrderByDescending), IsGenericMethodDefinition: true })
        .Single(m => m.GetParameters().Length == 2);

    private static readonly MethodInfo _thenByMethod = typeof(Queryable).GetMethods()
        .Where(m => m is { Name: nameof(Queryable.ThenBy), IsGenericMethodDefinition: true })
        .Single(m => m.GetParameters().Length == 2);

    private static readonly MethodInfo _thenByDescendingMethod = typeof(Queryable).GetMethods()
        .Where(m => m is { Name: nameof(Queryable.ThenByDescending), IsGenericMethodDefinition: true })
        .Single(m => m.GetParameters().Length == 2);

    /// <summary>
    /// Sorts the elements of a sequence in the order described by the sort definitions.
    /// </summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="sortDefinitions">A collection of <see cref="SortDefinition{T}"/> that describe how to sort the source.</param>
    /// <typeparam name="T">The type of the elements of source</typeparam>
    /// <returns>An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to the sort definitions or the <paramref name="source"/> itself if the sort definitions are empty.</returns>
    public static IQueryable<T> OrderBy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] T>(this IQueryable<T> source, IEnumerable<SortDefinition<T>> sortDefinitions)
    {
        return OrderBy(source, sortDefinitions, (parameter, sortDefinition) => Expression.Property(parameter, typeof(T), sortDefinition.SortBy));
    }

    /// <summary>
    /// Sorts the elements of a sequence in the order described by the sort definitions.
    /// </summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="sortDefinitions">A collection of <see cref="SortDefinition{T}"/> that describe how to sort the source.</param>
    /// <param name="expression">A function that constructs the body of the lambda expression for the given parameter expression and sort definition.</param>
    /// <typeparam name="T">The type of the elements of source</typeparam>
    /// <returns>An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to the sort definitions or the <paramref name="source"/> itself if the sort definitions are empty.</returns>
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, IEnumerable<SortDefinition<T>> sortDefinitions, Func<ParameterExpression, SortDefinition<T>, Expression> expression)
    {
        IOrderedQueryable<T>? query = null;

        foreach (var sortDefinition in sortDefinitions)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var body = expression(parameter, sortDefinition);
            var keySelector = Expression.Lambda(body, parameter);
            if (query is null)
            {
                var orderBy = (sortDefinition.Descending ? _orderByDescendingMethod : _orderByMethod).MakeGenericMethod(typeof(T), keySelector.ReturnType);
                query = (IOrderedQueryable<T>?)orderBy.Invoke(null, [source, keySelector]);
            }
            else
            {
                var thenBy = (sortDefinition.Descending ? _thenByDescendingMethod : _thenByMethod).MakeGenericMethod(typeof(T), keySelector.ReturnType);
                query = (IOrderedQueryable<T>?)thenBy.Invoke(null, [query, keySelector]);
            }
        }

        return query ?? source;
    }
}
