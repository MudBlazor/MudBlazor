using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System.Collections.Generic;
using System.Linq;

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
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        public List<MudExpansionPanel> Panels = new List<MudExpansionPanel>();

        internal void AddPanel(MudExpansionPanel panel)
        {
            Panels.Add(panel);
            StateHasChanged();
        }

        public void RemovePanel(MudExpansionPanel panel)
        {
            Panels.Remove(panel);
            try
            {
                StateHasChanged();
            }
            catch(InvalidOperationException) { /* this happens on page reload, probably a Blazor bug */ }
        }

        public void UpdateAll()
        {
            MudExpansionPanel last = null;
            foreach (var panel in Panels)
            {
                if (last != null)
                    last.NextPanelExpanded = panel.IsExpanded;
                last = panel;
            }
        }

        public void CloseAllExcept(MudExpansionPanel panel)
        {
            foreach (var p in Panels)
            {
                if (p == panel)
                    continue;
                p.Collapse(update_parent:false);
            }
            UpdateAll();
        }

     
    }
}
