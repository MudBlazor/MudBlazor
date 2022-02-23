﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public class ScrollSectionCenteredEventArgs
    {
        public ScrollSectionCenteredEventArgs(string id)
        {
            Id = id;
        }

        public string Id { get; init; }
    }

    public interface IScrollSpy : IAsyncDisposable
    {
        /// <summary>
        /// Start spying for scroll events for elements with the specified classes
        /// </summary>
        /// <param name="elementsSelector">the class name (without .) to identify the containers to spy on</param>
        /// <returns></returns>
        public Task StartSpying(string elementsSelector);

        /// <summary>
        /// Center the viewport to DOM element with the given Id 
        /// </summary>
        /// <param name="id">The Id of the DOM element, that should be centered</param>
        /// <returns></returns>
        Task ScrollToSection(string id);

        /// <summary>
        /// Center the viewport to the DOM element represented by the fragment inside the uri
        /// </summary>
        /// <param name="uri">The uri which contains the fragment. If no fragment it scrolls to the top of the page</param>
        /// <returns></returns>
        Task ScrollToSection(Uri uri);
        event EventHandler<ScrollSectionCenteredEventArgs> ScrollSectionSectionCentered;

        /// <summary>
        /// Does the same as ScrollToSection but without the scrolling. This can be used to initially set an value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task SetSectionAsActive(String id);

        /// <summary>
        /// Get the current position of the centered section
        /// </summary>
        string CenteredSection { get; }
    }

    public class ScrollSpy : IScrollSpy
    {
        public string CenteredSection { get; private set; }
        private readonly IJSRuntime _js;
        private DotNetObjectReference<ScrollSpy> _dotNetRef;

        public ScrollSpy(IJSRuntime js)
        {
            _js = js;
            _dotNetRef = DotNetObjectReference.Create(this);
        }

        public async Task StartSpying(string containerSelector) => await _js.InvokeVoidAsync
            ("mudScrollSpy.spying", containerSelector, _dotNetRef);

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
            await _js.InvokeVoidAsync
            ("mudScrollSpy.scrollToSection", id.Trim('#'));
        }

        public async Task SetSectionAsActive(string id)
        {
            CenteredSection = id;
            await _js.InvokeVoidAsync
            ("mudScrollSpy.activateSection", id.Trim('#'));
        }

        public async Task ScrollToSection(Uri uri) => await ScrollToSection(uri.Fragment);

        public async ValueTask DisposeAsync()
        {
            try
            {
                _dotNetRef?.Dispose();
                await _js.InvokeVoidAsync("mudScrollSpy.unspy");
            }
            catch (Exception)
            {
            }
        }
    }
}
