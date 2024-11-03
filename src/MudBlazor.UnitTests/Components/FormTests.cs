#pragma warning disable CS1998 // async without await

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor.Docs.Examples;
using MudBlazor.UnitTests.Dummy;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.Form;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class FormTests : BunitTest
    {
        /// <summary>
        /// Setting the required textfield's value should set IsValid true
        /// Clearing the value of a required textfield should set form's IsValid to false.
        /// </summary>
        [Test]
        public async Task FormIsValidTest()
        {
            var comp = Context.RenderComponent<FormIsValidTest>();
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
            var comp = Context.RenderComponent<FormIsValidTest2>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            // check initial state: form should be valid due to field not being required!
            form.IsValid.Should().Be(true);
            textFieldcomp.Find("input").Change("This value doesn't matter");
            form.IsValid.Should().Be(true);
        }

        /// <summary>
        /// Form should update the bound variables valid and touched whenever they change.
        /// </summary>
        [Test]
        public async Task FormIsValidTest3()
        {
            var comp = Context.RenderComponent<FormIsValidTest3>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textFields = comp.FindComponents<MudTextField<string>>();
            // check initial state: form should be invalid due to having a required field that is not filled
            form.IsValid.Should().Be(false);
            form.IsTouched.Should().Be(false);
            comp.FindComponents<MudSwitch<bool>>()[0].Instance.Value.Should().Be(false);
            comp.FindComponents<MudSwitch<bool>>()[1].Instance.Value.Should().Be(false);
            // filling in the required field
            textFields[1].Find("input").Change("Fill in the required field to make this form valid");
            form.IsValid.Should().Be(true);
            comp.FindComponents<MudSwitch<bool>>()[0].Instance.Value.Should().Be(true);
            comp.FindComponents<MudSwitch<bool>>()[1].Instance.Value.Should().Be(true);
        }

        /// <summary>
        /// Form should update the bound variable valid to true even though it is set false upon first render because there is no required field.
        /// </summary>
        [Test]
        public async Task FormIsValidTest4()
        {
            var comp = Context.RenderComponent<FormIsValidTest4>();
            var form = comp.FindComponent<MudForm>().Instance;
            // check initial state: form should be valid due to having no required field, but the user's two-way binding did override that value to false
            comp.WaitForAssertion(() => form.IsValid.Should().Be(true));
            comp.WaitForAssertion(() => comp.FindComponent<MudSwitch<bool>>().Instance.Value.Should().Be(true));
        }

        /// <summary>
        /// Changing a fields value should set IsTouched to true
        /// </summary>
        [Test]
        public async Task FormIsTouchedTest()
        {
            var comp = Context.RenderComponent<FormIsTouchedTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            var dateComp = comp.FindComponent<MudDatePicker>();
            // check initial state: form should not be touched
            form.IsTouched.Should().Be(false);
            // input a date, istouched should be true
            dateComp.Find("input").Change("2001-01-31");
            form.IsTouched.Should().Be(true);

            //reset should set touched to false
            await comp.InvokeAsync(() => form.ResetAsync());
            form.IsTouched.Should().Be(false);

            // clear value to null
            textFieldcomp.Find("input").Change("value is changed");
            form.IsTouched.Should().Be(true);

            //reset validation should not reset touched state
            await comp.InvokeAsync(() => form.ResetValidation());
            form.IsTouched.Should().Be(true);
        }

        /// <summary>
        /// Changing the nested form fields value should set IsTouched
        /// </summary>
        [Test]
        public async Task FormIsTouchedAndNestedFormIsNotTouchedWhenParentFormFieldIsTouchedTest()
        {
            var comp = Context.RenderComponent<FormIsTouchedNestedTest>();
            var formsComp = comp.FindComponents<MudForm>();
            var textCompFields = comp.FindComponents<MudTextField<string>>();
            var form = formsComp[0].Instance;
            var nestedForm = formsComp[1].Instance;

            // check initial state: form should not be touched
            form.IsTouched.Should().Be(false);
            nestedForm.IsTouched.Should().Be(false);
            // input a date, istouched should be true
            textCompFields[0].Find("input").Change("2001-01-31");
            form.IsTouched.Should().Be(true);
            nestedForm.IsTouched.Should().Be(false);

            //reset should set touched to false
            await comp.InvokeAsync(() => form.ResetAsync());
            form.IsTouched.Should().Be(false);
            nestedForm.IsTouched.Should().Be(false);

            // clear value to null
            textCompFields[0].Find("input").Change("value is changed");
            form.IsTouched.Should().Be(true);
            nestedForm.IsTouched.Should().Be(false);

            //reset validation should not reset touched state
            await comp.InvokeAsync(() => form.ResetValidation());
            form.IsTouched.Should().Be(true);
            nestedForm.IsTouched.Should().Be(false);
        }

        /// <summary>
        /// Changing the nested form fields value should set IsTouched to true on parent form
        /// </summary>
        [Test]
        public async Task FormIsUnTouchedWhenNestedFormTouchedTest()
        {
            var comp = Context.RenderComponent<FormIsTouchedNestedTest>();
            var formsComp = comp.FindComponents<MudForm>();
            var textCompFields = comp.FindComponents<MudTextField<string>>();
            var dateCompFields = comp.FindComponents<MudDatePicker>();
            var form = formsComp[0].Instance;
            var nestedForm = formsComp[1].Instance;
            var nestedFormDateField = dateCompFields[1].Instance;

            // check initial state: form should not be touched
            form.IsTouched.Should().Be(false);
            nestedForm.IsTouched.Should().Be(false);
            // input a date, istouched should be true
            textCompFields[1].Find("input").Change("2001-01-31");
            form.IsTouched.Should().Be(true);
            nestedForm.IsTouched.Should().Be(true);

            //reset should set touched to false
            await comp.InvokeAsync(() => form.ResetAsync());
            form.IsTouched.Should().Be(false);
            nestedForm.IsTouched.Should().Be(false);

            // clear value to null
            textCompFields[3].Find("input").Change("value is changed");
            form.IsTouched.Should().Be(false);
            nestedForm.IsTouched.Should().Be(true);

            //reset validation should not reset touched state
            await comp.InvokeAsync(() => nestedFormDateField.ResetValidation());
            form.IsTouched.Should().Be(false);
            nestedForm.IsTouched.Should().Be(true);
        }

        /// <summary>
        /// Calling ResetTouched should set the IsTouched property to false
        /// </summary>
        [Test]
        public async Task FormIsTouchedResetTest()
        {
            var comp = Context.RenderComponent<FormIsTouchedTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dateComp = comp.FindComponent<MudDatePicker>();
            // check initial state: form should not be touched
            form.IsTouched.Should().Be(false);
            // input a date, isTouched should be true
            dateComp.Find("input").Change("2001-01-31");
            form.IsTouched.Should().Be(true);

            // resetTouched should set the IsTouched property to default(false)
            await comp.InvokeAsync(() => form.ResetTouched());
            form.IsTouched.Should().Be(false);
        }



        /// <summary>
        /// Custom validation func should be called to determine whether or not a form value is good
        /// </summary>
        [Test]
        public async Task FormValidationTest1()
        {
            var validationFunc = new Func<string, bool>(x => x?.StartsWith("Marilyn") == true);
            var comp = Context.RenderComponent<FormValidationTest>(ComponentParameter.CreateParameter("validation", validationFunc));
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

            // note: this logic is invalid, so it was removed. Validation funcs are always called
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
            var comp = Context.RenderComponent<FormValidationTest>(ComponentParameter.CreateParameter("validation", validationFunc));
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            form.IsValid.Should().Be(false);
            textFieldcomp.Find("input").Change("Marilyn Manson");
            form.IsValid.Should().Be(true);
            // this one might not be a star, but our custom validation func deems him valid nonetheless
            textFieldcomp.Find("input").Change("Charles Manson");
            form.IsValid.Should().Be(true);

            // note: this logic is invalid, so it was removed. Validation funcs are always called
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
            var comp = Context.RenderComponent<FormValidationTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            var textField = textFieldcomp.Instance;
            form.IsValid.Should().Be(false);
            textFieldcomp.Find("input").Change("Some value");
            form.IsValid.Should().Be(true);
            // calling Reset() should reset the textField's value
            await comp.InvokeAsync(() => form.ResetAsync());
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
            var comp = Context.RenderComponent<FormValidationTest>(ComponentParameter.CreateParameter("validation", validationFunc));
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
        /// Validate that text typed during async validation of a component won't swallow user input on re-render.
        /// </summary>
        [Test]
        public async Task FormAsyncValidationWithFieldChangedSubscriberTest()
        {
            var comp = Context.RenderComponent<FormAsyncValidationWithFieldChangedSubscriberTest>();
            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            var input = comp.Find("input");
            input.Input(new ChangeEventArgs { Value = "test" });
            // trigger validation
            await Task.Delay(comp.Instance.DebounceInterval);
            // imitate "typing in progress" by extending the debounce interval until the async validation terminates
            var elapsedTime = 0;
            var currentText = "test";
            while (elapsedTime < comp.Instance.AsyncTaskDelay)
            {
                var delay = comp.Instance.DebounceInterval / 2;
                currentText += "a";
                input.Input(new ChangeEventArgs { Value = currentText });
                await Task.Delay(delay);
                elapsedTime += delay;
            }
            // after the final debounce, the value should be updated without swallowing any user input
            await Task.Delay(comp.Instance.DebounceInterval);
            textField.Value.Should().Be(currentText);
            textField.Text.Should().Be(currentText);
        }

        /// <summary>
        /// After changing any of the textfields with a For expression the corresponding chip should show a change message after the textfield blurred.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task EditFormOnFieldChangedTest()
        {
            var comp = Context.RenderComponent<EditFormOnFieldChangedTest>();
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
            var comp = Context.RenderComponent<FormWithCheckBoxAndTextFieldsTest>();
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
            comp.FindComponent<MudCheckBox<bool>>().Instance.Value.Should().Be(true);
            comp.FindAll("input")[3].Change(false); // it was on before
            comp.FindComponent<MudCheckBox<bool>>().Instance.Value.Should().Be(false);
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
            var comp = Context.RenderComponent<FormWithCheckBoxAndTextFieldsTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            form.IsValid.Should().BeTrue(because: "none of the fields are required");
        }

        /// <summary>
        /// Form should become valid as soon as all required fields are filled in correctly.
        /// </summary>
        [Test]
        public async Task Form_Should_BecomeValidIfUntouchedFieldsAreNotRequired()
        {
            var comp = Context.RenderComponent<FormValidationTest2>();
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
            var comp = Context.RenderComponent<FormConversionErrorTest>();
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
            var comp = Context.RenderComponent<MudFormExample>();
            var form = comp.FindComponent<MudForm>().Instance;
            comp.FindComponent<MudForm>().SetParam(x => x.ValidationDelay, 0);
            comp.WaitForAssertion(() => form.IsValid.Should().BeFalse(because: "it contains required fields that are not filled out"));
            var buttons = comp.FindComponents<MudButton>();
            // click validate button
            var validateButton = buttons[1];
            validateButton.Find("button").Click();
            var textfields = comp.FindComponents<MudTextField<string>>();
            comp.WaitForAssertion(() => textfields[0].Instance.HasErrors.Should().BeTrue());
            textfields[0].Instance.ErrorText.Should().Be("User name is required!");
            comp.WaitForAssertion(() => textfields[1].Instance.HasErrors.Should().BeTrue());
            textfields[1].Instance.ErrorText.Should().Be("Email is required!");
            comp.WaitForAssertion(() => textfields[2].Instance.HasErrors.Should().BeTrue());
            textfields[2].Instance.ErrorText.Should().Be("Password is required!");
            var checkbox = comp.FindComponent<MudCheckBox<bool>>();
            comp.WaitForAssertion(() => checkbox.Instance.HasErrors.Should().BeTrue());
            checkbox.Instance.ErrorText.Should().Be("You must agree");
            // click reset validation
            var resetValidationButton = buttons[3];
            resetValidationButton.Find("button").Click();
            comp.WaitForState(() => form.Errors.Length == 0);
            comp.WaitForAssertion(() => textfields[0].Instance.HasErrors.Should().BeFalse());
            textfields[0].Instance.ErrorText.Should().BeNullOrEmpty();
            comp.WaitForAssertion(() => textfields[1].Instance.HasErrors.Should().BeFalse());
            textfields[1].Instance.ErrorText.Should().BeNullOrEmpty();
            comp.WaitForAssertion(() => textfields[2].Instance.HasErrors.Should().BeFalse());
            textfields[2].Instance.ErrorText.Should().BeNullOrEmpty();
            comp.WaitForAssertion(() => checkbox.Instance.HasErrors.Should().BeFalse());
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
            comp.WaitForAssertion(() => textfields[0].Instance.HasErrors.Should().BeFalse());
            textfields[0].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[0].Instance.Text.Should().BeNullOrEmpty();
            comp.WaitForAssertion(() => textfields[1].Instance.HasErrors.Should().BeFalse());
            textfields[1].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[1].Instance.Text.Should().BeNullOrEmpty();
            comp.WaitForAssertion(() => textfields[2].Instance.HasErrors.Should().BeFalse());
            textfields[2].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[2].Instance.Text.Should().BeNullOrEmpty();
            comp.WaitForAssertion(() => checkbox.Instance.HasErrors.Should().BeFalse());
            checkbox.Instance.ErrorText.Should().BeNullOrEmpty();
            comp.WaitForAssertion(() => checkbox.Instance.Value.Should().BeFalse());
            // TODO: fill out the form with errors, field after field, check how fields get validation errors after blur
        }

        /// <summary>
        /// Setting the required radiogroup value should set IsValid true
        /// Clearing the value of a required radiogroup should set form's IsValid to false.
        /// </summary>
        [Test]
        public async Task FormWithRadioGroupIsValidTest()
        {
            var comp = Context.RenderComponent<FormWithRadioGroupTest>();
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
        /// ColorPicker should be validated like every other form component when color is changed via inputs
        /// </summary>
        [Test]
        public async Task Form_Should_Validate_ColorPicker_When_ColorSelectedViaInputs()
        {
            var comp = Context.RenderComponent<FormWithColorPickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var colorPickerComp = comp.FindComponent<MudColorPicker>();
            var colorPicker = comp.FindComponent<MudColorPicker>().Instance;
            var forbiddenColor = colorPicker.Value;
            colorPickerComp.SetParam(x => x.Validation, new Func<MudColor, string>(color => color != null && color.Value == forbiddenColor.Value ? $"{forbiddenColor.Value} is not allowed" : null));
            // should not be valid since the default color is invalid
            form.IsTouched.Should().BeFalse();
            form.IsValid.Should().BeFalse();
            colorPicker.Error.Should().BeFalse();
            colorPicker.ErrorText.Should().BeNullOrEmpty();
            // input a valid color
            await comp.InvokeAsync(() => colorPickerComp.FindAll("input")[0].Change("#111111"));
            form.IsTouched.Should().BeTrue();
            form.IsValid.Should().BeTrue();
            form.Errors.Length.Should().Be(0);
            colorPicker.Error.Should().BeFalse();
            colorPicker.ErrorText.Should().BeNullOrEmpty();
            // reset to forbidden color
            comp.SetParam(x => x.ColorValue, forbiddenColor);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be($"{forbiddenColor.Value} is not allowed");
            colorPicker.Error.Should().BeTrue();
            colorPicker.ErrorText.Should().Be($"{forbiddenColor.Value} is not allowed");
        }

        /// <summary>
        /// ColorPicker should be validated like every other form component when color is selected using picker
        /// </summary>
        [Test]
        public async Task Form_Should_ValidateColorPickerTest_When_ColorSelectedViaPicker()
        {
            var comp = Context.RenderComponent<FormWithColorPickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var colorPickerComp = comp.FindComponent<MudColorPicker>();
            var colorPicker = comp.FindComponent<MudColorPicker>().Instance;
            var forbiddenColor = colorPicker.Palette.First();
            colorPickerComp.SetParam(x => x.Validation, new Func<MudColor, string>(color => color != null && color.Value == forbiddenColor.Value ? $"{forbiddenColor.Value} is not allowed" : null));
            // initial form state
            form.IsTouched.Should().BeFalse();
            form.IsValid.Should().BeFalse();

            await comp.InvokeAsync(() => comp.Find("input").Click());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
            // open color collection view
            await comp.InvokeAsync(() => comp.Find("div.mud-picker-color-dot-current").Click());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-color-collection").Count.Should().Be(1));

            // set valid color
            await comp.InvokeAsync(() => comp.FindAll("div.mud-picker-color-collection>div.mud-picker-color-dot").Skip(1).First().Click());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-color-collection").Count.Should().Be(0));
            form.IsTouched.Should().BeTrue();
            form.IsValid.Should().BeTrue();
            form.Errors.Length.Should().Be(0);
            colorPicker.Error.Should().BeFalse();
            colorPicker.ErrorText.Should().BeNullOrEmpty();

            await comp.InvokeAsync(() => comp.Find("div.mud-picker-color-dot-current").Click());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-color-collection").Count.Should().Be(1));

            // set invalid color
            await comp.InvokeAsync(() => comp.FindAll("div.mud-picker-color-collection>div.mud-picker-color-dot").First().Click());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-color-collection").Count.Should().Be(0));
            form.IsTouched.Should().BeTrue();
            form.IsValid.Should().BeFalse();
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be($"{forbiddenColor.Value} is not allowed");
            colorPicker.Error.Should().BeTrue();
            colorPicker.ErrorText.Should().Be($"{forbiddenColor.Value} is not allowed");
        }

        /// <summary>
        /// DatePicker should be validated like every other form component
        /// </summary>
        [Test]
        public async Task FormWithDatePickerTest()
        {
            var comp = Context.RenderComponent<FormWithDatePickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dateComp = comp.FindComponent<MudDatePicker>();
            var datepicker = comp.FindComponent<MudDatePicker>().Instance;
            // check initial state: form should not be valid because datepicker is required
            form.IsValid.Should().Be(false);
            datepicker.Error.Should().BeFalse();
            datepicker.ErrorText.Should().BeNullOrEmpty();
            // input a date
            dateComp.Find("input").Change(new DateTime(2001, 01, 31).ToShortDateString());
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
            var comp = Context.RenderComponent<FormWithDatePickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dateComp = comp.FindComponent<MudDatePicker>();
            var datepicker = comp.FindComponent<MudDatePicker>().Instance;
            dateComp.SetParam(x => x.Validation, new Func<DateTime?, string>(date => date != null && date.Value.Year >= 2000 ? null : "Year must be >= 2000"));
            dateComp.Find("input").Change(new DateTime(2001, 01, 31).ToShortDateString());
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
        /// DateRangePicker should be validated like every other form component when the dateRange
        /// is changed via inputs
        /// </summary>
        [Test]
        public async Task Form_Should_Validate_DateRangePicker_When_DateRangeSelectedViaInputs()
        {
            var comp = Context.RenderComponent<FormWithDateRangePickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dateRangeComp = comp.FindComponent<MudDateRangePicker>();
            var dateRangePicker = comp.FindComponent<MudDateRangePicker>().Instance;
            var firstDateTime = new DateTime(2023, 01, 20);
            var secondDateTime = new DateTime(2023, 02, 20);
            // check initial state: form should not be valid because dateRangePicker is required
            form.IsValid.Should().Be(false);
            dateRangePicker.Error.Should().BeFalse();
            dateRangePicker.ErrorText.Should().BeNullOrEmpty();
            // input a date
            await comp.InvokeAsync(() => dateRangeComp.FindAll("input")[0].Change(firstDateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern)));
            await comp.InvokeAsync(() => dateRangeComp.FindAll("input")[1].Change(secondDateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern)));
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            dateRangePicker.Error.Should().BeFalse();
            dateRangePicker.ErrorText.Should().BeNullOrEmpty();
            // clear selection
            comp.SetParam(x => x.DateRange, null);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            dateRangePicker.Error.Should().BeTrue();
            dateRangePicker.ErrorText.Should().Be("Required");
        }

        /// <summary>
        /// DateRangePicker should be validated like every other form component when the dateRange is selected using
        /// the picker
        /// </summary>
        [Test]
        public async Task Form_Should_Validate_DateRangePicker_When_DateRangeSelectedViaPicker()
        {
            var comp = Context.RenderComponent<FormWithDateRangePickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dateRangePicker = comp.FindComponent<MudDateRangePicker>().Instance;
            // check initial state: form should not be valid because dateRangePicker is required
            form.IsValid.Should().Be(false);
            dateRangePicker.Error.Should().BeFalse();
            dateRangePicker.ErrorText.Should().BeNullOrEmpty();
            comp.Find("input").Click();
            // clicking day buttons to select a date range
            await comp.InvokeAsync(() => comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("10")).Click());
            await comp.InvokeAsync(() => comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("11")).Click());
            // wait for picker to close
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover").Count.Should().Be(1));
            form.IsTouched.Should().Be(true);
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            dateRangePicker.Error.Should().BeFalse();
            dateRangePicker.ErrorText.Should().BeNullOrEmpty();
            // clear selection
            comp.SetParam(x => x.DateRange, null);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            dateRangePicker.Error.Should().BeTrue();
            dateRangePicker.ErrorText.Should().Be("Required");
        }

        /// <summary>
        /// TimePicker should be validated like every other form component
        /// </summary>
        [Test]
        public async Task FormWithTimePickerTest()
        {
            var comp = Context.RenderComponent<FormWithTimePickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var timePickerComp = comp.FindComponent<MudTimePicker>();
            var timePicker = comp.FindComponent<MudTimePicker>().Instance;
            // check initial state: form should not be valid because datepicker is required
            form.IsValid.Should().Be(false);
            timePicker.Error.Should().BeFalse();
            timePicker.ErrorText.Should().BeNullOrEmpty();
            // input a date
            timePickerComp.Find("input").Change("09:30");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            timePicker.Error.Should().BeFalse();
            timePicker.ErrorText.Should().BeNullOrEmpty();
            // clear selection
            comp.SetParam(x => x.Time, null);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            timePicker.Error.Should().BeTrue();
            timePicker.ErrorText.Should().Be("Required");
        }

        /// <summary>
        /// TimePicker should be validated like every other form component
        /// </summary>
        [Test]
        public async Task Form_Should_ValidateTimePickerTest()
        {
            var comp = Context.RenderComponent<FormWithTimePickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var timeComp = comp.FindComponent<MudTimePicker>();
            var timePicker = comp.FindComponent<MudTimePicker>().Instance;
            timeComp.SetParam(x => x.Validation, new Func<TimeSpan?, string>(time => time != null && time.Value.Minutes == 0 ? null : "Only full hours allowed"));
            timeComp.Find("input").Change("09:00");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            timePicker.Error.Should().BeFalse();
            timePicker.ErrorText.Should().BeNullOrEmpty();
            // set invalid date:
            comp.SetParam(x => x.Time, (TimeSpan?)new TimeSpan(0, 17, 05, 00)); // "17:05"
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Only full hours allowed");
            timePicker.Error.Should().BeTrue();
            timePicker.ErrorText.Should().Be("Only full hours allowed");
        }

        /// <summary>
        /// FileUpload should be validated like every other form component when a file is added
        /// </summary>
        [Test]
        public async Task Form_Should_Validate_FileUpload_When_FileAdded()
        {
            var comp = Context.RenderComponent<FormWithFileUploadTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var fileUploadComp = comp.FindComponent<MudFileUpload<IBrowserFile>>();
            var fileUploadInstance = comp.FindComponent<MudFileUpload<IBrowserFile>>().Instance;
            var input = fileUploadComp.FindComponent<InputFile>();
            var fileName = "cat.jpg";
            var fileToUpload = InputFileContent.CreateFromText("I am a cat image, trust me.", fileName);

            // check initial state: form should not be valid because fileUploadInstance is required
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeFalse();
            fileUploadInstance.Error.Should().BeFalse();
            fileUploadInstance.ErrorText.Should().BeNullOrEmpty();

            // add a file
            input.UploadFiles(fileToUpload);
            fileUploadInstance.Files.Should().NotBeNull();
            fileUploadInstance.Files.Name.Should().Be(fileName);

            // form should now be valid and touched
            form.IsValid.Should().BeTrue();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(0);

            // clear selection, form should now be invalid
            await input.ClearFilesAsync();
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            fileUploadInstance.Error.Should().BeTrue();
            fileUploadInstance.ErrorText.Should().Be("Required");
        }

        /// <summary>
        /// FileUpload should be validated like every other form component when a file is cleared
        /// </summary>
        [Test]
        public async Task Form_Should_Validate_FileUpload_When_FileCleared()
        {
            var fileName = "cat.jpg";
            var defaultFile = new DummyBrowserFile(fileName, DateTimeOffset.Now, 0, "image/jpeg", Array.Empty<byte>());
            var fileToUpload = InputFileContent.CreateFromText("I am a cat image, trust me.", "cat.jpg");
            var comp = Context.RenderComponent<FormWithFileUploadTest>(
                ComponentParameterFactory.Parameter(nameof(FormWithFileUploadTest.File), defaultFile));
            var form = comp.FindComponent<MudForm>().Instance;
            var fileUploadComp = comp.FindComponent<MudFileUpload<IBrowserFile>>();
            var fileUploadInstance = comp.FindComponent<MudFileUpload<IBrowserFile>>().Instance;
            var input = fileUploadComp.FindComponent<InputFile>();

            // check initial state: form should not be valid because form is untouched
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeFalse();
            fileUploadInstance.Error.Should().BeFalse();
            fileUploadInstance.ErrorText.Should().BeNullOrEmpty();

            // clear files
            await input.ClearFilesAsync();
            fileUploadInstance.Files.Should().BeNull();

            // form should now be invalid because a file is required
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            fileUploadInstance.Error.Should().BeTrue();
            fileUploadInstance.ErrorText.Should().Be("Required");

            // re-add a file, form should now be valid and touched
            input.UploadFiles(fileToUpload);
            form.IsValid.Should().BeTrue();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(0);
            fileUploadInstance.Error.Should().BeFalse();
            fileUploadInstance.Files.Name.Should().Be(fileName);
        }

        /// <summary>
        /// FileUpload should be validated like every other form component when a file is cleared programmatically
        /// </summary>
        [Test]
        public async Task Form_Should_Validate_FileUpload_When_File_Cleared_Programmatically()
        {
            var fileName = "cat.jpg";
            var defaultFile = new DummyBrowserFile(fileName, DateTimeOffset.Now, 0, "image/jpeg", Array.Empty<byte>());
            var fileToUpload = InputFileContent.CreateFromText("I am a cat image, trust me.", "cat.jpg");
            var comp = Context.RenderComponent<FormWithFileUploadTest>(
                ComponentParameterFactory.Parameter(nameof(FormWithFileUploadTest.File), defaultFile));
            var form = comp.FindComponent<MudForm>().Instance;
            var fileUploadComp = comp.FindComponent<MudFileUpload<IBrowserFile>>();
            var fileUploadInstance = comp.FindComponent<MudFileUpload<IBrowserFile>>().Instance;
            var input = fileUploadComp.FindComponent<InputFile>();

            // check initial state: form should not be valid because form is untouched
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeFalse();
            fileUploadInstance.Files.Should().NotBeNull();
            fileUploadInstance.Error.Should().BeFalse();
            fileUploadInstance.ErrorText.Should().BeNullOrEmpty();

            // clear files
            await comp.InvokeAsync(async () => await fileUploadInstance.ClearAsync());
            fileUploadInstance.Files.Should().BeNull();

            // form should now be invalid because a file is required
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            fileUploadInstance.Error.Should().BeTrue();
            fileUploadInstance.ErrorText.Should().Be("Required");

            // re-add a file, form should now be valid and touched
            input.UploadFiles(fileToUpload);
            form.IsValid.Should().BeTrue();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(0);
            fileUploadInstance.Error.Should().BeFalse();
            fileUploadInstance.Files.Name.Should().Be(fileName);
        }

        /// <summary>
        /// FileUpload should be validated like every other form component when a file is cleared via ClearAsync method
        /// </summary>
        [Test]
        public async Task Form_Should_Validate_FileUpload_When_File_Cleared_Via_ClearAsync_Method()
        {
            var fileName = "cat.jpg";
            var fileToUpload = InputFileContent.CreateFromText("I am a cat image, trust me.", "cat.jpg");
            var comp = Context.RenderComponent<FormWithFileUploadAndDragAndDropActivatorTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var fileUploadComp = comp.FindComponent<MudFileUpload<IBrowserFile>>();
            var fileUploadInstance = comp.FindComponent<MudFileUpload<IBrowserFile>>().Instance;
            var input = fileUploadComp.FindComponent<InputFile>();

            // check initial state: form should not be valid because form is untouched
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeFalse();
            fileUploadInstance.Files.Should().NotBeNull();
            fileUploadInstance.Error.Should().BeFalse();
            fileUploadInstance.ErrorText.Should().BeNullOrEmpty();

            // clear files
            await comp.InvokeAsync(() => comp.Find("button#clear-button").Click());
            fileUploadInstance.Files.Should().BeNull();

            // form should now be invalid because a file is required
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            fileUploadInstance.Error.Should().BeTrue();
            fileUploadInstance.ErrorText.Should().Be("Required");

            // re-add a file, form should now be valid and touched
            input.UploadFiles(fileToUpload);
            form.IsValid.Should().BeTrue();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(0);
            fileUploadInstance.Error.Should().BeFalse();
            fileUploadInstance.Files.Name.Should().Be(fileName);
        }

        /// <summary>
        /// Testing the functionality of the EditForm example from the docs.
        /// </summary>
        [Test]
        public async Task EditFormExample_EmptyValidation()
        {
            var comp = Context.RenderComponent<EditFormExample>();
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
            var comp = Context.RenderComponent<EditFormExample>();
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
        /// <see cref="https://github.com/MudBlazor/MudBlazor/issues/1229"/>
        [Test]
        public async Task EditForm_Validation_NullContext()
        {
            var comp = Context.RenderComponent<EditFormIssue1229>();
            // Check first run attribute
            EditFormIssue1229.TestAttribute.ValidationContextOnCall.Should().BeEmpty();
            // Trigger change
            var input = comp.Find("input");
            input.Change("Test");
            input.Blur();
            // Verify context was set
            EditFormIssue1229.TestAttribute.ValidationContextOnCall.Should().NotBeEmpty();
            foreach (var vc in EditFormIssue1229.TestAttribute.ValidationContextOnCall)
            {
                vc.Should().NotBeNull();
            }
        }

        /// <summary>
        /// This test should prevent regressions like #1912, caused by commit 86bc257d (#1868)
        /// </summary>
        [Test]
        public async Task MudForm_MustNot_ValidateOnInitialRender()
        {
            var comp = Context.RenderComponent<MudFormExample>();
            await Task.Delay(100);
            var form = comp.FindComponent<MudForm>().Instance;
            form.Errors.Should().BeEmpty();
        }

        /// <summary>
        /// Testing the functionality of the MudForm example from the docs.
        /// Root MudForm is valid and nested MudForm is invalid
        /// </summary>
        [Test]
        public async Task MudFormExample_FillInValuesRootForm()
        {
            var comp = Context.RenderComponent<FluentValidationComplexExample>();
            comp.FindAll("input")[0].Input("Rick Sanchez");
            comp.FindAll("input")[0].Blur();
            comp.FindAll("input")[1].Input("rick.sanchez@citadel-of-ricks.com");
            comp.FindAll("input")[1].Blur();
            comp.FindAll("input")[3].Input("Wabalabadubdub1234!");
            comp.FindAll("input")[3].Blur();
            comp.FindAll("input")[4].Input("sdfsfsdf!");
            comp.FindAll("input")[4].Blur();
            comp.FindAll("input")[5].Input("adsadasad!");
            comp.FindAll("input")[5].Blur();

            var form = comp.FindComponent<MudForm>().Instance;
            await comp.InvokeAsync(() => form.Validate());
            form.IsValid.Should().BeFalse();

            var textfields = comp.FindComponents<MudTextField<string>>();
            var numericFields = comp.FindComponents<MudNumericField<decimal>>();

            textfields[0].Instance.HasErrors.Should().BeFalse();
            textfields[0].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[1].Instance.HasErrors.Should().BeFalse();
            textfields[1].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[2].Instance.HasErrors.Should().BeFalse();
            textfields[2].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[3].Instance.HasErrors.Should().BeFalse();
            textfields[3].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[4].Instance.HasErrors.Should().BeFalse();
            textfields[4].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[5].Instance.HasErrors.Should().BeFalse();
            textfields[5].Instance.ErrorText.Should().BeNullOrEmpty();

            //Nested Forms
            textfields[6].Instance.HasErrors.Should().BeFalse();
            textfields[6].Instance.ErrorText.Should().BeNullOrEmpty();
            numericFields[0].Instance.HasErrors.Should().BeFalse();
            numericFields[0].Instance.ErrorText.Should().BeNullOrEmpty();

            textfields[7].Instance.HasErrors.Should().BeTrue();
            textfields[7].Instance.ErrorText.Should().NotBeNullOrEmpty();
            numericFields[1].Instance.HasErrors.Should().BeTrue();
            numericFields[1].Instance.ErrorText.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Testing the functionality of the MudForm example from the docs.
        /// Root MudForm is invalid and nested MudForm is valid
        /// </summary>
        [Test]
        public async Task MudFormExample_FillInValuesNestedForm()
        {
            var comp = Context.RenderComponent<FluentValidationComplexExample>();
            comp.FindAll("input")[8].Change("SomeWork");
            comp.FindAll("input")[8].Blur();
            comp.FindAll("input")[9].Change("99");
            comp.FindAll("input")[9].Blur();

            var form = comp.FindComponent<MudForm>().Instance;
            await comp.InvokeAsync(() => form.Validate());
            form.IsValid.Should().BeFalse();

            var textfields = comp.FindComponents<MudTextField<string>>();
            var numericFields = comp.FindComponents<MudNumericField<decimal>>();

            textfields[0].Instance.HasErrors.Should().BeTrue();
            textfields[0].Instance.ErrorText.Should().NotBeNullOrEmpty();
            textfields[1].Instance.HasErrors.Should().BeTrue();
            textfields[1].Instance.ErrorText.Should().NotBeNullOrEmpty();
            textfields[2].Instance.HasErrors.Should().BeFalse();
            textfields[2].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[3].Instance.HasErrors.Should().BeTrue();
            textfields[3].Instance.ErrorText.Should().NotBeNullOrEmpty();
            textfields[4].Instance.HasErrors.Should().BeTrue();
            textfields[4].Instance.ErrorText.Should().NotBeNullOrEmpty();
            textfields[5].Instance.HasErrors.Should().BeTrue();
            textfields[5].Instance.ErrorText.Should().NotBeNullOrEmpty();

            //Nested Forms
            textfields[6].Instance.HasErrors.Should().BeFalse();
            textfields[6].Instance.ErrorText.Should().BeNullOrEmpty();
            numericFields[0].Instance.HasErrors.Should().BeFalse();
            numericFields[0].Instance.ErrorText.Should().BeNullOrEmpty();

            textfields[7].Instance.HasErrors.Should().BeFalse();
            textfields[7].Instance.ErrorText.Should().BeNullOrEmpty();
            numericFields[1].Instance.HasErrors.Should().BeFalse();
            numericFields[1].Instance.ErrorText.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Testing the functionality of the MudForm example from the docs.
        /// Both root MudForm and nested MudForm are valid
        /// </summary>
        [Test]
        public async Task MudFormExample_FillInValues()
        {
            var comp = Context.RenderComponent<FluentValidationComplexExample>();
            comp.FindAll("input")[0].Input("Rick Sanchez");
            comp.FindAll("input")[0].Blur();
            comp.FindAll("input")[1].Input("rick.sanchez@citadel-of-ricks.com");
            comp.FindAll("input")[1].Blur();
            comp.FindAll("input")[3].Input("Wabalabadubdub1234!");
            comp.FindAll("input")[3].Blur();
            comp.FindAll("input")[4].Input("sdfsfsdf!");
            comp.FindAll("input")[4].Blur();
            comp.FindAll("input")[5].Input("adsadasad!");
            comp.FindAll("input")[5].Blur();
            comp.FindAll("input")[8].Change("SomeWork");
            comp.FindAll("input")[8].Blur();
            comp.FindAll("input")[9].Change("99");
            comp.FindAll("input")[9].Blur();

            var form = comp.FindComponent<MudForm>().Instance;
            await comp.InvokeAsync(() => form.Validate());
            form.IsValid.Should().BeTrue();

            var textfields = comp.FindComponents<MudTextField<string>>();
            var numericFields = comp.FindComponents<MudNumericField<decimal>>();

            textfields[0].Instance.HasErrors.Should().BeFalse();
            textfields[0].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[1].Instance.HasErrors.Should().BeFalse();
            textfields[1].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[2].Instance.HasErrors.Should().BeFalse();
            textfields[2].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[3].Instance.HasErrors.Should().BeFalse();
            textfields[3].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[4].Instance.HasErrors.Should().BeFalse();
            textfields[4].Instance.ErrorText.Should().BeNullOrEmpty();
            textfields[5].Instance.HasErrors.Should().BeFalse();
            textfields[5].Instance.ErrorText.Should().BeNullOrEmpty();

            //Nested Forms
            textfields[6].Instance.HasErrors.Should().BeFalse();
            textfields[6].Instance.ErrorText.Should().BeNullOrEmpty();
            numericFields[0].Instance.HasErrors.Should().BeFalse();
            numericFields[0].Instance.ErrorText.Should().BeNullOrEmpty();

            textfields[7].Instance.HasErrors.Should().BeFalse();
            textfields[7].Instance.ErrorText.Should().BeNullOrEmpty();
            numericFields[1].Instance.HasErrors.Should().BeFalse();
            numericFields[1].Instance.ErrorText.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Testing error handling of MudFormComponent.ValidateModelWithFullPathOfMember
        /// Validation func throws an error, the error should contain the exception message
        /// </summary>
        [Test]
        public async Task MudFormComponent_ValidationWithModel_UnexpectedErrorInValidationFunc3()
        {
            var comp = Context.RenderComponent<FormWithSingleTextField>();
            var form = comp.FindComponent<MudForm>();
            var model = new { data = "asdf" };
            form.SetParam(nameof(MudForm.Model), model);
            var tf = comp.FindComponent<MudTextField<string>>();
            var validationFunc = new Func<object, string, IEnumerable<string>>((obj, property) =>
            {
                throw new InvalidOperationException("User error");
            });
            tf.SetParam(nameof(MudTextField<string>.Validation), validationFunc);
            Expression<Func<string>> expression = () => model.data;
            tf.SetParam(nameof(MudTextField<string>.For), expression);
            await comp.InvokeAsync(tf.Instance.Validate);
            tf.Instance.Error.Should().Be(true);
            tf.Instance.ErrorText.Should().Be("Error in validation func: User error");
        }

        /// <summary>
        /// Testing error handling of MudFormComponent.ValidateModelWithFullPathOfMember
        /// We have set no For expression, error should reflect that
        /// </summary>
        [Test]
        public async Task MudFormComponent_ValidationWithModelWithNoFor_ShouldShow_ExpectedError()
        {
            var comp = Context.RenderComponent<FormWithSingleTextField>();
            var form = comp.FindComponent<MudForm>();
            var model = new { data = "asdf" };
            form.SetParam(nameof(MudForm.Model), model);
            var tf = comp.FindComponent<MudTextField<string>>();
            var validationFunc = new Func<object, string, IEnumerable<string>>((obj, property) =>
            {
                throw new InvalidOperationException("User error");
            });
            tf.SetParam(nameof(MudTextField<string>.Validation), validationFunc);
            await comp.InvokeAsync(tf.Instance.Validate);
            tf.Instance.Error.Should().Be(true);
            tf.Instance.ErrorText.Should().Be("For is null, please set parameter For on the form input component of type MudTextField`1");
        }

        /// <summary>
        /// Testing error handling of MudFormComponent.ValidateModelWithFullPathOfMember
        /// We have set no For expression, error should reflect that
        /// </summary>
        [Test]
        public async Task MudFormComponent_AsyncValidationWithModelWithNoFor_ShouldShow_ExpectedError()
        {
            var comp = Context.RenderComponent<FormWithSingleTextField>();
            var form = comp.FindComponent<MudForm>();
            var model = new { data = "asdf" };
            form.SetParam(nameof(MudForm.Model), model);
            var tf = comp.FindComponent<MudTextField<string>>();
            var validationFunc = new Func<object, string, Task<IEnumerable<string>>>((obj, property) =>
            {
                throw new InvalidOperationException("User error");
            });
            tf.SetParam(nameof(MudTextField<string>.Validation), validationFunc);
            await comp.InvokeAsync(tf.Instance.Validate);
            tf.Instance.Error.Should().Be(true);
            tf.Instance.ErrorText.Should().Be("For is null, please set parameter For on the form input component of type MudTextField`1");
        }

        /// <summary>
        /// Testing validation with MudFormComponent.ValidateModelWithFullPathOfMember
        /// </summary>
        [Test]
        public async Task MudFormComponent_ValidationWithModel_UnexpectedErrorInValidationFunc5()
        {
            var comp = Context.RenderComponent<FormWithSingleTextField>();
            var form = comp.FindComponent<MudForm>();
            var model = new { data = "asdf" };
            form.SetParam(nameof(MudForm.Model), model);
            var tf = comp.FindComponent<MudTextField<string>>();
            var validationFunc = new Func<object, string, IEnumerable<string>>((obj, property) =>
            {
                obj.Should().Be(model);
                property.Should().Be("data");
                return new[] { "Error1", "Error2" };
            });
            tf.SetParam(nameof(MudTextField<string>.Validation), validationFunc);
            Expression<Func<string>> expression = () => model.data;
            tf.SetParam(nameof(MudTextField<string>.For), expression);
            await comp.InvokeAsync(tf.Instance.Validate);
            tf.Instance.Error.Should().Be(true);
            tf.Instance.ErrorText.Should().Be("Error1");
        }

        /// <summary>
        /// Calling form.Reset() should clear the text field
        /// </summary>
        [Test]
        public async Task FormReset_Should_ClearTextField()
        {
            var comp = Context.RenderComponent<FormResetTest>();
            var form = comp.FindComponent<MudForm>();
            var textFieldComp = comp.FindComponents<MudTextField<string>>()[1]; //the picker includes a MudTextField, so the MudTextField we want is the second in the DOM
            var textField = textFieldComp.Instance;

            // input some text
            textFieldComp.Find("input").Input("asdf");
            textField.Value.Should().Be("asdf");
            textField.Text.Should().Be("asdf");
            // call reset directly
            await comp.InvokeAsync(() => form.Instance.ResetAsync());
            textField.Value.Should().BeNullOrEmpty();
            textField.Text.Should().BeNullOrEmpty();
            // input some text
            textFieldComp.Find("input").Input("asdf");
            textField.Value.Should().Be("asdf");
            textField.Text.Should().Be("asdf");
            // hit reset button
            comp.Find("button.reset").Click();
            textField.Value.Should().BeNullOrEmpty();
            textField.Text.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Calling form.Reset() should clear the numeric field
        /// </summary>
        [Test]
        public async Task FormReset_Should_ClearNumericField()
        {
            var comp = Context.RenderComponent<FormResetTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var numericFieldComp = comp.FindComponent<MudNumericField<int?>>();
            var numericField = numericFieldComp.Instance;

            // input some text
            numericFieldComp.Find("input").Input(10);
            numericField.Value.Should().Be(10);
            numericField.Text.Should().Be("10");
            // call reset directly
            await comp.InvokeAsync(() => form.ResetAsync());
            numericField.Value.Should().BeNull();
            numericField.Text.Should().BeNullOrEmpty();
            // input some text

            numericFieldComp.Find("input").Input(20);
            numericField.Value.Should().Be(20);
            numericField.Text.Should().Be("20");
            // hit reset button
            comp.Find("button.reset").Click();
            numericField.Value.Should().BeNull();
            numericField.Text.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Calling form.Reset() should clear the datepicker
        /// </summary>
        [Test]
        public async Task FormReset_Should_ClearDatePicker()
        {
            var comp = Context.RenderComponent<FormResetTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var datePickerComp = comp.FindComponent<MudDatePicker>();
            var datePicker = datePickerComp.Instance;
            // create test value and it's localized string representation
            var testDate = new DateTime(2020, 05, 24);
            var testDateString = testDate.ToShortDateString();  // locale independent test, will work e.g. in germany too

            // input a date
            datePickerComp.Find("input").Change(testDateString);
            datePicker.Date.Should().Be(testDate);
            datePicker.Text.Should().Be(testDateString);
            // call reset directly
            await comp.InvokeAsync(() => form.ResetAsync());
            datePicker.Date.Should().BeNull();
            datePicker.Text.Should().BeNullOrEmpty();

            // input a date
            datePickerComp.Find("input").Change(testDateString);
            datePicker.Date.Should().Be(testDate);
            datePicker.Text.Should().Be(testDateString);
            // hit reset button
            comp.Find("button.reset").Click();
            datePicker.Date.Should().BeNull();
            datePicker.Text.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Reset() should reset the form's state
        /// </summary>
        [Test]
        public async Task FormReset_Should_ResetFormStateForFieldsThatWrapMudInput()
        {
            var comp = Context.RenderComponent<FormResetTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var datePickerComp = comp.FindComponent<MudDatePicker>();
            var textFieldComp = comp.FindComponents<MudTextField<string>>()[1]; //the picker includes a MudTextField, so the MudTextField we want is the second in the DOM
            var numericFieldComp = comp.FindComponent<MudNumericField<int?>>();
            // create test value and it's localized string representation
            var testDate = new DateTime(2022, 07, 29);
            var testDateString = testDate.ToShortDateString();  // locale independent test, will work e.g. in germany too

            form.IsValid.Should().Be(false);
            datePickerComp.Find("input").Change(testDateString);
            form.IsValid.Should().Be(false);
            textFieldComp.Find("input").Input("Some value");
            form.IsValid.Should().Be(false);
            numericFieldComp.Find("input").Input("1");
            form.IsValid.Should().Be(true);

            await comp.InvokeAsync(() => form.ResetAsync());
            form.IsValid.Should().Be(false); // required fields
        }

        /// <summary>
        /// Only the top SubscribeToParentForm fields should be registered inside the form.
        /// </summary>
        [Test]
        public async Task MudForm_Should_RegisterOnlyTopSubscribeToParentFormFormControls()
        {
            var comp = Context.RenderComponent<FormShouldRegisterOnlyTopSubscribeToParentFormFormControlsTest>();
            var form = comp.FindComponent<MudFormTestable>().Instance;

            form.FormControls.Count.Should().Be(14);
        }

        /// <summary>
        /// Test the cascading validaton parameter to override field validations or not depending of context.
        /// </summary>
        [Test]
        public async Task MudForm_Validation_Should_OverrideFieldValidation()
        {
            var comp = Context.RenderComponent<FormValidationOverrideFieldValidationTest>();
            var textFields = comp.FindComponents<MudTextField<string>>();
            var numericFields = comp.FindComponents<MudNumericField<int>>();
            var defaultValidation = "v";

            textFields[0].Instance.Validation.Should().Be(defaultValidation);
            textFields[1].Instance.Validation.Should().Be(defaultValidation);
            textFields[2].Instance.Validation.Should().Be(defaultValidation);
            textFields[3].Instance.Validation.Should().Be("a");

            numericFields[0].Instance.Validation.Should().Be(defaultValidation);
            numericFields[1].Instance.Validation.Should().NotBe(defaultValidation);
            numericFields[2].Instance.Validation.Should().Be(defaultValidation);
            numericFields[3].Instance.Validation.Should().Be("b");
        }

        /// <summary>
        /// When the field is initialised from cache, the value can be set before the cascading parameter "Form",
        /// triggering validation. Validations requiring "Form" or "For" properties should not crash.
        /// </summary>
        [Test]
        public async Task FieldValidationWithoutRequiredForm_ShouldNot_Validate()
        {
            var comp = Context.RenderComponent<FieldValidationWithoutRequiredFormTest>();

            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-input-error"));
        }

        /// <summary>
        /// When changing field values, the FieldChanged event should fire with the correct IFormComponent and new value
        /// </summary>
        [Test]
        public async Task FieldChangedEventShouldTriggerTest()
        {
            var comp = Context.RenderComponent<FormFieldChangedTest>();
            var formsComp = comp.FindComponents<MudForm>();

            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            var numeric = comp.FindComponent<MudNumericField<int>>().Instance;
            var radioGroup = comp.FindComponent<MudRadioGroup<string>>().Instance;

            comp.Instance.FormFieldChangedEventArgs.Should().BeNull();

            //in all below cases, the event args should switch to an instance of the field changed and contain the new value that was set

            await comp.InvokeAsync(() => textField.SetText("new value"));
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().Be("new value");
            textField.Should().Be(comp.Instance.FormFieldChangedEventArgs.Field);

            numeric.Value.Should().Be(0);
            await comp.InvokeAsync(() => numeric.Increment());
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().Be(1);
            numeric.Should().Be(comp.Instance.FormFieldChangedEventArgs.Field);

            var inputs = comp.FindAll("input").ToArray();
            // check initial state
            radioGroup.Value.Should().Be(null);
            // click radio 1
            inputs[2].Click();
            radioGroup.Value.Should().Be("1");
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().Be("1");
            radioGroup.Should().Be(comp.Instance.FormFieldChangedEventArgs.Field);

            var fileContent = InputFileContent.CreateFromText("", "upload.txt");

            var mudFile = comp.FindComponent<MudFileUpload<IBrowserFile>>().Instance;
            var input = comp.FindComponent<InputFile>();
            input.UploadFiles(fileContent);

            (comp.Instance.FormFieldChangedEventArgs.NewValue is IBrowserFile).Should().BeTrue();
            mudFile.Should().Be(comp.Instance.FormFieldChangedEventArgs.Field);
        }

        /// <summary>
        /// When changing field values, the FieldChanged event should fire with the correct IFormComponent and new value
        /// </summary>
        [Test]
        public async Task FieldChangedEventShouldTriggerPickerTest()
        {
            var comp = Context.RenderComponent<FormFieldChangedPickerTest>();
            var formsComp = comp.FindComponents<MudForm>();

            var datePicker = comp.FindComponent<MudDatePicker>();
            var timePicker = comp.FindComponent<MudTimePicker>();
            var colorPicker = comp.FindComponent<MudColorPicker>();

            comp.Instance.FormFieldChangedEventArgs.Should().BeNull();

            //in all below cases, the event args should switch to an instance of the field changed and contain the new value that was set

            var dateString = new DateTime(2022, 04, 03).ToShortDateString();
            await comp.InvokeAsync(() => datePicker.Find("input").Change(dateString));
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().Be(new DateTime(2022, 04, 03));
            datePicker.Instance.Should().Be(comp.Instance.FormFieldChangedEventArgs.Field);

            await comp.InvokeAsync(() => timePicker.Find("input").Change("00:45"));
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().Be(new TimeSpan(00, 45, 00));
            timePicker.Instance.Should().Be(comp.Instance.FormFieldChangedEventArgs.Field);

            await comp.InvokeAsync(() => colorPicker.Find("input").Change("#180f6fff"));
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().Be(new MudColor("#180f6fff"));
            colorPicker.Instance.Should().Be(comp.Instance.FormFieldChangedEventArgs.Field);
        }

        /// <summary>
        /// When Validation is set on a Form, it should only set validation on fields that have a For parameter
        /// </summary>
        [Test]
        public async Task FormAutoValidationSetTest()
        {
            var comp = Context.RenderComponent<FormAutomaticValidationTest>();
            var textComps = comp.FindComponents<MudTextField<string>>();
            var dateComps = comp.FindComponents<MudDatePicker>();

            textComps[0].Instance.For.Should().NotBeNull(); //For is set
            textComps[1].Instance.For.Should().BeNull(); //For is not set
            dateComps[0].Instance.For.Should().NotBeNull(); //For is set
            dateComps[1].Instance.For.Should().BeNull(); //For is not set

            //Ensure Validation is only set where For is set
            textComps[0].Instance.Validation.Should().NotBeNull(); //Validation is set
            textComps[1].Instance.Validation.Should().BeNull(); //Validation is not set
            dateComps[0].Instance.Validation.Should().NotBeNull(); //Validation is set
            dateComps[1].Instance.Validation.Should().BeNull(); //Validation is not set
        }

        /// <summaryReadonly
        /// Ensures that all child components are Readonly when the Form is Readonly
        /// </summary>
        [Test]
        public async Task FormReadonlyTest()
        {
            var comp = Context.RenderComponent<FormReadOnlyDisabledTest>();

            var textField = comp.FindComponents<MudTextField<string>>()[0];
            var maskedTextField = comp.FindComponents<MudTextField<string>>()[1];
            var checkBox = comp.FindComponent<MudCheckBox<bool>>();
            var radioGroup = comp.FindComponent<MudRadioGroup<string>>();
            var switch_ = comp.FindComponent<MudSwitch<bool>>();
            var select = comp.FindComponent<MudSelect<string>>(); //at present, we can't test if select is readonly based on attribute or classname. A future PR should enable this.
            var colorPicker = comp.FindComponent<MudColorPicker>();
            var datePicker = comp.FindComponent<MudDatePicker>();
            var dateRangePicker = comp.FindComponent<MudDateRangePicker>();
            var timePicker = comp.FindComponent<MudTimePicker>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>();
            var numericField = comp.FindComponent<MudNumericField<int>>();
            var fileUpload = comp.FindComponent<MudFileUpload<IBrowserFile>>();

            //form readonly = false, comp readonly = false
            textField.Find("input").HasAttribute("readonly").Should().BeFalse();
            maskedTextField.Find("input").HasAttribute("readonly").Should().BeFalse();
            checkBox.Find("label").ClassList.Should().NotContain("mud-readonly");
            radioGroup.Find("label").ClassList.Should().NotContain("mud-readonly");
            switch_.Find("label").ClassList.Should().NotContain("mud-readonly");
            autocomplete.Find("input").HasAttribute("readonly").Should().BeFalse();
            numericField.Find("input").HasAttribute("readonly").Should().BeFalse();
            fileUpload.Find("input").HasAttribute("disabled").Should().BeFalse(); //readonly = disabled in the calse of fileUpload

            //form readonly = true, comp readonly = false
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.FormReadOnly, true).Add(p => p.CompReadOnly, false));

            textField.Find("input").HasAttribute("readonly").Should().BeTrue();
            maskedTextField.Find("input").HasAttribute("readonly").Should().BeTrue();
            checkBox.Find("label").ClassList.Should().Contain("mud-readonly");
            radioGroup.Find("label").ClassList.Should().Contain("mud-readonly");
            switch_.Find("label").ClassList.Should().Contain("mud-readonly");
            autocomplete.Find("input").HasAttribute("readonly").Should().BeTrue();
            numericField.Find("input").HasAttribute("readonly").Should().BeTrue();
            fileUpload.Find("input").HasAttribute("disabled").Should().BeTrue();

            //form readonly = false, comp readonly = true
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.FormReadOnly, false).Add(p => p.CompReadOnly, true));

            textField.Find("input").HasAttribute("readonly").Should().BeTrue();
            maskedTextField.Find("input").HasAttribute("readonly").Should().BeTrue();
            checkBox.Find("label").ClassList.Should().Contain("mud-readonly");
            radioGroup.Find("label").ClassList.Should().Contain("mud-readonly");
            switch_.Find("label").ClassList.Should().Contain("mud-readonly");
            autocomplete.Find("input").HasAttribute("readonly").Should().BeTrue();
            numericField.Find("input").HasAttribute("readonly").Should().BeTrue();
            fileUpload.Find("input").HasAttribute("disabled").Should().BeFalse(); //the file upload can't be readonly

            //form readonly = false, comp readonly = false
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.FormReadOnly, false).Add(p => p.CompReadOnly, false));

            textField.Find("input").HasAttribute("readonly").Should().BeFalse();
            maskedTextField.Find("input").HasAttribute("readonly").Should().BeFalse();
            checkBox.Find("label").ClassList.Should().NotContain("mud-readonly");
            radioGroup.Find("label").ClassList.Should().NotContain("mud-readonly");
            switch_.Find("label").ClassList.Should().NotContain("mud-readonly");
            autocomplete.Find("input").HasAttribute("readonly").Should().BeFalse();
            numericField.Find("input").HasAttribute("readonly").Should().BeFalse();
            fileUpload.Find("input").HasAttribute("disabled").Should().BeFalse();
        }

        /// <summary>
        /// Ensures that all child components are Disabled when the Form is Disabled
        /// </summary>
        [Test]
        public async Task FormDisabledTest()
        {
            var comp = Context.RenderComponent<FormReadOnlyDisabledTest>();

            var textField = comp.FindComponents<MudTextField<string>>()[0];
            var maskedTextField = comp.FindComponents<MudTextField<string>>()[1];
            var checkBox = comp.FindComponent<MudCheckBox<bool>>();
            var radioGroup = comp.FindComponent<MudRadioGroup<string>>();
            var switch_ = comp.FindComponent<MudSwitch<bool>>();
            var select = comp.FindComponent<MudSelect<string>>();
            var colorPicker = comp.FindComponent<MudColorPicker>();
            var datePicker = comp.FindComponent<MudDatePicker>();
            var dateRangePicker = comp.FindComponent<MudDateRangePicker>();
            var timePicker = comp.FindComponent<MudTimePicker>();
            var autocomplete = comp.FindComponent<MudAutocomplete<string>>();
            var numericField = comp.FindComponent<MudNumericField<int>>();
            var fileUpload = comp.FindComponent<MudFileUpload<IBrowserFile>>();

            //form disabled = false, comp disabled = false
            textField.Find("input").HasAttribute("disabled").Should().BeFalse();
            maskedTextField.Find("input").HasAttribute("disabled").Should().BeFalse();
            checkBox.Find("input").HasAttribute("disabled").Should().BeFalse();
            radioGroup.Find("input").HasAttribute("disabled").Should().BeFalse();
            switch_.Find("input").HasAttribute("disabled").Should().BeFalse();
            select.Find("input").HasAttribute("disabled").Should().BeFalse();
            colorPicker.Find("input").HasAttribute("disabled").Should().BeFalse();
            datePicker.Find("input").HasAttribute("disabled").Should().BeFalse();
            dateRangePicker.Find("input").HasAttribute("disabled").Should().BeFalse();
            timePicker.Find("input").HasAttribute("disabled").Should().BeFalse();
            autocomplete.Find("input").HasAttribute("disabled").Should().BeFalse();
            numericField.Find("input").HasAttribute("disabled").Should().BeFalse();
            fileUpload.Find("input").HasAttribute("disabled").Should().BeFalse();

            //form disabled = true, comp disabled = false
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.FormDisabled, true).Add(p => p.CompDisabled, false));

            textField.Find("input").HasAttribute("disabled").Should().BeTrue();
            maskedTextField.Find("input").HasAttribute("disabled").Should().BeTrue();
            checkBox.Find("input").HasAttribute("disabled").Should().BeTrue();
            radioGroup.Find("input").HasAttribute("disabled").Should().BeTrue();
            switch_.Find("input").HasAttribute("disabled").Should().BeTrue();
            select.Find("input").HasAttribute("disabled").Should().BeTrue();
            colorPicker.Find("input").HasAttribute("disabled").Should().BeTrue();
            datePicker.Find("input").HasAttribute("disabled").Should().BeTrue();
            dateRangePicker.Find("input").HasAttribute("disabled").Should().BeTrue();
            timePicker.Find("input").HasAttribute("disabled").Should().BeTrue();
            autocomplete.Find("input").HasAttribute("disabled").Should().BeTrue();
            numericField.Find("input").HasAttribute("disabled").Should().BeTrue();
            fileUpload.Find("input").HasAttribute("disabled").Should().BeTrue();

            //form disabled = false, comp disabled = true
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.FormDisabled, false).Add(p => p.CompDisabled, true));

            textField.Find("input").HasAttribute("disabled").Should().BeTrue();
            maskedTextField.Find("input").HasAttribute("disabled").Should().BeTrue();
            checkBox.Find("input").HasAttribute("disabled").Should().BeTrue();
            radioGroup.Find("input").HasAttribute("disabled").Should().BeTrue();
            switch_.Find("input").HasAttribute("disabled").Should().BeTrue();
            select.Find("input").HasAttribute("disabled").Should().BeTrue();
            colorPicker.Find("input").HasAttribute("disabled").Should().BeTrue();
            datePicker.Find("input").HasAttribute("disabled").Should().BeTrue();
            dateRangePicker.Find("input").HasAttribute("disabled").Should().BeTrue();
            timePicker.Find("input").HasAttribute("disabled").Should().BeTrue();
            autocomplete.Find("input").HasAttribute("disabled").Should().BeTrue();
            numericField.Find("input").HasAttribute("disabled").Should().BeTrue();
            fileUpload.Find("input").HasAttribute("disabled").Should().BeTrue();

            //form disabled = false, comp disabled = false
            comp.SetParametersAndRender(parameters => parameters.Add(p => p.FormDisabled, false).Add(p => p.CompDisabled, false));

            textField.Find("input").HasAttribute("disabled").Should().BeFalse();
            maskedTextField.Find("input").HasAttribute("disabled").Should().BeFalse();
            checkBox.Find("input").HasAttribute("disabled").Should().BeFalse();
            radioGroup.Find("input").HasAttribute("disabled").Should().BeFalse();
            switch_.Find("input").HasAttribute("disabled").Should().BeFalse();
            select.Find("input").HasAttribute("disabled").Should().BeFalse();
            colorPicker.Find("input").HasAttribute("disabled").Should().BeFalse();
            datePicker.Find("input").HasAttribute("disabled").Should().BeFalse();
            dateRangePicker.Find("input").HasAttribute("disabled").Should().BeFalse();
            timePicker.Find("input").HasAttribute("disabled").Should().BeFalse();
            autocomplete.Find("input").HasAttribute("disabled").Should().BeFalse();
            numericField.Find("input").HasAttribute("disabled").Should().BeFalse();
            fileUpload.Find("input").HasAttribute("disabled").Should().BeFalse();
        }

        /// <summary>
        /// Ensures the child MudForm correctly inherits ReadOnly and applies it to its children
        /// </summary>
        [Test]
        public async Task FormNestedReadOnlyTest()
        {
            var comp = Context.RenderComponent<FormNestedReadOnlyDisabledTest>();
            comp.FindAll(".mud-checkbox.mud-readonly").Count.Should().Be(0);

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.ReadOnly, true));
            comp.FindAll(".mud-checkbox.mud-readonly").Count.Should().Be(1);

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.NestedReadOnly, true));
            comp.FindAll(".mud-checkbox.mud-readonly").Count.Should().Be(1);

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.ReadOnly, true).Add(p => p.NestedReadOnly, true));
            comp.FindAll(".mud-checkbox.mud-readonly").Count.Should().Be(1);

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.ReadOnly, false).Add(p => p.NestedReadOnly, false));
            comp.FindAll(".mud-checkbox.mud-readonly").Count.Should().Be(0);
        }

        /// <summary>
        /// Ensures the child MudForm correctly inherits Disabled and applies it to its children
        /// </summary>
        [Test]
        public async Task FormNestedDisabledTest()
        {
            var comp = Context.RenderComponent<FormNestedReadOnlyDisabledTest>();
            comp.FindAll(".mud-checkbox.mud-disabled").Count.Should().Be(0);

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Disabled, true));
            comp.FindAll(".mud-checkbox.mud-disabled").Count.Should().Be(1);

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.NestedDisabled, true));
            comp.FindAll(".mud-checkbox.mud-disabled").Count.Should().Be(1);

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Disabled, true).Add(p => p.NestedDisabled, true));
            comp.FindAll(".mud-checkbox.mud-disabled").Count.Should().Be(1);

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.Disabled, false).Add(p => p.NestedDisabled, false));
            comp.FindAll(".mud-checkbox.mud-disabled").Count.Should().Be(0);
        }

        [Test]
        public async Task FormWithChildFormTest()
        {
            var comp = Context.RenderComponent<FormWithChildForm>();
            var childFormSwitch = comp.Find(".mud-switch-input");
            var parentForm = comp.FindComponent<MudForm>().Instance;
            var parentTextFieldCmp = comp.FindComponent<MudTextField<string>>();
            var parentTextField = parentTextFieldCmp.Instance;
            // check initial state: form should not be valid, but text field does not display an error initially!
            parentForm.IsValid.Should().Be(false);
            parentTextField.Error.Should().BeFalse();
            parentTextField.ErrorText.Should().BeNullOrEmpty();
            parentTextFieldCmp.Find("input").Change("Marilyn Manson");
            parentForm.IsValid.Should().Be(true);
            parentForm.Errors.Length.Should().Be(0);
            parentTextField.Error.Should().BeFalse();
            parentTextField.ErrorText.Should().BeNullOrEmpty();
            // display the child form
            childFormSwitch.Change(true);
            var forms = comp.FindComponents<MudForm>();
            forms.Count.Should().Be(2);
            var childForm = forms[1];
            childForm.Instance.IsValid.Should().BeFalse();
            parentForm.IsValid.Should().Be(false);

            // remove the child form
            childFormSwitch.Change(false);
            forms = comp.FindComponents<MudForm>();
            forms.Count.Should().Be(1);
            parentForm.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task FormComponent_Should_UpdateValidationMessagesOnEditContextChanged()
        {
            var comp = Context.RenderComponent<FormComponentUpdateValidationMessagesOnEditContextChangedTest>();
            var validator = comp.FindComponent<FormComponentUpdateValidationMessagesValidator>();
            var errorMessage = "some error";

            await validator.InvokeAsync(() => validator.Instance.AddError(nameof(FormComponentUpdateValidationMessagesModel.Name), errorMessage));

            var tf = comp.FindComponent<MudTextField<string>>();
            tf.Instance.ValidationErrors.Should().HaveCount(1).And.Contain(new[] { errorMessage });

            await validator.InvokeAsync(() => validator.Instance.ClearErrors());

            tf.Instance.ValidationErrors.Should().HaveCount(0);
        }

        /// <summary>
        /// CheckBox should be validated like every other form component when ticked using mouse
        /// </summary>
        [Test]
        public async Task FormWithCheckBoxTest_When_CheckBoxTickedUsingMouse()
        {
            var comp = Context.RenderComponent<FormWithCheckBoxTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var checkBox = comp.FindComponent<MudCheckBox<bool>>().Instance;

            // check initial state: form should not be valid because checkBox is required
            form.IsValid.Should().Be(false);
            checkBox.Error.Should().BeFalse();
            checkBox.ErrorText.Should().BeNullOrEmpty();

            // tick checkBox with an emulated mouse click
            comp.Find("input").Change(true);
            form.IsTouched.Should().Be(true);
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            checkBox.Error.Should().BeFalse();
            checkBox.ErrorText.Should().BeNullOrEmpty();

            // untick checkBox with an emulated mouse click
            comp.Find("input").Change(false);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            checkBox.Error.Should().BeTrue();
            checkBox.ErrorText.Should().Be("Required");
        }

        /// <summary>
        /// CheckBox should be validated like every other form component when ticked using keyboard
        /// </summary>
        [Test]
        public async Task FormWithCheckBoxTest_When_CheckBoxTickedUsingKeyboard()
        {
            var comp = Context.RenderComponent<FormWithCheckBoxTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var checkBox = comp.FindComponent<MudCheckBox<bool>>().Instance;

            // check initial state: form should not be valid because checkBox is required
            form.IsValid.Should().Be(false);
            checkBox.Error.Should().BeFalse();
            checkBox.ErrorText.Should().BeNullOrEmpty();

            // tick checkBox with a key press
            comp.Find("input").KeyDown(Key.Space);
            form.IsTouched.Should().Be(true);
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            checkBox.Error.Should().BeFalse();
            checkBox.ErrorText.Should().BeNullOrEmpty();

            // untick checkBox with a key press
            comp.Find("input").KeyDown(Key.Space);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            checkBox.Error.Should().BeTrue();
            checkBox.ErrorText.Should().Be("Required");
        }

        [Test]
        public async Task FormSpacingClass()
        {
            var comp = Context.RenderComponent<MudForm>();

            for (var i = 0; i <= 20; i++)
            {
                comp.SetParam(x => x.Spacing, i);
                comp.Find("form.mud-form").ClassList.Should().Contain($"gap-{i}");
            }
        }
    }
}
