using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MudBlazor
{
    public partial class MudDatePicker : MudBasePicker
    {
        /// <summary>
        /// Max selectable date.
        /// </summary>
        [Parameter] public DateTime? MaxDate { get; set; }

        /// <summary>
        /// Max selectable date.
        /// </summary>
        [Parameter] public DateTime? MinDate { get; set; }

        /// <summary>
        /// First view to show in the MudDatePicker.
        /// </summary>
        [Parameter] public OpenTo OpenTo { get; set; } = OpenTo.Date;

        /// <summary>
        /// Sets the Input Icon.
        /// </summary>
        [Parameter] public string InputIcon { get; set; } = Icons.Material.Event;

        protected DateTime? _date;

        /// <summary>
        /// The currently selected date (two-way bindable). If null, then nothing was selected.
        /// </summary>
        [Parameter] public DateTime? Date
        {
            get => _date;
            set
            {
                if (value == _date)
                    return;
                _date = value;
                Value = _date.ToIsoDateString();
                InvokeAsync(StateHasChanged);
                DateChanged.InvokeAsync(value);
            }
        }

        /// <summary>
        /// Fired when the date changes.
        /// </summary>
        [Parameter] public EventCallback<DateTime?> DateChanged { get; set; }

        /// <summary>
        /// Defines on which day the week starts. Defaults to DayOfWeek.Monday
        /// </summary>
        [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Monday;

        /// <summary>
        /// The current month of the date picker (two-way bindable). This changes when the user browses through the calender.
        /// The month is represented as a DateTime which is always the first day of that month. You can also set this to define which month is initially shown. If not set, the current month is shown.
        /// </summary>
        [Parameter]
        public DateTime? PickerMonth
        {
            get => _picker_month;
            set
            {
                if (value == _picker_month)
                    return;
                _picker_month = value;
                InvokeAsync(StateHasChanged);
                PickerMonthChanged.InvokeAsync(value);
            }
        }

        private DateTime? _picker_month;

        /// <summary>
        /// Fired when the date changes.
        /// </summary>
        [Parameter] public EventCallback<DateTime?> PickerMonthChanged { get; set; }



        protected override void StringValueChanged(string value)
        {
            // update the date property
            if (DateTime.TryParse(value, out var date))
                Date = date;
            else
                Date = null;
        }

        /// <summary>
        /// Get the first of the month to display
        /// </summary>
        /// <returns></returns>
        protected DateTime GetMonthStart() => _picker_month == null ? DateTime.Today.StartOfMonth() : _picker_month.Value;

        /// <summary>
        /// Get the last of the month to display
        /// </summary>
        /// <returns></returns>
        protected DateTime GetMonthEnd() => _picker_month == null ? DateTime.Today.EndOfMonth() : _picker_month.Value;

        /// <summary>
        /// Gets the n-th week of the currently displayed month. 
        /// </summary>
        /// <param name="index">between 0 and 4</param>
        /// <returns></returns>
        protected IEnumerable<DateTime> GetWeek(int index)
        {
            if (index < 0 || index > 5)
                throw new ArgumentException("Index must be between 0 and 5");
            var month_first = GetMonthStart();
            var week_first = month_first.AddDays(index*7).StartOfWeek(FirstDayOfWeek);
            for (int i = 0; i < 7; i++)
                yield return week_first.AddDays(i);
        }

        protected string GetDayClasses(DateTime day)
        {
            var b = new CssBuilder("mud-day");
            if (day < GetMonthStart() || day > GetMonthEnd())
                return b.AddClass("mud-hidden").Build();
            if (_date.HasValue && _date.Value.Date == day)
                 return b.AddClass("mud-selected").AddClass($"mud-theme-color-{Color.ToDescriptionString()}").Build();
            if (day == DateTime.Today)
                return b.AddClass("mud-current").AddClass($"mud-color-text-{Color.ToDescriptionString()}").Build();
            return b.Build();
        }

        /// <summary>
        /// User clicked on a day
        /// </summary>
        private void OnDayClicked(DateTime dateTime)
        {
            Date = dateTime;
            Close();
        }
    }
}
