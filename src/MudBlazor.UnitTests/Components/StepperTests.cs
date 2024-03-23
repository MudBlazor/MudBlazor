using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class StepperTests : BunitTest
    {
        [Test]
        public void StepperContent_ShouldDisplayActiveStepContent()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "A");
                    step.Add(x => x.SecondaryText, "a");
                    step.Add(x => x.Class, "step-a");
                    step.Add(x => x.Style, "fontsize:11px");
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 1"));
                });
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "B");
                    step.Add(x => x.SecondaryText, "b");
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 2"));
                    step.Add(x => x.Class, "step-b");
                    step.Add(x => x.Style, "fontsize:12px");
                });
            });
            // check the steps
            stepper.Instance.Steps.Count.Should().Be(2);
            stepper.Instance.Steps.All(x => x.Parent == stepper.Instance).Should().Be(true);
            stepper.Instance.Steps[0].Title.Should().Be("A");
            stepper.Instance.Steps[0].IsActive.Should().Be(true);
            stepper.Instance.Steps[1].Title.Should().Be("B");
            stepper.Instance.Steps[1].IsActive.Should().Be(false);
            // check the DOM
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[0].TextContent.Trimmed().Should().Be("1");
            stepper.FindAll(".mud-stepper-nav-step-label-content .mud-typography-subtitle2")[0].TextContent.Trimmed().Should().Be("A");
            stepper.FindAll(".mud-stepper-nav-step-label-content .mud-typography-caption")[0].TextContent.Trimmed().Should().Be("a");
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[1].TextContent.Trimmed().Should().Be("2");
            stepper.FindAll(".mud-stepper-nav-step-label-content .mud-typography-body2")[0].TextContent.Trimmed().Should().Be("B");
            stepper.FindAll(".mud-stepper-nav-step-label-content .mud-typography-caption")[1].TextContent.Trimmed().Should().Be("b");
            stepper.Find(".mud-stepper-content").GetAttribute("class").Should().Contain("step-a");
            stepper.Find(".mud-stepper-content").GetAttribute("style").Should().Contain("fontsize:11px");
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 1");
        }

        [Test]
        public void Stepper_ShouldDisplayContentOfActiveStep()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.NonLinear, true);
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "A");
                    step.Add(x => x.SecondaryText, "a");
                    step.Add(x => x.Class, "step-a");
                    step.Add(x => x.Style, "fontsize:11px");
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 1"));
                });
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "B");
                    step.Add(x => x.SecondaryText, "b");
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 2"));
                    step.Add(x => x.Class, "step-b");
                    step.Add(x => x.Style, "fontsize:12px");
                });
            });
            // check the stepper content
            stepper.Find(".mud-stepper-content").GetAttribute("class").Should().Contain("step-a");
            stepper.Find(".mud-stepper-content").GetAttribute("style").Should().Contain("fontsize:11px");
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 1");
            stepper.FindAll(".mud-stepper-nav-step")[1].Click();
            stepper.Find(".mud-stepper-content").GetAttribute("class").Should().Contain("step-b");
            stepper.Find(".mud-stepper-content").GetAttribute("style").Should().Contain("fontsize:12px");
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 2");
            stepper.FindAll(".mud-stepper-nav-step")[0].Click();
            stepper.Find(".mud-stepper-content").GetAttribute("class").Should().Contain("step-a");
            stepper.Find(".mud-stepper-content").GetAttribute("style").Should().Contain("fontsize:11px");
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 1");
        }

        [Test]
        public void Stepper_ShouldNavigateViaNextAndPrevious()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.NonLinear, false);
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "A");
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 1"));
                });
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "B");
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 2"));
                });
            });
            // check the stepper content
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 1");
            stepper.FindAll("button")[0].HasAttribute("disabled").Should().Be(true); // previous
            stepper.FindAll("button")[1].HasAttribute("disabled").Should().Be(true); // skip
            stepper.FindAll("button")[2].HasAttribute("disabled").Should().Be(false); // next
            // step 1 icon should be "1", step 2 icon should be "2"
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[0].TextContent.Trimmed().Should().Be("1");
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[1].TextContent.Trimmed().Should().Be("2");
            stepper.FindAll("button")[2].Click(); // next
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 2");
            stepper.FindAll("button")[0].HasAttribute("disabled").Should().Be(false); // previous
            stepper.FindAll("button")[1].HasAttribute("disabled").Should().Be(true); // skip
            stepper.FindAll("button")[2].HasAttribute("disabled").Should().Be(false); // next
            // step 1 icon should be a check mark, step 2 icon should be "2"
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[0].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[1].TextContent.Trimmed().Should().Be("2");
            stepper.FindAll("button")[0].Click(); // prev
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 1");
            stepper.FindAll("button")[0].HasAttribute("disabled").Should().Be(true); // previous
            stepper.FindAll("button")[1].HasAttribute("disabled").Should().Be(true); // skip
            stepper.FindAll("button")[2].HasAttribute("disabled").Should().Be(false); // next
            // step 1 icon should be a check mark, step 2 icon should be "2"
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[0].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[1].TextContent.Trimmed().Should().Be("2");
            stepper.FindAll("button")[2].Click(); // next
            stepper.FindAll("button")[2].Click(); // next
            // step 1 and 2 icon should be check marks
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[0].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[1].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
        }

        [Test]
        public void Stepper_ShouldBeAbleToSkipSkippableSteps()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.NonLinear, false);
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "A");
                    step.Add(x => x.Skippable, true);
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 1"));
                });
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "B");
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 2"));
                });
            });
            // check the stepper content
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 1");
            stepper.FindAll("button")[0].HasAttribute("disabled").Should().Be(true); // previous
            stepper.FindAll("button")[1].HasAttribute("disabled").Should().Be(false); // skip
            stepper.FindAll("button")[2].HasAttribute("disabled").Should().Be(false); // next
            stepper.FindAll("button")[1].Click(); // skip
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 2");
            stepper.FindAll("button")[0].HasAttribute("disabled").Should().Be(false); // previous
            stepper.FindAll("button")[1].HasAttribute("disabled").Should().Be(true); // skip
            stepper.FindAll("button")[2].HasAttribute("disabled").Should().Be(false); // next
            Console.WriteLine(stepper.Markup);
            stepper.FindAll("button")[0].Click(); // prev
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 1");
            stepper.FindAll("button")[0].HasAttribute("disabled").Should().Be(true); // previous
            stepper.FindAll("button")[1].HasAttribute("disabled").Should().Be(false); // skip
            stepper.FindAll("button")[2].HasAttribute("disabled").Should().Be(false); // next
        }

        [Test]
        public void ActiveIndex_ShouldBeTwoWayBindable()
        {
            int activeIndex = 0;
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Bind(x => x.ActiveIndex, activeIndex, newValue => activeIndex = newValue);
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "0");
                    step.AddChildContent(text => text.AddMarkupContent(0, "index 0"));
                });
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "1");
                    step.AddChildContent(text => text.AddMarkupContent(0, "index 1"));
                });
            });
            // check how the stepper reacts to activeIndex changes
            activeIndex.Should().Be(0);
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("index 0");
            stepper.Instance.ActiveStep.Title.Should().Be("0");
            // goto 1
            stepper.SetParametersAndRender(Parameter(nameof(MudStepper.ActiveIndex), 1));
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("index 1");
            activeIndex.Should().Be(1);
            stepper.Instance.ActiveStep.Title.Should().Be("1");
            // goto 0
            stepper.SetParametersAndRender(Parameter(nameof(MudStepper.ActiveIndex), 0));
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("index 0");
            activeIndex.Should().Be(0);
            stepper.Instance.ActiveStep.Title.Should().Be("0");
            // check out-of-range handling
            // goto 5
            stepper.SetParametersAndRender(Parameter(nameof(MudStepper.ActiveIndex), 5));
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("index 1");
            stepper.Instance.ActiveStep.Title.Should().Be("1");
            activeIndex.Should().Be(1);
            // goto -69
            stepper.SetParametersAndRender(Parameter(nameof(MudStepper.ActiveIndex), -69));
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("index 0");
            stepper.Instance.ActiveStep.Title.Should().Be("0");
            activeIndex.Should().Be(0);
        }

        [Test]
        public async Task ManipulatingStepsProgrammatically_ShouldUpdateTheUi()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "A");
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 1"));
                });
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "B");
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 2"));
                });
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "C");
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 3"));
                });
            });
            // disable step 1
            stepper.FindAll(".mud-stepper-nav-step")[0].ClassList.Should().NotContain("mud-stepper-nav-step-disabled");
            await stepper.Instance.Steps[0].SetDisabledAsync(true);
            stepper.FindAll(".mud-stepper-nav-step")[0].ClassList.Should().Contain("mud-stepper-nav-step-disabled");
            // fail step 2
            stepper.FindAll(".mud-stepper-nav-step")[1].ClassList.Should().NotContain("mud-stepper-nav-step-error");
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[1].ClassList.Should().NotContain("mud-error");
            stepper.FindAll(".mud-stepper-nav-step-label-content")[1].ClassList.Should().NotContain("mud-error-text");
            await stepper.Instance.Steps[1].SetHasErrorAsync(true);
            stepper.FindAll(".mud-stepper-nav-step")[1].ClassList.Should().Contain("mud-stepper-nav-step-error");
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[1].ClassList.Should().Contain("mud-error");
            stepper.FindAll(".mud-stepper-nav-step-label-content")[1].ClassList.Should().Contain("mud-error-text");
            // complete step 3
            stepper.FindAll(".mud-stepper-nav-step")[2].ClassList.Should().NotContain("mud-stepper-nav-step-completed");
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[2].QuerySelectorAll("path").Should().BeEmpty(); // no svg icon if not completed
            await stepper.Instance.Steps[2].SetCompletedAsync(true);
            stepper.FindAll(".mud-stepper-nav-step-label-icon")[2].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
            stepper.FindAll(".mud-stepper-nav-step")[2].ClassList.Should().Contain("mud-stepper-nav-step-completed");
        }
        
        [Test]
        public async Task RemoveStep_ShouldUpdateActiveIndex()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.ActiveIndex, 2);
                self.AddChildContent<MudStep>(step => step.Add(x => x.Title, "A"));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Title, "B"));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Title, "C"));
            });
            stepper.Instance.ActiveStep.Title.Should().Be("C");
            stepper.Instance.ActiveIndex.Should().Be(2);
            await stepper.Instance.RemoveStepAsync(stepper.Instance.ActiveStep);
            stepper.Instance.ActiveStep.Title.Should().Be("B");
            stepper.Instance.ActiveIndex.Should().Be(1);
        }
    }
}
