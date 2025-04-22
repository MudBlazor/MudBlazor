using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Specifies where a negative margin is applied within a layout component.
/// This is typically used in conjunction with elements that have leading or trailing adornments.
/// </summary>
/// <remarks>
/// Negative margins are often applied to ensure adornments or icons align more closely with the edge of the component.
/// Depending on the design requirements, this margin can be applied at the start or end of an element,
/// or not applied at all (default behavior).
/// </remarks>
public enum Edge
{
    /// <summary>
    /// No negative margin is applied to the element.
    /// </summary>
    /// <remarks>
    /// This is the default behavior, meaning the element maintains its normal margins without adjustments.
    /// It is used when there is no need to modify the element’s margin for adornments or icons.
    /// </remarks>
    [Description("false")]
    False,

    /// <summary>
    /// A negative margin is applied to the start of the element.
    /// </summary>
    /// <remarks>
    /// This reduces the space at the start (leading side) of the element. It is typically used when an adornment, 
    /// such as an icon or label, is positioned at the beginning of the component and needs to be aligned more closely 
    /// with the left edge in left-to-right layouts (or the right edge in right-to-left layouts).
    /// </remarks>
    [Description("start")]
    Start,

    /// <summary>
    /// A negative margin is applied to the end of the element.
    /// </summary>
    /// <remarks>
    /// This reduces the space at the end (trailing side) of the element. It is commonly used when an adornment, 
    /// such as an icon or label, is positioned at the end of the component and requires alignment with the right 
    /// edge in left-to-right layouts (or the left edge in right-to-left layouts).
    /// </remarks>
    [Description("end")]
    End,
}
