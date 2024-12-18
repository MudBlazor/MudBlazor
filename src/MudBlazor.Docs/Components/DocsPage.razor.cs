// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;
using MudBlazor.Interfaces;

namespace MudBlazor.Docs.Components
{
    public partial class DocsPage : ComponentBase
    {
        [Parameter] public bool DisplayFooter { get; set; }

        private Queue<DocsSectionLink> _bufferedSections = new();
        private MudPageContentNavigation _contentNavigation;
        private NavigationFooterLink _previous;
        private NavigationFooterLink _next;
        private NavigationSection _section;
        private Stopwatch _stopwatch;
        private string _anchor = null;
        private bool _renderAds;
        [Inject] NavigationManager NavigationManager { get; set; }

        [Inject] private IDocsNavigationService DocsService { get; set; }
        [Inject] private IRenderQueueService RenderQueue { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        private bool _contentDrawerOpen = true;
        public event Action<Stopwatch> Rendered;
        private Dictionary<DocsPageSection, MudPageContentSection> _sectionMapper = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            if (relativePath.Contains('#'))
            {
                _anchor = relativePath.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries)[1];
            }
        }

        protected override void OnParametersSet()
        {
            _stopwatch = Stopwatch.StartNew();
            _previous = DocsService.Previous;
            _next = DocsService.Next;
            _section = DocsService.Section;
            RenderQueue.Clear();
            _bufferedSections.Clear();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                Rendered?.Invoke(_stopwatch);
            }

            if (firstRender)
            {
                _renderAds = true;
                StateHasChanged();
            }
        }

        public string GetParentTitle(DocsPageSection section)
        {
            if (section == null)
            {
                return string.Empty;
            }

            if (section.ParentSection == null || _sectionMapper.ContainsKey(section.ParentSection) == false)
            {
                return string.Empty;
            }

            var item = _sectionMapper[section.ParentSection];

            return item.Title;
        }

        internal async Task AddSectionAsync(DocsSectionLink sectionLinkInfo, DocsPageSection section)
        {
            _bufferedSections.Enqueue(sectionLinkInfo);

            if (_contentNavigation != null)
            {
                while (_bufferedSections.Count > 0)
                {
                    _ = _bufferedSections.Dequeue();

                    if (_contentNavigation.Sections.FirstOrDefault(x => x.Id == sectionLinkInfo.Id) == default)
                    {
                        MudPageContentSection parentInfo = null;
                        if (section.ParentSection != null && _sectionMapper.TryGetValue(section.ParentSection, out var value))
                        {
                            parentInfo = value;
                        }

                        var info =
                            new MudPageContentSection(sectionLinkInfo.Title, sectionLinkInfo.Id, section.Level,
                                parentInfo);
                        _sectionMapper.Add(section, info);
                        _contentNavigation.AddSection(info, false);
                    }
                }

                ((IMudStateHasChanged)_contentNavigation).StateHasChanged();

                if (_anchor != null)
                {
                    if (sectionLinkInfo.Id == _anchor)
                    {
                        await _contentNavigation.ScrollToSection(new Uri(NavigationManager.Uri));
                        _anchor = null;
                    }
                }
            }
        }
    }
}
