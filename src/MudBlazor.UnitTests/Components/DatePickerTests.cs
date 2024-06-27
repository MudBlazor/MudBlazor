using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.DatePicker;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
#nullable enable
    [TestFixture]
    public class DatePickerTests : BunitTest
    {
        [Test]
        public void Default()
        {
            var comp = Context.RenderComponent<MudDatePicker>();
            var picker = comp.Instance;

            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            picker.MaxDate.Should().Be(null);
            picker.MinDate.Should().Be(null);
            picker.OpenTo.Should().Be(OpenTo.Date);
            picker.FirstDayOfWeek.Should().Be(null);
            picker.ClosingDelay.Should().Be(100);
            picker.DisplayMonths.Should().Be(1);
            picker.MaxMonthColumns.Should().Be(null);
            picker.StartMonth.Should().Be(null);
            picker.ShowWeekNumbers.Should().BeFalse();
            picker.AutoClose.Should().BeFalse();
            picker.FixYear.Should().Be(null);
            picker.FixMonth.Should().Be(null);
            picker.FixDay.Should().Be(null);
        }

        [Test]
        public void DatePickerOpenButtonAriaLabel()
        {
            var comp = Context.RenderComponent<DatePickerValidationTest>();
            var openButton = comp.Find(".mud-input-adornment button");
            openButton.Attributes.GetNamedItem("aria-label")?.Value.Should().Be("Open Date Picker");
        }

        [Test]
        public void DatePickerLabelFor()
        {
            var comp = Context.RenderComponent<DatePickerValidationTest>();
            var label = comp.Find(".mud-input-label");
            label.Attributes.GetNamedItem("for")?.Value.Should().Be("datePickerLabelTest");
        }

        [Test]
        [Ignore("Unignore for performance measurements, not needed for code coverage")]
        public void DatePicker_Render_Performance()
        {
            // warmup
            Context.RenderComponent<MudDatePicker>();
            // measure
            var watch = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
            {
                Context.RenderComponent<MudDatePicker>();
            }

            watch.Stop();
            watch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task DatePicker_OpenClose_Performance()
        {
            // warmup
            var comp = Context.RenderComponent<MudDatePicker>();
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
        public void SetPickerValue_CheckDate_SetPickerDate_CheckValue()
        {
            var comp = Context.RenderComponent<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            comp.SetParam(p => p.Text, new DateTime(2020, 10, 23).ToShortDateString());
            picker.Date.Should().Be(new DateTime(2020, 10, 23));
            comp.SetParam(p => p.Date, new DateTime(2020, 10, 26));
            picker.Text.Should().Be(new DateTime(2020, 10, 26).ToShortDateString());
        }

        [Test]
        public void DatePicker_Should_ApplyDateFormat()
        {
            var comp = Context.RenderComponent<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            comp.SetParam(p => p.DateFormat, "dd/MM/yyyy");
            comp.SetParam(p => p.Culture, CultureInfo.InvariantCulture); // <-- this makes a huge difference!
            comp.SetParam(p => p.Text, "23/10/2020");
            picker.Date.Should().Be(new DateTime(2020, 10, 23));
            comp.SetParam(p => p.Date, new DateTime(2020, 10, 26));
            picker.Text.Should().Be("26/10/2020");
        }

        [Test]
        public void DatePicker_Should_ApplyDateFormatAfterDate()
        {
            var comp = Context.RenderComponent<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            comp.SetParam(p => p.DateFormat, "dd/MM/yyyy");
            comp.SetParam(p => p.Culture, CultureInfo.InvariantCulture); // <-- this makes a huge difference!
            comp.SetParam(p => p.Date, new DateTime(2020, 10, 26));
            picker.Date.Should().Be(new DateTime(2020, 10, 26));
            picker.Text.Should().Be("26/10/2020");
        }

        [Test]
        public void DatePicker_Should_ApplyCultureDateFormat()
        {
            var comp = Context.RenderComponent<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);

            var customCulture = new CultureInfo("en-US");
            customCulture.DateTimeFormat.ShortDatePattern.Should().Be("M/d/yyyy");
            customCulture.DateTimeFormat.ShortDatePattern = "dd MM yyyy";
            comp.SetParam(p => p.Culture, customCulture);

            comp.SetParam(p => p.Text, "23 10 2020");
            picker.Date.Should().Be(new DateTime(2020, 10, 23));
            comp.SetParam(p => p.Date, new DateTime(2020, 10, 26));
            picker.Text.Should().Be("26 10 2020");

            customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            comp.SetParam(p => p.Text, "13 10 2020");
            picker.Date.Should().Be(new DateTime(2020, 10, 13));
            comp.SetParam(p => p.Date, new DateTime(2020, 10, 16));
            picker.Text.Should().Be("16 10 2020");
        }

        [Test]
        public void DatePicker_Should_DateFormatTakesPrecedenceOverCulture()
        {
            var comp = Context.RenderComponent<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            comp.SetParam(p => p.DateFormat, "dd MM yyyy");
            comp.SetParam(p => p.Culture, CultureInfo.InvariantCulture); // <-- this makes a huge difference!
            comp.SetParam(p => p.Date, new DateTime(2020, 10, 26));
            picker.Date.Should().Be(new DateTime(2020, 10, 26));
            picker.Text.Should().Be("26 10 2020");
        }

        [Test]
        public void DatePicker_Should_Clear()
        {
            var comp = Context.RenderComponent<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            comp.SetParam(p => p.Clearable, true);
            comp.SetParam(p => p.Date, new DateTime(2020, 10, 26));
            picker.Date.Should().Be(new DateTime(2020, 10, 26));
            picker.Text.Should().Be(new DateTime(2020, 10, 26).ToShortDateString());

            comp.Find("button").Click(); //clear the input

            picker.Text.Should().Be(""); //ensure the text and date are reset. Note this is an empty string rather than null due to how the reset works internally
            picker.Date.Should().Be(null);
        }

        [Test]
        public async Task DataPicker_ShouldClearText_WhenDateSetNull()
        {
            var comp = Context.RenderComponent<MudDatePicker>();

            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);

            var invalid = "INVALID_DATE";
            comp.SetParam(p => p.Text, "INVALID_DATE");

            picker.Date.Should().Be(null);
            picker.Text.Should().Be(invalid);

            await Task.Delay(150);

            comp.SetParam(p => p.Date, null);

            picker.Date.Should().Be(null);
            picker.Text.Should().Be(null);
        }


        [Test]
        public void DataPicker_ShouldDeBounceSetDate_WhenDateSetToTheSameValueQuickly()
        {
            var comp = Context.RenderComponent<MudDatePicker>();

            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);

            var invalid = "INVALID_DATE";
            comp.SetParam(p => p.Text, "INVALID_DATE");

            picker.Date.Should().Be(null);
            picker.Text.Should().Be(invalid);

            comp.SetParam(p => p.Date, null);

            picker.Date.Should().Be(null);
            picker.Text.Should().Be(invalid);
        }

        [Test]
        public void DataPicker_ShouldDisplayError_WhenTextSetToInvalidValue()
        {
            var comp = Context.RenderComponent<MudDatePicker>();

            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            comp.SetParam(p => p.Text, "INVALID_DATE");

            picker.Error.Should().BeTrue();
        }

        [Test]
        public void Check_Initial_Date_Format()
        {
            DateTime? date = new DateTime(2021, 1, 13);
            var comp = Context.RenderComponent<MudDatePicker>(parameters => parameters
                .Add(p => p.Culture, CultureInfo.InvariantCulture)
                .Add(p => p.DateFormat, "dd/MM/yyyy")
                .Add(p => p.Date, date)
            );
            var picker = comp.Instance;
            picker.Date.Should().Be(new DateTime(2021, 1, 13));
            picker.Text.Should().Be("13/01/2021");
        }

        private IRenderedComponent<SimpleMudDatePickerTest> OpenPicker(ComponentParameter parameter)
        {
            return OpenPicker(new[] { parameter });
        }

        private IRenderedComponent<SimpleMudDatePickerTest> OpenPicker(ComponentParameter[]? parameters = null)
        {
            IRenderedComponent<SimpleMudDatePickerTest> comp;
            if (parameters is null)
            {
                comp = Context.RenderComponent<SimpleMudDatePickerTest>();
            }
            else
            {
                comp = Context.RenderComponent<SimpleMudDatePickerTest>(parameters);
            }

            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to open menu
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
            // should not be open anymore
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
        }

        [Test]
        public void Open_CloseBySelectingADate_CheckClosed()
        {
            var comp = OpenPicker();
            // clicking a day button to select a date and close
            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("23")).Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0), TimeSpan.FromSeconds(5));
            comp.Instance.Date.Should().NotBeNull();
        }

        [Test]
        public void Open_CloseBySelectingADate_CheckClosed_Check_DateChangedCount()
        {
            var eventCount = 0;
            DateTime? returnDate = null;
            var comp = OpenPicker(EventCallback(nameof(MudDatePicker.DateChanged), (DateTime? date) => { eventCount++; returnDate = date; }));
            // clicking a day button to select a date and close
            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("23")).Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0), TimeSpan.FromSeconds(5));
            comp.Instance.Date.Should().NotBeNull();
            eventCount.Should().Be(1);
            var now = DateTime.Now;
            returnDate.Should().Be(new DateTime(now.Year, now.Month, 23));
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
        public void OpenToYear_ClickYear_CheckMonthsShown()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Year));
            comp.Instance.Date.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-year")[0].Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public void OpenToYear_ClickYear_CheckMonthsShown_Close_Reopen_CheckYearsShown()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Year));
            comp.Instance.Date.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-year")[0].Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            // clicking outside to close
            comp.Find("div.mud-overlay").Click();
            // should not be open anymore
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            comp.Find("input").Click();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }


        [Test]
        public void OpenToMonth_CheckMonthsShown()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Month));
            comp.Instance.Date.Should().BeNull();
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
            var comp = OpenPicker(Parameter(nameof(MudDatePicker.OpenTo), OpenTo.Month));
            // should show years
            comp.FindAll("button.mud-picker-calendar-header-transition")[0].Click();
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public void OpenToMonth_Select3rdMonth_Select2ndDay_CheckDate()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Month));
            comp.Instance.Date.Should().BeNull();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[2].Click();
            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("2")).Click();
            comp.Instance.Date?.Date.Should().Be(new DateTime(DateTime.Now.Year, 3, 2));
        }

        private IRenderedComponent<SimpleMudDatePickerTest> OpenTo12thMonth()
        {
            var comp = OpenPicker(Parameter("PickerMonth", new DateTime(DateTime.Now.Year, 12, 01)));
            comp.Instance.PickerMonth?.Month.Should().Be(12);
            return comp;
        }

        [Test]
        public void Open_ClickCalendarHeader_Click4thMonth_Click23rdDay_CheckDate()
        {
            var comp = OpenPicker();
            comp.Find("button.mud-picker-calendar-header-transition").Click();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[3].Click();
            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("23")).Click();
            comp.Instance.Date?.Date.Should().Be(new DateTime(DateTime.Now.Year, 4, 23));
        }

        [Test]
        public void DatePickerStaticWithPickerActionsDayClick_Test()
        {
            var comp = Context.RenderComponent<DatePickerStaticTest>();
            var picker = comp.FindComponent<MudDatePicker>();
            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("23")).Click();
            picker.Instance.Date.Should().Be(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 23));
        }

        [Test]
        public void OpenTo12thMonth_NavigateBack_CheckMonth()
        {
            var comp = OpenTo12thMonth();
            var picker = comp.Instance;
            comp.Find("div.mud-picker-calendar-header-switch > button:nth-child(1)").Click();
            picker.PickerMonth?.Month.Should().Be(11);
            picker.PickerMonth?.Year.Should().Be(DateTime.Now.Year);
        }

        [Test]
        public void OpenTo12thMonth_NavigateForward_CheckYear()
        {
            var comp = OpenTo12thMonth();
            var picker = comp.Instance;
            comp.Find("div.mud-picker-calendar-header-switch > button:nth-child(3)").Click();
            picker.PickerMonth?.Month.Should().Be(1);
            picker.PickerMonth?.Year.Should().Be(DateTime.Now.Year + 1);
        }

        [Test]
        public void Open_ClickYear_ClickCurrentYear_Click2ndMonth_Click1_CheckDate()
        {
            var comp = OpenPicker();
            comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container > div.mud-picker-year").First(x => x.TrimmedText().Contains("2022")).Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[1].Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-calendar-header").Count.Should().Be(1);
            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("1")).Click();
            comp.Instance.Date?.Date.Should().Be(new DateTime(2022, 2, 1));
        }

        [Test]
        public void Open_FixYear_Click2ndMonth_Click3_CheckDate()
        {
            var comp = OpenPicker(ComponentParameter.CreateParameter("FixYear", 2021));
            comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(0);
            comp.Find("div.mud-picker-calendar-container > .mud-picker-calendar-header > .mud-picker-calendar-header-switch > .mud-button-month").Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[1].Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-calendar-header").Count.Should().Be(1);
            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("3")).Click();
            comp.Instance.Date?.Date.Should().Be(new DateTime(2021, 2, 3));
        }

        [Test]
        public void Open_FixDay_ClickYear_Click2ndMonth_CheckDate()
        {
            var comp = OpenPicker(ComponentParameter.CreateParameter("FixDay", 1));
            comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container > div.mud-picker-year").First(x => x.TrimmedText().Contains("2022")).Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[1].Click();
            comp.Instance.Date?.Date.Should().Be(new DateTime(2022, 2, 1));
        }

        [Test]
        public void Open_FixMonth_ClickYear_Click3_CheckDate()
        {
            var comp = OpenPicker(ComponentParameter.CreateParameter("FixMonth", 1));
            comp.FindAll("div.mud-picker-calendar-container > .mud-picker-calendar-header > .mud-picker-calendar-header-switch > .mud-button-month").Count().Should().Be(0);
            comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container > div.mud-picker-year").First(x => x.TrimmedText().Contains("2022")).Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(0);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-calendar-header").Count.Should().Be(1);
            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("3")).Click();
            comp.Instance.Date?.Date.Should().Be(new DateTime(2022, 1, 3));
        }

        [Test]
        public void Open_FixYear_FixMonth_Click3_CheckDate()
        {
            var comp = OpenPicker(new[] { ComponentParameter.CreateParameter("FixMonth", 1), ComponentParameter.CreateParameter("FixYear", 2022) });
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(0);
            comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(0);
            comp.FindAll("div.mud-picker-calendar-container > .mud-picker-calendar-header > .mud-picker-calendar-header-switch > .mud-button-month").Count.Should().Be(0);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-calendar-header").Count.Should().Be(1);
            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("3")).Click();
            comp.Instance.Date?.Date.Should().Be(new DateTime(2022, 1, 3));
        }

        [Test]
        public void Open_FixMonth_FixDay_ClickYear2022_CheckDate()
        {
            var comp = OpenPicker(new[] { ComponentParameter.CreateParameter("OpenTo", OpenTo.Year), ComponentParameter.CreateParameter("FixMonth", 1), ComponentParameter.CreateParameter("FixDay", 1) });
            comp.FindAll("div.mud-picker-calendar-container > .mud-picker-calendar-header > .mud-picker-calendar-header-switch > .mud-button-month").Count.Should().Be(0);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container > div.mud-picker-year").First(x => x.TrimmedText().Contains("2022")).Click();
            comp.Instance.Date?.Date.Should().Be(new DateTime(2022, 1, 1));
        }

        [Test]
        public void Open_FixYear_FixDay_Click3rdMonth_CheckDate()
        {
            var comp = OpenPicker(new[] { ComponentParameter.CreateParameter("OpenTo", OpenTo.Month), ComponentParameter.CreateParameter("FixYear", 2022), ComponentParameter.CreateParameter("FixDay", 1) });
            comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(0);
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[2].Click();
            comp.Instance.Date?.Date.Should().Be(new DateTime(2022, 3, 1));
        }

        [Test]
        public void Open_FixDay_CheckOpenTo()
        {
            var comp = OpenPicker(new[] { Parameter(nameof(MudDatePicker.FixDay), 1) });
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public void Open_FixMonth_FixDay_CheckOpenTo()
        {
            var comp = OpenPicker(new[] { Parameter(nameof(MudDatePicker.FixMonth), 1), Parameter(nameof(MudDatePicker.FixDay), 1) });
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public async Task Open_Programmatically_CheckOpen_Close_Programmatically_CheckClosed()
        {
            var comp = Context.RenderComponent<SimpleMudDatePickerTest>();
            comp.FindAll("div.mud-picker-content").Count.Should().Be(0);
            // open programmatically
            await comp.InvokeAsync(comp.Instance.Open);
            comp.FindAll("div.mud-picker-content").Count.Should().Be(1);
            // closing programmatically
            await comp.InvokeAsync(comp.Instance.Close);
            comp.FindAll("div.mud-picker-content").Count.Should().Be(0);
        }

        [Test]
        public async Task PersianCalendar()
        {
            var cal = new PersianCalendar();
            var date = new DateTime(2021, 02, 14);
            cal.GetYear(date).Should().Be(1399);
            cal.GetMonth(date).Should().Be(11);
            cal.GetDayOfMonth(date).Should().Be(26);
            // ---------------------------------------------------------------
            var comp = Context.RenderComponent<PersianDatePickerTest>();
            var datePicker = comp.FindComponent<MudDatePicker>();
            await comp.InvokeAsync(() => datePicker.Instance.OpenAsync());

            datePicker.Instance.Text.Should().Be("1399/11/26");
        }

        [Test]
        public async Task PersianCalendarTest_GoToDate()
        {
            var cal = new PersianCalendar();
            var comp = Context.RenderComponent<PersianDatePickerTest>();
            var datePicker = comp.FindComponent<MudDatePicker>().Instance;
            await comp.InvokeAsync(() => datePicker.OpenAsync());
            datePicker.Text.Should().Be("1399/11/26");
            await comp.InvokeAsync(() => datePicker.GoToDate(new DateTime(2024, 5, 8)));
            comp.WaitForAssertion(() => datePicker.Text.Should().Be("1403/02/19"));
            var button = comp
                .FindAll(".mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-picker-calendar-day.mud-day")
                .Single(x => x.GetAttribute("style") == "--day-id: 1;");
            button.TextContent.Should().Be("1");
        }

        [Test]
        public void SetPickerValue_CheckText()
        {
            var date = DateTime.Now;
            var comp = Context.RenderComponent<MudDatePicker>(
                Parameter(nameof(MudDatePicker.Date), date));
            // select elements needed for the test
            var picker = comp.Instance;

            var text = date.ToShortDateString();

            picker.Text.Should().Be(text);
            ((IHtmlInputElement)comp.FindAll("input")[0]).Value.Should().Be(text);
        }

        [Test]
        public void IsDateDisabledFunc_DisablesCalendarDateButtons()
        {
            Func<DateTime, bool> isDisabledFunc = _ => true;
            var comp = OpenPicker(Parameter(nameof(MudDatePicker.IsDateDisabledFunc), isDisabledFunc));

            comp.Instance.IsDateDisabledFunc.Should().Be(isDisabledFunc);
            comp.FindAll("button.mud-picker-calendar-day").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().OnlyContain(disabled => disabled);
        }

        [Test]
        public void IsDateDisabledFunc_DisablesCalendarMonthButtons()
        {
            Func<DateTime, bool> isDisabledFunc = _ => true;
            var comp = OpenPicker(new[]
            {
                Parameter(nameof(MudDatePicker.IsDateDisabledFunc), isDisabledFunc),
                Parameter(nameof(MudDatePicker.OpenTo), OpenTo.Month),
                Parameter(nameof(MudDatePicker.FixDay), 1)
            });

            comp.Instance.IsDateDisabledFunc.Should().Be(isDisabledFunc);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().OnlyContain(disabled => disabled);

            // None should be selected
            comp.FindAll("button.mud-picker-month > .mud-typography").Select(
                text => ((IHtmlElement)text).ClassList.Any(cls => cls == "mud-picker-month-select" || cls == "mud-primary-text"))
                .Should().OnlyContain(selected => selected == false);
        }

        [Test]
        public void DisableCalendarMonthButtonsWhenFixDayOutOfRange()
        {
            var comp = OpenPicker(new[]
            {
                Parameter(nameof(MudDatePicker.OpenTo), OpenTo.Month),
                Parameter(nameof(MudDatePicker.FixDay), 31)
            });

            comp
                .FindAll("button.mud-picker-month")
                .Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should()
                // Only months with 31 days not disabled
                .BeEquivalentTo(new[]
                    {
                        false,
                        true,
                        false,
                        true,
                        false,
                        true,
                        false,
                        false,
                        true,
                        false,
                        true,
                        false
                    },
                    options => options.WithStrictOrdering()
                );
        }

        [Test]
        public void IsDateDisabledFunc_DoesNotHaveEffectOnMonthsIfDayNotFixed()
        {
            Func<DateTime, bool> isDisabledFunc = _ => true;
            var comp = OpenPicker(new[]
            {
                Parameter(nameof(MudDatePicker.IsDateDisabledFunc), isDisabledFunc),
                Parameter(nameof(MudDatePicker.OpenTo), OpenTo.Month)
            });

            comp.Instance.IsDateDisabledFunc.Should().Be(isDisabledFunc);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().OnlyContain(disabled => disabled == false);
        }

        [Test]
        public void IsDateDisabledFunc_DoesNotHaveEffectOnMonthsIfFuncReturnsFalse()
        {
            Func<DateTime, bool> isDisabledFunc = _ => false;
            var comp = OpenPicker(new[]
            {
                Parameter(nameof(MudDatePicker.IsDateDisabledFunc), isDisabledFunc),
                Parameter(nameof(MudDatePicker.OpenTo), OpenTo.Month),
                Parameter(nameof(MudDatePicker.FixDay), 1)
            });

            comp.Instance.IsDateDisabledFunc.Should().Be(isDisabledFunc);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().OnlyContain(disabled => disabled == false);
        }

        [TestCase(10, 8, 2, 2)]
        [TestCase(10, 9, 2, 2)]
        [TestCase(10, 10, 2, 1)]
        [TestCase(10, 11, 2, 1)]
        public void MinDateEffectOnDisablingMonthsIfDayFixed(int minDatesDay, int fixedDay, int month, int disabledOnes)
        {
            var currentDate = DateTime.Now;
            var minDate = new DateTime(currentDate.Year, month, minDatesDay);
            var comp = OpenPicker(new[]
            {
                Parameter(nameof(MudDatePicker.MinDate), minDate),
                Parameter(nameof(MudDatePicker.OpenTo), OpenTo.Month),
                Parameter(nameof(MudDatePicker.FixDay), fixedDay),
            });

            var expectedResult = new bool[12];
            for (var i = 0; i < disabledOnes; ++i) expectedResult[i] = true;

            comp.Instance.MinDate.Should().Be(minDate);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().ContainInConsecutiveOrder(expectedResult);
        }

        [TestCase(10, 9, 11, 1)]
        [TestCase(10, 10, 11, 1)]
        [TestCase(10, 11, 11, 2)]
        [TestCase(10, 12, 11, 2)]
        public void MaxDateEffectOnDisablingMonthsIfDayFixed(int maxDatesDay, int fixedDay,
            int month, int disabledOnes)
        {
            var currentDate = DateTime.Now;
            var maxDate = new DateTime(currentDate.Year, month, maxDatesDay);
            var comp = OpenPicker(new[]
            {
                Parameter(nameof(MudDatePicker.MaxDate), maxDate),
                Parameter(nameof(MudDatePicker.OpenTo), OpenTo.Month),
                Parameter(nameof(MudDatePicker.FixDay), fixedDay),
            });

            var expectedResult = new bool[12];
            for (var i = 0; i < disabledOnes; ++i) expectedResult[11 - i] = true;

            comp.Instance.MaxDate.Should().Be(maxDate);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().ContainInConsecutiveOrder(expectedResult);
        }

        [TestCase(30, 3, 2)]
        [TestCase(31, 3, 2)]
        [TestCase(1, 4, 3)]
        [TestCase(2, 4, 3)]
        public void MinDateEffectOnDisablingMonthsIfDayNotFixed(int minDatesDay, int month, int disabledOnes)
        {
            var currentYear = DateTime.Now.Year;
            var minDate = new DateTime(currentYear, month, minDatesDay);
            var comp = OpenPicker(new[]
            {
                Parameter(nameof(MudDatePicker.MinDate), minDate),
                Parameter(nameof(MudDatePicker.OpenTo), OpenTo.Month),
            });

            var expectedResult = new bool[12];
            for (var i = 0; i < disabledOnes; ++i) expectedResult[i] = true;

            comp.Instance.MinDate.Should().Be(minDate);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().ContainInConsecutiveOrder(expectedResult);
        }

        [TestCase(1, 10, 2)]
        [TestCase(2, 10, 2)]
        [TestCase(30, 9, 3)]
        [TestCase(29, 9, 3)]
        public void MaxDateEffectOnDisablingMonthsIfDayNotFixed(int maxDatesDay, int month, int disabledOnes)
        {
            var currentYear = DateTime.Now.Year;
            var maxDate = new DateTime(currentYear, month, maxDatesDay);
            var comp = OpenPicker(new[]
            {
                Parameter(nameof(MudDatePicker.MaxDate), maxDate),
                Parameter(nameof(MudDatePicker.OpenTo), OpenTo.Month),
            });

            var expectedResult = new bool[12];
            for (var i = 0; i < disabledOnes; ++i) expectedResult[11 - i] = true;

            comp.Instance.MaxDate.Should().Be(maxDate);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().ContainInConsecutiveOrder(expectedResult);
        }

        [Test]
        public void IsDateDisabledFunc_SettingDateToADisabledDateYieldsNull()
        {
            var wasEventCallbackCalled = false;
            Func<DateTime, bool> isDisabledFunc = _ => true;
            var comp = Context.RenderComponent<MudDatePicker>(
                Parameter(nameof(MudDatePicker.IsDateDisabledFunc), isDisabledFunc),
                EventCallback("DateChanged", (DateTime? _) => wasEventCallbackCalled = true)
            );

            comp.SetParam(picker => picker.Date, DateTime.Now);

            comp.Instance.Date.Should().BeNull();
            wasEventCallbackCalled.Should().BeFalse();
        }

        [Test]
        public void IsDateDisabledFunc_SettingDateToAnEnabledDateYieldsTheDate()
        {
            var wasEventCallbackCalled = false;
            var today = DateTime.Today;
            Func<DateTime, bool> isDisabledFunc = date => date < today;
            var comp = Context.RenderComponent<MudDatePicker>(
                Parameter(nameof(MudDatePicker.IsDateDisabledFunc), isDisabledFunc),
                EventCallback("DateChanged", (DateTime? _) => wasEventCallbackCalled = true)
            );

            comp.SetParam(picker => picker.Date, today);

            comp.Instance.Date.Should().Be(today);
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
        //mud-button-root added for graying out and making buttons not clickable if month is disabled
        public void MonthButtons_ButtonRootClassPresent()
        {
            var comp = OpenPicker(Parameter(nameof(MudDatePicker.FixDay), 1));
            var monthsCount = 12;

            comp.FindAll("button.mud-picker-month").Select(button =>
                button.ClassName?.Contains("mud-button-root"))
                .Should().HaveCount(monthsCount);
        }

        [Test]
        public void AdditionalDateClassesFunc_ClassIsAdded()
        {
            Func<DateTime, string> additionalDateClassesFunc = _ => "__addedtestclass__";

            var comp = OpenPicker(Parameter(nameof(MudDatePicker.AdditionalDateClassesFunc), additionalDateClassesFunc));

            var daysCount = comp.FindAll("button.mud-picker-calendar-day")
                                .Select(button => (IHtmlButtonElement)button)
                                .Count();

            comp.FindAll("button.mud-picker-calendar-day")
                .Where(button => button.ClassName is not null && button.ClassName.Contains("__addedtestclass__"))
                .Should().HaveCount(daysCount);
        }

        public async Task CheckAutoCloseDatePickerTest()
        {
            // Define a date for comparison
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            // Get access to the datepicker of the instance
            var comp = Context.RenderComponent<AutoCompleteDatePickerTest>();
            var datePicker = comp.FindComponent<MudDatePicker>();

            // Open the datepicker
            await comp.InvokeAsync(datePicker.Instance.OpenAsync);

            // Clicking a day button to select a date
            // It must be a different day than the day of now!
            // So the test is working when the day is 20
            if (now.Day != 20)
            {
                comp
                    .FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("20")).Click();
            }
            else
            {
                comp
                    .FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("19")).Click();
            }

            // Check that the date should remain the same because autoclose is false
            // and there are actions which are defined
            datePicker.Instance.Date.Should().Be(now);

            // Close the datepicker without submitting the date
            // The date of the datepicker remains equal to now
            await comp.InvokeAsync(() => datePicker.Instance.CloseAsync(false));

            await comp.InvokeAsync(() => datePicker.Instance.OpenAsync());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover").Count.Should().Be(1));

            await comp.InvokeAsync(() => datePicker.Instance.ClearAsync());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover").Count.Should().Be(1));
            await comp.InvokeAsync(() => datePicker.Instance.CloseAsync(false));

            // Change the value of autoclose
            datePicker.SetParam(parameter => parameter.AutoClose, true);

            // Open the datepicker
            await comp.InvokeAsync(() => datePicker.Instance.OpenAsync());

            // Clicking a day button to select a date
            if (now.Day != 20)
            {
                comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("20")).Click();
            }
            else
            {
                comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("19")).Click();
            }

            // Check that the date should be equal to the new date 19 or 20
            if (now.Day != 20)
            {
                datePicker.Instance.Date.Should().Be(new DateTime(now.Year, now.Month, 20));
            }
            else
            {
                datePicker.Instance.Date.Should().Be(new DateTime(now.Year, now.Month, 19));
            }

            await comp.InvokeAsync(() => datePicker.Instance.OpenAsync());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover").Count.Should().Be(1));

            await comp.InvokeAsync(() => datePicker.Instance.ClearAsync());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover").Count.Should().Be(0));
        }

        [Test]
        public async Task CheckReadOnlyTest()
        {
            // Define a date for comparison
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            // Get access to the datepicker of the instance
            var comp = Context.RenderComponent<SimpleMudDatePickerTest>();
            var picker = comp.Instance;

            // Open the datepicker
            await picker.Open();

            // Clicking a day button to select a date
            // It must be a different day than the day of now!
            // So the test is working when the day is 20
            if (now.Day != 20)
            {
                comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("20")).Click();
            }
            else
            {
                comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("19")).Click();
            }

            // Close the datepicker
            await picker.Close();

            // Check that the date should be equal to the new date 19 or 20
            if (now.Day != 20)
            {
                picker.Date.Should().Be(new DateTime(now.Year, now.Month, 20));
            }
            else
            {
                picker.Date.Should().Be(new DateTime(now.Year, now.Month, 19));
            }

            // Change the value of readonly and update the value of now
            if (picker.Date is not null)
            {
                now = picker.Date.Value;
            }

            comp.SetParametersAndRender(p => p.Add(x => x.Readonly, true));

            // Open the datepicker
            await picker.Open();


            // Clicking a day button to select a date
            if (now.Day != 21)
            {
                comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("22")).Click();
            }
            else
            {
                comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("21")).Click();
            }

            // Close the datepicker
            await picker.Close();

            // Check that the date should remain the same because readonly is true
            picker.Date.Should().Be(now);
        }

        [Test]
        public async Task CheckDateTimeMinValueTest()
        {
            // Get access to the datepicker of the instance
            var comp = Context.RenderComponent<DateTimeMinValueDatePickerTest>();
            var datePicker = comp.FindComponent<MudDatePicker>();

            // Open the datepicker
            await comp.InvokeAsync(() => datePicker.Instance.OpenAsync());

            // An error should be raised if the datepicker could not be not opened and the days could not generated
            // It means that there would be an exception!
            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("1")).Click();
        }

        /// <summary>
        /// Tests if all buttons have type="button" to prevent accidental form submits.
        /// </summary>
        /// <param name="navigateToMonthSelection">If true navigates to the month selection page.</param>
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void CheckButtonTypeTest(bool navigateToMonthSelection)
        {
            var dateComp = Context.RenderComponent<MudDatePicker>(p =>
            p.Add(x => x.PickerVariant, PickerVariant.Dialog));

            //open picker
            dateComp.Find(".mud-picker input").Click();

            //navigate to month selection
            if (navigateToMonthSelection)
            {
                dateComp.Find(".mud-picker button.mud-picker-calendar-header-transition").Click();
            }

            var buttons = dateComp.FindAll(".mud-picker button");
            //expected values
            foreach (var button in buttons)
            {
                button.ToMarkup().Contains("type=\"button\"").Should().BeTrue();
            }
        }

        [Test]
        public async Task DatePickerTest_Editable()
        {
            var comp = Context.RenderComponent<SimpleMudDatePickerTest>();

            var cultureInfo = new CultureInfo("en-US");

            var datePickerComponent = comp.FindComponent<MudDatePicker>();
            datePickerComponent.SetParam(parameter => parameter.Editable, true);
            datePickerComponent.SetParam(parameter => parameter.Culture, cultureInfo);

            var datePicker = datePickerComponent.Instance;

            await comp.InvokeAsync(() => comp.Find("input").Change("10/10/2020"));
            comp.WaitForAssertion(() => datePicker.Date.Should().Be(new DateTime(2020, 10, 10)));
            comp.WaitForAssertion(() => datePicker.PickerMonth.Should().Be(null));

            await comp.InvokeAsync(datePicker.OpenAsync);
            comp.WaitForAssertion(() => datePicker.PickerMonth.Should().Be(new DateTime(2020, 10, 01)));
        }

        [Test]
        public async Task DatePickerTest_KeyboardNavigation()
        {
            var comp = Context.RenderComponent<SimpleMudDatePickerTest>();
            var datePickerComponent = comp.FindComponent<MudDatePicker>();
            var datePicker = datePickerComponent.Instance;

            await comp.InvokeAsync(() => datePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => datePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => datePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => datePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => datePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", AltKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => datePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", AltKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => datePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => datePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "Tab", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            datePickerComponent.SetParam(parameter => parameter.Disabled, true);

            await comp.InvokeAsync(() => datePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => datePicker.ToggleOpenAsync());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => datePicker.ToggleOpenAsync());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => datePicker.ToggleStateAsync());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            datePickerComponent.SetParam(parameter => parameter.Disabled, false);

            await comp.InvokeAsync(() => datePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(datePicker.ToggleStateAsync);
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
        }

        [Test]
        public async Task DatePickerTest_GoToDate()
        {
            var comp = Context.RenderComponent<SimpleMudDatePickerTest>();

            var datePicker = comp.FindComponent<MudDatePicker>().Instance;

            await comp.InvokeAsync(() => datePicker.GoToDate(new DateTime(2022, 03, 20)));
            comp.WaitForAssertion(() => datePicker.Date.Should().Be(new DateTime(2022, 03, 20)));

            await comp.InvokeAsync(() => datePicker.GoToDate(new DateTime(2023, 04, 21), false));
            comp.WaitForAssertion(() => datePicker.Date.Should().Be(new DateTime(2022, 03, 20)));

            await comp.InvokeAsync(() => datePicker.GoToDate(new DateTime(2023, 04, 21)));
            comp.WaitForAssertion(() => datePicker.Date.Should().Be(new DateTime(2023, 04, 21)));

            await comp.InvokeAsync(datePicker.GoToDate);
            comp.WaitForAssertion(() => datePicker.Date.Should().Be(new DateTime(2023, 04, 21)));
        }

        [Test]
        public async Task DatePickerTest_CheckIfMonthsAreDisabled()
        {
            var comp = Context.RenderComponent<SimpleMudDatePickerTest>();
            var datePickerComponent = comp.FindComponent<MudDatePicker>();
            var datePicker = datePickerComponent.Instance;

            datePickerComponent.SetParam(parameter => parameter.MinDate, DateTime.Now.AddDays(-1));
            datePickerComponent.SetParam(parameter => parameter.MaxDate, DateTime.Now.AddDays(1));

            // Open the datepicker
            await comp.InvokeAsync(datePicker.OpenAsync);

            comp.Find("button.mud-button-month").Click();
            comp.WaitForAssertion(() => comp.FindAll("button.mud-picker-month").Any(x => x.IsDisabled()).Should().Be(true));

            comp.FindAll("button.mud-picker-month").First(x => x.IsDisabled()).Click();

            var months = comp.FindAll("button.mud-picker-month");
            months.Should().NotBeNull();
            comp.Instance.Date.Should().BeNull();
        }

        [Test]
        public async Task OnPointerOver_ShouldCallJavaScriptFunction()
        {
            var comp = OpenPicker();

            var button = comp
                .FindAll(".mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-picker-calendar-day.mud-day")
                .Single(x => x.GetAttribute("style") == "--day-id: 5;");

            await button.PointerOverAsync(new());

            Context.JSInterop.VerifyInvoke("mudWindow.updateStyleProperty", 1);
            Context.JSInterop.Invocations["mudWindow.updateStyleProperty"].Single()
                .Arguments
                .Should()
                .HaveCount(3)
                .And
                .HaveElementAt(1, "--selected-day")
                .And
                .HaveElementAt(2, 5);
        }

        [Test]
        public void DatePicker_ImmediateText_Should_Callback_TextChanged()
        {
            string? changedText = null;

            var comp = Context.RenderComponent<MudDatePicker>(EventCallback<string>("TextChanged", x => changedText = x));

            comp.SetParam(x => x.Editable, true);
            comp.SetParam(x => x.ImmediateText, true);

            // This will make the input focused!
            comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "9", Type = "keydown" });

            // Simulate user input
            comp.Find("input").Input("22");

            changedText.Should().Be("22");

            // Set ImmediateText to false
            comp.SetParam(x => x.ImmediateText, false);

            // Simulate user input
            comp.Find("input").Input("33");

            // changed_text should not be updated since ImmediateText was false
            changedText.Should().Be("22");

            // Set ImmediateText to true
            comp.SetParam(x => x.ImmediateText, true);

            // Simulate user input
            comp.Find("input").Input("44");

            //changed_text should be updated
            changedText.Should().Be("44");

            // Set Editable to false.
            // ImmediateText should only work if Editable is also true.
            comp.SetParam(x => x.Editable, false);

            // Simulate user input
            comp.Find("input").Input("55");

            //changed_text should not be updated
            changedText.Should().Be("44");
        }

        [Test]
        public void OldDateWithDefinedKind_SetValue_KindUnchanged()
        {
            var comp = Context.RenderComponent<MudDatePicker>();
            var picker = comp.Instance;
            var oldDate = DateTime.Now;
            var newDate = oldDate.AddDays(1);
            comp.SetParam(p => p.Date, oldDate);

            comp.SetParam(p => p.Text, newDate.ToShortDateString());

            picker.Date.Should().NotBeNull();
            picker.Date!.Value.Kind.Should().Be(oldDate.Kind);
        }

        [Test]
        public void Display_SelectedDate_WhenWrapped()
        {
            var comp = Context.RenderComponent<WrappedDatePickerTest>();

            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            comp.Find(".mud-input-adornment button").Click();
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);

            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("15")).Click();

            ((IHtmlInputElement)comp.FindAll("input")[0]).Value.Should().Be(comp.Instance.Picker.Text);
        }

        /// <summary>
        /// A date picker with a label should auto-generate an id and use that id on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void DatePickerWithLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.RenderComponent<MudDatePicker>(parameters =>
                parameters.Add(p => p.Label, "Test Label"));

            comp.Find("input").Id.Should().NotBeNullOrEmpty();
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(comp.Find("input").Id);
        }

        /// <summary>
        /// A date picker with a label and UserAttributesId should use the UserAttributesId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void DatePickerWithLabelAndUserAttributesId_Should_UseUserAttributesIdForInputAndAccompanyingLabel()
        {
            var expectedId = "test-id";
            var comp = Context.RenderComponent<MudDatePicker>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes, new Dictionary<string, object?>
                    {
                        { "Id", expectedId }
                    }));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// Optional DatePicker should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalDatePicker_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.RenderComponent<MudDatePicker>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required DatePicker should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredDatePicker_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.RenderComponent<MudDatePicker>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required DatePicker attributes should be dynamic.
        /// </summary>
        [Test]
        public void RequiredAndAriaRequiredDatePickerAttributes_Should_BeDynamic()
        {
            var comp = Context.RenderComponent<MudDatePicker>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }
    }
}
