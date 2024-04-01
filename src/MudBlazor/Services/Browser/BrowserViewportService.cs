// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using MudBlazor.Interop;
using MudBlazor.Services;
using MudBlazor.Utilities.ObserverManager;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a service that serves to listen to browser window size changes and breakpoints.
/// </summary>
internal class BrowserViewportService : IBrowserViewportService
{
    private bool _disposed;
    private readonly SemaphoreSlim _semaphore;
    private readonly ResizeListenerInterop _resizeListenerInterop;
    private readonly Lazy<DotNetObjectReference<BrowserViewportService>> _dotNetReferenceLazy;
    private readonly ObserverManager<BrowserViewportSubscription, IBrowserViewportObserver> _observerManager;

    private BrowserWindowSize? _latestWindowSize;
    // ReSharper disable once NotAccessedField.Local
    private Breakpoint _latestBreakpoint = Breakpoint.None;

    /// <inheritdoc />
    public ResizeOptions ResizeOptions { get; }

    /// <summary>
    /// Gets the number of observers.
    /// </summary>
    /// <remarks>
    /// This property is not exposed in the public API of the <see cref="IBrowserViewportService"/> interface and is intended for internal use only.
    /// </remarks>
    internal int ObserversCount => _observerManager.Count;

    [DynamicDependency(nameof(RaiseOnResized))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ResizeOptions))]
    public BrowserViewportService(ILogger<BrowserViewportService> logger, IJSRuntime jsRuntime, IOptions<ResizeOptions>? options = null)
    {
        ResizeOptions = options?.Value ?? new ResizeOptions();
        _semaphore = new SemaphoreSlim(1, 1);
        _resizeListenerInterop = new ResizeListenerInterop(jsRuntime);
        _observerManager = new ObserverManager<BrowserViewportSubscription, IBrowserViewportObserver>(logger);
        _dotNetReferenceLazy = new Lazy<DotNetObjectReference<BrowserViewportService>>(CreateDotNetObjectReference);
    }

    /// <summary>
    /// Notifies observers when the browser size has changed and fires this method. 
    /// This method is invoked from the JavaScript code.
    /// </summary>
    /// <param name="browserWindowSize">The <see cref="BrowserWindowSize"/> representing the updated browser window size.</param>
    /// <param name="breakpoint">The <see cref="Breakpoint"/> representing the updated breakpoint.</param>
    /// <param name="javaScriptListenerId">The unique identifier of the JavaScript listener.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method is not exposed in the public API of the <see cref="IBrowserViewportService"/> interface and is intended to be used by JS and testing.
    /// </remarks>
    [JSInvokable]
    public Task RaiseOnResized(BrowserWindowSize browserWindowSize, Breakpoint breakpoint, Guid javaScriptListenerId)
    {
        _latestWindowSize = browserWindowSize;
        _latestBreakpoint = breakpoint;

        // Filters observers based on a predicate to notify only those that match the JavaScript listener ID.
        // Without this predicate, notifications from unrelated JavaScript listeners would be received, potentially causing duplicate or unwanted notifications for the observer.
        // This is due to the fact that BrowserViewportService instance is shared across all JavaScript listeners
        return _observerManager
            .NotifyAsync(observer => observer.NotifyBrowserViewportChangeAsync(
                    new BrowserViewportEventArgs(
                        javaScriptListenerId,
                        browserWindowSize,
                        breakpoint)
                ), predicate: (subscription, _) => subscription.JavaScriptListenerId == javaScriptListenerId);
    }

    /// <inheritdoc />
    public async Task SubscribeAsync(IBrowserViewportObserver observer, bool fireImmediately = true)
    {
        ArgumentNullException.ThrowIfNull(observer);

        if (_disposed)
        {
            return;
        }

        try
        {
            await _semaphore.WaitAsync();

            // Always clone the ResizeOptions, regardless of the circumstances.
            // This is necessary because the options may originate from the "ResizeOptions" variable (IOptions<ResizeOptions>) - these are the user-defined options when adding this service in the DI container.
            // Only the user should be allowed to modify these settings, and the service should not directly modify the reference to prevent potential bugs.
            var optionsClone = (observer.ResizeOptions ?? ResizeOptions).Clone();
            // Safe to modify now
            optionsClone.BreakpointDefinitions = BreakpointGlobalOptions.GetDefaultOrUserDefinedBreakpointDefinition(optionsClone);

            var subscription = await CreateJavaScriptListener(optionsClone, observer.Id);
            if (_observerManager.Observers.ContainsKey(subscription))
            {
                // Only re-subscribe
                _observerManager.Subscribe(subscription, observer);
            }
            else
            {
                // Subscribe and fire if necessary
                _observerManager.Subscribe(subscription, observer);
                if (fireImmediately)
                {
                    // Not waiting for Browser Size to change and RaiseOnResized to fire and post event with current breakpoint and browser window size
                    var latestWindowSize = await GetCurrentBrowserWindowSizeAsync();
                    var latestBreakpoint = await GetCurrentBreakpointAsync();
                    // Notify only current subscription
                    await observer.NotifyBrowserViewportChangeAsync(new BrowserViewportEventArgs(subscription.JavaScriptListenerId, latestWindowSize, latestBreakpoint, isImmediate: true));
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public Task SubscribeAsync(Guid observerId, Action<BrowserViewportEventArgs> lambda, ResizeOptions? options = null, bool fireImmediately = true)
    {
        ArgumentNullException.ThrowIfNull(lambda);

        return SubscribeAsync(new BrowserViewportLambdaObserver(observerId, lambda, options), fireImmediately);
    }

    /// <inheritdoc />
    public Task SubscribeAsync(Guid observerId, Func<BrowserViewportEventArgs, Task> lambda, ResizeOptions? options = null, bool fireImmediately = true)
    {
        ArgumentNullException.ThrowIfNull(lambda);

        return SubscribeAsync(new BrowserViewportLambdaTaskObserver(observerId, lambda, options), fireImmediately);
    }

    /// <inheritdoc />
    public Task UnsubscribeAsync(IBrowserViewportObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        return UnsubscribeAsync(observer.Id);
    }

    /// <inheritdoc />
    public async Task UnsubscribeAsync(Guid observerId)
    {
        try
        {
            await _semaphore.WaitAsync();

            var subscription = await RemoveJavaScriptListener(observerId);
            if (subscription is not null)
            {
                _observerManager.Unsubscribe(subscription);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsMediaQueryMatchAsync(string mediaQuery)
    {
        return await _resizeListenerInterop.MatchMedia(mediaQuery);
    }

    /// <inheritdoc />
    public async Task<bool> IsBreakpointWithinWindowSizeAsync(Breakpoint breakpoint)
    {
        if (breakpoint == Breakpoint.None)
        {
            return false;
        }

        if (breakpoint == Breakpoint.Always)
        {
            return true;
        }

        var currentBreakpoint = await GetCurrentBreakpointAsync();

        return await IsBreakpointWithinReferenceSizeAsync(breakpoint, currentBreakpoint);
    }

    /// <inheritdoc />
    public Task<bool> IsBreakpointWithinReferenceSizeAsync(Breakpoint breakpoint, Breakpoint referenceBreakpoint)
    {
        var isBreakpointMet = breakpoint switch
        {
            Breakpoint.None => false,
            Breakpoint.Always => true,
            Breakpoint.Xs => referenceBreakpoint == Breakpoint.Xs,
            Breakpoint.Sm => referenceBreakpoint == Breakpoint.Sm,
            Breakpoint.Md => referenceBreakpoint == Breakpoint.Md,
            Breakpoint.Lg => referenceBreakpoint == Breakpoint.Lg,
            Breakpoint.Xl => referenceBreakpoint == Breakpoint.Xl,
            Breakpoint.Xxl => referenceBreakpoint == Breakpoint.Xxl,
            // * and down
            Breakpoint.SmAndDown => referenceBreakpoint <= Breakpoint.Sm,
            Breakpoint.MdAndDown => referenceBreakpoint <= Breakpoint.Md,
            Breakpoint.LgAndDown => referenceBreakpoint <= Breakpoint.Lg,
            Breakpoint.XlAndDown => referenceBreakpoint <= Breakpoint.Xl,
            // * and up
            Breakpoint.SmAndUp => referenceBreakpoint >= Breakpoint.Sm,
            Breakpoint.MdAndUp => referenceBreakpoint >= Breakpoint.Md,
            Breakpoint.LgAndUp => referenceBreakpoint >= Breakpoint.Lg,
            Breakpoint.XlAndUp => referenceBreakpoint >= Breakpoint.Xl,
            _ => false
        };

        return Task.FromResult(isBreakpointMet);
    }

    /// <inheritdoc />
    public async Task<Breakpoint> GetCurrentBreakpointAsync()
    {
        var breakpointDefinition = BreakpointGlobalOptions.GetDefaultOrUserDefinedBreakpointDefinition(ResizeOptions);

        // Note: we don't need to get the size if we are listening for updates, so only if onResized==null, get the actual size
        // But there is potential problem, if there are no active observers, you are stuck will old cached value, it's not clear if such cases should be handled
        _latestWindowSize ??= await _resizeListenerInterop.GetBrowserWindowSize();

        if (_latestWindowSize == null)
            return Breakpoint.Xs;
        if (_latestWindowSize.Width >= breakpointDefinition[Breakpoint.Xxl])
            return Breakpoint.Xxl;
        if (_latestWindowSize.Width >= breakpointDefinition[Breakpoint.Xl])
            return Breakpoint.Xl;
        if (_latestWindowSize.Width >= breakpointDefinition[Breakpoint.Lg])
            return Breakpoint.Lg;
        if (_latestWindowSize.Width >= breakpointDefinition[Breakpoint.Md])
            return Breakpoint.Md;
        if (_latestWindowSize.Width >= breakpointDefinition[Breakpoint.Sm])
            return Breakpoint.Sm;

        return Breakpoint.Xs;
    }

    /// <inheritdoc />
    public async Task<BrowserWindowSize> GetCurrentBrowserWindowSizeAsync()
    {
        return await _resizeListenerInterop.GetBrowserWindowSize();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return DisposeAsyncCore(true);
    }

    private ValueTask DisposeAsyncCore(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _observerManager.Clear();

                if (_dotNetReferenceLazy.IsValueCreated)
                {
                    _dotNetReferenceLazy.Value.Dispose();
                }

                // https://github.com/MudBlazor/MudBlazor/pull/5367#issuecomment-1258649968
                // Fixed in NET8
                _ = _resizeListenerInterop.Dispose();
            }

            _disposed = true;
        }

        return ValueTask.CompletedTask;
    }


    // ReSharper disable once UnusedMember.Global used in tests
    internal BrowserViewportSubscription? GetInternalSubscription(IBrowserViewportObserver observer)
    {
        return GetInternalSubscription(observer.Id);
    }

    internal BrowserViewportSubscription? GetInternalSubscription(Guid observerId)
    {
        var subscription = _observerManager
            .Observers
            .Select(x => x.Key)
            .FirstOrDefault(x => x.ObserverId == observerId);

        return subscription;
    }

    private DotNetObjectReference<BrowserViewportService> CreateDotNetObjectReference()
    {
        return DotNetObjectReference.Create(this);
    }

    private async Task<BrowserViewportSubscription> CreateJavaScriptListener(ResizeOptions clonedOptions, Guid observerId)
    {
        // We check if we have an observer with equals options or same observer id
        var javaScriptListenerId = _observerManager
            .Observers
            .Where(x => clonedOptions.Equals(x.Key.Options ?? clonedOptions) || x.Key.ObserverId == observerId)
            .Select(x => x.Key.JavaScriptListenerId)
            .FirstOrDefault();

        // This implementation serves as an optimization to avoid creating a new JavaScript "listener" each time a subscription occurs.
        // Instead, it checks if a listener with the corresponding ResizeOption already exists (which is why it implements IEquatable), and only creates a new listener if necessary.
        // In certain scenarios, you may have multiple observers monitoring changes (e.g., 10 observers), but only a single JavaScript listener on the other side.
        // Without this optimization, the number of observers and JavaScript listeners would be equal.
        if (javaScriptListenerId == default)
        {
            // Create new listener on JS side
            var dotNetReference = _dotNetReferenceLazy.Value;
            var jsListenerId = Guid.NewGuid();
            await _resizeListenerInterop.ListenForResize(dotNetReference, clonedOptions, jsListenerId);

            return new BrowserViewportSubscription(jsListenerId, observerId, clonedOptions);
        }

        // Reuse existing JS listener
        return new BrowserViewportSubscription(javaScriptListenerId, observerId, clonedOptions);

    }

    private async Task<BrowserViewportSubscription?> RemoveJavaScriptListener(Guid observerId)
    {
        var subscription = GetInternalSubscription(observerId);

        if (subscription is null)
        {
            return null;
        }

        var observersWithSameJsListenerIdCount = _observerManager.Observers.Keys.Count(x => x.JavaScriptListenerId == subscription.JavaScriptListenerId);

        if (observersWithSameJsListenerIdCount == 1)
        {
            // This is the last observer with such JavaScriptListenerId therefore we need to remove it on the JS side.
            await _resizeListenerInterop.CancelListener(subscription.JavaScriptListenerId);
        }

        return subscription;
    }
}
