// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
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
        using var debounceDispatcher = new DebounceDispatcher(100);
        var counter = 0;
        Task Invoke()
        {
            counter++;

            return Task.CompletedTask;
        }

        // Act
        await debounceDispatcher.DebounceAsync(Invoke);
        counter.Should().Be(1);

        await Task.Delay(150);
        await debounceDispatcher.DebounceAsync(Invoke);
        counter.Should().Be(2);

        await Task.Delay(150);
        await debounceDispatcher.DebounceAsync(Invoke);

        // Assert
        counter.Should().Be(3);
    }

    [Test]
    public async Task DebounceAsync_SingleCall_ExecutesAfterInterval()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(100);
        var executed = false;
        Task Invoke()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act
        var task = debounceDispatcher.DebounceAsync(Invoke);
        executed.Should().BeFalse();
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
        using var debounceDispatcher = new DebounceDispatcher(50);
        Task ThrowingAction()
        {
            throw new InvalidOperationException("Test exception");
        }

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(
            () => debounceDispatcher.DebounceAsync(ThrowingAction));
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
    public void DebounceAsync_Dispose_CancelsPendingOperation()
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
        task.Wait(100);
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
        using var debounceDispatcher = new DebounceDispatcher(200);
        using var cts = new CancellationTokenSource();
        var executed = false;

        Task Invoke()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act - Start debounce with external cancellation token
        var task = debounceDispatcher.DebounceAsync(Invoke, cts.Token);

        // Cancel the external token while debounce is pending
        await Task.Delay(50, CancellationToken.None);
        // ReSharper disable once MethodHasAsyncOverload
        cts.Cancel();

        // Wait a bit more to ensure debounce would have completed if not cancelled
        await Task.Delay(200, CancellationToken.None);

        // Assert - Should not have executed due to cancellation
        executed.Should().BeFalse();
        task.IsCompleted.Should().BeTrue();
    }

    [Test]
    public async Task DebounceAsync_LeadingMode_ExternalCancellationAfterImmediate_DoesNotAffectExecution()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(200, leading: true);
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
        await Task.Delay(50, CancellationToken.None);
        // ReSharper disable once MethodHasAsyncOverload
        cts.Cancel();

        // Wait to ensure debounce completes
        await Task.Delay(200, CancellationToken.None);

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
        using var debounceDispatcher = new DebounceDispatcher(50);
        var firstStarted = new TaskCompletionSource<bool>();
        var firstCanComplete = new TaskCompletionSource<bool>();

        async Task LongRunningAction()
        {
            firstStarted.SetResult(true);
            await firstCanComplete.Task;
        }

        Task QuickAction() => Task.CompletedTask;

        // Act
        var firstTask = debounceDispatcher.DebounceAsync(LongRunningAction);
        await firstStarted.Task; // Wait for first action to start

        // Allow first to complete
        firstCanComplete.SetResult(true);
        await firstTask;

        // Now start a new debounce - should work fine
        await Task.Delay(100); // Wait for interval to pass
        await debounceDispatcher.DebounceAsync(QuickAction);

        // Assert - If we got here, it worked
        Assert.Pass();
    }

    [Test]
    public async Task DebounceAsync_LeadingMode_ExecutesImmediatelyOnFirstCall()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(100, leading: true);
        var executionCount = 0;
        var executionTimes = new List<DateTime>();

        Task TrackingAction()
        {
            executionTimes.Add(DateTime.UtcNow);
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act
        var startTime = DateTime.UtcNow;
        await debounceDispatcher.DebounceAsync(TrackingAction);

        // Assert - First call should execute immediately
        executionCount.Should().Be(1);
        (executionTimes[0] - startTime).TotalMilliseconds.Should().BeLessThan(50);
    }

    [Test]
    public async Task DebounceAsync_LeadingMode_DebounceSubsequentCalls()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(100, leading: true);
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
        _ = debounceDispatcher.DebounceAsync(TrackingAction);
        _ = debounceDispatcher.DebounceAsync(TrackingAction);
        _ = debounceDispatcher.DebounceAsync(TrackingAction);

        // Wait for debounce to complete
        await Task.Delay(150);

        // Assert - Should have executed twice (first immediate, last after debounce)
        executionCount.Should().Be(2);
    }

    [Test]
    public async Task UpdateInterval_ChangesDebounceInterval()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(1000);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - Update interval before any debounce
        await debounceDispatcher.UpdateIntervalAsync(100);

        // Start debounce with new interval
        _ = debounceDispatcher.DebounceAsync(TrackingAction);

        // Wait for the new shorter interval
        await Task.Delay(150);

        // Assert - Should have executed with the new interval
        executionCount.Should().Be(1);
    }

    [Test]
    public async Task UpdateInterval_PreservesPendingDebounce()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(200);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - Start debounce
        _ = debounceDispatcher.DebounceAsync(TrackingAction);

        // Update interval while debounce is pending (doesn't cancel the pending debounce)
        await Task.Delay(50);
        await debounceDispatcher.UpdateIntervalAsync(300);

        // Wait for original interval to complete
        await Task.Delay(200);

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
        using var debounceDispatcher = new DebounceDispatcher(1000);
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
        _ = debounceDispatcher.DebounceAsync(TrackingAction);

        // Wait for the final interval
        await Task.Delay(150);

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
                await debounceDispatcher.UpdateIntervalAsync(100 + i1 * 10);
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
        using var debounceDispatcher = new DebounceDispatcher(1000, leading: true);
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
        await Task.Delay(150);

        // Next call should execute immediately with new interval
        await debounceDispatcher.DebounceAsync(TrackingAction);

        // Assert
        executionCount.Should().Be(2);
    }

    [Test]
    public async Task UpdateInterval_FromTimeSpan_WorksCorrectly()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(TimeSpan.FromSeconds(10));
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - Update using TimeSpan
        await debounceDispatcher.UpdateIntervalAsync(TimeSpan.FromMilliseconds(100));

        _ = debounceDispatcher.DebounceAsync(TrackingAction);
        await Task.Delay(150);

        // Assert
        executionCount.Should().Be(1);
    }

    [Test]
    public async Task DebounceAsync_LeadingMode_ResetsAfterInterval()
    {
        // Arrange
        using var debounceDispatcher = new DebounceDispatcher(100, leading: true);
        var executionCount = 0;

        Task TrackingAction()
        {
            Interlocked.Increment(ref executionCount);
            return Task.CompletedTask;
        }

        // Act - First call executes immediately
        await debounceDispatcher.DebounceAsync(TrackingAction);
        executionCount.Should().Be(1);

        // Wait for interval to pass
        await Task.Delay(150);

        // Next call should execute immediately again
        await debounceDispatcher.DebounceAsync(TrackingAction);

        // Assert
        executionCount.Should().Be(2);
    }
}
