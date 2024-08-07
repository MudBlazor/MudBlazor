using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable
public partial class MudText : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-typography")
            .AddClass($"mud-typography-{Typo.ToDescriptionString()}")
            .AddClass($"mud-{Color.ToDescriptionString()}-text", Color != Color.Default && Color != Color.Inherit)
            .AddClass("mud-typography-gutterbottom", GutterBottom)
            .AddClass($"mud-typography-align-{ConvertAlign(Align).ToDescriptionString()}", Align != Align.Inherit)
            .AddClass("d-inline", Inline)
            .AddClass(Class)
            .Build();

    [CascadingParameter(Name = "RightToLeft")]
    public bool RightToLeft { get; set; }

    /// <summary>
    /// Applies theme typography styles to the element.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The rendered HTML tag is determined by the theme unless <see cref="HtmlTag"/> is set.
    /// The tag affects the display type and the applicability of properties like <see cref="Align"/> and <see cref="GutterBottom"/>.
    /// </para>
    /// Defaults to <see cref="Typo.body1"/> which uses the block-level <c>p</c> element.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Text.Appearance)]
    public Typo Typo { get; set; } = Typo.body1;

    /// <summary>
    /// The <c>text-align</c> that will be used.
    /// </summary>
    /// <remarks>
    /// Has no effect on inline displays.
    /// Defaults to <see cref="Align.Inherit"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Text.Appearance)]
    public Align Align { get; set; } = Align.Inherit;

    /// <summary>
    /// The theme color of the component.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Inherit"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Text.Appearance)]
    public Color Color { get; set; } = Color.Inherit;

    /// <summary>
    /// Adds a bottom margin.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// Has no effect on inline displays.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Text.Appearance)]
    public bool GutterBottom { get; set; }

    /// <summary>
    /// Adds the <c>d-inline</c> display class, allowing text to continue on the same line rather than starting a new line.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>, meaning no display class will be added.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Text.Behavior)]
    public bool Inline { get; set; }

    /// <summary>
    /// The child content to display.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Text.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The HTML element that will be rendered (Example: <c>span</c>, <c>p</c>, <c>h1</c>).
    /// </summary>
    /// <remarks>
    /// <para>
    /// This can be used to
    /// <see href="https://developer.mozilla.org/docs/Web/HTML/Element#text_content">
    /// specify the type of content for accessibility and SEO more accurately
    /// </see>.
    /// </para>
    /// Defaults to <c>null</c>, meaning the tag be decided based on <see cref="Typo"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Text.Behavior)]
    public string? HtmlTag { get; set; }

    private string GetActualTag() => string.IsNullOrEmpty(HtmlTag) ? GetTagName(Typo) : HtmlTag;

    private static string GetTagName(Typo typo) => typo switch
    {
        Typo.h1 => "h1",
        Typo.h2 => "h2",
        Typo.h3 => "h3",
        Typo.h4 => "h4",
        Typo.h5 => "h5",
        Typo.h6 => "h6",
        Typo.subtitle1 => "p",
        Typo.subtitle2 => "p",
        Typo.body1 => "p",
        Typo.body2 => "p",
        _ => "span"
    };

    private Align ConvertAlign(Align align) => align switch
    {
        Align.Start => RightToLeft ? Align.Right : Align.Left,
        Align.End => RightToLeft ? Align.Left : Align.Right,
        _ => align
    };
}
