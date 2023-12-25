﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class DateTimePicker : MudPicker<DateTime?>
    {
        [Parameter] public EventCallback<DateTime?> DateChanged { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public DateTime? Date
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
        public int DisplayMonths { get; set; } = 1;

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
        public string TitleDateFormat { get; set; } = "ddd, dd MMM";

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
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback<DateTime?> PickerMonthChanged { get; set; }

        private DateTime? _datePicked { get; set; }
        private TimeSpan? _timePicked { get; set; }

        public DateTimePicker() : base(new DefaultConverter<DateTime?>())
        {
            Converter.GetFunc = OnGet;
            Converter.SetFunc = OnSet;
            FirstDayOfWeek = Culture.DateTimeFormat.FirstDayOfWeek;
        }

        public string OnSet(DateTime? value)
        {
            if (value == null)
                return null;

            return value?.ToString(GetDateTimeFormat()) ?? null;
        }

        public DateTime? OnGet(string value)
        {
            if (string.IsNullOrEmpty(value)) 
                return null;

            bool parsed = DateTime.TryParseExact(value, ((DefaultConverter<DateTime?>)Converter).Format, Culture, DateTimeStyles.None, out DateTime date);
            
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
        protected Task DateSelected(DateTime? date)
        {
            _datePicked = date;
            if (_datePicked != null && _timePicked != null)
            {
                if (DateChanged.HasDelegate)
                {
                    DateChanged.InvokeAsync(GetDateTime());
                }
                SubmitAndClose();
            }
            return SetDateTimeText();
        }

        /// <summary>
        /// Called when a new time is picked
        /// </summary>
        protected Task TimeSelected(TimeSpan? time)
        {
            _timePicked = time;
            if (_datePicked != null && _timePicked != null)
            {
                if (DateChanged.HasDelegate)
                {
                    DateChanged.InvokeAsync(GetDateTime());
                }
                SubmitAndClose();
            }
            return SetDateTimeText();
        }

        protected DateTime? GetDateTime() 
            => (_datePicked != null && _timePicked != null) ? _datePicked?.Add(_timePicked ?? TimeSpan.Zero) : null;

        /// <summary>
        /// Sets the date and time selection
        /// </summary>
        protected Task SetDateTime(DateTime? dateTime)
        {
            _datePicked = dateTime?.Date;
            _timePicked = dateTime?.TimeOfDay;
            return SetDateTimeText();
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
            Submit();
            if (AutoClose && PickerVariant != PickerVariant.Static)
            {
                Close();
            }
        }

        /// <summary>
        /// Clears the date and time selection
        /// </summary>
        public override void Clear(bool close = true)
        {
            SetDateTime(null);
            base.Clear(close);
        }
    }
}
