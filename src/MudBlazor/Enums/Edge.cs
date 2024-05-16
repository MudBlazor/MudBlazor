using System.ComponentModel;

namespace MudBlazor
{
    /// <summary>
    /// Indicates the location of any negative margin.
    /// </summary>
    public enum Edge
    {
        /// <summary>
        /// No negative margin is applied.
        /// </summary>
        [Description("false")]
        False,

        /// <summary>
        /// A negative margin is applied at the start.
        /// </summary>
        [Description("start")]
        Start,

        /// <summary>
        /// A negative margin is applied at the end.
        /// </summary>
        [Description("end")]
        End
    }
}
