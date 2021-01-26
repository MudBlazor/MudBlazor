using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using static System.String;

namespace MudBlazor
{
    public class MudDatePicker : MudBaseDatePicker
    {
        private DateTime? _date;

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
            set => SetDateAsync(value, true).AndForget();
        }

        protected override void OnOpened()
        {
            Picker.Open();
        }

        protected override void OnClosed()
        {
            Picker.Close();
        }

        protected async Task SetDateAsync(DateTime? date, bool updateValue)
        {
            if (_date != date)
            {
                _date = date;

                if (updateValue)
                {
                    if ((!IsNullOrEmpty(DateFormat)) && _date.HasValue)
                        await SetValueAsync(_date.Value.ToString(DateFormat, Culture), false);
                    else
                        await SetValueAsync(_date.ToIsoDateString(), false);
                }

                await DateChanged.InvokeAsync(_date);
            }
        }

        protected override Task StringValueChanged(string value)
        {
            // Update the date property (without updating back the Value property)
            return SetDateAsync(ParseDateValue(value), false);
        }

        private DateTime? ParseDateValue(string value)
        {
            return DateTime.TryParse(value, out var date) ? date : (DateTime?)null;
        }

        protected override string GetDayClasses(int month, DateTime day)
        {
            var b = new CssBuilder("mud-day");
            if (day < GetMonthStart(month) || day > GetMonthEnd(month))
                return b.AddClass("mud-hidden").Build();
            if (Date?.Date == day)
                return b.AddClass("mud-selected").AddClass($"mud-theme-{Color.ToDescriptionString()}").Build();
            if (day == DateTime.Today)
                return b.AddClass("mud-current").AddClass($"mud-{Color.ToDescriptionString()}-text").Build();
            return b.Build();
        }

        protected override async void OnDayClicked(DateTime dateTime)
        {
            await SetDateAsync(dateTime, true);

            if (PickerVariant != PickerVariant.Static)
            {
                await Task.Delay(ClosingDelay);
                Picker.Close();
            }
        }

        protected override string GetFormattedDateString()
        {
            return Date?.ToString("ddd, dd MMM", Culture) ?? "";
        }
    }
}
