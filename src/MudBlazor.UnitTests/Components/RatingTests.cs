using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
#nullable enable
    [TestFixture]
    public class RatingTests : BunitTest
    {
        /// <summary>
        /// Click should change selected value
        /// </summary>
        [Test]
        public void RatingTest1()
        {
            var comp = Context.RenderComponent<MudRating>();
            // select elements needed for the test
            IRefreshableElementCollection<IElement> RatingItemsSpans() => comp.FindAll("span.mud-rating-item");
            IRefreshableElementCollection<IElement> Inputs() => comp.FindAll("input[type=\"radio\"].mud-rating-input");
            // check initial state
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(0);
            RatingItemsSpans().Count.Should().Be(5);
            Inputs().Count.Should().Be(5);

            // click first rating item
            RatingItemsSpans()[0].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(1);

            // click 2nd rating item
            RatingItemsSpans()[1].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(2);

            // click 3rd rating item
            RatingItemsSpans()[2].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(3);

            // click 4th rating item
            RatingItemsSpans()[3].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(4);

            // click 5th rating item
            RatingItemsSpans()[4].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(5);

            RatingItemsSpans()[1].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(2);
        }

        /// <summary>
        /// Click already selected item should change selected value to 0
        /// </summary>
        [Test]
        public void RatingTest2()
        {
            var comp = Context.RenderComponent<MudRating>();
            // select elements needed for the test
            IRefreshableElementCollection<IElement> RatingItemsSpans() => comp.FindAll("span.mud-rating-item");
            // check initial state
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(0);
            RatingItemsSpans().Count.Should().Be(5);

            // click 2nd rating item
            RatingItemsSpans()[1].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(2);

            // click 2nd rating item
            RatingItemsSpans()[1].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(0);

            // click 2nd rating item
            RatingItemsSpans()[1].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(2);

            // click first rating item
            RatingItemsSpans()[0].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(1);

            // click first rating item
            RatingItemsSpans()[0].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(0);

            // click first rating item
            RatingItemsSpans()[0].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(1);
        }

        /// <summary>
        /// Initialized selected value by parameter should equal component selected value
        /// </summary>
        [Test]
        public void RatingTest3()
        {
            var comp = Context.RenderComponent<MudRating>(parameters => parameters
                .Add(p => p.SelectedValue, 3));
            // print the generated html
            // check initial state
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(3);
        }

        /// <summary>
        /// Click disabled component don't change SelectedValue
        /// </summary>
        [Test]
        public void RatingTest4()
        {
            var comp = Context.RenderComponent<MudRating>(parameters => parameters
                .Add(p => p.Disabled, true)
                .Add(p => p.SelectedValue, 2));
            // print the generated html
            // select elements needed for the test
            IRefreshableElementCollection<IElement> RatingItemsSpans() => comp.FindAll("span.mud-rating-item");
            // check initial state
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(2);
            RatingItemsSpans().Count.Should().Be(5);

            // click first rating item
            RatingItemsSpans()[0].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(2);

            // click 2nd rating item
            RatingItemsSpans()[1].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(2);

            // click 3rd rating item
            RatingItemsSpans()[2].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(2);

            // click 4th rating item
            RatingItemsSpans()[3].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(2);

            // click 5th rating item
            RatingItemsSpans()[4].Click();
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(2);
        }

        /// <summary>
        /// Initialized MaxValue by parameter should equal rating items count.
        /// </summary>
        [Test]
        public async Task RatingTest5()
        {
            var comp = Context.RenderComponent<MudRating>(parameters => parameters
                .Add(p => p.MaxValue, 12));
            // print the generated html
            // select elements needed for the test
            IRefreshableElementCollection<IElement> RatingItemsSpans() => comp.FindAll("span.mud-rating-item");
            // check initial state
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(0);
            RatingItemsSpans().Count.Should().Be(12);

            await comp.Instance.HandleItemHoveredAsync(6);
            comp.Instance.HoveredValue.Should().Be(6);
            comp.Instance.GetState(x => x.SelectedValue).Should().Be(0);
            comp.Instance.IsRatingHover.Should().Be(true);
        }

        [Test]
        public void ReadOnlyRating_ShouldNotRenderInputs()
        {
            var comp = Context.RenderComponent<MudRating>(parameters => parameters
                .Add(p => p.ReadOnly, true));
            comp.FindAll("input").Should().BeEmpty();
        }

        [Test]
        public async Task RatingTest_KeyboardNavigation()
        {
            var comp = Context.RenderComponent<MudRating>(parameters => parameters
                .Add(p => p.MaxValue, 12));
            var item = comp.FindComponent<MudRatingItem>();
            // print the generated html

            await comp.InvokeAsync(() => item.Instance.HandleMouseOutAsync(new MouseEventArgs()));
            await comp.InvokeAsync(() => item.Instance.HandleMouseOverAsync(new MouseEventArgs()));

            await comp.InvokeAsync(() => comp.Instance.SetHoveredValueAsync(15));
            await comp.InvokeAsync(() => item.Instance.SelectIcon());
            comp.SetParam(x => x.SelectedValue, 12);
            await comp.InvokeAsync(() => comp.Instance.SetHoveredValueAsync(0));
            await comp.InvokeAsync(() => item.Instance.SelectIcon());
            comp.SetParam(x => x.SelectedValue, 0);

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.SelectedValue).Should().Be(1));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.SelectedValue).Should().Be(0));
            //ArrowLeft should not decrease when the value is 0
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.SelectedValue).Should().Be(0));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.SelectedValue).Should().Be(12));
            //Shift+ArrowKey should not go beyond the max value
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.SelectedValue).Should().Be(12));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.SelectedValue).Should().Be(0));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.SelectedValue).Should().Be(0));

            comp.SetParam(x => x.Disabled, true);
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.SelectedValue).Should().Be(0));

            await comp.InvokeAsync(() => item.Instance.HandleMouseOutAsync(new MouseEventArgs()));
            await comp.InvokeAsync(() => item.Instance.HandleMouseOverAsync(new MouseEventArgs()));
        }
    }
}
