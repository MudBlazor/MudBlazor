
using System;
using System.Linq;
using Bunit;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class RatingTests : BunitTest
    {
        /// <summary>
        /// click should change selected value
        /// </summary>
        [Test]
        public void RatingTest1()
        {
            var comp = Context.RenderComponent<MudRating>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var ratingItemsSpans = comp.FindAll("span.mud-rating-item").ToArray();
            var inputs = comp.FindAll("input[type=\"radio\"].mud-rating-input").ToArray();
            // check initial state
            comp.Instance.SelectedValue.Should().Be(0);
            Assert.AreEqual(ratingItemsSpans.Length, 5);
            Assert.AreEqual(inputs.Length, 5);

            // click first rating item
            ratingItemsSpans[0].Click();
            comp.Instance.SelectedValue.Should().Be(1);

            // click 2nd rating item
            ratingItemsSpans[1].Click();
            comp.Instance.SelectedValue.Should().Be(2);

            // click 3rd rating item
            ratingItemsSpans[2].Click();
            comp.Instance.SelectedValue.Should().Be(3);

            // click 4th rating item
            ratingItemsSpans[3].Click();
            comp.Instance.SelectedValue.Should().Be(4);

            // click 5th rating item
            ratingItemsSpans[4].Click();
            comp.Instance.SelectedValue.Should().Be(5);

            ratingItemsSpans[1].Click();
            comp.Instance.SelectedValue.Should().Be(2);
        }

        /// <summary>
        /// click already selected item should change selected value to 0
        /// </summary>
        [Test]
        public void RatingTest2()
        {
            var comp = Context.RenderComponent<MudRating>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var ratingItemsSpans = comp.FindAll("span.mud-rating-item").ToArray();
            // check initial state
            comp.Instance.SelectedValue.Should().Be(0);
            Assert.AreEqual(ratingItemsSpans.Length, 5);

            // click 2nd rating item
            ratingItemsSpans[1].Click();
            comp.Instance.SelectedValue.Should().Be(2);

            // click 2nd rating item
            ratingItemsSpans[1].Click();
            comp.Instance.SelectedValue.Should().Be(0);

            // click 2nd rating item
            ratingItemsSpans[1].Click();
            comp.Instance.SelectedValue.Should().Be(2);

            // click first rating item
            ratingItemsSpans[0].Click();
            comp.Instance.SelectedValue.Should().Be(1);

            // click first rating item
            ratingItemsSpans[0].Click();
            comp.Instance.SelectedValue.Should().Be(0);

            // click first rating item
            ratingItemsSpans[0].Click();
            comp.Instance.SelectedValue.Should().Be(1);
        }

        /// <summary>
        ///  initialized selected value by parameter should equal component selected value
        /// </summary>
        [Test]
        public void RatingTest3()
        {
            var comp = Context.RenderComponent<MudRating>(("SelectedValue", 3));
            // print the generated html
            Console.WriteLine(comp.Markup);
            // check initial state
            comp.Instance.SelectedValue.Should().Be(3);
        }

        /// <summary>
        /// Click disabled component don't change SelectedValue
        /// </summary>
        [Test]
        public void RatingTest4()
        {
            var comp = Context.RenderComponent<MudRating>(("Disabled", true), ("SelectedValue", 2));
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var ratingItemsSpans = comp.FindAll("span.mud-rating-item").ToArray();
            // check initial state
            comp.Instance.SelectedValue.Should().Be(2);
            Assert.AreEqual(ratingItemsSpans.Length, 5);

            // click first rating item
            ratingItemsSpans[0].Click();
            comp.Instance.SelectedValue.Should().Be(2);

            // click 2nd rating item
            ratingItemsSpans[1].Click();
            comp.Instance.SelectedValue.Should().Be(2);

            // click 3rd rating item
            ratingItemsSpans[2].Click();
            comp.Instance.SelectedValue.Should().Be(2);

            // click 4th rating item
            ratingItemsSpans[3].Click();
            comp.Instance.SelectedValue.Should().Be(2);

            // click 5th rating item
            ratingItemsSpans[4].Click();
            comp.Instance.SelectedValue.Should().Be(2);
        }

        /// <summary>
        /// Initialized MaxValue by parameter should equal rating items count.
        /// </summary>
        [Test]
        public void RatingTest5()
        {
            var comp = Context.RenderComponent<MudRating>(("MaxValue", 12));
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var ratingItemsSpans = comp.FindAll("span.mud-rating-item").ToArray();
            // check initial state
            comp.Instance.SelectedValue.Should().Be(0);
            Assert.AreEqual(ratingItemsSpans.Length, 12);
        }

        [Test]
        public void ReadOnlyRating_ShouldNotRenderInputs()
        {
            var comp = Context.RenderComponent<MudRating>(("ReadOnly", true));
            comp.FindAll("input").Should().BeEmpty();
        }
    }
}
