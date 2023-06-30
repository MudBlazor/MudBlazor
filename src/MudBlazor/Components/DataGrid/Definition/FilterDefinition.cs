// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;

namespace MudBlazor
{
#nullable enable
    public class FilterDefinition<T> : IFilterDefinition<T>
    {
        private int _cachedExpressionHashCode;
        private Func<T, bool>? _cachedFilterFunction;

        //Backward compatibility, in v7 should be removed
        internal MudDataGrid<T>? DataGrid { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        public Column<T>? Column { get; set; }

        public string? Title { get; set; }

        public string? Operator { get; set; }

        public object? Value { get; set; }

        public Func<T, bool>? FilterFunction { get; set; }

        public FieldType FieldType => FieldType.Identify(Column?.PropertyType);

        //Backward compatibility, in v7 should be removed
        public Func<T, bool> GenerateFilterFunction()
        {
            IFilterDefinition<T> filterDefinition = this;
            return filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = DataGrid?.FilterCaseSensitivity ?? DataGridFilterCaseSensitivity.Default
            });
        }

        Func<T, bool> IFilterDefinition<T>.GenerateFilterFunction(FilterOptions? filterOptions)
        {
            if (FilterFunction is not null)
                return FilterFunction;

            if (Column is null)
                return x => true;

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

        //Backward compatibility, in v7 should be removed and only public visible method should be GenerateFilterFunction
        [Obsolete($"Will be removed in v7. Use {nameof(FilterExpressionGenerator.GenerateExpression)} instead.")]
        public Expression<Func<T, bool>> GenerateFilterExpression()
        {
            //There should be no dependency of DataGrid, because it makes testing hard and it's not worth to have such a heavy dependency just to extract case sensitivity
            var filterOptions = new FilterOptions
            {
                FilterCaseSensitivity = DataGrid?.FilterCaseSensitivity ?? DataGridFilterCaseSensitivity.Default,
            };
            var expression = FilterExpressionGenerator.GenerateExpression(this, filterOptions);

            return expression;
        }

        //Backward compatibility, in v7 should be removed
        public FilterDefinition<T> Clone()
        {
            return new FilterDefinition<T>
            {
                Column = Column,
                DataGrid = DataGrid,
                FilterFunction = FilterFunction,
                Operator = Operator,
                Title = Title,
                Value = Value,
            };
        }

        IFilterDefinition<T> IFilterDefinition<T>.Clone()
        {
            return Clone();
        }
    }
}
