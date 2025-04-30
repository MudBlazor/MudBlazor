using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a picker for a range of dates.
    /// </summary>
    /// <seealso cref="MudDatePicker"/>
    public partial class MudDateRangePicker : MudBaseDatePicker
    {
        private readonly ParameterState<bool> _allowDisabledDatesInCountState;
        private DateTime? _firstDate = null, _secondDate, _minValidDate, _maxValidDate;
        private DateRange _dateRange;
        private Range<string> _rangeText;

        protected override bool IsRange => true;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public MudDateRangePicker()
        {
            using var registerScope = CreateRegisterScope();
            _allowDisabledDatesInCountState = registerScope.RegisterParameter<bool>(nameof(AllowDisabledDatesInCount))
                .WithParameter(() => AllowDisabledDatesInCount)
                .WithChangeHandler(RecalculateValidDays);

            DisplayMonths = 2;
            AdornmentAriaLabel = "Open Date Range Picker";
        }

        /// <summary>
        /// The maximum number of selectable days.
        /// </summary>
        /// <remarks>
        /// Inclusive of the selected date.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int? MaxDays { get; set; }

        /// <summary>
        /// The minimum number of selectable days.
        /// </summary>
        /// <remarks>
        /// Inclusive of the selected date.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int? MinDays { get; set; }

        /// <summary>
        /// Include disabled dates within the valid min/max days range.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>. Disabled days will be included in the min/max count. 
        /// This parameter will take effect when <see cref="MinDays"/> or <see cref="MaxDays"/> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public bool AllowDisabledDatesInCount { get; set; } = true;

        /// <summary>
        /// The text displayed in the start input if no date is specified.
        /// </summary>
        /// <remarks>
        /// This property is typically used to give the user a hint as to what kind of input is expected.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string PlaceholderStart { get; set; }

        /// <summary>
        /// The text displayed in the end input if no date is specified.
        /// </summary>
        /// <remarks>
        /// This property is typically used to give the user a hint as to what kind of input is expected.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string PlaceholderEnd { get; set; }

        /// <summary>
        /// The icon displayed between start and end dates.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowRightAlt"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string SeparatorIcon { get; set; } = Icons.Material.Filled.ArrowRightAlt;

        /// <summary>
        /// Occurs when <see cref="DateRange"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<DateRange> DateRangeChanged { get; set; }

        /// <summary>
        /// The currently selected date range.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public DateRange DateRange
        {
            get => _dateRange;
            set => SetDateRangeAsync(value, true).CatchAndLog();
        }

        /// <summary>
        /// Enables capture for disabled dates within the selected date range.
        /// </summary>
        /// <remarks>
        /// By default, it will always ignore disabled dates. This parameter will take effect when <see cref="MudBaseDatePicker.IsDateDisabledFunc"/> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public bool AllowDisabledDatesInRange { get; set; } = false;

        protected async Task SetDateRangeAsync(DateRange range, bool updateValue)
        {
            if (_dateRange != range)
            {
                var doesRangeContainDisabledDates = !AllowDisabledDatesInRange && range is { Start: not null, End: not null } && Enumerable
                    .Range(0, int.MaxValue)
                    .Select(index => range.Start.Value.AddDays(index))
                    .TakeWhile(date => date <= range.End.Value)
                    .Any(date => IsDateDisabledFunc(date.Date));

                if (doesRangeContainDisabledDates)
                {
                    _rangeText = null;
                    await SetTextAsync(null, false);
                    return;
                }

                Touched = true;

                if (range?.Start is not null)
                    PickerMonth = new DateTime(Culture.Calendar.GetYear(range.Start.Value), Culture.Calendar.GetMonth(range.Start.Value), 1, Culture.Calendar);

                _dateRange = range;
                _value = range?.End;
                HighlightedDate = range?.Start;

                if (updateValue)
                {
                    Converter.GetError = false;
                    if (_dateRange == null || (_dateRange.Start == null && _dateRange.End == null))
                    {
                        _rangeText = null;
                        await SetTextAsync(null, false);
                    }
                    else
                    {
                        _rangeText = new Range<string>(
                            Converter.Set(_dateRange.Start),
                            Converter.Set(_dateRange.End));
                        await SetTextAsync(_dateRange.ToString(Converter), false);
                    }
                }

                await DateRangeChanged.InvokeAsync(_dateRange);
                await BeginValidateAsync();
                FieldChanged(_value);
            }
        }

        private Range<string> RangeText
        {
            get => _rangeText;
            set
            {
                if (_rangeText?.Equals(value) ?? value == null)
                    return;

                Touched = true;
                _rangeText = value;
                SetDateRangeAsync(ParseDateRangeValue(value?.Start, value?.End), false).CatchAndLog();
            }
        }

        private MudRangeInput<string> _rangeInput;

        /// <summary>
        /// Focuses the start input.
        /// </summary>
        public ValueTask FocusStartAsync() => _rangeInput.FocusStartAsync();

        /// <summary>
        /// Selects the start input text.
        /// </summary>
        public ValueTask SelectStartAsync() => _rangeInput.SelectStartAsync();

        /// <summary>
        /// Selects a portion of the start input text.
        /// </summary>
        /// <param name="pos1">The index of the first character to select.</param>
        /// <param name="pos2">The index of the last character to select.</param>
        public ValueTask SelectRangeStartAsync(int pos1, int pos2) => _rangeInput.SelectRangeStartAsync(pos1, pos2);

        /// <summary>
        /// Focuses the end input.
        /// </summary>
        public ValueTask FocusEndAsync() => _rangeInput.FocusEndAsync();

        /// <summary>
        /// Selects the end input text.
        /// </summary>
        public ValueTask SelectEndAsync() => _rangeInput.SelectEndAsync();

        public override ValueTask BlurAsync() => _rangeInput.BlurAsync();

        /// <summary>
        /// Selects a portion of the end input text.
        /// </summary>
        /// <param name="pos1">The index of the first character to select.</param>
        /// <param name="pos2">The index of the last character to select.</param>
        public ValueTask SelectRangeEndAsync(int pos1, int pos2) => _rangeInput.SelectRangeEndAsync(pos1, pos2);

        protected override Task DateFormatChangedAsync(string newFormat)
        {
            Touched = true;
            _rangeText = null;
            if (_dateRange?.Start != null || _dateRange?.End != null)
            {
                _rangeText = new Range<string>(
                    Converter.Set(_dateRange.Start),
                    Converter.Set(_dateRange.End));
            }

            return SetTextAsync(_dateRange?.ToString(Converter), false);
        }

        protected override Task StringValueChangedAsync(string value)
        {
            Touched = true;
            // Update the date range property (without updating back the Value property)
            return SetDateRangeAsync(ParseDateRangeValue(value), false);
        }

        protected override bool HasValue(DateTime? value) => value is not null;

        protected override bool IsDayDisabled(DateTime date)
        {
            if (_firstDate is null || _secondDate is not null)
            {
                return base.IsDayDisabled(date);
            }

            var selectedDate = _firstDate.Value;
            var validDateRange = GetValidDateRange(selectedDate);

            return base.IsDayDisabled(date) || MudDateRangePicker.IsDateOutOfRange(date, selectedDate, validDateRange);
        }

        private DateRange GetValidDateRange(DateTime selectedDate)
        {
            var start = MinDays switch
            {
                null => MinDate ?? DateTime.MinValue,
                _ when _allowDisabledDatesInCountState.Value => selectedDate.Date.AddDays(MinDays.Value - 1),
                _ => _minValidDate
            };

            var end = MaxDays switch
            {
                null => MaxDate ?? DateTime.MaxValue,
                _ when _allowDisabledDatesInCountState.Value => selectedDate.Date.AddDays(MaxDays.Value - 1),
                _ => _maxValidDate
            };

            return new DateRange(start, end);
        }

        private static bool IsDateOutOfRange(DateTime date, DateTime selectedDate, DateRange validRange)
        {
            var isNotSelectedDate = date < selectedDate || date > selectedDate;
            var isOutsideValidRange = date < validRange.Start || date > validRange.End;

            return isNotSelectedDate && isOutsideValidRange;
        }

        private DateTime GetMaxSelectableDate(DateTime startDate, int maxDays)
        {
            var validDayCount = 1;
            var lastValidDate = startDate;
            var maxDate = startDate.AddDays(1);

            while (validDayCount < maxDays)
            {
                if (!IsDateDisabledFunc(maxDate))
                {
                    validDayCount++;
                    lastValidDate = maxDate;
                }

                if (validDayCount == maxDays)
                    break;

                if (maxDate.Date > MaxDate.GetValueOrDefault(startDate.AddYears(50)).Date)
                    break;

                if (maxDate.Date == DateTime.MaxValue.Date)
                    break;

                maxDate = maxDate.AddDays(1);
            }

            return lastValidDate;
        }

        /// <summary>
        /// Recalculate the valid days in relation to the <see cref="MinDays"/> and <see cref="MaxDays"/> allowed
        /// </summary>
        public void RecalculateValidDays()
        {
            if (_firstDate is null) return;

            if (MinDays is not null)
                _minValidDate = GetMaxSelectableDate(_firstDate.Value, MinDays.Value);

            if (MaxDays is not null)
                _maxValidDate = GetMaxSelectableDate(_firstDate.Value, MaxDays.Value);

            StateHasChanged();
        }

        private DateRange ParseDateRangeValue(string value)
        {
            return DateRange.TryParse(value, Converter, out var dateRange) ? dateRange : null;
        }

        private DateRange ParseDateRangeValue(string start, string end)
        {
            return DateRange.TryParse(start, end, Converter, out var dateRange) ? dateRange : null;
        }

        protected override Task OnPickerClosedAsync()
        {
            _firstDate = null;

            return base.OnPickerClosedAsync();
        }

        private bool CheckDateRange(DateTime day, Func<DateTime, DateTime, bool> compareStart, Func<DateTime, DateTime, bool> compareEnd)
        {
            return _firstDate is null
                && _dateRange is { Start: { } start, End: { } end }
                && compareStart(start.Date, day)
                && compareEnd(end.Date, day);
        }

        protected override string GetDayClasses(int month, DateTime day)
        {
            var b = new CssBuilder("mud-day");
            b.AddClass(AdditionalDateClassesFunc?.Invoke(day) ?? string.Empty);
            if (day < GetMonthStart(month) || day > GetMonthEnd(month))
            {
                return b.AddClass("mud-hidden").Build();
            }

            static bool isLessThan(DateTime date1, DateTime date2) => date1 < date2;
            static bool isGreaterThan(DateTime date1, DateTime date2) => date1 > date2;
            static bool isEqualTo(DateTime date1, DateTime date2) => date1 == date2;
            static bool isNotEqualTo(DateTime date1, DateTime date2) => date1 != date2;


            if ((_firstDate?.Date < day && _secondDate?.Date > day) || CheckDateRange(day, compareStart: isLessThan, compareEnd: isGreaterThan))
            {
                return b
                    .AddClass("mud-range")
                    .AddClass("mud-range-between")
                    .AddClass($"mud-current mud-{Color.ToDescriptionString()}-text mud-button-outlined mud-button-outlined-{Color.ToDescriptionString()}", day == DateTime.Today)
                    .Build();
            }

            if (_firstDate?.Date == day && _secondDate?.Date == day)
            {
                return b.AddClass("mud-selected")
                    .AddClass($"mud-theme-{Color.ToDescriptionString()}")
                    .Build();
            }

            if (_firstDate?.Date == day || CheckDateRange(day, compareStart: isEqualTo, compareEnd: isNotEqualTo))
            {
                return b.AddClass("mud-selected")
                    .AddClass("mud-range")
                    .AddClass("mud-range-start-selected")
                    .AddClass("mud-range-selection", _firstDate != null)
                    .AddClass($"mud-theme-{Color.ToDescriptionString()}")
                    .Build();
            }

            if ((_firstDate is { } && _secondDate?.Date == day) || CheckDateRange(day, compareStart: isNotEqualTo, compareEnd: isEqualTo))
            {
                return b.AddClass("mud-selected")
                    .AddClass("mud-range")
                    .AddClass("mud-range-end-selected")
                    .AddClass($"mud-theme-{Color.ToDescriptionString()}")
                    .Build();
            }

            if (CheckDateRange(day, compareStart: isEqualTo, compareEnd: isEqualTo))
            {
                return b.AddClass("mud-selected").AddClass($"mud-theme-{Color.ToDescriptionString()}").Build();
            }
            else if (_firstDate?.Date < day)
            {
                return b.AddClass("mud-range", _secondDate is null && day != DateTime.Today)
                    .AddClass("mud-range-selection")
                    .AddClass($"mud-range-selection-{Color.ToDescriptionString()}", _firstDate is not null)
                    .AddClass($"mud-current mud-{Color.ToDescriptionString()}-text mud-button-outlined mud-button-outlined-{Color.ToDescriptionString()}", day == DateTime.Today)
                    .Build();
            }

            if (day == DateTime.Today)
            {
                return b.AddClass("mud-current")
                    .AddClass($"mud-button-outlined mud-button-outlined-{Color.ToDescriptionString()}")
                    .AddClass($"mud-{Color.ToDescriptionString()}-text")
                    .Build();
            }

            return b.Build();
        }

        protected override async Task OnDayClickedAsync(DateTime dateTime)
        {
            if (_firstDate == null || _secondDate != null)
            {
                _secondDate = null;
                _firstDate = dateTime;

                RecalculateValidDays();

                return;
            }
            if (_firstDate > dateTime)
            {
                _secondDate = _firstDate;
                _firstDate = dateTime;
            }
            else
            {
                _secondDate = dateTime;
            }
            if (PickerActions == null || AutoClose)
            {
                await SubmitAsync();

                if (PickerVariant != PickerVariant.Static)
                {
                    await Task.Delay(ClosingDelay);
                    await CloseAsync(false);
                }
            }
        }

        protected override Task OnOpenedAsync()
        {
            _secondDate = null;
            return base.OnOpenedAsync();
        }

        protected internal override async Task SubmitAsync()
        {
            if (GetReadOnlyState())
                return;
            if (_firstDate == null || _secondDate == null)
                return;

            await SetDateRangeAsync(new DateRange(_firstDate, _secondDate), true);

            _firstDate = null;
            _secondDate = null;
        }

        public override Task ClearAsync(bool close = true)
        {
            DateRange = null;
            _firstDate = _secondDate = null;
            return base.ClearAsync(close);
        }

        protected override string GetTitleDateString()
        {
            if (_firstDate != null)
                return $"{FormatTitleDate(_firstDate)} - {FormatTitleDate(_secondDate)}";

            return DateRange?.Start != null
                ? $"{FormatTitleDate(DateRange.Start)} - {FormatTitleDate(DateRange.End)}"
                : "";
        }

        protected override DateTime GetCalendarStartOfMonth()
        {
            var date = StartMonth ?? DateRange?.Start ?? DateTime.Today;
            return date.StartOfMonth(Culture);
        }

        protected override int GetCalendarYear(DateTime yearDate)
        {
            var date = DateRange?.Start ?? DateTime.Today;
            var diff = Culture.Calendar.GetYear(date) - Culture.Calendar.GetYear(yearDate);
            var calenderYear = Culture.Calendar.GetYear(date);
            return calenderYear - diff;
        }
    }
}
