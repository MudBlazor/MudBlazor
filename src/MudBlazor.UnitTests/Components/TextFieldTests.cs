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
    }
}
