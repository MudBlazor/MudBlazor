// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public interface IScrollListenerFactory
    {
        IScrollListener Create(string selector);
    }

    public class ScrollListenerFactory : IScrollListenerFactory
    {
        private readonly IServiceProvider _provider;

        public ScrollListenerFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IScrollListener Create(string selector) =>
            new ScrollListener(selector, _provider.GetRequiredService<IJSRuntime>());
    }
}
