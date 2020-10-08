using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor
{
    public partial class MudTabs : MudComponentBase
    {
        protected string TabsClassnames =>
            new CssBuilder("mud-tabs")
            .AddClass($"mud-tabs-vertical", Vertical)
            .Build();

        protected string ToolbarClassnames => 
            new CssBuilder("mud-tabs-toolbar")
            .AddClass($"mud-tabs-rounded", Rounded)
            .AddClass($"mud-tabs-vertical", Vertical)
            .AddClass($"mud-border-right", Border)
            .AddClass($"mud-elevation-{Elevation.ToString()}" , Elevation != 0)
            .Build();

        protected string WrapperClassnames =>
            new CssBuilder("mud-tabs-toolbar-wrapper")
            .AddClass($"mud-tabs-centered", Centered)
            .AddClass($"mud-tabs-vertical", Vertical)
            .Build();

        protected string PanelsClassnames =>
            new CssBuilder("mud-tabs-panels")
            .AddClass($"mud-tabs-vertical", Vertical)
            .Build();

        [Parameter] public bool Rounded { get; set; }
        [Parameter] public bool Border { get; set; }
        [Parameter] public bool Centered { get; set; }
        [Parameter] public bool Vertical { get; set; }
        [Parameter] public int Elevation { set; get; } = 0;
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }


        public MudTabPanel ActivePanel { get; set; }
        public int ActivePanelIndex { get; set; }

        public List<MudTabPanel> Panels = new List<MudTabPanel>();

        internal void AddPanel(MudTabPanel tabPanel)
        {
            Panels.Add(tabPanel);
            if (Panels.Count == 1)
                ActivePanel = tabPanel;
            StateHasChanged();

        }

        string GetTabClass(MudTabPanel panel)
        {
            var TabClass = new CssBuilder("mud-tab")
              .AddClass($"mud-tab-active", when: () => panel == ActivePanel)
              .AddClass($"mud-disabled", panel.Disabled)
              .AddClass($"mud-ripple" ,!DisableRipple)
            .Build();

            return TabClass;
        }
        void ActivatePanel(MudTabPanel panel)
        {
            ActivePanel = panel;
            ActivePanelIndex = Panels.IndexOf(panel);
        }
    }
}
