// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor;

/// <summary>
/// Creates <see cref="IScrollSpy"/> instances tied to the current DI container.
/// </summary>
/// <remarks>
/// Used by components that need to observe active sections without constructing the JS interop dependencies themselves.
/// </remarks>
internal sealed class ScrollSpyFactory : IScrollSpyFactory
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScrollSpyFactory"/> class with the specified service provider.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public ScrollSpyFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public IScrollSpy Create()
    {
        var jsRuntime = _provider.GetRequiredService<IJSRuntime>();

        return new ScrollSpy(jsRuntime);
    }
}
