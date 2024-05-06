using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudExpansionPanel : MudComponentBase, IDisposable
    {
        internal readonly ParameterState<bool> _expandedState;

        [CascadingParameter]
        private MudExpansionPanels? Parent { get; set; }

        protected string Classname =>
            new CssBuilder("mud-expand-panel")
                .AddClass("mud-panel-expanded", _expandedState.Value)
                .AddClass("mud-panel-next-expanded", NextPanelExpanded)
                .AddClass("mud-disabled", Disabled)
                .AddClass($"mud-elevation-{Parent?.Elevation.ToString()}")
                .AddClass($"mud-expand-panel-border", Parent?.Outlined == true)
                .AddClass(Class)
                .Build();

        protected string PanelContentClassname =>
            new CssBuilder("mud-expand-panel-content")
                .AddClass("mud-expand-panel-disable-gutters", !Gutters && Parent?.Gutters != true)
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
        /// If true, left and right padding is added to the <see cref="ChildContent"/>. Default is true .
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Appearance)]
        public bool Gutters { get; set; } = true;

        /// <summary>
        /// Raised when <see cref="Expanded"/> changes.
        /// </summary>
        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        /// <summary>
        /// Expansion state of the panel (two-way bindable)
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ExpansionPanel.Behavior)]
        public bool Expanded { get; set; }

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

        public MudExpansionPanel()
        {
            using var registerScope = CreateRegisterScope();
            _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
                .WithParameter(() => Expanded)
                .WithEventCallback(() => ExpandedChanged)
                .WithChangeHandler(OnExpandedParameterChangedAsync);
        }

        private Task OnExpandedParameterChangedAsync(ParameterChangedEventArgs<bool> args)
        {
            if (Parent is null)
            {
                return Task.CompletedTask;
            }

            return Parent.NotifyPanelsChanged(this);
        }

        public async Task ToggleExpansionAsync()
        {
            if (Disabled)
            {
                return;
            }

            await _expandedState.SetValueAsync(!_expandedState.Value);
            if (Parent is not null)
            {
                await Parent.NotifyPanelsChanged(this);
            }
        }

        public async Task ExpandAsync()
        {
            await _expandedState.SetValueAsync(true);
            if (Parent is not null)
            {
                await Parent.NotifyPanelsChanged(this);
            }
        }

        public async Task CollapseAsync()
        {
            await _expandedState.SetValueAsync(false);
            if (Parent is not null)
            {
                await Parent.NotifyPanelsChanged(this);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            // NOTE: we can't throw here because we need to be able to instantiate the type for the API Docs to infer default values
            //if (Parent == null)
            //    throw new ArgumentNullException(nameof(Parent), "ExpansionPanel must exist within a ExpansionPanels component");
            await base.OnInitializedAsync();
            if (Parent is not null)
            {
                await Parent.AddPanelAsync(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Parent?.RemovePanel(this);
            }
        }
    }
}
