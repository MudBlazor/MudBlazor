// Copyright (c) MudBlazor 2021
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
    private readonly int _interval;
    private CancellationTokenSource? _cancellationTokenSource;

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
    /// This implementation will swallow any exceptions that is thrown by the invoked task.
    /// </remarks>
    /// <param name="action">The function that returns a Task to be invoked asynchronously.</param>
    /// <param name="cancellationToken">An optional CancellationToken.</param>
    /// <returns>A Task representing the asynchronous operation with minimal delay.</returns>
    public async Task DebounceAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        // ReSharper disable MethodHasAsyncOverload (not available in .net7)
        // Cancel the previous debounce task if it exists
        _cancellationTokenSource?.Cancel();
        // ReSharper restore MethodHasAsyncOverload

        // Create a new cancellation token source linked with provided token
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cancellationToken = _cancellationTokenSource.Token;

        try
        {
            await Task.Delay(_interval, cancellationToken);

            await action();
        }
        catch (TaskCanceledException)
        {
            // If the task was canceled, ignore it
        }
    }
}
