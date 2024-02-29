﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace MudBlazor.State;

#nullable enable
internal interface IParameterChangedHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task HandleAsync();
}
