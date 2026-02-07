// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities.Throttle;

/// <summary>
/// Limits the rate at which an action can be invoked.
/// </summary>
/// <remarks>
/// <para>
/// This dispatcher implements leading-edge throttling: when called, it executes the action immediately
/// if sufficient time has passed since the last execution started. Subsequent calls within the throttle interval
/// return the same Task as the currently executing action.
/// </para>
/// <para>
/// <strong>Thread Safety:</strong> This class is thread-safe. Multiple concurrent calls are properly synchronized.
/// </para>
/// <para>
/// <strong>Guarantees:</strong>
/// <list type="bullet">
/// <item>The action executes at most once per configured interval.</item>
/// <item>The first call in a new interval executes immediately (leading edge).</item>
/// <item>Calls within the interval return the same Task as the executing action.</item>
/// <item>Exceptions thrown by the action are propagated to all callers within that interval.</item>
/// <item>Disposal prevents further invocations.</item>
/// </list>
/// </para>
/// </remarks>
internal sealed class ThrottleDispatcher : IDisposable
{
    private readonly TimeSpan _interval;
    private readonly TimeProvider _timeProvider;
    // TODO: Replace with System.Threading.Lock when targeting .NET 9+
    // ReSharper disable once ChangeFieldTypeToSystemThreadingLock
    private readonly object _lock = new();
    private DateTimeOffset _lastExecutionStartTime = DateTimeOffset.MinValue;
    private Task? _currentTask;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrottleDispatcher"/> class with the specified interval.
    /// </summary>
    /// <param name="interval">The minimum interval in milliseconds between invocations. Must be non-negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is negative.</exception>
    public ThrottleDispatcher(int interval)
        : this(TimeSpan.FromMilliseconds(interval))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrottleDispatcher"/> class with the specified interval.
    /// </summary>
    /// <param name="interval">The minimum interval as a <see cref="TimeSpan"/> between invocations. Must be non-negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is negative.</exception>
    public ThrottleDispatcher(TimeSpan interval)
        : this(interval, TimeProvider.System)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrottleDispatcher"/> class with the specified interval and time provider.
    /// </summary>
    /// <param name="interval">The minimum interval in milliseconds between invocations. Must be non-negative.</param>
    /// <param name="timeProvider">The time provider to use for time queries.</param>
    /// <exception cref="ArgumentNullException">Thrown when TimeProvider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is negative.</exception>
    public ThrottleDispatcher(int interval, TimeProvider timeProvider)
        : this(TimeSpan.FromMilliseconds(interval), timeProvider)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrottleDispatcher"/> class with the specified interval and time provider.
    /// </summary>
    /// <param name="interval">The minimum interval as a <see cref="TimeSpan"/> between invocations. Must be non-negative.</param>
    /// <param name="timeProvider">The time provider to use for time queries.</param>
    /// <exception cref="ArgumentNullException">Thrown when TimeProvider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is negative.</exception>
    public ThrottleDispatcher(TimeSpan interval, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        if (interval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), @"Interval must be non-negative.");
        }

        _interval = interval;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Throttles the invocation of an asynchronous action.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If sufficient time has passed since the last execution started, the action executes immediately.
    /// Otherwise, this method returns the Task from the currently executing action (same instance).
    /// </para>
    /// <para>
    /// <strong>Exception Handling:</strong> Exceptions thrown by the action are propagated to all
    /// callers that received the same Task. Cancellation and disposal are handled silently without throwing exceptions.
    /// </para>
    /// </remarks>
    /// <param name="action">The asynchronous action to invoke.</param>
    /// <param name="cancellationToken">Optional cancellation token. Note: cancellation only prevents new executions; it does not cancel already-running actions.</param>
    /// <returns>A task representing the action's execution. Multiple calls within the interval return the same task instance, or Task.CompletedTask if cancelled/disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
    public Task ThrottleAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        // Silently return if cancelled or disposed
        if (cancellationToken.IsCancellationRequested || _disposed)
        {
            return Task.CompletedTask;
        }

        lock (_lock)
        {
            // Check again after acquiring lock
            if (_disposed)
            {
                return Task.CompletedTask;
            }

            var now = _timeProvider.GetUtcNow();
            var timeSinceLastExecution = now - _lastExecutionStartTime;

            // If we have a running task, and we're within the interval, return the same task
            if (_currentTask is not null && !_currentTask.IsCompleted && timeSinceLastExecution < _interval)
            {
                return _currentTask;
            }

            // Clear completed task if it exists
            if (_currentTask is not null && _currentTask.IsCompleted)
            {
                _currentTask = null;
            }

            // Enough time has passed - execute now
            _lastExecutionStartTime = now;
            // Note: action() is called synchronously; it returns a Task immediately
            // The actual async work happens when the task is awaited
            _currentTask = action();

            return _currentTask;
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="ThrottleDispatcher"/>.
    /// </summary>
    /// <remarks>
    /// This method prevents further invocations but does not cancel any currently executing action.
    /// </remarks>
    public void Dispose()
    {
        lock (_lock)
        {
            _disposed = true;
        }
    }
}
