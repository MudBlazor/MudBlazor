using System.Linq;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Docs.Examples;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class CheckBoxTests : BunitTest
    {

        [Test]
        public void CheckBox_Test_BooleanStateSelectors()
        {
            // the state of the checkbox should manifest itself in the classes
            // mud-checkbox-true, mud-checkbox-false, mud-checkbox-null applied to the span
            Context.RenderComponent<MudCheckBox<bool>>(self => self.Add(x => x.Value, false))
                .Find(".mud-checkbox .mud-checkbox-false").Should().NotBe(null);
            Context.RenderComponent<MudCheckBox<bool>>(self => self.Add(x => x.Value, true))
                .Find(".mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            Context.RenderComponent<MudCheckBox<bool>>(self => self.Add(x => x.Value, true))
                .Find(".mud-checkbox span").ClassList.Should().NotContain("mud-checkbox-false");
            Context.RenderComponent<MudCheckBox<bool>>(self => self.Add(x => x.Value, true))
                .Find(".mud-checkbox span").ClassList.Should().NotContain("mud-checkbox-null");
            var comp = Context.RenderComponent<MudCheckBox<bool?>>(self => self
                .Add(x => x.Value, null)
                .Add(x => x.TriState, true));
            comp.Find(".mud-checkbox span").ClassList.Should().Contain("mud-checkbox-null");
            comp.Find("input").Change(true);
            comp.Find(".mud-checkbox span").ClassList.Should().Contain("mud-checkbox-true");
            comp.Find("input").Change(false);
            comp.Find(".mud-checkbox span").ClassList.Should().Contain("mud-checkbox-false");
            comp.Find("input").Change("");
            comp.Find(".mud-checkbox span").ClassList.Should().Contain("mud-checkbox-null");
        }

        /// <summary>
        /// single checkbox, initialized false, check -  uncheck
        /// </summary>
        [Test]
        public void CheckBoxTest1()
        {
            var comp = Context.RenderComponent<MudCheckBox<bool>>();
            // print the generated html
            // select elements needed for the test
            var box = comp.Instance;
            // check initial state
            box.Value.Should().Be(false);
            // click and check if it has toggled
            comp.Find("input").Change(true);
            box.Value.Should().Be(true);
            comp.Find("input").Change(false);
            box.Value.Should().Be(false);
        }

        /// <summary>
        /// single checkbox, initialized true, check -  uncheck
        /// </summary>
        [Test]
        public void CheckBoxTest2()
        {
            var comp = Context.RenderComponent<MudCheckBox<bool>>(ComponentParameter.CreateParameter("Value", true));
            // select elements needed for the test
            var box = comp.Instance;
            // check initial state
            box.Value.Should().Be(true);
            // click and check if it has toggled
            comp.Find("input").Change(false);
            box.Value.Should().Be(false);
            comp.Find("input").Change(true);
            box.Value.Should().Be(true);
        }

        /// <summary>
        /// there are two checkboxes synced via a bound variable, so checking one also check the other and vice versa.
        /// </summary>
        [Test]
        public void CheckBoxTest3()
        {
            var comp = Context.RenderComponent<CheckBoxTest3>();
            // select elements needed for the test
            var boxes = comp.FindComponents<MudCheckBox<bool>>();
            // check initial state
            boxes[0].Instance.Value.Should().Be(true);
            boxes[1].Instance.Value.Should().Be(true);
            // click and check if it has toggled
            comp.FindAll("input")[0].Change(false);
            boxes[0].Instance.Value.Should().Be(false);
            boxes[1].Instance.Value.Should().Be(false);

            comp.FindAll("input")[0].Change(true);
            boxes[0].Instance.Value.Should().Be(true);
            boxes[1].Instance.Value.Should().Be(true);

            comp.FindAll("input")[1].Change(false);
            boxes[0].Instance.Value.Should().Be(false);
            boxes[1].Instance.Value.Should().Be(false);

            comp.FindAll("input")[1].Change(true);
            boxes[0].Instance.Value.Should().Be(true);
            boxes[1].Instance.Value.Should().Be(true);
        }

        /// <summary>
        /// Check the correct css classes are applied.
        /// </summary>

        [Test]
        public void CheckBoxTest4()
        {
            var comp = Context.RenderComponent<CheckBoxTest4>();

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
        public void CheckBoxTriStateTest()
        {
            var comp = Context.RenderComponent<MudCheckBox<bool?>>(ComponentParameter.CreateParameter("TriState", true));
            // print the generated html
            // select elements needed for the test
            var box = comp.Instance;
            // check initial state
            box.Value.Should().Be(default);
            // click and check if it has toggled
            comp.Find("input").Change(true);
            box.Value.Should().Be(true);
            comp.Find("input").Change(false);
            box.Value.Should().Be(false);
            // click and check if this is the indeterminate value
            comp.Find("input").Change(false);
            box.Value.Should().Be(default);
            // click and check if this is the true value
            comp.Find("input").Change(true);
            box.Value.Should().Be(true);
        }

        /// <summary>
        /// Without clicking the required checkbox the form should not validate
        /// </summary>
        [Test]
        public void CheckBoxFormTest1()
        {
            var comp = Context.RenderComponent<CheckBoxFormTest1>();
            var form = comp.FindComponent<MudForm>().Instance;
            form.IsValid.Should().BeFalse();
            form.Errors.Length.Should().Be(0);
            var checkbox = comp.FindComponent<MudCheckBox<bool>>();
            // click the checkbox to make the form valid
            checkbox.Find("input").Change(true);
            form.IsValid.Should().BeTrue();
            // click the checkbox to make the form invalid again because the checkbox is required
            checkbox.Find("input").Change(false);
            checkbox.Instance.Error.Should().BeTrue();
            checkbox.Instance.ErrorText.Should().Be("You must agree");
            form.IsValid.Should().BeFalse();
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("You must agree");
            // click the checkbox to make the form valid again
            checkbox.Find("input").Change(true);
            form.IsValid.Should().BeTrue();
            checkbox.Instance.Error.Should().BeFalse();
            checkbox.Instance.ErrorText.Should().Be(null);
        }

        /// <summary>
        /// Binding checkboxes two-way against an array of bools
        /// </summary>
        [Test]
        public void CheckBoxesBindAgainstArrayTest()
        {
            var comp = Context.RenderComponent<CheckBoxesBindAgainstArrayTest>();
            comp.FindAll("p")[^1].TrimmedText().Should().Be("A=True, B=False, C=True, D=False, E=True");
            comp.FindAll("input")[0].Change(false);
            comp.FindAll("p")[^1].TrimmedText().Should().Be("A=False, B=False, C=True, D=False, E=True");
            comp.FindAll("input")[1].Change(true);
            comp.FindAll("p")[^1].TrimmedText().Should().Be("A=False, B=True, C=True, D=False, E=True");
        }

        [Test]
        public void CheckBox_StopClickPropagation_Default_Is_True()
        {
            using var comp = Context.RenderComponent<MudCheckBox<bool>>();
            comp.Instance.StopClickPropagation.Should().BeTrue();
            comp.Markup.Contains("blazor:onclick:stopPropagation").Should().BeTrue();
        }

        /// <summary>
        /// Change state with several keys
        /// </summary>
        [Test]
        public void CheckBoxTest_KeyboardInput()
        {
            var comp = Context.RenderComponent<MudCheckBox<bool?>>();
            comp.SetParam(x => x.TriState, true);
            // print the generated html
            // select elements needed for the test
            var checkbox = comp.Instance;
            checkbox.Value.Should().Be(null);

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(true));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(false));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Delete", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(false));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(true));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(true));

            //Backspace should not change state on non-tristate checkbox
            comp.SetParam(x => x.TriState, false);
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(true));
            //Check tristate space key
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(false));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(true));

            comp.SetParam("Disabled", true);
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(true));
        }
        /// <summary>
        /// Test if the keyboard-disabling switch works
        /// </summary>
        [Test]
        public void CheckBoxTest_KeyboardDisabled()
        {
            var comp = Context.RenderComponent<MudCheckBox<bool?>>();
            comp.SetParam(x => x.TriState, true);
            comp.SetParam(x => x.KeyboardEnabled, false);
            // print the generated html
            // select elements needed for the test
            var checkbox = comp.Instance;
            checkbox.Value.Should().Be(null);

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Delete", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));

            //Backspace should not change state on non-tristate checkbox
            comp.SetParam(x => x.TriState, null);
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));
            //Check tristate space key
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));

            comp.SetParam("Disabled", true);
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Value.Should().Be(null));
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
        public void CheckBoxColorTest(Color color, Color uncheckedcolor)
        {
            var comp = Context.RenderComponent<MudCheckBox<bool>>(x => x.Add(c => c.Color, color).Add(b => b.UncheckedColor, uncheckedcolor));

            var box = comp.Instance;

            // check initial state
            box.Value.Should().Be(false);
            comp.Find(".mud-button-root.mud-icon-button").ClassList.Should().ContainInOrder(new[] { $"mud-{uncheckedcolor.ToDescriptionString()}-text", $"hover:mud-{uncheckedcolor.ToDescriptionString()}-hover" });

            // click and check if it has new color
            comp.Find("input").Change(true);
            box.Value.Should().Be(true);
            comp.Find(".mud-button-root.mud-icon-button").ClassList.Should().ContainInOrder(new[] { $"mud-{color.ToDescriptionString()}-text", $"hover:mud-{color.ToDescriptionString()}-hover" });
        }

        [Test]
        public void CheckBoxDisabledTest()
        {
            var comp = Context.RenderComponent<CheckboxLabelExample>();
            comp.FindAll("label.mud-checkbox")[3].ClassList.Should().Contain("mud-disabled"); // 4rd checkbox
        }

        [Test]
        public void CheckBoxLabelPositionTest()
        {
            var comp = Context.RenderComponent<CheckboxLabelExample>();

            comp.FindAll("label.mud-checkbox")[2].ClassList.Should().Contain("flex-row-reverse"); // 3rd checkbox: LabelPosition.Start
        }

        [Test]
        public void CheckBoxLabelTest()
        {
            var value = new DisplayNameLabelClass();

            var comp = Context.RenderComponent<MudCheckBox<bool>>(x => x.Add(f => f.For, () => value.Boolean));
            comp.Instance.Label.Should().Be("Boolean LabelAttribute"); //label should be set by the attribute

            var comp2 = Context.RenderComponent<MudCheckBox<bool>>(x => x.Add(f => f.For, () => value.Boolean).Add(l => l.Label, "Label Parameter"));
            comp2.Instance.Label.Should().Be("Label Parameter"); //existing label should remain
        }

        /// <summary>
        /// Optional CheckBox should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalCheckBox_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.RenderComponent<MudCheckBox<bool>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required CheckBox should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredCheckBox_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.RenderComponent<MudCheckBox<bool>>(parameters => parameters
                .Add(p => p.Required, true));
            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required CheckBox attributes should be dynamic.
        /// </summary>
        [Test]
        public void RequiredAndAriaRequiredCheckBoxAttributes_Should_BeDynamic()
        {
            var comp = Context.RenderComponent<MudCheckBox<bool>>();

            var input = () => comp.Find("input");
            input().HasAttribute("required").Should().BeFalse();
            input().GetAttribute("aria-required").Should().Be("false");

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.Required, true));

            input().HasAttribute("required").Should().BeTrue();
            input().GetAttribute("aria-required").Should().Be("true");
        }

        [Test]
        public void ReadOnlyDisabled_ShouldNot_Hover()
        {
            Context.RenderComponent<MudCheckBox<bool>>(self => self.Add(x => x.ReadOnly, false)).Find("span").ClassList.Should().Contain("hover:mud-default-hover");
            Context.RenderComponent<MudCheckBox<bool>>(self => self.Add(x => x.ReadOnly, true)).Find("span").ClassList.Should().NotContain("hover:mud-default-hover");
            Context.RenderComponent<MudCheckBox<bool>>(self => self.Add(x => x.ReadOnly, true).Add(x => x.Disabled, false)).Find("span").ClassList.Should().NotContain("hover:mud-default-hover");
            Context.RenderComponent<MudCheckBox<bool>>(self => self.Add(x => x.Disabled, false)).Find("span").ClassList.Should().Contain("hover:mud-default-hover");
            Context.RenderComponent<MudCheckBox<bool>>(self => self.Add(x => x.Disabled, true).Add(x => x.ReadOnly, false)).Find("span").ClassList.Should().NotContain("hover:mud-default-hover");
            Context.RenderComponent<MudCheckBox<bool>>(self => self.Add(x => x.Disabled, true).Add(x => x.ReadOnly, true)).Find("span").ClassList.Should().NotContain("hover:mud-default-hover");
        }
    }
}
