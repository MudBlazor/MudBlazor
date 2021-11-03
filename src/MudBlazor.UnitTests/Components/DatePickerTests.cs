#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
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
    [TestFixture]
    public class DatePickerTests : BunitTest
    {
        [Test]
        [Ignore("Unignore for performance measurements, not needed for code coverage")]
        public void DatePicker_Render_Performance()
        {
            // warmup
            Context.RenderComponent<MudDatePicker>();
            // measure
            var watch = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
                Context.RenderComponent<MudDatePicker>();
            watch.Stop();
            Console.WriteLine("Elapsed: " + watch.Elapsed);
            watch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }

        [Test]
        //[Ignore("Unignore for performance measurements, not needed for code coverage")]
        public async Task DatePicker_OpenClose_Performance()
        {
            // warmup
            var comp = Context.RenderComponent<MudDatePicker>();
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
        public async Task DatePicker_Should_ApplyDateFormat()
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
        public async Task DatePicker_Should_ApplyDateFormatAfterDate()
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
        public void Check_Intial_Date_Format()
        {
            DateTime? date = new DateTime(2021, 1, 13);
            var comp = Context.RenderComponent<MudDatePicker>(parameters => parameters
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

        public IRenderedComponent<SimpleMudDatePickerTest> OpenPicker(ComponentParameter parameter)
        {
            return OpenPicker(new ComponentParameter[] { parameter });
        }

        public IRenderedComponent<SimpleMudDatePickerTest> OpenPicker(ComponentParameter[] parameters = null)
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
            var comp = OpenPicker(EventCallback(nameof(MudDatePicker.DateChanged), (DateTime? date) => { eventCount++; returnDate = date; }));
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
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")
                [2].Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("2")).First().Click();
            comp.Instance.Date.Value.Date.Should().Be(new DateTime(DateTime.Now.Year, 3, 2));
        }

        public IRenderedComponent<SimpleMudDatePickerTest> OpenTo12thMonth()
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
            comp.Find("button.mud-picker-calendar-header-transition").Click();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")
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
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[1].Click();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-calendar-header").Count.Should().Be(1);
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("1")).First().Click();
            comp.Instance.Date.Value.Date.Should().Be(new DateTime(2022, 2, 1));
        }

        [Test]
        public async Task Open_Programmatically_CheckOpen_Close_Programmatically_CheckClosed()
        {
            var comp = Context.RenderComponent<SimpleMudDatePickerTest>();
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
            var comp = Context.RenderComponent<PersianDatePickerTest>();
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
            Func<DateTime, bool> isDisabledFunc = date => true;
            var comp = OpenPicker(Parameter(nameof(MudDatePicker.IsDateDisabledFunc), isDisabledFunc));

            comp.Instance.IsDateDisabledFunc.Should().Be(isDisabledFunc);
            comp.FindAll("button.mud-picker-calendar-day").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().OnlyContain(disabled => disabled == true);
        }

        [Test]
        public void IsDateDisabledFunc_SettingDateToADisabledDateYieldsNull()
        {
            var wasEventCallbackCalled = false;
            Func<DateTime, bool> isDisabledFunc = date => true;
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

        public async Task CheckAutoCloseDatePickerTest()
        {
            // Define a date for comparison
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            // Get access to the datepicker of the instance
            var comp = Context.RenderComponent<AutoCompleteDatePickerTest>();
            var datePicker = comp.FindComponent<MudDatePicker>();

            // Open the datepicker
            await comp.InvokeAsync(() => datePicker.Instance.Open());

            // Clicking a day button to select a date
            // It must be a different day than the day of now!
            // So the test is working when the day is 20
            if (now.Day != 20)
            {
                comp.FindAll("button.mud-picker-calendar-day")
                    .Where(x => x.TrimmedText().Equals("20")).First().Click();
            }
            else
            {
                comp.FindAll("button.mud-picker-calendar-day")
                    .Where(x => x.TrimmedText().Equals("19")).First().Click();
            }

            // Check that the date should remain the same because autoclose is false
            // and there are actions which are defined
            datePicker.Instance.Date.Should().Be(now);

            // Close the datepicker without submitting the date
            // The date of the datepicker remains equal to now 
            await comp.InvokeAsync(() => datePicker.Instance.Close(false));

            // Change the value of autoclose
            datePicker.Instance.AutoClose = true;

            // Open the datepicker
            await comp.InvokeAsync(() => datePicker.Instance.Open());

            // Clicking a day button to select a date
            if (now.Day != 20)
            {
                comp.FindAll("button.mud-picker-calendar-day")
                    .Where(x => x.TrimmedText().Equals("20")).First().Click();
            }
            else
            {
                comp.FindAll("button.mud-picker-calendar-day")
                    .Where(x => x.TrimmedText().Equals("19")).First().Click();
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
                comp.FindAll("button.mud-picker-calendar-day")
                    .Where(x => x.TrimmedText().Equals("20")).First().Click();
            }
            else
            {
                comp.FindAll("button.mud-picker-calendar-day")
                    .Where(x => x.TrimmedText().Equals("19")).First().Click();
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
            now = picker.Date.Value;

            comp.SetParametersAndRender(p => p.Add(x => x.Readonly, true));

            // Open the datepicker
            await picker.Open();


            // Clicking a day button to select a date
            if (now.Day != 21)
            {
                comp.FindAll("button.mud-picker-calendar-day")
                    .Where(x => x.TrimmedText().Equals("22")).First().Click();
            }
            else
            {
                comp.FindAll("button.mud-picker-calendar-day")
                    .Where(x => x.TrimmedText().Equals("21")).First().Click();
            }

            // Close the datepicker
            await picker.Close();

            // Check that the date should remain the same because readonly is true
            picker.Date.Should().Be(now);
        }

        [Test]
        public async Task CheckDateTimeMinValueTest()
        {
            // Define the datetime minvalue for the date
            var date = DateTime.MinValue;

            // Get access to the datepicker of the instance
            var comp = Context.RenderComponent<DateTimeMinValueDatePickerTest>();
            var datePicker = comp.FindComponent<MudDatePicker>();

            // Get the instance of the datepicker
            var picker = comp.Instance;

            // Open the datepicker
            await comp.InvokeAsync(() => datePicker.Instance.Open());

            // An error should be raised if the datepicker could not be not opened and the days could not generated
            // It means that there would be an exception!
            comp.FindAll("button.mud-picker-calendar-day").Where(x => x.TrimmedText().Equals("1")).First().Click();
        }

        /// <summary>
        /// Tests if all buttons have type="button" to prevent accidental form submits.
        /// </summary>
        /// <param name="navigateToMonthSelection">If true navigates to the month selection page.</param>
        [TestCase(false)]
        [TestCase(true)]
        [Test]
        public void CheckButtonTypeTest(bool navigateToMonthSelection)
        {
            var dateComp = Context.RenderComponent<MudDatePicker>(p =>
            p.Add(x => x.PickerVariant, PickerVariant.Dialog));

            Console.WriteLine(dateComp.Markup);

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
        public async Task DatePickerTest_KeyboardNavigation()
        {
            var comp = Context.RenderComponent<SimpleMudDatePickerTest>();

            Console.WriteLine(comp.Markup);
            var datePicker = comp.FindComponent<MudDatePicker>().Instance;

            await comp.InvokeAsync(() => comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "ArrowDown", AltKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "ArrowUp", AltKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Tab", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            datePicker.Disabled = true;

            await comp.InvokeAsync(() => comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => datePicker.ToggleOpen());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => datePicker.ToggleOpen());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => datePicker.ToggleState());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            datePicker.Disabled = false;

            await comp.InvokeAsync(() => comp.Find("input").KeyDown(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => datePicker.ToggleState());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
        }

        /// <summary>
        /// Test to check whether validation is working or not for required form-control
        /// </summary>
        /// <returns>The awaitable <see cref="Task"/></returns>
        [Test]
        public async Task DatePicker_Validation()
        {
            // render the component
            var datePickerComponent = Context.RenderComponent<DatePickerValidationTest>();

            // output the markup
            Console.WriteLine(datePickerComponent.Markup);

            // get the datepicker instance
            var datePickerInstance = datePickerComponent.FindComponent<MudDatePicker>().Instance;

            // ensure it's marked as required
            datePickerComponent.WaitForAssertion(() => datePickerInstance.Required.Should().BeTrue($"{typeof(DatePickerValidationTest).FullName}: {nameof(MudDatePicker)} is not marked as required."));
            
            // get the form's instance
            var formInstance = datePickerComponent.FindComponent<MudForm>().Instance;

            // ensure form is invalid
            datePickerComponent.WaitForAssertion(() => formInstance.IsValid.Should().BeFalse($"{typeof(DatePickerValidationTest).FullName}: {nameof(MudForm)} should be invalid at this point."));

            // set value for date-picker
            await datePickerComponent.InvokeAsync(() => datePickerInstance.Date = DateTime.Now.Date);

            // ensure date-picker is now valid
            datePickerComponent.WaitForAssertion(() => datePickerInstance.Error.Should().BeFalse($"{typeof(DatePickerValidationTest).FullName}: {nameof(MudDatePicker)} should be valid as it has a value."));

            // validate the form
            await datePickerComponent.InvokeAsync(() => formInstance.Validate());

            // ensure form is valid
            datePickerComponent.WaitForAssertion(() => formInstance.IsValid.Should().BeTrue($"{typeof(DatePickerValidationTest).FullName}: {nameof(MudForm)} should be valid as all required values have been provided."));
        }
    }
}
