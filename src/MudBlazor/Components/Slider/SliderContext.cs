// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable

/// <summary>
/// The current state of a <see cref="MudSlider{T}"/> component, containing both the value and nullable value.
/// </summary>
/// <remarks>
/// This state is a cascading parameter for <see cref="MudSlider{T}"/> components.
/// </remarks>
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
