#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System;
using System.Reflection;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Examples;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.Form;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{

    [TestFixture]
    public class FormTests
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
        /// Setting the required textfield's value should set IsValid true
        /// Clearing the value of a required textfield should set form's IsValid to false.
        /// </summary>
        [Test]
        public async Task FormIsValidTest()
        {
            var comp = ctx.RenderComponent<FormIsValidTest>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            var textField = textFieldcomp.Instance;
            // check initial state: form should not be valid, but text field does not display an error initially!
            form.IsValid.Should().Be(false);
            textField.Error.Should().BeFalse();
            textField.ErrorText.Should().BeNullOrEmpty();
            textFieldcomp.Find("input").Change("Marilyn Manson");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            textField.Error.Should().BeFalse();
            textField.ErrorText.Should().BeNullOrEmpty();
            // clear value to null
            textFieldcomp.Find("input").Change(null);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Enter a rock star");
            textField.Error.Should().BeTrue();
            textField.ErrorText.Should().Be("Enter a rock star");
            // set value to "" -> should also be an error
            textFieldcomp.Find("input").Change("");
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Enter a rock star");
            textField.Error.Should().BeTrue();
            textField.ErrorText.Should().Be("Enter a rock star");
            //
            textFieldcomp.Find("input").Change("Kurt Cobain");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            textField.Error.Should().BeFalse();
            textField.ErrorText.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Form's isvalid should be true, no matter whether or not the field was touched
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task FormIsValidTest2()
        {
            var comp = ctx.RenderComponent<FormIsValidTest2>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            // check initial state: form should be valid due to field not being required!
            form.IsValid.Should().Be(true);
            textFieldcomp.Find("input").Change("This value doesn't matter");
            form.IsValid.Should().Be(true);
        }

        /// <summary>
        /// Custom validation func should be called to determine whether or not a form value is good
        /// </summary>
        [Test]
        public async Task FormValidationTest1()
        {
            var validationFunc = new Func<string, bool>(x => x?.StartsWith("Marilyn") == true);
            var comp = ctx.RenderComponent<FormValidationTest>(ComponentParameter.CreateParameter("validation", validationFunc));
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            var textField = textFieldcomp.Instance;
            // check initial state: form should not be valid, but text field does not display an error initially!
            form.IsValid.Should().Be(false);
            textField.Error.Should().BeFalse();
            textField.ErrorText.Should().BeNullOrEmpty();
            // this rock star starts with Marilyn
            textFieldcomp.Find("input").Change("Marilyn Manson");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            textField.Error.Should().BeFalse();
            textField.ErrorText.Should().BeNullOrEmpty();
            // this rock star doesn't start with Marilyn
            textFieldcomp.Find("input").Change("Kurt Cobain");
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            textField.Error.Should().BeTrue();
            textField.ErrorText.Should().Be("Invalid");

            // note: this logic is invalid, so it was removed. Validaton funcs are always called
            // the validation func must validate non-required empty fields as valid.
            //
            //// value is not required, so don't call the validation func on empty text
            //await comp.InvokeAsync(() => textField.Value = "");
            //form.IsValid.Should().Be(true);
            //form.Errors.Length.Should().Be(0);
            //textField.Error.Should().BeFalse();
            //textField.ErrorText.Should().BeNullOrEmpty();

            // ok, not a rock star, but a star nonetheless
            textFieldcomp.Find("input").Change("Marilyn Monroe");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            textField.Error.Should().BeFalse();
            textField.ErrorText.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Custom validation func should be called to determine whether or not a form value is good
        /// </summary>
        [Test]
        public async Task FormValidationTest2()
        {
            var validationFunc = new Func<string, string>(s =>
            {
                if (!(s.StartsWith("Marilyn") || s.EndsWith("Manson")))
                    return "Not a star!";
                return null;
            });
            var comp = ctx.RenderComponent<FormValidationTest>(ComponentParameter.CreateParameter("validation", validationFunc));
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            form.IsValid.Should().Be(false);
            textFieldcomp.Find("input").Change("Marilyn Manson");
            form.IsValid.Should().Be(true);
            // this one might not be a star, but our custom validation func deems him valid nonetheless
            textFieldcomp.Find("input").Change("Charles Manson");
            form.IsValid.Should().Be(true);

            // note: this logic is invalid, so it was removed. Validaton funcs are always called
            // the validation func must validate non-required empty fields as valid.
            //
            //// value is not required, so don't call the validation func on empty text
            //await comp.InvokeAsync(() => textField.Value = "");
            //form.IsValid.Should().Be(true);

            // clearly a star
            textFieldcomp.Find("input").Change("Marilyn Monroe");
            form.IsValid.Should().Be(true);
            // not a star according to our validation func
            textFieldcomp.Find("input").Change("Manson Marilyn");
            form.IsValid.Should().Be(false);
        }

        /// <summary>
        /// Reset() should reset the input components of the form
        /// </summary>
        [Test]
        public async Task FormValidationTest3()
        {
            var comp = ctx.RenderComponent<FormValidationTest>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            var textField = textFieldcomp.Instance;
            form.IsValid.Should().Be(false);
            textFieldcomp.Find("input").Change("Some value");
            form.IsValid.Should().Be(true);
            // calling Reset() should reset the textField's value
            await comp.InvokeAsync(() => form.Reset());
            textField.Value.Should().Be(null);
            textField.Text.Should().Be(null);
            form.IsValid.Should().Be(false); // because we did reset validation state as a side-effect.
        }

        /// <summary>
        /// Validate that first async validation call returning after second call will not override result of second call
        /// </summary>
        [Test]
        public async Task FormAsyncValidationTest()
        {
            const int ValidDelay = 100;
            const int InvalidDelay = 200;
            var validationFunc = new Func<string, Task<string>>(async s =>
            {
                if (s == null)
                    return null;
                var valid = (s == "abc");
                await Task.Delay(valid ? ValidDelay : InvalidDelay);
                return valid ? null : "invalid";
            });
            var comp = ctx.RenderComponent<FormValidationTest>(ComponentParameter.CreateParameter("validation", validationFunc));
            Console.WriteLine(comp.Markup);
            var textFieldComp = comp.FindComponent<MudTextField<string>>();
            var textField = textFieldComp.Instance;
            // validate initial field state
            textField.ValidationErrors.Should().BeEmpty();
            // make sure error can be detected
            textFieldComp.Find("input").Change("def");
            comp.WaitForAssertion(() => textField.ValidationErrors.Should().ContainSingle("invalid"), TimeSpan.FromSeconds(5));
            // make sure success can be detected
            textFieldComp.Find("input").Change("abc");
            comp.WaitForAssertion(() => textField.ValidationErrors.Should().BeEmpty(), TimeSpan.FromSeconds(5));
            // send invalid value, then valid value
            textFieldComp.Find("input").Change("def");
            textFieldComp.Find("input").Change("abc");
            // validate that first call result (invalid, longer return time) will not overwrite second call result (valid, shorter return time)
            comp.WaitForAssertion(() => textField.ValidationErrors.Should().BeEmpty(), TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// After changing any of the textfields with a For expression the corresponding chip should show a change message after the textfield blurred.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task EditFormOnFieldChangedTest()
        {
            var comp = ctx.RenderComponent<EditFormOnFieldChangedTest>();
            Console.WriteLine(comp.Markup);
            var textFields = comp.FindAll("input");
            textFields.Count.Should().Be(3);
            var chips = comp.FindAll("span.mud-chip-content");
            chips.Count.Should().Be(3);
            foreach (var chip in chips)
                chip.TextContent.Trim().Should().EndWith("not changed");
            comp.FindAll("input")[0].Change(new ChangeEventArgs() { Value = "asdf" });
            comp.FindAll("input")[0].Blur();
            comp.FindComponents<MudTextField<string>>()[0].Instance.Text.Should().Be("asdf");
            comp.FindAll("span.mud-chip-content")[0].TextContent.Trim().Should().Be("Field1 changed");
            comp.FindAll("span.mud-chip-content")[1].TextContent.Trim().Should().EndWith("not changed");
            comp.FindAll("span.mud-chip-content")[2].TextContent.Trim().Should().EndWith("not changed");
            comp.FindAll("input")[1].Change(new ChangeEventArgs() { Value = "yxcv" });
            comp.FindAll("input")[1].Blur();
            comp.FindComponents<MudTextField<string>>()[1].Instance.Text.Should().Be("yxcv");
            comp.FindAll("span.mud-chip-content")[0].TextContent.Trim().Should().Be("Field1 changed");
            comp.FindAll("span.mud-chip-content")[1].TextContent.Trim().Should().EndWith("not changed", "Because it has no For, so the change can not be forwarded to the edit context for lack of a FieldIdentifier");
            comp.FindAll("span.mud-chip-content")[2].TextContent.Trim().Should().EndWith("not changed");
            comp.FindAll("input")[2].Change(new ChangeEventArgs() { Value = "qwer" });
            comp.FindAll("input")[2].Blur();
            comp.FindComponents<MudTextField<string>>()[2].Instance.Text.Should().Be("qwer");
            comp.FindAll("span.mud-chip-content")[0].TextContent.Trim().Should().Be("Field1 changed");
            comp.FindAll("span.mud-chip-content")[1].TextContent.Trim().Should().EndWith("not changed");
            comp.FindAll("span.mud-chip-content")[2].TextContent.Trim().Should().EndWith("Field3 changed");
        }

        /// <summary>
        /// Based on error report. Clicking the checkbox should not influence the other form fields.
        /// </summary>
        [Test]
        public async Task FormWithCheckboxTest()
        {
            var comp = ctx.RenderComponent<FormWithCheckboxTest>();
            Console.WriteLine(comp.Markup);
            var textFields = comp.FindAll("input");
            textFields.Count.Should().Be(4); // three textfields, one checkbox
            // let's fill in some values
            comp.FindAll("input")[0].Change("Garfield");
            comp.FindAll("input")[0].Blur();
            comp.FindAll("input")[1].Change("Jon");
            comp.FindAll("input")[1].Blur();
            comp.FindAll("input")[2].Change("17"); // kg ;)
            comp.FindAll("input")[2].Blur();
            foreach (var tf in comp.FindComponents<MudTextField<string>>())
                tf.Instance.Text.Should().NotBeNullOrEmpty();
            comp.FindComponent<MudTextField<int>>().Instance.Value.Should().Be(17);
            // then click the checkbox
            comp.FindComponent<MudCheckBox<bool>>().Instance.Checked.Should().Be(true);
            comp.FindAll("input")[3].Change(false); // it was on before
            comp.FindComponent<MudCheckBox<bool>>().Instance.Checked.Should().Be(false);
            // the text fields should be unchanged
            foreach (var tf in comp.FindComponents<MudTextField<string>>())
                tf.Instance.Text.Should().NotBeNullOrEmpty();
            comp.FindComponent<MudTextField<int>>().Instance.Value.Should().Be(17);
        }

        /// <summary>
        /// Based on error report. Even without clicking the checkbox the form should
        /// be valid if the checkbox is not required.
        /// </summary>
        [Test]
        public async Task FormWithCheckboxTest2()
        {
            var comp = ctx.RenderComponent<FormWithCheckboxTest>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            form.IsValid.Should().BeTrue(because: "none of the fields are required");
        }

        /// <summary>
        /// Form should become valid as soon as all required fields are filled in correctly.
        /// </summary>
        [Test]
        public async Task Form_Should_BecomeValidIfUntouchedFieldsAreNotRequired()
        {
            var comp = ctx.RenderComponent<FormValidationTest2>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            form.IsValid.Should().BeFalse(because: "textfield is required");
            var textfield = comp.FindComponent<MudTextField<string>>();
            textfield.Find("input").Change("Moby Dick");
            form.IsValid.Should().BeTrue(because: "select is not required");
        }

        /// <summary>
        /// Form should become invalid as soon as an in-convertible value is entered.
        /// </summary>
        [Test]
        public async Task Form_Should_BecomeInValidWhenAConversionErrorOccurs()
        {
            var comp = ctx.RenderComponent<FormConversionErrorTest>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            form.IsValid.Should().BeTrue();
            var textfield = comp.FindComponent<MudTextField<int>>();
            textfield.Find("input").Input("Not and int");
            form.IsValid.Should().BeFalse(because: "conversion error is forwarded to form");
            textfield.Find("input").Input("17");
            form.IsValid.Should().BeTrue(because: "conversion error is gone");
        }

        /// <summary>
        /// Testing the functionality of the MudForm example from the docs.
        /// </summary>
        [Test]
        public async Task MudFormExampleTest()
        {
            var comp = ctx.RenderComponent<MudFormExample>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            form.IsValid.Should().BeFalse(because: "it contains required fields that are not filled out");
            var buttons = comp.FindComponents<MudButton>();
            // click validate button
            var validateButton = buttons[1];
            validateButton.Find("button").Click();
            var textfields = comp.FindComponents<MudTextField<string>>();
            textfields[0].Instance.HasErrors.Should().BeTrue();
            textfields[0].Instance.ErrorText.Should().Be("User name is required!");
            textfields[1].Instance.HasErrors.Should().BeTrue();
            textfields[1].Instance.ErrorText.Should().Be("Email is required!");
            textfields[2].Instance.HasErrors.Should().BeTrue();
            textfields[2].Instance.ErrorText.Should().Be("Password is required!");
            var checkbox = comp.FindComponent<MudCheckBox<bool>>();
            checkbox.Instance.HasErrors.Should().BeTrue();
            checkbox.Instance.ErrorText.Should().Be("You must agree");
            // click reset validation
            var resetValidationButton = buttons[3];
            resetValidationButton.Find("button").Click();
            comp.WaitForState(() => form.Errors.Length == 0);
            textfields[0].Instance.HasErrors.Should().BeFalse();
            textfields[0].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[1].Instance.HasErrors.Should().BeFalse();
            textfields[1].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[2].Instance.HasErrors.Should().BeFalse();
            textfields[2].Instance.ErrorText.Should().BeNullOrEmpty();
            checkbox.Instance.HasErrors.Should().BeFalse();
            checkbox.Instance.ErrorText.Should().BeNullOrEmpty();
            // fill in the form to make it valid
            textfields[0].Find("input").Change("Rick Sanchez");
            textfields[1].Find("input").Change("rick.sanchez@citadel-of-ricks.com");
            textfields[2].Find("input").Change("Wabalabadubdub1234!");
            textfields[3].Find("input").Change("Wabalabadubdub1234!");
            checkbox.Find("input").Change(true);
            comp.WaitForAssertion(() => form.IsValid.Should().BeTrue());
            comp.WaitForState(() => form.Errors.Length == 0);
            // click reset
            var resetButton = buttons[2];
            resetButton.Find("button").Click();
            comp.WaitForState(() => form.Errors.Length == 0);
            textfields[0].Instance.HasErrors.Should().BeFalse();
            textfields[0].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[0].Instance.Text.Should().BeNullOrEmpty();
            textfields[1].Instance.HasErrors.Should().BeFalse();
            textfields[1].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[1].Instance.Text.Should().BeNullOrEmpty();
            textfields[2].Instance.HasErrors.Should().BeFalse();
            textfields[2].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[2].Instance.Text.Should().BeNullOrEmpty();
            checkbox.Instance.HasErrors.Should().BeFalse();
            checkbox.Instance.ErrorText.Should().BeNullOrEmpty();
            checkbox.Instance.Checked.Should().BeFalse();
            // TODO: fill out the form with errors, field after field, check how fields get validation erros after blur
        }

        /// <summary>
        /// Setting the required radiogroup value should set IsValid true
        /// Clearing the value of a required radiogroup should set form's IsValid to false.
        /// </summary>
        [Test]
        public async Task FormWithRadioGroupIsValidTest()
        {
            var comp = ctx.RenderComponent<FormWithRadioGroupTest>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var radioGroupcomp = comp.FindComponent<MudRadioGroup<string>>();
            var radioGroup = radioGroupcomp.Instance;
            // check initial state: form should not be valid
            form.IsValid.Should().Be(false);
            radioGroup.Error.Should().BeFalse();
            radioGroup.ErrorText.Should().BeNullOrEmpty();
            // click on first radio: form should be valid now
            radioGroupcomp.Find("input").Click();
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            radioGroup.Error.Should().BeFalse();
            radioGroup.ErrorText.Should().BeNullOrEmpty();
            // clear selection
            comp.SetParam("Selected", null);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            radioGroup.Error.Should().BeTrue();
            radioGroup.ErrorText.Should().Be("Required");
        }

        /// <summary>
        /// DatePicker should be validated like every other form component
        /// </summary>
        [Test]
        public async Task FormWithDatePickerTest()
        {
            var comp = ctx.RenderComponent<FormWithDatePickerTest>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var dateComp = comp.FindComponent<MudDatePicker>();
            var datepicker = comp.FindComponent<MudDatePicker>().Instance;
            // check initial state: form should not be valid because datepicker is required
            form.IsValid.Should().Be(false);
            datepicker.Error.Should().BeFalse();
            datepicker.ErrorText.Should().BeNullOrEmpty();
            // input a date
            dateComp.Find("input").Change("2001-01-31");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            datepicker.Error.Should().BeFalse();
            datepicker.ErrorText.Should().BeNullOrEmpty();
            // clear selection
            comp.SetParam(x => x.Date, null);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            datepicker.Error.Should().BeTrue();
            datepicker.ErrorText.Should().Be("Required");
        }

        /// <summary>
        /// DatePicker should be validated like every other form component
        /// </summary>
        [Test]
        public async Task Form_Should_ValidateDatePickerTest()
        {
            var comp = ctx.RenderComponent<FormWithDatePickerTest>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var dateComp = comp.FindComponent<MudDatePicker>();
            var datepicker = comp.FindComponent<MudDatePicker>().Instance;
            dateComp.SetParam(x => x.Validation, new Func<DateTime?, string>(date => date != null && date.Value.Year >= 2000 ? null : "Year must be >= 2000"));
            dateComp.Find("input").Change("2001-01-31");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            datepicker.Error.Should().BeFalse();
            datepicker.ErrorText.Should().BeNullOrEmpty();
            // set invalid date:
            comp.SetParam(x => x.Date, (DateTime?)new DateTime(1999, 1, 1));
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Year must be >= 2000");
            datepicker.Error.Should().BeTrue();
            datepicker.ErrorText.Should().Be("Year must be >= 2000");
        }

        /// <summary>
        /// TimePicker should be validated like every other form component
        /// </summary>
        [Test]
        public async Task FormWithTimePickerTest()
        {
            var comp = ctx.RenderComponent<FormWithTimePickerTest>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var dateComp = comp.FindComponent<MudTimePicker>();
            var datepicker = comp.FindComponent<MudTimePicker>().Instance;
            // check initial state: form should not be valid because datepicker is required
            form.IsValid.Should().Be(false);
            datepicker.Error.Should().BeFalse();
            datepicker.ErrorText.Should().BeNullOrEmpty();
            // input a date
            dateComp.Find("input").Change("09:30");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            datepicker.Error.Should().BeFalse();
            datepicker.ErrorText.Should().BeNullOrEmpty();
            // clear selection
            comp.SetParam(x => x.Time, null);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            datepicker.Error.Should().BeTrue();
            datepicker.ErrorText.Should().Be("Required");
        }

        /// <summary>
        /// TimePicker should be validated like every other form component
        /// </summary>
        [Test]
        public async Task Form_Should_ValidateTimePickerTest()
        {
            var comp = ctx.RenderComponent<FormWithTimePickerTest>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var dateComp = comp.FindComponent<MudTimePicker>();
            var datepicker = comp.FindComponent<MudTimePicker>().Instance;
            dateComp.SetParam(x => x.Validation, new Func<TimeSpan?, string>(time => time != null && time.Value.Minutes == 0 ? null : "Only full hours allowed"));
            dateComp.Find("input").Change("09:00");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            datepicker.Error.Should().BeFalse();
            datepicker.ErrorText.Should().BeNullOrEmpty();
            // set invalid date:
            comp.SetParam(x => x.Time, (TimeSpan?)new TimeSpan(0, 17, 05, 00)); // "17:05"
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Only full hours allowed");
            datepicker.Error.Should().BeTrue();
            datepicker.ErrorText.Should().Be("Only full hours allowed");
        }

        /// <summary>
        /// Testing the functionality of the EditForm example from the docs.
        /// </summary>
        [Test]
        public async Task EditFormExample_EmptyValidation()
        {
            var comp = ctx.RenderComponent<EditFormExample>();
            Console.WriteLine(comp.Markup);
            // same effect as clicking the validate button
            comp.Find("form").Submit();
            var textfields = comp.FindComponents<MudTextField<string>>();
            textfields[0].Instance.HasErrors.Should().BeTrue();
            textfields[0].Instance.ErrorText.Should().Be("The Username field is required.");
            textfields[1].Instance.HasErrors.Should().BeTrue();
            textfields[1].Instance.ErrorText.Should().Be("The Email field is required.");
            textfields[2].Instance.HasErrors.Should().BeTrue();
            textfields[2].Instance.ErrorText.Should().Be("The Password field is required.");
            textfields[3].Instance.HasErrors.Should().BeTrue();
            textfields[3].Instance.ErrorText.Should().Be("The Password2 field is required.");
        }

        /// <summary>
        /// Testing the functionality of the EditForm example from the docs.
        /// </summary>
        [Test]
        public async Task EditFormExample_FillInValues()
        {
            var comp = ctx.RenderComponent<EditFormExample>();
            //Console.WriteLine(comp.Markup);
            comp.FindAll("input")[0].Change("Rick Sanchez");
            comp.FindAll("input")[0].Blur();
            comp.FindAll("input")[1].Change("rick.sanchez@citadel-of-ricks.com");
            comp.FindAll("input")[1].Blur();
            comp.FindAll("input")[2].Change("Wabalabadubdub1234!");
            comp.FindAll("input")[2].Blur();
            comp.FindAll("input")[3].Change("Wabalabadubdub1234!");
            comp.FindAll("input")[3].Blur();
            // same effect as clicking the validate button
            comp.Find("form").Submit();
            var textfields = comp.FindComponents<MudTextField<string>>();
            textfields[0].Instance.ErrorText.Should().Be("Name length can't be more than 8.");
            textfields[0].Instance.HasErrors.Should().BeTrue();
            textfields[1].Instance.HasErrors.Should().BeFalse();
            textfields[1].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[2].Instance.HasErrors.Should().BeFalse();
            textfields[2].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[3].Instance.HasErrors.Should().BeFalse();
            textfields[3].Instance.ErrorText.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Ensure validation attributes aren't incorrectly called with `null` context.
        /// </summary>
        /// <see cref="https://github.com/Garderoben/MudBlazor/issues/1229"/>
        [Test]
        public async Task EditForm_Validation_NullContext()
        {
            var comp = ctx.RenderComponent<EditFormIssue1229>();
            // Check first run attribute
            EditFormIssue1229.TestAttribute.ValidationContextOnCall.Should().BeEmpty();
            // Trigger change
            var input= comp.Find("input");
            input.Change("Test");
            input.Blur();
            // Verify context was set
            EditFormIssue1229.TestAttribute.ValidationContextOnCall.Should().NotBeEmpty();
            foreach (var vc in EditFormIssue1229.TestAttribute.ValidationContextOnCall)
            {
                vc.Should().NotBeNull();
            }
        }
    }
}

