// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Models;

#nullable enable

/// <summary>
/// Represents the XML documentation for the members of documented types.
/// </summary>
/// <remarks>
/// <para>
/// Member documentation is about ten times the size of the type documentation, and an API page shows
/// one type's members - 37 rows for <c>MudButton</c>, 154 for <c>MudDataGrid</c>. So it is built one
/// type at a time, the first time something asks for that type's members. The pages that make up most
/// of the site never ask at all.
/// </para>
/// <para>
/// A member's key begins with the key of the type that declares it, so the owning type can be derived
/// from the member name and no separate index is needed.
/// </para>
/// </remarks>
public static partial class ApiDocumentationMembers
{
    /// <summary>
    /// Every member built so far, by key.
    /// </summary>
    /// <remarks>
    /// A member is created by the loader of the type that declares it, so a type which inherits members
    /// shares the instances declared by its base types.
    /// </remarks>
    internal static Dictionary<string, DocumentedProperty> PropertiesInternal { get; } = [];

    /// <inheritdoc cref="PropertiesInternal"/>
    internal static Dictionary<string, DocumentedMethod> MethodsInternal { get; } = [];

    /// <inheritdoc cref="PropertiesInternal"/>
    internal static Dictionary<string, DocumentedField> FieldsInternal { get; } = [];

    /// <inheritdoc cref="PropertiesInternal"/>
    internal static Dictionary<string, DocumentedEvent> EventsInternal { get; } = [];

    private static readonly HashSet<string> LoadedTypes = [];

    /// <summary>
    /// Guards building, and the reads that trigger it.
    /// </summary>
    /// <remarks>
    /// The WASM client is single threaded, but the prerender host renders crawler requests
    /// concurrently.  Every read of a member collection comes through here, so a reader cannot see a
    /// type that another thread is still building.  The lock is reentrant, which is what lets a type
    /// build the types it inherits from.
    /// </remarks>
    private static readonly Lock Gate = new();

    private static bool _allLoaded;

    /// <summary>
    /// Builds the members of a type, and of the types it inherits from.
    /// </summary>
    /// <param name="typeKey">The key of the type to build, or <c>null</c> to do nothing.</param>
    public static void EnsureType(string? typeKey)
    {
        if (typeKey is null)
        {
            return;
        }

        lock (Gate)
        {
            // Marked before loading, so a type reached again while it is loading is not built twice.
            if (!LoadedTypes.Add(typeKey))
            {
                return;
            }

            LoadType(typeKey);
        }
    }

    /// <summary>
    /// Builds the members of every documented type.
    /// </summary>
    /// <remarks>
    /// Only needed by the few places that look across all members rather than at one type - the global
    /// settings page, and the by-name fallback in <see cref="ApiDocumentation"/>.
    /// </remarks>
    public static void EnsureAll()
    {
        lock (Gate)
        {
            if (_allLoaded)
            {
                return;
            }

            _allLoaded = true;

            foreach (var typeKey in ApiDocumentation.Types.Keys)
            {
                EnsureType(typeKey);
            }
        }
    }

    /// <summary>
    /// Gets the key of the type which declares a member.
    /// </summary>
    /// <param name="memberKey">The key of the member, such as <c>MudBlazor.MudAlert.Class</c>.</param>
    /// <returns>The part before the final separator, or <c>null</c> when there is none.</returns>
    internal static string? DeclaringTypeKey(string? memberKey)
    {
        var separator = memberKey?.LastIndexOf('.') ?? -1;

        return separator <= 0 ? null : memberKey![..separator];
    }

    /// <summary>
    /// Gets a property, building the type that declares it if needed.
    /// </summary>
    internal static DocumentedProperty? Property(string key)
    {
        EnsureType(DeclaringTypeKey(key));

        return PropertiesInternal.GetValueOrDefault(key);
    }

    /// <inheritdoc cref="Property"/>
    internal static DocumentedMethod? Method(string key)
    {
        EnsureType(DeclaringTypeKey(key));

        return MethodsInternal.GetValueOrDefault(key);
    }

    /// <inheritdoc cref="Property"/>
    internal static DocumentedField? Field(string key)
    {
        EnsureType(DeclaringTypeKey(key));

        return FieldsInternal.GetValueOrDefault(key);
    }

    /// <inheritdoc cref="Property"/>
    internal static DocumentedEvent? Event(string key)
    {
        EnsureType(DeclaringTypeKey(key));

        return EventsInternal.GetValueOrDefault(key);
    }

    /// <summary>
    /// Builds the members declared by one type.
    /// </summary>
    /// <param name="typeKey">The key of the type to build.</param>
    static partial void LoadType(string typeKey);
}
