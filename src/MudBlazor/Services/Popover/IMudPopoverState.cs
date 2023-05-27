// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
public interface IMudPopoverState
{
    /// <summary>
    /// Gets the unique identifier of the popover.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets a value indicating whether the popover is connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets a value indicating whether the popover is detached.
    /// </summary>
    bool IsDetached { get; }

    /// <summary>
    /// Gets the CSS class of the popover.
    /// </summary>
    string? Class { get; }

    /// <summary>
    /// Gets the inline styles of the popover.
    /// </summary>
    string? Style { get; }

    /// <summary>
    /// Use Tag to attach any user data object to the component for your convenience.
    /// </summary>
    object? Tag { get; }

    /// <summary>
    /// Gets a value indicating whether the popover's content is visible.
    /// </summary>
    bool ShowContent { get; }

    /// <summary>
    /// Gets the activation date of the popover.
    /// </summary>
    DateTime? ActivationDate { get; }

    /// <summary>
    /// UserAttributes carries all attributes you add to the component that don't match any of its parameters.
    /// They will be splatted onto the underlying HTML tag.
    /// </summary>
    Dictionary<string, object> UserAttributes { get; }

    /// <summary>
    /// Gets or sets the element reference for the <see cref="MudRender"/>.
    /// It's used to re-render the component individually.
    /// </summary>
    MudRender? ElementReference { get; set; }

    /// <summary>
    /// Content of popover.
    /// </summary>
    RenderFragment? Fragment { get; }
}
