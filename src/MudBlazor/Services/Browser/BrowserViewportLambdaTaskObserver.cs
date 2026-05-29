// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Services;

namespace MudBlazor;

/// <summary>
/// Adapts an asynchronous lambda into an <see cref="IBrowserViewportObserver"/>.
/// </summary>
/// <remarks>
/// This is used by subscription overloads that accept <see cref="Func{T, TResult}"/> handlers.
/// </remarks>
internal class BrowserViewportLambdaTaskObserver : IBrowserViewportObserver
{
    private readonly Func<BrowserViewportEventArgs, Task> _lambda;

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public ResizeOptions? ResizeOptions { get; }

    public BrowserViewportLambdaTaskObserver(Guid id, Func<BrowserViewportEventArgs, Task> lambda, ResizeOptions? options)
    {
        Id = id;
        ResizeOptions = options;
        _lambda = lambda;
    }

    /// <inheritdoc />
    public Task NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs browserViewportEventArgs)
    {
        return _lambda(browserViewportEventArgs);
    }
}
