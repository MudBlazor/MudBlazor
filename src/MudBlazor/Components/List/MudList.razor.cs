using Microsoft.AspNetCore.Components;

using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudList : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-list")
           .AddClass("mud-list-padding", !DisablePadding)
          .AddClass(Class)
        .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Determens if the List will have clickable items.
        /// </summary>
        [Parameter] public bool Clickable { get; set; }

        /// <summary>
        /// If true, vertical padding will be removed from the list.
        /// </summary>
        [Parameter] public bool DisablePadding { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all list items.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed on all list items.
        /// </summary>
        [Parameter] public bool DisableGutters { get; set; }
    }
}
