// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor.Services;

/// <summary>
/// Provides a factory for creating instances of <see cref="IJsEvent"/>.
/// </summary>
internal sealed class JsEventFactory : IJsEventFactory
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsEventFactory"/> class.
    /// </summary>
    /// <param name="provider">The service provider to use for resolving dependencies.</param>
    public JsEventFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public IJsEvent Create()
    {
        var jsRuntime = _provider.GetRequiredService<IJSRuntime>();

        return new JsEvent(jsRuntime);
    }
}
