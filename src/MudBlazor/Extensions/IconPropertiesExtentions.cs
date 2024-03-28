#nullable enable
using System;
namespace MudBlazor.Extensions;

public static class IconPropertiesExtentions
{
    /// <summary>
    /// Returns <c>true</c> when <see cref="IconProperties.Icon"/> property has a value, <c>false</c> otherwise.
    /// </summary>
    public static bool HasIcon(this IconProperties props) => props.Icon.AsSpan().Trim().Length > 0;

    /// <summary>
    /// Returns <c>true</c> when <see cref="IconProperties.Title"/> property has a value, <c>false</c> otherwise.
    /// </summary>
    public static bool HasTitle(this IconProperties props) => props.Title.AsSpan().Trim().Length > 0;

    /// <summary>
    /// Returns <c>true</c> when <see cref="IconProperties.Class"/> property has a value, <c>false</c> otherwise.
    /// </summary>
    public static bool HasClass(this IconProperties props) => props.Class.AsSpan().Trim().Length > 0;

    /// <summary>
    /// Returns <c>true</c> when <see cref="IconProperties.Style"/> property has a value, <c>false</c> otherwise.
    /// </summary>
    public static bool HasStyle(this IconProperties props) => props.Style.AsSpan().Trim().Length > 0;

    /// <summary>
    /// Returns <c>true</c> when <see cref="IconProperties.Color"/> equals <see cref="Color.Default"/>, <c>false</c> otherwise.
    /// </summary>
    public static bool HasDefaultColor(this IconProperties props) => props.Color == Color.Default;

    /// <summary>
    /// Returns <c>true</c> when <see cref="IconProperties.Color"/> does not equal <see cref="Color.Default"/> and <see cref="Color.Inherit"/>, <c>false</c> otherwise.
    /// </summary>
    public static bool HasCustomColor(this IconProperties props) => !props.HasDefaultColor() && props.Color != Color.Inherit;

    /// <summary>
    /// Returns <c>true</c> when <see cref="IconProperties.Icon"/> is an SVG icon, <c>false</c> otherwise.
    /// </summary>
    public static bool IsSvg(this IconProperties props) => props.Icon.AsSpan().Trim().StartsWith("<", StringComparison.Ordinal);

    /// <summary>
    /// Returns <c>true</c> when <see cref="IconProperties.Icon"/> property has a value and <see cref="IconProperties.Position"/> equals <see cref="Position.Start"/>, <see cref="Position.Left"/> or <see cref="Position.Top"/> <c>false</c> otherwise.
    /// </summary>
    public static bool StartIcon(this IconProperties props) => props.HasIcon() && (props.Position == Position.Start || props.Position == Position.Left || props.Position == Position.Top);

    /// <summary>
    /// Returns <c>true</c> when <see cref="IconProperties.Icon"/> property has a value and  <see cref="IconProperties.Position"/> equals <see cref="Position.End"/>, <see cref="Position.Right"/> or <see cref="Position.Bottom"/>, <c>false</c> otherwise.
    /// </summary>
    public static bool EndIcon(this IconProperties props) => props.HasIcon() && (props.Position == Position.End || props.Position == Position.Right || props.Position == Position.Bottom);
}
