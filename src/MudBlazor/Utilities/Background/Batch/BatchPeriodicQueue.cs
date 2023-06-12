// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MudBlazor.Utilities.Background.Batch;

#nullable enable
/// <summary>
/// Represents a batch periodic queue for managing items of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of items in the queue.</typeparam>
internal class BatchPeriodicQueue<T> : BackgroundWorkerBase
{
    private readonly ConcurrentQueue<T> _items;
    private readonly PeriodicTimer _periodicTimer;
    private readonly IBatchTimerHandler<T> _handler;
    private readonly bool _tickOnDispose;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchPeriodicQueue{T}"/> class with the specified batch timer handler and period.
    /// </summary>
    /// <param name="handler">The batch timer handler.</param>
    /// <param name="period">The time period for triggering batch execution.</param>
    /// <param name="tickOnDispose">>Specifies whether to trigger a guaranteed <see cref="IBatchTimerHandler{T}.OnBatchTimerElapsedAsync"/> when calling <see cref="DisposeAsync"/>.</param>
    public BatchPeriodicQueue(IBatchTimerHandler<T> handler, TimeSpan period, bool tickOnDispose = true)
    {
        _handler = handler;
        _tickOnDispose = tickOnDispose;
        _items = new ConcurrentQueue<T>();
        _periodicTimer = new PeriodicTimer(period);
    }

    /// <summary>
    /// Enqueues an item to the batch queue.
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    public void QueueItem(T item) => _items.Enqueue(item);

    /// <summary>
    /// Gets the count of items in the batch queue.
    /// </summary>
    public int Count => _items.Count;

    /// <inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        _periodicTimer.Dispose();
        await base.DisposeAsync();

        if (_tickOnDispose)
        {
            //If there is anything left over in the list we trigger so the handler could do something with this for example cleanup
            await OnBatchTimerElapsedAsync();
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _periodicTimer.WaitForNextTickAsync(stoppingToken))
        {
            await OnBatchTimerElapsedAsync(stoppingToken);
        }
    }

    private Task OnBatchTimerElapsedAsync(CancellationToken stoppingToken = default)
    {
        var tempList = new List<T>();
        while (_items.TryDequeue(out var item))
        {
            tempList.Add(item);
        }

        return _handler.OnBatchTimerElapsedAsync(tempList, stoppingToken);
    }
}
