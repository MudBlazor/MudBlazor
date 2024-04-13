// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.State.Builder;

#nullable enable
/// <summary>
/// Helper class for creating instances of <see cref="ParameterAttachBuilder{T}"/>.
/// </summary>
/// <remarks>
/// You don't need to create this object directly.
/// Instead, use the "MudComponentBase.RegisterParameter" method from within the component's constructor.
/// </remarks>
internal class ParameterAttachBuilder
{
    /// <summary>
    /// Creates a new instance of the <see cref="ParameterAttachBuilder{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <returns>A new instance of <see cref="ParameterAttachBuilder{T}"/>.</returns>
    public static ParameterAttachBuilder<T> Create<T>() => new();
}
