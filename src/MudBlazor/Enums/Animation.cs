using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the type of animation used for a <see cref="MudSkeleton"/> component.
/// </summary>
public enum Animation
{
    /// <summary>
    /// No animation occurs.
    /// </summary>
    [Description("false")]
    False,

    /// <summary>
    /// The animation fades in and out in a pulsing loop.
    /// </summary>
    [Description("pulse")]
    Pulse,

    /// <summary>
    /// A left-to-right wave effect occurs.
    /// </summary>
    [Description("wave")]
    Wave
}
