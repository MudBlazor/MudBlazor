#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DateRangePickerTests : BunitTest
    {
        [Test]
        public void Default()
        {
            var comp = Context.RenderComponent<MudDateRangePicker>();
            var picker = comp.Instance;

            picker.Text.Should().Be(null);
            picker.DateRange.Should().Be(null);
            picker.MaxDate.Should().Be(null);
            picker.MinDate.Should().Be(null);
            picker.OpenTo.Should().Be(OpenTo.Date);
            picker.FirstDayOfWeek.Should().Be(null);
            picker.ClosingDelay.Should().Be(100);
            picker.DisplayMonths.Should().Be(2);
            picker.MaxMonthColumns.Should().Be(null);
            picker.StartMonth.Should().Be(null);
            picker.ShowWeekNumbers.Should().BeFalse();
            picker.AutoClose.Should().BeFalse();
            picker.FixYear.Should().Be(null);
            picker.FixMonth.Should().Be(null);
            picker.FixDay.Should().Be(null);
            picker.PlaceholderStart.Should().Be(null);
            picker.PlaceholderEnd.Should().Be(null);
            picker.SeparatorIcon.Should().Be(Icons.Material.Filled.ArrowRightAlt);
        }

        [Test]
        public void DateRangePickerPlaceHolders()
        {
            var comp = Context.RenderComponent<MudDateRangePicker>();
            comp.SetParametersAndRender(
                parameters =>
                parameters
                .Add(picker => picker.PlaceholderStart, "Start")
                .Add(picker => picker.PlaceholderEnd, "End")
                );

            var startInput = comp.Find("input").Attributes["placeholder"].Value.Should().Be("Start");
            var endInput = comp.FindAll("input").Skip(1).First().Attributes["placeholder"].Value.Should().Be("End");
        }

        [Test]
        public void DateRangePickerSeparatorIcon()
        {
            var newIcon = Icons.Material.Filled.Star;
            var comp = Context.RenderComponent<MudDateRangePicker>();
            comp.SetParametersAndRender(
                parameters =>
                parameters
                .Add(picker => picker.SeparatorIcon, newIcon)
                );
            var markup = comp.Markup;

            // Only check first svg section
            string startText = "<svg", endText = "</svg>";
            var sectionStart = markup.IndexOf(startText);
            var length = markup.IndexOf(endText) - sectionStart + endText.Length;
            var section = markup.Substring(sectionStart, length);

            section.Should().Contain(newIcon);
        }

        [Test]
        public void DateRangePickerOpenButtonAriaLabel()
        {
            var comp = Context.RenderComponent<MudDateRangePicker>();
            var openButton = comp.Find(".mud-input-adornment button");
            openButton.Attributes.GetNamedItem("aria-label")?.Value.Should().Be("Open Date Range Picker");
        }

        [Test]
        public void DateRangePicker_Preset_No_Timestamp()
        {
            var comp = Context.RenderComponent<DateRangePickerPresetWithoutTimestampTest>();

            comp.Markup.Should().Contain("mud-range-start-selected");
            comp.Markup.Should().Contain("mud-range-end-selected");
        }

        [Test]
        public void DateRangePicker_Preset_Timestamp()
        {
            var comp = Context.RenderComponent<DateRangePickerPresetRangeWithTimestampTest>();

            comp.Markup.Should().Contain("mud-range-start-selected");
            comp.Markup.Should().Contain("mud-range-end-selected");
        }

        [Test]
        public void DateRangePickerLabelFor()
        {
            var comp = Context.RenderComponent<DateRangePickerValidationTest>();
            var label = comp.Find(".mud-input-label");
            label.Attributes.GetNamedItem("for")?.Value.Should().Be("dateRangeLabelTest");
        }

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
                await comp.InvokeAsync(() => datepicker.OpenAsync());
                await comp.InvokeAsync(() => datepicker.CloseAsync());
            }
            watch.Stop();
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
        public void RangeConverter_RoundTrip_Ok()
        {
            var d1 = "val1";
            var d2 = "val2";

            var repr = RangeConverter<DateTime>.Join(d1, d2);
            RangeConverter<DateTime>.Split(repr, out var c1, out var c2).Should().BeTrue();

            c1.Should().Be(d1);
            c2.Should().Be(d2);
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

        public IRenderedComponent<SimpleMudMudDateRangePickerTest> OpenPicker(ComponentParameter parameter)
        {
            return OpenPicker(new ComponentParameter[] { parameter });
        }

        public IRenderedComponent<SimpleMudMudDateRangePickerTest> OpenPicker(ComponentParameter[] parameters = null)
        {
            IRenderedComponent<SimpleMudMudDateRangePickerTest> comp;
            if (parameters is null)
            {
                comp = Context.RenderComponent<SimpleMudMudDateRangePickerTest>();
            }
            else
            {
                comp = Context.RenderComponent<SimpleMudMudDateRangePickerTest>(parameters);
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

        public IRenderedComponent<SimpleMudMudDateRangePickerTest> OpenTo12thMonth()
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
            var comp = Context.RenderComponent<SimpleMudMudDateRangePickerTest>();
            comp.FindAll("div.mud-picker-content").Count.Should().Be(0);
            // open programmatically
            await comp.Instance.Open();
            comp.FindAll("div.mud-picker-content").Count.Should().Be(1);
            // closing programmatically
            await comp.Instance.Close();

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
                .Should().OnlyContain(disabled => disabled);
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
        public void AdditionalDateClassesFunc_ClassIsAdded()
        {
            Func<DateTime, string> additionalDateClassesFunc = date => "__addedtestclass__";

            var comp = OpenPicker(Parameter(nameof(MudDateRangePicker.AdditionalDateClassesFunc), additionalDateClassesFunc));

            var daysCount = comp.FindAll("button.mud-picker-calendar-day")
                                .Select(button => (IHtmlButtonElement)button)
                                .Count();

            comp.FindAll("button.mud-day")
                .Where(button => ((IHtmlButtonElement)button).ClassName.Contains("__addedtestclass__"))
                .Should().HaveCount(daysCount);
        }

        [Test]
        public void SetRangeTextFunc_NullInputNoError()
        {
            var comp = Context.RenderComponent<MudDateRangePicker>(parameters =>
                parameters.Add(p => p.DateRange,
                    new DateRange(new DateTime(2020, 12, 26), null)));
            comp.Find("input").Change("");
            comp.Instance.DateRange.End.Should().BeNull();
            comp.Instance.DateRange.Start.Should().BeNull();

        }

        [Test]
        public void SetRangeTextFunc_NullRangeTextNoError()
        {
            var dateTime = new DateTime(2020, 12, 26);
            var comp = Context.RenderComponent<MudDateRangePicker>(parameters =>
                parameters.Add(p => p.DateRange, null)
                    .Add(p => p.Culture, CultureInfo.CurrentCulture));
            comp.Find("input").Change(dateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern));
            comp.Instance.DateRange.Start.Should().Be(dateTime);

        }


        [Test]
        public void SetDateRange_NoChangedIfSameValues()
        {
            var dr1 = new DateRange(new DateTime(2021, 10, 08), new DateTime(2021, 10, 09));
            var dr2 = new DateRange(new DateTime(2021, 10, 08), new DateTime(2021, 10, 09));

            var wasEventCallbackCalled = false;

            var comp = Context.RenderComponent<MudDateRangePicker>(
                Parameter(nameof(MudDateRangePicker.DateRange), dr1),
                EventCallback(nameof(MudDateRangePicker.DateRangeChanged), (DateRange _) => wasEventCallbackCalled = true));

            comp.SetParam(nameof(MudDateRangePicker.DateRange), dr2);

            comp.Instance.DateRange.Should().Be(dr2);
            wasEventCallbackCalled.Should().BeFalse();
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

        [Test]
        public async Task DateRangePicker_RequiredValidation()
        {
            // define some "constant" values
            var errorMessage = "A valid date has to be picked";
            var startDate = DateTime.Now.Date;
            var endDate = DateTime.Now.Date.AddDays(2);

            // create the component
            var dateRangePickerComponent = Context.RenderComponent<MudDateRangePicker>(Parameter(nameof(MudDateRangePicker.Required), true), Parameter(nameof(MudDateRangePicker.RequiredError), errorMessage));

            // select the instance to work with
            var dateRangePickerInstance = dateRangePickerComponent.Instance;

            // assert default's
            dateRangePickerInstance.Text.Should().BeNullOrEmpty();
            dateRangePickerInstance.DateRange.Should().Be(null);

            // validated the picker
            await dateRangePickerComponent.InvokeAsync(() => dateRangePickerInstance.Validate());
            dateRangePickerInstance.Error.Should().BeTrue("Value is required and should be handled as invalid");
            dateRangePickerInstance.ErrorText.Should().Be(errorMessage);

            // set a value
            await dateRangePickerComponent.InvokeAsync(() => dateRangePickerInstance.Text = RangeConverter<DateTime>.Join(startDate.ToShortDateString(), endDate.ToShortDateString()));

            // asert new values have been applied
            dateRangePickerInstance.DateRange.Start.Should().Be(startDate);
            dateRangePickerInstance.DateRange.End.Should().Be(endDate);
            dateRangePickerInstance.Error.Should().BeFalse("Value has been set and should be handled as valid");
            dateRangePickerInstance.ErrorText.Should().BeNullOrWhiteSpace();

            // reset value
            await dateRangePickerComponent.InvokeAsync(() => dateRangePickerInstance.ClearAsync());

            // assert values have benn nulled
            dateRangePickerInstance.Text.Should().BeNullOrEmpty();
            dateRangePickerInstance.DateRange.Should().Be(null);
            dateRangePickerInstance.Error.Should().BeTrue("Value has been cleared and should be handled as invalid");
            dateRangePickerInstance.ErrorText.Should().Be(errorMessage);
        }

        [Test]
        public void CheckAutoCloseDateRangePicker_DoNotCloseWhenValueIsOff()
        {
            // Define a date range for comparison
            var initialDateRange = new DateRange(
               new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01),
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 02));

            // Get access to the date range picker of the instance
            var comp = Context.RenderComponent<AutoCloseDateRangePickerTest>(
                Parameter(nameof(AutoCloseDateRangePickerTest.DateRange), initialDateRange));

            // Open the date range picker
            comp.Find("input").Click();

            // Clicking day buttons to select a date range
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("10")).First().Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("11")).First().Click();

            // Check that the date range should remain the same because autoclose is false even when actions are defined
            comp.Instance.DateRange.Should().Be(initialDateRange);
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover").Count.Should().Be(1));
        }

        [Test]
        public void CheckAutoCloseDateRangePicker_CloseWhenValueIsOn()
        {
            // Define a date range for comparison
            var initialDateRange = new DateRange(
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01),
                  new DateTime(DateTime.Now.Year, DateTime.Now.Month, 02));

            // Get access to the date range picker of the instance
            var comp = Context.RenderComponent<AutoCloseDateRangePickerTest>(
                Parameter(nameof(AutoCloseDateRangePickerTest.DateRange), initialDateRange),
                Parameter(nameof(AutoCloseDateRangePickerTest.AutoClose), true));

            // Open the date range picker
            comp.Find("input").Click();

            // Clicking day buttons to select a date range
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("10")).First().Click();
            comp.FindAll("button.mud-picker-calendar-day")
                .Where(x => x.TrimmedText().Equals("11")).First().Click();

            // Check that the date range is changed because autoclose is true even when actions are defined
            comp.Instance.DateRange.Should().NotBe(initialDateRange);
            comp.Instance.DateRange.Should().Be(new DateRange(
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10),
                  new DateTime(DateTime.Now.Year, DateTime.Now.Month, 11)));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover").Count.Should().Be(1));
        }

        [Test]
        public void CurrentDate_ShouldBeMarked()
        {
            var currentDate = DateTime.Now.Date;
            var comp = OpenPicker();

            // Check that only one date is marked
            comp.FindAll("button.mud-current").Count.Should().Be(1);

            // Check that the marked date is the current date
            comp.Find("button.mud-current").Click();
            comp.Find("button.mud-range-start-selected").Click();
            comp.Instance.DateRange.Should().Be(new DateRange(currentDate, currentDate));
        }

        [Test]
        public void DateRangePicker_Should_Clear()
        {
            var comp = Context.RenderComponent<MudDateRangePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.DateRange.Should().Be(null);
            comp.SetParam(p => p.Clearable, true);
            comp.SetParam(p => p.DateRange, new DateRange(new DateTime(2020, 10, 26), new DateTime(2020, 10, 29)));
            picker.DateRange.Should().Be(new DateRange(new DateTime(2020, 10, 26), new DateTime(2020, 10, 29)));

            comp.Find("button").Click(); //clear the input

            picker.DateRange.Should().Be(new DateRange(null, null));
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

        /// <summary>
        /// Optional DateRangePicker should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalDateRangePicker_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.RenderComponent<MudDateRangePicker>();

            comp.FindAll("input").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeFalse();
                input.GetAttribute("aria-required").Should().Be("false");
            });
        }

        /// <summary>
        /// Required DateRangePicker should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredDateRangePicker_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.RenderComponent<MudDateRangePicker>(parameters => parameters
                .Add(p => p.Required, true));

            comp.FindAll("input").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeTrue();
                input.GetAttribute("aria-required").Should().Be("true");
            });
        }

        /// <summary>
        /// Required and aria-required DateRangePicker attributes should be dynamic.
        /// </summary>
        [Test]
        public void RequiredAndAriaRequiredDateRangePickerAttributes_Should_BeDynamic()
        {
            var comp = Context.RenderComponent<MudDateRangePicker>();

            comp.FindAll("input").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeFalse();
                input.GetAttribute("aria-required").Should().Be("false");
            });

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.Required, true));

            comp.FindAll("input").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeTrue();
                input.GetAttribute("aria-required").Should().Be("true");
            });
        }

        [Test]
        public void FormatFirst_Should_RenderCorrectly()
        {
            DateRange range = new DateRange(new DateTime(2024, 04, 22), new DateTime(2024, 04, 23));
            var comp = Context.RenderComponent<DateRangePickerFormatTest>
            (parameters =>
            {
                parameters.Add(p => p.DateRange, range);
                parameters.Add(p => p.FormatFirst, true);
            });
            var instance = comp.FindComponent<MudDateRangePicker>().Instance;
            instance.DateRange.Should().Be(range);
            instance.DateFormat.Should().Be("yyyy MMMM dd");
            comp.Markup.Should().Contain("2024 April 22");
            comp.Markup.Should().Contain("2024 April 23");
        }

        [Test]
        public void FormatLast_Should_RenderCorrectly()
        {
            DateRange range = new DateRange(new DateTime(2024, 04, 22), new DateTime(2024, 04, 23));
            var comp = Context.RenderComponent<DateRangePickerFormatTest>
            (parameters =>
            {
                parameters.Add(p => p.DateRange, range);
                parameters.Add(p => p.FormatFirst, false);
            });
            var instance = comp.FindComponent<MudDateRangePicker>().Instance;
            instance.DateRange.Should().Be(range);
            instance.DateFormat.Should().Be("yyyy MMMM dd");
            comp.Markup.Should().Contain("2024 April 22");
            comp.Markup.Should().Contain("2024 April 23");
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void CheckCloseOnClearDateRangePicker(bool closeOnClear)
        {
            // Define a date range for comparison
            var initialDateRange = new DateRange(
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01),
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 02));

            // Get access to the date range picker of the instance
            var comp = Context.RenderComponent<DateRangePickerCloseOnClearTest>(
                Parameter(nameof(DateRangePickerCloseOnClearTest.DateRange), initialDateRange),
                Parameter(nameof(DateRangePickerCloseOnClearTest.CloseOnClear), closeOnClear));

            // Open the date range picker
            comp.Find("input").Click();

            // Clicking day buttons to select a date range
            comp
                .FindAll("button.mud-button").First(x => x.TrimmedText().Equals("Clear")).Click();

            // Check that the date range was cleared
            comp.Instance.DateRange.Should().NotBe(initialDateRange);
            if (closeOnClear)
            {
                // Check that the component is closed
                comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            }
            else
            {
                // Check that the component is open
                comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            }
        }

        [Test]
        public void Should_respect_underline_parameter()
        {
            var underlinedComp = Context.RenderComponent<MudDateRangePicker>(parameters
                => parameters.Add(p => p.Underline, true));
            var notUnderlinedComp = Context.RenderComponent<MudDateRangePicker>(parameters
                => parameters.Add(p => p.Underline, false));

            underlinedComp.FindAll(".mud-input-underline").Should().HaveCount(1);
            notUnderlinedComp.FindAll(".mud-input-underline").Should().HaveCount(0);
        }
    }
}
