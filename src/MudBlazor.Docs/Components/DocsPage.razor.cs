// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        private Stopwatch _stopwatch = Stopwatch.StartNew();
        private string _anchor = null;
        private bool _renderAds;
        [Inject] NavigationManager NavigationManager { get; set; }

        [Inject] private IDocsNavigationService DocsService { get; set; }
        [Inject] private IRenderQueueService RenderQueue { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        private bool _contentDrawerOpen = true;
        public event Action<Stopwatch> Rendered;
        private Dictionary<DocsPageSection, MudPageContentSection> _sectionMapper = new();
        private string _typeName;
        private DocumentedType _type;

        /// <summary>
        /// Whether this page shows API documentation.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool IsApi { get; set; }

        /// <summary>
        /// The documentation related to this page.
        /// </summary>
        /// <remarks>
        /// Can also be set via <see cref="TypeName"/>.  Contains all of the XML documentation related to this page.
        /// </remarks>
        [Parameter]
        public DocumentedType Type
        {
            get => _type;
            set
            {
                _type = value;
                _typeName = value == null ? null : _type!.Name;
                StateHasChanged();
            }
        }

        /// <summary>
        /// The name of the type related to this page.
        /// </summary>
        /// <remarks>
        /// Can also be set via <see cref="Type"/>.  When set, all of the XML documentation related to this page is available via <see cref="Type"/>.
        /// </remarks>
        [Parameter]
        public string TypeName
        {
            get => _typeName;
            set
            {
                _typeName = value?.Replace("%601", "`1");
                _type = value == null ? null : ApiDocumentation.GetType(_typeName);
                StateHasChanged();
            }
        }

        private int _sectionCount;

        public int SectionCount
        {
            get
            {
                lock (this)
                    return _sectionCount;
            }
        }

        public int IncrementSectionCount()
        {
            lock (this)
                return _sectionCount++;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            RenderQueue.Clear();
            var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            if (relativePath.Contains('#'))
            {
                _anchor = relativePath.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries)[1];
            }
        }

        protected override void OnParametersSet()
        {
            _stopwatch = Stopwatch.StartNew();
            _sectionCount = 0;
            _previous = DocsService.Previous;
            _next = DocsService.Next;
            _section = DocsService.Section;
            IsApi = NavigationManager.Uri.ToString().Contains("/api/");
            TypeName = NavigationManager.Uri.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
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
