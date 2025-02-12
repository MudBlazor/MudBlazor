using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the footer of a <see cref="MudChat"/>.
    /// </summary>
    public partial class MudChatFooter : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-chat-footer")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The time to display within this header.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public string? Text { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
