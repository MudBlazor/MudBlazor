// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Indicates the editable values of a <see cref="MudTimePicker"/>.
/// </summary>
public enum TimeEditMode
{
    /// <summary>
    /// Hours, minutes, and seconds can be edited.
    /// </summary>
    Normal,

    /// <summary>
    /// Only minutes can be edited.
    /// </summary>
    OnlyMinutes,

    /// <summary>
    /// Only hours can be edited.
    /// </summary>
    OnlyHours,

    /// <summary>
    /// Only seconds can be edited.
    /// </summary>
    OnlySeconds,

    /// <summary>
    /// Only hours and minutes can be edited.
    /// </summary>
    HoursMinutes,

    /// <summary>
    /// Only minutes and seconds can be edited.
    /// </summary>
    MinutesSeconds,
}
