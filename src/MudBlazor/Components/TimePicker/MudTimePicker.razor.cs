using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using static System.String;

namespace MudBlazor
{
    public partial class MudTimePicker : MudPicker<TimeSpan?>
    {
        public MudTimePicker() : base(new DefaultConverter<TimeSpan?>())
        {
            Converter.GetFunc = OnGet;
            Converter.SetFunc = OnSet;
        }

        private string OnSet(TimeSpan? time)
        {
            return AmPm ? time.ToAmPmString() : time.ToIsoString();
        }

        private TimeSpan? OnGet(string value)
        {
            if (IsNullOrWhiteSpace(value))
                return null;
            var pm = false;
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
                    time = new TimeSpan((time.Hours + 12) % 24, time.Minutes, 0);
                return time;
            }
            return null;
        }

        private OpenTo _currentView;

        internal TimeSpan? TimeIntermediate { get; private set; }

        /// <summary>
        /// First view to show in the MudDatePicker.
        /// </summary>
        [Parameter] public OpenTo OpenTo { get; set; } = OpenTo.Hours;

        /// <summary>
        /// Choose the edition mode. By default you can edit hours and minutes.
        /// </summary>
        [Parameter] public TimeEditMode TimeEditMode { get; set; } = TimeEditMode.Normal;

        /// <summary>
        /// If true, sets 12 hour selection clock.
        /// </summary>
        [Parameter] public bool AmPm { get; set; }

        /// <summary>
        /// The currently selected time (two-way bindable). If null, then nothing was selected.
        /// </summary>
        [Parameter]
        public TimeSpan? Time
        {
            get => _value;
            set => SetTimeAsync(value, true).AndForget();
        }

        protected async Task SetTimeAsync(TimeSpan? time, bool updateValue)
        {
            if (_value != time)
            {
                TimeIntermediate = time;
                _value = time;
                if (updateValue)
                    await SetTextAsync(Converter.Set(_value), false);
                UpdateTimeSetFromTime();
                await TimeChanged.InvokeAsync(_value);
                BeginValidate();
            }
        }

        /// <summary>
        /// Fired when the date changes.
        /// </summary>
        [Parameter] public EventCallback<TimeSpan?> TimeChanged { get; set; }

        protected override Task StringValueChanged(string value)
        {
            Touched = true;
            // Update the time property (without updating back the Value property)
            return SetTimeAsync(Converter.Get(value), false);
        }

        protected override void OnPickerOpened()
        {
            base.OnPickerOpened();
            _currentView = TimeEditMode switch
            {
                TimeEditMode.Normal => OpenTo,
                TimeEditMode.OnlyHours => OpenTo.Hours,
                TimeEditMode.OnlyMinutes => OpenTo.Minutes,
                _ => _currentView
            };
        }

        protected override void Submit()
        {
            Time = TimeIntermediate;
        }

        public override void Clear(bool close = true)
        {
            Time = null;
            TimeIntermediate = null;
            base.Clear();
        }

        private string GetHourString()
        {
            if (TimeIntermediate == null)
                return "--";
            var h = AmPm ? TimeIntermediate.Value.ToAmPmHour() : TimeIntermediate.Value.Hours;
            return Math.Min(23, Math.Max(0, h)).ToString(CultureInfo.InvariantCulture);
        }

        private string GetMinuteString()
        {
            if (TimeIntermediate == null)
                return "--";
            return $"{Math.Min(59, Math.Max(0, TimeIntermediate.Value.Minutes)):D2}";
        }

        private void UpdateTime()
        {
            TimeIntermediate = new TimeSpan(_timeSet.Hour, _timeSet.Minute, 0);
        }

        private void OnHourClick()
        {
            _currentView = OpenTo.Hours;
        }

        private void OnMinutesClick()
        {
            _currentView = OpenTo.Minutes;
        }

        private void OnAmClicked()
        {
            _timeSet.Hour %= 12;  // "12:-- am" is "00:--" in 24h
            UpdateTime();
        }

        private void OnPmClicked()
        {
            if (_timeSet.Hour <= 12) // "12:-- pm" is "12:--" in 24h
                _timeSet.Hour += 12;
            _timeSet.Hour %= 24;
            UpdateTime();
        }

