// Copyright (c) MudBlazor 2022
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.NumericField;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class NumericFieldTests : BunitTest
    {
        // TestCaseSource does not know about "Nullable<T>" so having values as Nullable<T> does not make sense here
        static object[] TypeCases =
        {
            new object[] { (byte)5 },
            new object[] { (sbyte)5 },
            new object[] { (short)5 },
            new object[] { (ushort)5 },
            new object[] { (int)5 },
            new object[] { (uint)5 },
            new object[] { (long)5 },
            new object[] { (ulong)5 },
            new object[] { (float)5 },
            new object[] { (double)5 },
            new object[] { (decimal)5 }
        };

        /// <summary>
        /// Numeric Field id should propagate to label for attribute
        /// </summary>
        [Test]
        public void NumericFieldLabelFor()
        {
            var comp = Context.RenderComponent<NumericFieldTest>();
            var label = comp.FindAll(".mud-input-label");
            label[0].Attributes.GetNamedItem("for")?.Value.Should().Be("numericFieldLabelTest");
        }

        /// <summary>
        /// Initial Text for double should be 0, with F1 format it should be 0.0
        /// </summary>
        [Test]
        public async Task NumericFieldTest1()
        {
            var comp = Context.RenderComponent<MudNumericField<double>>();
            // print the generated html
            // select elements needed for the test
            var numericField = comp.Instance;
            numericField.Value.Should().Be(0.0);
            numericField.Text.Should().Be("0");
            //
            0.0.ToString("F1", CultureInfo.InvariantCulture).Should().Be("0.0");
            //
            await comp.InvokeAsync(() => numericField.Format = "F1");
            await comp.InvokeAsync(() => numericField.Culture = CultureInfo.InvariantCulture);
            numericField.Value.Should().Be(0.0);
            numericField.Text.Should().Be("0.0");
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        /// <summary>
        /// Initial Text for double? should be null
        /// </summary>
        [Test]
        public void NumericFieldTest2()
        {
            var comp = Context.RenderComponent<MudNumericField<double?>>();
            // print the generated html
            // select elements needed for the test
            var numericField = comp.Instance;
            numericField.Value.Should().Be(null);
            numericField.Text.Should().BeNullOrEmpty();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        /// <summary>
        /// Setting the value to null should not cause a validation error
        /// </summary>
        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericField_WithNullableTypes_ShouldAllowNulls<T>(T value) where T : struct
        {
            var comp = Context.RenderComponent<MudNumericField<T?>>(ComponentParameter.CreateParameter("Value", value));
            // print the generated html
            comp.SetParametersAndRender(ComponentParameter.CreateParameter("Value", null));
            comp.Find("input").Blur();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
            comp.Find("input").Change("");
            comp.Find("input").Blur();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        //This doesn't make any sense because you cannot set anything that's not a number
        ///// <summary>
        ///// Setting an invalid number should show the conversion error message
        ///// </summary>
        //[Test]
        //public async Task NumericFieldConversionError()
        //{
        //    var comp = ctx.RenderComponent<MudNumericField<int?>>();
        //    // print the generated html
        //    //Console.WriteLine(comp.Markup);
        //    comp.Find("input").Change("seventeen");
        //    comp.Find("input").Blur();
        //    //Console.WriteLine(comp.Markup);
        //    comp.FindAll("p.mud-input-error").Count.Should().Be(1);
        //    comp.Find("p.mud-input-error").TextContent.Trim().Should().Be("Not a valid number");
        //}

        /// <summary>
        /// If Debounce Interval is null or 0, Value should change immediately
        /// </summary>
        [Test]
        public void WithNoDebounceIntervalValueShouldChangeImmediatelyTest()
        {
            //no interval passed, so, by default is 0
            // We pass the Immediate parameter set to true, in order to bind to oninput
            var immediate = Parameter(nameof(MudNumericField<int?>.Immediate), true);
            var comp = Context.RenderComponent<MudNumericField<int?>>(immediate);
            var numericField = comp.Instance;
            var input = comp.Find("input");
            //Act
            input.Input(new ChangeEventArgs() { Value = "100" });
            //Assert
            //input value has changed, DebounceInterval is 0, so Value should change in NumericField immediately
            numericField.Value.Should().Be(100);
            numericField.Text.Should().Be("100");
        }

        /// <summary>
        /// Value should not change immediately. Should respect the Debounce Interval
        /// </summary>
        [Test]
        public async Task ShouldRespectDebounceIntervalPropertyInNumericFieldTest()
        {
            var interval = Parameter(nameof(MudNumericField<int?>.DebounceInterval), 200d);
            var comp = Context.RenderComponent<MudNumericField<int?>>(interval);
            var numericField = comp.Instance;
            var input = comp.Find("input");
            //Act
            input.Input(new ChangeEventArgs() { Value = "100" });
            //Assert
            //if DebounceInterval is set, Immediate should be true by default
            numericField.Immediate.Should().BeTrue();
            //input value has changed, but elapsed time is 0, so Value should not change in NumericField
            numericField.Value.Should().BeNull();
            numericField.Text.Should().Be("100");
            //DebounceInterval is 200 ms, so at 100 ms Value should not change in NumericField
            await Task.Delay(100);
            numericField.Value.Should().BeNull();
            numericField.Text.Should().Be("100");
            //More than 200 ms had elapsed, so Value should be updated
            await Task.Delay(150);
            numericField.Value.Should().Be(100);
            numericField.Text.Should().Be("100");
        }

        /// <summary>
        /// Label and placeholder should not overlap.
        /// When placeholder is set, label should shrink
        /// </summary>
        [Test]
        public void LabelShouldShrinkWhenPlaceholderIsSet()
        {
            //Arrange
            var label = Parameter(nameof(MudNumericField<int?>.Label), "label");
            var placeholder = Parameter(nameof(MudNumericField<int?>.Placeholder), "placeholder");
            //with no placeholder, label is not shrunk
            var comp = Context.RenderComponent<MudNumericField<int?>>(label);
            comp.Markup.Should().NotContain("shrink");
            //with placeholder label is shrunk
            comp.SetParametersAndRender(placeholder);
            comp.Markup.Should().Contain("shrink");
        }

        /// <summary>
        /// A glue class to make it easy to define validation rules for single values using FluentValidation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class FluentValueValidator<T> : AbstractValidator<T>
        {
            public FluentValueValidator(Action<IRuleBuilderInitial<T, T>> rule)
            {
                rule(RuleFor(x => x));
            }

            private IEnumerable<string> ValidateValue(T arg)
            {
                var result = Validate(arg);
                if (result.IsValid)
                    return Array.Empty<string>();
                return result.Errors.Select(e => e.ErrorMessage);
            }

            public Func<T, IEnumerable<string>> Validation => ValidateValue;
        }

        /// <summary>
        /// FluentValidation rules can be used for validating a NumericFields
        /// </summary>
        [Test]
        public async Task NumericFieldFluentValidationTest1()
        {
            var validator = new FluentValueValidator<string>(x => x.Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Length(1, 100));
            var comp = Context.RenderComponent<MudNumericField<decimal>>(Parameter(nameof(MudNumericField<decimal>.Validation), validator.Validation), Parameter(nameof(MudNumericField<decimal>.Max), 100M));
            var numericField = comp.Instance;
            // first try a valid value
            comp.Find("input").Change(99);
            numericField.Error.Should().BeFalse(because: "The value is < 100");
            numericField.ErrorText.Should().BeNullOrEmpty();
            // now try something that's outside of range
            comp.Find("input").Change("100.1");
            numericField.Error.Should().BeFalse(because: "The value should be set to Max (100)");
            numericField.Value.Should().Be(100M);
            numericField.ErrorText.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Validate handling of decimal support & precision kept
        /// </summary>
        [Test]
        public async Task NumericField_HandleDecimalPrecisionAndValues()
        {
            var comp = Context.RenderComponent<MudNumericField<decimal>>();
            var numericField = comp.Instance;

            // first try set max decimal value
            comp.Find("input").Change(decimal.MaxValue);
            numericField.Value.Should().Be(decimal.MaxValue);
            numericField.ErrorText.Should().BeNullOrEmpty();

            // next try set minimum decimal value
            comp.Find("input").Change(decimal.MinValue);
            numericField.Value.Should().Be(decimal.MinValue);
            numericField.ErrorText.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// An unstable converter should not cause an infinite update loop. This test must complete in under 1 sec!
        /// </summary>
        [Test, Timeout(1000)]
        public async Task NumericFieldUpdateLoopProtectionTest()
        {
            var comp = Context.RenderComponent<MudNumericField<int>>();
            // these conversion funcs are nonsense of course, but they are designed this way to
            // test against an infinite update loop that numericFields and other inputs are now protected against.
            var numericField = comp.Instance;
            numericField.Converter.SetFunc = s => s.ToString();
            numericField.Converter.GetFunc = s => int.Parse(s);
            comp.SetParametersAndRender(ComponentParameter.CreateParameter("Value", 1));
            numericField.Value.Should().Be(1);
            numericField.Text.Should().Be("1");
            comp.Find("input").Change("3");
            numericField.Value.Should().Be(3);
            numericField.Text.Should().Be("3");
        }

        [Test]
        public async Task NumericField_Should_FireValueChangedOnTextParameterChange()
        {
            var changed_value = 4;
            var comp = Context.RenderComponent<MudNumericField<int>>(EventCallback<int>("ValueChanged", x => changed_value = x));
            comp.SetParametersAndRender(ComponentParameter.CreateParameter("Text", "4"));
            changed_value.Should().Be(4);
        }

        [Test]
        public async Task NumericField_Should_FireTextChangedOnValueParameterChange()
        {
            var changed_text = "4";
            var comp = Context.RenderComponent<MudNumericField<int>>(EventCallback<string>("TextChanged", x => changed_text = x));
            comp.SetParametersAndRender(ComponentParameter.CreateParameter("Value", 4));
            changed_text.Should().Be("4");
        }

        [Test]
        public async Task NumericField_Should_FireTextAndValueChangedOnTextInput()
        {
            var changed_value = 4;
            string changed_text = null;
            var comp = Context.RenderComponent<MudNumericField<int>>(
                EventCallback<int>("ValueChanged", x => changed_value = x),
                EventCallback<string>("TextChanged", x => changed_text = x)
            );
            comp.Find("input").Change("4");
            changed_value.Should().Be(4);
            changed_text.Should().Be("4");
        }

        //This doesn't make any sense because you cannot set anything that's not a number
        ///// <summary>
        ///// Instead of RequiredError it should show the conversion error, because typing something (even if not a number) should
        ///// already fulfill the requirement of Required="true". If it is a valid value is a different question.
        ///// </summary>
        ///// <returns></returns>
        //[Test]
        //public async Task NumericField_ShouldNot_ShowRequiredErrorWhenThereIsAConversionError()
        //{
        //    var comp = ctx.RenderComponent<MudNumericField<int?>>(ComponentParameter.CreateParameter("Required", true));
        //    var numericField = comp.Instance;
        //    comp.Find("input").Change("A");
        //    comp.Find("input").Blur();
        //    numericField.Value.Should().BeNull();
        //    numericField.HasErrors.Should().Be(true);
        //    numericField.ErrorText.Should().Be("Not a valid number");
        //}

        /// <summary>
        /// Instead of RequiredError it should show the conversion error, because typing something (even if not a number) should
        /// already fulfill the requirement of Required="true". If it is a valid value is a different question.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task NumericField_ShouldNot_ShowRequiredErrorWhenInitialTextIsEmpty()
        {
            var comp = Context.RenderComponent<NumericFieldRequiredTest>();
            var numericField = comp.FindComponent<MudNumericField<int?>>().Instance;
            numericField.Touched.Should().BeFalse();
            numericField.ErrorText.Should().BeNullOrEmpty();
            numericField.HasErrors.Should().Be(false);
        }

        /// <summary>
        /// NumericField with any numeric type parameter should render.
        /// Test for decimal type moved to another method because it cannot be parameter for TestCaseAttribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericField_OfAnyType_Should_Render<T>(T value)
        {
            Assert.DoesNotThrow(() => Context.RenderComponent<MudNumericField<T>>(), $"{typeof(MudNumericField<>)}<{typeof(T)}> render failed.");
        }

        /// <summary>
        /// Increment / Decrement via up / down keys should work
        /// </summary>
        [Test]
        public async Task NumericFieldTest_KeyboardInput()
        {
            var comp = Context.RenderComponent<MudNumericField<double>>();
            comp.SetParam(x => x.Culture, CultureInfo.InvariantCulture);
            comp.SetParam(x => x.Format, "F2");
            comp.SetParam(x => x.Value, 1234.56);
            // print the generated html
            // select elements needed for the test
            var numericField = comp.Instance;
            numericField.Value.Should().Be(1234.56);
            numericField.Text.Should().Be("1234.56");
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", });
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keyup", });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1235.56));
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", });
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keyup", });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.56));
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "c", Type = "keydown", CtrlKey = false });
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "c", Type = "keyup", CtrlKey = false });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.56));
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "a", Type = "keydown", });
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "a", Type = "keyup", });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.56));
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "9", Type = "keydown", });
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "9", Type = "keyup", });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.56));
        }

        /// <summary>
        /// Keydown disabled, should not do anything
        /// </summary>
        [Test]
        public async Task NumericFieldTest_KeyboardInput_Disabled()
        {
            var comp = Context.RenderComponent<MudNumericField<double>>();
            comp.SetParam(x => x.Culture, CultureInfo.InvariantCulture);
            comp.SetParam(x => x.Format, "F2");
            comp.SetParam(x => x.Value, 1234.56);
            comp.SetParam(x => x.Disabled, true);
            comp.Instance.Value.Should().Be(1234.56);
            comp.Instance.Text.Should().Be("1234.56");
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", });
            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(1234.56));
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "9", Type = "keyup", });
            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(1234.56));
        }

        /// <summary>
        /// Keydown readonly, should not do anything
        /// </summary>
        [Test]
        public async Task NumericFieldTest_KeyboardInput_Readonly()
        {
            var comp = Context.RenderComponent<MudNumericField<double>>();
            comp.SetParam(x => x.Culture, CultureInfo.InvariantCulture);
            comp.SetParam(x => x.Format, "F2");
            comp.SetParam(x => x.Value, 1234.56);
            comp.SetParam(x => x.ReadOnly, true);
            comp.Instance.Value.Should().Be(1234.56);
            comp.Instance.Text.Should().Be("1234.56");
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", });
            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(1234.56));
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "9", Type = "keyup", });
            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(1234.56));
        }

        /// <summary>
        /// MouseWheel actions should work
        /// </summary>
        [Test]
        public async Task NumericFieldTest_MouseWheel()
        {
            var comp = Context.RenderComponent<MudNumericField<double>>();
            comp.SetParam(x => x.Value, 1234.56);
            var numericField = comp.Instance;

            //MouseWheel up
            comp.Find("input").MouseWheel(new WheelEventArgs() { DeltaY = -1, ShiftKey = true });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1235.56));

            //MouseWheel down
            comp.Find("input").MouseWheel(new WheelEventArgs() { DeltaY = 1, ShiftKey = true });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.56));

            //Invert MouseWheel
            numericField.InvertMouseWheel = true;

            //MouseWheel up
            comp.Find("input").MouseWheel(new WheelEventArgs() { DeltaY = -1, ShiftKey = true });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1233.56));

            //MouseWheel down
            comp.Find("input").MouseWheel(new WheelEventArgs() { DeltaY = 1, ShiftKey = true });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.56));

            //Try with different step
            numericField.Step = 0.5;

            //MouseWheel up
            comp.Find("input").MouseWheel(new WheelEventArgs() { DeltaY = -1, ShiftKey = true });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.06));

            //MouseWheel down
            comp.Find("input").MouseWheel(new WheelEventArgs() { DeltaY = 1, ShiftKey = true });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.56));

            //MouseWheel without Shift doesn't do anything
            comp.Find("input").MouseWheel(new WheelEventArgs() { DeltaY = 77, ShiftKey = false });
            comp.Find("input").MouseWheel(new WheelEventArgs() { DeltaY = -17, ShiftKey = false });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.56));
        }

        /// <summary>
        /// MouseWheel actions should work on Firefox
        /// </summary>
        [Test]
        public async Task NumericFieldTest_Wheel_Firefox()
        {
            var comp = Context.RenderComponent<MudNumericField<double>>();
            comp.SetParam(x => x.Value, 1234.56);
            var numericField = comp.Instance;

            //MouseWheel up
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = -1, ShiftKey = true });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1235.56));

            //MouseWheel down
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = 1, ShiftKey = true });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.56));

            //Invert MouseWheel
            numericField.InvertMouseWheel = true;

            //MouseWheel up
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = -1, ShiftKey = true });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1233.56));

            //MouseWheel down
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = 1, ShiftKey = true });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.56));

            //Try with different step
            numericField.Step = 0.5;

            //MouseWheel up
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = -1, ShiftKey = true });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.06));

            //MouseWheel down
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = 1, ShiftKey = true });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.56));

            //MouseWheel without Shift doesn't do anything
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = 77, ShiftKey = false });
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = -17, ShiftKey = false });
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234.56));
        }

        /// <summary>
        /// NumericalField Formats input according to culture
        /// </summary>
        [TestCase("English", "1234", "1,234.00", 1234.0)]
        [TestCase("English", "0", "0.00", 0.0)]
        [TestCase("English", "", null, null)]
        [TestCase("German", "1234", "1.234,00", 1234.0)]
        [TestCase("German", "0", "0,00", 0.0)]
        [TestCase("German", "", null, null)]
        [TestCase("French", "1234", "1\u202F234,00", 1234.0)]
        [TestCase("French", "0", "0,00", 0.0)]
        [TestCase("French", "", null, null)]
        public async Task NumericFieldTestCultureFormat(string inputName, string inputText, string expectedText, double? expectedValue)
        {
            var comp = Context.RenderComponent<NumericFieldCultureTest>();
            var inputs = comp.FindAll("input");
            var inputField = inputs.GetElementById(inputName);

            var propertiesOfTargetObject = comp.Instance.GetType().GetProperties().Where(x => x.Name == inputName).First();
            var fieldOfInterest = propertiesOfTargetObject.GetValue(comp.Instance);

            inputField.Change(inputText);
            inputField.Blur();
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Text.Should().Be(expectedText));
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Value.Should().Be(expectedValue));
        }

        [TestCase("English", "1234", "1,234.00", 1234.0)]
        [TestCase("English", "0", "0.00", 0.0)]
        [TestCase("English", "", null, null)]
        [TestCase("German", "1234", "1.234,00", 1234.0)]
        [TestCase("German", "0", "0,00", 0.0)]
        [TestCase("German", "", null, null)]
        [TestCase("French", "1234", "1\u202F234,00", 1234.0)]
        [TestCase("French", "0", "0,00", 0.0)]
        [TestCase("French", "", null, null)]
        public async Task NumericFieldImmediateTestCultureFormat(string inputName, string inputText, string expectedText, double? expectedValue)
        {
            var comp = Context.RenderComponent<NumericFieldImmediateCultureTest>();
            var inputs = comp.FindAll("input");
            var inputField = inputs.GetElementById(inputName);

            var propertiesOfTargetObject = comp.Instance.GetType().GetProperties().Where(x => x.Name == inputName).First();
            var fieldOfInterest = propertiesOfTargetObject.GetValue(comp.Instance);

            inputField.Input(inputText);
            inputField.Blur();
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Text.Should().Be(expectedText));
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Value.Should().Be(expectedValue));
        }

        /// <summary>
        /// NumericalField will not accept illegal chars
        /// </summary>
        [TestCase("German", "abcd", null, null)]
        [TestCase("German", "x+17,9y9z", null, null)]
        [TestCase("German", "17,99", "17,99", 17.99)]
        [TestCase("English", "abcd", null, null)]
        [TestCase("English", "-12-34abc.56", null, null)]
        [TestCase("English", "-1234.56", "-1,234.56", -1234.56)]
        [TestCase("French", "abcd", null, null)]
        [TestCase("French", "x+17,9y9z", null, null)]
        [TestCase("French", "17,99", "17,99", 17.99)]
        [TestCase("French", "1339,99", "1\u202F339,99", 1339.99)]
        public async Task NumericField_should_RejectIllegalCharacters(string inputName, string inputString, string? expectedText, double? expectedValue)
        {
            var comp = Context.RenderComponent<NumericFieldCultureTest>();
            var inputs = comp.FindAll("input");
            var field = inputs.GetElementById(inputName);

            var propertiesOfTargetObject = comp.Instance.GetType().GetProperties().Where(x => x.Name == inputName).First();
            var fieldOfInterest = propertiesOfTargetObject.GetValue(comp.Instance);
            //german
            field.Change(inputString);
            field.Blur();
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Text.Should().Be(expectedText));
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Value.Should().Be(expectedValue));
        }

        [TestCase("German", "abcd", null, null)]
        [TestCase("German", "x+17,9y9z", null, null)]
        [TestCase("German", "17,99", "17,99", 17.99)]
        [TestCase("English", "abcd", null, null)]
        [TestCase("English", "-12-34abc.56", null, null)]
        [TestCase("English", "-1234.56", "-1,234.56", -1234.56)]
        [TestCase("French", "abcd", null, null)]
        [TestCase("French", "x+17,9y9z", null, null)]
        [TestCase("French", "100.00", null, null)]
        [TestCase("French", "100,00", "100,00", 100.00)]
        [TestCase("French", "17,99", "17,99", 17.99)]
        [TestCase("French", "1339,99", "1\u202F339,99", 1339.99)]
        public async Task NumericFieldImmediate_should_RejectIllegalCharacters(string inputName, string inputString, string? expectedText, double? expectedValue)
        {
            var comp = Context.RenderComponent<NumericFieldImmediateCultureTest>();
            var inputs = comp.FindAll("input");
            var field = inputs.GetElementById(inputName);

            var propertiesOfTargetObject = comp.Instance.GetType().GetProperties().Where(x => x.Name == inputName).First();
            var fieldOfInterest = propertiesOfTargetObject.GetValue(comp.Instance);

            field.Input(inputString);
            field.Blur();
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Text.Should().Be(expectedText));
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Value.Should().Be(expectedValue));
        }

        [TestCase("English", "100.00", "100.00", 100.00)]
        [TestCase("English", "100,000.00", "100,000.00", 100000.00)]
        [TestCase("German", "100.000,00", "100.000,00", 100000.00)]
        [TestCase("German", "100,00", "100,00", 100.00)]
        [TestCase("French", "100.00", "100,00", 100.00)]
        [TestCase("French", "100,00", "100,00", 100.00)]
        [TestCase("French", "100000,00", "100\u202F000,00", 100000.00)]
        [TestCase("French", "100 000,00", "100\u202F000,00", 100000.00)]
        public async Task NumericFieldImmediateLocalKeyOverrideCultureTest_should_ConvertIllegalCharacters(string inputName, string inputString, string? expectedText, double? expectedValue)
        {
            var comp = Context.RenderComponent<NumericFieldImmediateLocalKeyOverrideCultureTest>();
            var inputs = comp.FindAll("input");
            var field = inputs.GetElementById(inputName);

            var propertiesOfTargetObject = comp.Instance.GetType().GetProperties().Where(x => x.Name == inputName).First();
            var fieldOfInterest = propertiesOfTargetObject.GetValue(comp.Instance);

            field.Input(inputString);
            field.Blur();
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Text.Should().Be(expectedText));
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Value.Should().Be(expectedValue));
        }

        [TestCase("English", "100.00", "100.00", 100.00)]
        [TestCase("English", "100,000.00", "100,000.00", 100000.00)]
        [TestCase("German", "100.000,00", "100.000,00", 100000.00)]
        [TestCase("German", "100,00", "100,00", 100.00)]
        [TestCase("French", "100.00", "100,00", 100.00)]
        [TestCase("French", "100,00", "100,00", 100.00)]
        [TestCase("French", "100000,00", "100\u202F000,00", 100000.00)]
        [TestCase("French", "100 000,00", "100\u202F000,00", 100000.00)]
        public async Task NumericFieldLocalKeyOverrideCultureTest_should_ConvertIllegalCharacters(string inputName, string inputString, string? expectedText, double? expectedValue)
        {
            var comp = Context.RenderComponent<NumericFieldLocalKeyOverrideCultureTest>();
            var inputs = comp.FindAll("input");
            var field = inputs.GetElementById(inputName);

            var propertiesOfTargetObject = comp.Instance.GetType().GetProperties().Where(x => x.Name == inputName).First();
            var fieldOfInterest = propertiesOfTargetObject.GetValue(comp.Instance);

            field.Change(inputString);
            field.Blur();
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Text.Should().Be(expectedText));
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Value.Should().Be(expectedValue));
        }

        [TestCase("English", "1,234.56", "1,234.56", 1234.56)]
        [TestCase("English", "1234.56", "1,234.56", 1234.56)]
        [TestCase("German", "7.000,99", "7.000,99", 7000.99)]
        [TestCase("German", "7000,99", "7.000,99", 7000.99)]
        [TestCase("French", "7\u202F000,99", "7\u202F000,99", 7000.99)]
        [TestCase("French", "7000,99", "7\u202F000,99", 7000.99)]
        public async Task NumericField_should_ReformatTextOnBlur(string inputName, string inputString, string? expectedText, double? expectedValue)
        {
            var comp = Context.RenderComponent<NumericFieldCultureTest>();
            var inputs = comp.FindAll("input");
            var field = inputs.GetElementById(inputName);

            var propertiesOfTargetObject = comp.Instance.GetType().GetProperties().Where(x => x.Name == inputName).First();
            var fieldOfInterest = propertiesOfTargetObject.GetValue(comp.Instance);

            field.Change(inputString);
            field.Blur();
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Text.Should().Be(expectedText));
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Value.Should().Be(expectedValue));
        }

        [TestCase("English", "1,234.56", "1,234.56", 1234.56)]
        [TestCase("English", "1234.56", "1,234.56", 1234.56)]
        [TestCase("German", "7.000,99", "7.000,99", 7000.99)]
        [TestCase("German", "7000,99", "7.000,99", 7000.99)]
        [TestCase("French", "7\u202F000,99", "7\u202F000,99", 7000.99)]
        [TestCase("French", "7000,99", "7\u202F000,99", 7000.99)]
        public async Task NumericFieldImmediate_should_ReformatTextOnBlur(string inputName, string inputString, string? expectedText, double? expectedValue)
        {
            var comp = Context.RenderComponent<NumericFieldImmediateCultureTest>();
            var inputs = comp.FindAll("input");
            var field = inputs.GetElementById(inputName);

            var propertiesOfTargetObject = comp.Instance.GetType().GetProperties().Where(x => x.Name == inputName).First();
            var fieldOfInterest = propertiesOfTargetObject.GetValue(comp.Instance);

            field.Input(inputString);
            field.Blur();
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Text.Should().Be(expectedText));
            comp.WaitForAssertion(() => (fieldOfInterest as MudNumericField<double?>).Value.Should().Be(expectedValue));
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericField_Validation<T>(T value)
        {
            var comp = Context.RenderComponent<MudNumericField<T>>();
            comp.SetParam(x => x.Max, value);
            comp.SetParam(x => x.Min, value);
            comp.SetParam(x => x.Value, value);
            var numericField = comp.Instance;
            numericField.Value.Should().Be(value);
            await comp.InvokeAsync(() =>
            {
                numericField.Validate().Wait();
            });
            numericField.Value.Should().Be(value);
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericFieldMinMax<T>(T value)
        {
            var min = (T)Convert.ChangeType(1, typeof(T));
            var max = (T)Convert.ChangeType(10, typeof(T));
            var comp = Context.RenderComponent<MudNumericField<T>>();
            comp.SetParam(x => x.Min, min);
            comp.SetParam(x => x.Max, max);

            comp.Find("input").Change("15");
            comp.Find("input").Blur();

            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(max));

            comp.Find("input").Change("0");
            comp.Find("input").Blur();

            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(min));
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericFieldMinMaxNullable<T>(T value) where T : struct
        {
            var min = (T)Convert.ChangeType(1, typeof(T));
            var max = (T)Convert.ChangeType(10, typeof(T));
            var comp = Context.RenderComponent<MudNumericField<T?>>();
            comp.SetParam(x => x.Min, min);
            comp.SetParam(x => x.Max, max);

            comp.Find("input").Change("15");
            comp.Find("input").Blur();

            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(max));

            comp.Find("input").Change("0");
            comp.Find("input").Blur();

            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(min));
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericField_Increment_Decrement<T>(T value)
        {
            var comp = Context.RenderComponent<MudNumericField<T>>();
            var max = Convert.ChangeType(10, typeof(T));
            var min = Convert.ChangeType(0, typeof(T));
            comp.SetParam(x => x.Max, max);
            comp.SetParam(x => x.Min, min);
            comp.SetParam(x => x.Step, value);
            comp.SetParam(x => x.Value, value);
            await comp.InvokeAsync(() => comp.Instance.Increment().Wait());
            await comp.InvokeAsync(() => comp.Instance.Decrement().Wait());
            comp.Instance.Value.Should().Be(value);
            // setting min and max to value will cover the boundary checking code
            comp.SetParam(x => x.Max, value);
            comp.SetParam(x => x.Min, value);
            await comp.InvokeAsync(() => comp.Instance.Increment().Wait());
            await comp.InvokeAsync(() => comp.Instance.Decrement().Wait());
            comp.Instance.Value.Should().Be(value);
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericFieldNullable_Increment_Decrement<T>(T value) where T : struct
        {
            var comp = Context.RenderComponent<MudNumericField<T?>>();
            var max = Convert.ChangeType(10, typeof(T));
            var min = Convert.ChangeType(0, typeof(T));
            comp.SetParam(x => x.Max, max);
            comp.SetParam(x => x.Min, min);
            comp.SetParam(x => x.Step, value);
            comp.SetParam(x => x.Value, value);
            await comp.InvokeAsync(() => comp.Instance.Increment().Wait());
            await comp.InvokeAsync(() => comp.Instance.Decrement().Wait());
            comp.Instance.Value.Should().Be(value);
            // setting min and max to value will cover the boundary checking code
            comp.SetParam(x => x.Max, value);
            comp.SetParam(x => x.Min, value);
            await comp.InvokeAsync(() => comp.Instance.Increment().Wait());
            await comp.InvokeAsync(() => comp.Instance.Decrement().Wait());
            comp.Instance.Value.Should().Be(value);
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericField_Increment_Decrement_OverflowHandled<T>(T value)
        {
            var comp = Context.RenderComponent<MudNumericField<T>>();
            comp.SetParam(x => x.Step, value);

            // test max overflow
            comp.SetParam(x => x.Value, comp.Instance.Max);
            await comp.InvokeAsync(() => comp.Instance.Increment().Wait());
            comp.Instance.Value.Should().Be(comp.Instance.Max);

            // test min overflow
            comp.SetParam(x => x.Value, comp.Instance.Min);
            await comp.InvokeAsync(() => comp.Instance.Decrement().Wait());
            comp.Instance.Value.Should().Be(comp.Instance.Min);
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericFieldNullable_Increment_Decrement_OverflowHandled<T>(T value) where T : struct
        {
            var comp = Context.RenderComponent<MudNumericField<T?>>();
            comp.SetParam(x => x.Step, value);

            // test max overflow
            comp.SetParam(x => x.Value, comp.Instance.Max);
            await comp.InvokeAsync(() => comp.Instance.Increment().Wait());
            comp.Instance.Value.Should().Be(comp.Instance.Max);

            // test min overflow
            comp.SetParam(x => x.Value, comp.Instance.Min);
            await comp.InvokeAsync(() => comp.Instance.Decrement().Wait());
            comp.Instance.Value.Should().Be(comp.Instance.Min);
        }

        /// <summary>
        /// NumericField with min/max set and nullable int can be cleared
        /// </summary>
        [TestCase(10, 20, 15)]
        [TestCase(-20, -10, -15)]
        public async Task NumericFieldCanBeCleared(int min, int max, int value)
        {
            var comp = Context.RenderComponent<MudNumericField<int?>>();
            comp.SetParam(x => x.Min, min);
            comp.SetParam(x => x.Max, max);
            comp.SetParam(x => x.Value, value);

            comp.Find("input").Change("");
            comp.Find("input").Blur();

            comp.WaitForAssertion(() => comp.Instance.Value.Should().BeNull());
        }

        /// <summary>
        /// Special format with currency format should not result in error
        /// </summary>
        [Test]
        public async Task NumericFieldWithCurrencyFormat()
        {
            var comp = Context.RenderComponent<MudNumericField<int?>>();
            comp.SetParam(x => x.Format, "€0");
            comp.SetParam(x => x.Culture, CultureInfo.InvariantCulture);
            // print the generated html
            // select elements needed for the test
            var numericField = comp.Instance;
            numericField.Value.Should().Be(null);
            numericField.Text.Should().Be(null);
            //
            77.ToString("€0", CultureInfo.InvariantCulture).Should().Be("€77");
            var conv = new DefaultConverter<int?>();
            conv.Format = "€0";
            conv.Culture = CultureInfo.InvariantCulture;
            conv.Set(77).Should().Be("€77");
            //
            comp.FindAll("input").First().Change("1234");
            comp.FindAll("input").First().Blur();
            comp.WaitForAssertion(() => numericField.Text.Should().Be("€1234"));
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1234));
        }

        /// <summary>
        /// Test that thousands separator is parsed properly
        /// </summary>
        [Test]
        public async Task NumericFieldThousandsSeparator()
        {
            var comp = Context.RenderComponent<MudNumericField<int?>>();
            var numericField = comp.Instance;

            numericField.Value.Should().Be(null);
            numericField.Text.Should().Be(null);

            // comma separator
            comp.SetParam(x => x.Culture, CultureInfo.InvariantCulture);
            comp.FindAll("input").First().Change("1,000");
            comp.FindAll("input").First().Blur();
            comp.WaitForAssertion(() => numericField.Text.Should().Be("1000"));
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1000));

            // period separator
            comp.SetParam(x => x.Culture, new CultureInfo("de-DE", false));
            comp.FindAll("input").First().Change("1.000");
            comp.FindAll("input").First().Blur();
            comp.WaitForAssertion(() => numericField.Text.Should().Be("1000"));
            comp.WaitForAssertion(() => numericField.Value.Should().Be(1000));
        }
    }
}
