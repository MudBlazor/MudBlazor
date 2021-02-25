using System.ComponentModel;

namespace MudBlazor
{
    public enum Animation
    {
        [Description("false")]
        False,
        [Description("pulse")]
        Pulse,
        [Description("wave")]
        Wave
    }

    public enum AnimationDirection
    {
        [Description("normal")]
        normal,
        [Description("reverse")]
        reverse,
        [Description("alternate")]
        alternate,
        [Description("alternate-reverse")]
        alternate_reverse
    }

    public enum AnimationTimmingFunc
    {
        [Description("linear")]
        linear,
        [Description("ease")]
        ease,
        [Description("ease-in")]
        ease_in,
        [Description("ease-out")]
        ease_out,
        [Description("ease-in-out")]
        ease_in_out,
    }

    public enum AnimationFillMode
    {
        [Description("none")]
        none,
        [Description("forwards")]
        forwards,
        [Description("backwards")]
        backwards,
        [Description("both")]
        both

    }

    public enum AnimationTrigger
    {
        OnRender,
        Explicity
    }
}
