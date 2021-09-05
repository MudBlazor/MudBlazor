﻿#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DateRangePickerTests : BunitTest
    {
        [Test]
        [Ignore("Unignore for performance measurements, not needed for code coverage")]
        public void RenderDateRangePicker_10000_Times_CheckPerformance()
        {
            // warmup
            Context.RenderComponent<MudDateRangePicker>();
            // measure
            var watch = Stopwatch.StartNew();
            for (var i = 0; i < 10000; i++)
                Context.RenderComponent<MudDateRangePicker>();
            watch.Stop();
            Console.WriteLine("Elapsed: " + watch.Elapsed);
            watch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }

        [Test]
        [Ignore("Unignore for performance measurements, not needed for code coverage")]
        public async Task Open_Close_DateRangePicker_10000_Times_CheckPerformance()
        {
            // warmup
            var comp = Context.RenderComponent<MudDateRangePicker>();
            var datepicker = comp.Instance;
            // measure
            var watch = Stopwatch.StartNew();
            for (var i = 0; i < 10000; i++)
            {
                await comp.InvokeAsync(() => datepicker.Open());
                await comp.InvokeAsync(() => datepicker.Close());
            }
            watch.Stop();
            Console.WriteLine("Elapsed: " + watch.Elapsed);
            watch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task SetPickerValue_CheckDateRange_SetPickerDate_CheckValue()
        {
            var comp = Context.RenderComponent<MudDateRangePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().BeNullOrEmpty();
            picker.DateRange.Should().Be(null);
            await comp.InvokeAsync(() => picker.Text = RangeConverter<DateTime>.Join(new DateTime(2021, 01, 01).ToShortDateString(), new DateTime(2021, 01, 10).ToShortDateString()));
            picker.DateRange.Start.Should().Be(new DateTime(2021, 01, 01));
            picker.DateRange.End.Should().Be(new DateTime(2021, 01, 10));
            await comp.InvokeAsync(() => picker.DateRange = new DateRange(new DateTime(2020, 12, 26), new DateTime(2021, 02, 01)));
            picker.Text.Should().Be(RangeConverter<DateTime>.Join(new DateTime(2020, 12, 26).ToShortDateString(), new DateTime(2021, 02, 01).ToShortDateString()));
        }

        [Test]
        public void Open_SelectTheSameDateTwice_RangeStartShouldEqualsEnd()
        {
            var comp = OpenPicker();
            // clicking a day button to select a date
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("10")).First().Click();
            // clicking a same date then close
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("10")).First().Click();
            comp.Instance.DateRange.Start.Should().Be(comp.Instance.DateRange.End);
        }

        [Test]
        public void OpenPickerWithCustomStartMonth_SetDateRange_CheckValue()
        {
            var start = DateTime.Now.AddMonths(-1);
            var comp = OpenPicker(Parameter(nameof(MudDateRangePicker.StartMonth), start));
            // clicking a day buttons to select a range and close
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("10")).First().Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("12")).First().Click();
            //check result
            comp.Instance.DateRange.Start.Value.Month.Should().Be(start.Month);
            comp.Instance.DateRange.End.Value.Month.Should().Be(start.Month);
        }

        public IRenderedComponent<MudDateRangePicker> OpenPicker(ComponentParameter parameter)
        {
            return OpenPicker(new ComponentParameter[] { parameter });
        }

        public IRenderedComponent<MudDateRangePicker> OpenPicker(ComponentParameter[] parameters = null)
        {
            IRenderedComponent<MudDateRangePicker> comp;
            if (parameters is null)
            {
                comp = Context.RenderComponent<MudDateRangePicker>();
            }
            else
            {
                comp = Context.RenderComponent<MudDateRangePicker>(parameters);
            }

            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            return comp;
        }

        [Test]
        public void Open_CloseByClickingOutsidePicker_CheckClosed()
        {
            var comp = OpenPicker();
            // clicking outside to close
            comp.Find("div.mud-overlay").Click();
            // should not be open any more
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
        }

        [Test]
        public void Open_CloseBySelectingADateRange_CheckClosed()
        {
            var comp = OpenPicker();
            // clicking a day buttons to select a range and close
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("10")).First().Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("23")).First().Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0), TimeSpan.FromSeconds(5));
            comp.Instance.DateRange.Should().NotBeNull();
        }

        [Test]
        public void Open_SelectEndDateLowerThanStart_CheckNotClosed_SelectRange_CheckClosed()
        {
            var comp = OpenPicker();
            // clicking a day buttons to select a range and close
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("10")).First().Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("8")).First().Click();
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            comp.Instance.DateRange.Should().BeNull();

            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("23")).First().Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0), TimeSpan.FromSeconds(5));
            comp.Instance.DateRange.Should().NotBeNull();
        }

        [Test]
        public void OpenToYear_CheckYearsShown()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Year));
            comp.Instance.DateRange.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public void OpenToYear_ClickYear_CheckMonthsShown()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Year));
            comp.Instance.DateRange.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-year")[0].Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public void OpenToYear_ClickYear_CheckMonthsShown_Close_Reopen_CheckYearsShown()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Year));
            comp.Instance.DateRange.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-year")[0].Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            // clicking outside to close
            comp.Find("div.mud-overlay").Click();
            // should not be open any more
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            comp.Find("input").Click();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public void OpenToMonth_CheckMonthsShown()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Month));
            comp.Instance.DateRange.Should().BeNull();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public void Open_ClickCalendarHeader_CheckMonthsShown()
        {
            var comp = OpenPicker();
            // should show months
            comp.FindAll("button.mud-picker-calendar-header-transition")[0].Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public void Open_ClickYear_CheckYearsShown()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Month));
            // should show years
            comp.FindAll("button.mud-picker-calendar-header-transition")[0].Click();
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public void OpenToMonth_Select3rdMonth_Select2ndDay_CheckDateRange()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Month));
            comp.Instance.DateRange.Should().BeNull();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[2].Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("2")).First().Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("10")).First().Click();
            comp.Instance.DateRange.Start.Should().Be(new DateTime(DateTime.Now.Year, 3, 2));
        }

        public IRenderedComponent<MudDateRangePicker> OpenTo12thMonth()
        {
            var comp = OpenPicker(Parameter("PickerMonth", new DateTime(DateTime.Now.Year, 12, 01)));
            comp.Instance.PickerMonth.Value.Month.Should().Be(12);
            return comp;
        }

        [Test]
        public void Open_ClickCalendarHeader_Click4thMonth_Click23rdDay_CheckDateRange()
        {
            var comp = OpenPicker();
            var picker = comp.Instance;
            comp.Find("button.mud-picker-calendar-header-transition").Click();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")
                [3].Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("10")).First().Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("12")).First().Click();
            comp.Instance.DateRange.End.Should().Be(new DateTime(DateTime.Now.Year, 4, 12));
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
        public void Open_ClickYear_ClickCurrentYear_Click2ndMonth_Click1_Click3_CheckDateRange()
        {
            var comp = OpenPicker();
            comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container > div.mud-picker-year")
                .Where(x => x.TrimmedText().Contains("2022")).First().Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[1].Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-calendar-header").Count.Should().Be(2);
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("1")).First().Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("3")).First().Click();
            comp.Instance.DateRange.End.Should().Be(new DateTime(2022, 2, 3));
        }

        [Test]
        public async Task Open_Programmatically_CheckOpen_Close_Programmatically_CheckClosed()
        {
            var comp = Context.RenderComponent<MudDateRangePicker>();
            Console.WriteLine(comp.Markup + "\n");
            comp.FindAll("div.mud-picker-content").Count.Should().Be(0);
            // open programmatically
            await comp.InvokeAsync(() => comp.Instance.Open());
            Console.WriteLine(comp.Markup);
            comp.FindAll("div.mud-picker-content").Count.Should().Be(1);
            // closing programmatically
            await comp.InvokeAsync(() => comp.Instance.Close());
            comp.FindAll("div.mud-picker-content").Count.Should().Be(0);
        }

        [Test]
        public void SetPickerValue_CheckText()
        {
            var date = DateTime.Now;
            var comp = Context.RenderComponent<MudDateRangePicker>(
                Parameter(nameof(MudDateRangePicker.DateRange), new DateRange(date, date.AddDays(5))));
            // select elements needed for the test
            var picker = comp.Instance;

            var textStart = date.ToShortDateString();
            var textEnd = date.AddDays(5).ToShortDateString();

            picker.Text.Should().Be(RangeConverter<DateTime>.Join(textStart, textEnd));
            var inputs = comp.FindAll("input");
            ((IHtmlInputElement)inputs[0]).Value.Should().Be(textStart);
            ((IHtmlInputElement)inputs[1]).Value.Should().Be(textEnd);
        }

        [Test]
        public void SetPickerToday_CheckSelected()
        {
            var today = DateTime.Now.Date;
            var comp = OpenPicker(Parameter(nameof(MudDateRangePicker.DateRange), new DateRange(today, today)));
            comp.FindAll("button.mud-selected").Count.Should().Be(1);
        }

        [Test]
        public void IsDateDisabledFunc_DisablesCalendarDateButtons()
        {
            Func<DateTime, bool> isDisabledFunc = date => true;
            var comp = OpenPicker(Parameter(nameof(MudDateRangePicker.IsDateDisabledFunc), isDisabledFunc));

            comp.Instance.IsDateDisabledFunc.Should().Be(isDisabledFunc);
            comp.FindAll("button.mud-picker-calendar-day").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().OnlyContain(disabled => disabled == true);
        }

        [Test]
        public void IsDateDisabledFunc_SettingRangeToIncludeADisabledDateYieldsNull()
        {
            var today = DateTime.Today;
            var yesterday = DateTime.Today.Subtract(TimeSpan.FromDays(1));
            var twoDaysAgo = DateTime.Today.Subtract(TimeSpan.FromDays(2));
            var wasEventCallbackCalled = false;

            Func<DateTime, bool> isDisabledFunc = date => date == yesterday;
            var comp = Context.RenderComponent<MudDateRangePicker>(
                Parameter(nameof(MudDateRangePicker.IsDateDisabledFunc), isDisabledFunc),
                EventCallback("DateRangeChanged", (DateRange _) => wasEventCallbackCalled = true)
            );

            comp.SetParam(picker => picker.DateRange, new DateRange(twoDaysAgo, today));

            comp.Instance.DateRange.Should().BeNull();
            wasEventCallbackCalled.Should().BeFalse();
        }

        [Test]
        public void IsDateDisabledFunc_SettingRangeToExcludeADisabledDateYieldsTheRange()
        {
            var today = DateTime.Today;
            var yesterday = DateTime.Today.Subtract(TimeSpan.FromDays(1));
            var twoDaysAgo = DateTime.Today.Subtract(TimeSpan.FromDays(2));
            var wasEventCallbackCalled = false;

            Func<DateTime, bool> isDisabledFunc = date => date == twoDaysAgo;
            var range = new DateRange(yesterday, today);
            var comp = Context.RenderComponent<MudDateRangePicker>(
                Parameter(nameof(MudDateRangePicker.IsDateDisabledFunc), isDisabledFunc),
                EventCallback("DateRangeChanged", (DateRange _) => wasEventCallbackCalled = true)
            );

            comp.SetParam(picker => picker.DateRange, range);

            comp.Instance.DateRange.Should().Be(range);
            wasEventCallbackCalled.Should().BeTrue();
        }

        [Test]
        public void IsDateDisabledFunc_NoDisabledDatesByDefault()
        {
            var comp = OpenPicker();
            comp.FindAll("button.mud-picker-calendar-day").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().OnlyContain(disabled => disabled == false);
        }

        [Test]
        public void InitializeDateRange_DefaultConstructor()
        {
            var range = new DateRange();

            var comp = OpenPicker(Parameter(nameof(MudDateRangePicker.DateRange), range));

            comp.Instance.DateRange.Should().NotBeNull();
            comp.Instance.DateRange.Start.Should().NotBe(default);
            comp.Instance.DateRange.End.Should().NotBe(default);
            comp.Instance.DateRange.Start.Should().BeNull();
            comp.Instance.DateRange.End.Should().BeNull();
        }

        [Test]
        public void InitializeDateRange_AllNullValues()
        {
            var range = new DateRange(null, null);

            var comp = OpenPicker(Parameter(nameof(MudDateRangePicker.DateRange), range));

            comp.Instance.DateRange.Should().NotBeNull();
            comp.Instance.DateRange.Start.Should().NotBe(default);
            comp.Instance.DateRange.End.Should().NotBe(default);
            comp.Instance.DateRange.Start.Should().BeNull();
            comp.Instance.DateRange.End.Should().BeNull();
        }
    }
}
