using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    /// <summary>
    /// Binds an object's property to a column by its property name 
    /// </summary>
    public partial class MudColumn<T> : MudBaseColumn
    {
        /// <summary>
        /// Specifies the name of the object's property bound to the column
        /// </summary>
        [Parameter]
        public T Value { get; set; }

        [Parameter]
        public T FooterValue { get; set; }

        /// <summary>
        /// Specifies which string format should be used.
        /// </summary>
        [Parameter]
        public string DataFormatString { get; set; }

        [Parameter]
        public bool ReadOnly { get; set; }

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
