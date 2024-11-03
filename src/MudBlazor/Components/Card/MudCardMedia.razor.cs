using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents an image displayed as part of a <see cref="MudCard"/>.
    /// </summary>
    public partial class MudCardMedia : MudComponentBase
    {
        protected string StyleString => StyleBuilder.Default($"background-image:url(\"{Image}\");height: {Height}px;")
            .AddStyle(Style)
            .Build();

        protected string Classname => new CssBuilder("mud-card-media")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Text for the <c>title</c> attribute which provides a basic tooltip.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public string? Title { get; set; }

        /// <summary>
        /// The URL of the image to display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public string? Image { get; set; }

        /// <summary>
        /// The height, in pixels, of the <see cref="Image"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>300</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public int Height { get; set; } = 300;
    }
}
