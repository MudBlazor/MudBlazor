using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
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
    /// <returns>The read-only parameter state of the specified property.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyExpression"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="propertyExpression"/> does not represent a property.</exception>
    public static IReadOnlyParameterState<T> GetSate<TComponent, T>(this TComponent component, Expression<Func<TComponent, T>> propertyExpression) where TComponent : MudComponentBase
    {
        var propertyName = GetPropertyName(propertyExpression);

        return GetSate<TComponent, T>(component, propertyName);
    }

    /// <summary>
    /// Gets the read-only parameter state of a specified property in the component.
    /// </summary>
    /// <typeparam name="TComponent">The type of the MudBlazor component.</typeparam>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="component">The MudBlazor component instance.</param>
    /// <param name="propertyName">The name of the property whose parameter state needs to be accessed.</param>
    /// <returns>The read-only parameter state of the specified property.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the parameter state with <paramref name="propertyName"/> is not found.</exception>
    public static IReadOnlyParameterState<T> GetSate<TComponent, T>(this TComponent component, string propertyName) where TComponent : MudComponentBase
    {
        if (component.Parameters.TryGetValue(propertyName, out var lifeCycle))
        {
            var parameterState = Unsafe.As<IReadOnlyParameterState<T>>(lifeCycle);

            return parameterState;
        }

        throw new KeyNotFoundException($"ParameterState with {propertyName} was not found!");
    }

    private static string GetPropertyName<TComponent, T>(Expression<Func<TComponent, T>> propertyExpression) where TComponent : MudComponentBase
    {
        ArgumentNullException.ThrowIfNull(nameof(propertyExpression));

        if (propertyExpression.Body is not MemberExpression body)
        {
            throw new ArgumentException(@"Invalid argument", nameof(propertyExpression));
        }

        if (body.Member is not PropertyInfo property)
        {
            throw new ArgumentException(@"Argument is not a property", nameof(propertyExpression));
        }

        return property.Name;
    }
}
