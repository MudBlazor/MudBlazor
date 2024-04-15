// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.State.Builder;

namespace MudBlazor;

#nullable enable
public class ComponentBaseWithState : ComponentBase
{
    internal readonly ParameterSet Parameters;
    private readonly ParameterRegistrationBuilderScope _scope;

    public ComponentBaseWithState()
    {
        _scope = new ParameterRegistrationBuilderScope(OnScopeEndedAction);
        Parameters = new ParameterSet(_scope);
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Parameters.OnInitialized();
    }

    /// <inheritdoc />
    public override Task SetParametersAsync(ParameterView parameters)
    {
        return Parameters.SetParametersAsync(base.SetParametersAsync, parameters);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Parameters.OnParametersSet();
    }

    /// <summary>
    /// Creates a scope for registering parameters.
    /// </summary>
    /// <returns>A <see cref="ParameterRegistrationBuilderScope"/> instance for registering parameters.</returns>
    internal IParameterRegistrationBuilderScope CreateRegisterScope()
    {
        if (_scope.IsLocked)
        {
            throw new InvalidOperationException($"You are not allowed to create more than one {nameof(CreateRegisterScope)} after the scope has ended!");
        }

        return _scope;
    }

    private void OnScopeEndedAction() => Parameters.ForceParametersAttachment();
}
