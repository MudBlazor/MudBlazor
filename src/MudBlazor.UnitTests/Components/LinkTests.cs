using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.Link;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class LinkTests : BunitTest
    {
        [Test]
        public void NavLink_CheckDisabled()
        {
            var comp = Context.RenderComponent<MudLink>(
                Parameter(nameof(MudLink.Href), "#"),
                Parameter(nameof(MudLink.Disabled), true));
            comp.Find("a").GetAttribute("href").Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Link should execute OnClick delegate when is not disabled
        /// </summary>
        [Test]
        public async Task LinkShouldExecuteOnClickDelegate()
        {
            var calls = 0;
            var onClickDelegate = EventCallback<MouseEventArgs>(nameof(MudLink.OnClick), e => calls++);

            var comp = Context.RenderComponent<MudLink>(onClickDelegate);
            await comp.Find("a").ClickAsync(new MouseEventArgs());

            calls.Should().Be(1);
        }

        /// <summary>
        /// Link should not execute OnClick delegate when is disabled
        /// </summary>
        [Test]
        public async Task LinkShouldNotExecuteOnClickDelegateWhenDisabled()
        {
            var calls = 0;
            var onClickDelegate = EventCallback<MouseEventArgs>(nameof(MudLink.OnClick), e => calls++);
            var isDisabled = Parameter(nameof(MudLink.Disabled), true);

            var comp = Context.RenderComponent<MudLink>(onClickDelegate, isDisabled);
            await comp.Find("a").ClickAsync(new MouseEventArgs());

            calls.Should().Be(0);
        }

        [Test]
        public async Task LinkOnClickErrorContentCaughtException()
        {
            var comp = Context.RenderComponent<LinkErrorContenCaughtException>();
            IElement AlertText() => MudAlert().Find("div.mud-alert-message");
            IRenderedComponent<MudAlert> MudAlert() => comp.FindComponent<MudAlert>();
            IRefreshableElementCollection<IElement> Links() => comp.FindAll("a.mud-link");
            IElement MudLink() => Links()[0];

            await MudLink().ClickAsync(new MouseEventArgs());

            AlertText().InnerHtml.Should().Be("Oh my! We caught an error and handled it!");
        }
    }
}
