﻿// Copyright (c) MudBlazor 2021
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

namespace MudBlazor.Docs.Components
{
    public partial class DocsPage : ComponentBase
    {
        private Queue<DocsSectionLink> _bufferedSections = new();
        private MudPageContentNavigation _contentNavigation;
        private NavigationFooterLink _previous;
        private NavigationFooterLink _next;
        private NavigationSection? _section = null;
        private Stopwatch _stopwatch = Stopwatch.StartNew();

        [Inject] NavigationManager NavigationManager { get; set; }

        [Inject] private IDocsNavigationService DocsService { get; set; }

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

        protected override void OnParametersSet()
        {
            _previous = DocsService.Previous;
            _next = DocsService.Next;
            _section = DocsService.Section;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Rendered?.Invoke(_stopwatch);
                await _contentNavigation.ScrollToSection(new Uri(NavigationManager.Uri));
            }
        }

        internal void AddSection(DocsSectionLink section)
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
            }
        }
    }
}
