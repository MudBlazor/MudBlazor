// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MudBlazor.Utilities.Throttle;

#nullable enable
/// <summary>
/// Utility class for throttling the execution of asynchronous tasks.
/// It limits the rate at which a function can be invoked based on a specified interval.
/// </summary>
/// <remarks>
/// Throttling ensures that a function is invoked no more than once within a defined time interval,
/// regardless of how many times it is called.
/// </remarks>
internal class ThrottleDispatcher
{
    private readonly int _interval;
    private readonly object _locker = new();
    private readonly bool _delayAfterExecution;
    private readonly bool _resetIntervalOnException;
    private bool _busy;
    private Task? _lastTask;
    private DateTime? _invokeTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrottleDispatcher"/> class with the specified parameters.
    /// </summary>
    /// <param name="interval">The minimum interval in milliseconds between invocations of the throttled function.</param>
    /// <param name="delayAfterExecution">If true, the interval is calculated from the end of the previous task execution; otherwise, from the start.</param>
    /// <param name="resetIntervalOnException">If true, the interval is reset when an exception occurs during the execution of the throttled function.</param>
    public ThrottleDispatcher(int interval, bool delayAfterExecution = false, bool resetIntervalOnException = false)
    {
        _interval = interval;
        _delayAfterExecution = delayAfterExecution;
        _resetIntervalOnException = resetIntervalOnException;
    }

    /// <summary>
    /// Determines whether the current invocation should wait based on the configured interval.
    /// </summary>
    /// <returns>True if waiting is required; otherwise, false.</returns>
    private bool ShouldWait() => _invokeTime.HasValue && (DateTime.UtcNow - _invokeTime.Value).TotalMilliseconds < _interval;

    /// <summary>
    /// Throttles the invocation of the provided function asynchronously.
    /// </summary>
    /// <param name="action">The function returning a Task to be invoked asynchronously.</param>
    /// <param name="cancellationToken">An optional CancellationToken.</param>
    /// <returns>The Task representing the asynchronous operation of the last executed function.</returns>
    public Task ThrottleAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        lock (_locker)
        {
            if (_lastTask is not null && (_busy || ShouldWait()))
            {
                return _lastTask;
            }

            _busy = true;
            _invokeTime = DateTime.UtcNow;

            _lastTask = action.Invoke();

            _lastTask.ContinueWith(_ =>
            {
                if (_delayAfterExecution)
                {
                    _invokeTime = DateTime.UtcNow;
                }

                _busy = false;
            }, cancellationToken);

            if (_resetIntervalOnException)
            {
                _lastTask.ContinueWith((_, _) =>
                {
                    _lastTask = null;
                    _invokeTime = null;
                }, cancellationToken, TaskContinuationOptions.OnlyOnFaulted);
            }

            return _lastTask;
        }
    }
}
