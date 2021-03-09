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
           .AddClass($"mud-table-bordered", Bordered)
           .AddClass($"mud-table-outlined", Outlined)
           .AddClass($"mud-table-striped", Striped)
           .AddClass($"mud-table-square", Square)
           .AddClass($"mud-table-sticky-header", FixedHeader)
           .AddClass($"mud-elevation-{Elevation}", !Outlined)
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
        /// If true, table will be outlined.
        /// </summary>
        [Parameter] public bool Outlined { get; set; }

        /// <summary>
        /// If true, table's cells will have left/right borders.
        /// </summary>
        [Parameter] public bool Bordered { get; set; }

        /// <summary>
        /// If true, striped table rows will be used.
        /// </summary>
        [Parameter] public bool Striped { get; set; }

        /// <summary>
        /// When true, the header will stay in place when the table is scrolled. Note: set Height to make the table scrollable.
        /// </summary>
        [Parameter] public bool FixedHeader { get; set; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}
