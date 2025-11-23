using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the header of a <see cref="MudChat"/>.
    /// </summary>
    /// <remarks>
    /// This component is deprecated and will be removed in v10. Please use MudX instead: https://github.com/MudXtra/MudX/
    /// </remarks>
    [Obsolete("MudChatHeader is deprecated and will be removed in v10. Please use MudX instead: https://github.com/MudXtra/MudX/")]
    public partial class MudChatHeader : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-chat-header")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The name to display within this header.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public string? Name { get; set; }

        /// <summary>
        /// The time to display within this header.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public string? Time { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
