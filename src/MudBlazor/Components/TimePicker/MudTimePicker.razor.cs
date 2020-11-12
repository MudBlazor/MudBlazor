using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;

namespace MudBlazor
{
    public partial class MudTimePicker : MudBasePicker
    {

        /// <summary>
        /// Reference to the Picker, initialized via @ref
        /// </summary>
        private MudPicker Picker;

        /// <summary>
        /// First view to show in the MudDatePicker.
        /// </summary>
        [Parameter] public OpenTo OpenTo { get; set; } = OpenTo.Hours;

        /// <summary>
        /// If true, sets 12 hour selection clock.
        /// </summary>
        [Parameter] public bool AmPm { get; set; }

        /// <summary>
        /// Sets the Input Icon.
        /// </summary>
        [Parameter] public string InputIcon { get; set; } = Icons.Material.Event;

        protected TimeSpan? _time;

        /// <summary>
        /// The currently selected time (two-way bindable). If null, then nothing was selected.
        /// </summary>
        [Parameter]
        public TimeSpan? Time
        {
            get => _time;
            set
            {
                if (value == _time)
                    return;
                _time = value;
                // to avoid an update loop we set flag _setting_value
                _setting_value = true;
                try
                {
                    Value = AmPm ? _time.ToAmPmString() : _time.ToIsoString();
                    UpdateTimeSetFromTime();
                }
                finally
                {
                    _setting_value = false;
                }
                InvokeAsync(StateHasChanged);
                TimeChanged.InvokeAsync(value);
            }
        }

        /// <summary>
        /// Fired when the date changes.
        /// </summary>
        [Parameter] public EventCallback<TimeSpan?> TimeChanged { get; set; }

        private bool _setting_value = false;
        protected override void StringValueChanged(string value)
        {
            if (_setting_value)
                return;
            Time = ParseTimeValue(value);
        }

        private TimeSpan? ParseTimeValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            bool pm = false;
            var value1 = value.Trim();
            var m = Regex.Match(value, "AM|PM", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                AmPm = true; // <-- this is kind of a hack, but we need to make sure it is set or else the string value might be converted to 24h format.
                pm = m.Value.ToLower() == "pm";
                value1 = Regex.Replace(value, "(AM|am|PM|pm)", "").Trim();
            }

            if (TimeSpan.TryParse(value1, out var time))
            {
                if (pm)
                    time = new TimeSpan((time.Hours + 12)%24, time.Minutes, 0);
                return time;
            }

            return null;
        }

        private string getHourString()
        {
            if (_time == null)
                return "--";
            var h = AmPm ? _time.Value.ToAmPmHour() : _time.Value.Hours;
            return Math.Min(23, Math.Max(0, h)).ToString(CultureInfo.InvariantCulture);
        }

        private string getMinuteString()
        {
            if (_time == null)
                return "--";
            return $"{Math.Min(59, Math.Max(0, _time.Value.Minutes)):D2}";
        }

        private void UpdateTime()
        {
            Time = new TimeSpan(TimeSet.Hour, TimeSet.Minute, 0);
        }

        private async void OnHourClick()
        {
            OpenTo = OpenTo.Hours;
            StateHasChanged();
        }

        private async void OnMinutesClick()
        {
            OpenTo = OpenTo.Minutes;
            StateHasChanged();
        }

        private void OnAmClicked()
        {
            TimeSet.Hour = TimeSet.Hour % 12;  // "12:-- am" is "00:--" in 24h
            UpdateTime();
            StateHasChanged();
        }

        private void OnPmClicked()
        {
            if (TimeSet.Hour <= 12) // "12:-- pm" is "12:--" in 24h
                TimeSet.Hour = TimeSet.Hour + 12;
            TimeSet.Hour = TimeSet.Hour % 24;
            UpdateTime();
            StateHasChanged();
        }

