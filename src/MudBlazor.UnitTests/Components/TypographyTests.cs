#pragma warning disable CS1998 // async without await

using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class TypographyTests : BunitTest
    {
        [Test]
        public void MudTextInlineTest()
        {
            var comp = Context.RenderComponent<TextInlineTest>();
            comp.FindComponents<MudText>().Count.Should().Be(2);
            comp.Markup.Should().NotContain("d-inline");
            var secondText = comp.FindComponents<MudText>().Last();
            secondText.SetParam("Inline", true);
            comp.Markup.Should().Contain("d-inline");
        }
    }
}
