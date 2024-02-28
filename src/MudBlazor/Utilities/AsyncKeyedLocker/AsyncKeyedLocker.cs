// Copyright (c) 2023 Mark Cilia Vincenti
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MudBlazor.Utilities.AsyncKeyedLocker;

#nullable enable
/// <summary>
/// Represents a thread-safe keyed locker that allows you to lock based on a key (keyed semaphores), only allowing a specified number of concurrent threads that share the same key.
/// </summary>
internal class AsyncKeyedLocker<TKey> : IDisposable where TKey : notnull
{
    internal readonly AsyncKeyedLockDictionary<TKey> Dictionary;

    /// <summary>
    /// Read-only index of objects held by <see cref="AsyncKeyedLocker{TKey}"/>.
    /// </summary>
    public IReadOnlyDictionary<TKey, AsyncKeyedLockReleaser<TKey>> Index => Dictionary;

    /// <summary>
    /// The maximum number of requests for the semaphore that can be granted concurrently. Defaults to 1.
    /// </summary>
    public int MaxCount => Dictionary.MaxCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncKeyedLocker{TKey}" /> class, sets the <see cref="SemaphoreSlim"/> initial count to 1, has the default concurrency level, has the default initial capacity, and uses the default comparer for the key type.
    /// </summary>
    public AsyncKeyedLocker()
    {
        Dictionary = new AsyncKeyedLockDictionary<TKey>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncKeyedLocker{TKey}" /> class, uses the specified <see cref="AsyncKeyedLockOptions"/>, has the default concurrency level, has the default initial capacity, and uses the default comparer for the key type.
    /// </summary>
    /// <param name="options">The <see cref="AsyncKeyedLockOptions"/> to use.</param>
    /// <exception cref="ArgumentOutOfRangeException">Parameter is out of range.</exception>
    public AsyncKeyedLocker(AsyncKeyedLockOptions options)
    {
        Dictionary = new AsyncKeyedLockDictionary<TKey>(options);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncKeyedLocker{TKey}" /> class, uses the specified <see cref="AsyncKeyedLockOptions"/>, has the default concurrency level, has the default initial capacity, and uses the default comparer for the key type.
    /// </summary>
    /// <param name="options">The <see cref="AsyncKeyedLockOptions"/> to use.</param>
    /// <exception cref="ArgumentOutOfRangeException">Parameter is out of range.</exception>
    public AsyncKeyedLocker(Action<AsyncKeyedLockOptions> options)
    {
        var optionsParam = new AsyncKeyedLockOptions();
        options(optionsParam);

        Dictionary = new AsyncKeyedLockDictionary<TKey>(optionsParam);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncKeyedLocker{TKey}" /> class, sets the <see cref="SemaphoreSlim"/> initial count to 1, has the default concurrency level, has the default initial capacity, and uses the specified <see cref="IEqualityComparer{TKey}"/>.
    /// </summary>
    /// <param name="comparer">The equality comparison implementation to use when comparing keys.</param>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
    public AsyncKeyedLocker(IEqualityComparer<TKey> comparer)
    {
        Dictionary = new AsyncKeyedLockDictionary<TKey>(comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncKeyedLocker{TKey}" /> class, uses the specified <see cref="AsyncKeyedLockOptions"/>, has the default concurrency level, has the default initial capacity, and uses the specified <see cref="IEqualityComparer{TKey}"/>.
    /// </summary>
    /// <param name="options">The <see cref="AsyncKeyedLockOptions"/> to use.</param>
    /// <param name="comparer">The equality comparison implementation to use when comparing keys.</param>
    /// <exception cref="ArgumentOutOfRangeException">Parameter is out of range.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
    public AsyncKeyedLocker(AsyncKeyedLockOptions options, IEqualityComparer<TKey> comparer)
    {
        Dictionary = new AsyncKeyedLockDictionary<TKey>(options, comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncKeyedLocker{TKey}" /> class, uses the specified <see cref="AsyncKeyedLockOptions"/>, has the default concurrency level, has the default initial capacity, and uses the specified <see cref="IEqualityComparer{TKey}"/>.
    /// </summary>
    /// <param name="options">The <see cref="AsyncKeyedLockOptions"/> to use.</param>
    /// <param name="comparer">The equality comparison implementation to use when comparing keys.</param>
    /// <exception cref="ArgumentOutOfRangeException">Parameter is out of range.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
    public AsyncKeyedLocker(Action<AsyncKeyedLockOptions> options, IEqualityComparer<TKey> comparer)
    {
        var optionsParam = new AsyncKeyedLockOptions();
        options(optionsParam);

        Dictionary = new AsyncKeyedLockDictionary<TKey>(optionsParam, comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncKeyedLocker{TKey}" /> class, sets the <see cref="SemaphoreSlim"/> initial count to 1, has the specified concurrency level and capacity, and uses the default comparer for the key type.
    /// </summary>
    /// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="AsyncKeyedLocker{TKey}"/> concurrently.</param>
    /// <param name="capacity">The initial number of elements that the <see cref="AsyncKeyedLocker{TKey}"/> can contain.</param>
    /// <exception cref="ArgumentOutOfRangeException">Parameter is out of range.</exception>
    public AsyncKeyedLocker(int concurrencyLevel, int capacity)
    {
        Dictionary = new AsyncKeyedLockDictionary<TKey>(concurrencyLevel, capacity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncKeyedLocker{TKey}" /> class, uses the specified <see cref="AsyncKeyedLockOptions"/>, has the specified concurrency level and capacity, and uses the default comparer for the key type.
    /// </summary>
    /// <param name="options">The <see cref="AsyncKeyedLockOptions"/> to use.</param>
    /// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="AsyncKeyedLocker{TKey}"/> concurrently.</param>
    /// <param name="capacity">The initial number of elements that the <see cref="AsyncKeyedLocker{TKey}"/> can contain.</param>
    /// <exception cref="ArgumentOutOfRangeException">Parameter is out of range.</exception>
    public AsyncKeyedLocker(AsyncKeyedLockOptions options, int concurrencyLevel, int capacity)
    {
        Dictionary = new AsyncKeyedLockDictionary<TKey>(options, concurrencyLevel, capacity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncKeyedLocker{TKey}" /> class, uses the specified <see cref="AsyncKeyedLockOptions"/>, has the specified concurrency level and capacity, and uses the default comparer for the key type.
    /// </summary>
    /// <param name="options">The <see cref="AsyncKeyedLockOptions"/> to use.</param>
    /// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="AsyncKeyedLocker{TKey}"/> concurrently.</param>
    /// <param name="capacity">The initial number of elements that the <see cref="AsyncKeyedLocker{TKey}"/> can contain.</param>
    /// <exception cref="ArgumentOutOfRangeException">Parameter is out of range.</exception>
    public AsyncKeyedLocker(Action<AsyncKeyedLockOptions> options, int concurrencyLevel, int capacity)
    {
        var optionsParam = new AsyncKeyedLockOptions();
        options(optionsParam);

        Dictionary = new AsyncKeyedLockDictionary<TKey>(optionsParam, concurrencyLevel, capacity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncKeyedLocker{TKey}" /> class, uses the specified <see cref="SemaphoreSlim"/> initial count, has the specified concurrency level and capacity, and uses the default comparer for the key type.
    /// </summary>
    /// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="AsyncKeyedLocker{TKey}"/> concurrently.</param>
    /// <param name="capacity">The initial number of elements that the <see cref="AsyncKeyedLocker{TKey}"/> can contain.</param>
    /// <param name="comparer">The equality comparison implementation to use when comparing keys.</param>
    /// <exception cref="ArgumentOutOfRangeException">Parameter is out of range.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
    public AsyncKeyedLocker(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
    {
        Dictionary = new AsyncKeyedLockDictionary<TKey>(concurrencyLevel, capacity, comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncKeyedLocker{TKey}" /> class, uses the specified <see cref="AsyncKeyedLockOptions"/>, has the specified concurrency level and capacity, and uses the default comparer for the key type.
    /// </summary>
    /// <param name="options">The <see cref="AsyncKeyedLockOptions"/> to use.</param>
    /// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="AsyncKeyedLocker{TKey}"/> concurrently.</param>
    /// <param name="capacity">The initial number of elements that the <see cref="AsyncKeyedLocker{TKey}"/> can contain.</param>
    /// <param name="comparer">The equality comparison implementation to use when comparing keys.</param>
    /// <exception cref="ArgumentOutOfRangeException">Parameter is out of range.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
    public AsyncKeyedLocker(AsyncKeyedLockOptions options, int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
    {
        Dictionary = new AsyncKeyedLockDictionary<TKey>(options, concurrencyLevel, capacity, comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncKeyedLocker{TKey}" /> class, uses the specified <see cref="AsyncKeyedLockOptions"/>, has the specified concurrency level and capacity, and uses the default comparer for the key type.
    /// </summary>
    /// <param name="options">The <see cref="AsyncKeyedLockOptions"/> to use.</param>
    /// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="AsyncKeyedLocker{TKey}"/> concurrently.</param>
    /// <param name="capacity">The initial number of elements that the <see cref="AsyncKeyedLocker{TKey}"/> can contain.</param>
    /// <param name="comparer">The equality comparison implementation to use when comparing keys.</param>
    /// <exception cref="ArgumentOutOfRangeException">Parameter is out of range.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
    public AsyncKeyedLocker(Action<AsyncKeyedLockOptions> options, int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
    {
        var optionsParam = new AsyncKeyedLockOptions();
        options(optionsParam);

        Dictionary = new AsyncKeyedLockDictionary<TKey>(optionsParam, concurrencyLevel, capacity, comparer);
    }

    /// <summary>
    /// Provider for <see cref="AsyncKeyedLockReleaser{TKey}"/>
    /// </summary>
    /// <param name="key">The key for which a releaser should be obtained.</param>
    /// <returns>A created or retrieved <see cref="AsyncKeyedLockReleaser{TKey}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AsyncKeyedLockReleaser<TKey> GetOrAdd(TKey key) => Dictionary.GetOrAdd(key);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Release(AsyncKeyedLockReleaser<TKey> releaser) => Dictionary.Release(releaser);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReleaseWithoutSemaphoreRelease(AsyncKeyedLockReleaser<TKey> releaser) => Dictionary.ReleaseWithoutSemaphoreRelease(releaser);


    #region Synchronous
    /// <summary>
    /// Synchronously lock based on a key.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <returns>A disposable value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Lock(TKey key)
    {
        var releaser = GetOrAdd(key);
        releaser.SemaphoreSlim.Wait();
        return releaser;
    }

    /// <summary>
    /// Synchronously lock based on a key, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A disposable value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Lock(TKey key, CancellationToken cancellationToken)
    {
        var releaser = GetOrAdd(key);
        try
        {
            releaser.SemaphoreSlim.Wait(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }
        return releaser;
    }

    /// <summary>
    /// Synchronously lock based on a key, setting a limit for the number of milliseconds to wait.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="entered">An out parameter showing whether or not the semaphore was entered.</param>
    /// <returns>A disposable value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Lock(TKey key, int millisecondsTimeout, out bool entered)
    {
        var releaser = GetOrAdd(key);
        if (millisecondsTimeout == Timeout.Infinite)
        {
            entered = true;
            releaser.SemaphoreSlim.Wait();
            return releaser;
        }
        entered = releaser.SemaphoreSlim.Wait(millisecondsTimeout);
        return new AsyncKeyedLockTimeoutReleaser<TKey>(entered, releaser);
    }

    /// <summary>
    /// Synchronously lock based on a key, setting a limit for the <see cref="TimeSpan"/> to wait.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="entered">An out parameter showing whether or not the semaphore was entered.</param>
    /// <returns>A disposable value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Lock(TKey key, TimeSpan timeout, out bool entered)
    {
        var releaser = GetOrAdd(key);
        if ((int)timeout.TotalMilliseconds == Timeout.Infinite)
        {
            entered = true;
            releaser.SemaphoreSlim.Wait();
            return releaser;
        }
        entered = releaser.SemaphoreSlim.Wait(timeout);
        return new AsyncKeyedLockTimeoutReleaser<TKey>(entered, releaser);
    }

    /// <summary>
    /// Synchronously lock based on a key, setting a limit for the number of milliseconds to wait, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="entered">An out parameter showing whether or not the semaphore was entered.</param>
    /// <returns>A disposable value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Lock(TKey key, int millisecondsTimeout, CancellationToken cancellationToken, out bool entered)
    {
        var releaser = GetOrAdd(key);
        try
        {
            if (millisecondsTimeout == Timeout.Infinite)
            {
                entered = true;
                releaser.SemaphoreSlim.Wait(cancellationToken);
                return releaser;
            }
            entered = releaser.SemaphoreSlim.Wait(millisecondsTimeout, cancellationToken);
            return new AsyncKeyedLockTimeoutReleaser<TKey>(entered, releaser);
        }
        catch (OperationCanceledException)
        {
            entered = false;
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }
    }

    /// <summary>
    /// Synchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="entered">An out parameter showing whether or not the semaphore was entered.</param>
    /// <returns>A disposable value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Lock(TKey key, TimeSpan timeout, CancellationToken cancellationToken, out bool entered)
    {
        var releaser = GetOrAdd(key);
        try
        {
            if ((int)timeout.TotalMilliseconds == Timeout.Infinite)
            {
                entered = true;
                releaser.SemaphoreSlim.Wait(cancellationToken);
                return releaser;
            }
            entered = releaser.SemaphoreSlim.Wait(timeout, cancellationToken);
            return new AsyncKeyedLockTimeoutReleaser<TKey>(entered, releaser);
        }
        catch (OperationCanceledException)
        {
            entered = false;
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }
    }
    #endregion Synchronous

    #region SynchronousTry
    /// <summary>
    /// Synchronously lock based on a key, setting a limit for the number of milliseconds to wait, and if not timed out, scynchronously execute an action and release.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="action">The synchronous action.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryLock(TKey key, Action action, int millisecondsTimeout)
    {
        var releaser = GetOrAdd(key);
        if (!releaser.SemaphoreSlim.Wait(millisecondsTimeout))
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            return false;
        }

        try
        {
            action();
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Synchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, and if not timed out, scynchronously execute an action and release.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="action">The synchronous action.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryLock(TKey key, Action action, TimeSpan timeout)
    {
        var releaser = GetOrAdd(key);
        if (!releaser.SemaphoreSlim.Wait(timeout))
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            return false;
        }

        try
        {
            action();
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Synchronously lock based on a key, setting a limit for the number of milliseconds to wait, and if not timed out, scynchronously execute an action and release, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="action">The synchronous action.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryLock(TKey key, Action action, int millisecondsTimeout, CancellationToken cancellationToken)
    {
        var releaser = GetOrAdd(key);
        try
        {
            if (!releaser.SemaphoreSlim.Wait(millisecondsTimeout, cancellationToken))
            {
                ReleaseWithoutSemaphoreRelease(releaser);
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }

        try
        {
            action();
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Synchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, and if not timed out, scynchronously execute an action and release, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="action">The synchronous action.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryLock(TKey key, Action action, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var releaser = GetOrAdd(key);
        try
        {
            if (!releaser.SemaphoreSlim.Wait(timeout, cancellationToken))
            {
                ReleaseWithoutSemaphoreRelease(releaser);
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }

        try
        {
            action();
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }
    #endregion SynchronousTry

    #region AsynchronousTry
    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the number of milliseconds to wait, and if not timed out, scynchronously execute an action and release.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="action">The synchronous action.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Action action, int millisecondsTimeout, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        if (!await releaser.SemaphoreSlim.WaitAsync(millisecondsTimeout).ConfigureAwait(continueOnCapturedContext))
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            return false;
        }

        try
        {
            action();
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the number of milliseconds to wait, and if not timed out, ascynchronously execute a <see cref="Func{Task}"/> and release.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="task">The asynchronous task.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Func<Task> task, int millisecondsTimeout, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        if (!await releaser.SemaphoreSlim.WaitAsync(millisecondsTimeout).ConfigureAwait(continueOnCapturedContext))
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            return false;
        }

        try
        {
            await task().ConfigureAwait(continueOnCapturedContext);
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, and if not timed out, scynchronously execute an action and release.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="action">The synchronous action.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Action action, TimeSpan timeout, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        if (!await releaser.SemaphoreSlim.WaitAsync(timeout).ConfigureAwait(continueOnCapturedContext))
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            return false;
        }

        try
        {
            action();
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, and if not timed out, ascynchronously execute a <see cref="Func{Task}"/> and release.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="task">The asynchronous task.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Func<Task> task, TimeSpan timeout, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        if (!await releaser.SemaphoreSlim.WaitAsync(timeout).ConfigureAwait(continueOnCapturedContext))
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            return false;
        }

        try
        {
            await task().ConfigureAwait(continueOnCapturedContext);
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the number of milliseconds to wait, and if not timed out, scynchronously execute an action and release, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="action">The synchronous action.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Action action, int millisecondsTimeout, CancellationToken cancellationToken, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        try
        {
            if (!await releaser.SemaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(continueOnCapturedContext))
            {
                ReleaseWithoutSemaphoreRelease(releaser);
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }

        try
        {
            action();
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the number of milliseconds to wait, and if not timed out, ascynchronously execute a <see cref="Func{Task}"/> and release, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="task">The asynchronous task.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Func<Task> task, int millisecondsTimeout, CancellationToken cancellationToken, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        try
        {
            if (!await releaser.SemaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(continueOnCapturedContext))
            {
                ReleaseWithoutSemaphoreRelease(releaser);
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }

        try
        {
            await task().ConfigureAwait(continueOnCapturedContext);
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, and if not timed out, scynchronously execute an action and release, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="action">The synchronous action.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Action action, TimeSpan timeout, CancellationToken cancellationToken, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        try
        {
            if (!await releaser.SemaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(continueOnCapturedContext))
            {
                ReleaseWithoutSemaphoreRelease(releaser);
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }

        try
        {
            action();
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, and if not timed out, ascynchronously execute a <see cref="Func{Task}"/> and release, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="task">The asynchronous task.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Func<Task> task, TimeSpan timeout, CancellationToken cancellationToken, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        try
        {
            if (!await releaser.SemaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(continueOnCapturedContext))
            {
                ReleaseWithoutSemaphoreRelease(releaser);
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }

        try
        {
            await task().ConfigureAwait(continueOnCapturedContext);
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }
    #endregion AsynchronousTry

    #region AsynchronousTryNet8.0
#if NET8_0_OR_GREATER
    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the number of milliseconds to wait, and if not timed out, scynchronously execute an action and release.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="action">The synchronous action.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Action action, int millisecondsTimeout, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        if (!await releaser.SemaphoreSlim.WaitAsync(millisecondsTimeout).ConfigureAwait(configureAwaitOptions))
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            return false;
        }

        try
        {
            action();
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the number of milliseconds to wait, and if not timed out, ascynchronously execute a <see cref="Func{Task}"/> and release.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="task">The asynchronous task.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Func<Task> task, int millisecondsTimeout, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        if (!await releaser.SemaphoreSlim.WaitAsync(millisecondsTimeout).ConfigureAwait(configureAwaitOptions))
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            return false;
        }

        try
        {
            await task().ConfigureAwait(configureAwaitOptions);
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, and if not timed out, scynchronously execute an action and release.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="action">The synchronous action.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Action action, TimeSpan timeout, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        if (!await releaser.SemaphoreSlim.WaitAsync(timeout).ConfigureAwait(configureAwaitOptions))
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            return false;
        }

        try
        {
            action();
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, and if not timed out, ascynchronously execute a <see cref="Func{Task}"/> and release.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="task">The asynchronous task.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Func<Task> task, TimeSpan timeout, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        if (!await releaser.SemaphoreSlim.WaitAsync(timeout).ConfigureAwait(configureAwaitOptions))
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            return false;
        }

        try
        {
            await task().ConfigureAwait(configureAwaitOptions);
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the number of milliseconds to wait, and if not timed out, scynchronously execute an action and release, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="action">The synchronous action.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Action action, int millisecondsTimeout, CancellationToken cancellationToken, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        try
        {
            if (!await releaser.SemaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(configureAwaitOptions))
            {
                ReleaseWithoutSemaphoreRelease(releaser);
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }

        try
        {
            action();
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the number of milliseconds to wait, and if not timed out, ascynchronously execute a <see cref="Func{Task}"/> and release, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="task">The asynchronous task.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Func<Task> task, int millisecondsTimeout, CancellationToken cancellationToken, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        try
        {
            if (!await releaser.SemaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(configureAwaitOptions))
            {
                ReleaseWithoutSemaphoreRelease(releaser);
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }

        try
        {
            await task().ConfigureAwait(configureAwaitOptions);
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, and if not timed out, scynchronously execute an action and release, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="action">The synchronous action.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Action action, TimeSpan timeout, CancellationToken cancellationToken, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        try
        {
            if (!await releaser.SemaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(configureAwaitOptions))
            {
                ReleaseWithoutSemaphoreRelease(releaser);
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }

        try
        {
            action();
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, and if not timed out, ascynchronously execute a <see cref="Func{Task}"/> and release, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="task">The asynchronous task.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>False if timed out, true if it successfully entered.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<bool> TryLockAsync(TKey key, Func<Task> task, TimeSpan timeout, CancellationToken cancellationToken, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        try
        {
            if (!await releaser.SemaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(configureAwaitOptions))
            {
                ReleaseWithoutSemaphoreRelease(releaser);
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }

        try
        {
            await task().ConfigureAwait(configureAwaitOptions);
        }
        finally
        {
            Release(releaser);
        }
        return true;
    }
#endif
    #endregion AsynchronousTryNet8.0

    #region Asynchronous
    /// <summary>
    /// Asynchronously lock based on a key.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>A disposable value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<IDisposable> LockAsync(TKey key, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        await releaser.SemaphoreSlim.WaitAsync().ConfigureAwait(continueOnCapturedContext);
        return releaser;
    }

    /// <summary>
    /// Asynchronously lock based on a key, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>A disposable value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<IDisposable> LockAsync(TKey key, CancellationToken cancellationToken, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        try
        {
            await releaser.SemaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext);
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }
        return releaser;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the number of milliseconds to wait.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>A disposable value of type <see cref="AsyncKeyedLockTimeoutReleaser{TKey}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<AsyncKeyedLockTimeoutReleaser<TKey>> LockAsync(TKey key, int millisecondsTimeout, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        return new AsyncKeyedLockTimeoutReleaser<TKey>(await releaser.SemaphoreSlim.WaitAsync(millisecondsTimeout).ConfigureAwait(continueOnCapturedContext), releaser);
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the <see cref="TimeSpan"/> to wait.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>A disposable value of type <see cref="AsyncKeyedLockTimeoutReleaser{TKey}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<AsyncKeyedLockTimeoutReleaser<TKey>> LockAsync(TKey key, TimeSpan timeout, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        return new AsyncKeyedLockTimeoutReleaser<TKey>(await releaser.SemaphoreSlim.WaitAsync(timeout).ConfigureAwait(continueOnCapturedContext), releaser);
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the number of milliseconds to wait, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>A disposable value of type <see cref="AsyncKeyedLockTimeoutReleaser{TKey}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<AsyncKeyedLockTimeoutReleaser<TKey>> LockAsync(TKey key, int millisecondsTimeout, CancellationToken cancellationToken, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        try
        {
            return new AsyncKeyedLockTimeoutReleaser<TKey>(await releaser.SemaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(continueOnCapturedContext), releaser);
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false. Defaults to false.</param>
    /// <returns>A disposable value of type <see cref="AsyncKeyedLockTimeoutReleaser{TKey}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<AsyncKeyedLockTimeoutReleaser<TKey>> LockAsync(TKey key, TimeSpan timeout, CancellationToken cancellationToken, bool continueOnCapturedContext = false)
    {
        var releaser = GetOrAdd(key);
        try
        {
            return new AsyncKeyedLockTimeoutReleaser<TKey>(await releaser.SemaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(continueOnCapturedContext), releaser);
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }
    }
    #endregion Asynchronous

    #region AsynchronousNet8.0
#if NET8_0_OR_GREATER
    /// <summary>
    /// Asynchronously lock based on a key.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>A disposable value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<IDisposable> LockAsync(TKey key, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        await releaser.SemaphoreSlim.WaitAsync().ConfigureAwait(configureAwaitOptions);
        return releaser;
    }

    /// <summary>
    /// Asynchronously lock based on a key, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>A disposable value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<IDisposable> LockAsync(TKey key, CancellationToken cancellationToken, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        try
        {
            await releaser.SemaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(configureAwaitOptions);
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }
        return releaser;
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the number of milliseconds to wait.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>A disposable value of type <see cref="AsyncKeyedLockTimeoutReleaser{TKey}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<AsyncKeyedLockTimeoutReleaser<TKey>> LockAsync(TKey key, int millisecondsTimeout, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        return new AsyncKeyedLockTimeoutReleaser<TKey>(await releaser.SemaphoreSlim.WaitAsync(millisecondsTimeout).ConfigureAwait(configureAwaitOptions), releaser);
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the <see cref="TimeSpan"/> to wait.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>A disposable value of type <see cref="AsyncKeyedLockTimeoutReleaser{TKey}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<AsyncKeyedLockTimeoutReleaser<TKey>> LockAsync(TKey key, TimeSpan timeout, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        return new AsyncKeyedLockTimeoutReleaser<TKey>(await releaser.SemaphoreSlim.WaitAsync(timeout).ConfigureAwait(configureAwaitOptions), releaser);
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the number of milliseconds to wait, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, <see cref="Timeout.Infinite"/> (-1) to wait indefinitely, or zero to test the state of the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>A disposable value of type <see cref="AsyncKeyedLockTimeoutReleaser{TKey}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<AsyncKeyedLockTimeoutReleaser<TKey>> LockAsync(TKey key, int millisecondsTimeout, CancellationToken cancellationToken, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        try
        {
            return new AsyncKeyedLockTimeoutReleaser<TKey>(await releaser.SemaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(configureAwaitOptions), releaser);
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously lock based on a key, setting a limit for the <see cref="System.TimeSpan"/> to wait, while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="key">The key to lock on.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely, or a <see cref="TimeSpan"/> that represents 0 milliseconds to test the wait handle and return immediately.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
    /// <param name="configureAwaitOptions">Options used to configure how awaits on this task are performed.</param>
    /// <returns>A disposable value of type <see cref="AsyncKeyedLockTimeoutReleaser{TKey}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<AsyncKeyedLockTimeoutReleaser<TKey>> LockAsync(TKey key, TimeSpan timeout, CancellationToken cancellationToken, ConfigureAwaitOptions configureAwaitOptions)
    {
        var releaser = GetOrAdd(key);
        try
        {
            return new AsyncKeyedLockTimeoutReleaser<TKey>(await releaser.SemaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(configureAwaitOptions), releaser);
        }
        catch (OperationCanceledException)
        {
            ReleaseWithoutSemaphoreRelease(releaser);
            throw;
        }
    }
#endif
    #endregion AsynchronousNet8.0

    /// <summary>
    /// Checks whether or not there is a thread making use of a keyed lock.
    /// </summary>
    /// <param name="key">The key requests are locked on.</param>
    /// <returns><see langword="true"/> if the key is in use; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsInUse(TKey key)
    {
        if (!Dictionary.TryGetValue(key, out var result))
        {
            return false;
        }
        Monitor.Enter(result);
        if (result.IsNotInUse)
        {
            Monitor.Exit(result);
            return false;
        }
        if (Dictionary.PoolingEnabled && !result.Key.Equals(key))
        {
            Monitor.Exit(result);
            return false;
        }
        Monitor.Exit(result);
        return true;
    }


    /// <summary>
    /// Get the number of requests concurrently locked for a given key.
    /// </summary>
    /// <param name="key">The key requests are locked on.</param>
    /// <returns>The number of requests concurrently locked for a given key.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetRemainingCount(TKey key)
    {
        if (Dictionary.TryGetValue(key, out var releaser))
        {
            return releaser.ReferenceCount;
        }
        return 0;
    }

    /// <summary>
    /// Get the number of remaining threads that can enter the lock for a given key.
    /// </summary>
    /// <param name="key">The key requests are locked on.</param>
    /// <returns>The number of remaining threads that can enter the lock for a given key.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetCurrentCount(TKey key)
    {
        return MaxCount - GetRemainingCount(key);
    }

    /// <summary>
    /// Disposes the AsyncKeyedLocker.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        Dictionary.Dispose();
    }
}
