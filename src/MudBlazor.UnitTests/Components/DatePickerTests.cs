using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;


namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class DatePickerTests
    {
        [Test]
        public async Task SimpleTest() {
            // Setting the date should change the value and vice versa
            // setup
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton< NavigationManager >(new MockNavigationManager());
            var comp = ctx.RenderComponent<MudDatePicker>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Value.Should().Be(null);
            picker.Date.Should().Be(null);
            await comp.InvokeAsync(() => picker.Value = "2020-10-23");
            picker.Date.Should().Be(new DateTime(2020, 10, 23)); // time of this writing ;)
            await comp.InvokeAsync(() => picker.Date = new DateTime(2020, 10, 26)); // <-- Austrian National Holiday
            picker.Value.Should().Be("2020-10-26");
        }



    }
}
