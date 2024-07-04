// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using LoxSmoke.DocXml;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services.XmlDocs;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// A link to a property, method, event, or field.
/// </summary>
public class ApiMemberLink : ComponentBase
{
    /// <summary>
    /// The service for XML documentation.
    /// </summary>
    [Inject]
    public IXmlDocsService? Docs { get; set; }

    /// <summary>
    /// The name of the member to link.
    /// </summary>
    [Parameter]
    public string? MemberName { get; set; }

    /// <summary>
    /// The member to link.
    /// </summary>
    public MemberInfo? Member { get; set; }

    /// <summary>
    /// The XML documentation for the member.
    /// </summary>
    public CommonComments? MemberComments { get; set; }

    /// <summary>
    /// The type the member belongs to.
    /// </summary>
    public Type? Type { get; set; }

    /// <summary>
    /// Shows a tooltip with the type's summary.
    /// </summary>
    [Parameter]
    public bool ShowTooltip { get; set; } = true;

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(MemberName) && (Member == null || Member.Name != MemberName))
        {
            Member = Docs?.GetMember(MemberName);
            MemberComments = Member == null ? null : Docs?.GetMemberComments(Member);
            Type = Member?.DeclaringType;
        }
    }

    protected override bool ShouldRender() => !string.IsNullOrEmpty(MemberName) || Member != null;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Is there a linkable member?  (i.e.  /api/Type#Member)
        if (Type != null && Member != null)
        {
            // Can we show a tooltip with the summary?
            if (MemberComments != null && !string.IsNullOrEmpty(MemberComments.Summary))
            {
                builder.AddMudTooltip(0, Placement.Top, MemberComments.Summary, (childSequence, childBuilder) =>
                {
                    builder.AddMudLink(childSequence, $"api/{Type.Name}#{Member?.Name}", $"{Type.Name}.{Member?.Name}", "docs-link docs-code docs-code-primary");
                });
            }
            else
            {
                builder.AddMudLink(0, $"api/{Type.Name}#{Member?.Name}", $"{Type.Name}.{Member?.Name}", "docs-link docs-code docs-code-primary");
            }
        }
        // Is this an internal type?
        else if (MemberName != null && (MemberName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) || MemberName.StartsWith("System", StringComparison.OrdinalIgnoreCase)))
        {
            Debugger.Break();
            //// Is this a linkable type?
            //if (!MemberName?.Contains("[["))
            //{
            //    builder.AddMudLink(0, $"https://learn.microsoft.com/en-us/dotnet/api/{MemberName}", MemberName, "docs-link docs-code docs-code-primary", "_external");
            //}
            //else
            //{
            //    builder.AddCode(0, TypeFriendlyName);
            //}
        }
        // Is there some text to link?
        //else if (!string.IsNullOrEmpty(TypeFriendlyName))
        //{
        //    builder.AddCode(0, TypeFriendlyName);
        //}
        // Is there some text to link?
        else
        {
            builder.AddCode(0, MemberName);
        }
    }
}
