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

        internal Func<T, bool> GenerateFilterFunction()
        {
            // short circuit
            //if (Value == null)
            //    return new Func<T, bool>(x => true);

            var parameter = Expression.Parameter(typeof(T), "x");

            Expression comparison = Expression.Empty();

            if (dataType == typeof(string))
            {
                var field = Expression.Property(parameter, typeof(T).GetProperty(Field));
                var valueString = Value?.ToString();
                var trim = Expression.Call(field, dataType.GetMethod("Trim", Type.EmptyTypes));
                var isnull = Expression.Equal(field, Expression.Constant(null));
                var isnotnull = Expression.NotEqual(field, Expression.Constant(null));

                switch (Operator)
                {
                    case FilterOperator.String.Contains:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull, 
                            Expression.Call(field, dataType.GetMethod("Contains", new[] { dataType }), Expression.Constant(valueString)));
                        break;
                    case FilterOperator.String.Equal:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull,
                            Expression.Equal(field, Expression.Constant(valueString)));
                        break;
                    case FilterOperator.String.StartsWith:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull, 
                            Expression.Call(field, dataType.GetMethod("StartsWith", new[] { dataType }), Expression.Constant(valueString)));
                        break;
                    case FilterOperator.String.EndsWith:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull, 
                            Expression.Call(field, dataType.GetMethod("EndsWith", new[] { dataType }), Expression.Constant(valueString)));
                        break;
                    case FilterOperator.String.Empty:
                        comparison = Expression.OrElse(isnull,
                            Expression.Equal(trim, Expression.Constant(string.Empty, dataType)));
                        break;
                    case FilterOperator.String.NotEmpty:
                        comparison = Expression.AndAlso(isnotnull,
                            Expression.NotEqual(trim, Expression.Constant(string.Empty, dataType)));
                        break;
                    default:
                        return alwaysTrue;
                }
            }
            else if (isNumber)
            {
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(double?));
                double? valueNumber = Value == null ? null : Convert.ToDouble(Value);
                var isnotnull = Expression.IsTrue(Expression.Property(field, "HasValue"));
                var isnull = Expression.IsFalse(Expression.Property(field, "HasValue"));
                var notNullNumber = Expression.Convert(field, typeof(double));
                var valueNumberConstant = Expression.Constant(valueNumber);

                switch (Operator)
                {
                    case FilterOperator.Number.Equal:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull,
                            Expression.Equal(notNullNumber, valueNumberConstant));
                        break;
                    case FilterOperator.Number.NotEqual:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.OrElse(isnull, 
                            Expression.NotEqual(notNullNumber, valueNumberConstant));
                        break;
                    case FilterOperator.Number.GreaterThan:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull, 
                            Expression.GreaterThan(notNullNumber, valueNumberConstant));
                        break;
                    case FilterOperator.Number.GreaterThanOrEqual:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull, 
                            Expression.GreaterThanOrEqual(notNullNumber, valueNumberConstant));
                        break;
                    case FilterOperator.Number.LessThan:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull, 
                            Expression.LessThan(notNullNumber, valueNumberConstant));
                        break;
                    case FilterOperator.Number.LessThanOrEqual:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull, 
                            Expression.LessThanOrEqual(notNullNumber, valueNumberConstant));
                        break;
                    case FilterOperator.Number.Empty:
                        comparison = isnull;
                        break;
                    case FilterOperator.Number.NotEmpty:
                        comparison = isnotnull;
                        break;

                    default:
                        return alwaysTrue;
                }
            }
            else if (isEnum)
            {
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), dataType);
                var valueEnum = Value == null ? null : (Enum)Value;
                var _null = Expression.Convert(Expression.Constant(null), dataType);
                var isnull = Expression.Equal(field, _null);
                var isnotnull = Expression.NotEqual(field, _null);
                var valueEnumConstant = Expression.Convert(Expression.Constant(valueEnum), dataType);

                switch (Operator)
                {
                    case FilterOperator.Enum.Is:
                        if (Value == null)
                            return alwaysTrue;

                        if (IsNullableEnum(dataType))
                        {
                            comparison = Expression.AndAlso(isnotnull,
                            Expression.Equal(field, valueEnumConstant));
                        }
                        else
                        {
                            comparison = Expression.Equal(field, valueEnumConstant);
                        }

                        break;
                    case FilterOperator.Enum.IsNot:
                        if (Value == null)
                            return alwaysTrue;

                        if (IsNullableEnum(dataType))
                        {
                            comparison = Expression.OrElse(isnull,
                                Expression.NotEqual(field, valueEnumConstant));
                        }
                        else
                        {
                            comparison = Expression.NotEqual(field, valueEnumConstant);
                        }

                        break;

                    default:
                        return alwaysTrue;
                }
            }
            else if (isBoolean)
            {
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(bool?));
                bool? valueBool = Value == null ? null : Convert.ToBoolean(Value);
                var isnotnull = Expression.IsTrue(Expression.Property(field, "HasValue"));
                var notNullBool = Expression.Convert(field, typeof(bool));

                switch (Operator)
                {
                    case FilterOperator.Enum.Is:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull, 
                            Expression.Equal(notNullBool, Expression.Constant(valueBool)));
                        break;

                    default:
                        return alwaysTrue;
                }
            }
            else if (isDateTime)
            {
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(DateTime?));
                DateTime? valueDateTime = Value == null ? null : (DateTime)Value;
                var isnotnull = Expression.IsTrue(Expression.Property(field, "HasValue"));
                var isnull = Expression.IsFalse(Expression.Property(field, "HasValue"));
                var notNullDateTime = Expression.Convert(field, typeof(DateTime));
                var valueDateTimeConstant = Expression.Constant(valueDateTime);

                switch (Operator)
                {
                    case FilterOperator.DateTime.Is:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull,
                            Expression.Equal(notNullDateTime, valueDateTimeConstant));
                        break;
                    case FilterOperator.DateTime.IsNot:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.OrElse(isnull,
                            Expression.NotEqual(notNullDateTime, valueDateTimeConstant));
                        break;
                    case FilterOperator.DateTime.After:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull,
                            Expression.GreaterThan(notNullDateTime, valueDateTimeConstant));
                        break;
                    case FilterOperator.DateTime.OnOrAfter:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull,
                            Expression.GreaterThanOrEqual(notNullDateTime, valueDateTimeConstant));
                        break;

                    case FilterOperator.DateTime.Before:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull,
                            Expression.LessThan(notNullDateTime, valueDateTimeConstant));
                        break;
                    case FilterOperator.DateTime.OnOrBefore:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.AndAlso(isnotnull,
                            Expression.LessThanOrEqual(notNullDateTime, valueDateTimeConstant));
                        break;
                    case FilterOperator.DateTime.Empty:
                        comparison = isnull;
                        break;
                    case FilterOperator.DateTime.NotEmpty:
                        comparison = isnotnull;
                        break;

                    default:
                        return alwaysTrue;
                }
            }
            else
            {
                return alwaysTrue;
            }

            var ex = Expression.Lambda<Func<T, bool>>(comparison, parameter);
            return ex.Compile();
        }

        private Func<T, bool> alwaysTrue = (x) => true;

        private bool IsNullableEnum(Type t)
        {
            Type u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }
    }
}
