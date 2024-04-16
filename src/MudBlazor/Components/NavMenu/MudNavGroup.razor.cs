using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudNavGroup : MudComponentBase
    {
        public MudNavGroup()
        {
            using var register = CreateRegisterScope();
            _isExpandedState = register.RegisterParameter<bool>(nameof(IsExpanded))
                .WithParameter(() => IsExpanded)
                .WithEventCallback(() => IsExpandedChanged);
        }

        private readonly ParameterState<bool> _isExpandedState;

        protected string Classname =>
            new CssBuilder("mud-nav-group")
                .AddClass(Class)
                .AddClass($"mud-nav-group-disabled", Disabled)
                .Build();

        protected string ButtonClassname =>
            new CssBuilder("mud-nav-link")
                .AddClass($"mud-ripple", Ripple)
                .AddClass("mud-expanded", IsExpanded)
                .Build();

        protected string IconClassname =>
            new CssBuilder("mud-nav-link-icon")
                .AddClass($"mud-nav-link-icon-default", IconColor == Color.Default)
                .Build();

        protected string ExpandIconClassname =>
            new CssBuilder("mud-nav-link-expand-icon")
                .AddClass($"mud-transform", IsExpanded && !Disabled)
                .AddClass($"mud-transform-disabled", IsExpanded && Disabled)
                .Build();

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
        public bool IsExpanded { get; set; }

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
        public string ExpandIcon { get; set; } = @Icons.Material.Filled.ArrowDropDown;

        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public EventCallback<bool> IsExpandedChanged { get; set; }

        private async Task OnIsExpandedToggleAsync()
        {
            IsExpanded = !IsExpanded;
            await _isExpandedState.SetValueAsync(IsExpanded);
        }
    }
}
