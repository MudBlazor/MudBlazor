// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.UnitTests.TestComponents.Card;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class CardTests : BunitTest
    {
        [Test]
        public async Task CardChildContent()
        {
            //Card header with child content should be render successfully
            var comp = Context.Render<CardChildContentTest>();
            var button = comp.FindComponent<MudButton>();
            var numeric = comp.FindComponent<MudNumericField<int>>();
            await comp.WaitForAssertionAsync(() => numeric.Instance.Value.Should().Be(0));
            await comp.InvokeAsync(() => button.Instance.OnClick.InvokeAsync());
            await comp.WaitForAssertionAsync(() => numeric.Instance.Value.Should().Be(1));
        }

        /// <summary>
        /// Card parts inside another component render once per card render, not a second time through the card's cascade.
        /// </summary>
        [Test]
        public async Task CardContentInsideAnotherComponent_RendersOncePerCardRender()
        {
            var comp = Context.Render<MudCard>(parameters => parameters
                .Add(p => p.ChildContent, (RenderFragment)(builder =>
                {
                    builder.OpenComponent<MudForm>(0);
                    builder.AddAttribute(1, nameof(MudForm.ChildContent), (RenderFragment)(form =>
                    {
                        form.OpenComponent<MudCardContent>(0);
                        form.AddAttribute(1, nameof(MudCardContent.ChildContent), (RenderFragment)(content =>
                        {
                            content.OpenComponent<MudText>(0);
                            content.AddAttribute(1, nameof(MudText.ChildContent), (RenderFragment)(text => text.AddContent(0, "Body")));
                            content.CloseComponent();
                        }));
                        form.CloseComponent();
                    }));
                    builder.CloseComponent();
                })));
            var text = comp.FindComponent<MudText>();
            var rendersBefore = text.RenderCount;

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Elevation, 4));

            (text.RenderCount - rendersBefore).Should().Be(1);
        }

        /// <summary>
        /// Turning ContentPadding off after the first render still removes the padding from every part, including a header with no content.
        /// </summary>
        [Test]
        public async Task CardParts_FollowContentPaddingChanges()
        {
            var comp = Context.Render<MudCard>(parameters => parameters
                .Add(p => p.ChildContent, (RenderFragment)(builder =>
                {
                    builder.OpenComponent<MudCardHeader>(0);
                    builder.CloseComponent();
                    builder.OpenComponent<MudCardContent>(1);
                    builder.CloseComponent();
                    builder.OpenComponent<MudCardActions>(2);
                    builder.CloseComponent();
                })));
            comp.FindAll(".mud-card-header-padding, .mud-card-content-padding, .mud-card-actions-padding").Count.Should().Be(3);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.ContentPadding, false));

            comp.FindAll(".mud-card-header-padding, .mud-card-content-padding, .mud-card-actions-padding").Should().BeEmpty();

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.ContentPadding, true));

            comp.FindAll(".mud-card-header-padding, .mud-card-content-padding, .mud-card-actions-padding").Count.Should().Be(3);
        }

        /// <summary>
        /// A part outside any card, inside a caller's own cascade of a card, still follows that card's ContentPadding.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void CardContentInsideCustomCardCascade_UsesThatCardsContentPadding(bool contentPadding)
        {
            var card = Context.Render<MudCard>(parameters => parameters.Add(p => p.ContentPadding, contentPadding)).Instance;

            var comp = Context.Render<CascadingValue<MudCard>>(parameters => parameters
                .Add(p => p.Value, card)
                .Add(p => p.ChildContent, (RenderFragment)(builder =>
                {
                    builder.OpenComponent<MudCardContent>(0);
                    builder.CloseComponent();
                })));

            comp.FindAll(".mud-card-content-padding").Count.Should().Be(contentPadding ? 1 : 0);
        }

        /// <summary>
        /// A caller-supplied title used to be erased by the trailing null literal.
        /// </summary>
        [Test]
        public void CardMedia_Should_KeepUserSuppliedTitle()
        {
            var comp = Context.Render<MudCardMedia>(parameters => parameters
                .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "title", "Cover art" } }));

            comp.Find("div").GetAttribute("title").Should().Be("Cover art");
        }
    }
}
