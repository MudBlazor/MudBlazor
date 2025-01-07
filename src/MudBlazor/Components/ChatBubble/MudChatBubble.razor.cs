using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
            .AddClass($"mud-chat-arrow-{ParentArrowPosition.ToDescriptionString()}")
            .AddClass("mud-chat-bubble-clickable", OnClick.HasDelegate || OnContextClick.HasDelegate)
            .AddClass("mud-ripple", OnClick.HasDelegate || OnContextClick.HasDelegate)
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The containing div Element Reference
        /// </summary>
        public ElementReference ElementReference { get; private set; }

        [CascadingParameter(Name = "MudChatBubbleVariant")]
        public Variant ParentVariant { get; private set; }

        [CascadingParameter(Name = "MudChatBubbleColor")]
        public Color ParentColor { get; private set; }

        [CascadingParameter(Name = "MudChatArrowPosition")]
        public ChatArrowPosition ParentArrowPosition { get; private set; } = ChatArrowPosition.None;

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

        /// <summary>
        /// Occurs when the chat bubble has been clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Behavior)]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// Occurs when the chat bubble has been right-clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Behavior)]
        public EventCallback<MouseEventArgs> OnContextClick { get; set; }

        /// <summary>
        /// Occurs when the chat bubble has been clicked.
        /// </summary>
        /// <param name="mouseEventArgs">
        /// A <see cref="MouseEventArgs"/> object.  The mouse coordinates related to this click.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> object.
        /// </returns>
        internal async Task OnClickHandler(MouseEventArgs mouseEventArgs)
        {
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(mouseEventArgs);
            }
        }

        /// <summary>
        /// Occurs when the chat bubble has been right-clicked.
        /// </summary>
        /// <param name="mouseEventArgs">
        /// A <see cref="MouseEventArgs"/> object.  The mouse coordinates related to this click.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> object.
        /// </returns>
        internal async Task OnContextHandler(MouseEventArgs mouseEventArgs)
        {
            if (OnContextClick.HasDelegate)
            {
                await OnContextClick.InvokeAsync(mouseEventArgs);
            }
        }
    }
}
