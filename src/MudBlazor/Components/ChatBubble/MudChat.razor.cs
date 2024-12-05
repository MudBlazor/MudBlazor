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
            .AddClass($"justify-sm-{ConvertHorizontalAlignment(ContentAlignment).ToDescriptionString()}")
            .AddClass(RootClass)
            .Build();

        /// <summary>
        /// Gets the horizontal alignment to use based on the current right-to-left setting.
        /// </summary>
        /// <param name="contentAlignment">
        /// A <see cref="HorizontalAlignment"/> value.  The alignment to adjust.
        /// </param>
        /// <returns>
        /// A <see cref="HorizontalAlignment"/> value.  The adjusted alignment.
        /// </returns>
        private HorizontalAlignment ConvertHorizontalAlignment(HorizontalAlignment contentAlignment)
        {
            return contentAlignment switch
            {
                HorizontalAlignment.Right => RightToLeft ? HorizontalAlignment.Start : HorizontalAlignment.End,
                HorizontalAlignment.Left => RightToLeft ? HorizontalAlignment.End : HorizontalAlignment.Start,
                _ => contentAlignment
            };
        }

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Gets or sets the position of the text to the start (Left in LTR and right in RTL).
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="HorizontalAlignment.Left"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public HorizontalAlignment ContentAlignment { get; set; } = HorizontalAlignment.Left;

        /// <summary>
        /// Chat bubble position.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public ChatBubblePosition ChatPosition { get; set; } = ChatBubblePosition.Left;

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
    }
}
