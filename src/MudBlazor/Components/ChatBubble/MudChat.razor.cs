// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudChat : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-chat")
            .AddClass($"mud-chat-{ChatPosition.ToDescriptionString()}")
            .AddClass($"mud-chat-arrow-{ArrowPosition.ToDescriptionString()}")
            .AddClass($"mud-square", Square)
            .AddClass($"mud-chat-rtl", RightToLeft)
            .AddClass($"mud-dense", Dense)
            .AddClass($"mud-elevation-{Elevation}")
            .AddClass(Class)
            .Build();

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; private set; }

        /// <summary>
        /// Child chat bubbles default color, can be overridden by bubble.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Gets or sets the display variant to use.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text" />. The variant changes the appearance of the chat bubbles, such as <c>Text</c>, <c>Outlined</c>, or <c>Filled</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Chat bubble position.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public ChatBubblePosition ChatPosition { get; set; } = ChatBubblePosition.Start;

        /// <summary>
        /// The Chat Bubble Arrow Position.
        /// </summary>
        /// <remarks>Defaults to Top</remarks>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public ChatArrowPosition ArrowPosition { get; set; } = ChatArrowPosition.Top;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the size of the drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public int Elevation { set; get; } = 0;

        /// <summary>
        /// Gets or sets whether rounded corners are disabled.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// Gets or sets whether compact padding will be used.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public bool Dense { get; set; }
    }
}
