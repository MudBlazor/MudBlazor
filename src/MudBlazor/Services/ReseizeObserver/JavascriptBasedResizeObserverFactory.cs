// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor.Services
{
    public class JavascriptBasedResizeObserverFactory : IResizeObserverFactory
    {
        private readonly IServiceProvider _provider;

        public JavascriptBasedResizeObserverFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IResizeObserver Create(ResizeObserverOptions options) =>
            new JavascriptBasedResizeObserver(_provider.GetRequiredService<IJSRuntime>() , options);
    }
}
