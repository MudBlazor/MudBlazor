﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;

namespace MudBlazor.Utilities.Expressions;

#nullable enable
internal static class ExpressionNull
{
    /// <summary>
    /// Adds null checks to the property expression to handle null in expressions.
    /// </summary>
    /// <param name="expression">The original property expression.</param>
    /// <returns>A new expression with null checks added.</returns>
    public static Expression<Func<T, TProperty>> AddNullChecks<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var parameter = expression.Parameters[0];
        var body = AddNullChecks(expression.Body);

        //return body;
        return Expression.Lambda<Func<T, TProperty>>(body, parameter);
    }

    /// <summary>
    /// Recursively adds null checks to the expression body.
    /// </summary>
    /// <param name="expression">The expression body.</param>
    /// <returns>A new expression with null checks added.</returns>
    public static Expression AddNullChecks(Expression expression)
    {
        if (expression is MemberExpression { Expression: not null } memberExpression)
        {
            var nullCheck = Expression.Equal(memberExpression.Expression, Expression.Constant(null));
            var defaultValue = Expression.Default(memberExpression.Type);
            var memberAccess = Expression.MakeMemberAccess(AddNullChecks(memberExpression.Expression), memberExpression.Member);
            return Expression.Condition(nullCheck, defaultValue, memberAccess);
        }

        if (expression is MethodCallExpression { Object: not null } methodCallExpression)
        {
            var nullCheck = Expression.Equal(methodCallExpression.Object, Expression.Constant(null));
            var defaultValue = Expression.Default(methodCallExpression.Type);
            var methodCall = Expression.Call(AddNullChecks(methodCallExpression.Object), methodCallExpression.Method, methodCallExpression.Arguments);
            return Expression.Condition(nullCheck, defaultValue, methodCall);
        }

        return expression;
    }
}
