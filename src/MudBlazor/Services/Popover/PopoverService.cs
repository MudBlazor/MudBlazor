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
using MudBlazor.Utilities.Background.Batch;
using MudBlazor.Utilities.ObserverManager;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a service for managing popovers.
/// </summary>
internal class PopoverService : IPopoverService, IBatchTimerHandler<MudPopoverHolder>
{
    private readonly SemaphoreSlim _semaphore;
    private readonly Dictionary<Guid, MudPopoverHolder> _holders;
    private readonly BatchPeriodicQueue<MudPopoverHolder> _batchExecutor;
    private readonly ObserverManager<Guid, IPopoverObserver> _observerManager;
    private readonly PopoverJsInterop _popoverJsInterop;

    /// <inheritdoc />
    public IEnumerable<IMudPopoverHolder> ActivePopovers => _holders.Values;

    /// <inheritdoc />
    public bool IsInitialized { get; private set; }

    /// <inheritdoc />
    public PopoverOptions PopoverOptions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PopoverService"/> class.
    /// </summary>
    /// <param name="logger">The logger used for logging.</param>
    /// <param name="jsInterop">Instance of a JavaScript runtime to calls are dispatched.</param>
    /// <param name="options">The options for the popover service (optional).</param>
    public PopoverService(ILogger<PopoverService> logger, IJSRuntime jsInterop, IOptions<PopoverOptions>? options = null)
    {
        PopoverOptions = options?.Value ?? new PopoverOptions();
        _semaphore = new SemaphoreSlim(1, 1);
        _holders = new Dictionary<Guid, MudPopoverHolder>();
        _popoverJsInterop = new PopoverJsInterop(jsInterop);
        _batchExecutor = new BatchPeriodicQueue<MudPopoverHolder>(this, PopoverOptions.QueueDelay);
        _observerManager = new ObserverManager<Guid, IPopoverObserver>(logger);
    }

    /// <inheritdoc />
    public void Subscribe(IPopoverObserver observer)
    {
        _observerManager.Subscribe(observer.Id, observer);
    }

    /// <inheritdoc />
    public void Unsubscribe(IPopoverObserver observer)
    {
        _observerManager.Unsubscribe(observer.Id);
    }

    /// <inheritdoc />
    public async Task CreatePopoverAsync(IPopover popover)
    {
        ArgumentNullException.ThrowIfNull(popover);

        await InitializeServiceIfNeededAsync();
        var holder = new MudPopoverHolder(popover.Id)
            .SetFragment(popover.ChildContent)
            .SetClass(popover.PopoverClass)
            .SetStyle(popover.PopoverStyles)
            .SetShowContent(popover.Open)
            .SetTag(popover.Tag)
            .SetUserAttributes(popover.UserAttributes);

        _holders.TryAdd(holder.Id, holder);
        await _observerManager.NotifyAsync(observer => observer.PopoverCollectionUpdatedNotificationAsync(new PopoverHolderContainer(PopoverHolderOperation.Create, new[] { holder })));
    }

