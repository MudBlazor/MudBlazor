// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
internal class MudPopoverHolder : IMudPopoverHolder
{
    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public RenderFragment? Fragment { get; internal set; }

    /// <inheritdoc />
    public bool IsConnected { get; internal set; }

    /// <inheritdoc />
    public bool IsDetached { get; internal set; }

    /// <inheritdoc />
    public string? Class { get; private set; }

    /// <inheritdoc />
    public string? Style { get; private set; }

    /// <inheritdoc />
    public object? Tag { get; private set; }

    /// <inheritdoc />
    public bool ShowContent { get; private set; }

    /// <inheritdoc />
    public DateTime? ActivationDate { get; private set; }

    /// <inheritdoc />
    public Dictionary<string, object?> UserAttributes { get; set; } = new();

    /// <inheritdoc />
    public MudRender? ElementReference { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MudPopoverHolder"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the popover.</param>
    public MudPopoverHolder(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Sets the CSS class of the popover.
    /// </summary>
    /// <param name="class">The CSS class of the popover.</param>
    /// <returns>The updated <see cref="MudPopoverHolder"/> instance.</returns>
    public MudPopoverHolder SetClass(string @class)
    {
        Class = @class;

        return this;
    }

    /// <summary>
    /// Sets the inline styles of the popover.
    /// </summary>
    /// <param name="style">The inline styles of the popover.</param>
    /// <returns>The updated <see cref="MudPopoverHolder"/> instance.</returns>
    public MudPopoverHolder SetStyle(string style)
    {
        Style = style;

        return this;
    }

    /// <summary>
    /// Sets the visibility of the popover content.
    /// </summary>
    /// <param name="showContent">A value indicating whether the popover is visible.</param>
    /// <returns>The updated <see cref="MudPopoverHolder"/> instance.</returns>
    public MudPopoverHolder SetShowContent(bool showContent)
    {
        ShowContent = showContent;
        if (showContent)
        {
            ActivationDate = DateTime.Now;
        }
        else
        {
            ActivationDate = null;
        }

        return this;
    }

    /// <summary>
    /// Sets the user-defined data object attached to the component.
    /// </summary>
    /// <param name="tag">The user-defined data object.</param>
    /// <returns>The updated <see cref="MudPopoverHolder"/> instance.</returns>
    public MudPopoverHolder SetTag(object? tag)
    {
        Tag = tag;

        return this;
    }

    /// <summary>
    /// Sets the user-defined attributes added to the component.
    /// </summary>
    /// <param name="userAttributes">The user-defined attributes.</param>
    /// <returns>The updated <see cref="MudPopoverHolder"/> instance.</returns>
    public MudPopoverHolder SetUserAttributes(Dictionary<string, object?> userAttributes)
    {
        UserAttributes = userAttributes;

        return this;
    }

    /// <summary>
    /// Sets the content of the popover.
    /// </summary>
    /// <param name="renderFragment">The new content of the popover.</param>
    /// <returns>The updated <see cref="MudPopoverHolder"/> instance.</returns>
    public MudPopoverHolder SetFragment(RenderFragment? renderFragment)
    {
        Fragment = renderFragment;

        return this;
    }
}
