// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
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
    /// <returns><c>false</c> when the expression is not a plain member chain rooted in a constant, in which case the caller must fall back to <see cref="FieldIdentifier.Create{TField}"/>; no part of the chain has been evaluated then.</returns>
    /// <remarks>
    /// A recognized chain is evaluated exactly once and any failure throws what <see cref="FieldIdentifier.Create{TField}"/> would have thrown, so the caller never re-runs user getters through the fallback.
    /// </remarks>
    public static bool TryCreate<TField>(Expression<Func<TField>> accessor, out FieldIdentifier fieldIdentifier)
    {
        var body = accessor.Body;
        if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary && unary.Type == typeof(object))
        {
            body = unary.Operand;
        }

        if (body is not MemberExpression member || !TryEvaluate(member.Expression, out var model))
        {
            fieldIdentifier = default;
            return false;
        }

        // FieldIdentifier.Create reads a member straight off a constant without compiling and lets the constructor reject the null model with ArgumentNullException; a deeper chain goes through its compiled path, which throws ArgumentException instead.
        if (model is null && member.Expression is not MemberExpression { Expression: ConstantExpression })
        {
            throw new ArgumentException("The provided expression must evaluate to a non-null value.");
        }

        fieldIdentifier = new FieldIdentifier(model!, member.Member.Name);
        return true;
    }

    /// <summary>
    /// Evaluates a supported owner chain, surfacing the same exceptions the compiled expression would raise.
    /// </summary>
    /// <returns><c>false</c> when the chain contains unsupported syntax; nothing has been evaluated in that case, because every shape is checked before the first member is read.</returns>
    private static bool TryEvaluate(Expression? expression, out object? value)
    {
        switch (expression)
        {
            case ConstantExpression constant:
                value = constant.Value;
                return true;
            // A static member has a null Expression, so it fails the recursive call and reports unresolved rather than reading off a null owner.
            case MemberExpression { Member: FieldInfo field } member when TryEvaluate(member.Expression, out var fieldOwner):
                // A compiled dereference of a null owner throws NullReferenceException, where reflection would throw TargetException.
                value = field.GetValue(fieldOwner ?? throw new NullReferenceException());
                return true;
            case MemberExpression { Member: PropertyInfo property } member when property.GetIndexParameters().Length == 0 && TryEvaluate(member.Expression, out var propertyOwner):
                value = ReadProperty(property, propertyOwner);
                return true;
            default:
                value = null;
                return false;
        }
    }

    private static object? ReadProperty(PropertyInfo property, object? owner)
    {
        // A Nullable<T> holding a value boxes as plain T, so the boxed owner already is .Value; NativeAOT also generates no code for reflective invocation of Nullable<T> members, and an empty one must fail like the compiled getter.
        if (property.DeclaringType is { } declaring && Nullable.GetUnderlyingType(declaring) is not null && property.Name == nameof(Nullable<int>.Value))
        {
            return owner ?? throw new InvalidOperationException("Nullable object must have a value.");
        }

        if (owner is null)
        {
            throw new NullReferenceException();
        }

        try
        {
            return property.GetValue(owner);
        }
        catch (TargetInvocationException e) when (e.InnerException is not null)
        {
            // The compiled expression surfaces the getter's own exception, so unwrap the reflection envelope without losing the original stack.
            ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            throw;
        }
    }
}
