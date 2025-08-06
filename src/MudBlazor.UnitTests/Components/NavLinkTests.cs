using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.NavLink;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class NavLinkTests : BunitTest
    {
        /// <summary>
        /// When Target is set to Blank, it should equal noopener noreferrer, otherwise it should be empty.
        /// </summary>
        [TestCase(LinkTarget.Self, "")]
        [TestCase(LinkTarget.Blank, "noopener noreferrer")]
        [TestCase(LinkTarget.Parent, "")]
        [TestCase(LinkTarget.Top, "")]
        [TestCase(LinkTarget.Iframe, "", "myFrameName")]
        [TestCase(LinkTarget.Iframe, "")]
        public void NavLink_CheckRelAttribute(LinkTarget target, string expectedRel, string iframe = "")
        {
            var comp = Context.RenderComponent<MudNavLink>(Parameter(nameof(MudNavLink.Target), target), Parameter(nameof(MudNavLink.FrameTarget), iframe));
            // print the generated html
            // select elements needed for the test
            comp.Find("a").GetAttribute("rel").Should().Be(expectedRel);
        }

        [Test]
        public void NavLink_CheckOnClickEvent()
        {
            var clicked = false;
            var comp = Context.RenderComponent<MudNavLink>(EventCallback(nameof(MudNavLink.OnClick), (MouseEventArgs args) => { clicked = true; }));
            // print the generated html
            comp.FindAll("a").Should().BeEmpty();
            comp.Find(".mud-nav-link").Click();
            clicked.Should().BeTrue();
        }

        [Test]
        public void NavLink_Active()
        {
            const string activeClass = "Custom__nav_active_css";
            var comp = Context.RenderComponent<MudNavLink>(Parameter(nameof(MudNavLink.ActiveClass), activeClass));
            comp.Find(".mud-nav-link").Click();
            comp.Markup.Should().Contain(activeClass);
        }

        [Test]
        public void NavLink_Enabled_CheckNavigation()
        {
            var comp = Context.RenderComponent<NavLinkDisabledTest>(Parameter(nameof(NavLinkDisabledTest.Disabled), false));
            comp.Find("a").Click();
            comp.Instance.IsNavigated.Should().BeTrue();
        }

        [Test]
        public void NavLink_Disabled_CheckNoNavigation()
        {
            var comp = Context.RenderComponent<NavLinkDisabledTest>(Parameter(nameof(NavLinkDisabledTest.Disabled), true));
            comp.Find("a").Click();
            comp.Instance.IsNavigated.Should().BeFalse();
        }

        [Test]
        public async Task NavLinkOnClickErrorContentCaughtException()
        {
            var comp = Context.RenderComponent<NavLinkErrorContenCaughtException>();
            IElement AlertText() => MudAlert().Find("div.mud-alert-message");
            IRenderedComponent<MudAlert> MudAlert() => comp.FindComponent<MudAlert>();
            IRefreshableElementCollection<IElement> Links() => comp.FindAll(".mud-nav-link");
            IElement MudLink() => Links()[0];

            await MudLink().ClickAsync(new MouseEventArgs());

            AlertText().InnerHtml.Should().Be("Oh my! We caught an error and handled it!");
        }
    }
}
