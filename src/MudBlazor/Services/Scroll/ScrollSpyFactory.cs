// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Factory class for creating instances of <see cref="IScrollSpy"/>.
/// </summary>
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
