using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudDateRangePicker : MudBaseDatePicker
    {
        private DateTime? _firstDate = null, _secondDate;
        private DateRange _dateRange;
        private Range<string> _rangeText;

        protected override bool IsRange => true;

        public MudDateRangePicker()
        {
            DisplayMonths = 2;
            AdornmentAriaLabel = "Open Date Range Picker";
        }

        /// <summary>
        /// Fired when the DateFormat changes.
        /// </summary>
        [Parameter] public EventCallback<DateRange> DateRangeChanged { get; set; }

        /// <summary>
        /// The currently selected range (two-way bindable). If null, then nothing was selected.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public DateRange DateRange
        {
            get => _dateRange;
            set => SetDateRangeAsync(value, true).AndForget();
        }

        protected async Task SetDateRangeAsync(DateRange range, bool updateValue)
        {
            if (_dateRange != range)
            {
                var doesRangeContainDisabledDates = range?.Start != null && range?.End != null && Enumerable
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

                _dateRange = range;
                _value = range?.End;

                if (updateValue)
                {
                    Converter.GetError = false;
                    if (_dateRange == null)
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
                SetDateRangeAsync(ParseDateRangeValue(value?.Start, value?.End), false).AndForget();
            }
        }

        private MudRangeInput<string> _rangeInput;

        /// <summary>
        /// Focuses the start date of MudDateRangePicker
        /// </summary>
        /// <returns></returns>
        public ValueTask FocusStartAsync() => _rangeInput.FocusStartAsync();

        /// <summary>
        /// Selects the start date of MudDateRangePicker
        /// </summary>
        /// <returns></returns>
        public ValueTask SelectStartAsync() => _rangeInput.SelectStartAsync();

        /// <summary>
        /// Selects the specified range of the start date text
        /// </summary>
        /// <param name="pos1">Start position of the selection</param>
        /// <param name="pos2">End position of the selection</param>
        /// <returns></returns>
        public ValueTask SelectRangeStartAsync(int pos1, int pos2) => _rangeInput.SelectRangeStartAsync(pos1, pos2);

        /// <summary>
        /// Focuses the end date of MudDateRangePicker
        /// </summary>
        /// <returns></returns>
        public ValueTask FocusEndAsync() => _rangeInput.FocusEndAsync();

        /// <summary>
        /// Selects the end date of MudDateRangePicker
        /// </summary>
        /// <returns></returns>
        public ValueTask SelectEndAsync() => _rangeInput.SelectEndAsync();

        /// <summary>
        /// Selects the specified range of the end date text
        /// </summary>
        /// <param name="pos1">Start position of the selection</param>
        /// <param name="pos2">End position of the selection</param>
        /// <returns></returns>
        public ValueTask SelectRangeEndAsync(int pos1, int pos2) => _rangeInput.SelectRangeEndAsync(pos1, pos2);

        protected override Task DateFormatChanged(string newFormat)
        {
            Touched = true;
            return SetTextAsync(_dateRange?.ToString(Converter), false);
        }

        protected override Task StringValueChanged(string value)
        {
            Touched = true;
            // Update the daterange property (without updating back the Value property)
            return SetDateRangeAsync(ParseDateRangeValue(value), false);
        }

        protected override bool HasValue(DateTime? value)
        {
            return null != value && value.HasValue;
        }

        private DateRange ParseDateRangeValue(string value)
        {
            return DateRange.TryParse(value, Converter, out var dateRange) ? dateRange : null;
        }

        private DateRange ParseDateRangeValue(string start, string end)
        {
            return DateRange.TryParse(start, end, Converter, out var dateRange) ? dateRange : null;
        }

        protected override void OnPickerClosed()
        {
            _firstDate = null;
            base.OnPickerClosed();
        }

        protected override string GetDayClasses(int month, DateTime day)
        {
            var b = new CssBuilder("mud-day");
            if (day < GetMonthStart(month) || day > GetMonthEnd(month))
            {
                return b.AddClass("mud-hidden").Build();
            }

            if ((_firstDate != null && _secondDate != null && _firstDate < day && _secondDate > day) ||
                (_firstDate == null && _dateRange != null && _dateRange.Start < day && _dateRange.End > day))
            {
                return b
                    .AddClass("mud-range")
                    .AddClass("mud-range-between")
                    .AddClass($"mud-current mud-{Color.ToDescriptionString()}-text mud-button-outlined mud-button-outlined-{Color.ToDescriptionString()}", day == DateTime.Today)
                    .Build();
            }

            if ((_firstDate != null && day == _firstDate) ||
                (_firstDate == null && _dateRange != null && _dateRange.Start == day && DateRange.End != day))
            {
                return b.AddClass("mud-selected")
                    .AddClass("mud-range")
                    .AddClass("mud-range-start-selected")
                    .AddClass("mud-range-selection", _firstDate != null)
                    .AddClass($"mud-theme-{Color.ToDescriptionString()}")
                    .Build();
            }

            if ((_firstDate != null && _secondDate != null && day == _secondDate) ||
                (_firstDate == null && _dateRange != null && _dateRange.Start != day && _dateRange.End == day))
            {
                return b.AddClass("mud-selected")
                    .AddClass("mud-range")
                    .AddClass("mud-range-end-selected")
                    .AddClass($"mud-theme-{Color.ToDescriptionString()}")
                    .Build();
            }

            if (_firstDate == null && _dateRange != null && _dateRange.Start == _dateRange.End && _dateRange.Start == day)
            {
                return b.AddClass("mud-selected").AddClass($"mud-theme-{Color.ToDescriptionString()}").Build();
            }
            else if (_firstDate != null && day > _firstDate)
            {
                return b.AddClass("mud-range", _secondDate == null && day != DateTime.Today)
                    .AddClass("mud-range-selection")
                    .AddClass($"mud-range-selection-{Color.ToDescriptionString()}", _firstDate != null)
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

        protected override async void OnDayClicked(DateTime dateTime)
        {
            if (_firstDate == null || _firstDate > dateTime || _secondDate != null)
            {
                _secondDate = null;
                _firstDate = dateTime;
                return;
            }

            _secondDate = dateTime;
            if (PickerActions == null || AutoClose)
            {
                Submit();

                if (PickerVariant != PickerVariant.Static)
                {
                    await Task.Delay(ClosingDelay);
                    Close(false);
                }
            }
        }

        protected override void OnOpened()
        {
            _secondDate = null;
            base.OnOpened();
        }

        protected internal override async void Submit()
        {
            if (GetReadOnlyState())
                return;
            if (_firstDate == null || _secondDate == null)
                return;

            await SetDateRangeAsync(new DateRange(_firstDate, _secondDate), true);

            _firstDate = null;
            _secondDate = null;
        }

        public override void Clear(bool close = true)
        {
            DateRange = null;
            _firstDate = _secondDate = null;
            base.Clear();
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

        protected override int GetCalendarYear(int year)
        {
            var date = DateRange?.Start ?? DateTime.Today;
            var diff = date.Year - year;
            var calenderYear = Culture.Calendar.GetYear(date);
            return calenderYear - diff;
        }
    }
}
