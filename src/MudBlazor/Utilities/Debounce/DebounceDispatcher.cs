﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MudBlazor.Utilities.Debounce;

#nullable enable
/// <summary>
/// The Debounce dispatcher delays the invocation of an action until a predetermined interval has elapsed since the last call. 
/// This ensures that the action is only invoked once after the calls have stopped for the specified duration.
/// </summary>
internal class DebounceDispatcher
{
    private Task? _waitingTask;
    private DateTime _lastInvokeTime;
    private Func<Task>? _funcToInvoke;
    private readonly int _interval;
    private readonly object _locker = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DebounceDispatcher"/> class with the specified interval.
    /// </summary>
    /// <param name="interval">The minimum interval in milliseconds between invocations of the debounced function.</param>
    public DebounceDispatcher(int interval)
    {
        _interval = interval;
    }

    /// <summary>
    /// Debounce the execution of asynchronous tasks.
    /// Ensures that a function is invoked only once within a specified interval, even if multiple invocations are requested.
    /// </summary>
    /// <remarks>
    /// This implementation will swallow any exceptions that is thrown by <see cref="function"/>.
    /// </remarks>
    /// <param name="function">The function that returns a Task to be invoked asynchronously.</param>
    /// <param name="cancellationToken">An optional CancellationToken.</param>
    /// <returns>A Task representing the asynchronous operation with minimal delay.</returns>
    public Task DebounceAsync(Func<Task> function, CancellationToken cancellationToken = default)
    {
        lock (_locker)
        {
            _funcToInvoke = function;
            _lastInvokeTime = DateTime.UtcNow;

            // If there's already a waiting task, return it
            if (_waitingTask is not null)
            {
                return _waitingTask;
            }

            var initialDelay = (int)(_interval - (DateTime.UtcNow - _lastInvokeTime).TotalMilliseconds);
            // Ensure delay is non-negative
            initialDelay = Math.Max(initialDelay, 0);

            _waitingTask = Task.Run(async () =>
            {
                // Wait for the initial delay
                await Task.Delay(initialDelay, cancellationToken);

                lock (_locker)
                {
                    if (_funcToInvoke != function)
                    {
                        // Another call was made within the debounce interval, so cancel this execution
                        _waitingTask = null;

                        return;
                    }
                }

                // Perform the function invocation
                try
                {
                    await _funcToInvoke.Invoke();
                }
                catch (Exception)
                {
                    // Ignore
                }

                // Clear the waiting task
                lock (_locker)
                {
                    _waitingTask = null;
                }
            }, cancellationToken);

            return _waitingTask;
        }
    }
}
