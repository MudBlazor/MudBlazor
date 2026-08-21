using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Selects a start and end date range from a calendar shown in a drop-down, dialog, or inline.
    /// </summary>
    /// <seealso cref="DateRange" />
    /// <seealso cref="MudDatePicker" />
    /// <seealso cref="MudTimePicker" />
    public partial class MudDateRangePicker : MudBaseDatePicker
    {
        private readonly ParameterState<bool> _allowDisabledDatesInCountState;
        private readonly ParameterState<DateRange?> _dateRangeState;
        private DateTime? _firstDate, _secondDate, _minValidDate, _maxValidDate;
        private DateRange? _dateRange;
        private DateRange? _dateRangeParameter;
        private Range<string>? _rangeText;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public MudDateRangePicker()
        {
            using var registerScope = CreateRegisterScope();
            _allowDisabledDatesInCountState = registerScope.RegisterParameter<bool>(nameof(AllowDisabledDatesInCount))
                .WithParameter(() => AllowDisabledDatesInCount)
                .WithChangeHandler(RecalculateValidDays);
            _dateRangeState = registerScope.RegisterParameter<DateRange?>(nameof(DateRange))
                .WithParameter(() => _dateRangeParameter)
                .WithEventCallback(() => DateRangeChanged)
                .WithChangeHandler(OnDateRangeParameterChangedAsync);

            DisplayMonths = 2;
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
        [Parameter, ParameterState]
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
        public string? PlaceholderStart { get; set; }

        /// <summary>
        /// The text displayed in the end input if no date is specified.
        /// </summary>
        /// <remarks>
        /// This property is typically used to give the user a hint as to what kind of input is expected.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? PlaceholderEnd { get; set; }

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
        public EventCallback<DateRange?> DateRangeChanged { get; set; }

        /// <summary>
        /// The currently selected date range.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public DateRange? DateRange
        {
            get => _dateRange;
            // DateRange is managed by MudBlazor's ParameterState framework (see _dateRangeState in the constructor).
            // Blazor stores the assigned parameter value here (raw); the state's change handler
            // (OnDateRangeParameterChangedAsync) reflects a programmatic/parent assignment in the display WITHOUT
            // raising DateRangeChanged (#10834). A genuine user selection instead writes through
            // _dateRangeState.SetValueAsync, which DOES raise DateRangeChanged. The getter keeps returning the
            // processed value (_dateRange) so normalization and disabled-date filtering remain observable through
            // the public API. Because assignment no longer starts async work here, the earlier suppression race
            // (PR #13328) can no longer occur through this setter.
            set => _dateRangeParameter = value;
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

        // Internal value change (user calendar selection, clear, or text edit): update the display AND notify the
        // parent through DateRangeChanged (two-way binding write-back). Programmatic/parent assignments instead flow
        // through the ParameterState change handler (OnDateRangeParameterChangedAsync), which never notifies.
        protected async Task SetDateRangeAsync(DateRange? range, bool updateValue, bool suppressInteraction = false)
        {
            if (!await UpdateDateRangeDisplayAsync(range, updateValue, suppressInteraction))
            {
                return;
            }

            // Keep the raw parameter mirror aligned with the internally selected value so that change detection in
            // SetParametersAsync compares a subsequent programmatic assignment against what the user actually sees
            // (otherwise a parent could re-assign the pre-change value and it would be missed as "unchanged").
            _dateRangeParameter = _dateRange;

            // Writing through the state raises DateRangeChanged and keeps the tracked ParameterState value in sync,
            // so a later programmatic assignment of the same value is still detected as "unchanged".
            await _dateRangeState.SetValueAsync(_dateRange);
            await BeginValidateAsync();
            if (!suppressInteraction)
            {
                FieldChanged(_value);
            }
        }

        // Runs when DateRange is assigned programmatically (from a parent) via the ParameterState framework.
        // It reflects the new value in the display but must NOT raise DateRangeChanged: echoing back an event the
        // parent never triggered is exactly the bug reported in #10834. suppressInteraction mirrors the previous
        // behavior where a parameter assignment was never treated as user interaction.
        private async Task OnDateRangeParameterChangedAsync(ParameterChangedEventArgs<DateRange?> args)
        {
            if (await UpdateDateRangeDisplayAsync(args.Value, updateValue: true, suppressInteraction: true))
            {
                await BeginValidateAsync();
            }
        }

        // Applies a date range to the picker's display state (normalization, text, highlighted date, picker month,
        // disabled-date filtering). Returns true when the value was accepted and actually changed, false when it was
        // unchanged or rejected (e.g. it contains a disabled date). This method never raises DateRangeChanged.
        private async Task<bool> UpdateDateRangeDisplayAsync(DateRange? range, bool updateValue, bool suppressInteraction)
        {
            // Normalize the DateRange before exception is thrown
            range = NormalizeDateRange(range);

            if (_dateRange == range)
            {
                return false;
            }

            var doesRangeContainDisabledDates = !AllowDisabledDatesInRange && range is { Start: not null, End: not null } && Enumerable
                .Range(0, int.MaxValue)
                .Select(index => range.Start.Value.AddDays(index))
                .TakeWhile(date => date <= range.End.Value)
                .Any(date => IsDateDisabledFunc(date.Date));

            if (doesRangeContainDisabledDates)
            {
                _rangeText = null;
                await SetTextAsync(null, false);
                return false;
            }

            if (!suppressInteraction)
            {
                Touched = true;
            }

            if (range?.Start is not null && StartMonth == null)
                PickerMonth = new DateTime(GetCulture().Calendar.GetYear(range.Start.Value), GetCulture().Calendar.GetMonth(range.Start.Value), 1, GetCulture().Calendar);

            _dateRange = range;
            _value = range?.End;
            HighlightedDate = range?.Start;

            if (updateValue)
            {
                ResetConverterErrors();
                if (_dateRange == null || (_dateRange.Start == null && _dateRange.End == null))
                {
                    _rangeText = null;
                    await SetTextAsync(null, false);
                }
                else
                {
                    _rangeText = new Range<string>(
                        ConvertSet(_dateRange.Start),
                        ConvertSet(_dateRange.End));
                    await SetTextAsync(_dateRange.ToString(GetConverter()), false);
                }
            }

            return true;
        }

        private Range<string>? RangeText
        {
            get => _rangeText;
            set
            {
                if (_rangeText?.Equals(value) ?? (value == null))
                    return;

                Touched = true;
                _rangeText = value;
                SetDateRangeAsync(value is null ? null : ParseDateRangeValue(value.Start, value.End), false).CatchAndLog();
            }
        }

        private MudRangeInput<string> _rangeInput = null!;

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

        protected override Task DateFormatChangedAsync(string? newFormat)
        {
            Touched = true;
            _rangeText = null;
            if (_dateRange?.Start != null || _dateRange?.End != null)
            {
                _rangeText = new Range<string>(
                    ConvertSet(_dateRange.Start),
                    ConvertSet(_dateRange.End));
            }

            return SetTextAsync(_dateRange?.ToString(GetConverter()), false);
        }

        protected override Task StringValueChangedAsync(string? value)
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

            return base.IsDayDisabled(date) || IsDateOutOfRange(date, validDateRange);
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

        private static bool IsDateOutOfRange(DateTime date, DateRange validRange)
        {
            var isOutsideValidRange = date < validRange.Start || date > validRange.End;

            return isOutsideValidRange;
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

        private DateRange? ParseDateRangeValue(string? value)
        {
            return DateRange.TryParse(value, GetConverter(), out var dateRange) ? dateRange : null;
        }

        private DateRange? ParseDateRangeValue(string? start, string? end)
        {
            return DateRange.TryParse(start, end, GetConverter(), out var dateRange) ? dateRange : null;
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
            var today = TimeProvider.GetLocalNow().Date;
            var b = new CssBuilder("mud-day");
            b.AddClass(AdditionalDateClassesFunc?.Invoke(day) ?? string.Empty);
            b.AddClass("mud-adjacent-month", IsAdjacentMonthDay(month, day));
            if (IsHiddenAdjacentMonthDay(month, day))
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
                    .AddClass($"mud-current mud-{Color.ToStringFast(true)}-text mud-button-outlined mud-button-outlined-{Color.ToStringFast(true)}", day == today)
                    .Build();
            }

            if (_firstDate?.Date == day && _secondDate?.Date == day)
            {
                return b.AddClass("mud-selected")
                    .AddClass($"mud-theme-{Color.ToStringFast(true)}")
                    .Build();
            }

            if (_firstDate?.Date == day || CheckDateRange(day, compareStart: isEqualTo, compareEnd: isNotEqualTo))
            {
                return b.AddClass("mud-selected")
                    .AddClass("mud-range")
                    .AddClass("mud-range-start-selected")
                    .AddClass("mud-range-selection", _firstDate != null)
                    .AddClass($"mud-theme-{Color.ToStringFast(true)}")
                    .Build();
            }

            if ((_firstDate is { } && _secondDate?.Date == day) || CheckDateRange(day, compareStart: isNotEqualTo, compareEnd: isEqualTo))
            {
                return b.AddClass("mud-selected")
                    .AddClass("mud-range")
                    .AddClass("mud-range-end-selected")
                    .AddClass($"mud-theme-{Color.ToStringFast(true)}")
                    .Build();
            }

            if (CheckDateRange(day, compareStart: isEqualTo, compareEnd: isEqualTo))
            {
                return b.AddClass("mud-selected").AddClass($"mud-theme-{Color.ToStringFast(true)}").Build();
            }

            if (_firstDate?.Date < day)
            {
                return b.AddClass("mud-range", _secondDate is null && day != today)
                    .AddClass("mud-range-selection")
                    .AddClass($"mud-range-selection-{Color.ToStringFast(true)}", _firstDate is not null)
                    .AddClass($"mud-current mud-{Color.ToStringFast(true)}-text mud-button-outlined mud-button-outlined-{Color.ToStringFast(true)}", day == today)
                    .Build();
            }

            if (day == today)
            {
                return b.AddClass("mud-current")
                    .AddClass($"mud-button-outlined mud-button-outlined-{Color.ToStringFast(true)}")
                    .AddClass($"mud-{Color.ToStringFast(true)}-text")
                    .Build();
            }

            return b.Build();
        }

        protected override async Task OnDayClickedAsync(DateTime dateTime)
        {
            if (GetReadOnlyState())
                return;
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
                    await Task.Delay(TimeSpan.FromMilliseconds(ClosingDelay), TimeProvider);
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

        protected override Task ResetValueAsync() => ClearAsync();

        public override async Task ClearAsync(bool close = true)
        {
            await SetDateRangeAsync(null, true);
            _firstDate = _secondDate = null;
            await base.ClearAsync(close);
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
            var date = StartMonth ?? DateRange?.Start ?? TimeProvider.GetLocalNow().Date;
            return date.StartOfMonth(GetCulture());
        }

        protected override async Task OnYearClickedAsync(int year)
        {
            await base.OnYearClickedAsync(year);

            if (DateRange?.Start is null && _firstDate is null)
            {
                HighlightedDate = PickerMonth;
            }
        }

        protected override int GetCalendarYear(DateTime yearDate)
        {
            var date = DateRange?.Start ?? TimeProvider.GetLocalNow().Date;
            var diff = GetCulture().Calendar.GetYear(date) - GetCulture().Calendar.GetYear(yearDate);
            var calenderYear = GetCulture().Calendar.GetYear(date);
            return calenderYear - diff;
        }

        /// <summary>
        /// Normalize a date by treating DateTime.MinValue as null
        /// This prevents an ArgumentOutOfRangeException from happening when performing date arithmetic
        /// </summary>
        /// <param name="date">The date to normalize</param>
        /// <returns>Normalized date or null</returns>
        private static DateTime? NormalizeDate(DateTime? date)
        {
            if (date is null)
                return null;

            // Treat DateTime.MinValue as null
            if (date.Value == DateTime.MinValue)
                return null;

            return date;
        }

        /// <summary>
        /// Normalize a date range by checking the start date and end date for DateTime.MinValue
        /// This prevents an ArgumentOutOfRangeException from happening when performing date arithmetic
        /// </summary>
        /// <see cref="NormalizeDate"/>
        /// <param name="range">The date range to normalize</param>
        /// <returns>Normalized date range or null</returns>
        private static DateRange? NormalizeDateRange(DateRange? range)
        {
            if (range is null)
                return null;

            var start = NormalizeDate(range.Start);
            var end = NormalizeDate(range.End);

            return new DateRange(start, end);
        }

    }
}
