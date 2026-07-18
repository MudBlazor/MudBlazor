using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// The type of animation used when content transitions.
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
