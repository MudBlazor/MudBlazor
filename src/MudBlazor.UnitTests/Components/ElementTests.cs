#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using Bunit;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ElementTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        /// <summary>
        /// MudElement renders first an anchor and then a button
        /// </summary>
        [Test]
        public void ShouldRenderAnAnchorAndThenAButton()
        {
            var htmlTag = Parameter(nameof(MudElement.HtmlTag), "a");
            var className = Parameter(nameof(MudElement.Class), "mud-button-root");
            var comp = ctx.RenderComponent<MudElement>(htmlTag, className);
            comp.MarkupMatches("<a class=\"mud-button-root\"></a>");
            htmlTag = Parameter(nameof(MudElement.HtmlTag), "button");
            comp.SetParametersAndRender(htmlTag, className);
            comp.MarkupMatches("<button class=\"mud-button-root\"></button>");
        }
    }
}
