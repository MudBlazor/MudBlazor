// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LoxSmoke.DocXml;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Services.XmlDocs;

#nullable enable

/// <summary>
/// A service for looking up XML documentation for types and members.
/// </summary>
public sealed class XmlDocsService : IXmlDocsService
{
    private readonly Assembly mudBlazorAssembly;
    private readonly string xmlDocumentationPath;
    private readonly DocXmlReader reader;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public XmlDocsService()
    {
        mudBlazorAssembly = typeof(MudBlazor._Imports).Assembly;
        xmlDocumentationPath = mudBlazorAssembly.Location.Replace(".dll", ".xml", StringComparison.OrdinalIgnoreCase);
        reader = new(xmlDocumentationPath);
    }

    /// <inheritdoc />
    public Type? GetType(string typeName)
    {
        var type = mudBlazorAssembly.GetType(typeName)
            ?? mudBlazorAssembly.GetType("MudBlazor." + typeName)
            ?? typeof(string).Assembly.GetType(typeName)
            ?? typeof(RenderFragment).Assembly.GetType(typeName);
        return type;
    }

    /// <inheritdoc />
    public MemberInfo? GetMember(string memberName)
    {
        // Assume the last part of the name is a member
        var parameterIndex = memberName.IndexOf('(');
        if (parameterIndex >= 0)
        {
            memberName = memberName.Replace(memberName.Substring(parameterIndex), "");
        }
        var typePortion = memberName.Substring(0, memberName.LastIndexOf('.'));
        var memberPortion = memberName.Substring(memberName.LastIndexOf('.') + 1);

        var type = GetType(typePortion);
        var member = type?.GetMember(memberPortion);
        return member?.FirstOrDefault();
    }

    /// <inheritdoc />
    public IEnumerable<PropertyInfo> GetProperties(Type type)
    {
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            // Exclude event callbacks
            if (property.PropertyType.Name == "EventCallback`1")
            {
                continue;
            }
            if (string.IsNullOrEmpty(property.DeclaringType!.FullName))
            {
                // MudBlazor type but no FullName (?)
            }
            // Exclude non-MudBlazor properties
            else if (!property.DeclaringType!.FullName!.StartsWith("MudBlazor."))
            {
                continue;
            }
            yield return property;
        }
    }

    /// <inheritdoc />
    public IEnumerable<FieldInfo> GetFields(Type type)
    {
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            // Exclude event callbacks
            if (field.FieldType.Name == "EventCallback`1" || field.Name == "value__")
            {
                continue;
            }
            if (string.IsNullOrEmpty(field.DeclaringType!.FullName))
            {
                // MudBlazor type but no FullName (?)
            }
            // Exclude non-MudBlazor fields
            else if (!field.DeclaringType!.FullName!.StartsWith("MudBlazor."))
            {
                continue;
            }
            yield return field;
        }
    }

    /// <inheritdoc />
    public IEnumerable<MethodInfo> GetMethods(Type type)
    {
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
            {
                continue;
            }
            if (string.IsNullOrEmpty(method.DeclaringType!.FullName))
            {
                // MudBlazor type but no FullName (?)
            }
            // Exclude non-MudBlazor methods
            else if (!method.DeclaringType!.FullName!.StartsWith("MudBlazor."))
            {
                continue;
            }
            yield return method;
        }
    }

    /// <inheritdoc />
    public IEnumerable<MemberInfo> GetEvents(Type type)
    {
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            // Include event callbacks
            if (field.FieldType.Name == "EventCallback`1")
            {
                yield return field;
            }
        }
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            // Include event callbacks
            if (property.PropertyType.Name == "EventCallback`1")
            {
                yield return property;
            }
        }
        foreach (var eventItem in type.GetEvents())
        {
            if (string.IsNullOrEmpty(eventItem.DeclaringType!.FullName))
            {
                // MudBlazor type but no FullName (?)
            }
            // Exclude non-MudBlazor properties
            else if (!eventItem.DeclaringType!.FullName!.StartsWith("MudBlazor."))
            {
                continue;
            }
            yield return eventItem;
        }
    }

    /// <inheritdoc />
    public IEnumerable<Type> GetTypes()
    {
        return mudBlazorAssembly.ExportedTypes;
    }

    /// <inheritdoc />
    public TypeComments? GetTypeComments(string typeName)
    {
        return GetTypeComments(GetType(typeName));
    }

    /// <inheritdoc />
    public TypeComments? GetTypeComments(Type? type)
    {
        return type == null ? null : reader.GetTypeComments(type);
    }

    /// <inheritdoc />
    public IEnumerable<CommonComments> GetPropertyComments(Type type)
    {
        foreach (var property in type.GetProperties(BindingFlags.Public))
        {
            yield return reader.GetMemberComments(property);
        }
    }

    /// <inheritdoc />
    public CommonComments GetMemberComments(MemberInfo memberInfo) => reader.GetMemberComments(memberInfo);
}
