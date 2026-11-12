// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace MudBlazor
{
    internal static class ExpressionModifier
    {
        internal static Expression<Func<T, bool>> Modify<T>(this Expression firstExpression, Expression secondExpression)
        {
            var bodyIdentifier = new ExpressionBodyIdentifier();
            var body = bodyIdentifier.Identify(firstExpression);
            var parameterIdentifier = new ExpressionParameterIdentifier();
            var parameter = (ParameterExpression)parameterIdentifier.Identify(firstExpression);
            var body2 = bodyIdentifier.Identify(secondExpression);
            var parameter2 = (ParameterExpression)parameterIdentifier.Identify(secondExpression);

            var treeModifier = new ExpressionReplacer(parameter2, body);
            return Expression.Lambda<Func<T, bool>>(treeModifier.Visit(body2), parameter);
        }

        internal static Expression ReplaceBinary(this Expression exp, ExpressionType from, ExpressionType to)
        {
            var binaryReplacer = new BinaryReplacer(from, to);
            return binaryReplacer.Visit(exp);
        }

        public static Expression<Func<T, bool>> GenerateBinary<T>(this Expression expression, ExpressionType binaryOperation, object? value)
        {
            var bodyIdentifier = new ExpressionBodyIdentifier();
            var body = bodyIdentifier.Identify(expression);
            var parameterIdentifier = new ExpressionParameterIdentifier();
            var parameter = (ParameterExpression)parameterIdentifier.Identify(expression);
            BinaryExpression? binaryExpression;

            if (Nullable.GetUnderlyingType(body.Type) is not null)
            {
                // property type is nullable...
                binaryExpression = Expression.MakeBinary(binaryOperation, body, ConvertConstant(value, body.Type));
            }
            else
            {
                if (value is null)
                {
                    // We can short circuit here because the value to be compared is null and the property type is not nullable.
                    return x => true;
                }

                binaryExpression = Expression.MakeBinary(binaryOperation, body, ConvertConstant(value, body.Type));
            }

            return Expression.Lambda<Func<T, bool>>(binaryExpression, parameter);
        }

        // Chains equality checks with OrElse instead of calling Contains on a list, so the expression needs no generic method construction and query providers such as EF Core can translate it.
        internal static Expression<Func<T, bool>> GenerateAnyOf<T>(this Expression expression, IEnumerable values)
        {
            var bodyIdentifier = new ExpressionBodyIdentifier();
            var body = bodyIdentifier.Identify(expression);
            var parameterIdentifier = new ExpressionParameterIdentifier();
            var parameter = (ParameterExpression)parameterIdentifier.Identify(expression);
            Expression? anyOf = null;

            foreach (var value in values)
            {
                if (value is null)
                {
                    continue;
                }

                var equal = Expression.Equal(body, ConvertConstant(value, body.Type));
                anyOf = anyOf is null ? equal : Expression.OrElse(anyOf, equal);
            }

            if (anyOf is null)
            {
                // Nothing is selected, so the filter does not narrow the rows.
                return x => true;
            }

            return Expression.Lambda<Func<T, bool>>(anyOf, parameter);
        }

        // A number filter holds a double, which a decimal property has to compare as written: converted here, the constant is already a decimal when a query provider sees it.
        private static Expression ConvertConstant(object? value, Type type)
        {
            if (value is double or float && (Nullable.GetUnderlyingType(type) ?? type) == typeof(decimal))
            {
                return Expression.Constant(Num.ToDecimal(value), type);
            }

            return Expression.Convert(Expression.Constant(value), type);
        }

        public static Expression<Func<T, U>> ChangeExpressionReturnType<T, U>(this Expression expression)
        {
            var bodyIdentifier = new ExpressionBodyIdentifier();
            var body = bodyIdentifier.Identify(expression);
            var parameterIdentifier = new ExpressionParameterIdentifier();
            var parameter = (ParameterExpression)parameterIdentifier.Identify(expression);

            if (body.Type is U)
            {
                // Expression already has the right type.
                return Expression.Lambda<Func<T, U>>(body, parameter);
            }

            // Change parameter.
            // A double or float going to decimal converts through Num.ToDecimal, so an aggregate shows 0.1 and not its binary expansion.
            var sourceType = Nullable.GetUnderlyingType(body.Type) ?? body.Type;
            var toDecimal = (Nullable.GetUnderlyingType(typeof(U)) ?? typeof(U)) != typeof(decimal) ? null
                : sourceType == typeof(double) ? ((Func<double, decimal>)Num.ToDecimal).Method
                : sourceType == typeof(float) ? ((Func<float, decimal>)Num.ToDecimal).Method
                : null;
            var converted = Expression.Convert(body, typeof(U), toDecimal);
            return Expression.Lambda<Func<T, U>>(converted, parameter);
        }
    }

    internal class ExpressionReplacer : ExpressionVisitor
    {
        private readonly Expression _from;
        private readonly Expression _to;

        public ExpressionReplacer(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }

        [return: NotNullIfNotNull(nameof(node))]
        public override Expression? Visit(Expression? node)
        {
            if (node == _from) return _to;
            return base.Visit(node);
        }
    }

    internal class ExpressionBodyIdentifier : ExpressionVisitor
    {
        public Expression Identify(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return node.Body;
        }
    }

    internal class ExpressionParameterIdentifier : ExpressionVisitor
    {
        public Expression Identify(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return node.Parameters[0];
        }
    }

    internal class BinaryReplacer : ExpressionVisitor
    {
        private readonly ExpressionType _from;
        private readonly ExpressionType _to;

        public BinaryReplacer(ExpressionType from, ExpressionType to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == _from)
            {
                return Expression.MakeBinary(_to, node.Left, node.Right);
            }

            return base.VisitBinary(node);
        }
    }
}
