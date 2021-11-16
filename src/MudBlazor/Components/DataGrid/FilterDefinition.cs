// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

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
                return Field == null ? typeof(object) : typeof(T).GetProperty(Field).PropertyType;
            }
        }

        /// <summary>
        /// This should be in a separate utility class.
        /// </summary>
        internal static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),
            typeof(double),
            typeof(decimal),
            typeof(long),
            typeof(short),
            typeof(sbyte),
            typeof(byte),
            typeof(ulong),
            typeof(ushort),
            typeof(uint),
            typeof(float),
            typeof(BigInteger)
        };

        private bool isNumber
        {
            get
            {
                return NumericTypes.Contains(dataType);
            }
        }

        internal Func<T, bool> GenerateFilterFunction()
        {
            // short circuit
            if (Value == null)
                return new Func<T, bool>(x => true);

            var parameter = Expression.Parameter(typeof(T), "x");

            Expression comparison = Expression.Empty();

            if (dataType == typeof(string))
            {
                var field = Expression.Property(parameter, typeof(T).GetProperty(Field));
                var valueString = Value?.ToString();

                switch (Operator)
                {
                    case "contains":
                        comparison = Expression.Call(field, dataType.GetMethod("Contains", new[] { dataType }), Expression.Constant(valueString));
                        break;
                    case "equals":
                        comparison = Expression.MakeBinary(ExpressionType.Equal, field, Expression.Constant(valueString));
                        break;
                    case "starts with":
                        comparison = Expression.Call(field, dataType.GetMethod("StartsWith", new[] { dataType }), Expression.Constant(valueString));
                        break;
                    case "ends with":
                        comparison = Expression.Call(field, dataType.GetMethod("EndsWith", new[] { dataType }), Expression.Constant(valueString));
                        break;
                    default:
                        return new Func<T, bool>(x => true);
                }
            }
            else if (isNumber)
            {
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(double));
                var valueNumber = Value == null ? 0 : Convert.ToDouble(Value);

                switch (Operator)
                {
                    case "=":
                        comparison = Expression.MakeBinary(ExpressionType.Equal, field, Expression.Constant(valueNumber));
                        break;
                    case "!=":
                        comparison = Expression.MakeBinary(ExpressionType.NotEqual, field, Expression.Constant(valueNumber));
                        break;
                    case ">":
                        comparison = Expression.MakeBinary(ExpressionType.GreaterThan, field, Expression.Constant(valueNumber));
                        break;
                    case ">=":
                        comparison = Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, field, Expression.Constant(valueNumber));
                        break;
                    case "<":
                        comparison = Expression.MakeBinary(ExpressionType.LessThan, field, Expression.Constant(valueNumber));
                        break;
                    case "<=":
                        comparison = Expression.MakeBinary(ExpressionType.LessThanOrEqual, field, Expression.Constant(valueNumber));
                        break;
                    default:
                        return new Func<T, bool>(x => true);
                }
            }
            else
            {
                return new Func<T, bool>(x => true);
            }

            var ex = Expression.Lambda<Func<T, bool>>(comparison, parameter);

            return ex.Compile();
        }

    }
}
