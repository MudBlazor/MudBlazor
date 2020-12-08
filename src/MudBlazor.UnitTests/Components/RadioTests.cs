using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bunit;
using FluentAssertions;
using NUnit.Framework;


namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class RadioTests
    {
        [Test]
        public void RadioGroupTest1() {
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<RadioGroupTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup>();
            var inputs =comp.FindAll("input").ToArray();
            var spans = comp.FindAll("span.mud-radio-icons").ToArray();
            // check initial state
            group.Instance.SelectedOption.Should().Be(null);
            group.Instance.SelectedLabel.Should().Be(null);
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
            // click radio 1
            inputs[0].Change("on");
            group.Instance.SelectedOption.Should().Be("1");
            group.Instance.SelectedLabel.Should().Be("L1");
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            spans[0].ClassList.Should().Contain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
            // click radio 2
            inputs[1].Change("on");
            group.Instance.SelectedOption.Should().Be("2");
            group.Instance.SelectedLabel.Should().Be("L2");
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().Contain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
            // click radio 3
            inputs[2].Change("on");
            group.Instance.SelectedOption.Should().Be("3");
            group.Instance.SelectedLabel.Should().Be("L3");
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().Contain("mud-checked");
            // click radio 1
            inputs[0].Change("on");
            group.Instance.SelectedOption.Should().Be("1");
            group.Instance.SelectedLabel.Should().Be("L1");
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            spans[0].ClassList.Should().Contain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public void RadioGroupTest2()
        {
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<RadioGroupTest2>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup>();
            var inputs = comp.FindAll("input").ToArray();
            var spans = comp.FindAll("span.mud-radio-icons").ToArray();
            // check initial state, should be initialized to second radio by default
            group.Instance.SelectedOption.Should().Be("2");
            group.Instance.SelectedLabel.Should().Be("L2");
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().Contain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public void RadioGroupTest3()
        {
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<RadioGroupTest3>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var groups = comp.FindComponents<MudRadioGroup>();
            var inputs = comp.FindAll("input").ToArray();
            var spans = comp.FindAll("span.mud-radio-icons").ToArray();
            // check initial state, should be initialized to second radio by default for both groups
            groups[0].Instance.SelectedOption.Should().Be("2");
            groups[0].Instance.SelectedLabel.Should().Be("L2");
            groups[1].Instance.SelectedOption.Should().Be("y");
            groups[1].Instance.SelectedLabel.Should().Be("L2");
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().Contain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
            spans[3].ClassList.Should().Contain("mud-checked");
            // click first radio of second group - they should both switch to L1
            inputs[2].Change("on");
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            groups[0].Instance.SelectedOption.Should().Be("1");
            groups[0].Instance.SelectedLabel.Should().Be("L1");
            groups[1].Instance.SelectedOption.Should().Be("x");
            groups[1].Instance.SelectedLabel.Should().Be("L1");
            spans[0].ClassList.Should().Contain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().Contain("mud-checked");
            spans[3].ClassList.Should().NotContain("mud-checked");
            // click second radio of first group - they should both switch to L1
            inputs[1].Change("on");
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            groups[0].Instance.SelectedOption.Should().Be("2");
            groups[0].Instance.SelectedLabel.Should().Be("L2");
            groups[1].Instance.SelectedOption.Should().Be("y");
            groups[1].Instance.SelectedLabel.Should().Be("L2");
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().Contain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
            spans[3].ClassList.Should().Contain("mud-checked");
        }

        [Test]
        public void RadioGroupTest4()
        {
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<RadioGroupTest4>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var groups = comp.FindComponents<MudRadioGroup>();
            var inputs = comp.FindAll("input").ToArray();
            var spans = comp.FindAll("span.mud-radio-icons").ToArray();
            // check initial state, should be uninitialized
            groups[0].Instance.SelectedOption.Should().Be(null);
            groups[0].Instance.SelectedLabel.Should().Be(null);
            groups[1].Instance.SelectedOption.Should().Be(null);
            groups[1].Instance.SelectedLabel.Should().Be(null);
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
            spans[3].ClassList.Should().NotContain("mud-checked");
            // click first radio of second group - only second group should switch to L1
            inputs[2].Change("on");
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            groups[0].Instance.SelectedOption.Should().Be(null);
            groups[0].Instance.SelectedLabel.Should().Be(null);
            groups[1].Instance.SelectedOption.Should().Be("x");
            groups[1].Instance.SelectedLabel.Should().Be("L1");
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().Contain("mud-checked");
            spans[3].ClassList.Should().NotContain("mud-checked");
            // click second radio of first group - only first group should switch to L1
            inputs[1].Change("on");
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            groups[0].Instance.SelectedOption.Should().Be("2");
            groups[0].Instance.SelectedLabel.Should().Be("L2");
            groups[1].Instance.SelectedOption.Should().Be("x");
            groups[1].Instance.SelectedLabel.Should().Be("L1");
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().Contain("mud-checked");
            spans[2].ClassList.Should().Contain("mud-checked");
            spans[3].ClassList.Should().NotContain("mud-checked");
        }
    }
}
