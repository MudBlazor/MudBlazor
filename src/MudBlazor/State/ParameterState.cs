// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable

/// <summary>
/// Helper class for creating <see cref="ParameterState{T}"/> object with different overloads of the <see cref="ParameterState{T}.Attach"/> method.
/// </summary>
internal class ParameterState
{
    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Action parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, eventCallbackFunc, new ParameterChangedLambdaHandler(parameterChangedHandler));
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Func<Task> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, eventCallbackFunc, new ParameterChangedLambdaTaskHandler(parameterChangedHandler));
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, eventCallbackFunc);
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Action parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, ()=> default, new ParameterChangedLambdaHandler(parameterChangedHandler));
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<Task> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, () => default, new ParameterChangedLambdaTaskHandler(parameterChangedHandler));
    }
}
