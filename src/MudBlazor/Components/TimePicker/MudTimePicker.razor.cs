using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;

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

        protected string ToolbarClass =>
        new CssBuilder()
          .AddClass($"mud-picker-timepicker-toolbar")
          .AddClass($"mud-picker-timepicker-toolbar-landscape", Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
        .Build();

        private string GetClockPinColor()
        {
           return $"mud-picker-time-clock-pin mud-color-{Color.ToDescriptionString()}";
        }
        private string GetClockPointerColor()
        {
            return $"mud-picker-time-clock-pointer mud-picker-time-clock-pointer-animation mud-color-{Color.ToDescriptionString()}";
        }

        private string GetClockPointerThumbColor()
        {
            return $"mud-picker-time-clock-pointer-thumb mud-color-{Color.ToDescriptionString()}";
        }

        private string GetPointerRotation()
        {
            return $"rotateZ({TimeSet.Degrees.ToString()}deg);";
        }

        private string GetPointerHeight()
        {
            return $"{TimeSet.Height.ToString()}%;";
        }

        private SetTime TimeSet = new SetTime();

        protected override void OnInitialized()
        {
            if (AmPm)
            {
                TimeSet.Hour = 1;
                TimeSet.Minute = 37;
                TimeSet.Degrees = 30;
                TimeSet.Height = 40;
            }
            else
            {
                TimeSet.Hour = 13;
                TimeSet.Minute = 37;
                TimeSet.Degrees = 30;
                TimeSet.Height = 40;
            }
        }

        public bool MouseDown { get; set; }

        private void OnMouseDown(MouseEventArgs e)
        {
            MouseDown = true;
        }

        private void OnMouseUp(MouseEventArgs e)
        {
            MouseDown = false;
        }

        private void OnMouseOverHour(SetTime setTime)
        {
            if(MouseDown)
            {
                TimeSet.Degrees = setTime.Degrees;
                TimeSet.Height = setTime.Height;
                TimeSet.Hour = setTime.Hour;
            }
        }

        private void OnMouseOverMinute(SetTime setTime)
        {
            if(MouseDown)
            {
                TimeSet.Degrees = setTime.Degrees;
                TimeSet.Height = 40;
                TimeSet.Minute = setTime.Minute;
            }
        }

        private class SetTime
        {
            public int Degrees { get; set; }

            public int Height { get; set; }

            public int Hour { get; set; }

            public int Minute { get; set; }
        }
    }
}
