// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.JSInterop;

namespace MudBlazor
{
    public interface IScrollListenerFactory
    {
        IScrollListener Create(string selector);
    }

    public class ScrollListenerFactory : IScrollListenerFactory
    {
        private readonly IJSRuntime _jsRuntime;

        public ScrollListenerFactory(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public IScrollListener Create(string selector) => new ScrollListener(selector, _jsRuntime);
    }
}
