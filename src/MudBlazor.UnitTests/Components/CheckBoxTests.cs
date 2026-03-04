using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents.CheckBox;
using MudBlazor.UnitTests.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class CheckBoxTests : BunitTest
    {

        [Test]
        public async Task CheckBox_Test_BooleanStateSelectors()
        {
            // the state of the checkbox should manifest itself in the classes
            // mud-checkbox-true, mud-checkbox-false, mud-checkbox-null applied to the span
            Context.Render<MudCheckBox<bool>>(self => self.Add(x => x.Value, false))
                .Find(".mud-checkbox .mud-checkbox-false").Should().NotBe(null);
            Context.Render<MudCheckBox<bool>>(self => self.Add(x => x.Value, true))
                .Find(".mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            Context.Render<MudCheckBox<bool>>(self => self.Add(x => x.Value, true))
                .Find(".mud-checkbox span").ClassList.Should().NotContain("mud-checkbox-false");
            Context.Render<MudCheckBox<bool>>(self => self.Add(x => x.Value, true))
                .Find(".mud-checkbox span").ClassList.Should().NotContain("mud-checkbox-null");
            var comp = Context.Render<MudCheckBox<bool?>>(self => self
                .Add(x => x.Value, null)
                .Add(x => x.TriState, true));
            comp.Find(".mud-checkbox span").ClassList.Should().Contain("mud-checkbox-null");
            await comp.Find("input").ChangeAsync(true);
            comp.Find(".mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            await comp.Find("input").ChangeAsync(false);
            comp.Find(".mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            await comp.Find("input").ChangeAsync("");
            comp.Find(".mud-checkbox span").ClassList.Should().Contain("mud-checkbox-null");
        }

        /// <summary>
        /// single checkbox, initialized false, check -  uncheck
        /// </summary>
        [Test]
        public async Task CheckBoxTest1()
        {
            var comp = Context.Render<MudCheckBox<bool>>();
            // print the generated html
            // select elements needed for the test
            var box = comp.Instance;
            // check initial state
            box.ReadValue.Should().Be(false);
            // click and check if it has toggled
            await comp.Find("input").ChangeAsync(true);
            box.ReadValue.Should().Be(true);
            await comp.Find("input").ChangeAsync(false);
            box.ReadValue.Should().Be(false);
        }

        /// <summary>
        /// single checkbox, initialized true, check -  uncheck
        /// </summary>
        [Test]
        public async Task CheckBoxTest2()
        {
            var comp = Context.Render<MudCheckBox<bool>>(parameters => parameters.Add(x => x.Value, true));
            // select elements needed for the test
            var box = comp.Instance;
            // check initial state
            box.ReadValue.Should().Be(true);
            // click and check if it has toggled
            await comp.Find("input").ChangeAsync(false);
            box.ReadValue.Should().Be(false);
            await comp.Find("input").ChangeAsync(true);
            box.ReadValue.Should().Be(true);
        }

        /// <summary>
        /// there are two checkboxes synced via a bound variable, so checking one also check the other and vice versa.
        /// </summary>
        [Test]
        public async Task CheckBoxTest3()
        {
            var comp = Context.Render<CheckBoxTest3>();
            // select elements needed for the test
            var boxes = comp.FindComponents<MudCheckBox<bool>>();
            // check initial state
            boxes[0].Instance.ReadValue.Should().Be(true);
            boxes[1].Instance.ReadValue.Should().Be(true);
            // click and check if it has toggled
            await comp.FindAll("input")[0].ChangeAsync(false);
            boxes[0].Instance.ReadValue.Should().Be(false);
            boxes[1].Instance.ReadValue.Should().Be(false);

            await comp.FindAll("input")[0].ChangeAsync(true);
            boxes[0].Instance.ReadValue.Should().Be(true);
            boxes[1].Instance.ReadValue.Should().Be(true);

            await comp.FindAll("input")[1].ChangeAsync(false);
            boxes[0].Instance.ReadValue.Should().Be(false);
            boxes[1].Instance.ReadValue.Should().Be(false);

            await comp.FindAll("input")[1].ChangeAsync(true);
            boxes[0].Instance.ReadValue.Should().Be(true);
            boxes[1].Instance.ReadValue.Should().Be(true);
        }

        /// <summary>
        /// Check the correct css classes are applied.
        /// </summary>

        [Test]
        public void CheckBoxTest4()
        {
            var comp = Context.Render<CheckBoxTest4>();

            // check dense
            comp.FindAll("span").ToArray()[0].ClassList.Should().Contain("mud-checkbox-dense");
            comp.FindAll("span").ToArray()[1].ClassList.Should().NotContain("mud-checkbox-dense");
            comp.FindAll("span").ToArray()[2].ClassList.Should().NotContain("mud-checkbox-dense");
            comp.FindAll("span").ToArray()[3].ClassList.Should().NotContain("mud-checkbox-dense");
            // check size
            comp.FindAll("svg").ToArray()[0].ClassList.Should().Contain("mud-icon-size-medium");
            comp.FindAll("svg").ToArray()[1].ClassList.Should().Contain("mud-icon-size-small");
            comp.FindAll("svg").ToArray()[2].ClassList.Should().Contain("mud-icon-size-medium");
            comp.FindAll("svg").ToArray()[3].ClassList.Should().Contain("mud-icon-size-large");
        }

        /// <summary>
        /// Check the implementation of the TriState parameter
        /// </summary>
        [Test]
        public async Task CheckBoxTriState()
        {
            var comp = Context.Render<MudCheckBox<bool?>>(parameters => parameters.Add(x => x.TriState, true));
            // print the generated html
            // select elements needed for the test
            var box = comp.Instance;
            // check initial state
            box.ReadValue.Should().BeNull();
            // click and check if it has toggled
            await comp.Find("input").ChangeAsync(true);
            box.ReadValue.Should().Be(true);
            await comp.Find("input").ChangeAsync(false);
            box.ReadValue.Should().Be(false);
            // click and check if this is the indeterminate value
            await comp.Find("input").ChangeAsync(false);
            box.ReadValue.Should().BeNull();
            // click and check if this is the true value
            await comp.Find("input").ChangeAsync(true);
            box.ReadValue.Should().Be(true);
        }

        /// <summary>
        /// Without clicking the required checkbox the form should not validate
        /// </summary>
        [Test]
        public async Task CheckBoxFormTest1()
        {
            var comp = Context.Render<CheckBoxFormTest1>();
            var form = comp.FindComponent<MudForm>().Instance;
            form.IsValid.Should().BeFalse();
            form.Errors.Length.Should().Be(0);
            var checkbox = comp.FindComponent<MudCheckBox<bool>>();
            // click the checkbox to make the form valid
            await checkbox.Find("input").ChangeAsync(true);
            form.IsValid.Should().BeTrue();
            // click the checkbox to make the form invalid again because the checkbox is required
            await checkbox.Find("input").ChangeAsync(false);
            checkbox.Instance.GetState(x => x.Error).Should().BeTrue();
            checkbox.Markup.Should().Contain("You must agree");
            checkbox.Instance.GetState(x => x.ErrorText).Should().Be("You must agree");
            form.IsValid.Should().BeFalse();
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("You must agree");
            // click the checkbox to make the form valid again
            await checkbox.Find("input").ChangeAsync(true);
            form.IsValid.Should().BeTrue();
            checkbox.Instance.GetState(x => x.Error).Should().BeFalse();
            checkbox.Instance.GetState(x => x.ErrorText).Should().Be(null);
        }

        /// <summary>
        /// A required tristate checkbox must have a value of true or false, but not null.
        /// </summary>
        [Test]
        public async Task TriStateCheckBoxForm()
        {
            var comp = Context.Render<CheckBoxFormTest2>();
            var form = comp.FindComponent<MudForm>().Instance;
            var checkbox = comp.FindComponent<MudCheckBox<bool?>>();

            // initial state: null, form should be invalid without errors
            form.IsValid.Should().BeFalse();
            form.Errors.Length.Should().Be(0);

            // after validating, the form should be invalid with errors
            await comp.InvokeAsync(() => form.ValidateAsync());
            form.IsValid.Should().BeFalse();
            checkbox.Instance.GetState(x => x.Error).Should().BeTrue();
            checkbox.Markup.Should().Contain("You must select a value");
            checkbox.Instance.GetState(x => x.ErrorText).Should().Be("You must select a value");

            // state: true, form should be valid
            await checkbox.Find("input").ChangeAsync(true);
            await comp.InvokeAsync(() => form.ValidateAsync());
            form.IsValid.Should().BeTrue();
            checkbox.Instance.GetState(x => x.Error).Should().BeFalse();
            checkbox.Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            // state: false, form should be valid
            await checkbox.Find("input").ChangeAsync(false);
            await comp.InvokeAsync(() => form.ValidateAsync());
            form.IsValid.Should().BeTrue();
            checkbox.Instance.GetState(x => x.Error).Should().BeFalse();
            checkbox.Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            // state: null, form should be invalid again
            await checkbox.Find("input").ChangeAsync(null);
            await comp.InvokeAsync(() => form.ValidateAsync());
            form.IsValid.Should().BeFalse();
            checkbox.Instance.GetState(x => x.Error).Should().BeTrue();
            checkbox.Markup.Should().Contain("You must select a value");
            checkbox.Instance.GetState(x => x.ErrorText).Should().Be("You must select a value");
        }

        /// <summary>
        /// Binding checkboxes two-way against an array of bools
        /// </summary>
        [Test]
        public async Task CheckBoxesBindAgainstArray()
        {
            var comp = Context.Render<CheckBoxesBindAgainstArrayTest>();
            comp.FindAll("p")[^1].TrimmedText().Should().Be("A=True, B=False, C=True, D=False, E=True");
            await comp.FindAll("input")[0].ChangeAsync(false);
            comp.FindAll("p")[^1].TrimmedText().Should().Be("A=False, B=False, C=True, D=False, E=True");
            await comp.FindAll("input")[1].ChangeAsync(true);
            comp.FindAll("p")[^1].TrimmedText().Should().Be("A=False, B=True, C=True, D=False, E=True");
        }

        [Test]
        public void CheckBox_StopClickPropagation_Default_Is_True()
        {
            using var comp = Context.Render<MudCheckBox<bool>>();
            comp.Instance.StopClickPropagation.Should().BeTrue();
            comp.Markup.Contains("blazor:onclick:stopPropagation").Should().BeTrue();
        }

        /// <summary>
        /// Change state with several keys
        /// </summary>
        [Test]
        public async Task CheckBox_KeyboardInput()
        {
            var comp = Context.Render<MudCheckBox<bool?>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.TriState, true));
            // print the generated html
            // select elements needed for the test
            var checkbox = comp.Instance;
            checkbox.ReadValue.Should().Be(null);

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(true));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(false));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Delete", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(false));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(true));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(true));

            //Backspace should not change state on non-tristate checkbox
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.TriState, false));
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(true));
            //Check tristate space key
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(false));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(true));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(true));
        }
        /// <summary>
        /// Test if the keyboard-disabling switch works
        /// </summary>
        [Test]
        public async Task CheckBox_KeyboardDisabled()
        {
            var comp = Context.Render<MudCheckBox<bool?>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.TriState, true)
                .Add(x => x.KeyboardEnabled, false));
            // print the generated html
            // select elements needed for the test
            var checkbox = comp.Instance;
            checkbox.ReadValue.Should().Be(null);

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Delete", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));

            //Backspace should not change state on non-tristate checkbox
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.TriState, false));
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));
            //Check tristate space key
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => checkbox.ReadValue.Should().Be(null));
        }

        [Test]
        [TestCase(Color.Default, Color.Primary)]
        [TestCase(Color.Primary, Color.Secondary)]
        [TestCase(Color.Secondary, Color.Info)]
        [TestCase(Color.Tertiary, Color.Success)]
        [TestCase(Color.Info, Color.Warning)]
        [TestCase(Color.Success, Color.Error)]
        [TestCase(Color.Warning, Color.Dark)]
        [TestCase(Color.Error, Color.Primary)]
        [TestCase(Color.Dark, Color.Primary)]
        public async Task CheckBoxColor(Color color, Color uncheckedcolor)
        {
            var comp = Context.Render<MudCheckBox<bool>>(x => x.Add(c => c.Color, color).Add(b => b.UncheckedColor, uncheckedcolor));

            var box = comp.Instance;

            // check initial state
            box.ReadValue.Should().Be(false);
            comp.Find(".mud-button-root.mud-icon-button").ClassList.Should().ContainInOrder(new[] { $"mud-{uncheckedcolor.ToStringFast(true)}-text", $"hover:mud-{uncheckedcolor.ToStringFast(true)}-hover" });

            // click and check if it has new color
            await comp.Find("input").ChangeAsync(true);
            box.ReadValue.Should().Be(true);
            comp.Find(".mud-button-root.mud-icon-button").ClassList.Should().ContainInOrder(new[] { $"mud-{color.ToStringFast(true)}-text", $"hover:mud-{color.ToStringFast(true)}-hover" });
        }

        [Test]
        public void CheckBoxDisabled()
        {
            var comp = Context.Render<CheckboxLabelTest>();
            comp.FindAll("label.mud-checkbox")[3].ClassList.Should().Contain("mud-disabled"); // 4rd checkbox
        }

        [Test]
        public void CheckBoxLabelPlacement()
        {
            var comp = Context.Render<CheckboxLabelTest>();

            comp.FindAll("label.mud-checkbox")[2].ClassList.Should().Contain("mud-input-content-placement-start"); // 3rd checkbox: Placement.Start
        }

        [Test]
        public void CheckBoxLabel()
        {
            var value = new DisplayNameLabelClass();

            var comp = Context.Render<MudCheckBox<bool>>(x => x.Add(f => f.For, () => value.Boolean));
            comp.Instance.Label.Should().Be("Boolean LabelAttribute"); //label should be set by the attribute

            var comp2 = Context.Render<MudCheckBox<bool>>(x => x.Add(f => f.For, () => value.Boolean).Add(l => l.Label, "Label Parameter"));
            comp2.Instance.Label.Should().Be("Label Parameter"); //existing label should remain
        }

        /// <summary>
        /// Optional CheckBox should not have required attribute.
        /// </summary>
        [Test]
        public void OptionalCheckBox_Should_NotHaveRequiredAttribute()
        {
            var comp = Context.Render<MudCheckBox<bool>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
        }

        /// <summary>
        /// Required CheckBox should have required attribute.
        /// </summary>
        [Test]
        public void RequiredCheckBox_Should_HaveRequiredAttribute()
        {
            var comp = Context.Render<MudCheckBox<bool>>(parameters => parameters
                .Add(p => p.Required, true));
            comp.Find("input").HasAttribute("required").Should().BeTrue();
        }

        /// <summary>
        /// Required CheckBox attribute should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredCheckBoxAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<MudCheckBox<bool>>();

            var input = () => comp.Find("input");
            input().HasAttribute("required").Should().BeFalse();

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            input().HasAttribute("required").Should().BeTrue();
        }

        [Test]
        public void CheckBox_Respects_Custom_TabIndex()
        {
            var comp = Context.Render<MudCheckBox<bool>>(parameters => parameters.AddUnmatched("tabindex", "-1"));

            comp.Find("input").GetAttribute("tabindex").Should().Be("-1");
        }

        [Test]
        public void CheckBox_Uses_Default_TabIndex_When_Enabled()
        {
            var comp = Context.Render<MudCheckBox<bool>>();

            comp.Find("input").GetAttribute("tabindex").Should().Be("0");
        }

        [Test]
        public void CheckBox_Uses_Default_TabIndex_When_Disabled()
        {
            var comp = Context.Render<MudCheckBox<bool>>(parameters => parameters.Add(x => x.Disabled, true));

            comp.Find("input").GetAttribute("tabindex").Should().Be("-1");
        }

        [Test]
        public void CheckBox_Respects_Custom_TabIndex_CaseInsensitive()
        {
            var comp = Context.Render<MudCheckBox<bool>>(parameters => parameters.AddUnmatched("TabIndex", "-1"));

            comp.Find("input").GetAttribute("tabindex").Should().Be("-1");
        }

        [Test]
        public void ReadOnlyDisabled_ShouldNot_Hover()
        {
            Context.Render<MudCheckBox<bool>>(self => self.Add(x => x.ReadOnly, false)).Find("span").ClassList.Should().Contain("hover:mud-default-hover");
            Context.Render<MudCheckBox<bool>>(self => self.Add(x => x.ReadOnly, true)).Find("span").ClassList.Should().NotContain("hover:mud-default-hover");
            Context.Render<MudCheckBox<bool>>(self => self.Add(x => x.ReadOnly, true).Add(x => x.Disabled, false)).Find("span").ClassList.Should().NotContain("hover:mud-default-hover");
            Context.Render<MudCheckBox<bool>>(self => self.Add(x => x.Disabled, false)).Find("span").ClassList.Should().Contain("hover:mud-default-hover");
            Context.Render<MudCheckBox<bool>>(self => self.Add(x => x.Disabled, true).Add(x => x.ReadOnly, false)).Find("span").ClassList.Should().NotContain("hover:mud-default-hover");
            Context.Render<MudCheckBox<bool>>(self => self.Add(x => x.Disabled, true).Add(x => x.ReadOnly, true)).Find("span").ClassList.Should().NotContain("hover:mud-default-hover");
        }

        [Test]
        public void CheckBox_AriaLabel_OverRides()
        {
            var comp = Context.Render<CheckBoxAriaLabelTest>();
            var checkboxes = comp.FindAll(".mud-input-control.mud-input-control-boolean-input");

            // verify checkbox one maintains it's original structure, no aria class used, label with a span element
            checkboxes[0].GetElementsByClassName("mud-sr-only").Length.Should().Be(0);
            var element0 = comp.Find(".cb1 label.mud-checkbox span.mud-typography");
            element0.HasAttribute("aria-hidden").Should().BeFalse();

            // checkbox two should have both a valid label with aria-hidden, an input with arialabelledby and the labelledby element
            checkboxes[1].GetElementsByClassName("mud-sr-only").Length.Should().Be(1);
            var element1 = comp.Find(".cb2 label.mud-checkbox span.mud-typography");
            element1.HasAttribute("aria-hidden").Should().BeTrue();
            var input1 = comp.Find(".cb2 label.mud-checkbox input");
            var input1ForId = input1.GetAttribute("aria-labelledby");
            comp.Find($".cb2 label.mud-checkbox #{input1ForId}").Should().NotBeNull();

            // checkbox three should have original structure intact, no aria class used, label with a span element for child content
            checkboxes[2].GetElementsByClassName("mud-sr-only").Length.Should().Be(0);
            var element2 = comp.Find(".cb3 label.mud-checkbox span.mud-typography");
            element2.HasAttribute("aria-hidden").Should().BeFalse();

            // checkbox four should look identical to two except this time it's with ChildContent
            checkboxes[3].GetElementsByClassName("mud-sr-only").Length.Should().Be(1);
            var element3 = comp.Find(".cb4 label.mud-checkbox span.mud-typography");
            element3.HasAttribute("aria-hidden").Should().BeTrue();
            var input3 = comp.Find(".cb4 label.mud-checkbox input");
            var input3ForId = input3.GetAttribute("aria-labelledby");
            comp.Find($".cb4 label.mud-checkbox #{input3ForId}").Should().NotBeNull();

            // checkbox five has no label, no child content, just arialabel
            checkboxes[4].GetElementsByClassName("mud-sr-only").Length.Should().Be(1);
            comp.FindAll(".cb5 label.mud-checkbox span.mud-typography").Count.Should().Be(0);
            var input4 = comp.Find(".cb5 label.mud-checkbox input");
            var input4ForId = input4.GetAttribute("aria-labelledby");
            comp.Find($".cb5 label.mud-checkbox #{input4ForId}").Should().NotBeNull();
        }
    }
}
