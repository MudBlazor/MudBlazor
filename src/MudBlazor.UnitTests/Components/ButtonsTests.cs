using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
namespace MudBlazor.UnitTests
{
    [TestFixture]
    public class ButtonsTests
    {
        /// <summary>
        /// MudButton whithout specifying HtmlTag, renders a button
        /// </summary>
        [Test]
        public void MudButtonShouldRenderAButtonByDefault()
        {
            using var ctx = new Bunit.TestContext();


            var comp = ctx.RenderComponent<MudButton>();

            //no HtmlTag nor Link properties are set, so HtmlTag is button by default
            comp.Instance
                .HtmlTag
                .Should()
                .Be("button");


            //it is a button, and has by default stopPropagation on onclick
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<button")
                .And
                .Contain("stopPropagation");

        }

        /// <summary>
        /// MudButton renders an anchor element when Link is set
        /// </summary>
        [Test]
        public void MudButtonShouldRenderAnAnchorIfLinkIsSet()
        {
            using var ctx = new Bunit.TestContext();
            var link = Parameter(nameof(MudButton.Link), "https://www.google.com");
            var target = Parameter(nameof(MudButton.Target), "_blank");

            var comp = ctx.RenderComponent<MudButton>(link, target);

            //Link property is set, so it has to render an anchor element
            comp.Instance
                .HtmlTag
                .Should()
                .Be("a");

            //Target property is set, so it must have the rel attribute set to noopener
            comp.Markup
                .Should()
                .Contain("rel=\"noopener\"");

            //it is an anchor and not contains stopPropagation 
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<a")
                .And
                .NotContain("__internal_stopPropagation_onclick");
        }



        /// <summary>
        /// MudButton whithout specifying HtmlTag, renders a button
        /// </summary>
        [Test]
        public void MudIconButtonShouldRenderAButtonByDefault()
        {
            using var ctx = new Bunit.TestContext();


            var comp = ctx.RenderComponent<MudIconButton>();

            //no HtmlTag nor Link properties are set, so HtmlTag is button by default
            comp.Instance
                .HtmlTag
                .Should()
                .Be("button");


            //it is a button
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<button");

        }

        /// <summary>
        /// MudButton renders an anchor element when Link is set
        /// </summary>
        [Test]
        public void MudIconButtonShouldRenderAnAnchorIfLinkIsSet()
        {
            using var ctx = new Bunit.TestContext();
            var link = Parameter(nameof(MudIconButton.Link), "https://www.google.com");
            var target = Parameter(nameof(MudIconButton.Target), "_blank");

            var comp = ctx.RenderComponent<MudIconButton>(link, target);

            //Link property is set, so it has to render an anchor element
            comp.Instance
                .HtmlTag
                .Should()
                .Be("a");

            //Target property is set, so it must have the rel attribute set to noopener
            comp.Markup
                .Should()
                .Contain("rel=\"noopener\"");

            //it is an anchor
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<a");
        }

        /// <summary>
        /// MudButton whithout specifying HtmlTag, renders a button
        /// </summary>
        [Test]
        public void MudFabShouldRenderAButtonByDefault()
        {
            using var ctx = new Bunit.TestContext();


            var comp = ctx.RenderComponent<MudFab>();

            //no HtmlTag nor Link properties are set, so HtmlTag is button by default
            comp.Instance
                .HtmlTag
                .Should()
                .Be("button");


            //it is a button
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<button");

        }

        /// <summary>
        /// MudButton renders an anchor element when Link is set
        /// </summary>
        [Test]
        public void MudFabShouldRenderAnAnchorIfLinkIsSet()
        {
            using var ctx = new Bunit.TestContext();
            var link = Parameter(nameof(MudFab.Link), "https://www.google.com");
            var target = Parameter(nameof(MudFab.Target), "_blank");

            var comp = ctx.RenderComponent<MudFab>(link, target);

            //Link property is set, so it has to render an anchor element
            comp.Instance
                .HtmlTag
                .Should()
                .Be("a");

            //Target property is set, so it must have the rel attribute set to noopener
            comp.Markup
                .Should()
                .Contain("rel=\"noopener\"");

            //it is an anchor
            comp.Markup
                .Replace(" ", string.Empty)
                .Should()
                .StartWith("<a");
        }
    }
}



