using AwesomeAssertions;
using Bunit;
using MudBlazor.UnitTests.TestComponents.Scroll;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ScrollToTopTests : BunitTest
    {
        /// <summary>
        /// Test scrolling and clicking on 'scroll to top' element
        /// </summary>
        [Test]
        public async Task ScrollToTopTestAsync()
        {
            var comp = Context.Render<ScrollToTopTest>();

            comp.Instance.Clicked.Should().BeFalse(because: "Not clicked yet");

            // scrollBottomButton click check
            await comp.Find("button").ClickAsync();
            var scrollIntoViewInvocation = Context.JSInterop.VerifyInvoke("mudScrollManager.scrollIntoView");
            scrollIntoViewInvocation.Arguments.Count.Should().Be(2);

            // checks invocation of js scroll function to ensure main functionality
            await comp.Find("span.mud-scroll-to-top").ClickAsync();
            var scrollToInvocation = Context.JSInterop.VerifyInvoke("mudScrollManager.scrollTo");
            scrollToInvocation.Arguments.Count.Should().Be(4);

            // checks that click on MudScrollToTop raised an event
            comp.Instance.Clicked.Should().BeTrue(because: "Clicked");
        }
    }
}
