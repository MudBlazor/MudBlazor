// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class MudDateTimePicker : MudPicker<DateTime?>
    {
        /// <summary>
        /// The currently selected datetime (two-way bindable). If null, then nothing was selected.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public DateTime? DateTime
        {
            get => GetDateTime();
            set => SetDateTime(value, true);
        }

        /// <summary>
        /// Fired when the DateTime changes.
        /// </summary>
        [Parameter]
        public EventCallback<DateTime?> DateTimeChanged { get; set; }

        /// <summary>
        /// If AutoClose is set to true and PickerActions are defined, selecting a day will close the MudDatePicker.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool AutoClose { get; set; } = false;

        /// <summary>
        /// String Format for selected datetime view
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string DateTimeFormat
        {
            get => GetDateTimeFormat();
            set => SetDateTimeFormat(value);
        }

        /// <summary>
        /// The min selectable date for the picker. If null, there is no minimum.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public DateTime? MinDateTime { get; set; }

        /// <summary>
        /// The max selectable date for the picker. If null, there is no maximum.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public DateTime? MaxDateTime { get; set; }

        /// <summary>
        /// Defines on which day the week starts. Depends on the value of Culture. 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public DayOfWeek FirstDayOfWeek { get; set; }

        /// <summary>
        /// Set a predefined fix day - no day can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? FixDay { get; set; }

        /// <summary>
        /// Set a predefined fix month - no month can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? FixMonth { get; set; }

        /// <summary>
        /// Set a predefined fix year - no year can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? FixYear { get; set; }

        /// <summary>
        /// First view to show in the MudDatePicker.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public OpenTo DateOpenTo { get; set; } = OpenTo.Date;

        /// <summary>
        /// The current month of the date picker (two-way bindable). This changes when the user browses through the calender.
        /// The month is represented as a DateTime which is always the first day of that month. You can also set this to define which month is initially shown. If not set, the current month is shown.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public DateTime? PickerMonth
        {
            get => _pickerMonth;
            set => _pickerMonth = value;
        }

        /// <summary>
        /// Fired when the month changes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback<DateTime?> PickerMonthChanged { get; set; }

        /// <summary>
        /// Display week numbers according to the Culture parameter. If no culture is defined, CultureInfo.CurrentCulture will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowWeekNumbers { get; set; } = false;

        /// <summary>
        /// Start month when opening the picker. 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public DateTime? StartMonth { get; set; }

        /// <summary>
        /// Format of the selected date in the title. By default, this is "ddd, dd MMM" which abbreviates day and month names. 
        /// For instance, display the long names like this "dddd, dd. MMMM". 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public string TitleDateTimeFormat { get; set; } = "ddd, dd MMM HH:mm";

        /// <summary>
        /// Custom next icon for the date.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public string DateNextIcon { get; set; } = Icons.Material.Filled.NavigateNext;

        /// <summary>
        /// Custom previous icon for the date.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public string DatePreviousIcon { get; set; } = Icons.Material.Filled.NavigateBefore;

        /// <summary>
        /// First view to show in the Picker for the time.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public OpenTo TimeOpenTo { get; set; } = OpenTo.Hours;

        /// <summary>
        /// Sets the number interval for minutes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int MinuteSelectionStep { get; set; } = 1;

        /// <summary>
        /// Choose the edition mode. By default, you can edit hours and minutes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public TimeEditMode TimeEditMode { get; set; } = TimeEditMode.Normal;

        /// <summary>
        /// Function to determine whether a date is disabled
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Func<DateTime, bool> IsDateTimeDisabledFunc { get; set; }

        /// <summary>
        /// Function to conditionally apply new classes to specific days.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Func<DateTime, string> AdditionalDateClassesFunc { get; set; }

        /// <summary>
        /// Called when the title date is clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback<DateTime> FormattedDateClick { get; set; }

        private DateTime? _datePicked { get; set; }
        private TimeSpan? _timePicked { get; set; }
        private DateTime? _pickerMonth { get; set; }

        private bool _datePickedChanged { get; set; }
        private bool _timePickedChanged { get; set; }

        private MudDatePicker _datePickerRef { get; set; }
        private MudTimePicker _timePickerRef { get; set; }

        public MudDateTimePicker() : base(new DefaultConverter<DateTime?>())
        { }

        protected override void OnInitialized()
        {
            Converter.GetFunc = OnGet;
            Converter.SetFunc = OnSet;
            ((DefaultConverter<DateTime?>)Converter).Culture = Culture;
            ((DefaultConverter<DateTime?>)Converter).Format = null;
        }

        protected override async Task OnPickerOpenedAsync()
        {
            _datePickedChanged = false;
            _timePickedChanged = false;
            await base.OnPickerOpenedAsync();
        }

        protected string OnSet(DateTime? value)
        {
            return value?.ToString(GetDateTimeFormat()) ?? null;
        }

        protected DateTime? OnGet(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            DateTime date;
            bool parsed = System.DateTime.TryParseExact(value, GetDateTimeFormat(), Culture, DateTimeStyles.None, out date);
            if (!parsed)
            {
                parsed = System.DateTime.TryParse(value, Culture, DateTimeStyles.None, out date);
            }
            if (parsed)
            {
                return date;
            }
            HandleParsingError();
            return null;
        }

        protected void HandleParsingError()
        {
            const string ParsingErrorMessage = "Not a valid datetime";
            Converter.GetError = true;
            Converter.GetErrorMessage = ParsingErrorMessage;
            Converter.OnError?.Invoke(ParsingErrorMessage);
        }

        protected override Task StringValueChangedAsync(string value)
        {
            Touched = true;
            if (string.IsNullOrEmpty(value))
            {
                ClearAsync(false).AndForget();
            }
            else
            {
                DateTime? dateTime = Converter.Get(value);
                if (dateTime is not null)
                {
                    SetDateTime(dateTime, false);
                }
            }
            return base.StringValueChangedAsync(value);
        }

        protected void SetDateTimeFormat(string format)
        {
            ((DefaultConverter<DateTime?>)Converter).Format = format;
            StateHasChanged();
        }

        protected string GetDateTimeFormat()
        {
            if (!string.IsNullOrEmpty(((DefaultConverter<DateTime?>)Converter).Format))
            {
                return ((DefaultConverter<DateTime?>)Converter).Format;
            }
            else
            {
                return $"{Culture.DateTimeFormat.ShortDatePattern} {Culture.DateTimeFormat.ShortTimePattern}";
            }
        }

        protected DateTime? GetDateTime()
        {
            return _value;
        }

        protected DateTime? GetPartialDateTime()
        {
            return _datePicked is null || _timePicked is null ? null : _datePicked?.Add((TimeSpan)_timePicked);
        }

        protected string GetFormattedYearString()
        {
            return (_pickerMonth ?? System.DateTime.Today).ToString("yyyy");
        }

        protected string GetTitleDateString()
        {
            string dateTimeFormat = TitleDateTimeFormat;
            if (_datePicked is null)
            {
                dateTimeFormat = dateTimeFormat
                    .Replace("y", "-")
                    .Replace("Y", "-")
                    .Replace("M", "-")
                    .Replace("d", "-");
            }
            if (_timePicked is null)
            {
                dateTimeFormat = dateTimeFormat
                    .Replace("h", "-")
                    .Replace("H", "-")
                    .Replace("m", "-")
                    .Replace("s", "-");
            }
            return (_datePicked?.Add(_timePicked ?? TimeSpan.Zero) ?? System.DateTime.MinValue.Add(_timePicked ?? TimeSpan.Zero)).ToString(dateTimeFormat, Culture);
        }

        protected virtual void OnFormattedDateClick()
        {
            FormattedDateClick.InvokeAsync();
        }

        private void OnYearClick()
        {
            if (!FixYear.HasValue)
            {
                _datePickerRef.SwitchCurrentView(OpenTo.Year);
            }
        }

        private async Task OnPickerMonthChanged(DateTime? month)
        {
            _pickerMonth = month;
            await PickerMonthChanged.InvokeAsync(month);
        }

        /// <summary>
        /// Called when a new date is picked
        /// </summary>
        protected void DateSelected(DateTime? date)
        {
            _datePickedChanged = _datePicked?.Date != date?.Date;
            _datePicked = date;
            SubmitAndClose();
        }

        /// <summary>
        /// Called when a new time is picked
        /// </summary>
        protected void TimeSelected(TimeSpan? time)
        {
            _timePickedChanged = _timePicked is not null && (_timePicked?.Minutes != time?.Minutes || _timePicked?.Seconds != time?.Seconds);
            _timePicked = time;
            SubmitAndClose();
        }

        /// <summary>
        /// Sets the date and time selection
        /// </summary>
        protected void SetDateTime(DateTime? dateTime, bool updateValue)
            => SetDateTimeAsync(dateTime, updateValue).AndForget();

        /// <summary>
        /// Goes to the specific date
        /// </summary>
        /// <param name="date"></param>
        /// <param name="submitDate"></param>
        public async Task GoToDate(DateTime date, bool submitDate = true)
            => await _datePickerRef.GoToDate(date, submitDate);

        /// <summary>
        /// Goes to the current date
        /// </summary>
        public void GoToDate()
            => _datePickerRef.GoToDate();

        protected async Task SetDateTimeAsync(DateTime? date, bool updateValue)
        {
            if (_value != null && date != null && date.Value.Kind == DateTimeKind.Unspecified)
            {
                date = System.DateTime.SpecifyKind(date.Value, _value.Value.Kind);
            }

            _datePicked = date?.Date;
            _timePicked = date?.TimeOfDay;

            if (_value != date)
            {
                Touched = true;

                if (date is not null && IsDateTimeDisabledFunc is not null && IsDateTimeDisabledFunc((DateTime) date))
                {
                    return;
                }

                _value = date;
                if (updateValue)
                {
                    Converter.GetError = false;
                    await SetTextAsync(Converter.Set(_value), false);
                }
                await DateTimeChanged.InvokeAsync(_value);
                await BeginValidateAsync();
                FieldChanged(_value);
            }
        }

        private void SubmitAndClose()
        {
            if (AutoClose && PickerVariant is not PickerVariant.Static && _datePickedChanged && _timePickedChanged)
            {
                CloseAsync().AndForget();
            }
        }

        protected internal override async Task SubmitAsync()
        {
            // Save the current partial date to the current value date
            await SetDateTimeAsync(GetPartialDateTime(), true);
        }

        protected override async Task OnClosedAsync()
        {
            await SubmitAsync();
            await base.OnClosedAsync();
        }

        /// <summary>
        /// Clears the date and time selection
        /// </summary>
        public override async Task ClearAsync(bool close = true)
        {
            SetDateTimeAsync(null, close).AndForget();
            await base.ClearAsync(close);
        }
    }
}
