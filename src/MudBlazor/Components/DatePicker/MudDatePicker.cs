using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class MudDatePicker<T> : MudBaseDatePicker<T>
        where T : struct
    {
        private T? _selectedDate;

        /// <summary>
        /// Fired when the DateFormat changes.
        /// </summary>
        [Parameter] public EventCallback<T?> DateChanged { get; set; }

        /// <summary>
        /// The currently selected date (two-way bindable). If null, then nothing was selected.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public T? Date
        {
            get => _value;
            set => SetDateAsync(value, true).AndForget();
        }

        private DateTime _lastSetTime = DateTime.MinValue;
        private const int DebounceTimeoutMs = 100;

        protected async Task SetDateAsync(T? date, bool updateValue)
        {
            if (_value != null && date != null)
            {
                date = DateWrapper.SetDateKind(date.Value, _value.Value);
            }

            var now = DateTime.UtcNow;

            /* See #7866 for more details
             * When the date is set in the UI, this method gets called with the same value multiple time. This guard
             * debounces the value to the same value in a short time frame is ignored
             */
            if (DateWrapper.AreEqual(_value, date) && (now - _lastSetTime).TotalMilliseconds < DebounceTimeoutMs)
            {
                return;
            }

            _lastSetTime = now;

            // When the _value is null and an invalid date is entered into the UI, the data value passed to this method
            // will be null. We need to check if the text has been set my the user and if so handle tha validation
            // without this the UI doesn't display a validation error correctly
            if (!DateWrapper.AreEqual(_value, date) || (date is null && Text != null))
            {
                Touched = true;

                if (date is not null && IsDateDisabledFunc(DateWrapper.GetDate(date.Value)))
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

        protected override Task DateFormatChangedAsync(string newFormat)
        {
            Touched = true;
            return SetTextAsync(Converter.Set(_value), false);
        }

        protected override Task StringValueChangedAsync(string value)
        {
            Touched = true;
            // Update the date property (without updating back the Value property)
            return SetDateAsync(Converter.Get(value), false);
        }

        protected override string GetDayClasses(int month, T day)
        {
            var b = new CssBuilder("mud-day");
            b.AddClass(AdditionalDateClassesFunc?.Invoke(day) ?? string.Empty);
            if (DateWrapper.LesserThan(day, GetMonthStart(month)) || DateWrapper.GreaterThan(day, GetMonthEnd(month)))
                return b.AddClass("mud-hidden").Build();
            if ((DateWrapper.DateEquals(Date, day) && _selectedDate == null) || DateWrapper.DateEquals(_selectedDate, day))
                return b.AddClass("mud-selected").AddClass($"mud-theme-{Color.ToDescriptionString()}").Build();
            if (DateWrapper.AreEqual(day, DateWrapper.Today))
                return b.AddClass("mud-current mud-button-outlined").AddClass($"mud-button-outlined-{Color.ToDescriptionString()} mud-{Color.ToDescriptionString()}-text").Build();
            return b.Build();
        }

        protected override async Task OnDayClickedAsync(T dateTime)
        {
            _selectedDate = dateTime;
            if (PickerActions == null || AutoClose || PickerVariant == PickerVariant.Static)
            {
                await Task.Run(() => InvokeAsync(SubmitAsync));

                if (PickerVariant != PickerVariant.Static)
                {
                    await Task.Delay(ClosingDelay);
                    await CloseAsync(false);
                }
            }
        }

        /// <summary>
        /// user clicked on a month
        /// </summary>
        /// <param name="month"></param>
        protected override async Task OnMonthSelectedAsync(T month)
        {
            PickerMonth = month;
            var nextView = GetNextView();
            if (nextView == null)
            {
                _selectedDate = DateWrapper.SetYearMonth(month, _selectedDate);
                await SubmitAndCloseAsync();
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
        protected override async Task OnYearClickedAsync(int year)
        {
            var current = GetMonthStart(0);
            
            PickerMonth = DateWrapper.SetYear(current, year);
            var nextView = GetNextView();
            if (nextView == null)
            {
                _selectedDate = DateWrapper.SetYear(_selectedDate, year);
                await SubmitAndCloseAsync();
            }
            else
            {
                CurrentView = (OpenTo)nextView;
            }
        }

        protected override Task OnOpenedAsync()
        {
            _selectedDate = null;

            return base.OnOpenedAsync();
        }

        protected internal override async Task SubmitAsync()
        {
            if (GetReadOnlyState())
                return;
            if (_selectedDate == null)
                return;

            _selectedDate = DateWrapper.GetFromFixedValues(_selectedDate.Value, FixYear, FixMonth, FixDay);

            await SetDateAsync(_selectedDate, true);
            _selectedDate = null;
        }

        public override async Task ClearAsync(bool close = true)
        {
            _selectedDate = null;
            await SetDateAsync(null, true);

            if (AutoClose)
            {
                await CloseAsync(false);
            }
        }

        protected override string GetTitleDateString()
        {
            return FormatTitleDate(_selectedDate ?? Date);
        }

        protected override T GetCalendarStartOfMonth()
        {
            var date = StartMonth ?? Date;
            return DateWrapper.StartOfMonth(date);
        }

        protected override int GetCalendarYear(T yearDate)
        {
            return DateWrapper.GetCalendarYear(Date, yearDate);
        }

        //To be completed on next PR
        protected internal override async Task OnHandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            await base.OnHandleKeyDownAsync(args);
            switch (args.Key)
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
                    else if (args.AltKey)
                    {
                        IsOpen = false;
                    }
                    else if (args.ShiftKey)
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
                    else if (args.ShiftKey)
                    {

                    }
                    else
                    {

                    }
                    break;
                case "Escape":
                    await ReturnDateBackUpAsync();
                    break;
                case "Enter":
                case "NumpadEnter":
                    if (!IsOpen)
                    {
                        await OpenAsync();
                    }
                    else
                    {
                        await SubmitAsync();
                        await CloseAsync();
                        _inputReference?.SetText(Text);
                    }
                    break;
                case " ":
                    if (!Editable)
                    {
                        if (!IsOpen)
                        {
                            await OpenAsync();
                        }
                        else
                        {
                            await SubmitAsync();
                            await CloseAsync();
                            _inputReference?.SetText(Text);
                        }
                    }
                    break;
            }

            StateHasChanged();
        }

        private Task ReturnDateBackUpAsync() => CloseAsync();

        /// <summary>
        /// Scrolls to the date.
        /// </summary>
        public void GoToDate()
        {
            if (Date.HasValue)
            {
                PickerMonth = DateWrapper.StartOfMonth(Date.Value);
                ScrollToYear();
            }
        }

        /// <summary>
        /// Scrolls to the defined date.
        /// </summary>
        public async Task GoToDate(T date, bool submitDate = true)
        {
            PickerMonth = DateWrapper.StartOfMonth(date);
            if (submitDate)
            {
                await SetDateAsync(date, true);
                ScrollToYear();
            }
        }
    }
}
