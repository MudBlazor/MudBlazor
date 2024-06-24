using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MudBlazor
{
#nullable enable
    public static class QuerySortExtensions
    {
        private static readonly MethodInfo OrderByMethod = typeof(Queryable).GetMethods()
            .Where(m => m is { Name: nameof(Queryable.OrderBy), IsGenericMethodDefinition: true })
            .Single(m => m.GetParameters().Length == 2);

        private static readonly MethodInfo OrderByDescendingMethod = typeof(Queryable).GetMethods()
            .Where(m => m is { Name: nameof(Queryable.OrderByDescending), IsGenericMethodDefinition: true })
            .Single(m => m.GetParameters().Length == 2);

        private static readonly MethodInfo ThenByMethod = typeof(Queryable).GetMethods()
            .Where(m => m is { Name: nameof(Queryable.ThenBy), IsGenericMethodDefinition: true })
            .Single(m => m.GetParameters().Length == 2);

        private static readonly MethodInfo ThenByDescendingMethod = typeof(Queryable).GetMethods()
            .Where(m => m is { Name: nameof(Queryable.ThenByDescending), IsGenericMethodDefinition: true })
            .Single(m => m.GetParameters().Length == 2);

        /// <summary>
        /// Sorts the elements of a sequence in the order described by the sort definitions.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="sortDefinitions">A collection of <see cref="SortDefinition{T}"/> that describe how to sort the source.</param>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to the sort definitions or the <paramref name="source"/> itself if the sort definitions are empty.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Suppressing because T is a type supplied by the user and it is unlikely that it is not referenced by their code.")]
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, IEnumerable<SortDefinition<T>> sortDefinitions)
        {
            return OrderBy(source, sortDefinitions, (parameter, sortDefinition) => Expression.Property(parameter, sortDefinition.SortBy));
        }

        /// <summary>
        /// Sorts the elements of a sequence in the order described by the sort definitions.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="sortDefinitions">A collection of <see cref="SortDefinition{T}"/> that describe how to sort the source.</param>
        /// <param name="expression">A function that constructs the body of the lambda expression for the given parameter expression and sort definition.</param>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to the sort definitions or the <paramref name="source"/> itself if the sort definitions are empty.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Suppressing because T is a type supplied by the user and it is unlikely that it is not referenced by their code.")]
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
                    var orderBy = (sortDefinition.Descending ? OrderByDescendingMethod : OrderByMethod).MakeGenericMethod(typeof(T), keySelector.ReturnType);
                    query = (IOrderedQueryable<T>?)orderBy.Invoke(null, [source, keySelector]);
                }
                else
                {
                    var thenBy = (sortDefinition.Descending ? ThenByDescendingMethod : ThenByMethod).MakeGenericMethod(typeof(T), keySelector.ReturnType);
                    query = (IOrderedQueryable<T>?)thenBy.Invoke(null, [query, keySelector]);
                }
            }

            return query ?? source;
        }
    }
}
