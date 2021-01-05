using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System;
using System.Threading.Tasks;

namespace MudBlazor
{
    public class MudDateRangePicker : MudBaseDatePicker, IDisposable
    {
        private DateTime? _minDate;
        private DateTime? _startDate = null, _currentDate;
        protected DateRange _dateRange;

        protected override bool IsRange => true;

        public MudDateRangePicker()
        {
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
        /// The currently selected date (two-way bindable). If null, then nothing was selected.
        /// </summary>
        [Parameter]
        public DateRange DateRange
        {
            get => _dateRange;
            set
            {
                if (value == _dateRange)
                    return;
                if (_setting_date)
                    return;
                _setting_date = true;
                try
                {
                    _dateRange = value;
                    if (value != null)
                    {
                        if ((!string.IsNullOrEmpty(DateFormat)) && _dateRange != null)
                            Value = _dateRange.ToString(DateFormat);
                        else
                            Value = _dateRange.ToIsoDateString();
                    }
                    InvokeAsync(StateHasChanged);
                    DateRangeChanged.InvokeAsync(value);
                }
                finally
                {
                    _setting_date = false;
                }
            }
        }

        private bool _setting_date;

        protected override void StringValueChanged(string value)
        {
            // update the date property
            if (DateRange.TryParse(value, out var dateRange))
                DateRange = dateRange;
            else
                DateRange = null;
        }

        protected override string GetDayClasses(DateTime day)
        {
            var b = new CssBuilder("mud-day");
            if (day < GetMonthStart() || day > GetMonthEnd())
                return b.AddClass("mud-hidden").Build();

            if ((_startDate != null && day > _startDate && day < _currentDate) ||
                (_startDate == null && _dateRange != null && _dateRange.Start < day && _dateRange.End > day))
            {
                return b.AddClass("mud-range").AddClass("mud-range-between").Build();
            }

            if ((_startDate != null && day == _startDate) ||
                (_startDate == null && _dateRange != null && _dateRange.Start == day))
            {
                return b.AddClass("mud-selected").AddClass("mud-range")
                    .AddClass("mud-range-start-selected").AddClass($"mud-theme-{Color.ToDescriptionString()}").Build();
            }

            if ((_startDate != null && day == _currentDate) ||
                (_startDate == null && _dateRange != null && _dateRange.End == day))
            {
                return b.AddClass("mud-selected").AddClass("mud-range")
                    .AddClass("mud-range-end-selected").AddClass($"mud-theme-{Color.ToDescriptionString()}").Build();
            }

            if (day == DateTime.Today)
                return b.AddClass("mud-current").AddClass($"mud-{Color.ToDescriptionString()}-text").Build();
            return b.Build();
        }

        protected override async void OnDayClicked(DateTime dateTime)
        {
            if (_startDate == null)
            {
                _startDate = dateTime;
                _minDate = MinDate;
                MinDate = dateTime;

                return;
            }

            DateRange = new DateRange(_startDate, dateTime);

            _startDate = null;
            _currentDate = null;
            MinDate = _minDate;

            if (PickerVariant != PickerVariant.Static)
            {
                await Task.Delay(ClosingDelay);
                Picker.Close();
            }
        }

        protected override void OnMouseOver(DateTime dateTime)
        {
            if (_startDate != null)
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
                _startDate = null;
                MinDate = _minDate;
            }
        }
    }
}
