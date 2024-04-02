using System;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
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
        public void ScrollToTopTest()
        {
            var comp = Context.RenderComponent<ScrollToTopTest>();

            comp.Instance.Clicked.Should().BeFalse(because: "Not clicked yet");

            // scrollBottomButton click check
            comp.Find("button").Click();
            var scrollIntoViewInvocation = Context.JSInterop.VerifyInvoke("mudScrollManager.scrollIntoView");
            scrollIntoViewInvocation.Arguments.Count.Should().Be(2);

            // checks invocation of js scroll function to ensure main functionality
            comp.Find("span.mud-scroll-to-top").Click();
            var scrollToInvocation = Context.JSInterop.VerifyInvoke("mudScrollManager.scrollTo");
            scrollToInvocation.Arguments.Count.Should().Be(4);

            // checks that click on MudScrollToTop raised an event
            comp.Instance.Clicked.Should().BeTrue(because: "Clicked");
        }
    }
}
