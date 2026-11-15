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
        /// A caller-supplied title used to be erased by the trailing null literal.
        /// </summary>
        [Test]
        public void CardMedia_Should_KeepUserSuppliedTitle()
        {
            var comp = Context.Render<MudCardMedia>(parameters => parameters
                .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "title", "Cover art" } }));

            comp.Find("div").GetAttribute("title").Should().Be("Cover art");
        }

        /// <summary>
        /// AsGrid renders a MudGrid inside the padded card-content element.
        /// </summary>
        [Test]
        public void CardContent_AsGrid_Should_LayOutItemsInAGrid()
        {
            var comp = Context.Render<MudCardContent>(parameters => parameters
                .Add(p => p.AsGrid, true)
                .AddChildContent(childBuilder =>
                {
                    childBuilder.OpenComponent<MudItem>(0);
                    childBuilder.AddAttribute(1, nameof(MudItem.xs), 6);
                    childBuilder.AddAttribute(2, nameof(MudItem.ChildContent), (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Left")));
                    childBuilder.CloseComponent();

                    childBuilder.OpenComponent<MudItem>(3);
                    childBuilder.AddAttribute(4, nameof(MudItem.xs), 6);
                    childBuilder.AddAttribute(5, nameof(MudItem.ChildContent), (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Right")));
                    childBuilder.CloseComponent();
                }));

            var content = comp.Find(".mud-card-content");
            content.ClassList.Should().NotContain("mud-grid");
            content.QuerySelector(".mud-grid").Should().NotBeNull();
            comp.FindAll(".mud-grid-item").Should().HaveCount(2);
        }

        /// <summary>
        /// Without AsGrid, the card content renders its children directly and no grid is added.
        /// </summary>
        [Test]
        public void CardContent_WithoutAsGrid_Should_Not_RenderAGrid()
        {
            var comp = Context.Render<MudCardContent>(parameters => parameters
                .AddChildContent("Card content"));

            comp.Find(".mud-card-content").ClassList.Should().NotContain("mud-grid");
            comp.FindAll(".mud-grid").Should().BeEmpty();
        }
    }
}
