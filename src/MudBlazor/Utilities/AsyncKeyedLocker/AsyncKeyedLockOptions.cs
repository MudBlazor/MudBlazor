// Copyright (c) 2023 Mark Cilia Vincenti
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities.AsyncKeyedLocker;

#nullable enable
/// <summary>
/// Options for the <see cref="AsyncKeyedLocker"/> constructors
/// </summary>
internal sealed class AsyncKeyedLockOptions
{
    /// <summary>
    /// The maximum number of requests for the semaphore that can be granted concurrently. Defaults to 1.
    /// </summary>
    public int MaxCount { get; set; }

    /// <summary>
    /// The size of the pool to use in order for generated objects to be reused. This is NOT a concurrency limit,
    /// but if the pool is empty then a new object will be created rather than waiting for an object to return to
    /// the pool. Defaults to 0 (disabled) but strongly recommended to use.
    /// </summary>
    public int PoolSize { get; set; }

    /// <summary>
    /// The number of items to fill the pool with during initialization. Defaults to -1 (fill up to pool size).
    /// </summary>
    public int PoolInitialFill { get; set; }

    /// <summary>
    /// Initializes options for the <see cref="AsyncKeyedLocker"/> constructors
    /// </summary>
    /// <param name="maxCount">The maximum number of requests for the semaphore that can be granted concurrently. Defaults to 1.</param>
    /// <param name="poolSize">The size of the pool to use in order for generated objects to be reused. This is NOT a concurrency limit,
    /// but if the pool is empty then a new object will be created rather than waiting for an object to return to
    /// the pool. Defaults to 0 (disabled) but strongly recommended to use.</param>
    /// <param name="poolInitialFill">The number of items to fill the pool with during initialization. Defaults to -1 (fill up to pool size).</param>
    public AsyncKeyedLockOptions(int maxCount = 1, int poolSize = 0, int poolInitialFill = -1)
    {
        MaxCount = maxCount;
        PoolSize = poolSize;
        PoolInitialFill = poolInitialFill;
    }
}
