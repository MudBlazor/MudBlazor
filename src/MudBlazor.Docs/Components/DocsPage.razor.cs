// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;
using Microsoft.AspNetCore.Components.Routing;

namespace MudBlazor.Docs.Components
{
    public partial class DocsPage : ComponentBase
    {
        private Queue<DocsSectionLink> _bufferedSections = new();
        private MudPageContentNavigation _contentNavigation;
        private NavigationFooterLink _previous;
        private NavigationFooterLink _next;
        private NavigationSection? _section = null;
        private string _anchor=null;
        private Stopwatch _stopwatch = Stopwatch.StartNew();

        [Inject] NavigationManager NavigationManager { get; set; }

        [Inject] private IDocsNavigationService DocsService { get; set; }
        [Inject] private IRenderQueueService RenderQueue { get; set; }

        [Parameter] public MaxWidth MaxWidth { get; set; } = MaxWidth.Medium;
        [Parameter] public RenderFragment ChildContent { get; set; }

        private bool _contentDrawerOpen = true;
        public event Action<Stopwatch> Rendered;

        int _sectionCount;
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
            var relativePath=NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            if (relativePath.Contains("#"))
                _anchor = relativePath.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        protected override void OnParametersSet()
        {
            _stopwatch = Stopwatch.StartNew();
            _sectionCount = 0;
            _previous = DocsService.Previous;
            _next = DocsService.Next;
            _section = DocsService.Section;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                Rendered?.Invoke(_stopwatch);
            }
        }

        internal async void AddSection(DocsSectionLink section)
        {
            _bufferedSections.Enqueue(section);
            if (_contentNavigation != null)
            {
                while (_bufferedSections.Count > 0)
                {
                    var item = _bufferedSections.Dequeue();

                    if (_contentNavigation.Sections.FirstOrDefault(x => x.Id == section.Id) == default)
                    {
                        _contentNavigation.AddSection(item.Title, item.Id, false);
                    }
                }
                _contentNavigation.Update();
                if (_anchor!=null)
                {
                    if (section.Id == _anchor)
                    {
                        await _contentNavigation.ScrollToSection(new Uri(NavigationManager.Uri));
                        _anchor= null;
                    }
                }
            }
        }

    }
}
