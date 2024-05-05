using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents event arguments containing the last and current values of a parameter.
/// </summary>
/// <typeparam name="T">The type of the parameter value.</typeparam>
public class ParameterChangedEventArgs<T> : EventArgs
{
    /// <summary>
    /// Gets the associated parameter name of the component's <see cref="ParameterAttribute"/>.
    /// </summary>
    public string ParameterName { get; }

    /// <summary>
    /// Gets the last value of the parameter.
    /// </summary>
    public T LastValue { get; }

    /// <summary>
    /// Gets the current value of the parameter.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterChangedEventArgs{T}"/> class with the specified last and current values.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="lastValue">The last value of the parameter.</param>
    /// <param name="value">The current value of the parameter.</param>
    public ParameterChangedEventArgs(string parameterName, T lastValue, T value)
    {
        LastValue = lastValue;
        Value = value;
        ParameterName = parameterName;
    }
}
