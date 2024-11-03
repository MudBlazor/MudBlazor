using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class OverlayTests : BunitTest
    {
        /// <summary>
        /// Should not render by default
        /// </summary>
        [Test]
        public void MarkupShouldBeEmptyTest()
        {
            var comp = Context.RenderComponent<MudOverlay>();
            comp.Markup.Should().BeEmpty();
        }

        /// <summary>
        /// Should render when Visible is "true"
        /// </summary>
        [Test]
        public void MarkupShouldNotBeEmptyTest()
        {
            var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
                .Add(p => p.Visible, true)
            );

            comp.Markup.Should().NotBeEmpty();
        }

        /// <summary>
        /// Should close on click when AutoClose is "true"
        /// </summary>
        [Test]
        public async Task OverlayShouldCloseOnClickTest()
        {
            var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
                .Add(p => p.Visible, true)
                .Add(p => p.AutoClose, true)
            );

            await comp.Find("div.mud-overlay").ClickAsync(new MouseEventArgs());
            comp.Markup.Should().BeEmpty();
        }

        /// <summary>
        /// Should not close on click when AutoClose is default ("false")
        /// </summary>
        [Test]
        public async Task OverlayShouldNotCloseOnClickTest()
        {
            var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
                .Add(p => p.Visible, true)
            );

            await comp.Find("div.mud-overlay").ClickAsync(new MouseEventArgs());
            comp.Markup.Should().NotBeEmpty();

            comp.SetParam(nameof(MudOverlay.AutoClose), false);
            await comp.Find("div.mud-overlay").ClickAsync(new MouseEventArgs());
            comp.Markup.Should().NotBeEmpty();
        }

        /// <summary>
        /// Should invoke OnClick event
        /// </summary>
        [Test]
        public async Task LifetimeTest()
        {
            var counter = 0;
            void OnClickHandler(MouseEventArgs args) => counter++;

            var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
                .Add(p => p.Visible, true)
                .Add(p => p.OnClick, OnClickHandler)
            );

            await comp.Find("div.mud-overlay").ClickAsync(new MouseEventArgs());
            comp.Markup.Trim().Should().NotBeEmpty();
            counter.Should().Be(1);
        }

        [Test]
        public async Task VisibleBindingWithAutoCloseTest()
        {
            var comp = Context.RenderComponent<OverlayVisibleBindingWithAutoCloseTest>();
            IElement Button() => comp.Find("#showBtn");

            comp.Instance.Visible.Should().BeFalse();
            await Button().ClickAsync(new MouseEventArgs());
            comp.Instance.Visible.Should().BeTrue();
            await comp.Find("div.mud-overlay").ClickAsync(new MouseEventArgs());
            comp.Instance.Visible.Should().BeFalse();
        }
    }
}
