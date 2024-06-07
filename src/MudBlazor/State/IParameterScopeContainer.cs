// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor.State;

internal interface IParameterScopeContainer : IParameterContainer, IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the parameter registration builder scope is locked.
    /// </summary>
    /// <remarks>
    /// The scope becomes locked when it has been read or ended (<see cref="IDisposable.Dispose"/>), indicating that no more parameter states will be registered.
    /// </remarks>
    bool IsLocked { get; }
}
