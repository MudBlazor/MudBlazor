
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
    public class BadgeTests : BunitTest
    {
        [Test]
        public async Task Badge_Renders_Using_Default_Values()
        {
            var comp = Context.RenderComponent<MudBadge>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("span").Should().HaveCount(3, "Default behavior of badge is to render 3 spans");
        }

        [Test]
        public async Task Badge_Renders_When_VisibleIsTrue()
        {
            var comp = Context.RenderComponent<MudBadge>();
            comp.SetParam("Visible", true);
            Console.WriteLine(comp.Markup);
            comp.FindAll("span").Should().HaveCount(3, "Visible badge renders 3 spans");
        }

        [Test]
        public async Task Badge_Does_Not_Render_When_VisibleIsFalse()
        {
            var comp = Context.RenderComponent<MudBadge>();
            comp.SetParam("Visible", false);
            Console.WriteLine(comp.Markup);
            comp.FindAll("span").Should().HaveCount(1, "Hidden badge renders 1 span");
        }
    }
}
