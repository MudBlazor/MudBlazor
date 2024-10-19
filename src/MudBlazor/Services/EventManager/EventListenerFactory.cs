// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Factory for creating instances of <see cref="IEventListener"/>.
/// </summary>
internal sealed class EventListenerFactory : IEventListenerFactory
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventListenerFactory"/> class.
    /// </summary>
    /// <param name="provider">The service provider used to resolve dependencies.</param>
    public EventListenerFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public IEventListener Create()
    {
        var jsRuntime = _provider.GetRequiredService<IJSRuntime>();

        return new EventListener(jsRuntime);
    }
}
