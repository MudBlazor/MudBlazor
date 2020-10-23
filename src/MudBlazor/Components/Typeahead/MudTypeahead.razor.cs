using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudTypeahead<T> : MudComponentBase
    {
        private T _value;
        private string _textValue;

        protected string Classname =>
            new CssBuilder("mud-select")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// If true, the input element will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, the input will be read only.
        /// </summary>
        [Parameter] public bool ReadOnly { get; set; }

        /// <summary>
        /// If true, the label will be displayed in an error state.
        /// </summary>
        [Parameter] public bool Error { get; set; }

        /// <summary>
        /// If true, the input will take up the full width of its container.
        /// </summary>
        [Parameter] public bool FullWidth { get; set; }

        /// <summary>
        /// If true, the input will not have an underline.
        /// </summary>
        [Parameter] public bool DisableUnderLine { get; set; }

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        /// <summary>
        /// If string has value, helpertext will be applied.
        /// </summary>
        [Parameter] public string HelperText { get; set; }

        /// <summary>
        /// Icon that will be used if Adornment is set to Start or End.
        /// </summary>
        [Parameter] public string AdornmentIcon { get; set; }

        /// <summary>
        /// Text that will be used if Adornment is set to Start or End, the Text overrides Icon.
        /// </summary>
        [Parameter] public string AdornmentText { get; set; }

        /// <summary>
        /// Sets Start or End Adornment if not set to None.
        /// </summary>
        [Parameter] public Adornment Adornment { get; set; } = Adornment.None;

        /// <summary>
        /// Sets the Icon Size.
        /// </summary>
        [Parameter] public Size IconSize { get; set; } = Size.Small;

        /// <summary>
        /// Button click event if set and Adornment used.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter] public InputType InputType { get; set; } = InputType.Text;

        /// <summary>
        /// Variant to use.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        ///  Will adjust vertical spacing. 
        /// </summary>
        [Parameter] public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// If true, compact vertical padding will be applied to all select items.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// The Open Select Icon
        /// </summary>
        [Parameter] public string OpenIcon { get; set; } = Icons.Material.ArrowDropUp;

        /// <summary>
        /// The Open Select Icon
        /// </summary>
        [Parameter] public string CloseIcon { get; set; } = Icons.Material.ArrowDropDown;

        internal event Action<HashSet<T>> SelectionChangedFromOutside;

        /// <summary>
        /// Sets the maxheight the select can have when open.
        /// </summary>
        [Parameter] public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// Field which will be displayed in the list
        /// </summary>
        [Parameter]
        public Expression<Func<T, object>> DisplayField { get; set; } = (x) => x;

        /// <summary>
        /// Expression to get Data
        /// </summary>
        [Parameter]
        public Func<string, Task<IEnumerable<T>>> Data { get; set; }

        /// <summary>
        /// Set the format for values if no template
        /// </summary>
        [Parameter]
        public string Format { get; set; }

        /// <summary>
        /// Maximum items to display, defaults to 10
        /// </summary>
        [Parameter]
        public int MaxItems { get; set; } = 10;

        /// <summary>
        /// Minimum characters to initiate a search, defaults to 2
        /// </summary>
        [Parameter]
        public int MinCharacters { get; set; } = 0;

        /// <summary>
        /// Debounce interval in milliseconds.
        /// </summary>
        [Parameter] public int DebounceInterval { get; set; } = 100;

        /// <summary>
        /// Fired when the Value property changes. 
        /// </summary>
        [Parameter] public EventCallback<T> ValueChanged { get; set; }

        /// <summary>
        /// The value of this input element. This property is two-way bindable.
        /// </summary>
        [Parameter]
        public T Value
        {
            get => _value;
            set
            {
                if (!value.Equals(_value))
                {
                    _value = value;
                    ValueChanged.InvokeAsync(value);
                }
            }
        }

        internal bool isOpen { get; set; }

        public string CurrentIcon { get; set; }

        public void SelectOption(T value)
        {
            Value = value;
            _textValue = GetValue(value);
            isOpen = false;
            IconContoller();
            
            StateHasChanged();
        }

        public void ToggleMenu()
        {
            if (Disabled)
                return;
            isOpen = !isOpen;
            IconContoller();
            StateHasChanged();
        }

        public void IconContoller()
        {
                if (isOpen)
                {
                    CurrentIcon = OpenIcon;
                }
                else
                {
                    CurrentIcon = CloseIcon;
                }
        }

        protected override void OnInitialized()
        {
            IconContoller();
        }

        private Timer Timer;
        private IEnumerable<T> Items;

        private void OnInput(ChangeEventArgs args)
        {
            Timer?.Dispose();
            _textValue = GetValue((T)args.Value);
            var autoReset = new AutoResetEvent(false);
            Timer = new Timer(OnTimerComplete, autoReset, DebounceInterval, Timeout.Infinite);
        }

        private void OnTimerComplete(object stateInfo) => InvokeAsync(async () =>
        {
            if (!string.IsNullOrWhiteSpace(_textValue) && _textValue.Length >= MinCharacters)
            {
                isOpen = false;
                IconContoller();
                StateHasChanged();

                Items = (await Data(_textValue)).Take(MaxItems);

                if (Items?.Count() == 0)
                {
                    return;
                }

                isOpen = true;
                IconContoller();
                StateHasChanged();
            }
        });

        private Func<T, object> renderCompiled;

        private string GetValue(T item)
        {
            if (item == null) return string.Empty;

            if (renderCompiled == null)
                renderCompiled = DisplayField.Compile();

            object value = null;

            try
            {
                value = renderCompiled.Invoke(item);
            }
            catch (NullReferenceException) { }

            if (value == null) return string.Empty;

            if (string.IsNullOrEmpty(Format))
                return value.ToString();

            return string.Format(CultureInfo.CurrentCulture, $"{{0:{Format}}}", value);
        }
    }
}
