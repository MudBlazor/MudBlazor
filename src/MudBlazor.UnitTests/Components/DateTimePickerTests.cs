using System.Diagnostics;
using System.Globalization;
using AwesomeAssertions;
using Bunit;
using MudBlazor.UnitTests.TestComponents.DateTimePicker;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
#nullable enable
    [TestFixture]
    public class DateTimePickerTests : BunitTest
    {
        [Test]
        public void Default()
        {
            var comp = Context.Render<MudDateTimePicker>();
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
        /*[Ignore("Unignore for performance measurements, not needed for code coverage")]*/
        public void DatePicker_Render_Performance()
        {
            // warmup
            Context.Render<MudDateTimePicker>();
            // measure
            var watch = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
                Context.Render<MudDateTimePicker>();
            watch.Stop();
            watch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task DateTimePicker_OpenClose_Performance()
        {
            // warmup
            var comp = Context.Render<MudDateTimePicker>();
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
            comp.Find(".mud-picker-calendar-header button.mud-button-month").TrimmedText().Should().Contain(DateTime.Now.ToString("MMMM"));
            comp.Find(".mud-picker-calendar-header button.mud-button-month").TrimmedText().Should().Contain(DateTime.Now.ToString("yyyy"));
            await comp.InvokeAsync(() => picker.GoToDate(DateTime.Parse("2024-06-26")));
            DateTime date = DateTime.Parse("2024-06-26");
            comp.Find(".mud-picker-calendar-header button.mud-button-month").TrimmedText().Should().Contain(date.ToString("MMMM"));
            comp.Find(".mud-picker-calendar-header button.mud-button-month").TrimmedText().Should().Contain(date.ToString("yyyy"));
        }

        [Test]
        public async Task DateTimePicker_TitleDateTimeFormat_Test()
        {
            var comp = OpenPicker(parameters => parameters
                .Add(p => p.TitleDateTimeFormat, "hh yyyy MMMM dddd"));

        }

        [Test]
        public async Task SetPickerValue_CheckDate_SetPickerDate_CheckValue()
        {
            var culture = CultureInfo.CurrentCulture;
            var comp = Context.Render<MudDateTimePicker>();
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.DateTimeFormat, "yyyy-MM-dd HH:mm:ss"));
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Text, DateTime.Parse("2020-10-23 20:30:00").ToString(picker.DateTimeFormat)));
            picker.DateTime.Should().Be(DateTime.Parse("2020-10-23 20:30:00"));
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.DateTime, DateTime.Parse("2020-10-26 12:45:20")));
            picker.Text.Should().Be(DateTime.Parse("2020-10-26 12:45:20").ToString(picker.DateTimeFormat));
        }

        [Test]
        public async Task DateTimePicker_Should_ApplyDateFormat()
        {
            var comp = Context.Render<MudDateTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.DateTimeFormat, "dd/MM/yyyy HH:mm")
                .Add(p => p.Culture, CultureInfo.InvariantCulture)
                .Add(p => p.Text, "23/10/2020 20:30" /*"10/23/2020 20:30:00"*/));
            await Task.Delay(500);
            picker.DateTime.Should().Be(DateTime.Parse("2020-10-23 20:30:00"));
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.DateTime, DateTime.Parse("2020-10-26 12:45:20")));
            picker.Text.Should().Be("26/10/2020 12:45");
        }

        [Test]
        public async Task DatePicker_Should_ApplyDateFormatAfterDate()
        {
            var comp = Context.Render<MudDateTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.DateTimeFormat, "dd/MM/yyyy HH:mm")
                .Add(p => p.Culture, CultureInfo.InvariantCulture) // <-- this makes a huge difference!
                .Add(p => p.DateTime, DateTime.Parse("2020-10-26 15:45:00")));
            picker.DateTime.Should().Be(DateTime.Parse("2020-10-26 15:45:00"));
            picker.Text.Should().Be("26/10/2020 15:45");
        }

        [Test]
        public async Task DatePicker_Should_ApplyCultureDateFormat()
        {
            var comp = Context.Render<MudDateTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);

            var customCulture = new CultureInfo("en-US");
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Culture, customCulture)
                .Add(p => p.DateTimeFormat, "dd MM yyyy HH:mm")
                .Add(p => p.Text, "23 10 2020 23:45"));
            picker.DateTime.Should().Be(DateTime.Parse("2020-10-23 23:45:00"));

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.DateTime, DateTime.Parse("2020-10-26 23:45:00")));
            picker.Text.Should().Be("26 10 2020 23:45");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.DateTimeFormat, "yyyy-MM-dd HH:mm")
                .Add(p => p.Text, "2024-03-13 00:00"));
            picker.DateTime.Should().Be(DateTime.Parse("2024-03-13 00:00"));

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.DateTime, DateTime.Parse("2024-3-16 00:00")));
            picker.Text.Should().Be("2024-03-16 00:00");
        }

        [Test]
        public async Task DatePicker_Should_DateFormatTakesPrecedenceOverCulture()
        {
            var comp = Context.Render<MudDateTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.DateTimeFormat, "dd MM yyyy HH:mm")
                .Add(p => p.Culture, CultureInfo.InvariantCulture) // <-- this makes a huge difference!
                .Add(p => p.DateTime, DateTime.Parse("2020-10-26 15:45:00")));
            picker.DateTime.Should().Be(DateTime.Parse("2020-10-26 15:45:00"));
            picker.Text.Should().Be("26 10 2020 15:45");
        }

        [Test]
        public async Task DatePicker_Should_Clear()
        {
            var culture = CultureInfo.CurrentCulture;
            var comp = Context.Render<MudDateTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateTime.Should().Be(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Clearable, true)
                .Add(p => p.DateTime, DateTime.Parse("2020-10-26 15:45:00")));
            picker.DateTime.Should().Be(DateTime.Parse("2020-10-26 15:45:00"));
            picker.Text.Should().Be(DateTime.Parse("2020-10-26 15:45:00").ToString(picker.DateTimeFormat));
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
            DateTime? date = DateTime.Parse("2024-01-28 10:15:00");
            var comp = Context.Render<MudDateTimePicker>(parameters => parameters
                .Add(p => p.Culture, CultureInfo.InvariantCulture)
                .Add(p => p.DateTimeFormat, "dd/MM/yyyy HH:mm")
                .Add(p => p.DateTime, date)
            );
            var picker = comp.Instance;
            picker.DateTime.Should().Be(DateTime.Parse("2024-01-28 10:15:00"));
            picker.Text.Should().Be(DateTime.Parse("2024-01-28 10:15:00").ToString("dd/MM/yyyy HH:mm"));
        }

        [Test]
        public async Task StringChange_ShouldUpdateValue()
        {
            var culture = CultureInfo.CurrentCulture;
            string dateFormat = $"{culture.DateTimeFormat.ShortDatePattern} {culture.DateTimeFormat.ShortTimePattern}";
            var comp = Context.Render<MudDateTimePicker>(parameters => parameters
                .Add(p => p.Culture, CultureInfo.InvariantCulture)
                .Add(p => p.DateTimeFormat, "dd/MM/yyyy HH:mm")
            );
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Text, "28/01/2024 10:15"));
            comp.Instance.DateTime.Should().Be(DateTime.Parse("2024-01-28 10:15"));
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Text, string.Empty));
            comp.Instance.DateTime.Should().Be(null);
        }

        [Test]
        public void FirstDayOfWeekTest()
        {
            var comp = OpenPicker(parameters => parameters
                .Add(x => x.FirstDayOfWeek, DayOfWeek.Monday));
            var picker = comp.FindComponent<MudDateTimePicker>();
            comp.FindAll("div.mud-picker-calendar-header-day > span")[0].TrimmedText().Should().Be("Mon");
        }

        [Test]
        public async Task ShowWeekNumbers()
        {
            var comp = OpenPicker(parameters => parameters
                .Add(p => p.ShowWeekNumbers, true));
            comp.FindAll(".mud-picker-calendar-week").Count.Should().Be(5 + 2);

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.ShowWeekNumbers, false));
            comp.FindAll(".mud-picker-calendar-week").Count.Should().Be(0);
        }

        public IRenderedComponent<SimpleDateTimePickerTest> OpenPicker(Action<ComponentParameterCollectionBuilder<SimpleDateTimePickerTest>>? parameterBuilder = null)
        {
            IRenderedComponent<SimpleDateTimePickerTest> comp;
            if (parameterBuilder is null)
            {
                comp = Context.Render<SimpleDateTimePickerTest>();
            }
            else
            {
                comp = Context.Render<SimpleDateTimePickerTest>(parameterBuilder);
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
            var comp = OpenPicker(parameters => parameters.Add(p => p.DateOpenTo, OpenTo.Year));
            comp.Instance.DateTime.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public void OpenToYear_ClickYear_CheckMonthsShown()
        {
            var comp = OpenPicker(parameters => parameters.Add(p => p.DateOpenTo, OpenTo.Year));
            comp.Instance.DateTime.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-year")[0].Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public void OpenToYear_ClickYear_CheckMonthsShown_Close_Reopen_CheckYearsShown()
        {
            var comp = OpenPicker(parameters => parameters.Add(p => p.DateOpenTo, OpenTo.Year));
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
            var comp = OpenPicker(parameters => parameters.Add(p => p.DateOpenTo, OpenTo.Month));
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
            var comp = OpenPicker(parameters => parameters.Add(p => p.DateOpenTo, OpenTo.Month));
            // should show years
            comp.FindAll("button.mud-picker-calendar-header-transition")[0].Click();
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
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
            var comp = OpenPicker(parameters => parameters
                .Add(p => p.PickerMonth, DateTime.Parse("2024-02-01"))
                .Add(p => p.IsDateTimeDisabledFunc, (DateTime dateTime) => dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday));
            var picker = comp.FindComponent<MudDateTimePicker>();
            comp.Find("button.mud-button-month").TextContent.Should().Contain("February");
            comp.Find("button.mud-button-month").TextContent.Should().Contain("2024");
            string[] lockedDays = ["28", "3", "4", "10", "11", "17", "18", "24", "25", "2", "3", "9"];
            comp.FindAll("button.mud-picker-calendar-day.mud-day[disabled]").All(x => lockedDays.Contains(x.TextContent)).Should().Be(true);
        }
    }
}
