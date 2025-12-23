// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable

/// <summary>
/// Represents a non-generic snapshot of a parameter's name, current value, and last value.
/// </summary>
/// <remarks>
/// This struct is used to pass parameter state information to shared change handlers
/// that need to coordinate changes across multiple parameters.
/// </remarks>
public readonly struct ParameterStateValue
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the current value of the parameter.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Gets the last value of the parameter before the change.
    /// </summary>
    public object? LastValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterStateValue"/> struct.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="lastValue">The last value of the parameter.</param>
    /// <param name="value">The current value of the parameter.</param>
    internal ParameterStateValue(string name, object? lastValue, object? value)
    {
        Name = name;
        LastValue = lastValue;
        Value = value;
    }
}

/// <summary>
/// A collection of parameter state values that allows efficient lookup by parameter name.
/// </summary>
/// <remarks>
/// This type is similar to <see cref="Microsoft.AspNetCore.Components.ParameterView"/> but for parameter state values.
/// It provides O(1) lookup performance for accessing parameter last values by name.
/// </remarks>
public readonly struct ParameterStateCollection
{
    private readonly ParameterStateValue[] _values;

    /// <summary>
    /// Gets an empty <see cref="ParameterStateCollection"/>.
    /// </summary>
    public static ParameterStateCollection Empty { get; } = new(Array.Empty<ParameterStateValue>());

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterStateCollection"/> struct.
    /// </summary>
    /// <param name="values">The array of parameter state values.</param>
    internal ParameterStateCollection(ParameterStateValue[] values)
    {
        _values = values;
    }

    /// <summary>
    /// Gets the number of parameter state values in the collection.
    /// </summary>
    public int Count => _values?.Length ?? 0;

    /// <summary>
    /// Attempts to get a parameter state value by its name.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">When this method returns, contains the parameter state value if found; otherwise, the default value.</param>
    /// <returns><c>true</c> if the parameter was found; otherwise, <c>false</c>.</returns>
    public bool TryGetValue(string parameterName, out ParameterStateValue value)
    {
        if (_values is not null)
        {
            foreach (var item in _values)
            {
                if (string.Equals(item.Name, parameterName, StringComparison.Ordinal))
                {
                    value = item;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to get a strongly-typed value for a parameter.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter value.</typeparam>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">When this method returns, contains the current value if found and successfully cast; otherwise, the default value.</param>
    /// <param name="lastValue">When this method returns, contains the last value if found and successfully cast; otherwise, the default value.</param>
    /// <returns><c>true</c> if the parameter was found; otherwise, <c>false</c>.</returns>
    public bool TryGetValue<T>(string parameterName, out T? value, out T? lastValue)
    {
        if (TryGetValue(parameterName, out var parameterState))
        {
            value = (T?)parameterState.Value;
            lastValue = (T?)parameterState.LastValue;
            return true;
        }

        value = default;
        lastValue = default;
        return false;
    }

    /// <summary>
    /// Gets a <see cref="ParameterStateEnumerator"/> that can be used to iterate over the parameter state values.
    /// </summary>
    /// <returns>A <see cref="ParameterStateEnumerator"/> for this collection.</returns>
    public ParameterStateEnumerator GetEnumerator() => new(_values);

    /// <summary>
    /// An enumerator that iterates through a <see cref="ParameterStateCollection"/>.
    /// </summary>
    public struct ParameterStateEnumerator
    {
        private readonly ParameterStateValue[] _values;
        private int _index;

        internal ParameterStateEnumerator(ParameterStateValue[] values)
        {
            _values = values;
            _index = -1;
        }

        /// <summary>
        /// Gets the current parameter state value.
        /// </summary>
        public ParameterStateValue Current => _values[_index];

        /// <summary>
        /// Advances the enumerator to the next parameter state value.
        /// </summary>
        /// <returns><c>true</c> if the enumerator successfully advanced; otherwise, <c>false</c>.</returns>
        public bool MoveNext()
        {
            _index++;
            return _index < (_values?.Length ?? 0);
        }
    }
}

/// <summary>
/// Combines <see cref="ParameterView"/> and <see cref="ParameterStateCollection"/> to provide
/// complete information about parameter changes including current values and last values.
/// </summary>
/// <remarks>
/// This type is passed to shared change handlers that need to coordinate changes across multiple parameters.
/// It provides access to both the current parameter values via <see cref="ParameterView"/> and 
/// the last values via <see cref="ParameterStateCollection"/>.
/// </remarks>
public readonly struct ParameterChangedContext
{
    /// <summary>
    /// Gets a snapshot of the component's <see cref="ParameterView"/> at the time the parameter change was detected.
    /// </summary>
    /// <remarks>
    /// Use this <see cref="ParameterView"/> to read current parameter values that were supplied together with the changed parameter.
    /// This snapshot reflects the raw parameter set Blazor provided during parameter assignment.
    /// </remarks>
    public ParameterView ParameterView { get; }

    /// <summary>
    /// Gets the collection of parameter state values containing last and current values.
    /// </summary>
    /// <remarks>
    /// Use this collection to access the last values of parameters before they changed.
    /// This is particularly useful for shared handlers that need to coordinate changes across multiple parameters.
    /// </remarks>
    public ParameterStateCollection ParameterStates { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterChangedContext"/> struct.
    /// </summary>
    /// <param name="parameterView">The parameter view snapshot.</param>
    /// <param name="parameterStates">The collection of parameter state values.</param>
    internal ParameterChangedContext(ParameterView parameterView, ParameterStateCollection parameterStates)
    {
        ParameterView = parameterView;
        ParameterStates = parameterStates;
    }

    /// <summary>
    /// Gets an empty <see cref="ParameterChangedContext"/>.
    /// </summary>
    public static ParameterChangedContext Empty { get; } = new(ParameterView.Empty, ParameterStateCollection.Empty);
}
