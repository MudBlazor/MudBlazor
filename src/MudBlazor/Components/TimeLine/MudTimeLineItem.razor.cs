using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTimeLineItem : MudComponentBase
    {
        protected string Classnames =>
            new CssBuilder("mud-timeline")
            .AddClass($"mud-elevation-{Elevation.ToString()}", Elevation != 0)
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 0;

        /// <summary>
        /// TimeLineItem's Alignment on main TimeLine component
        /// </summary>
        [Parameter] public Align Align { get; set; } = Align.Left;

        /// <summary>
        /// Icon for the TimeLineItem
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// Color for the TimeLineItem symbol
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}
