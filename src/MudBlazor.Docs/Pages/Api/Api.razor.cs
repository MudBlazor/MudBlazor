// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using LoxSmoke.DocXml;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services.XmlDocs;

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

    /// <summary>
    /// Whether this page is loading data.
    /// </summary>
    public bool IsLoading { get; set; }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        // Do we have a name to look up?  And is there a need to find the type?
        if (!IsLoading && !string.IsNullOrEmpty(TypeName) && (Type == null || Type.Name != TypeName))
        {
            try
            {
                // Need a moment
                IsLoading = true;
                StateHasChanged();

                // Yes.  Get the type
                Type = await Docs!.GetTypeAsync(TypeName);
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
                    // Is this a component?  An enum?  A class?
                    Title = Type.GetFriendlyName();
                    if (Type.IsSubclassOf(typeof(MudComponentBase)))
                    {
                        Title += " Component";
                    }
                    else if (Type.IsEnum)
                    {
                        Title += " Enumeration";
                    }
                    else if (Type.IsClass)
                    {
                        Title += " Class";
                    }
                    // Load the type's comments
                    TypeComments = await Docs!.GetTypeCommentsAsync(Type);
                    // Get types inheriting from this one
                    DerivedTypes = Type.Assembly.GetTypes().Where(type => type.IsSubclassOf(Type)).ToList();
                    Properties = new(await Docs!.GetPropertiesAsync(Type));
                    Fields = new(await Docs!.GetFieldsAsync(Type));
                    Methods = new(await Docs!.GetMethodsAsync(Type));
                    Events = new(await Docs!.GetEventsAsync(Type));
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
    }
}
