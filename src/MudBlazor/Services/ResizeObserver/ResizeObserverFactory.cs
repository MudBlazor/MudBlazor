using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace MudBlazor.Services
{
    public class ResizeObserverFactory : IResizeObserverFactory
    {
        private readonly IServiceProvider _provider;

        public ResizeObserverFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IResizeObserver Create(ResizeObserverOptions options) =>
            new ResizeObserver(_provider.GetRequiredService<IJSRuntime>(), new OptionsWrapper<ResizeObserverOptions>(options));

        public IResizeObserver Create()
        {
            var options = _provider.GetService<IOptions<ResizeObserverOptions>>();
            return Create(options?.Value ?? new ResizeObserverOptions());
        }
    }
}
