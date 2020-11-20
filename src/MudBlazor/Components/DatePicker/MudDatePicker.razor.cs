using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public partial class MudDatePicker : MudBasePicker
    {
        [Inject] IJSRuntime JsRuntime { get; set; }
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

        /// <summary>
        /// Fired when the DateFormat changes.
        /// </summary>
        [Parameter] public EventCallback<DateTime?> DateChanged { get; set; }

        /// <summary>
        /// String Format for selected date view
        /// </summary>
        [Parameter] public string DateFormat { get; set; }

        /// <summary>
        /// Defines on which day the week starts. Depends on the value of Culture. 
        /// </summary>
        [Parameter] public DayOfWeek? FirstDayOfWeek { get; set; } = null;

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

        /// <summary>
        /// The display culture
        /// </summary>
        [Parameter] public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// Milliseconds to wait before closing the picker. This helps the user see that the date was selected before the popover disappears.
        /// </summary>
        [Parameter] public int ClosingDelay { get; set; } = 100;

        /// <summary>
        /// Reference to the Picker, initialized via @ref
        /// </summary>
        private MudPicker Picker;

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
        protected DateTime GetMonthEnd() => _picker_month == null ? DateTime.Today.EndOfMonth() : _picker_month.Value.EndOfMonth();

        protected DayOfWeek GetFirstDayOfWeek()
        {
            if (FirstDayOfWeek.HasValue)
                return FirstDayOfWeek.Value;
            return Culture.DateTimeFormat.FirstDayOfWeek;
        }

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
            var week_first = month_first.AddDays(index*7).StartOfWeek(GetFirstDayOfWeek( ));
            for (int i = 0; i < 7; i++)
                yield return week_first.AddDays(i);
        }

        protected string GetDayClasses(DateTime day)
        {
            var b = new CssBuilder("mud-day");
            if (day < GetMonthStart() || day > GetMonthEnd())
                return b.AddClass("mud-hidden").Build();
            if (_date.HasValue && _date.Value.Date == day)
                 return b.AddClass("mud-selected").AddClass($"mud-theme-{Color.ToDescriptionString()}").Build();
            if (day == DateTime.Today)
                return b.AddClass("mud-current").AddClass($"mud-{Color.ToDescriptionString()}-text").Build();
            return b.Build();
        }

        /// <summary>
        /// User clicked on a day
        /// </summary>
        protected async void OnDayClicked(DateTime dateTime)
        {
            Date = dateTime;
            await Task.Delay(ClosingDelay);
            Picker.Close();
        }

        /// <summary>
        /// return Mo, Tu, We, Th, Fr, Sa, Su in the right culture
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<string> GetAbbreviatedDayNames()
        {
            string[] dayNamesNormal = Culture.DateTimeFormat.AbbreviatedDayNames;
            string[] dayNamesShifted = Shift(dayNamesNormal, (int)GetFirstDayOfWeek());
            return dayNamesShifted;
        }

        /// <summary>
        /// Shift array and cycle around from the end
        /// </summary>
        private static T[] Shift<T>(T[] array, int positions)
        {
            T[] copy = new T[array.Length];
            Array.Copy(array, 0, copy, array.Length - positions, positions);
            Array.Copy(array, positions, copy, 0, array.Length - positions);
            return copy;
        }

        protected string GetMonthName()
        {
            return GetMonthStart().ToString(Culture.DateTimeFormat.YearMonthPattern, Culture);
        }

        protected string GetFormattedDateString()
        {
            if (Date == null)
                return "";
            return Date.Value.ToString("ddd, dd MMM", Culture);
        }        
        protected string GetFormattedYearString()
        {
            return GetMonthStart().ToString("yyyy", Culture);
        }

        private void OnPreviousMonthClick()
        {
            PickerMonth = GetMonthStart().AddDays(-1).StartOfMonth();
        }

        private void OnNextMonthClick()
        {
            PickerMonth = GetMonthEnd().AddDays(1);
        }

        private async void OnYearClick()
        {
            OpenTo = OpenTo.Year;
            //await InvokeAsync(StateHasChanged);
            StateHasChanged();
            _scrollToYearAfterRender = true;
        }

        /// <summary>
        /// We need a random id for the year items in the year list so we can scroll to the item safely in every DatePicker.
        /// </summary>
        private string _componentId = Guid.NewGuid().ToString();

        /// <summary>
        /// Is set to true to scroll to the actual year after the next render
        /// </summary>
        private bool _scrollToYearAfterRender = false;

        public async void ScrollToYear()
        {
            _scrollToYearAfterRender = false;
            string id = $"{_componentId}{GetMonthStart().Year.ToString()}";
            await JsRuntime.InvokeVoidAsync("blazorHelpers.scrollToFragment", id);
            StateHasChanged();
        }

        private int GetMinYear()
        {
            if (MinDate.HasValue)
                return MinDate.Value.Year;
            return DateTime.Today.Year - 100;
        }

        private int GetMaxYear()
        {
            if (MaxDate.HasValue)
                return MaxDate.Value.Year;
            return DateTime.Today.Year + 100;
        }

        private string GetYearClasses(int year)
        {
            if (year == GetMonthStart().Year)
                return $"mud-picker-year-selected mud-{Color.ToDescriptionString()}-text";
            return null;
        }
        private Typo GetYearTypo(int year)
        {
            if (year == GetMonthStart().Year)
                return Typo.h5;
            return Typo.subtitle1;
        }

        private void OnFormattedDateClick()
        {
            // todo: raise an event the user can handdle
        }

        private void OnYearClicked(int year)
        {
            OpenTo = OpenTo.Date;
            var current = GetMonthStart();
            PickerMonth = new DateTime(year, current.Month,  1);
        }

        private IEnumerable<DateTime> GetAllMonths()
        {
            var current = GetMonthStart();
            for (int i = 1; i <= 12; i++)
                yield return new DateTime(current.Year, i, 1);
        }

        private string GetAbbreviatedMontName(in DateTime month)
        {
            return Culture.DateTimeFormat.AbbreviatedMonthNames[month.Month-1];
        }

        private string GetMonthClasses(DateTime month)
        {
            if (GetMonthStart() == month)
                return $"mud-picker-month-selected mud-color-text-{Color.ToDescriptionString()}";
            return null;
        }

        private Typo GetMonthTypo(in DateTime month)
        {
            if (GetMonthStart() == month)
                return Typo.h5;
            return Typo.subtitle1;
        }

        private void OnMonthClicked()
        {
            OpenTo = OpenTo.Month;
            StateHasChanged();
        }

        private void OnMonthSelected(in DateTime month)
        {
            OpenTo = OpenTo.Date;
            PickerMonth = month;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && OpenTo == OpenTo.Year)
            {
                ScrollToYear();
                return;
            }
            if (_scrollToYearAfterRender)
                ScrollToYear();
            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
