using Microsoft.AspNetCore.Components;

using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudListSubheader<T> : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-list-subheader")
            .AddClass("mud-list-subheader-gutters", !DisableGutters)
            .AddClass("mud-list-subheader-inset", Inset)
            .AddClass("mud-list-subheader-sticky", Sticky)
            .AddClass("mud-list-subheader-sticky-dense", Sticky && (MudList != null && MudList.DisablePadding))
            .AddClass(Class)
        .Build();

        [CascadingParameter] protected MudList<T> MudList { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableGutters { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Inset { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Sticky { get; set; }
    }
}
