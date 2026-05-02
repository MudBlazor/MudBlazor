using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a base class for designing date picker components.
    /// </summary>
    /// <typeparam name="TValue">The date type bound by the picker. Supported: <see cref="DateTime"/>, <see cref="DateTime"/>?, <see cref="DateOnly"/>, <see cref="DateOnly"/>?, <see cref="DateTimeOffset"/>, <see cref="DateTimeOffset"/>?.</typeparam>
    public abstract partial class MudBaseDatePicker<TValue> : MudPicker<TValue>
    {
        protected static readonly Type _underlyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

        private readonly string _mudPickerCalendarContentElementId;
        private readonly ParameterState<string?> _dateFormatState;

        /// <summary>
        /// Convert a public-API <typeparamref name="TValue"/> value into a <see cref="DateTime"/> for internal calendar math.
        /// </summary>
        protected static DateTime? ToDateTime(TValue? value)
        {
            if (value is null) return null;
            if (_underlyingType == typeof(DateTime))       return (DateTime)(object)value;
            if (_underlyingType == typeof(DateOnly))       return ((DateOnly)(object)value).ToDateTime(TimeOnly.MinValue);
            if (_underlyingType == typeof(DateTimeOffset)) return ((DateTimeOffset)(object)value).DateTime;
            throw new InvalidOperationException($"MudBaseDatePicker does not support TValue = {typeof(TValue)}. Use DateTime, DateOnly, or DateTimeOffset (or their nullable variants).");
        }

        /// <summary>
        /// Convert an internal <see cref="DateTime"/> back to <typeparamref name="TValue"/>.
        /// For <see cref="DateTimeOffset"/>, the picker-local offset (<see cref="TimeProvider.GetLocalNow"/>) is used unless a concrete subclass preserves the user's original offset.
        /// </summary>
        protected TValue? FromDateTime(DateTime? value)
        {
            if (value is null) return default;
            if (_underlyingType == typeof(DateTime))       return (TValue)(object)value.Value;
            if (_underlyingType == typeof(DateOnly))       return (TValue)(object)DateOnly.FromDateTime(value.Value);
            if (_underlyingType == typeof(DateTimeOffset)) return (TValue)(object)new DateTimeOffset(value.Value, TimeProvider.GetLocalNow().Offset);
            throw new InvalidOperationException($"MudBaseDatePicker does not support TValue = {typeof(TValue)}. Use DateTime, DateOnly, or DateTimeOffset (or their nullable variants).");
        }

        protected MudBaseDatePicker()
        {
            _mudPickerCalendarContentElementId = Identifier.Create();
            Culture = CultureInfo.CurrentCulture;

            using var registerScope = CreateRegisterScope();
            _dateFormatState = registerScope.RegisterParameter<string?>(nameof(DateFormat))
                .WithParameter(() => DateFormat)
                .WithChangeHandler(DateFormatChangedAsync);
        }

        [Inject]
        protected IScrollManager ScrollManager { get; set; } = null!;

        [Inject]
        private IJsApiService JsApiService { get; set; } = null!;

        [Inject]
        protected TimeProvider TimeProvider { get; set; } = null!;

        /// <summary>
        /// The maximum selectable date.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public TValue? MaxDate { get; set; }

        /// <summary>
        /// The minimum selectable date.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public TValue? MinDate { get; set; }

        /// <summary>
        /// The initial view to display.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="OpenTo.Date"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public OpenTo OpenTo { get; set; } = OpenTo.Date;

        /// <summary>
        /// The format for selected dates.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? DateFormat { get; set; }

        /// <summary>
        /// Occurs when the <see cref="DateFormat"/> has changed.
        /// </summary>
        protected virtual Task DateFormatChangedAsync(string? newFormat)
        {
            return Task.CompletedTask;
        }

        private Task DateFormatChangedAsync(ParameterChangedEventArgs<string?> args) => DateFormatChangedAsync(args.Value);

        /// <summary>
        /// The day representing the first day of the week.
        /// </summary>
        /// <remarks>
        /// Defaults to the current culture's <c>DateTimeFormat.FirstDayOfWeek</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public DayOfWeek? FirstDayOfWeek { get; set; }

        /// <summary>
        /// The current month shown in the date picker.
        /// </summary>
        /// <remarks>
        /// Defaults to the current month.<br />
        /// When bound via <c>@bind-PickerMonth</c>, controls the initial month displayed.  This value is always the first day of a month.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public TValue? PickerMonth
        {
            get => FromDateTime(_picker_month);
            set
            {
                var newValue = ToDateTime(value);
                if (newValue == _picker_month)
                    return;
                _picker_month = newValue;
                InvokeAsync(StateHasChanged);
                PickerMonthChanged.InvokeAsync(value);
            }
        }

        private DateTime? _picker_month;

        /// <summary>
        /// Represents the currently selected date
        /// </summary>
        /// <remarks>
        /// This date is highlighted in the UI
        /// </remarks>
        protected internal DateTime? HighlightedDate { get; set; }

        /// <summary>
        /// Occurs when <see cref="PickerMonth"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<TValue?> PickerMonthChanged { get; set; }

        /// <summary>
        /// The delay, in milliseconds, before closing the picker after a value is selected.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>100</c>.<br />
        /// This delay helps the user see that a date has been selected before the popover disappears.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int ClosingDelay { get; set; } = 100;

        /// <summary>
        /// The number of months to display in the calendar.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int DisplayMonths { get; set; } = 1;

        /// <summary>
        /// The maximum number of months allowed in one row.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.<br />
        /// When <c>null</c>, the <see cref="DisplayMonths"/> is used.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public int? MaxMonthColumns { get; set; }

        /// <summary>
        /// The start month when opening the picker.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public TValue? StartMonth { get; set; }

        /// <summary>
        /// Shows week numbers at the start of each week.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowWeekNumbers { get; set; }

        /// <summary>
        /// The format of the selected date in the title.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>ddd, dd MMM</c>.<br />
        /// Supported date formats can be found here: <see href="https://learn.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public string TitleDateFormat { get; set; } = "ddd, dd MMM";

        /// <summary>
        /// Closes this picker when a value is selected.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool AutoClose { get; set; }

        /// <summary>
        /// The function used to disable one or more dates.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.<br />
        /// When set, a date will be disabled if the function returns <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public Func<TValue, bool> IsDateDisabledFunc { get; set; } = _ => false;

        /// <summary>
        /// The function which returns CSS classes for a date.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Func<TValue, string>? AdditionalDateClassesFunc { get; set; }

        /// <summary>
        /// The icon for the button that navigates to the previous month or year.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ChevronLeft"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string PreviousIcon { get; set; } = Icons.Material.Filled.ChevronLeft;

        /// <summary>
        /// The icon for the button which navigates to the next month or year.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ChevronRight"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string NextIcon { get; set; } = Icons.Material.Filled.ChevronRight;

        /// <summary>
        /// The year to use, which cannot be changed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? FixYear { get; set; }

        /// <summary>
        /// The month to use, which cannot be changed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? FixMonth { get; set; }

        /// <summary>
        /// The day to use, which cannot be changed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? FixDay { get; set; }

        protected OpenTo CurrentView;

        protected override async Task OnPickerOpenedAsync()
        {
            await base.OnPickerOpenedAsync();
            if (Editable && Text != null)
            {
                var dateTime = ToDateTime(ConvertGet(Text));
                if (dateTime.HasValue)
                {
                    var culture = GetCulture();
                    var calendar = culture.Calendar;
                    PickerMonth = FromDateTime(new DateTime(calendar.GetYear(dateTime.Value), calendar.GetMonth(dateTime.Value), 1, calendar));
                }
            }
            if (OpenTo == OpenTo.Date && FixDay.HasValue && FixMonth.HasValue)
            {
                OpenTo = OpenTo.Year;
            }
            if (OpenTo == OpenTo.Date && FixDay.HasValue)
            {
                OpenTo = OpenTo.Month;
            }
            CurrentView = OpenTo;
            if (CurrentView == OpenTo.Year)
                _scrollToYearAfterRender = true;
        }

        /// <summary>
        /// Get the first of the month to display
        /// </summary>
        protected DateTime GetMonthStart(int month)
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;

            // Handle explicit min boundary
            if (_picker_month is { Year: 1, Month: 1 })
            {
                return calendar.MinSupportedDateTime;
            }

            var baseDate = _picker_month ?? DateTime.Today.StartOfMonth(culture);

            var year = FixYear ?? calendar.GetYear(baseDate);
            var startMonth = FixMonth ?? calendar.GetMonth(baseDate);

            var monthStart = new DateTime(year, startMonth, 1, 0, 0, 0, 0, calendar, DateTimeKind.Utc);

            // Handle explicit max boundary
            if (_picker_month.HasValue &&
                calendar.GetYear(_picker_month.Value) == 9999 &&
                calendar.GetMonth(_picker_month.Value) ==
                calendar.GetMonthsInYear(9999) &&
                month >= 1)
            {
                return calendar.MaxSupportedDateTime;
            }

            return calendar.AddMonths(monthStart, month);
        }

        /// <summary>
        /// Get the last of the month to display
        /// </summary>
        protected DateTime GetMonthEnd(int month)
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            var monthStartDate = _picker_month ?? DateTime.Today.StartOfMonth(culture);
            return calendar.AddMonths(monthStartDate, month).EndOfMonth(culture);
        }

        protected DayOfWeek GetFirstDayOfWeek()
        {
            if (FirstDayOfWeek.HasValue)
                return FirstDayOfWeek.Value;
            return GetCulture().DateTimeFormat.FirstDayOfWeek;
        }

        /// <summary>
        /// Gets the n-th week of the currently displayed month.
        /// </summary>
        /// <param name="month">offset from _picker_month</param>
        /// <param name="index">between 0 and 4</param>
        protected IEnumerable<DateTime> GetWeek(int month, int index)
        {
            if (index is < 0 or > 5)
            {
                throw new ArgumentException("Index must be between 0 and 5");
            }
            var culture = GetCulture();
            var calendar = culture.Calendar;
            var monthFirst = GetMonthStart(month);
            if ((calendar.MaxSupportedDateTime - monthFirst).Days >= index * 7)
            {
                var weekFirst = monthFirst.AddDays(index * 7).StartOfWeek(GetFirstDayOfWeek(), culture);
                for (var i = 0; i < 7; i++)
                {
                    if ((calendar.MaxSupportedDateTime - weekFirst).Days >= i)
                        yield return weekFirst.AddDays(i);
                    else
                        yield return calendar.MaxSupportedDateTime;
                }
            }
        }

        private string GetWeekNumber(int month, int index)
        {
            if (index is < 0 or > 5)
            {
                throw new ArgumentException("Index must be between 0 and 5");
            }

            var culture = GetCulture();
            var calendar = culture.Calendar;
            var monthFirst = GetMonthStart(month);
            var weekFirst = monthFirst.AddDays(index * 7).StartOfWeek(GetFirstDayOfWeek(), culture);
            //january 1st
            if (monthFirst.Month == 1 && index == 0)
            {
                weekFirst = monthFirst;
            }

            if (weekFirst.Month != monthFirst.Month && weekFirst.AddDays(6).Month != monthFirst.Month)
            {
                return string.Empty;
            }

            return calendar.GetWeekOfYear(weekFirst, culture.DateTimeFormat.CalendarWeekRule, FirstDayOfWeek ?? culture.DateTimeFormat.FirstDayOfWeek).ToString();
        }

        protected virtual OpenTo? GetNextView()
        {
            OpenTo? nextView = CurrentView switch
            {
                OpenTo.Year => GetNextViewFromYear(),
                OpenTo.Month => !FixDay.HasValue ? OpenTo.Date : null,
                _ => null,
            };
            return nextView;
        }

        protected virtual OpenTo? GetPreviousView()
        {
            OpenTo? previousView = CurrentView switch
            {
                OpenTo.Date => GetPreviousViewFromDate(),
                OpenTo.Month => !FixYear.HasValue ? OpenTo.Year : null,
                _ => null,
            };
            return previousView;
        }

        private OpenTo? GetNextViewFromYear()
        {
            if (!FixMonth.HasValue)
            {
                return OpenTo.Month;
            }

            if (!FixDay.HasValue)
            {
                return OpenTo.Date;
            }

            return null;
        }

        private OpenTo? GetPreviousViewFromDate()
        {
            if (!FixMonth.HasValue)
            {
                return OpenTo.Month;
            }

            if (!FixYear.HasValue)
            {
                return OpenTo.Year;
            }

            return null;
        }

        protected virtual async Task SubmitAndCloseAsync()
        {
            if (PickerActions == null)
            {
                await SubmitAsync();

                if (PickerVariant != PickerVariant.Static)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(ClosingDelay), TimeProvider);
                    await CloseAsync(false);
                }
            }
        }

        protected virtual bool IsDayDisabled(DateTime date)
        {
            var minDate = ToDateTime(MinDate);
            var maxDate = ToDateTime(MaxDate);
            if (minDate.HasValue && date < minDate) return true;
            if (maxDate.HasValue && date > maxDate) return true;
            var asTValue = FromDateTime(date);
            return asTValue is not null && IsDateDisabledFunc(asTValue);
        }

        protected abstract string GetDayClasses(int month, DateTime day);

        /// <summary>
        /// User clicked on a day
        /// </summary>
        protected abstract Task OnDayClickedAsync(DateTime dateTime);

        /// <summary>
        /// user clicked on a month
        /// </summary>
        /// <param name="month"></param>
        protected virtual Task OnMonthSelectedAsync(DateTime month)
        {
            PickerMonth = FromDateTime(month);
            var nextView = GetNextView();
            if (nextView != null)
            {
                CurrentView = (OpenTo)nextView;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// user clicked on a year
        /// </summary>
        /// <param name="year"></param>
        protected virtual Task OnYearClickedAsync(int year)
        {
            var current = GetMonthStart(0);
            PickerMonth = FromDateTime(new DateTime(year, current.Month, 1, GetCulture().Calendar));
            var nextView = GetNextView();
            if (nextView != null)
            {
                CurrentView = (OpenTo)nextView;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// user clicked on a month
        /// </summary>
        protected virtual void OnMonthClicked(int month)
        {
            CurrentView = OpenTo.Month;
            _picker_month = _picker_month?.AddMonths(month);
            StateHasChanged();
        }

        /// <summary>
        /// Check if month is disabled
        /// </summary>
        /// <param name="month">Month given with first day of the month</param>
        /// <returns>True if month should be disabled, false otherwise</returns>
        private bool IsMonthDisabled(DateTime month)
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            var minDate = ToDateTime(MinDate);
            var maxDate = ToDateTime(MaxDate);
            if (!FixDay.HasValue)
            {
                if (minDate.HasValue && month.EndOfMonth(culture) < minDate) return true;
                if (maxDate.HasValue && month > maxDate) return true;
                return false;
            }
            if (calendar.GetDaysInMonth(calendar.GetYear(month), calendar.GetMonth(month)) < FixDay!.Value)
            {
                return true;
            }
            var day = new DateTime(month.Year, month.Month, FixDay!.Value);
            if (minDate.HasValue && day < minDate) return true;
            if (maxDate.HasValue && day > maxDate) return true;
            var asTValue = FromDateTime(day);
            return asTValue is not null && IsDateDisabledFunc(asTValue);
        }

        /// <summary>
        /// return Mo, Tu, We, Th, Fr, Sa, Su in the right culture
        /// </summary>
        protected IEnumerable<string> GetAbbreviatedDayNames()
        {
            var dayNamesNormal = GetCulture().DateTimeFormat.AbbreviatedDayNames;
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
            return GetMonthStart(month).ToString(GetCulture().DateTimeFormat.YearMonthPattern, GetCulture());
        }

        protected abstract string GetTitleDateString();

        protected string FormatTitleDate(DateTime? date)
        {
            return date?.ToString(TitleDateFormat ?? "ddd, dd MMM", GetCulture()) ?? "";
        }

        protected string GetFormattedYearString()
        {
            var selectedYear = HighlightedDate ?? GetMonthStart(0);

            return GetCalendarYear(selectedYear).ToString();
        }

        private void OnPreviousMonthClick()
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            var pickerMonthDt = ToDateTime(PickerMonth);
            // It is impossible to go further into the past after reaching DateTime.MinValue
            if (pickerMonthDt.HasValue && calendar.GetYear(pickerMonthDt.Value) == DateTime.MinValue.Year && calendar.GetMonth(pickerMonthDt.Value) == DateTime.MinValue.Month)
            {
                return;
            }
            PickerMonth = FromDateTime(GetMonthStart(0).AddDays(-1).StartOfMonth(GetCulture()));
        }

        private void OnNextMonthClick()
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            var pickerMonthDt = ToDateTime(PickerMonth);
            // It is impossible to go further into the future after reaching DateTime.MaxValue
            if (pickerMonthDt.HasValue && calendar.GetYear(pickerMonthDt.Value) == DateTime.MaxValue.Year
                && calendar.GetMonth(pickerMonthDt.Value) == DateTime.MaxValue.Month)
            {
                return;
            }
            PickerMonth = FromDateTime(GetMonthEnd(0).AddDays(1));
        }

        private void OnPreviousYearClick()
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            var pickerMonthDt = ToDateTime(PickerMonth);
            // It is impossible to go further into the past after reaching DateTime.MinValue
            if (pickerMonthDt.HasValue && calendar.GetYear(pickerMonthDt.Value) == DateTime.MinValue.Year)
            {
                return;
            }
            PickerMonth = FromDateTime(GetMonthStart(0).AddYears(-1));
        }

        private void OnNextYearClick()
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            var pickerMonthDt = ToDateTime(PickerMonth);
            // It is impossible to go further into the future after reaching DateTime.MaxValue
            if (pickerMonthDt.HasValue && calendar.GetYear(pickerMonthDt.Value) == DateTime.MaxValue.Year)
            {
                return;
            }
            PickerMonth = FromDateTime(GetMonthStart(0).AddYears(1));
        }

        private void OnYearClick()
        {
            if (!FixYear.HasValue)
            {
                CurrentView = OpenTo.Year;
                StateHasChanged();
                _scrollToYearAfterRender = true;
            }
        }

        private void GoToSelectedYear()
        {
            PickerMonth = FromDateTime(HighlightedDate);
            OnYearClick();
        }

        /// <summary>
        /// We need a random id for the year items in the year list so we can scroll to the item safely in every DatePicker.
        /// </summary>
        private readonly string _componentId = Identifier.Create();

        /// <summary>
        /// Is set to true to scroll to the actual year after the next render
        /// </summary>
        protected bool _scrollToYearAfterRender = false;

        /// <summary>
        /// Scrolls to the current year.
        /// </summary>
        public async Task ScrollToYearAsync(DateTime? date = null)
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            _scrollToYearAfterRender = false;
            var dateTime = date ?? GetMonthStart(0);
            var id = $"{_componentId}{calendar.GetYear(dateTime)}";
            await ScrollManager.ScrollToYearAsync(id);
            StateHasChanged();
        }

        protected int GetMinYear()
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            var minDate = ToDateTime(MinDate);
            if (minDate.HasValue)
                return calendar.GetYear(minDate.Value);
            return calendar.GetYear(DateTime.Today) - 100;
        }

        protected int GetMaxYear()
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            var maxDate = ToDateTime(MaxDate);
            if (maxDate.HasValue)
                return calendar.GetYear(maxDate.Value);
            return calendar.GetYear(DateTime.Today) + 100;
        }

        private string? GetYearClasses(int year)
        {
            var selectedYear = HighlightedDate ?? GetMonthStart(0);
            var culture = GetCulture();
            var calendar = culture.Calendar;
            if (year == calendar.GetYear(selectedYear))
                return $"mud-picker-year-selected mud-{Color.ToStringFast(true)}-text";
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
            var selectedYear = HighlightedDate ?? GetMonthStart(0);
            var culture = GetCulture();
            var calendar = culture.Calendar;
            if (year == calendar.GetYear(selectedYear))
                return Typo.h5;

            return Typo.subtitle1;
        }

        private void OnFormattedDateClick()
        {
            // todo: raise an event the user can handle
        }

        private IEnumerable<DateTime> GetAllMonths()
        {
            var current = GetMonthStart(0);
            var culture = GetCulture();
            var calendar = culture.Calendar;
            var calendarYear = calendar.GetYear(current);
            var firstOfCalendarYear = calendar.ToDateTime(calendarYear, 1, 1, 0, 0, 0, 0);
            for (var i = 0; i < calendar.GetMonthsInYear(calendarYear); i++)
                yield return calendar.AddMonths(firstOfCalendarYear, i);
        }

        private string GetAbbreviatedMonthName(DateTime month)
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            var calendarMonth = calendar.GetMonth(month);
            return GetCulture().DateTimeFormat.AbbreviatedMonthNames[calendarMonth - 1];
        }

        private string GetMonthName(DateTime month)
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            var calendarMonth = calendar.GetMonth(month);
            return GetCulture().DateTimeFormat.MonthNames[calendarMonth - 1];
        }

        private string? GetMonthClasses(DateTime month)
        {
            var selectedMonth = HighlightedDate ?? GetMonthStart(0);
            var culture = GetCulture();
            var calendar = culture.Calendar;
            if (calendar.GetYear(month) != calendar.GetYear(selectedMonth))
                return null;

            if (calendar.GetMonth(month) == calendar.GetMonth(selectedMonth) && !IsMonthDisabled(selectedMonth))
                return $"mud-picker-month-selected mud-{Color.ToStringFast(true)}-text";

            return null;
        }

        private Typo GetMonthTypo(DateTime month)
        {
            var selectedMonth = HighlightedDate ?? GetMonthStart(0);
            var culture = GetCulture();
            var calendar = culture.Calendar;
            if (calendar.GetYear(month) != calendar.GetYear(selectedMonth))
                return Typo.subtitle1;

            if (calendar.GetMonth(month) == calendar.GetMonth(selectedMonth))
                return Typo.h5;

            return Typo.subtitle1;
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            AdornmentAriaLabel ??= Localizer[Resources.LanguageResource.MudBaseDatePicker_Open];
            CurrentView = OpenTo;

            if (HighlightedDate is not null)
            {
                return;
            }

            var culture = GetCulture();
            var calendar = culture.Calendar;
            var today = TimeProvider.GetLocalNow().Date;

            var year = FixYear ?? calendar.GetYear(today);
            var month = FixMonth ?? (year == calendar.GetYear(today) ? calendar.GetMonth(today) : 1);
            var day = FixDay ?? 1;

            if (DateTime.TryParseExact($"{year}-{month}-{day}", "yyyy-M-d", GetCulture(), DateTimeStyles.None, out var date))
            {
                HighlightedDate = date;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                _picker_month ??= GetCalendarStartOfMonth();
            }

            if (firstRender && CurrentView == OpenTo.Year)
            {
                ScrollToYearAsync().CatchAndLog();
                return;
            }

            if (_scrollToYearAfterRender)
                ScrollToYearAsync().CatchAndLog();
        }

        /// <inheritdoc />
        protected override IConverter<TValue?, string?> GetDefaultConverter()
        {
            return new DefaultConverter<TValue?>
            {
                Culture = GetCulture,
                Format = GetFormat
            };
        }

        protected override string GetFormat()
        {
            if (!string.IsNullOrEmpty(_dateFormatState.Value))
            {
                return _dateFormatState.Value;
            }

            var culture = GetCulture();
            if (!string.IsNullOrEmpty(culture.DateTimeFormat.ShortDatePattern))
            {
                // In some cases, a custom culture may set the ShortDatePattern to an empty string.
                // This could result in an invalid or unintended date format, but if it's not empty, return the custom ShortDatePattern.
                return culture.DateTimeFormat.ShortDatePattern;
            }

            return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        protected abstract DateTime GetCalendarStartOfMonth();

        private int GetCalendarDayOfMonth(DateTime date)
        {
            var culture = GetCulture();
            var calendar = culture.Calendar;
            return calendar.GetDayOfMonth(date);
        }

        /// <summary>
        /// Converts gregorian date into whatever year it is in the provided culture
        /// </summary>
        /// <param name="yearDate">Gregorian Date</param>
        /// <returns>Year according to culture</returns>
        protected abstract int GetCalendarYear(DateTime yearDate);

        private ValueTask HandleMouseoverOnPickerCalendarDayButton(int tempId)
        {
            return JsApiService.UpdateStyleProperty(_mudPickerCalendarContentElementId, "--selected-day", tempId);
        }
    }
}
