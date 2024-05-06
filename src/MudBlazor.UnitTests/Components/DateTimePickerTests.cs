#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DateTimePickerTests : BunitTest
    {
        [Test]
        public void Default()
        {
            var comp = Context.RenderComponent<MudDateTimePicker>();
            var picker = comp.Instance;

            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);
            picker.MaxDateTime.Should().Be(null);
            picker.MinDateTime.Should().Be(null);
            picker.DateOpenTo.Should().Be(OpenTo.Date);
            picker.TimeOpenTo.Should().Be(OpenTo.Hours);
            picker.FirstDayOfWeek.Should().Be(DayOfWeek.Sunday);
            picker.StartMonth.Should().Be(null);
            picker.ShowWeekNumbers.Should().BeFalse();
            picker.AutoClose.Should().BeFalse();
            picker.FixYear.Should().Be(null);
            picker.FixMonth.Should().Be(null);
            picker.FixDay.Should().Be(null);
        }

        [Test]
        // [WIP]
        public void DateTimePickerOpenButtonAriaLabel()
        {
            /*var comp = Context.RenderComponent<DateTimePickerValidationTest>();
            var openButton = comp.Find(".mud-input-adornment button");
            openButton.Attributes.GetNamedItem("aria-label")?.Value.Should().Be("Open DateTime Picker");*/
        }

        [Test]
        /*[Ignore("Unignore for performance measurements, not needed for code coverage")]*/
        public void DatePicker_Render_Performance()
        {
            // warmup
            Context.RenderComponent<MudDateTimePicker>();
            // measure
            var watch = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
                Context.RenderComponent<MudDateTimePicker>();
            watch.Stop();
            watch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task DateTimePicker_OpenClose_Performance()
        {
            // warmup
            var comp = Context.RenderComponent<MudDateTimePicker>();
            var datepicker = comp.Instance;
            // measure
            var watch = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
            {
                await comp.InvokeAsync(() => datepicker.OpenAsync());
                await comp.InvokeAsync(() => datepicker.CloseAsync());
            }
            watch.Stop();
            watch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task DateTimePicker_GoToDate_Test()
        {
            var comp = OpenPicker();
            var picker = comp.FindComponent<MudDateTimePicker>().Instance;
            picker.GoToDate();
            comp.Find(".mud-picker-calendar-header button.mud-button-month").TrimmedText().Should().Be(DateTime.Now.ToString("MMMM yyyy"));
            await comp.InvokeAsync(() => picker.GoToDate(DateTime.Parse("2024-06-26")));
            comp.Find(".mud-picker-calendar-header button.mud-button-month").TrimmedText().Should().Be(DateTime.Parse("2024-06-26").ToString("MMMM yyyy"));
        }

        [Test]
        public async Task DateTimePicker_TitleDateTimeFormat_Test()
        {
            var comp = OpenPicker(Parameter("TitleDateTimeFormat", "hh yyyy MMMM dddd"));

        }

        [Test]
        public async Task SetPickerValue_CheckDate_SetPickerDate_CheckValue()
        {
            var culture = CultureInfo.CurrentCulture;
            string dateFormat = $"{culture.DateTimeFormat.ShortDatePattern} {culture.DateTimeFormat.ShortTimePattern}";
            var comp = Context.RenderComponent<MudDateTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);
            comp.SetParam(p => p.Text, DateTime.Parse("2020-10-23 20:30:00").ToString(picker.DateTimeFormat));
            picker.DateTime.Should().Be(DateTime.Parse("2020-10-23 20:30:00"));
            comp.SetParam(p => p.DateTime, DateTime.Parse("2020-10-26 12:45:20"));
            picker.Text.Should().Be(DateTime.Parse("2020-10-26 12:45").ToString(dateFormat));
        }

        [Test]
        public async Task DateTimePicker_Should_ApplyDateFormat()
        {
            var comp = Context.RenderComponent<MudDateTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);
            comp.SetParam(p => p.DateTimeFormat, "dd/MM/yyyy HH:mm");
            comp.SetParam(p => p.Culture, CultureInfo.InvariantCulture);
            comp.SetParam(p => p.Text, "23/10/2020 20:30" /*"10/23/2020 20:30:00"*/);
            await Task.Delay(500);
            picker.DateTime.Should().Be(DateTime.Parse("2020-10-23 20:30:00"));
            comp.SetParam(p => p.DateTime, DateTime.Parse("2020-10-26 12:45:20"));
            picker.Text.Should().Be("26/10/2020 12:45");
        }

        [Test]
        public async Task DatePicker_Should_ApplyDateFormatAfterDate()
        {
            var comp = Context.RenderComponent<MudDateTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);
            comp.SetParam(p => p.DateTimeFormat, "dd/MM/yyyy HH:mm");
            comp.SetParam(p => p.Culture, CultureInfo.InvariantCulture); // <-- this makes a huge difference!
            comp.SetParam(p => p.DateTime, DateTime.Parse("2020-10-26 15:45:00"));
            picker.DateTime.Should().Be(DateTime.Parse("2020-10-26 15:45:00"));
            picker.Text.Should().Be("26/10/2020 15:45");
        }

        [Test]
        public async Task DatePicker_Should_ApplyCultureDateFormat()
        {
            var comp = Context.RenderComponent<MudDateTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);

            var customCulture = new CultureInfo("en-US");
            customCulture.DateTimeFormat.ShortDatePattern.Should().Be("M/d/yyyy");
            customCulture.DateTimeFormat.ShortDatePattern = "dd MM yyyy";
            customCulture.DateTimeFormat.ShortTimePattern = "HH:mm";
            comp.SetParam(p => p.Culture, customCulture);

            comp.SetParam(p => p.Text, "23 10 2020 23:45");
            picker.DateTime.Should().Be(DateTime.Parse("2020-10-23 23:45:00"));
            comp.SetParam(p => p.DateTime, DateTime.Parse("2020-10-26 23:45:00"));
            picker.Text.Should().Be("26 10 2020 23:45");

            customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            comp.SetParam(p => p.Text, "2024-03-13 00:00");
            picker.DateTime.Should().Be(DateTime.Parse("2024-03-13 00:00"));
            comp.SetParam(p => p.DateTime, DateTime.Parse("2024-3-16 00:00"));
            picker.Text.Should().Be("2024-03-16 00:00");
        }

        [Test]
        public async Task DatePicker_Should_DateFormatTakesPrecedenceOverCulture()
        {
            var comp = Context.RenderComponent<MudDateTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);
            comp.SetParam(p => p.DateTimeFormat, "dd MM yyyy HH:mm");
            comp.SetParam(p => p.Culture, CultureInfo.InvariantCulture); // <-- this makes a huge difference!
            comp.SetParam(p => p.DateTime, DateTime.Parse("2020-10-26 15:45:00"));
            picker.DateTime.Should().Be(DateTime.Parse("2020-10-26 15:45:00"));
            picker.Text.Should().Be("26 10 2020 15:45");
        }

        [Test]
        public async Task DatePicker_Should_Clear()
        {
            var culture = CultureInfo.CurrentCulture;
            var comp = Context.RenderComponent<MudDateTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);
            comp.SetParam(p => p.Clearable, true);
            comp.SetParam(p => p.DateTime, DateTime.Parse("2020-10-26 15:45:00"));
            picker.DateTime.Should().Be(DateTime.Parse("2020-10-26 15:45:00"));
            picker.Text.Should().Be(DateTime.Parse("2020-10-26 15:45:00").ToString($"{culture.DateTimeFormat.ShortDatePattern} {culture.DateTimeFormat.ShortTimePattern}"));
            //clear the input
            comp.Find("button").Click();
            //ensure the text and date are reset. Note this is an empty string rather than null due to how the reset works internally
            picker.Text.Should().Be(string.Empty);
            picker.DateTime.Should().Be(null);
        }

        [Test]
        public void Check_Intial_DateTime_Format()
        {
            var culture = CultureInfo.CurrentCulture;
            string dateFormat = $"{culture.DateTimeFormat.ShortDatePattern} {culture.DateTimeFormat.ShortTimePattern}";
            DateTime? date = DateTime.Parse("2024-01-28 10:15:00");
            var comp = Context.RenderComponent<MudDateTimePicker>(parameters => parameters
                .Add(p => p.Culture, CultureInfo.InvariantCulture)
                .Add(p => p.DateTimeFormat, "dd/MM/yyyy HH:mm")
                .Add(p => p.DateTime, date)
            );
            var picker = comp.Instance;
            picker.DateTime.Should().Be(DateTime.Parse("2024-01-28 10:15:00"));
            picker.Text.Should().Be(DateTime.Parse("2024-01-28 10:15:00").ToString("dd/MM/yyyy HH:mm"));
        }

        [Test]
        public void FirstDayOfWeekTest()
        {
            var comp = OpenPicker(Parameter("FirstDayOfWeek", DayOfWeek.Monday));
            var picker = comp.FindComponent<MudDateTimePicker>();
            comp.FindAll("div.mud-picker-calendar-header-day > span")[0].TrimmedText().Should().Be("Mon");
        }

        [Test]
        public void ShowWeekNumbers()
        {
            var comp = Context.RenderComponent<MudDateTimePicker>(parameters => parameters
                .Add(p => p.ShowWeekNumbers, true)
            );
            //comp.
        }

        public IRenderedComponent<SimpleDateTimePickerTest> OpenPicker(params ComponentParameter[] parameters)
        {
            IRenderedComponent<SimpleDateTimePickerTest> comp;
            if (parameters is null or [])
            {
                comp = Context.RenderComponent<SimpleDateTimePickerTest>();
            }
            else
            {
                comp = Context.RenderComponent<SimpleDateTimePickerTest>(parameters);
            }

            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find(".mud-picker-input-button input").Click();
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
        public void OpenToYear_CheckYearsShown()
        {
            var comp = OpenPicker(Parameter("DateOpenTo", OpenTo.Year));
            comp.Instance.DateTime.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public void OpenToYear_ClickYear_CheckMonthsShown()
        {
            var comp = OpenPicker(Parameter("DateOpenTo", OpenTo.Year));
            comp.Instance.DateTime.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-year")[0].Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public void OpenToYear_ClickYear_CheckMonthsShown_Close_Reopen_CheckYearsShown()
        {
            var comp = OpenPicker(Parameter("DateOpenTo", OpenTo.Year));
            comp.Instance.DateTime.Should().BeNull();
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
            var comp = OpenPicker(Parameter("DateOpenTo", OpenTo.Month));
            comp.Instance.DateTime.Should().BeNull();
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
            var comp = OpenPicker(Parameter("DateOpenTo", OpenTo.Month));
            // should show years
            comp.FindAll("button.mud-picker-calendar-header-transition")[0].Click();
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        // [WIP]: OpenToMonth_Select3rdMonth_Select2ndDay_CheckDate
        public void OpenToMonth_Select4thMonth_Select23rdDay_Select15Hour_Select25Minutes_CheckDate()
        {
            var comp = OpenPicker(Parameter("DateOpenTo", OpenTo.Month));
            var picker = comp.FindComponent<MudDateTimePicker>();
            picker.Instance.DateTime.Should().BeNull();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("button.mud-picker-month")[3].Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("23")).First().Click();
            // Click on external dial (15 hours) and then click on 25 minutes
            comp.FindAll("div.mud-hour")[5].Click();
            comp.FindAll("div.mud-minute")[25].Click();
            // clicking outside to close
            comp.Find("div.mud-overlay").Click();
            picker.Instance.DateTime.Should().Be(DateTime.Parse($"{DateTime.Now.Year}-4-23 15:25:00"));
        }

        [Test]
        // [WIP]: Open_ClickCalendarHeader_Click4thMonth_Click23rdDay_CheckDate
        public void Open_ClickCalendarHeader_Click4thMonth23rdDay_Click16Hour23Minute_CheckDateTime()
        {
            var comp = OpenPicker(Parameter("FixYear", DateTime.Now.Year));
            var picker = comp.FindComponent<MudDateTimePicker>();
            comp.Find("button.mud-button-month").Click(); 
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("button.mud-picker-month")[3].Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("23")).First().Click();
            // Click on external dial (16 hours) and then click on 35 minutes
            comp.FindAll("div.mud-hour")[7].Click();
            comp.FindAll("div.mud-minute")[35].Click();
            comp.Find("div.mud-overlay").Click();
            picker.Instance.DateTime.Should().Be(DateTime.Parse($"{DateTime.Now.Year}-4-23 16:35:00"));
        }

        [Test]
        // [WIP]: DatePickerStaticWithPickerActionsDayClick_Test
        public void DateTimePickerStaticWithPickerActionsDayAndTimeClick_Test()
        {
            var comp = Context.RenderComponent<DateTimePickerStaticTest>();
            var picker = comp.FindComponent<MudDateTimePicker>();
            // click the day 23
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("23")).First().Click();
            // Click on external dial (16 hours) and then click on 35 minutes
            comp.FindAll("div.mud-hour")[7].Click();
            comp.FindAll("div.mud-minute")[35].Click();
            picker.Instance.DateTime.Should().Be(DateTime.Today);
            comp.FindAll("button").Where(x => x.TrimmedText().Equals("Ok")).First().Click();
            picker.Instance.DateTime.Should().Be(DateTime.Parse($"{DateTime.Now.Year}-{DateTime.Now.Month}-23 16:35:00"));
        }

        [Test]
        public void DateTimePickerAutoclose_OpenSelectShouldClose_Test()
        {
            var comp = OpenPicker(Parameter("AutoClose", true));
            var picker = comp.FindComponent<MudDateTimePicker>();
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            // click the day 23
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("23")).First().Click();
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            // Click on external dial (16 hours) and then click on 35 minutes
            comp.FindAll("div.mud-hour")[7].Click();
            comp.FindAll("div.mud-minute")[35].Click();
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
        }

        [Test]
        public void DateTimePickerAutoclose_RepeatSelection_ShouldNotClose_Test()
        {
            var comp = OpenPicker(Parameter("AutoClose", true));
            var picker = comp.FindComponent<MudDateTimePicker>();
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            // Click on external dial (16 hours) and then click on 35 minutes
            comp.FindAll("div.mud-hour")[7].Click();
            comp.FindAll("div.mud-minute")[35].Click();
            picker.Instance.DateTime.Should().Be(null);
            // Click on external dial (16 hours) and then click on 33 minutes
            comp.FindAll("div.mud-hour")[7].Click();
            comp.FindAll("div.mud-minute")[33].Click();
            picker.Instance.DateTime.Should().Be(null);
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            // click the day 23
            comp.FindAll("button.mud-picker-calendar-day").Where(x => x.TrimmedText().Equals("23")).First().Click();
            picker.Instance.DateTime.Should().Be(DateTime.Parse($"{DateTime.Now.Year}-{DateTime.Now.Month}-23 16:33:00"));
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
        }

        [Test]
        public void DateTimePicker_ClickOnYear_ShouldShowYears_Test()
        {
            var comp = OpenPicker();
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(0);
            comp.FindAll("div.mud-picker-datepicker-toolbar button.mud-button-year")[0].Click();
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
            comp.Find("button.mud-button-date").Click();
        }

        [Test]
        public void DateTimePicker_SetPickerMonth_Test()
        {
            var comp = OpenPicker([
                    Parameter("PickerMonth", DateTime.Parse("2024-02-01")),
                    Parameter("IsDateTimeDisabledFunc", (DateTime dateTime) => dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                ]);
            var picker = comp.FindComponent<MudDateTimePicker>();
            comp.Find("button.mud-button-month").TextContent.Should().Be("February 2024");
            string[] lockedDays = ["28", "3", "4", "10", "11", "17", "18", "24", "25", "2", "3", "9"];
            comp.FindAll("button.mud-picker-calendar-day.mud-day[disabled]").All(x => lockedDays.Contains(x.TextContent)).Should().Be(true);
        }
    }
}
