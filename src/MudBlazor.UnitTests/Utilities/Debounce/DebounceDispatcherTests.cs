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

        // Wait for all tasks - first two should be cancelled
        Assert.ThrowsAsync<TaskCanceledException>(() => task1);
        Assert.ThrowsAsync<TaskCanceledException>(() => task2);
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
    public async Task DebounceAsync_ExceptionInAction_PropagatesException()
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
        cts.Cancel();

        // Assert
        Assert.ThrowsAsync<TaskCanceledException>(() => task);
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
        debounceDispatcher.Cancel();

        // Assert
        Assert.ThrowsAsync<TaskCanceledException>(() => task);
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

        // Assert
        Assert.ThrowsAsync<TaskCanceledException>(() => task);
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

        // Assert
        Assert.ThrowsAsync<ObjectDisposedException>(
            async () => await debounceDispatcher.DebounceAsync(Invoke));
    }

    [Test]
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

        // Assert
        Assert.ThrowsAsync<TaskCanceledException>(() => task);
        executed.Should().BeFalse();
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
        for (int i = 0; i < 10; i++)
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
                try
                {
                    await debounceDispatcher.DebounceAsync(Invoke);
                }
                catch (TaskCanceledException)
                {
                    // Expected for cancelled calls
                }
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
        Assert.Throws<ArgumentOutOfRangeException>(() => new DebounceDispatcher(-100));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DebounceDispatcher(TimeSpan.FromMilliseconds(-100)));
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
        var task2 = debounceDispatcher.DebounceAsync(TrackingAction);
        var task3 = debounceDispatcher.DebounceAsync(TrackingAction);
        var task4 = debounceDispatcher.DebounceAsync(TrackingAction);

        // First two should be cancelled
        Assert.ThrowsAsync<TaskCanceledException>(() => task2);
        Assert.ThrowsAsync<TaskCanceledException>(() => task3);

        // Last one should execute after interval
        await task4;

        // Assert - Should have executed twice (first immediate, last after debounce)
        executionCount.Should().Be(2);
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
