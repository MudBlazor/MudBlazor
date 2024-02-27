// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
internal class StateManager
{
    public static StateManager<T> Attach<T>(Func<T> parameterState, EventCallback<T> eventCallback = default, bool fireOnSynchronize = false) => StateManager<T>.Attach(parameterState, eventCallback, fireOnSynchronize);
}
