// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using MudBlazor.Services;

namespace MudBlazor;

#nullable enable
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
