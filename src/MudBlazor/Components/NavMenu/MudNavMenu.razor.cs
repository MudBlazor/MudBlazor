using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudNavMenu : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-navmenu")
                .AddClass($"mud-navmenu-{Color.ToDescriptionString()}")
                .AddClass($"mud-navmenu-margin-{Margin.ToDescriptionString()}")
                .AddClass("mud-navmenu-dense", Dense)
                .AddClass("mud-navmenu-rounded", Rounded)
                .AddClass($"mud-navmenu-bordered mud-border-{Color.ToDescriptionString()}", Bordered)
                .AddClass(Class)
                .Build();

        [CascadingParameter]
        private NavigationContext? NavigationContext { get; set; }

        /// <summary>
        /// The color of the active NavLink.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// If true, adds a border of the active NavLink, does nothing if variant outlined is used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public bool Bordered { get; set; }

        /// <summary>
        /// If true, default theme border-radius will be used on all navlinks.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public bool Rounded { get; set; }

        /// <summary>
        ///  Adjust the vertical spacing between navlinks.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// If true, compact vertical padding will be applied to all navmenu items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public bool Dense { get; set; }

        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
