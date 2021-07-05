﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public static class StringExtentionsForScrollSpy
    {
        public static string RemoveHashIfNeeded(this string input)
        {
            if (string.IsNullOrEmpty(input) == true)
            {
                return input;
            }

            if(input.StartsWith('#') == true)
            {
                return input.Substring(1);
            }

            return input;


        }
    }

    public class ScrollSectionSectionCenteredEventArgs
    {
        public ScrollSectionSectionCenteredEventArgs(string id)
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
        /// <param name="uri">The uri which contains the fragement. If no fragment it scrolls to the top of the page</param>
        /// <returns></returns>
        Task ScrollToSection(Uri uri);
        event EventHandler<ScrollSectionSectionCenteredEventArgs> ScrollSectionSectionCentered;

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
            ScrollSectionSectionCentered?.Invoke(this, new ScrollSectionSectionCenteredEventArgs(id));
        }

        public event EventHandler<ScrollSectionSectionCenteredEventArgs> ScrollSectionSectionCentered;

        public async Task ScrollToSection(string id)
        {
            CenteredSection = id;
            await _js.InvokeVoidAsync
            ("mudScrollSpy.scrollToSection", id.RemoveHashIfNeeded());
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
