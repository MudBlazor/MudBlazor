// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;
using MudBlazor.Utilities.ObserverManager;

namespace MudBlazor.Benchmarks;

/// <summary>
/// Benchmarks for ObserverManager performance and memory consumption.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(launchCount: 1, warmupCount: 3, iterationCount: 10)]
public class ObserverManagerBenchmark
{
    private ObserverManager<int, TestObserver>? _manager;
    private ObserverManager<int, TestObserver>? _managerWith10;
    private ObserverManager<int, TestObserver>? _managerWith100;
    private ObserverManager<int, TestObserver>? _managerWith1000;
    private ObserverManager<int, TestObserver>? _managerWith10000;

    [Params(100)]
    public int ObserverCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _manager = new ObserverManager<int, TestObserver>(NullLogger.Instance);

        // Setup managers with different observer counts for specific tests
        _managerWith10 = new ObserverManager<int, TestObserver>(NullLogger.Instance);
        for (var i = 0; i < 10; i++)
        {
            _managerWith10.Subscribe(i, new TestObserver { Id = i });
        }

        _managerWith100 = new ObserverManager<int, TestObserver>(NullLogger.Instance);
        for (var i = 0; i < 100; i++)
        {
            _managerWith100.Subscribe(i, new TestObserver { Id = i });
        }

        _managerWith1000 = new ObserverManager<int, TestObserver>(NullLogger.Instance);
        for (var i = 0; i < 1000; i++)
        {
            _managerWith1000.Subscribe(i, new TestObserver { Id = i });
        }

        _managerWith10000 = new ObserverManager<int, TestObserver>(NullLogger.Instance);
        for (var i = 0; i < 10000; i++)
        {
            _managerWith10000.Subscribe(i, new TestObserver { Id = i });
        }
    }

    [Benchmark(Description = "Subscribe - Add new observer")]
    public void Subscribe()
    {
        _manager!.Subscribe(ObserverCount, new TestObserver { Id = ObserverCount });
    }

    [Benchmark(Description = "Subscribe - Update existing observer")]
    public void SubscribeUpdate()
    {
        // Setup: Add observer first
        _manager!.Subscribe(ObserverCount, new TestObserver { Id = ObserverCount });

        // Benchmark: Update same observer
        _manager.Subscribe(ObserverCount, new TestObserver { Id = ObserverCount + 1 });
    }

    [Benchmark(Description = "Unsubscribe - Remove observer")]
    public void Unsubscribe()
    {
        // Setup: Add observer first
        _manager!.Subscribe(ObserverCount, new TestObserver { Id = ObserverCount });

        // Benchmark: Remove observer
        _manager.Unsubscribe(ObserverCount);
    }

    [Benchmark(Description = "IsSubscribed - Check subscription")]
    public bool IsSubscribed()
    {
        // Setup: Add observer first
        _manager!.Subscribe(ObserverCount, new TestObserver { Id = ObserverCount });

        // Benchmark: Check if subscribed
        return _manager.IsSubscribed(ObserverCount);
    }

    [Benchmark(Description = "NotifyAsync - No failures (10 observers)")]
    public async Task NotifyAsync_NoFailures_10()
    {
        await _managerWith10!.NotifyAsync(observer => observer.OnNotifyAsync());
    }

    [Benchmark(Description = "NotifyAsync - No failures (100 observers)")]
    public async Task NotifyAsync_NoFailures_100()
    {
        await _managerWith100!.NotifyAsync(observer => observer.OnNotifyAsync());
    }

    [Benchmark(Description = "NotifyAsync - No failures (1000 observers)")]
    public async Task NotifyAsync_NoFailures_1000()
    {
        await _managerWith1000!.NotifyAsync(observer => observer.OnNotifyAsync());
    }

    [Benchmark(Description = "NotifyAsync - No failures (10000 observers)")]
    public async Task NotifyAsync_NoFailures_10000()
    {
        await _managerWith10000!.NotifyAsync(observer => observer.OnNotifyAsync());
    }

    [Benchmark(Description = "NotifyAsync - With 1 failure (100 observers)")]
    public async Task NotifyAsync_With1Failure_100()
    {
        // Setup: Mark one observer to fail
        if (_managerWith100!.TryGetSubscription(50, out var observer))
        {
            observer.ShouldFail = true;
        }

        await _managerWith100.NotifyAsync(obs => obs.OnNotifyAsync());

        // Cleanup: Reset failure flag
        if (_managerWith100.TryGetSubscription(50, out observer))
        {
            observer.ShouldFail = false;
        }
    }

    [Benchmark(Description = "NotifyAsync - With 10 failures (100 observers)")]
    public async Task NotifyAsync_With10Failures_100()
    {
        // Setup: Mark 10 observers to fail
        for (var i = 0; i < 10; i++)
        {
            if (_managerWith100!.TryGetSubscription(i * 10, out var observer))
            {
                observer.ShouldFail = true;
            }
        }

        await _managerWith100!.NotifyAsync(obs => obs.OnNotifyAsync());

        // Note: Failed observers are removed, so we need to re-add them
        for (var i = 0; i < 10; i++)
        {
            _managerWith100.Subscribe(i * 10, new TestObserver { Id = i * 10 });
        }
    }

    [Benchmark(Description = "NotifyAsync - With predicate (100 observers, 10 match)")]
    public async Task NotifyAsync_WithPredicate_100()
    {
        await _managerWith100!.NotifyAsync(
            observer => observer.OnNotifyAsync(),
            (id, _) => id % 10 == 0);
    }

    [Benchmark(Description = "NotifyAsync - Concurrent modification (100 observers)")]
    public async Task NotifyAsync_ConcurrentModification_100()
    {
        var manager = new ObserverManager<int, TestObserver>(NullLogger.Instance);

        // Add 100 observers
        for (var i = 0; i < 100; i++)
        {
            manager.Subscribe(i, new TestObserver { Id = i });
        }

        // Notify while adding new observers during notification
        await manager.NotifyAsync(
            async observer =>
            {
                await observer.OnNotifyAsync();
                // Add a new observer during notification
                manager.Subscribe(observer.Id + 1000, new TestObserver { Id = observer.Id + 1000 });
            });
    }

    [Benchmark(Description = "GetEnumerator - Iterate all observers (100)")]
    public int GetEnumerator_100()
    {
        var count = 0;
        foreach (var observer in _managerWith100!)
        {
            count++;
        }
        return count;
    }

    [Benchmark(Description = "GetEnumerator - Iterate all observers (1000)")]
    public int GetEnumerator_1000()
    {
        var count = 0;
        foreach (var observer in _managerWith1000!)
        {
            count++;
        }
        return count;
    }

    [Benchmark(Description = "Observers property - Get dictionary copy (100)")]
    public int ObserversProperty_100()
    {
        return _managerWith100!.Observers.Count;
    }

    [Benchmark(Description = "Observers property - Get dictionary copy (1000)")]
    public int ObserversProperty_1000()
    {
        return _managerWith1000!.Observers.Count;
    }

    [Benchmark(Description = "FindObserverIdentities - Find matching (100 observers, 10 match)")]
    public int FindObserverIdentities_100()
    {
        var count = 0;
        foreach (var _ in _managerWith100!.FindObserverIdentities((id, _) => id % 10 == 0))
        {
            count++;
        }
        return count;
    }

    /// <summary>
    /// Test observer for benchmarking.
    /// </summary>
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
