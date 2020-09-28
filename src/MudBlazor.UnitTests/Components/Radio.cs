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
        public void RadioGroup1_Test() {
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
    }
}
