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
    /// Applies the theme typography styles.
    /// <para>
    /// This will determine the HTML tag rendered unless <see cref="HtmlTag"/> is specified.
    /// <br/>
    /// The tag influences the display type and whether certain other properties (like <see cref="Align"/> and <see cref="GutterBottom"/>) take effect.
    /// </para>
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Text.Appearance)]
    public Typo Typo { get; set; } = Typo.body1;

    /// <summary>
    /// The text-align on the component.
    /// <br/>
    /// Won't have any effect on inline displays.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Text.Appearance)]
    public Align Align { get; set; } = Align.Inherit;

    /// <summary>
    /// The theme color of the component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Text.Appearance)]
    public Color Color { get; set; } = Color.Inherit;

    /// <summary>
    /// Adds a bottom margin.
    /// <br/>
    /// Won't have any effect on inline displays.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Text.Appearance)]
    public bool GutterBottom { get; set; }

    /// <summary>
    /// If true, text will continue after previous text instead of placing in a new line.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Text.Appearance)]
    public bool Inline { get; set; }

    /// <summary>
    /// The child content to display.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Text.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The HTML element that will be rendered (<c>span</c>, <c>p</c>, <c>h1</c>).
    /// <br/>
    /// If <c>null</c> it will be automatically decided based on the <see cref="Typo"/>.
    /// </summary>
    /// <remarks>
    /// This can be used to specify the type of content for accessibility and SEO more accurately.
    /// <br/>
    /// https://developer.mozilla.org/en-US/docs/Web/HTML/Element#text_content
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Text.Behavior)]
    public string? HtmlTag { get; set; }

    private string GetActualTag() => HtmlTag ?? GetTagName(Typo);

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
