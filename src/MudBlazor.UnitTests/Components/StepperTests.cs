using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Css.Dom;
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
        public void StepperStepsTest()
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
            stepper.Instance.Steps.All(x => x.Parent==stepper.Instance).Should().Be(true);
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
    }
}



