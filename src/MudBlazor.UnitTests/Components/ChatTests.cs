// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ChatTests : BunitTest
    {
        [Test]
        public void MudChat_DefaultValues()
        {
            var comp = Context.RenderComponent<MudChat>();
            comp.Instance.Color.Should().Be(Color.Default);
            comp.Instance.ChatPosition.Should().Be(ChatBubblePosition.Start);
            comp.Instance.Elevation.Should().Be(0);
            comp.Instance.Square.Should().Be(false);
            comp.Instance.Dense.Should().Be(false);
            comp.Instance.Variant.Should().Be(Variant.Text);
        }

        [Test]
        public void MudChat_CssClasses()
        {
            var comp = Context.RenderComponent<MudChat>(parameters => parameters
                .Add(p => p.ChatPosition, ChatBubblePosition.End)
                .Add(p => p.ArrowPosition, ChatArrowPosition.Middle)
                .Add(p => p.Square, true)
                .Add(p => p.Dense, true)
                .Add(p => p.Elevation, 2)
                .Add(p => p.Class, "custom-class"));

            comp.Markup.Should().Contain("mud-chat-end");
            comp.Markup.Should().Contain("mud-square");
            comp.Markup.Should().Contain("mud-dense");
            comp.Markup.Should().Contain("mud-elevation-2");
            comp.Markup.Should().Contain("custom-class");
            comp.Markup.Should().Contain("mud-chat-arrow-middle");
        }

        [Test]
        public void MudChatBubble_CssClasses()
        {
            var comp = Context.RenderComponent<MudChatBubble>(parameters => parameters
                 .Add(p => p.Color, Color.Success)
                 .Add(p => p.Variant, Variant.Outlined));

            comp.Markup.Should().Contain("mud-chat-bubble");
            comp.Markup.Should().Contain("mud-chat-outlined-success");
            comp.Markup.Should().Contain("mud-chat-arrow-none");
        }

        [Test]
        public void MudChatBubble_InheritsParentValues()
        {
            var comp = Context.RenderComponent<MudChat>(parameters => parameters
                .Add(p => p.Color, Color.Primary)
                .Add(p => p.Variant, Variant.Filled)
                .Add(p => p.ArrowPosition, ChatArrowPosition.Middle)
                .Add(p => p.ChildContent, builder =>
                {
                    builder.OpenComponent<MudChatBubble>(0);
                    builder.CloseComponent();
                }));

            var bubble = comp.FindComponent<MudChatBubble>();
            bubble.Instance.ParentColor.Should().Be(Color.Primary);
            bubble.Instance.ParentVariant.Should().Be(Variant.Filled);
            bubble.Instance.ParentArrowPosition.Should().Be(ChatArrowPosition.Middle);
            bubble.Markup.Should().Contain("mud-chat-filled-primary");
        }

        [Test]
        public void MudChatBubble_HasElementReference()
        {
            var comp = Context.RenderComponent<MudChatBubble>();
            var elementRef = comp.Instance.ElementReference;
            elementRef.Should().NotBeNull();
        }

        [Test]
        public void MudChatBubble_OverridesParentValues()
        {
            var comp = Context.RenderComponent<MudChat>(parameters => parameters
                .Add(p => p.Color, Color.Primary)
                .Add(p => p.Variant, Variant.Filled)
                .Add(p => p.ChildContent, builder =>
                {
                    builder.OpenComponent<MudChatBubble>(0);
                    builder.AddAttribute(1, "Color", Color.Secondary);
                    builder.AddAttribute(2, "Variant", Variant.Outlined);
                    builder.CloseComponent();
                }));

            var bubble = comp.FindComponent<MudChatBubble>();
            bubble.Markup.Should().Contain("mud-chat-outlined-secondary");
        }

        [Test]
        public async Task MudChatBubble_ClickEvents()
        {
            var clicked = false;
            var rightClicked = false;

            var comp = Context.RenderComponent<MudChatBubble>(parameters => parameters
                .Add(p => p.OnClick, (MouseEventArgs e) => { clicked = true; })
                .Add(p => p.OnContextClick, (MouseEventArgs e) => { rightClicked = true; }));

            await comp.InvokeAsync(() => comp.Instance.OnClickHandler(new MouseEventArgs()));
            clicked.Should().BeTrue();

            await comp.InvokeAsync(() => comp.Instance.OnContextHandler(new MouseEventArgs()));
            rightClicked.Should().BeTrue();
        }

        [Test]
        public void MudChat_RightToLeft()
        {
            var comp = Context.RenderComponent<MudChat>(parameters => parameters
                .Add(p => p.RightToLeft, true));

            comp.Markup.Should().Contain("mud-chat-rtl");
        }

        [Test]
        public void MudChat_CustomStyles()
        {
            var comp = Context.RenderComponent<MudChat>(parameters => parameters
                .Add(p => p.Style, "background-color: red;")
                .Add(p => p.Class, "custom-class"));

            comp.Markup.Should().Contain("style=\"background-color: red;\"");
            comp.Markup.Should().Contain("custom-class");
        }

        [Test]
        public void MudChatHeader_Parameters()
        {
            var comp = Context.RenderComponent<MudChatHeader>(parameters => parameters
                .Add(p => p.Name, "John Doe")
                .Add(p => p.Time, "12:00 PM")
                .Add(p => p.Class, "custom-header-class"));

            comp.Markup.Should().Contain("mud-chat-header");
            comp.Markup.Should().Contain("John Doe");
            comp.Markup.Should().Contain("12:00 PM");
            comp.Markup.Should().Contain("custom-header-class");
        }

        [Test]
        public void MudChatHeader_ChildContent()
        {
            var comp = Context.RenderComponent<MudChatHeader>(parameters => parameters
                .Add(p => p.ChildContent, builder =>
                {
                    builder.AddContent(0, "Custom Header Content");
                }));

            comp.Markup.Should().Contain("Custom Header Content");
        }

        [Test]
        public void MudChatFooter_Parameters()
        {
            var comp = Context.RenderComponent<MudChatFooter>(parameters => parameters
                .Add(p => p.Text, "Typing...")
                .Add(p => p.Class, "custom-footer-class"));

            comp.Markup.Should().Contain("mud-chat-footer");
            comp.Markup.Should().Contain("Typing...");
            comp.Markup.Should().Contain("custom-footer-class");
        }

        [Test]
        public void MudChatFooter_ChildContent()
        {
            var comp = Context.RenderComponent<MudChatFooter>(parameters => parameters
                .Add(p => p.ChildContent, builder =>
                {
                    builder.AddContent(0, "Custom Footer Content");
                }));

            comp.Markup.Should().Contain("Custom Footer Content");
        }
    }
}
