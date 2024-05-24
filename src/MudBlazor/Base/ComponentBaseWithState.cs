// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.State.Builder;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents a base class for designing components which maintain state.
/// </summary>
public class ComponentBaseWithState : ComponentBase
{
    internal readonly ParameterSetUnion ParameterSetUnion = new();

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        ParameterSetUnion.OnInitialized();
    }

    /// <inheritdoc />
    public override Task SetParametersAsync(ParameterView parameters)
    {
        return ParameterSetUnion.SetParametersAsync(base.SetParametersAsync, parameters);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ParameterSetUnion.OnParametersSet();
    }

    /// <summary>
    /// Creates a scope for registering parameters.
    /// </summary>
    /// <returns>A <see cref="ParameterRegistrationBuilderScope"/> instance for registering parameters.</returns>
    protected IParameterRegistrationBuilderScope CreateRegisterScope()
    {
        var parameterRegistrationBuilderScope = new ParameterRegistrationBuilderScope(OnScopeEndedAction);
        var parameterSet = new ParameterSet(parameterRegistrationBuilderScope);
        ParameterSetUnion.Add(parameterSet);

        return parameterRegistrationBuilderScope;

        void OnScopeEndedAction(IParameterStatesReaderOwner? owner) => owner?.ForceParametersAttachment();
    }
}
