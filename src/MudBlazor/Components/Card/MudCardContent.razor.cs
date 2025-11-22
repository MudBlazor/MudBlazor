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
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Card.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
