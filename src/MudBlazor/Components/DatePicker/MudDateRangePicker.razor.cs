using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a picker for a range of dates.
    /// </summary>
    /// <typeparam name="T">The date type bound by the picker. Supported: <see cref="DateTime"/>, <see cref="DateTime"/>?, <see cref="DateOnly"/>, <see cref="DateOnly"/>?, <see cref="DateTimeOffset"/>, <see cref="DateTimeOffset"/>?.</typeparam>
    /// <seealso cref="MudDatePicker{T}"/>
    public partial class MudDateRangePicker<T> : MudBaseDatePicker<T>
    {
        private readonly ParameterState<bool> _allowDisabledDatesInCountState;
        private DateTime? _firstDate, _secondDate, _minValidDate, _maxValidDate;
        private DateRange<T>? _dateRange;
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
        public EventCallback<DateRange<T>?> DateRangeChanged { get; set; }

        /// <summary>
        /// The currently selected date range.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public DateRange<T>? DateRange
        {
            get => _dateRange;
            set => SetDateRangeAsync(value, true).CatchAndLog();
        }

        /// <summary>
        /// Enables capture for disabled dates within the selected date range.
        /// </summary>
        /// <remarks>
        /// By default, it will always ignore disabled dates. This parameter will take effect when <see cref="MudBaseDatePicker{T}.IsDateDisabledFunc"/> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public bool AllowDisabledDatesInRange { get; set; } = false;

        protected async Task SetDateRangeAsync(DateRange<T>? range, bool updateValue)
        {
            // Normalize the DateRange before exception is thrown
            range = NormalizeDateRange(range);

            if (_dateRange != range)
            {
                bool doesRangeContainDisabledDates = false;
                if (!AllowDisabledDatesInRange && range is { Start: not null, End: not null })
                {
                    var startDt = ToDateTime(range.Start)!.Value;
                    var endDt = ToDateTime(range.End)!.Value;
                    doesRangeContainDisabledDates = Enumerable
                        .Range(0, int.MaxValue)
                        .Select(index => startDt.AddDays(index))
                        .TakeWhile(date => date <= endDt)
                        .Any(date =>
                        {
                            var asTValue = FromDateTime(date.Date);
                            return asTValue is not null && IsDateDisabledFunc(asTValue);
                        });
                }

                if (doesRangeContainDisabledDates)
                {
                    _rangeText = null;
                    await SetTextAsync(null, false);
                    return;
                }

                Touched = true;

                if (range is { Start: not null } && StartMonth is null)
                {
                    var startDt = ToDateTime(range.Start)!.Value;
                    PickerMonth = FromDateTime(new DateTime(GetCulture().Calendar.GetYear(startDt), GetCulture().Calendar.GetMonth(startDt), 1, GetCulture().Calendar));
                }

                _dateRange = range;
                _value = range is null ? default : range.End;
                HighlightedDate = range is null ? null : ToDateTime(range.Start);

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

                await DateRangeChanged.InvokeAsync(_dateRange);
                await BeginValidateAsync();
                FieldChanged(_value);
            }
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
            if (_dateRange is { Start: not null } || _dateRange is { End: not null })
            {
                _rangeText = new Range<string>(
                    ConvertSet(_dateRange!.Start),
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

        protected override bool HasValue(T? value) => value is not null;

        protected override bool IsDayDisabled(DateTime date)
        {
            if (_firstDate is null || _secondDate is not null)
            {
                return base.IsDayDisabled(date);
            }

            var selectedDate = _firstDate.Value;
            var validDateRange = GetValidDateRange(selectedDate);

            return base.IsDayDisabled(date) || IsDateOutOfRange(date, selectedDate, validDateRange);
        }

        private (DateTime? Start, DateTime? End) GetValidDateRange(DateTime selectedDate)
        {
            var minDate = ToDateTimeLimit(MinDate);
            var maxDate = ToDateTimeLimit(MaxDate);
            var start = MinDays switch
            {
                null => minDate ?? DateTime.MinValue,
                _ when _allowDisabledDatesInCountState.Value => selectedDate.Date.AddDays(MinDays.Value - 1),
                _ => _minValidDate
            };

            var end = MaxDays switch
            {
                null => maxDate ?? DateTime.MaxValue,
                _ when _allowDisabledDatesInCountState.Value => selectedDate.Date.AddDays(MaxDays.Value - 1),
                _ => _maxValidDate
            };

            return (start, end);
        }

        private static bool IsDateOutOfRange(DateTime date, DateTime selectedDate, (DateTime? Start, DateTime? End) validRange)
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
            var maxDateLimit = ToDateTimeLimit(MaxDate) ?? startDate.AddYears(50);

            while (validDayCount < maxDays)
            {
                var asTValue = FromDateTime(maxDate);
                if (asTValue is not null && !IsDateDisabledFunc(asTValue))
                {
                    validDayCount++;
                    lastValidDate = maxDate;
                }

                if (validDayCount == maxDays)
                    break;

                if (maxDate.Date > maxDateLimit.Date)
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

        private DateRange<T>? ParseDateRangeValue(string? value)
        {
            return DateRange<T>.TryParse(value, GetConverter(), out var dateRange) ? dateRange : null;
        }

        private DateRange<T>? ParseDateRangeValue(string? start, string? end)
        {
            return DateRange<T>.TryParse(start, end, GetConverter(), out var dateRange) ? dateRange : null;
        }

        protected override Task OnPickerClosedAsync()
        {
            _firstDate = null;

            return base.OnPickerClosedAsync();
        }

        private bool CheckDateRange(DateTime day, Func<DateTime, DateTime, bool> compareStart, Func<DateTime, DateTime, bool> compareEnd)
        {
            if (_firstDate is not null || _dateRange is null) return false;
            var startDt = ToDateTime(_dateRange.Start);
            var endDt = ToDateTime(_dateRange.End);
            if (startDt is null || endDt is null) return false;
            return compareStart(startDt.Value.Date, day) && compareEnd(endDt.Value.Date, day);
        }

        protected override string GetDayClasses(int month, DateTime day)
        {
            var b = new CssBuilder("mud-day");
            var asTValue = FromDateTime(day);
            if (asTValue is not null)
                b.AddClass(AdditionalDateClassesFunc?.Invoke(asTValue) ?? string.Empty);
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
                    .AddClass($"mud-current mud-{Color.ToStringFast(true)}-text mud-button-outlined mud-button-outlined-{Color.ToStringFast(true)}", day == DateTime.Today)
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
                return b.AddClass("mud-range", _secondDate is null && day != DateTime.Today)
                    .AddClass("mud-range-selection")
                    .AddClass($"mud-range-selection-{Color.ToStringFast(true)}", _firstDate is not null)
                    .AddClass($"mud-current mud-{Color.ToStringFast(true)}-text mud-button-outlined mud-button-outlined-{Color.ToStringFast(true)}", day == DateTime.Today)
                    .Build();
            }

            if (day == DateTime.Today)
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

            await SetDateRangeAsync(new DateRange<T>(FromDateTime(_firstDate), FromDateTime(_secondDate)), true);

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

            return DateRange is { Start: not null }
                ? $"{FormatTitleDate(ToDateTime(DateRange.Start))} - {FormatTitleDate(ToDateTime(DateRange.End))}"
                : "";
        }

        protected override DateTime GetCalendarStartOfMonth()
        {
            var rangeStart = DateRange is null ? default : DateRange.Start;
            var date = ToDateTime(StartMonth) ?? ToDateTime(rangeStart) ?? TimeProvider.GetLocalNow().Date;
            return date.StartOfMonth(GetCulture());
        }

        protected override async Task OnYearClickedAsync(int year)
        {
            await base.OnYearClickedAsync(year);

            if (DateRange is not { Start: not null } && _firstDate is null)
            {
                HighlightedDate = ToDateTime(PickerMonth);
            }
        }

        protected override int GetCalendarYear(DateTime yearDate)
        {
            var rangeStart = DateRange is null ? default : DateRange.Start;
            var date = ToDateTime(rangeStart) ?? TimeProvider.GetLocalNow().Date;
            var diff = GetCulture().Calendar.GetYear(date) - GetCulture().Calendar.GetYear(yearDate);
            var calenderYear = GetCulture().Calendar.GetYear(date);
            return calenderYear - diff;
        }

        /// <summary>
        /// Normalize a date range by checking the start date and end date for DateTime.MinValue
        /// This prevents an ArgumentOutOfRangeException from happening when performing date arithmetic
        /// </summary>
        /// <param name="range">The date range to normalize</param>
        /// <returns>Normalized date range or null</returns>
        private static DateRange<T>? NormalizeDateRange(DateRange<T>? range)
        {
            if (range is null)
                return null;

            return new DateRange<T>(NormalizeValue(range.Start), NormalizeValue(range.End));
        }

        private static T? NormalizeValue(T? value)
        {
            if (value is null)
                return default;
            var dt = ToDateTime(value);
            if (dt is null || dt.Value == DateTime.MinValue)
                return default;
            return value;
        }
    }
}
