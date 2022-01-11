using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// If true, multiple panels can be expanded at the same time.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Behavior)]
        public bool MultiExpansion { get; set; }

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public int Elevation { set; get; } = 1;

        /// <summary>
        /// If true, removes vertical padding from all panels' childcontent.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed from all panels' childcontent.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public bool DisableGutters { get; set; }

        /// <summary>
        /// If true, the borders around each panel will be removed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public bool DisableBorders { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Behavior)]
        public RenderFragment ChildContent { get; set; }

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
                CollapseAllExcept(panel);
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

        [Obsolete("Use CollapseAllExcept instead.")]
        [ExcludeFromCodeCoverage]
        public void CloseAllExcept(MudExpansionPanel panel)
        {
            CollapseAllExcept(panel);
        }

        /// <summary>
        /// Collapses all panels except the given one.
        /// </summary>
        /// <param name="panel">The panel not to collapse.</param>
        public void CollapseAllExcept(MudExpansionPanel panel)
        {
            foreach (var p in _panels)
            {
                if (p == panel)
                    continue;
                p.Collapse(update_parent: false);
            }
            this.InvokeAsync(UpdateAll);
        }

        /// <summary>
        /// Collapses all panels.
        /// </summary>
        public void CollapseAll()
        {
            foreach (var p in _panels)
            {
                p.Collapse(update_parent: false);
            }
            this.InvokeAsync(UpdateAll);
        }

        /// <summary>
        /// Expands all panels.
        /// </summary>
        public void ExpandAll()
        {
            foreach (var p in _panels)
            {
                p.Expand(update_parent: false);
            }
            this.InvokeAsync(UpdateAll);
        }
    }
}
