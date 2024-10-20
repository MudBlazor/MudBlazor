﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    internal readonly ParameterContainer ParameterContainer = new() { AutoVerify = false };

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        ParameterContainer.OnInitialized();
    }

    /// <inheritdoc />
    public override Task SetParametersAsync(ParameterView parameters)
    {
        return ParameterContainer.SetParametersAsync(base.SetParametersAsync, parameters);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ParameterContainer.OnParametersSet();
    }

    /// <summary>
    /// Creates a scope for registering parameters.
    /// </summary>
    /// <returns>A <see cref="ParameterRegistrationBuilderScope"/> instance for registering parameters.</returns>
    protected IParameterRegistrationBuilderScope CreateRegisterScope()
    {
        var processor = new ParameterRegistrationBuilderScope.ParameterStatesProcessor();
        var parameterScopeContainer = new ParameterScopeContainer(processor);
        var parameterRegistrationBuilderScope = new ParameterRegistrationBuilderScope(parameterScopeContainer, processor);
        ParameterContainer.Add(parameterScopeContainer);

        return parameterRegistrationBuilderScope;
    }
}
