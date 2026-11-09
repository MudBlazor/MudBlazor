// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;
using MudBlazor.Utilities.ObserverManager;

namespace MudBlazor.Benchmarks;

/// <summary>
/// Benchmarks to compare different approaches for ObserverManager optimizations.
/// Tests suggestions from code review:
/// 1. AddOrUpdate vs TryGetValue + indexer
/// 2. TryRemove vs Remove
/// 3. Direct TryRemove on exception vs defunct list
/// 4. Tuple deconstruction vs KeyValuePair
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 1, iterationCount: 5)]
public class ObserverManagerComparisonBenchmark
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

    [Benchmark(Description = "TryGetOrAddSubscription - Update existing (100)")]
    public void TryGetOrAddSubscription_Update_100()
    {
        for (var i = 0; i < 100; i++)
        {
            _manager100!.TryGetOrAddSubscription(i, new TestObserver { Id = i + 1000 }, out _);
        }
    }

    [Benchmark(Description = "TryGetOrAddSubscription - Add new (100)")]
    public void TryGetOrAddSubscription_Add_100()
    {
        var manager = new ObserverManager<int, TestObserver>(NullLogger.Instance);
        for (var i = 0; i < 100; i++)
        {
            manager.TryGetOrAddSubscription(i, new TestObserver { Id = i }, out _);
        }
    }

    [Benchmark(Description = "Unsubscribe - 50 of 100")]
    public void Unsubscribe_50of100()
    {
        var manager = new ObserverManager<int, TestObserver>(NullLogger.Instance);
        for (var i = 0; i < 100; i++)
        {
            manager.Subscribe(i, new TestObserver { Id = i });
        }

        for (var i = 0; i < 50; i++)
        {
            manager.Unsubscribe(i * 2);
        }
    }

    [Benchmark(Description = "NotifyAsync - 1 failure in 100")]
    public async Task NotifyAsync_1Failure_100()
    {
        var manager = new ObserverManager<int, TestObserver>(NullLogger.Instance);
        for (var i = 0; i < 100; i++)
        {
            manager.Subscribe(i, new TestObserver { Id = i, ShouldFail = i == 50 });
        }

        await manager.NotifyAsync(obs => obs.OnNotifyAsync());
    }

    [Benchmark(Description = "NotifyAsync - 10 failures in 100")]
    public async Task NotifyAsync_10Failures_100()
    {
        var manager = new ObserverManager<int, TestObserver>(NullLogger.Instance);
        for (var i = 0; i < 100; i++)
        {
            manager.Subscribe(i, new TestObserver { Id = i, ShouldFail = i % 10 == 0 });
        }

        await manager.NotifyAsync(obs => obs.OnNotifyAsync());
    }

    [Benchmark(Description = "Iteration - foreach KeyValuePair (100)")]
    public int Iteration_KVP_100()
    {
        var count = 0;
        foreach (var observer in _manager100!)
        {
            count++;
        }
        return count;
    }

    [Benchmark(Description = "Iteration - foreach KeyValuePair (1000)")]
    public int Iteration_KVP_1000()
    {
        var count = 0;
        foreach (var observer in _manager1000!)
        {
            count++;
        }
        return count;
    }

    public class TestObserver
    {
        public int Id { get; set; }
        public bool ShouldFail { get; set; }

        public Task OnNotifyAsync()
        {
            if (ShouldFail)
            {
                throw new InvalidOperationException("Observer failed");
            }

            return Task.CompletedTask;
        }
    }
}
