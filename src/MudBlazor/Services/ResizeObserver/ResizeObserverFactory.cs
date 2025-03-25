using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace MudBlazor.Services;

#nullable enable
/// <summary>
/// Factory for creating instances of <see cref="IResizeObserver"/>.
/// </summary>
internal sealed class ResizeObserverFactory : IResizeObserverFactory
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResizeObserverFactory"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public ResizeObserverFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public IResizeObserver Create()
    {
        var options = _provider.GetService<IOptions<ResizeObserverOptions>>();

        return Create(options?.Value ?? new ResizeObserverOptions());
    }

    /// <inheritdoc />
    public IResizeObserver Create(ResizeObserverOptions options)
    {
        var jsRuntime = _provider.GetRequiredService<IJSRuntime>();

        return new ResizeObserver(jsRuntime, new OptionsWrapper<ResizeObserverOptions>(options));
    }
}
