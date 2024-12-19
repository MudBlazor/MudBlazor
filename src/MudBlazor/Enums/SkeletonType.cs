using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the shape of a <see cref="MudSkeleton"/> component.
/// </summary>
public enum SkeletonType
{
    /// <summary>
    /// The skeleton is a placeholder for text.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="MudSkeleton.Width"/> and <see cref="MudSkeleton.Height"/> parameters to control its size.
    /// </remarks>
    [Description("text")]
    Text,

    /// <summary>
    /// The skeleton displays a circle shape.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="MudSkeleton.Width"/> and <see cref="MudSkeleton.Height"/> parameters to control its size.
    /// </remarks>
    [Description("circle")]
    Circle,

    /// <summary>
    /// The skeleton displays a rectangle shape.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="MudSkeleton.Width"/> and <see cref="MudSkeleton.Height"/> parameters to control its size.
    /// </remarks>
    [Description("rectangle")]
    Rectangle
}
