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
internal class PopoverService : IPopoverService, IBatchTimerHandler<MudPopoverState>
{
    private readonly SemaphoreSlim _semaphore;
    private readonly Dictionary<Guid, MudPopoverState> _states;
    private readonly BatchPeriodicQueue<MudPopoverState> _batchExecutor;
    private readonly ObserverManager<Guid, IPopoverObserver> _observerManager;
    private readonly PopoverJsInterop _popoverJsInterop;

    /// <inheritdoc />
    public IEnumerable<IMudPopoverState> ActivePopovers => _states.Values;

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
        _states = new Dictionary<Guid, MudPopoverState>();
        _popoverJsInterop = new PopoverJsInterop(jsInterop);
        _batchExecutor = new BatchPeriodicQueue<MudPopoverState>(this, PopoverOptions.QueueDelay);
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

        await InitializeServiceIfNeededAsync().ConfigureAwait(false);
        var state = new MudPopoverState(popover.Id, popover.ChildContent)
            .SetComponentBaseParameters(popover.PopoverClass, popover.PopoverStyles, popover.Open, popover.Tag, popover.UserAttributes);

        _states.TryAdd(state.Id, state);
        await _observerManager.NotifyAsync(observer => observer.PopoverCollectionUpdatedNotification(new[] { state })).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> UpdatePopoverAsync(IPopover popover)
    {
        ArgumentNullException.ThrowIfNull(popover);

        //We initialize the service regardless of whether the popover exists or not.
        //Adding it in an if clause doesn't provide significant benefits.
        //Instead, we prioritize ensuring that the service is ready for use, as its initialization is a one-time operation.
        await InitializeServiceIfNeededAsync().ConfigureAwait(false);
        if (!_states.TryGetValue(popover.Id, out var state))
        {
            return false;
        }

        //Do not put after the semaphore as it can cause deadlock
        await InitializePopoverIfNeededAsync(state).ConfigureAwait(false);

        if (state.IsDetached)
        {
            return false;
        }

        try
        {

            await _semaphore.WaitAsync().ConfigureAwait(false);

            state
                .SetFragment(popover.ChildContent)
                .SetComponentBaseParameters(popover.PopoverClass, popover.PopoverStyles, popover.Open, popover.Tag, popover.UserAttributes);

            //This basically calls StateHasChanged on the popover.
            //To be honest, there's no real need for us to update each popover individually through MudRendered as we currently do.
            //Instead, we can consider invoking PopoverCollectionUpdatedNotification (or make new function).
            //This function would trigger a StateHasChanged on the entire MudPopoverProvider, effectively updating all the underlying popovers at once.
            //By performing these updates in batches, we can minimize the number of re-renders to a minimum.
            state.ElementReference?.StateHasChanged();

            return true;
        }
        finally
        {
            _semaphore.Release();
        }

        return false;
    }

    /// <inheritdoc />
    public async Task<bool> DestroyPopoverAsync(IPopover popover)
    {
        ArgumentNullException.ThrowIfNull(popover);

        //We initialize the service regardless of whether the popover exists or not.
        //Adding it in an if clause doesn't provide significant benefits.
        //Instead, we prioritize ensuring that the service is ready for use, as its initialization is a one-time operation.
        await InitializeServiceIfNeededAsync().ConfigureAwait(false);

        return await DestroyPopoverByIdAsync(popover.Id).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<int> CountProvidersAsync()
    {
        await InitializeServiceIfNeededAsync().ConfigureAwait(false);

        var (success, value) = await _popoverJsInterop.CountProviders().ConfigureAwait(false);

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
        foreach (var stateKeyValuePair in _states)
        {
            await DestroyPopoverByIdAsync(stateKeyValuePair.Key).ConfigureAwait(false);
        }

        //Cleanup and call the OnBatchTimerElapsedAsync with everything what's left.
        await _batchExecutor.DisposeAsync().ConfigureAwait(false);

        _ = _popoverJsInterop.Dispose();
    }

    /// <inheritdoc />
    public virtual Task OnBatchTimerElapsedAsync(IReadOnlyCollection<MudPopoverState> items, CancellationToken stoppingToken)
    {
        //In our case we do not care if the cancellation token in requested, we should not interrupt the process and and just detach to cleanup resources.
        //In the future, there might be a requirement to split the jobs and introduce a change where instead of using IReadOnlyCollection<MudPopoverState>,
        //we would utilize IReadOnlyCollection<PopoverQueueContainer>. This new collection would consist of various operations, such as detaching items, rendering items,
        //and triggering the PopoverCollectionUpdatedNotification, among others.
        return DetachRange(items);
    }

    private async Task<bool> DestroyPopoverByIdAsync(Guid id)
    {
        if (!_states.Remove(id, out var state))
        {
            return false;
        }

        _batchExecutor.QueueItem(state);
        // Although it is not completely detached from the JS side until OnBatchTimerElapsedAsync fires, we mark it as "Detached"
        // because we want let know the UpdatePopoverAsync method that there is no need to update it anymore,
        // as it is no longer being rendered by MudPopoverProvider since it has been removed from the ActivePopovers collection.
        // Perhaps we could consider adding a state indicating that the object is queued for detaching instead.
        state.IsDetached = true;

        await _observerManager.NotifyAsync(observer => observer.PopoverCollectionUpdatedNotification(new[] { state })).ConfigureAwait(false);

        return true;
    }

    private async Task DetachRange(IReadOnlyCollection<MudPopoverState> states)
    {
        //Ignore task if zero items in collection to not enter in the semaphore
        if (states.Count == 0)
        {
            return;
        }

        try
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            foreach (var state in states)
            {
                try
                {
                    state.IsDetached = true;
                    if (state.IsConnected)
                    {
                        await _popoverJsInterop.Disconnect(state.Id).ConfigureAwait(false);
                    }
                }
                finally
                {
                    state.IsConnected = false;
                }
            }

        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task InitializePopoverIfNeededAsync(MudPopoverState state)
    {
        if (state.IsConnected || state.IsDetached)
        {
            return;
        }

        try
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);

            if (state.IsConnected || state.IsDetached)
            {
                //It is not redundant to include a check before and after the semaphore.
                //If we call InitializePopoverIfNeededAsync multiple times in parallel in the background,
                //it may lead to double connection, which is undesired.
                //The initial check helps to prevent double initialization of the popover.
                return;
            }

            state.IsConnected = await _popoverJsInterop.Connect(state.Id).ConfigureAwait(false);
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
            await _semaphore.WaitAsync().ConfigureAwait(false);
            if (IsInitialized)
            {
                //It is not redundant to include a check before and after the semaphore.
                //If we call InitializeServiceIfNeededAsync multiple times in parallel in the background,
                //it may lead to double initialization, which is undesired.
                //The initial check helps to prevent unnecessary reinitialization of the service.
                return;
            }

            await _popoverJsInterop.Initialize(PopoverOptions.ContainerClass, PopoverOptions.FlipMargin).ConfigureAwait(false);
            //Starts in background
            await _batchExecutor.StartAsync().ConfigureAwait(false);
            IsInitialized = true;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
