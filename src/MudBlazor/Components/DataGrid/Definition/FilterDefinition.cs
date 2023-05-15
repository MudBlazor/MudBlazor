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

            var filterExpression = GenerateFilterExpression(propertyExpression, filterOptions);
            var function = filterExpression.Compile();
            _cachedExpressionHashCode = hash;
            _cachedFilterFunction = function;

            return function;
        }

        //Backward compatibility, in v7 should be removed and only public visible method should be GenerateFilterFunction
        public Expression<Func<T, bool>> GenerateFilterExpression()
        {
            //There should be no dependency of DataGrid, because it makes testing hard and it's not worth to have such a heavy dependency just to extract case sensitivity
            return GenerateFilterExpression(Column?.PropertyExpression, new FilterOptions
            {
                FilterCaseSensitivity = DataGrid?.FilterCaseSensitivity ?? DataGridFilterCaseSensitivity.Default
            });
        }

        //Maybe make is static or move totally different place?
        private Expression<Func<T, bool>> GenerateFilterExpression(LambdaExpression? propertyExpression, FilterOptions? filterOptions)
        {
            filterOptions ??= FilterOptions.Default; //Default if null
            var fieldType = FieldType;

            if (propertyExpression is null)
            {
                return x => true;
            }

            if (fieldType.IsString)
            {
                var value = Value?.ToString();
                var stringComparer = filterOptions.FilterCaseSensitivity == DataGridFilterCaseSensitivity.Default ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                if (value is null && Operator != FilterOperator.String.Empty && Operator != FilterOperator.String.NotEmpty)
                    return x => true;

                return Operator switch
                {
                    FilterOperator.String.Contains =>
                        propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => value != null && x != null && x.Contains(value, stringComparer))),
                    FilterOperator.String.NotContains =>
                        propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => value != null && x != null && !x.Contains(value, stringComparer))),
                    FilterOperator.String.Equal =>
                        propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && x.Equals(value, stringComparer))),
                    FilterOperator.String.NotEqual =>
                        propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && !x.Equals(value, stringComparer))),
                    FilterOperator.String.StartsWith =>
                        propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => value != null && x != null && x.StartsWith(value, stringComparer))),
                    FilterOperator.String.EndsWith =>
                        propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => value != null && x != null && x.EndsWith(value, stringComparer))),
                    FilterOperator.String.Empty => propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => string.IsNullOrWhiteSpace(x))),
                    FilterOperator.String.NotEmpty => propertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => !string.IsNullOrWhiteSpace(x))),
                    _ => x => true
                };
            }

            if (fieldType.IsNumber)
            {
                if (Value is null && Operator != FilterOperator.Number.Empty && Operator != FilterOperator.Number.NotEmpty)
                    return x => true;

                return Operator switch
                {
                    FilterOperator.Number.Equal => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, Value),
                    FilterOperator.Number.NotEqual => propertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, Value),
                    FilterOperator.Number.GreaterThan => propertyExpression.GenerateBinary<T>(ExpressionType.GreaterThan, Value),
                    FilterOperator.Number.GreaterThanOrEqual => propertyExpression.GenerateBinary<T>(ExpressionType.GreaterThanOrEqual, Value),
                    FilterOperator.Number.LessThan => propertyExpression.GenerateBinary<T>(ExpressionType.LessThan, Value),
                    FilterOperator.Number.LessThanOrEqual => propertyExpression.GenerateBinary<T>(ExpressionType.LessThanOrEqual, Value),
                    FilterOperator.Number.Empty => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, null),
                    FilterOperator.Number.NotEmpty => propertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, null),
                    _ => x => true
                };
            }

            if (fieldType.IsDateTime)
            {
                if (Value is null && Operator != FilterOperator.DateTime.Empty && Operator != FilterOperator.DateTime.NotEmpty)
                    return x => true;

                return Operator switch
                {
                    FilterOperator.DateTime.Is => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, Value),
                    FilterOperator.DateTime.IsNot => propertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, Value),
                    FilterOperator.DateTime.After => propertyExpression.GenerateBinary<T>(ExpressionType.GreaterThan, Value),
                    FilterOperator.DateTime.OnOrAfter => propertyExpression.GenerateBinary<T>(ExpressionType.GreaterThanOrEqual, Value),
                    FilterOperator.DateTime.Before => propertyExpression.GenerateBinary<T>(ExpressionType.LessThan, Value),
                    FilterOperator.DateTime.OnOrBefore => propertyExpression.GenerateBinary<T>(ExpressionType.LessThanOrEqual, Value),
                    FilterOperator.DateTime.Empty => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, null),
                    FilterOperator.DateTime.NotEmpty => propertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, null),
                    _ => x => true
                };
            }

            if (fieldType.IsBoolean)
            {
                if (Value is null)
                    return x => true;

                return Operator switch
                {
                    FilterOperator.Boolean.Is => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, Value),
                    _ => x => true
                };
            }

            if (fieldType.IsEnum)
            {
                if (Value is null)
                    return x => true;

                return Operator switch
                {
                    FilterOperator.Enum.Is => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, Value),
                    FilterOperator.Enum.IsNot => propertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, Value),
                    _ => x => true
                };
            }

            if (fieldType.IsGuid)
            {
                return Operator switch
                {
                    FilterOperator.Guid.Equal => propertyExpression.GenerateBinary<T>(ExpressionType.Equal, Value),
                    FilterOperator.Guid.NotEqual => propertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, Value),
                    _ => x => true
                };
            }

            return x => true;
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
