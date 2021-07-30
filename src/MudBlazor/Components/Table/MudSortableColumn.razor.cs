using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    /// <summary>
    /// Binds an object's property to a column by its property name 
    /// </summary>
    public partial class MudSortableColumn<T, ModelType> : MudBaseColumn
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

        /// <summary>
        /// Specifies the name of the object's property bound to the footer
        /// </summary>
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

        /// <summary>
        /// Specifies if the column should be readonly even if the DataTable is in editmode.
        /// </summary>
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public string SortLabel { get; set; }

        [Parameter] public Func<ModelType, object> SortBy { get; set; } = default;

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
