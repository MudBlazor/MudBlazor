using Microsoft.AspNetCore.Components;

using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudListSubheader : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-list-subheader")
           .AddClass("mud-list-subheader-gutters", !DisableGutters)
           .AddClass("mud-list-subheader-inset", Inset)
          .AddClass(Class)
        .Build();

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableGutters { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Inset { get; set; }
    }
}
