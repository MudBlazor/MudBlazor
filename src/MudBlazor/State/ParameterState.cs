// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
internal class ParameterState
{
    public static ParameterState<T> Attach<T>(Func<T> getParameterValueFunc, EventCallback<T> eventCallback = default, bool fireOnSynchronize = false) => ParameterState<T>.Attach(getParameterValueFunc, eventCallback, fireOnSynchronize);
}
