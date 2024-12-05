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
            .AddClass($"mud-chat-{Variant.ToDescriptionString()}")
            .AddClass($"mud-chat-rtl", RightToLeft)
            .AddClass($"mud-dense", Dense)
            .AddClass($"mud-elevation-{Elevation}")
            .AddClass(RootClass)
            .Build();

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Chat bubble position.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public ChatBubblePosition ChatPosition { get; set; } = ChatBubblePosition.Start;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Styles applied directly to root component of the chat bubble
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public string? RootStyle { get; set; }

        /// <summary>
        /// Classes applied directly to root component of the chat bubble
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public string? RootClass { get; set; }

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
        /// Gets or sets whether compact padding will be used.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// Gets or sets the display variant to use.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Filled" />. The variant changes the appearance of the alert, such as <c>Text</c>, <c>Outlined</c>, or <c>Filled</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;
    }
}
