// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Extensions;
using MudBlazor.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.JSInterop;

namespace MudBlazor.Docs.Components
{
    public partial class DocsPage : ComponentBase, IDisposable
    {
        [Inject] IScrollListener ScrollListener { get; set; }
        [Inject] IScrollManager ScrollManager { get; set; }
        [Inject] IResizeObserver ResizeObserver { get; set; }
        [Inject] IJSRuntime JsRuntime { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        [Parameter] public MaxWidth MaxWidth { get; set; } = MaxWidth.Medium;
        [Parameter] public EventCallback<ScrollEventArgs> OnScroll { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        private List<DocsSectionLink> _sections = new List<DocsSectionLink>();
        private string _pageuri { get; set; }

        private bool _scrolled = false;
        private bool _eventScrolled = false;

        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += TryFragmentNavigation;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                ScrollListener.OnScroll += ScrollListener_OnScroll;
            }
        }

        internal void AddSection(DocsSectionLink section)
        {
            _sections.Add(section);
            StateHasChanged();
        }

        private async void ScrollListener_OnScroll(object sender, ScrollEventArgs e)
        {
            await OnScroll.InvokeAsync(e);

            if(_scrolled == false)
            {
                await ObserveAllSectionsAsync();
            }

            if(_eventScrolled)
            {
                _eventScrolled = false;
                return;
            }

            var activelink = _sections.OrderBy(item => Math.Abs((e.FirstChildBoundingClientRect.Top * -1) - item.Location.Top)).First();

            _sections.ToList().ForEach(item => item.Active = false);
            activelink.Active = true;

            await ChangeUrl(activelink.Id);

            StateHasChanged();
        }

        private async void TryFragmentNavigation(object sender, LocationChangedEventArgs args)
        {
            string currenturl = NavigationManager.Uri;

            if (currenturl.Contains("#"))
            {
                string id = currenturl.Substring(currenturl.IndexOf('#') + 1);
                var activelink = _sections.FirstOrDefault(item => item.Id == id);
                activelink.Active = true;
                _eventScrolled = true;
                await ScrollManager.ScrollToFragmentAsync(id, ScrollBehavior.Smooth);
            }
            StateHasChanged();
        }

        private async void OnNavLinkClick(string id)
        {
            _sections.ToList().ForEach(item => item.Active = false);
            var activelink = _sections.FirstOrDefault(item => item.Id == id);
            activelink.Active = true;
            _eventScrolled = true;

            await ChangeUrl(activelink.Id);
            await ScrollManager.ScrollToFragmentAsync(id, ScrollBehavior.Smooth);

            StateHasChanged();
        }

        private async Task ChangeUrl(string id)
        {
            string currenturl = $"{NavigationManagerExtensions.GetSection(NavigationManager)}/{NavigationManagerExtensions.GetComponentLink(NavigationManager)}";
            if (currenturl.Contains("#"))
            {
                currenturl = currenturl.Substring(0, currenturl.IndexOf("#") + 0);
            }
            _pageuri = currenturl;
            await JsRuntime.InvokeVoidAsync("ChangeUrl", $"{_pageuri}#{id}");
        }

        private async Task ObserveAllSectionsAsync()
        {
            var observeResult = (await ResizeObserver.Observe(_sections.Select(x => x.Reference).ToArray())).ToArray();

            for (int i = 0; i < _sections.Count; i++)
            {
                _sections[i].Location = observeResult[i];
            }
            _scrolled = true;
            StateHasChanged();
        }

        public void Dispose()
        {
            ScrollListener.OnScroll -= ScrollListener_OnScroll;
            NavigationManager.LocationChanged -= TryFragmentNavigation;
            ResizeObserver.Dispose();
        }

        private string GetNavLinkClass(bool active)
        {
            if (active)
                return $"docs-contents-navlink active";
            else
                return $"docs-contents-navlink";
        }
    }
}
