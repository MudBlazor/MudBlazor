using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTimePicker : MudPicker<TimeSpan?>
    {
        private const string format24Hours = "HH:mm";
        private const string format12Hours = "hh:mm tt";

        public MudTimePicker() : base(new DefaultConverter<TimeSpan?>())
        {
            Converter.GetFunc = OnGet;
            Converter.SetFunc = OnSet;
            ((DefaultConverter<TimeSpan?>)Converter).Format = format24Hours;
            AdornmentIcon = Icons.Material.Filled.AccessTime;
            AdornmentAriaLabel = "Open Time Picker";
        }

        private string OnSet(TimeSpan? timespan)
        {
            if (timespan == null)
                return string.Empty;

            var time = DateTime.Today.Add(timespan.Value);

            return time.ToString(((DefaultConverter<TimeSpan?>)Converter).Format, Culture);
        }

        private TimeSpan? OnGet(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (DateTime.TryParseExact(value, ((DefaultConverter<TimeSpan?>)Converter).Format, Culture, DateTimeStyles.None, out var time))
            {
                return time.TimeOfDay;
            }
            else
            {
                var m = Regex.Match(value, "AM|PM", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    return DateTime.ParseExact(value, format12Hours, CultureInfo.InvariantCulture).TimeOfDay;
                }
                else
                {
                    return DateTime.ParseExact(value, format24Hours, CultureInfo.InvariantCulture).TimeOfDay;
                }
            }
        }

        private bool _amPm = false;
        private OpenTo _currentView;
        private string _timeFormat = string.Empty;

        internal TimeSpan? TimeIntermediate { get; private set; }

        /// <summary>
        /// First view to show in the MudDatePicker.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public OpenTo OpenTo { get; set; } = OpenTo.Hours;

        /// <summary>
        /// Choose the edition mode. By default, you can edit hours and minutes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public TimeEditMode TimeEditMode { get; set; } = TimeEditMode.Normal;

        /// <summary>
        /// Sets the amount of time in milliseconds to wait before closing the picker. This helps the user see that the time was selected before the popover disappears.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int ClosingDelay { get; set; } = 200;

        /// <summary>
        /// If AutoClose is set to true and PickerActions are defined, the hour and the minutes can be defined without any action.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool AutoClose { get; set; }

        /// <summary>
        /// If true, sets 12 hour selection clock.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool AmPm
        {
            get => _amPm;
            set
            {
                if (value == _amPm)
                    return;

                _amPm = value;

                if (Converter is DefaultConverter<TimeSpan?> defaultConverter && string.IsNullOrWhiteSpace(_timeFormat))
                {
                    defaultConverter.Format = AmPm ? format12Hours : format24Hours;
                }

                Touched = true;
                SetTextAsync(Converter.Set(_value), false).AndForget();
            }
        }

        /// <summary>
        /// String Format for selected time view
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string TimeFormat
        {
            get => _timeFormat;
            set
            {
                if (_timeFormat == value)
                    return;

                _timeFormat = value;
                if (Converter is DefaultConverter<TimeSpan?> defaultConverter)
                    defaultConverter.Format = _timeFormat;

                Touched = true;
                SetTextAsync(Converter.Set(_value), false).AndForget();
            }
        }

        /// <summary>
        /// The currently selected time (two-way bindable). If null, then nothing was selected.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public TimeSpan? Time
        {
            get => _value;
            set => SetTimeAsync(value, true).AndForget();
        }

        protected async Task SetTimeAsync(TimeSpan? time, bool updateValue)
        {
            if (_value != time)
            {
                Touched = true;
                TimeIntermediate = time;
                _value = time;
                if (updateValue)
                    await SetTextAsync(Converter.Set(_value), false);
                UpdateTimeSetFromTime();
                await TimeChanged.InvokeAsync(_value);
                await BeginValidateAsync();
                FieldChanged(_value);
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

        //The last line cannot be tested
        [ExcludeFromCodeCoverage]
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

        protected internal override void Submit()
        {
            if (GetReadOnlyState())
                return;
            Time = TimeIntermediate;
        }

        public override async void Clear(bool close = true)
        {
            TimeIntermediate = null;
            await SetTimeAsync(null, true);

            if (AutoClose == true)
            {
                Close(false);
            }
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
            if((PickerVariant == PickerVariant.Static && PickerActions == null) || (PickerActions != null && AutoClose))
            {
                Submit();
            }
        }

        private void OnHourClick()
        {
            _currentView = OpenTo.Hours;
            FocusAsync().AndForget();
        }

        private void OnMinutesClick()
        {
            _currentView = OpenTo.Minutes;
            FocusAsync().AndForget();
        }

        private void OnAmClicked()
        {
            _timeSet.Hour %= 12;  // "12:-- am" is "00:--" in 24h
            UpdateTime();
            FocusAsync().AndForget();
        }

        private void OnPmClicked()
        {
            if (_timeSet.Hour <= 12) // "12:-- pm" is "12:--" in 24h
                _timeSet.Hour += 12;
            _timeSet.Hour %= 24;
            UpdateTime();
            FocusAsync().AndForget();
        }

        protected string ToolbarClass =>
        new CssBuilder("mud-picker-timepicker-toolbar")
          .AddClass($"mud-picker-timepicker-toolbar-landscape", Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
          .AddClass(Class)
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

        private readonly SetTime _timeSet = new();
        private int _initialHour;
        private int _initialMinute;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            UpdateTimeSetFromTime();
            _currentView = OpenTo;
            _initialHour = _timeSet.Hour;
            _initialMinute = _timeSet.Minute;
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
            if (MouseDown && _currentView == OpenTo.Minutes && _timeSet.Minute != _initialMinute || _currentView == OpenTo.Hours && _timeSet.Hour != _initialHour && TimeEditMode == TimeEditMode.OnlyHours)
            {
                MouseDown = false;
                SubmitAndClose();
            }

            MouseDown = false;

            if (_currentView == OpenTo.Hours && _timeSet.Hour != _initialHour && TimeEditMode == TimeEditMode.Normal)
            {
                _currentView = OpenTo.Minutes;
            }
        }

        /// <summary>
        /// If MouseDown is true enables "dragging" effect on the clock pin/stick.
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
        /// On click for the hour "sticks", sets the hour.
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

            if(_currentView == OpenTo.Hours)
            {
                UpdateTime();
            }

            if (TimeEditMode == TimeEditMode.Normal)
            {
                _currentView = OpenTo.Minutes;
            }
            else if (TimeEditMode == TimeEditMode.OnlyHours)
            {
                SubmitAndClose();
            }
        }

        /// <summary>
        /// On mouse over for the minutes "sticks", sets the minute.
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
            SubmitAndClose();
        }

        protected async void SubmitAndClose()
        {
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

        protected internal override void HandleKeyDown(KeyboardEventArgs obj)
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            base.HandleKeyDown(obj);
            switch (obj.Key)
            {
                case "ArrowRight":
                    if (IsOpen)
                    {
                        if (obj.CtrlKey == true)
                        {
                            ChangeHour(1);
                        }
                        else if (obj.ShiftKey == true)
                        {
                            if (_timeSet.Minute > 55)
                            {
                                ChangeHour(1);
                            }
                            ChangeMinute(5);
                        }
                        else
                        {
                            if (_timeSet.Minute == 59)
                            {
                                ChangeHour(1);
                            }
                            ChangeMinute(1);
                        }
                    }
                    break;
                case "ArrowLeft":
                    if (IsOpen)
                    {
                        if (obj.CtrlKey == true)
                        {
                            ChangeHour(-1);
                        }
                        else if (obj.ShiftKey == true)
                        {
                            if (_timeSet.Minute < 5)
                            {
                                ChangeHour(-1);
                            }
                            ChangeMinute(-5);
                        }
                        else
                        {
                            if (_timeSet.Minute == 0)
                            {
                                ChangeHour(-1);
                            }
                            ChangeMinute(-1);
                        }
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
                        ChangeHour(5);
                    }
                    else
                    {
                        ChangeHour(1);
                    }
                    break;
                case "ArrowDown":
                    if (IsOpen == false && Editable == false)
                    {
                        IsOpen = true;
                    }
                    else if (obj.ShiftKey == true)
                    {
                        ChangeHour(-5);
                    }
                    else
                    {
                        ChangeHour(-1);
                    }
                    break;
                case "Escape":
                    ReturnTimeBackUp();
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

            StateHasChanged();
        }

        protected void ChangeMinute(int val)
        {
            _currentView = OpenTo.Minutes;
            _timeSet.Minute = (_timeSet.Minute + val + 60) % 60;
            UpdateTime();
        }

        protected void ChangeHour(int val)
        {
            _currentView = OpenTo.Hours;
            _timeSet.Hour = (_timeSet.Hour + val + 24) % 24;
            UpdateTime();
        }

        protected void ReturnTimeBackUp()
        {
            if (Time == null)
            {
                TimeIntermediate = null;
            }
            else
            {
                _timeSet.Hour = Time.Value.Hours;
                _timeSet.Minute = Time.Value.Minutes;
                UpdateTime();
            }
        }

        private class SetTime
        {
            public int Hour { get; set; }

            public int Minute { get; set; }

        }
    }
}
