using System.ComponentModel;

namespace MudBlazor
{
    /// <summary>
    /// Indicates how items in a <see cref="MudStack"/> are wrapped.
    /// </summary>
    public enum Wrap
    {
        /// <summary>
        /// No wrapping occurs.
        /// </summary>
        /// <remarks>
        /// Items may overflow the container.  
        /// </remarks>
        [Description("nowrap")]
        NoWrap,

        /// <summary>
        /// Items are wrapped to fit the container.
        /// </summary>
        /// <remarks>
        /// When <see cref="MudStack.Row"/> is <c>true</c>, items are wrapped to fit into the width of the container.<br />
        /// When <c>false</c>, items are wrapped to fit into the height of the container.
        /// </remarks>
        [Description("wrap")]
        Wrap,

        /// <summary>
        /// Behaves the same as wrap but cross-start and cross-end are permuted.
        /// </summary>
        [Description("wrap-reverse")]
        WrapReverse
    }
}
