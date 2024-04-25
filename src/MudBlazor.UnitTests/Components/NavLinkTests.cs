
#pragma warning disable CS1998 // async without await

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class NavLinkTests : BunitTest
    {
        /// <summary>
        /// When Target is not empty, rel attribute should be equals to "noopener noreferrer" on the a element
        /// </summary>
        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("_self", "noopener noreferrer")]
        [TestCase("_blank", "noopener noreferrer")]
        [TestCase("_parent", "noopener noreferrer")]
        [TestCase("_top", "noopener noreferrer")]
        [TestCase("myFrameName", "noopener noreferrer")]
        public async Task NavLink_CheckRelAttribute(string target, string expectedRel)
        {
            var comp = Context.RenderComponent<MudNavLink>(Parameter(nameof(MudNavLink.Target), target));
            // print the generated html
            // select elements needed for the test
            comp.Find("a").GetAttribute("rel").Should().Be(expectedRel);
        }

        [Test]
        public async Task NavLink_CheckOnClickEvent()
        {
            var clicked = false;
            var comp = Context.RenderComponent<MudNavLink>(EventCallback(nameof(MudNavLink.OnClick), (MouseEventArgs args) => { clicked = true; }));
            // print the generated html
            comp.FindAll("a").Should().BeEmpty();
            comp.Find(".mud-nav-link").Click();
            clicked.Should().BeTrue();
        }

        [Test]
        public async Task NavLink_Active()
        {
            const string activeClass = "Custom__nav_active_css";
            var comp = Context.RenderComponent<MudNavLink>(Parameter(nameof(MudNavLink.ActiveClass), activeClass));
            comp.Find(".mud-nav-link").Click();
            comp.Markup.Should().Contain(activeClass);
        }

        [Test]
        public async Task NavLink_Enabled_CheckNavigation()
        {
            var comp = Context.RenderComponent<NavLinkDisabledTest>(Parameter(nameof(NavLinkDisabledTest.Disabled), false));
            comp.Find("a").Click();
            comp.Instance.IsNavigated.Should().BeTrue();
        }

        [Test]
        public async Task NavLink_Disabled_CheckNoNavigation()
        {
            var comp = Context.RenderComponent<NavLinkDisabledTest>(Parameter(nameof(NavLinkDisabledTest.Disabled), true));
            comp.Find("a").Click();
            comp.Instance.IsNavigated.Should().BeFalse();
        }

        /// <summary>
        /// Verify that when IconOnly is set on the MudNavLink component, the div with the class ".mud-nav-link-text" is not rendered
        /// Also verify that when IconOnly is not set on the MudNavLink component, that the div with the class ".mud-nav-link-text" is rendered
        /// </summary>
        [Test]
        public async Task NavLink_IconOnly_NoText()
        {
            var comp = Context.RenderComponent<MudNavLink>(Parameter(nameof(MudNavLink.IconOnly), true));
            comp.FindAll(".mud-nav-link-text").Should().BeEmpty();
            comp = Context.RenderComponent<MudNavLink>();
            comp.FindAll(".mud-nav-link-text").Should().NotBeEmpty();
        }
    }
}
