using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the timing function used for animations and transitions.
/// </summary>
public enum AnimationTimingFunction
{
    /// <summary>
    /// Linear animation with constant speed throughout.
    /// </summary>
    [Description("linear")]
    Linear,

    /// <summary>
    /// Eased animation using Material Design's standard easing curve (cubic-bezier(0.4, 0, 0.2, 1)).
    /// </summary>
    [Description("cubic-bezier(0.4, 0, 0.2, 1)")]
    Ease
}
