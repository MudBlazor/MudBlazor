// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Extensions;

public static class RenderTreeExtensions
{
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

    public static void AddMudLink(this RenderTreeBuilder builder, int sequence, string href, string text = null, string cssClass = null)
    {
        builder.OpenRegion(sequence);
        builder.OpenComponent<MudLink>(0);
        builder.AddComponentParameter(1, "Href", href);
        builder.AddComponentParameter(2, "Class", cssClass);
        builder.AddComponentParameter(3, "ChildContent", (RenderFragment)(linkContentBuilder =>
        {
            linkContentBuilder.AddContent(4, text ?? href);
        }));
        builder.CloseComponent();
        builder.CloseRegion();
    }

    public static void AddCode(this RenderTreeBuilder builder, int sequence, string code)
    {
        builder.OpenRegion(sequence);
        builder.OpenElement(0, "code");
        builder.AddAttribute(1, "class", "docs-code docs-code-primary");
        builder.AddContent(2, code);
        builder.CloseElement();
        builder.CloseRegion();
    }

    public static void AddDocumentedTypeLink(this RenderTreeBuilder builder, int sequence, DocumentedType type)
    {
        // Is a summary available?
        if (!string.IsNullOrEmpty(type.Summary))
        {
            // <MudTooltip Placement="Placement.Top" Text="{summary}">
            builder.AddMudTooltip(sequence, Placement.Top, type.SummaryPlain, ((childSequence, childContentBuilder) =>
            {
                childContentBuilder.AddMudLink(childSequence++, type.ApiUrl, type.NameFriendly, "docs-link docs-code docs-code-primary");
            }));
        }
        else
        {
            // <MudLink Href="{api_link}" Class="docs-link">
            builder.AddMudLink(sequence, type.ApiUrl, type.NameFriendly, "docs-link docs-code docs-code-primary");
        }
    }

    public static void AddDocumentedMemberLink(this RenderTreeBuilder builder, int sequence, DocumentedMember member)
    {
        // Is a summary available?
        if (!string.IsNullOrEmpty(member.Summary))
        {
            // <MudTooltip Placement="Placement.Top" Text="{summary}">
            builder.AddMudTooltip(sequence, Placement.Top, member.SummaryPlain, ((childSequence, childContentBuilder) =>
            {
                childContentBuilder.AddMudLink(childSequence++, member.DeclaringType?.ApiUrl, member.Name, "docs-link docs-code docs-code-primary");
            }));
        }
        else
        {
            // <MudLink Href="{api_link}" Class="docs-link">
            builder.AddMudLink(sequence, member.DeclaringType?.ApiUrl, member.Name, "docs-link docs-code docs-code-primary");
        }
    }
}
