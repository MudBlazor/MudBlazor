// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor;

/// <summary>
/// Creates <see cref="IScrollListener"/> instances with the configured JS runtime.
/// </summary>
/// <remarks>
/// This factory keeps scroll listener construction consistent and avoids repeating DI lookups.
/// </remarks>
internal sealed class ScrollListenerFactory : IScrollListenerFactory
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScrollListenerFactory"/> class.
    /// </summary>
    /// <param name="provider">The service provider used to resolve dependencies.</param>
    public ScrollListenerFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public IScrollListener Create(string? selector)
    {
        return Create(selector, 10);
    }

    /// <inheritdoc />
    public IScrollListener Create(string? selector, int reportRateMs)
    {
        var jsRuntime = _provider.GetRequiredService<IJSRuntime>();
        return new ScrollListener(selector, jsRuntime, reportRateMs);
    }
}
