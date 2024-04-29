using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

public partial class MudPickerToolbar : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-picker-toolbar")
            .AddClass($"mud-theme-{Color.ToDescriptionString()}")
            .AddClass("mud-picker-toolbar-landscape",
                Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
            .AddClass(Class)
            .Build();

    /// <summary>
    /// If true, show toolbar
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Picker.Behavior)]
    public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Sets the orientation of the toolbar
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Picker.Appearance)]
    public Orientation Orientation { get; set; }

    /// <summary>
    /// Picker container option
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Picker.Appearance)]
    public PickerVariant PickerVariant { get; set; }

    /// <summary>
    /// The color of the toolbar, selected and active. It supports the theme colors
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Picker.Appearance)]
    public Color Color { get; set; }

    /// <summary>
    /// Child content of toolbar
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Picker.Behavior)]
    public RenderFragment ChildContent { get; set; }
}
