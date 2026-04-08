using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.Dummy;
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
        public async Task FormIsValid()
        {
            var comp = Context.Render<FormIsValidTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            var textField = textFieldcomp.Instance;
            // check initial state: form should not be valid, but text field does not display an error initially!
            form.IsValid.Should().Be(false);
            textField.GetState(x => x.Error).Should().BeFalse();
            textField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            await textFieldcomp.Find("input").ChangeAsync("Marilyn Manson");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            textField.GetState(x => x.Error).Should().BeFalse();
            textField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // clear value to null
            await textFieldcomp.Find("input").ChangeAsync(null);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Enter a rock star");
            textField.GetState(x => x.Error).Should().BeTrue();
            textField.GetState(x => x.ErrorText).Should().Be("Enter a rock star");
            // set value to "" -> should also be an error
            await textFieldcomp.Find("input").ChangeAsync("");
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Enter a rock star");
            textField.GetState(x => x.Error).Should().BeTrue();
            textField.GetState(x => x.ErrorText).Should().Be("Enter a rock star");
            //
            await textFieldcomp.Find("input").ChangeAsync("Kurt Cobain");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            textField.GetState(x => x.Error).Should().BeFalse();
            textField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Form's isvalid should be true, no matter whether or not the field was touched
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task FormIsValidTest2()
        {
            var comp = Context.Render<FormIsValidTest2>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            // check initial state: form should be valid due to field not being required!
            form.IsValid.Should().Be(true);
            await textFieldcomp.Find("input").ChangeAsync("This value doesn't matter");
            form.IsValid.Should().Be(true);
        }

        /// <summary>
        /// Form should update the bound variables valid and touched whenever they change.
        /// </summary>
        [Test]
        public async Task FormIsValidTest3()
        {
            var comp = Context.Render<FormIsValidTest3>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textFields = comp.FindComponents<MudTextField<string>>();
            // check initial state: form should be invalid due to having a required field that is not filled
            form.IsValid.Should().Be(false);
            form.IsTouched.Should().Be(false);
            comp.FindComponents<MudSwitch<bool>>()[0].Instance.ReadValue.Should().Be(false);
            comp.FindComponents<MudSwitch<bool>>()[1].Instance.ReadValue.Should().Be(false);
            // filling in the required field
            await textFields[1].Find("input").ChangeAsync("Fill in the required field to make this form valid");
            form.IsValid.Should().Be(true);
            comp.FindComponents<MudSwitch<bool>>()[0].Instance.ReadValue.Should().Be(true);
            comp.FindComponents<MudSwitch<bool>>()[1].Instance.ReadValue.Should().Be(true);
        }

        /// <summary>
        /// Form should update the bound variable valid to true even though it is set false upon first render because there is no required field.
        /// </summary>
        [Test]
        public async Task FormIsValidTest4()
        {
            var comp = Context.Render<FormIsValidTest4>();
            var form = comp.FindComponent<MudForm>().Instance;
            // check initial state: form should be valid due to having no required field, but the user's two-way binding did override that value to false
            await comp.WaitForAssertionAsync(() => form.IsValid.Should().Be(true));
            await comp.WaitForAssertionAsync(() => comp.FindComponent<MudSwitch<bool>>().Instance.ReadValue.Should().Be(true));
        }

        /// <summary>
        /// Changing a fields value should set IsTouched to true
        /// </summary>
        [Test]
        public async Task FormIsTouched()
        {
            var comp = Context.Render<FormIsTouchedTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            var dateComp = comp.FindComponent<MudDatePicker>();
            // check initial state: form should not be touched
            form.IsTouched.Should().Be(false);
            // input a date, istouched should be true
            await dateComp.Find("input").ChangeAsync("2001-01-31");
            form.IsTouched.Should().Be(true);

            //reset should set touched to false
            await comp.InvokeAsync(() => form.ResetAsync());
            form.IsTouched.Should().Be(false);

            // clear value to null
            await textFieldcomp.Find("input").ChangeAsync("value is changed");
            form.IsTouched.Should().Be(true);

            //reset validation should not reset touched state
            await comp.InvokeAsync(() => form.ResetValidationAsync());
            form.IsTouched.Should().Be(true);
        }

        /// <summary>
        /// Changing the nested form fields value should set IsTouched
        /// </summary>
        [Test]
        public async Task FormIsTouchedAndNestedFormIsNotTouchedWhenParentFormFieldIsTouched()
        {
            var comp = Context.Render<FormIsTouchedNestedTest>();
            var formsComp = comp.FindComponents<MudForm>();
            var textCompFields = comp.FindComponents<MudTextField<string>>();
            var form = formsComp[0].Instance;
            var nestedForm = formsComp[1].Instance;

            // check initial state: form should not be touched
            form.IsTouched.Should().Be(false);
            nestedForm.IsTouched.Should().Be(false);
            // input a date, istouched should be true
            await textCompFields[0].Find("input").ChangeAsync("2001-01-31");
            form.IsTouched.Should().Be(true);
            nestedForm.IsTouched.Should().Be(false);

            //reset should set touched to false
            await comp.InvokeAsync(() => form.ResetAsync());
            form.IsTouched.Should().Be(false);
            nestedForm.IsTouched.Should().Be(false);

            // clear value to null
            await textCompFields[0].Find("input").ChangeAsync("value is changed");
            form.IsTouched.Should().Be(true);
            nestedForm.IsTouched.Should().Be(false);

            //reset validation should not reset touched state
            await comp.InvokeAsync(() => form.ResetValidationAsync());
            form.IsTouched.Should().Be(true);
            nestedForm.IsTouched.Should().Be(false);
        }

        /// <summary>
        /// Changing the nested form fields value should set IsTouched to true on parent form
        /// </summary>
        [Test]
        public async Task FormIsUnTouchedWhenNestedFormTouched()
        {
            var comp = Context.Render<FormIsTouchedNestedTest>();
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
            await textCompFields[1].Find("input").ChangeAsync("2001-01-31");
            form.IsTouched.Should().Be(true);
            nestedForm.IsTouched.Should().Be(true);

            //reset should set touched to false
            await comp.InvokeAsync(() => form.ResetAsync());
            form.IsTouched.Should().Be(false);
            nestedForm.IsTouched.Should().Be(false);

            // clear value to null
            await textCompFields[3].Find("input").ChangeAsync("value is changed");
            form.IsTouched.Should().Be(false);
            nestedForm.IsTouched.Should().Be(true);

            //reset validation should not reset touched state
            await comp.InvokeAsync(() => nestedFormDateField.ResetValidationAsync());
            form.IsTouched.Should().Be(false);
            nestedForm.IsTouched.Should().Be(true);
        }

        /// <summary>
        /// Calling ResetTouched should set the IsTouched property to false
        /// </summary>
        [Test]
        public async Task FormIsTouchedReset()
        {
            var comp = Context.Render<FormIsTouchedTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dateComp = comp.FindComponent<MudDatePicker>();
            // check initial state: form should not be touched
            form.IsTouched.Should().Be(false);
            // input a date, isTouched should be true
            await dateComp.Find("input").ChangeAsync("2001-01-31");
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
            var comp = Context.Render<FormValidationTest>(parameters => parameters.Add(p => p.Validation, validationFunc));
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            var textField = textFieldcomp.Instance;
            // check initial state: form should not be valid, but text field does not display an error initially!
            form.IsValid.Should().Be(false);
            textField.GetState(x => x.Error).Should().BeFalse();
            textField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // this rock star starts with Marilyn
            await textFieldcomp.Find("input").ChangeAsync("Marilyn Manson");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            textField.GetState(x => x.Error).Should().BeFalse();
            textField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // this rock star doesn't start with Marilyn
            await textFieldcomp.Find("input").ChangeAsync("Kurt Cobain");
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            textField.GetState(x => x.Error).Should().BeTrue();
            textField.GetState(x => x.ErrorText).Should().Be("Invalid");

            // note: this logic is invalid, so it was removed. Validation funcs are always called
            // the validation func must validate non-required empty fields as valid.
            //
            //// value is not required, so don't call the validation func on empty text
            //await comp.InvokeAsync(() => textField.ReadValue = "");
            //form.IsValid.Should().Be(true);
            //form.Errors.Length.Should().Be(0);
            //textField.Error.Should().BeFalse();
            //textField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            // ok, not a rock star, but a star nonetheless
            await textFieldcomp.Find("input").ChangeAsync("Marilyn Monroe");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            textField.GetState(x => x.Error).Should().BeFalse();
            textField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
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
                {
                    return "Not a star!";
                }

                return null;
            });
            var comp = Context.Render<FormValidationTest>(parameters => parameters.Add(p => p.Validation, validationFunc));
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            form.IsValid.Should().Be(false);
            await textFieldcomp.Find("input").ChangeAsync("Marilyn Manson");
            form.IsValid.Should().Be(true);
            // this one might not be a star, but our custom validation func deems him valid nonetheless
            await textFieldcomp.Find("input").ChangeAsync("Charles Manson");
            form.IsValid.Should().Be(true);

            // note: this logic is invalid, so it was removed. Validation funcs are always called
            // the validation func must validate non-required empty fields as valid.
            //
            //// value is not required, so don't call the validation func on empty text
            //await comp.InvokeAsync(() => textField.ReadValue = "");
            //form.IsValid.Should().Be(true);

            // clearly a star
            await textFieldcomp.Find("input").ChangeAsync("Marilyn Monroe");
            form.IsValid.Should().Be(true);
            // not a star according to our validation func
            await textFieldcomp.Find("input").ChangeAsync("Manson Marilyn");
            form.IsValid.Should().Be(false);
        }

        /// <summary>
        /// Reset() should reset the input components of the form
        /// </summary>
        [Test]
        public async Task FormValidationTest3()
        {
            var comp = Context.Render<FormValidationTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldcomp = comp.FindComponent<MudTextField<string>>();
            var textField = textFieldcomp.Instance;
            form.IsValid.Should().Be(false);
            await textFieldcomp.Find("input").ChangeAsync("Some value");
            form.IsValid.Should().Be(true);
            // calling Reset() should reset the textField's value
            await comp.InvokeAsync(() => form.ResetAsync());
            textField.ReadValue.Should().Be(null);
            textField.ReadText.Should().Be(null);
            form.IsValid.Should().Be(false); // because we did reset validation state as a side-effect.
        }

        /// <summary>
        /// Validate that first async validation call returning after second call will not override result of second call
        /// </summary>
        [Test]
        public async Task FormAsyncValidation()
        {
            const int ValidDelay = 100;
            const int InvalidDelay = 200;
            var validationFunc = new Func<string, Task<string>>(async s =>
            {
                if (s == null)
                {
                    return null;
                }

                var valid = s == "abc";
                await Task.Delay(valid ? ValidDelay : InvalidDelay);
                return valid ? null : "invalid";
            });
            var comp = Context.Render<FormValidationTest>(parameters => parameters.Add(p => p.Validation, validationFunc));
            var textFieldComp = comp.FindComponent<MudTextField<string>>();
            var textField = textFieldComp.Instance;

            IElement TextFieldInput() => textFieldComp.Find("input");
            // validate initial field state
            textField.ValidationErrors.Should().BeEmpty();
            // make sure error can be detected
            await TextFieldInput().ChangeAsync("def");
            await comp.WaitForAssertionAsync(() => textField.ValidationErrors.Should().ContainSingle("invalid"), TimeSpan.FromSeconds(5));
            // make sure success can be detected
            await TextFieldInput().ChangeAsync("abc");
            await comp.WaitForAssertionAsync(() => textField.ValidationErrors.Should().BeEmpty(), TimeSpan.FromSeconds(5));
            // send invalid value, then valid value
            await TextFieldInput().ChangeAsync("def");
            await TextFieldInput().ChangeAsync("abc");
            // validate that first call result (invalid, longer return time) will not overwrite second call result (valid, shorter return time)
            await comp.WaitForAssertionAsync(() => textField.ValidationErrors.Should().BeEmpty(), TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Validate that text typed during async validation of a component won't swallow user input on re-render.
        /// </summary>
        [Test]
        public async Task FormAsyncValidationWithFieldChangedSubscriber()
        {
            var comp = Context.Render<FormAsyncValidationWithFieldChangedSubscriberTest>();
            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            var input = () => comp.Find("input");
            await input().InputAsync("test");
            // trigger validation
            await Task.Delay(comp.Instance.DebounceInterval);
            // imitate "typing in progress" by extending the debounce interval until the async validation terminates
            var elapsedTime = 0;
            var currentText = "test";
            while (elapsedTime < comp.Instance.AsyncTaskDelay)
            {
                var delay = comp.Instance.DebounceInterval / 2;
                currentText += "a";
                await input().InputAsync(currentText);
                await Task.Delay(delay);
                elapsedTime += delay;
            }
            // after the final debounce, the value should be updated without swallowing any user input
            await comp.WaitForAssertionAsync(() =>
            {
                textField.ReadValue.Should().Be(currentText);
                textField.ReadText.Should().Be(currentText);
            });
        }

        /// <summary>
        /// #12790: After changing any of the textfields with a For expression the corresponding chip should show a change message after the textfield blurred.
        /// </summary>
        [Test]
        public async Task EditFormOnFieldChanged_BlurWithoutValueChange_ShouldNotNotify()
        {
            var comp = Context.Render<EditFormOnFieldChangedTest>();
            var chips = comp.FindAll("span.mud-chip-content");
            chips.Count.Should().Be(3);

            await comp.FindAll("input")[0].BlurAsync();

            chips = comp.FindAll("span.mud-chip-content");
            chips[0].TextContent.Trim().Should().EndWith("not changed");
            chips[1].TextContent.Trim().Should().EndWith("not changed");
            chips[2].TextContent.Trim().Should().EndWith("not changed");
        }

        /// <summary>
        /// After changing any of the textfields with a For expression the corresponding chip should show a change message after the textfield blurred.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task EditFormOnFieldChanged()
        {
            var comp = Context.Render<EditFormOnFieldChangedTest>();
            var textFields = comp.FindAll("input");
            textFields.Count.Should().Be(3);
            var chips = comp.FindAll("span.mud-chip-content");
            chips.Count.Should().Be(3);
            foreach (var chip in chips)
            {
                chip.TextContent.Trim().Should().EndWith("not changed");
            }

            await comp.FindAll("input")[0].ChangeAsync(new ChangeEventArgs() { Value = "asdf" });
            await comp.FindAll("input")[0].BlurAsync();
            comp.FindComponents<MudTextField<string>>()[0].Instance.ReadText.Should().Be("asdf");
            comp.FindAll("span.mud-chip-content")[0].TextContent.Trim().Should().Be("Field1 changed");
            comp.FindAll("span.mud-chip-content")[1].TextContent.Trim().Should().EndWith("not changed");
            comp.FindAll("span.mud-chip-content")[2].TextContent.Trim().Should().EndWith("not changed");
            await comp.FindAll("input")[1].ChangeAsync(new ChangeEventArgs() { Value = "yxcv" });
            await comp.FindAll("input")[1].BlurAsync();
            comp.FindComponents<MudTextField<string>>()[1].Instance.ReadText.Should().Be("yxcv");
            comp.FindAll("span.mud-chip-content")[0].TextContent.Trim().Should().Be("Field1 changed");
            comp.FindAll("span.mud-chip-content")[1].TextContent.Trim().Should().EndWith("not changed", "Because it has no For, so the change can not be forwarded to the edit context for lack of a FieldIdentifier");
            comp.FindAll("span.mud-chip-content")[2].TextContent.Trim().Should().EndWith("not changed");
            await comp.FindAll("input")[2].ChangeAsync(new ChangeEventArgs() { Value = "qwer" });
            await comp.FindAll("input")[2].BlurAsync();
            comp.FindComponents<MudTextField<string>>()[2].Instance.ReadText.Should().Be("qwer");
            comp.FindAll("span.mud-chip-content")[0].TextContent.Trim().Should().Be("Field1 changed");
            comp.FindAll("span.mud-chip-content")[1].TextContent.Trim().Should().EndWith("not changed");
            comp.FindAll("span.mud-chip-content")[2].TextContent.Trim().Should().EndWith("Field3 changed");
        }

        /// <summary>
        /// Based on error report. Clicking the checkbox should not influence the other form fields.
        /// </summary>
        [Test]
        public async Task FormWithCheckbox()
        {
            var comp = Context.Render<FormWithCheckBoxAndTextFieldsTest>();
            var textFields = comp.FindAll("input");
            textFields.Count.Should().Be(4); // three textfields, one checkbox
            // let's fill in some values
            await comp.FindAll("input")[0].ChangeAsync("Garfield");
            await comp.FindAll("input")[0].BlurAsync();
            await comp.FindAll("input")[1].ChangeAsync("Jon");
            await comp.FindAll("input")[1].BlurAsync();
            await comp.FindAll("input")[2].ChangeAsync("17"); // kg ;)
            await comp.FindAll("input")[2].BlurAsync();
            foreach (var tf in comp.FindComponents<MudTextField<string>>())
            {
                tf.Instance.ReadText.Should().NotBeNullOrEmpty();
            }

            comp.FindComponent<MudTextField<int>>().Instance.ReadValue.Should().Be(17);
            // then click the checkbox
            comp.FindComponent<MudCheckBox<bool>>().Instance.ReadValue.Should().Be(true);
            await comp.FindAll("input")[3].ChangeAsync(false); // it was on before
            comp.FindComponent<MudCheckBox<bool>>().Instance.ReadValue.Should().Be(false);
            // the text fields should be unchanged
            foreach (var tf in comp.FindComponents<MudTextField<string>>())
            {
                tf.Instance.ReadText.Should().NotBeNullOrEmpty();
            }

            comp.FindComponent<MudTextField<int>>().Instance.ReadValue.Should().Be(17);
        }

        /// <summary>
        /// Based on error report. Even without clicking the checkbox the form should
        /// be valid if the checkbox is not required.
        /// </summary>
        [Test]
        public void FormWithCheckboxTest2()
        {
            var comp = Context.Render<FormWithCheckBoxAndTextFieldsTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            form.IsValid.Should().BeTrue(because: "none of the fields are required");
        }

        /// <summary>
        /// Form should become valid as soon as all required fields are filled in correctly.
        /// </summary>
        [Test]
        public async Task Form_Should_BecomeValidIfUntouchedFieldsAreNotRequired()
        {
            var comp = Context.Render<FormValidationTest2>();
            var form = comp.FindComponent<MudForm>().Instance;
            form.IsValid.Should().BeFalse(because: "textfield is required");
            var textfield = comp.FindComponent<MudTextField<string>>();
            await textfield.Find("input").ChangeAsync("Moby Dick");
            form.IsValid.Should().BeTrue(because: "select is not required");
        }

        /// <summary>
        /// Form should become invalid as soon as an in-convertible value is entered.
        /// </summary>
        [Test]
        public async Task Form_Should_BecomeInValidWhenAConversionErrorOccurs()
        {
            var comp = Context.Render<FormConversionErrorTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            form.IsValid.Should().BeTrue();
            var textfield = comp.FindComponent<MudTextField<int>>();
            await textfield.Find("input").InputAsync("Not and int");
            form.IsValid.Should().BeFalse(because: "conversion error is forwarded to form");
            await textfield.Find("input").InputAsync("17");
            form.IsValid.Should().BeTrue(because: "conversion error is gone");
        }

        /// <summary>
        /// Testing the functionality of the MudForm example from the docs.
        /// </summary>
        [Test]
        public async Task MudFormExample()
        {
            var comp = Context.Render<FormValidationTest4>();
            var form = comp.FindComponent<MudForm>().Instance;
            await comp.FindComponent<MudForm>().SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ValidationDelay, 0));
            await comp.WaitForAssertionAsync(() => form.IsValid.Should().BeFalse(because: "it contains required fields that are not filled out"));
            var buttons = comp.FindComponents<MudButton>();
            // click validate button
            var validateButton = buttons[1];
            await validateButton.Find("button").ClickAsync();
            var textfields = comp.FindComponents<MudTextField<string>>();
            await comp.WaitForAssertionAsync(() => textfields[0].Instance.HasErrors.Should().BeTrue());
            textfields[0].Instance.GetState(x => x.ErrorText).Should().Be("User name is required!");
            await comp.WaitForAssertionAsync(() => textfields[1].Instance.HasErrors.Should().BeTrue());
            textfields[1].Instance.GetState(x => x.ErrorText).Should().Be("Email is required!");
            await comp.WaitForAssertionAsync(() => textfields[2].Instance.HasErrors.Should().BeTrue());
            textfields[2].Instance.GetState(x => x.ErrorText).Should().Be("Password is required!");
            var checkbox = comp.FindComponent<MudCheckBox<bool>>();
            await comp.WaitForAssertionAsync(() => checkbox.Instance.HasErrors.Should().BeTrue());
            checkbox.Instance.GetState(x => x.ErrorText).Should().Be("You must agree");
            // click reset validation
            var resetValidationButton = buttons[3];
            await resetValidationButton.Find("button").ClickAsync();
            await comp.WaitForStateAsync(() => form.Errors.Length == 0);
            await comp.WaitForAssertionAsync(() => textfields[0].Instance.HasErrors.Should().BeFalse());
            textfields[0].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() => textfields[1].Instance.HasErrors.Should().BeFalse());
            textfields[1].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() => textfields[2].Instance.HasErrors.Should().BeFalse());
            textfields[2].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() => checkbox.Instance.HasErrors.Should().BeFalse());
            checkbox.Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // fill in the form to make it valid
            await textfields[0].Find("input").ChangeAsync("Rick Sanchez");
            await textfields[1].Find("input").ChangeAsync("rick.sanchez@citadel-of-ricks.com");
            await textfields[2].Find("input").ChangeAsync("Wabalabadubdub1234!");
            await textfields[3].Find("input").ChangeAsync("Wabalabadubdub1234!");
            await checkbox.Find("input").ChangeAsync(true);
            await comp.WaitForAssertionAsync(() => form.IsValid.Should().BeTrue());
            await comp.WaitForStateAsync(() => form.Errors.Length == 0);
            // click reset
            var resetButton = buttons[2];
            await resetButton.Find("button").ClickAsync();
            await comp.WaitForStateAsync(() => form.Errors.Length == 0);
            await comp.WaitForAssertionAsync(() => textfields[0].Instance.HasErrors.Should().BeFalse());
            textfields[0].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[0].Instance.ReadText.Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() => textfields[1].Instance.HasErrors.Should().BeFalse());
            textfields[1].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[1].Instance.ReadText.Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() => textfields[2].Instance.HasErrors.Should().BeFalse());
            textfields[2].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[2].Instance.ReadText.Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() => checkbox.Instance.HasErrors.Should().BeFalse());
            checkbox.Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            await comp.WaitForAssertionAsync(() => checkbox.Instance.ReadValue.Should().BeFalse());
            // TODO: fill out the form with errors, field after field, check how fields get validation errors after blur
        }

        /// <summary>
        /// Setting the required radiogroup value should set IsValid true
        /// Clearing the value of a required radiogroup should set form's IsValid to false.
        /// </summary>
        [Test]
        public async Task FormWithRadioGroupIsValid()
        {
            var comp = Context.Render<FormWithRadioGroupTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var radioGroupcomp = comp.FindComponent<MudRadioGroup<string>>();
            var radioGroup = radioGroupcomp.Instance;
            // check initial state: form should not be valid
            form.IsValid.Should().Be(false);
            radioGroup.GetState(x => x.Error).Should().BeFalse();
            radioGroup.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // click on first radio: form should be valid now
            await radioGroupcomp.Find("input").ClickAsync();
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            radioGroup.GetState(x => x.Error).Should().BeFalse();
            radioGroup.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // clear selection
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Selected, null));
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            radioGroup.GetState(x => x.Error).Should().BeTrue();
            radioGroup.GetState(x => x.ErrorText).Should().Be("Required");
        }

        /// <summary>
        /// ColorPicker should be validated like every other form component
        /// </summary>
        [Test]
        public async Task FormWithColorPicker()
        {
            var comp = Context.Render<FormWithColorPickerTest>(parameters => parameters.Add(x => x.ColorValue, null));
            var form = comp.FindComponent<MudForm>().Instance;
            var colorPickerComp = comp.FindComponent<MudColorPicker>();
            var colorPicker = comp.FindComponent<MudColorPicker>().Instance;
            // check initial state: form should not be valid because colorpicker is required
            form.IsTouched.Should().BeFalse();
            form.IsValid.Should().BeFalse();
            colorPicker.GetState(x => x.Error).Should().BeFalse();
            colorPicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // input a valid color
            await colorPickerComp.FindAll("input")[0].ChangeAsync("#111111");
            form.IsTouched.Should().BeTrue();
            form.IsValid.Should().BeTrue();
            form.Errors.Length.Should().Be(0);
            colorPicker.GetState(x => x.Error).Should().BeFalse();
            colorPicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // clear selection
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ColorValue, null));
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            colorPicker.GetState(x => x.Error).Should().BeTrue();
            colorPicker.GetState(x => x.ErrorText).Should().Be("Required");
        }

        /// <summary>
        /// ColorPicker should be validated like every other form component when text input is cleared
        /// </summary>
        [Test]
        public async Task Form_Should_Validate_ColorPicker_When_EditableInputCleared()
        {
            var comp = Context.Render<FormWithColorPickerTest>(parameters => parameters.Add(x => x.ColorValue, null));
            var form = comp.FindComponent<MudForm>().Instance;
            var colorPickerComp = comp.FindComponent<MudColorPicker>();
            var colorPicker = comp.FindComponent<MudColorPicker>().Instance;
            await colorPickerComp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Editable, true));
            // check initial state: form should not be valid because colorpicker is required
            form.IsTouched.Should().BeFalse();
            form.IsValid.Should().BeFalse();
            colorPicker.GetState(x => x.Error).Should().BeFalse();
            colorPicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // input a valid color
            await colorPickerComp.FindAll("input")[0].ChangeAsync("#111111");
            form.IsTouched.Should().BeTrue();
            form.IsValid.Should().BeTrue();
            form.Errors.Length.Should().Be(0);
            colorPicker.GetState(x => x.Error).Should().BeFalse();
            colorPicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // clear selection
            await colorPickerComp.FindAll("input")[0].ChangeAsync(null);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            colorPicker.GetState(x => x.Error).Should().BeTrue();
            colorPicker.GetState(x => x.ErrorText).Should().Be("Required");
        }

        /// <summary>
        /// ColorPicker should be validated like every other form component when color is changed via inputs
        /// </summary>
        [Test]
        public async Task Form_Should_Validate_ColorPicker_When_ColorSelectedViaInputs()
        {
            var comp = Context.Render<FormWithColorPickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var colorPickerComp = comp.FindComponent<MudColorPicker>();
            var colorPicker = comp.FindComponent<MudColorPicker>().Instance;
            var forbiddenColor = colorPicker.Value;
            await colorPickerComp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, new Func<MudColor, string>(color => color != null && color.Value == forbiddenColor.Value ? $"{forbiddenColor.Value} is not allowed" : null)));
            // should not be valid since the default color is invalid
            form.IsTouched.Should().BeFalse();
            form.IsValid.Should().BeFalse();
            colorPicker.GetState(x => x.Error).Should().BeFalse();
            colorPicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // input a valid color
            await colorPickerComp.FindAll("input")[0].ChangeAsync("#111111");
            form.IsTouched.Should().BeTrue();
            form.IsValid.Should().BeTrue();
            form.Errors.Length.Should().Be(0);
            colorPicker.GetState(x => x.Error).Should().BeFalse();
            colorPicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // reset to forbidden color
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ColorValue, forbiddenColor));
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be($"{forbiddenColor.Value} is not allowed");
            colorPicker.GetState(x => x.Error).Should().BeTrue();
            colorPicker.GetState(x => x.ErrorText).Should().Be($"{forbiddenColor.Value} is not allowed");
        }

        /// <summary>
        /// ColorPicker should be validated like every other form component when color is selected using picker
        /// </summary>
        [Test]
        public async Task Form_Should_ValidateColorPickerTest_When_ColorSelectedViaPicker()
        {
            var comp = Context.Render<FormWithColorPickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var colorPickerComp = comp.FindComponent<MudColorPicker>();
            var colorPicker = comp.FindComponent<MudColorPicker>().Instance;
            var forbiddenColor = colorPicker.Palette.First();
            await colorPickerComp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, new Func<MudColor, string>(color => color != null && color.Value == forbiddenColor.Value ? $"{forbiddenColor.Value} is not allowed" : null)));
            // initial form state
            form.IsTouched.Should().BeFalse();
            form.IsValid.Should().BeFalse();

            await comp.InvokeAsync(() => comp.Find("input").Click());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
            // open color collection view
            await comp.InvokeAsync(() => comp.Find("div.mud-picker-color-dot-current").Click());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-color-collection").Count.Should().Be(1));

            // set valid color
            await comp.InvokeAsync(() => comp.FindAll("div.mud-picker-color-collection>div.mud-picker-color-dot").Skip(1).First().Click());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-color-collection").Count.Should().Be(0));
            form.IsTouched.Should().BeTrue();
            form.IsValid.Should().BeTrue();
            form.Errors.Length.Should().Be(0);
            colorPicker.GetState(x => x.Error).Should().BeFalse();
            colorPicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            await comp.InvokeAsync(() => comp.Find("div.mud-picker-color-dot-current").Click());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-color-collection").Count.Should().Be(1));

            // set invalid color
            await comp.InvokeAsync(() => comp.FindAll("div.mud-picker-color-collection>div.mud-picker-color-dot").First().Click());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-color-collection").Count.Should().Be(0));
            form.IsTouched.Should().BeTrue();
            form.IsValid.Should().BeFalse();
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be($"{forbiddenColor.Value} is not allowed");
            colorPicker.GetState(x => x.Error).Should().BeTrue();
            colorPicker.GetState(x => x.ErrorText).Should().Be($"{forbiddenColor.Value} is not allowed");
        }

        /// <summary>
        /// DatePicker should be validated like every other form component
        /// </summary>
        [Test]
        public async Task FormWithDatePicker()
        {
            var comp = Context.Render<FormWithDatePickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dateComp = comp.FindComponent<MudDatePicker>();
            var datepicker = comp.FindComponent<MudDatePicker>().Instance;
            // check initial state: form should not be valid because datepicker is required
            form.IsValid.Should().Be(false);
            datepicker.GetState(x => x.Error).Should().BeFalse();
            datepicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // input a date
            await dateComp.Find("input").ChangeAsync(new DateTime(2001, 01, 31).ToShortDateString());
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            datepicker.GetState(x => x.Error).Should().BeFalse();
            datepicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // clear selection
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Date, null));
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            datepicker.GetState(x => x.Error).Should().BeTrue();
            datepicker.GetState(x => x.ErrorText).Should().Be("Required");
        }

        /// <summary>
        /// DatePicker should be validated like every other form component
        /// </summary>
        [Test]
        public async Task Form_Should_ValidateDatePicker()
        {
            var comp = Context.Render<FormWithDatePickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dateComp = comp.FindComponent<MudDatePicker>();
            var datepicker = comp.FindComponent<MudDatePicker>().Instance;
            await dateComp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, new Func<DateTime?, string>(date => date != null && date.Value.Year >= 2000 ? null : "Year must be >= 2000")));
            await dateComp.Find("input").ChangeAsync(new DateTime(2001, 01, 31).ToShortDateString());
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            datepicker.GetState(x => x.Error).Should().BeFalse();
            datepicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // set invalid date:
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Date, (DateTime?)new DateTime(1999, 1, 1)));
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Year must be >= 2000");
            datepicker.GetState(x => x.Error).Should().BeTrue();
            datepicker.GetState(x => x.ErrorText).Should().Be("Year must be >= 2000");
        }

        /// <summary>
        /// DateRangePicker should be validated like every other form component when the dateRange
        /// is changed via inputs
        /// </summary>
        [Test]
        public async Task Form_Should_Validate_DateRangePicker_When_DateRangeSelectedViaInputs()
        {
            var comp = Context.Render<FormWithDateRangePickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dateRangeComp = comp.FindComponent<MudDateRangePicker>();
            var dateRangePicker = comp.FindComponent<MudDateRangePicker>().Instance;
            var firstDateTime = new DateTime(2023, 01, 20);
            var secondDateTime = new DateTime(2023, 02, 20);
            // check initial state: form should not be valid because dateRangePicker is required
            form.IsValid.Should().Be(false);
            dateRangePicker.GetState(x => x.Error).Should().BeFalse();
            dateRangePicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // input a date
            await dateRangeComp.FindAll("input")[0].ChangeAsync(firstDateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern));
            await dateRangeComp.FindAll("input")[1].ChangeAsync(secondDateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern));
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            dateRangePicker.GetState(x => x.Error).Should().BeFalse();
            dateRangePicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // clear selection
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.DateRange, null));
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            dateRangePicker.GetState(x => x.Error).Should().BeTrue();
            dateRangePicker.GetState(x => x.ErrorText).Should().Be("Required");
        }

        /// <summary>
        /// DateRangePicker should be validated like every other form component when the dateRange is selected using
        /// the picker
        /// </summary>
        [Test]
        public async Task Form_Should_Validate_DateRangePicker_When_DateRangeSelectedViaPicker()
        {
            var comp = Context.Render<FormWithDateRangePickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dateRangePicker = comp.FindComponent<MudDateRangePicker>().Instance;
            // check initial state: form should not be valid because dateRangePicker is required
            form.IsValid.Should().Be(false);
            dateRangePicker.GetState(x => x.Error).Should().BeFalse();
            dateRangePicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            await comp.Find("input").ClickAsync();
            // clicking day buttons to select a date range
            await comp.InvokeAsync(() => comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("10")).Click());
            await comp.InvokeAsync(() => comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("11")).Click());
            // wait for picker to close
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));

            form.IsTouched.Should().Be(true);
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            dateRangePicker.GetState(x => x.Error).Should().BeFalse();
            dateRangePicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // clear selection
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.DateRange, null));
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            dateRangePicker.GetState(x => x.Error).Should().BeTrue();
            dateRangePicker.GetState(x => x.ErrorText).Should().Be("Required");
        }

        /// <summary>
        /// TimePicker should be validated like every other form component
        /// </summary>
        [Test]
        public async Task FormWithTimePicker()
        {
            var comp = Context.Render<FormWithTimePickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var timePickerComp = comp.FindComponent<MudTimePicker>();
            var timePicker = comp.FindComponent<MudTimePicker>().Instance;
            // check initial state: form should not be valid because datepicker is required
            form.IsValid.Should().Be(false);
            timePicker.GetState(x => x.Error).Should().BeFalse();
            timePicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // input a date
            await timePickerComp.Find("input").ChangeAsync("09:30");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            timePicker.GetState(x => x.Error).Should().BeFalse();
            timePicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // clear selection
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Time, null));
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            timePicker.GetState(x => x.Error).Should().BeTrue();
            timePicker.GetState(x => x.ErrorText).Should().Be("Required");
        }

        /// <summary>
        /// TimePicker should be validated like every other form component
        /// </summary>
        [Test]
        public async Task Form_Should_ValidateTimePicker()
        {
            var comp = Context.Render<FormWithTimePickerTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var timeComp = comp.FindComponent<MudTimePicker>();
            var timePicker = comp.FindComponent<MudTimePicker>().Instance;
            await timeComp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, new Func<TimeSpan?, string>(time => time != null && time.Value.Minutes == 0 ? null : "Only full hours allowed")));
            await timeComp.Find("input").ChangeAsync("09:00");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            timePicker.GetState(x => x.Error).Should().BeFalse();
            timePicker.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // set invalid date:
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Time, (TimeSpan?)new TimeSpan(0, 17, 05, 00)));
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Only full hours allowed");
            timePicker.GetState(x => x.Error).Should().BeTrue();
            timePicker.GetState(x => x.ErrorText).Should().Be("Only full hours allowed");
        }

        /// <summary>
        /// FileUpload should be validated like every other form component when a file is added
        /// </summary>
        [Test]
        public async Task Form_Should_Validate_FileUpload_When_FileAdded()
        {
            var comp = Context.Render<FormWithFileUploadTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var fileUploadComp = comp.FindComponent<MudFileUpload<IBrowserFile>>();
            var fileUploadInstance = comp.FindComponent<MudFileUpload<IBrowserFile>>().Instance;
            var input = fileUploadComp.FindComponent<InputFile>();
            var fileName = "cat.jpg";
            var fileToUpload = InputFileContent.CreateFromText("I am a cat image, trust me.", fileName);

            // check initial state: form should not be valid because fileUploadInstance is required
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeFalse();
            fileUploadInstance.GetState(x => x.Error).Should().BeFalse();
            fileUploadInstance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

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
            fileUploadInstance.GetState(x => x.Error).Should().BeTrue();
            fileUploadInstance.GetState(x => x.ErrorText).Should().Be("Required");
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
            var comp = Context.Render<FormWithFileUploadTest>(parameters =>
                parameters.Add(x => x.File, defaultFile));
            var form = comp.FindComponent<MudForm>().Instance;
            var fileUploadComp = comp.FindComponent<MudFileUpload<IBrowserFile>>();
            var fileUploadInstance = comp.FindComponent<MudFileUpload<IBrowserFile>>().Instance;
            var input = fileUploadComp.FindComponent<InputFile>();

            // check initial state: form should not be valid because form is untouched
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeFalse();
            fileUploadInstance.GetState(x => x.Error).Should().BeFalse();
            fileUploadInstance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            // clear files
            await input.ClearFilesAsync();
            fileUploadInstance.Files.Should().BeNull();

            // form should now be invalid because a file is required
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            fileUploadInstance.GetState(x => x.Error).Should().BeTrue();
            fileUploadInstance.GetState(x => x.ErrorText).Should().Be("Required");

            // re-add a file, form should now be valid and touched
            input.UploadFiles(fileToUpload);
            form.IsValid.Should().BeTrue();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(0);
            fileUploadInstance.GetState(x => x.Error).Should().BeFalse();
            fileUploadInstance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
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
            var comp = Context.Render<FormWithFileUploadTest>(
                parameters => parameters.Add(x => x.File, defaultFile));
            var form = comp.FindComponent<MudForm>().Instance;
            var fileUploadComp = comp.FindComponent<MudFileUpload<IBrowserFile>>();
            var fileUploadInstance = comp.FindComponent<MudFileUpload<IBrowserFile>>().Instance;
            var input = fileUploadComp.FindComponent<InputFile>();

            // check initial state: form should not be valid because form is untouched
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeFalse();
            fileUploadInstance.Files.Should().NotBeNull();
            fileUploadInstance.GetState(x => x.Error).Should().BeFalse();
            fileUploadInstance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            // clear files
            await comp.InvokeAsync(async () => await fileUploadInstance.ClearAsync());
            fileUploadInstance.Files.Should().BeNull();

            // form should now be invalid because a file is required
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            fileUploadInstance.GetState(x => x.Error).Should().BeTrue();
            fileUploadInstance.GetState(x => x.ErrorText).Should().Be("Required");

            // re-add a file, form should now be valid and touched
            input.UploadFiles(fileToUpload);
            form.IsValid.Should().BeTrue();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(0);
            fileUploadInstance.GetState(x => x.Error).Should().BeFalse();
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
            var comp = Context.Render<FormWithFileUploadAndDragAndDropActivatorTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var fileUploadComp = comp.FindComponent<MudFileUpload<IBrowserFile>>();
            var fileUploadInstance = comp.FindComponent<MudFileUpload<IBrowserFile>>().Instance;
            var input = fileUploadComp.FindComponent<InputFile>();

            // check initial state: form should not be valid because form is untouched
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeFalse();
            fileUploadInstance.Files.Should().NotBeNull();
            fileUploadInstance.GetState(x => x.Error).Should().BeFalse();
            fileUploadInstance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            // clear files
            await comp.InvokeAsync(() => comp.Find("button#clear-button").Click());
            fileUploadInstance.Files.Should().BeNull();

            // form should now be invalid because a file is required
            form.IsValid.Should().BeFalse();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            fileUploadInstance.GetState(x => x.Error).Should().BeTrue();
            fileUploadInstance.GetState(x => x.ErrorText).Should().Be("Required");

            // re-add a file, form should now be valid and touched
            input.UploadFiles(fileToUpload);
            form.IsValid.Should().BeTrue();
            form.IsTouched.Should().BeTrue();
            form.Errors.Length.Should().Be(0);
            fileUploadInstance.GetState(x => x.Error).Should().BeFalse();
            fileUploadInstance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            fileUploadInstance.Files.Name.Should().Be(fileName);
        }

        /// <summary>
        /// Testing the functionality of the EditForm example from the docs.
        /// </summary>
        [Test]
        public async Task EditFormExample_EmptyValidation()
        {
            var comp = Context.Render<FormValidationTest3>();
            // same effect as clicking the validate button
            await comp.Find("form").SubmitAsync();
            var textfields = comp.FindComponents<MudTextField<string>>();
            textfields[0].Instance.HasErrors.Should().BeTrue();
            textfields[0].Markup.Should().Contain("The Username field is required.");
            textfields[0].Instance.GetState(x => x.ErrorText).Should().Be("The Username field is required.");
            textfields[1].Instance.HasErrors.Should().BeTrue();
            textfields[1].Markup.Should().Contain("The Email field is required.");
            textfields[1].Instance.GetState(x => x.ErrorText).Should().Be("The Email field is required.");
            textfields[2].Instance.HasErrors.Should().BeTrue();
            textfields[2].Markup.Should().Contain("The Password field is required.");
            textfields[2].Instance.GetState(x => x.ErrorText).Should().Be("The Password field is required.");
            textfields[3].Instance.HasErrors.Should().BeTrue();
            textfields[3].Markup.Should().Contain("The Password2 field is required.");
            textfields[3].Instance.GetState(x => x.ErrorText).Should().Be("The Password2 field is required.");
        }

        /// <summary>
        /// Testing the functionality of the EditForm example from the docs.
        /// </summary>
        [Test]
        public async Task EditFormExample_FillInValues()
        {
            var comp = Context.Render<FormValidationTest3>();
            await comp.FindAll("input")[0].ChangeAsync("Rick Sanchez");
            await comp.FindAll("input")[0].BlurAsync();
            await comp.FindAll("input")[1].ChangeAsync("rick.sanchez@citadel-of-ricks.com");
            await comp.FindAll("input")[1].BlurAsync();
            await comp.FindAll("input")[2].ChangeAsync("Wabalabadubdub1234!");
            await comp.FindAll("input")[2].BlurAsync();
            await comp.FindAll("input")[3].ChangeAsync("Wabalabadubdub1234!");
            await comp.FindAll("input")[3].BlurAsync();
            // same effect as clicking the validate button
            await comp.Find("form").SubmitAsync();
            var textfields = comp.FindComponents<MudTextField<string>>();
            textfields[0].Markup.Should().Contain("Name length can't be more than 8.");
            textfields[0].Instance.GetState(x => x.ErrorText).Should().Be("Name length can't be more than 8.");
            textfields[0].Instance.HasErrors.Should().BeTrue();
            textfields[1].Instance.HasErrors.Should().BeFalse();
            textfields[1].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[2].Instance.HasErrors.Should().BeFalse();
            textfields[2].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[3].Instance.HasErrors.Should().BeFalse();
            textfields[3].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Ensure validation attributes aren't incorrectly called with `null` context.
        /// </summary>
        /// <see cref="https://github.com/MudBlazor/MudBlazor/issues/1229"/>
        [Test]
        public async Task EditForm_Validation_NullContext()
        {
            var comp = Context.Render<EditFormIssue1229>();
            // Check first run attribute
            EditFormIssue1229.TestAttribute.ValidationContextOnCall.Should().BeEmpty();
            // Trigger change
            var input = comp.Find("input");
            await input.ChangeAsync("Test");
            await input.BlurAsync();
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
            var comp = Context.Render<FormValidationTest4>();
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
            var comp = Context.Render<FormValidationTest5>();
            await comp.FindAll("input")[0].InputAsync("Rick Sanchez");
            await comp.FindAll("input")[0].BlurAsync();
            await comp.FindAll("input")[1].InputAsync("rick.sanchez@citadel-of-ricks.com");
            await comp.FindAll("input")[1].BlurAsync();
            await comp.FindAll("input")[3].InputAsync("Wabalabadubdub1234!");
            await comp.FindAll("input")[3].BlurAsync();
            await comp.FindAll("input")[4].InputAsync("sdfsfsdf!");
            await comp.FindAll("input")[4].BlurAsync();
            await comp.FindAll("input")[5].InputAsync("adsadasad!");
            await comp.FindAll("input")[5].BlurAsync();

            var form = comp.FindComponent<MudForm>().Instance;
            await comp.InvokeAsync(() => form.ValidateAsync());
            form.IsValid.Should().BeFalse();

            var textfields = comp.FindComponents<MudTextField<string>>();
            var numericFields = comp.FindComponents<MudNumericField<decimal>>();

            textfields[0].Instance.HasErrors.Should().BeFalse();
            textfields[0].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[1].Instance.HasErrors.Should().BeFalse();
            textfields[1].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[2].Instance.HasErrors.Should().BeFalse();
            textfields[2].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[3].Instance.HasErrors.Should().BeFalse();
            textfields[3].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[4].Instance.HasErrors.Should().BeFalse();
            textfields[4].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[5].Instance.HasErrors.Should().BeFalse();
            textfields[5].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            //Nested Forms
            textfields[6].Instance.HasErrors.Should().BeFalse();
            textfields[6].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            numericFields[0].Instance.HasErrors.Should().BeFalse();
            numericFields[0].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            textfields[7].Instance.HasErrors.Should().BeTrue();
            textfields[7].Instance.GetState(x => x.ErrorText).Should().NotBeNullOrEmpty();
            numericFields[1].Instance.HasErrors.Should().BeTrue();
            numericFields[1].Instance.GetState(x => x.ErrorText).Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Testing the functionality of the MudForm example from the docs.
        /// Root MudForm is invalid and nested MudForm is valid
        /// </summary>
        [Test]
        public async Task MudFormExample_FillInValuesNestedForm()
        {
            var comp = Context.Render<FormValidationTest5>();
            await comp.FindAll("input")[8].ChangeAsync("SomeWork");
            await comp.FindAll("input")[8].BlurAsync();
            await comp.FindAll("input")[9].ChangeAsync("99");
            await comp.FindAll("input")[9].BlurAsync();

            var form = comp.FindComponent<MudForm>().Instance;
            await comp.InvokeAsync(() => form.ValidateAsync());
            form.IsValid.Should().BeFalse();

            var textfields = comp.FindComponents<MudTextField<string>>();
            var numericFields = comp.FindComponents<MudNumericField<decimal>>();

            textfields[0].Instance.HasErrors.Should().BeTrue();
            textfields[0].Instance.GetState(x => x.ErrorText).Should().NotBeNullOrEmpty();
            textfields[1].Instance.HasErrors.Should().BeTrue();
            textfields[1].Instance.GetState(x => x.ErrorText).Should().NotBeNullOrEmpty();
            textfields[2].Instance.HasErrors.Should().BeFalse();
            textfields[2].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[3].Instance.HasErrors.Should().BeTrue();
            textfields[3].Instance.GetState(x => x.ErrorText).Should().NotBeNullOrEmpty();
            textfields[4].Instance.HasErrors.Should().BeTrue();
            textfields[4].Instance.GetState(x => x.ErrorText).Should().NotBeNullOrEmpty();
            textfields[5].Instance.HasErrors.Should().BeTrue();
            textfields[5].Instance.GetState(x => x.ErrorText).Should().NotBeNullOrEmpty();

            //Nested Forms
            textfields[6].Instance.HasErrors.Should().BeFalse();
            textfields[6].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            numericFields[0].Instance.HasErrors.Should().BeFalse();
            numericFields[0].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            textfields[7].Instance.HasErrors.Should().BeFalse();
            textfields[7].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            numericFields[1].Instance.HasErrors.Should().BeFalse();
            numericFields[1].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Testing the functionality of the MudForm example from the docs.
        /// Both root MudForm and nested MudForm are valid
        /// </summary>
        [Test]
        public async Task MudFormExample_FillInValues()
        {
            var comp = Context.Render<FormValidationTest5>();
            await comp.FindAll("input")[0].InputAsync("Rick Sanchez");
            await comp.FindAll("input")[0].BlurAsync();
            await comp.FindAll("input")[1].InputAsync("rick.sanchez@citadel-of-ricks.com");
            await comp.FindAll("input")[1].BlurAsync();
            await comp.FindAll("input")[3].InputAsync("Wabalabadubdub1234!");
            await comp.FindAll("input")[3].BlurAsync();
            await comp.FindAll("input")[4].InputAsync("sdfsfsdf!");
            await comp.FindAll("input")[4].BlurAsync();
            await comp.FindAll("input")[5].InputAsync("adsadasad!");
            await comp.FindAll("input")[5].BlurAsync();
            await comp.FindAll("input")[8].ChangeAsync("SomeWork");
            await comp.FindAll("input")[8].BlurAsync();
            await comp.FindAll("input")[9].ChangeAsync("99");
            await comp.FindAll("input")[9].BlurAsync();

            var form = comp.FindComponent<MudForm>().Instance;
            await comp.InvokeAsync(() => form.ValidateAsync());
            form.IsValid.Should().BeTrue();

            var textfields = comp.FindComponents<MudTextField<string>>();
            var numericFields = comp.FindComponents<MudNumericField<decimal>>();

            textfields[0].Instance.HasErrors.Should().BeFalse();
            textfields[0].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[1].Instance.HasErrors.Should().BeFalse();
            textfields[1].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[2].Instance.HasErrors.Should().BeFalse();
            textfields[2].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[3].Instance.HasErrors.Should().BeFalse();
            textfields[3].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[4].Instance.HasErrors.Should().BeFalse();
            textfields[4].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            textfields[5].Instance.HasErrors.Should().BeFalse();
            textfields[5].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            //Nested Forms
            textfields[6].Instance.HasErrors.Should().BeFalse();
            textfields[6].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            numericFields[0].Instance.HasErrors.Should().BeFalse();
            numericFields[0].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            textfields[7].Instance.HasErrors.Should().BeFalse();
            textfields[7].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            numericFields[1].Instance.HasErrors.Should().BeFalse();
            numericFields[1].Instance.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Testing error handling of MudFormComponent.ValidateModelWithFullPathOfMember
        /// Validation func throws an error, the error should contain the exception message
        /// </summary>
        [Test]
        public async Task MudFormComponent_ValidationWithModel_UnexpectedErrorInValidationFunc3()
        {
            var comp = Context.Render<FormWithSingleTextField>();
            var form = comp.FindComponent<MudForm>();
            var model = new { data = "asdf" };
            await form.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Model, model));
            var tf = comp.FindComponent<MudTextField<string>>();
            var validationFunc = new Func<object, string, IEnumerable<string>>((obj, property) =>
            {
                throw new InvalidOperationException("User error");
            });
            await tf.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, validationFunc));
            Expression<Func<string>> expression = () => model.data;
            await tf.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.For, expression));
            await comp.InvokeAsync(tf.Instance.ValidateAsync);
            tf.Instance.GetState(x => x.Error).Should().Be(true);
            tf.Instance.GetState(x => x.ErrorText).Should().Be("Error in validation func: User error");
        }

        /// <summary>
        /// Testing error handling of MudFormComponent.ValidateModelWithFullPathOfMember
        /// We have set no For expression, error should reflect that
        /// </summary>
        [Test]
        public async Task MudFormComponent_ValidationWithModelWithNoFor_ShouldShow_ExpectedError()
        {
            var comp = Context.Render<FormWithSingleTextField>();
            var form = comp.FindComponent<MudForm>();
            var model = new { data = "asdf" };
            await form.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Model, model));
            var tf = comp.FindComponent<MudTextField<string>>();
            var validationFunc = new Func<object, string, IEnumerable<string>>((obj, property) => throw new InvalidOperationException("User error"));
            await tf.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, validationFunc));
            await comp.InvokeAsync(tf.Instance.ValidateAsync);
            tf.Instance.GetState(x => x.Error).Should().Be(true);
            tf.Instance.GetState(x => x.ErrorText).Should().Be("For is null, please set parameter For on the form input component of type MudTextField`1");
        }

        /// <summary>
        /// Testing error handling of MudFormComponent.ValidateModelWithFullPathOfMember
        /// We have set no For expression, error should reflect that
        /// </summary>
        [Test]
        public async Task MudFormComponent_AsyncValidationWithModelWithNoFor_ShouldShow_ExpectedError()
        {
            var comp = Context.Render<FormWithSingleTextField>();
            var form = comp.FindComponent<MudForm>();
            var model = new { data = "asdf" };
            await form.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Model, model));
            var tf = comp.FindComponent<MudTextField<string>>();
            var validationFunc = new Func<object, string, Task<IEnumerable<string>>>((obj, property) =>
            {
                throw new InvalidOperationException("User error");
            });
            await tf.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, validationFunc));
            await comp.InvokeAsync(tf.Instance.ValidateAsync);
            tf.Instance.GetState(x => x.Error).Should().Be(true);
            tf.Instance.GetState(x => x.ErrorText).Should().Be("For is null, please set parameter For on the form input component of type MudTextField`1");
        }

        /// <summary>
        /// Testing validation with MudFormComponent.ValidateModelWithFullPathOfMember
        /// </summary>
        [Test]
        public async Task MudFormComponent_ValidationWithModel_UnexpectedErrorInValidationFunc5()
        {
            var comp = Context.Render<FormWithSingleTextField>();
            var form = comp.FindComponent<MudForm>();
            var model = new { data = "asdf" };
            await form.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Model, model));
            var tf = comp.FindComponent<MudTextField<string>>();
            var validationFunc = new Func<object, string, IEnumerable<string>>((obj, property) =>
            {
                obj.Should().Be(model);
                property.Should().Be("data");
                return new[] { "Error1", "Error2" };
            });
            await tf.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Validation, validationFunc));
            Expression<Func<string>> expression = () => model.data;
            await tf.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.For, expression));
            await comp.InvokeAsync(tf.Instance.ValidateAsync);
            tf.Instance.GetState(x => x.Error).Should().Be(true);
            tf.Instance.GetState(x => x.ErrorText).Should().Be("Error1");
        }

        /// <summary>
        /// Calling form.Reset() should clear the text field
        /// </summary>
        [Test]
        public async Task FormReset_Should_ClearTextField()
        {
            var comp = Context.Render<FormResetTest>();
            var form = comp.FindComponent<MudForm>();
            var textFieldComp = comp.FindComponents<MudTextField<string>>()[1]; //the picker includes a MudTextField, so the MudTextField we want is the second in the DOM
            var textField = textFieldComp.Instance;

            // input some text
            await textFieldComp.Find("input").InputAsync("asdf");
            textField.ReadValue.Should().Be("asdf");
            textField.ReadText.Should().Be("asdf");
            // call reset directly
            await comp.InvokeAsync(() => form.Instance.ResetAsync());
            textField.ReadValue.Should().BeNullOrEmpty();
            textField.ReadText.Should().BeNullOrEmpty();
            // input some text
            await textFieldComp.Find("input").InputAsync("asdf");
            textField.ReadValue.Should().Be("asdf");
            textField.ReadText.Should().Be("asdf");
            // hit reset button
            await comp.Find("button.reset").ClickAsync();
            textField.ReadValue.Should().BeNullOrEmpty();
            textField.ReadText.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Calling form.Reset() should clear the numeric field
        /// </summary>
        [Test]
        public async Task FormReset_Should_ClearNumericField()
        {
            var comp = Context.Render<FormResetTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var numericFieldComp = comp.FindComponent<MudNumericField<int?>>();
            var numericField = numericFieldComp.Instance;

            // input some text
            await numericFieldComp.Find("input").InputAsync(10);
            numericField.Value.Should().Be(10);
            numericField.ReadText.Should().Be("10");
            // call reset directly
            await comp.InvokeAsync(() => form.ResetAsync());
            numericField.Value.Should().BeNull();
            numericField.ReadText.Should().BeNullOrEmpty();
            // input some text

            await numericFieldComp.Find("input").InputAsync(20);
            numericField.Value.Should().Be(20);
            numericField.ReadText.Should().Be("20");
            // hit reset button
            await comp.Find("button.reset").ClickAsync();
            numericField.Value.Should().BeNull();
            numericField.ReadText.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Calling form.Reset() should clear the datepicker
        /// </summary>
        [Test]
        public async Task FormReset_Should_ClearDatePicker()
        {
            var comp = Context.Render<FormResetTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var datePickerComp = comp.FindComponent<MudDatePicker>();
            var datePicker = datePickerComp.Instance;
            // create test value and it's localized string representation
            var testDate = new DateTime(2020, 05, 24);
            var testDateString = testDate.ToShortDateString();  // locale independent test, will work e.g. in germany too

            // input a date
            await datePickerComp.Find("input").ChangeAsync(testDateString);
            datePicker.Date.Should().Be(testDate);
            datePicker.Text.Should().Be(testDateString);
            // call reset directly
            await comp.InvokeAsync(() => form.ResetAsync());
            datePicker.Date.Should().BeNull();
            datePicker.Text.Should().BeNullOrEmpty();

            // input a date
            await datePickerComp.Find("input").ChangeAsync(testDateString);
            datePicker.Date.Should().Be(testDate);
            datePicker.Text.Should().Be(testDateString);
            // hit reset button
            await comp.Find("button.reset").ClickAsync();
            datePicker.Date.Should().BeNull();
            datePicker.Text.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Calling form.Reset() should clear the DateRangePicker
        /// </summary>
        [Test]
        public async Task FormReset_Should_ClearDateRangePicker()
        {
            var comp = Context.Render<FormResetTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dateRangePickerComp = comp.FindComponent<MudDateRangePicker>();
            var dateRangePicker = dateRangePickerComp.Instance;
            // create test value and it's localized string representation
            var testStartDate = new DateTime(2020, 05, 24);
            var testEndDate = new DateTime(2020, 06, 24);

            // input a date
            await dateRangePickerComp.FindAll("input")[0].ChangeAsync(testStartDate.ToShortDateString());
            await dateRangePickerComp.FindAll("input")[1].ChangeAsync(testEndDate.ToShortDateString());
            dateRangePicker.DateRange.Start.Should().Be(testStartDate);
            dateRangePicker.DateRange.End.Should().Be(testEndDate);

            // call reset directly
            await comp.InvokeAsync(() => form.ResetAsync());
            dateRangePicker.DateRange.Should().BeNull();

            // input a date
            await dateRangePickerComp.FindAll("input")[0].ChangeAsync(testStartDate.ToShortDateString());
            await dateRangePickerComp.FindAll("input")[1].ChangeAsync(testEndDate.ToShortDateString());
            dateRangePicker.DateRange.Start.Should().Be(testStartDate);
            dateRangePicker.DateRange.End.Should().Be(testEndDate);
            // hit reset button
            await comp.Find("button.reset").ClickAsync();
            dateRangePicker.DateRange.Should().BeNull();
        }

        /// <summary>
        /// Reset() should reset the form's state
        /// </summary>
        [Test]
        public async Task FormReset_Should_ResetFormStateForFieldsThatWrapMudInput()
        {
            var comp = Context.Render<FormResetTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var datePickerComp = comp.FindComponent<MudDatePicker>();
            var textFieldComp = comp.FindComponents<MudTextField<string>>()[1]; //the picker includes a MudTextField, so the MudTextField we want is the second in the DOM
            var numericFieldComp = comp.FindComponent<MudNumericField<int?>>();
            // create test value and it's localized string representation
            var testDate = new DateTime(2022, 07, 29);
            var testDateString = testDate.ToShortDateString();  // locale independent test, will work e.g. in germany too

            form.IsValid.Should().Be(false);
            await datePickerComp.Find("input").ChangeAsync(testDateString);
            form.IsValid.Should().Be(false);
            await textFieldComp.Find("input").InputAsync("Some value");
            form.IsValid.Should().Be(false);
            await numericFieldComp.Find("input").InputAsync("1");
            form.IsValid.Should().Be(true);

            await comp.InvokeAsync(() => form.ResetAsync());
            form.IsValid.Should().Be(false); // required fields
        }

        /// <summary>
        /// Only the top SubscribeToParentForm fields should be registered inside the form.
        /// </summary>
        [Test]
        public void MudForm_Should_RegisterOnlyTopSubscribeToParentFormFormControls()
        {
            var comp = Context.Render<FormShouldRegisterOnlyTopSubscribeToParentFormFormControlsTest>();
            var form = comp.FindComponent<MudFormTestable>().Instance;

            form.FormControls.Count.Should().Be(13);
        }

        /// <summary>
        /// Test the cascading validaton parameter to override field validations or not depending of context.
        /// </summary>
        [Test]
        public void MudForm_Validation_Should_OverrideFieldValidation()
        {
            var comp = Context.Render<FormValidationOverrideFieldValidationTest>();
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
        public void FieldValidationWithoutRequiredForm_ShouldNot_Validate()
        {
            var comp = Context.Render<FieldValidationWithoutRequiredFormTest>();

            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-input-error"));
        }

        /// <summary>
        /// When changing field values, the FieldChanged event should fire with the correct IFormComponent and new value
        /// </summary>
        [Test]
        public async Task FieldChangedEventShouldTrigger()
        {
            var comp = Context.Render<FormFieldChangedTest>();

            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            var numeric = comp.FindComponent<MudNumericField<int>>().Instance;
            var radioGroup = comp.FindComponent<MudRadioGroup<string>>().Instance;

            comp.Instance.FormFieldChangedEventArgs.Should().BeNull();

            //in all below cases, the event args should switch to an instance of the field changed and contain the new value that was set

            await comp.InvokeAsync(() => textField.SetTextAsync("new value"));
            comp.Instance.FormFieldChangedEventArgs!.NewValue.Should().Be("new value");
            textField.Should().Be(comp.Instance.FormFieldChangedEventArgs.Field);

            numeric.Value.Should().Be(0);
            await comp.InvokeAsync(() => numeric.Increment());
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().Be(1);
            numeric.Should().Be(comp.Instance.FormFieldChangedEventArgs.Field);

            var inputs = comp.FindAll("input").ToArray();
            // check initial state
            radioGroup.Value.Should().Be(null);
            // click radio 1
            await inputs[2].ClickAsync();
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
        public async Task FieldChangedEventShouldTriggerPicker()
        {
            var comp = Context.Render<FormFieldChangedPickerTest>();
            var formsComp = comp.FindComponents<MudForm>();

            var datePicker = comp.FindComponent<MudDatePicker>();
            var timePicker = comp.FindComponent<MudTimePicker>();
            var colorPicker = comp.FindComponent<MudColorPicker>();

            comp.Instance.FormFieldChangedEventArgs.Should().BeNull();

            //in all below cases, the event args should switch to an instance of the field changed and contain the new value that was set

            var dateString = new DateTime(2022, 04, 03).ToShortDateString();
            await datePicker.Find("input").ChangeAsync(dateString);
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().Be(new DateTime(2022, 04, 03));
            datePicker.Instance.Should().Be(comp.Instance.FormFieldChangedEventArgs.Field);

            await timePicker.Find("input").ChangeAsync("00:45");
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().Be(new TimeSpan(00, 45, 00));
            timePicker.Instance.Should().Be(comp.Instance.FormFieldChangedEventArgs.Field);

            await colorPicker.Find("input").ChangeAsync("#180f6fff");
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().Be(new MudColor("#180f6fff"));
            colorPicker.Instance.Should().Be(comp.Instance.FormFieldChangedEventArgs.Field);
        }

        /// <summary>
        /// When Validation is set on a Form, it should only set validation on fields that have a For parameter
        /// </summary>
        [Test]
        public void FormAutoValidationSet()
        {
            var comp = Context.Render<FormAutomaticValidationTest>();
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

        /// <summary>
        /// Ensures that all child components are Readonly when the Form is Readonly
        /// </summary>
        [Test]
        public async Task FormReadonly()
        {
            var comp = Context.Render<FormReadOnlyDisabledTest>();

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
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.FormReadOnly, true).Add(p => p.CompReadOnly, false));

            textField.Find("input").HasAttribute("readonly").Should().BeTrue();
            maskedTextField.Find("input").HasAttribute("readonly").Should().BeTrue();
            checkBox.Find("label").ClassList.Should().Contain("mud-readonly");
            radioGroup.Find("label").ClassList.Should().Contain("mud-readonly");
            switch_.Find("label").ClassList.Should().Contain("mud-readonly");
            autocomplete.Find("input").HasAttribute("readonly").Should().BeTrue();
            numericField.Find("input").HasAttribute("readonly").Should().BeTrue();
            fileUpload.Find("input").HasAttribute("disabled").Should().BeTrue();

            //form readonly = false, comp readonly = true
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.FormReadOnly, false).Add(p => p.CompReadOnly, true));

            textField.Find("input").HasAttribute("readonly").Should().BeTrue();
            maskedTextField.Find("input").HasAttribute("readonly").Should().BeTrue();
            checkBox.Find("label").ClassList.Should().Contain("mud-readonly");
            radioGroup.Find("label").ClassList.Should().Contain("mud-readonly");
            switch_.Find("label").ClassList.Should().Contain("mud-readonly");
            autocomplete.Find("input").HasAttribute("readonly").Should().BeTrue();
            numericField.Find("input").HasAttribute("readonly").Should().BeTrue();
            fileUpload.Find("input").HasAttribute("disabled").Should().BeFalse(); //the file upload can't be readonly

            //form readonly = false, comp readonly = false
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.FormReadOnly, false).Add(p => p.CompReadOnly, false));

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
        public async Task FormDisabled()
        {
            var comp = Context.Render<FormReadOnlyDisabledTest>();

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
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.FormDisabled, true).Add(p => p.CompDisabled, false));

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
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.FormDisabled, false).Add(p => p.CompDisabled, true));

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
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.FormDisabled, false).Add(p => p.CompDisabled, false));

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
        public async Task FormNestedReadOnly()
        {
            var comp = Context.Render<FormNestedReadOnlyDisabledTest>();
            comp.FindAll(".mud-checkbox.mud-readonly").Count.Should().Be(0);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.ReadOnly, true));
            comp.FindAll(".mud-checkbox.mud-readonly").Count.Should().Be(1);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.NestedReadOnly, true));
            comp.FindAll(".mud-checkbox.mud-readonly").Count.Should().Be(1);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.ReadOnly, true).Add(p => p.NestedReadOnly, true));
            comp.FindAll(".mud-checkbox.mud-readonly").Count.Should().Be(1);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.ReadOnly, false).Add(p => p.NestedReadOnly, false));
            comp.FindAll(".mud-checkbox.mud-readonly").Count.Should().Be(0);
        }

        /// <summary>
        /// Ensures the child MudForm correctly inherits Disabled and applies it to its children
        /// </summary>
        [Test]
        public async Task FormNestedDisabled()
        {
            var comp = Context.Render<FormNestedReadOnlyDisabledTest>();
            comp.FindAll(".mud-checkbox.mud-disabled").Count.Should().Be(0);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Disabled, true));
            comp.FindAll(".mud-checkbox.mud-disabled").Count.Should().Be(1);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.NestedDisabled, true));
            comp.FindAll(".mud-checkbox.mud-disabled").Count.Should().Be(1);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Disabled, true).Add(p => p.NestedDisabled, true));
            comp.FindAll(".mud-checkbox.mud-disabled").Count.Should().Be(1);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Disabled, false).Add(p => p.NestedDisabled, false));
            comp.FindAll(".mud-checkbox.mud-disabled").Count.Should().Be(0);
        }

        [Test]
        public async Task FormWithChildForm()
        {
            var comp = Context.Render<FormWithChildFormTest>();
            var childFormSwitch = comp.Find(".mud-switch-input");
            var parentForm = comp.FindComponent<MudForm>().Instance;
            var parentTextFieldCmp = comp.FindComponent<MudTextField<string>>();
            var parentTextField = parentTextFieldCmp.Instance;
            // check initial state: form should not be valid, but text field does not display an error initially!
            parentForm.IsValid.Should().Be(false);
            parentTextField.GetState(x => x.Error).Should().BeFalse();
            parentTextField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            await parentTextFieldCmp.Find("input").ChangeAsync("Marilyn Manson");
            parentForm.IsValid.Should().Be(true);
            parentForm.Errors.Length.Should().Be(0);
            parentTextField.GetState(x => x.Error).Should().BeFalse();
            parentTextField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            // display the child form
            await childFormSwitch.ChangeAsync(true);
            var forms = comp.FindComponents<MudForm>();
            forms.Count.Should().Be(2);
            var childForm = forms[1];
            childForm.Instance.IsValid.Should().BeFalse();
            parentForm.IsValid.Should().Be(false);

            // remove the child form
            await childFormSwitch.ChangeAsync(false);
            forms = comp.FindComponents<MudForm>();
            forms.Count.Should().Be(1);
            parentForm.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task FormComponent_Should_UpdateValidationMessagesOnEditContextChanged()
        {
            var comp = Context.Render<FormComponentUpdateValidationMessagesOnEditContextChangedTest>();
            var validator = comp.FindComponent<FormComponentUpdateValidationMessagesValidator>();
            var errorMessage = "some error";

            await validator.InvokeAsync(() => validator.Instance.AddError(nameof(FormComponentUpdateValidationMessagesModel.Name), errorMessage));

            var tf = comp.FindComponent<MudTextField<string>>();
            tf.Instance.ValidationErrors.Should().HaveCount(1).And.Contain(new[] { errorMessage });

            await validator.InvokeAsync(() => validator.Instance.ClearErrors());

            tf.Instance.ValidationErrors.Should().HaveCount(0);
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public async Task FormComponent_ErrorTextTwoWayBinding()
        {
            var comp = Context.Render<FormWithErrorTextTwoWayBindingTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textField = comp.FindComponent<MudTextField<string>>().Instance;

            // check initial state: ErrorText and bound Property should be the default value defined inside the component property.
            comp.Instance.BoundErrorText.Should().Be("Default value not changed by binding");
            textField.GetState(x => x.ErrorText).Should().Be("Default value not changed by binding");

            // call validation on the textfield: now the error text should be null and the bound property aswell
            await comp.InvokeAsync(() => textField.ValidateAsync());
            textField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            comp.Instance.BoundErrorText.Should().BeNullOrEmpty();

            // empty the input text and call validation: now the error text and the bound property should be the validation error message.
            await comp.Find("input").ChangeAsync("");
            await comp.InvokeAsync(() => textField.ValidateAsync());
            textField.GetState(x => x.ErrorText).Should().Be("EmptyOrWhitespace!");
            comp.Instance.BoundErrorText.Should().Be("EmptyOrWhitespace!");
            comp.Markup.Should().Contain("EmptyOrWhitespace!");
        }

        /// <summary>
        /// CheckBox should be validated like every other form component when ticked using mouse
        /// </summary>
        [Test]
        public async Task FormWithCheckBox_When_CheckBoxTickedUsingMouse()
        {
            var comp = Context.Render<FormWithCheckBoxTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var checkBox = comp.FindComponent<MudCheckBox<bool>>().Instance;

            // check initial state: form should not be valid because checkBox is required
            form.IsValid.Should().Be(false);
            checkBox.GetState(x => x.Error).Should().BeFalse();
            checkBox.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            // tick checkBox with an emulated mouse click
            await comp.Find("input").ChangeAsync(true);
            form.IsTouched.Should().Be(true);
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            checkBox.GetState(x => x.Error).Should().BeFalse();
            checkBox.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            // untick checkBox with an emulated mouse click
            await comp.Find("input").ChangeAsync(false);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            checkBox.GetState(x => x.Error).Should().BeTrue();
            checkBox.GetState(x => x.ErrorText).Should().Be("Required");
        }

        /// <summary>
        /// CheckBox should be validated like every other form component when ticked using keyboard
        /// </summary>
        [Test]
        public void FormWithCheckBox_When_CheckBoxTickedUsingKeyboard()
        {
            var comp = Context.Render<FormWithCheckBoxTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var checkBox = comp.FindComponent<MudCheckBox<bool>>().Instance;

            // check initial state: form should not be valid because checkBox is required
            form.IsValid.Should().Be(false);
            checkBox.GetState(x => x.Error).Should().BeFalse();
            checkBox.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            // tick checkBox with a key press
            comp.Find("input").KeyDown(Key.Space);
            form.IsTouched.Should().Be(true);
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            checkBox.GetState(x => x.Error).Should().BeFalse();
            checkBox.GetState(x => x.ErrorText).Should().BeNullOrEmpty();

            // untick checkBox with a key press
            comp.Find("input").KeyDown(Key.Space);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Required");
            checkBox.GetState(x => x.Error).Should().BeTrue();
            checkBox.GetState(x => x.ErrorText).Should().Be("Required");
        }

        [Test]
        public async Task FormSpacingClass()
        {
            var comp = Context.Render<MudForm>();

            for (var i = 0; i <= 20; i++)
            {
                await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Spacing, i));
                comp.Find("form.mud-form").ClassList.Should().Contain($"gap-{i}");
            }
        }

        [Test]
        public async Task ChildForm_TouchChangedPropagate()
        {
            var comp = Context.Render<FormWithChildFormTest>();
            var childFormSwitch = () => comp.Find(".mud-switch-input");
            var parentForm = comp.FindComponent<MudForm>().Instance;
            // display the child form
            await childFormSwitch().ChangeAsync(true);
            var forms = comp.FindComponents<MudForm>();
            forms.Count.Should().Be(2);
            var childForm = forms[1];
            var childTextFieldCmp = childForm.FindComponent<MudTextField<string>>();
            childForm.Instance.IsValid.Should().BeFalse();
            parentForm.IsValid.Should().Be(false);

            // triggering childform touch should trigger parent form touched
            await childTextFieldCmp.Find("input").ChangeAsync("Marilyn Manson");

            // verify child and parent touch events happened
            comp.Instance.IsParentTouchChanged.Should().BeTrue();
            comp.Instance.IsChildTouchChanged.Should().BeTrue();

            // verify they start as false
            await comp.InvokeAsync(async () => await parentForm.ResetAsync());
            comp.Instance.IsParentTouchChanged.Should().BeFalse();
            comp.Instance.IsChildTouchChanged.Should().BeFalse();
        }

        /// <summary>
        /// Regression test for: https://github.com/MudBlazor/MudBlazor/issues/12012.
        /// When a form has a validation error and the bound property is updated through code,
        /// the validation error should be cleared if the new value is valid, or updated if still invalid.
        /// </summary>
        [Test]
        public async Task FormValidationErrorClearedOnProgrammaticValueChange()
        {
            var comp = Context.Render<FormWithSingleTextField>();
            var form = comp.FindComponent<MudForm>().Instance;
            var textFieldComp = comp.FindComponent<MudTextField<string>>();
            var textField = textFieldComp.Instance;

            // Set validation that requires non-empty string
            await textFieldComp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Required, true)
                .Add(x => x.RequiredError, "This field is required"));

            // Simulate user interaction: Tab out of field to trigger validation error
            await textFieldComp.Find("input").BlurAsync();
            await comp.WaitForAssertionAsync(() =>
            {
                form.IsValid.Should().BeFalse();
                textField.GetState(x => x.Error).Should().BeTrue();
                textField.GetState(x => x.ErrorText).Should().Be("This field is required");
            });

            // Now set a valid value programmatically through parameter binding
            await textFieldComp.SetParametersAndRenderAsync(parameters =>
                parameters.Add(x => x.Value, "Valid Value"));

            // The validation error should be cleared because the value is now valid
            await comp.WaitForAssertionAsync(() =>
            {
                form.IsValid.Should().BeTrue();
                textField.GetState(x => x.Error).Should().BeFalse();
                textField.GetState(x => x.ErrorText).Should().BeNullOrEmpty();
            });

            // Clear the value programmatically through parameter binding
            await textFieldComp.SetParametersAndRenderAsync(parameters =>
                parameters.Add(x => x.Value, string.Empty));

            // The validation error should reappear because the value is now invalid
            await comp.WaitForAssertionAsync(() =>
            {
                form.IsValid.Should().BeFalse();
                textField.GetState(x => x.Error).Should().BeTrue();
                textField.GetState(x => x.ErrorText).Should().Be("This field is required");
            });
        }
    }
}
