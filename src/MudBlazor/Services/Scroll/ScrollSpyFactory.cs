// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public interface IScrollSpyFactory
    {
        IScrollSpy Create();
    }

    public class ScrollSpyFactory : IScrollSpyFactory
    {
        private readonly IServiceProvider _provider;

        public ScrollSpyFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IScrollSpy Create() =>
            new ScrollSpy(_provider.GetRequiredService<IJSRuntime>());
    }
}
