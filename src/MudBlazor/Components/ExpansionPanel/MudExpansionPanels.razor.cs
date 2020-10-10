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
        .Build();

        [Parameter] public bool Square { get; set; }
        [Parameter] public bool MultiExpansion { get; set; }
        [Parameter] public int Elevation { set; get; } = 1;
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
            StateHasChanged();
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
