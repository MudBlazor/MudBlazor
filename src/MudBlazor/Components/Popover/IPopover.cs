// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a popover component.
/// </summary>
public interface IPopover
{
    /// <summary>
    /// The unique identifier of the popover.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// The CSS class of the popover.
    /// </summary>
    public string PopoverClass { get; }

    /// <summary>
    /// The inline styles of the popover.
    /// </summary>
    public string PopoverStyles { get; }

    /// <summary>
    /// Shows the popover.
    /// </summary>
    bool Open { get; set; }

    /// <summary>
    /// Any user data to link to this popover.
    /// </summary>
    object? Tag { get; set; }

    /// <summary>
    /// Any additional attributes to add to this component.
    /// </summary>
    /// <remarks>
    /// Use this for any attributes which don't have a parameter.  They will be "splatted" onto the underlying HTML tag.
    /// </remarks>
    Dictionary<string, object?> UserAttributes { get; set; }

    /// <summary>
    /// The content within this popover.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
