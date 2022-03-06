using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCardMedia : MudComponentBase
    {
        protected string StyleString =>
            StyleBuilder.Default($"background-image:url(\"{Image}\");height: {Height}px;")
                .AddStyle(this.Style)
                .Build();

        protected string Classname =>
            new CssBuilder("mud-card-media")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// Title of the image used for accessibility.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public string Title { get; set; }
        
        /// <summary>
        /// Specifies the path to the image.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public string Image { get; set; }
        
        /// <summary>
        /// Specifies the height of the image in px.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public int Height { get; set; } = 300;
    }
}
