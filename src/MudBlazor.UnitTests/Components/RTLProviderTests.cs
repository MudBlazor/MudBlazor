using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class RTLProviderTests : BunitTest
    {
        /// <summary>
        /// The provider element is rendered directly, without a nested component between the caller and the DOM.
        /// </summary>
        [Test]
        public void RTLProvider_ShouldRenderItsElementWithoutAWrapperComponent()
        {
            var comp = Context.Render<MudRTLProvider>(parameters => parameters
                .Add(x => x.Class, "my-class")
                .Add(x => x.Style, "color:red"));

            comp.FindComponents<MudElement>().Should().BeEmpty();
            comp.MarkupMatches("""<div class="mud-rtl-provider my-class" style="color:red"></div>""");
        }

        /// <summary>
        /// Content inside the provider still receives the cascaded direction.
        /// </summary>
        [TestCase(true, "mud-typography-align-right")]
        [TestCase(false, "mud-typography-align-left")]
        public void RTLProvider_ShouldCascadeDirectionToItsContent(bool rightToLeft, string expectedClass)
        {
            var comp = Context.Render<MudRTLProvider>(parameters => parameters
                .Add(x => x.RightToLeft, rightToLeft)
                .AddChildContent<MudText>(text => text.Add(x => x.Align, Align.Start)));

            comp.Find("p.mud-typography").ClassList.Should().Contain(expectedClass);
        }
    }
}
