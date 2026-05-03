using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor;

/// <summary>
/// Indicates the direction that search results are sorted by.
/// </summary>
[EnumExtensions]
public enum SortDirection
{
    /// <summary>
    /// No sort direction.
    /// </summary>
    [Description("none")]
    None,

    /// <summary>
    /// Results are sorted in ascending order (A-Z).
    /// </summary>
    [Description("ascending")]
    Ascending,

    /// <summary>
    /// Results are sorted in descending order (Z-A).
    /// </summary>
    [Description("descending")]
    Descending,
}
