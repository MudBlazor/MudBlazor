// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Docs.Models;

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
    public static void AddMudText(this RenderTreeBuilder builder, int sequence, Typo typo = Typo.body1, Color color = Color.Inherit, string text = null)
    {
        if (!string.IsNullOrEmpty(text))
        {
            builder.OpenRegion(sequence);
            builder.OpenComponent<MudText>(0);
            if (typo != Typo.body1) // Only render Typo if not the MudText default
            {
                builder.AddComponentParameter(1, "Typo", typo);
            }
            if (color != Color.Inherit) // Only render Color if not the MudText default
            {
                builder.AddComponentParameter(2, "Color", color);
            }
            builder.AddComponentParameter(3, "HtmlTag", "span");
            builder.AddComponentParameter(4, "ChildContent", (RenderFragment)(textContentBuilder =>
            {
                textContentBuilder.AddContent(0, text);
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
        // Limit the tooltip to 60 characters
        var truncatedText = text.Length > 60 ? string.Concat(text.AsSpan(0, 60), "...") : text;

        // <MudTooltip Placement="Placement.Top" Text="{summary}">
        builder.OpenRegion(sequence);
        builder.OpenComponent<MudTooltip>(0);
        builder.AddComponentParameter(1, "Text", truncatedText);
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
    public static void AddMudIcon(this RenderTreeBuilder builder, int sequence, string iconTypeName, Color color = Color.Default, Size size = Size.Small)
    {
        // Use Reflection to get the SVG for the icon
        var parts = iconTypeName.Split('.');
        var icon = parts[parts.Length - 1];
        var svg = typeof(Icons).GetNestedType(parts[2])?.GetNestedType(parts[3])?.GetField(icon)?.GetValue(null);
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
    /// <param name="color"></param>
    public static void AddMudLink(this RenderTreeBuilder builder, int sequence, string href, string text = null, Typo typo = Typo.body1, string cssClass = null, string target = null, Action<int, RenderTreeBuilder> childContentBuilder = null)
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
        builder.AddComponentParameter(4, "Typo", typo);
        builder.AddComponentParameter(3, "ChildContent", (RenderFragment)(contentBuilder =>
        {
            if (childContentBuilder == null)
            {
                contentBuilder.AddContent(6, text);
            }
            else
            {
                childContentBuilder(sequence, contentBuilder);
            }
        }));
        builder.CloseComponent();
        builder.CloseRegion();
    }

    /// <summary>
    /// Adds a code block.
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

    /// <summary>
    /// Adds a link to APi documentation to the render tree.
    /// </summary>
    /// <param name="builder">The render tree builder.</param>
    /// <param name="sequence">The ordinal of this item relative to the other components.</param>
    /// <param name="type">The type to link.</param>
    /// <param name="showTooltip">When <c>true</c>, a tooltip will display with the type's summary.</param>
    public static void AddDocumentedTypeLink(this RenderTreeBuilder builder, int sequence, DocumentedType type, Typo typo = Typo.body1, bool showTooltip = true)
    {
        // Is a summary available?
        if (!string.IsNullOrEmpty(type.Summary) && showTooltip)
        {
            // <MudTooltip Placement="Placement.Top" Text="{summary}">
            builder.AddMudTooltip(sequence, Placement.Top, type.SummaryPlain, (childSequence, childContentBuilder) =>
            {
                childContentBuilder.AddMudLink(childSequence++, type.ApiUrl, type.NameFriendly, typo, "docs-link docs-code docs-code-primary");
            });
        }
        else
        {
            // <MudLink Href="{api_link}" Class="docs-link">
            builder.AddMudLink(sequence, type.ApiUrl, type.NameFriendly, typo, "docs-link docs-code docs-code-primary");
        }
    }

    public static void AddDocumentedMemberLink(this RenderTreeBuilder builder, int sequence, DocumentedMember member, Typo typo = Typo.body1)
    {
        // Is a summary available?
        if (!string.IsNullOrEmpty(member.Summary))
        {
            // <MudTooltip Placement="Placement.Top" Text="{summary}">
            builder.AddMudTooltip(sequence, Placement.Top, member.SummaryPlain, (childSequence, childContentBuilder) =>
            {
                childContentBuilder.AddMudLink(childSequence++, member.DeclaringType?.ApiUrl + "#" + member.Name, member.Name, typo, "docs-link docs-code docs-code-primary");
            });
        }
        else
        {
            // <MudLink Href="{api_link}" Class="docs-link">
            builder.AddMudLink(sequence, member.DeclaringType?.ApiUrl, member.Name, typo, "docs-link docs-code docs-code-primary");
        }
    }
}
