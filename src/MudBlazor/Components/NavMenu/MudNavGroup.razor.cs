using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNavGroup : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-nav-link")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass("mud-expanded", Expanded)
          .AddClass(Class)
        .Build();

        [Parameter] public string Title { get; set; }

        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter] public string Icon { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool DisableRipple { get; set; }
        [Parameter] public bool Expanded { get; set; }

        /// <summary>
        /// If true, adds expand-icon at the end of the NavGroup. Set to true by default.
        /// </summary>
        [Parameter] public bool ExpandIcon { get; set; } = true;
        [Parameter] public RenderFragment ChildContent { get; set; }

        protected void ExpandedToggle()
        {
            Expanded = !Expanded;
        }
    }
}
