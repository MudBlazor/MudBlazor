namespace MudBlazor;

#nullable enable

/// <summary>
/// Indicates the position of the edit button which starts inline edits for a <see cref="MudTable{T}"/>.
/// </summary>
public enum TableEditButtonPosition
{
    /// <summary>
    /// The edit button is at the start of the row.
    /// </summary>
    Start,

    /// <summary>
    /// The edit button is at the end of the row.
    /// </summary>
    End,

    /// <summary>
    /// The edit button is both at the start and end of the row.
    /// </summary>
    StartAndEnd,
}
