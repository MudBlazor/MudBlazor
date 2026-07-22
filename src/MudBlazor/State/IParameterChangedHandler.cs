// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents an interface for handling parameter change.
/// </summary>
/// <typeparam name="T">The type of the component's property value.</typeparam>
public interface IParameterChangedHandler<T>
{
    /// <summary>
    /// Handles parameter changes asynchronously.
    /// </summary>
    /// <param name="parameterChangedEventArgs">The <see cref="ParameterChangedEventArgs{T}"/> containing the information about the last and current values of a parameter.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleAsync(ParameterChangedEventArgs<T> parameterChangedEventArgs);
}
