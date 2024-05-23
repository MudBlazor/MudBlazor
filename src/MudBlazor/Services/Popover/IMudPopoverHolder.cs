// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
/// <summary>
/// This interface serves as a container for the values of a <see cref="IPopover"/> and is used by the <see cref="MudPopoverProvider"/> to render the popover.
/// </summary>
public interface IMudPopoverHolder
{
    /// <summary>
    /// Gets the unique identifier of the popover.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets a value indicating whether the popover is connected.
    /// </summary>
    /// <remarks>
    /// This property is used to determine the connection state of the popover on the JavaScript side. It indicates whether
    /// the popover is connected and actively rendered in the DOM.
    /// </remarks>
    bool IsConnected { get; }

    /// <summary>
    /// Gets a value indicating whether the popover is detached.
    /// </summary>
    /// <remarks>
    /// This property is used to determine the detachment state of the popover on the JavaScript side. It indicates whether
    /// the popover is detached from its parent component, allowing it to be rendered outside the normal component hierarchy.
    /// </remarks>
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
    Dictionary<string, object?> UserAttributes { get; }

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
