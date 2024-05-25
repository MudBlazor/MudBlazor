using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the display behavior of a <see cref="MudPicker{T}"/> component.
/// </summary>
public enum PickerVariant
{
    /// <summary>
    /// The picker displays when the input is clicked.
    /// </summary>
    [Description("inline")]
    Inline,

    /// <summary>
    /// A dialog is displayed to pick a value.
    /// </summary>
    [Description("dialog")]
    Dialog,

    /// <summary>
    /// The picker is always visible.
    /// </summary>
    [Description("static")]
    Static,
}
