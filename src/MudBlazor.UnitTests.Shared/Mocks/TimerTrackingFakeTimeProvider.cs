// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Time.Testing;

namespace MudBlazor.UnitTests.Shared.Mocks;

/// <summary>
/// A <see cref="FakeTimeProvider"/> that counts the timers it creates, so a test can wait for a timer to exist before advancing the clock.
/// </summary>
/// <remarks>
/// Debounced inputs return to the caller before the debouncer creates its timer.
/// Advancing the clock in that window moves time that nothing is waiting on yet, and the debounced commit never fires.
/// </remarks>
public class TimerTrackingFakeTimeProvider : FakeTimeProvider
{
    private readonly object _lock = new();
    private readonly List<(int Count, TaskCompletionSource Signal)> _waiters = [];
    private int _timersCreated;

    /// <summary>
    /// The number of timers created so far.
    /// </summary>
    public int TimersCreated
    {
        get
        {
            lock (_lock)
            {
                return _timersCreated;
            }
        }
    }

    /// <inheritdoc />
    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        var timer = base.CreateTimer(callback, state, dueTime, period);

        lock (_lock)
        {
            _timersCreated++;
            for (var i = _waiters.Count - 1; i >= 0; i--)
            {
                if (_timersCreated >= _waiters[i].Count)
                {
                    _waiters[i].Signal.TrySetResult();
                    _waiters.RemoveAt(i);
                }
            }
        }

        return timer;
    }

    /// <summary>
    /// Completes once a timer has been created after <paramref name="timersCreatedBefore"/> timers existed.
    /// </summary>
    /// <param name="timersCreatedBefore">The value of <see cref="TimersCreated"/> read before the action that schedules the timer.</param>
    public Task WaitForTimerAsync(int timersCreatedBefore)
    {
        lock (_lock)
        {
            if (_timersCreated > timersCreatedBefore)
            {
                return Task.CompletedTask;
            }

            var signal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _waiters.Add((timersCreatedBefore + 1, signal));
            return signal.Task.WaitAsync(TimeSpan.FromSeconds(10));
        }
    }

    /// <summary>
    /// Completes once a timer has been created after <paramref name="timersCreatedBefore"/> timers existed, or once <paramref name="settled"/> returns <c>true</c>.
    /// </summary>
    /// <remarks>
    /// A debounced input can commit without creating a timer.
    /// When validation of an earlier commit finds a debounce pending, it cancels it and commits the typed text immediately, which can happen before the new timer exists.
    /// </remarks>
    /// <param name="timersCreatedBefore">The value of <see cref="TimersCreated"/> read before the action that schedules the timer.</param>
    /// <param name="settled">Returns <c>true</c> once the outcome the timer would have produced is already in place.</param>
    public async Task WaitForTimerAsync(int timersCreatedBefore, Func<bool> settled)
    {
        ArgumentNullException.ThrowIfNull(settled);

        TaskCompletionSource signal;
        lock (_lock)
        {
            if (_timersCreated > timersCreatedBefore)
            {
                return;
            }

            signal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _waiters.Add((timersCreatedBefore + 1, signal));
        }

        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!signal.Task.IsCompleted)
        {
            if (settled())
            {
                lock (_lock)
                {
                    _waiters.RemoveAll(waiter => waiter.Signal == signal);
                }

                return;
            }

            if (DateTime.UtcNow > deadline)
            {
                throw new TimeoutException("No timer was created and the debounced outcome was not reached within 10 seconds.");
            }

            await Task.WhenAny(signal.Task, Task.Delay(5));
        }
    }
}
