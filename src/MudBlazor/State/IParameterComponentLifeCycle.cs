// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
public interface IParameterComponentLifeCycle
{
    /// <summary>
    /// Gets the associated parameter name of the component's <see cref="ParameterAttribute"/>.
    /// </summary>
    string ParameterName { get; }


    bool HasHandler { get; }

    /// <summary>
    /// Checks if a parameter changed.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns><c>true</c> if the parameter value has changed, <c>false</c> otherwise.</returns>
    bool HasParameterChanged(ParameterView parameters);

    /// <summary>
    /// Called by the ParameterState framework. You shouldn't need to call this directly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ParameterChangeHandleAsync();

    /// <summary>
    /// Implements <see cref="IParameterComponentLifeCycle.OnInitialized"/>.
    /// Called by the ParameterState framework. You shouldn't need to call this directly.
    /// </summary>
    void OnInitialized();

    /// <summary>
    /// Implements <see cref="IParameterComponentLifeCycle.OnParametersSet"/>.
    /// Called by the ParameterState framework. You shouldn't need to call this directly.
    /// </summary>
    void OnParametersSet();
}
