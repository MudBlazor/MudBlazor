// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using LoxSmoke.DocXml;

namespace MudBlazor.Docs.Services.XmlDocs;

#nullable enable

/// <summary>
/// Implements XML documentation features for a service.
/// </summary>
public interface IXmlDocsService
{
    /// <summary>
    /// Loads XML documentation for use by documentation pages.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Gets the comments for a type's member.
    /// </summary>
    /// <param name="memberInfo">The member to find.</param>
    /// <returns>The set of comments for the member, or <c>null</c>.</returns>
    Task<CommonComments> GetMemberCommentsAsync(MemberInfo? memberInfo);

    /// <summary>
    /// Gets a type by its name.
    /// </summary>
    /// <param name="typeName">The type to find.</param>
    /// <returns>The type or <c>null</c> if none was found.</returns>
    Task<Type?> GetTypeAsync(string typeName);

    /// <summary>
    /// Gets a member by its name.
    /// </summary>
    /// <param name="memberName">The member to find.</param>
    /// <returns>The member or <c>null</c> if none was found.</returns>
    Task<MemberInfo?> GetMemberAsync(string memberName);

    /// <summary>
    /// Gets all public MudBlazor types.
    /// </summary>
    /// <returns>All public MudBlazor types.</returns>
    Task<List<Type>> GetTypesAsync();

    /// <summary>
    /// Gets the XML documentation for a type.
    /// </summary>
    /// <param name="typeName">The type to find.</param>
    /// <returns>The comments for the type.</returns>
    Task<TypeComments?> GetTypeCommentsAsync(string typeName);

    /// <summary>
    /// Gets the XML documentation for a type.
    /// </summary>
    /// <param name="typeName">The type to find.</param>
    /// <returns>The comments for the type.</returns>
    Task<TypeComments?> GetTypeCommentsAsync(Type type);

    /// <summary>
    /// Gets the XML documentation for a type's properties.
    /// </summary>
    /// <param name="type">The type to find properties for.</param>
    /// <returns>The comments for the type's properties.</returns>
    Task<List<CommonComments>> GetPropertyCommentsAsync(Type type);

    /// <summary>
    /// Gets the public properties for the type.
    /// </summary>
    /// <param name="type">The type to search.</param>
    /// <returns>Public properties declared in MudBlazor types, except for <c>EventCallback</c> properties (which are considered events).</returns>
    Task<List<PropertyInfo>> GetPropertiesAsync(Type type);

    /// <summary>
    /// Gets the public methods for the type.
    /// </summary>
    /// <param name="type">The type to search.</param>
    /// <returns>Public methods declared in MudBlazor types.</returns>
    Task<List<MethodInfo>> GetMethodsAsync(Type type);

    /// <summary>
    /// Gets the public fields for the type.
    /// </summary>
    /// <param name="type">The type to search.</param>
    /// <returns>Public fields declared in MudBlazor types.</returns>
    Task<List<FieldInfo>> GetFieldsAsync(Type type);

    /// <summary>
    /// Gets the public events (and EventCallback properties) for the type.
    /// </summary>
    /// <param name="type">The type to search.</param>
    /// <returns>Public events and <c>EventCallback</c> properties declared in MudBlazor types.</returns>
    Task<List<MemberInfo>> GetEventsAsync(Type type);
}
