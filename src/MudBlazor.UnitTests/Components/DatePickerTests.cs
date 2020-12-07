using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;
using NUnit.Framework.Internal;


namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class DatePickerTests
    {
        /// <summary>
        /// Setting the date should change the value and vice versa
        /// </summary>
        [Test]
        public async Task SimpleTest() {
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


        /// <summary>
        /// Rendering 10.000 DatePickers to measure performance impact of initial render.
        /// </summary>
        [Test]
        public async Task PerformanceTest1()
        {
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            // warmup
            ctx.RenderComponent<DatePickerPerformanceTest>();
            // measure
            var watch = Stopwatch.StartNew();
            for(int i = 0; i<10; i++)
                ctx.RenderComponent<DatePickerPerformanceTest>();
            watch.Stop();
            Console.WriteLine("Elapsed: " + watch.Elapsed);
        }

        /// <summary>
        /// Measuring performance impact of drop-down opening
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PerformanceTest2()
        {
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            // warmup
            var comp=ctx.RenderComponent<MudDatePicker>();
            var datepicker = comp.Instance;
            // measure
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                await comp.InvokeAsync(() => datepicker.Open());
                await comp.InvokeAsync(() => datepicker.Close());
            }
            watch.Stop();
            Console.WriteLine("Elapsed: " + watch.Elapsed);
        }
    }
}
