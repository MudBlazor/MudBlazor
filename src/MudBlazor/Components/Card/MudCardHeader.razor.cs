using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudCardHeader : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-card-header")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// If used renders child content of the CardHeaderAvatar.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? CardHeaderAvatar { get; set; }

        /// <summary>
        /// If used renders child content of the CardHeaderContent.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? CardHeaderContent { get; set; }

        /// <summary>
        /// If used renders child content of the CardHeaderActions.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? CardHeaderActions { get; set; }

        /// <summary>
        /// Optional child content
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
