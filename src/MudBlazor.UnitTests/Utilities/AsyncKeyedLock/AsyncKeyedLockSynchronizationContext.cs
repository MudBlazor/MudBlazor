// Copyright (c) 2023 Mark Cilia Vincenti (original source code: https://github.com/MarkCiliaVincenti/AsyncKeyedLock)
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;

namespace MudBlazor.UnitTests.Utilities.AsyncKeyedLock;

#nullable enable
public class AsyncKeyedLockSynchronizationContext : SynchronizationContext
{
    public int LastPostThreadId { get; private set; }

    public override void Post(SendOrPostCallback d, object? state)
    {
        LastPostThreadId = Environment.CurrentManagedThreadId;
        d(state);
    }
}
