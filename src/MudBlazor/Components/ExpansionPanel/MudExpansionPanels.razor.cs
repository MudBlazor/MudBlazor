using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudExpansionPanels : MudComponentBase
    {
        private List<MudExpansionPanel> _panels = new();

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
        /// If true, removes vertical padding from all panels' <see cref="ChildContent"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed from all panels' <see cref="ChildContent"/>.
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
        public RenderFragment? ChildContent { get; set; }

        internal void AddPanel(MudExpansionPanel panel)
        {
            if (!MultiExpansion && _panels.Any(p => p.IsExpanded))
            {
                panel.Collapse(updateParent: false);
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
            if(!MultiExpansion && panel.IsExpanded)
            {
                CollapseAllExcept(panel);
                return;
            }

            UpdateAll();
        }

        public void UpdateAll()
        {
            MudExpansionPanel? last = null;
            foreach (var panel in _panels)
            {
                if (last is not null)
                {
                    last.NextPanelExpanded = panel.IsExpanded;
                }

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
            foreach (var expansionPanel in _panels)
            {
                if (expansionPanel == panel)
                {
                    continue;
                }

                expansionPanel.Collapse(updateParent: false);
            }
            InvokeAsync(UpdateAll);
        }

        /// <summary>
        /// Collapses all panels.
        /// </summary>
        public void CollapseAll()
        {
            foreach (var expansionPanel in _panels)
            {
                expansionPanel.Collapse(updateParent: false);
            }
            InvokeAsync(UpdateAll);
        }

        /// <summary>
        /// Expands all panels.
        /// </summary>
        public void ExpandAll()
        {
            foreach (var expansionPanel in _panels)
            {
                expansionPanel.Expand(updateParent: false);
            }
            InvokeAsync(UpdateAll);
        }
    }
}
