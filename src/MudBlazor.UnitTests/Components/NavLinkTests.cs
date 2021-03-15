#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

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
    public class NavLinkTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

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
            var comp = ctx.RenderComponent<MudNavLink>(new[]
            {
               Parameter(nameof(MudNavLink.Target), target),
            });
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            comp.Find("a").GetAttribute("rel").Should().Be(expectedRel);
        }

        [Test]
        public async Task NavLink_CheckOnClickEvent()
        {
            var clicked = false;
            var comp = ctx.RenderComponent<MudNavLink>(new[]
            {
               EventCallback(nameof(MudNavLink.OnClick), (MouseEventArgs args) => { clicked = true; }),
            });
            // print the generated html
            Console.WriteLine(comp.Markup);
            comp.FindAll("a").Should().BeEmpty();
            comp.Find(".mud-nav-link").Click();
            clicked.Should().BeTrue();
        }

        [Test]
        public async Task NavLink_Enabled_CheckNavigation()
        {
            var comp = ctx.RenderComponent<NavLinkDisabledTest>(new[]
            {
                Parameter(nameof(NavLinkDisabledTest.Disabled), false)
            });
            Console.WriteLine(comp.Markup);
            comp.Find("a").Click();
            comp.Instance.IsNavigated.Should().BeTrue();
        }

        [Test]
        public async Task NavLink_Disabled_CheckNoNavigation()
        {
            var comp = ctx.RenderComponent<NavLinkDisabledTest>(new[]
            {
                Parameter(nameof(NavLinkDisabledTest.Disabled), true)
            });
            Console.WriteLine(comp.Markup);
            comp.Find("a").Click();
            comp.Instance.IsNavigated.Should().BeFalse();
        }
    }
}