        protected string ToolbarClass =>
        new CssBuilder("mud-picker-timepicker-toolbar")
          .AddClass($"mud-picker-timepicker-toolbar-landscape", Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
        .Build();

        protected string HoursButtonClass =>
        new CssBuilder("mud-timepicker-button")
          .AddClass($"mud-timepicker-toolbar-text", _currentView == OpenTo.Minutes)
        .Build();

        protected string MinuteButtonClass =>
        new CssBuilder("mud-timepicker-button")
          .AddClass($"mud-timepicker-toolbar-text", _currentView == OpenTo.Hours)
        .Build();

        protected string AmButtonClass =>
        new CssBuilder("mud-timepicker-button")
          .AddClass($"mud-timepicker-toolbar-text", !IsAm) // gray it out
        .Build();

        protected string PmButtonClass =>
        new CssBuilder("mud-timepicker-button")
          .AddClass($"mud-timepicker-toolbar-text", !IsPm) // gray it out
        .Build();

        private string HourDialClass =>
        new CssBuilder("mud-time-picker-hour")
          .AddClass($"mud-time-picker-dial")
          .AddClass($"mud-time-picker-dial-out", _currentView != OpenTo.Hours)
          .AddClass($"mud-time-picker-dial-hidden", _currentView != OpenTo.Hours)
        .Build();

        private string MinuteDialClass =>
        new CssBuilder("mud-time-picker-minute")
          .AddClass($"mud-time-picker-dial")
          .AddClass($"mud-time-picker-dial-out", _currentView != OpenTo.Minutes)
          .AddClass($"mud-time-picker-dial-hidden", _currentView != OpenTo.Minutes)
        .Build();

        private bool IsAm => _timeSet.Hour >= 00 && _timeSet.Hour < 12; // am is 00:00 to 11:59 
        private bool IsPm => _timeSet.Hour >= 12 && _timeSet.Hour < 24; // pm is 12:00 to 23:59 

        private string GetClockPinColor()
        {
            return $"mud-picker-time-clock-pin mud-{Color.ToDescriptionString()}";
        }

        private string GetClockPointerColor()
        {
            if (MouseDown)
                return $"mud-picker-time-clock-pointer mud-{Color.ToDescriptionString()}";
            else
                return $"mud-picker-time-clock-pointer mud-picker-time-clock-pointer-animation mud-{Color.ToDescriptionString()}";
        }

        private string GetClockPointerThumbColor()
        {
            var deg = GetDeg();
            if (deg % 30 == 0)
                return $"mud-picker-time-clock-pointer-thumb mud-onclock-text mud-onclock-primary mud-{Color.ToDescriptionString()}";
            else
                return $"mud-picker-time-clock-pointer-thumb mud-onclock-minute mud-{Color.ToDescriptionString()}-text";
        }

        private string GetNumberColor(int value)
        {
            if (_currentView == OpenTo.Hours)
            {
                var h = _timeSet.Hour;
                if (AmPm)
                {
                    h = _timeSet.Hour % 12;
                    if (_timeSet.Hour % 12 == 0)
                        h = 12;
                }
                if (h == value)
                    return $"mud-clock-number mud-theme-{Color.ToDescriptionString()}";
            }
            else if (_currentView == OpenTo.Minutes && _timeSet.Minute == value)
            {
                return $"mud-clock-number mud-theme-{Color.ToDescriptionString()}";
            }
            return $"mud-clock-number";
        }

        private double GetDeg()
        {
            double deg = 0;
            if (_currentView == OpenTo.Hours)
                deg = (_timeSet.Hour * 30) % 360;
            if (_currentView == OpenTo.Minutes)
                deg = (_timeSet.Minute * 6) % 360;
            return deg;
        }

        private string GetTransform(double angle, double radius, double offsetX, double offsetY)
        {
            angle = angle / 180 * Math.PI;
            var x = (Math.Sin(angle) * radius + offsetX).ToString("F3", CultureInfo.InvariantCulture);
            var y = ((Math.Cos(angle) + 1) * radius + offsetY).ToString("F3", CultureInfo.InvariantCulture);
            return $"transform: translate({x}px, {y}px);";
        }

        private string GetPointerRotation()
        {
            double deg = 0;
            if (_currentView == OpenTo.Hours)
                deg = (_timeSet.Hour * 30) % 360;
            if (_currentView == OpenTo.Minutes)
                deg = (_timeSet.Minute * 6) % 360;
            return $"rotateZ({deg}deg);";
        }

        private string GetPointerHeight()
        {
            var height = 40;
            if (_currentView == OpenTo.Minutes)
                height = 40;
            if (_currentView == OpenTo.Hours)
            {
                if (!AmPm && _timeSet.Hour > 0 && _timeSet.Hour < 13)
                    height = 26;
                else
                    height = 40;
            }
            return $"{height}%;";
        }

        private readonly SetTime _timeSet = new SetTime();
        private int _initialHour;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            UpdateTimeSetFromTime();
            _currentView = OpenTo;
            _initialHour = _timeSet.Hour;
        }


        private void UpdateTimeSetFromTime()
        {
            if (TimeIntermediate == null)
            {
                _timeSet.Hour = 0;
                _timeSet.Minute = 0;
                return;
            }
            _timeSet.Hour = TimeIntermediate.Value.Hours;
            _timeSet.Minute = TimeIntermediate.Value.Minutes;
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
            if (_currentView == OpenTo.Hours && _timeSet.Hour != _initialHour && TimeEditMode == TimeEditMode.Normal)
            {
                _currentView = OpenTo.Minutes;
            }
        }

        /// <summary>
        /// If MouseDown is true enabels "dragging" effect on the clock pin/stick.
        /// </summary>
        private void OnMouseOverHour(int value)
        {
            if (MouseDown)
            {
                _timeSet.Hour = value;
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
            _timeSet.Hour = h;
            UpdateTime();

            if (TimeEditMode == TimeEditMode.Normal)
            {
                _currentView = OpenTo.Minutes;
            }
        }

        /// <summary>
        /// On click for the minutes "sticks", sets the minute.
        /// </summary>
        private void OnMouseOverMinute(int value)
        {
            if (MouseDown)
            {
                _timeSet.Minute = value;
                UpdateTime();
            }
        }

        /// <summary>
        /// On click for the minute "sticks", sets the minute.
        /// </summary>
        private void OnMouseClickMinute(int value)
        {
            _timeSet.Minute = value;
            UpdateTime();
        }

        private class SetTime
        {
            public int Hour { get; set; }

            public int Minute { get; set; }

        }

    }
}
