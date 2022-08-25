using System;
using System.Linq;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Docs.Examples;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class CheckBoxTests : BunitTest
    {
        /// <summary>
        /// single checkbox, initialized false, check -  uncheck
        /// </summary>
        [Test]
        public void CheckBoxTest1()
        {
            var comp = Context.RenderComponent<MudCheckBox<bool>>();
            // print the generated html
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var box = comp.Instance;
            var input = comp.Find("input");
            // check initial state
            box.Checked.Should().Be(false);
            // click and check if it has toggled
            input.Change(true);
            box.Checked.Should().Be(true);
            input.Change(false);
            box.Checked.Should().Be(false);
        }

        /// <summary>
        /// single checkbox, initialized true, check -  uncheck
        /// </summary>
        [Test]
        public void CheckBoxTest2()
        {
            var comp = Context.RenderComponent<MudCheckBox<bool>>(ComponentParameter.CreateParameter("Checked", true));
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var box = comp.Instance;
            var input = comp.Find("input");
            // check initial state
            box.Checked.Should().Be(true);
            // click and check if it has toggled
            input.Change(false);
            box.Checked.Should().Be(false);
            input.Change(true);
            box.Checked.Should().Be(true);
        }

        /// <summary>
        /// there are two checkboxes synced via a bound variable, so checking one also check the other and vice versa.
        /// </summary>
        [Test]
        public void CheckBoxTest3()
        {
            var comp = Context.RenderComponent<CheckBoxTest3>();
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var boxes = comp.FindComponents<MudCheckBox<bool>>();
            var inputs = comp.FindAll("input");
            // check initial state
            boxes[0].Instance.Checked.Should().Be(true);
            boxes[1].Instance.Checked.Should().Be(true);
            // click and check if it has toggled
            inputs[0].Change(false);
            boxes[0].Instance.Checked.Should().Be(false);
            boxes[1].Instance.Checked.Should().Be(false);
            inputs = comp.FindAll("input");
            inputs[0].Change(true);
            boxes[0].Instance.Checked.Should().Be(true);
            boxes[1].Instance.Checked.Should().Be(true);
            inputs = comp.FindAll("input");
            inputs[1].Change(false);
            boxes[0].Instance.Checked.Should().Be(false);
            boxes[1].Instance.Checked.Should().Be(false);
            inputs = comp.FindAll("input");
            inputs[1].Change(true);
            boxes[0].Instance.Checked.Should().Be(true);
            boxes[1].Instance.Checked.Should().Be(true);
        }

        /// <summary>
        /// Check the correct css classes are applied.
        /// </summary>

        [Test]
        public void CheckBoxTest4()
        {
            var comp = Context.RenderComponent<CheckBoxTest4>();
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var spans = comp.FindAll("span").ToArray();
            var svgs = comp.FindAll("svg").ToArray();
            // check dense
            spans[0].ClassList.Should().Contain("mud-checkbox-dense");
            spans[1].ClassList.Should().NotContain("mud-checkbox-dense");
            spans[2].ClassList.Should().NotContain("mud-checkbox-dense");
            spans[3].ClassList.Should().NotContain("mud-checkbox-dense");
            // check size
            svgs[0].ClassList.Should().Contain("mud-icon-size-medium");
            svgs[1].ClassList.Should().Contain("mud-icon-size-small");
            svgs[2].ClassList.Should().Contain("mud-icon-size-medium");
            svgs[3].ClassList.Should().Contain("mud-icon-size-large");
        }

        /// <summary>
        /// Check the implementation of the TriState parameter
        /// </summary>
        [Test]
        public void CheckBoxTriStateTest()
        {
            var comp = Context.RenderComponent<MudCheckBox<bool?>>(ComponentParameter.CreateParameter("TriState", true));
            // print the generated html
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var box = comp.Instance;
            var input = comp.Find("input");
            // check initial state
            box.Checked.Should().Be(default);
            // click and check if it has toggled
            input.Change(true);
            box.Checked.Should().Be(true);
            //Console.WriteLine(comp.Markup);
            input.Change(false);
            box.Checked.Should().Be(false);
            //Console.WriteLine(comp.Markup);
            // click and check if this is the indeterminate value
            input.Change(false);
            box.Checked.Should().Be(default);
            //Console.WriteLine(comp.Markup);
            // click and check if this is the true value
            input.Change(true);
            box.Checked.Should().Be(true);
            //Console.WriteLine(comp.Markup);
        }

        /// <summary>
        /// Without clicking the required checkbox the form should not validate
        /// </summary>
        [Test]
        public void CheckBoxFormTest1()
        {
            var comp = Context.RenderComponent<CheckBoxFormTest1>();
            //Console.WriteLine(comp.Markup);
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
            //Console.WriteLine(comp.Markup);
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
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var checkbox = comp.Instance;
            checkbox.Checked.Should().Be(null);

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(true));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(false));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Delete", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(false));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(true));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(true));

            //Backspace should not change state on non-tristate checkbox
            comp.SetParam(x => x.TriState, false);
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(true));
            //Check tristate space key
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(false));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(true));

            comp.SetParam("Disabled", true);
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(true));
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
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var checkbox = comp.Instance;
            checkbox.Checked.Should().Be(null);

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Delete", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));

            //Backspace should not change state on non-tristate checkbox
            comp.SetParam(x => x.TriState, null);
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));
            //Check tristate space key
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));

            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));

            comp.SetParam("Disabled", true);
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", });
            comp.WaitForAssertion(() => checkbox.Checked.Should().Be(null));
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
            var comp = Context.RenderComponent<MudCheckBox<bool>>(x => x.Add(c => c.Color, color).Add(b => b.UnCheckedColor, uncheckedcolor));

            var box = comp.Instance;
            var input = comp.Find("input");

            var checkboxClasses = comp.Find(".mud-button-root.mud-icon-button");
            // check initial state
            box.Checked.Should().Be(false);
            checkboxClasses.ClassList.Should().ContainInOrder(new[] { $"mud-{uncheckedcolor.ToDescriptionString()}-text", $"hover:mud-{uncheckedcolor.ToDescriptionString()}-hover" });

            // click and check if it has new color
            input.Change(true);
            box.Checked.Should().Be(true);
            checkboxClasses.ClassList.Should().ContainInOrder(new[] { $"mud-{color.ToDescriptionString()}-text", $"hover:mud-{color.ToDescriptionString()}-hover" });
        }

        [Test]
        public void CheckBoxDisabledTest()
        {
            var comp = Context.RenderComponent<CheckboxLabelExample>();
            var checkboxes = comp.FindAll("label.mud-checkbox");
            checkboxes[3].ClassList.Should().Contain("mud-disabled"); // 4rd checkbox
        }

        [Test]
        public void CheckBoxLabelPositionTest()
        {
            var comp = Context.RenderComponent<CheckboxLabelExample>();
            //Console.WriteLine(comp.Markup);
            var checkboxes = comp.FindAll("label.mud-checkbox");

            checkboxes[0].ClassList.Should().Contain("mud-ltr"); // 1st checkbox: (default) LabelPosition.End
            checkboxes[2].ClassList.Should().Contain("mud-rtl"); // 3rd checkbox: LabelPosition.Start
        }
    }
}
