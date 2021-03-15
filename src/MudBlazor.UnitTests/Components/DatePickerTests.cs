#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using NUnit.Framework.Internal;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DatePickerTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        [Ignore("Unignore for performance measurements, not needed for code coverage")]
        public void DatePicker_Render_Performance()
        {
            // warmup
            ctx.RenderComponent<MudDatePicker>();
            // measure
            var watch = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
                ctx.RenderComponent<MudDatePicker>();
            watch.Stop();
            Console.WriteLine("Elapsed: " + watch.Elapsed);
            watch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }

        [Test]
        //[Ignore("Unignore for performance measurements, not needed for code coverage")]
        public async Task DatePicker_OpenClose_Performance()
        {
            // warmup
            var comp = ctx.RenderComponent<MudDatePicker>();
            var datepicker = comp.Instance;
            // measure
            var watch = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
            {
                await comp.InvokeAsync(() => datepicker.Open());
                await comp.InvokeAsync(() => datepicker.Close());
            }
            watch.Stop();
            Console.WriteLine("Elapsed: " + watch.Elapsed);
            watch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task SetPickerValue_CheckDate_SetPickerDate_CheckValue()
        {
            var comp = ctx.RenderComponent<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            comp.SetParam(p => p.Text, "2020-10-23");
            picker.Date.Should().Be(new DateTime(2020, 10, 23));
            comp.SetParam(p => p.Date, new DateTime(2020, 10, 26));
            picker.Text.Should().Be("2020-10-26");
        }

        [Test]
        public async Task DatePicker_Should_ApplyDateFormat()
        {
            var comp = ctx.RenderComponent<MudDatePicker>();
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
        public async Task DatePicker_Should_ApplyDateFormatAfterDate()
        {
            var comp = ctx.RenderComponent<MudDatePicker>();
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
        public void Check_Intial_Date_Format()
        {
            DateTime? date = new DateTime(2021, 1, 13);
            var comp = ctx.RenderComponent<MudDatePicker>(parameters => parameters
                .Add(p => p.Culture, CultureInfo.InvariantCulture)
                .Add(p => p.DateFormat, "dd/MM/yyyy")
                .Add(p => p.Date, date)
            );
            Console.WriteLine(comp.Markup);
            var picker = comp.Instance;
            var instance = comp.Instance;
            picker.Date.Should().Be(new DateTime(2021, 1, 13));
            picker.Text.Should().Be("13/01/2021");
        }

        public IRenderedComponent<MudDatePicker> OpenPicker(ComponentParameter parameter)
        {
            return OpenPicker(new ComponentParameter[] { parameter });
        }

        public IRenderedComponent<MudDatePicker> OpenPicker(ComponentParameter[] parameters = null)
        {
            IRenderedComponent<MudDatePicker> comp;
            if (parameters is null)
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
        public void Open_CloseBySelectingADate_CheckClosed()
        {
            var comp = OpenPicker();
            // clicking a day button to select a date and close
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("23")).First().Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0), TimeSpan.FromSeconds(5));
            comp.Instance.Date.Should().NotBeNull();
        }

        [Test]
        public void Open_CloseBySelectingADate_CheckClosed_Check_DateChangedCount()
        {
            var eventCount = 0;
            DateTime? returnDate = null;
            var comp = OpenPicker(EventCallback("DateChanged", (DateTime? date) => { eventCount++; returnDate = date; }));
            // clicking a day button to select a date and close
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("23")).First().Click();
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
            comp.Instance.Date.Should().BeNull();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public void Open_ClickCalendarHeader_CheckMonthsShown()
        {
            var comp = OpenPicker();
            // should show months
            comp.FindAll("div.mud-picker-calendar-header-transition")[0].Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public void Open_ClickYear_CheckYearsShown()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Month));
            // should show years
            comp.FindAll("div.mud-picker-calendar-header-transition")[0].Click();
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public void OpenToMonth_Select3rdMonth_Select2ndDay_CheckDate()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Month));
            comp.Instance.Date.Should().BeNull();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > div.mud-picker-month")
                [2].Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("2")).First().Click();
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
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > div.mud-picker-month")
                [3].Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("23")).First().Click();
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
        public void Open_ClickYear_ClickCurrentYear_Click2ndMonth_Click1_CheckDate()
        {
            var comp = OpenPicker();
            comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container > div.mud-picker-year")
                .Where(x => x.TrimmedText().Contains("2022")).First().Click();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > div.mud-picker-month")[1].Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-calendar-header").Count.Should().Be(1);
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("1")).First().Click();
            comp.Instance.Date.Value.Date.Should().Be(new DateTime(2022, 2, 1));
        }

        [Test]
        public async Task Open_Programmatically_CheckOpen_Close_Programmatically_CheckClosed()
        {
            var comp = ctx.RenderComponent<MudDatePicker>();
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
        public async Task PersianCalendar()
        {
            var cal = new PersianCalendar();
            var date = new DateTime(2021, 02, 14);
            cal.GetYear(date).Should().Be(1399);
            cal.GetMonth(date).Should().Be(11);
            cal.GetDayOfMonth(date).Should().Be(26);
            // ---------------------------------------------------------------
            var comp = ctx.RenderComponent<PersianDatePickerTest>();
            var datePicker = comp.FindComponent<MudDatePicker>();
            await comp.InvokeAsync(() => datePicker.Instance.Open());
            Console.WriteLine(comp.Markup);

            // didn't have time to finish this test case
            // TODO: check that the days are like here https://mrmashal.github.io/angular-material-persian-datepicker/demo/demoBasicUsage/index.html
            // for 1399-11-26
        }

        [Test]
        public void SetPickerValue_CheckText()
        {
            var comp = ctx.RenderComponent<MudDatePicker>(
                Parameter(nameof(MudDatePicker.Date), DateTime.Now));
            // select elements needed for the test
            var picker = comp.Instance;

            var text = DateTime.Now.ToIsoDateString();

            picker.Text.Should().Be(text);
            (comp.FindAll("input")[0] as IHtmlInputElement).Value.Should().Be(text);
        }
    }
}
