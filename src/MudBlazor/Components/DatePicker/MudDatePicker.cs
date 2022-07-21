﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

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

        /// <summary>
        /// If AutoClose is set to true and PickerActions are defined, selecting a day will close the MudDatePicker.
        /// </summary>
        [Parameter] public bool AutoClose { get; set; }

        protected async Task SetDateAsync(DateTime? date, bool updateValue)
        {
            if (_value != date)
            {
                Touched = true;

                if (date is not null && IsDateDisabledFunc(date.Value.Date))
                {
                    await SetTextAsync(null, false);
                    return;
                }

                _value = date;
                if (updateValue)
                {
                    Converter.GetError = false;
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
                return b.AddClass("mud-current mud-button-outlined").AddClass($"mud-button-outlined-{Color.ToDescriptionString()} mud-{Color.ToDescriptionString()}-text").Build();
            return b.Build();
        }

        protected override async void OnDayClicked(DateTime dateTime)
        {
            _selectedDate = dateTime;
            if (PickerActions == null || AutoClose)
            {
                Submit();

                if (PickerVariant != PickerVariant.Static)
                {
                    await Task.Delay(ClosingDelay);
                    Close(false);
                }
            }
        }

        /// <summary>
        /// user clicked on a month
        /// </summary>
        /// <param name="month"></param>
        protected override void OnMonthSelected(DateTime month)
        {
            PickerMonth = month;
            var nextView = GetNextView();
            if (nextView == null)
            {
                _selectedDate = _selectedDate.HasValue ?
                    //everything has to be set because a value could already defined -> fix values can be ignored as they are set in submit anyway
                    new DateTime(month.Year, month.Month, _selectedDate.Value.Day, _selectedDate.Value.Hour, _selectedDate.Value.Minute, _selectedDate.Value.Second, _selectedDate.Value.Millisecond, _selectedDate.Value.Kind)
                    //We can assume day here, as it was not set yet. If a fix value is set, it will be overriden in Submit
                    : new DateTime(month.Year, month.Month, 1);
                SubmitAndClose();
            }
            else
            {
                CurrentView = (OpenTo)nextView;
            }
        }

        /// <summary>
        /// user clicked on a year
        /// </summary>
        /// <param name="year"></param>
        protected override void OnYearClicked(int year)
        {
            var current = GetMonthStart(0);
            PickerMonth = new DateTime(year, current.Month, 1);
            var nextView = GetNextView();
            if (nextView == null)
            {
                _selectedDate = _selectedDate.HasValue ?
                    //everything has to be set because a value could already defined -> fix values can be ignored as they are set in submit anyway
                    new DateTime(_selectedDate.Value.Year, _selectedDate.Value.Month, _selectedDate.Value.Day, _selectedDate.Value.Hour, _selectedDate.Value.Minute, _selectedDate.Value.Second, _selectedDate.Value.Millisecond, _selectedDate.Value.Kind)
                    //We can assume month and day here, as they were not set yet
                    : new DateTime(year, 1, 1);
                SubmitAndClose();
            }
            else
            {
                CurrentView = (OpenTo)nextView;
            }
        }

        protected override void OnOpened()
        {
            _selectedDate = null;

            base.OnOpened();
        }

        protected override async void Submit()
        {
            if (ReadOnly)
                return;
            if (_selectedDate == null)
                return;

            if (FixYear.HasValue || FixMonth.HasValue || FixDay.HasValue)
                _selectedDate = new DateTime(FixYear ?? _selectedDate.Value.Year,
                    FixMonth ?? _selectedDate.Value.Month,
                    FixDay ?? _selectedDate.Value.Day,
                    _selectedDate.Value.Hour,
                    _selectedDate.Value.Minute,
                    _selectedDate.Value.Second,
                    _selectedDate.Value.Millisecond);

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
            return FormatTitleDate(_selectedDate ?? Date);
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

        //To be completed on next PR
        protected internal override void HandleKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly)
                return;
            base.HandleKeyDown(obj);
            switch (obj.Key)
            {
                case "ArrowRight":
                    if (IsOpen)
                    {

                    }
                    break;
                case "ArrowLeft":
                    if (IsOpen)
                    {

                    }
                    break;
                case "ArrowUp":
                    if (IsOpen == false && Editable == false)
                    {
                        IsOpen = true;
                    }
                    else if (obj.AltKey == true)
                    {
                        IsOpen = false;
                    }
                    else if (obj.ShiftKey == true)
                    {
                        
                    }
                    else
                    {
                        
                    }
                    break;
                case "ArrowDown":
                    if (IsOpen == false && Editable == false)
                    {
                        IsOpen = true;
                    }
                    else if (obj.ShiftKey == true)
                    {
                        
                    }
                    else
                    {
                        
                    }
                    break;
                case "Escape":
                    ReturnDateBackUp();
                    break;
                case "Enter":
                case "NumpadEnter":
                    if (!IsOpen)
                    {
                        Open();
                    }
                    else
                    {
                        Submit();
                        Close();
                        _inputReference?.SetText(Text);
                    }
                    break;
                case " ":
                    if (!Editable)
                    {
                        if (!IsOpen)
                        {
                            Open();
                        }
                        else
                        {
                            Submit();
                            Close();
                            _inputReference?.SetText(Text);
                        }
                    }
                    break;
            }
        }

        private void ReturnDateBackUp()
        {
            Close();
        }
    }
}
