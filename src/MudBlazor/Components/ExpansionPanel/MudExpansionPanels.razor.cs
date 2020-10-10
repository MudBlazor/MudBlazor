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
        [Parameter] public bool MultipleExpand { get; set; }
        [Parameter] public int Elevation { set; get; } = 1;
        [Parameter] public RenderFragment ChildContent { get; set; }

        public bool ExpandedPanelSet { get; set; }
        public int ExpandedPanelIndex { get; set; }
        public List<MudExpandPanel> Panels = new List<MudExpandPanel>();

        internal void AddPanel(MudExpandPanel expandPanel)
        {
            Panels.Add(expandPanel);
            StateHasChanged();
        }

        internal void AddPreviousPanelClass(MudExpandPanel expandPanel)
        {
            expandPanel.SetNextPanelExpanded();
            StateHasChanged();
        }

        internal void SetExpandedPanel(int index)
        {
            ExpandedPanelIndex = index;
            ExpandedPanelSet = true;
        }

        internal void ExpandPanel()
        {
            Panels.ElementAt(ExpandedPanelIndex).SetExpanded();
            Panels.ElementAt(ExpandedPanelIndex).SetPreviousPanelIndex();
            StateHasChanged();
        }
    }
}
