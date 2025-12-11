// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.State.Builder;

namespace MudBlazor.Benchmarks;

#nullable enable

/// <summary>
/// A synthetic container that simulates Blazor component lifecycle for ParameterState benchmarking.
/// This does NOT depend on the Blazor runtime and manually drives lifecycle phases.
/// </summary>
/// <remarks>
/// Blazor lifecycle (https://blazor-university.com/components/component-lifecycles/):
/// 
/// For NEW instances:
/// 1. SetParametersAsync - receives parameters, calls base.SetParametersAsync which assigns [Parameter] properties
/// 2. OnInitialized - called once after parameters are assigned (only for new instances)
/// 3. OnParametersSet - called after OnInitialized
/// 
/// For RE-RENDERS (existing instances):
/// 1. SetParametersAsync - receives parameters, calls base.SetParametersAsync which assigns [Parameter] properties  
/// 2. OnParametersSet - called immediately (OnInitialized is skipped)
/// 
/// This container simulates both scenarios to stress-test ParameterState performance.
/// </remarks>
public class SyntheticParameterStateContainer
{
    private readonly ParameterContainer _parameterContainer = new() { AutoVerify = false };
    private bool _isFirstRender = true;

    /// <summary>
    /// Exposes the internal ParameterContainer for benchmarking direct lookups.
    /// </summary>
    internal ParameterContainer ParameterContainer => _parameterContainer;

    /// <summary>
    /// Creates a scope for registering parameters, similar to ComponentBaseWithState.
    /// </summary>
    public IParameterRegistrationBuilderScope CreateRegisterScope()
    {
        var processor = new ParameterRegistrationBuilderScope.ParameterStatesProcessor();
        var parameterScopeContainer = new ParameterScopeContainer(processor);
        var parameterRegistrationBuilderScope = new ParameterRegistrationBuilderScope(parameterScopeContainer, processor);
        _parameterContainer.Add(parameterScopeContainer);

        return parameterRegistrationBuilderScope;
    }

    /// <summary>
    /// Simulates the complete Blazor lifecycle for a new component instance.
    /// Order: SetParametersAsync -> (assigns parameters) -> OnInitialized -> OnParametersSet
    /// </summary>
    public async Task SimulateFirstRenderAsync(ParameterView parameters)
    {
        if (!_isFirstRender)
        {
            throw new InvalidOperationException("SimulateFirstRenderAsync can only be called once. Use SimulateReRenderAsync for subsequent renders.");
        }

        // SetParametersAsync with base.SetParametersAsync behavior
        await _parameterContainer.SetParametersAsync(
            async (p) =>
            {
                // base.SetParametersAsync assigns [Parameter] properties here
                // Then triggers OnInitialized (first render only) and OnParametersSet

                // OnInitialized - only on first render
                _parameterContainer.OnInitialized();
                _isFirstRender = false;

                // OnParametersSet - after OnInitialized on first render
                _parameterContainer.OnParametersSet();

                await Task.CompletedTask;
            },
            parameters);
    }

    /// <summary>
    /// Simulates the Blazor lifecycle for a re-render (existing component).
    /// Order: SetParametersAsync -> (assigns parameters) -> OnParametersSet
    /// Note: OnInitialized is NOT called on re-renders.
    /// </summary>
    public async Task SimulateReRenderAsync(ParameterView parameters)
    {
        if (_isFirstRender)
        {
            throw new InvalidOperationException("SimulateReRenderAsync can only be called after SimulateFirstRenderAsync. Use SimulateFirstRenderAsync for the initial render.");
        }

        // SetParametersAsync with base.SetParametersAsync behavior
        await _parameterContainer.SetParametersAsync(
            async (p) =>
            {
                // base.SetParametersAsync assigns [Parameter] properties here
                // OnInitialized is skipped on re-renders

                // OnParametersSet - called directly on re-renders
                _parameterContainer.OnParametersSet();

                await Task.CompletedTask;
            },
            parameters);
    }

    /// <summary>
    /// Simulates the Blazor lifecycle automatically (first render or re-render based on state).
    /// </summary>
    public async Task SimulateLifecycleAsync(ParameterView parameters)
    {
        if (_isFirstRender)
        {
            await SimulateFirstRenderAsync(parameters);
        }
        else
        {
            await SimulateReRenderAsync(parameters);
        }
    }

    /// <summary>
    /// Manually triggers OnInitialized (for testing scenarios where you control lifecycle directly)
    /// </summary>
    public void OnInitialized()
    {
        _parameterContainer.OnInitialized();
        _isFirstRender = false;
    }

    /// <summary>
    /// Manually triggers OnParametersSet (for testing scenarios where you control lifecycle directly)
    /// </summary>
    public void OnParametersSet()
    {
        _parameterContainer.OnParametersSet();
    }

    /// <summary>
    /// Manually triggers SetParametersAsync (for testing scenarios where you control lifecycle directly)
    /// </summary>
    public Task SetParametersAsync(ParameterView parameters)
    {
        return _parameterContainer.SetParametersAsync(_ => Task.CompletedTask, parameters);
    }
}
