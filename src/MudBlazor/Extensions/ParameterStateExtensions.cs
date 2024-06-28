// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MudBlazor.State;

namespace MudBlazor.Extensions;

#nullable enable
/// <summary>
/// Provides extension methods for <see cref="ParameterState{T}"/>.
/// </summary>
internal static class ParameterStateExtensions
{
    /// <summary>
    /// Converts the specified <see cref="ParameterState{T}"/> to an <see cref="IParameterStateUnsafe{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="parameterState">The parameter state to convert.</param>
    /// <returns>The <see cref="IParameterStateUnsafe{T}"/> interface for the specified parameter state.</returns>
    public static IParameterStateUnsafe<T> AsUnsafe<T>(this ParameterState<T> parameterState) => (IParameterStateUnsafe<T>)parameterState;

    /// <summary>
    /// Unlike the <see cref="ParameterState{T}.SetValueAsync"/> sets the value of the parameter state after invoking the event callback asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="parameterState">The parameter state to set the value for.</param>
    /// <param name="value">The new value for the parameter state.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task SetValueAfterEventCallbackAsync<T>(this ParameterState<T> parameterState, T value)
    {
        var parameterStateUnsafe = parameterState.AsUnsafe();
        if (!parameterStateUnsafe.Comparer.Equals(parameterState.Value, value))
        {
            var eventCallback = parameterStateUnsafe.ValueChanged;
            if (eventCallback.HasDelegate)
            {
                await eventCallback.InvokeAsync(value);
            }

            parameterStateUnsafe.Value = value;
        }
    }
}
