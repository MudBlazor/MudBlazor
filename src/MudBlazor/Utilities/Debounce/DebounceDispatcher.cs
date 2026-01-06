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

        // Check if disposed or cancelled before attempting to acquire lock
        if (_disposed || cancellationToken.IsCancellationRequested)
        {
            return;
        }

        var executeImmediately = false;
        CancellationTokenSource? newCts = null;
        CancellationTokenSource? oldCts = null;

        // Acquire lock with explicit acquired flag to avoid releasing when not acquired
        var acquired = false;
        try
        {
            await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            acquired = true;

            if (_disposed)
            {
                return;
            }

            if (_leading)
            {
                var now = DateTime.UtcNow;
                var timeSinceLast = now - _lastExecutionTime;
                if (timeSinceLast >= _interval)
                {
                    executeImmediately = true;
                    _lastExecutionTime = now;
                }
            }

            if (!executeImmediately)
            {
                // Create a new CTS linked to the provided token
                newCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                // Swap atomically so other threads see the new CTS
                oldCts = Interlocked.Exchange(ref _cancellationTokenSource, newCts);
                // Do not dispose oldCts while still holding the lock; capture and dispose after releasing
            }
        }
        catch (OperationCanceledException)
        {
            // Cancellation requested while waiting for the lock or via provided token
            return;
        }
        finally
        {
            if (acquired)
            {
                _lock.Release();
            }
        }

        // Cancel the previous CTS outside the lock, but don't dispose it yet.
        // The thread that owns it will dispose it in its finally block.
        oldCts?.Cancel();

        if (executeImmediately)
        {
            // Execute immediately on leading edge
            await action().ConfigureAwait(false);
            return;
        }

        // local reference for delay
        var localCts = newCts;
        if (localCts is null)
        {
            // Shouldn't happen, but guard defensively
            return;
        }

        CancellationToken token;
        try
        {
            token = localCts.Token;
        }
        catch (ObjectDisposedException)
        {
            // CTS was disposed by another thread between capture and access
            return;
        }

        try
        {
            await Task.Delay(_interval, token).ConfigureAwait(false);

            if (_leading)
            {
                // Update last execution time under lock to avoid races with other leading checks
                var acquired2 = false;
                try
                {
                    await _lock.WaitAsync(token).ConfigureAwait(false);
                    acquired2 = true;
                    _lastExecutionTime = DateTime.UtcNow;
                }
                finally
                {
                    if (acquired2)
                    {
                        _lock.Release();
                    }
                }
            }

            await action().ConfigureAwait(false);
        }
        catch (ObjectDisposedException)
        {
            // Silently ignore if CTS was disposed (happens when a new debounce call comes in or dispatcher is disposed)
        }
        catch (TaskCanceledException)
        {
            // Silently ignore task cancellation (either from new call or external cancellation)
        }
        catch (OperationCanceledException)
        {
            // Cancellation (either external or from a new debounce call) — swallow silently
        }
        finally
        {
            // If the CTS we used is still the current one, clear and dispose it.
            // Use Interlocked.CompareExchange to avoid disposing a CTS that was replaced by a newer call.
            var current = Interlocked.CompareExchange(ref _cancellationTokenSource, null, localCts);
            if (current == localCts)
            {
                // We successfully cleared the field; dispose our CTS
                localCts.Dispose();
            }
            // If current != localCts, another thread replaced it and will be responsible for disposal.
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
        // Swap out the CTS and cancel/dispose it outside the lock to avoid holding the lock while cancelling.
        CancellationTokenSource? ctsToCancel = null;
        _lock.Wait();
        try
        {
            ctsToCancel = Interlocked.Exchange(ref _cancellationTokenSource, null);
        }
        finally
        {
            _lock.Release();
        }

        if (ctsToCancel is not null)
        {
            try
            {
                ctsToCancel.Cancel();
            }
            catch
            {
                // Ignore exceptions during cancellation
            }
            ctsToCancel.Dispose();
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
        CancellationTokenSource? ctsToCancel = null;
        await _lock.WaitAsync().ConfigureAwait(false);
        try
        {
            ctsToCancel = Interlocked.Exchange(ref _cancellationTokenSource, null);
        }
        finally
        {
            _lock.Release();
        }

        if (ctsToCancel is not null)
        {
            try
            {
                ctsToCancel.Cancel();
            }
            catch
            {
                // Ignore exceptions during cancellation
            }
            ctsToCancel.Dispose();
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

            // Swap and capture CTS to cancel/dispose outside of lock
            var cts = Interlocked.Exchange(ref _cancellationTokenSource, null);
            if (cts is not null)
            {
                try
                {
                    cts.Cancel();
                }
                catch
                {
                    // Ignore exceptions during cancellation
                }
                cts.Dispose();
            }
        }
        finally
        {
            _lock.Release();
        }

        _lock.Dispose();
    }
}
