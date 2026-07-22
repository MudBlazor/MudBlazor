// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.Benchmarks;

/// <summary>
/// Benchmarks for large-scale ParameterState scenarios with many parameters.
/// Tests performance with 100, 1000, and 10000 parameters.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class ParameterStateLargeScaleBenchmark
{
    private SyntheticParameterStateContainer? _container;
    private List<ParameterState<int>>? _states;
    private int[] _values = Array.Empty<int>();
    private EventCallback<int> _callback;

    [Params(100, 1000)]
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
    /// Benchmark: Register N parameters
    /// </summary>
    [Benchmark]
    public void RegisterManyParameters()
    {
        using var scope = _container!.CreateRegisterScope();
        for (int i = 0; i < ParameterCount; i++)
        {
            var localI = i;
            _states!.Add(scope.RegisterParameter<int>($"Param{i}")
                .WithParameter(() => _values[localI])
                .WithEventCallback(() => _callback));
        }
    }

    /// <summary>
    /// Benchmark: Full lifecycle with N parameters
    /// </summary>
    [Benchmark]
    public async Task FullLifecycleManyParameters()
    {
        using var scope = _container!.CreateRegisterScope();
        for (int i = 0; i < ParameterCount; i++)
        {
            var localI = i;
            _states!.Add(scope.RegisterParameter<int>($"Param{i}")
                .WithParameter(() => _values[localI])
                .WithEventCallback(() => _callback));
        }

        var parameterView = ParameterView.Empty;
        await _container.SimulateLifecycleAsync(parameterView);
    }

    /// <summary>
    /// Benchmark: OnParametersSet with N parameters (no changes)
    /// </summary>
    [Benchmark]
    public void OnParametersSet_NoChanges()
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
        _container.OnParametersSet(); // No values changed
    }

    /// <summary>
    /// Benchmark: OnParametersSet with all N parameters changed
    /// </summary>
    [Benchmark]
    public void OnParametersSet_AllChanged()
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

        // Change all values
        for (int i = 0; i < ParameterCount; i++)
        {
            _values[i] = i + 1000;
        }

        _container.OnParametersSet();
    }

    /// <summary>
    /// Benchmark: SetValueAsync on all N parameters
    /// </summary>
    [Benchmark]
    public async Task SetValueAsync_AllParameters()
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

        for (int i = 0; i < ParameterCount; i++)
        {
            await _states![i].SetValueAsync(i + 1000);
        }
    }
}
