#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests
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
    }
}
