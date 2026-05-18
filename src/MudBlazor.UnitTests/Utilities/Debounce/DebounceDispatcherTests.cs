// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using AwesomeAssertions;
using Microsoft.Extensions.Time.Testing;
using MudBlazor.Utilities.Debounce;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Debounce;

#nullable enable
[TestFixture]
public class DebounceDispatcherTests
{
    [Test]
    public async Task DebounceAsync_MultipleCallsWithinInterval_ExecutesOnce()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(100);
        var counter = 0;
        Task Invoke()
        {
            counter++;

            return Task.CompletedTask;
        }

        // Act
        var task1 = debounceDispatcher.DebounceAsync(Invoke);
        var task2 = debounceDispatcher.DebounceAsync(Invoke);
        var task3 = debounceDispatcher.DebounceAsync(Invoke);

        // Wait for all tasks - first two should complete silently (cancelled internally)
        await task1;
        await task2;
        await task3; // Last one should succeed

        // Assert
        counter.Should().Be(1);
    }

    [Test]
    public async Task DebounceAsync_MultipleCallsOutsideInterval_ExecutesMultipleTimes()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(100, false, timeProvider);
        var counter = 0;
        Task Invoke()
        {
            counter++;

            return Task.CompletedTask;
        }

        // Act & Assert
        var task1 = debounceDispatcher.DebounceAsync(Invoke);
        timeProvider.Advance(TimeSpan.FromMilliseconds(150));
        await task1;
        counter.Should().Be(1);

        var task2 = debounceDispatcher.DebounceAsync(Invoke);
        timeProvider.Advance(TimeSpan.FromMilliseconds(150));
        await task2;
        counter.Should().Be(2);

        var task3 = debounceDispatcher.DebounceAsync(Invoke);
        timeProvider.Advance(TimeSpan.FromMilliseconds(150));
        await task3;
        counter.Should().Be(3);
    }

    [Test]
    public async Task DebounceAsync_SingleCall_ExecutesAfterInterval()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(100, false, timeProvider);
        var executed = false;
        Task Invoke()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act
        var task = debounceDispatcher.DebounceAsync(Invoke);
        executed.Should().BeFalse();
        timeProvider.Advance(TimeSpan.FromMilliseconds(100));
        await task;

        // Assert
        executed.Should().BeTrue();
    }

    [Test]
    public async Task DebounceAsync_ZeroInterval_ExecutesImmediately()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(0);
        var executed = false;
        Task Invoke()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act
        await debounceDispatcher.DebounceAsync(Invoke);

        // Assert
        executed.Should().BeTrue();
    }

    [Test]
    public void DebounceAsync_ExceptionInAction_PropagatesException()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(50, false, timeProvider);
        Task ThrowingAction()
        {
            throw new InvalidOperationException("Test exception");
        }

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                var task = debounceDispatcher.DebounceAsync(ThrowingAction);
                timeProvider.Advance(TimeSpan.FromMilliseconds(50));
                await task;
            });
        exception!.Message.Should().Be("Test exception");
    }

    [Test]
    public async Task DebounceAsync_CancellationToken_CancelsOperation()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(1000);
        using var cts = new CancellationTokenSource();
        var executed = false;
        Task Invoke()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act
        var task = debounceDispatcher.DebounceAsync(Invoke, cts.Token);
        // ReSharper disable once MethodHasAsyncOverload
        cts.Cancel();

        // Assert - should complete silently without throwing
        await task;
        executed.Should().BeFalse();
    }

    [Test]
    public async Task DebounceAsync_CancelMethod_CancelsPendingOperation()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(1000);
        var executed = false;
        Task Invoke()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act
        var task = debounceDispatcher.DebounceAsync(Invoke);
        // ReSharper disable once MethodHasAsyncOverload
        debounceDispatcher.Cancel();

        // Assert - should complete silently without throwing
        await task;
        executed.Should().BeFalse();
    }

    [Test]
    public async Task DebounceAsync_CancelAsyncMethod_CancelsPendingOperation()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(1000);
        var executed = false;
        Task Invoke()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act
        var task = debounceDispatcher.DebounceAsync(Invoke);
        await debounceDispatcher.CancelAsync();

        // Assert - should complete silently without throwing
        await task;
        executed.Should().BeFalse();
    }

    [Test]
    public void DebounceAsync_Dispose_PreventsNewCalls()
    {
        // Arrange
        var debounceDispatcher = new DebounceDispatcher(100);
        Task Invoke() => Task.CompletedTask;

        // Act
        debounceDispatcher.Dispose();

        // Assert - should complete silently without throwing
        var task = debounceDispatcher.DebounceAsync(Invoke);
        task.IsCompleted.Should().BeTrue();
    }

    [Test]
    [CancelAfter(5000)]
    public async Task DebounceAsync_Dispose_CancelsPendingOperation()
    {
        // Arrange
        var debounceDispatcher = new DebounceDispatcher(1000);
        var executed = false;
        Task Invoke()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act
        var task = debounceDispatcher.DebounceAsync(Invoke);
        debounceDispatcher.Dispose();

        // Assert - should complete silently without throwing
        await task.WaitAsync(TestContext.CurrentContext.CancellationToken);
        executed.Should().BeFalse();
    }

    [Test]
    public void DebounceAsync_DoubleDispose_DoesNotThrow()
    {
        // Arrange
        var debounceDispatcher = new DebounceDispatcher(100);

        // Act - Dispose twice
        debounceDispatcher.Dispose();
        debounceDispatcher.Dispose();

        // Assert - Should not throw, just pass if we get here
        Assert.Pass();
    }

    [Test]
    public async Task DebounceAsync_ExternalCancellationDuringDebounce_CancelsCorrectly()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(200, false, timeProvider);
        using var cts = new CancellationTokenSource();
        var executed = false;

        Task Invoke()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act - Start debounce with external cancellation token
        var task = debounceDispatcher.DebounceAsync(Invoke, cts.Token);

        // Cancel the external token while debounce is pending.
        timeProvider.Advance(TimeSpan.FromMilliseconds(50));
        // ReSharper disable once MethodHasAsyncOverload
        cts.Cancel();

        // Advance enough time so the debounce would have run if not cancelled.
        timeProvider.Advance(TimeSpan.FromMilliseconds(200));
        await task;

        // Assert - Should not have executed due to cancellation
        executed.Should().BeFalse();
        task.IsCompleted.Should().BeTrue();
    }

    [Test]
    public async Task DebounceAsync_LeadingMode_ExternalCancellationAfterImmediate_DoesNotAffectExecution()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(200, leading: true, timeProvider);
        using var cts = new CancellationTokenSource();
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - First call executes immediately
        await debounceDispatcher.DebounceAsync(TrackingAction, cts.Token);
        executionCount.Should().Be(1);

        // Start another debounce with same token
        var task = debounceDispatcher.DebounceAsync(TrackingAction, cts.Token);

        // Cancel the token during the debounce wait
        timeProvider.Advance(TimeSpan.FromMilliseconds(50));
        // ReSharper disable once MethodHasAsyncOverload
        cts.Cancel();

        // Advance enough time so the debounce would have run if not cancelled.
        timeProvider.Advance(TimeSpan.FromMilliseconds(200));
        await task;

        // Assert - Second call should not have executed due to cancellation
        executionCount.Should().Be(1);
        task.IsCompleted.Should().BeTrue();
    }

    [Test]
    public async Task DebounceAsync_RapidCalls_OnlyLastExecutes()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(100);
        var executionOrder = new List<int>();

        Func<Task> CreateAction(int id) => () =>
        {
            executionOrder.Add(id);
            return Task.CompletedTask;
        };

        // Act - Fire 10 rapid calls
        var tasks = new List<Task>();
        for (var i = 0; i < 10; i++)
        {
            tasks.Add(debounceDispatcher.DebounceAsync(CreateAction(i)));
        }

        // Wait for the last one
        await tasks[9];

        // Assert - Only the last action (id=9) should have executed
        executionOrder.Should().ContainSingle();
        executionOrder[0].Should().Be(9);
    }

    [Test]
    public async Task DebounceAsync_ConcurrentCalls_ThreadSafe()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(50);
        var executionCount = 0;
        Task Invoke()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - Fire many concurrent calls
        var tasks = Enumerable.Range(0, 100)
            .Select(_ => Task.Run(async () =>
            {
                // ReSharper disable once AccessToDisposedClosure
                await debounceDispatcher.DebounceAsync(Invoke);
            }))
            .ToArray();

        await Task.WhenAll(tasks);

        // Give time for last debounce to complete
        await Task.Delay(100);

        // Assert - Should execute at least once, but may execute a few times due to timing
        executionCount.Should().BeGreaterThanOrEqualTo(1);
        executionCount.Should().BeLessThan(10); // But not too many times
    }

    [Test]
    public void Constructor_NegativeInterval_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new DebounceDispatcher(-100));
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new DebounceDispatcher(TimeSpan.FromMilliseconds(-100)));
    }

    [Test]
    public void DebounceAsync_NullAction_ThrowsArgumentNullException()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(100);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await debounceDispatcher.DebounceAsync(null!));
    }

    [Test]
    public async Task DebounceAsync_LongRunningAction_DoesNotBlockSubsequentCalls()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(50, false, timeProvider);
        var firstStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var firstCanComplete = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        async Task LongRunningAction()
        {
            firstStarted.TrySetResult();
            await firstCanComplete.Task;
        }

        Task QuickAction() => Task.CompletedTask;

        // Act
        var firstTask = debounceDispatcher.DebounceAsync(LongRunningAction);
        timeProvider.Advance(TimeSpan.FromMilliseconds(50));
        await firstStarted.Task.WaitAsync(TestContext.CurrentContext.CancellationToken); // Wait for first action to start

        // Allow first to complete
        firstCanComplete.TrySetResult();
        await firstTask;

        // Now start a new debounce - should work fine
        var secondTask = debounceDispatcher.DebounceAsync(QuickAction);
        timeProvider.Advance(TimeSpan.FromMilliseconds(50));
        await secondTask;

        // Assert - If we got here, it worked
        Assert.Pass();
    }

    [Test]
    public async Task DebounceAsync_LeadingMode_ExecutesImmediatelyOnFirstCall()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(100, leading: true, timeProvider);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act
        await debounceDispatcher.DebounceAsync(TrackingAction);

        // Assert - First call should execute immediately
        executionCount.Should().Be(1);
    }

    [Test]
    public async Task DebounceAsync_LeadingMode_DebounceSubsequentCalls()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(100, leading: true, timeProvider);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - First call executes immediately
        await debounceDispatcher.DebounceAsync(TrackingAction);
        executionCount.Should().Be(1);

        // Rapid subsequent calls within interval should be debounced
        var task1 = debounceDispatcher.DebounceAsync(TrackingAction);
        var task2 = debounceDispatcher.DebounceAsync(TrackingAction);
        var task3 = debounceDispatcher.DebounceAsync(TrackingAction);

        timeProvider.Advance(TimeSpan.FromMilliseconds(100));

        // Wait for all debounced tasks to complete
        await task1;
        await task2;
        await task3;

        // Assert - Should have executed twice (first immediate, last after debounce)
        executionCount.Should().Be(2);
    }

    [Test]
    public async Task UpdateInterval_ChangesDebounceInterval()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(1000, false, timeProvider);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - Update interval before any debounce
        await debounceDispatcher.UpdateIntervalAsync(100);

        // Start debounce with new interval
        var task = debounceDispatcher.DebounceAsync(TrackingAction);

        timeProvider.Advance(TimeSpan.FromMilliseconds(150));
        await task;

        // Assert - Should have executed with the new interval
        executionCount.Should().Be(1);
    }

    [Test]
    public async Task UpdateInterval_PreservesPendingDebounce()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(200, false, timeProvider);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - Start debounce
        var task = debounceDispatcher.DebounceAsync(TrackingAction);

        // Update interval while debounce is pending (doesn't cancel the pending debounce)
        timeProvider.Advance(TimeSpan.FromMilliseconds(50));
        await debounceDispatcher.UpdateIntervalAsync(300);

        // Wait for original interval to complete
        timeProvider.Advance(TimeSpan.FromMilliseconds(200));
        await task;

        // Assert - Should have executed with original interval since update doesn't cancel pending
        executionCount.Should().Be(1);
    }

    [Test]
    public void UpdateInterval_NegativeInterval_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(100);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await debounceDispatcher.UpdateIntervalAsync(-100));
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await debounceDispatcher.UpdateIntervalAsync(TimeSpan.FromMilliseconds(-100)));
    }

    [Test]
    public async Task UpdateInterval_ToZero_AllowsImmediateExecution()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(1000);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - Update to zero interval
        await debounceDispatcher.UpdateIntervalAsync(0);

        // Debounce should execute immediately with zero interval
        await debounceDispatcher.DebounceAsync(TrackingAction);

        // Assert
        executionCount.Should().Be(1);
    }

    [Test]
    public async Task UpdateInterval_MultipleUpdates_UsesLatestInterval()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(1000, false, timeProvider);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - Update interval multiple times
        await debounceDispatcher.UpdateIntervalAsync(500);
        await debounceDispatcher.UpdateIntervalAsync(200);
        await debounceDispatcher.UpdateIntervalAsync(100);

        // Start debounce
        var task = debounceDispatcher.DebounceAsync(TrackingAction);

        // Wait for the final interval
        timeProvider.Advance(TimeSpan.FromMilliseconds(150));
        await task;

        // Assert - Should use latest interval (100ms)
        executionCount.Should().Be(1);
    }

    [Test]
    public async Task UpdateInterval_ConcurrentWithDebounce_ThreadSafe()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(100);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - Update interval concurrently with debounce calls
        var tasks = new List<Task>();
        for (var i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                // ReSharper disable AccessToDisposedClosure
                await debounceDispatcher.DebounceAsync(TrackingAction);
            }));

            var i1 = i;
            tasks.Add(Task.Run(async () =>
            {
                await debounceDispatcher.UpdateIntervalAsync(100 + (i1 * 10));
                // ReSharper restore AccessToDisposedClosure
            }));
        }

        await Task.WhenAll(tasks);
        await Task.Delay(300); // Wait for final debounce

        // Assert - Should have executed at least once without crashing
        executionCount.Should().BeGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task UpdateInterval_WithLeadingMode_UsesNewIntervalForSubsequentCalls()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(1000, leading: true, timeProvider);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - First call executes immediately
        await debounceDispatcher.DebounceAsync(TrackingAction);
        executionCount.Should().Be(1);

        // Update to shorter interval
        await debounceDispatcher.UpdateIntervalAsync(100);

        // Wait for new interval to pass
        timeProvider.Advance(TimeSpan.FromMilliseconds(150));

        // Next call should execute immediately with new interval
        await debounceDispatcher.DebounceAsync(TrackingAction);

        // Assert
        executionCount.Should().Be(2);
    }

    [Test]
    public async Task UpdateInterval_FromTimeSpan_WorksCorrectly()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(TimeSpan.FromSeconds(10), false, timeProvider);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - Update using TimeSpan
        await debounceDispatcher.UpdateIntervalAsync(TimeSpan.FromMilliseconds(100));

        var task = debounceDispatcher.DebounceAsync(TrackingAction);
        timeProvider.Advance(TimeSpan.FromMilliseconds(150));
        await task;

        // Assert
        executionCount.Should().Be(1);
    }

    [Test]
    public async Task DebounceAsync_LeadingMode_ResetsAfterInterval()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(100, leading: true, timeProvider: timeProvider);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - First call executes immediately
        var task1 = debounceDispatcher.DebounceAsync(TrackingAction);
        await task1;
        executionCount.Should().Be(1);

        // Wait for interval to pass
        timeProvider.Advance(TimeSpan.FromMilliseconds(150));

        // Next call should execute immediately again
        var task2 = debounceDispatcher.DebounceAsync(TrackingAction);
        await task2;

        // Assert
        executionCount.Should().Be(2);
    }

    [Test]
    public async Task DebounceAsync_IsPending_TracksDelayWindowOnly()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(100, false, timeProvider);
        var actionGate = new TaskCompletionSource<bool>();

        async Task BlockingAction() => await actionGate.Task;

        // Act
        var task = debounceDispatcher.DebounceAsync(BlockingAction);
        await Task.Yield();

        // Assert - pending during delay
        debounceDispatcher.IsPending.Should().BeTrue();

        // advance debounce interval to begin action execution
        timeProvider.Advance(TimeSpan.FromMilliseconds(100));
        var pendingCleared = await WaitUntilAsync(() => !debounceDispatcher.IsPending, TimeSpan.FromSeconds(1));

        // Assert - pending cleared once delay elapses, even while action is still running
        pendingCleared.Should().BeTrue();
        debounceDispatcher.IsPending.Should().BeFalse();

        actionGate.SetResult(true);
        await task;
        debounceDispatcher.IsPending.Should().BeFalse();
    }

    [Test]
    public async Task DebounceAsync_IsPending_ClearsAfterCancellation()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        using var debounceDispatcher = new DebounceDispatcher(100, false, timeProvider);

        // Act
        var task = debounceDispatcher.DebounceAsync(() => Task.CompletedTask);
        await Task.Yield();
        debounceDispatcher.IsPending.Should().BeTrue();

        await debounceDispatcher.CancelAsync();
        await task;

        // Assert
        debounceDispatcher.IsPending.Should().BeFalse();
    }

    [Test]
    public async Task Cancel_Swallows_AggregateException_From_Callbacks()
    {
        var dispatcher = new DebounceDispatcher(TimeSpan.FromMilliseconds(200));

        // create CTS by starting a debounce
        _ = dispatcher.DebounceAsync(() => Task.CompletedTask);

        var cts = await WaitForPrivateCtsAsync(dispatcher);

        // register a callback that throws when Cancel() is called
        cts.Token.Register(() => throw new InvalidOperationException("callback fail"));

        // Cancel should swallow exceptions
        var act = () => dispatcher.Cancel();

        act.Should().NotThrow();
    }

    [Test]
    public async Task Cancel_Swallows_ObjectDisposedException_When_CtsDisposed()
    {
        var dispatcher = new DebounceDispatcher(TimeSpan.FromMilliseconds(200));

        _ = dispatcher.DebounceAsync(() => Task.CompletedTask);
        var cts = await WaitForPrivateCtsAsync(dispatcher);

        // Dispose the CTS to simulate race
        cts.Dispose();

        var act = () => dispatcher.Cancel();

        act.Should().NotThrow();
    }

    [Test]
    [Explicit]
    public async Task Cancel_Race_Stress_NoUnhandledExceptions()
    {
        var dispatcher = new DebounceDispatcher(TimeSpan.FromMilliseconds(50));

        var tasks = new Task[100];
        for (var i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(async () =>
            {
                var debounceTask = dispatcher.DebounceAsync(() => Task.CompletedTask);
                // ReSharper disable once MethodHasAsyncOverload
                dispatcher.Cancel();
                await debounceTask;
            });
        }

        var act = async () => await Task.WhenAll(tasks).WaitAsync(TimeSpan.FromSeconds(10));

        await act.Should().NotThrowAsync();
    }

    [Test]
    public async Task CancelAsync_Swallows_AggregateException_From_Callbacks()
    {
        var dispatcher = new DebounceDispatcher(TimeSpan.FromMilliseconds(200));

        // create CTS by starting a debounce
        _ = dispatcher.DebounceAsync(() => Task.CompletedTask);

        var cts = await WaitForPrivateCtsAsync(dispatcher);

        // register a callback that throws when Cancel() is called
        cts.Token.Register(() => throw new InvalidOperationException("callback fail"));

        // Cancel should swallow exceptions
        var act = () => dispatcher.CancelAsync();

        await act.Should().NotThrowAsync();
    }

    [Test]
    public async Task CancelAsync_Swallows_ObjectDisposedException_When_CtsDisposed()
    {
        var dispatcher = new DebounceDispatcher(TimeSpan.FromMilliseconds(200));

        _ = dispatcher.DebounceAsync(() => Task.CompletedTask);
        var cts = await WaitForPrivateCtsAsync(dispatcher);

        // Dispose the CTS to simulate race
        cts.Dispose();

        var act = () => dispatcher.CancelAsync();

        await act.Should().NotThrowAsync();
    }

    [Test]
    [Explicit]
    public async Task CancelAsync_Race_Stress_NoUnhandledExceptions()
    {
        var dispatcher = new DebounceDispatcher(TimeSpan.FromMilliseconds(50));

        var tasks = new Task[100];
        for (var i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(async () =>
            {
                var debounceTask = dispatcher.DebounceAsync(() => Task.CompletedTask);
                await dispatcher.CancelAsync();
                await debounceTask;
            });
        }

        var act = async () => await Task.WhenAll(tasks).WaitAsync(TimeSpan.FromSeconds(10));

        await act.Should().NotThrowAsync();
    }

    [Test]
    public async Task Dispose_Swallows_AggregateException_From_Callbacks()
    {
        var dispatcher = new DebounceDispatcher(TimeSpan.FromMilliseconds(200));

        // create CTS by starting a debounce
        _ = dispatcher.DebounceAsync(() => Task.CompletedTask);

        var cts = await WaitForPrivateCtsAsync(dispatcher);

        // register a callback that throws when Cancel() is called
        cts.Token.Register(() => throw new InvalidOperationException("callback fail"));

        // Cancel should swallow exceptions
        var act = () => dispatcher.Dispose();

        act.Should().NotThrow();
    }

    [Test]
    public async Task Dispose_Swallows_ObjectDisposedException_When_CtsDisposed()
    {
        var dispatcher = new DebounceDispatcher(TimeSpan.FromMilliseconds(200));

        _ = dispatcher.DebounceAsync(() => Task.CompletedTask);
        var cts = await WaitForPrivateCtsAsync(dispatcher);

        // Dispose the CTS to simulate race
        cts.Dispose();

        var act = () => dispatcher.Dispose();

        act.Should().NotThrow();
    }

    [Test]
    public async Task Dispose_ConcurrentWithDebounceCalls_DoesNotHangOrThrow()
    {
        var dispatcher = new DebounceDispatcher(TimeSpan.FromMilliseconds(50));

        var workers = Enumerable.Range(0, 100)
            .Select(_ => Task.Run(async () =>
            {
                await dispatcher.DebounceAsync(() => Task.CompletedTask);
            }))
            .ToArray();

        var disposer = Task.Run(() => dispatcher.Dispose());

        var act = async () => await Task.WhenAll(workers.Append(disposer)).WaitAsync(TimeSpan.FromSeconds(5));

        await act.Should().NotThrowAsync();
    }

    private static CancellationTokenSource? GetPrivateCts(object dispatcher)
    {
        var field = dispatcher.GetType().GetField("_cancellationTokenSource", BindingFlags.NonPublic | BindingFlags.Instance);
        return (CancellationTokenSource?)field?.GetValue(dispatcher);
    }

    private static async Task<CancellationTokenSource> WaitForPrivateCtsAsync(object dispatcher)
    {
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(1));
        while (!timeoutTask.IsCompleted)
        {
            var cts = GetPrivateCts(dispatcher);
            if (cts is not null)
            {
                return cts;
            }

            await Task.Yield();
        }

        Assert.Fail("Timed out waiting for DebounceDispatcher to create its cancellation token source.");
        return null!;
    }

    private static async Task<bool> WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
    {
        var timeoutTask = Task.Delay(timeout);
        while (!timeoutTask.IsCompleted)
        {
            if (condition())
            {
                return true;
            }

            await Task.Yield();
        }

        return condition();
    }
}
