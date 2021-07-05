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
    public partial class DocsPage : ComponentBase, IAsyncDisposable
    {
        [Inject] IScrollSpy ScrollSpy { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        [Parameter] public MaxWidth MaxWidth { get; set; } = MaxWidth.Medium;
        [Parameter] public RenderFragment ChildContent { get; set; }

        private List<DocsSectionLink> _sections = new();


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ScrollSpy.ScrollSectionSectionCentered += ScrollSpy_ScrollSectionSectionCentered;

                await ScrollSpy.ScrollToSection(new Uri(NavigationManager.Uri));
                await ScrollSpy.StartSpying("docs-section-header");
                SelectActiveSection(ScrollSpy.CenteredSection);
            }
        }

        internal void AddSection(DocsSectionLink section)
        {
            _sections.Add(section);
            if(section.Id == ScrollSpy.CenteredSection)
            {
                section.Active = true;
            }

            StateHasChanged();
        }

        private void SelectActiveSection(string id)
        {
            if(string.IsNullOrEmpty(id)) { return; }
            
            var activelink = _sections.FirstOrDefault(x => x.Id == id);
            if(activelink == null) { return; }

            _sections.ToList().ForEach(item => item.Active = false);
            activelink.Active = true;

            StateHasChanged();
        }

        private void ScrollSpy_ScrollSectionSectionCentered(object sender, ScrollSectionSectionCenteredEventArgs e)
        {
            SelectActiveSection(e.Id);
        }

        private async Task OnNavLinkClick(string id)
        {
            _sections.ToList().ForEach(item => item.Active = false);
            var activelink = _sections.FirstOrDefault(item => item.Id == id);
            activelink.Active = true;
            await ScrollSpy.ScrollToSection(id);
        }

        public async ValueTask DisposeAsync()
        {
            ScrollSpy.ScrollSectionSectionCentered -= ScrollSpy_ScrollSectionSectionCentered;
            await ScrollSpy.DisposeAsync();
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
