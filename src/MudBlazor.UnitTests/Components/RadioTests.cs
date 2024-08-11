using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
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

            // check initial state
            group.Instance.Value.Should().Be(null);
            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().NotContain("mud-checked");
            // click radio 1
            inputs[0].Click();
            group.Instance.Value.Should().Be("1");

            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().Contain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().NotContain("mud-checked");
            // click radio 2
            inputs[1].Click();
            group.Instance.Value.Should().Be("2");

            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().Contain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().NotContain("mud-checked");
            // click radio 3
            inputs[2].Click();
            group.Instance.Value.Should().Be("3");

            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().Contain("mud-checked");
            // click radio 1
            inputs[0].Click();
            group.Instance.Value.Should().Be("1");

            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().Contain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public void RadioGroupTest2()
        {
            var comp = Context.RenderComponent<RadioGroupTest2>();
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup<string>>();
            // check initial state, should be initialized to second radio by default
            group.Instance.Value.Should().Be("2");
            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().Contain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public void RadioGroupTest3()
        {
            var comp = Context.RenderComponent<RadioGroupTest3>();
            // select elements needed for the test
            var groups = comp.FindComponents<MudRadioGroup<string>>();
            var inputs = comp.FindAll("input").ToArray();

            // check initial state, should be initialized to second radio by default for both groups
            groups[0].Instance.Value.Should().Be("2");
            groups[1].Instance.Value.Should().Be("2");
            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().Contain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[3].ClassList.Should().Contain("mud-checked");
            // click first radio of second group - they should both switch to L1
            inputs[2].Click();

            groups[0].Instance.Value.Should().Be("1");
            groups[1].Instance.Value.Should().Be("1");
            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().Contain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().Contain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[3].ClassList.Should().NotContain("mud-checked");
            // click second radio of first group - they should both switch to L1
            inputs[1].Click();

            groups[0].Instance.Value.Should().Be("2");
            groups[1].Instance.Value.Should().Be("2");
            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().Contain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[3].ClassList.Should().Contain("mud-checked");
        }

        [Test]
        public void RadioGroupTest4()
        {
            var comp = Context.RenderComponent<RadioGroupTest4>();
            // select elements needed for the test
            var groups = comp.FindComponents<MudRadioGroup<string>>();

            // check initial state, should be uninitialized
            groups[0].Instance.Value.Should().Be(null);
            groups[1].Instance.Value.Should().Be(null);
            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[3].ClassList.Should().NotContain("mud-checked");
            // click first radio of second group - only second group should switch to L1
            comp.FindAll("input")[2].Click();

            groups[0].Instance.Value.Should().Be(null);
            groups[1].Instance.Value.Should().Be("x");
            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().Contain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[3].ClassList.Should().NotContain("mud-checked");
            // click second radio of first group - only first group should switch to L1
            comp.FindAll("input")[1].Click();

            groups[0].Instance.Value.Should().Be("2");
            groups[1].Instance.Value.Should().Be("x");
            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().Contain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().Contain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[3].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public void RadioGroupTest5()
        {
            var comp = Context.RenderComponent<RadioGroupTest5>();
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup<string>>();
            // check initial state
            group.Instance.Value.Should().Be(null);
            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().NotContain("mud-checked");
            // click radio 1
            comp.FindAll("input")[0].Click();
            group.Instance.Value.Should().Be("1");

            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().Contain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().NotContain("mud-checked");
            // click reset button
            comp.Find("button").Click();
            group.Instance.Value.Should().Be(null);

            comp.FindAll("span.mud-radio-icons")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll("span.mud-radio-icons")[2].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public void RadioGroupTest6()
        {
            var comp = Context.RenderComponent<RadioGroupTest6>();
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup<string>>();
            // check dense
            comp.FindAll("label > span")[0].ClassList.Should().Contain("mud-radio-dense");
            comp.FindAll("label > span")[1].ClassList.Should().NotContain("mud-radio-dense");
            comp.FindAll("label > span")[2].ClassList.Should().NotContain("mud-radio-dense");
            comp.FindAll("label > span")[3].ClassList.Should().NotContain("mud-radio-dense");
            // check size (two comp.FindAll("svg") per radio)
            comp.FindAll("svg")[0].ClassList.Should().Contain("mud-icon-size-medium");
            comp.FindAll("svg")[1].ClassList.Should().Contain("mud-icon-size-medium");
            comp.FindAll("svg")[2].ClassList.Should().Contain("mud-icon-size-small");
            comp.FindAll("svg")[3].ClassList.Should().Contain("mud-icon-size-small");
            comp.FindAll("svg")[4].ClassList.Should().Contain("mud-icon-size-medium");
            comp.FindAll("svg")[5].ClassList.Should().Contain("mud-icon-size-medium");
            comp.FindAll("svg")[6].ClassList.Should().Contain("mud-icon-size-large");
            comp.FindAll("svg")[7].ClassList.Should().Contain("mud-icon-size-large");
        }

        [Test]
        public void RadioTest_KeyboardInput()
        {
            var comp = Context.RenderComponent<RadioGroupTest1>();
            // print the generated html
            // select elements needed for the test
            var radio = comp.FindComponent<MudRadioGroup<string>>();
            radio.Instance.Value.Should().Be(null);

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            comp.WaitForAssertion(() => radio.Instance.Value.Should().Be("1"));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            comp.WaitForAssertion(() => radio.Instance.Value.Should().Be(null));

            //Can't tabbed around the radios in test.
        }

        [Test]
        public async Task RadioTest_Other()
        {
            var comp = Context.RenderComponent<RadioGroupTest1>();
            var group = comp.FindComponent<MudRadioGroup<string>>();
            var radio = comp.FindComponent<MudRadio<string>>();

            await comp.InvokeAsync(() => radio.Instance.IMudRadioGroup = null);
            await comp.InvokeAsync(() => radio.Instance.OnClickAsync());
            comp.WaitForAssertion(() => radio.Instance.Value.Should().Be("1"));
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            await comp.InvokeAsync(() => radio.Instance.Disabled = true);
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
            comp.WaitForAssertion(() => group.Instance.Value.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            comp.WaitForAssertion(() => group.Instance.Value.Should().Be(null));
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
                typeof(MudBlazor.Utilities.Exceptions.GenericTypeMismatchException).Should().Be(ex.InnerException.GetType());
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

            comp.FindAll("input")[2].Click(); //click enabled radio
            comp.Instance.SelectedOption.Should().Be("Radio 3");
            comp.FindAll("input")[3].Click(); //click disable radio
            comp.Instance.SelectedOption.Should().Be("Radio 3");

            comp.FindAll("label")[3].ClassList.Contains("mud-disabled").Should().BeTrue();
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
        public void RadioGroupReadOnlyTest()
        {
            var comp = Context.RenderComponent<RadioReadOnlyDisabledExample>();
            var radioGroup = comp.FindComponents<MudRadioGroup<string>>()[0];

            var radios = radioGroup.FindComponents<MudRadio<string>>();
            radios.Count.Should().Be(4);
            radioGroup.FindAll(".mud-radio.mud-readonly").Count.Should().Be(0);

            comp.FindAll(".mud-switch-button > input")[0].Change(true);
            radioGroup.FindAll(".mud-radio.mud-readonly").Count.Should().Be(4);
        }

        /// <summary>
        /// Optional RadioGroup should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalRadioGroup_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.RenderComponent<RadioGroupRequiredTest>();

            comp.Find("div[role=\"radiogroup\"]").HasAttribute("required").Should().BeFalse();
            comp.Find("div[role=\"radiogroup\"]").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required RadioGroup should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredRadioGroup_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.RenderComponent<RadioGroupRequiredTest>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("div[role=\"radiogroup\"]").HasAttribute("required").Should().BeTrue();
            comp.Find("div[role=\"radiogroup\"]").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required RadioGroup attributes should be dynamic.
        /// </summary>
        [Test]
        public void RequiredAndAriaRequiredRadioGroupAttributes_Should_BeDynamic()
        {
            var comp = Context.RenderComponent<RadioGroupRequiredTest>();


            comp.Find("div[role=\"radiogroup\"]").HasAttribute("required").Should().BeFalse();
            comp.Find("div[role=\"radiogroup\"]").GetAttribute("aria-required").Should().Be("false");

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("div[role=\"radiogroup\"]").HasAttribute("required").Should().BeTrue();
            comp.Find("div[role=\"radiogroup\"]").GetAttribute("aria-required").Should().Be("true");
        }

        [Test]
        public void ReadOnlyDisabled_ShouldNot_Ripple()
        {
            var create = (bool readOnly, bool disabled) => Context.RenderComponent<MudRadioGroup<bool>>(self => self
                .Add(x => x.Disabled, disabled)
                .Add(x => x.ReadOnly, readOnly)
                .AddChildContent<MudRadio<bool>>(self => self.Add(x => x.Ripple, true)));
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            bool readOnly, disabled;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            create(readOnly = false, disabled = false).Find("span.mud-button-root").ClassList.Should().Contain("mud-ripple");
            create(readOnly = true, disabled = false).Find("span.mud-button-root").ClassList.Should().NotContain("mud-ripple");
            create(readOnly = false, disabled = true).Find("span.mud-button-root").ClassList.Should().NotContain("mud-ripple");
            create(readOnly = true, disabled = true).Find("span.mud-button-root").ClassList.Should().NotContain("mud-ripple");
        }

        [Test]
        public void ReadOnlyDisabled_ShouldNot_Hover()
        {
            var create = (bool readOnly, bool disabled) => Context.RenderComponent<MudRadioGroup<bool>>(self => self
                .Add(x => x.Disabled, disabled)
                .Add(x => x.ReadOnly, readOnly)
                .AddChildContent<MudRadio<bool>>(self => self.Add(x => x.UncheckedColor, Color.Default)));
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            bool readOnly, disabled;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            create(readOnly = false, disabled = false).Find("span.mud-button-root").ClassList.Should().Contain("hover:mud-default-hover");
            create(readOnly = true, disabled = false).Find("span.mud-button-root").ClassList.Should().NotContain("hover:mud-default-hover");
            create(readOnly = false, disabled = true).Find("span.mud-button-root").ClassList.Should().NotContain("hover:mud-default-hover");
            create(readOnly = true, disabled = true).Find("span.mud-button-root").ClassList.Should().NotContain("hover:mud-default-hover");
        }
    }
}
