#pragma warning disable 1998

using System;
using System.Threading.Tasks;
using Bunit;
using Bunit.Rendering;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents.Form;
using NUnit.Framework;

namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class FormTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            ctx.Services.AddSingleton<IDialogService>(new DialogService());
            ctx.Services.AddSingleton<ISnackbar>(new MockSnackbar());
            ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();
        
        /// <summary>
        /// Setting the required textfield's value should set IsValid true
        /// Clearing the value of a required textfield should set form's IsValid to false.
        /// </summary>
        [Test]
        public async Task FormIsValidTest() {
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
            var validationFunc = new Func<string, bool>(x => x?.StartsWith("Marilyn") == true);
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
            comp.FindAll("input")[0].Change(new ChangeEventArgs(){ Value = "asdf"});
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
    }
}

