// Copyright (c) 2023 Mark Cilia Vincenti
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MudBlazor.Utilities.AsyncKeyedLocker;

#nullable enable
internal sealed class AsyncKeyedLockPool<TKey> : IDisposable where TKey : notnull
{
    private readonly int _capacity;
    private readonly IList<AsyncKeyedLockReleaser<TKey>> _objects;
    private readonly Func<TKey, AsyncKeyedLockReleaser<TKey>> _objectGenerator;

    public AsyncKeyedLockPool(AsyncKeyedLockDictionary<TKey> asyncKeyedLockDictionary, int capacity, int initialFill = -1)
    {
        _capacity = capacity;
        _objects = new List<AsyncKeyedLockReleaser<TKey>>(capacity);
        _objectGenerator = key => new AsyncKeyedLockReleaser<TKey>(
            key,
            new SemaphoreSlim(asyncKeyedLockDictionary.MaxCount, asyncKeyedLockDictionary.MaxCount),
            asyncKeyedLockDictionary);

        if (initialFill < 0)
        {
            for (var i = 0; i < capacity; ++i)
            {
                var releaser = _objectGenerator(default!);
                releaser.IsNotInUse = true;
                _objects.Add(releaser);
            }
        }
        else
        {
            initialFill = Math.Min(initialFill, capacity);
            for (var i = 0; i < initialFill; ++i)
            {
                var releaser = _objectGenerator(default!);
                releaser.IsNotInUse = true;
                _objects.Add(releaser);
            }
        }
    }

    public void Dispose()
    {
        _objects.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AsyncKeyedLockReleaser<TKey> GetObject(TKey key)
    {
        Monitor.Enter(_objects);
        if (_objects.Count > 0)
        {
            var lastPos = _objects.Count - 1;
            var item = _objects[lastPos];
            _objects.RemoveAt(lastPos);
            Monitor.Exit(_objects);
            item.Key = key;
            item.IsNotInUse = false;
            return item;
        }
        Monitor.Exit(_objects);

        return _objectGenerator(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PutObject(AsyncKeyedLockReleaser<TKey> item)
    {
        Monitor.Enter(_objects);
        if (_objects.Count < _capacity)
        {
            _objects.Add(item);
        }
        Monitor.Exit(_objects);
    }
}
