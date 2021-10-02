
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ElementTests : BunitTest
    {
        /// <summary>
        /// MudElement renders first an anchor and then a button
        /// </summary>
        [Test]
        public void Should_Render_An_Anchor_And_Then_A_Button()
        {
            var htmlTag = Parameter(nameof(MudElement.HtmlTag), "a");
            var className = Parameter(nameof(MudElement.Class), "mud-button-root");
            var comp = Context.RenderComponent<MudElement>(htmlTag, className);
            comp.MarkupMatches("<a class=\"mud-button-root\"></a>");
            htmlTag = Parameter(nameof(MudElement.HtmlTag), "button");
            comp.SetParametersAndRender(htmlTag, className);
            comp.MarkupMatches("<button class=\"mud-button-root\"></button>");
        }

        /// <summary>
        /// In this example, there is a mouseover event conditionally attached
        /// if the property Attached is set to true is attached
        /// if not, there shouldn't have any event present
        /// </summary>
        [Test]
        public void MudElement_Should_Not_Attach_A_Null_Event()
        {
            var comp = Context.RenderComponent<ElementTestEventNull>();

            //initially, renders just an empty span, because AttachEvent is false;
            comp.MarkupMatches("<span></span>");

            //we set AttachEvent to true, so it has to attach the mouseover event
            var attached = Parameter(nameof(ElementTestEventNull.AttachEvent), true);
            var comp2 = Context.RenderComponent<ElementTestEventNull>(attached);

            //because we didn't hovered yet the element, the WasHovered property is false
            comp2.Instance.WasHovered.Should().BeFalse();

            //after hovered the element, the property WasHovered should be true
            comp2.Find("span").MouseOver();
            comp2.Instance.WasHovered.Should().BeTrue();
        }
    }
}
