// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPageContentNavigation : IAsyncDisposable
    {
        private List<MudPageContenSection> _sections = new();

        [Inject] IScrollSpy ScrollSpy { get; set; }

        public IEnumerable<MudPageContenSection> Sections => _sections.AsEnumerable();
        public MudPageContenSection ActiveSection => _sections.FirstOrDefault(x => x.IsActive == true);

        [Parameter] public string Headline { get; set; } = "Contents";
        [Parameter] public string SectionClassSelector { get; set; } = string.Empty;

        private async Task OnNavLinkClick(string id)
        {
            SelectActiveSection(id);
            await ScrollSpy.ScrollToSection(id);
        }

        private void ScrollSpy_ScrollSectionSectionCentered(object sender, ScrollSectionCenteredEventArgs e) =>
             SelectActiveSection(e.Id);

        private void SelectActiveSection(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            var activelink = _sections.FirstOrDefault(x => x.Id == id);
            if (activelink == null)
            {
                return;
            }

            _sections.ToList().ForEach(item => item.Deactive());
            activelink.Activate();

            StateHasChanged();
        }

        private string GetNavLinkClass(bool active) => new CssBuilder("page-content-navigation-navlink").AddClass("active", active).Build();
        private string GetPanelClass() => new CssBuilder("page-content-navigation").AddClass(Class).Build();

        public async Task ScrollToSection(Uri uri) => await ScrollSpy.ScrollToSection(uri);

        public void AddSection(string sectionName, string sectionId, bool forceUpdate) => AddSection(new(sectionName, sectionId), forceUpdate);

        public void AddSection(MudPageContenSection section, bool forceUpdate)
        {
            _sections.Add(section);

            if (section.Id == ScrollSpy.CenteredSection)
            {
                section.Activate();
            }

            if (forceUpdate == true)
            {
                StateHasChanged();
            }
        }

        public void Update() => StateHasChanged();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ScrollSpy.ScrollSectionSectionCentered += ScrollSpy_ScrollSectionSectionCentered;

                if (string.IsNullOrEmpty(SectionClassSelector) == false)
                {
                    await ScrollSpy.StartSpying(SectionClassSelector);
                }

                SelectActiveSection(ScrollSpy.CenteredSection);
            }
        }

        public async ValueTask DisposeAsync()
        {
            ScrollSpy.ScrollSectionSectionCentered -= ScrollSpy_ScrollSectionSectionCentered;
            await ScrollSpy.DisposeAsync();
        }
    }
}
