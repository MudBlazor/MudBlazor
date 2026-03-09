// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities.Debounce;

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
    private bool _disposed;
    private TimeSpan _interval;
    private int _pendingOperations;
    private readonly bool _leading;
    private readonly TimeProvider _timeProvider;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private CancellationTokenSource? _cancellationTokenSource;
    private DateTimeOffset _lastExecutionTime = DateTimeOffset.MinValue;

    /// <summary>
    /// Indicates whether a debounce delay is currently pending.
    /// </summary>
    public bool IsPending => Volatile.Read(ref _pendingOperations) > 0;

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
        : this(interval, leading, TimeProvider.System)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebounceDispatcher"/> class with the specified interval and time provider.
    /// </summary>
    /// <param name="interval">The debounce interval in milliseconds. Must be non-negative.</param>
    /// <param name="leading">If true, executes on the leading edge (immediately on first call). Default is false (trailing edge).</param>
    /// <param name="timeProvider">The time provider to use for delays and time queries.</param>
    /// <exception cref="ArgumentNullException">Thrown when TimeProvider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is negative.</exception>
    public DebounceDispatcher(int interval, bool leading, TimeProvider timeProvider)
        : this(TimeSpan.FromMilliseconds(interval), leading, timeProvider)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebounceDispatcher"/> class with the specified interval and time provider.
    /// </summary>
    /// <param name="interval">The debounce interval as a <see cref="TimeSpan"/>. Must be non-negative.</param>
    /// <param name="leading">If true, executes on the leading edge (immediately on first call). Default is false (trailing edge).</param>
    /// <param name="timeProvider">The time provider to use for delays and time queries.</param>
    /// <exception cref="ArgumentNullException">Thrown when TimeProvider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is negative.</exception>
    public DebounceDispatcher(TimeSpan interval, bool leading, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        if (interval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), @"Interval must be non-negative.");
        }

        _interval = interval;
        _leading = leading;
        _timeProvider = timeProvider;
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

        if (Volatile.Read(ref _disposed) || cancellationToken.IsCancellationRequested)
        {
            return;
        }

        var executeImmediately = false;
        CancellationTokenSource? localCts = null;
        CancellationTokenSource? previousCts = null;
        var scheduledInterval = TimeSpan.Zero;
        var lockAcquired = false;

        try
        {
            await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            lockAcquired = true;

            // Check again after acquiring lock
            if (_disposed)
            {
                return;
            }

            // In leading mode, check if we should execute immediately
            if (_leading)
            {
                var now = _timeProvider.GetUtcNow();
                var timeSinceLastExecution = now - _lastExecutionTime;

                // Execute immediately if enough time has passed since last execution
                if (timeSinceLastExecution >= _interval)
                {
                    executeImmediately = true;
                    _lastExecutionTime = now;
                }
            }

            // Replace the current pending CTS if we're not executing immediately.
            if (!executeImmediately)
            {
                scheduledInterval = _interval;
                previousCts = _cancellationTokenSource;
                localCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _cancellationTokenSource = localCts;
            }
        }
        catch (Exception ex) when (IsExpectedDebounceFlowException(ex))
        {
            // Lock-dispose and cancellation races are expected in debounce control flow.
            return;
        }
        finally
        {
            if (lockAcquired)
            {
                _lock.Release();
            }
        }

        await SafeCancelAsync(previousCts).ConfigureAwait(false);

        if (executeImmediately)
        {
            // Execute immediately without delay
            await action().ConfigureAwait(false);
            return;
        }

        if (localCts is not { } scheduledCts)
        {
            return;
        }

        Interlocked.Increment(ref _pendingOperations);
        var proceedToExecution = false;
        try
        {
            CancellationToken delayToken;
            try
            {
                delayToken = scheduledCts.Token;
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            // Wait for the debounce interval
            await Task.Delay(scheduledInterval, _timeProvider, delayToken).ConfigureAwait(false);
            proceedToExecution = true;
        }
        catch (Exception ex) when (IsExpectedDebounceFlowException(ex))
        {
            // Cancellation/disposal races are expected while waiting the debounce delay.
            return;
        }
        finally
        {
            Interlocked.Decrement(ref _pendingOperations);
            if (!proceedToExecution)
            {
                scheduledCts.Dispose();
            }
        }

        try
        {
            // Update last execution time for leading mode
            if (_leading)
            {
                var leadingLockAcquired = false;
                try
                {
                    await _lock.WaitAsync(scheduledCts.Token).ConfigureAwait(false);
                    leadingLockAcquired = true;
                    _lastExecutionTime = _timeProvider.GetUtcNow();
                }
                finally
                {
                    if (leadingLockAcquired)
                    {
                        _lock.Release();
                    }
                }
            }

            // Execute the action
            await action().ConfigureAwait(false);
        }
        catch (Exception ex) when (IsExpectedDebounceFlowException(ex))
        {
            // Cancellation/disposal races are expected around execution handoff.
        }
        finally
        {
            var cleanupLockAcquired = false;
            try
            {
                await _lock.WaitAsync(CancellationToken.None).ConfigureAwait(false);
                cleanupLockAcquired = true;
                if (ReferenceEquals(_cancellationTokenSource, scheduledCts))
                {
                    _cancellationTokenSource = null;
                }
            }
            catch (ObjectDisposedException)
            {
                // Ignore races with disposal.
            }
            finally
            {
                if (cleanupLockAcquired)
                {
                    _lock.Release();
                }

                scheduledCts.Dispose();
            }
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
        CancellationTokenSource? ctsToCancel;
        // ReSharper disable once MethodSupportsCancellation
        _lock.Wait();
        try
        {
            ctsToCancel = _cancellationTokenSource;
            _cancellationTokenSource = null;
        }
        finally
        {
            _lock.Release();
        }

        CancelAndDispose(ctsToCancel);
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

        await _lock.WaitAsync(CancellationToken.None).ConfigureAwait(false);
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
        CancellationTokenSource? ctsToCancel;
        await _lock.WaitAsync(CancellationToken.None).ConfigureAwait(false);
        try
        {
            ctsToCancel = _cancellationTokenSource;
            _cancellationTokenSource = null;
        }
        finally
        {
            _lock.Release();
        }

        await CancelAndDisposeAsync(ctsToCancel).ConfigureAwait(false);
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

        CancellationTokenSource? ctsToCancel;
        // ReSharper disable once MethodSupportsCancellation
        _lock.Wait();
        try
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            ctsToCancel = _cancellationTokenSource;
            _cancellationTokenSource = null;
        }
        finally
        {
            _lock.Release();
        }

        CancelAndDispose(ctsToCancel);

        // Intentionally do not dispose _lock. DebounceAsync/Cancel/UpdateIntervalAsync may still be racing and
        // disposing SemaphoreSlim while waiters exist can surface hangs/ObjectDisposedException paths.
    }

    private static void SafeCancel(CancellationTokenSource? cancellationTokenSource)
    {
        if (cancellationTokenSource is null)
        {
            return;
        }

        try
        {
            cancellationTokenSource.Cancel();
        }
        catch (Exception ex) when (IsExpectedCancellationException(ex))
        {
            // Ignore cancellation callback/disposal race exceptions.
        }
    }

    private static void CancelAndDispose(CancellationTokenSource? cancellationTokenSource)
    {
        SafeCancel(cancellationTokenSource);
        DisposeSafely(cancellationTokenSource);
    }

    private static async Task SafeCancelAsync(CancellationTokenSource? cancellationTokenSource)
    {
        if (cancellationTokenSource is null)
        {
            return;
        }

        try
        {
            await cancellationTokenSource.CancelAsync().ConfigureAwait(false);
        }
        catch (Exception ex) when (IsExpectedCancellationException(ex))
        {
            // Ignore cancellation callback/disposal race exceptions.
        }
    }

    private static async Task CancelAndDisposeAsync(CancellationTokenSource? cancellationTokenSource)
    {
        await SafeCancelAsync(cancellationTokenSource).ConfigureAwait(false);
        DisposeSafely(cancellationTokenSource);
    }

    private static void DisposeSafely(CancellationTokenSource? cancellationTokenSource)
    {
        try
        {
            cancellationTokenSource?.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Ignore races with disposal.
        }
    }

    private static bool IsExpectedCancellationException(Exception exception) =>
        exception is ObjectDisposedException or AggregateException;

    private static bool IsExpectedDebounceFlowException(Exception exception) =>
        exception is ObjectDisposedException or OperationCanceledException;
}
