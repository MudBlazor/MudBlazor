// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// A no-op reversible converter that returns inputs unchanged.
/// </summary>
/// <typeparam name="T">The type handled by the converter. Both forward and backward conversions operate on this type.</typeparam>
/// <remarks>
/// Use <see cref="EmptyConverter{T}"/> when no conversion is required but an <see cref="IReversibleConverter{T,T}"/>
/// instance is needed (for example as a default or placeholder converter). Both <see cref="Convert"/> and
/// <see cref="ConvertBack"/> simply return the supplied value.
/// </remarks>
public class EmptyConverter<T> : IReversibleConverter<T, T>
{
    /// <inheritdoc />
    public T Convert(T input) => input;

    /// <inheritdoc />
    public T ConvertBack(T output) => output;
}
