// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Services;

namespace MudBlazor;

/// <summary>
/// Adapts a synchronous lambda into an <see cref="IBrowserViewportObserver"/>.
/// </summary>
/// <remarks>
/// Used by the viewport service overloads that accept an <see cref="Action{T}"/> so callers can subscribe without implementing a full observer type.
/// </remarks>
internal class BrowserViewportLambdaObserver : IBrowserViewportObserver
{
    private readonly Action<BrowserViewportEventArgs> _lambda;

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public ResizeOptions? ResizeOptions { get; }

    public BrowserViewportLambdaObserver(Guid id, Action<BrowserViewportEventArgs> lambda, ResizeOptions? options)
    {
        Id = id;
        ResizeOptions = options;
        _lambda = lambda;
    }

    /// <inheritdoc />
    public Task NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs browserViewportEventArgs)
    {
        _lambda(browserViewportEventArgs);

        return Task.CompletedTask;
    }
}
