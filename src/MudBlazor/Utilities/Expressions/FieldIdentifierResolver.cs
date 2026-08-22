// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components.Forms;

namespace MudBlazor.Utilities.Expressions;

/// <summary>
/// Builds a <see cref="FieldIdentifier"/> from a <c>For</c> expression without compiling it.
/// </summary>
/// <remarks>
/// <see cref="FieldIdentifier.Create{TField}"/> only has a fast path when the member is read straight off a constant.
/// A nested path such as <c>() =&gt; _model.Address.Street</c> falls back to <see cref="Expression.Lambda(Expression, ParameterExpression[])"/> plus <see cref="LambdaExpression.Compile()"/>, which costs about 100 us and 4 KB every call.
/// Walking the member chain with reflection reads the same object graph and stays correct when the model instance is swapped.
/// </remarks>
internal static class FieldIdentifierResolver
{
    /// <summary>
    /// Resolves the field the expression points at.
    /// </summary>
    /// <returns><c>false</c> when the expression is not a plain member chain rooted in a constant, in which case the caller must fall back to <see cref="FieldIdentifier.Create{TField}"/>.</returns>
    public static bool TryCreate<TField>(Expression<Func<TField>> accessor, out FieldIdentifier fieldIdentifier)
    {
        var body = accessor.Body;
        if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary && unary.Type == typeof(object))
        {
            body = unary.Operand;
        }

        if (body is MemberExpression member && TryEvaluate(member.Expression, out var model) && model is not null)
        {
            fieldIdentifier = new FieldIdentifier(model, member.Member.Name);
            return true;
        }

        fieldIdentifier = default;
        return false;
    }

    private static bool TryEvaluate(Expression? expression, out object? value)
    {
        switch (expression)
        {
            case ConstantExpression constant:
                value = constant.Value;
                return true;
            // A static member has a null Expression, so it fails the recursive call and reports unresolved rather than reading off a null owner.
            case MemberExpression { Member: FieldInfo field } member when TryEvaluate(member.Expression, out var fieldOwner) && fieldOwner is not null:
                value = field.GetValue(fieldOwner);
                return true;
            case MemberExpression { Member: PropertyInfo property } member when property.GetIndexParameters().Length == 0 && TryEvaluate(member.Expression, out var propertyOwner) && propertyOwner is not null:
                value = property.GetValue(propertyOwner);
                return true;
            default:
                value = null;
                return false;
        }
    }
}
