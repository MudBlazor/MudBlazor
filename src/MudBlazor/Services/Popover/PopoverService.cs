// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.Utilities.AsyncKeyedLock;
using MudBlazor.Utilities.Background.Batch;
using MudBlazor.Utilities.ObserverManager;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a service for managing popovers.
/// </summary>
internal class PopoverService : IPopoverService, IBatchTimerHandler<MudPopoverHolder>
{
    private bool _disposed;
    private bool _isInitializing;
    private readonly PopoverJsInterop _popoverJsInterop;
    private readonly AsyncKeyedLocker<Guid> _popoverSemaphore;
    private readonly Dictionary<Guid, MudPopoverHolder> _holders;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly BatchPeriodicQueue<MudPopoverHolder> _batchExecutor;
    private readonly ObserverManager<Guid, IPopoverObserver> _observerManager;

    /// <inheritdoc />
    public IEnumerable<IMudPopoverHolder> ActivePopovers => _holders.Values;

    /// <inheritdoc />
    public bool IsInitialized { get; private set; }

    /// <inheritdoc />
    public PopoverOptions PopoverOptions { get; }

    /// <summary>
    /// Gets the number of items currently queued in the <see cref="_batchExecutor"/> for processing in the <see cref="OnBatchTimerElapsedAsync"/> method.
    /// </summary>
    /// <remarks>
    /// This property is not exposed in the public API of the <see cref="IPopoverService"/> interface and is intended for internal use only.
    /// </remarks>
    public int QueueCount => _batchExecutor.Count;

    /// <summary>
    /// Gets the number of observers.
    /// </summary>
    /// <remarks>
    /// This property is not exposed in the public API of the <see cref="IPopoverService"/> interface and is intended for internal use only.
    /// </remarks>
    public int ObserversCount => _observerManager.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="PopoverService"/> class.
    /// </summary>
    /// <param name="logger">The logger used for logging.</param>
    /// <param name="jsInterop">Instance of a JavaScript runtime to calls are dispatched.</param>
    /// <param name="options">The options for the popover service (optional).</param>
    public PopoverService(ILogger<PopoverService> logger, IJSRuntime jsInterop, IOptions<PopoverOptions>? options = null)
    {
        PopoverOptions = options?.Value ?? new PopoverOptions();
        _popoverSemaphore = new AsyncKeyedLocker<Guid>(lockOptions =>
        {
            lockOptions.PoolSize = PopoverOptions.PoolSize;
            lockOptions.PoolInitialFill = PopoverOptions.PoolInitialFill;
        });
        _holders = new Dictionary<Guid, MudPopoverHolder>();
        _cancellationTokenSource = new CancellationTokenSource();
        _popoverJsInterop = new PopoverJsInterop(jsInterop);
        _batchExecutor = new BatchPeriodicQueue<MudPopoverHolder>(this, PopoverOptions.QueueDelay);
        _observerManager = new ObserverManager<Guid, IPopoverObserver>(logger);
    }

    /// <inheritdoc />
    public void Subscribe(IPopoverObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        if (_disposed)
        {
            return;
        }

        _observerManager.Subscribe(observer.Id, observer);
    }

    /// <inheritdoc />
    public void Unsubscribe(IPopoverObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _observerManager.Unsubscribe(observer.Id);
    }

    /// <inheritdoc />
    public async Task CreatePopoverAsync(IPopover popover)
    {
        ArgumentNullException.ThrowIfNull(popover);

        if (_disposed)
        {
            // Do not accept new popover when service is disposed, they are getting cleared.
            return;
        }

        var holder = new MudPopoverHolder(popover.Id)
            .SetFragment(popover.ChildContent)
            .SetClass(popover.PopoverClass)
            .SetStyle(popover.PopoverStyles)
            .SetShowContent(popover.Open)
            .SetTag(popover.Tag)
            .SetUserAttributes(popover.UserAttributes);

        _holders.TryAdd(holder.Id, holder);
        await _observerManager.NotifyAsync(observer => observer.PopoverCollectionUpdatedNotificationAsync(new PopoverHolderContainer(PopoverHolderOperation.Create, new[] { holder }), _cancellationTokenSource.Token));
    }

