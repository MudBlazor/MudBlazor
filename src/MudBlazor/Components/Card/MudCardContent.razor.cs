using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// The primary content area of a <see cref="MudCard"/>, holding its text and body elements.
    /// </summary>
    /// <seealso cref="MudCard" />
    /// <seealso cref="MudCardActions" />
    /// <seealso cref="MudCardHeader" />
    /// <seealso cref="MudCardMedia" />
    public partial class MudCardContent : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-card-content")
            .AddClass("mud-card-content-padding", ParentCard?.ContentPadding ?? true)
            .AddClass(Class)
            .Build();

        [CascadingParameter]
        private MudCard? ParentCard { get; set; }

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
