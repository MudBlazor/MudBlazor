// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using LoxSmoke.DocXml;
using MudBlazor.Docs.Services.XmlDocs;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor.Docs.Pages.Api;

#nullable enable

/// <summary>
/// Represents a page for viewing the documentation for a type.
/// </summary>
public partial class Api
{
    /// <summary>
    /// The service for XML documentation.
    /// </summary>
    [Inject]
    public IXmlDocsService? Docs { get; set; }

    /// <summary>
    /// The name of the type to display.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string? TypeName { get; set; }

    /// <summary>
    /// The name at the top of the page.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The type being displayed.
    /// </summary>
    public Type? Type { get; set; }

    /// <summary>
    /// The properties for the type.
    /// </summary>
    public List<MemberInfo> Properties { get; set; } = [];

    /// <summary>
    /// The fields for the type.
    /// </summary>
    public List<MemberInfo> Fields { get; set; } = [];

    /// <summary>
    /// The methods for the type.
    /// </summary>
    public List<MemberInfo> Methods { get; set; } = [];

    /// <summary>
    /// The events and callbacks for the type.
    /// </summary>
    public List<MemberInfo> Events { get; set; } = [];

    /// <summary>
    /// The types inheriting from this type.
    /// </summary>
    public List<Type> DerivedTypes { get; set; } = [];

    /// <summary>
    /// The documentation for this type.
    /// </summary>
    public TypeComments? TypeComments { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // Do we have a name to look up?  And is there a need to find the type?
        if (!string.IsNullOrEmpty(TypeName) && (Type == null || Type.Name != TypeName))
        {
            // Yes.  Get the type
            Type = Docs!.GetType(TypeName);
            // Was a type found?
            if (Type == null)
            {
                Title = $"{TypeName} Not Found";
                TypeComments = null;
                DerivedTypes = [];
                Properties = [];
                Fields = [];
                Methods = [];
                Events = [];
            }
            else
            {
                // Is this a component?
                Title = Type.IsSubclassOf(typeof(MudComponentBase)) ? $"{Type.Name} Component" : $"{Type.Name} Class";
                // Load the type's comments
                TypeComments = Docs!.GetTypeComments(Type);
                // Get types inheriting from this one
                DerivedTypes = Type.Assembly.GetTypes().Where(type => type.IsSubclassOf(Type)).ToList();
                Properties = new(Docs!.GetProperties(Type));
                Fields = new(Docs!.GetFields(Type));
                Methods = new(Docs!.GetMethods(Type));
                Events = new(Docs!.GetEvents(Type));
            }
            StateHasChanged();
        }
    }
}
