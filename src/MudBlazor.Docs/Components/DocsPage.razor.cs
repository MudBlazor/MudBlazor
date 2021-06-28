// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components
{
    public partial class DocsPage : IDisposable
    {
        [Inject] IScrollListener ScrollListener { get; set; }
        [Inject] IScrollManager ScrollManager { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        [Parameter] public MaxWidth MaxWidth { get; set; } = MaxWidth.Medium;
        [Parameter] public EventCallback<ScrollEventArgs> OnScroll { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        private List<DocsLink> _sections = new List<DocsLink>();
        private string _pageuri { get; set; }

        protected override void OnInitialized()
        {
            _pageuri = NavigationManager.Uri;
            NavigationManager.LocationChanged += TryFragmentNavigation;
        }

        internal void AddSection(string section)
        {
            var newsection = new DocsLink()
            {
                Href = $"{_pageuri}#{section.Replace(" ", "").ToLower()}",
                Title = section
            };

            _sections.Add(newsection);

            StateHasChanged();
        }

        private async void ScrollListener_OnScroll(object sender, ScrollEventArgs e)
        {
            await OnScroll.InvokeAsync(e);
        }

        private async void TryFragmentNavigation(object sender, LocationChangedEventArgs args)
        {
            string currenturl = NavigationManager.Uri;

            if(currenturl.Contains("#"))
            {
                string id = currenturl.Substring(currenturl.IndexOf('#') + 1);
                await ScrollManager.ScrollToFragmentAsync(id, ScrollBehavior.Smooth);
            }
        }

        public void Dispose()
        {
            ScrollListener.OnScroll -= ScrollListener_OnScroll;
            NavigationManager.LocationChanged -= TryFragmentNavigation;
        }
    }
}
