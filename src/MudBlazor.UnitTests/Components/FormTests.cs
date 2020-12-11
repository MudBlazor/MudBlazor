using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using Bunit.Rendering;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;
using static MudBlazor.UnitTests.SelectWithEnumTest;

namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class FormTests
    {
        /// <summary>
        /// Setting the required textfield's value should set IsValid true
        /// Clearing the value of a required textfield should set form's IsValid to false.
        /// </summary>
        [Test]
        public async Task FormIsValidTest() {
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<FormIsValidTest>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            // check initial state: form should not be valid, but text field does not display an error initially!
            form.IsValid.Should().Be(false);
            textField.Error.Should().BeFalse();
            textField.ErrorText.Should().BeNullOrEmpty();
            await comp.InvokeAsync(()=> textField.Value = "Marilyn Manson");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            textField.Error.Should().BeFalse();
            textField.ErrorText.Should().BeNullOrEmpty();
            // clear value to null
            await comp.InvokeAsync(() => textField.Value = null);
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Enter a rock star");
            textField.Error.Should().BeTrue();
            textField.ErrorText.Should().Be("Enter a rock star");
            // set value to "" -> should also be an error
            await comp.InvokeAsync(() => textField.Value = "");
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            form.Errors[0].Should().Be("Enter a rock star");
            textField.Error.Should().BeTrue();
            textField.ErrorText.Should().Be("Enter a rock star");
            //
            await comp.InvokeAsync(() => textField.Value = "Kurt Cobain");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            textField.Error.Should().BeFalse();
            textField.ErrorText.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Custom validation func should be called to determine whether or not a form value is good
        /// </summary>
        [Test]
        public async Task FormValidationTest1()
        {
            using var ctx = new Bunit.TestContext();
            var validationFunc = new Func<string, Task<bool>>(async x => x?.StartsWith("Marilyn") == true);
            var comp = ctx.RenderComponent<FormValidationTest>(ComponentParameter.CreateParameter("validation", validationFunc));
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            // check initial state: form should not be valid, but text field does not display an error initially!
            form.IsValid.Should().Be(false);
            textField.Error.Should().BeFalse();
            textField.ErrorText.Should().BeNullOrEmpty();
            await comp.InvokeAsync(() => textField.Value = "Marilyn Manson");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            textField.Error.Should().BeFalse();
            textField.ErrorText.Should().BeNullOrEmpty();
            // this rock star doesn't start with Marilyn
            await comp.InvokeAsync(() => textField.Value = "Kurt Cobain"); 
            form.IsValid.Should().Be(false);
            form.Errors.Length.Should().Be(1);
            textField.Error.Should().BeTrue();
            textField.ErrorText.Should().Be("Invalid");
            // value is not required, so don't call the validation func on empty text
            await comp.InvokeAsync(() => textField.Value = "");
            form.IsValid.Should().Be(true);
            form.Errors.Length.Should().Be(0);
            textField.Error.Should().BeFalse();
            textField.ErrorText.Should().BeNullOrEmpty();
            // ok, not a rock star, but a star nonetheless
            await comp.InvokeAsync(() => textField.Value = "Marilyn Monroe");
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
            using var ctx = new Bunit.TestContext();
            var validationFunc = new Func<string, string>(s =>
            {
                if (!(s.StartsWith("Marilyn") || s.EndsWith("Manson")))
                    return "Not a star!";
                return null;
            });
            var comp = ctx.RenderComponent<FormValidationTest>(ComponentParameter.CreateParameter("validation", validationFunc));
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            form.IsValid.Should().Be(false);
            await comp.InvokeAsync(() => textField.Value = "Marilyn Manson");
            form.IsValid.Should().Be(true);
            // this one might not be a star, but our custom validation func deems him valid nonetheless
            await comp.InvokeAsync(() => textField.Value = "Charles Manson");
            form.IsValid.Should().Be(true);
            // value is not required, so don't call the validation func on empty text
            await comp.InvokeAsync(() => textField.Value = "");
            form.IsValid.Should().Be(true);
            // clearly a star
            await comp.InvokeAsync(() => textField.Value = "Marilyn Monroe");
            form.IsValid.Should().Be(true);
            // not a star according to our validation func
            await comp.InvokeAsync(() => textField.Value = "Manson Marilyn");
            form.IsValid.Should().Be(false);
        }

        /// <summary>
        /// Reset() should reset the input components of the form
        /// </summary>
        [Test]
        public async Task FormValidationTest3()
        {
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<FormValidationTest>();
            Console.WriteLine(comp.Markup);
            var form = comp.FindComponent<MudForm>().Instance;
            var textField = comp.FindComponent<MudTextField<string>>().Instance;
            form.IsValid.Should().Be(false);
            await comp.InvokeAsync(() => textField.Value = "Some value");
            form.IsValid.Should().Be(true);
            // calling Reset() should reset the textField's value
            await comp.InvokeAsync(() =>form.Reset());
            textField.Value.Should().Be(null);
            textField.Text.Should().Be(null);
            form.IsValid.Should().Be(false); // because we did reset validation state as a side-effect.
        }
    }
}
