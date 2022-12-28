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
        internal MudDataGrid<T> DataGrid { get; set; }
        internal LambdaExpression? PropertyExpression { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Column<T> Column { get; set; }
        public string Field { get; set; }
        public string Title { get; set; }
        public Type FieldType { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public Func<T, bool> FilterFunction { get; set; }

        private Type dataType
        {
            get
            {
                if (Column == null)
                    return typeof(object);

                return Column.PropertyType;
            }
        }

        public Func<T, bool> GenerateFilterFunction()
        {
            if (FilterFunction != null)
                return FilterFunction;

            if (Column != null)
            {
                var expression = GenerateFilterExpression();
                return expression.Compile();
            }

            return x => true;
        }

        public Expression<Func<T, bool>> GenerateFilterExpression()
        {
            if (dataType == typeof(string))
            {
                string value = Value == null ? null : Value.ToString();
                var stringComparer = DataGrid.FilterCaseSensitivity == DataGridFilterCaseSensitivity.Default ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                return Operator switch
                {
                    FilterOperator.String.Contains =>
                        PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && value != null && x.Contains(value, stringComparer))),
                    FilterOperator.String.NotContains =>
                        PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && value != null && !x.Contains(value, stringComparer))),
                    FilterOperator.String.Equal =>
                        PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && x.Equals(value, stringComparer))),
                    FilterOperator.String.NotEqual =>
                        PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && !x.Equals(value, stringComparer))),
                    FilterOperator.String.StartsWith =>
                        PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && x.StartsWith(value, stringComparer))),
                    FilterOperator.String.EndsWith =>
                        PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => x != null && x.EndsWith(value, stringComparer))),
                    FilterOperator.String.Empty => PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => string.IsNullOrWhiteSpace(x))),
                    FilterOperator.String.NotEmpty => PropertyExpression.Modify<T>((Expression<Func<string?, bool>>)(x => !string.IsNullOrWhiteSpace(x))),
                    _ => x => true
                };
            }
            else if (isNumber)
            {
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
            else if (isDateTime)
            {
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
            else if (isBoolean)
            {
                return Operator switch
                {
                    FilterOperator.Boolean.Is => PropertyExpression.GenerateBinary<T>(ExpressionType.Equal, Value),
                    _ => x => true
                };
            }
            else if (isEnum)
            {
                return Operator switch
                {
                    FilterOperator.Enum.Is => PropertyExpression.GenerateBinary<T>(ExpressionType.Equal, Value),
                    FilterOperator.Enum.IsNot => PropertyExpression.GenerateBinary<T>(ExpressionType.NotEqual, Value),
                    _ => x => true
                };
            }
            else if (isGuid)
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

        private bool isNumber
        {
            get
            {
                return FilterOperator.IsNumber(dataType);
            }
        }

        private bool isEnum
        {
            get
            {
                return FilterOperator.IsEnum(dataType);
            }
        }

        private bool isDateTime
        {
            get
            {
                return FilterOperator.IsDateTime(dataType);
            }
        }

        private bool isBoolean
        {
            get
            {
                return FilterOperator.IsBoolean(dataType);
            }
        }

        private bool isGuid
        {
            get
            {
                return FilterOperator.IsGuid(dataType);
            }
        }
    }

#nullable disable
}
