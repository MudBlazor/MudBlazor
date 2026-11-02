// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;
using MudBlazor.Utilities.ObserverManager;

namespace MudBlazor.Benchmarks;

/// <summary>
/// Quick benchmarks for ObserverManager to demonstrate key improvements.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 1, iterationCount: 5)]
public class ObserverManagerQuickBenchmark
{
    private ObserverManager<int, TestObserver>? _manager100;
    private ObserverManager<int, TestObserver>? _manager1000;

    [GlobalSetup]
    public void Setup()
    {
        _manager100 = new ObserverManager<int, TestObserver>(NullLogger.Instance);
        for (var i = 0; i < 100; i++)
        {
            _manager100.Subscribe(i, new TestObserver { Id = i });
        }

        _manager1000 = new ObserverManager<int, TestObserver>(NullLogger.Instance);
        for (var i = 0; i < 1000; i++)
        {
            _manager1000.Subscribe(i, new TestObserver { Id = i });
        }
    }

    [Benchmark(Description = "NotifyAsync 100 observers, no failures")]
    public async Task NotifyAsync_100_NoFailures()
    {
        await _manager100!.NotifyAsync(observer => observer.OnNotifyAsync());
    }

    [Benchmark(Description = "NotifyAsync 1000 observers, no failures")]
    public async Task NotifyAsync_1000_NoFailures()
    {
        await _manager1000!.NotifyAsync(observer => observer.OnNotifyAsync());
    }

    [Benchmark(Description = "GetEnumerator and iterate 100")]
    public int Enumerate_100()
    {
        var count = 0;
        foreach (var observer in _manager100!)
        {
            count++;
        }
        return count;
    }

    [Benchmark(Description = "GetEnumerator and iterate 1000")]
    public int Enumerate_1000()
    {
        var count = 0;
        foreach (var observer in _manager1000!)
        {
            count++;
        }
        return count;
    }

    [Benchmark(Description = "TryGetSubscription 100 times")]
    public bool TryGetSubscription_100()
    {
        var result = false;
        for (var i = 0; i < 100; i++)
        {
            result = _manager100!.TryGetSubscription(i, out var observer);
        }
        return result;
    }

    [Benchmark(Description = "Observers property access (100)")]
    public int ObserversProperty_100()
    {
        return _manager100!.Observers.Count;
    }

    public class TestObserver
    {
        public int Id { get; set; }

        public Task OnNotifyAsync()
        {
            return Task.CompletedTask;
        }
    }
}
