// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents the lifecycle methods for Blazor component parameters used by the ParameterState framework.
/// </summary>
internal interface IParameterComponentLifeCycle
{
    /// <summary>
    /// Gets metadata associated with the parameter, including its name, handler name etc.
    /// </summary>
    ParameterMetadata Metadata { get; }

    /// <summary>
    /// Indicates whether a <see cref="IParameterChangedHandler{T}"/> is supplied for handling parameter changes.
    /// </summary>
    bool HasHandler { get; }

    /// <summary>
    /// Determines whether the parameter has changed by comparing it with the provided parameters.
    /// </summary>
    /// <param name="parameters">The parameter view containing the parameters to compare with.</param>
    /// <returns><c>true</c> if the parameter value has changed, <c>false</c> otherwise.</returns>
    bool HasParameterChanged(ParameterView parameters);

    /// <summary>
    /// Called by the <see cref="ParameterState{T}"/> framework when <see cref="IParameterChangedHandler{T}"/> is supplied.
    /// </summary>
    /// <remarks>
    /// This method is intended for internal use and is controlled by the <see cref="MudComponentBase"/> and <see cref="ParameterScopeContainer"/>.
    /// It should only be invoked after <see cref="HasParameterChanged"/> has been called.
    /// Direct invocation of this method by external code is discouraged.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ParameterChangeHandleAsync();

    /// <summary>
    /// Invoked when <see cref="ComponentBase.OnInitialized"/> is called, used to set the initial parameter value.
    /// </summary>
    /// <remarks>
    /// This method is intended for internal use and is controlled by the <see cref="MudComponentBase"/> and <see cref="ParameterScopeContainer"/>.
    /// Direct invocation of this method by external code is discouraged.
    /// </remarks>
    void OnInitialized();

    /// <summary>
    /// Invoked when <see cref="ComponentBase.OnParametersSet"/> is called, used to synchronize the parameter value when Blazor updates the parameters.
    /// </summary>
    /// <remarks>
    /// This method is intended for internal use and is controlled by the <see cref="MudComponentBase"/> and <see cref="ParameterScopeContainer"/>.
    /// Direct invocation of this method by external code is discouraged.
    /// </remarks>
    void OnParametersSet();
}
