using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System;

namespace MudBlazor
{
    public partial class MudDatePicker : MudBasePicker
    {
        /// <summary>
        /// Max selectable date.
        /// </summary>
        [Parameter] public DateTime MaxDate { get; set; }

        /// <summary>
        /// Max selectable date.
        /// </summary>
        [Parameter] public DateTime MinDate { get; set; }

        /// <summary>
        /// First view to show in the MudDatePicker.
        /// </summary>
        [Parameter] public OpenTo OpenTo { get; set; } = OpenTo.Date;

        /// <summary>
        /// Sets the Input Icon.
        /// </summary>
        [Parameter] public string InputIcon { get; set; } = Icons.Material.Event;



        protected string CurrentDayClass => new CssBuilder("mud-day").AddClass("mud-current").AddClass($"mud-color-text-{Color.ToDescriptionString()}").Build();
        protected string SelectedDayClass => new CssBuilder("mud-day").AddClass("mud-selected").AddClass($"mud-theme-color-{Color.ToDescriptionString()}").Build();

    }
}
