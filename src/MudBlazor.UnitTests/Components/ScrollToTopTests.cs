using MudBlazor.UnitTests.TestComponents;
using Bunit;
using NUnit.Framework;
using FluentAssertions;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ScrollToTopTests : BunitTest
    {
        /// <summary>
        /// Test scrolling and clicking on 'scroll to top' element
        /// </summary>
        [Test]
        public void ScrollToTopTest()
        {
            var comp = Context.RenderComponent<ScrollToTopTest>();

            comp.Find("h6[id='scroll_on_top_click_event_raised']").Should().NotBeNull();
            comp.Find("h6[id='scroll_on_top_click_event_raised']").TextContent.Should().Be("false", because: "Not clicked yet");

            // scrollBottomButton click check
            comp.Find("button").Click();
            var scrollIntoViewInvocation = Context.JSInterop.VerifyInvoke("mudScrollManager.scrollIntoView");
            scrollIntoViewInvocation.Arguments.Count.Should().Be(2);

            // checks invocation of js scroll function to ensure main functionality
            comp.Find("span").Click();
            var scrollToInvocation = Context.JSInterop.VerifyInvoke("mudScrollManager.scrollTo");
            scrollToInvocation.Arguments.Count.Should().Be(4);

            // checks that click on MudScrollToTop raised an event
            comp.Find("h6[id='scroll_on_top_click_event_raised']").TextContent.Should().Be("true", because: "Clicked");
        }
    }
}
