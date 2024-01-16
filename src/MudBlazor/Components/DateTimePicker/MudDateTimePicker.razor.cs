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
        public EventCallback<DateTime?> DateTimeChanged { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public DateTime? DateTime
        {
            get => GetDateTime();
            set => SetDateTime(value);
        }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool AutoClose { get; set; } = false;

        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string DateFormat { get; set; } = "yyyy/MM/dd";

        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string TimeFormat { get; set; } = "HH:mm:ss";

        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string DateTimeFormat { get; set; } = "yyyy/MM/dd HH:mm:ss";

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

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public DateTime? PickerMonth { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowWeekNumbers { get; set; } = false;

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public DateTime? StartMonth { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public string TitleDateFormat { get; set; } = "ddd, dd MMM hh:mm";

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int? MaxMonthColumns { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public string NextIcon { get; set; } = Icons.Material.Filled.NavigateNext;

        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public string PreviousIcon { get; set; } = Icons.Material.Filled.NavigateBefore;

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
        public EventCallback<DateTime?> PickerMonthChanged { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Func<DateTime, bool> IsDateTimeDisabledFunc { get; set; }

        /// <summary>
        /// Function to conditionally apply new classes to specific days
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Func<DateTime, string> AdditionalDateClassesFunc { get; set; }

        private DateTime? _datePicked { get; set; }
        private TimeSpan? _timePicked { get; set; }

        private MudDatePicker _datePickerRef { get; set; }
        private MudTimePicker _timePickerRef { get; set; }

        public MudDateTimePicker() : base(new DefaultConverter<DateTime?>())
        {
            Converter.GetFunc = OnGet;
            Converter.SetFunc = OnSet;
            ((DefaultConverter<DateTime?>)Converter).Format = DateTimeFormat;
            FirstDayOfWeek = Culture.DateTimeFormat.FirstDayOfWeek;
        }

        protected string OnSet(DateTime? value)
        {
            if (value == null)
                return null;

            return value?.ToString(GetDateTimeFormat()) ?? null;
        }

        protected override Task StringValueChanged(string value)
        {
            Touched = true;
            DateTime? date = Converter.Get(value);
            _datePicked = date?.Date;
            _timePicked = date?.TimeOfDay;
            return base.StringValueChanged(value);
        }

        protected DateTime? OnGet(string value)
        {
            if (string.IsNullOrEmpty(value)) 
                return null;

            bool parsed = System.DateTime.TryParseExact(value, ((DefaultConverter<DateTime?>)Converter).Format, Culture, DateTimeStyles.None, out var date);
            
            if (parsed)
                return date;

            HandleParsingError();
            return null;
        }

        private void HandleParsingError()
        {
            const string ParsingErrorMessage = "Not a valid datetime";
            Converter.GetError = true;
            Converter.GetErrorMessage = ParsingErrorMessage;
            Converter.OnError?.Invoke(ParsingErrorMessage);
        }

        /// <summary>
        /// Called when a new date is picked
        /// </summary>
        protected void DateSelected(DateTime? date)
        {
            _datePicked = date;
            SubmitAndClose();
        }

        /// <summary>
        /// Called when a new time is picked
        /// </summary>
        protected void TimeSelected(TimeSpan? time)
        {
            _timePicked = time;
            SubmitAndClose();
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
            return GetDateTime()?.ToString(TitleDateFormat);
        }

        private void OnFormattedDateClick()
        {
            // todo: raise an event the user can handle
        }

        private void OnYearClick()
        {
            if (!FixYear.HasValue)
            {
                _datePickerRef.SwitchCurrentView(OpenTo.Year);
            }
        }

        /// <summary>
        /// Sets the date and time selection
        /// </summary>
        protected void SetDateTime(DateTime? dateTime)
        {
            _datePicked = dateTime?.Date;
            _timePicked = dateTime?.TimeOfDay;
            SetDateTimeAsync(dateTime, true).AndForget();
            SetTextAsync(Converter.Set(_value), false).AndForget();
        }

        protected async Task SetDateTimeAsync(DateTime? date, bool updateValue)
        {
            if (_value != null && date != null && date.Value.Kind == DateTimeKind.Unspecified)
            {
                date = System.DateTime.SpecifyKind(date.Value, _value.Value.Kind);
            }

            _datePicked = date?.Date;
            _timePicked = date?.TimeOfDay;
            StateHasChanged();

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
                    await DateTimeChanged.InvokeAsync(GetDateTime());
                }
                await DateTimeChanged.InvokeAsync(_value);
                await BeginValidateAsync();
                FieldChanged(_value);
            }
        }

        protected Task SetDateTimeText()
        {
            string dateTimeString = GetDateTime()?.ToString(GetDateTimeFormat()) ?? string.Empty;
            return SetTextAsync(dateTimeString, false);
        }

        protected string GetDateTimeFormat()
        {
            string format = DateTimeFormat;
            return format;
        }

        private void SubmitAndClose()
        {
            if (AutoClose && PickerVariant != PickerVariant.Static)
            {
                Close(AutoClose);
            }
        }

        protected internal override void Submit()
        {
            base.Submit();
            SetDateTimeAsync(GetDateTime(), false).AndForget();
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
            SetDateTimeAsync(null, true).AndForget();
            base.Clear(close);
        }
    }
}
