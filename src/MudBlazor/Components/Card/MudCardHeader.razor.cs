using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the top portion of a <see cref="MudCard"/>.
    /// </summary>
    public partial class MudCardHeader : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-card-header")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The avatar to display within this header.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? CardHeaderAvatar { get; set; }

        /// <summary>
        /// The main content of this header.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? CardHeaderContent { get; set; }

        /// <summary>
        /// The actions displayed within this header.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? CardHeaderActions { get; set; }

        /// <summary>
        /// The custom content within this header.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
