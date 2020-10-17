using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;



namespace MudBlazor
{
    public partial class MudSimpleTable : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-table mud-simple-table")
           .AddClass($"mud-table-dense", Dense)
           .AddClass($"mud-table-hover", Hover)
           .AddClass($"mud-table-outlined", Outlined)
           .AddClass($"mud-table-square", Square)
           .AddClass($"mud-elevation-{Elevation.ToString()}", !Outlined)
          .AddClass(Class)
        .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 1;

        /// <summary>
        /// If true, the table row will shade on hover.
        /// </summary>
        [Parameter] public bool Hover { get; set; }

        /// <summary>
        /// If true, border-radius is set to 0.
        /// </summary>
        [Parameter] public bool Square { get; set; }

        /// <summary>
        /// If true, compact padding will be used.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// If true, card will be outlined.
        /// </summary>
        [Parameter] public bool Outlined { get; set; }
        // todo: implement
        [Parameter] public bool StickyHeader { get; set; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}
