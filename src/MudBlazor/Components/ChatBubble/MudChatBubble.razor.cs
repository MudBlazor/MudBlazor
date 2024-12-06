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
        private Color GetBubbleColor => Color != null ? Color.Value : ParentColor;
        private Variant GetBubbleVariant => Variant != null ? Variant.Value : ParentVariant;

        protected string Classname => new CssBuilder("mud-chat-bubble")
            .AddClass($"mud-chat-{GetBubbleVariant.ToDescriptionString()}-{GetBubbleColor.ToDescriptionString()}")
            .AddClass(Class)
            .Build();

        [CascadingParameter(Name = "MudChatBubbleVariant")]
        public Variant ParentVariant { get; private set; }

        [CascadingParameter(Name = "MudChatBubbleColor")]
        public Color ParentColor { get; private set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public Color? Color { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public Variant? Variant { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
