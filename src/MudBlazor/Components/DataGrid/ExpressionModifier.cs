// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;

namespace MudBlazor
{
    internal static class ExpressionModifier
    {
        internal static Expression<Func<T, bool>> Modify<T>(this Expression exp1, Expression exp2)
        {
            var bodyIdentifier = new ExpressionBodyIdentifier();
            var body = bodyIdentifier.Identify(exp1);
            var parameterIdentifier = new ExpressionParameterIdentifier();
            var parameter = (ParameterExpression)parameterIdentifier.Identify(exp1);

            var bodyIdentifier2 = new ExpressionBodyIdentifier();
            var body2 = bodyIdentifier.Identify(exp2);
            var parameterIdentifier2 = new ExpressionParameterIdentifier();
            var parameter2 = (ParameterExpression)parameterIdentifier.Identify(exp2);

            var treeModifier = new ExpressionReplacer(parameter2, body);
            return Expression.Lambda<Func<T, bool>>(treeModifier.Visit(body2), parameter);
        }

        internal static Expression ReplaceBinary(this Expression exp, ExpressionType from, ExpressionType to)
        {
            var binaryReplacer = new BinaryReplacer(from, to);
            return binaryReplacer.Visit(exp);
        }

        public static Expression<Func<T, bool>> GenerateBinary<T>(this Expression exp1, ExpressionType binaryOperation, object value)
        {
            var bodyIdentifier = new ExpressionBodyIdentifier();
            var body = bodyIdentifier.Identify(exp1);
            var parameterIdentifier = new ExpressionParameterIdentifier();
            var parameter = (ParameterExpression)parameterIdentifier.Identify(exp1);
            BinaryExpression b = null;

            if (Nullable.GetUnderlyingType(body.Type) != null)
            {
                // property type is nullable...
                b = Expression.MakeBinary(binaryOperation, body, Expression.Convert(Expression.Constant(value), body.Type));
            }
            else
            {
                if (value == null)
                {
                    // We can short circuit here because the value to be compared is null and the property type is not nullable.
                    return x => true;
                }

                b = Expression.MakeBinary(binaryOperation, body, Expression.Convert(Expression.Constant(value), body.Type));
            }

            return Expression.Lambda<Func<T, bool>>(b, parameter);
        }

        public static Expression<Func<T, U>> ChangeExpressionReturnType<T, U>(this Expression exp)
        {
            var bodyIdentifier = new ExpressionBodyIdentifier();
            var body = bodyIdentifier.Identify(exp);
            var parameterIdentifier = new ExpressionParameterIdentifier();
            var parameter = (ParameterExpression)parameterIdentifier.Identify(exp);

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
        Expression from, to;

        public ExpressionReplacer(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }

        public override Expression Visit(Expression node)
        {
            if (node == from) return to;
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
        ExpressionType from, to;

        public BinaryReplacer(ExpressionType from, ExpressionType to)
        {
            this.from = from;
            this.to = to;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == from)
            {
                return Expression.MakeBinary(to, node.Left, node.Right);
            }

            return base.VisitBinary(node);
        }
    }
}
