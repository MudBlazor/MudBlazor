// Copyright (c) 2023 Mark Cilia Vincenti
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

// ReSharper disable LocalizableElement
namespace MudBlazor.Utilities.AsyncKeyedLocker;

#nullable enable
internal sealed class AsyncKeyedLockDictionary<TKey> : ConcurrentDictionary<TKey, AsyncKeyedLockReleaser<TKey>>, IDisposable where TKey : notnull
{
    public int MaxCount { get; } = 1;

    internal AsyncKeyedLockPool<TKey>? Pool { get; }

    [MemberNotNullWhen(true, nameof(Pool))]
    internal bool PoolingEnabled { get; }

    public AsyncKeyedLockDictionary()
    {
    }

    public AsyncKeyedLockDictionary(AsyncKeyedLockOptions options)
    {
        if (options.MaxCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), options.MaxCount, $"{nameof(options.MaxCount)} should be greater than or equal to 1.");
        }

        MaxCount = options.MaxCount;
        if (options.PoolSize > 0)
        {
            PoolingEnabled = true;
            Pool = new AsyncKeyedLockPool<TKey>(this, options.PoolSize, options.PoolInitialFill);
        }
    }

    public AsyncKeyedLockDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
    {
    }

    public AsyncKeyedLockDictionary(AsyncKeyedLockOptions options, IEqualityComparer<TKey> comparer) : base(comparer)
    {
        if (options.MaxCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), options.MaxCount, $"{nameof(options.MaxCount)} should be greater than or equal to 1.");
        }

        MaxCount = options.MaxCount;
        if (options.PoolSize > 0)
        {
            PoolingEnabled = true;
            Pool = new AsyncKeyedLockPool<TKey>(this, options.PoolSize, options.PoolInitialFill);
        }
    }

    public AsyncKeyedLockDictionary(int concurrencyLevel, int capacity) : base(concurrencyLevel, capacity)
    {
    }

    public AsyncKeyedLockDictionary(AsyncKeyedLockOptions options, int concurrencyLevel, int capacity) : base(concurrencyLevel, capacity)
    {
        if (options.MaxCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), options.MaxCount, $"{nameof(options.MaxCount)} should be greater than or equal to 1.");
        }

        MaxCount = options.MaxCount;
        if (options.PoolSize > 0)
        {
            PoolingEnabled = true;
            Pool = new AsyncKeyedLockPool<TKey>(this, options.PoolSize, options.PoolInitialFill);
        }
    }

    public AsyncKeyedLockDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer) : base(concurrencyLevel, capacity, comparer)
    {
    }

    public AsyncKeyedLockDictionary(AsyncKeyedLockOptions options, int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer) : base(concurrencyLevel, capacity, comparer)
    {
        if (options.MaxCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), options.MaxCount, $"{nameof(options.MaxCount)} should be greater than or equal to 1.");
        }

        MaxCount = options.MaxCount;
        if (options.PoolSize > 0)
        {
            PoolingEnabled = true;
            Pool = new AsyncKeyedLockPool<TKey>(this, options.PoolSize, options.PoolInitialFill);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AsyncKeyedLockReleaser<TKey> GetOrAdd(TKey key)
    {
        if (PoolingEnabled)
        {
            if (TryGetValue(key, out var releaser) && releaser.TryIncrement(key))
            {
                return releaser;
            }

            var releaserToAdd = Pool.GetObject(key);
            if (TryAdd(key, releaserToAdd))
            {
                return releaserToAdd;
            }

            while (true)
            {
                releaser = GetOrAdd(key, releaserToAdd);
                if (ReferenceEquals(releaser, releaserToAdd))
                {
                    return releaser;
                }
                if (releaser.TryIncrement(key))
                {
                    releaserToAdd.IsNotInUse = true;
                    Pool.PutObject(releaserToAdd);
                    return releaser;
                }
            }
        }

        if (TryGetValue(key, out var releaserNoPooling) && releaserNoPooling.TryIncrementNoPooling())
        {
            return releaserNoPooling;
        }

        var releaserToAddNoPooling = new AsyncKeyedLockReleaser<TKey>(key, new SemaphoreSlim(MaxCount, MaxCount), this);
        if (TryAdd(key, releaserToAddNoPooling))
        {
            return releaserToAddNoPooling;
        }

        while (true)
        {
            releaserNoPooling = GetOrAdd(key, releaserToAddNoPooling);
            if (ReferenceEquals(releaserNoPooling, releaserToAddNoPooling) || releaserNoPooling.TryIncrementNoPooling())
            {
                return releaserNoPooling;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Release(AsyncKeyedLockReleaser<TKey> releaser)
    {
        Monitor.Enter(releaser);

        if (releaser.ReferenceCount == 1)
        {
            TryRemove(releaser.Key, out _);
            releaser.IsNotInUse = true;
            Monitor.Exit(releaser);
            if (PoolingEnabled)
            {
                Pool.PutObject(releaser);
            }
            releaser.SemaphoreSlim.Release();
            return;
        }

        --releaser.ReferenceCount;
        Monitor.Exit(releaser);
        releaser.SemaphoreSlim.Release();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReleaseWithoutSemaphoreRelease(AsyncKeyedLockReleaser<TKey> releaser)
    {
        Monitor.Enter(releaser);

        if (releaser.ReferenceCount == 1)
        {
            TryRemove(releaser.Key, out _);
            releaser.IsNotInUse = true;
            Monitor.Exit(releaser);
            if (PoolingEnabled)
            {
                Pool.PutObject(releaser);
            }
            return;
        }
        --releaser.ReferenceCount;
        Monitor.Exit(releaser);
    }

    public void Dispose()
    {
        foreach (var semaphore in Values)
        {
            try
            {
                semaphore.Dispose();
            }
            catch
            {
                // do nothing
            }
        }
        Clear();
        if (PoolingEnabled)
        {
            Pool.Dispose();
        }
    }
}
