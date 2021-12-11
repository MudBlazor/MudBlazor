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
                return Field == null ? typeof(object) : typeof(T).GetProperty(Field).PropertyType;
            }
        }

        private bool isNumber
        {
            get
            {
                return FilterOperator.NumericTypes.Contains(dataType);
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

                // reuseable expressions
                var trim = Expression.Call(field, dataType.GetMethod("Trim", Type.EmptyTypes));

                switch (Operator)
                {
                    case FilterOperator.String.Contains:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.Call(field, dataType.GetMethod("Contains", new[] { dataType }), Expression.Constant(valueString));
                        break;
                    case FilterOperator.String.Equal:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.MakeBinary(ExpressionType.Equal, field, Expression.Constant(valueString));
                        break;
                    case FilterOperator.String.StartsWith:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.Call(field, dataType.GetMethod("StartsWith", new[] { dataType }), Expression.Constant(valueString));
                        break;
                    case FilterOperator.String.EndsWith:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.Call(field, dataType.GetMethod("EndsWith", new[] { dataType }), Expression.Constant(valueString));
                        break;
                    case FilterOperator.String.Empty:
                        comparison = Expression.MakeBinary(ExpressionType.Or,
                            Expression.MakeBinary(ExpressionType.Equal, field, Expression.Constant(null, dataType)),
                            Expression.MakeBinary(ExpressionType.Equal, trim, Expression.Constant(string.Empty, dataType)));
                        break;
                    case FilterOperator.String.NotEmpty:
                        comparison = Expression.MakeBinary(ExpressionType.And,
                            Expression.MakeBinary(ExpressionType.NotEqual, field, Expression.Constant(null, dataType)),
                            Expression.MakeBinary(ExpressionType.NotEqual, trim, Expression.Constant(string.Empty, dataType)));
                        break;
                    default:
                        return alwaysTrue;
                }
            }
            else if (isNumber)
            {
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), typeof(double));
                var valueNumber = Value == null ? 0 : Convert.ToDouble(Value);

                switch (Operator)
                {
                    case FilterOperator.Number.Equal:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.MakeBinary(ExpressionType.Equal, field, Expression.Constant(valueNumber));
                        break;
                    case FilterOperator.Number.NotEqual:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.MakeBinary(ExpressionType.NotEqual, field, Expression.Constant(valueNumber));
                        break;
                    case FilterOperator.Number.GreaterThan:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.MakeBinary(ExpressionType.GreaterThan, field, Expression.Constant(valueNumber));
                        break;
                    case FilterOperator.Number.GreaterThanOrEqual:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, field, Expression.Constant(valueNumber));
                        break;
                    case FilterOperator.Number.LessThan:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.MakeBinary(ExpressionType.LessThan, field, Expression.Constant(valueNumber));
                        break;
                    case FilterOperator.Number.LessThanOrEqual:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.MakeBinary(ExpressionType.LessThanOrEqual, field, Expression.Constant(valueNumber));
                        break;
                    case FilterOperator.Number.Empty:
                        comparison = Expression.MakeBinary(ExpressionType.Equal, field, Expression.Constant(null, dataType));
                        break;
                    case FilterOperator.Number.NotEmpty:
                        comparison = Expression.MakeBinary(ExpressionType.NotEqual, field, Expression.Constant(null, dataType));
                        break;

                    default:
                        return alwaysTrue;
                }
            }
            else if (dataType.IsEnum)
            {
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), dataType);
                var valueEnum = Value == null ? null : (Enum)Value;

                switch (Operator)
                {
                    case FilterOperator.Enum.Is:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.MakeBinary(ExpressionType.Equal, field, Expression.Convert(Expression.Constant(valueEnum), dataType));
                        break;
                    case FilterOperator.Enum.IsNot:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.MakeBinary(ExpressionType.NotEqual, field, Expression.Convert(Expression.Constant(valueEnum), dataType));
                        break;

                    default:
                        return alwaysTrue;
                }
            }
            else if (dataType == typeof(bool))
            {
                var field = Expression.Convert(Expression.Property(parameter, typeof(T).GetProperty(Field)), dataType);
                var valueBool = Value == null ? false : Convert.ToBoolean(Value);

                switch (Operator)
                {
                    case FilterOperator.Enum.Is:
                        if (Value == null)
                            return alwaysTrue;

                        comparison = Expression.MakeBinary(ExpressionType.Equal, field, Expression.Constant(valueBool));
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
    }
}
