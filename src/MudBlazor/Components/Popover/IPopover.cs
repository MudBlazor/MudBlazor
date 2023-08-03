// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a popover component.
/// </summary>
public interface IPopover
{
    /// <summary>
    /// Gets the unique identifier of the popover.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the CSS class of the popover.
    /// </summary>
    public string PopoverClass { get; }

    /// <summary>
    /// Gets the inline styles of the popover.
    /// </summary>
    public string PopoverStyles { get; }

    /// <summary>
    /// If true, the popover is visible.
    /// </summary>
    bool Open { get; set; }

    /// <summary>
    /// Use Tag to attach any user data object to the component for your convenience.
    /// </summary>
    object? Tag { get; set; }

    /// <summary>
    /// UserAttributes carries all attributes you add to the component that don't match any of its parameters.
    /// They will be splatted onto the underlying HTML tag.
    /// </summary>
    Dictionary<string, object?> UserAttributes { get; set; }

    /// <summary>
    /// Child content of the component.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
