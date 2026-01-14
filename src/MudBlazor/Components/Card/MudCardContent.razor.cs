using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the primary content displayed within a <see cref="MudCard"/>.
    /// </summary>
    /// <seealso cref="MudCard" />
    /// <seealso cref="MudCardActions" />
    /// <seealso cref="MudCardHeader" />
    /// <seealso cref="MudCardMedia" />
    public partial class MudCardContent : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-card-content")
            .AddClass("mud-card-content-padding", InnerPadding)
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Adds padding of <c>16px</c> around all sides of the card content.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Card.Appearance)]
        public bool InnerPadding { get; set; } = true;
    }
}
