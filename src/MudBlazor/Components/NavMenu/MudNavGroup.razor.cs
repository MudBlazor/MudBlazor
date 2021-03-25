using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNavGroup : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-nav-group")
          .AddClass(Class)
        .Build();

        protected string ButtonClassname =>
        new CssBuilder("mud-nav-link")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass("mud-expanded", Expanded)
        .Build();

        protected string IconClassname =>
        new CssBuilder("mud-nav-link-icon")
          .AddClass($"mud-nav-link-icon-default", IconColor == Color.Default)
          .Build();

        [Parameter] public string Title { get; set; }

        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors, default value uses the themes drawer icon color.
        /// </summary>
        [Parameter] public Color IconColor { get; set; } = Color.Default;

        /// <summary>
        /// If true, the button will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, expands the nav group, otherwise collapse it. Setting this prop enables control over the panel.
        /// </summary>
        [Parameter] public bool Expanded { get; set; }

        /// <summary>
        /// If true, hides expand-icon at the end of the NavGroup.
        /// </summary>
        [Parameter] public bool HideExpandIcon { get; set; }

        /// <summary>
        /// Explicitly sets the height for the Collapse element to override the css default.
        /// </summary>
        [Parameter] public int? MaxHeight { get; set; }

        /// <summary>
        /// If set, overrides the default expand icon.
        /// </summary>
        [Parameter] public string ExpandIcon { get; set; } = @Icons.Filled.ArrowDropDown;
        [Parameter] public RenderFragment ChildContent { get; set; }

        protected void ExpandedToggle()
        {
            Expanded = !Expanded;
        }
    }
}
