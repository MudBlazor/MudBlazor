

using System.ComponentModel;

namespace MudBlazor
{
    public enum TabHeaderPosition
    {
        /// <summary>
        /// Additional content is placed after the first tab
        /// </summary>
        [Description("after")]
        After,
        /// <summary>
        /// Additional content is placed before the first tab
        /// </summary>
        [Description("before")]
        Before,
        /// <summary>
        /// No additional content is rendered
        /// </summary>
        [Description("none")]
        None,
    }
}
