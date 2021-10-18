﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudExpansionPanels : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-expansion-panels")
            .AddClass($"mud-expansion-panels-square", Square)
            .AddClass(Class)
        .Build();

        /// <summary>
        /// If true, border-radius is set to 0.
        /// </summary>
        [Parameter] public bool Square { get; set; }

        /// <summary>
        /// If true, multiple panels can be expanded at the same time.
        /// </summary>
        [Parameter] public bool MultiExpansion { get; set; }

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 1;

        /// <summary>
        /// If true, removes vertical padding from all panels' childcontent.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed from all panels' childcontent.
        /// </summary>
        [Parameter] public bool DisableGutters { get; set; }

        /// <summary>
        /// If true, the borders around each panel will be removed.
        /// </summary>
        [Parameter] public bool DisableBorders { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        private List<MudExpansionPanel> _panels = new();

        internal void AddPanel(MudExpansionPanel panel)
        {
            if (MultiExpansion == false && _panels.Any(p => p.IsExpanded))
            {
                panel.Collapse(update_parent: false);
            }

            panel.NotifyIsExpandedChanged += UpdatePanelsOnPanelsChanged;
            _panels.Add(panel);
        }

        public void RemovePanel(MudExpansionPanel panel)
        {
            panel.NotifyIsExpandedChanged -= UpdatePanelsOnPanelsChanged;
            _panels.Remove(panel);
            try
            {
                StateHasChanged();
            }
            catch (InvalidOperationException) { /* this happens on page reload, probably a Blazor bug */ }
        }

        internal void UpdatePanelsOnPanelsChanged(MudExpansionPanel panel)
        {
            if(MultiExpansion == false && panel.IsExpanded)
            {
                CloseAllExcept(panel);
                return;
            }

            UpdateAll();
        }

        public void UpdateAll()
        {
            MudExpansionPanel last = null;
            foreach (var panel in _panels)
            {
                if (last != null)
                    last.NextPanelExpanded = panel.IsExpanded;
                last = panel;
            }
            StateHasChanged();
        }

        public void CloseAllExcept(MudExpansionPanel panel)
        {
            foreach (var p in _panels)
            {
                if (p == panel)
                    continue;
                p.Collapse(update_parent: false);
            }
            UpdateAll();
        }
    }
}
