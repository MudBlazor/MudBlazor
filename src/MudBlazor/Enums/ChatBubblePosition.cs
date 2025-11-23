using System.ComponentModel;

namespace MudBlazor
{
    /// <summary>
    /// The position of chat bubbles.
    /// </summary>
    /// <remarks>
    /// This enum has moved to https://github.com/MudXtra/MudX and will be removed in v10
    /// </remarks>
    [Obsolete("ChatBubblePosition has moved to https://github.com/MudXtra/MudX and will be removed in v10")]
    public enum ChatBubblePosition
    {
        /// <summary>
        /// The component is aligned based on Right-to-Left (RTL) settings.
        /// </summary>
        /// <remarks>
        /// When Right-to-Left is enabled, the component is aligned to the right.  Otherwise, the left.
        /// </remarks>
        [Description("start")]
        Start,

        /// <summary>
        /// The component is aligned based on Right-to-Left (RTL) settings.
        /// </summary>
        /// <remarks>
        /// When Right-to-Left is enabled, the component is aligned to the left.  Otherwise, the right.
        /// </remarks>
        [Description("end")]
        End
    }
}
