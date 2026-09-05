// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.State.Builder;

namespace MudBlazor;

/// <summary>
/// Base class for Blazor components that track parameter changes and manage state through MudBlazor's parameter framework, such as <see cref="MudComponentBase"/>.
/// </summary>
public class ComponentBaseWithState : ComponentBase
{
    private ParameterContainer? _parameterContainer;

    /// <summary>
    /// The registered parameter states, created on first registration.
    /// </summary>
    /// <remarks>
    /// Many components register nothing, so the container is not allocated until <see cref="CreateRegisterScope"/> runs.
    /// </remarks>
    internal ParameterContainer ParameterContainer => _parameterContainer ??= new ParameterContainer { AutoVerify = false };

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _parameterContainer?.OnInitialized();
    }

    /// <inheritdoc />
    public override Task SetParametersAsync(ParameterView parameters)
    {
        return _parameterContainer is null
            ? base.SetParametersAsync(parameters)
            : _parameterContainer.SetParametersAsync(base.SetParametersAsync, parameters);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _parameterContainer?.OnParametersSet();
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
