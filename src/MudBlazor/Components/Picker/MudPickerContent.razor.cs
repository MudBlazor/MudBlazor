using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// Represents the content within a <see cref="MudPicker{T}"/>.
/// </summary>
/// <seealso cref="MudPicker{T}" />
/// <seealso cref="MudPickerToolbar" />
public partial class MudPickerContent : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-picker-content")
            .AddClass(Class)
            .Build();

    /// <summary>
    /// The content to display.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Picker.Behavior)]
    public RenderFragment ChildContent { get; set; }
}
