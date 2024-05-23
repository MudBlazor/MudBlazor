using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a set of buttons displayed as part of a <see cref="MudCard"/>.
    /// </summary>
    public partial class MudCardActions : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-card-actions")
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
