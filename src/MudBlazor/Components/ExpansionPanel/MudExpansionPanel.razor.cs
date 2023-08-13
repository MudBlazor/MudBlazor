using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudExpansionPanel : MudComponentBase, IDisposable
    {
        private bool _isExpanded;
        private bool _collapseIsExpanded;

        [CascadingParameter]
        private MudExpansionPanels? Parent { get; set; }

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
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public int? MaxHeight { get; set; }

        /// <summary>
        /// RenderFragment to be displayed in the expansion panel which will override header text if defined.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Behavior)]
        public RenderFragment? TitleContent { get; set; }

        /// <summary>
        /// The text to be displayed in the expansion panel.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Behavior)]
        public string? Text { get; set; }

        /// <summary>
        /// If true, expand icon will not show
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public bool HideIcon { get; set; }

        /// <summary>
        /// Custom hide icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public string Icon { get; set; } = Icons.Material.Filled.ExpandMore;

        /// <summary>
        /// If true, removes vertical padding from <see cref="ChildContent"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed from <see cref="ChildContent"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public bool DisableGutters { get; set; }

        /// <summary>
        /// Raised when IsExpanded changes.
        /// </summary>
        [Parameter]
        public EventCallback<bool> IsExpandedChanged { get; set; }

        internal event Action<MudExpansionPanel>? NotifyIsExpandedChanged;
        /// <summary>
        /// Expansion state of the panel (two-way bindable)
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Behavior)]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value)
                    return;
                _isExpanded = value;

                NotifyIsExpandedChanged?.Invoke(this);
                IsExpandedChanged.InvokeAsync(_isExpanded).ContinueWith(t =>
                {
                    if (_collapseIsExpanded != _isExpanded)
                    {
                        _collapseIsExpanded = _isExpanded;
                        InvokeAsync(StateHasChanged);
                    }
                });
            }
        }

        /// <summary>
        /// Sets the initial expansion state. Do not use in combination with IsExpanded.
        /// Combine with MultiExpansion to have more than one panel start open.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Behavior)]
        public bool IsInitiallyExpanded { get; set; }

        /// <summary>
        /// If true, the component will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        public bool NextPanelExpanded { get; set; }

        public void ToggleExpansion()
        {
            if (Disabled)
            {
                return;
            }

            IsExpanded = !IsExpanded;
        }

        public void Expand(bool updateParent = true)
        {
            if (updateParent)
                IsExpanded = true;
            else
            {
                _isExpanded = true;
                _collapseIsExpanded = true;
                IsExpandedChanged.InvokeAsync(_isExpanded);
            }
        }

        public void Collapse(bool updateParent = true)
        {
            if (updateParent)
                IsExpanded = false;
            else
            {
                _isExpanded = false;
                _collapseIsExpanded = false;
                IsExpandedChanged.InvokeAsync(_isExpanded);
            }
        }

        protected override void OnInitialized()
        {
            // NOTE: we can't throw here because we need to be able to instantiate the type for the API Docs to infer default values
            //if (Parent == null)
            //    throw new ArgumentNullException(nameof(Parent), "ExpansionPanel must exist within a ExpansionPanels component");
            base.OnInitialized();
            if (!IsExpanded && IsInitiallyExpanded)
            {
                _isExpanded = true;
                _collapseIsExpanded = true;
            }

            Parent?.AddPanel(this);
        }

        public void Dispose()
        {
            Parent?.RemovePanel(this);
        }
    }
}
