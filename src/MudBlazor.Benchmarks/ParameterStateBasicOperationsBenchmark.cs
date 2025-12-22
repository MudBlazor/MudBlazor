// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.Benchmarks;

/// <summary>
/// Benchmarks for basic ParameterState operations: registration, initialization, and value updates.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class ParameterStateBasicOperationsBenchmark
{
    private SyntheticParameterStateContainer? _container;
    private ParameterState<int>? _intState;
    private int _intValue;
    private string _stringValue = "test";
    private EventCallback<int> _intCallback;
    private EventCallback<string> _stringCallback;

    [GlobalSetup]
    public void Setup()
    {
        _intCallback = new EventCallback<int>(null, (int _) => { });
        _stringCallback = new EventCallback<string>(null, (string _) => { });
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _container = new SyntheticParameterStateContainer();
        _intValue = 42;
        _stringValue = "test";
    }

    /// <summary>
    /// Benchmark: Register a single int parameter
    /// </summary>
    [Benchmark]
    public ParameterState<int> RegisterSingleIntParameter()
    {
        using var scope = _container!.CreateRegisterScope();
        return scope.RegisterParameter<int>(nameof(_intValue))
            .WithParameter(() => _intValue)
            .WithEventCallback(() => _intCallback);
    }

    /// <summary>
    /// Benchmark: Register a single string parameter
    /// </summary>
    [Benchmark]
    public ParameterState<string> RegisterSingleStringParameter()
    {
        using var scope = _container!.CreateRegisterScope();
        return scope.RegisterParameter<string>(nameof(_stringValue))
            .WithParameter(() => _stringValue)
            .WithEventCallback(() => _stringCallback);
    }

    /// <summary>
    /// Benchmark: Register multiple parameters (10 parameters)
    /// </summary>
    [Benchmark]
    public void RegisterTenParameters()
    {
        using var scope = _container!.CreateRegisterScope();
        for (int i = 0; i < 10; i++)
        {
            var localI = i;
            scope.RegisterParameter<int>($"Param{i}")
                .WithParameter(() => localI)
                .WithEventCallback(() => _intCallback);
        }
    }

    /// <summary>
    /// Benchmark: Full lifecycle with single parameter
    /// </summary>
    [Benchmark]
    public async Task FullLifecycleSingleParameter()
    {
        using var scope = _container!.CreateRegisterScope();
        _intState = scope.RegisterParameter<int>(nameof(_intValue))
            .WithParameter(() => _intValue)
            .WithEventCallback(() => _intCallback);

        var parameterView = ParameterView.Empty;
        await _container.SimulateLifecycleAsync(parameterView);
    }

    /// <summary>
    /// Benchmark: SetValueAsync on initialized parameter
    /// </summary>
    [Benchmark]
    public async Task SetValueAsync_NoCallback()
    {
        using var scope = _container!.CreateRegisterScope();
        _intState = scope.RegisterParameter<int>(nameof(_intValue))
            .WithParameter(() => _intValue);

        _container.OnInitialized();
        await _intState.SetValueAsync(100);
    }

    /// <summary>
    /// Benchmark: SetValueAsync with callback
    /// </summary>
    [Benchmark]
    public async Task SetValueAsync_WithCallback()
    {
        using var scope = _container!.CreateRegisterScope();
        _intState = scope.RegisterParameter<int>(nameof(_intValue))
            .WithParameter(() => _intValue)
            .WithEventCallback(() => _intCallback);

        _container.OnInitialized();
        await _intState.SetValueAsync(100);
    }

    /// <summary>
    /// Benchmark: SetValueAsync with same value (should skip callback)
    /// </summary>
    [Benchmark]
    public async Task SetValueAsync_SameValue()
    {
        using var scope = _container!.CreateRegisterScope();
        _intState = scope.RegisterParameter<int>(nameof(_intValue))
            .WithParameter(() => _intValue)
            .WithEventCallback(() => _intCallback);

        _container.OnInitialized();
        await _intState.SetValueAsync(_intValue); // Same as current value
    }

    /// <summary>
    /// Benchmark: Repeated SetValueAsync calls
    /// </summary>
    [Benchmark]
    public async Task SetValueAsync_Repeated100Times()
    {
        using var scope = _container!.CreateRegisterScope();
        _intState = scope.RegisterParameter<int>(nameof(_intValue))
            .WithParameter(() => _intValue)
            .WithEventCallback(() => _intCallback);

        _container.OnInitialized();
        for (int i = 0; i < 100; i++)
        {
            await _intState.SetValueAsync(i);
        }
    }
}
