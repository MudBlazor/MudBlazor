// Copyright (c) 2023 Mark Cilia Vincenti
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MudBlazor.Utilities.AsyncKeyedLocker;

#nullable enable
/// <summary>
/// Represents an <see cref="IDisposable"/> for AsyncKeyedLock with timeouts.
/// </summary>
internal sealed class AsyncKeyedLockTimeoutReleaser<TKey> : IDisposable where TKey : notnull
{
    /// <summary>
    /// True if the timeout was reached, false if not.
    /// </summary>
    public bool EnteredSemaphore { get; internal set; }

    internal readonly AsyncKeyedLockReleaser<TKey> Releaser;

    internal AsyncKeyedLockTimeoutReleaser(bool enteredSemaphore, AsyncKeyedLockReleaser<TKey> releaser)
    {
        EnteredSemaphore = enteredSemaphore;
        Releaser = releaser;
    }

    /// <summary>
    /// Releases the <see cref="SemaphoreSlim"/> object once, depending on whether or not the semaphore was entered.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        Releaser.Dispose(EnteredSemaphore);
    }
}
