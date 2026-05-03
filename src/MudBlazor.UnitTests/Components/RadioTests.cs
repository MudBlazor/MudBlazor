using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.Radio;
using MudBlazor.UnitTests.TestComponents.RadioGroup;
using MudBlazor.UnitTests.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class RadioTests : BunitTest
    {
        [Test]
        public void RadiGroup_CheckClass()
        {
            var comp = Context.Render<RadioGroupTest1>();

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
        public void Radio_AriaLabel()
        {
            var comp = Context.Render<RadioAriaLabelTest>();

            // verify radio one maintains it's original structure, no aria class used, label with a span element
            var r1 = comp.Find(".r1");
            r1.GetElementsByClassName("mud-sr-only").Length.Should().Be(0);
            var element0 = comp.Find(".r1 label.mud-radio span.mud-typography");
            element0.HasAttribute("aria-hidden").Should().BeFalse();

            // radio two should have both a valid label with aria-hidden, an input with arialabelledby and the labelledby element
            var r2 = comp.Find(".r2");
            r2.GetElementsByClassName("mud-sr-only").Length.Should().Be(1);
            var element1 = comp.Find(".r2 label.mud-radio span.mud-typography");
            element1.HasAttribute("aria-hidden").Should().BeTrue();
            var input1 = comp.Find(".r2 label.mud-radio input");
            var input1ForId = input1.GetAttribute("aria-labelledby");
            comp.Find($".r2 label.mud-radio #{input1ForId}").Should().NotBeNull();

            // radio three should have original structure intact, no aria class used, label with a span element for child content
            var r3 = comp.Find(".r3");
            r3.GetElementsByClassName("mud-sr-only").Length.Should().Be(0);
            var element2 = comp.Find(".r3 label.mud-radio span.mud-typography");
            element2.HasAttribute("aria-hidden").Should().BeFalse();

            // radio four should look identical to two except this time it's with ChildContent
            var r4 = comp.Find(".r4");
            r4.GetElementsByClassName("mud-sr-only").Length.Should().Be(1);
            var element3 = comp.Find(".r4 label.mud-radio span.mud-typography");
            element3.HasAttribute("aria-hidden").Should().BeTrue();
            var input3 = comp.Find(".r4 label.mud-radio input");
            var input3ForId = input3.GetAttribute("aria-labelledby");
            comp.Find($".r4 label.mud-radio #{input3ForId}").Should().NotBeNull();

            // radio five has no label, no child content, just arialabel
            var r5 = comp.Find(".r5");
            r5.GetElementsByClassName("mud-sr-only").Length.Should().Be(1);
            comp.FindAll(".r5 label.mud-radio span.mud-typography").Count.Should().Be(0);
            var input4 = comp.Find(".r5 label.mud-radio input");
            var input4ForId = input4.GetAttribute("aria-labelledby");
            comp.Find($".r5 label.mud-radio #{input4ForId}").Should().NotBeNull();
        }

        [Test]
        public async Task RadioGroupTest1()
        {
            var comp = Context.Render<RadioGroupTest1>();
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup<string>>();
            var inputs = comp.FindAll("input").ToArray();

            // check initial state
            group.Instance.Value.Should().Be(null);
            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().NotContain("mud-checked");
            // click radio 1
            await inputs[0].ClickAsync();
            group.Instance.Value.Should().Be("1");

            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().Contain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().NotContain("mud-checked");
            // click radio 2
            await inputs[1].ClickAsync();
            group.Instance.Value.Should().Be("2");

            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().Contain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().NotContain("mud-checked");
            // click radio 3
            await inputs[2].ClickAsync();
            group.Instance.Value.Should().Be("3");

            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().Contain("mud-checked");
            // click radio 1
            await inputs[0].ClickAsync();
            group.Instance.Value.Should().Be("1");

            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().Contain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public void RadioGroupTest2()
        {
            var comp = Context.Render<RadioGroupTest2>();
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup<string>>();
            // check initial state, should be initialized to second radio by default
            group.Instance.Value.Should().Be("2");
            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().Contain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public async Task RadioGroupTest3()
        {
            var comp = Context.Render<RadioGroupTest3>();
            // select elements needed for the test
            var groups = comp.FindComponents<MudRadioGroup<string>>();
            var inputs = comp.FindAll("input").ToArray();

            // check initial state, should be initialized to second radio by default for both groups
            groups[0].Instance.Value.Should().Be("2");
            groups[1].Instance.Value.Should().Be("2");
            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().Contain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[3].ClassList.Should().Contain("mud-checked");
            // click first radio of second group - they should both switch to L1
            await inputs[2].ClickAsync();

            groups[0].Instance.Value.Should().Be("1");
            groups[1].Instance.Value.Should().Be("1");
            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().Contain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().Contain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[3].ClassList.Should().NotContain("mud-checked");
            // click second radio of first group - they should both switch to L1
            await inputs[1].ClickAsync();

            groups[0].Instance.Value.Should().Be("2");
            groups[1].Instance.Value.Should().Be("2");
            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().Contain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[3].ClassList.Should().Contain("mud-checked");
        }

        [Test]
        public async Task RadioGroupTest4()
        {
            var comp = Context.Render<RadioGroupTest4>();
            // select elements needed for the test
            var groups = comp.FindComponents<MudRadioGroup<string>>();

            // check initial state, should be uninitialized
            groups[0].Instance.Value.Should().Be(null);
            groups[1].Instance.Value.Should().Be(null);
            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[3].ClassList.Should().NotContain("mud-checked");
            // click first radio of second group - only second group should switch to L1
            await comp.FindAll("input")[2].ClickAsync();

            groups[0].Instance.Value.Should().Be(null);
            groups[1].Instance.Value.Should().Be("x");
            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().Contain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[3].ClassList.Should().NotContain("mud-checked");
            // click second radio of first group - only first group should switch to L1
            await comp.FindAll("input")[1].ClickAsync();

            groups[0].Instance.Value.Should().Be("2");
            groups[1].Instance.Value.Should().Be("x");
            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().Contain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().Contain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[3].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public async Task RadioGroupTest5()
        {
            var comp = Context.Render<RadioGroupTest5>();
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup<string>>();
            // check initial state
            group.Instance.Value.Should().Be(null);
            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().NotContain("mud-checked");
            // click radio 1
            await comp.FindAll("input")[0].ClickAsync();
            group.Instance.Value.Should().Be("1");

            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().Contain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().NotContain("mud-checked");
            // click reset button
            await comp.Find("button").ClickAsync();
            group.Instance.Value.Should().Be(null);

            comp.FindAll(".mud-radio > span.mud-icon-button")[0].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[1].ClassList.Should().NotContain("mud-checked");
            comp.FindAll(".mud-radio > span.mud-icon-button")[2].ClassList.Should().NotContain("mud-checked");
        }

        [Test]
        public void RadioGroupTest6()
        {
            var comp = Context.Render<RadioGroupTest6>();
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup<string>>();
            // check dense
            comp.FindAll("label > span")[0].ClassList.Should().Contain("mud-radio-dense");
            comp.FindAll("label > span")[1].ClassList.Should().NotContain("mud-radio-dense");
            comp.FindAll("label > span")[2].ClassList.Should().NotContain("mud-radio-dense");
            comp.FindAll("label > span")[3].ClassList.Should().NotContain("mud-radio-dense");
            // check size
            comp.FindAll("svg")[0].ClassList.Should().Contain("mud-icon-size-medium");
            comp.FindAll("svg")[1].ClassList.Should().Contain("mud-icon-size-small");
            comp.FindAll("svg")[2].ClassList.Should().Contain("mud-icon-size-medium");
            comp.FindAll("svg")[3].ClassList.Should().Contain("mud-icon-size-large");
        }

        [Test]
        public async Task Radio_BindAfter()
        {
            var comp = Context.Render<RadioGroupTest5>();
            // select elements needed for the test
            var group = comp.FindComponent<MudRadioGroup<string>>();
            var inputs = comp.FindAll("input").ToArray();

            //Value should change on radio click and bind after should fire
            await inputs[1].ClickAsync();
            group.Instance.Value.Should().Be("2");
            comp.Instance.BindAfterCount.Should().Be(1);

            //Value should change when reset via the button, but bind after should NOT fire
            await comp.Find("button").ClickAsync();
            group.Instance.Value.Should().Be(null);
            comp.Instance.BindAfterCount.Should().Be(1);
        }

        [Test]
        public async Task Radio_KeyboardInput()
        {
            var comp = Context.Render<RadioGroupTest1>();
            // print the generated html
            // select elements needed for the test
            var radio = comp.FindComponent<MudRadioGroup<string>>();
            radio.Instance.Value.Should().Be(null);

            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => radio.Instance.Value.Should().Be("1"));

            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => radio.Instance.Value.Should().Be(null));

            //Can't tabbed around the radios in test.
        }

        [Test]
        public async Task Radio_Other()
        {
            var comp = Context.Render<RadioGroupTest1>();
            var group = comp.FindComponent<MudRadioGroup<string>>();
            var radio = comp.FindComponent<MudRadio<string>>();

            await comp.InvokeAsync(() => radio.Instance.IMudRadioGroup = null);
            await comp.InvokeAsync(() => radio.Instance.OnClickAsync());
            await comp.WaitForAssertionAsync(() => radio.Instance.ReadValue.Should().Be("1"));
            await radio.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));
            await comp.WaitForAssertionAsync(() => group.Instance.Value.Should().Be(null));

            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => group.Instance.Value.Should().Be(null));
        }

        [Test]
        public void Radio_TypeException()
        {
            try
            {
                var comp = Context.Render<RadioGroupExceptionTest>();
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
        public async Task RadioDisabled()
        {
            var comp = Context.Render<RadioGroupTest7>();
            comp.Instance.SelectedOption.Should().BeNull();

            await comp.FindAll("input")[2].ClickAsync(); //click enabled radio
            comp.Instance.SelectedOption.Should().Be("Radio 3");
            await comp.FindAll("input")[3].ClickAsync(); //click disable radio
            comp.Instance.SelectedOption.Should().Be("Radio 3");

            comp.FindAll("label")[3].ClassList.Contains("mud-disabled").Should().BeTrue();
        }

        /// <summary>
        /// Tests the Disabled property of the MudRadioGroup
        /// </summary>
        [Test]
        public async Task RadioGroupDisabled()
        {
            var comp = Context.Render<RadioReadOnlyDisabledTest>();
            var radioGroup = comp.FindComponents<MudRadioGroup<string>>()[1];

            var radios = radioGroup.FindComponents<MudRadio<string>>();
            radios.Count.Should().Be(4);
            radioGroup.FindAll(".mud-radio > span.mud-disabled").Count.Should().Be(0);

            await comp.FindAll(".mud-switch-button > input")[1].ChangeAsync(true);
            radioGroup.FindAll(".mud-radio > span.mud-disabled").Count.Should().Be(4);
        }

        /// <summary>
        /// Tests the Readonly property of the MudRadioGroup
        /// </summary>
        [Test]
        public async Task RadioGroupReadOnly()
        {
            var comp = Context.Render<RadioReadOnlyDisabledTest>();
            var radioGroup = comp.FindComponents<MudRadioGroup<string>>()[0];

            var radios = radioGroup.FindComponents<MudRadio<string>>();
            radios.Count.Should().Be(4);
            radioGroup.FindAll(".mud-radio > span.mud-readonly").Count.Should().Be(0);

            await comp.FindAll(".mud-switch-button > input")[0].ChangeAsync(true);
            radioGroup.FindAll(".mud-radio > span.mud-readonly").Count.Should().Be(4);
        }

        [Test]
        public void OptionalRadioGroup_Should_HaveAriaRequiredFalse()
        {
            var comp = Context.Render<RadioGroupRequiredTest>();

            comp.Find("div[role=\"radiogroup\"]").GetAttribute("aria-required").Should().Be("false");
        }

        [Test]
        public void RequiredRadioGroup_Should_HaveAriaRequiredTrue()
        {
            var comp = Context.Render<RadioGroupRequiredTest>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("div[role=\"radiogroup\"]").GetAttribute("aria-required").Should().Be("true");
        }

        [Test]
        public async Task RadioGroupAriaRequired_Should_BeDynamic()
        {
            var comp = Context.Render<RadioGroupRequiredTest>();

            comp.Find("div[role=\"radiogroup\"]").GetAttribute("aria-required").Should().Be("false");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("div[role=\"radiogroup\"]").GetAttribute("aria-required").Should().Be("true");
        }

        [Test]
        public void Radio_Respects_Custom_TabIndex()
        {
            var comp = Context.Render<MudRadio<bool>>(parameters => parameters.AddUnmatched("tabindex", "-1"));

            comp.Find("input").GetAttribute("tabindex").Should().Be("-1");
        }

        [Test]
        public void Radio_Uses_Default_TabIndex_When_Enabled()
        {
            var comp = Context.Render<MudRadio<bool>>();

            comp.Find("input").GetAttribute("tabindex").Should().Be("0");
        }

        [Test]
        public void Radio_Uses_Default_TabIndex_When_Disabled()
        {
            var comp = Context.Render<MudRadio<bool>>(parameters => parameters.Add(x => x.Disabled, true));

            comp.Find("input").GetAttribute("tabindex").Should().Be("-1");
        }

        [Test]
        public void Radio_Respects_Custom_TabIndex_CaseInsensitive()
        {
            var comp = Context.Render<MudRadio<bool>>(parameters => parameters.AddUnmatched("TabIndex", "-1"));

            comp.Find("input").GetAttribute("tabindex").Should().Be("-1");
        }

        [Test]
        public void ReadOnlyDisabled_ShouldNot_Ripple()
        {
            var create = (bool readOnly, bool disabled) => Context.Render<MudRadioGroup<bool>>(self => self
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
            var create = (bool readOnly, bool disabled) => Context.Render<MudRadioGroup<bool>>(self => self
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

        [Test]
        public void RadioLabel()
        {
            var value = new DisplayNameLabelClass();

            var comp = Context.Render<MudRadio<bool>>(x => x.Add(f => f.For, () => value.Boolean));
            comp.Instance.Label.Should().Be("Boolean LabelAttribute"); //label should be set by the attribute

            var comp2 = Context.Render<MudRadio<bool>>(x => x.Add(f => f.For, () => value.Boolean).Add(l => l.Label, "Label Parameter"));
            comp2.Instance.Label.Should().Be("Label Parameter"); //existing label should remain
        }
    }
}
