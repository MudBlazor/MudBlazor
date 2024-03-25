// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.State;

/// <summary>
/// Represents a read-only parameter state with a parameter value.
/// </summary>
/// <typeparam name="T">The type of the parameter.</typeparam>
internal class ReadOnlyParameterState<T> : IReadOnlyParameterState<T>
{
    /// <inheritdoc/>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyParameterState{T}"/> class with the specified value.
    /// </summary>
    /// <param name="value">The value of the parameter state.</param>
    public ReadOnlyParameterState(T value)
    {
        Value = value;
    }
}
