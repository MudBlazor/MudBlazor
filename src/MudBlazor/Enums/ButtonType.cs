using System.ComponentModel;

namespace MudBlazor
{
    /// <summary>
    /// Indicates the behavior performed when a button is clicked.
    /// </summary>
    public enum ButtonType
    {
        /// <summary>
        /// A regular click occurs.
        /// </summary>
        [Description("button")]
        Button,

        /// <summary>
        /// The button will submit a form.
        /// </summary>
        [Description("submit")]
        Submit,

        /// <summary>
        /// The button resets a form.
        /// </summary>
        [Description("reset")]
        Reset
    }
}
