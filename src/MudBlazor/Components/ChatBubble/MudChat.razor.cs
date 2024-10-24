// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudChat : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-chat")
            .AddClass($"mud-chat-{Position.ToDescriptionString()}")
            .AddClass(RootClass)
            .Build();

        /// <summary>
        /// Chat bubble position.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chat.Appearance)]
        public ChatBubblePosition Position { get; set; } = ChatBubblePosition.Start;

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
