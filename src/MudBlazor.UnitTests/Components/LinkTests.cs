
#pragma warning disable CS1998 // async without await

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class LinkTests : BunitTest
    {
        [Test]
        public async Task NavLink_CheckDisabled()
        {
            var comp = Context.RenderComponent<MudLink>(
                Parameter(nameof(MudLink.Href), "#"),
                Parameter(nameof(MudLink.Disabled), true));
            Console.WriteLine(comp.Markup);
            comp.Find("a").GetAttribute("href").Should().BeNullOrEmpty();
        }
    }
}
