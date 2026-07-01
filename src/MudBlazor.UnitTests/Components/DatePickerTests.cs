using System.Globalization;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents.DatePicker;
using MudBlazor.UnitTests.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
#nullable enable
    [TestFixture]
    public class DatePickerTests : BunitTest
    {
        [Test]
        public void Default()
        {
            var comp = Context.Render<MudDatePicker>();
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
        public void DatePickerOpenButtonDefaultAriaLabel()
        {
            var comp = Context.Render<DatePickerValidationTest>();
            var openButton = comp.Find(".mud-input-adornment button");
            openButton.Attributes.GetNamedItem("aria-label")?.Value.Should().Be("Open");
        }

        [Test]
        public void DatePickerLabelFor()
        {
            var comp = Context.Render<DatePickerValidationTest>();
            var label = comp.Find(".mud-input-label");
            label.Attributes.GetNamedItem("for")?.Value.Should().Be("datePickerLabelTest");
        }

        [Test]
        public void DatePickerInputId()
        {
            var comp = Context.Render<SimpleMudDatePickerTest>(parameters => parameters
                .Add(c => c.InputId, "birthday"));

            comp.Find("input[id='birthday']").Should().NotBeNull();
        }

        [Test]
        public async Task SetPickerValue_CheckDate_SetPickerDate_CheckValue()
        {
            var comp = Context.Render<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, new DateTime(2020, 10, 23).ToShortDateString()));
            picker.Date.Should().Be(new DateTime(2020, 10, 23));
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Date, new DateTime(2020, 10, 26)));
            picker.Text.Should().Be(new DateTime(2020, 10, 26).ToShortDateString());
        }

        [Test]
        public async Task DatePicker_Should_ApplyDateFormat()
        {
            var comp = Context.Render<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.DateFormat, "dd/MM/yyyy")
                .Add(p => p.Culture, CultureInfo.InvariantCulture)); // <-- this makes a huge difference!
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, "23/10/2020"));
            picker.Date.Should().Be(new DateTime(2020, 10, 23));
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Date, new DateTime(2020, 10, 26)));
            picker.Text.Should().Be("26/10/2020");
        }

        [Test]
        public async Task DatePicker_Should_ApplyDateFormatAfterDate()
        {
            var comp = Context.Render<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.DateFormat, "dd/MM/yyyy")
                .Add(p => p.Culture, CultureInfo.InvariantCulture)
                .Add(p => p.Date, new DateTime(2020, 10, 26)));
            picker.Date.Should().Be(new DateTime(2020, 10, 26));
            picker.Text.Should().Be("26/10/2020");
        }

        [Test]
        public async Task DatePicker_Should_ApplyCultureDateFormat()
        {
            var comp = Context.Render<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);

            var customCulture = new CultureInfo("en-US") { DateTimeFormat = { ShortDatePattern = "dd MM yyyy" } };
            customCulture.DateTimeFormat.ShortDatePattern.Should().Be("dd MM yyyy");
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Culture, customCulture));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, "23 10 2020"));
            picker.Date.Should().Be(new DateTime(2020, 10, 23));
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Date, new DateTime(2020, 10, 26)));
            picker.Text.Should().Be("26 10 2020");

            customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, "2020-10-13"));
            picker.Date.Should().Be(new DateTime(2020, 10, 13));
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Date, new DateTime(2020, 10, 16)));
            picker.Text.Should().Be("2020-10-16");
        }

        [Test]
        public async Task DatePicker_Should_DateFormatTakesPrecedenceOverCulture()
        {
            var comp = Context.Render<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.DateFormat, "dd MM yyyy")
                .Add(p => p.Culture, CultureInfo.InvariantCulture)
                .Add(p => p.Date, new DateTime(2020, 10, 26)));
            picker.Date.Should().Be(new DateTime(2020, 10, 26));
            picker.Text.Should().Be("26 10 2020");
        }

        [Test]
        public async Task ReadOnlyShouldNotHaveClearButton()
        {
            var comp = Context.Render<MudDatePicker>(p => p
                .Add(x => x.Text, "some value")
                .Add(x => x.Clearable, true)
                .Add(x => x.ReadOnly, false));

            comp.FindAll(".mud-input-clear-button").Count.Should().Be(1);

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ReadOnly, true)); //no clear button when readonly
            comp.FindAll(".mud-input-clear-button").Count.Should().Be(0);
        }

        [Test]
        public async Task DatePicker_Should_Clear()
        {
            var comp = Context.Render<MudDatePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.ReadOnly.Should().Be(false);
            picker.Date.Should().Be(null);
            picker.Text.Should().Be(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Clearable, true)
                .Add(p => p.Date, new DateTime(2020, 10, 26)));
            picker.Date.Should().Be(new DateTime(2020, 10, 26));
            picker.Text.Should().Be(new DateTime(2020, 10, 26).ToShortDateString());

            await comp.Find(".mud-input-clear-button").ClickAsync(); //clear the input

            picker.Text.Should().Be(""); //ensure the text and date are reset. Note this is an empty string rather than null due to how the reset works internally
            picker.Date.Should().Be(null);
        }

        [Test]
        public async Task DataPicker_ShouldClearText_WhenDateSetNull()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            var comp = Context.Render<MudDatePicker>();

            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);

            var invalid = "INVALID_DATE";
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, "INVALID_DATE"));

            picker.Date.Should().Be(null);
            picker.Text.Should().Be(invalid);

            // Advance past the SetDateAsync debounce window so the clear is not debounced away.
            timeProvider.Advance(TimeSpan.FromMilliseconds(150));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Date, null));

            picker.Date.Should().Be(null);
            picker.Text.Should().Be(null);
        }

        [Test]
        public async Task DataPicker_ShouldDeBounceSetDate_WhenDateSetToTheSameValueQuickly()
        {
            // Pin the clock so both quick sets land inside the same SetDateAsync debounce window
            // (contrast with the sibling above, which advances past it).
            _ = Context.AddFakeTimeProvider();
            var comp = Context.Render<MudDatePicker>();

            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);

            var invalid = "INVALID_DATE";
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, "INVALID_DATE"));

            picker.Date.Should().Be(null);
            picker.Text.Should().Be(invalid);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Date, null));

            // The clear is debounced (same value within the window), so the invalid text is retained.
            picker.Date.Should().Be(null);
            picker.Text.Should().Be(invalid);
        }

        [Test]
        public async Task DataPicker_ShouldDisplayError_WhenTextSetToInvalidValue()
        {
            var comp = Context.Render<MudDatePicker>();

            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, "INVALID_DATE"));

            picker.GetState(x => x.Error).Should().BeTrue();
        }

        [Test]
        public async Task DatePicker_ShouldClearValidationError_WhenInvalidDateIsQuicklyErased()
        {
            var comp = Context.Render<MudDatePicker>();

            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Date.Should().Be(null);

            var invalid = "INVALID_DATE";
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, "INVALID_DATE"));

            picker.Date.Should().Be(null);
            picker.Text.Should().Be(invalid);

            picker.GetState(x => x.Error).Should().BeTrue();
            picker.ConversionError.Should().BeTrue();

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, ""));

            picker.GetState(x => x.Error).Should().BeFalse();
            picker.ConversionError.Should().BeFalse();
            picker.Date.Should().Be(null);
        }

        [Test]
        public void Check_Initial_Date_Format()
        {
            DateTime? date = new DateTime(2021, 1, 13);
            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(p => p.Culture, CultureInfo.InvariantCulture)
                .Add(p => p.DateFormat, "dd/MM/yyyy")
                .Add(p => p.Date, date)
            );
            var picker = comp.Instance;
            picker.Date.Should().Be(new DateTime(2021, 1, 13));
            picker.Text.Should().Be("13/01/2021");
        }

        [Test]
        public async Task Check_DateTime_MaxValue()
        {
            DateTime? date = DateTime.MaxValue;

            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.Date, date));

            comp.Instance.Date.Should().Be(DateTime.MaxValue);

            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("31")).ToMarkup().Should().Contain("mud-selected");
            comp.Find("button.mud-button-date .mud-button-label").InnerHtml.Should().Be("Fri, 31 Dec");
            comp.Find("button.mud-button-year .mud-button-label").InnerHtml.Should().Be("9999");
        }

        [Test]
        public async Task Open_CloseByClickingOutsidePicker_CheckClosed()
        {
            var comp = await OpenPicker();
            // clicking outside to close
            await comp.Find("div.mud-overlay").ClickAsync();
            // should not be open anymore
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
        }

        [Test]
        public async Task Open_CloseBySelectingADate_CheckClosed()
        {
            var comp = await OpenPicker();
            // clicking a day button to select a date and close
            await comp.SelectDateAsync("23");
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0), TimeSpan.FromSeconds(5));
            comp.Instance.Date.Should().NotBeNull();
        }

        [Test]
        public async Task Open_CloseBySelectingADate_CheckClosed_Check_DateChangedCount()
        {
            var eventCount = 0;
            DateTime? returnDate = null;
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.DateChanged, (DateTime? date) => { eventCount++; returnDate = date; }));
            // clicking a day button to select a date and close
            await comp.SelectDateAsync("23");
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0), TimeSpan.FromSeconds(5));
            comp.Instance.Date.Should().NotBeNull();
            eventCount.Should().Be(1);
            var now = DateTime.Now;
            returnDate.Should().Be(new DateTime(now.Year, now.Month, 23));
        }

        [Test]
        public async Task OpenToYear_CheckYearsShown()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.OpenTo, OpenTo.Year));
            comp.Instance.Date.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public async Task OpenToYear_ClickYear_CheckMonthsShown()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.OpenTo, OpenTo.Year));
            comp.Instance.Date.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
            await comp.FindAll("div.mud-picker-year")[0].ClickAsync();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public async Task OpenToYear_ClickYear_CheckMonthsShown_Close_Reopen_CheckYearsShown()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.OpenTo, OpenTo.Year));
            comp.Instance.Date.Should().BeNull();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
            await comp.FindAll("div.mud-picker-year")[0].ClickAsync();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            // clicking outside to close
            await comp.Find("div.mud-overlay").ClickAsync();
            // should not be open anymore
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            await comp.Find("input").ClickAsync();
            // should show years
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public async Task OpenToMonth_CheckMonthsShown()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.OpenTo, OpenTo.Month));
            comp.Instance.Date.Should().BeNull();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public async Task Open_ClickCalendarHeader_CheckMonthsShown()
        {
            var comp = await OpenPicker();
            // should show months
            await comp.FindAll("button.mud-picker-calendar-header-transition")[0].ClickAsync();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public async Task Open_ClickYear_CheckYearsShown()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.OpenTo, OpenTo.Month));
            // should show years
            await comp.FindAll("button.mud-picker-calendar-header-transition")[0].ClickAsync();
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public async Task OpenToMonth_Select3rdMonth_Select2ndDay_CheckDate()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.OpenTo, OpenTo.Month));
            comp.Instance.Date.Should().BeNull();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            await comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[2].ClickAsync();
            await comp.SelectDateAsync("2");
            comp.Instance.Date?.Date.Should().Be(new DateTime(DateTime.Now.Year, 3, 2));
        }

        [Test]
        public async Task Open_ClickCalendarHeader_Click4thMonth_Click23rdDay_CheckDate()
        {
            var comp = await OpenPicker();
            await comp.Find("button.mud-picker-calendar-header-transition").ClickAsync();
            // should show months
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            await comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[3].ClickAsync();
            await comp.SelectDateAsync("23");
            comp.Instance.Date?.Date.Should().Be(new DateTime(DateTime.Now.Year, 4, 23));
        }

        [Test]
        public async Task DatePickerStaticWithPickerActionsDayClick()
        {
            var comp = Context.Render<DatePickerStaticTest>();
            var picker = comp.FindComponent<MudDatePicker>();

            picker.Markup.Should().Contain("mud-selected"); //confirm selected date is shown

            // Calculate expected date before selection
            var date = DateTime.Today.Subtract(TimeSpan.FromDays(60));
            var expectedDate = new DateTime(date.Year, date.Month, 23);

            // Select the date
            await comp.SelectDateAsync("23");

            // Wait for the date picker to update its state after selection
            await comp.WaitForAssertionAsync(() => picker.Instance.Date.Should().Be(expectedDate));
        }

        [Test]
        public async Task DatePickerBinding()
        {
            var comp = Context.Render<DatePickerBindingTest>();

            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            await comp.Find(".mud-input-adornment button").ClickAsync();
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);

            var picker = comp.FindComponent<MudDatePicker>();

            comp.Markup.Should().Contain("mud-selected");

            picker.Instance.Date.Should().Be(comp.Instance.ExpiresOn);

            await comp.Find(".mud-overlay").ClickAsync();
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);

            var currentDate = comp.Instance.ExpiresOn;

            await comp.Find(".mud-button").ClickAsync();

            comp.Instance.ExpiresOn.Should().Be(currentDate!.Value.AddMonths(10));

            await comp.Find(".mud-input-adornment button").ClickAsync();
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);

            comp.Markup.Should().Contain("mud-selected");

            picker.Instance.Date.Should().Be(comp.Instance.ExpiresOn);
        }

        [Test]
        public async Task OpenTo12thMonth_NavigateBack_CheckMonth()
        {
            var comp = await OpenTo12ThMonth();
            var picker = comp.Instance;
            await comp.Find("div.mud-picker-calendar-header-switch > button:nth-child(1)").ClickAsync();
            picker.PickerMonth?.Month.Should().Be(11);
            picker.PickerMonth?.Year.Should().Be(DateTime.Now.Year);
        }

        [Test]
        public async Task OpenTo12thMonth_NavigateForward_CheckYear()
        {
            var comp = await OpenTo12ThMonth();
            var picker = comp.Instance;
            await comp.Find("div.mud-picker-calendar-header-switch > button:nth-child(3)").ClickAsync();
            picker.PickerMonth?.Month.Should().Be(1);
            picker.PickerMonth?.Year.Should().Be(DateTime.Now.Year + 1);
        }

        [Test]
        public async Task Open_ClickYear_ClickCurrentYear_Click2ndMonth_Click1_CheckDate()
        {
            var comp = await OpenPicker();
            await comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").ClickAsync();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(1);
            await comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container > div.mud-picker-year").First(x => x.TrimmedText().Contains("2022")).ClickAsync();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            await comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[1].ClickAsync();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-calendar-header").Count.Should().Be(1);
            await comp.SelectDateAsync("1");
            comp.Instance.Date?.Date.Should().Be(new DateTime(2022, 2, 1));
        }

        [Test]
        public async Task Open_FixYear_Click2ndMonth_Click3_CheckDate()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.FixYear, 2021));
            await comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").ClickAsync();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(0);
            await comp.Find("div.mud-picker-calendar-container > .mud-picker-calendar-header > .mud-picker-calendar-header-switch > .mud-button-month").ClickAsync();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            await comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[1].ClickAsync();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-calendar-header").Count.Should().Be(1);
            await comp.SelectDateAsync("3");
            comp.Instance.Date?.Date.Should().Be(new DateTime(2021, 2, 3));
        }

        [Test]
        public async Task Open_FixDay_ClickYear_Click2ndMonth_CheckDate()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.FixDay, 1));
            await comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").ClickAsync();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(1);
            await comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container > div.mud-picker-year").First(x => x.TrimmedText().Contains("2022")).ClickAsync();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            await comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[1].ClickAsync();
            comp.Instance.Date?.Date.Should().Be(new DateTime(2022, 2, 1));
        }

        [Test]
        public async Task Open_FixMonth_ClickYear_Click3_CheckDate()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.FixMonth, 1));
            comp.FindAll("div.mud-picker-calendar-container > .mud-picker-calendar-header > .mud-picker-calendar-header-switch > .mud-button-month").Count.Should().Be(0);
            await comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").ClickAsync();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(1);
            await comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container > div.mud-picker-year").First(x => x.TrimmedText().Contains("2022")).ClickAsync();
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(0);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-calendar-header").Count.Should().Be(1);
            await comp.SelectDateAsync("3");
            comp.Instance.Date?.Date.Should().Be(new DateTime(2022, 1, 3));
        }

        [Test]
        public async Task Open_FixYear_FixMonth_Click3_CheckDate()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.FixMonth, 1)
                .Add(x => x.FixYear, 2022));
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(0);
            await comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").ClickAsync();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(0);
            comp.FindAll("div.mud-picker-calendar-container > .mud-picker-calendar-header > .mud-picker-calendar-header-switch > .mud-button-month").Count.Should().Be(0);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-calendar-header").Count.Should().Be(1);
            await comp.SelectDateAsync("3");
            comp.Instance.Date?.Date.Should().Be(new DateTime(2022, 1, 3));
        }

        [Test]
        public async Task Open_FixMonth_FixDay_ClickYear2022_CheckDate()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.OpenTo, OpenTo.Year)
                .Add(x => x.FixMonth, 1)
                .Add(x => x.FixDay, 1));
            comp.FindAll("div.mud-picker-calendar-container > .mud-picker-calendar-header > .mud-picker-calendar-header-switch > .mud-button-month").Count.Should().Be(0);
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(1);
            await comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container > div.mud-picker-year").First(x => x.TrimmedText().Contains("2022")).ClickAsync();
            comp.Instance.Date?.Date.Should().Be(new DateTime(2022, 1, 1));
        }

        [Test]
        public async Task Open_FixYear_FixDay_Click3rdMonth_CheckDate()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.OpenTo, OpenTo.Month)
                .Add(x => x.FixYear, 2022)
                .Add(x => x.FixDay, 1));
            await comp.Find("div.mud-picker-datepicker-toolbar > button.mud-button-year").ClickAsync();
            comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-year-container").Count.Should().Be(0);
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
            await comp.FindAll("div.mud-picker-calendar-container > div.mud-picker-month-container > button.mud-picker-month")[2].ClickAsync();
            comp.Instance.Date?.Date.Should().Be(new DateTime(2022, 3, 1));
        }

        [Test]
        public async Task Open_FixDay_CheckOpenTo()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.FixDay, 1));
            comp.FindAll("div.mud-picker-month-container").Count.Should().Be(1);
        }

        [Test]
        public async Task Open_FixMonth_FixDay_CheckOpenTo()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.FixMonth, 1)
                .Add(x => x.FixDay, 1));
            comp.FindAll("div.mud-picker-year-container").Count.Should().Be(1);
        }

        [Test]
        public async Task Open_Programmatically_CheckOpen_Close_Programmatically_CheckClosed()
        {
            var comp = Context.Render<SimpleMudDatePickerTest>();
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
            var comp = Context.Render<PersianDatePickerTest>();
            var datePicker = comp.FindComponent<MudDatePicker>();
            await comp.InvokeAsync(() => datePicker.Instance.OpenAsync());

            datePicker.Instance.Text.Should().Be("1399/11/26");
        }

        [Test]
        public async Task PersianCalendar_GoToDate()
        {
            var cal = new PersianCalendar();
            var comp = Context.Render<PersianDatePickerTest>();
            var datePicker = comp.FindComponent<MudDatePicker>().Instance;
            await comp.InvokeAsync(() => datePicker.OpenAsync());
            datePicker.Text.Should().Be("1399/11/26");
            await comp.InvokeAsync(() => datePicker.GoToDate(new DateTime(2024, 5, 8)));
            await comp.WaitForAssertionAsync(() => datePicker.Text.Should().Be("1403/02/19"));
            var button = comp
                .FindAll(".mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-picker-calendar-day.mud-day")
                .Single(x => x.GetAttribute("style") == "--day-id: 1;");
            button.TextContent.Should().Be("1");
        }

        [Test]
        public async Task PersianCalendarDefault()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            timeProvider.SetUtcNow(new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc));

            var comp = Context.Render<PersianDatePickerTest>(paramter => paramter.Add(p => p.Date, null));
            var datePicker = comp.FindComponent<MudDatePicker>().Instance;
            await comp.InvokeAsync(() => datePicker.OpenAsync());

            datePicker.Text.Should().BeNull();
            comp.Find("button.mud-button-year").TrimmedText().Equals("1403");
            comp.Find("button.mud-button-month").TrimmedText().Should().Contain("1403");
            comp.Find("button.mud-button-date").TrimmedText().Should().BeNullOrEmpty();
        }

        [Test]
        public async Task PersianCalendarFixedDay()
        {
            var cal = new PersianCalendar();
            var date = new DateTime(1404, 1, 1, cal);

            var comp = Context.Render<PersianDatePickerTest>(parameter => parameter.Add(p => p.Date, date).Add(p => p.FixDay, 1));

            comp.FindAll("button.mud-picker-month").Count.Should().Be(0);
            await comp.Find("input").ClickAsync();
            comp.FindAll("button.mud-picker-month").Count.Should().Be(12);
            await comp.FindAll("button.mud-picker-month")[0].ClickAsync();
            comp.Instance.Date?.Date.Should().Be(new DateTime(1404, 1, 1, cal));
        }

        [Test]
        public void SetPickerValue_CheckText()
        {
            var date = DateTime.Now;
            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(x => x.Date, date));
            // select elements needed for the test
            var picker = comp.Instance;

            var text = date.ToShortDateString();

            picker.Text.Should().Be(text);
            ((IHtmlInputElement)comp.FindAll("input")[0]).Value.Should().Be(text);
        }

        [Test]
        public async Task IsDateDisabledFunc_DisablesCalendarDateButtons()
        {
            Func<DateTime, bool> isDisabledFunc = _ => true;
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.IsDateDisabledFunc, isDisabledFunc));

            comp.Instance.IsDateDisabledFunc.Should().Be(isDisabledFunc);
            comp.FindAll("button.mud-picker-calendar-day").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().OnlyContain(disabled => disabled);
        }

        [Test]
        public async Task IsDateDisabledFunc_DisablesCalendarMonthButtons()
        {
            Func<DateTime, bool> isDisabledFunc = _ => true;
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.IsDateDisabledFunc, isDisabledFunc)
                .Add(x => x.OpenTo, OpenTo.Month)
                .Add(x => x.FixDay, 1));

            comp.Instance.IsDateDisabledFunc.Should().Be(isDisabledFunc);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().OnlyContain(disabled => disabled);

            // None should be selected
            comp.FindAll("button.mud-picker-month > .mud-typography").Select(
                text => ((IHtmlElement)text).ClassList.Any(cls => cls == "mud-picker-month-select" || cls == "mud-primary-text"))
                .Should().OnlyContain(selected => selected == false);
        }

        [Test]
        public async Task DisableCalendarMonthButtonsWhenFixDayOutOfRange()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            timeProvider.SetUtcNow(new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc));

            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.OpenTo, OpenTo.Month)
                .Add(x => x.FixDay, 31));

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
        public async Task IsDateDisabledFunc_DoesNotHaveEffectOnMonthsIfDayNotFixed()
        {
            Func<DateTime, bool> isDisabledFunc = _ => true;
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.IsDateDisabledFunc, isDisabledFunc)
                .Add(x => x.OpenTo, OpenTo.Month));

            comp.Instance.IsDateDisabledFunc.Should().Be(isDisabledFunc);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().OnlyContain(disabled => disabled == false);
        }

        [Test]
        public async Task IsDateDisabledFunc_DoesNotHaveEffectOnMonthsIfFuncReturnsFalse()
        {
            Func<DateTime, bool> isDisabledFunc = _ => false;
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.IsDateDisabledFunc, isDisabledFunc)
                .Add(x => x.OpenTo, OpenTo.Month)
                .Add(x => x.FixDay, 1));

            comp.Instance.IsDateDisabledFunc.Should().Be(isDisabledFunc);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().OnlyContain(disabled => disabled == false);
        }

        [TestCase(10, 8, 2, 2)]
        [TestCase(10, 9, 2, 2)]
        [TestCase(10, 10, 2, 1)]
        [TestCase(10, 11, 2, 1)]
        public async Task MinDateEffectOnDisablingMonthsIfDayFixed(int minDatesDay, int fixedDay, int month, int disabledOnes)
        {
            var currentDate = DateTime.Now;
            var minDate = new DateTime(currentDate.Year, month, minDatesDay);
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.MinDate, minDate)
                .Add(x => x.OpenTo, OpenTo.Month)
                .Add(x => x.FixDay, fixedDay));

            var expectedResult = new bool[12];
            for (var i = 0; i < disabledOnes; ++i)
            {
                expectedResult[i] = true;
            }

            comp.Instance.MinDate.Should().Be(minDate);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().ContainInConsecutiveOrder(expectedResult);
        }

        [TestCase(10, 9, 11, 1)]
        [TestCase(10, 10, 11, 1)]
        [TestCase(10, 11, 11, 2)]
        [TestCase(10, 12, 11, 2)]
        public async Task MaxDateEffectOnDisablingMonthsIfDayFixed(int maxDatesDay, int fixedDay,
            int month, int disabledOnes)
        {
            var currentDate = DateTime.Now;
            var maxDate = new DateTime(currentDate.Year, month, maxDatesDay);
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.MaxDate, maxDate)
                .Add(x => x.OpenTo, OpenTo.Month)
                .Add(x => x.FixDay, fixedDay));

            var expectedResult = new bool[12];
            for (var i = 0; i < disabledOnes; ++i)
            {
                expectedResult[11 - i] = true;
            }

            comp.Instance.MaxDate.Should().Be(maxDate);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().ContainInConsecutiveOrder(expectedResult);
        }

        [TestCase(30, 3, 2)]
        [TestCase(31, 3, 2)]
        [TestCase(1, 4, 3)]
        [TestCase(2, 4, 3)]
        public async Task MinDateEffectOnDisablingMonthsIfDayNotFixed(int minDatesDay, int month, int disabledOnes)
        {
            var currentYear = DateTime.Now.Year;
            var minDate = new DateTime(currentYear, month, minDatesDay);
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.MinDate, minDate)
                .Add(x => x.OpenTo, OpenTo.Month));

            var expectedResult = new bool[12];
            for (var i = 0; i < disabledOnes; ++i)
            {
                expectedResult[i] = true;
            }

            comp.Instance.MinDate.Should().Be(minDate);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().ContainInConsecutiveOrder(expectedResult);
        }

        [TestCase(1, 10, 2)]
        [TestCase(2, 10, 2)]
        [TestCase(30, 9, 3)]
        [TestCase(29, 9, 3)]
        public async Task MaxDateEffectOnDisablingMonthsIfDayNotFixed(int maxDatesDay, int month, int disabledOnes)
        {
            var currentYear = DateTime.Now.Year;
            var maxDate = new DateTime(currentYear, month, maxDatesDay);
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.MaxDate, maxDate)
                .Add(x => x.OpenTo, OpenTo.Month));

            var expectedResult = new bool[12];
            for (var i = 0; i < disabledOnes; ++i)
            {
                expectedResult[11 - i] = true;
            }

            comp.Instance.MaxDate.Should().Be(maxDate);
            comp.FindAll("button.mud-picker-month").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().ContainInConsecutiveOrder(expectedResult);
        }

        [Test]
        public async Task IsDateDisabledFunc_SettingDateToADisabledDateYieldsNull()
        {
            var wasEventCallbackCalled = false;
            Func<DateTime, bool> isDisabledFunc = _ => true;
            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(x => x.IsDateDisabledFunc, isDisabledFunc)
                .Add(x => x.DateChanged, (DateTime? _) => wasEventCallbackCalled = true));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(picker => picker.Date, DateTime.Now));

            comp.Instance.Date.Should().BeNull();
            wasEventCallbackCalled.Should().BeFalse();
        }

        [Test]
        public async Task IsDateDisabledFunc_SettingDateToAnEnabledDateYieldsTheDate()
        {
            var wasEventCallbackCalled = false;
            var today = DateTime.Today;
            Func<DateTime, bool> isDisabledFunc = date => date < today;
            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(x => x.IsDateDisabledFunc, isDisabledFunc)
                .Add(x => x.DateChanged, (DateTime? _) => wasEventCallbackCalled = true));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(picker => picker.Date, today));

            comp.Instance.Date.Should().Be(today);
            wasEventCallbackCalled.Should().BeTrue();
        }

        [Test]
        public async Task IsDateDisabledFunc_NoDisabledDatesByDefault()
        {
            var comp = await OpenPicker();
            comp.FindAll("button.mud-picker-calendar-day").Select(button => ((IHtmlButtonElement)button).IsDisabled)
                .Should().OnlyContain(disabled => disabled == false);
        }

        [Test]
        //mud-button-root added for graying out and making buttons not clickable if month is disabled
        public async Task MonthButtons_ButtonRootClassPresent()
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.FixDay, 1));
            var monthsCount = 12;

            comp.FindAll("button.mud-picker-month")
                .Should().HaveCount(monthsCount)
                .And.OnlyContain(button => button.ClassName != null && button.ClassName.Contains("mud-button-root"));
        }

        [Test]
        public async Task AdditionalDateClassesFunc_ClassIsAdded()
        {
            Func<DateTime, string> additionalDateClassesFunc = _ => "__addedtestclass__";

            var comp = await OpenPicker(parameters => parameters.Add(x => x.AdditionalDateClassesFunc, additionalDateClassesFunc));

            var daysCount = comp.FindAll("button.mud-picker-calendar-day")
                                .Select(button => (IHtmlButtonElement)button)
                                .Count();

            comp.FindAll("button.mud-picker-calendar-day")
                .Where(button => button.ClassName is not null && button.ClassName.Contains("__addedtestclass__"))
                .Should().HaveCount(daysCount);
        }

        // AutocompleteDatePickerTest initializes to a fixed date (2025-06-10); these tests select the 15th,
        // so a commit is observable as a change from the 10th to the 15th regardless of the run date.
        [Test]
        public async Task DatePicker_WithPickerActions_DayClickDoesNotAutoCommitOrClose()
        {
            var comp = Context.Render<AutocompleteDatePickerTest>();
            var datePicker = comp.FindComponent<MudDatePicker>();
            var initialDate = datePicker.Instance.Date;
            initialDate.Should().Be(new DateTime(2025, 6, 10));

            await comp.InvokeAsync(datePicker.Instance.OpenAsync);
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            await comp.SelectDateAsync("15");

            // With PickerActions defined and AutoClose off, the click waits for the OK button:
            // neither the value nor the open state changes.
            datePicker.Instance.Date.Should().Be(initialDate);
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);

            // ClearAsync also leaves the popover open while AutoClose is off.
            await comp.InvokeAsync(() => datePicker.Instance.ClearAsync());
            datePicker.Instance.Date.Should().BeNull();
            comp.FindAll("div.mud-popover-open").Count.Should().Be(1);
        }

        [Test]
        public async Task DatePicker_WithAutoClose_DayClickCommitsAndCloses()
        {
            // ClosingDelay = 0 removes the post-commit close timer so the day click commits the value
            // and closes the popover deterministically, without depending on a timer settling (the commit
            // runs on a pooled thread, so advancing a fake clock after the commit races the timer and hangs).
            var comp = Context.Render<AutocompleteDatePickerTest>(parameters => parameters
                .Add(p => p.AutoClose, true)
                .Add(p => p.ClosingDelay, 0));
            var datePicker = comp.FindComponent<MudDatePicker>();

            await comp.InvokeAsync(datePicker.Instance.OpenAsync);
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            // The initial date is the 10th, so committing the 15th is an observable change.
            await comp.SelectDateAsync("15");

            await comp.WaitForAssertionAsync(() =>
            {
                datePicker.Instance.Date.Should().Be(new DateTime(2025, 6, 15));
                comp.FindAll("div.mud-popover-open").Count.Should().Be(0);
            });
        }

        [Test]
        public async Task DatePicker_ClearAsync_WithAutoClose_ClosesPopover()
        {
            var comp = Context.Render<AutocompleteDatePickerTest>(parameters => parameters.Add(p => p.AutoClose, true));
            var datePicker = comp.FindComponent<MudDatePicker>();

            await comp.InvokeAsync(datePicker.Instance.OpenAsync);
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => datePicker.Instance.ClearAsync());

            // ClearAsync clears the value and, because AutoClose is on, closes the popover.
            datePicker.Instance.Date.Should().BeNull();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
        }

        [Test]
        public async Task CheckReadOnly()
        {
            // Define a date for comparison
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            // Get access to the datepicker of the instance
            var comp = Context.Render<SimpleMudDatePickerTest>();
            var picker = comp.Instance;

            // Open the datepicker
            await picker.Open();

            // Clicking a day button to select a date
            // It must be a different day than the day of now!
            // So the test is working when the day is 20
            if (now.Day != 20)
            {
                await comp.SelectDateAsync("20");
            }
            else
            {
                await comp.SelectDateAsync("19");
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

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Readonly, true));

            // Open the datepicker
            await picker.Open();

            // Clicking a day button to select a date
            if (now.Day != 21)
            {
                await comp.SelectDateAsync("22");
            }
            else
            {
                await comp.SelectDateAsync("21");
            }

            // Close the datepicker
            await picker.Close();

            // Check that the date should remain the same because readonly is true
            picker.Date.Should().Be(now);
        }

        [Test]
        public async Task StaticReadOnly_ShouldNotChangeDate()
        {
            var initialDate = new DateTime(2025, 6, 15);
            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(p => p.PickerVariant, PickerVariant.Static)
                .Add(p => p.ReadOnly, true)
                .Add(p => p.Date, initialDate));
            var picker = comp.Instance;

            // Try to select a different day - should be blocked by ReadOnly
            await comp.SelectDateAsync("10");

            // Date should remain unchanged because ReadOnly is true
            picker.Date.Should().Be(initialDate);
        }

        // In the editable path a month/year click advances the view (Year->Month, Month->Date); the ReadOnly
        // guard must suppress that, so the originally-shown container stays put. Asserting the view (not Date,
        // which a month/year click never commits anyway) is what actually proves the guard.
        [Test]
        [TestCase(OpenTo.Year, "div.mud-picker-year-container", "div.mud-picker-year")]
        [TestCase(OpenTo.Month, "div.mud-picker-month-container", "button.mud-picker-month")]
        public async Task StaticReadOnly_MonthOrYearClick_DoesNotAdvanceView(OpenTo openTo, string container, string entry)
        {
            var initialDate = new DateTime(2025, 6, 15);
            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(p => p.PickerVariant, PickerVariant.Static)
                .Add(p => p.ReadOnly, true)
                .Add(p => p.OpenTo, openTo)
                .Add(p => p.Date, initialDate));
            var picker = comp.Instance;

            // The requested view is shown.
            comp.FindAll(container).Count.Should().Be(1);

            await comp.FindAll(entry)[0].ClickAsync();

            // ReadOnly blocks the click: the view does not advance and the date is untouched.
            comp.FindAll(container).Count.Should().Be(1);
            picker.Date.Should().Be(initialDate);
        }

        [Test]
        public void ShowWeekNumbers_RendersWeekNumberColumn()
        {
            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(p => p.PickerVariant, PickerVariant.Static)
                .Add(p => p.ShowWeekNumbers, true)
                .Add(p => p.Culture, new CultureInfo("en-US"))
                .Add(p => p.PickerMonth, new DateTime(2025, 1, 1)));

            // A week cell is rendered in the header plus one per calendar row.
            comp.FindAll(".mud-picker-calendar-week").Count.Should().BeGreaterThan(1);

            // The first calendar week of January is week 1.
            var weekNumbers = comp.FindAll(".mud-picker-calendar-week-text")
                .Select(x => x.TrimmedText())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();
            weekNumbers.Should().Contain("1");
        }

        [Test]
        public void For_WithoutExplicitLabel_UsesLabelAttribute()
        {
            var model = new DisplayNameLabelClass();

            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(p => p.For, () => model.Date));
            comp.Instance.Label.Should().Be("Date LabelAttribute");

            // An explicit Label always wins over the [Label] attribute.
            var explicitLabel = Context.Render<MudDatePicker>(parameters => parameters
                .Add(p => p.For, () => model.Date)
                .Add(p => p.Label, "Explicit"));
            explicitLabel.Instance.Label.Should().Be("Explicit");
        }

        [Test]
        public void Mask_Parameter_RoundTrips()
        {
            var mask = new DateMask("0000-00-00");
            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(p => p.Editable, true)
                .Add(p => p.Mask, mask));

            comp.Instance.Mask.Should().BeSameAs(mask);
        }

        [Test]
        public async Task FocusSelectAndSelectRange_AreNoOps_WhenNoInputIsRendered()
        {
            // A Static picker renders no text input, so _inputReference stays null and these must not throw.
            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(p => p.PickerVariant, PickerVariant.Static));
            var picker = comp.Instance;

            var act = async () => await comp.InvokeAsync(async () =>
            {
                await picker.FocusAsync();
                await picker.SelectAsync();
                await picker.SelectRangeAsync(0, 1);
            });

            await act.Should().NotThrowAsync();
        }

        [Test]
        public async Task CheckDateTimeMinValue()
        {
            // Get access to the datepicker of the instance
            var comp = Context.Render<DateTimeMinValueDatePickerTest>();
            var datePicker = comp.FindComponent<MudDatePicker>();

            // Open the datepicker
            await comp.InvokeAsync(() => datePicker.Instance.OpenAsync());

            // An error should be raised if the datepicker could not be not opened and the days could not generated
            // It means that there would be an exception!
            await comp.SelectDateAsync("1");
        }

        /// <summary>
        /// Tests if all buttons have type="button" to prevent accidental form submits.
        /// </summary>
        /// <param name="navigateToMonthSelection">If true navigates to the month selection page.</param>
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task CheckButtonType(bool navigateToMonthSelection)
        {
            var dateComp = Context.Render<MudDatePicker>(p =>
            p.Add(x => x.PickerVariant, PickerVariant.Dialog));

            //open picker
            await dateComp.Find(".mud-picker input").ClickAsync();

            //navigate to month selection
            if (navigateToMonthSelection)
            {
                await dateComp.Find(".mud-picker button.mud-picker-calendar-header-transition").ClickAsync();
            }

            var buttons = dateComp.FindAll(".mud-picker button");
            //expected values
            foreach (var button in buttons)
            {
                button.ToMarkup().Contains("type=\"button\"").Should().BeTrue();
            }
        }

        [Test]
        public async Task DatePicker_Editable()
        {
            var comp = Context.Render<SimpleMudDatePickerTest>();

            var cultureInfo = new CultureInfo("en-US");

            var datePickerComponent = comp.FindComponent<MudDatePicker>();
            await datePickerComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(parameter => parameter.Editable, true)
                .Add(parameter => parameter.Culture, cultureInfo));

            var datePicker = datePickerComponent.Instance;

            await comp.Find("input").ChangeAsync("10/10/2020");
            await comp.WaitForAssertionAsync(() => datePicker.Date.Should().Be(new DateTime(2020, 10, 10)));
            await comp.WaitForAssertionAsync(() => datePicker.PickerMonth.Should().Be(new DateTime(2020, 10, 1)));

            await comp.InvokeAsync(datePicker.OpenAsync);
            await comp.WaitForAssertionAsync(() => datePicker.PickerMonth.Should().Be(new DateTime(2020, 10, 01)));
        }

        [Test]
        public async Task DatePicker_KeyboardNavigation()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<SimpleMudDatePickerTest>();
            var datePickerComponent = comp.FindComponent<MudDatePicker>();
            var datePicker = datePickerComponent.Instance;

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs { Key = "Escape", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs { Key = " ", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs { Key = " ", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs { Key = "ArrowDown", AltKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs { Key = "ArrowUp", AltKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs { Key = "NumpadEnter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs { Key = "Tab", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await datePickerComponent.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.Disabled, true));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => datePicker.ToggleOpenAsync());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => datePicker.ToggleOpenAsync());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => datePicker.ToggleStateAsync());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await datePickerComponent.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.Disabled, false));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs { Key = "NumpadEnter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(datePicker.ToggleStateAsync);
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
        }

        [Test]
        public async Task DatePicker_KeyboardNavigation_SelectDate()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<SimpleMudDatePickerTest>();
            var startDate = new DateTime(2022, 12, 31, new CultureInfo("en-US").Calendar);
            var expectedDate1 = new DateTime(2021, 1, 23, new CultureInfo("en-US").Calendar);
            var expectedDate2 = new DateTime(2023, 11, 22, new CultureInfo("en-US").Calendar);

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(parameter => parameter.Date, startDate)
                .Add(parameter => parameter.OpenTo, OpenTo.Year));
            var datePickerComponent = comp.FindComponent<MudDatePicker>();
            var datePicker = datePickerComponent.Instance;

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            //year
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            //month
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            //date
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));
            datePicker.Date.Should().Be(expectedDate1);

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            //year
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            //month
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            //date
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            // select month with shift key
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", ShiftKey = true }));
            // select year with shift key
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));
            datePicker.Date.Should().Be(expectedDate2);
        }

        [Test]
        public async Task DatePicker_KeyboardNavigation_SelectMonth()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<SimpleMudDatePickerTest>();
            var startDate = new DateTime(2022, 12, 31, new CultureInfo("en-US").Calendar);
            var expectedDate1 = new DateTime(2022, 1, 28, new CultureInfo("en-US").Calendar);
            var expectedDate2 = new DateTime(2023, 1, 28, new CultureInfo("en-US").Calendar);

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(parameter => parameter.Date, startDate)
                .Add(parameter => parameter.OpenTo, OpenTo.Month));
            var datePickerComponent = comp.FindComponent<MudDatePicker>();
            var datePicker = datePickerComponent.Instance;

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown" }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown" }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown" }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown" }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown" }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown" }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown" }));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));
            datePicker.Date.Should().Be(expectedDate1);

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
            // select year with shift key
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", ShiftKey = true }));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));
            datePicker.Date.Should().Be(expectedDate2);

        }

        [Test]
        public async Task DatePicker_KeyboardNavigation_BackspacePreviousView()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<SimpleMudDatePickerTest>();
            var startDate = new DateTime(2022, 12, 31, new CultureInfo("en-US").Calendar);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(parameter => parameter.Date, startDate)
                .Add(parameter => parameter.OpenTo, OpenTo.Year));
            var datePickerComponent = comp.FindComponent<MudDatePicker>();
            var datePicker = datePickerComponent.Instance;

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            //year
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            //month
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            //date
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Backspace", Type = "keydown", }));

            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));
            // no change to date should happen
            datePicker.Date.Should().Be(startDate);

        }

        [Test]
        public async Task DatePicker_KeyboardNavigation_MinMaxYearLimits()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<SimpleMudDatePickerTest>();
            var startDate = new DateTime(2022, 12, 31, new CultureInfo("en-US").Calendar);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(parameter => parameter.Date, startDate)
                .Add(parameter => parameter.OpenTo, OpenTo.Year));
            var maxDate = new DateTime(2023, 12, 31);
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.MaxDate, maxDate));
            var minDate = new DateTime(2021, 12, 31);
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.MinDate, minDate));
            var datePickerComponent = comp.FindComponent<MudDatePicker>();
            var datePicker = datePickerComponent.Instance;
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
            //try to select year below min
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));
            datePicker.Date.Should().Be(minDate);

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
            //try to select year above max
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));
            datePicker.Date.Should().Be(maxDate);

        }

        [Test]
        public async Task DatePicker_KeyboardNavigation_FixYear_CannotBeChanged()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var startDate = new DateTime(2022, 12, 31, new CultureInfo("en-US").Calendar);
            var comp = Context.Render<SimpleMudDatePickerTest>(parameters => parameters
                .Add(x => x.Date, startDate)
                .Add(x => x.OpenTo, OpenTo.Month)
                .Add(x => x.FixYear, 2022));
            var datePickerComponent = comp.FindComponent<MudDatePicker>();
            var datePicker = datePickerComponent.Instance;
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
            //try to select year outside fixed year in month view
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));
            datePicker.Date.Should().Be(startDate);
        }

        [Test]
        public async Task DatePicker_KeyboardNavigation_FixMonth_CannotBeChanged()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var startDate = new DateTime(2022, 12, 31, new CultureInfo("en-US").Calendar);
            var comp = Context.Render<SimpleMudDatePickerTest>(parameters => parameters
                .Add(x => x.Date, startDate)
                .Add(x => x.OpenTo, OpenTo.Date)
                .Add(x => x.FixMonth, 12));
            var datePickerComponent = comp.FindComponent<MudDatePicker>();
            var datePicker = datePickerComponent.Instance;
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
            //try to select month outside fixed month in date view
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", ShiftKey = true }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(datePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));
            datePicker.Date.Should().Be(startDate);
        }

        [Test]
        public async Task DatePicker_GoToDate()
        {
            var comp = Context.Render<SimpleMudDatePickerTest>();

            var datePicker = comp.FindComponent<MudDatePicker>().Instance;

            await comp.InvokeAsync(() => datePicker.GoToDate(new DateTime(2022, 03, 20)));
            await comp.WaitForAssertionAsync(() => datePicker.Date.Should().Be(new DateTime(2022, 03, 20)));

            await comp.InvokeAsync(() => datePicker.GoToDate(new DateTime(2023, 04, 21), false));
            await comp.WaitForAssertionAsync(() => datePicker.Date.Should().Be(new DateTime(2022, 03, 20)));

            await comp.InvokeAsync(() => datePicker.GoToDate(new DateTime(2023, 04, 21)));
            await comp.WaitForAssertionAsync(() => datePicker.Date.Should().Be(new DateTime(2023, 04, 21)));

            await comp.InvokeAsync(datePicker.GoToDate);
            await comp.WaitForAssertionAsync(() => datePicker.Date.Should().Be(new DateTime(2023, 04, 21)));
        }

        [Test]
        public async Task DatePicker_CheckIfMonthsAreDisabled()
        {
            var comp = Context.Render<SimpleMudDatePickerTest>();
            var datePickerComponent = comp.FindComponent<MudDatePicker>();
            var datePicker = datePickerComponent.Instance;

            await datePickerComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(parameter => parameter.MinDate, DateTime.Now.AddDays(-1))
                .Add(parameter => parameter.MaxDate, DateTime.Now.AddDays(1)));

            // Open the datepicker
            await comp.InvokeAsync(datePicker.OpenAsync);

            await comp.Find("button.mud-button-month").ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll("button.mud-picker-month").Any(x => x.IsDisabled()).Should().Be(true));

            await comp.FindAll("button.mud-picker-month").First(x => x.IsDisabled()).ClickAsync();

            var months = comp.FindAll("button.mud-picker-month");
            months.Should().NotBeNull();
            comp.Instance.Date.Should().BeNull();
        }

        [Test]
        public async Task OnPointerOver_ShouldCallJavaScriptFunction()
        {
            var comp = await OpenPicker();

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
        public async Task DatePicker_ImmediateText_Should_Callback_TextChanged()
        {
            string? changedText = null;

            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(x => x.TextChanged, x => changedText = x));

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.Editable, true)
                .Add(x => x.ImmediateText, true));

            IElement Input() => comp.Find("input");

            // This will make the input focused!
            await Input().KeyDownAsync(new KeyboardEventArgs { Key = "9", Type = "keydown" });

            // Simulate user input
            await Input().InputAsync("22");

            changedText.Should().Be("22");

            // Set ImmediateText to false
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ImmediateText, false));

            // Simulate user input
            await Input().ChangeAsync("33");

            changedText.Should().Be("33");

            // Set ImmediateText to true
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ImmediateText, true));

            // Simulate user input
            await Input().InputAsync("44");

            //changed_text should be updated
            changedText.Should().Be("44");

            // Set Editable to false.
            // ImmediateText should only work if Editable is also true.
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Editable, false));

            // Simulate user input
            await Input().ChangeAsync("55");

            changedText.Should().Be("55");
        }

        [Test]
        public async Task OldDateWithDefinedKind_SetValue_KindUnchanged()
        {
            var comp = Context.Render<MudDatePicker>();
            var picker = comp.Instance;
            var oldDate = DateTime.Now;
            var newDate = oldDate.AddDays(1);
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Date, oldDate));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Text, newDate.ToShortDateString()));

            picker.Date.Should().NotBeNull();
            picker.Date!.Value.Kind.Should().Be(oldDate.Kind);
        }

        [Test]
        public async Task Display_SelectedDate_WhenWrapped()
        {
            var comp = Context.Render<WrappedDatePickerTest>();

            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            await comp.Find(".mud-input-adornment button").ClickAsync();
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);

            await comp.SelectDateAsync("15");

            ((IHtmlInputElement)comp.FindAll("input")[0]).Value.Should().Be(comp.Instance.Picker.Text);
        }

        /// <summary>
        /// A date picker with a label should auto-generate an id and use that id on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void DatePickerWithLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.Render<MudDatePicker>(parameters =>
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
            var comp = Context.Render<MudDatePicker>(parameters
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
            var comp = Context.Render<MudDatePicker>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required DatePicker should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredDatePicker_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required DatePicker attributes should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredDatePickerAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<MudDatePicker>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Test to check if the outlined dates class shows up correctly
        /// </summary>
        [Test]
        [SetCulture("en-US")]
        public async Task DatePicker_CustomTimerProvider()
        {
            var timeProvider = Context.AddFakeTimeProvider();
            timeProvider.SetUtcNow(new DateTime(2003, 4, 4, 0, 0, 0, DateTimeKind.Utc));
            var comp = Context.Render<DatePickerCustomDateTest>();

            // click to open menu
            await comp.Find("input").ClickAsync();

            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            comp.Find(".mud-button-outlined").InnerHtml.Should().Contain("4");
            comp.Find(".mud-button-month").InnerHtml.Should().Contain("April");
            comp.Find(".mud-button-year").InnerHtml.Should().Contain("2003");
        }

        [Test]
        [SetCulture("en-US")]
        public async Task DatePickerWithFixYearAndFixMonth()
        {
            var comp = Context.Render<FixYearFixMonthTest>();
            await comp.Find("input").ClickAsync();
            await comp.WaitForAssertionAsync(() =>
            {
                comp.Find(".mud-button-year").GetInnerText().Should().Be("2022");
                comp.Find(".mud-picker-calendar-header-transition").GetInnerText().Should().Be("October 2022");
            });
        }

        [Test]
        [SetCulture("en-US")]
        public async Task DatePickerToolbar_DisplaysSelectedDate()
        {
            var selectedDate = new DateTime(2025, 1, 10);
            var comp = Context.Render<DatePickerStaticTest>(p => p.Add(x => x.Date, selectedDate));

            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("10")).ToMarkup().Should().Contain("mud-selected");
            comp.Find("button.mud-button-date .mud-button-label").InnerHtml.Should().Be("Fri, 10 Jan");
            comp.Find("button.mud-button-year .mud-button-label").InnerHtml.Should().Be("2025");

            //navigate to previous month
            await comp.Find(".mud-picker-nav-button-prev").ClickAsync();

            //toolbar should display 2025 Fri, 10 Jan
            comp.Find("button.mud-button-year .mud-button-label").InnerHtml.Should().Be("2025");
            comp.Find("button.mud-button-date .mud-button-label").InnerHtml.Should().Be("Fri, 10 Jan");
            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("10")).ToMarkup().Should().NotContain("mud-selected");

            //select new month
            await comp.Find("button.mud-button-month").ClickAsync();
            await comp.FindAll("button.mud-picker-month").First(x => x.TrimmedText().Equals("May")).ClickAsync();

            //toolbar should display 2025 Fri, 10 Jan
            comp.Find("button.mud-button-year .mud-button-label").InnerHtml.Should().Be("2025");
            comp.Find("button.mud-button-date .mud-button-label").InnerHtml.Should().Be("Fri, 10 Jan");
            comp.FindAll("button.mud-picker-calendar-day").First(x => x.TrimmedText().Equals("10")).ToMarkup().Should().NotContain("mud-selected");

            //select new year
            await comp.Find("button.mud-button-month").ClickAsync();
            await comp.Find("button.mud-picker-calendar-header-transition").ClickAsync();
            await comp.FindAll("div.mud-picker-year").First(x => x.TrimmedText().Equals("2022")).ClickAsync();

            //toolbar should display 2025 Fri, 10 Jan
            comp.Find("button.mud-button-year .mud-button-label").InnerHtml.Should().Be("2025");
            comp.Find("button.mud-button-date .mud-button-label").InnerHtml.Should().Be("Fri, 10 Jan");
        }

        [Test]
        [SetCulture("en-US")]
        public async Task DatePicker_HighlightSelectedMonthOnly()
        {
            var selectedDate = new DateTime(2025, 1, 10);
            var comp = Context.Render<DatePickerStaticTest>(p => p.Add(x => x.Date, selectedDate));

            //go to month view
            await comp.Find("button.mud-button-month").ClickAsync();

            //confirm Jan is highlighted
            comp.FindAll("button.mud-picker-month").First(x => x.TrimmedText().Equals("Jan")).ToMarkup().Should().Contain("mud-picker-month-selected");

            //select new month (March)
            await comp.FindAll("button.mud-picker-month").First(x => x.TrimmedText().Equals("Mar")).ClickAsync();
            await comp.Find("button.mud-button-month").ClickAsync();

            //confirm Jan is highlighted
            comp.FindAll("button.mud-picker-month").First(x => x.TrimmedText().Equals("Jan")).ToMarkup().Should().Contain("mud-picker-month-selected");

            //change year
            await comp.Find(".mud-picker-calendar-header-switch button[aria-label^='Previous year']").ClickAsync();
            await comp.Find(".mud-picker-calendar-header-switch button[aria-label^='Previous year']").ClickAsync();

            //confirm no month is highlighted
            comp.Find(".mud-picker-month-container").ToMarkup().Should().NotContain("mud-picker-month-selected");

            //back to present year
            await comp.Find(".mud-picker-calendar-header-switch button[aria-label^='Next year']").ClickAsync();
            await comp.Find(".mud-picker-calendar-header-switch button[aria-label^='Next year']").ClickAsync();

            //confirm Jan is highlighted
            comp.FindAll("button.mud-picker-month").First(x => x.TrimmedText().Equals("Jan")).ToMarkup().Should().Contain("mud-picker-month-selected");
        }

        [Test]
        [SetCulture("en-US")]
        public async Task DatePicker_HighlightSelectedYearOnly()
        {
            var selectedDate = new DateTime(2025, 1, 10);
            var comp = Context.Render<DatePickerStaticTest>(p => p.Add(x => x.Date, selectedDate));

            //go to year view
            await comp.Find("button.mud-button-month").ClickAsync();
            await comp.Find("button.mud-picker-calendar-header-transition").ClickAsync();

            //2025 is highlighted
            comp.FindAll("div.mud-picker-year").First(x => x.TrimmedText().Equals("2025")).ToMarkup().Should().Contain("mud-picker-year-selected");

            //select new year
            await comp.FindAll("div.mud-picker-year").First(x => x.TrimmedText().Equals("2020")).ClickAsync();
            await comp.Find("button.mud-picker-calendar-header-transition").ClickAsync();

            //2025 is still highlighted
            comp.FindAll("div.mud-picker-year").First(x => x.TrimmedText().Equals("2025")).ToMarkup().Should().Contain("mud-picker-year-selected");
        }

        [Test]
        [SetCulture("en-US")]
        public async Task DatePicker_JumpToYear()
        {
            var selectedDate = new DateTime(2025, 1, 10);
            var comp = Context.Render<DatePickerStaticTest>(p => p.Add(x => x.Date, selectedDate));
            var picker = comp.Instance;

            await comp.Find("button.mud-button-month").ClickAsync();

            //back 5 years
            await comp.Find(".mud-picker-calendar-header-switch button[aria-label^='Previous year']").ClickAsync();
            await comp.Find(".mud-picker-calendar-header-switch button[aria-label^='Previous year']").ClickAsync();
            await comp.Find(".mud-picker-calendar-header-switch button[aria-label^='Previous year']").ClickAsync();
            await comp.Find(".mud-picker-calendar-header-switch button[aria-label^='Previous year']").ClickAsync();
            await comp.Find(".mud-picker-calendar-header-switch button[aria-label^='Previous year']").ClickAsync();

            //Jump to 2020
            await comp.Find("button.mud-picker-calendar-header-transition").ClickAsync();

            picker.PickerReference.PickerMonth!.Value.Year.Should().Be(2020);
            comp.FindAll("div.mud-picker-year").First(x => x.TrimmedText().Equals("2025")).ToMarkup().Should().Contain("mud-picker-year-selected");

            //Jump to 2025
            await comp.Find("button.mud-button-year").ClickAsync();

            picker.PickerReference.PickerMonth!.Value.Year.Should().Be(2025);
            comp.FindAll("div.mud-picker-year").First(x => x.TrimmedText().Equals("2025")).ToMarkup().Should().Contain("mud-picker-year-selected");
        }

        [Test]
        public async Task DatePicker_FixYear_Past()
        {
            var comp = Context.Render<DatePickerFixYearTest>(p => p.Add(x => x.FixYear, 1900));

            // click to open menu
            await comp.Find("input").ClickAsync();

            await comp.Find("button.mud-button-month").ClickAsync();
            var pickerHeader = comp.Find(".mud-picker-calendar-header-switch");
            pickerHeader.TextContent.Trim().Should().Be("1900");
        }

        [Test]
        public async Task DatePicker_FixYear_Future()
        {
            var futureYear = DateTime.Now.Year + 1;
            var component = Context.Render<DatePickerFixYearTest>(p => p.Add(x => x.FixYear, futureYear));
            var datePickerComponent = component.FindComponent<MudDatePicker>();

            // Click to open date picker menu
            await component.Find("input").ClickAsync();

            var highlightedDate = datePickerComponent.Instance.HighlightedDate.GetValueOrDefault();
            var firstDayOfMonth = new DateTime(highlightedDate.Year, highlightedDate.Month, 1);
            // Calculate how many days from the previous month are shown at the start
            var daysFromPreviousMonth = ((int)firstDayOfMonth.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;

            // Calculate total days in the current month
            var totalDaysInMonth = DateTime.DaysInMonth(highlightedDate.Year, highlightedDate.Month);

            // Get all the date buttons in the calendar
            var dayButtons = component.FindAll(".mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-picker-calendar-day.mud-day");

            // Split the buttons into previous, current, and next month days
            var prevMonthDays = dayButtons.Take(daysFromPreviousMonth);
            var currMonthDays = dayButtons.Skip(daysFromPreviousMonth).Take(totalDaysInMonth);
            var nextMonthDays = dayButtons.Skip(daysFromPreviousMonth + totalDaysInMonth);

            // Validate hidden and visible days
            foreach (var prevMonthDay in prevMonthDays)
            {
                prevMonthDay.ClassList.Contains("mud-hidden").Should().BeTrue("Previous month days should be hidden");
            }

            foreach (var currMonthDay in currMonthDays)
            {
                currMonthDay.ClassList.Contains("mud-hidden").Should().BeFalse("Current month days should be visible");
            }

            foreach (var nextMonthDay in nextMonthDays)
            {
                nextMonthDay.ClassList.Contains("mud-hidden").Should().BeTrue("Next month days should be hidden");
            }
        }

        [Test]
        public async Task DatePicker_NavigationButtons_ShouldNotThrowExceptionAtMaxDate()
        {
            // Test that clicking next month arrow at December 9999 doesn't throw exception
            var maxDate = new DateTime(9999, 12, 1);
            var comp = Context.Render<SimpleMudDatePickerTest>(parameters => parameters
                .Add(x => x.Date, maxDate));

            var datePicker = comp.FindComponent<MudDatePicker>().Instance;
            await comp.InvokeAsync(() => datePicker.OpenAsync());

            // Verify we're at December 9999
            comp.Find("button.mud-button-year").InnerHtml.Should().Contain("9999");
            comp.Find(".mud-button-month").InnerHtml.Should().Contain("Dec");

            // Click next month button - should not throw exception
            var nextMonthButton = comp.Find(".mud-picker-nav-button-next");
            await nextMonthButton.ClickAsync();

            // Should still be at December 9999
            comp.Find("button.mud-button-year").InnerHtml.Should().Contain("9999");
            comp.Find(".mud-button-month").InnerHtml.Should().Contain("Dec");
            datePicker.PickerMonth.Should().Be(maxDate);
        }

        [Test]
        public async Task DatePicker_NavigationButtons_ShouldNotThrowExceptionAtMinDate()
        {
            // Test that clicking previous month arrow at January 0001 doesn't throw exception
            var minDate = new DateTime(1, 1, 1);
            var comp = Context.Render<SimpleMudDatePickerTest>(parameters => parameters
                .Add(x => x.Date, minDate));

            var datePicker = comp.FindComponent<MudDatePicker>().Instance;
            await comp.InvokeAsync(() => datePicker.OpenAsync());

            // Verify we're at January 0001
            comp.Find("button.mud-button-year").InnerHtml.Should().Contain("1");

            // Click previous month button - should not throw exception
            var prevMonthButton = comp.Find(".mud-picker-nav-button-prev");
            await prevMonthButton.ClickAsync();

            // Should still be at January 0001
            comp.Find("button.mud-button-year").InnerHtml.Should().Contain("1");
            datePicker.PickerMonth.Should().Be(minDate);
        }

        [Test]
        public async Task DatePicker_YearNavigationButtons_ShouldNotThrowExceptionAtMaxYear()
        {
            // Test that clicking next year arrow at year 9999 doesn't throw exception
            var maxDate = new DateTime(9999, 6, 15);
            var comp = Context.Render<SimpleMudDatePickerTest>(parameters => parameters
                .Add(x => x.Date, maxDate)
                .Add(x => x.OpenTo, OpenTo.Month));

            var datePicker = comp.FindComponent<MudDatePicker>().Instance;
            await comp.InvokeAsync(() => datePicker.OpenAsync());

            // Navigate to month view
            comp.Find("button.mud-picker-calendar-header-transition").InnerHtml.Should().Contain("9999");

            // Click next year button - should not throw exception
            var button = comp.Find("button[aria-label*='Next year']");
            await button.ClickAsync();

            // Should still be at year 9999
            comp.Find("button.mud-picker-calendar-header-transition").InnerHtml.Should().Contain("9999");
            datePicker.PickerMonth.Should().NotBeNull();
            datePicker.PickerMonth!.Value.Year.Should().Be(9999);
        }

        [Test]
        public async Task DatePicker_YearNavigationButtons_ShouldNotThrowExceptionAtMinYear()
        {
            // Test that clicking previous year arrow at year 0001 doesn't throw exception
            var minDate = new DateTime(1, 6, 15);
            var comp = Context.Render<SimpleMudDatePickerTest>(parameters => parameters
                .Add(x => x.Date, minDate)
                .Add(x => x.OpenTo, OpenTo.Month));

            var datePicker = comp.FindComponent<MudDatePicker>().Instance;
            await comp.InvokeAsync(() => datePicker.OpenAsync());

            // Navigate to month view
            comp.Find("button.mud-picker-calendar-header-transition").InnerHtml.Should().Contain("1");

            // Click previous year button - should not throw exception
            var button = comp.Find("button[aria-label*='Previous year']");
            await button.ClickAsync();

            // Should still be at year 0001
            comp.Find("button.mud-picker-calendar-header-transition").InnerHtml.Should().Contain("1");
            datePicker.PickerMonth.Should().NotBeNull();
            datePicker.PickerMonth!.Value.Year.Should().Be(1);
        }

        [Test]
        public async Task GetMonthStart_Should_NormalizeToFirstDayOfMonth()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.PickerMonth, new DateTime(2026, 2, 15))
                .Add(x => x.FirstDayOfWeek, DayOfWeek.Sunday));

            var button = comp
                .FindAll(".mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-picker-calendar-day.mud-day")
                .Single(x => x.GetAttribute("style") == "--day-id: 1;");

            button.TextContent.Should().Be("1");
        }

        [Test]
        public void DatePicker_CustomClearIcon_Should_BeRenderedInMarkup()
        {
            var comp = Context.Render<MudDatePicker>(parameters => parameters
                .Add(p => p.Date, new DateTime(2026, 2, 15))
                .Add(p => p.Editable, true)
                .Add(p => p.Clearable, true)
                .Add(p => p.ClearIcon, Icons.Custom.Brands.MudBlazor));

            comp.Markup.Should().Contain(comp.Instance.ClearIcon);
        }

        private async Task<IRenderedComponent<SimpleMudDatePickerTest>> OpenPicker(Action<ComponentParameterCollectionBuilder<SimpleMudDatePickerTest>>? parameterBuilder = null)
        {
            IRenderedComponent<SimpleMudDatePickerTest> comp;
            if (parameterBuilder is null)
            {
                comp = Context.Render<SimpleMudDatePickerTest>();
            }
            else
            {
                comp = Context.Render<SimpleMudDatePickerTest>(parameterBuilder);
            }

            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to open menu
            await comp.Find("input").ClickAsync();
            // now its open
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
            return comp;
        }

        private async Task<IRenderedComponent<SimpleMudDatePickerTest>> OpenTo12ThMonth()
        {
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.PickerMonth, new DateTime(DateTime.Now.Year, 12, 01)));
            comp.Instance.PickerMonth?.Month.Should().Be(12);
            return comp;
        }
    }
}
