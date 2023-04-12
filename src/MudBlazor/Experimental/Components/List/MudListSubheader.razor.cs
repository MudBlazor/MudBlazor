using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExperimental
{
    public partial class MudListSubheader<T> : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-list-subheader")
            .AddClass("mud-list-subheader-gutters", !DisableGutters)
            .AddClass("mud-list-subheader-inset", Inset)
            .AddClass("mud-list-subheader-secondary-background", SecondaryBackground)
            .AddClass("mud-list-subheader-sticky", Sticky)
            .AddClass("mud-list-subheader-sticky-dense", Sticky && (MudList != null && MudList.DisablePadding))
            .AddClass(Class)
        .Build();

        [CascadingParameter] protected MudList<T> MudList { get; set; }

        /// <summary>
        /// The child render fragment.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Disables the left and right spaces.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableGutters { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Inset { get; set; }

        /// <summary>
        /// If true, subheader behaves sticky and remains on top until other subheader comes to top.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Sticky { get; set; }

        /// <summary>
        /// If true, subheader has darken background.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool SecondaryBackground { get; set; }
    }
}
