using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    /// <summary>
    /// Represents a base class for designing column components.
    /// </summary>
    public abstract class MudBaseColumn : ComponentBase
    {
        /// <summary>
        /// Indicates how a column is being rendered.
        /// </summary>
        public enum Rendermode
        {
            /// <summary>
            /// A column header is being rendered.
            /// </summary>
            Header,

            /// <summary>
            /// An item is being rendered.
            /// </summary>
            Item,

            /// <summary>
            /// An item is being rendered in edit mode.
            /// </summary>
            Edit,

            /// <summary>
            /// A column footer is being rendered.
            /// </summary>
            Footer
        }

        /// <summary>
        /// The way to render this column.
        /// </summary>
        [CascadingParameter(Name = "Mode")]
        public Rendermode Mode { get; set; }

        /// <summary>
        /// Whether the column is displayed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool Visible { get; set; } = true;

        /// <summary>
        /// The text to display for the column header.
        /// </summary>
        [Parameter]
        public string HeaderText { get; set; }

        protected bool IsDefault<T>(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }
    }
}
