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
        private DateTime? _selectedDate;

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
            get => _value;
            set => SetDateAsync(value, true).AndForget();
        }

        protected async Task SetDateAsync(DateTime? date, bool updateValue)
        {
            if (_value != date)
            {
                _value = date;
                if (updateValue)
                {
                    await SetTextAsync(Converter.Set(_value), false);
                }
                await DateChanged.InvokeAsync(_value);
                BeginValidate();
            }
        }

        protected override Task DateFormatChanged(string newFormat)
        {
            Touched = true;
            return SetTextAsync(Converter.Set(_value), false);
        }

        protected override Task StringValueChanged(string value)
        {
            Touched = true;
            // Update the date property (without updating back the Value property)
            return SetDateAsync(Converter.Get(value), false);
        }

        protected override string GetDayClasses(int month, DateTime day)
        {
            var b = new CssBuilder("mud-day");
            if (day < GetMonthStart(month) || day > GetMonthEnd(month))
                return b.AddClass("mud-hidden").Build();
            if ((Date?.Date == day && _selectedDate == null) || _selectedDate?.Date == day)
                return b.AddClass("mud-selected").AddClass($"mud-theme-{Color.ToDescriptionString()}").Build();
            if (day == DateTime.Today)
                return b.AddClass("mud-current").AddClass($"mud-{Color.ToDescriptionString()}-text").Build();
            return b.Build();
        }

        protected override async void OnDayClicked(DateTime dateTime)
        {
            _selectedDate = dateTime;
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
            _selectedDate = null;

            base.OnOpened();
        }

        protected override async void Submit()
        {
            if (_selectedDate == null)
                return;

            await SetDateAsync(_selectedDate, true);
            _selectedDate = null;
        }

        public override void Clear(bool close = true)
        {
            Date = null;
            _selectedDate = null;
            base.Clear();
        }

        protected override string GetTitleDateString()
        {
            if (_selectedDate != null)
                return _selectedDate.Value.ToString(TitleDateFormat ?? "ddd, dd MMM", Culture) ?? "";
            return Date?.ToString(TitleDateFormat ?? "ddd, dd MMM", Culture) ?? "";
        }

        protected override DateTime GetCalendarStartOfMonth()
        {
            var date = StartMonth ?? Date ?? DateTime.Today;
            return date.StartOfMonth(Culture);
        }

        protected override int GetCalendarYear(int year)
        {
            var date = Date ?? DateTime.Today;
            var diff = date.Year - year;
            var calenderYear = Culture.Calendar.GetYear(date);
            return calenderYear - diff;
        }
    }
}
