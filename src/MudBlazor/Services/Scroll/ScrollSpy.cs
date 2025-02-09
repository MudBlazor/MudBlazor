// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Manages scroll spying behavior for specified elements.
    /// </summary>
    internal sealed class ScrollSpy : IScrollSpy
    {
        private bool _disposed;
        private readonly IJSRuntime _js;
        private readonly DotNetObjectReference<ScrollSpy> _dotNetRef;

        /// <inheritdoc />
        public string? CenteredSection { get; private set; }

        /// <inheritdoc />
        public event EventHandler<ScrollSectionCenteredEventArgs>? ScrollSectionSectionCentered;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollSpy"/> class with the specified JavaScript runtime.
        /// </summary>
        /// <param name="js">The JavaScript runtime.</param>
        [DynamicDependency(nameof(SectionChangeOccured))]
        public ScrollSpy(IJSRuntime js)
        {
            _js = js;
            _dotNetRef = DotNetObjectReference.Create(this);
        }

        /// <inheritdoc />
        public async Task StartSpying(string containerSelector, string sectionClassSelector) =>
            await _js.InvokeVoidAsync("mudScrollSpy.spying", _dotNetRef, containerSelector, sectionClassSelector);

        /// <inheritdoc />
        public async Task ScrollToSection(string id)
        {
            CenteredSection = id;
            await _js.InvokeVoidAsyncWithErrorHandling("mudScrollSpy.scrollToSection", id.Trim('#'));
        }

        /// <inheritdoc />
        public async Task SetSectionAsActive(string id)
        {
            CenteredSection = id;
            await _js.InvokeVoidAsyncWithErrorHandling("mudScrollSpy.activateSection", id.Trim('#'));
        }

        /// <inheritdoc />
        public async Task ScrollToSection(Uri uri) => await ScrollToSection(uri.Fragment);

        /// <summary>
        /// Invoked by JavaScript when a section change occurs.
        /// </summary>
        /// <param name="id">The ID of the centered scroll section.</param>
        [JSInvokable]
        public void SectionChangeOccured(string id)
        {
            CenteredSection = id;
            ScrollSectionSectionCentered?.Invoke(this, new ScrollSectionCenteredEventArgs(id));
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _disposed = true;

                await _js.InvokeVoidAsyncWithErrorHandling("mudScrollSpy.unspy");
                _dotNetRef.Dispose();
            }
        }
    }
}
