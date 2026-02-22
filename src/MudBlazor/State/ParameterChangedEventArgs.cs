using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

/// <summary>
/// Represents event arguments containing the last and current values of a parameter.
/// </summary>
/// <typeparam name="T">The type of the parameter value.</typeparam>
public class ParameterChangedEventArgs<T> : EventArgs
{
    /// <summary>
    /// Gets a snapshot of the component's <see cref="ParameterView"/> at the time the parameter change was detected.
    /// Use this <see cref="ParameterView"/> to read other parameters that were supplied together with the changed parameter.
    /// This snapshot reflects the raw parameter set Blazor provided during
    /// parameter assignment and should be preferred by handlers that need access to related parameter values instead of
    /// relying on the component's current property values which may not yet be updated.
    /// </summary>
    public ParameterView ParameterView { get; }

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
    /// Gets a value indicating whether the change was originated by the child,
    /// meaning the change was propagated from the child to the parent. 
    /// This property is used to track whether the parent received the update 
    /// as a result of the child triggering the change or updating its own state.
    /// </summary>
    public bool IsChildOriginatedChange { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterChangedEventArgs{T}"/> class with the specified last and current values.
    /// </summary>
    /// <param name="parameterView">A snapshot of the component's <see cref="ParameterView"/> as provided by Blazor when parameters were set.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="lastValue">The last value of the parameter.</param>
    /// <param name="value">The current value of the parameter.</param>
    public ParameterChangedEventArgs(ParameterView parameterView, string parameterName, T lastValue, T value)
    {
        ParameterView = parameterView;
        LastValue = lastValue;
        Value = value;
        ParameterName = parameterName;
    }

    internal ParameterChangedEventArgs<T> ChildOriginated(bool ishildOriginatedChange)
    {
        IsChildOriginatedChange = ishildOriginatedChange;

        return this;
    }

    internal ParameterChangedEventArgs<T> Clone()
    {
        return new ParameterChangedEventArgs<T>(ParameterView, ParameterName, LastValue, Value)
        {
            IsChildOriginatedChange = IsChildOriginatedChange
        };
    }
}
