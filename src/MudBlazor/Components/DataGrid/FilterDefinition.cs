// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;

namespace MudBlazor
{
#nullable enable
    public class FilterDefinition<T>
    {
        private int _cachedExpressionHashCode;
        private Func<T, bool>? _cachedFilterFunction;

        internal MudDataGrid<T>? DataGrid { get; set; }

        internal LambdaExpression? PropertyExpression { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        public Column<T>? Column { get; set; }
        public string? Title { get; set; }
        public string? Operator { get; set; }
        public object? Value { get; set; }
        public Func<T, bool>? FilterFunction { get; set; }

        public FieldType FieldType => FieldType.Identify(Column?.PropertyType);

        internal Type dataType
        {
            get
            {
                if (Column is null)
                    return typeof(object);

                return Column.PropertyType;
            }
        }

        public Func<T, bool> GenerateFilterFunction()
        {
            if (FilterFunction is not null)
                return FilterFunction;

            if (Column is null)
                return x => true;

            // We need a PropertyExpression to filter. This allows us to pass in an arbitrary PropertyExpression.
            // Although, it would be better in that case to simple use the FilterFunction so that we do not 
            // have to generate and compile anything.
            PropertyExpression ??= Column.PropertyExpression;

            var hash = HashCode.Combine(PropertyExpression, Operator, Value);

            if (_cachedExpressionHashCode == hash && _cachedFilterFunction is not null)
            {
                return _cachedFilterFunction;
            }

            var expression = GenerateFilterExpression();
            var f = expression.Compile();
            _cachedExpressionHashCode = hash;
            _cachedFilterFunction = f;

            return f;
        }

        public Expression<Func<T, bool>> GenerateFilterExpression()
        {
            var fieldType = FieldType;

            if (PropertyExpression is null)
            {
                return x => true;
            }

            if (fieldType.IsString)
            {
                var value = Value?.ToString();
                var stringComparer = DataGrid?.FilterCaseSensitivity == DataGridFilterCaseSensitivity.Default ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                if (value is null && Operator != FilterOperator.String.Empty && Operator != FilterOperator.String.NotEmpty)
                    return x => true;

                return Operator switch
                {
                    FilterOperator.String.Contains =>
                        PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => value != null && x != null && x.Contains(value, stringComparer))),
                    FilterOperator.String.NotContains =>
                        PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => value != null && x != null && !x.Contains(value, stringComparer))),
                    FilterOperator.String.Equal =>
                        PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && x.Equals(value, stringComparer))),
                    FilterOperator.String.NotEqual =>
                        PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && !x.Equals(value, stringComparer))),
                    FilterOperator.String.StartsWith =>
                        PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => value != null && x != null && x.StartsWith(value, stringComparer))),
                    FilterOperator.String.EndsWith =>
                        PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => value != null && x != null && x.EndsWith(value, stringComparer))),
                    FilterOperator.String.Empty => PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => string.IsNullOrWhiteSpace(x))),
                    FilterOperator.String.NotEmpty => PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => !string.IsNullOrWhiteSpace(x))),
                    _ => x => true
                };
            }

            if (fieldType.IsNumber)
            {
                if (Value == null && Operator != FilterOperator.Number.Empty && Operator != FilterOperator.Number.NotEmpty)
                    return x => true;

                return Operator switch
                {
                    FilterOperator.Number.Equal => PropertyExpression.GenerateBinary<T>(ExpressionType.Equal, Value),
                    FilterOperator.Number.NotEqual => PropertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, Value),
                    FilterOperator.Number.GreaterThan => PropertyExpression.GenerateBinary<T>(ExpressionType.GreaterThan, Value),
                    FilterOperator.Number.GreaterThanOrEqual => PropertyExpression.GenerateBinary<T>(ExpressionType.GreaterThanOrEqual, Value),
                    FilterOperator.Number.LessThan => PropertyExpression.GenerateBinary<T>(ExpressionType.LessThan, Value),
                    FilterOperator.Number.LessThanOrEqual => PropertyExpression.GenerateBinary<T>(ExpressionType.LessThanOrEqual, Value),
                    FilterOperator.Number.Empty => PropertyExpression.GenerateBinary<T>(ExpressionType.Equal, null),
                    FilterOperator.Number.NotEmpty => PropertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, null),
                    _ => x => true
                };
            }

            if (fieldType.IsDateTime)
            {
                if (Value == null && Operator != FilterOperator.DateTime.Empty && Operator != FilterOperator.DateTime.NotEmpty)
                    return x => true;

                return Operator switch
                {
                    FilterOperator.DateTime.Is => PropertyExpression.GenerateBinary<T>(ExpressionType.Equal, Value),
                    FilterOperator.DateTime.IsNot => PropertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, Value),
                    FilterOperator.DateTime.After => PropertyExpression.GenerateBinary<T>(ExpressionType.GreaterThan, Value),
                    FilterOperator.DateTime.OnOrAfter => PropertyExpression.GenerateBinary<T>(ExpressionType.GreaterThanOrEqual, Value),
                    FilterOperator.DateTime.Before => PropertyExpression.GenerateBinary<T>(ExpressionType.LessThan, Value),
                    FilterOperator.DateTime.OnOrBefore => PropertyExpression.GenerateBinary<T>(ExpressionType.LessThanOrEqual, Value),
                    FilterOperator.DateTime.Empty => PropertyExpression.GenerateBinary<T>(ExpressionType.Equal, null),
                    FilterOperator.DateTime.NotEmpty => PropertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, null),
                    _ => x => true
                };
            }

            if (fieldType.IsBoolean)
            {
                if (Value == null)
                    return x => true;

                return Operator switch
                {
                    FilterOperator.Boolean.Is => PropertyExpression.GenerateBinary<T>(ExpressionType.Equal, Value),
                    _ => x => true
                };
            }

            if (fieldType.IsEnum)
            {
                if (Value == null)
                    return x => true;

                return Operator switch
                {
                    FilterOperator.Enum.Is => PropertyExpression.GenerateBinary<T>(ExpressionType.Equal, Value),
                    FilterOperator.Enum.IsNot => PropertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, Value),
                    _ => x => true
                };
            }

            if (fieldType.IsGuid)
            {
                return Operator switch
                {
                    FilterOperator.Guid.Equal => PropertyExpression.GenerateBinary<T>(ExpressionType.Equal, Value),
                    FilterOperator.Guid.NotEqual => PropertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, Value),
                    _ => x => true
                };
            }

            return x => true;
        }

        public FilterDefinition<T> Clone()
        {
            return new FilterDefinition<T>
            {
                Column = Column,
                DataGrid = DataGrid,
                FilterFunction = FilterFunction,
                Operator = Operator,
                PropertyExpression = PropertyExpression,
                Title = Title,
                Value = Value,
            };
        }
    }
}
