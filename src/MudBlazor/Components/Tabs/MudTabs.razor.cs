using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using MudBlazor.Services;
using System.Globalization;

namespace MudBlazor
{
    public partial class MudTabs : MudComponentBase, IDisposable
    {
        private bool _isDisposed;
        public ElementReference TabsRef;
        private double _size;
        private double _position;

        protected string TabsClassnames =>
            new CssBuilder("mud-tabs")
            .AddClass($"mud-tabs-reverse", !Vertical && TabsPlacement == Placement.Bottom)
            .AddClass($"mud-tabs-vertical", Vertical)
            .AddClass($"mud-tabs-vertical-reverse", Vertical && TabsPlacement == Placement.End)
            .AddClass(Class)
            .Build();

        protected string ToolbarClassnames =>
            new CssBuilder("mud-tabs-toolbar")
            .AddClass($"mud-tabs-rounded", Rounded)
            .AddClass($"mud-tabs-vertical", Vertical)
            .AddClass($"mud-tabs-toolbar-{Color.ToDescriptionString()}", Color != Color.Default)
            .AddClass($"mud-border-right", Border)
            .AddClass($"mud-paper-outlined", Outlined)
            .AddClass($"mud-elevation-{Elevation}", Elevation != 0)
            .Build();

        protected string WrapperClassnames =>
            new CssBuilder("mud-tabs-toolbar-wrapper")
            .AddClass($"mud-tabs-centered", Centered)
            .AddClass($"mud-tabs-vertical", Vertical)
            .Build();

        protected string PanelsClassnames =>
            new CssBuilder("mud-tabs-panels")
            .AddClass($"mud-tabs-vertical", Vertical)
            .AddClass(PanelClass)
            .Build();

        protected string SliderClass =>
            new CssBuilder("mud-tab-slider")
            .AddClass($"mud-tab-slider-horizontal", !Vertical)
            .AddClass($"mud-tab-slider-vertical", Vertical)
            .AddClass($"mud-tab-slider-horizontal-reverse", !Vertical && TabsPlacement == Placement.Bottom)
            .AddClass($"mud-tab-slider-vertical-reverse", Vertical && TabsPlacement == Placement.End)
            .Build();

        protected string SliderStyle =>
        new StyleBuilder()
            .AddStyle("width", $"{_size.ToString(CultureInfo.InvariantCulture)}px", !Vertical)
            .AddStyle("left", $"{_position.ToString(CultureInfo.InvariantCulture)}px", !Vertical)
            .AddStyle("height", $"{_size.ToString(CultureInfo.InvariantCulture)}px", Vertical)
            .AddStyle("top", $"{_position.ToString(CultureInfo.InvariantCulture)}px", Vertical)
        .Build();

        [Inject] public IDomService DomService { get; set; }

        /// <summary>
        /// If true, render all tabs and hide (display:none) every non-active.
        /// </summary>
        [Parameter] public bool KeepPanelsAlive { get; set; } = false;

        /// <summary>
        /// If true, sets the border-radius to theme default.
        /// </summary>
        [Parameter] public bool Rounded { get; set; }

        /// <summary>
        /// If true, sets a border.
        /// </summary>
        [Parameter] public bool Border { get; set; }

        /// <summary>
        /// If true, toolbar will be outlined.
        /// </summary>
        [Parameter] public bool Outlined { get; set; }

        /// <summary>
        /// If true, centers the tabitems.
        /// </summary>
        [Parameter] public bool Centered { get; set; }

        /// <summary>
        /// If true, displays the MudTabs verticaly.
        /// </summary>
        [Parameter] public bool Vertical { get; set; }

        [Parameter] public Placement TabsPlacement { get; set; } = Placement.Top;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 0;

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Custom class/classes for TabPanel
        /// </summary>
        [Parameter] public string TabPanelClass { get; set; }

        /// <summary>
        /// Custom class/classes for Selected Content Panel
        /// </summary>
        [Parameter] public string PanelClass { get; set; }

