// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor
{
#nullable enable
    public class FilterDefinition<T> : IFilterDefinition<T>
    {
        private int _cachedExpressionHashCode;
        private Func<T, bool>? _cachedFilterFunction;

        public Guid Id { get; set; } = Guid.NewGuid();

        public Column<T>? Column { get; set; }

        public string? Title { get; set; }

        public string? Operator { get; set; }

        public object? Value { get; set; }

        public Func<T, bool>? FilterFunction { get; set; }

        public FieldType FieldType => FieldType.Identify(Column?.PropertyType);

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
