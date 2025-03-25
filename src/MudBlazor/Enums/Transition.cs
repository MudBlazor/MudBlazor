using System.ComponentModel;

namespace MudBlazor
{
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
