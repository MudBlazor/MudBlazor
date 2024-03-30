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
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public DateTime? DateTime
        {
            get => GetDateTime();
            set => SetDateTime(value, true);
        }

        [Parameter]
        public EventCallback<DateTime?> DateTimeChanged { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool AutoClose { get; set; } = false;

        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string DateTimeFormat
        {
            get => GetDateTimeFormat();
            set => SetDateTimeFormat(value);
        }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public DateTime? MinDateTime { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public DateTime? MaxDateTime { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public DayOfWeek FirstDayOfWeek { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? FixDay { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? FixMonth { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? FixYear { get; set; }

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

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback<DateTime?> PickerMonthChanged { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowWeekNumbers { get; set; } = false;

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public DateTime? StartMonth { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public string TitleDateTimeFormat { get; set; } = "ddd, dd MMM HH:mm";

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public string DateNextIcon { get; set; } = Icons.Material.Filled.NavigateNext;

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public string DatePreviousIcon { get; set; } = Icons.Material.Filled.NavigateBefore;

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public OpenTo TimeOpenTo { get; set; } = OpenTo.Hours;

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int MinuteSelectionStep { get; set; } = 1;

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public TimeEditMode TimeEditMode { get; set; } = TimeEditMode.Normal;

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Func<DateTime, bool> IsDateTimeDisabledFunc { get; set; }

        /// <summary>
        /// Function to conditionally apply new classes to specific days
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Func<DateTime, string> AdditionalDateClassesFunc { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback<DateTime> FormattedDateClick { get; set; }

        private DateTime? _datePicked { get; set; }
        private TimeSpan? _timePicked { get; set; }
        private DateTime? _pickerMonth { get; set; }

        private int _dateInstanceSelectionCount = 0;
        private int _timeInstanceSelectionCount = 0;
        private bool _dateInstanceSelectionReady => _dateInstanceSelectionCount >= 1; 
        private bool _timeInstanceSelectionReady => _timeInstanceSelectionCount >= 2;

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

        protected override void OnPickerOpened()
        {
            _dateInstanceSelectionCount = _timeInstanceSelectionCount = 0;
            base.OnPickerOpened();
        }

        protected override Task StringValueChanged(string value)
        {
            Touched = true;
            if (string.IsNullOrEmpty(value))
            {
                Clear(false);
            }
            else
            {
                DateTime? dateTime = Converter.Get(value);
                if (dateTime is not null)
                {
                    SetDateTime(dateTime, false);
                }
            }
            return base.StringValueChanged(value);
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
            return _datePicked == null || _timePicked == null ? null : _datePicked?.Add((TimeSpan) _timePicked);
        }

        protected string GetFormattedYearString()
        {
            return (GetDateTime() ?? System.DateTime.Today).ToString("yyyy");
        }

        protected string GetTitleDateString()
        {
            return GetDateTime()?.ToString(TitleDateTimeFormat, Culture);
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
            _datePicked = date;
            _dateInstanceSelectionCount = 1;
            SubmitAndClose();
        }

        /// <summary>
        /// Called when a new time is picked
        /// </summary>
        protected void TimeSelected(TimeSpan? time)
        {
            if (time is not null && _timePicked is null)
            {
                _timeInstanceSelectionCount = 1;
            }
            else
            {
                if (time?.Hours != _timePicked?.Hours)
                {
                    _timeInstanceSelectionCount = 1;
                }
                else if (time?.Minutes != _timePicked?.Minutes)
                {
                    _timeInstanceSelectionCount = 2;
                }
            }
            _timePicked = time;
            SubmitAndClose();
        }

        /// <summary>
        /// Sets the date and time selection
        /// </summary>
        protected void SetDateTime(DateTime? dateTime, bool updateValue)
            => SetDateTimeAsync(dateTime, updateValue).AndForget();

        public async Task GoToDate(DateTime date, bool submitDate = true)
            => await _datePickerRef.GoToDate(date, submitDate);

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
            // Only autoclose if both date and time where clicked after this instance of the dialog was opened
            if (AutoClose && PickerVariant is not PickerVariant.Static && _dateInstanceSelectionReady && _timeInstanceSelectionReady)
            {
                Close(_datePicked is not null && _timePicked is not null);
            }
        }

        protected internal override void Submit()
        {
            base.Submit();
            SetDateTimeAsync(GetDateTime(), true).AndForget();
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            _datePicked = _value?.Date;
            _timePicked = _value?.TimeOfDay;
            StateHasChanged();
        }

        /// <summary>
        /// Clears the date and time selection
        /// </summary>
        public override void Clear(bool close = true)
        {
            SetDateTimeAsync(null, close).AndForget();
            base.Clear(close);
        }
    }
}
