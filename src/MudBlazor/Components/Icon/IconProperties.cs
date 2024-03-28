#nullable enable
namespace MudBlazor;

/// <summary>
/// The properties for an SVG- or font icon.
/// </summary>
public class IconProperties
{
    /// <summary>
    /// The default viewbox dimensions for an SVG icon.
    /// </summary>
    /// <remarks>The default is "0 0 24 24"</remarks>
    public const string DefaultViewBox = "0 0 24 24";


    /// <summary>
    /// The icon to use. This can either be an SVG- or font icon.
    /// </summary>
    [Category(CategoryTypes.Icon.Behavior)]
    public string? Icon { get; set; } = string.Empty;

    /// <summary>
    /// The title of the icon, used for accessibility.
    /// </summary>
    [Category(CategoryTypes.Icon.Behavior)]
    public string? Title { get; set; } = string.Empty;

    /// <summary>
    /// The size of the icon.
    /// </summary>
    /// <remarks>The default is <see cref=" Size.Medium"/></remarks>
    [Category(CategoryTypes.Icon.Appearance)]
    public Size Size { get; set; } = Size.Medium;

    /// <summary>
    /// The color of the icon.
    /// </summary>
    /// <remarks>The default is <see cref="Color.Inherit"/></remarks>
    [Category(CategoryTypes.Icon.Appearance)]
    public Color Color { get; set; } = Color.Inherit;

    /// <summary>
    /// The class names to apply to the icon, separated by space.
    /// </summary>
    [Category(CategoryTypes.Button.Appearance)]
    public string? Class { get; set; }

    /// <summary>
    /// The CSS  style to apply to the icon.
    /// </summary>
    [Category(CategoryTypes.ComponentBase.Common)]
    public string? Style { get; set; }

    /// <summary>
    /// The position of the icon.
    /// </summary>
    public Position? Position { get; set; }


    // SVG properties

    /// <summary>
    /// The viewbox dimensions for an SVG icon.
    /// </summary>
    /// <remarks>The default is "0 0 24 24"</remarks>
    [Category(CategoryTypes.Icon.Behavior)]
    public string ViewBox { get; set; } = DefaultViewBox;

    /// <summary>
    /// The focusable attribute for an SVG icon.
    /// </summary>
    public bool Focusable { get; set; }

    /// <summary>
    /// Theattribute to indicates whether the element is exposed to an accessibility API.
    /// </summary>
    /// <remarks>The default is <c>true</c></remarks>
    public bool AriaHidden { get; set; } = true;
}
