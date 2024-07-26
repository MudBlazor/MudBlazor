using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the avatar of a <see cref="MudChat"/>.
    /// </summary>
    public partial class MudChatAvatar : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-chat-image")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
