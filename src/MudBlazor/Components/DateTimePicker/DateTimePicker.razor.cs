// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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

        private DateTime? _datePicked { get; set; }
        private TimeSpan? _timePicked { get; set; }

        protected void DateSelected(DateTime? date)
        {
            _datePicked = date;
            if (DateChanged.HasDelegate)
            {
                DateChanged.InvokeAsync(GetDateTime());
            }
            if (_datePicked != null && _timePicked != null && AutoClose)
            {
                Close();
            }
        }

        protected void TimeSelected(TimeSpan? time)
        {
            _timePicked = time;
            if (DateChanged.HasDelegate)
            {
                DateChanged.InvokeAsync(GetDateTime());
            }
            if (_datePicked != null && _timePicked != null && AutoClose)
            {
                Close();
            }
        }

        protected DateTime? GetDateTime()
            => _datePicked?.Add(_timePicked ?? TimeSpan.Zero) ?? DateTime.Now.Add(_timePicked ?? TimeSpan.Zero);

        protected void SetDateTime(DateTime? dateTime)
        {
            _datePicked = dateTime?.Date;
            _timePicked = dateTime?.TimeOfDay;
        }
    }
}
