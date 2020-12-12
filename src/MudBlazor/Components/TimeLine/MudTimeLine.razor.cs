using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTimeLine : MudComponentBase
    {
        protected string Classnames =>
            new CssBuilder("mud-timeline")
            .AddClass($"mud-timeline-rounded", Rounded)
            .AddClass($"mud-border-right", Border)
            .AddClass($"mud-paper-outlined", Outlined)
            .AddClass($"mud-elevation-{Elevation.ToString()}", Elevation != 0)
            .AddClass(Class)
            .Build();

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
        /// Child content of component.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 0;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }




    }
}
