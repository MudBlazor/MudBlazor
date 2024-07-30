using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the content displayed within a <see cref="MudChat"/>.
    /// </summary>
    public partial class MudChatBubble : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-chat-bubble")
            .AddClass("mud-chat-bubble-default", Color == Color.Default)
            .AddClass($"mud-theme-{Color.ToDescriptionString()}", Color != Color.Default)
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
