using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a block of content which can include a header, image, content, and actions.
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
        public int Elevation { set; get; } = MudGlobal.CardDefaults.Elevation;

        /// <summary>
        /// Disables rounded corners.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Card.Appearance)]
        public bool Square { get; set; } = MudGlobal.CardDefaults.Square;

        /// <summary>
        /// Displays an outline.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  This property is useful to differentiate cards which are the same color or use images.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Card.Appearance)]
        public bool Outlined { get; set; } = MudGlobal.CardDefaults.Outlined;

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
