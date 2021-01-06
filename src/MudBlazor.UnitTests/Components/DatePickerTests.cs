﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bunit;
using static Bunit.ComponentParameterFactory;
using FluentAssertions;
using Microsoft.JSInterop;
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

        [Test]
        [Ignore("Unignore for performance measurements, not needed for code coverage")]
        public void RenderDatePicker_10000_Times_CheckPerformance()
        {
            // warmup
            ctx.RenderComponent<DatePickerPerformanceTest>();
            // measure
            var watch = Stopwatch.StartNew();
            for(int i = 0; i<10000; i++)
                ctx.RenderComponent<DatePickerPerformanceTest>();
            watch.Stop();
            Console.WriteLine("Elapsed: " + watch.Elapsed);
        }

        /// <summary>

        [Test]
        [Ignore("Unignore for performance measurements, not needed for code coverage")]
        public async Task Open_Close_DatePicker_10000_Times_CheckPerformance()
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

        [Test]
        public async Task SetPickerValue_CheckDate_SetPickerDate_CheckValue() 
        {
            var comp = ctx.RenderComponent<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Value.Should().Be(null);
            picker.Date.Should().Be(null);
            await comp.InvokeAsync(() => picker.Value = "2020-10-23");
            picker.Date.Should().Be(new DateTime(2020, 10, 23));
            await comp.InvokeAsync(() => picker.Date = new DateTime(2020, 10, 26));
            picker.Value.Should().Be("2020-10-26");
        }

        public IRenderedComponent<MudDatePicker> OpenPicker(ComponentParameter parameter)
        {
            return OpenPicker(new ComponentParameter[]{parameter});
        }

        public IRenderedComponent<MudDatePicker> OpenPicker(ComponentParameter[] parameters = null)
        {
            IRenderedComponent<MudDatePicker> comp;
            if(parameters is null)
            {
                comp = ctx.RenderComponent<MudDatePicker>();
            }
            else
            {
                comp = ctx.RenderComponent<MudDatePicker>(parameters);
            }

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
        public void Open_CloseByClickingOutsidePicker()
        {
            var comp = OpenPicker();
            // clicking outside to close
            comp.Find("div.mud-overlay").Click();
            // should not be open any more
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
        }

        [Test]
        public async Task Open_CloseBySelectingADate_CheckClosed()
        {
            var comp = OpenPicker();
            // clicking a day button to select a date and close
            comp.FindAll("div.mud-picker-calendar-day > button")
                .Where(x=>x.TrimmedText().Equals("23")).First().Click();
            await Task.Delay(comp.Instance.ClosingDelay + 50); // allow a delay
            // should not be open any more
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            comp.Instance.Date.Should().NotBeNull();
        }

        [Test]
        public void OpenToYear_CheckYearsShown()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Year));
            comp.Instance.Date.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public void OpenToMonth_Select3rdMonth_Select2ndDay_CheckDate()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Month));
            comp.Instance.Date.Should().BeNull();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-content > div.mud-picker-month-container > div.mud-picker-month")
                .Skip(2).First().Click();
            comp.FindAll("div.mud-picker-calendar-day > button")
                .Where(x=>x.TrimmedText().Equals("2")).First().Click();
            comp.Instance.Date.Value.Date.Should().Be(new DateTime(DateTime.Now.Year, 3, 2));
        }

        public IRenderedComponent<MudDatePicker> OpenTo12thMonth()
        {
            var comp = OpenPicker(Parameter("PickerMonth", new DateTime(DateTime.Now.Year, 12, 01)));
            comp.Instance.PickerMonth.Value.Month.Should().Be(12);
            return comp;
        }

        [Test]
        public void Open_ClickCalendarHeader_Click4thMonth_Click23rdDay_CheckDate()
        {
            var comp = OpenPicker();
            var picker = comp.Instance;
            comp.Find("div.mud-picker-calendar-header-transition").Click();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-content > div.mud-picker-month-container > div.mud-picker-month")
                .Skip(3).First().Click();
            comp.FindAll("div.mud-picker-calendar-day > button")
                .Where(x=>x.TrimmedText().Equals("23")).First().Click();
            comp.Instance.Date.Value.Date.Should().Be(new DateTime(DateTime.Now.Year, 4, 23));
        }

        [Test]
        public void OpenTo12thMonth_NavigateBack_CheckMonth()
        {
            var comp = OpenTo12thMonth();
            var picker = comp.Instance;
            comp.Find("div.mud-picker-calendar-header-switch > button:nth-child(1)").Click();
            picker.PickerMonth.Value.Month.Should().Be(11);
            picker.PickerMonth.Value.Year.Should().Be(DateTime.Now.Year);
        }

        [Test]
        public void OpenTo12thMonth_NavigateForward_CheckYear()
        {
            var comp = OpenTo12thMonth();
            var picker = comp.Instance;
            comp.Find("div.mud-picker-calendar-header-switch > button:nth-child(3)").Click();
            picker.PickerMonth.Value.Month.Should().Be(1);
            picker.PickerMonth.Value.Year.Should().Be(DateTime.Now.Year + 1);
        }
     
        [Test]
        public void Open_ClickYear_ClickCurrentYear_Click1_CheckDate()
        {
            var comp = OpenPicker();
            comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").Click();
            comp.FindAll("div.mud-picker-content > div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-content > div.mud-picker-year-container > div.mud-picker-year")
                .Where(x=>x.TrimmedText().Contains("2022")).First().Click();
            comp.FindAll("div.mud-picker-content > div.mud-picker-calendar-header").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-day > button")
                .Where(x=>x.TrimmedText().Equals("1")).First().Click();
            comp.Instance.Date.Value.Date.Should().Be(new DateTime(2022, DateTime.Now.Month, 1));
        }
    }
}