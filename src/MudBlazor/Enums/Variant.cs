using System.ComponentModel;

namespace MudBlazor
{
    /// <summary>
    /// Indicates the display variation applied to a component.
    /// </summary>
    public enum Variant
    {
        /// <summary>
        /// The component has no drop shadow, background or border.
        /// </summary>
        [Description("text")]
        Text,

        /// <summary>
        /// The component interior is filled with a solid color.
        /// </summary>
        [Description("filled")]
        Filled,

        /// <summary>
        /// The component has an outline around the edge.
        /// </summary>
        [Description("outlined")]
        Outlined
    }
}
