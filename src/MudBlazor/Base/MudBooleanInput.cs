using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public class MudBooleanInput<T> : MudFormComponent<T>
    {
        /// <summary>
        /// Fired when Checked changes.
        /// </summary>
        [Parameter]
        public EventCallback<T> CheckedChanged { get; set; }

        private Converter<T, bool?> _boolConverter = new BoolConverter<T>();

        protected bool? BoolValue
        {
            get => _boolConverter.Set(_value);
            set => Checked = _boolConverter.Get(value);
        }

        /// <summary>
        /// The state of the component
        /// </summary>
        [Parameter]
        public T Checked
        {
            get => _value;
            set
            {
                if (object.Equals(value, _value))
                    return;
                _value = value;
                CheckedChanged.InvokeAsync(value);
                ValidateValue(value);
                EditFormValidate();
            }
        }

        [Parameter]
        public Converter<T, bool?> Converter
        {
            get => _boolConverter;
            set
            {
                _boolConverter = value;
                if (_boolConverter == null)
                    return;
                _boolConverter.OnError = OnConversionError;
                BoolValue = Converter.Set(Checked);
            }
        }


        protected override Task OnInitializedAsync()
        {
            if (_boolConverter != null)
                _boolConverter.OnError = OnConversionError;
            return base.OnInitializedAsync();
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
