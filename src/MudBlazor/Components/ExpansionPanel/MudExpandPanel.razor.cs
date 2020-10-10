using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System;
using System.Linq;

namespace MudBlazor
{
    public partial class MudExpandPanel : MudComponentBase
    {
        [CascadingParameter] private MudExpansionPanels Parent { get; set; }

        protected string Classname =>
        new CssBuilder("mud-expand-panel")
            .AddClass("mud-panel-expanded", Expanded)
            .AddClass("mud-panel-next-expanded", NextPanelExpanded)
            .AddClass("mud-disabled", Disabled)
            .AddClass($"mud-elevation-{Parent.Elevation.ToString()}")
        .Build();

        [Parameter] public string Text { get; set; }
        [Parameter] public bool Expanded { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        private bool NextPanelExpanded { get; set; }
        private int ActivePanelIndex { get; set; }
        private int PreviousPanelIndex { get; set; }
        public void ExpandPanel()
        {
            if (!Disabled)
            {
                if (Parent.MultipleExpand)
                {
                    SetExpanded();
                    SetPreviousPanelIndex();
                }
                else
                {
                    SetExpanded();
                    SetPreviousPanelIndex();
                    if (Parent.ExpandedPanelSet)
                    {
                        Parent.ExpandPanel();
                    }
                    Parent.SetExpandedPanel(ActivePanelIndex);
                }
            }
        }

        public void SetExpanded()
        {
            Expanded = !Expanded;
        }

        public void SetNextPanelExpanded()
        {
            NextPanelExpanded = !NextPanelExpanded;
        }

        public void SetPreviousPanelIndex()
        {
            if (ActivePanelIndex >= 1)
            {
                PreviousPanelIndex = ActivePanelIndex - 1;
                var PreviousPanel = Parent.Panels.ElementAt(PreviousPanelIndex);
                Parent.AddPreviousPanelClass(PreviousPanel);
            }
        }

        protected override void OnInitialized()
        {
            if (Parent == null)
                throw new ArgumentNullException(nameof(Parent), "ExpandPanel must exist within a ExpansionPanels component");
            base.OnInitialized();
            Parent.AddPanel(this);
            ActivePanelIndex = Parent.Panels.IndexOf(this);
        }
    }
}