        protected string ToolbarClass =>
        new CssBuilder("mud-picker-timepicker-toolbar")
          .AddClass($"mud-picker-timepicker-toolbar-landscape", Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
        .Build();

        protected string HoursButtonClass =>
        new CssBuilder("mud-timepicker-button")
          .AddClass($"mud-timepicker-toolbar-text", OpenTo == OpenTo.Minutes)
        .Build();

        protected string MinuteButtonClass =>
        new CssBuilder("mud-timepicker-button")
          .AddClass($"mud-timepicker-toolbar-text", OpenTo == OpenTo.Hours)
        .Build();

        protected string AmButtonClass =>
        new CssBuilder("mud-timepicker-button")
          .AddClass($"mud-timepicker-toolbar-text", !IsAm) // gray it out
        .Build();

        protected string PmButtonClass =>
        new CssBuilder("mud-timepicker-button")
          .AddClass($"mud-timepicker-toolbar-text", !IsPm) // gray it out
        .Build();

        private bool IsAm => TimeSet.Hour >= 00 && TimeSet.Hour < 12; // am is 00:00 to 11:59 
        private bool IsPm => TimeSet.Hour >= 12 && TimeSet.Hour < 24; // pm is 12:00 to 23:59 

        private string GetClockPinColor()
        {
            return $"mud-picker-time-clock-pin mud-{Color.ToDescriptionString()}";
        }
        private string GetClockPointerColor()
        {
            if (MouseDown)
            {
                return $"mud-picker-time-clock-pointer mud-{Color.ToDescriptionString()}";
            }
            else
            {
                return $"mud-picker-time-clock-pointer mud-picker-time-clock-pointer-animation mud-{Color.ToDescriptionString()}";
            }
            
        }

        private string GetClockPointerThumbColor()
        {
            double deg = GetDeg();
            if (deg % 30 == 0)
            {
                return $"mud-picker-time-clock-pointer-thumb mud-onclock-text mud-onclock-primary mud-{Color.ToDescriptionString()}";
            }
            else
            {
                return $"mud-picker-time-clock-pointer-thumb mud-onclock-minute mud-{Color.ToDescriptionString()}-text";
            }
        }

        private string GetNumberColor(int value)
        {
            if(OpenTo == OpenTo.Hours)
            {
                var h = TimeSet.Hour;
                if (AmPm)
                {
                    h = TimeSet.Hour % 12;
                    if (TimeSet.Hour % 12 == 0)
                        h = 12;
                }
                if (h==value)
                    return $"mud-clock-number mud-theme-{Color.ToDescriptionString()}";
            }
            else if (OpenTo == OpenTo.Minutes && TimeSet.Minute == value)
            {
                return $"mud-clock-number mud-theme-{Color.ToDescriptionString()}";
            }
            return $"mud-clock-number";
        }

        private double GetDeg()
        {
            double deg = 0;
            if (OpenTo == OpenTo.Hours)
                deg = (TimeSet.Hour * 30) % 360;
            if (OpenTo == OpenTo.Minutes)
                deg = (TimeSet.Minute * 6) % 360;
            return deg;
        }

        private string GetPointerRotation()
        {
            double deg = 0;
            if (OpenTo == OpenTo.Hours)
                deg = (TimeSet.Hour * 30) % 360;
            if (OpenTo == OpenTo.Minutes)
                deg = (TimeSet.Minute * 6) % 360;
            return $"rotateZ({deg}deg);";
        }

        private string GetPointerHeight()
        {
            int height = 40;
            if (OpenTo == OpenTo.Minutes)
                height = 40;
            if (OpenTo == OpenTo.Hours)
            {
                if (!AmPm && TimeSet.Hour > 0 && TimeSet.Hour < 13)
                    height = 26;
                else
                    height = 40;
            }
            return $"{height}%;";
        }

        private SetTime TimeSet = new SetTime();

        protected override void OnInitialized()
        {
            UpdateTimeSetFromTime();
        }


        private void UpdateTimeSetFromTime()
        {
            if (_time == null)
            {
                TimeSet.Hour = 0;
                TimeSet.Minute = 0;
                return;
            }
            TimeSet.Hour = _time.Value.Hours;
            TimeSet.Minute = _time.Value.Minutes;
        }

        public bool MouseDown { get; set; }

        /// <summary>
        /// Sets Mouse Down bool to true if mouse is inside the clock mask.
        /// </summary>
        private void OnMouseDown(MouseEventArgs e)
        {
            MouseDown = true;
        }

        /// <summary>
        /// Sets Mouse Down bool to false if mouse is inside the clock mask.
        /// </summary>
        private void OnMouseUp(MouseEventArgs e)
        {
            MouseDown = false;
        }

        /// <summary>
        /// If MouseDown is true enabels "dragging" effect on the clock pin/stick.
        /// </summary>
        private void OnMouseOverHour(int value)
        {
            if (MouseDown)
            {
                TimeSet.Hour = value;
                UpdateTime();
            }
        }

        /// <summary>
        /// On click for the hour "sticks", sets the houre.
        /// </summary>
        private void OnMouseClickHour(int value)
        {
            var h = value;
            if (AmPm)
            {
                if (IsAm && value == 12)
                    h = 0;
                else if (IsPm && value < 12)
                    h = value + 12;
            }
            TimeSet.Hour = h;
            UpdateTime();
        }

        /// <summary>
        /// On click for the minutes "sticks", sets the minute.
        /// </summary>
        private void OnMouseOverMinute(int value)
        {
            if (MouseDown)
            {
                TimeSet.Minute = value;
                UpdateTime();
            }
        }

        /// <summary>
        /// On click for the minute "sticks", sets the minute.
        /// </summary>
        private void OnMouseClickMinute(int value)
        {
            TimeSet.Minute = value;
            UpdateTime();
        }

        private class SetTime
        {
            public int Hour { get; set; }

            public int Minute { get; set; }

        }
    }
}
