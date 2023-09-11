// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace MudBlazor
{
#nullable enable
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
                binaryExpression = Expression.MakeBinary(binaryOperation, body, Expression.Convert(Expression.Constant(value), body.Type));
            }
            else
            {
                if (value is null)
                {
                    // We can short circuit here because the value to be compared is null and the property type is not nullable.
                    return x => true;
                }

                binaryExpression = Expression.MakeBinary(binaryOperation, body, Expression.Convert(Expression.Constant(value), body.Type));
            }

            return Expression.Lambda<Func<T, bool>>(binaryExpression, parameter);
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
            var converted = Expression.Convert(body, typeof(U));
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
