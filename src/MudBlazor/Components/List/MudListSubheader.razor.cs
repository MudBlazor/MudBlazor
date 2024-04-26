using Microsoft.AspNetCore.Components;

using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudListSubheader : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-list-subheader")
                .AddClass("mud-list-subheader-gutters", Gutters)
                .AddClass("mud-list-subheader-inset", Inset)
                .AddClass(Class)
                .Build();

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// If true, left and right padding is added. Default is true
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Gutters { get; set; } = true;

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Inset { get; set; }
    }
}
