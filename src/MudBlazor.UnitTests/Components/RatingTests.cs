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
            comp.Instance.GetState(x => x.Value).Should().Be(0);
            RatingItemsSpans().Count.Should().Be(5);
            Inputs().Count.Should().Be(5);

            // click first rating item
            RatingItemsSpans()[0].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(1);

            // click 2nd rating item
            RatingItemsSpans()[1].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(2);

            // click 3rd rating item
            RatingItemsSpans()[2].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(3);

            // click 4th rating item
            RatingItemsSpans()[3].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(4);

            // click 5th rating item
            RatingItemsSpans()[4].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(5);

            RatingItemsSpans()[1].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(2);
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
            comp.Instance.GetState(x => x.Value).Should().Be(0);
            RatingItemsSpans().Count.Should().Be(5);

            // click 2nd rating item
            RatingItemsSpans()[1].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(2);

            // click 2nd rating item
            RatingItemsSpans()[1].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(0);

            // click 2nd rating item
            RatingItemsSpans()[1].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(2);

            // click first rating item
            RatingItemsSpans()[0].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(1);

            // click first rating item
            RatingItemsSpans()[0].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(0);

            // click first rating item
            RatingItemsSpans()[0].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(1);
        }

        /// <summary>
        /// Initialized selected value by parameter should equal component selected value
        /// </summary>
        [Test]
        public void RatingTest3()
        {
            var comp = Context.RenderComponent<MudRating>(parameters => parameters
                .Add(p => p.Value, 3));
            // print the generated html
            // check initial state
            comp.Instance.GetState(x => x.Value).Should().Be(3);
        }

        /// <summary>
        /// Click disabled component don't change SelectedValue
        /// </summary>
        [Test]
        public void RatingTest4()
        {
            var comp = Context.RenderComponent<MudRating>(parameters => parameters
                .Add(p => p.Disabled, true)
                .Add(p => p.Value, 2));
            // print the generated html
            // select elements needed for the test
            IRefreshableElementCollection<IElement> RatingItemsSpans() => comp.FindAll("span.mud-rating-item");
            // check initial state
            comp.Instance.GetState(x => x.Value).Should().Be(2);
            RatingItemsSpans().Count.Should().Be(5);

            // click first rating item
            RatingItemsSpans()[0].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(2);

            // click 2nd rating item
            RatingItemsSpans()[1].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(2);

            // click 3rd rating item
            RatingItemsSpans()[2].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(2);

            // click 4th rating item
            RatingItemsSpans()[3].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(2);

            // click 5th rating item
            RatingItemsSpans()[4].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(2);
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
            comp.Instance.GetState(x => x.Value).Should().Be(0);
            RatingItemsSpans().Count.Should().Be(12);

            await comp.Instance.HandleItemHoveredAsync(6);
            comp.Instance.HoveredValue.Should().Be(6);
            comp.Instance.GetState(x => x.Value).Should().Be(0);
            comp.Instance.IsRatingHover.Should().Be(true);
        }

        [Test]
        public void RatingTestHalfStar()
        {

#pragma warning disable CS0618 // Type or member is obsolete

            var comp = Context.RenderComponent<MudRating>(parameters => parameters
                .Add(p => p.Value, 2.4m));

            // print the generated html
            // select elements needed for the test
            IRefreshableElementCollection<IElement> RatingItemsSpans() => comp.FindAll("span.mud-rating-item");
            // check initial state
            comp.Instance.GetState(x => x.Value).Should().Be(2.4m);
            RatingItemsSpans().Count.Should().Be(5);

            // confirm SelectedValue == 2
            comp.Instance.SelectedValue.Should().Be(2);

            // set Value to 2.6, confirm SelectedValue == 3
            comp.SetParam(x => x.Value, 2.6m);
            comp.Instance.SelectedValue.Should().Be(3);

            // set SelectedValue to 4, confirm Value == 4
            comp.SetParam(x => x.SelectedValue, 4);
            comp.Instance.Value.Should().Be(4m);

#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Initialized EmptyIconColor and FullIconColor by parameter should have the correct colors set.
        /// Pre half star test - so no test of half star color.
        /// </summary>
        [Test]
        public void RatingTestIconColors()
        {
            var comp = Context.RenderComponent<MudRating>(parameters => parameters
                .Add(p => p.Value, 2)
                .Add(p => p.EmptyIconColor, Color.Tertiary)
                .Add(p => p.HalfIconColor, Color.Secondary)
                .Add(p => p.FullIconColor, Color.Primary));

            // Select elements needed for the test
            IRefreshableElementCollection<IElement> SvgColors() => comp.FindAll("svg.mud-icon-root");
            IRefreshableElementCollection<IElement> RatingItemsSpans() => comp.FindAll("span.mud-rating-item");

            // Check initial state
            SvgColors()[0].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[1].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[2].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[3].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[4].ClassName.Should().Contain("mud-tertiary-text");

            comp.Instance.GetState(x => x.Value).Should().Be(2);
            RatingItemsSpans()[0].Click();
            comp.Instance.GetState(x => x.Value).Should().Be(1);

            SvgColors()[0].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[1].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[2].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[3].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[4].ClassName.Should().Contain("mud-tertiary-text");

            RatingItemsSpans()[2].PointerOver();
            comp.Instance.HoveredValue.Should().Be(3);
            comp.Instance.GetState(x => x.Value).Should().Be(1);
            comp.Instance.IsRatingHover.Should().Be(true);

            SvgColors()[0].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[1].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[2].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[3].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[4].ClassName.Should().Contain("mud-tertiary-text");
            RatingItemsSpans()[2].ClassName.Should().Contain("mud-rating-item-active");

            RatingItemsSpans()[2].PointerOut();

            RatingItemsSpans()[4].Click();
            RatingItemsSpans()[1].PointerOver();
            comp.Instance.HoveredValue.Should().Be(2);
            comp.Instance.GetState(x => x.Value).Should().Be(5);
            comp.Instance.IsRatingHover.Should().Be(true);

            SvgColors()[0].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[1].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[2].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[3].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[4].ClassName.Should().Contain("mud-tertiary-text");
            RatingItemsSpans()[1].ClassName.Should().Contain("mud-rating-item-active");

            RatingItemsSpans()[1].PointerOut();
        }

        /// <summary>
        /// Color tests for half star
        /// </summary>
        [Test]
        public void RatingTestIconHalfColors()
        {
            var comp = Context.RenderComponent<MudRating>(parameters => parameters
                .Add(p => p.Value, 2.24m)
                .Add(p => p.EmptyIconColor, Color.Tertiary)
                .Add(p => p.HalfIconColor, Color.Secondary)
                .Add(p => p.FullIconColor, Color.Primary));

            // Select elements needed for the test
            IRefreshableElementCollection<IElement> SvgColors() => comp.FindAll("svg.mud-icon-root");

            // Check initial state
            SvgColors()[0].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[1].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[2].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[3].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[4].ClassName.Should().Contain("mud-tertiary-text");

            // > 2.5 should have a half star
            comp.SetParam(x => x.Value, 2.26m);

            SvgColors()[0].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[1].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[2].ClassName.Should().Contain("mud-secondary-text");
            SvgColors()[3].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[4].ClassName.Should().Contain("mud-tertiary-text");

            // < 2.75 still have a half star
            comp.SetParam(x => x.Value, 2.74m);

            SvgColors()[0].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[1].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[2].ClassName.Should().Contain("mud-secondary-text");
            SvgColors()[3].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[4].ClassName.Should().Contain("mud-tertiary-text");

            // > 2.75 should have a full star
            comp.SetParam(x => x.Value, 2.76m);

            SvgColors()[0].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[1].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[2].ClassName.Should().Contain("mud-primary-text");
            SvgColors()[3].ClassName.Should().Contain("mud-tertiary-text");
            SvgColors()[4].ClassName.Should().Contain("mud-tertiary-text");
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

            await comp.InvokeAsync(() => item.Instance.HandlePointerOutAsync(new PointerEventArgs()));
            await comp.InvokeAsync(() => item.Instance.HandlePointerOverAsync(new PointerEventArgs()));

            await comp.InvokeAsync(() => comp.Instance.SetHoveredValueAsync(15));
            await comp.InvokeAsync(() => item.Instance.SelectIcon());
            comp.SetParam(x => x.Value, 12m);
            await comp.InvokeAsync(() => comp.Instance.SetHoveredValueAsync(0));
            await comp.InvokeAsync(() => item.Instance.SelectIcon());
            comp.SetParam(x => x.Value, 0m);

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.Value).Should().Be(1));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.Value).Should().Be(0));
            //ArrowLeft should not decrease when the value is 0
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.Value).Should().Be(0));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.Value).Should().Be(12));
            //Shift+ArrowKey should not go beyond the max value
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.Value).Should().Be(12));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.Value).Should().Be(0));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.Value).Should().Be(0));

            comp.SetParam(x => x.Disabled, true);
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.GetState(x => x.Value).Should().Be(0));

            await comp.InvokeAsync(() => item.Instance.HandlePointerOutAsync(new PointerEventArgs()));
            await comp.InvokeAsync(() => item.Instance.HandlePointerOverAsync(new PointerEventArgs()));
        }
    }
}
