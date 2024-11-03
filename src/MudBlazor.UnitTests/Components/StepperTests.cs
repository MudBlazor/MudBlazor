using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents.Stepper;
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
            stepper.FindAll(".mud-step-label-icon")[0].TextContent.Trimmed().Should().Be("1");
            stepper.FindAll(".mud-step-label-content .mud-typography-subtitle2")[0].TextContent.Trimmed().Should().Be("A");
            stepper.FindAll(".mud-step-label-content .mud-typography-caption")[0].TextContent.Trimmed().Should().Be("a");
            stepper.FindAll(".mud-step-label-icon")[1].TextContent.Trimmed().Should().Be("2");
            stepper.FindAll(".mud-step-label-content .mud-typography-body2")[0].TextContent.Trimmed().Should().Be("B");
            stepper.FindAll(".mud-step-label-content .mud-typography-caption")[1].TextContent.Trimmed().Should().Be("b");
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
            stepper.FindAll(".mud-step")[1].Click();
            stepper.Find(".mud-stepper-content").GetAttribute("class").Should().Contain("step-b");
            stepper.Find(".mud-stepper-content").GetAttribute("style").Should().Contain("fontsize:12px");
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 2");
            stepper.FindAll(".mud-step")[0].Click();
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
            stepper.Find(".mud-stepper-button-previous").IsDisabled().Should().Be(true); // previous
            stepper.Find(".mud-stepper-button-next").IsDisabled().Should().Be(false); // next
            stepper.FindAll(".mud-stepper-button-complete").Count.Should().Be(0); // can't complete yet
            // step 1 icon should be "1", step 2 icon should be "2"
            stepper.FindAll(".mud-step-label-icon")[0].TextContent.Trimmed().Should().Be("1");
            stepper.FindAll(".mud-step-label-icon")[1].TextContent.Trimmed().Should().Be("2");
            stepper.Find(".mud-stepper-button-next").Click(); // next
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 2");
            stepper.Find(".mud-stepper-button-previous").IsDisabled().Should().Be(false); // previous
            stepper.FindAll(".mud-stepper-button-next").Count.Should().Be(0); // no next button when completable
            stepper.FindAll(".mud-stepper-button-complete").Count.Should().Be(1);
            // step 1 icon should be a check mark, step 2 icon should be "2"
            stepper.FindAll(".mud-step-label-icon")[0].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
            stepper.FindAll(".mud-step-label-icon")[1].TextContent.Trimmed().Should().Be("2");
            stepper.Find(".mud-stepper-button-previous").Click(); // prev
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 1");
            stepper.Find(".mud-stepper-button-previous").IsDisabled().Should().Be(true); // previous
            stepper.Find(".mud-stepper-button-next").IsDisabled().Should().Be(false); // next
            stepper.FindAll(".mud-stepper-button-complete").Count.Should().Be(0); // can't complete yet
            // step 1 icon should be a check mark, step 2 icon should be "2"
            stepper.FindAll(".mud-step-label-icon")[0].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
            stepper.FindAll(".mud-step-label-icon")[1].TextContent.Trimmed().Should().Be("2");
            stepper.Find(".mud-stepper-button-next").Click(); // next
            stepper.FindAll(".mud-stepper-button-next").Count.Should().Be(0); // no next button when completable
            stepper.FindAll(".mud-stepper-button-complete").Count.Should().Be(1); // can complete now
            stepper.Find(".mud-stepper-button-complete").Click(); // next
            stepper.FindAll(".mud-stepper-button-next").Count.Should().Be(0); // all done, no need to show the next button
            stepper.FindAll(".mud-stepper-button-complete").Count.Should().Be(0); // all done, no need to show the complete button
            // step 1 and 2 icon should be check marks
            stepper.FindAll(".mud-step-label-icon")[0].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
            stepper.FindAll(".mud-step-label-icon")[1].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
        }

        [Test]
        public void Stepper_ShouldBeginWithFirstEnabledStep()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.AddChildContent<MudStep>(step => step.Add(x => x.Disabled, true));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Disabled, true));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Disabled, false));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Disabled, false));
            });
            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(2);
        }

        [Test]
        public void PreviousAndNext_ShouldCorrectlyHandleDisabledSteps()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.NonLinear, true);
                self.AddChildContent<MudStep>(step => step.Add(x => x.Disabled, true));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Disabled, false));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Disabled, true));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Disabled, false));
            });

            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(1);
            stepper.Find(".mud-stepper-button-next").IsDisabled().Should().Be(false);
            stepper.Find(".mud-stepper-button-next").Click(); // next

            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(3);
            stepper.Find(".mud-stepper-button-next").IsDisabled().Should().Be(true);
            stepper.Find(".mud-stepper-button-previous").IsDisabled().Should().Be(false);
            stepper.Find(".mud-stepper-button-previous").Click(); // prev

            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(1);
            stepper.Find(".mud-stepper-button-previous").IsDisabled().Should().Be(true);
            stepper.Find(".mud-stepper-button-previous").Click(); // prev
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
            stepper.Find(".mud-stepper-button-previous").IsDisabled().Should().Be(true); // previous
            stepper.Find(".mud-stepper-button-skip").IsDisabled().Should().Be(false); // skippable step
            stepper.Find(".mud-stepper-button-next").IsDisabled().Should().Be(false); // next
            stepper.FindAll(".mud-stepper-button-complete").Count.Should().Be(0); // can't complete yet
            stepper.Find(".mud-stepper-button-skip").Click(); // skip
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 2");
            stepper.Find(".mud-stepper-button-previous").IsDisabled().Should().Be(false); // previous
            stepper.FindAll(".mud-stepper-button-skip").Count.Should().Be(0); // non-skippable step
            stepper.FindAll(".mud-stepper-button-next").Count.Should().Be(0); // no next button on last step
            stepper.FindAll(".mud-stepper-button-complete").Count.Should().Be(1); // next
            stepper.Find(".mud-stepper-button-previous").Click(); // prev
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 1");
            stepper.Find(".mud-stepper-button-previous").IsDisabled().Should().Be(true); // previous
            stepper.Find(".mud-stepper-button-skip").IsDisabled().Should().Be(false); // skippable step
            stepper.Find(".mud-stepper-button-next").IsDisabled().Should().Be(false); // next
            stepper.FindAll(".mud-stepper-button-complete").Count.Should().Be(0); // can't complete yet, because step 2 is not Completed yet
        }

        [Test]
        public void ActiveIndex_ShouldBeTwoWayBindable()
        {
            var comp = Context.RenderComponent<StepperTwoWayBindingTestComponent>();
            var stepper1 = comp.FindComponents<MudStepper>()[0];
            var stepper2 = comp.FindComponents<MudStepper>()[1];
            stepper1.Instance.ActiveStep?.Title.Should().Be("A");
            stepper2.Instance.ActiveStep?.Title.Should().Be("X");
            stepper1.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("Step A");
            stepper2.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("Step X");
            // next
            stepper1.Find(".mud-stepper-button-next").Click();
            stepper1.Instance.ActiveStep?.Title.Should().Be("B");
            stepper2.Instance.ActiveStep?.Title.Should().Be("Y");
            stepper1.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("Step B");
            stepper2.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("Step Y");
            // next
            stepper1.Find(".mud-stepper-button-next").Click();
            stepper1.Instance.ActiveStep?.Title.Should().Be("C");
            stepper2.Instance.ActiveStep?.Title.Should().Be("Z");
            stepper1.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("Step C");
            stepper2.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("Step Z");
            // prev
            stepper1.Find(".mud-stepper-button-previous").Click();
            stepper1.Instance.ActiveStep?.Title.Should().Be("B");
            stepper2.Instance.ActiveStep?.Title.Should().Be("Y");
            stepper1.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("Step B");
            stepper2.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("Step Y");
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

            // this doesn't work on CI?
            // check render count. first render is just setup, second render renders the active step 
            // stepper.WaitForAssertion(() => stepper.RenderCount.Should().Be(2));

            // disable step 1
            stepper.FindAll(".mud-step")[0].IsDisabled().Should().Be(false);
            await stepper.InvokeAsync(async () => await stepper.Instance.Steps[0].SetDisabledAsync(true));
            stepper.FindAll(".mud-step")[0].IsDisabled().Should().Be(true);
            // fail step 2
            stepper.FindAll(".mud-step")[1].ClassList.Should().NotContain("mud-step-error");
            stepper.FindAll(".mud-step-label-icon")[1].ClassList.Should().NotContain("mud-error");
            stepper.FindAll(".mud-step-label-content")[1].ClassList.Should().NotContain("mud-error-text");
            await stepper.InvokeAsync(async () => await stepper.Instance.Steps[1].SetHasErrorAsync(true));
            stepper.FindAll(".mud-step")[1].ClassList.Should().Contain("mud-step-error");
            stepper.FindAll(".mud-step-label-icon")[1].ClassList.Should().Contain("mud-error");
            stepper.FindAll(".mud-step-label-content")[1].ClassList.Should().Contain("mud-error-text");
            // complete step 3
            stepper.FindAll(".mud-step")[2].ClassList.Should().NotContain("mud-step-completed");
            stepper.FindAll(".mud-step-label-icon")[2].QuerySelectorAll("path").Should().BeEmpty(); // no svg icon if not completed
            await stepper.InvokeAsync(async () => await stepper.Instance.Steps[2].SetCompletedAsync(true));
            stepper.FindAll(".mud-step-label-icon")[2].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
            stepper.FindAll(".mud-step")[2].ClassList.Should().Contain("mud-step-completed");
        }

        [Test]
        public void FirstStep_ShouldBeActiveIfActiveIndexNotSet()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.AddChildContent<MudStep>(step => step.Add(x => x.Title, "A"));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Title, "B"));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Title, "C"));
            });
            stepper.WaitForAssertion(() => stepper.Instance.ActiveStep?.Title.Should().Be("A"));
            stepper.FindAll(".mud-step-label-icon")[0].TextContent.Trimmed().Should().Be("1");
            stepper.FindAll(".mud-step-label-icon")[1].TextContent.Trimmed().Should().Be("2");
            stepper.FindAll(".mud-step-label-icon")[2].TextContent.Trimmed().Should().Be("3");
        }

        [Test]
        public void InitialActiveIndex_ShouldBeRespectedIfSet()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.ActiveIndex, 1);
                self.AddChildContent<MudStep>(step => step.Add(x => x.Title, "A"));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Title, "B"));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Title, "C"));
            });
            stepper.WaitForAssertion(() => stepper.Instance.ActiveStep?.Title.Should().Be("B"));
        }

        [Test]
        public async Task RemoveStep_ShouldUpdateActiveIndex()
        {
            int activeIndex = 2;
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Bind(x => x.ActiveIndex, activeIndex, newValue => activeIndex = newValue);
                self.AddChildContent<MudStep>(step => step.Add(x => x.Title, "A"));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Title, "B"));
                self.AddChildContent<MudStep>(step => step.Add(x => x.Title, "C"));
            });
            stepper.WaitForAssertion(() => stepper.Instance.ActiveStep?.Title.Should().Be("C"));
            activeIndex.Should().Be(2);
            // remove active step C, stepper should fall back to B  
            await stepper.InvokeAsync(async () => await stepper.Instance.RemoveStepAsync(stepper.Instance.ActiveStep!));
            stepper.Instance.ActiveStep?.Title.Should().Be("B");
            activeIndex.Should().Be(1);
            // remove step A, stepper should remain on B, active index should fall back to 0  
            await stepper.InvokeAsync(async () => await stepper.Instance.RemoveStepAsync(stepper.Instance.Steps[0]));
            stepper.Instance.ActiveStep?.Title.Should().Be("B");
            activeIndex.Should().Be(0);
            // remove active step B, stepper has no more steps, active index should be -1, active step should be null  
            await stepper.InvokeAsync(async () => await stepper.Instance.RemoveStepAsync(stepper.Instance.ActiveStep!));
            stepper.Instance.ActiveStep.Should().BeNull();
            activeIndex.Should().Be(-1);
        }

        [Test]
        public async Task AddStep_ShouldUpdateActiveIndexAndStep()
        {
            int activeIndex = -1;
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Bind(x => x.ActiveIndex, activeIndex, newValue => activeIndex = newValue);
            });
            stepper.WaitForAssertion(() => stepper.RenderCount.Should().Be(1));
            activeIndex.Should().Be(-1);
            // adding a step changes active index to 0
