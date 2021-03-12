using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using static System.String;

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
        }

        /// <summary>
        /// Fired when the DateFormat changes.
        /// </summary>
        [Parameter] public EventCallback<DateRange> DateRangeChanged { get; set; }

        /// <summary>
        /// The currently selected range (two-way bindable). If null, then nothing was selected.
        /// </summary>
        [Parameter]
        public DateRange DateRange
        {
            get => _dateRange;
            set => SetDateRangeAsync(value, true).AndForget();
        }

        protected async Task SetDateRangeAsync(DateRange range, bool updateValue)
        {
            if (_dateRange != range)
            {
                _dateRange = range;

                if (updateValue)
                {
                    if (_dateRange == null)
                    {
                        _rangeText = null;
                        await SetTextAsync(null, false);
                    }
                    else
                    {
                        if (!IsNullOrEmpty(DateFormat))
                        {
                            _rangeText = new Range<string>(
                                _dateRange.Start?.ToString(DateFormat) ?? Empty,
                                _dateRange.End?.ToString(DateFormat) ?? Empty);
                            await SetTextAsync(_dateRange.ToString(DateFormat), false);
                        }
                        else
                        {
                            _rangeText = new Range<string>(
                                _dateRange.Start?.ToIsoDateString() ?? Empty,
                                _dateRange.End?.ToIsoDateString() ?? Empty);
                            await SetTextAsync(_dateRange.ToIsoDateString(), false);
                        }
                    }
                }

                await DateRangeChanged.InvokeAsync(_dateRange);
            }
        }

        private Range<string> RangeText
        {
            get => _rangeText;
            set
            {
                if (_rangeText.Equals(value))
                    return;

                _rangeText = value;
                SetDateRangeAsync(ParseDateRangeValue(value.Start, value.End), false).AndForget();
            }
        }

        protected override Task StringValueChanged(string value)
        {
            // Update the daterange property (without updating back the Value property)
            return SetDateRangeAsync(ParseDateRangeValue(value), false);
        }

        private DateRange ParseDateRangeValue(string value)
        {
            return DateRange.TryParse(value, out var dateRange) ? dateRange : null;
        }

        private DateRange ParseDateRangeValue(string start, string end)
        {
            return DateRange.TryParse(start, end, out var dateRange) ? dateRange : null;
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
                return b.AddClass("mud-range")
                    .AddClass("mud-range-selection", _secondDate == null)
                    .AddClass($"mud-range-selection-{Color.ToDescriptionString()}", _firstDate != null)
                    .Build();
            }

            if (day == DateTime.Today)
            {
                return b.AddClass("mud-current")
                    .AddClass("mud-range", _firstDate != null && day > _firstDate)
                    .AddClass("mud-range-selection", _firstDate != null && day > _firstDate)
                    .AddClass($"mud-range-selection-{Color.ToDescriptionString()}", _firstDate != null && day > _firstDate)
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
            if (PickerActions == null)
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

        protected override async void Submit()
        {
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
            if (_firstDate != null && _secondDate != null)
                return $"{_firstDate.Value.ToString("dd MMM", Culture)} - {_secondDate.Value.ToString("dd MMM", Culture)}";
            else if (_firstDate != null)
                return _firstDate.Value.ToString("dd MMM", Culture) + " - ";

            if (DateRange == null || DateRange.Start == null)
                return "";
            if (DateRange.End == null)
                return DateRange.Start.Value.ToString("dd MMM", Culture);

            return $"{DateRange.Start.Value.ToString("dd MMM", Culture)} - {DateRange.End.Value.ToString("dd MMM", Culture)}";
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
