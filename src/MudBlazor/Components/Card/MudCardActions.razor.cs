using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a set of buttons displayed as part of a <see cref="MudCard"/>.
    /// </summary>
    /// <seealso cref="MudCard" />
    /// <seealso cref="MudCardContent" />
    /// <seealso cref="MudCardHeader" />
    /// <seealso cref="MudCardMedia" />
    public partial class MudCardActions : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-card-actions")
            .AddClass("mud-card-actions-padding", InnerPadding)
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Adds padding of <c>8px</c> to all sides of the actions.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Card.Appearance)]
        public bool InnerPadding { get; set; } = true;
    }
}
