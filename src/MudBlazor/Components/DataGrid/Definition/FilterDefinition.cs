// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the logic of a filter applied to <see cref="MudGrid"/> data.
    /// </summary>
    /// <typeparam name="T">The type of object being filtered.</typeparam>
    public class FilterDefinition<T> : IFilterDefinition<T>
    {
        private int _cachedExpressionHashCode;
        private Func<T, bool>? _cachedFilterFunction;

        /// <inheritdoc />
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <inheritdoc />
        public Column<T>? Column { get; set; }

        /// <inheritdoc />
        public string? Title { get; set; }

        /// <inheritdoc />
        public string? Operator { get; set; }

        /// <inheritdoc />
        public object? Value { get; set; }

        /// <summary>
        /// The function which performs the filter.
        /// </summary>
        public Func<T, bool>? FilterFunction { get; set; }

        /// <summary>
        /// The type of column being filtered.
        /// </summary>
        public FieldType FieldType => FieldType.Identify(Column?.PropertyType);

        /// <summary>
        /// Generates a function which performs the filter.
        /// </summary>
        /// <param name="filterOptions">Any options for generation, such as case sensitivity.</param>
        /// <returns>A function which performs the filter.</returns>
        public Func<T, bool> GenerateFilterFunction(FilterOptions? filterOptions = null)
        {
            if (FilterFunction is not null)
            {
                return FilterFunction;
            }

            if (Column is null)
            {
                return x => true;
            }

            // We need a PropertyExpression to filter. This allows us to pass in an arbitrary PropertyExpression.
            // Although, it would be better in that case to simple use the FilterFunction so that we do not 
            // have to generate and compile anything.
            var propertyExpression = Column.PropertyExpression;

            var hash = HashCode.Combine(propertyExpression, Operator, Value);

            if (_cachedExpressionHashCode == hash && _cachedFilterFunction is not null)
            {
                return _cachedFilterFunction;
            }

            var filterExpression = FilterExpressionGenerator.GenerateExpression(this, filterOptions);
            var function = filterExpression.Compile();
            _cachedExpressionHashCode = hash;
            _cachedFilterFunction = function;

            return function;
        }

        public IFilterDefinition<T> Clone()
        {
            return new FilterDefinition<T>
            {
                Column = Column,
                FilterFunction = FilterFunction,
                Operator = Operator,
                Title = Title,
                Value = Value,
            };
        }
    }
}
