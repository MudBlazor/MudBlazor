using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using static System.String;

namespace MudBlazor
{
    public class MudDateRangePicker : MudBaseDatePicker, IDisposable
    {
        private DateTime? _firstDate = null, _currentDate;
        private DateRange _dateRange;

        protected override bool IsRange => true;

        public override void Open()
        {
            base.Open();
            Picker.Open();
        }

        public override void Close()
        {
            base.Open();
            Picker.Close();
        }

        public MudDateRangePicker()
        {
            DisplayMonths = 2;
            //DisableToolbar = true;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                Picker.OnOpenStateChanged += OnPickerStateChanged;
            }
        }

        public void Dispose()
        {
            Picker.OnOpenStateChanged -= OnPickerStateChanged;
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
                        await SetValueAsync(null, false);
                    else
                    {
                        if (!IsNullOrEmpty(DateFormat))
                            await SetValueAsync(_dateRange.ToString(DateFormat), false);
                        else
                            await SetValueAsync(_dateRange.ToIsoDateString(), false);
                    }
                }

                await DateRangeChanged.InvokeAsync(_dateRange);
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

        protected override string GetDayClasses(int month, DateTime day)
        {
            var b = new CssBuilder("mud-day");
            if (day < GetMonthStart(month) || day > GetMonthEnd(month))
            {
                return b.AddClass("mud-hidden").Build();
            }

            if ((_firstDate != null && day > _firstDate && day < _currentDate) ||
               (_firstDate == null && _dateRange != null && _dateRange.Start < day && _dateRange.End > day))
            {
                return b.AddClass("mud-range")
                    .AddClass("mud-range-between")
                    .Build();
            }

            if ((_firstDate != null && day == _firstDate) ||
                (_firstDate == null && _dateRange != null && _dateRange.Start == day && DateRange.End != day))
            {
                return b.AddClass("mud-selected")
                    .AddClass("mud-range")
                    .AddClass("mud-range-start-selected")
                    .AddClass($"mud-theme-{Color.ToDescriptionString()}")
                    .Build();
            }

            if ((_firstDate != null && day == _currentDate && day > _firstDate) ||
                (_firstDate == null && _dateRange != null && _dateRange.Start != day && _dateRange.End == day))
            {
                return b.AddClass("mud-selected")
                    .AddClass("mud-range")
                    .AddClass("mud-range-end-selected")
                    .AddClass($"mud-theme-{Color.ToDescriptionString()}")
                    .Build();
            }

            if (day == DateTime.Today)
            {
                return b.AddClass("mud-current")
                    .AddClass($"mud-{Color.ToDescriptionString()}-text")
                    .Build();
            }

            if (_firstDate == null && _dateRange != null && _dateRange.Start == _dateRange.End && _dateRange.Start == day)
            {
                return b.AddClass("mud-selected").AddClass($"mud-theme-{Color.ToDescriptionString()}").Build();
            }
            else if (_firstDate != null)
            {
                return b.AddClass("mud-range").Build();
            }

            return b.Build();
        }

        protected override async void OnDayClicked(DateTime dateTime)
        {
            if (_firstDate == null || _firstDate > dateTime)
            {
                _firstDate = dateTime;
                return;
            }

            await SetDateRangeAsync(new DateRange(_firstDate, dateTime), true);

            _firstDate = null;
            _currentDate = null;

            if (PickerVariant != PickerVariant.Static)
            {
                await Task.Delay(ClosingDelay);
                Picker.Close();
            }
        }

        protected override void OnMouseOver(DateTime dateTime)
        {
            if (_firstDate != null)
            {
                _currentDate = dateTime;
            }
        }

        protected override string GetFormattedDateString()
        {
            if (DateRange == null || DateRange.Start == null)
                return "";
            if (DateRange.End == null)
                return DateRange.Start.Value.ToString("dd MMM", Culture);

            return $"{DateRange.Start.Value.ToString("dd MMM", Culture)} - {DateRange.End.Value.ToString("dd MMM", Culture)}";
        }

        private void OnPickerStateChanged(bool args)
        {
            if (!args)
            {
                _firstDate = null;
            }
        }
    }
}
