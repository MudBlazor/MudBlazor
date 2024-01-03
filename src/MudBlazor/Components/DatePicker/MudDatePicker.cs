using System;
using System.Threading.Tasks;
using System.Xml;
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
        [Category(CategoryTypes.FormComponent.Data)]
        public DateTime? Date
        {
            get => _value;
            set => SetDateAsync(value, true).AndForget();
        }
        
        private DateTime _lastSetTime = DateTime.MinValue;
        private const int DebounceTimeoutMs = 100;
        
        protected async Task SetDateAsync(DateTime? date, bool updateValue)
        {
            if (_value != null && date != null && date.Value.Kind == DateTimeKind.Unspecified)
            {
                date = DateTime.SpecifyKind(date.Value, _value.Value.Kind);
            }
 
            var now = DateTime.UtcNow;
            
            /* See #7866 for more details
             * When the date is set in the UI, this method gets called with the same value multiple time. This guard
             * debounces the value to the same value in a short time frame is ignored
             */
            if (_value == date && (now - _lastSetTime).TotalMilliseconds < DebounceTimeoutMs)
            {
                return;
            }

            _lastSetTime = now;
            
            // When the _value is null and an invalid date is entered into the UI, the data value passed to this method
            // will be null. We need to check if the text has been set my the user and if so handle tha validation
            // without this the UI doesn't display a validation error correctly
            if (_value != date || (date is null && Text != null))
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
                await BeginValidateAsync();
                FieldChanged(_value);
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
            b.AddClass(AdditionalDateClassesFunc?.Invoke(day) ?? string.Empty);
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
            if (PickerActions == null || AutoClose || PickerVariant == PickerVariant.Static)
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

        protected internal override async void Submit()
        {
            if (GetReadOnlyState())
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

        public override async void Clear(bool close = true)
        {
            _selectedDate = null;
            await SetDateAsync(null, true);

            if (AutoClose == true)
            {
                Close(false);
            }
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
            if (GetDisabledState() || GetReadOnlyState())
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

            StateHasChanged();
        }

        private void ReturnDateBackUp()
        {
            Close();
        }

        /// <summary>
        /// Scrolls to the date.
        /// </summary>
        public void GoToDate()
        {
            if (Date.HasValue)
            {
                PickerMonth = new DateTime(Date.Value.Year, Date.Value.Month, 1);
                ScrollToYear();
            }
        }

        /// <summary>
        /// Scrolls to the defined date.
        /// </summary>
        public async Task GoToDate(DateTime date, bool submitDate = true)
        {
            PickerMonth = new DateTime(date.Year, date.Month, 1);
            if (submitDate)
            {
                await SetDateAsync(date, true);
                ScrollToYear();
            }
        }
    }
}
