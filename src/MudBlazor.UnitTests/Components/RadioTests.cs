
using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class RadioTests : BunitTest
    {
        [Test]
        public void RadiGroup_CheckClassTest()
        {
            var comp = Context.RenderComponent<RadioGroupTest1>();

            var inputControl = comp.FindComponent<MudInputControl>();
            inputControl.Instance.InputContent.Should().NotBeNull();

            comp.FindAll("div.mud-radio-group").Should().ContainSingle();
            comp.FindAll("div.some-main-class").Should().ContainSingle();
            comp.FindAll("div.some-input-class").Should().ContainSingle();
            comp.FindAll(".some-main-class .some-input-class").Should().ContainSingle();
            comp.FindAll(".mud-radio").Count.Should().Be(3);
            // Input content should not have main class (Classname), but should have input class (InputClass)
            comp.FindAll(".mud-radio-group.some-main-class").Should().BeEmpty();
            comp.FindAll(".mud-radio-group.some-input-class").Should().ContainSingle();
        }

        [Test]
        public void RadioGroupTest1()
        {
            var comp = Context.RenderComponent<RadioGroupTest1>();
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup<string>>();
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
            var comp = Context.RenderComponent<RadioGroupTest2>();
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup<string>>();
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
            var comp = Context.RenderComponent<RadioGroupTest3>();
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var groups = comp.FindComponents<MudRadioGroup<string>>();
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
            var comp = Context.RenderComponent<RadioGroupTest4>();
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var groups = comp.FindComponents<MudRadioGroup<string>>();
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

        [Test]
        public void RadioGroupTest5()
        {
            var comp = Context.RenderComponent<RadioGroupTest5>();
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup<string>>();
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
            // click reset button
            comp.Find("button").Click();
            group.Instance.SelectedOption.Should().Be(null);
            spans = comp.FindAll("span.mud-radio-icons").ToArray();
            spans[0].ClassList.Should().NotContain("mud-checked");
            spans[1].ClassList.Should().NotContain("mud-checked");
            spans[2].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public void RadioGroupTest6()
        {
            var comp = Context.RenderComponent<RadioGroupTest6>();
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup<string>>();
            var buttons = comp.FindAll("label > span").ToArray();
            var svgs = comp.FindAll("svg").ToArray();
            // check dense
            buttons[0].ClassList.Should().Contain("mud-radio-dense");
            buttons[1].ClassList.Should().NotContain("mud-radio-dense");
            buttons[2].ClassList.Should().NotContain("mud-radio-dense");
            buttons[3].ClassList.Should().NotContain("mud-radio-dense");
            // check size (two svgs per radio)
            svgs[0].ClassList.Should().Contain("mud-icon-size-medium");
            svgs[1].ClassList.Should().Contain("mud-icon-size-medium");
            svgs[2].ClassList.Should().Contain("mud-icon-size-small");
            svgs[3].ClassList.Should().Contain("mud-icon-size-small");
            svgs[4].ClassList.Should().Contain("mud-icon-size-medium");
            svgs[5].ClassList.Should().Contain("mud-icon-size-medium");
            svgs[6].ClassList.Should().Contain("mud-icon-size-large");
            svgs[7].ClassList.Should().Contain("mud-icon-size-large");
        }

        [Test]
        public void RadioTest_KeyboardInput()
        {
            var comp = Context.RenderComponent<RadioGroupTest1>();
            // print the generated html
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var radio = comp.FindComponent<MudRadioGroup<string>>();
            radio.Instance.SelectedOption.Should().Be(null);

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            comp.WaitForAssertion(() => radio.Instance.SelectedOption.Should().Be("1"));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            comp.WaitForAssertion(() => radio.Instance.SelectedOption.Should().Be(null));

            //Can't tabbed around the radios in test.
        }

        [Test]
        public async Task RadioTest_Other()
        {
            var comp = Context.RenderComponent<RadioGroupTest1>();
            var group = comp.FindComponent<MudRadioGroup<string>>();
            var radio = comp.FindComponent<MudRadio<string>>();

            await comp.InvokeAsync(() => radio.Instance.IMudRadioGroup = null);
            await comp.InvokeAsync(() => radio.Instance.OnClick());
#pragma warning disable BL0005
            await comp.InvokeAsync(() => radio.Instance.Disabled = true);
            comp.WaitForAssertion(() => group.Instance.SelectedOption.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            comp.WaitForAssertion(() => group.Instance.SelectedOption.Should().Be(null));
        }


        [Test]
        public void RadioTest_TypeException()
        {
            try
            {
                var comp = Context.RenderComponent<RadioGroupExceptionTest>();
            }
            catch (Exception ex)
            {
                ex.Message.Should().Be("Unable to set property 'IMudRadioGroup' on object of type 'MudBlazor.MudRadio`1[[System.String, System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]'. The error was: MudRadioGroup<Char> has a child MudRadio<System.String> with mismatching generic type.");
            }
        }
    }
}
