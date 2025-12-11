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
/// Benchmarks for ParameterState with different comparer strategies.
/// Tests performance impact of custom comparers, static vs dynamic comparers, and equality checks.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class ParameterStateComparerBenchmark
{
    private SyntheticParameterStateContainer? _container;
    private ParameterState<string>? _stringState;
    private ParameterState<List<int>>? _listState;
    private ParameterState<ComplexObject>? _complexState;
    private string _stringValue = "test";
    private List<int> _listValue = new() { 1, 2, 3 };
    private ComplexObject _complexValue = new();
    private EventCallback<string> _stringCallback;
    private EventCallback<List<int>> _listCallback;
    private EventCallback<ComplexObject> _complexCallback;

    // Custom comparers
    private readonly StringComparer _customStringComparer = StringComparer.OrdinalIgnoreCase;
    private readonly CustomListComparer _customListComparer = new();
    private readonly ComplexObjectComparer _complexObjectComparer = new();

    public class ComplexObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = "default";
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public class CustomListComparer : IEqualityComparer<List<int>>
    {
        public bool Equals(List<int>? x, List<int>? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            if (x.Count != y.Count) return false;
            for (int i = 0; i < x.Count; i++)
            {
                if (x[i] != y[i]) return false;
            }
            return true;
        }

        public int GetHashCode(List<int> obj) => obj.Count;
    }

    public class ComplexObjectComparer : IEqualityComparer<ComplexObject>
    {
        public bool Equals(ComplexObject? x, ComplexObject? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.Id == y.Id && x.Name == y.Name && x.Timestamp == y.Timestamp;
        }

        public int GetHashCode(ComplexObject obj) => HashCode.Combine(obj.Id, obj.Name, obj.Timestamp);
    }

    [GlobalSetup]
    public void Setup()
    {
        _stringCallback = new EventCallback<string>(null, (string _) => { });
        _listCallback = new EventCallback<List<int>>(null, (List<int> _) => { });
        _complexCallback = new EventCallback<ComplexObject>(null, (ComplexObject _) => { });
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _container = new SyntheticParameterStateContainer();
        _stringValue = "test";
        _listValue = new() { 1, 2, 3 };
        _complexValue = new ComplexObject { Id = 1, Name = "test", Timestamp = DateTime.Now };
    }

    /// <summary>
    /// Baseline: String parameter with default comparer
    /// </summary>
    [Benchmark(Baseline = true)]
    public async Task StringParameter_DefaultComparer()
    {
        using var scope = _container!.CreateRegisterScope();
        _stringState = scope.RegisterParameter<string>(nameof(_stringValue))
            .WithParameter(() => _stringValue)
            .WithEventCallback(() => _stringCallback);

        await _container.SimulateFirstRenderAsync(ParameterView.Empty);
        await _stringState.SetValueAsync("new value");
    }

    /// <summary>
    /// String parameter with custom static comparer
    /// </summary>
    [Benchmark]
    public async Task StringParameter_CustomStaticComparer()
    {
        using var scope = _container!.CreateRegisterScope();
        _stringState = scope.RegisterParameter<string>(nameof(_stringValue))
            .WithParameter(() => _stringValue)
            .WithEventCallback(() => _stringCallback)
            .WithComparer(_customStringComparer);

        await _container.SimulateFirstRenderAsync(ParameterView.Empty);
        await _stringState.SetValueAsync("new value");
    }

    /// <summary>
    /// String parameter with dynamic comparer (lambda)
    /// </summary>
    [Benchmark]
    public async Task StringParameter_DynamicComparer()
    {
        using var scope = _container!.CreateRegisterScope();
        _stringState = scope.RegisterParameter<string>(nameof(_stringValue))
            .WithParameter(() => _stringValue)
            .WithEventCallback(() => _stringCallback)
            .WithComparer(() => StringComparer.OrdinalIgnoreCase);

        await _container.SimulateFirstRenderAsync(ParameterView.Empty);
        await _stringState.SetValueAsync("new value");
    }

    /// <summary>
    /// List parameter with default comparer (reference equality)
    /// </summary>
    [Benchmark]
    public async Task ListParameter_DefaultComparer()
    {
        using var scope = _container!.CreateRegisterScope();
        _listState = scope.RegisterParameter<List<int>>(nameof(_listValue))
            .WithParameter(() => _listValue)
            .WithEventCallback(() => _listCallback);

        await _container.SimulateFirstRenderAsync(ParameterView.Empty);
        await _listState.SetValueAsync(new List<int> { 1, 2, 3 });
    }

    /// <summary>
    /// List parameter with custom element-wise comparer
    /// </summary>
    [Benchmark]
    public async Task ListParameter_CustomComparer()
    {
        using var scope = _container!.CreateRegisterScope();
        _listState = scope.RegisterParameter<List<int>>(nameof(_listValue))
            .WithParameter(() => _listValue)
            .WithEventCallback(() => _listCallback)
            .WithComparer(_customListComparer);

        await _container.SimulateFirstRenderAsync(ParameterView.Empty);
        await _listState.SetValueAsync(new List<int> { 1, 2, 3 });
    }

    /// <summary>
    /// Complex object with default comparer (reference equality)
    /// </summary>
    [Benchmark]
    public async Task ComplexObject_DefaultComparer()
    {
        using var scope = _container!.CreateRegisterScope();
        _complexState = scope.RegisterParameter<ComplexObject>(nameof(_complexValue))
            .WithParameter(() => _complexValue)
            .WithEventCallback(() => _complexCallback);

        await _container.SimulateFirstRenderAsync(ParameterView.Empty);
        await _complexState.SetValueAsync(new ComplexObject { Id = 2, Name = "changed" });
    }

    /// <summary>
    /// Complex object with custom deep equality comparer
    /// </summary>
    [Benchmark]
    public async Task ComplexObject_CustomComparer()
    {
        using var scope = _container!.CreateRegisterScope();
        _complexState = scope.RegisterParameter<ComplexObject>(nameof(_complexValue))
            .WithParameter(() => _complexValue)
            .WithEventCallback(() => _complexCallback)
            .WithComparer(_complexObjectComparer);

        await _container.SimulateFirstRenderAsync(ParameterView.Empty);
        await _complexState.SetValueAsync(new ComplexObject { Id = 2, Name = "changed" });
    }

    /// <summary>
    /// Repeated equality checks with default comparer
    /// </summary>
    [Benchmark]
    public async Task RepeatedEqualityChecks_DefaultComparer()
    {
        using var scope = _container!.CreateRegisterScope();
        _stringState = scope.RegisterParameter<string>(nameof(_stringValue))
            .WithParameter(() => _stringValue)
            .WithEventCallback(() => _stringCallback);

        await _container.SimulateFirstRenderAsync(ParameterView.Empty);

        // 100 SetValueAsync calls with same value (should trigger equality check each time)
        for (int i = 0; i < 100; i++)
        {
            await _stringState.SetValueAsync("test"); // Same value
        }
    }

    /// <summary>
    /// Repeated equality checks with custom comparer
    /// </summary>
    [Benchmark]
    public async Task RepeatedEqualityChecks_CustomComparer()
    {
        using var scope = _container!.CreateRegisterScope();
        _stringState = scope.RegisterParameter<string>(nameof(_stringValue))
            .WithParameter(() => _stringValue)
            .WithEventCallback(() => _stringCallback)
            .WithComparer(_customStringComparer);

        await _container.SimulateFirstRenderAsync(ParameterView.Empty);

        // 100 SetValueAsync calls with same value (should trigger equality check each time)
        for (int i = 0; i < 100; i++)
        {
            await _stringState.SetValueAsync("test"); // Same value
        }
    }
}
