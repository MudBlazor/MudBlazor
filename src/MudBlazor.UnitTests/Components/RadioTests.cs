
using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Docs.Examples;
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
                Assert.AreEqual(ex.InnerException.GetType(), typeof(MudBlazor.Utilities.Exceptions.GenericTypeMismatchException));
            }
        }

        /// <summary>
        /// Tests the Disabled property of the MudRadio
        /// </summary>
        [Test]
        public void RadioDisabledTest()
        {
            var comp = Context.RenderComponent<RadioGroupExample>();
            comp.Instance.SelectedOption.Should().BeNull();

            var inputs = comp.FindAll("input");
            inputs[2].Click(); //click enabled radio
            comp.Instance.SelectedOption.Should().Be("Radio 3");
            inputs[3].Click(); //click disable radio
            comp.Instance.SelectedOption.Should().Be("Radio 3");

            var labels = comp.FindAll("label");
            labels[3].ClassList.Contains("mud-disabled").Should().BeTrue();
        }

        /// <summary>
        /// Tests the Disabled property of the MudRadioGroup
        /// </summary>
        [Test]
        public void RadioGroupDisabledTest()
        {
            var comp = Context.RenderComponent<RadioReadOnlyDisabledExample>();
            var radioGroup = comp.FindComponents<MudRadioGroup<string>>()[1];

            var radios = radioGroup.FindComponents<MudRadio<string>>();
            radios.Count.Should().Be(4);
            radioGroup.FindAll(".mud-radio.mud-disabled").Count.Should().Be(0);

            comp.FindAll(".mud-switch-button > input")[1].Change(true);
            radioGroup.FindAll(".mud-radio.mud-disabled").Count.Should().Be(4);
        }

        /// <summary>
        /// Tests the Readonly property of the MudRadioGroup
        /// </summary>
        [Test]
        public async Task RadioGroupReadOnlyTest()
        {
            var comp = Context.RenderComponent<RadioReadOnlyDisabledExample>();
            var radioGroup = comp.FindComponents<MudRadioGroup<string>>()[0];

            var radios = radioGroup.FindComponents<MudRadio<string>>();
            radios.Count.Should().Be(4);
            radioGroup.FindAll(".mud-radio.mud-readonly").Count.Should().Be(0);

            comp.FindAll(".mud-switch-button > input")[0].Change(true);
            radioGroup.FindAll(".mud-radio.mud-readonly").Count.Should().Be(4);
        }
    }
}
