// Copyright (c) MudBlazor 2022
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.Dummy;
using MudBlazor.UnitTests.TestComponents.NumericField;
using NUnit.Framework;

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
            var comp = Context.Render<NumericFieldTest>();
            var label = comp.FindAll(".mud-input-label");
            label[0].Attributes.GetNamedItem("for")?.Value.Should().Be("numericFieldLabelTest");
        }

        /// <summary>
        /// Initial Text for double should be 0, with F1 format it should be 0.0
        /// </summary>
        [Test]
        public async Task NumericFieldTest1()
        {
            var comp = Context.Render<MudNumericField<double>>();
            // print the generated html
            // select elements needed for the test
            var numericField = comp.Instance;
            numericField.ReadValue.Should().Be(0.0);
            numericField.ReadText.Should().Be("0");
            //
            0.0.ToString("F1", CultureInfo.InvariantCulture).Should().Be("0.0");
            //
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Format, "F1")
                .Add(x => x.Culture, CultureInfo.InvariantCulture));

            numericField.ReadValue.Should().Be(0.0);
            numericField.ReadText.Should().Be("0.0");
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        /// <summary>
        /// Initial Text for double? should be null
        /// </summary>
        [Test]
        public void NumericFieldTest2()
        {
            var comp = Context.Render<MudNumericField<double?>>();
            // print the generated html
            // select elements needed for the test
            var numericField = comp.Instance;
            numericField.ReadValue.Should().Be(null);
            numericField.ReadText.Should().BeNullOrEmpty();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        /// <summary>
        /// Setting the value to null should not cause a validation error
        /// </summary>
        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericField_WithNullableTypes_ShouldAllowNulls<T>(T value) where T : struct
        {
            var comp = Context.Render<MudNumericField<T?>>(parameters => parameters.Add(x => x.Value, value));
            // print the generated html
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, null));
            comp.Find("input").Blur();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
            comp.Find("input").Change("");
            comp.Find("input").Blur();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        /// <summary>
        /// If Debounce Interval is null or 0, Value should change immediately
        /// </summary>
        [Test]
        public void WithNoDebounceIntervalValueShouldChangeImmediatelyTest()
        {
            //no interval passed, so, by default is 0
            // We pass the Immediate parameter set to true, in order to bind to oninput
            var comp = Context.Render<MudNumericField<int?>>(parameters => parameters
                .Add(x => x.Immediate, true));
            var numericField = comp.Instance;
            var input = comp.Find("input");
            //Act
            input.Input(new ChangeEventArgs() { Value = "100" });
            //Assert
            //input value has changed, DebounceInterval is 0, so Value should change in NumericField immediately
            numericField.ReadValue.Should().Be(100);
            numericField.ReadText.Should().Be("100");
        }

        /// <summary>
        /// Value should not change immediately. Should respect the Debounce Interval
        /// </summary>
        [Test]
        public async Task ShouldRespectDebounceIntervalPropertyInNumericFieldTest()
        {
            var comp = Context.Render<MudNumericField<int?>>(parameters => parameters
                .Add(x => x.DebounceInterval, 200d));
            var numericField = comp.Instance;
            var input = comp.Find("input");
            //Act
            input.Input(new ChangeEventArgs() { Value = "100" });
            //Assert
            //if DebounceInterval is set, Immediate should be true by default
            numericField.Immediate.Should().BeTrue();
            //input value has changed, but elapsed time is 0, so Value should not change in NumericField
            numericField.ReadValue.Should().BeNull();
            numericField.ReadText.Should().Be("100");
            //DebounceInterval is 200 ms, so at 100 ms Value should not change in NumericField
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().NotBe(100), TimeSpan.FromMilliseconds(100));
            numericField.ReadValue.Should().BeNull();
            numericField.ReadText.Should().Be("100");
            //More than 200 ms had elapsed, so Value should be updated (CPU time will likely take more than 200ms)
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(100), TimeSpan.FromMilliseconds(300));
            numericField.ReadText.Should().Be("100");
        }

        /// <summary>
        /// Label and placeholder should not overlap.
        /// When placeholder is set, label should shrink
        /// </summary>
        [Test]
        public async Task LabelShouldShrinkWhenPlaceholderIsSet()
        {
            //Arrange
            //with no placeholder, label is not shrunk
            var comp = Context.Render<MudNumericField<int?>>(parameters => parameters
                .Add(x => x.Label, "label"));
            comp.Markup.Should().NotContain("shrink");
            //with placeholder label is shrunk
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Placeholder, "placeholder"));
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
        public void NumericFieldFluentValidationTest1()
        {
            var validator = new FluentValueValidator<string>(x => x.Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Length(1, 100));
            var comp = Context.Render<MudNumericField<decimal>>(parameters => parameters
                .Add(x => x.Validation, validator.Validation)
                .Add(x => x.Max, 100M));
            var numericField = comp.Instance;
            // first try a valid value
            comp.Find("input").Change(99);
            numericField.GetState(x => x.Error).Should().BeFalse(because: "The value is < 100");
            numericField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // now try something that's outside of range
            comp.Find("input").Change("100.1");
            numericField.GetState(x => x.Error).Should().BeFalse(because: "The value should be set to Max (100)");
            numericField.ReadValue.Should().Be(100M);
            numericField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Validate handling of decimal support & precision kept
        /// </summary>
        [Test]
        public void NumericField_HandleDecimalPrecisionAndValues()
        {
            var comp = Context.Render<MudNumericField<decimal>>();
            var numericField = comp.Instance;

            // first try set max decimal value
            comp.Find("input").Change(decimal.MaxValue);
            numericField.ReadValue.Should().Be(decimal.MaxValue);
            numericField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            // next try set minimum decimal value
            comp.Find("input").Change(decimal.MinValue);
            numericField.ReadValue.Should().Be(decimal.MinValue);
            numericField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
        }

        /// <summary>
        /// An unstable converter should not cause an infinite update loop. This test must complete in under 1 sec!
        /// </summary>
        [Test, CancelAfter(1000)]
        public async Task NumericFieldUpdateLoopProtectionTest()
        {
            var comp = Context.Render<MudNumericField<int>>(parameters => parameters
                .Add(x => x.Converter, Conversions.From((int s) => s.ToString(), int.Parse)));
            // these conversion funcs are nonsense of course, but they are designed this way to
            // test against an infinite update loop that numericFields and other inputs are now protected against.
            var numericField = comp.Instance;
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, 1));
            numericField.ReadValue.Should().Be(1);
            numericField.ReadText.Should().Be("1");
            comp.Find("input").Change("3");
            numericField.ReadValue.Should().Be(3);
            numericField.ReadText.Should().Be("3");
        }

        [Test]
        public async Task NumericField_Should_FireValueChangedOnTextParameterChange()
        {
            var changed_value = 4;
            var comp = Context.Render<MudNumericField<int>>(parameters => parameters
                .Add(x => x.ValueChanged, x => changed_value = x));
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Text, "4"));
            changed_value.Should().Be(4);
        }

        [Test]
        public async Task NumericField_Should_FireTextChangedOnValueParameterChange()
        {
            var changed_text = "4";
            var comp = Context.Render<MudNumericField<int>>(parameters => parameters
                .Add(x => x.TextChanged, x => changed_text = x));
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, 4));
            changed_text.Should().Be("4");
        }

        [Test]
        public void NumericField_Should_FireTextAndValueChangedOnTextInput()
        {
            var changed_value = 4;
            string changed_text = null;
            var comp = Context.Render<MudNumericField<int>>(parameters => parameters
                .Add(x => x.ValueChanged, x => changed_value = x)
                .Add(x => x.TextChanged, x => changed_text = x));
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
        //    var comp = ctx.RenderComponent<MudNumericField<int?>>(parameters => parameters.Add(p => p.Required, true));
        //    var numericField = comp.Instance;
        //    comp.Find("input").Change("A");
        //    comp.Find("input").Blur();
        //    numericField.ReadValue.Should().BeNull();
        //    numericField.HasErrors.Should().Be(true);
        //    numericField.GetState(x => x.ErrorText).Should().Be("Not a valid number");
        //}

        /// <summary>
        /// Instead of RequiredError it should show the conversion error, because typing something (even if not a number) should
        /// already fulfill the requirement of Required="true". If it is a valid value is a different question.
        /// </summary>
        /// <returns></returns>
        [Test]
        public void NumericField_ShouldNot_ShowRequiredErrorWhenInitialTextIsEmpty()
        {
            var comp = Context.Render<NumericFieldRequiredTest>();
            var numericField = comp.FindComponent<MudNumericField<int?>>().Instance;
            numericField.Touched.Should().BeFalse();
            numericField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
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
        public void NumericField_OfAnyType_Should_Render<T>(T value)
        {
            Assert.DoesNotThrow(() => Context.Render<MudNumericField<T>>(), $"{typeof(MudNumericField<>)}<{typeof(T)}> render failed.");
        }

        /// <summary>
        /// Increment / Decrement via up / down keys should work
        /// </summary>
        [Test]
        public async Task NumericFieldTest_KeyboardInput()
        {
            var comp = Context.Render<MudNumericField<double>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Culture, CultureInfo.InvariantCulture)
                .Add(x => x.Format, "F2")
                .Add(x => x.Value, 1234.56));
            // print the generated html
            // select elements needed for the test
            var numericField = comp.Instance;
            numericField.ReadValue.Should().Be(1234.56);
            numericField.ReadText.Should().Be("1234.56");
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", });
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keyup", });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1235.56));
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", });
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keyup", });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.56));
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "c", Type = "keydown", CtrlKey = false });
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "c", Type = "keyup", CtrlKey = false });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.56));
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "a", Type = "keydown", });
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "a", Type = "keyup", });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.56));
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "9", Type = "keydown", });
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "9", Type = "keyup", });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.56));
        }

        /// <summary>
        /// KeyDown disabled, should not do anything
        /// </summary>
        [Test]
        public async Task NumericFieldTest_KeyboardInput_Disabled()
        {
            var comp = Context.Render<MudNumericField<double>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Culture, CultureInfo.InvariantCulture)
                .Add(x => x.Format, "F2")
                .Add(x => x.Value, 1234.56)
                .Add(x => x.Disabled, true));
            comp.Instance.ReadValue.Should().Be(1234.56);
            comp.Instance.ReadText.Should().Be("1234.56");
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(1234.56));
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "9", Type = "keyup", });
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(1234.56));
        }

        /// <summary>
        /// KeyDown readonly, should not do anything
        /// </summary>
        [Test]
        public async Task NumericFieldTest_KeyboardInput_Readonly()
        {
            var comp = Context.Render<MudNumericField<double>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Culture, CultureInfo.InvariantCulture)
                .Add(x => x.Format, "F2")
                .Add(x => x.Value, 1234.56)
                .Add(x => x.ReadOnly, true));
            comp.Instance.ReadValue.Should().Be(1234.56);
            comp.Instance.ReadText.Should().Be("1234.56");
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(1234.56));
            comp.Find("input").KeyUp(new KeyboardEventArgs() { Key = "9", Type = "keyup", });
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(1234.56));
        }

        /// <summary>
        /// MouseWheel actions should work
        /// </summary>
        [Test]
        public async Task NumericFieldTest_MouseWheel()
        {
            var comp = Context.Render<MudNumericField<double>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, 1234.56));
            var numericField = comp.Instance;

            //MouseWheel up
            await comp.Find("input").WheelAsync(new WheelEventArgs() { DeltaY = -1, ShiftKey = true });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1235.56));

            //MouseWheel down
            await comp.Find("input").WheelAsync(new WheelEventArgs() { DeltaY = 1, ShiftKey = true });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.56));

            //Invert MouseWheel
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.InvertMouseWheel, true));

            //MouseWheel up
            await comp.Find("input").WheelAsync(new WheelEventArgs() { DeltaY = -1, ShiftKey = true });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1233.56));

            //MouseWheel down
            await comp.Find("input").WheelAsync(new WheelEventArgs() { DeltaY = 1, ShiftKey = true });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.56));

            //Try with different step
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Step, 0.5));

            //MouseWheel up
            await comp.Find("input").WheelAsync(new WheelEventArgs() { DeltaY = -1, ShiftKey = true });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.06));

            //MouseWheel down
            await comp.Find("input").WheelAsync(new WheelEventArgs() { DeltaY = 1, ShiftKey = true });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.56));

            //MouseWheel without Shift doesn't do anything
            await comp.Find("input").WheelAsync(new WheelEventArgs() { DeltaY = 77, ShiftKey = false });
            await comp.Find("input").WheelAsync(new WheelEventArgs() { DeltaY = -17, ShiftKey = false });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.56));
        }

        /// <summary>
        /// MouseWheel actions should work on Firefox
        /// </summary>
        [Test]
        public async Task NumericFieldTest_Wheel_Firefox()
        {
            var comp = Context.Render<MudNumericField<double>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, 1234.56));
            var numericField = comp.Instance;

            //MouseWheel up
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = -1, ShiftKey = true });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1235.56));

            //MouseWheel down
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = 1, ShiftKey = true });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.56));

            //Invert MouseWheel
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.InvertMouseWheel, true));

            //MouseWheel up
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = -1, ShiftKey = true });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1233.56));

            //MouseWheel down
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = 1, ShiftKey = true });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.56));

            //Try with different step
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Step, 0.5));

            //MouseWheel up
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = -1, ShiftKey = true });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.06));

            //MouseWheel down
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = 1, ShiftKey = true });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.56));

            //MouseWheel without Shift doesn't do anything
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = 77, ShiftKey = false });
            comp.Find("input").Wheel(new WheelEventArgs() { DeltaY = -17, ShiftKey = false });
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234.56));
        }

        /// <summary>
        /// NumericalField Formats input according to culture
        /// </summary>
        [Test]
        public async Task NumericFieldTestCultureFormat()
        {
            var comp = Context.Render<NumericFieldCultureTest>();
            IElement Immediate() => comp.Find("#immediate");
            IElement NotImmediate() => comp.Find("#notImmediate");

            //german
            NotImmediate().Change("1234");
            await NotImmediate().BlurAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadText.Should().Be("1.234,00"));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadValue.Should().Be(1234.0));
            NotImmediate().GetAttribute("type").Should().Be("text");
            NotImmediate().GetAttribute("value").Should().Be("1.234,00");
            NotImmediate().Change("0");
            await NotImmediate().BlurAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadText.Should().Be("0,00"));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadValue.Should().Be(0.0));
            NotImmediate().Change("");
            await NotImmediate().BlurAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadText.Should().Be(null));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadValue.Should().Be(null));
            // English
            Immediate().Input("1234");
            await Immediate().BlurAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadText.Should().Be("1,234.00"));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadValue.Should().Be(1234.0));
            Immediate().GetAttribute("type").Should().Be("text");
            Immediate().GetAttribute("value").Should().Be("1,234.00");
            Immediate().Input("0");
            await Immediate().BlurAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadText.Should().Be("0.00"));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadValue.Should().Be(0.0));
            Immediate().Input("");
            await Immediate().BlurAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadText.Should().Be(null));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadValue.Should().Be(null));
        }

        /// <summary>
        /// NumericalField will not accept illegal chars
        /// </summary>
        [Test]
        public async Task NumericField_should_RejectIllegalCharacters()
        {
            var comp = Context.Render<NumericFieldCultureTest>();
            //german
            comp.FindAll("input").Last().Change("abcd");
            comp.FindAll("input").Last().Blur();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadText.Should().Be(null));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadValue.Should().Be(null));
            // English
            comp.FindAll("input").First().Input("abcd");
            comp.FindAll("input").First().Blur();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadText.Should().Be(null));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadValue.Should().Be(null));
            // English
            comp.FindAll("input").First().Input("-12-34abc.56");
            comp.FindAll("input").First().Blur();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadText.Should().Be(null));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadValue.Should().Be(null));
            comp.FindAll("input").First().Input("-1234.56");
            comp.FindAll("input").First().Blur();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadText.Should().Be("-1,234.56"));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadValue.Should().Be(-1234.56));
            comp.FindAll("input").Last().Change("x+17,9y9z");
            comp.FindAll("input").Last().Blur();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadText.Should().Be(null));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadValue.Should().Be(null));
            comp.FindAll("input").Last().Change("17,99");
            comp.FindAll("input").Last().Blur();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadText.Should().Be("17,99"));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadValue.Should().Be(17.99));
        }

        [Test]
        public async Task NumericField_should_ReformatTextOnBlur()
        {
            var comp = Context.Render<NumericFieldCultureTest>();
            // english
            comp.FindAll("input").First().Input("1,234.56");
            comp.FindAll("input").First().Blur();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadText.Should().Be("1,234.56"));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadValue.Should().Be(1234.56));
            comp.FindAll("input").First().Input("1234.56");
            comp.FindAll("input").First().Blur();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadText.Should().Be("1,234.56"));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadValue.Should().Be(1234.56));
            // german
            comp.FindAll("input").Last().Change("7.000,99");
            comp.FindAll("input").Last().Blur();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadText.Should().Be("7.000,99"));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadValue.Should().Be(7000.99));
            comp.FindAll("input").Last().Change("7000,99");
            comp.FindAll("input").Last().Blur();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadText.Should().Be("7.000,99"));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadValue.Should().Be(7000.99));
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericField_Validation<T>(T value)
        {
            var comp = Context.Render<MudNumericField<T>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Max, value)
                .Add(x => x.Min, value)
                .Add(x => x.Value, value));
            var numericField = comp.Instance;
            numericField.ReadValue.Should().Be(value);
            await comp.InvokeAsync(numericField.ValidateAsync);
            numericField.ReadValue.Should().Be(value);
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericFieldMinMax<T>(T value)
        {
            var min = (T)Convert.ChangeType(1, typeof(T));
            var max = (T)Convert.ChangeType(10, typeof(T));
            var comp = Context.Render<MudNumericField<T>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Min, min)
                .Add(x => x.Max, max));

            comp.Find("input").Change("15");
            comp.Find("input").Blur();

            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(max));

            comp.Find("input").Change("0");
            comp.Find("input").Blur();

            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(min));
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericFieldMinMaxNullable<T>(T value) where T : struct
        {
            var min = (T)Convert.ChangeType(1, typeof(T));
            var max = (T)Convert.ChangeType(10, typeof(T));
            var comp = Context.Render<MudNumericField<T?>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Min, min)
                .Add(x => x.Max, max));

            comp.Find("input").Change("15");
            comp.Find("input").Blur();

            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(max));

            comp.Find("input").Change("0");
            comp.Find("input").Blur();

            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(min));
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericField_Increment_Decrement<T>(T value)
        {
            var comp = Context.Render<MudNumericField<T>>();
            var max = Convert.ChangeType(10, typeof(T));
            var min = Convert.ChangeType(0, typeof(T));
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Max, (T)max)
                .Add(x => x.Min, (T)min)
                .Add(x => x.Step, value)
                .Add(x => x.Value, value));
            await comp.InvokeAsync(() => comp.Instance.Increment());
            await comp.InvokeAsync(() => comp.Instance.Decrement());
            comp.Instance.ReadValue.Should().Be(value);
            // setting min and max to value will cover the boundary checking code
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Max, value)
                .Add(x => x.Min, value));
            await comp.InvokeAsync(() => comp.Instance.Increment());
            await comp.InvokeAsync(() => comp.Instance.Decrement());
            comp.Instance.ReadValue.Should().Be(value);
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericFieldNullable_Increment_Decrement<T>(T value) where T : struct
        {
            var comp = Context.Render<MudNumericField<T?>>();
            var max = Convert.ChangeType(10, typeof(T));
            var min = Convert.ChangeType(0, typeof(T));
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Max, (T?)max)
                .Add(x => x.Min, (T?)min)
                .Add(x => x.Step, value)
                .Add(x => x.Value, value));
            await comp.InvokeAsync(() => comp.Instance.Increment());
            await comp.InvokeAsync(() => comp.Instance.Decrement());
            comp.Instance.ReadValue.Should().Be(value);
            // setting min and max to value will cover the boundary checking code
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Max, value)
                .Add(x => x.Min, value));
            await comp.InvokeAsync(() => comp.Instance.Increment());
            await comp.InvokeAsync(() => comp.Instance.Decrement());
            comp.Instance.ReadValue.Should().Be(value);
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericFieldNullable_NoMinMax_Increment_Decrement<T>(T value) where T : struct
        {
            var comp = Context.Render<MudNumericField<T?>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Step, value));

            await comp.InvokeAsync(() => comp.Instance.Increment());
            comp.Instance.ReadValue.Should().Be(value);

            comp.Find("input").Change("");

            if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
                value = Num.To<T>(0);
            else
                value = (T)Convert.ChangeType(-Convert.ToDouble(value), typeof(T));

            await comp.InvokeAsync(() => comp.Instance.Decrement());
            comp.Instance.ReadValue.Should().Be(value);
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericField_Increment_Decrement_OverflowHandled<T>(T value)
        {
            var comp = Context.Render<MudNumericField<T>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Step, value));

            // test max overflow
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, comp.Instance.Max));
            await comp.InvokeAsync(() => comp.Instance.Increment());
            comp.Instance.ReadValue.Should().Be(comp.Instance.Max);

            // test min overflow
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, comp.Instance.Min));
            await comp.InvokeAsync(() => comp.Instance.Decrement());
            comp.Instance.ReadValue.Should().Be(comp.Instance.Min);
        }

        [TestCaseSource(nameof(TypeCases))]
        public async Task NumericFieldNullable_Increment_Decrement_OverflowHandled<T>(T value) where T : struct
        {
            var comp = Context.Render<MudNumericField<T?>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Step, value));

            // test max overflow
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, comp.Instance.Max));
            await comp.InvokeAsync(() => comp.Instance.Increment());
            comp.Instance.ReadValue.Should().Be(comp.Instance.Max);

            // test min overflow
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, comp.Instance.Min));
            await comp.InvokeAsync(() => comp.Instance.Decrement());
            comp.Instance.ReadValue.Should().Be(comp.Instance.Min);
        }

        /// <summary>
        /// NumericField with min/max set and nullable int can be cleared
        /// </summary>
        [TestCase(10, 20, 15)]
        [TestCase(-20, -10, -15)]
        public async Task NumericFieldCanBeCleared(int min, int max, int value)
        {
            var comp = Context.Render<MudNumericField<int?>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Min, min)
                .Add(x => x.Max, max)
                .Add(x => x.Value, value));

            comp.Find("input").Change("");
            comp.Find("input").Blur();

            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().BeNull());
        }

        /// <summary>
        /// Special format with currency format should not result in error
        /// </summary>
        [Test]
        public async Task NumericFieldWithCurrencyFormat()
        {
            var comp = Context.Render<MudNumericField<int?>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Format, "€0")
                .Add(x => x.Culture, CultureInfo.InvariantCulture));
            // print the generated html
            // select elements needed for the test
            var numericField = comp.Instance;
            numericField.ReadValue.Should().Be(null);
            numericField.ReadText.Should().Be(null);
            //
            77.ToString("€0", CultureInfo.InvariantCulture).Should().Be("€77");
            var conv = new DefaultConverter<int?>()
            {
                Culture = () => CultureInfo.InvariantCulture,
                Format = () => "€0"
            };
            conv.Convert(77).Should().Be("€77");
            //
            comp.FindAll("input").First().Change("1234");
            comp.FindAll("input").First().Blur();
            await comp.WaitForAssertionAsync(() => numericField.ReadText.Should().Be("€1234"));
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1234));
        }

        /// <summary>
        /// Test that thousands separator is parsed properly
        /// </summary>
        [Test]
        public async Task NumericFieldThousandsSeparator()
        {
            var comp = Context.Render<MudNumericField<int?>>();
            var numericField = comp.Instance;

            numericField.ReadValue.Should().Be(null);
            numericField.ReadText.Should().Be(null);

            // comma separator
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Culture, CultureInfo.InvariantCulture));
            comp.FindAll("input").First().Change("1,000");
            comp.FindAll("input").First().Blur();
            await comp.WaitForAssertionAsync(() => numericField.ReadText.Should().Be("1000"));
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1000));

            // period separator
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Culture, new CultureInfo("de-DE", false)));
            comp.FindAll("input").First().Change("1.000");
            comp.FindAll("input").First().Blur();
            await comp.WaitForAssertionAsync(() => numericField.ReadText.Should().Be("1000"));
            await comp.WaitForAssertionAsync(() => numericField.ReadValue.Should().Be(1000));
        }

        /// <summary>
        /// Validate that a re-render of a debounced numeric field does not cause a loss of uncommitted text.
        /// </summary>
        [Test]
        public async Task DebouncedNumericFieldRerenderTest()
        {
            var comp = Context.Render<DebouncedNumericFieldRerenderTest>();
            var numericField = comp.FindComponent<MudNumericField<int>>().Instance;
            IElement DelayedRerenderButton() => comp.Find("button#re-render");
            IElement Input() => comp.Find("input");
            var converter = new DefaultConverter<int>();
            await Input().InputAsync("1");
            // trigger first value change
            await Task.Delay(comp.Instance.DebounceInterval);
            // trigger delayed re-render
            await DelayedRerenderButton().ClickAsync();
            // imitate "typing in progress" by extending the debounce interval until component re-renders
            var elapsedTime = 0;
            var currentText = "1";
            while (elapsedTime < comp.Instance.RerenderDelay)
            {
                var delay = comp.Instance.DebounceInterval / 2;
                currentText += "2";
                await Input().InputAsync(currentText);
                await Task.Delay(delay);
                elapsedTime += delay;
            }
            // after the final debounce, the value should be updated without swallowing any user input
            await Task.Delay(comp.Instance.DebounceInterval);
            comp.Instance.Value.Should().Be(converter.ConvertBack(currentText));
            numericField.ReadText.Should().Be(currentText);
        }

        [Test]
        public void DebouncedNumericField_Should_RenderDefaultValueTextOnFirstRender()
        {
            var defaultValue = 1;
            var converter = new DefaultConverter<int>();
            var comp = Context.Render<DebouncedNumericFieldRerenderTest>(parameters => parameters
                .Add(x => x.Value, defaultValue));
            var textfield = comp.FindComponent<MudNumericField<int>>().Instance;
            textfield.ReadText.Should().Be(converter.Convert(defaultValue));
        }

        /// <summary>
        /// Validate that a re-render of a debounced numeric field does not cause a loss of uncommitted text while changing culture.
        /// </summary>
        [Test]
        public async Task DebouncedNumericFieldCultureChangeRerenderTest()
        {
            var comp = Context.Render<DebouncedNumericFieldCultureChangeRerenderTest>();
            var numericField = comp.FindComponent<MudNumericField<double>>().Instance;
            var delayedCultureChange = comp.Find("button#culture-change");
            // ensure text is updated on initialize
            numericField.ReadText.Should().Be(comp.Instance.Value.ToString(comp.Instance.Format, comp.Instance.Culture));
            // trigger first value change
            await Task.Delay(comp.Instance.DebounceInterval);
            // trigger the culture change
            delayedCultureChange.Click();
            // imitate "typing in progress" by extending the debounce interval until component re-renders
            var elapsedTime = 0;
            var currentText = comp.Instance.Value.ToString(comp.Instance.Format, comp.Instance.Culture);
            while (elapsedTime < comp.Instance.RerenderDelay)
            {
                var delay = comp.Instance.DebounceInterval / 2;
                currentText += "2";
                comp.Find("input").Input(new ChangeEventArgs { Value = currentText });
                await Task.Delay(delay);
                elapsedTime += delay;
            }
            // after the culture change delay has elapsed, the uncommitted text is retained (with the old culture)
            numericField.ReadText.Should().Be(currentText);
            // once debounce occurs, both value and text are translated into the new culture
            // e.g. 1.00222222 (one comma something in en-US) turns into 100.222.222 (hundred million something in de-DE)
            await Task.Delay(comp.Instance.DebounceInterval * 2);
            numericField.ReadText.Should().Be(comp.Instance.Value.ToString(comp.Instance.Format, comp.Instance.Culture));
        }

        /// <summary>
        /// A numeric field with a label should auto-generate an id and use that id on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void NumericFieldWithLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.Render<MudNumericField<int>>(parameters
                => parameters.Add(p => p.Label, "Test Label"));

            comp.Find("input").Id.Should().NotBeNullOrEmpty();
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(comp.Find("input").Id);
        }

        /// <summary>
        /// A numeric field with a label and UserAttributesId should use the UserAttributesId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void NumericFieldWithLabelAndUserAttributesId_Should_UseUserAttributesIdForInputAndAccompanyingLabel()
        {
            var expectedId = "userattributes-id";
            var comp = Context.Render<MudNumericField<int>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes, new Dictionary<string, object>
                    {
                        { "Id", expectedId }
                    }));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// A numeric field with a label, a UserAttributesId, and an InputId should use the InputId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void NumericFieldWithLabelAndUserAttributesIdAndInputId_Should_UseInputIdForInputAndAccompanyingLabel()
        {
            var expectedId = "input-id";
            var comp = Context.Render<MudNumericField<int>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes, new Dictionary<string, object>
                    {
                        { "Id", "userattributes-id" }
                    })
                    .Add(p => p.InputId, expectedId));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// Optional NumericField should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalNumericField_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.Render<MudNumericField<int>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required NumericField should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredNumericField_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<MudNumericField<int>>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required NumericField attributes should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredNumericFieldAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<MudNumericField<int>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        [Test]
        public async Task Should_render_appropriate_type()
        {
            var comp = Context.Render<NumericFieldRenderTest>();
            var field = comp.Find("#num-field-id");

            comp.Markup.Should().NotContain("pattern");
            field.GetAttribute("type").Should().Be("number");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.UsePattern, true));
            comp.Markup.Should().Contain("pattern");
            field.GetAttribute("type").Should().Be("text");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.UsePattern, false));

            comp.Markup.Should().NotContain("pattern");
            field.GetAttribute("type").Should().Be("number");
        }

        [Test]
        [SetUICulture("ru-RU")]
        public async Task Should_ignore_default_culture()
        {
            var comp = Context.Render<NumericFieldRenderTest>();
            var numericField = comp.FindComponent<MudNumericField<decimal>>();

            comp.Find("input").Change("123.45");
            comp.Find("input").Blur();

            await comp.WaitForAssertionAsync(() => comp.Instance.Value.Should().Be(123.45M));
            numericField.Instance.ReadText.Should().Be("123.45");
            numericField.Instance.GetState(x => x.Culture).Name.Should().Be("");
        }

        [Test]
        [SetUICulture("ru-RU")]
        public async Task Format_should_use_default_culture()
        {
            var comp = Context.Render<MudNumericField<decimal>>(parameters => parameters
                                .Add(p => p.Format, "N3"));

            comp.Find("input").Change("123,45");
            comp.Find("input").Blur();

            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(123.45M));
            comp.Instance.ReadText.Should().Be("123,450");
            comp.Instance.Culture.Name.Should().Be("ru-RU");
        }

        [Test]
        [SetUICulture("ru-RU")]
        public async Task Pattern_should_use_default_culture()
        {
            var comp = Context.Render<MudNumericField<decimal>>(parameters => parameters
                                .Add(p => p.Pattern, "[0-9,.\\-]"));

            comp.Find("input").Change("123,45");
            comp.Find("input").Blur();

            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(123.45M));
            comp.Instance.ReadText.Should().Be("123,45");
            comp.Instance.GetState(x => x.Culture).Name.Should().Be("ru-RU");
        }

        [Test]
        [SetUICulture("ru-RU")]
        public async Task Should_apply_defined_culture()
        {
            var comp = Context.Render<NumericFieldCultureTest>();
            IElement Immediate() => comp.Find("#immediate");
            IElement NotImmediate() => comp.Find("#notImmediate");

            //german
            NotImmediate().Change("1.234,56");
            await NotImmediate().BlurAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadText.Should().Be("1.234,56"));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldNotImmediate.ReadValue.Should().Be(1234.56));
            comp.Instance.FieldNotImmediate.Culture.Name.Should().Be("de-DE");

            // English
            Immediate().Input("1234.56");
            await Immediate().BlurAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadText.Should().Be("1,234.56"));
            await comp.WaitForAssertionAsync(() => comp.Instance.FieldImmediate.ReadValue.Should().Be(1234.56));
            comp.Instance.FieldImmediate.Culture.Name.Should().Be("en-US");
        }

        [Test]
        [SetUICulture("ru-RU")]
        public async Task Format_should_use_defined_culture()
        {
            var comp = Context.Render<MudNumericField<decimal>>(parameters => parameters
                                .Add(p => p.Format, "N3")
                                .Add(p => p.Culture, CultureInfo.GetCultureInfo("en-US")));

            comp.Find("input").Change("123.45");
            comp.Find("input").Blur();

            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(123.45M));
            comp.Instance.ReadText.Should().Be("123.450");
            comp.Instance.Culture.Name.Should().Be("en-US");
        }

        [Test]
        [SetUICulture("ru-RU")]
        public async Task Pattern_should_use_defined_culture()
        {
            var comp = Context.Render<MudNumericField<decimal>>(parameters => parameters
                                .Add(p => p.Pattern, "[0-9,.\\-]")
                                .Add(p => p.Culture, CultureInfo.GetCultureInfo("en-US")));

            comp.Find("input").Change("123.45");
            comp.Find("input").Blur();

            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(123.45M));
            comp.Instance.ReadText.Should().Be("123.45");
            comp.Instance.Culture.Name.Should().Be("en-US");
        }

        [Test]
        public void Should_render_conversion_error_message()
        {
            var comp = Context.Render<MudNumericField<int>>(parameters => parameters
                .Add(p => p.ErrorId, "error-id")
                .Add(p => p.Text, "not a number")
                .Add(p => p.Converter, new DummyErrorConverter()));

            comp.Instance.ConversionErrorMessage.Should().NotBeNullOrEmpty();
            comp.Find("#error-id").InnerHtml.Should().Be(comp.Instance.ConversionErrorMessage);
        }

        [TestCase(Adornment.Start)]
        [TestCase(Adornment.End)]
        public void Should_render_aria_label_for_adornment_if_provided(Adornment adornment)
        {
            var ariaLabel = "the aria label";
            var comp = Context.Render<MudNumericField<int>>(parameters => parameters
                .Add(p => p.Adornment, adornment)
                .Add(p => p.AdornmentIcon, Icons.Material.Filled.Accessibility)
                .Add(p => p.AdornmentAriaLabel, ariaLabel));

            comp.Find(".mud-input-adornment-icon").Attributes.GetNamedItem("aria-label")!.Value.Should().Be(ariaLabel);
        }

