using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public abstract class MudBasePicker : MudComponentBase
    {
        private string _value;

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow set to 8 by default in inline mode and 0 in static mode.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 8;

        /// <summary>
        /// If true, border-radius is set to 0 this is set to true automaticly in static mode but can be overridden with Rounded bool.
        /// </summary>
        [Parameter] public bool Square { get; set; }

        /// <summary>
        /// If true, border-radius is set to theme default when in Static Mode.
        /// </summary>
        [Parameter] public bool Rounded { get; set; }

        /// <summary>
        /// If string has value, helpertext will be applied.
        /// </summary>
        [Parameter] public string HelperText { get; set; }

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// If true, the picker will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, the picker will be editable.
        /// </summary>
        [Parameter] public bool Editable { get; set; } = false;

        /// <summary>
        /// Hide toolbar and show only date/time views.
        /// </summary>
        [Parameter] public bool DisableToolbar { get; set; }

        /// <summary>
        /// User class names for picker's ToolBar, separated by space
        /// </summary>
        [Parameter] public string ToolBarClass { get; set; }

        /// <summary>
        /// Picker container option
        /// </summary>
        [Parameter] public PickerVariant PickerVariant { get; set; } = PickerVariant.Inline;

        /// <summary>
        /// InputVariant, if Picker is static this option will not change anything.
        /// </summary>
        [Parameter] public Variant InputVariant { get; set; } = Variant.Text;

        /// <summary>
        /// Sets if the icon will be att start or end, set to false to disable.
        /// </summary>
        [Parameter] public Adornment Adornment { get; set; } = Adornment.End;

        /// <summary>
        /// What orientation to render in when in PickerVariant Static Mode.
        /// </summary>
        [Parameter] public Orientation Orientation { get; set; } = Orientation.Portrait;

        /// <summary>
        /// Sets the Icon Size.
        /// </summary>
        [Parameter] public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the toolbar, selected and active. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Allows text input from keyboard.
        /// </summary>
        [Parameter] public bool AllowKeyboardInput { get; set; }

        /// <summary>
        /// Fired when the value changes.
        /// </summary>
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        /// <summary>
        /// The currently selected value represented as a string (two-way bindable)
        /// </summary>
        [Parameter]
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    StringValueChanged(_value);
                    ValueChanged.InvokeAsync(_value).FireAndForget();
                }
            }
        }

        /// <summary>
        /// Value change hook for descendants.
        /// </summary>
        protected virtual void StringValueChanged(string value)
        {
        }

        protected bool IsOpen { get; set; }

        public virtual void ToggleOpen()
        {
            IsOpen = !IsOpen;
        }

        public void Close()
        {
            IsOpen = false;
            StateHasChanged();
        }

        public virtual void Open()
        {
            IsOpen = true;
            StateHasChanged();
        }
    }
}
