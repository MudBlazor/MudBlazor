// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;

namespace MudBlazor
{
    public class FilterDefinition<T>
    {
        public Guid Id { get; set; }
        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }

        private Type dataType
        {
            get
            {
                if (Field == null) return typeof(object);

                return typeof(T).GetProperty(Field).PropertyType;
            }
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

        public Func<T, bool> GenerateFilterFunction()
        {
            var expression = GenerateFilterExpression();

            return expression.Compile();
        }

        public Expression<Func<T, bool>> GenerateFilterExpression()
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression expression;

            if (dataType == typeof(string))
            {
                expression = GenerateFilterExpressionForStringType(parameter);
            }
            else if (isNumber)
            {
                expression = GenerateFilterExpressionForNumericTypes(parameter);
            }
            else if (isEnum)
            {
                expression = GenerateFilterExpressionForEnumTypes(parameter);
            }
            else if (isBoolean)
            {
                expression = GenerateFilterExpressionForBooleanTypes(parameter);
            }
            else if (isDateTime)
            {
                expression = GenerateFilterExpressionForDateTimeTypes(parameter);
            }
            else
            {
                expression = Expression.Constant(true, typeof(bool));
            }

            return Expression.Lambda<Func<T, bool>>(expression, parameter);
        }

        private Expression GenerateFilterExpressionForDateTimeTypes(ParameterExpression parameter)
        {
            var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(DateTime?));
            DateTime? valueDateTime = Value == null ? null : (DateTime)Value;
            var isnotnull = Expression.IsTrue(Expression.Property(field, "HasValue"));
            var isnull = Expression.IsFalse(Expression.Property(field, "HasValue"));
            var notNullDateTime = Expression.Convert(field, typeof(DateTime));
            var valueDateTimeConstant = Expression.Constant(valueDateTime);

            return Operator switch
            {
                FilterOperator.DateTime.Is when null != Value =>
                    Expression.AndAlso(isnotnull,
                        Expression.Equal(notNullDateTime, valueDateTimeConstant)),

                FilterOperator.DateTime.IsNot when null != Value =>
                    Expression.OrElse(isnull,
                        Expression.NotEqual(notNullDateTime, valueDateTimeConstant)),

                FilterOperator.DateTime.After when null != Value =>
                    Expression.AndAlso(isnotnull,
                        Expression.GreaterThan(notNullDateTime, valueDateTimeConstant)),

                FilterOperator.DateTime.OnOrAfter when null != Value =>
                    Expression.AndAlso(isnotnull,
                        Expression.GreaterThanOrEqual(notNullDateTime, valueDateTimeConstant)),

                FilterOperator.DateTime.Before when null != Value =>
                    Expression.AndAlso(isnotnull,
                        Expression.LessThan(notNullDateTime, valueDateTimeConstant)),

                FilterOperator.DateTime.OnOrBefore when null != Value =>
                    Expression.AndAlso(isnotnull,
                        Expression.LessThanOrEqual(notNullDateTime, valueDateTimeConstant)),

                FilterOperator.DateTime.Empty => isnull,
                FilterOperator.DateTime.NotEmpty => isnotnull,

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private Expression GenerateFilterExpressionForBooleanTypes(ParameterExpression parameter)
        {
            var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(bool?));
            bool? valueBool = Value == null ? null : Convert.ToBoolean(Value);
            var isnotnull = Expression.IsTrue(Expression.Property(field, "HasValue"));
            var notNullBool = Expression.Convert(field, typeof(bool));

            return Operator switch
            {
                FilterOperator.Enum.Is when Value != null => Expression.AndAlso(isnotnull,
                    Expression.Equal(notNullBool, Expression.Constant(valueBool))),

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private Expression GenerateFilterExpressionForEnumTypes(ParameterExpression parameter)
        {
            var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), dataType);
            var valueEnum = Value == null ? null : (Enum)Value;
            var _null = Expression.Convert(Expression.Constant(null), dataType);
            var isnull = Expression.Equal(field, _null);
            var isnotnull = Expression.NotEqual(field, _null);
            var valueEnumConstant = Expression.Convert(Expression.Constant(valueEnum), dataType);

            return Operator switch
            {
                FilterOperator.Enum.Is when Value != null =>
                    IsNullableEnum(dataType) ? Expression.AndAlso(isnotnull,
                            Expression.Equal(field, valueEnumConstant))
                        : Expression.Equal(field, valueEnumConstant),

                FilterOperator.Enum.IsNot when Value != null =>
                    IsNullableEnum(dataType) ? Expression.OrElse(isnull,
                            Expression.NotEqual(field, valueEnumConstant))
                        : Expression.NotEqual(field, valueEnumConstant),

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private Expression GenerateFilterExpressionForNumericTypes(ParameterExpression parameter)
        {
            var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(double?));
            double? valueNumber = Value == null ? null : Convert.ToDouble(Value);
            var isnotnull = Expression.IsTrue(Expression.Property(field, "HasValue"));
            var isnull = Expression.IsFalse(Expression.Property(field, "HasValue"));
            var notNullNumber = Expression.Convert(field, typeof(double));
            var valueNumberConstant = Expression.Constant(valueNumber);

            return Operator switch
            {
                FilterOperator.Number.Equal when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Equal(notNullNumber, valueNumberConstant)),

                FilterOperator.Number.NotEqual when Value != null =>
                    Expression.OrElse(isnull,
                        Expression.NotEqual(notNullNumber, valueNumberConstant)),

                FilterOperator.Number.GreaterThan when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.GreaterThan(notNullNumber, valueNumberConstant)),

                FilterOperator.Number.GreaterThanOrEqual when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.GreaterThanOrEqual(notNullNumber, valueNumberConstant)),

                FilterOperator.Number.LessThan when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.LessThan(notNullNumber, valueNumberConstant)),

                FilterOperator.Number.LessThanOrEqual when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.LessThanOrEqual(notNullNumber, valueNumberConstant)),

                FilterOperator.Number.Empty => isnull,
                FilterOperator.Number.NotEmpty => isnotnull,

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private Expression GenerateFilterExpressionForStringType(ParameterExpression parameter)
        {
            var field = Expression.Property(parameter, typeof(T).GetProperty(Field));
            var valueString = Value?.ToString();
            var trim = Expression.Call(field, dataType.GetMethod("Trim", Type.EmptyTypes));
            var isnull = Expression.Equal(field, Expression.Constant(null));
            var isnotnull = Expression.NotEqual(field, Expression.Constant(null));

            return Operator switch
            {
                FilterOperator.String.Contains when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Call(field, dataType.GetMethod("Contains", new[] { dataType }), Expression.Constant(valueString))),

                FilterOperator.String.Equal when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Equal(field, Expression.Constant(valueString))),

                FilterOperator.String.StartsWith when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Call(field, dataType.GetMethod("StartsWith", new[] { dataType }), Expression.Constant(valueString))),

                FilterOperator.String.EndsWith when Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Call(field, dataType.GetMethod("EndsWith", new[] { dataType }), Expression.Constant(valueString))),

                FilterOperator.String.Empty =>
                    Expression.OrElse(isnull,
                        Expression.Equal(trim, Expression.Constant(string.Empty, dataType))),

                FilterOperator.String.NotEmpty =>
                    Expression.AndAlso(isnotnull,
                        Expression.NotEqual(trim, Expression.Constant(string.Empty, dataType))),

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private static bool IsNullableEnum(Type t)
        {
            Type u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }
    }
}