#nullable enable
        /// <summary>
        /// Verifies that a numeric field with various configurations renders the expected <c>aria-describedby</c> attribute.
        /// </summary>
        // no helpers, validates error id is present when error is present
        [TestCase(false, false)]
        // with helper text, helper element should only be present when there is no error
        [TestCase(false, true)]
        // with user helper id, helper id should always be present
        [TestCase(true, false)]
        // with user helper id and helper text, should always favour user helper id
        [TestCase(true, true)]
        public async Task Should_pass_various_aria_describedby_tests(
            bool withUserHelperId,
            bool withHelperText)
        {
            var inputId = "input-id";
            var helperId = withUserHelperId ? "user-helper-id" : null;
            var helperText = withHelperText ? "helper text" : null;
            var errorId = "error-id";
            var errorText = "error text";
            var inputSelector = "input";
            var firstExpectedAriaDescribedBy = withUserHelperId
                ? helperId
                : withHelperText
                    ? $"{inputId}-helper-text"
                    : null;

            var comp = Context.Render<MudNumericField<string>>(parameters => parameters
                .Add(p => p.InputId, inputId)
                .Add(p => p.HelperId, helperId)
                .Add(p => p.HelperText, helperText)
                .Add(p => p.Error, false)
                .Add(p => p.ErrorId, errorId)
                .Add(p => p.ErrorText, errorText));

            // verify helper text is rendered
            if (withUserHelperId is false && withHelperText)
            {
                var action = () => comp.Find($"#{inputId}-helper-text");
                action.Should().NotThrow();
            }

            if (firstExpectedAriaDescribedBy is null)
            {
                comp.Find(inputSelector).HasAttribute("aria-describedby").Should().BeFalse();
            }
            else
            {
                comp.Find(inputSelector).GetAttribute("aria-describedby").Should().Be(firstExpectedAriaDescribedBy);
            }

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Error, true));
            var secondExpectedAriaDescribedBy = withUserHelperId ? $"{errorId} {helperId}" : errorId;

            // verify error text is rendered
            var errorAction = () => comp.Find($"#{errorId}");
            errorAction.Should().NotThrow();

            comp.Find(inputSelector).GetAttribute("aria-describedby").Should().Be(secondExpectedAriaDescribedBy);
        }
#nullable disable

        /// <summary>
        /// Test that reset method clears conversion errors.
        /// </summary>
        [Test]
        public async Task NumericFieldConverterErrorReset()
        {
            var comp = Context.Render<MudNumericField<int>>();
            var numericField = comp.Instance;

            // insert an invalid int number, greater then maximum int value
            comp.FindAll("input").First().Change("2147483648");
            comp.FindAll("input").First().Blur();

            // conversion is not possible and conversion error is set
            await comp.WaitForAssertionAsync(() =>
            {
                numericField.ReadValue.Should().Be(0);
                numericField.HasErrors.Should().Be(true);
                numericField.ConversionError.Should().Be(true);
                numericField.ConversionErrorMessage.Should().NotBeNullOrEmpty();
            });

            // reset the field
            await comp.InvokeAsync(numericField.ResetAsync);

            // conversion error is cleared
            await comp.WaitForAssertionAsync(() =>
            {
                numericField.ReadValue.Should().Be(0);
                numericField.HasErrors.Should().Be(false);
                numericField.ConversionError.Should().Be(false);
                numericField.ConversionErrorMessage.Should().BeNull();
            });
        }
    }
}
