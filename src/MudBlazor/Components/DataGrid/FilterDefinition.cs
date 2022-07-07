// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        private bool IsEnumFlags
        {
            get
            {
                return FilterOperator.IsEnumFlags(dataType);
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

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, Type extendedType)
        {
            var query = from type in assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == extendedType
                        select method;
            return query;
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
                var nullableDoubleType = typeof(double?);
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), nullableDoubleType);
                double? valueNumber = Value == null ? null : Convert.ToDouble(Value);
                var isnotnull = Expression.IsTrue(Expression.Property(field, nullableDoubleType, "HasValue"));
                var isnull = Expression.IsFalse(Expression.Property(field, nullableDoubleType, "HasValue"));
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
            else if (IsEnumFlags)
            {
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(Enum));
                var valueEnumFlags = Value == null ? null : (Enum)Value;
                var _null = Expression.Convert(Expression.Constant(null), typeof(Enum));
                var isnotnull = Expression.NotEqual(field, _null);
                var method = typeof(Enum).GetMethod("HasFlag");

                switch (Operator)
                {
                    case FilterOperator.EnumFlags.Contains:
                        if (Value == null)
                            return alwaysTrue;
                        if (IsNullableEnum(dataType))
                        {
                            var left = isnotnull;
                            var right = Expression.Call(field, method, Expression.Constant(valueEnumFlags, typeof(Enum)));
                            comparison = Expression.AndAlso(left, right);
                        }
                        else
                        {
                            comparison = Expression.Call(field, method, Expression.Constant(valueEnumFlags, typeof(Enum)));
                        }
                        break;
                    case FilterOperator.EnumFlags.Is:
                        if (Value == null)
                            return alwaysTrue;

                        List<Enum> selectedValues = new List<Enum>();
                        foreach (Enum item in Enum.GetValues(dataType))
                            if (valueEnumFlags.HasFlag(item) && Convert.ToUInt64(item) != 0)
                                selectedValues.Add(item);

                        var overload = typeof(List<Enum>).GetMethod("Contains", new[] { typeof(Enum) });
                        if (IsNullableEnum(dataType))
                        {
                            var left = isnotnull;
                            var right = Expression.Call(Expression.Constant(selectedValues), overload, field);
                            comparison = Expression.AndAlso(left, right);
                        }
                        else
                        {
                            comparison = Expression.Call(Expression.Constant(selectedValues), overload, field);
                        }
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
                var nullableBoolType = typeof(bool?);
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), nullableBoolType);
                bool? valueBool = Value == null ? null : Convert.ToBoolean(Value);
                var isnotnull = Expression.IsTrue(Expression.Property(field, nullableBoolType, "HasValue"));
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
                var nullableDateTimeType = typeof(DateTime?);
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), nullableDateTimeType);
                DateTime? valueDateTime = Value == null ? null : (DateTime)Value;
                var isnotnull = Expression.IsTrue(Expression.Property(field, nullableDateTimeType, "HasValue"));
                var isnull = Expression.IsFalse(Expression.Property(field, nullableDateTimeType, "HasValue"));
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