#pragma warning disable BL0005
            var step = new MudStep() { Title = "X" };
            step.IsActive.Should().Be(false); // <-- fight partial line coverage
            await stepper.InvokeAsync(async () => await stepper.Instance.AddStepAsync(step));
#pragma warning restore BL0005
            activeIndex.Should().Be(0);
            stepper.Instance.ActiveStep?.Title.Should().Be("X");
            // adding another step won't change active index
#pragma warning disable BL0005
            await stepper.InvokeAsync(async () => await stepper.Instance.AddStepAsync(new MudStep() { Title = "Y" }));
#pragma warning restore BL0005
            activeIndex.Should().Be(0);
            stepper.Instance.ActiveStep?.Title.Should().Be("X");
        }

        [Test]
        public async Task Stepper_ShouldNavigateViaProgrammaticApi()
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
            stepper.Find(".mud-stepper-button-previous").IsDisabled().Should().Be(true); // previous
            stepper.Find(".mud-stepper-button-next").IsDisabled().Should().Be(false); // next
            stepper.FindAll(".mud-stepper-button-complete").Count.Should().Be(0);
            // step 1 icon should be "1", step 2 icon should be "2"
            stepper.FindAll(".mud-step-label-icon")[0].TextContent.Trimmed().Should().Be("1");
            stepper.FindAll(".mud-step-label-icon")[1].TextContent.Trimmed().Should().Be("2");
            await stepper.InvokeAsync(async () => await stepper.Instance.NextStepAsync()); // next
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 2");
            stepper.Find(".mud-stepper-button-previous").IsDisabled().Should().Be(false); // previous
            stepper.FindAll(".mud-stepper-button-next").Count.Should().Be(0);
            stepper.FindAll(".mud-stepper-button-complete").Count.Should().Be(1);
            // step 1 icon should be a check mark, step 2 icon should be "2"
            stepper.FindAll(".mud-step-label-icon")[0].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
            stepper.FindAll(".mud-step-label-icon")[1].TextContent.Trimmed().Should().Be("2");
            await stepper.InvokeAsync(async () => await stepper.Instance.PreviousStepAsync());  // prev
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 1");
            stepper.Find(".mud-stepper-button-previous").IsDisabled().Should().Be(true); // previous
            stepper.Find(".mud-stepper-button-next").IsDisabled().Should().Be(false); // next
            stepper.FindAll(".mud-stepper-button-complete").Count.Should().Be(0);
            // step 1 icon should be a check mark, step 2 icon should be "2"
            stepper.FindAll(".mud-step-label-icon")[0].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
            stepper.FindAll(".mud-step-label-icon")[1].TextContent.Trimmed().Should().Be("2");
            await stepper.InvokeAsync(async () => await stepper.Instance.NextStepAsync()); // next
            await stepper.InvokeAsync(async () => await stepper.Instance.NextStepAsync()); // complete also uses the next api!
            // step 1 and 2 icon should be check marks
            stepper.FindAll(".mud-step-label-icon")[0].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
            stepper.FindAll(".mud-step-label-icon")[1].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
        }

        [Test]
        public async Task CompletedContent_ShouldShowUpIfAllStepsAreComplete_Horizontal()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.CompletedContent, markupFactory => markupFactory.AddMarkupContent(0, "voilà"));
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
            stepper.FindAll(".mud-stepper-complete").FirstOrDefault()?.TextContent.Trimmed().Should().BeNullOrEmpty();
            await stepper.InvokeAsync(async () => await stepper.Instance.NextStepAsync()); // next
            stepper.Find(".mud-stepper-content").TextContent.Trimmed().Should().Contain("step 2");
            stepper.FindAll(".mud-stepper-complete").FirstOrDefault()?.TextContent.Trimmed().Should().BeNullOrEmpty();
            await stepper.InvokeAsync(async () => await stepper.Instance.NextStepAsync()); // next
            // completed content
            stepper.Find(".mud-stepper-complete").TextContent.Trimmed().Should().Contain("voilà");
        }

        [Test]
        public async Task CompletedContent_ShouldShowUpIfAllStepsAreComplete_Vertical()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.Vertical, true);
                self.Add(x => x.CompletedContent, markupFactory => markupFactory.AddMarkupContent(0, "voilà"));
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
            stepper.FindAll(".mud-stepper-complete").FirstOrDefault()?.TextContent.Trimmed().Should().BeNullOrEmpty();
            await stepper.InvokeAsync(async () => await stepper.Instance.NextStepAsync()); // next
            stepper.FindAll(".mud-stepper-complete").FirstOrDefault()?.TextContent.Trimmed().Should().BeNullOrEmpty();
            await stepper.InvokeAsync(async () => await stepper.Instance.NextStepAsync()); // next
            // completed content
            stepper.Find(".mud-stepper-complete").TextContent.Trimmed().Should().Contain("voilà");
        }

        [Test]
        public void UpdatingStepProperties_ShouldUpdateStepper()
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
            stepper.FindAll(".mud-step")[0].IsDisabled().Should().Be(false);
            stepper.FindComponents<MudStep>()[0].SetParametersAndRender(Parameter(nameof(MudStep.Disabled), true));
            stepper.FindAll(".mud-step")[0].IsDisabled().Should().Be(true);
            // fail step 2
            stepper.FindAll(".mud-step")[1].ClassList.Should().NotContain("mud-step-error");
            stepper.FindAll(".mud-step-label-icon")[1].ClassList.Should().NotContain("mud-error");
            stepper.FindAll(".mud-step-label-content")[1].ClassList.Should().NotContain("mud-error-text");
            stepper.FindComponents<MudStep>()[1].SetParametersAndRender(Parameter(nameof(MudStep.HasError), true));
            stepper.FindAll(".mud-step")[1].ClassList.Should().Contain("mud-step-error");
            stepper.FindAll(".mud-step-label-icon")[1].ClassList.Should().Contain("mud-error");
            stepper.FindAll(".mud-step-label-content")[1].ClassList.Should().Contain("mud-error-text");
            // complete step 3
            stepper.FindAll(".mud-step")[2].ClassList.Should().NotContain("mud-step-completed");
            stepper.FindAll(".mud-step-label-icon")[2].QuerySelectorAll("path").Should().BeEmpty(); // no svg icon if not completed
            stepper.FindComponents<MudStep>()[2].SetParametersAndRender(Parameter(nameof(MudStep.Completed), true));
            stepper.FindAll(".mud-step-label-icon")[2].QuerySelectorAll("path").Last().GetAttribute("d").Should().Be("M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4L9 16.2z");
            stepper.FindAll(".mud-step")[2].ClassList.Should().Contain("mud-step-completed");
        }

        [Test]
        public void StepOnClick_ShouldFireForNonLinearStepper()
        {
            int aClick = 0;
            int bClick = 0;
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.NonLinear, true);
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "A");
                    step.Add(x => x.OnClick, () => aClick++);
                });
                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "B");
                    step.Add(x => x.OnClick, () => bClick++);
                });
            });
            stepper.FindAll(".mud-step")[0].Click();
            aClick.Should().Be(1);
            bClick.Should().Be(0);
            stepper.FindAll(".mud-step")[1].Click();
            aClick.Should().Be(1);
            bClick.Should().Be(1);
        }

        [Test]
        public void ActionContentTemplate_ShouldReplaceTheNavButtons()
        {
            var stepper = Context.RenderComponent<MudStepper>();
            stepper.FindAll(".mud-card-actions .mud-button").Count.Should().Be(2, "because no action content defined"); // prev, next
            stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.Tag, "je ne sais pas");
                // this replaces the action buttons prev, skip and next with just text
                self.Add(x => x.ActionContent, stepperRef => (string)stepperRef.Tag); // <-- here we simulate using the passed in stepper context
            });
            stepper.FindAll(".mud-card-actions .mud-button").Count.Should().Be(0, "because the action content replaces them");
            stepper.Find(".mud-card-actions").InnerHtml?.Trim().Should().Be("je ne sais pas");
        }

        [Test]
        public void ShowReset_ShouldControlResetButtonVisibilty()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.ShowResetButton, true);
            });
            stepper.FindAll(".mud-card-actions .mud-button").Count.Should().Be(3); // reset, previous, next
            stepper.FindAll(".mud-card-actions .mud-stepper-button-reset").Count.Should().Be(1);
            stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.ShowResetButton, false);
            });
            stepper.FindAll(".mud-card-actions .mud-button").Count.Should().Be(2); // previous, next
            stepper.FindAll(".mud-card-actions .mud-stepper-button-reset").Count.Should().Be(0);
        }

        [Test]
        public void ResetButton_ShouldResetActiveStep()
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.ShowResetButton, true);
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
            stepper.Instance.GetState(x => x.ActiveIndex).Should().Be(0);
            stepper.Instance.ActiveStep?.Title.Should().Be("A");
            stepper.Instance.Steps[0].GetState(x => x.Completed).Should().Be(false);
            stepper.InvokeAsync(async () => await stepper.Instance.NextStepAsync());
            stepper.Instance.ActiveStep?.Title.Should().Be("B");
            stepper.Instance.Steps[0].GetState(x => x.Completed).Should().Be(true);
            stepper.Instance.GetState(x => x.ActiveIndex).Should().Be(1);
            stepper.Find(".mud-stepper-button-reset").Click();
            stepper.Instance.ActiveStep?.Title.Should().Be("A");
            stepper.Instance.Steps[0].GetState(x => x.Completed).Should().Be(false);
            stepper.Instance.GetState(x => x.ActiveIndex).Should().Be(0);
        }


        [Test]
        public void Stepper_ControlledNavigationTest()
        {
            var cancel = false;
            var action = StepAction.Reset;
            var index = -1;
            Task OnPreviewInteraction(StepperInteractionEventArgs args)
            {
                action = args.Action;
                index = args.StepIndex;
                // ReSharper disable once AccessToModifiedClosure
                args.Cancel = cancel;
                return Task.CompletedTask;
            }
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.OnPreviewInteraction, OnPreviewInteraction);
                self.Add(x => x.ShowResetButton, true);
                self.Add(x => x.NonLinear, true);
                self.AddChildContent<MudStep>(step => { });
                self.AddChildContent<MudStep>(step => { });
                self.AddChildContent<MudStep>(step => { });
            });

            // check that clicking next sends Complete action requests
            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(0);
            stepper.Find(".mud-stepper-button-next").Click();
            index.Should().Be(0);
            action.Should().Be(StepAction.Complete);
            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(1);
            stepper.Find(".mud-stepper-button-next").Click();
            index.Should().Be(1);
            action.Should().Be(StepAction.Complete);
            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(2);
            stepper.Find(".mud-stepper-button-complete").Click();
            index.Should().Be(2);
            action.Should().Be(StepAction.Complete);
            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(2);

            // cancel next
            stepper.Find(".mud-stepper-button-reset").Click();
            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(0);
            cancel = true; // this should cancel the completion of step 1
            stepper.Find(".mud-stepper-button-next").Click();
            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(0);

            // cancel previous
            cancel = false;
            stepper.Find(".mud-stepper-button-next").Click(); // go to step2
            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(1);
            cancel = true; // this should cancel the activation of step 1
            stepper.Find(".mud-stepper-button-previous").Click(); // try to go to step1
            index.Should().Be(0);
            action.Should().Be(StepAction.Activate);
            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(1);

            // cancel jumping via header click
            cancel = false;
            stepper.Find(".mud-stepper-button-reset").Click();
            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(0);
            cancel = true; // this should cancel the activation of step 3
            stepper.FindAll(".mud-step")[2].Click(); // try to go to step3
            index.Should().Be(2);
            action.Should().Be(StepAction.Activate);
            stepper.Instance.GetState<int>(nameof(MudStepper.ActiveIndex)).Should().Be(0);

        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void HasRippleClass(bool ripple, bool hasClass)
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.Ripple, ripple);

                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "A");
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 1"));
                });
            });

            var stepButton = stepper.Find(".mud-step");

            if (hasClass)
            {
                stepButton.ClassList.Should().Contain("mud-ripple");
            }
            else
            {
                stepButton.ClassList.Should().NotContain("mud-ripple");
            }
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void HasClickableClassIfNonLinear(bool nonLinear, bool hasClass)
        {
            var stepper = Context.RenderComponent<MudStepper>(self =>
            {
                self.Add(x => x.NonLinear, nonLinear);

                self.AddChildContent<MudStep>(step =>
                {
                    step.Add(x => x.Title, "A");
                    step.AddChildContent(text => text.AddMarkupContent(0, "step 1"));
                });
            });

            var stepButton = stepper.Find(".mud-step");

            if (hasClass)
            {
                stepButton.ClassList.Should().Contain("mud-clickable");
            }
            else
            {
                stepButton.ClassList.Should().NotContain("mud-clickable");
            }
        }
    }
}
