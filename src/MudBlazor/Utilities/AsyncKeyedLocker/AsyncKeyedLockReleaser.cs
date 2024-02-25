﻿// Copyright (c) 2023 Mark Cilia Vincenti
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MudBlazor.Utilities.AsyncKeyedLocker
{
    /// <summary>
    /// Represents an <see cref="IDisposable"/> for AsyncKeyedLock.
    /// </summary>
    internal sealed class AsyncKeyedLockReleaser<TKey> : IDisposable
    {
        private TKey _key;
        private int _referenceCount = 1;
        private readonly AsyncKeyedLockDictionary<TKey> _dictionary;

        internal bool IsNotInUse { get; set; } = false;

        /// <summary>
        /// The key used for locking.
        /// </summary>
        public TKey Key
        {
            get => _key;
            internal set => _key = value;
        }

        /// <summary>
        /// The number of threads processing or waiting to process for the specific <see cref="Key"/>.
        /// </summary>
        public int ReferenceCount
        {
            get => _referenceCount;
            internal set => _referenceCount = value;
        }

        /// <summary>
        /// The exposed <see cref="SemaphoreSlim"/> instance used to limit the number of threads that can access the lock concurrently.
        /// </summary>
        public SemaphoreSlim SemaphoreSlim { get; }

        internal AsyncKeyedLockReleaser(TKey key, SemaphoreSlim semaphoreSlim, AsyncKeyedLockDictionary<TKey> dictionary)
        {
            _key = key;
            SemaphoreSlim = semaphoreSlim;
            _dictionary = dictionary;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryIncrement(TKey key)
        {
            if (Monitor.TryEnter(this))
            {
                if (IsNotInUse || !_key.Equals(key)) // rare race condition
                {
                    Monitor.Exit(this);
                    return false;
                }
                ++_referenceCount;
                Monitor.Exit(this);
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryIncrementNoPooling()
        {
            if (Monitor.TryEnter(this))
            {
                if (IsNotInUse) // rare race condition
                {
                    Monitor.Exit(this);
                    return false;
                }
                ++_referenceCount;
                Monitor.Exit(this);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Releases the <see cref="SemaphoreSlim"/> object once.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            _dictionary.Release(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Dispose(bool enteredSemaphore)
        {
            if (enteredSemaphore)
            {
                _dictionary.Release(this);
            }
            else
            {
                _dictionary.ReleaseWithoutSemaphoreRelease(this);
            }
        }
    }
}
