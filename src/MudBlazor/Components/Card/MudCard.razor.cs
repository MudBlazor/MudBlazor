using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudCard : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-card")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Appearance)]
        public int Elevation { set; get; } = 1;

        /// <summary>
        /// If true, border-radius is set to 0.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// If true, card will be outlined.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Appearance)]
        public bool Outlined { get; set; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
