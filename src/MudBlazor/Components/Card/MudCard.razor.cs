using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Cards contain actions, text, or media like images or graphics. Keeping a card to a single subject keeps the design clean.
    /// </summary>
    /// <seealso cref="MudCardActions" />
    /// <seealso cref="MudCardContent" />
    /// <seealso cref="MudCardHeader" />
    /// <seealso cref="MudCardMedia" />
    public partial class MudCard : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-card")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The size of the drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Card.Appearance)]
        public int Elevation { set; get; } = 1;

        /// <summary>
        /// Disables rounded corners.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Card.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// Displays an outline.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  This property is useful to differentiate cards which are the same color or use images.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Card.Appearance)]
        public bool Outlined { get; set; }

        /// <summary>
        /// Adds visual padding to the content (<see cref="MudCardHeader"/>, <see cref="MudCardContent"/> or <see cref="MudCardActions"/>).
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Card.Appearance)]
        public bool ContentPadding { get; set; } = true;

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
