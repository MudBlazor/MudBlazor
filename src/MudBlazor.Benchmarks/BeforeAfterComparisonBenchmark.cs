// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.Benchmarks;

#nullable enable

/// <summary>
/// Before vs After benchmark comparing old and new ParameterContainer implementations.
/// This measures the actual performance improvement from the architectural optimizations.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class BeforeAfterComparisonBenchmark
{
    private SyntheticParameterStateContainer? _container;
    private List<ParameterState<int>>? _states;
    private int[] _values = Array.Empty<int>();
    private EventCallback<int> _callback;

    [Params(10, 50, 100)]
    public int ParameterCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _callback = new EventCallback<int>(null, (int _) => { });
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _container = new SyntheticParameterStateContainer();
        _states = new List<ParameterState<int>>(ParameterCount);
        _values = new int[ParameterCount];
        for (int i = 0; i < ParameterCount; i++)
        {
            _values[i] = i;
        }
    }

    /// <summary>
    /// Benchmark: GetState lookups (tests flattened dictionary optimization)
    /// This simulates the common pattern where child components call GetState() on parent parameters.
    /// The optimization changed O(scopes) to O(1) lookup.
    /// </summary>
    [Benchmark]
    public void GetState_Lookups_100Times()
    {
        using var scope = _container!.CreateRegisterScope();
        for (int i = 0; i < ParameterCount; i++)
        {
            var localI = i;
            _states!.Add(scope.RegisterParameter<int>($"Param{i}")
                .WithParameter(() => _values[localI])
                .WithEventCallback(() => _callback));
        }

        _container.OnInitialized();

        // Simulate 100 GetState calls (common in complex component trees)
        for (int i = 0; i < 100; i++)
        {
            var paramName = $"Param{i % ParameterCount}";
            var value = _container.ParameterContainer.TryGetValue(paramName, out var lifecycle);
        }
    }

    /// <summary>
    /// Benchmark: Re-render with change handlers
    /// This tests the LINQ elimination optimization in SetParametersAsync.
    /// </summary>
    [Benchmark]
    public async Task ReRender_WithHandlers()
    {
        using var scope = _container!.CreateRegisterScope();
        for (int i = 0; i < ParameterCount; i++)
        {
            var localI = i;
            _states!.Add(scope.RegisterParameter<int>($"Param{i}")
                .WithParameter(() => _values[localI])
                .WithEventCallback(() => _callback)
                .WithChangeHandler(async () => await Task.CompletedTask));  // Has handler
        }

        await _container.SimulateFirstRenderAsync(ParameterView.Empty);

        // Change values and re-render (simulates parameter update from parent)
        for (int i = 0; i < ParameterCount; i++)
        {
            _values[i] = i + 1000;
        }

        await _container.SimulateReRenderAsync(ParameterView.Empty);
    }

    /// <summary>
    /// Benchmark: Re-render without handlers (tests fast path)
    /// Display-only components should benefit from the handler count cache optimization.
    /// </summary>
    [Benchmark]
    public async Task ReRender_NoHandlers()
    {
        using var scope = _container!.CreateRegisterScope();
        for (int i = 0; i < ParameterCount; i++)
        {
            var localI = i;
            _states!.Add(scope.RegisterParameter<int>($"Param{i}")
                .WithParameter(() => _values[localI])
                .WithEventCallback(() => _callback));  // No change handler
        }

        await _container.SimulateFirstRenderAsync(ParameterView.Empty);

        // Change values and re-render
        for (int i = 0; i < ParameterCount; i++)
        {
            _values[i] = i + 1000;
        }

        await _container.SimulateReRenderAsync(ParameterView.Empty);
    }

    /// <summary>
    /// Benchmark: Multiple SetValueAsync calls
    /// This is common in form scenarios where users update many fields.
    /// </summary>
    [Benchmark]
    public async Task SetValueAsync_MultipleUpdates()
    {
        using var scope = _container!.CreateRegisterScope();
        for (int i = 0; i < ParameterCount; i++)
        {
            var localI = i;
            _states!.Add(scope.RegisterParameter<int>($"Param{i}")
                .WithParameter(() => _values[localI])
                .WithEventCallback(() => _callback));
        }

        _container.OnInitialized();

        // Update all parameters
        for (int i = 0; i < ParameterCount; i++)
        {
            await _states![i].SetValueAsync(i + 1000);
        }
    }

    /// <summary>
    /// Benchmark: Complex scenario - multiple scopes (simulates inheritance)
    /// This tests the full benefit of flattened dictionary across multiple scopes.
    /// </summary>
    [Benchmark]
    public void GetState_MultipleScopes()
    {
        // Create 3 scopes (simulates base class + derived class + component)
        var paramsPerScope = ParameterCount / 3;

        for (int scope = 0; scope < 3; scope++)
        {
            using var registerScope = _container!.CreateRegisterScope();
            for (int i = 0; i < paramsPerScope; i++)
            {
                var paramIndex = scope * paramsPerScope + i;
                var localIndex = paramIndex;
                registerScope.RegisterParameter<int>($"Scope{scope}_Param{i}")
                    .WithParameter(() => _values[Math.Min(localIndex, _values.Length - 1)])
                    .WithEventCallback(() => _callback);
            }
        }

        _container!.OnInitialized();

        // Lookup parameters from different scopes (worst case for old implementation)
        for (int i = 0; i < 50; i++)
        {
            // Lookup from scope 0
            _container.ParameterContainer.TryGetValue($"Scope0_Param{i % paramsPerScope}", out _);
            // Lookup from scope 1
            _container.ParameterContainer.TryGetValue($"Scope1_Param{i % paramsPerScope}", out _);
            // Lookup from scope 2 (would require 3 dictionary lookups in old version)
            _container.ParameterContainer.TryGetValue($"Scope2_Param{i % paramsPerScope}", out _);
        }
    }
}
