using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudExpansionPanel : MudComponentBase, IDisposable
    {
        private bool _nextPanelExpanded;
        private bool _isExpanded;
        [CascadingParameter] private MudExpansionPanels Parent { get; set; }

        protected string Classname =>
        new CssBuilder("mud-expand-panel")
            .AddClass("mud-panel-expanded", IsExpanded)
            .AddClass("mud-panel-next-expanded", NextPanelExpanded)
            .AddClass("mud-disabled", Disabled)
            .AddClass($"mud-elevation-{Parent?.Elevation.ToString()}")
            .AddClass($"mud-expand-panel-border", Parent?.DisableBorders != true)
            .AddClass(Class)
        .Build();

        protected string PanelContentClassname =>
        new CssBuilder("mud-expand-panel-content")
            .AddClass("mud-expand-panel-gutters", DisableGutters || Parent?.DisableGutters == true)
            .AddClass("mud-expand-panel-dense", Dense || Parent?.Dense == true)
        .Build();

        /// <summary>
        /// Explicitly sets the height for the Collapse element to override the css default.
        /// </summary>
        [Parameter] public int? MaxHeight { get; set; }

        /// <summary>
        /// RenderFragment to be displayed in the expansion panel which will override header text if defined.
        /// </summary>
        [Parameter] public RenderFragment TitleContent { get; set; }

        /// <summary>
        /// The text to be displayed in the expansion panel.
        /// </summary>
        [Parameter] public string Text { get; set; }

        /// <summary>
        /// If true, expand icon will not show
        /// </summary>
        [Parameter] public bool HideIcon { get; set; }

        /// <summary>
        /// If true, removes vertical padding from childcontent.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed from childcontent.
        /// </summary>
        [Parameter] public bool DisableGutters { get; set; }

        /// <summary>
        /// Raised when IsExpanded changes.
        /// </summary>
        [Parameter] public EventCallback<bool> IsExpandedChanged { get; set; }

        /// <summary>
        /// Expansion state of the panel (two-way bindable)
        /// </summary>
        [Parameter]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value)
                    return;
                _isExpanded = value;
                if (Parent?.MultiExpansion == true)
                    Parent?.UpdateAll();
                else
                    Parent?.CloseAllExcept(this);
                //InvokeAsync(StateHasChanged);
                IsExpandedChanged.InvokeAsync(_isExpanded);
            }
        }

        /// <summary>
        /// If true, the component will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        public bool NextPanelExpanded
        {
            get => _nextPanelExpanded;
            set
            {
                if (_nextPanelExpanded == value)
                    return;
                _nextPanelExpanded = value;
                InvokeAsync(StateHasChanged);
            }
        }

        public void ToggleExpansion()
        {
            if (Disabled)
                return;
            if (Parent?.MultiExpansion == true)
            {
                IsExpanded = !IsExpanded;
            }
            else
            {
                IsExpanded = !IsExpanded;
            }
        }

        public void Expand(bool update_parent = true)
        {
            if (update_parent)
                IsExpanded = true;
            else
            {
                _isExpanded = true;
                IsExpandedChanged.InvokeAsync(_isExpanded);
            }
        }

        public void Collapse(bool update_parent = true)
        {
            if (update_parent)
                IsExpanded = false;
            else
            {
                _isExpanded = false;
                IsExpandedChanged.InvokeAsync(_isExpanded);
            }
        }

        protected override void OnInitialized()
        {
            // NOTE: we can't throw here because we need to be able to instanciate the type for the API Docs to infer default values
            //if (Parent == null)
            //    throw new ArgumentNullException(nameof(Parent), "ExpansionPanel must exist within a ExpansionPanels component");
            base.OnInitialized();
            Parent?.AddPanel(this);
        }

        public void Dispose()
        {
            Parent?.RemovePanel(this);
        }

    }
}
