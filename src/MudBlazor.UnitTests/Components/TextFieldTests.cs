using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using AwesomeAssertions;
using Bunit;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.Dummy;
using MudBlazor.UnitTests.TestComponents.Field;
using MudBlazor.UnitTests.TestComponents.Form;
using MudBlazor.UnitTests.TestComponents.TextField;
using MudBlazor.UnitTests.Utilities;
using NUnit.Framework;

#nullable enable
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    [NonParallelizable]
    public class TextFieldTests : BunitTest
    {
        /// <summary>
        /// Text Field id should propagate to label for attribute
        /// </summary>
        [Test]
        public void TestFieldLabelFor()
        {
            var comp = Context.Render<FormIsValidTest3>();
            var label = comp.FindAll(".mud-input-label");
            label[0].Attributes.GetNamedItem("for")?.Value.Should().Be("textFieldLabelTest");
            label[1].Attributes.GetNamedItem("for")?.Value.Should().StartWith("mudinput");
        }

        /// <summary>
        /// Initial Text for double should be 0, with F1 format it should be 0.0
        /// </summary>
        [Test]
        public void TextFieldLabelFor()
        {
            var comp = Context.Render<FieldTest>();
            var label = comp.FindAll(".mud-input-label");
            label[0].Attributes.GetNamedItem("for")?.Value.Should().StartWith("mudinput");
            label[1].Attributes.GetNamedItem("for")?.Value.Should().StartWith("mudinput");
            label[2].Attributes.GetNamedItem("for")?.Value.Should().Be("fieldLabelTest");
        }

        /// <summary>
        /// Initial Text for double should be 0, with F1 format it should be 0.0
        /// </summary>
        [Test]
        public async Task TextFieldTest1()
        {
            var comp = Context.Render<MudTextField<double>>();

            // print the generated html
            // select elements needed for the test
            var textfield = comp.Instance;
            textfield.ReadValue.Should().Be(0.0);
            textfield.ReadText.Should().Be("0");

            //
            0.0.ToString("F1", CultureInfo.InvariantCulture).Should().Be("0.0");

            //
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Format, "F1")
                .Add(x => x.Culture, CultureInfo.InvariantCulture));

            textfield.ReadValue.Should().Be(0.0);
            textfield.ReadText.Should().Be("0.0");
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        /// <summary>
        /// Initial Text for double? should be null
        /// </summary>
        [Test]
        public void TextFieldTest2()
        {
            var comp = Context.Render<MudTextField<double?>>();

            // print the generated html
            // select elements needed for the test
            var textfield = comp.Instance;
            textfield.ReadValue.Should().Be(null);
            textfield.ReadText.Should().BeNullOrEmpty();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        /// <summary>
        /// Setting the value to null should not cause a validation error
        /// </summary>
        [Test]
        public async Task TextFieldWithNullableTypes()
        {
            var comp = Context.Render<MudTextField<int?>>(parameters => parameters.Add(p => p.Value, 17));

            // print the generated html
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Value, null));
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
            await comp.Find("input").ChangeAsync("");
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        /// <summary>
        /// Setting an invalid number should show the conversion error message
        /// </summary>
        [Test]
        public async Task TextFieldConversionError()
        {
            var comp = Context.Render<MudTextField<int?>>();

            // print the generated html
            await comp.Find("input").ChangeAsync("seventeen");
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(3);
            comp.Find("div.mud-input-error").TextContent.Trim().Should().Be("Not a valid number");
        }

        [Test]
        public async Task TextField_Should_PreserveInvalidTextOnKeyRerender()
        {
            var comp = Context.Render<TextFieldConversionErrorKeyRerenderTest>();

            await comp.Find("input").InputAsync("123456");

            var textField = comp.FindComponent<MudTextField<TextFieldConversionErrorKeyRerenderTest.Pod>>().Instance;
            textField.ReadValue.Should().BeNull();
            textField.ReadText.Should().Be("123456");
            textField.ConversionError.Should().BeTrue();
            textField.ConversionErrorMessage.Should().Be("Error message");

            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "6", Type = "keydown" });

            textField = comp.FindComponent<MudTextField<TextFieldConversionErrorKeyRerenderTest.Pod>>().Instance;
            textField.ReadValue.Should().BeNull();
            textField.ReadText.Should().Be("123456");
            textField.ConversionError.Should().BeTrue();
            comp.Find("input").GetAttribute("value").Should().Be("123456");
        }

        /// <summary>
        /// If Debounce Interval is null or 0, Value should change immediately
        /// </summary>
        [Test]
        public async Task WithNoDebounceIntervalValueShouldChangeImmediately()
        {
            //no interval passed, so, by default is 0
            // We pass the Immediate parameter set to true, in order to bind to oninput
            var comp = Context.Render<MudTextField<string>>(parameters => parameters.Add(p => p.Immediate, true));
            var textField = comp.Instance;
            var input = comp.Find("input");

            //Act
            await input.InputAsync(new ChangeEventArgs() { Value = "Some Value" });

            //Assert
            //input value has changed, DebounceInterval is 0, so Value should change in TextField immediately
            textField.ReadValue.Should().Be("Some Value");
        }

        /// <summary>
        /// Value should not change immediately. Should respect the Debounce Interval
        /// </summary>
        [Test]
        public async Task ShouldRespectDebounceIntervalPropertyInTextField()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters.Add(p => p.DebounceInterval, 200d));
            var textField = comp.Instance;
            var input = comp.Find("input");

            //Act
            await input.InputAsync(new ChangeEventArgs() { Value = "Some Value" });

            //Assert
            //if DebounceInterval is set, Immediate should be true by default
            textField.Immediate.Should().BeTrue();

            //input value has changed, but elapsed time is 0, so Value should not change in TextField
            textField.ReadValue.Should().BeNull();

            //DebounceInterval is 200 ms, so at 100 ms Value should not change in TextField
            await Task.Delay(100);
            textField.ReadValue.Should().BeNull();

            //More than 200 ms had elapsed, so Value should be updated
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be("Some Value"));
        }

        /// <summary>
        /// DebounceInterval updates with epsilon-equivalent values should not break debouncing
        /// </summary>
        [Test]
        public async Task DebounceInterval_EpsilonEquivalentValues_PreservesDebounce()
        {
            // Arrange
            var comp = Context.Render<MudTextField<string>>(parameters => parameters.Add(p => p.DebounceInterval, 200.0));
            var textField = comp.Instance;
            var input = comp.Find("input");

            // Act - Input a value
            await input.InputAsync(new ChangeEventArgs() { Value = "Test Value" });

            // Change DebounceInterval to an epsilon-equivalent value (should not reset debouncer)
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.DebounceInterval, 200.0000001));

            // Assert - Value should still be null (debounce still pending)
            textField.ReadValue.Should().BeNull();

            // Wait for the debounce to complete
            await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be("Test Value"));
        }

        [Test]
        public async Task DebouncedTextField_ShouldStayInSyncWithBoundValueAfterAsyncInitialization()
        {
            var comp = Context.Render<DebouncedTextFieldAsyncInitializationSyncTest>();

            await comp.WaitForAssertionAsync(() =>
            {
                var inputs = comp.FindAll("input");
                inputs[0].GetAttribute("value").Should().Be("init value");
                inputs[1].GetAttribute("value").Should().Be("init value");
            });

            var immediateInput = comp.FindAll("input")[1];
            await immediateInput.ChangeAsync(new ChangeEventArgs { Value = "changed value" });

            await comp.WaitForAssertionAsync(() =>
            {
                var inputs = comp.FindAll("input");
                inputs[0].GetAttribute("value").Should().Be("changed value");
                inputs[1].GetAttribute("value").Should().Be("changed value");
            });
        }

        [Test]
        public async Task DebouncedTextField_Should_ValidatePendingValueImmediatelyWhenFormIsValidated()
        {
            var timeProvider = new FakeTimeProvider();
            Context.Services.AddSingleton<TimeProvider>(timeProvider);

            var comp = Context.Render<DebouncedTextFieldFormValidationSyncTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textField = comp.FindComponents<MudTextField<string>>();

            await comp.Find("#username").InputAsync(new ChangeEventArgs { Value = "username" });
            await comp.Find("#password").ChangeAsync(new ChangeEventArgs { Value = "password" });

            textField[0].Instance.ReadText.Should().Be("username");
            textField[0].Instance.ReadValue.Should().BeNull();
            comp.Instance.Model.Username.Should().BeNull();
            textField[1].Instance.ReadText.Should().Be("password");
            textField[1].Instance.ReadValue.Should().Be("password");
            comp.Instance.Model.Password.Should().Be("password");
            form.IsValid.Should().BeTrue();

            await comp.Find("#validate-button").ClickAsync();

            await comp.WaitForAssertionAsync(() =>
            {
                comp.Instance.Model.Username.Should().Be("username");
                comp.Instance.Model.Password.Should().Be("password");
                comp.Instance.ResultText.Should().Be("succeeded");
                form.IsValid.Should().BeTrue();
                textField[0].Instance.ReadValue.Should().Be("username");
                textField[1].Instance.ReadValue.Should().Be("password");
            });
        }

        /// <summary>
        /// Label and placeholder should not overlap.
        /// When placeholder is set, label should shrink
        /// </summary>
        [Test]
        public async Task LabelShouldShrinkWhenPlaceholderIsSet()
        {
            //Arrange
            //with no placeholder, label is not shrinked
            var comp = Context.Render<MudTextField<string>>(parameters => parameters.Add(p => p.Label, "label"));
            comp.Markup.Should().NotContain("shrink");

            //with placeholder label is shrinked
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Placeholder, "placeholder"));
            comp.Markup.Should().Contain("shrink");
        }

        /// <summary>
        /// Setting ShrinkLabel should apply mud-shrink class.
        /// </summary>
        [Test]
        public void LabelShouldShrinkWhenShrinkLabelIsSet()
        {
            var comp = Context.Render<TextFieldShrinkLabelTest>();
            var noMask = comp.FindComponents<MudTextField<string>>()[0];
            var masked = comp.FindComponents<MudTextField<string>>()[1];

            noMask.Markup.Should().Contain("mud-shrink");
            masked.Markup.Should().Contain("mud-shrink");
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
                {
                    return Array.Empty<string>();
                }

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
            var comp = Context.Render<MudTextField<string>>(parameters => parameters.Add(p => p.Validation, validator.Validation));
            var textfield = comp.Instance;

            // first try a valid credit card number
            await comp.Find("input").ChangeAsync("4012 8888 8888 1881");
            textfield.GetState(x => x.Error).Should().BeFalse(because: "The number is a valid VISA test credit card number");
            textfield.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            // now try something that produces a validation error
            await comp.Find("input").ChangeAsync("0000 1111 2222 3333");
            textfield.GetState(x => x.Error).Should().BeTrue(because: "The credit card number is fake");
            textfield.GetState(x => x.ErrorText).Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// An unstable converter should not cause an infinite update loop. This test must complete in under 1 sec!
        /// </summary>
        [Test, CancelAfter(1000)]
        public async Task TextFieldUpdateLoopProtection()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(x => x.Converter, Conversions.From<string?, string>(s => $"{s}x", s => $"{s}y")));

            // these conversion funcs are nonsense of course, but they are designed this way to
            // test against an infinite update loop that textfields and other inputs are now protected against.
            var textfield = comp.Instance;
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Value, "A"));
            textfield.ReadValue.Should().Be("A");
            textfield.ReadText.Should().Be("Ax");
            await comp.Find("input").ChangeAsync("B");
            textfield.ReadValue.Should().Be("By");
            textfield.ReadText.Should().Be("B");
        }

        [Test]
        public void TextField_Should_FireValueChangedOnTextParameterChange()
        {
            string? changed_value = null;
            _ = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.ValueChanged, x => changed_value = x)
                .Add(p => p.Text, "A"));
            changed_value.Should().Be("A");
        }

        [Test]
        public void TextField_Should_FireTextChangedOnValueParameterChange()
        {
            string? changed_text = null;
            _ = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.TextChanged, x => changed_text = x)
                .Add(p => p.Value, "A"));
            changed_text.Should().Be("A");
        }

        [Test]
        public async Task TextField_Should_FireTextAndValueChangedOnTextInput()
        {
            string? changed_value = null;
            string? changed_text = null;
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.ValueChanged, x => changed_value = x)
                .Add(p => p.TextChanged, x => changed_text = x)
            );
            await comp.Find("input").ChangeAsync("B");
            changed_value.Should().Be("B");
            changed_text.Should().Be("B");
        }

        /// <summary>
        /// Instead of RequiredError it should show the conversion error, because typing something (even if not a number) should
        /// already fulfill the requirement of Required="true". If it is a valid value is a different question.
        /// </summary>
        [Test]
        public async Task TextField_ShouldNot_ShowRequiredErrorWhenThereIsAConversionError()
        {
            var comp = Context.Render<MudTextField<int?>>(parameters => parameters.Add(p => p.Required, true));
            var textfield = comp.Instance;
            await comp.Find("input").ChangeAsync("A");
            await comp.Find("input").BlurAsync();
            textfield.ReadText.Should().Be("A");
            textfield.HasErrors.Should().Be(true);
            textfield.GetState(x => x.ErrorText).Should().Be("Not a valid number");
        }

        [Test]
        public async Task RequiredTextField_Should_ReuseGeneratedErrorIdWhileInvalid()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters.Add(p => p.Required, true));

            await comp.InvokeAsync(() => comp.Instance.ValidateAsync());

            var firstErrorId = comp.Find("input").GetAttribute("aria-describedby");
            firstErrorId.Should().NotBeNullOrWhiteSpace();
            comp.Find($"[id='{firstErrorId}']").TextContent.Should().Be("Required");

            await comp.InvokeAsync(() => comp.Instance.ValidateAsync());

            comp.Find("input").GetAttribute("aria-describedby").Should().Be(firstErrorId);

            await comp.Find("input").ChangeAsync("valid");
            await comp.Find("input").BlurAsync();

            comp.Find("input").HasAttribute("aria-describedby").Should().BeFalse();

            await comp.Find("input").ChangeAsync(string.Empty);
            await comp.Find("input").BlurAsync();

            var secondErrorId = comp.Find("input").GetAttribute("aria-describedby");
            secondErrorId.Should().NotBeNullOrWhiteSpace();
            secondErrorId.Should().NotBe(firstErrorId);
        }

        [Test]
        public async Task RequiredTextField_Should_RestoreProvidedErrorIdAfterBecomingValid()
        {
            const string errorId = "provided-error-id";

            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Required, true)
                .Add(p => p.ErrorId, errorId));

            await comp.InvokeAsync(() => comp.Instance.ValidateAsync());
            comp.Find("input").GetAttribute("aria-describedby").Should().Be(errorId);

            await comp.Find("input").ChangeAsync("valid");
            await comp.Find("input").BlurAsync();
            comp.Find("input").HasAttribute("aria-describedby").Should().BeFalse();

            await comp.Find("input").ChangeAsync(string.Empty);
            await comp.Find("input").BlurAsync();
            comp.Find("input").GetAttribute("aria-describedby").Should().Be(errorId);
        }

        /// <summary>
        /// Instead of RequiredError it should show the conversion error, because typing something (even if not a number) should
        /// already fulfill the requirement of Required="true". If it is a valid value is a different question.
        /// </summary>
        [Test]
        public void TextField_ShouldNot_ShowRequiredErrorWhenInitialTextIsEmpty()
        {
            var comp = Context.Render<TextFieldRequiredTest>();
            var textfield = comp.FindComponent<MudTextField<string>>().Instance;
            textfield.Touched.Should().BeFalse();
            textfield.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfield.HasErrors.Should().Be(false);
        }

        /// <summary>
        /// This is based on a bug reported by a user
        /// </summary>
        [Test]
        public void DebouncedTextField_ShouldNot_ThrowException()
        {
            // Arrange & Act
            var renderComponent = () => Context.Render<DebouncedTextFieldTest>();

            // Assert
            renderComponent.Should().NotThrow();
        }

        [Test]
        public void TextFieldMultiline_CheckRenderedText()
        {
            var text = "Hello world!";
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Text, text)
                .Add(p => p.Lines, 2));

            // print the generated html
            // select elements needed for the test
            comp.Find("textarea").InnerHtml.Should().Be(text);
        }

        /// <summary>
        /// Ensures that a text field with both 'Lines' > 1 and 'Mask' parameters generates a 'textarea'.
        /// </summary>
        /// <returns></returns>
        [Test]
        public void TextFieldMultilineWithMask_CheckRendered()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Mask, new RegexMask(@"\d"))
                .Add(p => p.Lines, 2));
            comp.Find("textarea").Should().NotBeNull();
        }

        [Test]
        public async Task MultilineTextField_Should_UpdateTextOnInput()
        {
            var comp = Context.Render<MudTextField<string>>();
            var textfield = comp.Instance;
            await comp.Find("input").ChangeAsync("A");
            await comp.Find("input").BlurAsync();
            textfield.ReadText.Should().Be("A");
            textfield.ReadValue.Should().Be("A");
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Lines, 2));
            await comp.Find("textarea").ChangeAsync("B\nC");
            await comp.Find("textarea").BlurAsync();
            textfield.ReadText.Should().Be("B\nC");
            textfield.ReadValue.Should().Be("B\nC");
        }

        /// <summary>
        /// <para>This is based on a bug reported by a user</para>
        /// <para>After editing the second (multi-line) tf it would not accept any updates from the first tf.</para>
        /// </summary>
        [Test]
        public async Task MultiLineTextField_ShouldBe_TwoWayBindable()
        {
            var comp = Context.Render<MultilineTextfieldBindingTest>();

            // print the generated html
            var tf1 = comp.FindComponents<MudTextField<string>>()[0].Instance;
            var tf2 = comp.FindComponents<MudTextField<string>>()[1].Instance;
            await comp.Find("input").InputAsync("Bossmang");
            await comp.Find("input").BlurAsync(); // <-- note: Blur is important here because input does not allow render updates while focused!
            tf1.ReadText.Should().Be("Bossmang");
            tf2.ReadText.Should().Be("Bossmang");
            comp.Find("textarea").TrimmedText().Should().Be("Bossmang");
            await comp.Find("textarea").InputAsync("Beltalowda");
            await comp.Find("textarea").BlurAsync(); // Blur is important
            tf1.ReadText.Should().Be("Beltalowda");
            tf2.ReadText.Should().Be("Beltalowda");
            comp.Find("textarea").TrimmedText().Should().Be("Beltalowda");
            await comp.Find("input").InputAsync("Beratna");
            await comp.Find("input").BlurAsync(); // Blur is important
            tf1.ReadText.Should().Be("Beratna");
            tf2.ReadText.Should().Be("Beratna");
            comp.Find("textarea").TrimmedText().Should().Be("Beratna");
        }

        [Test]
        public async Task AutoSizingTextField_Should_InvokeJavaScriptInitOnRender()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Sizing, InputSizing.Auto)
                .Add(p => p.MaxLines, 5));

            Context.JSInterop.VerifyInvoke("mudInputSizing.init", 1);
            Context.JSInterop.Invocations["mudInputSizing.init"].Single()
                .Arguments
                .Should()
                .HaveCount(2)
                .And
                .HaveElementAt(1, 5); // MaxLines

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Value, "A"));

            Context.JSInterop.Invocations["mudInputSizing.adjustHeight"].Single()
                .Arguments
                .Should()
                .HaveCount(1);
        }

        [Test]
        public async Task TextFieldClearable()
        {
            var comp = Context.Render<TextFieldClearableTest>();
            var textField = comp.FindComponent<MudTextField<string>>();

            // No button when initialized
            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();

            // Button shows after entering text
            await comp.Find("input").ChangeAsync("text");
            textField.Instance.Value.Should().Be("text");
            comp.Find(".mud-input-clear-button").Should().NotBeNull();

            // Text cleared and button removed after clicking clear button
            await comp.Find(".mud-input-clear-button").ClickAsync();
            textField.Instance.Value.Should().BeNullOrEmpty();
            comp.FindAll("button").Should().BeEmpty();

            // Clear button click handler should have been invoked
            comp.Instance.ClearButtonClicked.Should().BeTrue();

            // Button shows again after entering text
            await comp.Find("input").ChangeAsync("text");
            textField.Instance.Value.Should().Be("text");
            comp.Find(".mud-input-clear-button").Should().NotBeNull();

            // Button removed after clearing text by typing
            await comp.Find("input").ChangeAsync(string.Empty);
            comp.FindAll(".mud-input-clear-button").Should().BeEmpty();
        }

        [Test]
        public void TextField_ClearButton_TabIndex()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(parameter => parameter.Clearable, true)
                .Add(x => x.Text, "Test"));

            // Button should have tabindex -1
            comp.Find(".mud-input-clear-button").GetAttribute("tabindex").Should().Be("-1");
        }

        #region ValidationAttribute support
        [Test]
        public async Task TextField_Should_Validate_Data_Attribute_Fail()
        {
            var comp = Context.Render<TextFieldValidationDataAttrTest>();
            var textfieldcomp = comp.FindComponent<MudTextField<string>>();
            var textfield = textfieldcomp.Instance;
            await textfieldcomp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.DebounceInterval, 0));

            // Set invalid text
            await comp.Find("input").ChangeAsync("Quux");

            // check initial state
            textfield.ReadValue.Should().Be("Quux");
            textfield.ReadText.Should().Be("Quux");

            // check validity
            await comp.InvokeAsync(() => textfield.ValidateAsync());
            textfield.ValidationErrors.Should().NotBeEmpty();
            textfield.ValidationErrors.Should().HaveCount(1);
            textfield.ValidationErrors[0].Should().Be("Should not be longer than 3");
        }

        [Test]
        public async Task TextField_Should_Validate_Data_Attribute_Success()
        {
            var comp = Context.Render<TextFieldValidationDataAttrTest>();
            var textfieldcomp = comp.FindComponent<MudTextField<string>>();
            var textfield = textfieldcomp.Instance;
            await textfieldcomp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.DebounceInterval, 0));

            // Set valid text
            await comp.Find("input").ChangeAsync("Qux");

            // check initial state
            textfield.ReadValue.Should().Be("Qux");
            textfield.ReadText.Should().Be("Qux");

            // check validity
            await comp.InvokeAsync(() => textfield.ValidateAsync());
            textfield.ValidationErrors.Should().BeEmpty();
        }

        #region Custom ValidationAttribute
        public class CustomFailingValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        class TestFailingModel
        {
            [CustomFailingValidation(ErrorMessage = "Foo")]
            public virtual string? Foo { get; set; }
        }

        [Test]
        public async Task TextField_Should_HaveCorrectMessageWithCustomAttr_Failing()
        {
            var model = new TestFailingModel();
            var comp = Context.Render<MudTextField<string>>(parameters =>
                parameters.Add(p => p.For, (Expression<Func<string>>)(() => model.Foo!)));
            await comp.InvokeAsync(() => comp.Instance.ValidateAsync());
            comp.Instance.GetState(x => x.Error).Should().BeTrue();
            comp.Instance.ValidationErrors.Should().HaveCount(1);
            comp.Instance.ValidationErrors[0].Should().Be("Foo");
            comp.Instance.GetErrorText().Should().Be("Foo");
        }

        class TestFailingModel2 : TestFailingModel
        {
            [CustomFailingValidation(ErrorMessage = "Bar")]
            public override string? Foo { get; set; }
        }

        /// <summary>
        /// This test checks specifically the case where validation is made on a child class, but linq expression returns the property of the parent.
        /// </summary>
        [Test]
        public async Task TextField_Should_HaveCorrectMessageWithCustomAttr_Override_Failing()
        {
            TestFailingModel model = new TestFailingModel2();
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                    .Add(p => p.For, (Expression<Func<string>>)(() => (model as TestFailingModel2)!.Foo!))

            //ComponentParameter.CreateParameter("ForModel", typeof(TestFailingModel2)) // Explicitly set the `For` class
            );
            await comp.InvokeAsync(() => comp.Instance.ValidateAsync());
            comp.Instance.GetState(x => x.Error).Should().BeTrue();
            comp.Instance.ValidationErrors.Should().HaveCount(1);
            comp.Instance.ValidationErrors[0].Should().Be("Bar");
            comp.Instance.GetErrorText().Should().Be("Bar");
        }

        public class CustomThrowingValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
            {
                throw new Exception("This is a test exception");
            }
        }

        class TestThrowingModel
        {
            [CustomThrowingValidation]
            public string? Foo { get; set; }
        }

        [Test]
        public async Task TextField_Should_HaveCorrectMessageWithCustomAttr_Throwing()
        {
            var model = new TestThrowingModel();
            var comp = Context.Render<MudTextField<string>>(parameters =>
                parameters.Add(p => p.For, (Expression<Func<string>>)(() => model.Foo!)));
            await comp.InvokeAsync(() => comp.Instance.ValidateAsync());
            comp.Instance.GetState(x => x.Error).Should().BeTrue();
            comp.Instance.ValidationErrors.Should().HaveCount(1);
            comp.Instance.ValidationErrors[0].Should().Be("An unhandled exception occurred: This is a test exception");
            comp.Instance.GetErrorText().Should().Be("An unhandled exception occurred: This is a test exception");
        }
        #endregion
        #endregion

        [Test]
        public async Task TextField_ClearTest1()
        {
            var comp = Context.Render<MudTextField<int>>();
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Text, "17"));
            var textfield = comp.Instance;
            textfield.ReadValue.Should().Be(17);
            textfield.ReadText.Should().Be("17");
            await comp.InvokeAsync(async () => await textfield.ClearAsync());
            textfield.ReadValue.Should().Be(0);
            textfield.ReadText.Should().Be(null);
        }

        [Test]
        public async Task TextField_ClearTest2()
        {
            var comp = Context.Render<MudTextField<string>>();
            await comp.Find("input").ChangeAsync("Viva la ignorancia");
            var textfield = comp.Instance;
            textfield.ReadValue.Should().Be("Viva la ignorancia");
            textfield.ReadText.Should().Be("Viva la ignorancia");
            await comp.InvokeAsync(async () => await textfield.ClearAsync());
            textfield.ReadValue.Should().Be(null);
            textfield.ReadText.Should().Be(null);
        }

        [Test]
        public async Task TextField_CharacterCount()
        {
            var comp = Context.Render<MudTextField<string>>();
            var inputControl = comp.FindComponent<MudInputControl>();

            //Condition 1
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Counter, null));
            inputControl.Instance.CounterText.Should().Be("");

            //Condition 2
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Counter, 25));
            await comp.Find("input").ChangeAsync("Test text");
            inputControl.Instance.CounterText.Should().Be("9 / 25");

            //Condition 3
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Counter, 0));
            await comp.Find("input").ChangeAsync("Test text with total of 56 characters a aaaaaaaaa aaaaaa");
            inputControl.Instance.CounterText.Should().Be("56");

            //Condition 4
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Counter, 25)
                .Add(x => x.MaxLength, 30));
            await comp.Find("input").ChangeAsync("Test text with total of25");
            inputControl.Instance.CounterText.Should().Be("25 / 25");

            //Condition 5
            await comp.Find("input").ChangeAsync("Test text with total of 56 characters a aaaaaaaaa aaaaaa");
            inputControl.Instance.CounterText.Should().Be("56 / 25");
        }

        /// <summary>
        /// This tests the suppression of the suppression (fix for #1012)
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TextField_TextUpdateSuppression()
        {
            var comp = Context.Render<MudTextField<string>>();
            var input = comp.FindComponent<MudInput<string>>();
            var textfield = comp.Instance;
            await comp.Find("input").ChangeAsync("Vat of acid");

            // this will make the input focused!
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            textfield.ReadValue.Should().Be("Vat of acid");
            textfield.ReadText.Should().Be("Vat of acid");

            // TextUpdateSuppression has been removed - text now always updates
            // Setting value directly will always update the text
            await input.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, ""));
            input.Instance.ReadValue.Should().Be("");
            input.Instance.ReadText.Should().Be("");

            // Set a new value
            await input.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, "In case of ladle"));
            input.Instance.ReadValue.Should().Be("In case of ladle");
            input.Instance.ReadText.Should().Be("In case of ladle");

            // Set empty again
            await input.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, ""));
            input.Instance.ReadValue.Should().Be("");
            input.Instance.ReadText.Should().Be("");

            // force text update (should still work)
            await input.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, "Test"));
            await comp.InvokeAsync(() => input.Instance.ForceRender(forceTextUpdate: true));

            input.Instance.ReadValue.Should().Be("Test");
            input.Instance.ReadText.Should().Be("Test");
        }

        [Test]
        public async Task TextField_Should_UpdateOnBoundValueChange_WhenFocused_WithTextUpdateSuppressionOff()
        {
            var comp = Context.Render<TextFieldUpdateViaBindingTest>();
            var input = comp.FindComponent<MudInput<string>>();

            // this will make the input focused!
            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "a", Type = "keydown", });

            // now simulate user input:
            await comp.Find("input").InputAsync("The Stormlight Archive");

            // check binding update
            comp.Find("span").TrimmedText().Should().Be("value: The Stormlight Archive");
            input.Instance.ReadValue.Should().Be("The Stormlight Archive");
            input.Instance.ReadText.Should().Be("The Stormlight Archive");

            // now hit Enter to cause the clearing of the focused text field
            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", });
            await comp.WaitForAssertionAsync(() => comp.Find("span").TrimmedText().Should().Be("value:"));
            await comp.WaitForAssertionAsync(() => input.Instance.ReadValue.Should().Be(""));
            await comp.WaitForAssertionAsync(() => input.Instance.ReadText.Should().Be(""));
        }

        [Test]
        public void TextField_ElementReferenceId_ShouldNot_BeEmpty()
        {
            var comp = Context.Render<MudTextField<string>>();
            var inputId = comp.Instance.InputReference?.ElementReference.Id;

            inputId.Should().NotBeEmpty();
        }

        private class TestDataAnnotationModel
        {
            [Required(ErrorMessage = "The {0} field is required.")]
            public string? Foo1 { get; set; }

            [Required(ErrorMessage = "The {0} field is required.")]
            [Display(Name = FooTwoDisplayName)]
            [Compare(nameof(Foo1), ErrorMessage = "'{0}' and '{1}' do not match.")]
            public string? Foo2 { get; set; }

            public const string FooTwoDisplayName = "Foo two";
        }

        [Test]
        public async Task TextField_Data_Annotation_Resolve_Name_Of_Field()
        {
            var model = new TestDataAnnotationModel();
            var comp = Context.Render<MudTextField<string>>(parameters =>
                parameters.Add(p => p.For, (Expression<Func<string>>)(() => model.Foo1!)));
            await comp.InvokeAsync(() => comp.Instance.ValidateAsync());
            comp.Instance.GetState(x => x.Error).Should().BeTrue();
            comp.Instance.ValidationErrors.Should().HaveCount(1);
            comp.Instance.ValidationErrors[0].Should().Be($"The {nameof(TestDataAnnotationModel.Foo1)} field is required.");
            comp.Instance.GetErrorText().Should().Be($"The {nameof(TestDataAnnotationModel.Foo1)} field is required.");
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, "Foo"));
            await comp.InvokeAsync(() =>
            {
                comp.Instance.ValidateAsync();
            });
            comp.Instance.GetState(x => x.Error).Should().BeFalse();
            comp.Instance.ValidationErrors.Should().HaveCount(0);
        }

        [Test]
        public async Task TextField_Data_Annotation_Resolve_Display_Name_Of_Field()
        {
            var model = new TestDataAnnotationModel();
            var comp = Context.Render<MudTextField<string>>(parameters =>
                parameters.Add(p => p.For, (Expression<Func<string>>)(() => model.Foo2!)));
            await comp.InvokeAsync(() => comp.Instance.ValidateAsync());
            comp.Instance.GetState(x => x.Error).Should().BeTrue();
            comp.Instance.ValidationErrors.Should().HaveCount(1);
            comp.Instance.ValidationErrors[0].Should().Be($"The {TestDataAnnotationModel.FooTwoDisplayName} field is required.");
            comp.Instance.GetErrorText().Should().Be($"The {TestDataAnnotationModel.FooTwoDisplayName} field is required.");
        }

        [Test]
        public async Task TextField_Data_Annotation_Compare()
        {
            var model = new TestDataAnnotationModel();
            var value = "Foo";
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.For, (Expression<Func<string>>)(() => model.Foo2!))
                .Add(p => p.Value, value));
            await comp.InvokeAsync(() => comp.Instance.ValidateAsync());
            comp.Instance.GetState(x => x.Error).Should().BeTrue();
            comp.Instance.ValidationErrors.Should().HaveCount(1);
            comp.Instance.ValidationErrors[0].Should().Be($"'{TestDataAnnotationModel.FooTwoDisplayName}' and '{nameof(TestDataAnnotationModel.Foo1)}' do not match.");
            comp.Instance.GetErrorText().Should().Be($"'{TestDataAnnotationModel.FooTwoDisplayName}' and '{nameof(TestDataAnnotationModel.Foo1)}' do not match.");
            model.Foo1 = value;
            await comp.InvokeAsync(() =>
            {
                comp.Instance.ValidateAsync();
            });
            comp.Instance.GetState(x => x.Error).Should().BeFalse();
            comp.Instance.ValidationErrors.Should().HaveCount(0);

            await comp.WaitForAssertionAsync(() => comp.Instance.GetInputType().Should().Be(InputType.Text));
            await comp.InvokeAsync(async () => await comp.Instance.SelectAsync());
            await comp.InvokeAsync(async () => await comp.Instance.SelectRangeAsync(0, 1));
            await comp.WaitForAssertionAsync(() => comp.Instance.ValidationErrors.Should().HaveCount(0));
        }

        [Test]
        public void InputMode_DefaultValue_IsText()
        {
            var comp = Context.Render<MudTextField<string>>();

            comp.Instance.InputMode.Should().Be(InputMode.text);
            comp
                .Find("input")
                .Attributes
                .First(x => x.Name.Equals("inputmode", StringComparison.Ordinal))
                .Value
                .Should()
                .Be("text");
        }

        [Test]
        public void InputMode_DefaultValueWithMask_IsText()
        {
            var mask = new PatternMask("0000");
            var comp = Context.Render<MudTextField<string>>(x =>
                x.Add(f => f.Mask, mask));

            comp.Instance.InputMode.Should().Be(InputMode.text);
            comp
                .Find("input")
                .Attributes
                .First(x => x.Name.Equals("inputmode", StringComparison.Ordinal))
                .Value
                .Should()
                .Be("text");
        }

        [Test]
        public void InputMode_ChangedValue_IsPropagated()
        {
            var comp = Context.Render<MudTextField<string>>(x =>
                x.Add(f => f.InputMode, InputMode.numeric));

            comp.Instance.InputMode.Should().Be(InputMode.numeric);
            comp
                .Find("input")
                .Attributes
                .First(x => x.Name.Equals("inputmode", StringComparison.Ordinal))
                .Value
                .Should()
                .Be("numeric");
        }

        [Test]
        public void InputMode_ChangedValueWithMask_IsPropagated()
        {
            var mask = new PatternMask("0000");
            var comp = Context.Render<MudTextField<string>>(x => x
                .Add(f => f.InputMode, InputMode.numeric)
                .Add(f => f.Mask, mask));

            comp.Instance.InputMode.Should().Be(InputMode.numeric);
            comp
                .Find("input")
                .Attributes
                .First(x => x.Name.Equals("inputmode", StringComparison.Ordinal))
                .Value
                .Should()
                .Be("numeric");
        }

        [Test]
        public async Task TextField_OnlyValidateIfDirty_Is_True_Should_OnlyHaveInputErrorWhenValueChanged()
        {
            var comp = Context.Render<MudTextField<int?>>(parameters => parameters
                .Add(p => p.Required, true)
                .Add(p => p.OnlyValidateIfDirty, true));
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);

            // user does not change input value but changes focus
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);

            // user puts in a invalid integer value
            await comp.Find("input").ChangeAsync("invalid");
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(3);
            comp.Find("div.mud-input-error").TextContent.Trim().Should().Be("Not a valid number");

            // user does not change invalid input value but changes focus
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(3);
            comp.Find("div.mud-input-error").TextContent.Trim().Should().Be("Not a valid number");

            // reset (must reset dirty state)
            await comp.InvokeAsync(() => comp.Instance.ResetAsync());
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);

            // user does not change input value but changes focus
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);

            // user puts in a invalid integer value
            await comp.Find("input").ChangeAsync("invalid");
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(3);
            comp.Find("div.mud-input-error").TextContent.Trim().Should().Be("Not a valid number");

            // user corrects input
            await comp.Find("input").ChangeAsync(55);
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        [Test]
        public async Task TextField_OnlyValidateIfDirty_WithNonDefaultInitialValue_ShouldNotValidateOnBlur()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Value, string.Empty)
                .Add(p => p.Required, true)
                .Add(p => p.OnlyValidateIfDirty, true));
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);

            // blur without user interaction should not trigger validation
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);

            // user types then clears — now dirty, validation should fire
            await comp.Find("input").ChangeAsync("x");
            await comp.Find("input").ChangeAsync("");
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().BeGreaterThan(0);
            comp.Find("div.mud-input-error").TextContent.Trim().Should().Be("Required");
        }

        [Test]
        public async Task TextField_OnlyValidateIfDirty_Is_False_Should_HaveInputErrorWhenFocusChanged()
        {
            var comp = Context.Render<MudTextField<int?>>(parameters => parameters
                .Add(p => p.Required, true)
                .Add(p => p.OnlyValidateIfDirty, false));
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);

            // user does not change input value but changes focus
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(3);
            comp.Find("div.mud-input-error").TextContent.Trim().Should().Be("Required");

            // user puts in a invalid integer value
            await comp.Find("input").ChangeAsync("invalid");
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(3);
            comp.Find("div.mud-input-error").TextContent.Trim().Should().Be("Not a valid number");

            // reset
            await comp.InvokeAsync(() => comp.Instance.ResetAsync());
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);

            // user does not change input value but changes focus
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(3);
            comp.Find("div.mud-input-error").TextContent.Trim().Should().Be("Required");

            // user corrects input
            await comp.Find("input").ChangeAsync(55);
            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        [Test]
        public void TextFieldLabel()
        {
            var value = new DisplayNameLabelClass();

            var comp = Context.Render<MudTextField<string>>(x => x.Add(f => f.For, () => value.String));
            comp.Instance.Label.Should().Be("String LabelAttribute"); //label should be set by the attribute

            var comp2 = Context.Render<MudTextField<string>>(x => x.Add(f => f.For, () => value.String).Add(l => l.Label, "Label Parameter"));
            comp2.Instance.Label.Should().Be("Label Parameter"); //existing label should remain
        }

        /// <summary>
        /// ReadOnly TextFields should not validate when blurred
        /// </summary>
        [Test]
        public async Task ReadOnlyTextFieldShouldNotValidate()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.ReadOnly, true)
                .Add(p => p.Required, true));

            await comp.Find("input").BlurAsync();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
        }

        /// <summary>
        /// https://github.com/MudBlazor/MudBlazor/issues/6322
        /// </summary>
        [Test]
        public async Task OnBlurErrorContentCaughtException()
        {
            var comp = Context.Render<TextFieldErrorContenCaughtException>();
            await comp.Find("input").BlurAsync(new FocusEventArgs());
            var mudAlert = comp.FindComponent<MudAlert>();
            var text = mudAlert.Find("div.mud-alert-message");
            text.InnerHtml.Should().Be("Oh my! We caught an error and handled it!");
        }

        /// <summary>
        /// Reproduce https://github.com/MudBlazor/MudBlazor/issues/7034
        /// </summary>
        [Test]
        public async Task OnBlurWithModifiedValueTriggerValidationOnce1()
        {
            var callCounter = 0;
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Validation, (string _) =>
                {
                    callCounter++;
                    return true;
                })
            );
            await comp.Find("input").ChangeAsync("A");
            callCounter.Should().Be(1);
            await comp.Find("input").BlurAsync();
            callCounter.Should().Be(1);
        }

        /// <summary>
        /// Reproduce https://github.com/MudBlazor/MudBlazor/issues/7034
        /// </summary>
        [Test]
        public async Task OnBlurWithModifiedValueTriggerValidationOnce2()
        {
            var callCounter = 0;
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.OnlyValidateIfDirty, true)
                .Add(p => p.Validation, (string _) =>
                {
                    callCounter++;
                    return true;
                })
            );
            await comp.Find("input").ChangeAsync("A");
            callCounter.Should().Be(1);
            await comp.Find("input").BlurAsync();
            callCounter.Should().Be(1);
        }

        /// <summary>
        /// Reproduce https://github.com/MudBlazor/MudBlazor/issues/7034
        /// </summary>
        [Test]
        public async Task OnBlurWithModifiedValueTriggerValidationOnce3()
        {
            var callCounter = 0;
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.OnlyValidateIfDirty, true)
                .Add(p => p.Validation, async (string _) =>
                {
                    callCounter++;
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    return true;
                })
            );
            await comp.Find("input").ChangeAsync("A");
            await comp.WaitForAssertionAsync(() => callCounter.Should().Be(1));
            await comp.Find("input").BlurAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            callCounter.Should().Be(1);
        }

        [Test]
        public async Task OnKeyDownErrorContentCaughtException()
        {
            var comp = Context.Render<TextFieldErrorContenCaughtException>();
            await comp.Find("input").KeyDownAsync(new KeyboardEventArgs { Key = "Enter", Type = "keydown" });
            var mudAlert = comp.FindComponent<MudAlert>();
            var text = mudAlert.Find("div.mud-alert-message");
            text.InnerHtml.Should().Be("Oh my! We caught an error and handled it!");
        }

        [Test]
        public async Task OnKeyUpErrorContentCaughtException()
        {
            var comp = Context.Render<TextFieldErrorContenCaughtException>();
            await comp.Find("input").KeyUpAsync(new KeyboardEventArgs { Key = "Enter", Type = "keyup" });
            var mudAlert = comp.FindComponent<MudAlert>();
            var text = mudAlert.Find("div.mud-alert-message");
            text.InnerHtml.Should().Be("Oh my! We caught an error and handled it!");
        }

        /// <summary>
        /// Validate that a re-render of a debounced text field does not cause a loss of uncommitted text.
        /// </summary>
        [Test]
        public async Task DebouncedTextFieldRerender()
        {
            var timeProvider = new FakeTimeProvider();
            Context.Services.AddSingleton<TimeProvider>(timeProvider);

            var comp = Context.Render<DebouncedTextFieldRerenderTest>();
            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            await comp.Find("input").InputAsync(new ChangeEventArgs { Value = "test" });

            // trigger first value change
            timeProvider.Advance(TimeSpan.FromMilliseconds(comp.Instance.DebounceInterval));

            // trigger delayed re-render
            await comp.InvokeAsync(() => comp.Find("#re-render-button").Click());

            // imitate "typing in progress" by extending the debounce interval until component re-renders
            var elapsedTime = 0;
            var currentText = "test";
            while (elapsedTime < comp.Instance.RerenderDelay)
            {
                var delay = comp.Instance.DebounceInterval / 2;
                currentText += "a";
                await comp.Find("input").InputAsync(new ChangeEventArgs { Value = currentText });
                timeProvider.Advance(TimeSpan.FromMilliseconds(delay));
                elapsedTime += delay;
            }

            // after the final debounce, the value should be updated without swallowing any user input
            timeProvider.Advance(TimeSpan.FromMilliseconds(comp.Instance.DebounceInterval));
            await Task.Delay(10); // Give the debouncer's InvokeAsync a chance to complete
            textField.ReadValue.Should().Be(currentText);
            textField.ReadText.Should().Be(currentText);
        }

        [Test]
        public void DebouncedTextField_Should_RenderDefaultValueTextOnFirstRender()
        {
            var defaultValue = "test";
            var comp = Context.Render<DebouncedTextFieldRerenderTest>(parameters => parameters
                .Add(p => p.Value, defaultValue));
            var textfield = comp.FindComponent<MudTextField<string>>().Instance;
            textfield.ReadText.Should().Be(defaultValue);
        }

        /// <summary>
        /// Validate that a re-render of a debounced text field does not cause a loss of uncommitted text while changing format.
        /// </summary>
        [Test]
        public async Task DebouncedTextFieldFormatChangeRerender()
        {
            var timeProvider = new FakeTimeProvider();
            Context.Services.AddSingleton<TimeProvider>(timeProvider);

            var comp = Context.Render<DebouncedTextFieldFormatChangeRerenderTest>();
            var textField = comp.FindComponent<MudTextField<DateTime>>().Instance;
            DateTime expectedFinalDateTime = default;

            // ensure text is updated on initialize
            textField.ReadText.Should().Be(comp.Instance.Date.Date.ToString(comp.Instance.Format, CultureInfo.InvariantCulture));

            // trigger the format change
            await comp.Find("#format-change-button").ClickAsync();

            // imitate "typing in progress" by extending the debounce interval until component re-renders
            var elapsedTime = 0;
            var currentText = comp.Instance.Date.Date.ToString(comp.Instance.Format, CultureInfo.InvariantCulture);
            while (elapsedTime < comp.Instance.RerenderDelay)
            {
                var delay = comp.Instance.DebounceInterval / 2;
                currentText += "a";
                await comp.Find("input").InputAsync(currentText);
                timeProvider.Advance(TimeSpan.FromMilliseconds(delay));
                elapsedTime += delay;
            }

            // after the format change delay has elapsed, the uncommitted text is retained (with the old Format)
            textField.ReadText.Should().Be(currentText);

            // once debounce occurs, both value and text are reset because they define an invalid DateTime,
            // now with the new Format
            timeProvider.Advance(TimeSpan.FromMilliseconds(comp.Instance.DebounceInterval));
            comp.WaitForAssertion(() =>
            {
                textField.ReadValue.Should().Be(expectedFinalDateTime);
                textField.ReadText.Should().Be(expectedFinalDateTime.ToString(comp.Instance.Format, CultureInfo.InvariantCulture));
            });
        }

        /// <summary>
        /// A text field with sizing enabled should contain the correct sizing class.
        /// </summary>
        [TestCase(InputSizing.Auto, "mud-input-sizing-auto")]
        [TestCase(InputSizing.Fixed, "mud-input-sizing-fixed")]
        public void TextFieldSizingHasClass(InputSizing sizing, string expectedClass)
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Sizing, sizing));

            comp.Find("div.mud-input").ClassList.Should().Contain(expectedClass);
        }

        /// <summary>
        /// A text field with a label should auto-generate an id and use that id on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TextFieldWithLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.Render<MudTextField<string>>(parameters
                => parameters.Add(p => p.Label, "Test Label"));

            comp.Find("input").Id.Should().NotBeNullOrEmpty();
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(comp.Find("input").Id);
        }

        /// <summary>
        /// A text field with a label and UserAttributesId should use the UserAttributesId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TextFieldWithLabelAndUserAttributesId_Should_UseUserAttributesIdForInputAndAccompanyingLabel()
        {
            var expectedId = "userattributes-id";
            var comp = Context.Render<MudTextField<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "Id", expectedId } }));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// A text field with a label, a UserAttributesId, and an InputId should use the InputId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TextFieldWithLabelAndUserAttributesIdAndInputId_Should_UseInputIdForInputAndAccompanyingLabel()
        {
            var expectedId = "input-id";
            var comp = Context.Render<MudTextField<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "Id", "userattributes-id" } })
                    .Add(p => p.InputId, "input-id"));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// A text field with a mask and a label should auto-generate an id and use that id on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TextFieldWithMultipleLinesAndLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.Render<MudTextField<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.Lines, 5));

            comp.Find("textarea").Id.Should().NotBeNullOrEmpty();
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(comp.Find("textarea").Id);
        }

        /// <summary>
        /// A text field with multiple lines, a label, and UserAttributesId should use the UserAttributesId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TextFieldWithMultipleLinesAndLabelAndUserAttributesId_Should_UseUserAttributesIdForInputAndAccompanyingLabel()
        {
            var expectedId = "userattributes-id";
            var comp = Context.Render<MudTextField<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "Id", expectedId } })
                    .Add(p => p.Lines, 5));

            comp.Find("textarea").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// A text field with multiple lines, a label, a UserAttributesId, and an InputId should use the InputId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TextFieldWithMultipleLinesAndLabelAndUserAttributesIdAndInputId_Should_UseInputIdForInputAndAccompanyingLabel()
        {
            var expectedId = "input-id";
            var comp = Context.Render<MudTextField<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "Id", "userattributes-id" } })
                    .Add(p => p.InputId, "input-id")
                    .Add(p => p.Lines, 5));

            comp.Find("textarea").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// A text field with a mask and a label should auto-generate an id and use that id on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TextFieldWithMaskAndLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.Render<MudTextField<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.Mask, new PatternMask("0000")));

            comp.Find("input").Id.Should().NotBeNullOrEmpty();
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(comp.Find("input").Id);
        }

        /// <summary>
        /// A text field with a mask, a label, and UserAttributesId should use the UserAttributesId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TextFieldWithMaskAndLabelAndUserAttributesId_Should_UseUserAttributesIdForInputAndAccompanyingLabel()
        {
            var expectedId = "userattributes-id";
            var comp = Context.Render<MudTextField<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "Id", expectedId } })
                    .Add(p => p.Mask, new PatternMask("0000")));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// A text field with a mask, a label, a UserAttributesId, and an InputId should use the InputId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TextFieldWithMaskAndLabelAndUserAttributesIdAndInputId_Should_UseInputIdForInputAndAccompanyingLabel()
        {
            var expectedId = "input-id";
            var comp = Context.Render<MudTextField<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "Id", "userattributes-id" } })
                    .Add(p => p.InputId, expectedId)
                    .Add(p => p.Mask, new PatternMask("0000")));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// A text field with a mask, multiple lines, and a label should auto-generate an id and use that id on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TextFieldWithMaskAndMultipleLinesAndLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.Render<MudTextField<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.Mask, new PatternMask("0000"))
                    .Add(p => p.Lines, 5));

            comp.Find("textarea").Id.Should().NotBeNullOrEmpty();
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(comp.Find("textarea").Id);
        }

        /// <summary>
        /// A text field with a mask, multiple lines, a label, and UserAttributesId should use the UserAttributesId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TextFieldWithMaskAndMultipleLinesAndLabelAndUserAttributesId_Should_UseUserAttributesIdForInputAndAccompanyingLabel()
        {
            var expectedId = "userattributes-id";
            var comp = Context.Render<MudTextField<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "Id", expectedId } })
                    .Add(p => p.Mask, new PatternMask("0000"))
                    .Add(p => p.Lines, 5));

            comp.Find("textarea").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// A text field with a mask, multiple lines, a label, a UserAttributesId, and an InputId should use the InputId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TextFieldWithMaskAndMultipleLinesAndLabelAndUserAttributesIdAndInputId_Should_UseInputIdForInputAndAccompanyingLabel()
        {
            var expectedId = "input-id";
            var comp = Context.Render<MudTextField<string>>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "Id", "userattributes-id" } })
                    .Add(p => p.InputId, expectedId)
                    .Add(p => p.Mask, new PatternMask("0000"))
                    .Add(p => p.Lines, 5));

            comp.Find("textarea").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// Optional TextField should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalTextField_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.Render<MudTextField<string>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required TextField should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredTextField_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Bug : https://github.com/MudBlazor/MudBlazor/issues/10606
        /// When the user inputs a single space, the required text field should show an error.
        /// </summary>
        [Test]
        public async Task RequiredTextField_WhenInputOneSpace_ShowError()
        {
            // Arrange

            var comp = Context.Render<MudTextField<string>>(parameters =>
                parameters.Add(p => p.Required, true)
            );
            var textfield = comp.Instance;

            // Act

            await comp.Find("input").ChangeAsync(" ");

            // Assert

            textfield.GetState(x => x.Error).Should().BeTrue();
        }

        /// <summary>
        /// Required and aria-required TextField attributes should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredTextFieldAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<MudTextField<string>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Optional TextField with Sizing=Auto should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalTextFieldWithAutoSizing_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Sizing, InputSizing.Auto));

            comp.Find("textarea").HasAttribute("required").Should().BeFalse();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required TextField with Sizing=Auto should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredTextFieldWithAutoSizing_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Required, true)
                .Add(p => p.Sizing, InputSizing.Auto));

            comp.Find("textarea").HasAttribute("required").Should().BeTrue();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required TextField with Sizing=Auto attributes should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredTextFieldWithAutoSizingAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Sizing, InputSizing.Auto));

            comp.Find("textarea").HasAttribute("required").Should().BeFalse();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("false");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("textarea").HasAttribute("required").Should().BeTrue();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Optional TextField with Mask should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalTextFieldWithMask_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Mask, new PatternMask("0000")));

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required TextField with Mask should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredTextFieldWithMask_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Required, true)
                .Add(p => p.Mask, new PatternMask("0000")));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required TextField with Mask should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredTextFieldWithMask_Should_BeDynamic()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Mask, new PatternMask("0000")));

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Optional TextField with Mask and multiple lines should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalTextFieldWithMaskAndMultipleLines_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Lines, 5)
                .Add(p => p.Mask, new PatternMask("0000")));

            comp.Find("textarea").HasAttribute("required").Should().BeFalse();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required TextField with Mask and multiple lines should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredTextFieldWithMaskAndMultipleLines_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Lines, 5)
                .Add(p => p.Required, true)
                .Add(p => p.Mask, new PatternMask("0000")));

            comp.Find("textarea").HasAttribute("required").Should().BeTrue();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required TextField with Mask and multiple lines should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredTextFieldWithMaskAndMultipleLines_Should_BeDynamic()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Lines, 5)
                .Add(p => p.Mask, new PatternMask("0000")));

            comp.Find("textarea").HasAttribute("required").Should().BeFalse();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("false");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("textarea").HasAttribute("required").Should().BeTrue();
            comp.Find("textarea").GetAttribute("aria-required").Should().Be("true");
        }

        [Test]
        public void Should_render_conversion_error_message()
        {
            var comp = Context.Render<MudTextField<int>>(parameters => parameters
                .Add(p => p.ErrorId, "error-id")
                .Add(p => p.Text, "not a number")
                .Add(p => p.Converter, new DummyErrorConverter()));

            comp.Instance.ConversionErrorMessage.Should().NotBeNullOrEmpty();
            comp.Find("#error-id").InnerHtml.Should().Be(comp.Instance.ConversionErrorMessage);
            comp.Find("input").GetAttribute("aria-describedby").Should().Be("error-id");
            comp.Find("input").GetAttribute("aria-invalid").Should().Be("true");
        }

        [TestCase(Adornment.Start, false, false)]
        [TestCase(Adornment.Start, false, true)]
        [TestCase(Adornment.Start, true, false)]
        [TestCase(Adornment.Start, true, true)]
        [TestCase(Adornment.End, false, false)]
        [TestCase(Adornment.End, false, true)]
        [TestCase(Adornment.End, true, false)]
        [TestCase(Adornment.End, true, true)]
        public void Should_render_aria_label_for_adornment_if_provided(Adornment adornment, bool withMultipleLines, bool withMask)
        {
            var ariaLabel = "the aria label";
            var lines = withMultipleLines ? 5 : 1;
            var mask = withMask ? new PatternMask("0000") : null;
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Adornment, adornment)
                .Add(p => p.AdornmentIcon, Icons.Material.Filled.Accessibility)
                .Add(p => p.AdornmentAriaLabel, ariaLabel)
                .Add(p => p.Lines, lines)
                .Add(p => p.Mask, mask));

            comp.Find(".mud-input-adornment-icon").Attributes.GetNamedItem("aria-label")!.Value.Should().Be(ariaLabel);
        }

        /// <summary>
        /// Verifies that a text field with various configurations renders the expected <c>aria-describedby</c> attribute.
        /// </summary>
        // no helpers, validates error id is present when error is present
        [TestCase(false, false, false, false)]
        [TestCase(false, false, false, true)]
        [TestCase(false, false, true, false)]
        [TestCase(false, false, true, true)]
        // with helper text, helper element should only be present when there is no error
        [TestCase(false, true, false, false)]
        [TestCase(false, true, false, true)]
        [TestCase(false, true, true, false)]
        [TestCase(false, true, true, true)]
        // with user helper id, helper id should always be present
        [TestCase(true, false, false, false)]
        [TestCase(true, false, false, true)]
        [TestCase(true, false, true, false)]
        [TestCase(true, false, true, true)]
        // with user helper id and helper text, should always favour user helper id
        [TestCase(true, true, false, false)]
        [TestCase(true, true, false, true)]
        [TestCase(true, true, true, false)]
        [TestCase(true, true, true, true)]
        public async Task Should_pass_various_aria_describedby_tests(
            bool withUserHelperId,
            bool withHelperText,
            bool withMultipleLines,
            bool withMask)
        {
            var inputId = "input-id";
            var helperId = withUserHelperId ? "user-helper-id" : null;
            var helperText = withHelperText ? "helper text" : null;
            var lines = withMultipleLines ? 5 : 1;
            var mask = withMask ? new PatternMask("0000") : null;
            var errorId = "error-id";
            var errorText = "error text";
            var inputSelector = withMultipleLines ? "textarea" : "input";
            var firstExpectedAriaDescribedBy = withUserHelperId
                ? helperId
                : withHelperText
                    ? $"{inputId}-helper-text"
                    : null;

            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.InputId, inputId)
                .Add(p => p.HelperId, helperId)
                .Add(p => p.HelperText, helperText)
                .Add(p => p.Error, false)
                .Add(p => p.ErrorId, errorId)
                .Add(p => p.ErrorText, errorText)
                .Add(p => p.Lines, lines)
                .Add(p => p.Mask, mask));

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

        [Test]
        public async Task ReadOnlyShouldNotHaveClearButton()
        {
            var comp = Context.Render<MudTextField<string>>(p => p
                .Add(x => x.Text, "some value")
                .Add(x => x.Clearable, true)
                .Add(x => x.ReadOnly, false));

            comp.FindAll(".mud-input-clear-button").Count.Should().Be(1);

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ReadOnly, true)); //no clear button when readonly
            comp.FindAll(".mud-input-clear-button").Count.Should().Be(0);
        }

        [Test]
        public void OutlineLegendRender()
        {
            var comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Variant, Variant.Outlined)
                .Add(p => p.Label, "Test Label"));
            var elem = comp.Find("legend");
            elem.InnerHtml.Should().Be("Test Label");

            comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Variant, Variant.Outlined)
                .Add(p => p.Label, ""));
            Assert.Throws<ElementNotFoundException>(() =>
            {
                elem = comp.Find("legend");
            });

            comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Variant, Variant.Outlined)
                .Add(p => p.Mask, new PatternMask("0000"))
                .Add(p => p.Label, "test"));
            elem = comp.Find("legend");
            elem.InnerHtml.Should().Be("test");

            comp = Context.Render<MudTextField<string>>(parameters => parameters
                .Add(p => p.Variant, Variant.Outlined)
                .Add(p => p.Mask, new PatternMask("0000"))
                .Add(p => p.Label, ""));
            Assert.Throws<ElementNotFoundException>(() =>
            {
                elem = comp.Find("legend");
            });
        }

        [Test]
        public async Task GetCurrentCaretPositionAsyncCallsJsCorrectly()
        {
            var jsRuntimeMock = new Mock<IJSRuntime>();
            jsRuntimeMock.Setup(x => x.InvokeAsync<int>("mudInput.getCaretPosition", It.IsAny<object[]>())).ReturnsAsync(5);
            Context.Services.AddSingleton(jsRuntimeMock.Object);

            var textField = Context.Render<MudTextField<string>>().Instance;
            await textField.GetCurrentCaretPositionAsync();

            jsRuntimeMock.Verify(x => x.InvokeAsync<int>("mudInput.getCaretPosition", It.IsAny<object[]>()), Times.Exactly(1));
        }

        [Test]
        public async Task InsertTextAsyncCallsJsCorrectly()
        {
            var jsRuntimeMock = new Mock<IJSRuntime>();
            jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudInput.insertAtPosition", It.IsAny<object[]>()));
            Context.Services.AddSingleton(jsRuntimeMock.Object);

            var textField = Context.Render<MudTextField<string>>().Instance;
            await textField.InsertTextAsync("test", 3);
            jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudInput.insertAtPosition", It.IsAny<object[]>()), Times.Exactly(1));
        }

        [Test]
        public async Task InsertTextAtCurrentCaretPositionAsyncCallsJsCorrectly()
        {
            var jsRuntimeMock = new Mock<IJSRuntime>();
            jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudInput.insertAtCurrentCaretPosition", It.IsAny<object[]>()));
            Context.Services.AddSingleton(jsRuntimeMock.Object);

            var textField = Context.Render<MudTextField<string>>().Instance;
            await textField.InsertTextAtCurrentCaretPositionAsync("test");
            jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudInput.insertAtCurrentCaretPosition", It.IsAny<object[]>()), Times.Exactly(1));
        }
    }
}
