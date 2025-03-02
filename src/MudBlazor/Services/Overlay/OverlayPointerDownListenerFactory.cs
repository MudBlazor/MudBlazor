// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor;

#nullable enable

internal sealed class OverlayPointerDownListenerFactory : IOverlayPointerDownListenerFactory
{
    private readonly IServiceProvider _provider;

    public OverlayPointerDownListenerFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IOverlayPointerDownListener Create(string elementId)
    {
        var jsRuntime = _provider.GetRequiredService<IJSRuntime>();
        return new OverlayPointerDownListener(elementId, jsRuntime);
    }
}
