// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Factory class for creating instances of <see cref="IScrollListener"/>.
/// </summary>
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
        var jsRuntime = _provider.GetRequiredService<IJSRuntime>();

        return new ScrollListener(selector, jsRuntime);
    }
}