    /// <inheritdoc />
    public async Task<bool> UpdatePopoverAsync(IPopover popover)
    {
        ArgumentNullException.ThrowIfNull(popover);

        //We initialize the service regardless of whether the popover exists or not.
        //Adding it in an if clause doesn't provide significant benefits.
        //Instead, we prioritize ensuring that the service is ready for use, as its initialization is a one-time operation.
        await InitializeServiceIfNeededAsync();
        if (!_holders.TryGetValue(popover.Id, out var holder))
        {
            return false;
        }

        //Do not put after the semaphore as it can cause deadlock
        await InitializePopoverIfNeededAsync(holder);

        if (holder.IsDetached)
        {
            return false;
        }

        try
        {
            await _semaphore.WaitAsync();

            holder
                .SetFragment(popover.ChildContent)
                .SetClass(popover.PopoverClass)
                .SetStyle(popover.PopoverStyles)
                .SetShowContent(popover.Open)
                .SetTag(popover.Tag)
                .SetUserAttributes(popover.UserAttributes);

            await _observerManager.NotifyAsync(observer => observer.PopoverCollectionUpdatedNotificationAsync(new PopoverHolderContainer(PopoverHolderOperation.Update, new[] { holder })));

            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task<bool> DestroyPopoverAsync(IPopover popover)
    {
        ArgumentNullException.ThrowIfNull(popover);

        //We initialize the service regardless of whether the popover exists or not.
        //Adding it in an if clause doesn't provide significant benefits.
        //Instead, we prioritize ensuring that the service is ready for use, as its initialization is a one-time operation.
        await InitializeServiceIfNeededAsync();

        return await DestroyPopoverByIdAsync(popover.Id);
    }

    /// <inheritdoc />
    public async ValueTask<int> GetProviderCountAsync()
    {
        await InitializeServiceIfNeededAsync();

        var (success, value) = await _popoverJsInterop.CountProviders();

        return success ? value : 0;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!IsInitialized)
        {
            return;
        }

        //Queue all lefts overs.
        foreach (var holderKeyValuePair in _holders)
        {
            await DestroyPopoverByIdAsync(holderKeyValuePair.Key);
        }

        //Cleanup and call the OnBatchTimerElapsedAsync with everything what's left.
        await _batchExecutor.DisposeAsync();

        _ = _popoverJsInterop.Dispose();
    }

    /// <inheritdoc />
    public virtual Task OnBatchTimerElapsedAsync(IReadOnlyCollection<MudPopoverHolder> items, CancellationToken stoppingToken)
    {
        //In our case we do not care if the cancellation token in requested, we should not interrupt the process and and just detach to cleanup resources.
        //In the future, there might be a requirement to split the jobs and introduce a change where instead of using IReadOnlyCollection<MudPopoverHolder>,
        //we would utilize IReadOnlyCollection<PopoverQueueContainer>. This new collection would consist of various operations, such as detaching items, rendering items,
        //and triggering the PopoverCollectionUpdatedNotification, among others.
        return DetachRange(items);
    }

    private async Task<bool> DestroyPopoverByIdAsync(Guid id)
    {
        if (!_holders.Remove(id, out var holder))
        {
            return false;
        }

        _batchExecutor.QueueItem(holder);
        // Although it is not completely detached from the JS side until OnBatchTimerElapsedAsync fires, we mark it as "Detached"
        // because we want let know the UpdatePopoverAsync method that there is no need to update it anymore,
        // as it is no longer being rendered by MudPopoverProvider since it has been removed from the ActivePopovers collection.
        // Perhaps we could consider adding a state indicating that the object is queued for detaching instead.
        holder.IsDetached = true;

        await _observerManager.NotifyAsync(observer => observer.PopoverCollectionUpdatedNotificationAsync(new PopoverHolderContainer(PopoverHolderOperation.Remove, new[] { holder })));

        return true;
    }

    private async Task DetachRange(IReadOnlyCollection<MudPopoverHolder> holders)
    {
        //Ignore task if zero items in collection to not enter in the semaphore
        if (holders.Count == 0)
        {
            return;
        }

        try
        {
            await _semaphore.WaitAsync();
            foreach (var holder in holders)
            {
                try
                {
                    holder.IsDetached = true;
                    if (holder.IsConnected)
                    {
                        await _popoverJsInterop.Disconnect(holder.Id);
                    }
                }
                finally
                {
                    holder.IsConnected = false;
                }
            }

        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task InitializePopoverIfNeededAsync(MudPopoverHolder holder)
    {
        if (holder.IsConnected || holder.IsDetached)
        {
            return;
        }

        try
        {
            await _semaphore.WaitAsync();

            if (holder.IsConnected || holder.IsDetached)
            {
                //It is not redundant to include a check before and after the semaphore.
                //If we call InitializePopoverIfNeededAsync multiple times in parallel in the background,
                //it may lead to double connection, which is undesired.
                //The initial check helps to prevent double initialization of the popover.
                return;
            }

            holder.IsConnected = await _popoverJsInterop.Connect(holder.Id);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task InitializeServiceIfNeededAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        try
        {
            await _semaphore.WaitAsync();
            if (IsInitialized)
            {
                //It is not redundant to include a check before and after the semaphore.
                //If we call InitializeServiceIfNeededAsync multiple times in parallel in the background,
                //it may lead to double initialization, which is undesired.
                //The initial check helps to prevent unnecessary reinitialization of the service.
                return;
            }

            await _popoverJsInterop.Initialize(PopoverOptions.ContainerClass, PopoverOptions.FlipMargin);
            //Starts in background
            await _batchExecutor.StartAsync();
            IsInitialized = true;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
