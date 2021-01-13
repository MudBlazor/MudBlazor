using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace MudBlazor
{
    public class MudBooleanInput<T> : MudFormComponent<T, bool?>
    {
        public MudBooleanInput() : base(new BoolConverter<T>()) { }

        /// <summary>
        /// If true, the input will be read only.
        /// </summary>
        [Parameter] public bool ReadOnly { get; set; }

        /// <summary>
        /// Fired when Checked changes.
        /// </summary>
        [Parameter]
        public EventCallback<T> CheckedChanged { get; set; }

        /// <summary>
        /// The state of the component
        /// </summary>
        [Parameter]
        public T Checked
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    BeginValidateAfter(CheckedChanged.InvokeAsync(value));
                }
            }
        }

        protected bool? BoolValue
        {
            get => Converter.Set(_value);
            set => Checked = Converter.Get(value);
        }

        protected override bool SetConverter(Converter<T, bool?> value)
        {
            var changed = base.SetConverter(value);
            if (changed)
                BoolValue = Converter.Set(Checked);

            return changed;
        }

        /// <summary>
        /// A value is required, so if not checked we return ERROR.
        /// </summary>
        protected override bool HasValue(T value)
        {
            return (BoolValue == true);
        }
    }
}
