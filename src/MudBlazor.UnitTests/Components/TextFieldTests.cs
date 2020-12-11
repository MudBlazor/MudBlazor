using System;
using System.Collections.Generic;
using System.Globalization;
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
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class TextFieldTests
    {

        /// <summary>
        /// Initial Text for double should be 0, with F1 format it should be 0.0
        /// </summary>
        [Test]
        public void TextFieldTest1() {
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
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
            textfield.Culture = CultureInfo.InvariantCulture;
            textfield.Format = "F1";
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
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
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
            using var ctx = new Bunit.TestContext();
            //ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
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
            using var ctx = new Bunit.TestContext();
            //ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
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
            //Arrange
            using var ctx = new Bunit.TestContext();
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
            //Arrange
            using var ctx = new Bunit.TestContext();
            var interval = Parameter(nameof(MudTextField<string>.DebounceInterval), 1000d);
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

            //DebounceInterval is 1000 ms, so at 500 ms Value should not change in TextField
            await Task.Delay(500);
            textField.Value.Should().BeNull();

            //More than 1000 ms had elapsed, so Value should be updated
            await Task.Delay(510);
            textField.Value.Should().Be("Some Value");
        }




    }
}