        public MudTabPanel ActivePanel { get; set; }

        private int _activePanelIndex = 0;
        /// <summary>
        /// The current active panel index. Also with Bidirectional Binding
        /// </summary>
        [Parameter]
        public int ActivePanelIndex
        {
            get => _activePanelIndex;
            set
            {
                if (_activePanelIndex != value)
                {
                    _activePanelIndex = value;
                    ActivatePanel(_activePanelIndex);
                    ActivePanelIndexChanged.InvokeAsync(value);
                }
            }
        }

        /// <summary>
        /// Fired when ActivePanelIndex changes.
        /// </summary>
        [Parameter]
        public EventCallback<int> ActivePanelIndexChanged { get; set; }

        public List<MudTabPanel> Panels = new List<MudTabPanel>();

        public void Dispose() => _isDisposed = true;

        internal void AddPanel(MudTabPanel tabPanel)
        {
            Panels.Add(tabPanel);
            if (Panels.Count == 1)
                ActivePanel = tabPanel;
            StateHasChanged();
        }

        internal void RemovePanel(MudTabPanel tabPanel)
        {
            if (_isDisposed)
                return;

            var index = Panels.IndexOf(tabPanel);
            if (ActivePanelIndex == index && index == Panels.Count - 1)
            {
                ActivePanelIndex = index > 0 ? index - 1 : 0;
                if (Panels.Count == 1)
                    ActivePanel = null;
            }
            Panels.Remove(tabPanel);
            StateHasChanged();
        }

        string GetTabClass(MudTabPanel panel)
        {
            var tabClass = new CssBuilder("mud-tab")
              .AddClass($"mud-tab-active", when: () => panel == ActivePanel)
              .AddClass($"mud-disabled", panel.Disabled)
              .AddClass($"mud-ripple", !DisableRipple)
              .AddClass(TabPanelClass)
            .Build();

            return tabClass;
        }
        void ActivatePanel(MudTabPanel panel, MouseEventArgs ev)
        {
            if (!panel.Disabled)
            {
                ActivePanel = panel;
                ActivePanelIndex = Panels.IndexOf(panel);
                _ = UpdateSlider();
                if (ev != null)
                    ActivePanel.OnClick.InvokeAsync(ev);
            }
        }

        public void ActivatePanel(MudTabPanel panel)
        {
            ActivatePanel(panel, null);
        }

        public void ActivatePanel(int index)
        {
            var panel = Panels[index];
            ActivatePanel(panel, null);
        }

        public void ActivatePanel(object id)
        {
            var panel = Panels.Where((p) => p.ID == id).FirstOrDefault();
            if (panel != null)
                ActivatePanel(panel, null);
        }

        private Placement GetTooltipPlacement()
        {
            if (Vertical)
            {
                if (TabsPlacement == Placement.End)
                    return Placement.Start;
                else
                    return Placement.End;
            }
            else
            {
                if (TabsPlacement == Placement.Bottom)
                    return Placement.Top;
                else
                    return Placement.Bottom;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await UpdateSlider();
            }
        }

        private async Task UpdateSlider()
        {
            if(ActivePanel != null)
            {
                if (!Vertical)
                    _size = (await DomService.GetBoundingClientRect(ActivePanel.PanelRef))?.Width ?? 0;
                else
                    _size = (await DomService.GetBoundingClientRect(ActivePanel.PanelRef))?.Height ?? 0;

                _position = 0;

                if (ActivePanelIndex != 0)
                {
                    double position = 0;
                    var counter = 0;
                    foreach (var panel in Panels) if (counter < ActivePanelIndex)
                        {
                            if (!Vertical)
                            {
                                position += (await DomService.GetBoundingClientRect(panel.PanelRef))?.Width ?? 0;
                            }
                            else
                            {
                                position += (await DomService.GetBoundingClientRect(panel.PanelRef))?.Height ?? 0;
                            }
                            counter++;
                        }
                    _position = position;
                }
                StateHasChanged();
            }
        }
    }
}
