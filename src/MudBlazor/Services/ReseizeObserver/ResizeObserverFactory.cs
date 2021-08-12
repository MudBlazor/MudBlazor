using System;
using Microsoft.Extensions.DependencyInjection;
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
            new ResizeObserver(_provider.GetRequiredService<IJSRuntime>(), options);
    }
}
