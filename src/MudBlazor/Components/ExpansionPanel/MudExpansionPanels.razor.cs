using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                .AddClass("mud-expansion-panels-square", Square)
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
        /// If true, left and right padding is added to all panels' <see cref="ChildContent"/>. Default is true
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public bool Gutters { get; set; } = true;

        /// <summary>
        /// Determines whether the borders around each panel are shown.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public bool Outlined { get; set; } = true;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        internal async Task AddPanelAsync(MudExpansionPanel panel)
        {
            if (!MultiExpansion && _panels.Any(p => p._expandedState.Value))
            {
                await panel.CollapseAsync();
            }

            _panels.Add(panel);
        }

        internal void RemovePanel(MudExpansionPanel panel)
        {
            _panels.Remove(panel);
            try
            {
                StateHasChanged();
            }
            catch (InvalidOperationException) { /* this happens on page reload, probably a Blazor bug */ }
        }

        internal async Task NotifyPanelsChanged(MudExpansionPanel panel)
        {
            if (!MultiExpansion && panel._expandedState.Value)
            {
                await CollapseAllExceptAsync(panel);
                return;
            }

            await UpdateAllAsync();
        }

        public Task UpdateAllAsync()
        {
            MudExpansionPanel? last = null;
            foreach (var panel in _panels)
            {
                if (last is not null)
                {
                    last.NextPanelExpanded = panel._expandedState.Value;
                }

                last = panel;
            }
            StateHasChanged();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Collapses all panels except the given one.
        /// </summary>
        /// <param name="panel">The panel not to collapse.</param>
        public async Task CollapseAllExceptAsync(MudExpansionPanel panel)
        {
            foreach (var expansionPanel in _panels)
            {
                if (expansionPanel == panel)
                {
                    continue;
                }

                await expansionPanel.CollapseAsync();
            }
            await InvokeAsync(UpdateAllAsync);
        }

        /// <summary>
        /// Collapses all panels.
        /// </summary>
        public async Task CollapseAllAsync()
        {
            foreach (var expansionPanel in _panels)
            {
                await expansionPanel.CollapseAsync();
            }
            await InvokeAsync(UpdateAllAsync);
        }

        /// <summary>
        /// Expands all panels.
        /// </summary>
        public async Task ExpandAllAsync()
        {
            foreach (var expansionPanel in _panels)
            {
                await expansionPanel.ExpandAsync();
            }
            await InvokeAsync(UpdateAllAsync);
        }
    }
}
