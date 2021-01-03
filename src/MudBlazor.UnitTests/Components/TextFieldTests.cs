#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class TextFieldTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddMudBlazorServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        /// <summary>
        /// Initial Text for double should be 0, with F1 format it should be 0.0
        /// </summary>
        [Test]
        public async Task TextFieldTest1() {
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
            var textfield = comp.Instance;
            await comp.InvokeAsync(()=>textfield.Value=null);
            comp.Find("input").Blur();
            comp.FindAll("div.mud-input-error").Count.Should().Be(0);
            await comp.InvokeAsync(() => textfield.Text = "");
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
            Console.WriteLine(comp.Markup);
            var textfield = comp.Instance;
            await comp.InvokeAsync(() => textfield.Text = "seventeen");
            comp.Find("input").Blur();
            comp.FindAll("div.mud-input-error").Count.Should().Be(1);
            comp.Find("div.mud-input-error").TextContent.Trim().Should().Be("Not a valid number");
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
            textField.Value.Should().Be("Some Value");
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
            var comp = ctx.RenderComponent<MudTextField<string>>( label);
            comp.Markup.Should().NotContain("shrink");
            //with placeholder label is shrinked
            comp.SetParametersAndRender( placeholder);
            comp.Markup.Should().Contain("shrink");
        }

        /// <summary>
        /// This is a FluentValidation validator which we'll use to validate a MudTextfield
        /// </summary>
        public class TestValidator : AbstractValidator<string>
        {
            public TestValidator()
            {
                RuleFor(x => x).CreditCard();
            }
        }
        
        /// <summary>
        /// FluentValidation rules can be used for validating a TextFields
        /// </summary>
        [Test]
        public async Task TextFieldFluentValidationTest()
        {
            bool validatonFuncHasBeenCalled = false;
            // create a validation func based on a FluentValidation validator.
            var validationFunc = new Func<string, IEnumerable<string>>(input =>
            {
                validatonFuncHasBeenCalled = true;
                var validator = new TestValidator();
                var result = validator.Validate(input);
                if (result.IsValid)
                    return new string[0];
                return result.Errors.Select(e => e.ErrorMessage);
            });
            var comp = ctx.RenderComponent<MudTextField<string>>(Parameter(nameof(MudTextField<string>.Validation), validationFunc));
            var textfield = comp.Instance;
            Console.WriteLine(comp.Markup);
            // first try a valid credit card number
            await comp.InvokeAsync(() => textfield.Text = "4012 8888 8888 1881");
            validatonFuncHasBeenCalled.Should().BeTrue();
            textfield.Error.Should().BeFalse(because: "The number is a valid VISA test credit card number");
            textfield.ErrorText.Should().BeNullOrEmpty();
            // now try something that produces a validation error
            await comp.InvokeAsync(() => textfield.Text = "0000 1111 2222 3333");
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
            // these convertsion funcs are nonsense of course, but they are designed this way to
            // test against an infinite update loop that textfields and other inputs are now protected against.
            var textfield = comp.Instance;
            textfield.Converter.SetFunc = s => $"{s}x";
            textfield.Converter.GetFunc = s => $"{s}y";
            await comp.InvokeAsync(() => textfield.Value = "A");
            textfield.Value.Should().Be("A");
            textfield.Text.Should().Be("Ax");
            await comp.InvokeAsync(() => textfield.Text = "B");
            textfield.Value.Should().Be("By");
            textfield.Text.Should().Be("B");
        }
    }
}
