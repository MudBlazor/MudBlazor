using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// The action bar of a <see cref="MudCard"/>, typically holding buttons that trigger the card's related actions.
    /// </summary>
    /// <seealso cref="MudCard" />
    /// <seealso cref="MudCardContent" />
    /// <seealso cref="MudCardHeader" />
    /// <seealso cref="MudCardMedia" />
    public partial class MudCardActions : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-card-actions")
            .AddClass("mud-card-actions-padding", ParentCard?.ContentPadding ?? true)
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
