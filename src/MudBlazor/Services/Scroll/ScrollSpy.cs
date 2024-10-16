// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public class ScrollSpy : IScrollSpy
    {
        public string CenteredSection { get; private set; }
        private readonly IJSRuntime _js;
        private readonly DotNetObjectReference<ScrollSpy> _dotNetRef;

        [DynamicDependency(nameof(SectionChangeOccured))]
        public ScrollSpy(IJSRuntime js)
        {
            _js = js;
            _dotNetRef = DotNetObjectReference.Create(this);
        }

        public async Task StartSpying(string containerSelector, string sectionClassSelector) =>
            await _js.InvokeVoidAsync("mudScrollSpy.spying", _dotNetRef, containerSelector, sectionClassSelector);

        [JSInvokable]
        public void SectionChangeOccured(string id)
        {
            CenteredSection = id;
            ScrollSectionSectionCentered?.Invoke(this, new ScrollSectionCenteredEventArgs(id));
        }

        public event EventHandler<ScrollSectionCenteredEventArgs> ScrollSectionSectionCentered;

        public async Task ScrollToSection(string id)
        {
            CenteredSection = id;
            await _js.InvokeVoidAsyncWithErrorHandling("mudScrollSpy.scrollToSection", id.Trim('#'));
        }

        public async Task SetSectionAsActive(string id)
        {
            CenteredSection = id;
            await _js.InvokeVoidAsyncWithErrorHandling("mudScrollSpy.activateSection", id.Trim('#'));
        }

        public async Task ScrollToSection(Uri uri) => await ScrollToSection(uri.Fragment);

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _js.InvokeVoidAsyncWithErrorHandling("mudScrollSpy.unspy");
                _dotNetRef?.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
