﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MudBlazor.State;

namespace MudBlazor.Extensions;

#nullable enable
/// <summary>
/// Provides extension methods for MudBlazor components to facilitate accessing parameter states.
/// </summary>
internal static class MudComponentExtensions
{
    /// <summary>
    /// Gets the read-only parameter state of a specified property in the component.
    /// </summary>
    /// <typeparam name="TComponent">The type of the MudBlazor component.</typeparam>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="component">The MudBlazor component instance.</param>
    /// <param name="propertyExpression">An expression representing the property whose parameter state needs to be accessed.</param>
    /// <param name="propertyNameCallerArgumentExpression">The property name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The <see cref="IParameterState{T}.Value"/> of the specified property.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyExpression"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="propertyExpression"/> does not represent a property.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the parameter state with <paramref name="propertyExpression"/> is not found.</exception>
    public static T? GetSate<TComponent, T>(this TComponent component, Func<TComponent, T> propertyExpression, [CallerArgumentExpression(nameof(propertyExpression))] string? propertyNameCallerArgumentExpression = null) where TComponent : MudComponentBase
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        var propertyName = GetPropertyFromFuncLambda(propertyNameCallerArgumentExpression);

        return GetSate<TComponent, T>(component, propertyName);
    }

    /// <summary>
    /// Gets the read-only parameter state of a specified property in the component.
    /// </summary>
    /// <typeparam name="TComponent">The type of the MudBlazor component.</typeparam>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="component">The MudBlazor component instance.</param>
    /// <param name="propertyName">The name of the property whose parameter state needs to be accessed. Use nameof(...) to get the property name.</param>
    /// <returns>The <see cref="IParameterState{T}.Value"/> of the specified property.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the parameter state with <paramref name="propertyName"/> is not found.</exception>
    public static T? GetSate<TComponent, T>(this TComponent component, string propertyName) where TComponent : MudComponentBase
    {
        if (component.Parameters.TryGetValue(propertyName, out var lifeCycle))
        {
            if (lifeCycle is ParameterState<T> parameterState)
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

            return propertyName[(lastDotIndex + 1)..];
        }

        throw new ArgumentException("Invalid property expression ()!");
    }
}
