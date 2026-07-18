using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// Specifies the animation used when a carousel changes slides, such as none, fade, slide, or a custom transition.
    /// </summary>
    [EnumExtensions]
    public enum Transition
    {
        [Description("None")]
        None = 0,
        [Description("Fade")]
        Fade = 1,
        [Description("Slide")]
        Slide = 2,
        [Description("Custom")]
        Custom = 99
    }
}
