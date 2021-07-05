using System.ComponentModel;


namespace MudBlazor
{
    public enum ArrowsPosition
    {
        /// <summary>
        /// Arrows are placed at the top of the carousel
        /// </summary>
        [Description("start")]
        Top,
        /// <summary>
        /// Arrows are placed at the center of the carousel. This is the default position
        /// </summary>
        [Description("center")]
        Center,
        /// <summary>
        /// Arrows are placed at the bottom of the carousel
        /// </summary>
        [Description("end")]
        Bottom
    }
}
