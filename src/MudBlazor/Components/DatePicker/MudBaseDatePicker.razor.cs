using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract partial class MudBaseDatePicker : MudPicker<DateTime?>
    {
        protected MudBaseDatePicker() : base(new DefaultConverter<DateTime?>
        {
            Format = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern,
            Culture = CultureInfo.CurrentCulture
        })
        { }

        [Inject] protected IScrollManager ScrollManager { get; set; }

        /// <summary>
        /// Max selectable date.
        /// </summary>
        [Parameter] public DateTime? MaxDate { get; set; }

        /// <summary>
        /// Max selectable date.
        /// </summary>
        [Parameter] public DateTime? MinDate { get; set; }

        /// <summary>
        /// First view to show in the MudDatePicker.
        /// </summary>
        [Parameter] public OpenTo OpenTo { get; set; } = OpenTo.Date;

        /// <summary>
        /// String Format for selected date view
        /// </summary>
        [Parameter]
        public string DateFormat
        {
            get
            {
                return (Converter as DefaultConverter<DateTime?>)?.Format;
            }
            set
            {
                if (Converter is DefaultConverter<DateTime?> defaultConverter)
                    defaultConverter.Format = value;
                DateFormatChanged(value);
            }
        }

        /// <summary>
        /// Date format value change hook for descendants.
        /// </summary>
        protected virtual Task DateFormatChanged(string newFormat)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Defines on which day the week starts. Depends on the value of Culture. 
        /// </summary>
        [Parameter] public DayOfWeek? FirstDayOfWeek { get; set; } = null;

        /// <summary>
        /// The current month of the date picker (two-way bindable). This changes when the user browses through the calender.
        /// The month is represented as a DateTime which is always the first day of that month. You can also set this to define which month is initially shown. If not set, the current month is shown.
        /// </summary>
        [Parameter]
        public DateTime? PickerMonth
        {
            get => _picker_month;
            set
            {
                if (value == _picker_month)
                    return;
                _picker_month = value;
                InvokeAsync(StateHasChanged);
                PickerMonthChanged.InvokeAsync(value);
            }
        }

        private DateTime? _picker_month;

        /// <summary>
        /// Fired when the date changes.
        /// </summary>
        [Parameter] public EventCallback<DateTime?> PickerMonthChanged { get; set; }

        /// <summary>
        /// Milliseconds to wait before closing the picker. This helps the user see that the date was selected before the popover disappears.
        /// </summary>
        [Parameter] public int ClosingDelay { get; set; } = 100;

        /// <summary>
        /// Number of months to display in the calendar
        /// </summary>
        [Parameter] public int DisplayMonths { get; set; } = 1;

        /// <summary>
        /// Maximum number of months in one row
        /// </summary>
        [Parameter] public int? MaxMonthColumns { get; set; }

        /// <summary>
        /// Start month when opening the picker. 
        /// </summary>
        [Parameter] public DateTime? StartMonth { get; set; }

        /// <summary>
        /// Display week numbers according to the Culture parameter. If no culture is defined, CultureInfo.CurrentCulture will be used.
        /// </summary>
        [Parameter] public bool ShowWeekNumbers { get; set; }

        /// <summary>
        /// Format of the selected date in the title. By default, this is "ddd, dd MMM" which abbreviates day and month names. 
        /// For instance, display the long names like this "dddd, dd. MMMM". 
        /// </summary>
        [Parameter] public string TitleDateFormat { get; set; } = "ddd, dd MMM";

        /// <summary>
        /// Function to determine whether a date is disabled
        /// </summary>
        [Parameter]
        public Func<DateTime, bool> IsDateDisabledFunc
        {
            get => _isDateDisabledFunc;
            set
            {
                _isDateDisabledFunc = value ?? (_ => false);
            }
        }
        private Func<DateTime, bool> _isDateDisabledFunc = _ => false;

        /// <summary>
        /// Custom previous icon.
        /// </summary>
        [Parameter] public string PreviousIcon { get; set; } = Icons.Material.Filled.ChevronLeft;

        /// <summary>
        /// Custom next icon.
        /// </summary>
        [Parameter] public string NextIcon { get; set; } = Icons.Material.Filled.ChevronRight;

        protected virtual bool IsRange { get; } = false;

        private OpenTo _currentView;

        protected override void OnPickerOpened()
        {
            base.OnPickerOpened();
            _currentView = OpenTo;
            if (_currentView == OpenTo.Year)
                _scrollToYearAfterRender = true;
        }

        /// <summary>
        /// Get the first of the month to display
        /// </summary>
        /// <returns></returns>
        protected DateTime GetMonthStart(int month)
        {
            var monthStartDate = _picker_month ?? DateTime.Today.StartOfMonth(Culture);
            // Return the min supported datetime of the calendar when this is year 1 and first month!
            if (_picker_month.HasValue && _picker_month.Value.Year == 1 && _picker_month.Value.Month == 1)
            {
                return Culture.Calendar.MinSupportedDateTime;
            }
            return Culture.Calendar.AddMonths(monthStartDate, month);
        }

        /// <summary>
        /// Get the last of the month to display
        /// </summary>
        /// <returns></returns>
        protected DateTime GetMonthEnd(int month)
        {
            var monthStartDate = _picker_month ?? DateTime.Today.StartOfMonth(Culture);
            return Culture.Calendar.AddMonths(monthStartDate, month).EndOfMonth(Culture);
        }

        protected DayOfWeek GetFirstDayOfWeek()
        {
            if (FirstDayOfWeek.HasValue)
                return FirstDayOfWeek.Value;
            return Culture.DateTimeFormat.FirstDayOfWeek;
        }

        /// <summary>
        /// Gets the n-th week of the currently displayed month. 
        /// </summary>
        /// <param name="month">offset from _picker_month</param>
        /// <param name="index">between 0 and 4</param>
        /// <returns></returns>
        protected IEnumerable<DateTime> GetWeek(int month, int index)
        {
            if (index is < 0 or > 5)
                throw new ArgumentException("Index must be between 0 and 5");
            var month_first = GetMonthStart(month);
            var week_first = month_first.AddDays(index * 7).StartOfWeek(GetFirstDayOfWeek());
            for (var i = 0; i < 7; i++)
                yield return week_first.AddDays(i);
        }

        private string GetWeekNumber(int month, int index)
        {
            if (index is < 0 or > 5)
                throw new ArgumentException("Index must be between 0 and 5");
            var month_first = GetMonthStart(month);
            var week_first = month_first.AddDays(index * 7).StartOfWeek(GetFirstDayOfWeek());
            //january 1st
            if (month_first.Month == 1 && index == 0)
            {
                week_first = month_first;
            }

            if (week_first.Month != month_first.Month && week_first.AddDays(6).Month != month_first.Month)
                return "";

            return Culture.Calendar.GetWeekOfYear(week_first,
                Culture.DateTimeFormat.CalendarWeekRule, FirstDayOfWeek ?? Culture.DateTimeFormat.FirstDayOfWeek).ToString();
        }

        protected abstract string GetDayClasses(int month, DateTime day);

        /// <summary>
        /// User clicked on a day
        /// </summary>
        protected abstract void OnDayClicked(DateTime dateTime);

        /// <summary>
        /// return Mo, Tu, We, Th, Fr, Sa, Su in the right culture
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<string> GetAbbreviatedDayNames()
        {
            var dayNamesNormal = Culture.DateTimeFormat.AbbreviatedDayNames;
            var dayNamesShifted = Shift(dayNamesNormal, (int)GetFirstDayOfWeek());
            return dayNamesShifted;
        }

        /// <summary>
        /// Shift array and cycle around from the end
        /// </summary>
        private static T[] Shift<T>(T[] array, int positions)
        {
            var copy = new T[array.Length];
            Array.Copy(array, 0, copy, array.Length - positions, positions);
            Array.Copy(array, positions, copy, 0, array.Length - positions);
            return copy;
        }

        protected string GetMonthName(int month)
        {
            return GetMonthStart(month).ToString(Culture.DateTimeFormat.YearMonthPattern, Culture);
        }

        protected abstract string GetTitleDateString();

        protected string FormatTitleDate(DateTime? date)
        {
            return date?.ToString(TitleDateFormat ?? "ddd, dd MMM", Culture) ?? "";
        }

        protected string GetFormattedYearString()
        {
            return GetMonthStart(0).ToString("yyyy", Culture);
        }

        private void OnPreviousMonthClick()
        {
            // It is impossible to go further into the past after the first year and the first month!
            if (PickerMonth.HasValue && PickerMonth.Value.Year == 1 && PickerMonth.Value.Month == 1)
            {
                return;
            }
            PickerMonth = GetMonthStart(0).AddDays(-1).StartOfMonth(Culture);
        }

        private void OnNextMonthClick()
        {
            PickerMonth = GetMonthEnd(0).AddDays(1);
        }

        private void OnPreviousYearClick()
        {
            PickerMonth = GetMonthStart(0).AddYears(-1);
        }

        private void OnNextYearClick()
        {
            PickerMonth = GetMonthStart(0).AddYears(1);
        }

        private void OnYearClick()
        {
            _currentView = OpenTo.Year;
            StateHasChanged();
            _scrollToYearAfterRender = true;
        }

        /// <summary>
        /// We need a random id for the year items in the year list so we can scroll to the item safely in every DatePicker.
        /// </summary>
        private string _componentId = Guid.NewGuid().ToString();

        /// <summary>
        /// Is set to true to scroll to the actual year after the next render
        /// </summary>
        private bool _scrollToYearAfterRender = false;

        public async void ScrollToYear()
        {
            _scrollToYearAfterRender = false;
            var id = $"{_componentId}{GetMonthStart(0).Year}";
            await ScrollManager.ScrollToYearAsync(id);
            StateHasChanged();
        }

        private int GetMinYear()
        {
            if (MinDate.HasValue)
                return MinDate.Value.Year;
            return DateTime.Today.Year - 100;
        }

        private int GetMaxYear()
        {
            if (MaxDate.HasValue)
                return MaxDate.Value.Year;
            return DateTime.Today.Year + 100;
        }

        private string GetYearClasses(int year)
        {
            if (year == GetMonthStart(0).Year)
                return $"mud-picker-year-selected mud-{Color.ToDescriptionString()}-text";
            return null;
        }

        private string GetCalendarHeaderClasses(int month)
        {
            return new CssBuilder("mud-picker-calendar-header")
                .AddClass($"mud-picker-calendar-header-{month + 1}")
                .AddClass($"mud-picker-calendar-header-last", month == DisplayMonths - 1)
                .Build();
        }

        private Typo GetYearTypo(int year)
        {
            if (year == GetMonthStart(0).Year)
                return Typo.h5;
            return Typo.subtitle1;
        }

        private void OnFormattedDateClick()
        {
            // todo: raise an event the user can handle
        }

        private void OnYearClicked(int year)
        {
            _currentView = OpenTo.Month;
            var current = GetMonthStart(0);
            PickerMonth = new DateTime(year, current.Month, 1);
        }

        private IEnumerable<DateTime> GetAllMonths()
        {
            var current = GetMonthStart(0);
            var calendarYear = Culture.Calendar.GetYear(current);
            var firstOfCalendarYear = Culture.Calendar.ToDateTime(calendarYear, 1, 1, 0, 0, 0, 0);
            for (var i = 0; i < Culture.Calendar.GetMonthsInYear(calendarYear); i++)
                yield return Culture.Calendar.AddMonths(firstOfCalendarYear, i);
        }

        private string GetAbbreviatedMonthName(DateTime month)
        {
            var calendarMonth = Culture.Calendar.GetMonth(month);
            return Culture.DateTimeFormat.AbbreviatedMonthNames[calendarMonth - 1];
        }

        private string GetMonthName(DateTime month)
        {
            var calendarMonth = Culture.Calendar.GetMonth(month);
            return Culture.DateTimeFormat.MonthNames[calendarMonth - 1];
        }

        private string GetMonthClasses(DateTime month)
        {
            if (GetMonthStart(0) == month)
                return $"mud-picker-month-selected mud-{Color.ToDescriptionString()}-text";
            return null;
        }

        private Typo GetMonthTypo(DateTime month)
        {
            if (GetMonthStart(0) == month)
                return Typo.h5;
            return Typo.subtitle1;
        }

        private void OnMonthClicked(int month)
        {
            _currentView = OpenTo.Month;
            _picker_month = _picker_month?.AddMonths(month);
            StateHasChanged();
        }

        private void OnMonthSelected(DateTime month)
        {
            _currentView = OpenTo.Date;
            PickerMonth = month;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _currentView = OpenTo;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _picker_month ??= GetCalendarStartOfMonth();
            }

            if (firstRender && _currentView == OpenTo.Year)
            {
                ScrollToYear();
                return;
            }

            if (_scrollToYearAfterRender)
                ScrollToYear();
            await base.OnAfterRenderAsync(firstRender);
        }

        protected abstract DateTime GetCalendarStartOfMonth();

        private int GetCalendarDayOfMonth(DateTime date)
        {
            return Culture.Calendar.GetDayOfMonth(date);
        }

        /// <summary>
        /// Converts gregorian year into whatever year it is in the provided culture
        /// </summary>
        /// <param name="year">Gregorian year</param>
        /// <returns>Year according to culture</returns>
        protected abstract int GetCalendarYear(int year);
    }
}
