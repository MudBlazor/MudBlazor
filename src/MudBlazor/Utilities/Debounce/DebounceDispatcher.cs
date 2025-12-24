// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities.Debounce;

#nullable enable
/// <summary>
/// Delays the invocation of an action until a predetermined interval has elapsed since the last call.
/// </summary>
/// <remarks>
/// <para>
/// This dispatcher implements debouncing with optional leading-edge execution.
/// In trailing mode (default), the action executes only after the specified interval has passed
/// with no new invocations. In leading mode, the first call executes immediately, then subsequent
/// calls are debounced.
/// </para>
/// <para>
/// <strong>Thread Safety:</strong> This class is thread-safe. Multiple concurrent calls to <see cref="DebounceAsync"/>
/// are properly synchronized.
/// </para>
/// <para>
/// <strong>Guarantees:</strong>
/// <list type="bullet">
/// <item>In trailing mode: Only the last invocation's action will execute after the interval elapses.</item>
/// <item>In leading mode: First call executes immediately, subsequent calls within the interval are debounced.</item>
/// <item>Previous pending invocations are automatically cancelled.</item>
/// <item>Exceptions thrown by the action are propagated to the caller.</item>
/// <item>Disposal cancels any pending invocation.</item>
/// </list>
/// </para>
/// </remarks>
internal sealed class DebounceDispatcher : IDisposable
{
    private TimeSpan _interval;
    private readonly bool _leading;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private CancellationTokenSource? _cancellationTokenSource;
    private DateTime _lastExecutionTime = DateTime.MinValue;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebounceDispatcher"/> class with the specified interval.
    /// </summary>
    /// <param name="interval">The debounce interval in milliseconds. Must be non-negative.</param>
    /// <param name="leading">If true, executes on the leading edge (immediately on first call). Default is false (trailing edge).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is negative.</exception>
    public DebounceDispatcher(int interval, bool leading = false)
        : this(TimeSpan.FromMilliseconds(interval), leading)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebounceDispatcher"/> class with the specified interval.
    /// </summary>
    /// <param name="interval">The debounce interval as a <see cref="TimeSpan"/>. Must be non-negative.</param>
    /// <param name="leading">If true, executes on the leading edge (immediately on first call). Default is false (trailing edge).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is negative.</exception>
    public DebounceDispatcher(TimeSpan interval, bool leading = false)
    {
        if (interval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), @"Interval must be non-negative.");
        }

        _interval = interval;
        _leading = leading;
    }

    /// <summary>
    /// Debounces the execution of an asynchronous action.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In trailing mode (default): Each call cancels any previously pending action and starts a new timer.
    /// The action executes only if no new calls occur within the configured interval.
    /// </para>
    /// <para>
    /// In leading mode: The first call (or first call after the interval expires) executes immediately.
    /// Subsequent calls within the interval cancel previous pending actions and are debounced.
    /// </para>
    /// <para>
    /// <strong>Exception Handling:</strong> Exceptions thrown by the action are propagated to the caller.
    /// Cancellation (either from the token or disposal) is handled silently without throwing exceptions.
    /// </para>
    /// </remarks>
    /// <param name="action">The asynchronous action to invoke after the debounce interval.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the debounced action.</param>
    /// <returns>A task that completes when the action executes or is cancelled/disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
    public async Task DebounceAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        var executeImmediately = false;
        CancellationTokenSource? localCts = null;

        // Check if disposed before attempting to acquire lock
        if (_disposed)
        {
            return;
        }

        // Check if cancellation was requested before starting
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Check again after acquiring lock
            if (_disposed)
            {
                return;
            }

            // In leading mode, check if we should execute immediately
            if (_leading)
            {
                var now = DateTime.UtcNow;
                var timeSinceLastExecution = now - _lastExecutionTime;

                // Execute immediately if enough time has passed since last execution
                if (timeSinceLastExecution >= _interval)
                {
                    executeImmediately = true;
                    _lastExecutionTime = now;
                }
            }

            // Cancel and dispose previous cancellation token source if we're not executing immediately
            if (!executeImmediately)
            {
                if (_cancellationTokenSource is not null)
                {
                    await _cancellationTokenSource.CancelAsync().ConfigureAwait(false);
                    _cancellationTokenSource.Dispose();
                }

                // Create new cancellation token source linked with provided token
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                // Capture the CTS while still holding the lock to prevent race conditions
                localCts = _cancellationTokenSource;
            }
        }
        catch (OperationCanceledException)
        {
            // Silently return if operation was cancelled during lock acquisition
            return;
        }
        finally
        {
            _lock.Release();
        }

        if (executeImmediately)
        {
            // Execute immediately without delay
            await action().ConfigureAwait(false);
            return;
        }
        try
        {
            // Wait for the debounce interval
            await Task.Delay(_interval, localCts!.Token).ConfigureAwait(false);

            // Update last execution time for leading mode
            if (_leading)
            {
                await _lock.WaitAsync(localCts.Token).ConfigureAwait(false);
                try
                {
                    _lastExecutionTime = DateTime.UtcNow;
                }
                finally
                {
                    _lock.Release();
                }
            }

            // Execute the action
            await action().ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            // Silently ignore cancellation (either from new call or external cancellation)
        }
        catch (OperationCanceledException)
        {
            // Silently ignore cancellation
        }
    }

    /// <summary>
    /// Cancels any pending debounced action.
    /// </summary>
    /// <remarks>
    /// This method is thread-safe and can be called concurrently with <see cref="DebounceAsync"/>.
    /// </remarks>
    public void Cancel()
    {
        _lock.Wait();
        try
        {
            _cancellationTokenSource?.Cancel();
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Updates the debounce interval asynchronously.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method updates the interval without affecting any currently pending debounced action.
    /// The new interval will be used for the next debounce operation.
    /// </para>
    /// <para>
    /// This method is thread-safe and can be called concurrently with <see cref="DebounceAsync"/>.
    /// </para>
    /// </remarks>
    /// <param name="interval">The new debounce interval in milliseconds. Must be non-negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is negative.</exception>
    public Task UpdateIntervalAsync(int interval) => UpdateIntervalAsync(TimeSpan.FromMilliseconds(interval));

    /// <summary>
    /// Updates the debounce interval asynchronously.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method updates the interval without affecting any currently pending debounced action.
    /// The new interval will be used for the next debounce operation.
    /// </para>
    /// <para>
    /// This method is thread-safe and can be called concurrently with <see cref="DebounceAsync"/>.
    /// </para>
    /// </remarks>
    /// <param name="interval">The new debounce interval as a <see cref="TimeSpan"/>. Must be non-negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is negative.</exception>
    public async Task UpdateIntervalAsync(TimeSpan interval)
    {
        if (interval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), @"Interval must be non-negative.");
        }

        await _lock.WaitAsync().ConfigureAwait(false);
        try
        {
            _interval = interval;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Cancels any pending debounced action asynchronously.
    /// </summary>
    /// <remarks>
    /// This method is thread-safe and can be called concurrently with <see cref="DebounceAsync"/>.
    /// </remarks>
    public async Task CancelAsync()
    {
        await _lock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_cancellationTokenSource is not null)
            {
                await _cancellationTokenSource.CancelAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="DebounceDispatcher"/>.
    /// </summary>
    /// <remarks>
    /// This method cancels any pending debounced action and prevents further use of the dispatcher.
    /// Cancellation is performed synchronously as this is a synchronous Dispose method.
    /// </remarks>
    public void Dispose()
    {
        // Check if already disposed before attempting to acquire lock
        if (_disposed)
        {
            return;
        }

        _lock.Wait();
        try
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            // Use synchronous Cancel() in Dispose since this is a synchronous method
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
        finally
        {
            _lock.Release();
        }

        _lock.Dispose();
    }
}
