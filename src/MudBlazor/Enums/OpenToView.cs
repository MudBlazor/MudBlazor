
namespace MudBlazor;

/// <summary>
/// Indicates the initial view of a date or time picker component.
/// </summary>
public enum OpenTo
{
    /// <summary>
    /// No default view.
    /// </summary>
    None,

    /// <summary>
    /// The day picker is displayed.
    /// </summary>
    Date,

    /// <summary>
    /// The year picker is displayed.
    /// </summary>
    Year,

    /// <summary>
    /// The month picker is displayed.
    /// </summary>
    Month,

    /// <summary>
    /// The hours picker is displayed.
    /// </summary>
    Hours,

    /// <summary>
    /// The minutes picker is displayed.
    /// </summary>
    Minutes,
}
