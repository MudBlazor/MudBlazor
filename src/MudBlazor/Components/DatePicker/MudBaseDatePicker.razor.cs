using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract partial class MudBaseDatePicker<T> : MudPicker<T?> where T : struct
    {
        private readonly string _mudPickerCalendarContentElementId;
        private bool _dateFormatTouched;

        protected MudBaseDatePicker() : base(new DefaultConverter<T?>
        {
            Format = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern,
            Culture = CultureInfo.CurrentCulture
        })
        {
            AdornmentAriaLabel = "Open Date Picker";
            _mudPickerCalendarContentElementId = Guid.NewGuid().ToString();
        }

        [Inject]
        protected IScrollManager ScrollManager { get; set; }

        [Inject]
        private IJsApiService JsApiService { get; set; }

        [Inject]
        private IDateOperations<T> DateOperations { get; set; }

        /// <summary>
        /// Max selectable date.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public T? MaxDate { get; set; }

        /// <summary>
        /// Min selectable date.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public T? MinDate { get; set; }

        /// <summary>
        /// First view to show in the MudDatePicker.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public OpenTo OpenTo { get; set; } = OpenTo.Date;

        /// <summary>
        /// String Format for selected date view
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string DateFormat
        {
            get
            {
                return (Converter as DefaultConverter<T?>)?.Format;
            }
            set
            {
                if (Converter is DefaultConverter<T?> defaultConverter)
                {
                    defaultConverter.Format = value;
                    _dateFormatTouched = true;
                }
                DateFormatChangedAsync(value);
            }
        }

        /// <summary>
        /// Date format value change hook for descendants.
        /// </summary>
        protected virtual Task DateFormatChangedAsync(string newFormat)
        {
            return Task.CompletedTask;
        }

        protected override bool SetCulture(CultureInfo value)
        {
            if (!base.SetCulture(value))
                return false;

            if (!_dateFormatTouched && Converter is DefaultConverter<T?> defaultConverter)
                defaultConverter.Format = value.DateTimeFormat.ShortDatePattern;

            return true;
        }

        /// <summary>
        /// Defines on which day the week starts. Depends on the value of Culture. 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public DayOfWeek? FirstDayOfWeek { get; set; } = null;

        /// <summary>
        /// The current month of the date picker (two-way bindable). This changes when the user browses through the calender.
        /// The month is represented as a DateTime which is always the first day of that month. You can also set this to define which month is initially shown. If not set, the current month is shown.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public T? PickerMonth
        {
            get => _picker_month;
            set
            {
                if (!value.HasValue && !_picker_month.HasValue ||
                    value.HasValue && _picker_month.HasValue && DateOperations.WithDate(value.Value).Equal(_picker_month))
                    return;
                _picker_month = value;
                InvokeAsync(StateHasChanged);
                PickerMonthChanged.InvokeAsync(value);
            }
        }

        private T? _picker_month;

        /// <summary>
        /// Fired when the date changes.
        /// </summary>
        [Parameter] public EventCallback<T?> PickerMonthChanged { get; set; }

        /// <summary>
        /// Sets the amount of time in milliseconds to wait before closing the picker. This helps the user see that the date was selected before the popover disappears.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int ClosingDelay { get; set; } = 100;

        /// <summary>
        /// Number of months to display in the calendar
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int DisplayMonths { get; set; } = 1;

        /// <summary>
        /// Maximum number of months in one row
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public int? MaxMonthColumns { get; set; }

        /// <summary>
        /// Start month when opening the picker. 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public T? StartMonth { get; set; }

        /// <summary>
        /// Display week numbers according to the Culture parameter. If no culture is defined, CultureInfo.CurrentCulture will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowWeekNumbers { get; set; }

        /// <summary>
        /// Format of the selected date in the title. By default, this is "ddd, dd MMM" which abbreviates day and month names. 
        /// For instance, display the long names like this "dddd, dd. MMMM". 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public string TitleDateFormat { get; set; } = "ddd, dd MMM";

        /// <summary>
        /// If AutoClose is set to true and PickerActions are defined, selecting a day will close the MudDatePicker.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool AutoClose { get; set; }

        /// <summary>
        /// Function to determine whether a date is disabled
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public Func<T, bool> IsDateDisabledFunc
        {
            get => _isDateDisabledFunc;
            set
            {
                _isDateDisabledFunc = value ?? (_ => false);
            }
        }
        private Func<T, bool> _isDateDisabledFunc = _ => false;

        /// <summary>
        /// Function to conditionally apply new classes to specific days
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Func<T, string> AdditionalDateClassesFunc { get; set; }

        /// <summary>
        /// Custom previous icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string PreviousIcon { get; set; } = Icons.Material.Filled.ChevronLeft;

        /// <summary>
        /// Custom next icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string NextIcon { get; set; } = Icons.Material.Filled.ChevronRight;

        /// <summary>
        /// Set a predefined fix year - no year can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? FixYear { get; set; }
        /// <summary>
        /// Set a predefined fix month - no month can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? FixMonth { get; set; }
        /// <summary>
        /// Set a predefined fix day - no day can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? FixDay { get; set; }

        protected virtual bool IsRange { get; } = false;

        protected OpenTo CurrentView;

        protected override async Task OnPickerOpenedAsync()
        {
            await base.OnPickerOpenedAsync();
            if (Editable == true && Text != null)
            {
                var a = Converter.Get(Text);
                if (a.HasValue)
                {
                    a = DateOperations.WithDate(a.Value).StartOfMonth().Value;
                    PickerMonth = a;
                }
            }
            if (OpenTo == OpenTo.Date && FixDay.HasValue)
            {
                OpenTo = OpenTo.Month;
            }
            if (OpenTo == OpenTo.Date && FixDay.HasValue && FixMonth.HasValue)
            {
                OpenTo = OpenTo.Year;
            }
            CurrentView = OpenTo;
            if (CurrentView == OpenTo.Year)
                _scrollToYearAfterRender = true;
        }

        /// <summary>
        /// Get the first of the month to display
        /// </summary>
        /// <returns></returns>
        protected T GetMonthStart(int month)
        {
            var monthStartDate = _picker_month ?? DateOperations.WithToday().StartOfMonth().Value;
            // Return the min supported datetime of the calendar when this is year 1 and first month!
            if (DateOperations.WithDate(monthStartDate).IsMinDate())
            {
                return DateOperations.MinSupportedDate();
            }
            return DateOperations.WithDate(monthStartDate).AddMonths(month).Value;
        }

        /// <summary>
        /// Get the last of the month to display
        /// </summary>
        /// <returns></returns>
        protected T GetMonthEnd(int month)
        {
            var monthStartDate = _picker_month ?? DateOperations.WithToday().StartOfMonth().Value;
            return DateOperations.WithDate(monthStartDate).AddMonths(month).EndOfMonth(0).Value;
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
        protected IEnumerable<T> GetWeek(int month, int index)
        {
            if (index is < 0 or > 5)
                throw new ArgumentException("Index must be between 0 and 5");
            var month_first = GetMonthStart(month);
            return DateOperations.WithDate(month_first).AddDays(index * 7).StartOfWeek(GetFirstDayOfWeek()).GetWeekDays();
        }

        private string GetWeekNumber(int month, int index)
        {
            if (index is < 0 or > 5)
                throw new ArgumentException("Index must be between 0 and 5");

            return DateOperations.WithDate(GetMonthStart(month)).GetWeekNumber(index);
        }

        protected virtual OpenTo? GetNextView()
        {
            OpenTo? nextView = CurrentView switch
            {
                OpenTo.Year => !FixMonth.HasValue ? OpenTo.Month : !FixDay.HasValue ? OpenTo.Date : null,
                OpenTo.Month => !FixDay.HasValue ? OpenTo.Date : null,
                _ => null,
            };
            return nextView;
        }

        protected virtual async Task SubmitAndCloseAsync()
        {
            if (PickerActions == null)
            {
                await SubmitAsync();

                if (PickerVariant != PickerVariant.Static)
                {
                    await Task.Delay(ClosingDelay);
                    await CloseAsync(false);
                }
            }
        }

        protected abstract string GetDayClasses(int month, T day);

        /// <summary>
        /// User clicked on a day
        /// </summary>
        protected abstract Task OnDayClickedAsync(T dateTime);

        /// <summary>
        /// user clicked on a month
        /// </summary>
        /// <param name="month"></param>
        protected virtual Task OnMonthSelectedAsync(T month)
        {
            PickerMonth = month;
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
            PickerMonth = DateOperations.WithDate(GetMonthStart(0)).SetYear(year).Value;
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
            if (_picker_month.HasValue)
            {
                _picker_month = DateOperations.WithDate(_picker_month.Value).AddMonths(month).Value;
            }
            StateHasChanged();
        }

        /// <summary>
        /// Check if month is disabled
        /// </summary>
        /// <param name="month">Month given with first day of the month</param>
        /// <returns>True if month should be disabled, false otherwise</returns>
        private bool IsMonthDisabled(T month)
        {
            if (!FixDay.HasValue)
            {
                var monthOperations = DateOperations.WithDate(month);
                return monthOperations.EndOfMonth(0).LesserThan(MinDate) ||
                       monthOperations.GreaterThan(MaxDate);
            }
            if (DateOperations.WithDate(month).DaysInMonth() < FixDay.Value)
            {
                return true;
            }

            var dayOperation = DateOperations.WithDate(month).SetDay(FixDay.Value);
            return dayOperation.LesserThan(MinDate) || dayOperation.GreaterThan(MaxDate) || IsDateDisabledFunc(dayOperation.Value);
        }

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
        private static U[] Shift<U>(U[] array, int positions)
        {
            var copy = new U[array.Length];
            Array.Copy(array, 0, copy, array.Length - positions, positions);
            Array.Copy(array, positions, copy, 0, array.Length - positions);
            return copy;
        }

        protected string GetMonthName(int month)
        {
            return DateOperations.WithDate(GetMonthStart(month)).ToString(Culture.DateTimeFormat.YearMonthPattern, Culture);
        }

        protected abstract string GetTitleDateString();

        protected string FormatTitleDate(DateTime? date)
        {
            return date?.ToString(TitleDateFormat ?? "ddd, dd MMM", Culture) ?? "";
        }

        protected string GetFormattedYearString()
        {
            return DateOperations.WithDate(GetMonthStart(0)).ToString("yyyy", Culture);
        }

        private void OnPreviousMonthClick()
        {
            // It is impossible to go further into the past after the first year and the first month!
            if (PickerMonth.HasValue && DateOperations.WithDate(PickerMonth.Value).IsMinDate())
            {
                return;
            }
            PickerMonth = DateOperations.WithDate(GetMonthStart(0)).AddDays(-1).StartOfMonth().Value;
        }

        private void OnNextMonthClick()
        {
            PickerMonth = DateOperations.WithDate(GetMonthEnd(0)).AddDays(1).Value;
        }

        private void OnPreviousYearClick()
        {
            PickerMonth = DateOperations.WithDate(GetMonthStart(0)).AddYears(-1).Value;
        }

        private void OnNextYearClick()
        {
            PickerMonth = DateOperations.WithDate(GetMonthStart(0)).AddYears(1).Value;
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
            var id = $"{_componentId}{DateOperations.WithDate(GetMonthStart(0)).GetYear()}";
            await ScrollManager.ScrollToYearAsync(id);
            StateHasChanged();
        }

        private int GetMinYear()
        {
            if (MinDate.HasValue)
                return DateOperations.WithDate(MinDate.Value).GetYear();
            return DateOperations.WithToday().AddYears(-100).GetYear();
        }

        private int GetMaxYear()
        {
            if (MaxDate.HasValue)
                return DateOperations.WithDate(MaxDate.Value).GetYear();
            return DateOperations.WithToday().AddYears(100).GetYear();
        }

        private string GetYearClasses(int year)
        {
            if (year == DateOperations.WithDate(GetMonthStart(0)).GetYear())
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
            if (year == DateOperations.WithDate(GetMonthStart(0)).GetYear())
                return Typo.h5;
            return Typo.subtitle1;
        }

        private void OnFormattedDateClick()
        {
            // todo: raise an event the user can handle
        }
        
        private IEnumerable<T> GetAllMonths()
        {
            return DateOperations.WithDate(GetMonthStart(0)).GetAllMonths();
        }

        private string GetAbbreviatedMonthName(T month)
        {
            var calendarMonth = DateOperations.WithDate(month).GetMonth();
            return Culture.DateTimeFormat.AbbreviatedMonthNames[calendarMonth - 1];
        }

        private string GetMonthName(T month)
        {
            var calendarMonth = DateOperations.WithDate(month).GetMonth();
            return Culture.DateTimeFormat.MonthNames[calendarMonth - 1];
        }

        private string GetMonthClasses(T month)
        {
            
            if (DateOperations.WithDate(GetMonthStart(0)).GetMonth() == DateOperations.WithDate(month).GetMonth() && !IsMonthDisabled(month))
                return $"mud-picker-month-selected mud-{Color.ToDescriptionString()}-text";
            return null;
        }

        private Typo GetMonthTypo(T month)
        {
            if (DateOperations.WithDate(month).GetMonth() == DateOperations.WithDate(GetMonthStart(0)).GetMonth())
                return Typo.h5;
            return Typo.subtitle1;
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            CurrentView = OpenTo;
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
                ScrollToYear();
                return;
            }

            if (_scrollToYearAfterRender)
                ScrollToYear();
        }

        protected abstract T GetCalendarStartOfMonth();

        private int GetCalendarDayOfMonth(T date)
        {
            return DateOperations.WithDate(date).GetDayOfMonth();
        }

        /// <summary>
        /// Converts gregorian date into whatever year it is in the provided culture
        /// </summary>
        /// <param name="yearDate">Gregorian Date</param>
        /// <returns>Year according to culture</returns>
        protected abstract int GetCalendarYear(T yearDate);

        private ValueTask HandleMouseoverOnPickerCalendarDayButton(int tempId)
        {
            return this.JsApiService.UpdateStyleProperty(_mudPickerCalendarContentElementId, "--selected-day", tempId);
        }
    }
}
