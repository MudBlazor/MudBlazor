// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudPageContentNavigation : IAsyncDisposable, IMudStateHasChanged
    {
        private List<MudPageContentSection> _sections = new();
        private IScrollSpy? _scrollSpy;

        [Inject]
        private IScrollSpyFactory ScrollSpyFactory { get; set; } = null!;

        /// <summary>
        /// The displayed section within the MudPageContentNavigation
        /// </summary>
        public IEnumerable<MudPageContentSection> Sections => _sections.AsEnumerable();

        /// <summary>
        /// The currently active session. null if there is no section selected
        /// </summary>
        public MudPageContentSection? ActiveSection => _sections.FirstOrDefault(x => x.Active);

        /// <summary>
        /// The text displayed about the section links. Defaults to "Contents"
        /// </summary>
        [Parameter]
        public string Headline { get; set; } = "Contents";

        /// <summary>
        /// The CSS selector used to identify the scroll container
        /// </summary>
        [Parameter]
        public string ScrollContainerSelector { get; set; } = "html";

        /// <summary>
        /// The class name (without .) to identify the HTML elements that should be observed for viewport changes
        /// </summary>
        [Parameter]
        public string SectionClassSelector { get; set; } = string.Empty;

        /// <summary>
        /// If there are multiple levels, this can specified to make a mapping between a level class like "second-level" and the level in the hierarchy
        /// </summary>
        [Parameter]
        public IDictionary<string, int> HierarchyMapper { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// If there are multiple levels, this property controls they visibility of them.
        /// </summary>
        [Parameter]
        public ContentNavigationExpandBehaviour ExpandBehaviour { get; set; } = ContentNavigationExpandBehaviour.Always;

        /// <summary>
        /// If this option is true the first added section will become active when there is no other indication of an active session. Default value is false  
        /// </summary>
        [Parameter]
        public bool ActivateFirstSectionAsDefault { get; set; } = false;

        private Task OnNavLinkClick(string id)
        {
            return _scrollSpy is not null
                ? _scrollSpy.ScrollToSection(id)
                : Task.CompletedTask;
        }

        private void ScrollSpy_ScrollSectionSectionCentered(object? sender, ScrollSectionCenteredEventArgs e) =>
             SelectActiveSection(e.Id);

        private void SelectActiveSection(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            var activeLink = _sections.FirstOrDefault(x => x.Id == id);
            if (activeLink == null)
            {
                return;
            }

            _sections.ToList().ForEach(item => item.Deactive());
            activeLink.Activate();

            StateHasChanged();
        }

        private string GetNavLinkClass(MudPageContentSection section) =>
            new CssBuilder("page-content-navigation-navlink")
                .AddClass("active", section.Active)
                .AddClass($"navigation-level-{section.Level}")
                .Build();

        private string GetPanelClass() => new CssBuilder("page-content-navigation").AddClass(Class).Build();

        /// <summary>
        /// Scrolls to a section based on the fragment of the uri. If there is no fragment, no scroll will occurred
        /// </summary>
        /// <param name="uri">The uri containing the fragment to scroll</param>
        /// <returns>A task that completes when the viewport has scrolled</returns>
        public Task ScrollToSection(Uri uri)
        {
            return _scrollSpy is not null
                ? _scrollSpy.ScrollToSection(uri)
                : Task.CompletedTask;
        }

        /// <summary>
        /// Add a section to the content navigation
        /// </summary>
        /// <param name="sectionName">name of the section will be displayed in the navigation</param>
        /// <param name="sectionId">id of the section. It will be appending to the current url, if the section becomes active</param>
        /// <param name="forceUpdate">If true, StateHasChanged is called, forcing a re-render of the component</param>
        public void AddSection(string sectionName, string sectionId, bool forceUpdate) => AddSection(new MudPageContentSection(sectionName, sectionId), forceUpdate);

        private Dictionary<MudPageContentSection, MudPageContentSection> _parentMapper = new();

        /// <summary>
        /// Add a section to the content navigation
        /// </summary>
        /// <param name="section">The section that needs to be added</param>
        /// <param name="forceUpdate">If true, StateHasChanged is called, forcing a re-render of the component</param>
        public void AddSection(MudPageContentSection section, bool forceUpdate)
        {
            _sections.Add(section);

            var diffRootLevel = 1_000_000;
            var counter = 0;
            foreach (var item in _sections.Where(x => x.Parent is null))
            {
                item.SetLevelStructure(counter, diffRootLevel);
                counter += diffRootLevel;
            }

            if (section.Id == _scrollSpy?.CenteredSection)
            {
                section.Activate();
            }
            else if (_sections.Count == 1 && ActivateFirstSectionAsDefault)
            {
                section.Activate();
                _scrollSpy?.SetSectionAsActive(section.Id).CatchAndLog();
            }

            if (forceUpdate)
            {
                StateHasChanged();
            }
        }

        void IMudStateHasChanged.StateHasChanged() => StateHasChanged();

        protected override void OnInitialized()
        {
            _scrollSpy = ScrollSpyFactory.Create();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (_scrollSpy is not null)
                {
                    _scrollSpy.ScrollSectionSectionCentered += ScrollSpy_ScrollSectionSectionCentered;

                    if (!string.IsNullOrEmpty(SectionClassSelector))
                    {
                        await _scrollSpy.StartSpying(ScrollContainerSelector, SectionClassSelector);
                    }

                    SelectActiveSection(_scrollSpy.CenteredSection);
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_scrollSpy is null)
            {
                return;
            }

            _scrollSpy.ScrollSectionSectionCentered -= ScrollSpy_ScrollSectionSectionCentered;
            if (IsJSRuntimeAvailable)
            {
                await _scrollSpy.DisposeAsync();
            }
        }
    }
}
