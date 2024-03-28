// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks;

#nullable enable
public class MockBrowserViewportService : IBrowserViewportService
{
    public ResizeOptions ResizeOptions { get; } = new();

    public Task SubscribeAsync(IBrowserViewportObserver observer, bool fireImmediately = true) => Task.CompletedTask;

    public Task SubscribeAsync(Guid observerId, Action<BrowserViewportEventArgs> lambda, ResizeOptions? options = null, bool fireImmediately = true) => Task.CompletedTask;

    public Task SubscribeAsync(Guid id, Func<BrowserViewportEventArgs, Task> lambda, ResizeOptions? options = null, bool fireImmediately = true) => Task.CompletedTask;

    public Task UnsubscribeAsync(IBrowserViewportObserver observer) => Task.CompletedTask;

    public Task UnsubscribeAsync(Guid id) => Task.CompletedTask;

    public Task<bool> IsMediaQueryMatchAsync(string mediaQuery) => Task.FromResult(false);

    public Task<bool> IsBreakpointWithinWindowSizeAsync(Breakpoint breakpoint) => Task.FromResult(false);

    public Task<bool> IsBreakpointWithinReferenceSizeAsync(Breakpoint breakpoint, Breakpoint reference) => Task.FromResult(false);

    public Task<Breakpoint> GetCurrentBreakpointAsync() => Task.FromResult(Breakpoint.None);

    public Task<BrowserWindowSize> GetCurrentBrowserWindowSizeAsync() => Task.FromResult(new BrowserWindowSize());

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
