using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class MudDatePicker : MudBaseDatePicker
    {
        protected DateTime? _date;

        /// <summary>
        /// Fired when the DateFormat changes.
        /// </summary>
        [Parameter] public EventCallback<DateTime?> DateChanged { get; set; }

        /// <summary>
        /// The currently selected date (two-way bindable). If null, then nothing was selected.
        /// </summary>
        [Parameter]
        public DateTime? Date
        {
            get => _date;
            set
            {
                if (value == _date)
                    return;
                if (_setting_date)
                    return;
                _setting_date = true;
                try
                {
                    _date = value;
                    if ((!string.IsNullOrEmpty(DateFormat)) && _date.HasValue)
                        Value = _date.Value.ToString(DateFormat);
                    else
                        Value = _date.ToIsoDateString();
                    InvokeAsync(StateHasChanged);
                    DateChanged.InvokeAsync(value);
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
            if (DateTime.TryParse(value, out var date))
                Date = date;
            else
                Date = null;
        }

        protected override string GetDayClasses(int month, DateTime day)
        {
            var b = new CssBuilder("mud-day");
            if (day < GetMonthStart(month) || day > GetMonthEnd(month))
                return b.AddClass("mud-hidden").Build();
            if (_date.HasValue && _date.Value.Date == day)
                return b.AddClass("mud-selected").AddClass($"mud-theme-{Color.ToDescriptionString()}").Build();
            if (day == DateTime.Today)
                return b.AddClass("mud-current").AddClass($"mud-{Color.ToDescriptionString()}-text").Build();
            return b.Build();
        }

        protected override async void OnDayClicked(DateTime dateTime)
        {
            Date = dateTime;
            if (PickerVariant != PickerVariant.Static)
            {
                await Task.Delay(ClosingDelay);
                Picker.Close();
            }
        }

        protected override string GetFormattedDateString()
        {
            if (Date == null)
                return "";
            return Date.Value.ToString("ddd, dd MMM", Culture);
        }
    }
}
