using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;
using NUnit.Framework.Internal;


namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class DatePickerTests
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
        /// Rendering 10.000 DatePickers to measure performance impact of initial render.
        /// </summary>
        [Test]
        [Ignore("Unignore for performance measurements, not needed for code coverage")]
        public void PerformanceTest1()
        {
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
        [Ignore("Unignore for performance measurements, not needed for code coverage")]
        public async Task PerformanceTest2()
        {
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

        /// <summary>
        /// Setting the date should change the value and vice versa
        /// </summary>
        [Test]
        public async Task SetPickerDateAndValue() {
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

        public IRenderedComponent<MudDatePicker> OpenPicker()
        {
            var comp = ctx.RenderComponent<MudDatePicker>();
            Console.WriteLine(comp.Markup);
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            return comp;
        }

        /// <summary>
        /// Datepicker should open on input click and close on outside click
        /// </summary>
        [Test]
        public void OpenThenCloseByClickingOutsidePicker()
        {
            var comp = OpenPicker();
            // clicking outside to close
            comp.Find("div.mud-overlay").Click();
            // should not be open any more
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
        }

        /// <summary>
        /// Datepicker should close on day button click and return a date
        /// </summary>
        [Test]
        public async Task OpenThenCloseBySelectingADate()
        {
            var comp = OpenPicker();
            // clicking a day button to select a date and close
            comp.FindAll("button.mud-day")[8].Click(); // take a day from the middle section (at the beginning buttons may be disabled)
            await Task.Delay(comp.Instance.ClosingDelay + 50); // Check 10ms after closing
            // should not be open any more
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            comp.Instance.Date.Should().NotBeNull();
        }

        /// <summary>
        /// open to month
        /// </summary>
        [Test]
        public void OpenToYear()
        {
            var comp = ctx.RenderComponent<MudDatePicker>(("OpenTo", OpenTo.Year));
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            comp.Instance.Date.Should().BeNull();
            // click to to open menu
            comp.Find("input").Click();
            Console.WriteLine(comp.Markup);
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            comp.Instance.Date.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        /// <summary>
        /// open to month
        /// </summary>
        [Test]
        public void OpenToMonth()
        {
            var comp = ctx.RenderComponent<MudDatePicker>(("OpenTo", OpenTo.Month));
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            comp.Instance.Date.Should().BeNull();
            // click to to open menu
            comp.Find("input").Click();
            Console.WriteLine(comp.Markup);
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            comp.Instance.Date.Should().BeNull();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        public IRenderedComponent<MudDatePicker> OpenToDecember()
        {
            var comp = ctx.RenderComponent<MudDatePicker>(("PickerMonth", new DateTime(2020, 12, 01)));
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);;
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            comp.Instance.PickerMonth.Value.Month.Should().Be(12);
            return comp;
        }

        /// <summary>
        /// navigate prev month
        /// </summary>
        [Test]
        public void OpenToDecemberNavigateToNovember()
        {
            var comp = OpenToDecember();
            var picker = comp.Instance;
            comp.Find("div.mud-picker-calendar-header-switch > button:nth-child(1)").Click();
            picker.PickerMonth.Value.Month.Should().Be(11);
        }

        /// <summary>
        /// navigate next month
        /// </summary>
        [Test]
        public void OpenToDecemberNavigateToJanuaryCheckYearChange()
        {
            var comp = OpenToDecember();
            var picker = comp.Instance;
            comp.Find("div.mud-picker-calendar-header-switch > button:nth-child(3)").Click();
            picker.PickerMonth.Value.Month.Should().Be(1);
            picker.PickerMonth.Value.Year.Should().Be(2021);
        }
    }
}