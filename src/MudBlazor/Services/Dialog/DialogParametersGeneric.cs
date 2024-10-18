// Copyright (c) 2019 - Blazored
// Copyright (c) 2023 - Adaptations by Simon Schulze

using System;
using System.Linq.Expressions;

namespace MudBlazor;

#nullable enable
/// <summary>
/// The parameters passed into a <see cref="MudDialog"/> instance.
/// </summary>
/// <seealso cref="MudDialogInstance"/>
/// <seealso cref="MudDialogProvider"/>
/// <seealso cref="MudDialog"/>
/// <seealso cref="DialogOptions"/>
/// <seealso cref="DialogReference"/>
/// <seealso cref="DialogService"/>
public class DialogParameters<T> : DialogParameters
{
    /// <summary>
    /// Adds a parameter using a member expression.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter to add.</typeparam>
    /// <param name="propertyExpression">The property to add as a parameter.</param>
    /// <param name="value">The parameter value.</param>
    public void Add<TParam>(Expression<Func<T, TParam>> propertyExpression, TParam value)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        if (propertyExpression.Body is not MemberExpression memberExpression)
        {
            throw new ArgumentException($"Argument '{nameof(propertyExpression)}' must be a '{nameof(MemberExpression)}'");
        }

        Add(memberExpression.Member.Name, value);
    }

    /// <summary>
    /// Gets a parameter using a property expression.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter to get.</typeparam>
    /// <param name="propertyExpression">The property to get as a parameter.</param>
    /// <returns>The parameter value.</returns>
    public TParam? Get<TParam>(Expression<Func<T, TParam>> propertyExpression)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        if (propertyExpression.Body is not MemberExpression memberExpression)
        {
            throw new ArgumentException($"Argument '{nameof(propertyExpression)}' must be a '{nameof(MemberExpression)}'");
        }

        return Get<TParam?>(memberExpression.Member.Name);
    }

    /// <summary>
    /// Gets a parameter using a property expression or a default value if no parameter was found.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter to get.</typeparam>
    /// <param name="propertyExpression">The property to get as a parameter.</param>
    /// <returns>The parameter value.</returns>
    public TParam? TryGet<TParam>(Expression<Func<T, TParam>> propertyExpression)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        if (propertyExpression.Body is not MemberExpression memberExpression)
        {
            throw new ArgumentException($"Argument '{nameof(propertyExpression)}' must be a '{nameof(MemberExpression)}'");
        }

        return TryGet<TParam>(memberExpression.Member.Name);
    }
}
