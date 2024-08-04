using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Typography controls the text throughout the theme, like font-family, size, and other settings.
/// </summary>
public enum Typo
{
    /// <summary>
    /// Inherits the typography style from the parent element.
    /// </summary>
    [Description("inherit")]
    inherit,

    /// <summary>
    /// Applies the h1 typography style, using the <c>h1</c> HTML tag.
    /// </summary>
    [Description("h1")]
    h1,

    /// <summary>
    /// Applies the h2 typography style, using the <c>h2</c> HTML tag.
    /// </summary>
    [Description("h2")]
    h2,

    /// <summary>
    /// Applies the h3 typography style, using the <c>h3</c> HTML tag.
    /// </summary>
    [Description("h3")]
    h3,

    /// <summary>
    /// Applies the h4 typography style, using the <c>h4</c> HTML tag.
    /// </summary>
    [Description("h4")]
    h4,

    /// <summary>
    /// Applies the h5 typography style, using the <c>h5</c> HTML tag.
    /// </summary>
    [Description("h5")]
    h5,

    /// <summary>
    /// Applies the h6 typography style, using the <c>h6</c> HTML tag.
    /// </summary>
    [Description("h6")]
    h6,

    /// <summary>
    /// Applies the subtitle1 typography style, using the <c>p</c> HTML tag.
    /// </summary>
    /// <remarks>The tag was changed from <c>h6</c> to <c>p</c> in v7.</remarks>
    [Description("subtitle1")]
    subtitle1,

    /// <summary>
    /// Applies the subtitle2 typography style, using the <c>p</c> HTML tag.
    /// </summary>
    /// <remarks>The tag was changed from <c>h6</c> to <c>p</c> in v7.</remarks>
    [Description("subtitle2")]
    subtitle2,

    /// <summary>
    /// Applies the body1 typography style, using the <c>p</c> HTML tag.
    /// </summary>
    [Description("body1")]
    body1,

    /// <summary>
    /// Applies the body2 typography style, using the <c>p</c> HTML tag.
    /// </summary>
    [Description("body2")]
    body2,

    /// <summary>
    /// Applies the input typography style, using the <c>span</c> HTML tag.
    /// </summary>
    [Description("input")]
    input,

    /// <summary>
    /// Applies the button typography style, using the <c>span</c> HTML tag.
    /// </summary>
    [Description("button")]
    button,

    /// <summary>
    /// Applies the caption typography style, using the <c>span</c> HTML tag.
    /// </summary>
    [Description("caption")]
    caption,

    /// <summary>
    /// Applies the overline typography style, using the <c>span</c> HTML tag.
    /// </summary>
    [Description("overline")]
    overline,
}
