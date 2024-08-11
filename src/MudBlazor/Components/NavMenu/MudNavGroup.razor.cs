using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudNavGroup : MudComponentBase
    {
        private readonly ParameterState<bool> _expandedState;
        private readonly ParameterState<bool> _disabledState;
        private readonly ParameterState<NavigationContext?> _parentNavigationContextState;
        private NavigationContext _navigationContext = new(false, true);

        public MudNavGroup()
        {
            using var registerScope = CreateRegisterScope();
            _disabledState = registerScope.RegisterParameter<bool>(nameof(Disabled))
                .WithParameter(() => Disabled)
                .WithChangeHandler(UpdateNavigationContext);
            _parentNavigationContextState = registerScope.RegisterParameter<NavigationContext?>(nameof(ParentNavigationContext))
                .WithParameter(() => ParentNavigationContext)
                .WithChangeHandler(UpdateNavigationContext);
            _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
                .WithParameter(() => Expanded)
                .WithEventCallback(() => ExpandedChanged)
                .WithChangeHandler(UpdateNavigationContext);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            UpdateNavigationContext();
        }

        protected string Classname =>
            new CssBuilder("mud-nav-group")
                .AddClass(Class)
                .AddClass("mud-nav-group-disabled", _disabledState.Value)
                .Build();

        protected string ButtonClassname =>
            new CssBuilder("mud-nav-link")
                .AddClass($"mud-ripple", Ripple)
                .AddClass("mud-expanded", _expandedState.Value)
                .Build();

        protected string IconClassname =>
            new CssBuilder("mud-nav-link-icon")
                .AddClass("mud-nav-link-icon-default", IconColor == Color.Default)
                .Build();

        protected string ExpandIconClassname =>
            new CssBuilder("mud-nav-link-expand-icon")
                .AddClass("mud-transform", _expandedState.Value && _disabledState.Value is false)
                .AddClass("mud-transform-disabled", _expandedState.Value && _disabledState.Value)
                .Build();

        protected int ButtonTabIndex => _disabledState.Value || _parentNavigationContextState.Value is { Disabled: true } or { Expanded: false } ? -1 : 0;

        [CascadingParameter]
        private NavigationContext? ParentNavigationContext { get; set; }

        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public string? Title { get; set; }

        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors, default value uses the themes drawer icon color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public Color IconColor { get; set; } = Color.Default;

        /// <summary>
        /// If true, the button will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// If true, expands the nav group, otherwise collapse it.
        /// Two-way bindable
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public bool Expanded { get; set; }

        /// <summary>
        /// If true, hides expand-icon at the end of the NavGroup.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public bool HideExpandIcon { get; set; }

        /// <summary>
        /// Explicitly sets the height for the Collapse element to override the css default.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public int? MaxHeight { get; set; }

        /// <summary>
        /// If set, overrides the default expand icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public string ExpandIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        private async Task ExpandedToggleAsync()
        {
            await _expandedState.SetValueAsync(!_expandedState.Value);
            UpdateNavigationContext();
        }

        private void UpdateNavigationContext()
            => _navigationContext = _navigationContext with
            {
                Disabled = _disabledState.Value || _parentNavigationContextState.Value is { Disabled: true },
                Expanded = _expandedState.Value
                           && _parentNavigationContextState.Value is null or { Expanded: true }
            };
    }
}
