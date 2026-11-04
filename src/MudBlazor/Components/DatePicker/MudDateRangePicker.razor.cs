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
        private readonly ParameterState<int?> _minDaysState;
        private readonly ParameterState<int?> _maxDaysState;
        private readonly ParameterState<bool> _allowDisabledDatesInCountState;
        private readonly ParameterState<DateRange?> _dateRangeState;
        private DateTime? _firstDate, _secondDate;
        private (DateTime Start, DateTime End)? _earlierValidDates, _laterValidDates;
        private DateRange? _dateRange;
        private DateRange? _dateRangeParameter;
        private Range<string>? _rangeText;
        private int _rangeTextEdit;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public MudDateRangePicker()
        {
            using var registerScope = CreateRegisterScope();
            _minDaysState = registerScope.RegisterParameter<int?>(nameof(MinDays))
                .WithParameter(() => MinDays)
                .WithChangeHandler(RecalculateValidDays);
            _maxDaysState = registerScope.RegisterParameter<int?>(nameof(MaxDays))
                .WithParameter(() => MaxDays)
                .WithChangeHandler(RecalculateValidDays);
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
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int? MaxDays { get; set; }

        /// <summary>
        /// The minimum number of selectable days.
        /// </summary>
        /// <remarks>
        /// Inclusive of the selected date.
        /// </remarks>
        [Parameter, ParameterState]
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

        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // The change handler only runs for values that arrive through the parameters, so a range the component already carries, such as one a derived constructor assigns, would never be shown.
            // A range that did arrive that way is applied here first, and the handler then finds nothing left to do.
            if (_dateRange is null && _dateRangeParameter is not null && await UpdateDateRangeDisplayAsync(_dateRangeParameter, updateValue: true, suppressInteraction: true))
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

            // Text that fails to convert leaves the range null, so a null assignment looks like a no-op and the bad text would stick.
            // Run it anyway while text remains, as MudDatePicker does.
            // A parent render that hands back the same null never gets here, because the change handler only runs when the parameter changes.
            if (_dateRange == range && !(range is null && !string.IsNullOrEmpty(Text)))
            {
                return false;
            }

            // Count the days up front so the walk never steps past the end, which would throw when the end is the last supported day.
            var doesRangeContainDisabledDates = !AllowDisabledDatesInRange && range is { Start: not null, End: not null } && range.Start.Value <= range.End.Value && Enumerable
                .Range(0, (range.End.Value - range.Start.Value).Days + 1)
                .Select(index => range.Start.Value.AddDays(index))
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
                // The range input binds its Value rather than its Text, so nothing else writes MudPicker.Text on the user-input path.
                // Without this, Text only tracked programmatic DateRange assignments and went stale on typing and on the clear button.
                ApplyRangeTextAsync(value, ++_rangeTextEdit).CatchAndLog();

                async Task ApplyRangeTextAsync(Range<string>? rangeText, int edit)
                {
                    // The range goes first so it lands before any consumer callback can yield, as it did before Text was written here at all.
                    await SetDateRangeAsync(rangeText is null ? null : ParseDateRangeValue(rangeText.Start, rangeText.End), false);

                    // A handler on that callback can yield long enough for a newer edit to overtake this one.
                    // Writing Text now would leave it describing an edit the range no longer reflects.
                    if (edit != _rangeTextEdit)
                    {
                        return;
                    }

                    await SetTextAsync(rangeText is null ? null : RangeUtility.Join(rangeText.Start, rangeText.End), callback: false);
                }
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

            return base.IsDayDisabled(date) || !IsWithinValidSpan(date.Date, _firstDate.Value.Date);
        }

        private bool IsWithinValidSpan(DateTime date, DateTime firstDate)
        {
            var minDays = _minDaysState.Value;
            var maxDays = _maxDaysState.Value;
            if (minDays is null && maxDays is null)
            {
                return true;
            }

            // An earlier second click becomes the range start, so both limits apply on either side of the first date.
            if (_allowDisabledDatesInCountState.Value)
            {
                var days = Math.Abs((date - firstDate).Days) + 1;
                return days >= (minDays ?? 1) && days <= (maxDays ?? int.MaxValue);
            }

            var validDates = date >= firstDate ? _laterValidDates : _earlierValidDates;
            return validDates is { } range && date >= range.Start && date <= range.End;
        }

        private (DateTime Date, bool Reached) GetLastSelectableDate(DateTime startDate, int days, int step)
        {
            // Without a MinDate or MaxDate, the walk stops 50 years out so a calendar that disables every date in that direction cannot hang it.
            var limit = step > 0
                ? MaxDateState.Value?.Date ?? startDate.AddYears(Math.Min(50, DateTime.MaxValue.Year - startDate.Year)).Date
                : MinDateState.Value?.Date ?? startDate.AddYears(-Math.Min(50, startDate.Year - DateTime.MinValue.Year)).Date;
            var edge = step > 0 ? DateTime.MaxValue.Date : DateTime.MinValue.Date;
            var validDayCount = 1;
            var lastValidDate = startDate;
            var date = startDate;

            while (validDayCount < days && date.Date != edge)
            {
                date = date.AddDays(step);

                if (!IsDateDisabledFunc(date))
                {
                    validDayCount++;
                    lastValidDate = date;
                }

                if (step > 0 ? date.Date > limit : date.Date < limit)
                    break;
            }

            return (lastValidDate, validDayCount >= days);
        }

        /// <summary>
        /// Recalculate the valid days in relation to the <see cref="MinDays"/> and <see cref="MaxDays"/> allowed
        /// </summary>
        public void RecalculateValidDays()
        {
            if (_firstDate is null) return;

            // Only the disabled-date-aware count reads these, and without MinDate or MaxDate each walk can scan 50 years of disabled dates.
            // Changing MinDays, MaxDays, MinDate or MaxDate reruns this, but changing IsDateDisabledFunc does not.
            // A lambda that captures state is a new delegate on every parent render, so a handler on it would rerun both walks each time.
            var countEnabledDates = !_allowDisabledDatesInCountState.Value;
            _laterValidDates = countEnabledDates ? GetValidDates(_firstDate.Value.Date, 1) : null;
            _earlierValidDates = countEnabledDates ? GetValidDates(_firstDate.Value.Date, -1) : null;

            StateHasChanged();

            (DateTime Start, DateTime End)? GetValidDates(DateTime firstDate, int step)
            {
                var nearest = firstDate;
                if (_minDaysState.Value is { } minDays)
                {
                    // Too few enabled dates on this side means no range on this side can reach MinDays.
                    var (date, reached) = GetLastSelectableDate(firstDate, minDays, step);
                    if (!reached)
                    {
                        return null;
                    }

                    nearest = date;
                }

                var furthest = _maxDaysState.Value is { } maxDays
                    ? GetLastSelectableDate(firstDate, maxDays, step).Date
                    : step > 0 ? DateTime.MaxValue : DateTime.MinValue;

                return step > 0 ? (nearest, furthest) : (furthest, nearest);
            }
        }

        internal override void OnDateLimitsChanged() => RecalculateValidDays();

        internal override bool IsRangeSelectionPending => _firstDate is not null && _secondDate is null;

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
                    .AddClass("mud-range-selection-anchor", _firstDate != null && _secondDate is null)
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

            // The first date already returned above, so any other day is a preview candidate on either side of it.
            if (_firstDate is { } firstDate)
            {
                return b.AddClass("mud-range", _secondDate is null && day != today)
                    .AddClass("mud-range-selection")
                    .AddClass("mud-range-selection-before", day < firstDate.Date)
                    .AddClass($"mud-range-selection-{Color.ToStringFast(true)}")
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

        protected override async Task ResetValueAsync()
        {
            await ClearAsync();
            // Clearing marks the picker as touched, and the base reset puts Touched back to false.
            await base.ResetValueAsync();
            // Clearing keeps the month the user was looking at, but a reset should reopen the calendar where an empty picker starts.
            ResetCalendarToStart();
        }

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
