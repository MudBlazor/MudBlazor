using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.NavLink;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Components
{
    public class NavLinkTests : BunitTest
    {
        /// <summary>
        /// When Target is not empty, rel attribute should be equals to "noopener noreferrer" on the a element
        /// </summary>
        [Test]
        [Arguments(null, "")]
        [Arguments("", "")]
        [Arguments("_self", "noopener noreferrer")]
        [Arguments("_blank", "noopener noreferrer")]
        [Arguments("_parent", "noopener noreferrer")]
        [Arguments("_top", "noopener noreferrer")]
        [Arguments("myFrameName", "noopener noreferrer")]
        public void NavLink_CheckRelAttribute(string target, string expectedRel)
        {
            var comp = Context.Render<MudNavLink>(parameters => parameters.Add(x => x.Target, target));
            // print the generated html
            // select elements needed for the test
            comp.Find("a").GetAttribute("rel").Should().Be(expectedRel);
        }

        [Test]
        public async Task NavLink_CheckOnClickEvent()
        {
            var clicked = false;
            var comp = Context.Render<MudNavLink>(parameters => parameters.Add(x => x.OnClick, (MouseEventArgs args) => { clicked = true; }));
            // print the generated html
            comp.FindAll("a").Should().BeEmpty();
            await comp.Find(".mud-nav-link").ClickAsync();
            clicked.Should().BeTrue();
        }

        [Test]
        public async Task NavLink_Active()
        {
            const string activeClass = "Custom__nav_active_css";
            var comp = Context.Render<MudNavLink>(parameters => parameters.Add(x => x.ActiveClass, activeClass));
            await comp.Find(".mud-nav-link").ClickAsync();
            comp.Markup.Should().Contain(activeClass);
        }

        [Test]
        public async Task NavLink_Enabled_CheckNavigation()
        {
            var comp = Context.Render<NavLinkDisabledTest>(parameters => parameters.Add(x => x.Disabled, false));
            await comp.Find("a").ClickAsync();
            comp.Instance.IsNavigated.Should().BeTrue();
        }

        [Test]
        public async Task NavLink_Disabled_CheckNoNavigation()
        {
            var comp = Context.Render<NavLinkDisabledTest>(parameters => parameters.Add(x => x.Disabled, true));
            await comp.Find("a").ClickAsync();
            comp.Instance.IsNavigated.Should().BeFalse();
        }

        [Test]
        public async Task NavLinkOnClickErrorContentCaughtException()
        {
            var comp = Context.Render<NavLinkErrorContenCaughtException>();
            IElement AlertText() => MudAlert().Find("div.mud-alert-message");
            IRenderedComponent<MudAlert> MudAlert() => comp.FindComponent<MudAlert>();
            IReadOnlyList<IElement> Links() => comp.FindAll(".mud-nav-link");
            IElement MudLink() => Links()[0];

            await MudLink().ClickAsync(new MouseEventArgs());

            AlertText().InnerHtml.Should().Be("Oh my! We caught an error and handled it!");
        }
    }
}