using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace MudBlazor.Services;

/// <summary>
/// Creates <see cref="IResizeObserver"/> instances wired to the current DI container and JS runtime.
/// </summary>
/// <remarks>
/// This factory is used by components and services that need to track element size changes without directly depending on JavaScript interop wiring. It keeps the construction details in one place so callers can focus on subscribing to resize events.
/// </remarks>
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
