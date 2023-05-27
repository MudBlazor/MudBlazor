// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
internal class MudPopoverState : IMudPopoverState
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
    public Dictionary<string, object> UserAttributes { get; set; } = new();

    /// <inheritdoc />
    public MudRender? ElementReference { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MudPopoverState"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the popover.</param>
    /// <param name="fragment">The content of the popover.</param>
    public MudPopoverState(Guid id, RenderFragment? fragment)
    {
        Id = id;
        Fragment = fragment;
    }

    /// <summary>
    /// Sets the component base parameters.
    /// </summary>
    /// <param name="class">The CSS class of the popover.</param>
    /// <param name="style">The inline styles of the popover.</param>
    /// <param name="showContent">A value indicating whether the popover is visible.</param>
    /// <param name="tag">The user-defined data object attached to the component.</param>
    /// <param name="userAttributes">The user-defined attributes added to the component.</param>
    /// <returns>The updated <see cref="MudPopoverState"/> instance.</returns>
    public MudPopoverState SetComponentBaseParameters(string @class, string style, bool showContent, object? tag, Dictionary<string, object> userAttributes)
    {
        Class = @class;
        Style = style;
        ShowContent = showContent;
        Tag = tag;
        UserAttributes = userAttributes;
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
    /// Sets the content of the popover.
    /// </summary>
    /// <param name="renderFragment">The new content of the popover.</param>
    /// <returns>The updated <see cref="MudPopoverState"/> instance.</returns>
    public MudPopoverState SetFragment(RenderFragment? renderFragment)
    {
        Fragment = renderFragment;

        return this;
    }
}
