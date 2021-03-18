#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{

    [TestFixture]
    public class TextFieldTests
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

        /// <summary>
        /// Initial Text for double should be 0, with F1 format it should be 0.0
        /// </summary>
        [Test]
        public async Task TextFieldTest1()
        {
            var comp = ctx.RenderComponent<MudTextField<double>>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var textfield = comp.Instance;
            textfield.Value.Should().Be(0.0);
            textfield.Text.Should().Be("0");
            //
            0.0.ToString("F1", CultureInfo.InvariantCulture).Should().Be("0.0");
            //
            await comp.InvokeAsync(() => textfield.Format = "F1");
            await comp.InvokeAsync(() => textfield.Culture = CultureInfo.InvariantCulture);
            textfield.Value.Should().Be(0.0);
            textfield.Text.Should().Be("0.0");
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        /// <summary>
        /// Initial Text for double? should be null
        /// </summary>
        [Test]
        public void TextFieldTest2()
        {
            var comp = ctx.RenderComponent<MudTextField<double?>>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var textfield = comp.Instance;
            textfield.Value.Should().Be(null);
            textfield.Text.Should().BeNullOrEmpty();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        /// <summary>
        /// Setting the value to null should not cause a validation error
        /// </summary>
        [Test]
        public async Task TextFieldWithNullableTypes()
        {
            var comp = ctx.RenderComponent<MudTextField<int?>>(ComponentParameter.CreateParameter("Value", 17));
            // print the generated html
            Console.WriteLine(comp.Markup);
            comp.SetParametersAndRender(ComponentParameter.CreateParameter("Value", null));
            comp.Find("input").Blur();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
            comp.Find("input").Change("");
            comp.Find("input").Blur();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        /// <summary>
        /// Setting an invalid number should show the conversion error message
        /// </summary>
        [Test]
        public async Task TextFieldConversionError()
        {
            var comp = ctx.RenderComponent<MudTextField<int?>>();
            // print the generated html
            //Console.WriteLine(comp.Markup);
            comp.Find("input").Change("seventeen");
            comp.Find("input").Blur();
            Console.WriteLine(comp.Markup);
            comp.FindAll("p.mud-input-error").Count.Should().Be(1);
            comp.Find("p.mud-input-error").TextContent.Trim().Should().Be("Not a valid number");
        }

        /// <summary>
        /// If Debounce Interval is null or 0, Value should change immediately
        /// </summary>
        [Test]
        public void WithNoDebounceIntervalValueShouldChangeImmediatelyTest()
        {
            //no interval passed, so, by default is 0
            // We pass the Immediate parameter set to true, in order to bind to oninput
            var immediate = Parameter(nameof(MudTextField<string>.Immediate), true);
            var comp = ctx.RenderComponent<MudTextField<string>>(immediate);
            var textField = comp.Instance;
            var input = comp.Find("input");
            //Act
            input.Input(new ChangeEventArgs() { Value = "Some Value" });
            //Assert
            //input value has changed, DebounceInterval is 0, so Value should change in TextField immediately
            textField.Value.Should().Be("Some Value");
        }


        /// <summary>
        /// Value should not change immediately. Should respect the Debounce Interval
        /// </summary>
        [Test]
        public async Task ShouldRespectDebounceIntervalPropertyInTextFieldTest()
        {
            var interval = Parameter(nameof(MudTextField<string>.DebounceInterval), 200d);
            var comp = ctx.RenderComponent<MudTextField<string>>(interval);
            var textField = comp.Instance;
            var input = comp.Find("input");
            //Act
            input.Input(new ChangeEventArgs() { Value = "Some Value" });
            //Assert
            //if DebounceInterval is set, Immediate should be true by default
            textField.Immediate.Should().BeTrue();
            //input value has changed, but elapsed time is 0, so Value should not change in TextField
            textField.Value.Should().BeNull();
            //DebounceInterval is 200 ms, so at 100 ms Value should not change in TextField
            await Task.Delay(100);
            textField.Value.Should().BeNull();
            //More than 200 ms had elapsed, so Value should be updated
            await Task.Delay(150);
            comp.WaitForAssertion(() => textField.Value.Should().Be("Some Value"));
        }

        /// <summary>
        /// Label and placeholder should not overlap.
        /// When placeholder is set, label should shrink
        /// </summary>
        [Test]
        public void LableShouldShrinkWhenPlaceholderIsSet()
        {
            //Arrange
            using var ctx = new Bunit.TestContext();
            var label = Parameter(nameof(MudTextField<string>.Label), "label");
            var placeholder = Parameter(nameof(MudTextField<string>.Placeholder), "placeholder");
            //with no placeholder, label is not shrinked
            var comp = ctx.RenderComponent<MudTextField<string>>(label);
            comp.Markup.Should().NotContain("shrink");
            //with placeholder label is shrinked
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
        /// FluentValidation rules can be used for validating a TextFields
        /// </summary>
        [Test]
        public async Task TextFieldFluentValidationTest1()
        {
            var validator = new FluentValueValidator<string>(x => x.Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Length(1, 100)
                .CreditCard());
            var comp = ctx.RenderComponent<MudTextField<string>>(Parameter(nameof(MudTextField<string>.Validation), validator.Validation));
            var textfield = comp.Instance;
            Console.WriteLine(comp.Markup);
            // first try a valid credit card number
            comp.Find("input").Change("4012 8888 8888 1881");
            textfield.Error.Should().BeFalse(because: "The number is a valid VISA test credit card number");
            textfield.ErrorText.Should().BeNullOrEmpty();
            // now try something that produces a validation error
            comp.Find("input").Change("0000 1111 2222 3333");
            textfield.Error.Should().BeTrue(because: "The credit card number is fake");
            Console.WriteLine("Error message: " + textfield.ErrorText);
            textfield.ErrorText.Should().NotBeNullOrEmpty();
        }


        /// <summary>
        /// An unstable converter should not cause an infinite update loop. This test must complete in under 1 sec!
        /// </summary>
        [Test, Timeout(1000)]
        public async Task TextFieldUpdateLoopProtectionTest()
        {
            var comp = ctx.RenderComponent<MudTextField<string>>();
            // these conversion funcs are nonsense of course, but they are designed this way to
            // test against an infinite update loop that textfields and other inputs are now protected against.
            var textfield = comp.Instance;
            textfield.Converter.SetFunc = s => $"{s}x";
            textfield.Converter.GetFunc = s => $"{s}y";
            comp.SetParametersAndRender(ComponentParameter.CreateParameter("Value", "A"));
            textfield.Value.Should().Be("A");
            textfield.Text.Should().Be("Ax");
            comp.Find("input").Change("B");
            textfield.Value.Should().Be("By");
            textfield.Text.Should().Be("B");
        }

        [Test]
        public async Task TextField_Should_FireValueChangedOnTextParameterChange()
        {
            string changed_value = null;
            var comp = ctx.RenderComponent<MudTextField<string>>(EventCallback<string>("ValueChanged", x => changed_value = x));
            comp.SetParametersAndRender(ComponentParameter.CreateParameter("Text", "A"));
            changed_value.Should().Be("A");
        }

        [Test]
        public async Task TextField_Should_FireTextChangedOnValueParameterChange()
        {
            string changed_text = null;
            var comp = ctx.RenderComponent<MudTextField<string>>(EventCallback<string>("TextChanged", x => changed_text = x));
            comp.SetParametersAndRender(ComponentParameter.CreateParameter("Value", "A"));
            changed_text.Should().Be("A");
        }

        [Test]
        public async Task TextField_Should_FireTextAndValueChangedOnTextInput()
        {
            string changed_value = null;
            string changed_text = null;
            var comp = ctx.RenderComponent<MudTextField<string>>(
                EventCallback<string>("ValueChanged", x => changed_value = x),
                EventCallback<string>("TextChanged", x => changed_text = x)
            );
            comp.Find("input").Change("B");
            changed_value.Should().Be("B");
            changed_text.Should().Be("B");
        }

        /// <summary>
        /// Instead of RequiredError it should show the conversion error, because typing something (even if not a number) should
        /// already fulfill the requirement of Required="true". If it is a valid value is a different question.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TextField_ShouldNot_ShowRequiredErrorWhenThereIsAConversionError()
        {
            var comp = ctx.RenderComponent<MudTextField<int?>>(ComponentParameter.CreateParameter("Required", true));
            var textfield = comp.Instance;
            comp.Find("input").Change("A");
            comp.Find("input").Blur();
            textfield.Text.Should().Be("A");
            textfield.HasErrors.Should().Be(true);
            textfield.ErrorText.Should().Be("Not a valid number");
        }

        /// <summary>
        /// Instead of RequiredError it should show the conversion error, because typing something (even if not a number) should
        /// already fulfill the requirement of Required="true". If it is a valid value is a different question.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TextField_ShouldNot_ShowRequiredErrorWhenInitialTextIsEmpty()
        {
            var comp = ctx.RenderComponent<TextFieldRequiredTest>();
            var textfield = comp.FindComponent<MudTextField<string>>().Instance;
            textfield.Touched.Should().BeFalse();
            textfield.ErrorText.Should().BeNullOrEmpty();
            textfield.HasErrors.Should().Be(false);
        }

        /// <summary>
        /// This is based on a bug reported by a user
        /// </summary>
        [Test]
        public async Task DebouncedTextField_ShouldNot_ThrowException()
        {
            ctx.RenderComponent<DebouncedTextFieldTest>();
        }

        [Test]
        public async Task TextFieldMultiline_CheckRenderedText()
        {
            var text = "Hello world!";
            var comp = ctx.RenderComponent<MudTextField<string>>(new[]
            {
                Parameter(nameof(MudTextField<string>.Text), text),
                Parameter(nameof(MudTextField<string>.Lines), 2)
            });
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var textfield = comp.Instance;
            comp.Find("textarea").InnerHtml.Should().Be(text);
        }

        [Test]
        public async Task MultilineTextField_Should_UpdateTextOnInput()
        {
            var comp = ctx.RenderComponent<MudTextField<string>>();
            var textfield = comp.Instance;
            comp.Find("input").Change("A");
            comp.Find("input").Blur();
            textfield.Text.Should().Be("A");
            textfield.Value.Should().Be("A");
            comp.SetParam(x => x.Lines, 2);
            comp.Find("textarea").Change("B\nC");
            comp.Find("textarea").Blur();
            textfield.Text.Should().Be("B\nC");
            textfield.Value.Should().Be("B\nC");
        }

        /// <summary>
        /// This is based on a bug reported by a user
        ///
        /// After editing the second (multi-line) tf it would not accept any updates from the first tf.
        /// </summary>
        [Test]
        public async Task MultiLineTextField_ShouldBe_TwoWayBindable()
        {
            var comp = ctx.RenderComponent<MultilineTextfieldBindingTest>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            var tf1 = comp.FindComponents<MudTextField<string>>()[0].Instance;
            var tf2 = comp.FindComponents<MudTextField<string>>()[1].Instance;
            comp.Find("input").Input("Bossmang");
            comp.Find("input").Blur(); // <-- note: blurring is important here because input does only allow render updates while not being focused!
            tf1.Text.Should().Be("Bossmang");
            tf2.Text.Should().Be("Bossmang");
            comp.Find("textarea").TrimmedText().Should().Be("Bossmang");
            comp.Find("textarea").Input("Beltalowda");
            comp.Find("textarea").Blur(); // <-- note: blurring is important here because input does only allow render updates while not being focused!
            tf1.Text.Should().Be("Beltalowda");
            tf2.Text.Should().Be("Beltalowda");
            comp.Find("textarea").TrimmedText().Should().Be("Beltalowda");
            comp.Find("input").Input("Beratna");
            comp.Find("input").Blur(); // <-- note: blurring is important here because input does only allow render updates while not being focused!
            tf1.Text.Should().Be("Beratna");
            tf2.Text.Should().Be("Beratna");
            comp.Find("textarea").TrimmedText().Should().Be("Beratna");
            Console.WriteLine(comp.Markup);
        }


        #region ValidationAttribute support
        [Test]
        public async Task TextField_Should_Validate_Data_Attribute_Fail()
        {
            var comp = ctx.RenderComponent<TextFieldValidationDataAttrTest>();
            Console.WriteLine(comp.Markup);
            var textfieldcomp = comp.FindComponent<MudTextField<string>>();
            var textfield = textfieldcomp.Instance;
            await comp.InvokeAsync(() => textfield.DebounceInterval = 0);
            // Set invalid text
            comp.Find("input").Change("Quux");
            // check initial state
            textfield.Value.Should().Be("Quux");
            textfield.Text.Should().Be("Quux");
            // check validity
            await comp.InvokeAsync(() => textfield.Validate());
            textfield.ValidationErrors.Should().NotBeEmpty();
            textfield.ValidationErrors.Should().HaveCount(1);
            textfield.ValidationErrors[0].Should().Equals("Should not be longer than 3");
        }

        [Test]
        public async Task TextField_Should_Validate_Data_Attribute_Success()
        {
            var comp = ctx.RenderComponent<TextFieldValidationDataAttrTest>();
            Console.WriteLine(comp.Markup);
            var textfieldcomp = comp.FindComponent<MudTextField<string>>();
            var textfield = textfieldcomp.Instance;
            await comp.InvokeAsync(() => textfield.DebounceInterval = 0);
            // Set valid text
            comp.Find("input").Change("Qux");
            // check initial state
            textfield.Value.Should().Be("Qux");
            textfield.Text.Should().Be("Qux");
            // check validity
            await comp.InvokeAsync(() => textfield.Validate());
            textfield.ValidationErrors.Should().BeEmpty();
        }

        #region Custom ValidationAttribute
        public class CustomFailingValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value,
                ValidationContext validationContext)
            {
                return new ValidationResult("TEST ERROR");
            }
        }
        class TestFailingModel
        {
            [CustomFailingValidation]
            public string Foo { get; set; }
        }
        [Test]
        public async Task TextField_Should_HaveCorrectMessageWithCustomAttr_Failing()
        {
            var model = new TestFailingModel();
            var comp = ctx.RenderComponent<MudTextField<string>>(ComponentParameter.CreateParameter("For", (Expression<Func<string>>)(() => model.Foo)));
            await comp.InvokeAsync(() => comp.Instance.Validate());
            comp.Instance.Error.Should().BeTrue();
            comp.Instance.ValidationErrors.Should().HaveCount(1);
            comp.Instance.ValidationErrors[0].Should().Be("TEST ERROR");
            comp.Instance.GetErrorText().Should().Be("TEST ERROR");
        }


        public class CustomThrowingValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value,
                ValidationContext validationContext)
            {
                throw new Exception("This is a test exception");
            }
        }
        class TestThrowingModel
        {
            [CustomThrowingValidation]
            public string Foo { get; set; }
        }
        [Test]
        public async Task TextField_Should_HaveCorrectMessageWithCustomAttr_Throwing()
        {
            var model = new TestThrowingModel();
            var comp = ctx.RenderComponent<MudTextField<string>>(ComponentParameter.CreateParameter("For", (Expression<Func<string>>)(() => model.Foo)));
            await comp.InvokeAsync(() => comp.Instance.Validate());
            comp.Instance.Error.Should().BeTrue();
            comp.Instance.ValidationErrors.Should().HaveCount(1);
            comp.Instance.ValidationErrors[0].Should().Be("An unhandled exception occured: This is a test exception");
            comp.Instance.GetErrorText().Should().Be("An unhandled exception occured: This is a test exception");
        }
        #endregion
        #endregion
    }

}
