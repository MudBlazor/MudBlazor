using System.ComponentModel;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Indicates a browser width used to trigger behaviors.
/// </summary>
/// <remarks>
/// Breakpoints are typically used to show or hide content based on the width of the browser window, such as customizing content for desktops, tablets, and mobile devices.
/// </remarks>
public enum Breakpoint
{
    /// <summary>
    /// A small to large phone.
    /// </summary>
    /// <remarks>
    /// <c>600</c> pixels wide or less.
    /// </remarks>
    [Description("xs")]
    Xs,

    /// <summary>
    /// A small to medium tablet.
    /// </summary>
    /// <remarks>
    /// Between <c>600</c> and <c>960</c> pixels wide.
    /// </remarks>
    [Description("sm")]
    Sm,

    /// <summary>
    /// A large tablet or laptop.
    /// </summary>
    /// <remarks>
    /// Between <c>960</c> and <c>1280</c> pixels wide.
    /// </remarks>
    [Description("md")]
    Md,

    /// <summary>
    /// A desktop computer.
    /// </summary>
    /// <remarks>
    /// Between <c>1280</c> and <c>1920</c> pixels wide.
    /// </remarks>
    [Description("lg")]
    Lg,

    /// <summary>
    /// A high-definition or 4K desktop computer monitor.
    /// </summary>
    /// <remarks>
    /// Between <c>1920</c> and <c>2560</c> pixels wide.
    /// </remarks>
    [Description("xl")]
    Xl,

    /// <summary>
    /// An ultra-wide of 4K+ desktop computer monitor.
    /// </summary>
    /// <remarks>
    /// <c>2560</c> or more pixels wide.
    /// </remarks>
    [Description("xxl")]
    Xxl,

    /// <summary>
    /// A small to medium tablet, or smaller device.
    /// </summary>
    /// <remarks>
    /// <c>960</c> pixels wide, or less.
    /// </remarks>
    [Description("smanddown")]
    SmAndDown,

    /// <summary>
    /// A large tablet, laptop, tablet, or smaller device.
    /// </summary>
    /// <remarks>
    /// <c>1280</c> pixels wide, or less.
    /// </remarks>
    [Description("mdanddown")]
    MdAndDown,

    /// <summary>
    /// A desktop computer or smaller device.
    /// </summary>
    /// <remarks>
    /// <c>1920</c> pixels wide, or less.
    /// </remarks>
    [Description("lganddown")]
    LgAndDown,

    /// <summary>
    /// A high-definition or 4K desktop computer monitor, or smaller device.
    /// </summary>
    /// <remarks>
    /// <c>2560</c> pixels wide, or less.
    /// </remarks>
    [Description("xlanddown")]
    XlAndDown,

    /// <summary>
    /// A small to medium-sized tablet, or larger device.
    /// </summary>
    /// <remarks>
    /// <c>600</c> pixels wide, or more.
    /// </remarks>
    [Description("smandup")]
    SmAndUp,

    /// <summary>
    /// A large tablet, laptop, or larger device.
    /// </summary>
    /// <remarks>
    /// <c>960</c> pixels wide, or more.
    /// </remarks>
    [Description("mdandup")]
    MdAndUp,

    /// <summary>
    /// A desktop computer, or larger device.
    /// </summary>
    /// <remarks>
    /// <c>1280</c> pixels wide, or more.
    /// </remarks>
    [Description("lgandup")]
    LgAndUp,

    /// <summary>
    /// A high-definition or 4K desktop computer monitor, or larger device.
    /// </summary>
    /// <remarks>
    /// <c>1920</c> pixels wide, or more.
    /// </remarks>
    [Description("xlandup")]
    XlAndUp,

    /// <summary>
    /// No breakpoint applies.
    /// </summary>
    [Description("none")]
    None,

    /// <summary>
    /// Content will always be visible.
    /// </summary>
    [Description("always")]
    Always
}
