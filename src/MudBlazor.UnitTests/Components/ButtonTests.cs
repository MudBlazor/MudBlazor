using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
using System.Reflection.Metadata;
using System.Threading.Tasks;
namespace MudBlazor.UnitTests
{
    [TestFixture]
    public class ButtonTests
    {


        /// <summary>
        /// Button Link renders an anchor
        /// </summary>
        [Test]
        public void ShouldRenderAnAnchorWithHref()
        {
            using var ctx = new Bunit.TestContext();
            var href = Parameter(nameof(MudButton.Link), "https://www.google.com");
            var comp = ctx.RenderComponent<MudButton>(href);

            //the button should render an anchor element with href property
            var element = comp.Find("a[href=\"https://www.google.com\"]");
            element.Should().NotBeNull();
        }

        /// <summary>
        /// Button rendering a label
        /// </summary>
        [Test]
        public void ShouldRenderALabel()
        {
            using var ctx = new Bunit.TestContext();
            var htmlTag = Parameter(nameof(MudButton.HtmlTag), "label");
            var comp = ctx.RenderComponent<MudButton>(htmlTag);

            //the button should render a label
            var element = comp.Find("label");
            element.Should().NotBeNull();

            //no button should be present
            var button = comp.FindAll("button");
            button.Should().HaveCount(0);
        }
    }
}
