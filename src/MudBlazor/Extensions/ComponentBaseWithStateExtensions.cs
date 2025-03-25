// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MudBlazor.State;

namespace MudBlazor.Extensions;

#nullable enable
/// <summary>
/// Provides extension methods for <see cref="ComponentBaseWithState"/> components to facilitate accessing parameter states.
/// </summary>
public static class ComponentBaseWithStateExtensions
{
    /// <summary>
    /// Gets the read-only parameter state of a specified property in the component.
    /// </summary>
    /// <typeparam name="TComponent">The type of the MudBlazor component.</typeparam>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="component">The MudBlazor component instance.</param>
    /// <param name="propertyExpression">An expression representing the property whose parameter state needs to be accessed.</param>
    /// <param name="propertyNameCallerArgumentExpression">The property name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The <see cref="ParameterState{T}.Value"/> of the specified property.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyExpression"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="propertyExpression"/> is invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the parameter state with <paramref name="propertyExpression"/> is not found.</exception>
    public static T? GetState<TComponent, T>(this TComponent component, Func<TComponent, T> propertyExpression, [CallerArgumentExpression(nameof(propertyExpression))] string? propertyNameCallerArgumentExpression = null) where TComponent : ComponentBaseWithState
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        var propertyName = GetPropertyFromFuncLambda(propertyNameCallerArgumentExpression);

        return GetState<T>(component, propertyName);
    }

    /// <summary>
    /// Gets the read-only parameter state of a specified property in the component.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="component">The MudBlazor component instance.</param>
    /// <param name="propertyName">The name of the property whose parameter state needs to be accessed. Use nameof(...) to get the property name.</param>
    /// <returns>The <see cref="ParameterState{T}.Value"/> of the specified property.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the parameter state with <paramref name="propertyName"/> is not found.</exception>
    public static T? GetState<T>(this ComponentBaseWithState component, string propertyName)
    {
        if (component.ParameterContainer.TryGetValue(propertyName, out var lifeCycle))
        {
            if (lifeCycle is ParameterStateInternal<T> parameterState)
            {
                return parameterState.Value;
            }
        }

        throw new KeyNotFoundException($"ParameterState<{typeof(T).Name}> with {propertyName} was not found!");
    }

    private static string GetPropertyFromFuncLambda(string? propertyNameExpression)
    {
        ArgumentNullException.ThrowIfNull(propertyNameExpression);

        var lastDotIndex = propertyNameExpression.LastIndexOf('.');

        if (lastDotIndex != -1)
        {
            var propertyName = propertyNameExpression[(lastDotIndex + 1)..];

            return propertyName;
        }

        throw new ArgumentException($"Invalid property expression ({propertyNameExpression})!");
    }
}
