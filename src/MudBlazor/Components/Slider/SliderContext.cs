// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents the context of a slider component, containing both the value and nullable value of the slider.
/// </summary>
/// <typeparam name="T">The type of the value the slider represents.</typeparam>
public class SliderContext<T> where T : struct
{
    /// <summary>
    /// The value of the slider.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// The nullable value of the slider.
    /// </summary>
    public T? NullableValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SliderContext{T}"/> class with the specified value and nullable value.
    /// </summary>
    /// <param name="value">The value of the slider.</param>
    /// <param name="nullableValue">The nullable value of the slider.</param>
    public SliderContext(T value, T? nullableValue)
    {
        NullableValue = nullableValue;
        Value = value;
    }
}
