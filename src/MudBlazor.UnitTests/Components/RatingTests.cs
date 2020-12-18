using System;
using System.Linq;
using Bunit;
using FluentAssertions;
using NUnit.Framework;


namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class RatingTests
    {
        [Test]
        public void RatingTest1() {
            // click should change selected value
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<RatingTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRating>();
            var ratingItemsSpans = comp.FindAll("span.mud-rating-item").ToArray();
            var inputs = comp.FindAll("input[type=\"radio\"].mud-rating-input").ToArray();
            // check initial state
            group.Instance.SelectedValue.Should().Be(0);
            Assert.AreEqual(ratingItemsSpans.Length, 5);
            Assert.AreEqual(inputs.Length, 5);

            // click first rating item
            ratingItemsSpans[0].Click();
            group.Instance.SelectedValue.Should().Be(1);

            // click 2nd rating item
            ratingItemsSpans[1].Click();
            group.Instance.SelectedValue.Should().Be(2);

            // click 3rd rating item
            ratingItemsSpans[2].Click();
            group.Instance.SelectedValue.Should().Be(3);

            // click 4th rating item
            ratingItemsSpans[3].Click();
            group.Instance.SelectedValue.Should().Be(4);

            // click 5th rating item
            ratingItemsSpans[4].Click();
            group.Instance.SelectedValue.Should().Be(5);

            ratingItemsSpans[1].Click();
            group.Instance.SelectedValue.Should().Be(2);
        }

        [Test]
        public void RatingTest2()
        {
            // click already selected item should change selected value to 0
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<RatingTest2>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRating>();
            var ratingItemsSpans = comp.FindAll("span.mud-rating-item").ToArray();
            // check initial state
            group.Instance.SelectedValue.Should().Be(0);
            Assert.AreEqual(ratingItemsSpans.Length, 5);

            // click 2nd rating item
            ratingItemsSpans[1].Click();
            group.Instance.SelectedValue.Should().Be(2);

            // click 2nd rating item
            ratingItemsSpans[1].Click();
            group.Instance.SelectedValue.Should().Be(0);

            // click 2nd rating item
            ratingItemsSpans[1].Click();
            group.Instance.SelectedValue.Should().Be(2);

            // click first rating item
            ratingItemsSpans[0].Click();
            group.Instance.SelectedValue.Should().Be(1);

            // click first rating item
            ratingItemsSpans[0].Click();
            group.Instance.SelectedValue.Should().Be(0);

            // click first rating item
            ratingItemsSpans[0].Click();
            group.Instance.SelectedValue.Should().Be(1);
        }

        [Test]
        public void RatingTest3()
        {
            // initialized selected value by parameter should equal component selected value
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<RatingTest3>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRating>();
            // check initial state
            group.Instance.SelectedValue.Should().Be(3);
        }

        [Test]
        public void RatingTest4()
        {
            // Click disabled component don't change SelectedValue
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<RatingTest4>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRating>();
            var ratingItemsSpans = comp.FindAll("span.mud-rating-item").ToArray();
            // check initial state
            group.Instance.SelectedValue.Should().Be(2);
            Assert.AreEqual(ratingItemsSpans.Length, 5);

            // click first rating item
            ratingItemsSpans[0].Click();
            group.Instance.SelectedValue.Should().Be(2);

            // click 2nd rating item
            ratingItemsSpans[1].Click();
            group.Instance.SelectedValue.Should().Be(2);

            // click 3rd rating item
            ratingItemsSpans[2].Click();
            group.Instance.SelectedValue.Should().Be(2);

            // click 4th rating item
            ratingItemsSpans[3].Click();
            group.Instance.SelectedValue.Should().Be(2);

            // click 5th rating item
            ratingItemsSpans[4].Click();
            group.Instance.SelectedValue.Should().Be(2);
        }

        [Test]
        public void RatingTest5()
        {
            // Initialized MaxValue by parameter should equal rating items count.
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<RatingTest5>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRating>();
            var ratingItemsSpans = comp.FindAll("span.mud-rating-item").ToArray();
            // check initial state
            group.Instance.SelectedValue.Should().Be(0);
            Assert.AreEqual(ratingItemsSpans.Length, 12);
        }


    }
}
