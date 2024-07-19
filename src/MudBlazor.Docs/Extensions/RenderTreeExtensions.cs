// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MudBlazor.Docs.Extensions;

/// <summary>
/// Extension methods added to <see cref="RenderTreeBuilder"/> for custom components.
/// </summary>
public static class RenderTreeExtensions
{
    /// <summary>
    /// Adds a MudText to the render tree.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="sequence"></param>
    /// <param name="typo"></param>
    /// <param name="text"></param>
    public static void AddMudText(this RenderTreeBuilder builder, int sequence, Typo typo = Typo.body1, string text = null)
    {
        if (!string.IsNullOrEmpty(text))
        {
            builder.OpenRegion(sequence);
            builder.OpenComponent<MudText>(0);
            builder.AddComponentParameter(1, "Typo", typo);
            builder.AddComponentParameter(2, "ChildContent", (RenderFragment)(textContentBuilder =>
            {
                textContentBuilder.AddContent(3, text);
            }));
            builder.CloseComponent();
            builder.CloseRegion();
        }
    }

    /// <summary>
    /// Adds a MudTooltip to the render tree.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="sequence"></param>
    /// <param name="placement"></param>
    /// <param name="text"></param>
    /// <param name="childContentBuilder"></param>
    public static void AddMudTooltip(this RenderTreeBuilder builder, int sequence, Placement placement = Placement.Top, string text = "", Action<int, RenderTreeBuilder> childContentBuilder = null)
    {
        // <MudTooltip Placement="Placement.Top" Text="{summary}">
        builder.OpenRegion(sequence);
        builder.OpenComponent<MudTooltip>(0);
        builder.AddComponentParameter(1, "Text", text);
        builder.AddComponentParameter(2, "Placement", placement);
        builder.AddComponentParameter(3, "ChildContent", (RenderFragment)(contentBuilder =>
        {
            childContentBuilder(sequence, contentBuilder);
        }));
        builder.CloseComponent();
        builder.CloseRegion();
    }

    /// <summary>
    /// Adds a MudIcon component to the render tree.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="sequence"></param>
    /// <param name="iconTypeName"></param>
    /// <param name="color"></param>
    /// <param name="size"></param>
    public static void AddMudIconWithTooltip(this RenderTreeBuilder builder, int sequence, string iconTypeName, Color color = Color.Default, Size size = Size.Small)
    {
        AddMudTooltip(builder, sequence, Placement.Top, iconTypeName, (childSequence, childContentBuilder) =>
        {
            AddMudIcon(childContentBuilder, childSequence, iconTypeName, color, size);
        });
    }

    /// <summary>
    /// Adds a MudIcon component to the render tree.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="sequence"></param>
    /// <param name="iconTypeName"></param>
    /// <param name="color"></param>
    /// <param name="size"></param>
    public static void AddMudIcon(this RenderTreeBuilder builder, int sequence, string iconTypeName, Color color = Color.Default, Size size = Size.Small)
    {
        // Use Reflection to get the SVG for the icon
        var parts = iconTypeName.Split('.');
        var icon = parts[parts.Length - 1];
        var svg = typeof(Icons).GetNestedType(parts[2])?.GetNestedType(parts[3])?.GetField(icon)?.GetValue(null);
        // And pass into a <MudIcon>
        builder.OpenComponent<MudIcon>(sequence++);
        builder.AddComponentParameter(sequence++, "Color", color);
        builder.AddComponentParameter(sequence++, "Size", size);
        builder.AddComponentParameter(sequence++, "Icon", svg);
        builder.AddComponentParameter(sequence++, "Style", "position:relative;top:7px;"); // Vertically center the icon
        builder.CloseComponent();
    }

    /// <summary>
    /// Adds a MudLink component.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="sequence"></param>
    /// <param name="href"></param>
    /// <param name="text"></param>
    /// <param name="cssClass"></param>
    /// <param name="target"></param>
    public static void AddMudLink(this RenderTreeBuilder builder, int sequence, string href, string text = null, string cssClass = null, string target = null)
    {
        builder.OpenRegion(sequence);
        builder.OpenComponent<MudLink>(0);
        builder.AddComponentParameter(1, "Href", href);
        if (!string.IsNullOrEmpty(cssClass))
        {
            builder.AddComponentParameter(2, "Class", cssClass);
        }
        if (!string.IsNullOrEmpty(target))
        {
            builder.AddComponentParameter(3, "Target", target);
        }
        builder.AddComponentParameter(4, "ChildContent", (RenderFragment)(linkContentBuilder =>
        {
            linkContentBuilder.AddContent(5, text);
        }));
        builder.CloseComponent();
        builder.CloseRegion();
    }

    /// <summary>
    /// Adds a MudLink component.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="sequence"></param>
    /// <param name="href"></param>
    /// <param name="text"></param>
    /// <param name="cssClass"></param>
    /// <param name="target"></param>
    public static void AddExternalMudLink(this RenderTreeBuilder builder, int sequence, string href, string text = null, string cssClass = null)
    {
        builder.OpenRegion(sequence);
        builder.OpenComponent<MudLink>(0);
        builder.AddComponentParameter(1, "Href", href);
        if (!string.IsNullOrEmpty(cssClass))
        {
            builder.AddComponentParameter(2, "Class", cssClass);
        }
        builder.AddComponentParameter(3, "Target", "_external");
        builder.AddComponentParameter(4, "ChildContent", (RenderFragment)(linkContentBuilder =>
        {
            linkContentBuilder.AddContent(5, text);
        }));
        builder.CloseComponent();
        //builder.AddMudIcon(6, "MudBlazor.Icons.Material.Filled.ArrowOutward", Color.Primary, Size.Small);
        builder.CloseRegion();
    }

    /// <summary>
    /// Adds a short section of code.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="sequence"></param>
    /// <param name="code"></param>
    public static void AddCode(this RenderTreeBuilder builder, int sequence, string code)
    {
        builder.OpenRegion(sequence);
        builder.OpenElement(0, "code");
        builder.AddAttribute(1, "class", "docs-code docs-code-primary");
        builder.AddContent(2, code);
        builder.CloseElement();
        builder.CloseRegion();
    }
}
