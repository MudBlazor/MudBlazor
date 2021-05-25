

using System.ComponentModel;

namespace MudBlazor
{
    public enum TabHeaderPosition
    {
        /// <summary>
        /// Additional content is placed after the the first tab
        /// </summary>
        [Description("after")]
        After,
        /// <summary>
        /// Addtional content is placed before the first tab
        /// </summary>
        [Description("before")]
        Before,
        /// <summary>
        /// No additional contnet is rendered
        /// </summary>
        [Description("none")]
        None,
    }
}
