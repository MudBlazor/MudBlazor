#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System;
using System.Linq;
using Bunit;
using FluentAssertions;
using NUnit.Framework;


namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class RadioTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public void RadioGroupTest1()
        {
            var comp = ctx.RenderComponent<RadioGroupTest1>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup>();
            var inputs = comp.FindAll("input").ToArray();
            var spans = comp.FindAll("span.mud-radio-icons").ToArray();
            // check initial state
            group.Instance.SelectedOption.Should().Be(null);
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
            // click radio 1
            inputs[0].Click();
            group.Instance.SelectedOption.Should().Be("1");
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            spans[0].ClassList.Should().Contain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
            // click radio 2
            inputs[1].Click();
            group.Instance.SelectedOption.Should().Be("2");
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().Contain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
            // click radio 3
            inputs[2].Click();
            group.Instance.SelectedOption.Should().Be("3");
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().Contain("mud-checked");
            // click radio 1
            inputs[0].Click();
            group.Instance.SelectedOption.Should().Be("1");
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            spans[0].ClassList.Should().Contain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public void RadioGroupTest2()
        {
            var comp = ctx.RenderComponent<RadioGroupTest2>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup>();
            var spans = comp.FindAll("span.mud-radio-icons").ToArray();
            // check initial state, should be initialized to second radio by default
            group.Instance.SelectedOption.Should().Be("2");
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().Contain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public void RadioGroupTest3()
        {
            var comp = ctx.RenderComponent<RadioGroupTest3>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var groups = comp.FindComponents<MudRadioGroup>();
            var inputs = comp.FindAll("input").ToArray();
            var spans = comp.FindAll("span.mud-radio-icons").ToArray();
            // check initial state, should be initialized to second radio by default for both groups
            groups[0].Instance.SelectedOption.Should().Be("2");
            groups[1].Instance.SelectedOption.Should().Be("2");
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().Contain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
            spans[3].ClassList.Should().Contain("mud-checked");
            // click first radio of second group - they should both switch to L1
            inputs[2].Click();
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            groups[0].Instance.SelectedOption.Should().Be("1");
            groups[1].Instance.SelectedOption.Should().Be("1");
            spans[0].ClassList.Should().Contain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().Contain("mud-checked");
            spans[3].ClassList.Should().NotContain("mud-checked");
            // click second radio of first group - they should both switch to L1
            inputs[1].Click();
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            groups[0].Instance.SelectedOption.Should().Be("2");
            groups[1].Instance.SelectedOption.Should().Be("2");
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().Contain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
            spans[3].ClassList.Should().Contain("mud-checked");
        }

        [Test]
        public void RadioGroupTest4()
        {
            var comp = ctx.RenderComponent<RadioGroupTest4>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var groups = comp.FindComponents<MudRadioGroup>();
            var inputs = comp.FindAll("input").ToArray();
            var spans = comp.FindAll("span.mud-radio-icons").ToArray();
            // check initial state, should be uninitialized
            groups[0].Instance.SelectedOption.Should().Be(null);
            groups[1].Instance.SelectedOption.Should().Be(null);
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
            spans[3].ClassList.Should().NotContain("mud-checked");
            // click first radio of second group - only second group should switch to L1
            inputs[2].Click();
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            groups[0].Instance.SelectedOption.Should().Be(null);
            groups[1].Instance.SelectedOption.Should().Be("x");
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().Contain("mud-checked");
            spans[3].ClassList.Should().NotContain("mud-checked");
            // click second radio of first group - only first group should switch to L1
            inputs[1].Click();
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            groups[0].Instance.SelectedOption.Should().Be("2");
            groups[1].Instance.SelectedOption.Should().Be("x");
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().Contain("mud-checked");
            spans[2].ClassList.Should().Contain("mud-checked");
            spans[3].ClassList.Should().NotContain("mud-checked");
        }
    }
}
