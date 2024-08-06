using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a button consisting of an icon that can be toggled between two distinct states.
/// </summary>
/// <remarks>
/// Creates a <see href="https://developer.mozilla.org/docs/Web/HTML/Element/Button">button</see> element,
/// or <see href="https://developer.mozilla.org/docs/Web/HTML/Element/a">anchor</see> if <c>Href</c> is set.<br/>
/// You can directly add attributes like <c>title</c> or <c>aria-label</c>.
/// </remarks>
public partial class MudToggleIconButton : MudComponentBase
{
    /// <summary>
    /// Whether the icon is in the toggled state.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public bool Toggled { get; set; }

    /// <summary>
    /// Occurs when <see cref="Toggled"/> is changed.
    /// </summary>
    [Parameter]
    public EventCallback<bool> ToggledChanged { get; set; }

    /// <summary>
    /// The icon to use.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public string? Icon { get; set; }

    /// <summary>
    /// An alternative icon to use in the toggled state.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public string? ToggledIcon { get; set; }

    /// <summary>
    /// The color of the icon.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Default"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public Color Color { get; set; } = Color.Default;

    /// <summary>
    /// An alternative color to use in the toggled state.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public Color? ToggledColor { get; set; }

    /// <summary>
    /// The size of the button.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Size.Medium"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public Size Size { get; set; } = Size.Medium;

    /// <summary>
    /// An alternative size to use in the toggled state.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public Size? ToggledSize { get; set; }

    /// <summary>
    /// The variant to use in the regular state.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Variant.Text"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public Variant Variant { get; set; } = Variant.Text;

    /// <summary>
    /// An alternative variant to use in the toggled state.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public Variant? ToggledVariant { get; set; }

    /// <summary>
    /// Applies a negative margin.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Edge.False"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public Edge Edge { get; set; }

    /// <summary>
    /// Shows a ripple effect when the user clicks the button.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public bool Ripple { get; set; } = true;

    /// <summary>
    /// Displays a shadow.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public bool DropShadow { get; set; } = true;

    /// <summary>
    /// Disables interaction with the button.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Allows the click event to bubble up to the parent component.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public bool ClickPropagation { get; set; }

    /// <summary>
    /// Gets the icon to display based on the toggled state.
    /// </summary>
    /// <returns>
    /// <see cref="ToggledIcon"/> if toggled and that property is set; otherwise, <see cref="Icon"/>.
    /// </returns>
    internal string? GetIcon() => Toggled ? (ToggledIcon ?? Icon) : Icon;

    /// <summary>
    /// Gets the size to use based on the toggled state.
    /// </summary>
    /// <returns>
    /// <see cref="ToggledSize"/> if toggled and that property is set; otherwise, <see cref="Size"/>.
    /// </returns>
    internal Size GetSize() => Toggled ? (ToggledSize ?? Size) : Size;

    /// <summary>
    /// Gets the color to use based on the toggled state.
    /// </summary>
    /// <returns>
    /// <see cref="ToggledColor"/> if toggled and that property is set; otherwise, <see cref="Color"/>.
    /// </returns>
    internal Color GetColor() => Toggled ? (ToggledColor ?? Color) : Color;

    /// <summary>
    /// Gets the variant to use based on the toggled state.
    /// </summary>
    /// <returns>
    /// <see cref="ToggledVariant"/> if toggled and that property is set; otherwise, <see cref="Variant"/>.
    /// </returns>
    internal Variant GetVariant() => Toggled ? (ToggledVariant ?? Variant) : Variant;

    /// <summary>
    /// Toggles the state of the button.
    /// </summary>
    public Task Toggle() => SetToggledAsync(!Toggled);

    protected internal async Task SetToggledAsync(bool toggled)
    {
        if (Disabled)
            return;
        if (Toggled != toggled)
        {
            Toggled = toggled;
            await ToggledChanged.InvokeAsync(Toggled);
        }
    }
}
