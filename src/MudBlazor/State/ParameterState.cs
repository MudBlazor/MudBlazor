// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
internal class ParameterState
{
    /// <summary>
    /// Create a ParameterState&lt;T&gt; object which automatically manages parameter value changes as part of
    /// MudBlazor's ParameterState framework. For details and usage please read CONTRIBUTING.md
    ///
    /// Note: usually you don't need to call this directly. Instead use the RegisterParameter method (<see cref="MudComponentBase"/>) from within the
    /// component's constructor.  
    /// </summary>
    /// <param name="parameterName">pass the parameter name using nameof(...)</param>
    /// <param name="getParameterValueFunc">a get func that allows ParameterState to read the property value</param>
    /// <param name="eventCallbackFunc">a get func that allows ParameterState to get the EventCallback of the parameter</param>
    /// <param name="parameterChangedHandler">
    ///     a change handler containing code that needs to be executed when the parameter value changes
    /// </param>
    /// <typeparam name="T">The type of the property value</typeparam>
    /// <returns>The ParameterState object to be stored in a field for accessing the current value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Action parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, eventCallbackFunc, new ParameterChangedLambdaHandler(parameterChangedHandler));
    }

    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Func<Task> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, eventCallbackFunc, new ParameterChangedLambdaTaskHandler(parameterChangedHandler));
    }

    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, eventCallbackFunc);
    }

    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Action parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, ()=> default, new ParameterChangedLambdaHandler(parameterChangedHandler));
    }

    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<Task> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, () => default, new ParameterChangedLambdaTaskHandler(parameterChangedHandler));
    }
}
