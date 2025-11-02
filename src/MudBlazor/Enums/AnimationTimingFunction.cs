using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the timing function used for animations and transitions.
/// </summary>
/// <remarks>
/// Based on Material Design motion guidelines: https://m2.material.io/design/motion/understanding-motion.html
/// </remarks>
public enum AnimationTimingFunction
{
    /// <summary>
    /// Linear animation with constant speed throughout.
    /// </summary>
    /// <remarks>
    /// Best for opacity-only fades (dialogs, scrims) where even perception of fading is desired.
    /// </remarks>
    [Description("linear")]
    Linear,

    /// <summary>
    /// Standard easing curve for most transitions (cubic-bezier(0.4, 0, 0.2, 1)).
    /// </summary>
    /// <remarks>
    /// Material Design's standard curve. Smooth, natural acceleration and deceleration.
    /// Use for most transitions where elements remain on screen.
    /// </remarks>
    [Description("cubic-bezier(0.4, 0, 0.2, 1)")]
    Ease,

    /// <summary>
    /// Deceleration curve for entering elements (cubic-bezier(0, 0, 0.2, 1)).
    /// </summary>
    /// <remarks>
    /// Starts fast and ends gently. Use when elements enter the screen.
    /// </remarks>
    [Description("cubic-bezier(0, 0, 0.2, 1)")]
    EaseIn,

    /// <summary>
    /// Acceleration curve for exiting elements (cubic-bezier(0.4, 0, 1, 1)).
    /// </summary>
    /// <remarks>
    /// Starts smoothly and ends quickly. Use when elements exit the screen.
    /// </remarks>
    [Description("cubic-bezier(0.4, 0, 1, 1)")]
    EaseOut
}
