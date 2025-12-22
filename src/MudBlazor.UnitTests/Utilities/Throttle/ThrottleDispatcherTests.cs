// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using MudBlazor.Utilities.Throttle;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Throttle;

#nullable enable
[TestFixture]
public class ThrottleDispatcherTests
{
    [Test]
    public async Task ThrottleAsync_SingleCall_ExecutesImmediately()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(100);
        var executed = false;
        Task Invoke()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act
        await dispatcher.ThrottleAsync(Invoke);

        // Assert
        executed.Should().BeTrue();
    }

    [Test]
    public async Task ThrottleAsync_MultipleRapidCalls_ExecutesOnce()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(100);
        var counter = 0;
        async Task Invoke()
        {
            Interlocked.Increment(ref counter);
            await Task.Delay(10); // Small delay to keep the task running
        }

        // Act - All three calls should return the same task
        var task1 = dispatcher.ThrottleAsync(Invoke);
        var task2 = dispatcher.ThrottleAsync(Invoke);
        var task3 = dispatcher.ThrottleAsync(Invoke);

        await Task.WhenAll(task1, task2, task3);

        // Assert
        counter.Should().Be(1);
        task1.Should().BeSameAs(task2);
        task2.Should().BeSameAs(task3);
    }

    [Test]
    public async Task ThrottleAsync_CallsSpacedByInterval_ExecutesMultipleTimes()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(50);
        var counter = 0;
        Task Invoke()
        {
            Interlocked.Increment(ref counter);
            return Task.CompletedTask;
        }

        // Act
        await dispatcher.ThrottleAsync(Invoke);
        counter.Should().Be(1);

        await Task.Delay(100); // Wait for interval to pass
        await dispatcher.ThrottleAsync(Invoke);
        counter.Should().Be(2);

        await Task.Delay(100);
        await dispatcher.ThrottleAsync(Invoke);

        // Assert
        counter.Should().Be(3);
    }

    [Test]
    public void ThrottleAsync_ExceptionInAction_PropagatesException()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(100);
        Task ThrowingAction()
        {
            throw new InvalidOperationException("Test exception");
        }

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.ThrottleAsync(ThrowingAction));
        exception!.Message.Should().Be("Test exception");
    }

    [Test]
    public async Task ThrottleAsync_ExceptionResetsThrottle_AllowsImmediateNextCall()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(100);
        var counter = 0;

        Task ThrowingAction()
        {
            Interlocked.Increment(ref counter);
            throw new InvalidOperationException();
        }

        Task SuccessAction()
        {
            Interlocked.Increment(ref counter);
            return Task.CompletedTask;
        }

        // Act
        Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.ThrottleAsync(ThrowingAction));
        counter.Should().Be(1);

        // Should be able to call immediately after exception
        await dispatcher.ThrottleAsync(SuccessAction);

        // Assert
        counter.Should().Be(2);
    }

    [Test]
    public async Task ThrottleAsync_MultipleCallersWithinInterval_GetSameTask()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(100);
        var tcs = new TaskCompletionSource<bool>();

        async Task SlowAction()
        {
            await tcs.Task;
        }

        // Act
        var task1 = dispatcher.ThrottleAsync(SlowAction);
        var task2 = dispatcher.ThrottleAsync(SlowAction);
        var task3 = dispatcher.ThrottleAsync(SlowAction);

        // Complete the action
        tcs.SetResult(true);
        await Task.WhenAll(task1, task2, task3);

        // Assert - All should be the same task
        task1.Should().BeSameAs(task2);
        task2.Should().BeSameAs(task3);
    }

    [Test]
    public async Task ThrottleAsync_ZeroInterval_AllowsEveryCall()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(0);
        var counter = 0;
        Task Invoke()
        {
            Interlocked.Increment(ref counter);
            return Task.CompletedTask;
        }

        // Act
        await dispatcher.ThrottleAsync(Invoke);
        await dispatcher.ThrottleAsync(Invoke);
        await dispatcher.ThrottleAsync(Invoke);

        // Assert - With zero interval, each call should execute
        counter.Should().Be(3);
    }

    [Test]
    public async Task ThrottleAsync_ConcurrentCalls_ThreadSafe()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(100);
        var executionCount = 0;
        var callCount = 0;

        Task Invoke()
        {
            Interlocked.Increment(ref executionCount);
            return Task.Delay(10); // Small delay to simulate work
        }

        // Act - Fire many concurrent calls rapidly
        var tasks = Enumerable.Range(0, 100)
            .Select(_ => Task.Run(async () =>
            {
                Interlocked.Increment(ref callCount);
                await dispatcher.ThrottleAsync(Invoke);
            }))
            .ToArray();

        await Task.WhenAll(tasks);

        // Assert
        callCount.Should().Be(100); // All calls should have been made
        executionCount.Should().BeGreaterThanOrEqualTo(1); // At least one execution
        executionCount.Should().BeLessThan(20); // But throttled to much fewer executions
    }

    [Test]
    public void ThrottleAsync_CancellationToken_PreventsNewExecution()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(1000);
        using var cts = new CancellationTokenSource();
        var executed = false;

        Task Invoke()
        {
            executed = true;
            return Task.CompletedTask;
        }

        // Act
        cts.Cancel();

        // Assert - should return completed task silently
        var task = dispatcher.ThrottleAsync(Invoke, cts.Token);
        task.IsCompleted.Should().BeTrue();
        executed.Should().BeFalse();
    }

    [Test]
    public void ThrottleAsync_Dispose_PreventsNewCalls()
    {
        // Arrange
        var dispatcher = new ThrottleDispatcher(100);
        Task Invoke() => Task.CompletedTask;

        // Act
        dispatcher.Dispose();

        // Assert - should return completed task silently
        var task = dispatcher.ThrottleAsync(Invoke);
        task.IsCompleted.Should().BeTrue();
    }

    [Test]
    public void ThrottleAsync_Dispose_DoesNotCancelRunningAction()
    {
        // Arrange
        var dispatcher = new ThrottleDispatcher(100);
        var actionStarted = new TaskCompletionSource<bool>();
        var actionCanComplete = new TaskCompletionSource<bool>();
        var actionCompleted = false;

        async Task LongRunningAction()
        {
            actionStarted.SetResult(true);
            await actionCanComplete.Task;
            actionCompleted = true;
        }

        // Act
        var task = dispatcher.ThrottleAsync(LongRunningAction);
        actionStarted.Task.Wait(); // Wait for action to start

        dispatcher.Dispose(); // Dispose while action is running

        actionCanComplete.SetResult(true); // Allow action to complete
        task.Wait();

        // Assert
        actionCompleted.Should().BeTrue();
    }

    [Test]
    public void ThrottleAsync_DoubleDispose_DoesNotThrow()
    {
        // Arrange
        var dispatcher = new ThrottleDispatcher(100);

        // Act - Dispose twice
        dispatcher.Dispose();
        dispatcher.Dispose();

        // Assert - Should not throw, just pass if we get here
        Assert.Pass();
    }

    [Test]
    public void ThrottleAsync_DisposeDuringLock_HandlesGracefully()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(100);
        var firstStarted = new TaskCompletionSource<bool>();
        var firstCanComplete = new TaskCompletionSource<bool>();

        async Task SlowAction()
        {
            firstStarted.SetResult(true);
            await firstCanComplete.Task;
        }

        // Act - Start a slow action to hold the task
        var task1 = dispatcher.ThrottleAsync(SlowAction);
        firstStarted.Task.Wait();

        // Now dispose and try to call again
        // ReSharper disable once DisposeOnUsingVariable
        dispatcher.Dispose();
        var task2 = dispatcher.ThrottleAsync(() => Task.CompletedTask);

        // Complete the first action
        firstCanComplete.SetResult(true);
        task1.Wait();

        // Assert - second task should complete immediately (disposed)
        task2.IsCompleted.Should().BeTrue();
    }

    [Test]
    public void Constructor_NegativeInterval_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new ThrottleDispatcher(-100));
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new ThrottleDispatcher(TimeSpan.FromMilliseconds(-100)));
    }

    [Test]
    public void ThrottleAsync_NullAction_ThrowsArgumentNullException()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(100);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await dispatcher.ThrottleAsync(null!));
    }

    [Test]
    public async Task ThrottleAsync_LongRunningAction_AllowsNewCallAfterCompletion()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(50);
        var firstCompleted = false;
        var secondExecuted = false;

        async Task FirstAction()
        {
            await Task.Delay(200); // Longer than throttle interval
            firstCompleted = true;
        }

        Task SecondAction()
        {
            secondExecuted = true;
            return Task.CompletedTask;
        }

        // Act
        await dispatcher.ThrottleAsync(FirstAction);
        firstCompleted.Should().BeTrue();

        // Now call again - should execute since first is complete and time has passed
        await dispatcher.ThrottleAsync(SecondAction);

        // Assert
        secondExecuted.Should().BeTrue();
    }

    [Test]
    public async Task ThrottleAsync_RapidCallsWithSlowAction_ThrottlesCorrectly()
    {
        // Arrange
        using var dispatcher = new ThrottleDispatcher(200); // 200ms throttle
        var counter = 0;

        async Task SlowInvoke()
        {
            Interlocked.Increment(ref counter);
            await Task.Delay(50); // Task takes 50ms
        }

        // Act - Fire 10 rapid calls with no delay between them
        var tasks = new List<Task>();
        for (var i = 0; i < 10; i++)
        {
            tasks.Add(dispatcher.ThrottleAsync(SlowInvoke));
        }

        await Task.WhenAll(tasks);

        // Assert - All calls during the first action's execution should share the same task
        // So we should only see 1 execution (or maybe 2 if timing is tight)
        counter.Should().BeLessThanOrEqualTo(2);
    }
}
