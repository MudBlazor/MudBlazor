using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
using System.Reflection.Metadata;
using System.Threading.Tasks;
namespace MudBlazor.UnitTests
{
    [TestFixture]
    public class ElementTests
    {


        /// <summary>
        /// MudElement renders first an anchor and then a button
        /// </summary>
        [Test]
        public void ShouldRenderAnAnchorAndThenAButton()
        {
            using var ctx = new Bunit.TestContext();
            var htmlTag = Parameter(nameof(MudElement.HtmlTag), "a");
            var className= Parameter(nameof(MudElement.Class), "mud-button-root");
            var comp = ctx.RenderComponent<MudElement>(htmlTag, className);
            comp.MarkupMatches("<a class=\"mud-button-root\"></a>");

            htmlTag = Parameter(nameof(MudElement.HtmlTag), "button");
            comp.SetParametersAndRender(htmlTag, className);
            comp.MarkupMatches("<button class=\"mud-button-root\"></button>");


        }
    }
}
