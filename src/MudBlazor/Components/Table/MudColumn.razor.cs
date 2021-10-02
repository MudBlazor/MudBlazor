using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    /// <summary>
    /// Binds an object's property to a column by its property name 
    /// </summary>
    public partial class MudColumn<T> : MudBaseColumn
    {
        T InternalValue
        {
            get => Value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(value, Value))
                {
                    Value = value;
                    ValueChanged.InvokeAsync(value);
                }
            }
        }
        /// <summary>
        /// Specifies the name of the object's property bound to the column
        /// </summary>
        [Parameter] public T Value { get; set; }
        [Parameter] public EventCallback<T> ValueChanged { get; set; }

        [Parameter]
        public T FooterValue
        {
            get { return _footerValue; }
            set { _footerValue = value; _footerValueAvailable = true; }
        }
        private T _footerValue;
        private bool _footerValueAvailable = false;

        /// <summary>
        /// Used if no FooterValue is available
        /// </summary>
        [Parameter] public string FooterText { get; set; }
        /// <summary>
        /// Specifies which string format should be used.
        /// </summary>
        [Parameter] public string DataFormatString { get; set; }

        [Parameter] public bool ReadOnly { get; set; }

        private string GetFormattedString(T item)
        {
            if (DataFormatString != null)
            {
                return string.Format(DataFormatString, item);
            }
            else
            {
                return item?.ToString();
            }
        }
    }
}
