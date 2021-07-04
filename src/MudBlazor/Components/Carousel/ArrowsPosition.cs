using System.ComponentModel;


namespace MudBlazor
{
    public enum ArrowsPosition
    {
        /// <summary>
        /// Arrows are placed at the start of the carousel
        /// </summary>
        [Description("start")]
        Start,
        /// <summary>
        /// Arrows are placed at the center of the carousel. This is the default position
        /// </summary>
        [Description("center")]
        Center,
        /// <summary>
        /// Arrows are placed at the end of the carousel
        /// </summary>
        [Description("end")]
        End
    }
}
