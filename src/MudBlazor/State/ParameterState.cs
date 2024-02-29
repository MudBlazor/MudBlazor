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
