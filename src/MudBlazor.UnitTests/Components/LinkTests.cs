#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class LinkTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [Test]
        public async Task NavLink_CheckDisabled()
        {
            var comp = ctx.RenderComponent<MudLink>(new[]
            {
                Parameter(nameof(MudLink.Href), "#"),
                Parameter(nameof(MudLink.Disabled), true)
            });
            Console.WriteLine(comp.Markup);
            comp.Find("a").GetAttribute("href").Should().BeNullOrEmpty();
        }
    }
}