    /// <inheritdoc />
    public async Task<bool> UpdatePopoverAsync(IPopover popover)
    {
        ArgumentNullException.ThrowIfNull(popover);

        if (_disposed)
        {
            // Do not update popover when service is disposed, they are getting cleared.
            return false;
        }

        // We initialize the service regardless of whether the popover exists or not.
        // Adding it in an if-clause doesn't provide significant benefits.
        // Instead, we prioritize ensuring that the service is ready for use, as its initialization is a one-time operation.
        await InitializeServiceIfNeededAsync();
        if (!_holders.TryGetValue(popover.Id, out var holder))
        {
            return false;
        }

        // It's a legacy thing that should be removed, new popover doesn't need this.
        if (holder.IsDetached)
        {
            return false;
        }

        // Do not put after the semaphore as it can cause deadlock
        await InitializePopoverIfNeededAsync(holder);

        using (await _popoverSemaphore.LockAsync(popover.Id, _cancellationTokenSource.Token))
        {
            holder
                .SetFragment(popover.ChildContent)
                .SetClass(popover.PopoverClass)
                .SetStyle(popover.PopoverStyles)
                .SetShowContent(popover.Open)
                .SetTag(popover.Tag)
                .SetUserAttributes(popover.UserAttributes);

            await _observerManager.NotifyAsync(observer => observer.PopoverCollectionUpdatedNotificationAsync(new PopoverHolderContainer(PopoverHolderOperation.Update, new[] { holder }), _cancellationTokenSource.Token));

            return true;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DestroyPopoverAsync(IPopover popover)
    {
        ArgumentNullException.ThrowIfNull(popover);

        if (_disposed)
        {
            return false;
        }

        // We initialize the service regardless of whether the popover exists or not.
        // Adding it in an if-clause doesn't provide significant benefits.
        // Instead, we prioritize ensuring that the service is ready for use, as its initialization is a one-time operation.
        await InitializeServiceIfNeededAsync();

        return await DestroyPopoverByIdAsync(popover.Id);
    }

    /// <inheritdoc />
    public async ValueTask<int> GetProviderCountAsync()
    {
        var (success, value) = await _popoverJsInterop.CountProviders(_cancellationTokenSource.Token);

        return success ? value : 0;
    }

    /// <inheritdoc />
    public virtual Task OnBatchTimerElapsedAsync(IReadOnlyCollection<MudPopoverHolder> items, CancellationToken stoppingToken)
    {
        // In our case we do not care if the cancellation token in requested, we should not interrupt the process and just detach to clean-up resources.
        // In the future, there might be a requirement to split the jobs and introduce a change where instead of using IReadOnlyCollection<MudPopoverHolder>,
        // we would utilize IReadOnlyCollection<PopoverQueueContainer>. This new collection would consist of various operations, such as detaching items, rendering items,
        // and triggering the PopoverCollectionUpdatedNotification, among others.
        return DetachRangeAsync(items);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return DisposeAsyncCore();
    }

    /// <summary>
    /// Disposes the current <see cref="PopoverService"/> instance.
    /// </summary>
    private async ValueTask DisposeAsyncCore()
    {
        if (!_disposed)
        {
            _disposed = true;
            // ReSharper disable once MethodHasAsyncOverload - not available in .NET6 & .NET7
            _cancellationTokenSource.Cancel();
            await DestroyPopoversQuick();

            _batchExecutor.Dispose();

            // In case someone has custom implementation and didn't unsubscribe
            _observerManager.Clear();

            _popoverSemaphore.Dispose();

            // https://github.com/MudBlazor/MudBlazor/pull/5367#issuecomment-1258649968
            // Fixed in NET8
            // Do not send CancellationToken as it was cancelled.
#pragma warning disable CA2012 // Use ValueTasks correctly
            _ = _popoverJsInterop.Dispose();
#pragma warning restore CA2012 // Use ValueTasks correctly

            _cancellationTokenSource.Dispose();
        }
    }

    private Task DestroyPopoversQuick()
    {
        var holdersListCopy = new List<MudPopoverHolder>(_holders.Values);
        _holders.Clear();

        holdersListCopy.ForEach(holder => holder.IsDetached = true);

        return _observerManager.NotifyAsync(observer => observer.PopoverCollectionUpdatedNotificationAsync(new PopoverHolderContainer(PopoverHolderOperation.Remove, holdersListCopy), _cancellationTokenSource.Token));
    }

    private async Task<bool> DestroyPopoverByIdAsync(Guid id, bool queueForDisconnect = true)
    {
        if (!_holders.Remove(id, out var holder))
        {
            return false;
        }

        if (queueForDisconnect)
        {
            _batchExecutor.QueueItem(holder);
        }
        // Although it is not completely detached from the JS side until OnBatchTimerElapsedAsync fires, we mark it as "Detached"
        // because we want to let know the UpdatePopoverAsync method that there is no need to update it anymore,
        // as it is no longer being rendered by MudPopoverProvider since it has been removed from the ActivePopovers collection.
        // Perhaps we could consider adding a state indicating that the object is queued for detaching instead.
        holder.IsDetached = true;

        await _observerManager.NotifyAsync(observer => observer.PopoverCollectionUpdatedNotificationAsync(new PopoverHolderContainer(PopoverHolderOperation.Remove, new[] { holder }), _cancellationTokenSource.Token));

        return true;
    }

    private async Task DetachRangeAsync(IReadOnlyCollection<MudPopoverHolder> holders)
    {
        if (_disposed)
        {
            return;
        }

        foreach (var holder in holders)
        {
            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }

            using (await _popoverSemaphore.LockAsync(holder.Id, _cancellationTokenSource.Token))
            {
                try
                {
                    holder.IsDetached = true;
                    if (holder.IsConnected)
                    {
                        await _popoverJsInterop.Disconnect(holder.Id, _cancellationTokenSource.Token);
                    }
                }
                finally
                {
                    holder.IsConnected = false;
                }
            }
        }
    }

    private async Task InitializePopoverIfNeededAsync(MudPopoverHolder holder)
    {
        if (holder.IsConnected)
        {
            return;
        }

        using (await _popoverSemaphore.LockAsync(holder.Id, _cancellationTokenSource.Token))
        {
            // Double-check if IsConnected has been completed by another thread.
            if (holder.IsConnected || holder.IsDetached)
            {
                return;
            }

            holder.IsConnected = await _popoverJsInterop.Connect(holder.Id, _cancellationTokenSource.Token);
        }
    }

    private async Task InitializeServiceIfNeededAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        if (_isInitializing)
        {
            return;
        }

        try
        {
            _isInitializing = true;

            // Double-check if initialization has been completed by another thread.
            if (IsInitialized)
            {
                return;
            }

            await _popoverJsInterop.Initialize(PopoverOptions.ContainerClass, PopoverOptions.FlipMargin, _cancellationTokenSource.Token);
            // Starts in background
            await _batchExecutor.StartAsync(_cancellationTokenSource.Token);
            IsInitialized = true;
        }
        finally
        {
            _isInitializing = false;
        }
    }
}
